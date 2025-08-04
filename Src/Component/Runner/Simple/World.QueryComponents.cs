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
        public readonly ref struct WithComponents<W> where W : struct, IQueryMethod {
            private readonly W With;

            [MethodImpl(AggressiveInlining)]
            public WithComponents(W with) {
                With = with;
            }

            #region BY_STRUCT
            [MethodImpl(AggressiveInlining)]
            public void For<C1, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where R : struct, IQueryFunction<C1> {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(ref runner, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where R : struct, IQueryFunction<C1> {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(ref runner, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2> {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(ref runner, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2> {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(ref runner, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3> {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(ref runner, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3> {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(ref runner, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(ref runner, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(ref runner, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(ref runner, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(ref runner, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(ref runner, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(ref runner, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(ref runner, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(ref runner, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, C8, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(ref runner, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, C8, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(ref runner, With, entities, components);
            }
            #endregion

            #region QUERY_FUNCTION
            [MethodImpl(AggressiveInlining)]
            public void For<C1>(QueryFunction<C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2>(QueryFunction<C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3>(QueryFunction<C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4>(QueryFunction<C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5>(QueryFunction<C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6>(QueryFunction<C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7>(QueryFunction<C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, C8>(QueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(function, With, entities, components);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_DATA
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(D data, QueryFunctionWithData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(D data, QueryFunctionWithData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(D data, QueryFunctionWithData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(D data, QueryFunctionWithData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(data, function, With, entities, components);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_REF_DATA
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(ref D data, QueryFunctionWithRefData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(ref data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(ref D data, QueryFunctionWithRefData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(ref data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(ref data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(ref data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(ref data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(ref data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(ref data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(ref data, function, With, entities, components);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_ENTITY
            [MethodImpl(AggressiveInlining)]
            public void For<C1>(QueryFunctionWithEntity<WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2>(QueryFunctionWithEntity<WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3>(QueryFunctionWithEntity<WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, C8>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(function, With, entities, components);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_DATA_ENTITY
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(D data, QueryFunctionWithDataEntity<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(data, function, With, entities, components);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_DATA_ENTITY
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(ref data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(ref data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(ref data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(ref data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(ref data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(ref data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(ref data, function, With, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(ref data, function, With, entities, components);
            }
            #endregion
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public partial struct QueryComponents {
            public static WithComponents<W> With<W>(W with = default)
                where W : struct, IQueryMethod {
                return new WithComponents<W>(with);
            }

            #region BY_RUNNER
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where R : struct, IQueryFunction<C1> {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(ref runner, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where R : struct, IQueryFunction<C1> {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(ref runner, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2> {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(ref runner, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2> {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(ref runner, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3> {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(ref runner, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3> {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(ref runner, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(ref runner, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(ref runner, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(ref runner, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(ref runner, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(ref runner, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(ref runner, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(ref runner, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(ref runner, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7, C8, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(ref runner, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7, C8, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(ref runner, default, entities, components);
            }
            #endregion

            #region QUERY_FUNCTION
            [MethodImpl(AggressiveInlining)]
            public static void For<C1>(QueryFunction<C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2>(QueryFunction<C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3>(QueryFunction<C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4>(QueryFunction<C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5>(QueryFunction<C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6>(QueryFunction<C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7>(QueryFunction<C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7, C8>(QueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(function, default, entities, components);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_DATA
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1>(D data, QueryFunctionWithData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2>(D data, QueryFunctionWithData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3>(D data, QueryFunctionWithData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4>(D data, QueryFunctionWithData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(data, function, default, entities, components);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_REF_DATA
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1>(ref D data, QueryFunctionWithRefData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2>(ref D data, QueryFunctionWithRefData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_ENTITY
            [MethodImpl(AggressiveInlining)]
            public static void For<C1>(QueryFunctionWithEntity<WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2>(QueryFunctionWithEntity<WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3>(QueryFunctionWithEntity<WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7, C8>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(function, default, entities, components);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_DATA_ENTITY
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1>(D data, QueryFunctionWithDataEntity<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(data, function, default, entities, components);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_DATA_ENTITY
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(ref data, function, default, entities, components);
            }
            #endregion
        }
    }
}