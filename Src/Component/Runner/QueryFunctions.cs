#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        #region ABSTRACT_QUERY_FUNCTION
        public interface IQueryFunction<C1>
            where C1 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1);
        }

        public interface IQueryFunction<C1, C2>
            where C1 : struct, IComponent
            where C2 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1, ref C2 c2);
        }

        public interface IQueryFunction<C1, C2, C3>
            where C1 : struct, IComponent
            where C2 : struct, IComponent
            where C3 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1, ref C2 c2, ref C3 c3);
        }

        public interface IQueryFunction<C1, C2, C3, C4>
            where C1 : struct, IComponent
            where C2 : struct, IComponent
            where C3 : struct, IComponent
            where C4 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4);
        }

        public interface IQueryFunction<C1, C2, C3, C4, C5>
            where C1 : struct, IComponent
            where C2 : struct, IComponent
            where C3 : struct, IComponent
            where C4 : struct, IComponent
            where C5 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5);
        }

        public interface IQueryFunction<C1, C2, C3, C4, C5, C6>
            where C1 : struct, IComponent
            where C2 : struct, IComponent
            where C3 : struct, IComponent
            where C4 : struct, IComponent
            where C5 : struct, IComponent
            where C6 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6);
        }

        public interface IQueryFunction<C1, C2, C3, C4, C5, C6, C7>
            where C1 : struct, IComponent
            where C2 : struct, IComponent
            where C3 : struct, IComponent
            where C4 : struct, IComponent
            where C5 : struct, IComponent
            where C6 : struct, IComponent
            where C7 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7);
        }

        public interface IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8>
            where C1 : struct, IComponent
            where C2 : struct, IComponent
            where C3 : struct, IComponent
            where C4 : struct, IComponent
            where C5 : struct, IComponent
            where C6 : struct, IComponent
            where C7 : struct, IComponent
            where C8 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7, ref C8 c8);
        }
        #endregion
    }

    #region QUERY_FUNCTION
    public delegate void QueryFunction<C1>(ref C1 c1)
        where C1 : struct, IComponent;

    public delegate void QueryFunction<C1, C2>(ref C1 c1, ref C2 c2)
        where C1 : struct, IComponent
        where C2 : struct, IComponent;

    public delegate void QueryFunction<C1, C2, C3>(ref C1 c1, ref C2 c2, ref C3 c3)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent;

    public delegate void QueryFunction<C1, C2, C3, C4>(ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent;

    public delegate void QueryFunction<C1, C2, C3, C4, C5>(ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent;

    public delegate void QueryFunction<C1, C2, C3, C4, C5, C6>(ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent;

    public delegate void QueryFunction<C1, C2, C3, C4, C5, C6, C7>(ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent;

    public delegate void QueryFunction<C1, C2, C3, C4, C5, C6, C7, C8>(ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7, ref C8 c8)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent;
    #endregion
    
    #region QUERY_FUNCTION_WITH_DATA
    public delegate void QueryFunctionWithData<D, C1>(D data, ref C1 c1)
        where C1 : struct, IComponent;

    public delegate void QueryFunctionWithData<D, C1, C2>(D data, ref C1 c1, ref C2 c2)
        where C1 : struct, IComponent
        where C2 : struct, IComponent;

    public delegate void QueryFunctionWithData<D, C1, C2, C3>(D data, ref C1 c1, ref C2 c2, ref C3 c3)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent;

    public delegate void QueryFunctionWithData<D, C1, C2, C3, C4>(D data, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent;

    public delegate void QueryFunctionWithData<D, C1, C2, C3, C4, C5>(D data, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent;

    public delegate void QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6>(D data, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent;

    public delegate void QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6, C7>(D data, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent;

    public delegate void QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6, C7, C8>(D data, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7, ref C8 c8)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent;
    #endregion
    
    #region QUERY_FUNCTION_WITH_REF_DATA
    public delegate void QueryFunctionWithRefData<D, C1>(ref D data, ref C1 c1)
        where D : struct
        where C1 : struct, IComponent;

    public delegate void QueryFunctionWithRefData<D, C1, C2>(ref D data, ref C1 c1, ref C2 c2)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent;

    public delegate void QueryFunctionWithRefData<D, C1, C2, C3>(ref D data, ref C1 c1, ref C2 c2, ref C3 c3)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent;

    public delegate void QueryFunctionWithRefData<D, C1, C2, C3, C4>(ref D data, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent;

    public delegate void QueryFunctionWithRefData<D, C1, C2, C3, C4, C5>(ref D data, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent;

    public delegate void QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6>(ref D data, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent;

    public delegate void QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7>(ref D data, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent;

    public delegate void QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8>(ref D data, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7, ref C8 c8)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent;
    #endregion

    #region QUERY_FUNCTION_WITH_ENTITY
    public delegate void QueryFunctionWithEntity<WorldType, C1>(World<WorldType>.Entity entity, ref C1 c1)
        where C1 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithEntity<WorldType, C1, C2>(World<WorldType>.Entity entity, ref C1 c1, ref C2 c2)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithEntity<WorldType, C1, C2, C3>(World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithEntity<WorldType, C1, C2, C3, C4>(World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5>(World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6>(World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7>(World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7, C8>(World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7, ref C8 c8)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent
        where WorldType : struct, IWorldType;
    #endregion

    #region QUERY_FUNCTION_WITH_DATA_ENTITY
    public delegate void QueryFunctionWithDataEntity<D, WorldType, C1>(D data, World<WorldType>.Entity entity, ref C1 c1)
        where C1 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithDataEntity<D, WorldType, C1, C2>(D data, World<WorldType>.Entity entity, ref C1 c1, ref C2 c2)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3>(D data, World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4>(D data, World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5>(D data, World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6>(D data, World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7>(D data, World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8>(D data, World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7, ref C8 c8)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent
        where WorldType : struct, IWorldType;
    #endregion

    #region QUERY_FUNCTION_WITH_REF_DATA_ENTITY
    public delegate void QueryFunctionWithRefDataEntity<D, WorldType, C1>(ref D data, World<WorldType>.Entity entity, ref C1 c1)
        where D : struct
        where C1 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithRefDataEntity<D, WorldType, C1, C2>(ref D data, World<WorldType>.Entity entity, ref C1 c1, ref C2 c2)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3>(ref D data, World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4>(ref D data, World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5>(ref D data, World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6>(ref D data, World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7>(ref D data, World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8>(ref D data, World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7, ref C8 c8)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent
        where WorldType : struct, IWorldType;
    #endregion

    #region QUERY_FUNCTION_WITH_ENTITY_PARALLEL
    public delegate void QueryFunctionWithEntityParallel<WorldType, C1>(World<WorldType>.ROEntity entity, ref C1 c1)
        where C1 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithEntityParallel<WorldType, C1, C2>(World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithEntityParallel<WorldType, C1, C2, C3>(World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithEntityParallel<WorldType, C1, C2, C3, C4>(World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithEntityParallel<WorldType, C1, C2, C3, C4, C5>(World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6>(World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6, C7>(
        World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7
    )
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, C8>(
        World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7, ref C8 c8
    )
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent
        where WorldType : struct, IWorldType;
    #endregion
    
    #region QUERY_FUNCTION_WITH_DATA_ENTITY_PARALLEL
    public delegate void QueryFunctionWithDataEntityParallel<D, WorldType, C1>(D data, World<WorldType>.ROEntity entity, ref C1 c1)
        where C1 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2>(D data, World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3>(D data, World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3, C4>(D data, World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5>(D data, World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6>(D data, World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7>(D data, World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8>(D data, World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7, ref C8 c8)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent
        where WorldType : struct, IWorldType;
    #endregion

    #region QUERY_FUNCTION_WITH_REF_DATA_ENTITY_PARALLEL
    public delegate void QueryFunctionWithRefDataEntityParallel<D, WorldType, C1>(ref D data, World<WorldType>.ROEntity entity, ref C1 c1)
        where D : struct
        where C1 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2>(ref D data, World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3>(ref D data, World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4>(ref D data, World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5>(ref D data, World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6>(ref D data, World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7>(ref D data, World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8>(ref D data, World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7, ref C8 c8)
        where D : struct
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent
        where WorldType : struct, IWorldType;
    #endregion
}