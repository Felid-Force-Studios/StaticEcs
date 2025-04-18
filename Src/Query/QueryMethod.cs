﻿using System.Runtime.CompilerServices;

namespace FFS.Libraries.StaticEcs {
    
    public interface IQueryMethod {
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType;
        
        public bool CheckEntity(uint entityId);

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        public void Dispose<WorldType>() where WorldType : struct, IWorldType;
        #endif
    }
    
    public static class Extension {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckOne<WorldType>(this IQueryMethod method, World<WorldType>.Entity entity) where WorldType : struct, IWorldType {
            var count = uint.MaxValue;
            uint[] entities = null;
            method.SetData<WorldType>(ref count, ref entities);
            var checkEntity = method.CheckEntity(entity._id);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            method.Dispose<WorldType>();
            #endif
            return checkEntity;
        }
    }
    
    public interface IPrimaryQueryMethod : IQueryMethod { }
    
    public interface ISealedQueryMethod : IQueryMethod { }

    public enum EntityStatusType : byte {
        Enabled = 0,
        Disabled = 1,
        Any = 2,
    }
}