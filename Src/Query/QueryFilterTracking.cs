#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif
#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace FFS.Libraries.StaticEcs {

    #region ALL_ADDED
    /// <summary>
    /// Query filter that matches entities which had the specified component types added since the system's last tick.
    /// Uses AddedHeuristicChunks/AddedMask for filtering.
    /// <para>
    /// Available with 1-5 type parameters. Requires TrackAdded to be enabled for the component types.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Requires <c>ComponentTypeConfig&lt;T&gt;.TrackAdded</c> to be <c>true</c> for the component type.
    /// Combine with <c>All&lt;T&gt;</c> to filter entities that were added AND currently have the component:
    /// <c>world.Query&lt;All&lt;Position&gt;, AllAdded&lt;Position&gt;&gt;()</c>.
    /// </para>
    /// <para>
    /// Tracking is managed automatically by `W.Tick()`. Use the `fromTick` constructor parameter for custom tick ranges.
    /// Added and Deleted masks are independent — an entity can have both bits set if a component was
    /// added and deleted (or deleted and re-added) between ticks.
    /// </para>
    /// </remarks>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllAdded<T0> : IQueryFilter
        where T0 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AllAdded(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.AddedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AllAdded{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllAdded<T0, T1> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AllAdded(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T1>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T1>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T1>.Instance.AddedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AllAdded{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllAdded<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AllAdded(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T1>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T2>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T1>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T2>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T1>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T2>.Instance.AddedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AllAdded{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllAdded<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AllAdded(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T1>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T2>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T3>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & World<TWorld>.Components<T3>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T1>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T2>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T3>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T1>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T2>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T3>.Instance.AddedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AllAdded{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllAdded<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AllAdded(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T1>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T2>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T3>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T4>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & World<TWorld>.Components<T3>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & World<TWorld>.Components<T4>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T1>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T2>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T3>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T4>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T1>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T2>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T3>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T4>.Instance.AddedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }
    #endregion
    #region NONE_ADDED
    /// <summary>
    /// Negative query filter that excludes entities which had the specified component types added since the system's last tick.
    /// Uses inverted AddedMask for entity-level filtering. Chunk-level filtering is a no-op (cannot efficiently invert at chunk level).
    /// <para>
    /// Available with 1-5 type parameters. Requires TrackAdded to be enabled for the component types.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneAdded<T0> : IQueryFilter
        where T0 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public NoneAdded(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return ~World<TWorld>.Components<T0>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return ~World<TWorld>.Components<T0>.Instance.AddedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="NoneAdded{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneAdded<T0, T1> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public NoneAdded(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return ~(World<TWorld>.Components<T0>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx));
            }
            return ~World<TWorld>.Components<T0>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T1>.Instance.AddedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="NoneAdded{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneAdded<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public NoneAdded(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return ~(World<TWorld>.Components<T0>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx));
            }
            return ~World<TWorld>.Components<T0>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T1>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T2>.Instance.AddedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="NoneAdded{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneAdded<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public NoneAdded(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return ~(World<TWorld>.Components<T0>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T3>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx));
            }
            return ~World<TWorld>.Components<T0>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T1>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T2>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T3>.Instance.AddedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="NoneAdded{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneAdded<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public NoneAdded(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return ~(World<TWorld>.Components<T0>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T3>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T4>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx));
            }
            return ~World<TWorld>.Components<T0>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T1>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T2>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T3>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T4>.Instance.AddedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }
    #endregion
    #region ANY_ADDED
    /// <summary>
    /// Query filter that matches entities which had at least one of the specified component types added since the system's last tick.
    /// Uses AddedHeuristicChunks/AddedMask for filtering with OR logic.
    /// <para>
    /// Available with 2-5 type parameters. Requires TrackAdded to be enabled for the component types.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyAdded<T0, T1> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AnyAdded(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T1>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T1>.Instance.AddedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AnyAdded{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyAdded<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AnyAdded(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T1>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T2>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T1>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T2>.Instance.AddedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AnyAdded{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyAdded<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AnyAdded(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T1>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T2>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T3>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | World<TWorld>.Components<T3>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T3>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T1>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T2>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T3>.Instance.AddedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AnyAdded{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyAdded<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AnyAdded(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T1>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T2>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T3>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T4>.Instance.AddedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | World<TWorld>.Components<T3>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | World<TWorld>.Components<T4>.Instance.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T3>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T4>.Instance.AddedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T1>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T2>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T3>.Instance.AddedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T4>.Instance.AddedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.TrackingHeuristicChunks[chunkIdx].AddedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.AddedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackAdded<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }
    #endregion
    #region ALL_DELETED
    /// <summary>
    /// Query filter that matches entities which had the specified component types deleted since the system's last tick.
    /// Uses DeletedHeuristicChunks/DeletedMask for filtering.
    /// <para>
    /// Available with 1-5 type parameters. Requires TrackDeleted to be enabled for the component types.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Requires <c>ComponentTypeConfig&lt;T&gt;.TrackDeleted</c> to be <c>true</c> for the component type.
    /// Note that matched entities may no longer have the component — the mask records the deletion event,
    /// not the current presence. Combine with <c>All&lt;T&gt;</c> if you need entities that still have the component.
    /// </para>
    /// <para>
    /// Tracking is managed automatically by `W.Tick()`. Use the `fromTick` constructor parameter for custom tick ranges.
    /// Added and Deleted masks are independent — an entity can have both bits set if a component was
    /// added and deleted (or deleted and re-added) between ticks.
    /// </para>
    /// </remarks>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllDeleted<T0> : IQueryFilter
        where T0 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AllDeleted(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.DeletedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AllDeleted{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllDeleted<T0, T1> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AllDeleted(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T1>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T1>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T1>.Instance.DeletedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AllDeleted{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllDeleted<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AllDeleted(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T1>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T2>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T1>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T2>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T1>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T2>.Instance.DeletedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AllDeleted{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllDeleted<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AllDeleted(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T1>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T2>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T3>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & World<TWorld>.Components<T3>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T1>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T2>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T3>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T1>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T2>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T3>.Instance.DeletedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AllDeleted{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllDeleted<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AllDeleted(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T1>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T2>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T3>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T4>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & World<TWorld>.Components<T3>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & World<TWorld>.Components<T4>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T1>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T2>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T3>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T4>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T1>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T2>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T3>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T4>.Instance.DeletedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }
    #endregion
    #region NONE_DELETED
    /// <summary>
    /// Negative query filter that excludes entities which had the specified component types deleted since the system's last tick.
    /// Uses inverted DeletedMask for entity-level filtering. Chunk-level filtering is a no-op (cannot efficiently invert at chunk level).
    /// <para>
    /// Available with 1-5 type parameters. Requires TrackDeleted to be enabled for the component types.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneDeleted<T0> : IQueryFilter
        where T0 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public NoneDeleted(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return ~World<TWorld>.Components<T0>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return ~World<TWorld>.Components<T0>.Instance.DeletedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="NoneDeleted{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneDeleted<T0, T1> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public NoneDeleted(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return ~(World<TWorld>.Components<T0>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx));
            }
            return ~World<TWorld>.Components<T0>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T1>.Instance.DeletedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="NoneDeleted{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneDeleted<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public NoneDeleted(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return ~(World<TWorld>.Components<T0>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx));
            }
            return ~World<TWorld>.Components<T0>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T1>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T2>.Instance.DeletedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="NoneDeleted{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneDeleted<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public NoneDeleted(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return ~(World<TWorld>.Components<T0>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T3>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx));
            }
            return ~World<TWorld>.Components<T0>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T1>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T2>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T3>.Instance.DeletedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="NoneDeleted{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneDeleted<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public NoneDeleted(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return ~(World<TWorld>.Components<T0>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T3>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T4>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx));
            }
            return ~World<TWorld>.Components<T0>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T1>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T2>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T3>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T4>.Instance.DeletedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }
    #endregion
    #region ANY_DELETED
    /// <summary>
    /// Query filter that matches entities which had at least one of the specified component types deleted since the system's last tick.
    /// Uses DeletedHeuristicChunks/DeletedMask for filtering with OR logic.
    /// <para>
    /// Available with 2-5 type parameters. Requires TrackDeleted to be enabled for the component types.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyDeleted<T0, T1> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AnyDeleted(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T1>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T1>.Instance.DeletedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AnyDeleted{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyDeleted<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AnyDeleted(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T1>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T2>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T1>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T2>.Instance.DeletedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AnyDeleted{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyDeleted<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AnyDeleted(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T1>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T2>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T3>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | World<TWorld>.Components<T3>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T3>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T1>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T2>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T3>.Instance.DeletedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AnyDeleted{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyDeleted<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag {

        public readonly ulong FromTick;
        public AnyDeleted(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T1>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T2>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T3>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T4>.Instance.DeletedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | World<TWorld>.Components<T3>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | World<TWorld>.Components<T4>.Instance.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T3>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T4>.Instance.DeletedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T1>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T2>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T3>.Instance.DeletedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T4>.Instance.DeletedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.TrackingHeuristicChunks[chunkIdx].DeletedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.DeletedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackDeleted<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }
    #endregion
    #region CREATED
    /// <summary>
    /// Query filter that matches entities which were created since the system's last tick.
    /// <para>
    /// Requires <c>WorldConfig.TrackCreated</c> to be <c>true</c>.
    /// Tracking is managed automatically by `W.Tick()`. Use the `fromTick` constructor parameter for custom tick ranges.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct Created : IQueryFilter {

        public readonly ulong FromTick;
        public Created(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return data.CreatedHeuristicHistory(from, data.CurrentTick, chunkIdx);
            }
            return data.CreatedTrackingChunks[chunkIdx];
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return data.CreatedMaskHistory(from, data.CurrentTick, segmentIdx, segmentBlockIdx);
            }
            return data.CreatedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertTrackCreated();
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif

        public ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType => throw new System.NotImplementedException();

        public ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType => throw new System.NotImplementedException();
    }
    #endregion

    #if !FFS_ECS_DISABLE_CHANGED_TRACKING

    #region ALL_CHANGED
    /// <summary>
    /// Query filter that matches entities which had the specified component types changed since the system's last tick.
    /// Uses ChangedHeuristicChunks/ChangedMask for filtering.
    /// <para>
    /// Available with 1-5 type parameters. Requires TrackChanged to be enabled for the component types.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllChanged<T0> : IQueryFilter
        where T0 : struct, IComponent {

        public readonly ulong FromTick;
        public AllChanged(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.ChangedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AllChanged{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllChanged<T0, T1> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent {

        public readonly ulong FromTick;
        public AllChanged(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T1>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T1>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T1>.Instance.ChangedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AllChanged{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllChanged<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent {

        public readonly ulong FromTick;
        public AllChanged(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T1>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T2>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T1>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T2>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T1>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T2>.Instance.ChangedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AllChanged{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllChanged<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent {

        public readonly ulong FromTick;
        public AllChanged(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T1>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T2>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T3>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & World<TWorld>.Components<T3>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T1>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T2>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T3>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T1>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T2>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T3>.Instance.ChangedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AllChanged{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllChanged<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent {

        public readonly ulong FromTick;
        public AllChanged(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T1>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T2>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T3>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       & World<TWorld>.Components<T4>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & World<TWorld>.Components<T3>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & World<TWorld>.Components<T4>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T1>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T2>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T3>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       & World<TWorld>.Components<T4>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T1>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T2>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T3>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & World<TWorld>.Components<T4>.Instance.ChangedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }
    #endregion

    #region NONE_CHANGED
    /// <summary>
    /// Negative query filter that excludes entities which had the specified component types changed since the system's last tick.
    /// Uses inverted ChangedMask for entity-level filtering. Chunk-level filtering is a no-op (cannot efficiently invert at chunk level).
    /// <para>
    /// Available with 1-5 type parameters. Requires TrackChanged to be enabled for the component types.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneChanged<T0> : IQueryFilter
        where T0 : struct, IComponent {

        public readonly ulong FromTick;
        public NoneChanged(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return ~World<TWorld>.Components<T0>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return ~World<TWorld>.Components<T0>.Instance.ChangedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="NoneChanged{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneChanged<T0, T1> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent {

        public readonly ulong FromTick;
        public NoneChanged(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return ~(World<TWorld>.Components<T0>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx));
            }
            return ~World<TWorld>.Components<T0>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T1>.Instance.ChangedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="NoneChanged{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneChanged<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent {

        public readonly ulong FromTick;
        public NoneChanged(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return ~(World<TWorld>.Components<T0>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx));
            }
            return ~World<TWorld>.Components<T0>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T1>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T2>.Instance.ChangedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="NoneChanged{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneChanged<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent {

        public readonly ulong FromTick;
        public NoneChanged(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return ~(World<TWorld>.Components<T0>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T3>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx));
            }
            return ~World<TWorld>.Components<T0>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T1>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T2>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T3>.Instance.ChangedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="NoneChanged{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneChanged<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent {

        public readonly ulong FromTick;
        public NoneChanged(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return ~(World<TWorld>.Components<T0>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T3>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T4>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx));
            }
            return ~World<TWorld>.Components<T0>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T1>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T2>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T3>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~World<TWorld>.Components<T4>.Instance.ChangedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }
    #endregion

    #region ANY_CHANGED
    /// <summary>
    /// Query filter that matches entities which had at least one of the specified component types changed since the system's last tick.
    /// Uses ChangedHeuristicChunks/ChangedMask for filtering with OR logic.
    /// <para>
    /// Available with 2-5 type parameters. Requires TrackChanged to be enabled for the component types.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyChanged<T0, T1> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent {

        public readonly ulong FromTick;
        public AnyChanged(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T1>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T1>.Instance.ChangedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AnyChanged{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyChanged<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent {

        public readonly ulong FromTick;
        public AnyChanged(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T1>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T2>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T1>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T2>.Instance.ChangedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AnyChanged{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyChanged<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent {

        public readonly ulong FromTick;
        public AnyChanged(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T1>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T2>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T3>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | World<TWorld>.Components<T3>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T3>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T1>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T2>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T3>.Instance.ChangedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }

    /// <inheritdoc cref="AnyChanged{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyChanged<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent {

        public readonly ulong FromTick;
        public AnyChanged(ulong fromTick = 0) { FromTick = fromTick; }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T1>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T2>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T3>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx)
                       | World<TWorld>.Components<T4>.Instance.ChangedHeuristicHistory(from, data.CurrentTick, data.TrackingBufferSize, chunkIdx);
            }
            return World<TWorld>.Components<T0>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | World<TWorld>.Components<T1>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | World<TWorld>.Components<T2>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | World<TWorld>.Components<T3>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | World<TWorld>.Components<T4>.Instance.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            if (data.TrackingBufferSize > 0) {
                var from = FromTick != 0 ? FromTick : data.CurrentLastTick;
                return World<TWorld>.Components<T0>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T1>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T2>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T3>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx)
                       | World<TWorld>.Components<T4>.Instance.ChangedMaskHistory(from, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx);
            }
            return World<TWorld>.Components<T0>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T1>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T2>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T3>.Instance.ChangedMask(segmentIdx, segmentBlockIdx)
                   | World<TWorld>.Components<T4>.Instance.ChangedMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value
                   | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.TrackingHeuristicChunks[chunkIdx].ChangedBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx)
                   | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.ChangedMask(segmentIdx, segmentBlockIdx);
        }
        #endif

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertComponentTrackChanged<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif
    }
    #endregion

    #region TRACKER
    public abstract partial class World<TWorld> {

        internal interface IChangedTracker {
            bool IsActive { get; }
            void ApplyBlock(uint segmentIdx, byte segmentBlockIdx, ulong entitiesMask, byte chunkBlockIdx, uint chunkIdx);
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
        #endif
        internal readonly struct ChangedTracker<T0> : IChangedTracker
            where T0 : struct, IComponent {
            private readonly bool _track0;

            public bool IsActive { [MethodImpl(AggressiveInlining)] get => _track0; }

            [MethodImpl(AggressiveInlining)]
            // ReSharper disable once UnusedParameter.Local
            public ChangedTracker(byte _) {
                _track0 = Components<T0>.Instance.TrackChanged;
            }

            [MethodImpl(AggressiveInlining)]
            public void ApplyBlock(uint segmentIdx, byte segmentBlockIdx, ulong entitiesMask, byte chunkBlockIdx, uint chunkIdx) {
                if (_track0) Components<T0>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
            }
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
        #endif
        internal readonly struct ChangedTracker<T0, T1> : IChangedTracker
            where T0 : struct, IComponent
            where T1 : struct, IComponent {
            private readonly bool _track0;
            private readonly bool _track1;

            public bool IsActive { [MethodImpl(AggressiveInlining)] get => _track0 || _track1; }

            [MethodImpl(AggressiveInlining)]
            // ReSharper disable once UnusedParameter.Local
            public ChangedTracker(byte _) {
                _track0 = Components<T0>.Instance.TrackChanged;
                _track1 = Components<T1>.Instance.TrackChanged;
            }

            [MethodImpl(AggressiveInlining)]
            public void ApplyBlock(uint segmentIdx, byte segmentBlockIdx, ulong entitiesMask, byte chunkBlockIdx, uint chunkIdx) {
                if (_track0) Components<T0>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                if (_track1) Components<T1>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
            }
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
        #endif
        internal readonly struct ChangedTracker<T0, T1, T2> : IChangedTracker
            where T0 : struct, IComponent
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            private readonly bool _track0;
            private readonly bool _track1;
            private readonly bool _track2;

            public bool IsActive { [MethodImpl(AggressiveInlining)] get => _track0 || _track1 || _track2; }

            [MethodImpl(AggressiveInlining)]
            // ReSharper disable once UnusedParameter.Local
            public ChangedTracker(byte _) {
                _track0 = Components<T0>.Instance.TrackChanged;
                _track1 = Components<T1>.Instance.TrackChanged;
                _track2 = Components<T2>.Instance.TrackChanged;
            }

            [MethodImpl(AggressiveInlining)]
            public void ApplyBlock(uint segmentIdx, byte segmentBlockIdx, ulong entitiesMask, byte chunkBlockIdx, uint chunkIdx) {
                if (_track0) Components<T0>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                if (_track1) Components<T1>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                if (_track2) Components<T2>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
            }
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
        #endif
        internal readonly struct ChangedTracker<T0, T1, T2, T3> : IChangedTracker
            where T0 : struct, IComponent
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            private readonly bool _track0;
            private readonly bool _track1;
            private readonly bool _track2;
            private readonly bool _track3;

            public bool IsActive { [MethodImpl(AggressiveInlining)] get => _track0 || _track1 || _track2 || _track3; }

            [MethodImpl(AggressiveInlining)]
            // ReSharper disable once UnusedParameter.Local
            public ChangedTracker(byte _) {
                _track0 = Components<T0>.Instance.TrackChanged;
                _track1 = Components<T1>.Instance.TrackChanged;
                _track2 = Components<T2>.Instance.TrackChanged;
                _track3 = Components<T3>.Instance.TrackChanged;
            }

            [MethodImpl(AggressiveInlining)]
            public void ApplyBlock(uint segmentIdx, byte segmentBlockIdx, ulong entitiesMask, byte chunkBlockIdx, uint chunkIdx) {
                if (_track0) Components<T0>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                if (_track1) Components<T1>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                if (_track2) Components<T2>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                if (_track3) Components<T3>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
            }
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
        #endif
        internal readonly struct ChangedTracker<T0, T1, T2, T3, T4> : IChangedTracker
            where T0 : struct, IComponent
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            private readonly bool _track0;
            private readonly bool _track1;
            private readonly bool _track2;
            private readonly bool _track3;
            private readonly bool _track4;

            public bool IsActive { [MethodImpl(AggressiveInlining)] get => _track0 || _track1 || _track2 || _track3 || _track4; }

            [MethodImpl(AggressiveInlining)]
            // ReSharper disable once UnusedParameter.Local
            public ChangedTracker(byte _) {
                _track0 = Components<T0>.Instance.TrackChanged;
                _track1 = Components<T1>.Instance.TrackChanged;
                _track2 = Components<T2>.Instance.TrackChanged;
                _track3 = Components<T3>.Instance.TrackChanged;
                _track4 = Components<T4>.Instance.TrackChanged;
            }

            [MethodImpl(AggressiveInlining)]
            public void ApplyBlock(uint segmentIdx, byte segmentBlockIdx, ulong entitiesMask, byte chunkBlockIdx, uint chunkIdx) {
                if (_track0) Components<T0>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                if (_track1) Components<T1>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                if (_track2) Components<T2>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                if (_track3) Components<T3>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                if (_track4) Components<T4>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
            }
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
        #endif
        internal readonly struct ChangedTracker<T0, T1, T2, T3, T4, T5> : IChangedTracker
            where T0 : struct, IComponent
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            private readonly bool _track0;
            private readonly bool _track1;
            private readonly bool _track2;
            private readonly bool _track3;
            private readonly bool _track4;
            private readonly bool _track5;

            public bool IsActive { [MethodImpl(AggressiveInlining)] get => _track0 || _track1 || _track2 || _track3 || _track4 || _track5; }

            [MethodImpl(AggressiveInlining)]
            // ReSharper disable once UnusedParameter.Local
            public ChangedTracker(byte _) {
                _track0 = Components<T0>.Instance.TrackChanged;
                _track1 = Components<T1>.Instance.TrackChanged;
                _track2 = Components<T2>.Instance.TrackChanged;
                _track3 = Components<T3>.Instance.TrackChanged;
                _track4 = Components<T4>.Instance.TrackChanged;
                _track5 = Components<T5>.Instance.TrackChanged;
            }

            [MethodImpl(AggressiveInlining)]
            public void ApplyBlock(uint segmentIdx, byte segmentBlockIdx, ulong entitiesMask, byte chunkBlockIdx, uint chunkIdx) {
                if (_track0) Components<T0>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                if (_track1) Components<T1>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                if (_track2) Components<T2>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                if (_track3) Components<T3>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                if (_track4) Components<T4>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                if (_track5) Components<T5>.Instance.SetChangedBitBatch(entitiesMask, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
            }
        }
    }
    #endregion

    #endif
}
