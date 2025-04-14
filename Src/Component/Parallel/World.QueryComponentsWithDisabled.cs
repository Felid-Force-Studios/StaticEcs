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
        public readonly ref partial struct WithComponentsParallel<W> where W : struct, IQueryMethod {
            #region BY_DELEGATE
            [MethodImpl(AggressiveInlining)]
            public void ForWithDisabled<C1>(uint minChunkSize, DelegateQueryFunctionParallel<WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, W>>().Run(function, With, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForWithDisabled<C1, C2>(uint minChunkSize, DelegateQueryFunctionParallel<WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, W>>().Run(function, With, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForWithDisabled<C1, C2, C3>(uint minChunkSize, DelegateQueryFunctionParallel<WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, W>>().Run(function, With, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForWithDisabled<C1, C2, C3, C4>(
                uint minChunkSize, DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, W>>()
                       .Run(function, With, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForWithDisabled<C1, C2, C3, C4, C5>(
                uint minChunkSize, DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, W>>()
                       .Run(function, With, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForWithDisabled<C1, C2, C3, C4, C5, C6>(
                uint minChunkSize, DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, C6, W>>()
                       .Run(function, With, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForWithDisabled<C1, C2, C3, C4, C5, C6, C7>(
                uint minChunkSize, DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
            )
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, W>>()
                       .Run(function, With, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForWithDisabled<C1, C2, C3, C4, C5, C6, C7, C8>(
                uint minChunkSize,
                DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
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
                       .Run(function, With, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
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
            public partial struct Parallel {
                #region BY_DELEGATE
                [MethodImpl(AggressiveInlining)]
                public static void ForWithDisabled<C1>(uint minChunkSize, DelegateQueryFunctionParallel<WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0)
                    where C1 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, WithNothing>>()
                           .Run(function, default, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForWithDisabled<C1, C2>(
                    uint minChunkSize, DelegateQueryFunctionParallel<WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, WithNothing>>()
                           .Run(function, default, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForWithDisabled<C1, C2, C3>(
                    uint minChunkSize, DelegateQueryFunctionParallel<WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, WithNothing>>()
                           .Run(function, default, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForWithDisabled<C1, C2, C3, C4>(
                    uint minChunkSize, DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, WithNothing>>()
                           .Run(function, default, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForWithDisabled<C1, C2, C3, C4, C5>(
                    uint minChunkSize, DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, WithNothing>>()
                           .Run(function, default, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForWithDisabled<C1, C2, C3, C4, C5, C6>(
                    uint minChunkSize, DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>>()
                           .Run(function, default, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForWithDisabled<C1, C2, C3, C4, C5, C6, C7>(
                    uint minChunkSize,
                    DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
                )
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent {
                    Context.Value.GetOrCreate<QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>>()
                           .Run(function, default, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForWithDisabled<C1, C2, C3, C4, C5, C6, C7, C8>(
                    uint minChunkSize,
                    DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, uint workersLimit = 0
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
                           .Run(function, default, entities, Const.EmptyComponentMask, 0, minChunkSize, workersLimit);
                }
                #endregion
            }
        }
    }
}