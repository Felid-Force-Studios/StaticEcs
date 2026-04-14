#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace FFS.Libraries.StaticEcs {

    #region ALL
    /// <summary>
    /// Query filter that requires an entity to have ALL specified enabled component types.
    /// Entities missing any of the listed components, or having them in a disabled state, are excluded.
    /// <para>
    /// This is the most common filter. Use in query type parameters to select entities with a specific
    /// set of components: <c>world.Query&lt;All&lt;Position, Velocity&gt;&gt;()</c> iterates only
    /// entities that have both <c>Position</c> AND <c>Velocity</c> enabled.
    /// </para>
    /// <para>
    /// Available with 1–8 type parameters. For single-component queries, <c>All&lt;T&gt;</c> is valid.
    /// Combine with other filters via query type composition:
    /// <c>world.Query&lt;All&lt;Pos, Vel&gt;, None&lt;Frozen&gt;&gt;()</c>.
    /// </para>
    /// <para>
    /// During strict-mode iteration, deleting or disabling any of the listed component types on OTHER entities
    /// is forbidden (asserted in debug mode) because it would invalidate the iteration bitmask.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct All<T0> : IQueryFilter
        where T0 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteDisable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteDisable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteDisable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="All{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct All<T0, T1> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteDisable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteDisable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteDisable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="All{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct All<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteDisable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteDisable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteDisable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="All{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct All<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteDisable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteDisable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteDisable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="All{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct All<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteDisable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteDisable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteDisable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="All{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct All<T0, T1, T2, T3, T4, T5> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T5>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDeleteDisable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDeleteDisable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T5>.Instance.BlockDeleteDisable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="All{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct All<T0, T1, T2, T3, T4, T5, T6> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag
        where T6 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T5>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T6>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForDeleteDisable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForDeleteDisable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T5>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T6>.Instance.BlockDeleteDisable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="All{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct All<T0, T1, T2, T3, T4, T5, T6, T7> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag
        where T6 : struct, IComponentOrTag
        where T7 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T7>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T5>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T6>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T7>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T7>.Instance.PushQueryDataForDeleteDisable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T7>.Instance.PopQueryDataForDeleteDisable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T7>(World<TWorld>.Components<T7>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T5>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T6>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T7>.Instance.BlockDeleteDisable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }
    #endregion

    #region ALL_ONLY_DISABLED
    /// <summary>
    /// Query filter that requires an entity to have ALL specified component types in the disabled state.
    /// Entities where any of the listed components is absent or enabled are excluded.
    /// <para>
    /// Use this to find entities where specific components have been explicitly disabled via
    /// <c>entity.Disable&lt;T&gt;()</c>. For example, <c>AllOnlyDisabled&lt;AI&gt;</c> matches entities
    /// with AI component present but disabled — useful for finding paused/frozen entities.
    /// </para>
    /// <para>Available with 1–8 type parameters.</para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllOnlyDisabled<T0> : IQueryFilter
        where T0 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AllOnlyDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllOnlyDisabled<T0, T1> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AllOnlyDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllOnlyDisabled<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AllOnlyDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllOnlyDisabled<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AllOnlyDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllOnlyDisabled<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AllOnlyDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllOnlyDisabled<T0, T1, T2, T3, T4, T5> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T5>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDeleteEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDeleteEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T5>.Instance.BlockDeleteEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AllOnlyDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllOnlyDisabled<T0, T1, T2, T3, T4, T5, T6> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T5>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T6>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForDeleteEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForDeleteEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T5>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T6>.Instance.BlockDeleteEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AllOnlyDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllOnlyDisabled<T0, T1, T2, T3, T4, T5, T6, T7> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
        where T7 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T7>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T5>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T6>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T7>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T7>.Instance.PushQueryDataForDeleteEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T7>.Instance.PopQueryDataForDeleteEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T7>(World<TWorld>.Components<T7>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T5>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T6>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T7>.Instance.BlockDeleteEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }
    #endregion

    #region ALL_WITH_DISABLED
    /// <summary>
    /// Query filter that requires an entity to have ALL specified component types, regardless of
    /// whether they are enabled or disabled. Unlike <see cref="All{T0}"/> which only matches enabled
    /// components, this filter matches any presence state.
    /// <para>
    /// Use this when you need to process entities that have the component data regardless of
    /// its enabled/disabled state. For example, serialization or cleanup logic that should run
    /// on all entities with certain components, even paused ones.
    /// </para>
    /// <para>Available with 1–8 type parameters.</para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllWithDisabled<T0> : IQueryFilter
        where T0 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDelete(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDelete(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AllWithDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllWithDisabled<T0, T1> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDelete(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDelete(val);
            World<TWorld>.Components<T1>.Instance.BlockDelete(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AllWithDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllWithDisabled<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDelete(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDelete(val);
            World<TWorld>.Components<T1>.Instance.BlockDelete(val);
            World<TWorld>.Components<T2>.Instance.BlockDelete(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AllWithDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllWithDisabled<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDelete(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDelete(val);
            World<TWorld>.Components<T1>.Instance.BlockDelete(val);
            World<TWorld>.Components<T2>.Instance.BlockDelete(val);
            World<TWorld>.Components<T3>.Instance.BlockDelete(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AllWithDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllWithDisabled<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDelete(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDelete(val);
            World<TWorld>.Components<T1>.Instance.BlockDelete(val);
            World<TWorld>.Components<T2>.Instance.BlockDelete(val);
            World<TWorld>.Components<T3>.Instance.BlockDelete(val);
            World<TWorld>.Components<T4>.Instance.BlockDelete(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AllWithDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllWithDisabled<T0, T1, T2, T3, T4, T5> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T5>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDelete(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDelete(val);
            World<TWorld>.Components<T1>.Instance.BlockDelete(val);
            World<TWorld>.Components<T2>.Instance.BlockDelete(val);
            World<TWorld>.Components<T3>.Instance.BlockDelete(val);
            World<TWorld>.Components<T4>.Instance.BlockDelete(val);
            World<TWorld>.Components<T5>.Instance.BlockDelete(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AllWithDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllWithDisabled<T0, T1, T2, T3, T4, T5, T6> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag
        where T6 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T5>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T6>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForDelete(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDelete(val);
            World<TWorld>.Components<T1>.Instance.BlockDelete(val);
            World<TWorld>.Components<T2>.Instance.BlockDelete(val);
            World<TWorld>.Components<T3>.Instance.BlockDelete(val);
            World<TWorld>.Components<T4>.Instance.BlockDelete(val);
            World<TWorld>.Components<T5>.Instance.BlockDelete(val);
            World<TWorld>.Components<T6>.Instance.BlockDelete(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AllWithDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AllWithDisabled<T0, T1, T2, T3, T4, T5, T6, T7> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag
        where T6 : struct, IComponentOrTag
        where T7 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & World<TWorld>.Components<T7>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T5>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T6>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & World<TWorld>.Components<T7>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T7>.Instance.PushQueryDataForDelete(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T7>.Instance.PopQueryDataForDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T7>(World<TWorld>.Components<T7>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDelete(val);
            World<TWorld>.Components<T1>.Instance.BlockDelete(val);
            World<TWorld>.Components<T2>.Instance.BlockDelete(val);
            World<TWorld>.Components<T3>.Instance.BlockDelete(val);
            World<TWorld>.Components<T4>.Instance.BlockDelete(val);
            World<TWorld>.Components<T5>.Instance.BlockDelete(val);
            World<TWorld>.Components<T6>.Instance.BlockDelete(val);
            World<TWorld>.Components<T7>.Instance.BlockDelete(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         & BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }
    #endregion

    #region NONE
    /// <summary>
    /// Query filter that excludes entities having ANY of the specified enabled component types.
    /// An entity passes this filter only if it does NOT have any of the listed components in an enabled state.
    /// <para>
    /// Use for exclusion patterns: <c>world.Query&lt;All&lt;Position&gt;, None&lt;Frozen&gt;&gt;()</c>
    /// iterates entities with Position but without an enabled Frozen component.
    /// </para>
    /// <para>
    /// During strict-mode iteration, adding or enabling any of the listed component types on OTHER entities
    /// is forbidden (asserted in debug mode) because it would invalidate the exclusion bitmask.
    /// </para>
    /// <para>Available with 1–8 type parameters.</para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct None<T0> : IQueryFilter
        where T0 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAddEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAddEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAddEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="None{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct None<T0, T1> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForAddEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForAddEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockAddEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="None{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct None<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForAddEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForAddEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockAddEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="None{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct None<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForAddEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForAddEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T3>.Instance.BlockAddEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }


    /// <inheritdoc cref="None{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct None<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForAddEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForAddEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T3>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T4>.Instance.BlockAddEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="None{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct None<T0, T1, T2, T3, T4, T5> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T5>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForAddEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForAddEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T3>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T4>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T5>.Instance.BlockAddEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="None{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct None<T0, T1, T2, T3, T4, T5, T6> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag
        where T6 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T5>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T6>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForAddEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForAddEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T3>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T4>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T5>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T6>.Instance.BlockAddEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="None{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct None<T0, T1, T2, T3, T4, T5, T6, T7> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag
        where T6 : struct, IComponentOrTag
        where T7 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T7>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T5>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T6>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T7>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForAddEnable(data);
            World<TWorld>.Components<T7>.Instance.PushQueryDataForAddEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForAddEnable();
            World<TWorld>.Components<T7>.Instance.PopQueryDataForAddEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T7>(World<TWorld>.Components<T7>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T3>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T4>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T5>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T6>.Instance.BlockAddEnable(val);
            World<TWorld>.Components<T7>.Instance.BlockAddEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }
    #endregion

    #region NONE_WITH_DISABLED
    /// <summary>
    /// Query filter that excludes entities having ANY of the specified component types in any state
    /// (enabled or disabled). Unlike <see cref="None{T0}"/> which only excludes enabled components,
    /// this filter excludes entities that have the component at all, even if disabled.
    /// <para>
    /// Use when disabled components should also cause exclusion. For example, if an entity should
    /// not be processed if a component was ever added (regardless of enable/disable state).
    /// </para>
    /// <para>Available with 1–8 type parameters.</para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneWithDisabled<T0> : IQueryFilter
        where T0 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAdd(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAdd(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="NoneWithDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneWithDisabled<T0, T1> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForAdd(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAdd(val);
            World<TWorld>.Components<T1>.Instance.BlockAdd(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="NoneWithDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneWithDisabled<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForAdd(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAdd(val);
            World<TWorld>.Components<T1>.Instance.BlockAdd(val);
            World<TWorld>.Components<T2>.Instance.BlockAdd(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="NoneWithDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneWithDisabled<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForAdd(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAdd(val);
            World<TWorld>.Components<T1>.Instance.BlockAdd(val);
            World<TWorld>.Components<T2>.Instance.BlockAdd(val);
            World<TWorld>.Components<T3>.Instance.BlockAdd(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }


    /// <inheritdoc cref="NoneWithDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneWithDisabled<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForAdd(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAdd(val);
            World<TWorld>.Components<T1>.Instance.BlockAdd(val);
            World<TWorld>.Components<T2>.Instance.BlockAdd(val);
            World<TWorld>.Components<T3>.Instance.BlockAdd(val);
            World<TWorld>.Components<T4>.Instance.BlockAdd(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="NoneWithDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneWithDisabled<T0, T1, T2, T3, T4, T5> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T5>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForAdd(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAdd(val);
            World<TWorld>.Components<T1>.Instance.BlockAdd(val);
            World<TWorld>.Components<T2>.Instance.BlockAdd(val);
            World<TWorld>.Components<T3>.Instance.BlockAdd(val);
            World<TWorld>.Components<T4>.Instance.BlockAdd(val);
            World<TWorld>.Components<T5>.Instance.BlockAdd(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="NoneWithDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneWithDisabled<T0, T1, T2, T3, T4, T5, T6> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag
        where T6 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T5>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T6>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForAdd(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAdd(val);
            World<TWorld>.Components<T1>.Instance.BlockAdd(val);
            World<TWorld>.Components<T2>.Instance.BlockAdd(val);
            World<TWorld>.Components<T3>.Instance.BlockAdd(val);
            World<TWorld>.Components<T4>.Instance.BlockAdd(val);
            World<TWorld>.Components<T5>.Instance.BlockAdd(val);
            World<TWorld>.Components<T6>.Instance.BlockAdd(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="NoneWithDisabled{T0}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct NoneWithDisabled<T0, T1, T2, T3, T4, T5, T6, T7> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag
        where T6 : struct, IComponentOrTag
        where T7 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~World<TWorld>.Components<T7>.Instance.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T5>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T6>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~World<TWorld>.Components<T7>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForAdd(data);
            World<TWorld>.Components<T7>.Instance.PushQueryDataForAdd(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForAdd();
            World<TWorld>.Components<T7>.Instance.PopQueryDataForAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T7>(World<TWorld>.Components<T7>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockAdd(val);
            World<TWorld>.Components<T1>.Instance.BlockAdd(val);
            World<TWorld>.Components<T2>.Instance.BlockAdd(val);
            World<TWorld>.Components<T3>.Instance.BlockAdd(val);
            World<TWorld>.Components<T4>.Instance.BlockAdd(val);
            World<TWorld>.Components<T5>.Instance.BlockAdd(val);
            World<TWorld>.Components<T6>.Instance.BlockAdd(val);
            World<TWorld>.Components<T7>.Instance.BlockAdd(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value
                         & ~BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.HeuristicChunks[chunkIdx].FullBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ~BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            & ~BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }
    #endregion

    #region ANY
    /// <summary>
    /// Query filter that requires an entity to have AT LEAST ONE of the specified enabled component types.
    /// Entities that have none of the listed components enabled are excluded.
    /// <para>
    /// Use for "or" logic in queries: <c>world.Query&lt;Any&lt;Renderable, UIElement&gt;&gt;()</c>
    /// iterates entities that have either Renderable OR UIElement (or both).
    /// </para>
    /// <para>
    /// Note: <c>Any</c> requires a minimum of 2 type parameters (a single-component "any" is equivalent
    /// to <see cref="All{T0}"/>). Available with 2–8 type parameters.
    /// </para>
    /// <para>
    /// During strict-mode iteration, deleting or disabling any of the listed component types on OTHER entities
    /// is forbidden (asserted in debug mode).
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct Any<T0, T1> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }

            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteDisable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteDisable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteDisable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="Any{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct Any<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteDisable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteDisable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteDisable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="Any{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct Any<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteDisable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteDisable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteDisable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }


    /// <inheritdoc cref="Any{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct Any<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteDisable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteDisable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteDisable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="Any{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct Any<T0, T1, T2, T3, T4, T5> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T5>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T5>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T5>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDeleteDisable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDeleteDisable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T5>.Instance.BlockDeleteDisable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="Any{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct Any<T0, T1, T2, T3, T4, T5, T6> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag
        where T6 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T5>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T6>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T5>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T6>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T5>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T6>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForDeleteDisable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForDeleteDisable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T5>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T6>.Instance.BlockDeleteDisable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="Any{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct Any<T0, T1, T2, T3, T4, T5, T6, T7> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag
        where T6 : struct, IComponentOrTag
        where T7 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T7>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T5>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T6>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T7>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T5>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T6>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T7>.Instance.EnabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T3>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T4>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T5>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T6>.Instance.EnabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T7>.Instance.EnabledMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForDeleteDisable(data);
            World<TWorld>.Components<T7>.Instance.PushQueryDataForDeleteDisable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForDeleteDisable();
            World<TWorld>.Components<T7>.Instance.PopQueryDataForDeleteDisable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T7>(World<TWorld>.Components<T7>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T5>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T6>.Instance.BlockDeleteDisable(val);
            World<TWorld>.Components<T7>.Instance.BlockDeleteDisable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.EnabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }
    #endregion

    #region ANY_ONLY_DISABLED
    /// <summary>
    /// Query filter that requires an entity to have AT LEAST ONE of the specified component types
    /// in the disabled state. Entities where none of the listed components are disabled are excluded.
    /// <para>
    /// Use to find entities that have been partially or fully paused. For example,
    /// <c>AnyOnlyDisabled&lt;AI, Physics&gt;</c> matches entities where at least one of those
    /// components has been disabled.
    /// </para>
    /// <para>Available with 2–8 type parameters.</para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyOnlyDisabled<T0, T1> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AnyOnlyDisabled{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyOnlyDisabled<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AnyOnlyDisabled{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyOnlyDisabled<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }


    /// <inheritdoc cref="AnyOnlyDisabled{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyOnlyDisabled<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AnyOnlyDisabled{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyOnlyDisabled<T0, T1, T2, T3, T4, T5> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T5>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T5>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T5>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDeleteEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDeleteEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T5>.Instance.BlockDeleteEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AnyOnlyDisabled{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyOnlyDisabled<T0, T1, T2, T3, T4, T5, T6> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T5>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T6>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T5>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T6>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T5>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T6>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForDeleteEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForDeleteEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T5>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T6>.Instance.BlockDeleteEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AnyOnlyDisabled{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyOnlyDisabled<T0, T1, T2, T3, T4, T5, T6, T7> : IQueryFilter
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where T6 : struct, IComponent
        where T7 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T7>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T5>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T6>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T7>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T5>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T6>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T7>.Instance.DisabledMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T3>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T4>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T5>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T6>.Instance.DisabledMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T7>.Instance.DisabledMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForDeleteEnable(data);
            World<TWorld>.Components<T7>.Instance.PushQueryDataForDeleteEnable(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForDeleteEnable();
            World<TWorld>.Components<T7>.Instance.PopQueryDataForDeleteEnable();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T7>(World<TWorld>.Components<T7>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T1>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T2>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T3>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T4>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T5>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T6>.Instance.BlockDeleteEnable(val);
            World<TWorld>.Components<T7>.Instance.BlockDeleteEnable(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.DisabledMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }
    #endregion

    #region ANY_WITH_DISABLED
    /// <summary>
    /// Query filter that requires an entity to have AT LEAST ONE of the specified component types
    /// present in any state (enabled or disabled). Unlike <see cref="Any{T0, T1}"/> which only matches
    /// enabled components, this filter matches presence regardless of enabled/disabled state.
    /// <para>Available with 2–8 type parameters.</para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyWithDisabled<T0, T1> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDelete(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDelete(val);
            World<TWorld>.Components<T1>.Instance.BlockDelete(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AnyWithDisabled{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyWithDisabled<T0, T1, T2> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDelete(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDelete(val);
            World<TWorld>.Components<T1>.Instance.BlockDelete(val);
            World<TWorld>.Components<T2>.Instance.BlockDelete(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AnyWithDisabled{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyWithDisabled<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDelete(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDelete(val);
            World<TWorld>.Components<T1>.Instance.BlockDelete(val);
            World<TWorld>.Components<T2>.Instance.BlockDelete(val);
            World<TWorld>.Components<T3>.Instance.BlockDelete(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }


    /// <inheritdoc cref="AnyWithDisabled{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyWithDisabled<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDelete(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDelete(val);
            World<TWorld>.Components<T1>.Instance.BlockDelete(val);
            World<TWorld>.Components<T2>.Instance.BlockDelete(val);
            World<TWorld>.Components<T3>.Instance.BlockDelete(val);
            World<TWorld>.Components<T4>.Instance.BlockDelete(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AnyWithDisabled{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyWithDisabled<T0, T1, T2, T3, T4, T5> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T5>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T5>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T5>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDelete(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDelete(val);
            World<TWorld>.Components<T1>.Instance.BlockDelete(val);
            World<TWorld>.Components<T2>.Instance.BlockDelete(val);
            World<TWorld>.Components<T3>.Instance.BlockDelete(val);
            World<TWorld>.Components<T4>.Instance.BlockDelete(val);
            World<TWorld>.Components<T5>.Instance.BlockDelete(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AnyWithDisabled{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyWithDisabled<T0, T1, T2, T3, T4, T5, T6> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag
        where T6 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T5>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T6>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T5>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T6>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T5>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T6>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForDelete(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDelete(val);
            World<TWorld>.Components<T1>.Instance.BlockDelete(val);
            World<TWorld>.Components<T2>.Instance.BlockDelete(val);
            World<TWorld>.Components<T3>.Instance.BlockDelete(val);
            World<TWorld>.Components<T4>.Instance.BlockDelete(val);
            World<TWorld>.Components<T5>.Instance.BlockDelete(val);
            World<TWorld>.Components<T6>.Instance.BlockDelete(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }

    /// <inheritdoc cref="AnyWithDisabled{T0, T1}"/>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    #if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
    #endif
    public readonly struct AnyWithDisabled<T0, T1, T2, T3, T4, T5, T6, T7> : IQueryFilter
        where T0 : struct, IComponentOrTag
        where T1 : struct, IComponentOrTag
        where T2 : struct, IComponentOrTag
        where T3 : struct, IComponentOrTag
        where T4 : struct, IComponentOrTag
        where T5 : struct, IComponentOrTag
        where T6 : struct, IComponentOrTag
        where T7 : struct, IComponentOrTag {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T1>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T2>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T3>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T4>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T5>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T6>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | World<TWorld>.Components<T7>.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T5>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T6>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                            | World<TWorld>.Components<T7>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool FilterEntity<TWorld>(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            var eMask = ~invertedBlockEntityMask;
            return (World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T5>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T6>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                   || (World<TWorld>.Components<T7>.Instance.AnyMask(segmentIdx, segmentBlockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType {
            data.OnCacheUpdate = static (cache, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, batch) => {
                if (!batch) {
                    if (!FilterEntity<TWorld>(invertedBlockEntityMask, segmentIdx, segmentBlockIdx)) {
                        cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
                    }
                }
                else {
                    var mask = World<TWorld>.Components<T0>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T1>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T2>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T3>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T4>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T5>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T6>.Instance.AnyMask(segmentIdx, segmentBlockIdx)
                               | World<TWorld>.Components<T7>.Instance.AnyMask(segmentIdx, segmentBlockIdx);
                    cache[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= mask;
                }
            };
            World<TWorld>.Components<T0>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T1>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T2>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T3>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T4>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T5>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T6>.Instance.PushQueryDataForDelete(data);
            World<TWorld>.Components<T7>.Instance.PushQueryDataForDelete(data);
        }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T1>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T2>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T3>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T4>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T5>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T6>.Instance.PopQueryDataForDelete();
            World<TWorld>.Components<T7>.Instance.PopQueryDataForDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertRegisteredComponent<T0>(World<TWorld>.Components<T0>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T1>(World<TWorld>.Components<T1>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T2>(World<TWorld>.Components<T2>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T3>(World<TWorld>.Components<T3>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T4>(World<TWorld>.Components<T4>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T5>(World<TWorld>.Components<T5>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T6>(World<TWorld>.Components<T6>.ComponentsTypeName);
            World<TWorld>.AssertRegisteredComponent<T7>(World<TWorld>.Components<T7>.ComponentsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType {
            World<TWorld>.Components<T0>.Instance.BlockDelete(val);
            World<TWorld>.Components<T1>.Instance.BlockDelete(val);
            World<TWorld>.Components<T2>.Instance.BlockDelete(val);
            World<TWorld>.Components<T3>.Instance.BlockDelete(val);
            World<TWorld>.Components<T4>.Instance.BlockDelete(val);
            World<TWorld>.Components<T5>.Instance.BlockDelete(val);
            World<TWorld>.Components<T6>.Instance.BlockDelete(val);
            World<TWorld>.Components<T7>.Instance.BlockDelete(val);
        }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                         | BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return BurstView<TWorld>.ComponentMasks<T0>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T1>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T2>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T3>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T4>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T5>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T6>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx)
                            | BurstView<TWorld>.ComponentMasks<T7>.Instance.Data.AnyMask(segmentIdx, segmentBlockIdx);
        }
        #endif
    }
    #endregion

}