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
    /// multiple filters with OR semantics. Each <c>By</c> overload constructs an <c>Or&lt;...&gt;</c>
    /// struct that applies all provided filters and ORs their results — an entity matches if it satisfies
    /// at least one of the filters.
    /// <para>
    /// This factory exists because C# cannot infer generic type arguments for constructors.
    /// Using <c>Or.By(filter1, filter2)</c> enables full type inference, avoiding the need to
    /// spell out <c>new Or&lt;All&lt;Pos&gt;, All&lt;Vel&gt;&gt;(filter1, filter2)</c> explicitly.
    /// </para>
    /// </summary>
    public static class Or {
        /// <summary>
        /// Creates a composite filter combining two <see cref="IQueryFilter"/> instances with OR semantics.
        /// An entity matches if it satisfies at least one filter.
        /// </summary>
        /// <returns>An <see cref="Or{TFilter0,TFilter1}"/> composite filter.</returns>
        [MethodImpl(AggressiveInlining)]
        public static Or<TFilter0, TFilter1> By<TFilter0, TFilter1>(
            TFilter0 filter0,
            TFilter1 filter1
        )
            where TFilter0 : struct, IQueryFilter
            where TFilter1 : struct, IQueryFilter {
            return new Or<TFilter0, TFilter1>(
                filter0,
                filter1
            );
        }

        /// <inheritdoc cref="By{TFilter0,TFilter1}"/>
        [MethodImpl(AggressiveInlining)]
        public static Or<TFilter0, TFilter1, TFilter2> By<TFilter0, TFilter1, TFilter2>(
            TFilter0 filter0,
            TFilter1 filter1,
            TFilter2 filter2
        )
            where TFilter0 : struct, IQueryFilter
            where TFilter1 : struct, IQueryFilter
            where TFilter2 : struct, IQueryFilter {
            return new Or<TFilter0, TFilter1, TFilter2>(
                filter0,
                filter1,
                filter2
            );
        }

        /// <inheritdoc cref="By{TFilter0,TFilter1}"/>
        [MethodImpl(AggressiveInlining)]
        public static Or<TFilter0, TFilter1, TFilter2, TFilter3> By<TFilter0, TFilter1, TFilter2, TFilter3>(
            TFilter0 filter0,
            TFilter1 filter1,
            TFilter2 filter2,
            TFilter3 filter3
        )
            where TFilter0 : struct, IQueryFilter
            where TFilter1 : struct, IQueryFilter
            where TFilter2 : struct, IQueryFilter
            where TFilter3 : struct, IQueryFilter {
            return new Or<TFilter0, TFilter1, TFilter2, TFilter3>(
                filter0,
                filter1,
                filter2,
                filter3
            );
        }

        /// <inheritdoc cref="By{TFilter0,TFilter1}"/>
        [MethodImpl(AggressiveInlining)]
        public static Or<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4> By<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4>(
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
            return new Or<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4>(
                filter0,
                filter1,
                filter2,
                filter3,
                filter4
            );
        }

        /// <inheritdoc cref="By{TFilter0,TFilter1}"/>
        [MethodImpl(AggressiveInlining)]
        public static Or<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4, TFilter5> By<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4, TFilter5>(
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
            return new Or<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4, TFilter5>(
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
    /// Composite query filter that combines two <see cref="IQueryFilter"/> instances with OR semantics.
    /// During filtering, each sub-filter's <see cref="IQueryFilter.FilterChunk{TWorld}"/> and
    /// <see cref="IQueryFilter.FilterEntities{TWorld}"/> results are ORed together.
    /// An entity matches if ANY sub-filter accepts it.
    /// <para>
    /// Created explicitly via <c>Or.By(filter1, filter2)</c> or <c>new Or&lt;F1, F2&gt;(filter1, filter2)</c>.
    /// Example: <c>Or&lt;All&lt;Position&gt;, All&lt;Velocity&gt;&gt;</c> matches entities
    /// that have Position OR Velocity (or both).
    /// </para>
    /// </summary>
    public struct Or<TFilter0, TFilter1> : IQueryFilter
        where TFilter0 : struct, IQueryFilter
        where TFilter1 : struct, IQueryFilter {
        private TFilter0 _filter0;
        private TFilter1 _filter1;

        /// <summary>
        /// Initializes the composite filter with two sub-filter instances.
        /// </summary>
        public Or(TFilter0 filter0,
                    TFilter1 filter1) {
            _filter0 = filter0;
            _filter1 = filter1;
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterChunk<TWorld>(chunkIdx)
                 | _filter1.FilterChunk<TWorld>(chunkIdx);
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter1.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
        }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterChunk<TWorld>(chunkIdx)
                 | _filter1.BurstFilterChunk<TWorld>(chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter1.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
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
    /// <inheritdoc cref="Or{TFilter0,TFilter1}"/>
    public struct Or<TFilter0, TFilter1, TFilter2> : IQueryFilter
        where TFilter0 : struct, IQueryFilter
        where TFilter1 : struct, IQueryFilter
        where TFilter2 : struct, IQueryFilter {
        private TFilter0 _filter0;
        private TFilter1 _filter1;
        private TFilter2 _filter2;

        /// <inheritdoc cref="Or{TFilter0,TFilter1}.Or(TFilter0, TFilter1)"/>
        public Or(TFilter0 filter0,
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
                 | _filter1.FilterChunk<TWorld>(chunkIdx)
                 | _filter2.FilterChunk<TWorld>(chunkIdx);
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter1.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter2.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
        }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterChunk<TWorld>(chunkIdx)
                 | _filter1.BurstFilterChunk<TWorld>(chunkIdx)
                 | _filter2.BurstFilterChunk<TWorld>(chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter1.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter2.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
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
    /// <inheritdoc cref="Or{TFilter0,TFilter1}"/>
    public struct Or<TFilter0, TFilter1, TFilter2, TFilter3> : IQueryFilter
        where TFilter0 : struct, IQueryFilter
        where TFilter1 : struct, IQueryFilter
        where TFilter2 : struct, IQueryFilter
        where TFilter3 : struct, IQueryFilter {
        private TFilter0 _filter0;
        private TFilter1 _filter1;
        private TFilter2 _filter2;
        private TFilter3 _filter3;

        /// <inheritdoc cref="Or{TFilter0,TFilter1}.Or(TFilter0, TFilter1)"/>
        public Or(TFilter0 filter0,
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
                 | _filter1.FilterChunk<TWorld>(chunkIdx)
                 | _filter2.FilterChunk<TWorld>(chunkIdx)
                 | _filter3.FilterChunk<TWorld>(chunkIdx);
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter1.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter2.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter3.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
        }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterChunk<TWorld>(chunkIdx)
                 | _filter1.BurstFilterChunk<TWorld>(chunkIdx)
                 | _filter2.BurstFilterChunk<TWorld>(chunkIdx)
                 | _filter3.BurstFilterChunk<TWorld>(chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter1.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter2.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter3.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
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
    /// <inheritdoc cref="Or{TFilter0,TFilter1}"/>
    public struct Or<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4> : IQueryFilter
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

        /// <inheritdoc cref="Or{TFilter0,TFilter1}.Or(TFilter0, TFilter1)"/>
        public Or(TFilter0 filter0,
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
                 | _filter1.FilterChunk<TWorld>(chunkIdx)
                 | _filter2.FilterChunk<TWorld>(chunkIdx)
                 | _filter3.FilterChunk<TWorld>(chunkIdx)
                 | _filter4.FilterChunk<TWorld>(chunkIdx);
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter1.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter2.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter3.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter4.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
        }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterChunk<TWorld>(chunkIdx)
                 | _filter1.BurstFilterChunk<TWorld>(chunkIdx)
                 | _filter2.BurstFilterChunk<TWorld>(chunkIdx)
                 | _filter3.BurstFilterChunk<TWorld>(chunkIdx)
                 | _filter4.BurstFilterChunk<TWorld>(chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter1.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter2.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter3.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter4.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
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
    /// <inheritdoc cref="Or{TFilter0,TFilter1}"/>
    public struct Or<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4, TFilter5> : IQueryFilter
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

        /// <inheritdoc cref="Or{TFilter0,TFilter1}.Or(TFilter0, TFilter1)"/>
        public Or(TFilter0 filter0,
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
                 | _filter1.FilterChunk<TWorld>(chunkIdx)
                 | _filter2.FilterChunk<TWorld>(chunkIdx)
                 | _filter3.FilterChunk<TWorld>(chunkIdx)
                 | _filter4.FilterChunk<TWorld>(chunkIdx)
                 | _filter5.FilterChunk<TWorld>(chunkIdx);
        }

        /// <inheritdoc/>
        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter1.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter2.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter3.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter4.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter5.FilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
        }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterChunk<TWorld>(chunkIdx)
                 | _filter1.BurstFilterChunk<TWorld>(chunkIdx)
                 | _filter2.BurstFilterChunk<TWorld>(chunkIdx)
                 | _filter3.BurstFilterChunk<TWorld>(chunkIdx)
                 | _filter4.BurstFilterChunk<TWorld>(chunkIdx)
                 | _filter5.BurstFilterChunk<TWorld>(chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return _filter0.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter1.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter2.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter3.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter4.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx)
                 | _filter5.BurstFilterEntities<TWorld>(segmentIdx, segmentBlockIdx);
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