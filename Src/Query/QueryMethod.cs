namespace FFS.Libraries.StaticEcs {
    
    public interface IQueryMethod {
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType;
        
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType;
        
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType;
        
        public void DecQ<WorldType>() where WorldType : struct, IWorldType;
        
        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType;
        #endif
    }

    public enum EntityStatusType : byte {
        Enabled = 0,
        Disabled = 1,
        Any = 2,
    }

    public enum ComponentStatus : byte {
        Enabled = 0,
        Any = 1,
        Disabled = 2,
    }
}