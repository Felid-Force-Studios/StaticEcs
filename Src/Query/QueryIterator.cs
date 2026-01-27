#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    /// <summary>
    /// Flexible (re-checking) entity iterator returned by
    /// <see cref="World{TWorld}.WorldQuery{TFilter}.EntitiesFlexible"/>.
    /// <para>
    /// Unlike <see cref="QueryStrictIterator{TWorld,TFilter}"/>, this iterator re-evaluates
    /// the entity bitmask on every block transition, allowing safe iteration even when the
    /// filtered component or tag types are modified on <b>other</b> entities during the loop.
    /// This makes it slightly slower than the strict variant but tolerant of concurrent mutations.
    /// </para>
    /// <para>
    /// This is a <c>ref struct</c> — it cannot be boxed, stored in fields, or used across
    /// <c>await</c> boundaries. Use it in a <c>foreach</c> loop.
    /// </para>
    /// </summary>
    /// <typeparam name="TWorld">World type identifying the static ECS world to iterate.</typeparam>
    /// <typeparam name="TFilter">Query filter constraining which entities are visited.</typeparam>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public ref struct QueryFlexibleIterator<TWorld, TFilter>
        where TFilter : struct, IQueryFilter
        where TWorld : struct, IWorldType {

        private readonly QueryData _queryData;
        private ulong _availableMask;
        private World<TWorld>.Entity _current;
        private int _firstGlobalBlockIdx;
        private World<TWorld>.WorldQuery<TFilter> _query;
        private readonly EntityStatusType _entities;

        /// <summary>Creates a flexible iterator over entities in the specified clusters.</summary>
        /// <param name="clusters">Cluster IDs to iterate. Pass <c>default</c> for all active clusters.</param>
        /// <param name="filter">Query filter instance.</param>
        /// <param name="entities">Which entity status to include (enabled, disabled, or both).</param>
        [MethodImpl(AggressiveInlining)]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public QueryFlexibleIterator(ReadOnlySpan<ushort> clusters, TFilter filter, EntityStatusType entities) {
            #if FFS_ECS_DEBUG
            World<TWorld>.AssertNotNestedParallelQuery(World<TWorld>.WorldTypeName);
            filter.Assert<TWorld>();
            #endif

            _entities = entities;
            _current = default;
            _query = World<TWorld>.Query(filter);
            _query.Prepare(_query.Filter, clusters, entities, false, out _queryData, out _firstGlobalBlockIdx);
            _availableMask = _firstGlobalBlockIdx >= 0 ? ulong.MaxValue : 0;
        }

        [MethodImpl(AggressiveInlining)]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        internal QueryFlexibleIterator(ReadOnlySpan<uint> chunks, TFilter filter, EntityStatusType entities) {
            #if FFS_ECS_DEBUG
            World<TWorld>.AssertNotNestedParallelQuery(World<TWorld>.WorldTypeName);
            filter.Assert<TWorld>();
            #endif

            _entities = entities;
            _current = default;
            _query = World<TWorld>.Query(filter);
            _query.Prepare(_query.Filter, chunks, entities, false, out _queryData, out _firstGlobalBlockIdx);
            _availableMask = _firstGlobalBlockIdx >= 0 ? ulong.MaxValue : 0;
        }

        /// <summary>The entity at the current iterator position.</summary>
        public readonly World<TWorld>.Entity Current {
            [MethodImpl(AggressiveInlining)] get => _current;
        }

        [MethodImpl(AggressiveInlining)]
        public bool MoveNext() {
            if (_firstGlobalBlockIdx >= 0) {
                var mask = _queryData.Blocks[_firstGlobalBlockIdx].EntitiesMask & _availableMask;
                if (mask != 0) {
                    var isolatedBit = mask & (ulong)-(long)mask;
                    _availableMask = ~(isolatedBit | (isolatedBit - 1));
                    #if NET6_0_OR_GREATER
                    var bitIdx = (uint)System.Numerics.BitOperations.TrailingZeroCount(mask);
                    #else
                    var bitIdx = (uint)Utils.DeBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                    #endif
                    _current.IdWithOffset = (uint)(_firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT) + Const.ENTITY_ID_OFFSET + bitIdx;
                    #if FFS_ECS_DEBUG
                    World<TWorld>.Data.Instance.SetCurrentQueryEntity(_current.IdWithOffset);
                    #endif
                    return true;
                }
                return Advance();
            }
            return false;
        }

        [MethodImpl(NoInlining)]
        private bool Advance() {
            while (true) {
                _firstGlobalBlockIdx = _queryData.Blocks[_firstGlobalBlockIdx].NextGlobalBlock;
                if (_firstGlobalBlockIdx < 0) return false;
                _availableMask = ulong.MaxValue;
                var mask = _queryData.Blocks[_firstGlobalBlockIdx].EntitiesMask;
                if (mask != 0) {
                    var isolatedBit = mask & (ulong)-(long)mask;
                    _availableMask = ~(isolatedBit | (isolatedBit - 1));
                    #if NET6_0_OR_GREATER
                    var bitIdx = (uint)System.Numerics.BitOperations.TrailingZeroCount(mask);
                    #else
                    var bitIdx = (uint)Utils.DeBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                    #endif
                    _current.IdWithOffset = (uint)(_firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT) + Const.ENTITY_ID_OFFSET + bitIdx;
                    #if FFS_ECS_DEBUG
                    World<TWorld>.Data.Instance.SetCurrentQueryEntity(_current.IdWithOffset);
                    #endif
                    return true;
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public readonly QueryFlexibleIterator<TWorld, TFilter> GetEnumerator() => this;

        [MethodImpl(AggressiveInlining)]
        public void Dispose() {
            if (_queryData.Blocks != null) {
                _query.Dispose(_query.Filter, _entities, false, _queryData);
            }
        }
    }

    /// <summary>
    /// Strict (fast-path) entity iterator returned by
    /// <see cref="World{TWorld}.WorldQuery{TFilter}.Entities"/>.
    /// <para>
    /// The strict iterator assumes that the filtered component and tag types are <b>not</b>
    /// modified on other entities during iteration. This allows it to skip per-block bitmask
    /// re-evaluation, making it faster than <see cref="QueryFlexibleIterator{TWorld,TFilter}"/>.
    /// Additionally, it uses a sequential bit-scan optimization: when the next entity is
    /// adjacent in the bitmask, it advances with a single shift + increment instead of a
    /// full trailing-zero-count scan.
    /// </para>
    /// <para>
    /// This is a <c>ref struct</c> — it cannot be boxed, stored in fields, or used across
    /// <c>await</c> boundaries. Use it in a <c>foreach</c> loop.
    /// </para>
    /// </summary>
    /// <typeparam name="TWorld">World type identifying the static ECS world to iterate.</typeparam>
    /// <typeparam name="TFilter">Query filter constraining which entities are visited.</typeparam>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public ref struct QueryStrictIterator<TWorld, TFilter>
        where TFilter : struct, IQueryFilter
        where TWorld : struct, IWorldType {

        private readonly QueryData _queryData;
        private ulong _entitiesMask;
        private ulong _isolatedBit;
        private World<TWorld>.Entity _current;
        private int _firstGlobalBlockIdx;
        private World<TWorld>.WorldQuery<TFilter> _query;
        private readonly EntityStatusType _entities;

        /// <summary>Creates a strict iterator over entities in the specified clusters.</summary>
        /// <param name="clusters">Cluster IDs to iterate. Pass <c>default</c> for all active clusters.</param>
        /// <param name="filter">Query filter instance.</param>
        /// <param name="entities">Which entity status to include (enabled, disabled, or both).</param>
        [MethodImpl(AggressiveInlining)]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public QueryStrictIterator(ReadOnlySpan<ushort> clusters, TFilter filter, EntityStatusType entities) {
            #if FFS_ECS_DEBUG
            World<TWorld>.AssertNotNestedParallelQuery(World<TWorld>.WorldTypeName);
            filter.Assert<TWorld>();
            #endif

            _entities = entities;
            _current = default;
            _query = World<TWorld>.Query(filter);
            _query.Prepare(_query.Filter, clusters, entities, true, out _queryData, out _firstGlobalBlockIdx);

            _isolatedBit = 0;
            _entitiesMask = _firstGlobalBlockIdx >= 0 ? _queryData.Blocks[_firstGlobalBlockIdx].EntitiesMask : 0;
        }

        [MethodImpl(AggressiveInlining)]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        internal QueryStrictIterator(ReadOnlySpan<uint> chunks, TFilter filter, EntityStatusType entities) {
            #if FFS_ECS_DEBUG
            World<TWorld>.AssertNotNestedParallelQuery(World<TWorld>.WorldTypeName);
            filter.Assert<TWorld>();
            #endif

            _entities = entities;
            _current = default;
            _query = World<TWorld>.Query(filter);
            _query.Prepare(_query.Filter, chunks, entities, true, out _queryData, out _firstGlobalBlockIdx);

            _isolatedBit = 0;
            _entitiesMask = _firstGlobalBlockIdx >= 0 ? _queryData.Blocks[_firstGlobalBlockIdx].EntitiesMask : 0;
        }

        /// <summary>The entity at the current iterator position.</summary>
        public readonly World<TWorld>.Entity Current {
            [MethodImpl(AggressiveInlining)] get => _current;
        }

        [MethodImpl(AggressiveInlining)]
        public bool MoveNext() {
            var nextBit = _isolatedBit << 1;
            if ((_entitiesMask & nextBit) != 0) {
                _isolatedBit = nextBit;
                ++_current.IdWithOffset;
                #if FFS_ECS_DEBUG
                World<TWorld>.Data.Instance.SetCurrentQueryEntity(_current.IdWithOffset);
                #endif
                return true;
            }
            return Advance();
        }

        [MethodImpl(NoInlining)]
        private bool Advance() {
            if (_isolatedBit != 0) {
                _entitiesMask &= ~(_isolatedBit | (_isolatedBit - 1));
            }

            while (true) {
                if (_entitiesMask != 0) {
                    var isolatedBit = _entitiesMask & (ulong)-(long)_entitiesMask;
                    #if NET6_0_OR_GREATER
                    var bitIndex = (uint)System.Numerics.BitOperations.TrailingZeroCount(_entitiesMask);
                    #else
                    var bitIndex = (uint)Utils.DeBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                    #endif
                    _isolatedBit = isolatedBit;
                    _current.IdWithOffset = (uint)(_firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT) + Const.ENTITY_ID_OFFSET + bitIndex;
                    #if FFS_ECS_DEBUG
                    World<TWorld>.Data.Instance.SetCurrentQueryEntity(_current.IdWithOffset);
                    #endif
                    return true;
                }

                if (_firstGlobalBlockIdx < 0) return false;
                _firstGlobalBlockIdx = _queryData.Blocks[_firstGlobalBlockIdx].NextGlobalBlock;
                if (_firstGlobalBlockIdx < 0) return false;
                _entitiesMask = _queryData.Blocks[_firstGlobalBlockIdx].EntitiesMask;
            }
        }

        [MethodImpl(AggressiveInlining)]
        public readonly QueryStrictIterator<TWorld, TFilter> GetEnumerator() => this;

        [MethodImpl(AggressiveInlining)]
        public void Dispose() {
            if (_queryData.Blocks != null) {
                _query.Dispose(_query.Filter, _entities, true, _queryData);
            }
        }
    }
}