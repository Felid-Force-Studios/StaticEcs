using System.Runtime.CompilerServices;

namespace FFS.Libraries.StaticEcs {
    
    public interface IQueryMethod {
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType;
        
        public bool CheckEntity(int entityId);

        public void Dispose<WorldType>() where WorldType : struct, IWorldType;
    }
    
    public static class Extension {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckOne<WorldType>(this IQueryMethod method, Ecs<WorldType>.Entity entity) where WorldType : struct, IWorldType {
            var count = int.MaxValue;
            int[] entities = null;
            method.SetData<WorldType>(ref count, ref entities);
            var checkEntity = method.CheckEntity(entity._id);
            method.Dispose<WorldType>();
            return checkEntity;
        }
    }
    
    public interface IPrimaryQueryMethod : IQueryMethod { }
    
    public interface ISealedQueryMethod : IQueryMethod { }
}