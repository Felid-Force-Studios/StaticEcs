using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace FFS.Libraries.StaticEcs {
    #region ALL
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1> : IQueryMethod
        where C1 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteDisable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteDisable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteDisable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1, C2> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteDisable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C2>.Value.DecQDeleteDisable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteDisable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1, C2, C3> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteDisable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C2>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C3>.Value.DecQDeleteDisable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteDisable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1, C2, C3, C4> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteDisable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C2>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C3>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C4>.Value.DecQDeleteDisable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteDisable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1, C2, C3, C4, C5> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteDisable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C2>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C3>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C4>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C5>.Value.DecQDeleteDisable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteDisable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1, C2, C3, C4, C5, C6> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C6>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C6>.Value.IncQDeleteDisable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C2>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C3>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C4>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C5>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C6>.Value.DecQDeleteDisable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C6>.Value.BlockDeleteDisable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C7>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C6>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C7>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C6>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C7>.Value.IncQDeleteDisable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C2>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C3>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C4>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C5>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C6>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C7>.Value.DecQDeleteDisable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C6>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C7>.Value.BlockDeleteDisable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C7>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C8>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C6>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C7>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C8>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C6>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C7>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C8>.Value.IncQDeleteDisable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C2>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C3>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C4>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C5>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C6>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C7>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C8>.Value.DecQDeleteDisable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C6>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C7>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C8>.Value.BlockDeleteDisable(val);
        }
        #endif
    }
    #endregion

    #region ALL_ONLY_DISABLED
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1> : IQueryMethod
        where C1 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1, C2> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C2>.Value.DecQDeleteEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1, C2, C3> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C2>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C3>.Value.DecQDeleteEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1, C2, C3, C4> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C4>.Value.DMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C2>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C3>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C4>.Value.DecQDeleteEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1, C2, C3, C4, C5> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C4>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C5>.Value.DMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C2>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C3>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C4>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C5>.Value.DecQDeleteEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1, C2, C3, C4, C5, C6> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C4>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C5>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C6>.Value.DMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C6>.Value.IncQDeleteEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C2>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C3>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C4>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C5>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C6>.Value.DecQDeleteEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C6>.Value.BlockDeleteEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C7>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C4>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C5>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C6>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C7>.Value.DMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C6>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C7>.Value.IncQDeleteEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C2>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C3>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C4>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C5>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C6>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C7>.Value.DecQDeleteEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C6>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C7>.Value.BlockDeleteEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C7>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C8>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C4>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C5>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C6>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C7>.Value.DMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C8>.Value.DMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C6>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C7>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C8>.Value.IncQDeleteEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C2>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C3>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C4>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C5>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C6>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C7>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C8>.Value.DecQDeleteEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C6>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C7>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C8>.Value.BlockDeleteEnable(val);
        }
        #endif
    }
    #endregion

    #region ALL_WITH_DISABLED
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1> : IQueryMethod
        where C1 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDelete();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1, C2> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDelete(data);
            World<WorldType>.Components<C2>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDelete();
            World<WorldType>.Components<C2>.Value.DecQDelete();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDelete(val);
            World<WorldType>.Components<C2>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1, C2, C3> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDelete(data);
            World<WorldType>.Components<C2>.Value.IncQDelete(data);
            World<WorldType>.Components<C3>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDelete();
            World<WorldType>.Components<C2>.Value.DecQDelete();
            World<WorldType>.Components<C3>.Value.DecQDelete();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDelete(val);
            World<WorldType>.Components<C2>.Value.BlockDelete(val);
            World<WorldType>.Components<C3>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1, C2, C3, C4> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDelete(data);
            World<WorldType>.Components<C2>.Value.IncQDelete(data);
            World<WorldType>.Components<C3>.Value.IncQDelete(data);
            World<WorldType>.Components<C4>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDelete();
            World<WorldType>.Components<C2>.Value.DecQDelete();
            World<WorldType>.Components<C3>.Value.DecQDelete();
            World<WorldType>.Components<C4>.Value.DecQDelete();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDelete(val);
            World<WorldType>.Components<C2>.Value.BlockDelete(val);
            World<WorldType>.Components<C3>.Value.BlockDelete(val);
            World<WorldType>.Components<C4>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1, C2, C3, C4, C5> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDelete(data);
            World<WorldType>.Components<C2>.Value.IncQDelete(data);
            World<WorldType>.Components<C3>.Value.IncQDelete(data);
            World<WorldType>.Components<C4>.Value.IncQDelete(data);
            World<WorldType>.Components<C5>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDelete();
            World<WorldType>.Components<C2>.Value.DecQDelete();
            World<WorldType>.Components<C3>.Value.DecQDelete();
            World<WorldType>.Components<C4>.Value.DecQDelete();
            World<WorldType>.Components<C5>.Value.DecQDelete();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDelete(val);
            World<WorldType>.Components<C2>.Value.BlockDelete(val);
            World<WorldType>.Components<C3>.Value.BlockDelete(val);
            World<WorldType>.Components<C4>.Value.BlockDelete(val);
            World<WorldType>.Components<C5>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1, C2, C3, C4, C5, C6> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C6>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDelete(data);
            World<WorldType>.Components<C2>.Value.IncQDelete(data);
            World<WorldType>.Components<C3>.Value.IncQDelete(data);
            World<WorldType>.Components<C4>.Value.IncQDelete(data);
            World<WorldType>.Components<C5>.Value.IncQDelete(data);
            World<WorldType>.Components<C6>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDelete();
            World<WorldType>.Components<C2>.Value.DecQDelete();
            World<WorldType>.Components<C3>.Value.DecQDelete();
            World<WorldType>.Components<C4>.Value.DecQDelete();
            World<WorldType>.Components<C5>.Value.DecQDelete();
            World<WorldType>.Components<C6>.Value.DecQDelete();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDelete(val);
            World<WorldType>.Components<C2>.Value.BlockDelete(val);
            World<WorldType>.Components<C3>.Value.BlockDelete(val);
            World<WorldType>.Components<C4>.Value.BlockDelete(val);
            World<WorldType>.Components<C5>.Value.BlockDelete(val);
            World<WorldType>.Components<C6>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C7>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C6>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C7>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDelete(data);
            World<WorldType>.Components<C2>.Value.IncQDelete(data);
            World<WorldType>.Components<C3>.Value.IncQDelete(data);
            World<WorldType>.Components<C4>.Value.IncQDelete(data);
            World<WorldType>.Components<C5>.Value.IncQDelete(data);
            World<WorldType>.Components<C6>.Value.IncQDelete(data);
            World<WorldType>.Components<C7>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDelete();
            World<WorldType>.Components<C2>.Value.DecQDelete();
            World<WorldType>.Components<C3>.Value.DecQDelete();
            World<WorldType>.Components<C4>.Value.DecQDelete();
            World<WorldType>.Components<C5>.Value.DecQDelete();
            World<WorldType>.Components<C6>.Value.DecQDelete();
            World<WorldType>.Components<C7>.Value.DecQDelete();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDelete(val);
            World<WorldType>.Components<C2>.Value.BlockDelete(val);
            World<WorldType>.Components<C3>.Value.BlockDelete(val);
            World<WorldType>.Components<C4>.Value.BlockDelete(val);
            World<WorldType>.Components<C5>.Value.BlockDelete(val);
            World<WorldType>.Components<C6>.Value.BlockDelete(val);
            World<WorldType>.Components<C7>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C7>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Components<C8>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C6>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C7>.Value.AMask(chunkIdx, blockIdx)
                            & World<WorldType>.Components<C8>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQDelete(data);
            World<WorldType>.Components<C2>.Value.IncQDelete(data);
            World<WorldType>.Components<C3>.Value.IncQDelete(data);
            World<WorldType>.Components<C4>.Value.IncQDelete(data);
            World<WorldType>.Components<C5>.Value.IncQDelete(data);
            World<WorldType>.Components<C6>.Value.IncQDelete(data);
            World<WorldType>.Components<C7>.Value.IncQDelete(data);
            World<WorldType>.Components<C8>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDelete();
            World<WorldType>.Components<C2>.Value.DecQDelete();
            World<WorldType>.Components<C3>.Value.DecQDelete();
            World<WorldType>.Components<C4>.Value.DecQDelete();
            World<WorldType>.Components<C5>.Value.DecQDelete();
            World<WorldType>.Components<C6>.Value.DecQDelete();
            World<WorldType>.Components<C7>.Value.DecQDelete();
            World<WorldType>.Components<C8>.Value.DecQDelete();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDelete(val);
            World<WorldType>.Components<C2>.Value.BlockDelete(val);
            World<WorldType>.Components<C3>.Value.BlockDelete(val);
            World<WorldType>.Components<C4>.Value.BlockDelete(val);
            World<WorldType>.Components<C5>.Value.BlockDelete(val);
            World<WorldType>.Components<C6>.Value.BlockDelete(val);
            World<WorldType>.Components<C7>.Value.BlockDelete(val);
            World<WorldType>.Components<C8>.Value.BlockDelete(val);
        }
        #endif
    }
    #endregion

    #region NONE
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1> : IQueryMethod
        where C1 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAddEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAddEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAddEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C2>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C2>.Value.IncQAddEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAddEnable();
            World<WorldType>.Components<C2>.Value.DecQAddEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C2>.Value.BlockAddEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C3>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C2>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C3>.Value.IncQAddEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAddEnable();
            World<WorldType>.Components<C2>.Value.DecQAddEnable();
            World<WorldType>.Components<C3>.Value.DecQAddEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C2>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C3>.Value.BlockAddEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3, C4> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C3>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C4>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C2>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C3>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C4>.Value.IncQAddEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAddEnable();
            World<WorldType>.Components<C2>.Value.DecQAddEnable();
            World<WorldType>.Components<C3>.Value.DecQAddEnable();
            World<WorldType>.Components<C4>.Value.DecQAddEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C2>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C3>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C4>.Value.BlockAddEnable(val);
        }
        #endif
    }


    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3, C4, C5> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C3>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C4>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C5>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C2>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C3>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C4>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C5>.Value.IncQAddEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAddEnable();
            World<WorldType>.Components<C2>.Value.DecQAddEnable();
            World<WorldType>.Components<C3>.Value.DecQAddEnable();
            World<WorldType>.Components<C4>.Value.DecQAddEnable();
            World<WorldType>.Components<C5>.Value.DecQAddEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C2>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C3>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C4>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C5>.Value.BlockAddEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3, C4, C5, C6> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C3>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C4>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C5>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C6>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C6>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C2>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C3>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C4>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C5>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C6>.Value.IncQAddEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAddEnable();
            World<WorldType>.Components<C2>.Value.DecQAddEnable();
            World<WorldType>.Components<C3>.Value.DecQAddEnable();
            World<WorldType>.Components<C4>.Value.DecQAddEnable();
            World<WorldType>.Components<C5>.Value.DecQAddEnable();
            World<WorldType>.Components<C6>.Value.DecQAddEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C2>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C3>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C4>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C5>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C6>.Value.BlockAddEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C3>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C4>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C5>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C6>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C7>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C6>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C7>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C2>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C3>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C4>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C5>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C6>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C7>.Value.IncQAddEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAddEnable();
            World<WorldType>.Components<C2>.Value.DecQAddEnable();
            World<WorldType>.Components<C3>.Value.DecQAddEnable();
            World<WorldType>.Components<C4>.Value.DecQAddEnable();
            World<WorldType>.Components<C5>.Value.DecQAddEnable();
            World<WorldType>.Components<C6>.Value.DecQAddEnable();
            World<WorldType>.Components<C7>.Value.DecQAddEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C2>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C3>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C4>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C5>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C6>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C7>.Value.BlockAddEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C3>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C4>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C5>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C6>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C7>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C8>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C6>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C7>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C8>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C2>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C3>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C4>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C5>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C6>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C7>.Value.IncQAddEnable(data);
            World<WorldType>.Components<C8>.Value.IncQAddEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAddEnable();
            World<WorldType>.Components<C2>.Value.DecQAddEnable();
            World<WorldType>.Components<C3>.Value.DecQAddEnable();
            World<WorldType>.Components<C4>.Value.DecQAddEnable();
            World<WorldType>.Components<C5>.Value.DecQAddEnable();
            World<WorldType>.Components<C6>.Value.DecQAddEnable();
            World<WorldType>.Components<C7>.Value.DecQAddEnable();
            World<WorldType>.Components<C8>.Value.DecQAddEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C2>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C3>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C4>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C5>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C6>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C7>.Value.BlockAddEnable(val);
            World<WorldType>.Components<C8>.Value.BlockAddEnable(val);
        }
        #endif
    }
    #endregion

    #region NONE_WITH_DISABLED
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1> : IQueryMethod
        where C1 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAdd();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAdd(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1, C2> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C2>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAdd(data);
            World<WorldType>.Components<C2>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAdd();
            World<WorldType>.Components<C2>.Value.DecQAdd();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAdd(val);
            World<WorldType>.Components<C2>.Value.BlockAdd(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1, C2, C3> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C3>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAdd(data);
            World<WorldType>.Components<C2>.Value.IncQAdd(data);
            World<WorldType>.Components<C3>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAdd();
            World<WorldType>.Components<C2>.Value.DecQAdd();
            World<WorldType>.Components<C3>.Value.DecQAdd();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAdd(val);
            World<WorldType>.Components<C2>.Value.BlockAdd(val);
            World<WorldType>.Components<C3>.Value.BlockAdd(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1, C2, C3, C4> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C3>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C4>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAdd(data);
            World<WorldType>.Components<C2>.Value.IncQAdd(data);
            World<WorldType>.Components<C3>.Value.IncQAdd(data);
            World<WorldType>.Components<C4>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAdd();
            World<WorldType>.Components<C2>.Value.DecQAdd();
            World<WorldType>.Components<C3>.Value.DecQAdd();
            World<WorldType>.Components<C4>.Value.DecQAdd();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAdd(val);
            World<WorldType>.Components<C2>.Value.BlockAdd(val);
            World<WorldType>.Components<C3>.Value.BlockAdd(val);
            World<WorldType>.Components<C4>.Value.BlockAdd(val);
        }
        #endif
    }


    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1, C2, C3, C4, C5> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C3>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C4>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C5>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAdd(data);
            World<WorldType>.Components<C2>.Value.IncQAdd(data);
            World<WorldType>.Components<C3>.Value.IncQAdd(data);
            World<WorldType>.Components<C4>.Value.IncQAdd(data);
            World<WorldType>.Components<C5>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAdd();
            World<WorldType>.Components<C2>.Value.DecQAdd();
            World<WorldType>.Components<C3>.Value.DecQAdd();
            World<WorldType>.Components<C4>.Value.DecQAdd();
            World<WorldType>.Components<C5>.Value.DecQAdd();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAdd(val);
            World<WorldType>.Components<C2>.Value.BlockAdd(val);
            World<WorldType>.Components<C3>.Value.BlockAdd(val);
            World<WorldType>.Components<C4>.Value.BlockAdd(val);
            World<WorldType>.Components<C5>.Value.BlockAdd(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1, C2, C3, C4, C5, C6> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C3>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C4>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C5>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C6>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C6>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAdd(data);
            World<WorldType>.Components<C2>.Value.IncQAdd(data);
            World<WorldType>.Components<C3>.Value.IncQAdd(data);
            World<WorldType>.Components<C4>.Value.IncQAdd(data);
            World<WorldType>.Components<C5>.Value.IncQAdd(data);
            World<WorldType>.Components<C6>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAdd();
            World<WorldType>.Components<C2>.Value.DecQAdd();
            World<WorldType>.Components<C3>.Value.DecQAdd();
            World<WorldType>.Components<C4>.Value.DecQAdd();
            World<WorldType>.Components<C5>.Value.DecQAdd();
            World<WorldType>.Components<C6>.Value.DecQAdd();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAdd(val);
            World<WorldType>.Components<C2>.Value.BlockAdd(val);
            World<WorldType>.Components<C3>.Value.BlockAdd(val);
            World<WorldType>.Components<C4>.Value.BlockAdd(val);
            World<WorldType>.Components<C5>.Value.BlockAdd(val);
            World<WorldType>.Components<C6>.Value.BlockAdd(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C3>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C4>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C5>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C6>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Components<C7>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C6>.Value.AMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Components<C7>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAdd(data);
            World<WorldType>.Components<C2>.Value.IncQAdd(data);
            World<WorldType>.Components<C3>.Value.IncQAdd(data);
            World<WorldType>.Components<C4>.Value.IncQAdd(data);
            World<WorldType>.Components<C5>.Value.IncQAdd(data);
            World<WorldType>.Components<C6>.Value.IncQAdd(data);
            World<WorldType>.Components<C7>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAdd();
            World<WorldType>.Components<C2>.Value.DecQAdd();
            World<WorldType>.Components<C3>.Value.DecQAdd();
            World<WorldType>.Components<C4>.Value.DecQAdd();
            World<WorldType>.Components<C5>.Value.DecQAdd();
            World<WorldType>.Components<C6>.Value.DecQAdd();
            World<WorldType>.Components<C7>.Value.DecQAdd();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAdd(val);
            World<WorldType>.Components<C2>.Value.BlockAdd(val);
            World<WorldType>.Components<C3>.Value.BlockAdd(val);
            World<WorldType>.Components<C4>.Value.BlockAdd(val);
            World<WorldType>.Components<C5>.Value.BlockAdd(val);
            World<WorldType>.Components<C6>.Value.BlockAdd(val);
            World<WorldType>.Components<C7>.Value.BlockAdd(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Components<C1>.Value.chunks[chunkIdx].fullBlocks
                           & ~World<WorldType>.Components<C2>.Value.chunks[chunkIdx].fullBlocks
                           & ~World<WorldType>.Components<C3>.Value.chunks[chunkIdx].fullBlocks
                           & ~World<WorldType>.Components<C4>.Value.chunks[chunkIdx].fullBlocks
                           & ~World<WorldType>.Components<C5>.Value.chunks[chunkIdx].fullBlocks
                           & ~World<WorldType>.Components<C6>.Value.chunks[chunkIdx].fullBlocks
                           & ~World<WorldType>.Components<C7>.Value.chunks[chunkIdx].fullBlocks
                           & ~World<WorldType>.Components<C8>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                              & ~World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                              & ~World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx)
                              & ~World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx)
                              & ~World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx)
                              & ~World<WorldType>.Components<C6>.Value.AMask(chunkIdx, blockIdx)
                              & ~World<WorldType>.Components<C7>.Value.AMask(chunkIdx, blockIdx)
                              & ~World<WorldType>.Components<C8>.Value.AMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.IncQAdd(data);
            World<WorldType>.Components<C2>.Value.IncQAdd(data);
            World<WorldType>.Components<C3>.Value.IncQAdd(data);
            World<WorldType>.Components<C4>.Value.IncQAdd(data);
            World<WorldType>.Components<C5>.Value.IncQAdd(data);
            World<WorldType>.Components<C6>.Value.IncQAdd(data);
            World<WorldType>.Components<C7>.Value.IncQAdd(data);
            World<WorldType>.Components<C8>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQAdd();
            World<WorldType>.Components<C2>.Value.DecQAdd();
            World<WorldType>.Components<C3>.Value.DecQAdd();
            World<WorldType>.Components<C4>.Value.DecQAdd();
            World<WorldType>.Components<C5>.Value.DecQAdd();
            World<WorldType>.Components<C6>.Value.DecQAdd();
            World<WorldType>.Components<C7>.Value.DecQAdd();
            World<WorldType>.Components<C8>.Value.DecQAdd();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockAdd(val);
            World<WorldType>.Components<C2>.Value.BlockAdd(val);
            World<WorldType>.Components<C3>.Value.BlockAdd(val);
            World<WorldType>.Components<C4>.Value.BlockAdd(val);
            World<WorldType>.Components<C5>.Value.BlockAdd(val);
            World<WorldType>.Components<C6>.Value.BlockAdd(val);
            World<WorldType>.Components<C7>.Value.BlockAdd(val);
            World<WorldType>.Components<C8>.Value.BlockAdd(val);
        }
        #endif
    }
    #endregion

    #region ANY
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx));
        }

        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteDisable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C2>.Value.DecQDeleteDisable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteDisable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteDisable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C2>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C3>.Value.DecQDeleteDisable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteDisable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3, C4> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteDisable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C2>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C3>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C4>.Value.DecQDeleteDisable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteDisable(val);
        }
        #endif
    }


    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3, C4, C5> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteDisable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C2>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C3>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C4>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C5>.Value.DecQDeleteDisable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteDisable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3, C4, C5, C6> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C6>.Value.EMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C6>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C6>.Value.IncQDeleteDisable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C2>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C3>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C4>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C5>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C6>.Value.DecQDeleteDisable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C6>.Value.BlockDeleteDisable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C7>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C6>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C7>.Value.EMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C6>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C7>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C6>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C7>.Value.IncQDeleteDisable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C2>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C3>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C4>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C5>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C6>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C7>.Value.DecQDeleteDisable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C6>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C7>.Value.BlockDeleteDisable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C7>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C8>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C6>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C7>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C8>.Value.EMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C4>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C5>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C6>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C7>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C8>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C6>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C7>.Value.IncQDeleteDisable(data);
            World<WorldType>.Components<C8>.Value.IncQDeleteDisable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C2>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C3>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C4>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C5>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C6>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C7>.Value.DecQDeleteDisable();
            World<WorldType>.Components<C8>.Value.DecQDeleteDisable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C6>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C7>.Value.BlockDeleteDisable(val);
            World<WorldType>.Components<C8>.Value.BlockDeleteDisable(val);
        }
        #endif
    }
    #endregion

    #region ANY_ONLY_DISABLED
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyOnlyDisabled<C1, C2> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C2>.Value.DecQDeleteEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyOnlyDisabled<C1, C2, C3> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C2>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C3>.Value.DecQDeleteEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyOnlyDisabled<C1, C2, C3, C4> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C4>.Value.DMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C4>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C2>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C3>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C4>.Value.DecQDeleteEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteEnable(val);
        }
        #endif
    }


    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyOnlyDisabled<C1, C2, C3, C4, C5> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C4>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C5>.Value.DMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C4>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C5>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C2>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C3>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C4>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C5>.Value.DecQDeleteEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyOnlyDisabled<C1, C2, C3, C4, C5, C6> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C4>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C5>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C6>.Value.DMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C4>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C5>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C6>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C6>.Value.IncQDeleteEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C2>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C3>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C4>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C5>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C6>.Value.DecQDeleteEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C6>.Value.BlockDeleteEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyOnlyDisabled<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C7>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C4>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C5>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C6>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C7>.Value.DMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C4>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C5>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C6>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C7>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C6>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C7>.Value.IncQDeleteEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C2>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C3>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C4>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C5>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C6>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C7>.Value.DecQDeleteEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C6>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C7>.Value.BlockDeleteEnable(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyOnlyDisabled<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C7>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C8>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C4>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C5>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C6>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C7>.Value.DMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C8>.Value.DMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C4>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C5>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C6>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C7>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C8>.Value.DMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C2>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C3>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C4>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C5>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C6>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C7>.Value.IncQDeleteEnable(data);
            World<WorldType>.Components<C8>.Value.IncQDeleteEnable(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C2>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C3>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C4>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C5>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C6>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C7>.Value.DecQDeleteEnable();
            World<WorldType>.Components<C8>.Value.DecQDeleteEnable();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C2>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C3>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C4>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C5>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C6>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C7>.Value.BlockDeleteEnable(val);
            World<WorldType>.Components<C8>.Value.BlockDeleteEnable(val);
        }
        #endif
    }
    #endregion

    #region ANY_WITH_DISABLED
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyWithDisabled<C1, C2> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDelete(data);
            World<WorldType>.Components<C2>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDelete();
            World<WorldType>.Components<C2>.Value.DecQDelete();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDelete(val);
            World<WorldType>.Components<C2>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyWithDisabled<C1, C2, C3> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDelete(data);
            World<WorldType>.Components<C2>.Value.IncQDelete(data);
            World<WorldType>.Components<C3>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDelete();
            World<WorldType>.Components<C2>.Value.DecQDelete();
            World<WorldType>.Components<C3>.Value.DecQDelete();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDelete(val);
            World<WorldType>.Components<C2>.Value.BlockDelete(val);
            World<WorldType>.Components<C3>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyWithDisabled<C1, C2, C3, C4> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDelete(data);
            World<WorldType>.Components<C2>.Value.IncQDelete(data);
            World<WorldType>.Components<C3>.Value.IncQDelete(data);
            World<WorldType>.Components<C4>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDelete();
            World<WorldType>.Components<C2>.Value.DecQDelete();
            World<WorldType>.Components<C3>.Value.DecQDelete();
            World<WorldType>.Components<C4>.Value.DecQDelete();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDelete(val);
            World<WorldType>.Components<C2>.Value.BlockDelete(val);
            World<WorldType>.Components<C3>.Value.BlockDelete(val);
            World<WorldType>.Components<C4>.Value.BlockDelete(val);
        }
        #endif
    }


    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyWithDisabled<C1, C2, C3, C4, C5> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDelete(data);
            World<WorldType>.Components<C2>.Value.IncQDelete(data);
            World<WorldType>.Components<C3>.Value.IncQDelete(data);
            World<WorldType>.Components<C4>.Value.IncQDelete(data);
            World<WorldType>.Components<C5>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDelete();
            World<WorldType>.Components<C2>.Value.DecQDelete();
            World<WorldType>.Components<C3>.Value.DecQDelete();
            World<WorldType>.Components<C4>.Value.DecQDelete();
            World<WorldType>.Components<C5>.Value.DecQDelete();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDelete(val);
            World<WorldType>.Components<C2>.Value.BlockDelete(val);
            World<WorldType>.Components<C3>.Value.BlockDelete(val);
            World<WorldType>.Components<C4>.Value.BlockDelete(val);
            World<WorldType>.Components<C5>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyWithDisabled<C1, C2, C3, C4, C5, C6> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C6>.Value.AMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C6>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDelete(data);
            World<WorldType>.Components<C2>.Value.IncQDelete(data);
            World<WorldType>.Components<C3>.Value.IncQDelete(data);
            World<WorldType>.Components<C4>.Value.IncQDelete(data);
            World<WorldType>.Components<C5>.Value.IncQDelete(data);
            World<WorldType>.Components<C6>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDelete();
            World<WorldType>.Components<C2>.Value.DecQDelete();
            World<WorldType>.Components<C3>.Value.DecQDelete();
            World<WorldType>.Components<C4>.Value.DecQDelete();
            World<WorldType>.Components<C5>.Value.DecQDelete();
            World<WorldType>.Components<C6>.Value.DecQDelete();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDelete(val);
            World<WorldType>.Components<C2>.Value.BlockDelete(val);
            World<WorldType>.Components<C3>.Value.BlockDelete(val);
            World<WorldType>.Components<C4>.Value.BlockDelete(val);
            World<WorldType>.Components<C5>.Value.BlockDelete(val);
            World<WorldType>.Components<C6>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyWithDisabled<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C7>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C6>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C7>.Value.AMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C6>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C7>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDelete(data);
            World<WorldType>.Components<C2>.Value.IncQDelete(data);
            World<WorldType>.Components<C3>.Value.IncQDelete(data);
            World<WorldType>.Components<C4>.Value.IncQDelete(data);
            World<WorldType>.Components<C5>.Value.IncQDelete(data);
            World<WorldType>.Components<C6>.Value.IncQDelete(data);
            World<WorldType>.Components<C7>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDelete();
            World<WorldType>.Components<C2>.Value.DecQDelete();
            World<WorldType>.Components<C3>.Value.DecQDelete();
            World<WorldType>.Components<C4>.Value.DecQDelete();
            World<WorldType>.Components<C5>.Value.DecQDelete();
            World<WorldType>.Components<C6>.Value.DecQDelete();
            World<WorldType>.Components<C7>.Value.DecQDelete();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDelete(val);
            World<WorldType>.Components<C2>.Value.BlockDelete(val);
            World<WorldType>.Components<C3>.Value.BlockDelete(val);
            World<WorldType>.Components<C4>.Value.BlockDelete(val);
            World<WorldType>.Components<C5>.Value.BlockDelete(val);
            World<WorldType>.Components<C6>.Value.BlockDelete(val);
            World<WorldType>.Components<C7>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyWithDisabled<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Components<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C7>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Components<C8>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C6>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C7>.Value.AMask(chunkIdx, blockIdx)
                             | World<WorldType>.Components<C8>.Value.AMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Components<C1>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Components<C2>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C3>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C4>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C5>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C6>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C7>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Components<C8>.Value.AMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Components<C1>.Value.IncQDelete(data);
            World<WorldType>.Components<C2>.Value.IncQDelete(data);
            World<WorldType>.Components<C3>.Value.IncQDelete(data);
            World<WorldType>.Components<C4>.Value.IncQDelete(data);
            World<WorldType>.Components<C5>.Value.IncQDelete(data);
            World<WorldType>.Components<C6>.Value.IncQDelete(data);
            World<WorldType>.Components<C7>.Value.IncQDelete(data);
            World<WorldType>.Components<C8>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.DecQDelete();
            World<WorldType>.Components<C2>.Value.DecQDelete();
            World<WorldType>.Components<C3>.Value.DecQDelete();
            World<WorldType>.Components<C4>.Value.DecQDelete();
            World<WorldType>.Components<C5>.Value.DecQDelete();
            World<WorldType>.Components<C6>.Value.DecQDelete();
            World<WorldType>.Components<C7>.Value.DecQDelete();
            World<WorldType>.Components<C8>.Value.DecQDelete();
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.BlockDelete(val);
            World<WorldType>.Components<C2>.Value.BlockDelete(val);
            World<WorldType>.Components<C3>.Value.BlockDelete(val);
            World<WorldType>.Components<C4>.Value.BlockDelete(val);
            World<WorldType>.Components<C5>.Value.BlockDelete(val);
            World<WorldType>.Components<C6>.Value.BlockDelete(val);
            World<WorldType>.Components<C7>.Value.BlockDelete(val);
            World<WorldType>.Components<C8>.Value.BlockDelete(val);
        }
        #endif
    }
    #endregion
}