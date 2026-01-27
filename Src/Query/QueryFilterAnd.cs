#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    /// <summary>
    /// Static factory class for creating composite <see cref="IQueryFilter"/> instances that combine
    /// multiple filters with AND semantics. Each <c>By</c> overload constructs an <c>And&lt;...&gt;</c>
    /// struct that applies all provided filters sequentially — an entity must satisfy every filter to match.
    /// <para>
    /// This factory exists because C# cannot infer generic type arguments for constructors.
    /// Using <c>And.By(filter1, filter2)</c> enables full type inference, avoiding the need to
    /// spell out <c>new And&lt;All&lt;Pos&gt;, None&lt;Dead&gt;&gt;(filter1, filter2)</c> explicitly.
    /// </para>
    /// <para>
    /// Alternatively, you can use <see cref="World{TWorld}.Query{TFilter0,TFilter1}"/> which wraps
    /// filters into <c>And&lt;...&gt;</c> automatically. The factory is useful when you need to
    /// construct a composite filter separately from the query (e.g., to store or pass it around).
    /// </para>
    /// </summary>
    public static class And {
        /// <summary>
        /// Creates a composite filter combining two <see cref="IQueryFilter"/> instances with AND semantics.
        /// An entity matches only if it satisfies both filters. Filters are applied sequentially:
        /// <paramref name="filter0"/> first, then <paramref name="filter1"/> on the surviving bitmask.
        /// </summary>
        /// <returns>A <see cref="And{TFilter0,TFilter1}"/> composite filter.</returns>
        [MethodImpl(AggressiveInlining)]
        public static And<TFilter0, TFilter1> By<TFilter0, TFilter1>(
            TFilter0 filter0,
            TFilter1 filter1
        )
            where TFilter0 : struct, IQueryFilter
            where TFilter1 : struct, IQueryFilter {
            return new And<TFilter0, TFilter1>(
                filter0,
                filter1
            );
        }

        /// <inheritdoc cref="By{TFilter0,TFilter1}"/>
        [MethodImpl(AggressiveInlining)]
        public static And<TFilter0, TFilter1, TFilter2> By<TFilter0, TFilter1, TFilter2>(
            TFilter0 filter0,
            TFilter1 filter1,
            TFilter2 filter2
        )
            where TFilter0 : struct, IQueryFilter
            where TFilter1 : struct, IQueryFilter
            where TFilter2 : struct, IQueryFilter {
            return new And<TFilter0, TFilter1, TFilter2>(
                filter0,
                filter1,
                filter2
            );
        }

        /// <inheritdoc cref="By{TFilter0,TFilter1}"/>
        [MethodImpl(AggressiveInlining)]
        public static And<TFilter0, TFilter1, TFilter2, TFilter3> By<TFilter0, TFilter1, TFilter2, TFilter3>(
            TFilter0 filter0,
            TFilter1 filter1,
            TFilter2 filter2,
            TFilter3 filter3
        )
            where TFilter0 : struct, IQueryFilter
            where TFilter1 : struct, IQueryFilter
            where TFilter2 : struct, IQueryFilter
            where TFilter3 : struct, IQueryFilter {
            return new And<TFilter0, TFilter1, TFilter2, TFilter3>(
                filter0,
                filter1,
                filter2,
                filter3
            );
        }

        /// <inheritdoc cref="By{TFilter0,TFilter1}"/>
        [MethodImpl(AggressiveInlining)]
        public static And<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4> By<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4>(
            TFilter0 filter0,
            TFilter1 filter1,
            TFilter2 filter2,
            TFilter3 filter3,
            TFilter4 filter4
        )
            where TFilter0 : struct, IQueryFilter
            where TFilter1 : struct, IQueryFilter
            where TFilter2 : struct, IQueryFilter
            where TFilter3 : struct, IQueryFilter
            where TFilter4 : struct, IQueryFilter {
            return new And<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4>(
                filter0,
                filter1,
                filter2,
                filter3,
                filter4
            );
        }

        /// <inheritdoc cref="By{TFilter0,TFilter1}"/>
        [MethodImpl(AggressiveInlining)]
        public static And<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4, TFilter5> By<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4, TFilter5>(
            TFilter0 filter0,
            TFilter1 filter1,
            TFilter2 filter2,
            TFilter3 filter3,
            TFilter4 filter4,
            TFilter5 filter5
        )
            where TFilter0 : struct, IQueryFilter
            where TFilter1 : struct, IQueryFilter
            where TFilter2 : struct, IQueryFilter
            where TFilter3 : struct, IQueryFilter
            where TFilter4 : struct, IQueryFilter
            where TFilter5 : struct, IQueryFilter {
            return new And<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4, TFilter5>(
                filter0,
                filter1,
                filter2,
                filter3,
                filter4,
                filter5);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    /// <summary>
    /// A no-op query filter that matches all entities unconditionally. All <see cref="IQueryFilter"/>
    /// methods are empty — no chunk or entity bitmask bits are ever cleared.
    /// <para>
    /// Used as the default filter when <see cref="World{TWorld}.Query()"/> is called without
    /// type parameters. Iterates over every alive entity (subject to <see cref="EntityStatusType"/>).
    /// </para>
    /// </summary>
    public readonly struct Nothing : IQueryFilter {
        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType { return ulong.MaxValue; }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType { return ulong.MaxValue; }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType { return ulong.MaxValue; }

        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType { return ulong.MaxValue; }
        #endif

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_DEBUG
        /// <inheritdoc/>
        public void Assert<TWorld>() where TWorld : struct, IWorldType { }

        /// <inheritdoc/>
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    /// <summary>
    /// Composite query filter that combines two <see cref="IQueryFilter"/> instances with AND semantics.
    /// During filtering, each sub-filter's <see cref="IQueryFilter.FilterChunk{TWorld}"/> and
    /// <see cref="IQueryFilter.FilterEntities{TWorld}"/> are called sequentially, progressively
    /// narrowing the bitmask. An entity matches only if ALL sub-filters accept it.
    /// <para>
    /// This is the most commonly used composite filter, created implicitly by
    /// <see cref="World{TWorld}.Query{TFilter0, TFilter1}"/> or explicitly via
    /// <see cref="And.By{TFilter0,TFilter1}"/>.
    /// Example: <c>And&lt;All&lt;Position&gt;, None&lt;Dead&gt;&gt;</c> matches entities
    /// that have Position AND do not have Dead.
    /// </para>
    /// </summary>
    public struct And<TFilter0, TFilter1> : IQueryFilter
        where TFilter0 : struct, IQueryFilter
        where TFilter1 : struct, IQueryFilter {
        private TFilter0 _filter0;
        private TFilter1 _filter1;

        /// <summary>
        /// Initializes the composite filter with two sub-filter instances.
        /// </summary>
        public And(TFilter0 filter0,
                    TFilter1 filter1) {
            _filter0 = filter0;
            _filter1 = filter1;
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterChunk<TWorld>(chunkIdx)
                 & _filter1.FilterChunk<TWorld>(chunkIdx);
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter1.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
        }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterChunk<TWorld>(chunkIdx)
                 & _filter1.BurstFilterChunk<TWorld>(chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter1.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
        }
        #endif

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            _filter0.PushQueryData<TWorld>(data);
            _filter1.PushQueryData<TWorld>(data);
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            _filter0.PopQueryData<TWorld>();
            _filter1.PopQueryData<TWorld>();
        }

        #if FFS_ECS_DEBUG
        /// <inheritdoc/>
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            _filter0.Assert<TWorld>();
            _filter1.Assert<TWorld>();
        }

        /// <inheritdoc/>
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            _filter0.Block<TWorld>(val);
            _filter1.Block<TWorld>(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    /// <inheritdoc cref="And{TFilter0,TFilter1}"/>
    public struct And<TFilter0, TFilter1, TFilter2> : IQueryFilter
        where TFilter0 : struct, IQueryFilter
        where TFilter1 : struct, IQueryFilter
        where TFilter2 : struct, IQueryFilter {
        private TFilter0 _filter0;
        private TFilter1 _filter1;
        private TFilter2 _filter2;

        /// <inheritdoc cref="And{TFilter0,TFilter1}.And(TFilter0, TFilter1)"/>
        public And(TFilter0 filter0,
                    TFilter1 filter1,
                    TFilter2 filter2) {
            _filter0 = filter0;
            _filter1 = filter1;
            _filter2 = filter2;
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterChunk<TWorld>(chunkIdx)
                 & _filter1.FilterChunk<TWorld>(chunkIdx)
                 & _filter2.FilterChunk<TWorld>(chunkIdx);
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter1.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter2.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
        }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterChunk<TWorld>(chunkIdx)
                 & _filter1.BurstFilterChunk<TWorld>(chunkIdx)
                 & _filter2.BurstFilterChunk<TWorld>(chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter1.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter2.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
        }
        #endif

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            _filter0.PushQueryData<TWorld>(data);
            _filter1.PushQueryData<TWorld>(data);
            _filter2.PushQueryData<TWorld>(data);
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            _filter0.PopQueryData<TWorld>();
            _filter1.PopQueryData<TWorld>();
            _filter2.PopQueryData<TWorld>();
        }

        #if FFS_ECS_DEBUG
        /// <inheritdoc/>
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            _filter0.Assert<TWorld>();
            _filter1.Assert<TWorld>();
            _filter2.Assert<TWorld>();
        }

        /// <inheritdoc/>
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            _filter0.Block<TWorld>(val);
            _filter1.Block<TWorld>(val);
            _filter2.Block<TWorld>(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    /// <inheritdoc cref="And{TFilter0,TFilter1}"/>
    public struct And<TFilter0, TFilter1, TFilter2, TFilter3> : IQueryFilter
        where TFilter0 : struct, IQueryFilter
        where TFilter1 : struct, IQueryFilter
        where TFilter2 : struct, IQueryFilter
        where TFilter3 : struct, IQueryFilter {
        private TFilter0 _filter0;
        private TFilter1 _filter1;
        private TFilter2 _filter2;
        private TFilter3 _filter3;

        /// <inheritdoc cref="And{TFilter0,TFilter1}.And(TFilter0, TFilter1)"/>
        public And(TFilter0 filter0,
                    TFilter1 filter1,
                    TFilter2 filter2,
                    TFilter3 filter3) {
            _filter0 = filter0;
            _filter1 = filter1;
            _filter2 = filter2;
            _filter3 = filter3;
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterChunk<TWorld>(chunkIdx)
                 & _filter1.FilterChunk<TWorld>(chunkIdx)
                 & _filter2.FilterChunk<TWorld>(chunkIdx)
                 & _filter3.FilterChunk<TWorld>(chunkIdx);
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter1.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter2.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter3.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
        }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterChunk<TWorld>(chunkIdx)
                 & _filter1.BurstFilterChunk<TWorld>(chunkIdx)
                 & _filter2.BurstFilterChunk<TWorld>(chunkIdx)
                 & _filter3.BurstFilterChunk<TWorld>(chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter1.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter2.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter3.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
        }
        #endif

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            _filter0.PushQueryData<TWorld>(data);
            _filter1.PushQueryData<TWorld>(data);
            _filter2.PushQueryData<TWorld>(data);
            _filter3.PushQueryData<TWorld>(data);
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            _filter0.PopQueryData<TWorld>();
            _filter1.PopQueryData<TWorld>();
            _filter2.PopQueryData<TWorld>();
            _filter3.PopQueryData<TWorld>();
        }

        #if FFS_ECS_DEBUG
        /// <inheritdoc/>
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            _filter0.Assert<TWorld>();
            _filter1.Assert<TWorld>();
            _filter2.Assert<TWorld>();
            _filter3.Assert<TWorld>();
        }

        /// <inheritdoc/>
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            _filter0.Block<TWorld>(val);
            _filter1.Block<TWorld>(val);
            _filter2.Block<TWorld>(val);
            _filter3.Block<TWorld>(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    /// <inheritdoc cref="And{TFilter0,TFilter1}"/>
    public struct And<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4> : IQueryFilter
        where TFilter0 : struct, IQueryFilter
        where TFilter1 : struct, IQueryFilter
        where TFilter2 : struct, IQueryFilter
        where TFilter3 : struct, IQueryFilter
        where TFilter4 : struct, IQueryFilter {
        private TFilter0 _filter0;
        private TFilter1 _filter1;
        private TFilter2 _filter2;
        private TFilter3 _filter3;
        private TFilter4 _filter4;

        /// <inheritdoc cref="And{TFilter0,TFilter1}.And(TFilter0, TFilter1)"/>
        public And(TFilter0 filter0,
                    TFilter1 filter1,
                    TFilter2 filter2,
                    TFilter3 filter3,
                    TFilter4 filter4) {
            _filter0 = filter0;
            _filter1 = filter1;
            _filter2 = filter2;
            _filter3 = filter3;
            _filter4 = filter4;
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterChunk<TWorld>(chunkIdx)
                 & _filter1.FilterChunk<TWorld>(chunkIdx)
                 & _filter2.FilterChunk<TWorld>(chunkIdx)
                 & _filter3.FilterChunk<TWorld>(chunkIdx)
                 & _filter4.FilterChunk<TWorld>(chunkIdx);
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter1.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter2.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter3.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter4.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
        }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterChunk<TWorld>(chunkIdx)
                 & _filter1.BurstFilterChunk<TWorld>(chunkIdx)
                 & _filter2.BurstFilterChunk<TWorld>(chunkIdx)
                 & _filter3.BurstFilterChunk<TWorld>(chunkIdx)
                 & _filter4.BurstFilterChunk<TWorld>(chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter1.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter2.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter3.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter4.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
        }
        #endif

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            _filter0.PushQueryData<TWorld>(data);
            _filter1.PushQueryData<TWorld>(data);
            _filter2.PushQueryData<TWorld>(data);
            _filter3.PushQueryData<TWorld>(data);
            _filter4.PushQueryData<TWorld>(data);
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            _filter0.PopQueryData<TWorld>();
            _filter1.PopQueryData<TWorld>();
            _filter2.PopQueryData<TWorld>();
            _filter3.PopQueryData<TWorld>();
            _filter4.PopQueryData<TWorld>();
        }

        #if FFS_ECS_DEBUG
        /// <inheritdoc/>
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            _filter0.Assert<TWorld>();
            _filter1.Assert<TWorld>();
            _filter2.Assert<TWorld>();
            _filter3.Assert<TWorld>();
            _filter4.Assert<TWorld>();
        }

        /// <inheritdoc/>
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            _filter0.Block<TWorld>(val);
            _filter1.Block<TWorld>(val);
            _filter2.Block<TWorld>(val);
            _filter3.Block<TWorld>(val);
            _filter4.Block<TWorld>(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    /// <inheritdoc cref="And{TFilter0,TFilter1}"/>
    public struct And<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4, TFilter5> : IQueryFilter
        where TFilter0 : struct, IQueryFilter
        where TFilter1 : struct, IQueryFilter
        where TFilter2 : struct, IQueryFilter
        where TFilter3 : struct, IQueryFilter
        where TFilter4 : struct, IQueryFilter
        where TFilter5 : struct, IQueryFilter {
        private TFilter0 _filter0;
        private TFilter1 _filter1;
        private TFilter2 _filter2;
        private TFilter3 _filter3;
        private TFilter4 _filter4;
        private TFilter5 _filter5;

        /// <inheritdoc cref="And{TFilter0,TFilter1}.And(TFilter0, TFilter1)"/>
        public And(TFilter0 filter0,
                    TFilter1 filter1,
                    TFilter2 filter2,
                    TFilter3 filter3,
                    TFilter4 filter4,
                    TFilter5 filter5) {
            _filter0 = filter0;
            _filter1 = filter1;
            _filter2 = filter2;
            _filter3 = filter3;
            _filter4 = filter4;
            _filter5 = filter5;
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterChunk<TWorld>(chunkIdx)
                 & _filter1.FilterChunk<TWorld>(chunkIdx)
                 & _filter2.FilterChunk<TWorld>(chunkIdx)
                 & _filter3.FilterChunk<TWorld>(chunkIdx)
                 & _filter4.FilterChunk<TWorld>(chunkIdx)
                 & _filter5.FilterChunk<TWorld>(chunkIdx);
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter1.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter2.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter3.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter4.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter5.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
        }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterChunk<TWorld>(chunkIdx)
                 & _filter1.BurstFilterChunk<TWorld>(chunkIdx)
                 & _filter2.BurstFilterChunk<TWorld>(chunkIdx)
                 & _filter3.BurstFilterChunk<TWorld>(chunkIdx)
                 & _filter4.BurstFilterChunk<TWorld>(chunkIdx)
                 & _filter5.BurstFilterChunk<TWorld>(chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter1.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter2.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter3.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter4.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 & _filter5.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
        }
        #endif

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            _filter0.PushQueryData<TWorld>(data);
            _filter1.PushQueryData<TWorld>(data);
            _filter2.PushQueryData<TWorld>(data);
            _filter3.PushQueryData<TWorld>(data);
            _filter4.PushQueryData<TWorld>(data);
            _filter5.PushQueryData<TWorld>(data);
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            _filter0.PopQueryData<TWorld>();
            _filter1.PopQueryData<TWorld>();
            _filter2.PopQueryData<TWorld>();
            _filter3.PopQueryData<TWorld>();
            _filter4.PopQueryData<TWorld>();
            _filter5.PopQueryData<TWorld>();
        }

        #if FFS_ECS_DEBUG
        /// <inheritdoc/>
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            _filter0.Assert<TWorld>();
            _filter1.Assert<TWorld>();
            _filter2.Assert<TWorld>();
            _filter3.Assert<TWorld>();
            _filter4.Assert<TWorld>();
            _filter5.Assert<TWorld>();
        }

        /// <inheritdoc/>
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            _filter0.Block<TWorld>(val);
            _filter1.Block<TWorld>(val);
            _filter2.Block<TWorld>(val);
            _filter3.Block<TWorld>(val);
            _filter4.Block<TWorld>(val);
            _filter5.Block<TWorld>(val);
        }
        #endif
    }
}