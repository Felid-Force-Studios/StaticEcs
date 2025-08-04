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

            #region QUERY_FUNCTION
            [MethodImpl(AggressiveInlining)]
            public void For<C1>(
                uint minChunkSize, QueryFunction<C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                uint workersLimit = 0
            )
                where C1 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, W>>()
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2>(
                uint minChunkSize, QueryFunction<C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, W>>()
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3>(
                uint minChunkSize, QueryFunction<C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, W>>()
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4>(
                uint minChunkSize, QueryFunction<C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, W>>()
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5>(
                uint minChunkSize, QueryFunction<C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, W>>()
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6>(
                uint minChunkSize, QueryFunction<C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, C6, W>>()
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7>(
                uint minChunkSize,
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
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, C8>(
                uint minChunkSize,
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
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_DATA
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(
                uint minChunkSize, D data, QueryFunctionWithData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(
                uint minChunkSize, D data, QueryFunctionWithData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, C2, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(
                uint minChunkSize, D data, QueryFunctionWithData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, C2, C3, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(
                uint minChunkSize, D data, QueryFunctionWithData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, C2, C3, C4, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(
                uint minChunkSize, D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, C2, C3, C4, C5, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(
                uint minChunkSize, D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(
                D data, uint minChunkSize,
                QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                D data, uint minChunkSize,
                QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
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
                Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_REF_DATA_ENTITY
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(
                uint minChunkSize, ref D data, QueryFunctionWithRefData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(
                uint minChunkSize, ref D data, QueryFunctionWithRefData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(
                uint minChunkSize, ref D data, QueryFunctionWithRefData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(
                uint minChunkSize, ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(
                uint minChunkSize, ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(
                uint minChunkSize, ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(
                ref D data, uint minChunkSize,
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
                where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                ref D data, uint minChunkSize,
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
                where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_ENTITY
            [MethodImpl(AggressiveInlining)]
            public void For<C1>(
                uint minChunkSize, QueryFunctionWithEntityParallel<WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, W>>()
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2>(
                uint minChunkSize, QueryFunctionWithEntityParallel<WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, W>>()
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3>(
                uint minChunkSize, QueryFunctionWithEntityParallel<WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, W>>()
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4>(
                uint minChunkSize, QueryFunctionWithEntityParallel<WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, W>>()
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5>(
                uint minChunkSize, QueryFunctionWithEntityParallel<WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, C5, W>>()
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6>(
                uint minChunkSize, QueryFunctionWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6, W>>()
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7>(
                uint minChunkSize,
                QueryFunctionWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
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
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, C8>(
                uint minChunkSize,
                QueryFunctionWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
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
                       .Run(function, With, entities, components, minChunkSize, workersLimit);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_DATA_ENTITY
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(
                uint minChunkSize, D data, QueryFunctionWithDataEntityParallel<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(
                uint minChunkSize, D data, QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, C2, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(
                uint minChunkSize, D data, QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, C2, C3, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(
                uint minChunkSize, D data, QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, C2, C3, C4, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(
                uint minChunkSize, D data, QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, C2, C3, C4, C5, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(
                uint minChunkSize, D data, QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(
                D data, uint minChunkSize,
                QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                D data, uint minChunkSize,
                QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
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
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>>()
                       .Run(data, function, With, entities, components, minChunkSize, workersLimit);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_REF_DATA_ENTITY
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(
                uint minChunkSize, ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(
                uint minChunkSize, ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(
                uint minChunkSize, ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(
                uint minChunkSize, ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(
                uint minChunkSize, ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(
                uint minChunkSize, ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(
                ref D data, uint minChunkSize,
                QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                ref D data, uint minChunkSize,
                QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
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
                where D : struct {
                Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>>()
                       .Run(ref data, function, With, entities, components, minChunkSize, workersLimit);
            }
            #endregion
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public partial struct QueryComponents {
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

                #region QUERY_FUNCTION
                [MethodImpl(AggressiveInlining)]
                public static void For<C1>(
                    uint minChunkSize, QueryFunction<C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                    uint workersLimit = 0
                )
                    where C1 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, WithNothing>>()
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2>(
                    uint minChunkSize, QueryFunction<C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                    uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, WithNothing>>()
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3>(
                    uint minChunkSize, QueryFunction<C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled,
                    uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, WithNothing>>()
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4>(
                    uint minChunkSize, QueryFunction<C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, WithNothing>>()
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5>(
                    uint minChunkSize, QueryFunction<C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, WithNothing>>()
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5, C6>(
                    uint minChunkSize, QueryFunction<C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>>()
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5, C6, C7>(
                    uint minChunkSize,
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
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5, C6, C7, C8>(
                    uint minChunkSize,
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
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }
                #endregion

                #region QUERY_FUNCTION_WITH_DATA
                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1>(
                    uint minChunkSize, D data, QueryFunctionWithData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2>(
                    uint minChunkSize, D data, QueryFunctionWithData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, C2, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3>(
                    uint minChunkSize, D data, QueryFunctionWithData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, C2, C3, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4>(
                    uint minChunkSize, D data, QueryFunctionWithData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, C2, C3, C4, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5>(
                    uint minChunkSize, D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, C2, C3, C4, C5, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6>(
                    uint minChunkSize, D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7>(
                    D data, uint minChunkSize,
                    QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                    D data, uint minChunkSize,
                    QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
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
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }
                #endregion

                #region QUERY_FUNCTION_WITH_REF_DATA_ENTITY
                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1>(
                    uint minChunkSize, ref D data, QueryFunctionWithRefData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2>(
                    uint minChunkSize, ref D data, QueryFunctionWithRefData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3>(
                    uint minChunkSize, ref D data, QueryFunctionWithRefData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4>(
                    uint minChunkSize, ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5>(
                    uint minChunkSize, ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6>(
                    uint minChunkSize, ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7>(
                    ref D data, uint minChunkSize,
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
                    where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                    ref D data, uint minChunkSize,
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
                    where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }
                #endregion

                #region QUERY_FUNCTION_WITH_ENTITY
                [MethodImpl(AggressiveInlining)]
                public static void For<C1>(
                    uint minChunkSize, QueryFunctionWithEntityParallel<WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, WithNothing>>()
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2>(
                    uint minChunkSize, QueryFunctionWithEntityParallel<WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, WithNothing>>()
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3>(
                    uint minChunkSize, QueryFunctionWithEntityParallel<WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, WithNothing>>()
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4>(
                    uint minChunkSize, QueryFunctionWithEntityParallel<WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, WithNothing>>()
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5>(
                    uint minChunkSize, QueryFunctionWithEntityParallel<WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, C5, WithNothing>>()
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5, C6>(
                    uint minChunkSize, QueryFunctionWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>>()
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5, C6, C7>(
                    uint minChunkSize,
                    QueryFunctionWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
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
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<C1, C2, C3, C4, C5, C6, C7, C8>(
                    uint minChunkSize,
                    QueryFunctionWithEntityParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
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
                           .Run(function, default, entities, components, minChunkSize, workersLimit);
                }
                #endregion

                #region QUERY_FUNCTION_WITH_DATA_ENTITY
                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1>(
                    uint minChunkSize, D data, QueryFunctionWithDataEntityParallel<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2>(
                    uint minChunkSize, D data, QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, C2, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3>(
                    uint minChunkSize, D data, QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, C2, C3, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4>(
                    uint minChunkSize, D data, QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, C2, C3, C4, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5>(
                    uint minChunkSize, D data, QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, C2, C3, C4, C5, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6>(
                    uint minChunkSize, D data, QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7>(
                    D data, uint minChunkSize,
                    QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                    D data, uint minChunkSize,
                    QueryFunctionWithDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
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
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>>()
                           .Run(data, function, default, entities, components, minChunkSize, workersLimit);
                }
                #endregion

                #region QUERY_FUNCTION_WITH_REF_DATA_ENTITY
                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1>(
                    uint minChunkSize, ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2>(
                    uint minChunkSize, ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3>(
                    uint minChunkSize, ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4>(
                    uint minChunkSize, ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5>(
                    uint minChunkSize, ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6>(
                    uint minChunkSize, ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7>(
                    ref D data, uint minChunkSize,
                    QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled,
                    ComponentStatus components = ComponentStatus.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(
                    ref D data, uint minChunkSize,
                    QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled,
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
                    where D : struct {
                    Context.Value.GetOrCreate<QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>>()
                           .Run(ref data, function, default, entities, components, minChunkSize, workersLimit);
                }
                #endregion
            }
        }
    }
}