using System.Runtime.CompilerServices;

namespace FFS.Libraries.StaticEcs {
    
    public interface IQueryMethod {
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId;
        
        public bool CheckEntity(int entityId);

        public void Dispose<WorldID>() where WorldID : struct, IWorldId;
    }
    
    public static class Extension {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckOne<WorldID>(this IQueryMethod method, Ecs<WorldID>.Entity entity) where WorldID : struct, IWorldId {
            var count = 0;
            int[] entities = null;
            method.SetData<WorldID>(ref count, ref entities);
            var checkEntity = method.CheckEntity(entity._id);
            method.Dispose<WorldID>();
            return checkEntity;
        }
    }
    
    public interface IPrimaryQueryMethod : IQueryMethod { }
    
    public interface ISealedQueryMethod : IQueryMethod { }
}