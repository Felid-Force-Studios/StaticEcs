using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public readonly ref struct WithComponentsParallel<W> where W : struct, IQueryMethod {
            private readonly W With;

            [MethodImpl(AggressiveInlining)]
            public WithComponentsParallel(W with) {
                With = with;
            }
            
            #region BY_RUNNER
            [MethodImpl(AggressiveInlining)]
            public void For<R>(
                uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0)
                where R : struct, IQueryFunction {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, W>>()
                       .Run(ref runner, With, entities, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<R>(
                uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0)
                where R : struct, IQueryFunction {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, W>>()
                       .Run(ref runner, With, entities, minEntitiesPerThread, workersLimit);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void For<C1, R>(
                uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where R : struct, IQueryFunction<C1> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, R>(
                uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where R : struct, IQueryFunction<C1> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, R>(
                uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, R>(
                uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, R>(
                uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, R>(
                uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, R>(
                uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, R>(
                uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, R>(
                uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, R>(
                uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, R>(
                uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, C6, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, R>(
                uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, C6, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, R>(
                uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, C6, C7, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, R>(
                uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, C6, C7, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, C8, R>(
                uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, C8, R>(
                uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>>()
                       .Run(ref runner, With, entities, components, minEntitiesPerThread, workersLimit);
            }
            #endregion

            #region QUERY_FUNCTION
            [MethodImpl(AggressiveInlining)]
            public void For<C1>(
                uint minEntitiesPerThread, QueryFunction<C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                uint workersLimit = 0
            )
                where C1 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2>(
                uint minEntitiesPerThread, QueryFunction<C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3>(
                uint minEntitiesPerThread, QueryFunction<C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4>(
                uint minEntitiesPerThread, QueryFunction<C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5>(
                uint minEntitiesPerThread, QueryFunction<C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6>(
                uint minEntitiesPerThread, QueryFunction<C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, C6, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7>(
                uint minEntitiesPerThread,
                QueryFunction<C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, C8>(
                uint minEntitiesPerThread,
                QueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_DATA
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(
                uint minEntitiesPerThread, D data, QueryFunctionWithRefData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(
                uint minEntitiesPerThread, D data, QueryFunctionWithRefData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(
                uint minEntitiesPerThread, D data, QueryFunctionWithRefData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(
                uint minEntitiesPerThread, D data, QueryFunctionWithRefData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(
                uint minEntitiesPerThread, D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(
                uint minEntitiesPerThread, D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(
                D data, uint minEntitiesPerThread,
                QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                D data, uint minEntitiesPerThread,
                QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_REF_DATA
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(
                uint minEntitiesPerThread, ref D data, QueryFunctionWithRefData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(
                uint minEntitiesPerThread, ref D data, QueryFunctionWithRefData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(
                uint minEntitiesPerThread, ref D data, QueryFunctionWithRefData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(
                uint minEntitiesPerThread, ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(
                uint minEntitiesPerThread, ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(
                uint minEntitiesPerThread, ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(
                ref D data, uint minEntitiesPerThread,
                QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                ref D data, uint minEntitiesPerThread,
                QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent
                {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_ENTITY
            [MethodImpl(AggressiveInlining)]
            public void For(
                uint minEntitiesPerThread, QueryFunctionWithEntity<WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
            ) {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, W>>()
                       .Run(function, With, entities, minEntitiesPerThread, workersLimit);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void For<C1>(
                uint minEntitiesPerThread, QueryFunctionWithEntity<WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2>(
                uint minEntitiesPerThread, QueryFunctionWithEntity<WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3>(
                uint minEntitiesPerThread, QueryFunctionWithEntity<WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4>(
                uint minEntitiesPerThread, QueryFunctionWithEntity<WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5>(
                uint minEntitiesPerThread, QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, C5, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6>(
                uint minEntitiesPerThread, QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7>(
                uint minEntitiesPerThread,
                QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, C8>(
                uint minEntitiesPerThread,
                QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>>()
                       .Run(function, With, entities, components, minEntitiesPerThread, workersLimit);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_DATA_ENTITY
            [MethodImpl(AggressiveInlining)]
            public void For<D>(
                uint minEntitiesPerThread, D data, QueryFunctionWithRefDataEntity<D, WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
            ) {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, W>>()
                       .Run(ref data, function, With, entities, minEntitiesPerThread, workersLimit);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(
                uint minEntitiesPerThread, D data, QueryFunctionWithRefDataEntity<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(
                uint minEntitiesPerThread, D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(
                uint minEntitiesPerThread, D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(
                uint minEntitiesPerThread, D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(
                uint minEntitiesPerThread, D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(
                uint minEntitiesPerThread, D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(
                D data, uint minEntitiesPerThread,
                QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                D data, uint minEntitiesPerThread,
                QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_REF_DATA_ENTITY
            [MethodImpl(AggressiveInlining)]
            public void For<D>(
                uint minEntitiesPerThread, ref D data, QueryFunctionWithRefDataEntity<D, WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
            ) {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, W>>()
                       .Run(ref data, function, With, entities, minEntitiesPerThread, workersLimit);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(
                uint minEntitiesPerThread, ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(
                uint minEntitiesPerThread, ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(
                uint minEntitiesPerThread, ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(
                uint minEntitiesPerThread, ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(
                uint minEntitiesPerThread, ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(
                uint minEntitiesPerThread, ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(
                ref D data, uint minEntitiesPerThread,
                QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                ref D data, uint minEntitiesPerThread,
                QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent
                {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>>()
                       .Run(ref data, function, With, entities, components, minEntitiesPerThread, workersLimit);
            }
            #endregion
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public partial struct Query {
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            public struct Parallel {
                [MethodImpl(AggressiveInlining)]
                public static WithComponentsParallel<W> With<W>(W with = default)
                    where W : struct, IQueryMethod {
                    return new WithComponentsParallel<W>(with);
                }
                
                #region BY_RUNNER
                [MethodImpl(AggressiveInlining)]
                public void For<R>(
                    uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0)
                    where R : struct, IQueryFunction {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, WithNothing>>()
                           .Run(ref runner, default, entities, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<R>(
                    uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0)
                    where R : struct, IQueryFunction {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, WithNothing>>()
                           .Run(ref runner, default, entities, minEntitiesPerThread, workersLimit);
                }
                
                [MethodImpl(AggressiveInlining)]
                public void For<C1, R>(
                    uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where R : struct, IQueryFunction<C1> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<C1, R>(
                    uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where R : struct, IQueryFunction<C1> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<C1, C2, R>(
                    uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<C1, C2, R>(
                    uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<C1, C2, C3, R>(
                    uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<C1, C2, C3, R>(
                    uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<C1, C2, C3, C4, R>(
                    uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<C1, C2, C3, C4, R>(
                    uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<C1, C2, C3, C4, C5, R>(
                    uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<C1, C2, C3, C4, C5, R>(
                    uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<C1, C2, C3, C4, C5, C6, R>(
                    uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, C6, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<C1, C2, C3, C4, C5, C6, R>(
                    uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, C6, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<C1, C2, C3, C4, C5, C6, C7, R>(
                    uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<C1, C2, C3, C4, C5, C6, C7, R>(
                    uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<C1, C2, C3, C4, C5, C6, C7, C8, R>(
                    uint minEntitiesPerThread, R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where C8 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
            
                [MethodImpl(AggressiveInlining)]
                public void For<C1, C2, C3, C4, C5, C6, C7, C8, R>(
                    uint minEntitiesPerThread, ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where C8 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityFunctionParallel<R, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>>()
                           .Run(ref runner, default, entities, components, minEntitiesPerThread, workersLimit);
                }
                #endregion

                #region QUERY_FUNCTION
                [MethodImpl(AggressiveInlining)]
                public static void For<C1>(
                    uint minEntitiesPerThread, QueryFunction<C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                    uint workersLimit = 0
                )
                    where C1 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2>(
                    uint minEntitiesPerThread, QueryFunction<C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                    uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3>(
                    uint minEntitiesPerThread, QueryFunction<C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                    uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4>(
                    uint minEntitiesPerThread, QueryFunction<C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5>(
                    uint minEntitiesPerThread, QueryFunction<C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5, C6>(
                    uint minEntitiesPerThread, QueryFunction<C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5, C6, C7>(
                    uint minEntitiesPerThread,
                    QueryFunction<C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                    uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5, C6, C7, C8>(
                    uint minEntitiesPerThread,
                    QueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                    uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where C8 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }
                #endregion

                #region QUERY_FUNCTION_WITH_DATA
                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1>(
                    uint minEntitiesPerThread, D data, QueryFunctionWithRefData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2>(
                    uint minEntitiesPerThread, D data, QueryFunctionWithRefData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3>(
                    uint minEntitiesPerThread, D data, QueryFunctionWithRefData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4>(
                    uint minEntitiesPerThread, D data, QueryFunctionWithRefData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5>(
                    uint minEntitiesPerThread, D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6>(
                    uint minEntitiesPerThread, D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7>(
                    D data, uint minEntitiesPerThread,
                    QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                    D data, uint minEntitiesPerThread,
                    QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where C8 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }
                #endregion

                #region QUERY_FUNCTION_WITH_REF_DATA
                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1>(
                    uint minEntitiesPerThread, ref D data, QueryFunctionWithRefData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2>(
                    uint minEntitiesPerThread, ref D data, QueryFunctionWithRefData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3>(
                    uint minEntitiesPerThread, ref D data, QueryFunctionWithRefData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4>(
                    uint minEntitiesPerThread, ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5>(
                    uint minEntitiesPerThread, ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6>(
                    uint minEntitiesPerThread, ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7>(
                    ref D data, uint minEntitiesPerThread,
                    QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                    ref D data, uint minEntitiesPerThread,
                    QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where C8 : struct, IComponent
                    {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }
                #endregion

                #region QUERY_FUNCTION_WITH_ENTITY
                [MethodImpl(AggressiveInlining)]
                public static void For(
                    uint minEntitiesPerThread, QueryFunctionWithEntity<WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
                ) {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, WithNothing>>()
                           .Run(function, default, entities, minEntitiesPerThread, workersLimit);
                }
                
                [MethodImpl(AggressiveInlining)]
                public static void For<C1>(
                    uint minEntitiesPerThread, QueryFunctionWithEntity<WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2>(
                    uint minEntitiesPerThread, QueryFunctionWithEntity<WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3>(
                    uint minEntitiesPerThread, QueryFunctionWithEntity<WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4>(
                    uint minEntitiesPerThread, QueryFunctionWithEntity<WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5>(
                    uint minEntitiesPerThread, QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, C5, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5, C6>(
                    uint minEntitiesPerThread, QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5, C6, C7>(
                    uint minEntitiesPerThread,
                    QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5, C6, C7, C8>(
                    uint minEntitiesPerThread,
                    QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where C8 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>>()
                           .Run(function, default, entities, components, minEntitiesPerThread, workersLimit);
                }
                #endregion

                #region QUERY_FUNCTION_WITH_DATA_ENTITY
                [MethodImpl(AggressiveInlining)]
                public static void For<D>(
                    uint minEntitiesPerThread, D data, QueryFunctionWithRefDataEntity<D, WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
                )  {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, WithNothing>>()
                           .Run(ref data, function, default, entities, minEntitiesPerThread, workersLimit);
                }
                
                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1>(
                    uint minEntitiesPerThread, D data, QueryFunctionWithRefDataEntity<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2>(
                    uint minEntitiesPerThread, D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3>(
                    uint minEntitiesPerThread, D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4>(
                    uint minEntitiesPerThread, D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5>(
                    uint minEntitiesPerThread, D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6>(
                    uint minEntitiesPerThread, D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7>(
                    D data, uint minEntitiesPerThread,
                    QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                    D data, uint minEntitiesPerThread,
                    QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where C8 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }
                #endregion

                #region QUERY_FUNCTION_WITH_REF_DATA_ENTITY
                [MethodImpl(AggressiveInlining)]
                public static void For<D>(
                    uint minEntitiesPerThread, ref D data, QueryFunctionWithRefDataEntity<D, WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
                ) {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, WithNothing>>()
                           .Run(ref data, function, default, entities, minEntitiesPerThread, workersLimit);
                }
                
                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1>(
                    uint minEntitiesPerThread, ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2>(
                    uint minEntitiesPerThread, ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3>(
                    uint minEntitiesPerThread, ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4>(
                    uint minEntitiesPerThread, ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5>(
                    uint minEntitiesPerThread, ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6>(
                    uint minEntitiesPerThread, ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7>(
                    ref D data, uint minEntitiesPerThread,
                    QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                    ref D data, uint minEntitiesPerThread,
                    QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where C8 : struct, IComponent
                    {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minEntitiesPerThread, workersLimit);
                }
                #endregion
            }
        }
    }
}