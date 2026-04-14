#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

#pragma warning disable CS0649

namespace FFS.Libraries.StaticEcs {

    #region IS
    /// <summary>
    /// Query filter that matches only entities whose entity type is <typeparamref name="T0"/>.
    /// </summary>
    /// <typeparam name="T0">Required entity type.</typeparam>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct EntityIs<T0> : IQueryFilter
        where T0 : struct, IEntityType {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Data.Instance.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T0).Name, World<TWorld>.EntityTypeInfo<T0>.Instance.Id);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return World<TWorld>.Data.Instance.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }
        #endif
    }
    #endregion

    #region IS_NOT
    /// <summary>
    /// Query filter that excludes entities whose entity type is <typeparamref name="T0"/>.
    /// </summary>
    /// <typeparam name="T0">Entity type to exclude.</typeparam>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct EntityIsNot<T0> : IQueryFilter
        where T0 : struct, IEntityType {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Data.Instance.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T0).Name, World<TWorld>.EntityTypeInfo<T0>.Instance.Id);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            return ~World<TWorld>.Data.Instance.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }
        #endif
    }

    /// <summary>
    /// Query filter that excludes entities whose entity type is <typeparamref name="T0"/> or <typeparamref name="T1"/>.
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct EntityIsNot<T0, T1> : IQueryFilter
        where T0 : struct, IEntityType
        where T1 : struct, IEntityType {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T0).Name, World<TWorld>.EntityTypeInfo<T0>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T1).Name, World<TWorld>.EntityTypeInfo<T1>.Instance.Id);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }
        #endif
    }

    /// <summary>
    /// Query filter that excludes entities whose entity type is any of the specified types.
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct EntityIsNot<T0, T1, T2> : IQueryFilter
        where T0 : struct, IEntityType
        where T1 : struct, IEntityType
        where T2 : struct, IEntityType {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T2>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T0).Name, World<TWorld>.EntityTypeInfo<T0>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T1).Name, World<TWorld>.EntityTypeInfo<T1>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T2).Name, World<TWorld>.EntityTypeInfo<T2>.Instance.Id);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T2>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }
        #endif
    }

    /// <summary>
    /// Query filter that excludes entities whose entity type is any of the specified types.
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct EntityIsNot<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IEntityType
        where T1 : struct, IEntityType
        where T2 : struct, IEntityType
        where T3 : struct, IEntityType {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T2>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T3>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T0).Name, World<TWorld>.EntityTypeInfo<T0>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T1).Name, World<TWorld>.EntityTypeInfo<T1>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T2).Name, World<TWorld>.EntityTypeInfo<T2>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T3).Name, World<TWorld>.EntityTypeInfo<T3>.Instance.Id);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T2>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T3>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }
        #endif
    }

    /// <summary>
    /// Query filter that excludes entities whose entity type is any of the specified types.
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct EntityIsNot<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IEntityType
        where T1 : struct, IEntityType
        where T2 : struct, IEntityType
        where T3 : struct, IEntityType
        where T4 : struct, IEntityType {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T2>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T3>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T4>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T0).Name, World<TWorld>.EntityTypeInfo<T0>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T1).Name, World<TWorld>.EntityTypeInfo<T1>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T2).Name, World<TWorld>.EntityTypeInfo<T2>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T3).Name, World<TWorld>.EntityTypeInfo<T3>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T4).Name, World<TWorld>.EntityTypeInfo<T4>.Instance.Id);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T2>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T3>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   & ~data.EntityTypes[World<TWorld>.EntityTypeInfo<T4>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }
        #endif
    }
    #endregion

    #region IS_ANY
    /// <summary>
    /// Query filter that matches entities whose entity type is <typeparamref name="T0"/> or <typeparamref name="T1"/>.
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct EntityIsAny<T0, T1> : IQueryFilter
        where T0 : struct, IEntityType
        where T1 : struct, IEntityType {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T0).Name, World<TWorld>.EntityTypeInfo<T0>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T1).Name, World<TWorld>.EntityTypeInfo<T1>.Instance.Id);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }
        #endif
    }

    /// <summary>
    /// Query filter that matches entities whose entity type is any of the specified types.
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct EntityIsAny<T0, T1, T2> : IQueryFilter
        where T0 : struct, IEntityType
        where T1 : struct, IEntityType
        where T2 : struct, IEntityType {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T2>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T0).Name, World<TWorld>.EntityTypeInfo<T0>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T1).Name, World<TWorld>.EntityTypeInfo<T1>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T2).Name, World<TWorld>.EntityTypeInfo<T2>.Instance.Id);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T2>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }
        #endif
    }

    /// <summary>
    /// Query filter that matches entities whose entity type is any of the specified types.
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct EntityIsAny<T0, T1, T2, T3> : IQueryFilter
        where T0 : struct, IEntityType
        where T1 : struct, IEntityType
        where T2 : struct, IEntityType
        where T3 : struct, IEntityType {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T2>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T3>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T0).Name, World<TWorld>.EntityTypeInfo<T0>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T1).Name, World<TWorld>.EntityTypeInfo<T1>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T2).Name, World<TWorld>.EntityTypeInfo<T2>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T3).Name, World<TWorld>.EntityTypeInfo<T3>.Instance.Id);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T2>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T3>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }
        #endif
    }

    /// <summary>
    /// Query filter that matches entities whose entity type is any of the specified types.
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct EntityIsAny<T0, T1, T2, T3, T4> : IQueryFilter
        where T0 : struct, IEntityType
        where T1 : struct, IEntityType
        where T2 : struct, IEntityType
        where T3 : struct, IEntityType
        where T4 : struct, IEntityType {

        [MethodImpl(AggressiveInlining)]
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T2>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T3>.Instance.Id].HeuristicChunks[chunkIdx].Value
                   | data.EntityTypes[World<TWorld>.EntityTypeInfo<T4>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }

        [MethodImpl(AggressiveInlining)]
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType { }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<TWorld>() where TWorld : struct, IWorldType {
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T0).Name, World<TWorld>.EntityTypeInfo<T0>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T1).Name, World<TWorld>.EntityTypeInfo<T1>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T2).Name, World<TWorld>.EntityTypeInfo<T2>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T3).Name, World<TWorld>.EntityTypeInfo<T3>.Instance.Id);
            World<TWorld>.AssertEntityTypeIsRegistered(typeof(T4).Name, World<TWorld>.EntityTypeInfo<T4>.Instance.Id);
        }

        [MethodImpl(AggressiveInlining)]
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType { }
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType {
            ref var data = ref World<TWorld>.Data.Instance;
            return data.EntityTypes[World<TWorld>.EntityTypeInfo<T0>.Instance.Id].HeuristicChunks[chunkIdx].Value
                         | data.EntityTypes[World<TWorld>.EntityTypeInfo<T1>.Instance.Id].HeuristicChunks[chunkIdx].Value
                         | data.EntityTypes[World<TWorld>.EntityTypeInfo<T2>.Instance.Id].HeuristicChunks[chunkIdx].Value
                         | data.EntityTypes[World<TWorld>.EntityTypeInfo<T3>.Instance.Id].HeuristicChunks[chunkIdx].Value
                         | data.EntityTypes[World<TWorld>.EntityTypeInfo<T4>.Instance.Id].HeuristicChunks[chunkIdx].Value;
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType {
            return ulong.MaxValue;
        }
        #endif
    }
    #endregion

}