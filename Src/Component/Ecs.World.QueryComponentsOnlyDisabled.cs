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
    public abstract partial class Ecs<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public readonly ref partial struct WithComponents<W> where W : struct, IQueryMethod {

            #region BY_STRUCT
            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where R : struct, IQueryFunction<C1> {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where R : struct, IQueryFunction<C1> {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2> {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2> {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3> {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3> {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, C4, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, C4, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, C4, C5, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, C4, C5, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, C7, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, C7, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, C7, C8, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, C7, C8, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(ref runner, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }
            #endregion

            #region BY_DELEGATE
            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1>(DelegateQueryFunction<WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(function, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2>(DelegateQueryFunction<WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(function, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3>(DelegateQueryFunction<WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(function, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, C4>(DelegateQueryFunction<WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(function, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, C4, C5>(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(function, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, C4, C5, C6>(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(function, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, C7>(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(function, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }

            [MethodImpl(AggressiveInlining)]
            public void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, C7, C8>(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(function, With, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
            }
            #endregion
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public abstract partial class World {
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            public partial struct QueryComponents {

                #region BY_RUNNER
                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where R : struct, IQueryFunction<C1> {
                    QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }
                
                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where R : struct, IQueryFunction<C1> {
                    QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2> {
                    QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }
                
                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2> {
                    QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3> {
                    QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }
                
                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3> {
                    QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, C4, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4> {
                    QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }
                
                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, C4, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4> {
                    QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, C4, C5, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                    QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }
                
                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, C4, C5, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                    QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                    QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }
                
                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                    QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, C7, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                    QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }
                
                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, C7, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                    QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, C7, C8, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where C8 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                    QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }
                
                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, C7, C8, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where C8 : struct, IComponent
                    where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                    QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(ref runner, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }
                #endregion

                #region BY_DELEGATE
                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1>(DelegateQueryFunction<WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent {
                    QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(function, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2>(DelegateQueryFunction<WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent {
                    QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(function, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3>(DelegateQueryFunction<WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent {
                    QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(function, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, C4>(DelegateQueryFunction<WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent {
                    QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(function, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, C4, C5>(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent {
                    QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(function, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, C4, C5, C6>(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent {
                    QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(function, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, C7>(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent {
                    QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(function, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }

                [MethodImpl(AggressiveInlining)]
                public static void ForOnlyDisabled<C1, C2, C3, C4, C5, C6, C7, C8>(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled)
                    where C1 : struct, IComponent
                    where C2 : struct, IComponent
                    where C3 : struct, IComponent
                    where C4 : struct, IComponent
                    where C5 : struct, IComponent
                    where C6 : struct, IComponent
                    where C7 : struct, IComponent
                    where C8 : struct, IComponent {
                    QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(function, default, entities, Const.EmptyAndDisabledComponentMask, Const.DisabledComponentMask);
                }
                #endregion
            }
        }
    }
}