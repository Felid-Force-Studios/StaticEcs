#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

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
    public struct TagAll<C1> : IQueryMethod
        where C1 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAll<C1, C2> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQDelete(data);
            World<WorldType>.Tags<C2>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQDelete();
            World<WorldType>.Tags<C2>.Value.DecQDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockDelete(val);
            World<WorldType>.Tags<C2>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAll<C1, C2, C3> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQDelete(data);
            World<WorldType>.Tags<C2>.Value.IncQDelete(data);
            World<WorldType>.Tags<C3>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQDelete();
            World<WorldType>.Tags<C2>.Value.DecQDelete();
            World<WorldType>.Tags<C3>.Value.DecQDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockDelete(val);
            World<WorldType>.Tags<C2>.Value.BlockDelete(val);
            World<WorldType>.Tags<C3>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAll<C1, C2, C3, C4> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C4>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQDelete(data);
            World<WorldType>.Tags<C2>.Value.IncQDelete(data);
            World<WorldType>.Tags<C3>.Value.IncQDelete(data);
            World<WorldType>.Tags<C4>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQDelete();
            World<WorldType>.Tags<C2>.Value.DecQDelete();
            World<WorldType>.Tags<C3>.Value.DecQDelete();
            World<WorldType>.Tags<C4>.Value.DecQDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C4>(World<WorldType>.Tags<C4>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockDelete(val);
            World<WorldType>.Tags<C2>.Value.BlockDelete(val);
            World<WorldType>.Tags<C3>.Value.BlockDelete(val);
            World<WorldType>.Tags<C4>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAll<C1, C2, C3, C4, C5> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C5>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQDelete(data);
            World<WorldType>.Tags<C2>.Value.IncQDelete(data);
            World<WorldType>.Tags<C3>.Value.IncQDelete(data);
            World<WorldType>.Tags<C4>.Value.IncQDelete(data);
            World<WorldType>.Tags<C5>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQDelete();
            World<WorldType>.Tags<C2>.Value.DecQDelete();
            World<WorldType>.Tags<C3>.Value.DecQDelete();
            World<WorldType>.Tags<C4>.Value.DecQDelete();
            World<WorldType>.Tags<C5>.Value.DecQDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C4>(World<WorldType>.Tags<C4>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C5>(World<WorldType>.Tags<C5>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockDelete(val);
            World<WorldType>.Tags<C2>.Value.BlockDelete(val);
            World<WorldType>.Tags<C3>.Value.BlockDelete(val);
            World<WorldType>.Tags<C4>.Value.BlockDelete(val);
            World<WorldType>.Tags<C5>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAll<C1, C2, C3, C4, C5, C6> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C6>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C6>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQDelete(data);
            World<WorldType>.Tags<C2>.Value.IncQDelete(data);
            World<WorldType>.Tags<C3>.Value.IncQDelete(data);
            World<WorldType>.Tags<C4>.Value.IncQDelete(data);
            World<WorldType>.Tags<C5>.Value.IncQDelete(data);
            World<WorldType>.Tags<C6>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQDelete();
            World<WorldType>.Tags<C2>.Value.DecQDelete();
            World<WorldType>.Tags<C3>.Value.DecQDelete();
            World<WorldType>.Tags<C4>.Value.DecQDelete();
            World<WorldType>.Tags<C5>.Value.DecQDelete();
            World<WorldType>.Tags<C6>.Value.DecQDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C4>(World<WorldType>.Tags<C4>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C5>(World<WorldType>.Tags<C5>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C6>(World<WorldType>.Tags<C6>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockDelete(val);
            World<WorldType>.Tags<C2>.Value.BlockDelete(val);
            World<WorldType>.Tags<C3>.Value.BlockDelete(val);
            World<WorldType>.Tags<C4>.Value.BlockDelete(val);
            World<WorldType>.Tags<C5>.Value.BlockDelete(val);
            World<WorldType>.Tags<C6>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAll<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C7>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C6>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C7>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQDelete(data);
            World<WorldType>.Tags<C2>.Value.IncQDelete(data);
            World<WorldType>.Tags<C3>.Value.IncQDelete(data);
            World<WorldType>.Tags<C4>.Value.IncQDelete(data);
            World<WorldType>.Tags<C5>.Value.IncQDelete(data);
            World<WorldType>.Tags<C6>.Value.IncQDelete(data);
            World<WorldType>.Tags<C7>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQDelete();
            World<WorldType>.Tags<C2>.Value.DecQDelete();
            World<WorldType>.Tags<C3>.Value.DecQDelete();
            World<WorldType>.Tags<C4>.Value.DecQDelete();
            World<WorldType>.Tags<C5>.Value.DecQDelete();
            World<WorldType>.Tags<C6>.Value.DecQDelete();
            World<WorldType>.Tags<C7>.Value.DecQDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C4>(World<WorldType>.Tags<C4>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C5>(World<WorldType>.Tags<C5>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C6>(World<WorldType>.Tags<C6>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C7>(World<WorldType>.Tags<C7>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockDelete(val);
            World<WorldType>.Tags<C2>.Value.BlockDelete(val);
            World<WorldType>.Tags<C3>.Value.BlockDelete(val);
            World<WorldType>.Tags<C4>.Value.BlockDelete(val);
            World<WorldType>.Tags<C5>.Value.BlockDelete(val);
            World<WorldType>.Tags<C6>.Value.BlockDelete(val);
            World<WorldType>.Tags<C7>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAll<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag
        where C8 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C7>.Value.chunks[chunkIdx].notEmptyBlocks
                         & World<WorldType>.Tags<C8>.Value.chunks[chunkIdx].notEmptyBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C6>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C7>.Value.EMask(chunkIdx, blockIdx)
                            & World<WorldType>.Tags<C8>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQDelete(data);
            World<WorldType>.Tags<C2>.Value.IncQDelete(data);
            World<WorldType>.Tags<C3>.Value.IncQDelete(data);
            World<WorldType>.Tags<C4>.Value.IncQDelete(data);
            World<WorldType>.Tags<C5>.Value.IncQDelete(data);
            World<WorldType>.Tags<C6>.Value.IncQDelete(data);
            World<WorldType>.Tags<C7>.Value.IncQDelete(data);
            World<WorldType>.Tags<C8>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQDelete();
            World<WorldType>.Tags<C2>.Value.DecQDelete();
            World<WorldType>.Tags<C3>.Value.DecQDelete();
            World<WorldType>.Tags<C4>.Value.DecQDelete();
            World<WorldType>.Tags<C5>.Value.DecQDelete();
            World<WorldType>.Tags<C6>.Value.DecQDelete();
            World<WorldType>.Tags<C7>.Value.DecQDelete();
            World<WorldType>.Tags<C8>.Value.DecQDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C4>(World<WorldType>.Tags<C4>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C5>(World<WorldType>.Tags<C5>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C6>(World<WorldType>.Tags<C6>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C7>(World<WorldType>.Tags<C7>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C8>(World<WorldType>.Tags<C8>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockDelete(val);
            World<WorldType>.Tags<C2>.Value.BlockDelete(val);
            World<WorldType>.Tags<C3>.Value.BlockDelete(val);
            World<WorldType>.Tags<C4>.Value.BlockDelete(val);
            World<WorldType>.Tags<C5>.Value.BlockDelete(val);
            World<WorldType>.Tags<C6>.Value.BlockDelete(val);
            World<WorldType>.Tags<C7>.Value.BlockDelete(val);
            World<WorldType>.Tags<C8>.Value.BlockDelete(val);
        }
        #endif
    }
    #endregion

    #region NONE
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1> : IQueryMethod
        where C1 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockAdd(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1, C2> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQAdd(data);
            World<WorldType>.Tags<C2>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQAdd();
            World<WorldType>.Tags<C2>.Value.DecQAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockAdd(val);
            World<WorldType>.Tags<C2>.Value.BlockAdd(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1, C2, C3> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQAdd(data);
            World<WorldType>.Tags<C2>.Value.IncQAdd(data);
            World<WorldType>.Tags<C3>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQAdd();
            World<WorldType>.Tags<C2>.Value.DecQAdd();
            World<WorldType>.Tags<C3>.Value.DecQAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockAdd(val);
            World<WorldType>.Tags<C2>.Value.BlockAdd(val);
            World<WorldType>.Tags<C3>.Value.BlockAdd(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1, C2, C3, C4> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C4>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQAdd(data);
            World<WorldType>.Tags<C2>.Value.IncQAdd(data);
            World<WorldType>.Tags<C3>.Value.IncQAdd(data);
            World<WorldType>.Tags<C4>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQAdd();
            World<WorldType>.Tags<C2>.Value.DecQAdd();
            World<WorldType>.Tags<C3>.Value.DecQAdd();
            World<WorldType>.Tags<C4>.Value.DecQAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C4>(World<WorldType>.Tags<C4>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockAdd(val);
            World<WorldType>.Tags<C2>.Value.BlockAdd(val);
            World<WorldType>.Tags<C3>.Value.BlockAdd(val);
            World<WorldType>.Tags<C4>.Value.BlockAdd(val);
        }
        #endif
    }


    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1, C2, C3, C4, C5> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C4>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C5>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQAdd(data);
            World<WorldType>.Tags<C2>.Value.IncQAdd(data);
            World<WorldType>.Tags<C3>.Value.IncQAdd(data);
            World<WorldType>.Tags<C4>.Value.IncQAdd(data);
            World<WorldType>.Tags<C5>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQAdd();
            World<WorldType>.Tags<C2>.Value.DecQAdd();
            World<WorldType>.Tags<C3>.Value.DecQAdd();
            World<WorldType>.Tags<C4>.Value.DecQAdd();
            World<WorldType>.Tags<C5>.Value.DecQAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C4>(World<WorldType>.Tags<C4>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C5>(World<WorldType>.Tags<C5>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockAdd(val);
            World<WorldType>.Tags<C2>.Value.BlockAdd(val);
            World<WorldType>.Tags<C3>.Value.BlockAdd(val);
            World<WorldType>.Tags<C4>.Value.BlockAdd(val);
            World<WorldType>.Tags<C5>.Value.BlockAdd(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1, C2, C3, C4, C5, C6> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C4>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C5>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C6>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C6>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQAdd(data);
            World<WorldType>.Tags<C2>.Value.IncQAdd(data);
            World<WorldType>.Tags<C3>.Value.IncQAdd(data);
            World<WorldType>.Tags<C4>.Value.IncQAdd(data);
            World<WorldType>.Tags<C5>.Value.IncQAdd(data);
            World<WorldType>.Tags<C6>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQAdd();
            World<WorldType>.Tags<C2>.Value.DecQAdd();
            World<WorldType>.Tags<C3>.Value.DecQAdd();
            World<WorldType>.Tags<C4>.Value.DecQAdd();
            World<WorldType>.Tags<C5>.Value.DecQAdd();
            World<WorldType>.Tags<C6>.Value.DecQAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C4>(World<WorldType>.Tags<C4>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C5>(World<WorldType>.Tags<C5>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C6>(World<WorldType>.Tags<C6>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockAdd(val);
            World<WorldType>.Tags<C2>.Value.BlockAdd(val);
            World<WorldType>.Tags<C3>.Value.BlockAdd(val);
            World<WorldType>.Tags<C4>.Value.BlockAdd(val);
            World<WorldType>.Tags<C5>.Value.BlockAdd(val);
            World<WorldType>.Tags<C6>.Value.BlockAdd(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C4>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C5>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C6>.Value.chunks[chunkIdx].fullBlocks
                         & ~World<WorldType>.Tags<C7>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C6>.Value.EMask(chunkIdx, blockIdx)
                            & ~World<WorldType>.Tags<C7>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQAdd(data);
            World<WorldType>.Tags<C2>.Value.IncQAdd(data);
            World<WorldType>.Tags<C3>.Value.IncQAdd(data);
            World<WorldType>.Tags<C4>.Value.IncQAdd(data);
            World<WorldType>.Tags<C5>.Value.IncQAdd(data);
            World<WorldType>.Tags<C6>.Value.IncQAdd(data);
            World<WorldType>.Tags<C7>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQAdd();
            World<WorldType>.Tags<C2>.Value.DecQAdd();
            World<WorldType>.Tags<C3>.Value.DecQAdd();
            World<WorldType>.Tags<C4>.Value.DecQAdd();
            World<WorldType>.Tags<C5>.Value.DecQAdd();
            World<WorldType>.Tags<C6>.Value.DecQAdd();
            World<WorldType>.Tags<C7>.Value.DecQAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C4>(World<WorldType>.Tags<C4>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C5>(World<WorldType>.Tags<C5>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C6>(World<WorldType>.Tags<C6>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C7>(World<WorldType>.Tags<C7>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockAdd(val);
            World<WorldType>.Tags<C2>.Value.BlockAdd(val);
            World<WorldType>.Tags<C3>.Value.BlockAdd(val);
            World<WorldType>.Tags<C4>.Value.BlockAdd(val);
            World<WorldType>.Tags<C5>.Value.BlockAdd(val);
            World<WorldType>.Tags<C6>.Value.BlockAdd(val);
            World<WorldType>.Tags<C7>.Value.BlockAdd(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag
        where C8 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= ~World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].fullBlocks
                           & ~World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].fullBlocks
                           & ~World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].fullBlocks
                           & ~World<WorldType>.Tags<C4>.Value.chunks[chunkIdx].fullBlocks
                           & ~World<WorldType>.Tags<C5>.Value.chunks[chunkIdx].fullBlocks
                           & ~World<WorldType>.Tags<C6>.Value.chunks[chunkIdx].fullBlocks
                           & ~World<WorldType>.Tags<C7>.Value.chunks[chunkIdx].fullBlocks
                           & ~World<WorldType>.Tags<C8>.Value.chunks[chunkIdx].fullBlocks;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= ~World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                              & ~World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                              & ~World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx)
                              & ~World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx)
                              & ~World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx)
                              & ~World<WorldType>.Tags<C6>.Value.EMask(chunkIdx, blockIdx)
                              & ~World<WorldType>.Tags<C7>.Value.EMask(chunkIdx, blockIdx)
                              & ~World<WorldType>.Tags<C8>.Value.EMask(chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.IncQAdd(data);
            World<WorldType>.Tags<C2>.Value.IncQAdd(data);
            World<WorldType>.Tags<C3>.Value.IncQAdd(data);
            World<WorldType>.Tags<C4>.Value.IncQAdd(data);
            World<WorldType>.Tags<C5>.Value.IncQAdd(data);
            World<WorldType>.Tags<C6>.Value.IncQAdd(data);
            World<WorldType>.Tags<C7>.Value.IncQAdd(data);
            World<WorldType>.Tags<C8>.Value.IncQAdd(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQAdd();
            World<WorldType>.Tags<C2>.Value.DecQAdd();
            World<WorldType>.Tags<C3>.Value.DecQAdd();
            World<WorldType>.Tags<C4>.Value.DecQAdd();
            World<WorldType>.Tags<C5>.Value.DecQAdd();
            World<WorldType>.Tags<C6>.Value.DecQAdd();
            World<WorldType>.Tags<C7>.Value.DecQAdd();
            World<WorldType>.Tags<C8>.Value.DecQAdd();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C4>(World<WorldType>.Tags<C4>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C5>(World<WorldType>.Tags<C5>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C6>(World<WorldType>.Tags<C6>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C7>(World<WorldType>.Tags<C7>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C8>(World<WorldType>.Tags<C8>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockAdd(val);
            World<WorldType>.Tags<C2>.Value.BlockAdd(val);
            World<WorldType>.Tags<C3>.Value.BlockAdd(val);
            World<WorldType>.Tags<C4>.Value.BlockAdd(val);
            World<WorldType>.Tags<C5>.Value.BlockAdd(val);
            World<WorldType>.Tags<C6>.Value.BlockAdd(val);
            World<WorldType>.Tags<C7>.Value.BlockAdd(val);
            World<WorldType>.Tags<C8>.Value.BlockAdd(val);
        }
        #endif
    }
    #endregion

    #region ANY
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAny<C1, C2> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx));
        }

        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Tags<C1>.Value.IncQDelete(data);
            World<WorldType>.Tags<C2>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQDelete();
            World<WorldType>.Tags<C2>.Value.DecQDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockDelete(val);
            World<WorldType>.Tags<C2>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAny<C1, C2, C3> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Tags<C1>.Value.IncQDelete(data);
            World<WorldType>.Tags<C2>.Value.IncQDelete(data);
            World<WorldType>.Tags<C3>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQDelete();
            World<WorldType>.Tags<C2>.Value.DecQDelete();
            World<WorldType>.Tags<C3>.Value.DecQDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockDelete(val);
            World<WorldType>.Tags<C2>.Value.BlockDelete(val);
            World<WorldType>.Tags<C3>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAny<C1, C2, C3, C4> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C4>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Tags<C1>.Value.IncQDelete(data);
            World<WorldType>.Tags<C2>.Value.IncQDelete(data);
            World<WorldType>.Tags<C3>.Value.IncQDelete(data);
            World<WorldType>.Tags<C4>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQDelete();
            World<WorldType>.Tags<C2>.Value.DecQDelete();
            World<WorldType>.Tags<C3>.Value.DecQDelete();
            World<WorldType>.Tags<C4>.Value.DecQDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C4>(World<WorldType>.Tags<C4>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockDelete(val);
            World<WorldType>.Tags<C2>.Value.BlockDelete(val);
            World<WorldType>.Tags<C3>.Value.BlockDelete(val);
            World<WorldType>.Tags<C4>.Value.BlockDelete(val);
        }
        #endif
    }


    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAny<C1, C2, C3, C4, C5> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C5>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Tags<C1>.Value.IncQDelete(data);
            World<WorldType>.Tags<C2>.Value.IncQDelete(data);
            World<WorldType>.Tags<C3>.Value.IncQDelete(data);
            World<WorldType>.Tags<C4>.Value.IncQDelete(data);
            World<WorldType>.Tags<C5>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQDelete();
            World<WorldType>.Tags<C2>.Value.DecQDelete();
            World<WorldType>.Tags<C3>.Value.DecQDelete();
            World<WorldType>.Tags<C4>.Value.DecQDelete();
            World<WorldType>.Tags<C5>.Value.DecQDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C4>(World<WorldType>.Tags<C4>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C5>(World<WorldType>.Tags<C5>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockDelete(val);
            World<WorldType>.Tags<C2>.Value.BlockDelete(val);
            World<WorldType>.Tags<C3>.Value.BlockDelete(val);
            World<WorldType>.Tags<C4>.Value.BlockDelete(val);
            World<WorldType>.Tags<C5>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAny<C1, C2, C3, C4, C5, C6> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C6>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C6>.Value.EMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C6>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Tags<C1>.Value.IncQDelete(data);
            World<WorldType>.Tags<C2>.Value.IncQDelete(data);
            World<WorldType>.Tags<C3>.Value.IncQDelete(data);
            World<WorldType>.Tags<C4>.Value.IncQDelete(data);
            World<WorldType>.Tags<C5>.Value.IncQDelete(data);
            World<WorldType>.Tags<C6>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQDelete();
            World<WorldType>.Tags<C2>.Value.DecQDelete();
            World<WorldType>.Tags<C3>.Value.DecQDelete();
            World<WorldType>.Tags<C4>.Value.DecQDelete();
            World<WorldType>.Tags<C5>.Value.DecQDelete();
            World<WorldType>.Tags<C6>.Value.DecQDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C4>(World<WorldType>.Tags<C4>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C5>(World<WorldType>.Tags<C5>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C6>(World<WorldType>.Tags<C6>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockDelete(val);
            World<WorldType>.Tags<C2>.Value.BlockDelete(val);
            World<WorldType>.Tags<C3>.Value.BlockDelete(val);
            World<WorldType>.Tags<C4>.Value.BlockDelete(val);
            World<WorldType>.Tags<C5>.Value.BlockDelete(val);
            World<WorldType>.Tags<C6>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAny<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C7>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C6>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C7>.Value.EMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C6>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C7>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Tags<C1>.Value.IncQDelete(data);
            World<WorldType>.Tags<C2>.Value.IncQDelete(data);
            World<WorldType>.Tags<C3>.Value.IncQDelete(data);
            World<WorldType>.Tags<C4>.Value.IncQDelete(data);
            World<WorldType>.Tags<C5>.Value.IncQDelete(data);
            World<WorldType>.Tags<C6>.Value.IncQDelete(data);
            World<WorldType>.Tags<C7>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQDelete();
            World<WorldType>.Tags<C2>.Value.DecQDelete();
            World<WorldType>.Tags<C3>.Value.DecQDelete();
            World<WorldType>.Tags<C4>.Value.DecQDelete();
            World<WorldType>.Tags<C5>.Value.DecQDelete();
            World<WorldType>.Tags<C6>.Value.DecQDelete();
            World<WorldType>.Tags<C7>.Value.DecQDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C4>(World<WorldType>.Tags<C4>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C5>(World<WorldType>.Tags<C5>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C6>(World<WorldType>.Tags<C6>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C7>(World<WorldType>.Tags<C7>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockDelete(val);
            World<WorldType>.Tags<C2>.Value.BlockDelete(val);
            World<WorldType>.Tags<C3>.Value.BlockDelete(val);
            World<WorldType>.Tags<C4>.Value.BlockDelete(val);
            World<WorldType>.Tags<C5>.Value.BlockDelete(val);
            World<WorldType>.Tags<C6>.Value.BlockDelete(val);
            World<WorldType>.Tags<C7>.Value.BlockDelete(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAny<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag
        where C8 : struct, ITag {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            chunkMask &= (World<WorldType>.Tags<C1>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C2>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C3>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C4>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C5>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C6>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C7>.Value.chunks[chunkIdx].notEmptyBlocks
                          | World<WorldType>.Tags<C8>.Value.chunks[chunkIdx].notEmptyBlocks);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            entitiesMask &= (World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C6>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C7>.Value.EMask(chunkIdx, blockIdx)
                             | World<WorldType>.Tags<C8>.Value.EMask(chunkIdx, blockIdx));
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool CheckEntity<WorldType>(uint chunkIdx, int blockIdx, int blockEntityIdx) where WorldType : struct, IWorldType {
            var eMask = 1UL << blockEntityIdx;
            return (World<WorldType>.Tags<C1>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0 
                   || (World<WorldType>.Tags<C2>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C3>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C4>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C5>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C6>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C7>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   || (World<WorldType>.Tags<C8>.Value.EMask(chunkIdx, blockIdx) & eMask) != 0
                   ;
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            data.OnCacheUpdate = static (cache, chunkIdx, blockIdx, blockEntityIdx) => {
                if (!CheckEntity<WorldType>(chunkIdx, blockIdx, blockEntityIdx)) {
                    cache[(chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT ) + blockIdx].EntitiesMask &= ~(1UL << blockEntityIdx);
                }
            };
            World<WorldType>.Tags<C1>.Value.IncQDelete(data);
            World<WorldType>.Tags<C2>.Value.IncQDelete(data);
            World<WorldType>.Tags<C3>.Value.IncQDelete(data);
            World<WorldType>.Tags<C4>.Value.IncQDelete(data);
            World<WorldType>.Tags<C5>.Value.IncQDelete(data);
            World<WorldType>.Tags<C6>.Value.IncQDelete(data);
            World<WorldType>.Tags<C7>.Value.IncQDelete(data);
            World<WorldType>.Tags<C8>.Value.IncQDelete(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.DecQDelete();
            World<WorldType>.Tags<C2>.Value.DecQDelete();
            World<WorldType>.Tags<C3>.Value.DecQDelete();
            World<WorldType>.Tags<C4>.Value.DecQDelete();
            World<WorldType>.Tags<C5>.Value.DecQDelete();
            World<WorldType>.Tags<C6>.Value.DecQDelete();
            World<WorldType>.Tags<C7>.Value.DecQDelete();
            World<WorldType>.Tags<C8>.Value.DecQDelete();
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.AssertRegisteredTag<C1>(World<WorldType>.Tags<C1>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C2>(World<WorldType>.Tags<C2>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C3>(World<WorldType>.Tags<C3>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C4>(World<WorldType>.Tags<C4>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C5>(World<WorldType>.Tags<C5>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C6>(World<WorldType>.Tags<C6>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C7>(World<WorldType>.Tags<C7>.TagsTypeName);
            World<WorldType>.AssertRegisteredTag<C8>(World<WorldType>.Tags<C8>.TagsTypeName);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Tags<C1>.Value.BlockDelete(val);
            World<WorldType>.Tags<C2>.Value.BlockDelete(val);
            World<WorldType>.Tags<C3>.Value.BlockDelete(val);
            World<WorldType>.Tags<C4>.Value.BlockDelete(val);
            World<WorldType>.Tags<C5>.Value.BlockDelete(val);
            World<WorldType>.Tags<C6>.Value.BlockDelete(val);
            World<WorldType>.Tags<C7>.Value.BlockDelete(val);
            World<WorldType>.Tags<C8>.Value.BlockDelete(val);
        }
        #endif
    }
    #endregion
}
