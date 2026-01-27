// ReSharper disable InconsistentNaming
#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public abstract partial class World<TWorld> {
        #region QUERY_INTERFACES
        /// <summary>
        /// Struct-based query callback interface hierarchy.
        /// Entity-only: implement IQuery directly.
        /// With components: implement nested Write&lt;&gt; / Read&lt;&gt; / Write&lt;&gt;.Read&lt;&gt;.
        /// </summary>
        public interface IQuery {
            /// <summary>Called once for each entity that matches the query filter.</summary>
            public void Invoke(Entity entity);

            /// <summary>Query callback with 1 writable component(s).</summary>
            public interface Write<T0> where T0 : struct, IComponent {
                public void Invoke(Entity entity, ref T0 comp0);
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                /// <summary>Query callback with 1 writable and 1 readonly component(s).</summary>
                public interface Read<T1> where T1 : struct, IComponent {
                    public void Invoke(Entity entity, ref T0 comp0, in T1 comp1);
                }
                /// <summary>Query callback with 1 writable and 2 readonly component(s).</summary>
                public interface Read<T1, T2> where T1 : struct, IComponent where T2 : struct, IComponent {
                    public void Invoke(Entity entity, ref T0 comp0, in T1 comp1, in T2 comp2);
                }
                /// <summary>Query callback with 1 writable and 3 readonly component(s).</summary>
                public interface Read<T1, T2, T3> where T1 : struct, IComponent where T2 : struct, IComponent where T3 : struct, IComponent {
                    public void Invoke(Entity entity, ref T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3);
                }
                /// <summary>Query callback with 1 writable and 4 readonly component(s).</summary>
                public interface Read<T1, T2, T3, T4> where T1 : struct, IComponent where T2 : struct, IComponent where T3 : struct, IComponent where T4 : struct, IComponent {
                    public void Invoke(Entity entity, ref T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4);
                }
                /// <summary>Query callback with 1 writable and 5 readonly component(s).</summary>
                public interface Read<T1, T2, T3, T4, T5> where T1 : struct, IComponent where T2 : struct, IComponent where T3 : struct, IComponent where T4 : struct, IComponent where T5 : struct, IComponent {
                    public void Invoke(Entity entity, ref T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5);
                }
                #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
            }

            /// <summary>Query callback with 2 writable component(s).</summary>
            public interface Write<T0, T1> where T0 : struct, IComponent where T1 : struct, IComponent {
                public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1);
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                /// <summary>Query callback with 2 writable and 1 readonly component(s).</summary>
                public interface Read<T2> where T2 : struct, IComponent {
                    public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, in T2 comp2);
                }
                /// <summary>Query callback with 2 writable and 2 readonly component(s).</summary>
                public interface Read<T2, T3> where T2 : struct, IComponent where T3 : struct, IComponent {
                    public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, in T2 comp2, in T3 comp3);
                }
                /// <summary>Query callback with 2 writable and 3 readonly component(s).</summary>
                public interface Read<T2, T3, T4> where T2 : struct, IComponent where T3 : struct, IComponent where T4 : struct, IComponent {
                    public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4);
                }
                /// <summary>Query callback with 2 writable and 4 readonly component(s).</summary>
                public interface Read<T2, T3, T4, T5> where T2 : struct, IComponent where T3 : struct, IComponent where T4 : struct, IComponent where T5 : struct, IComponent {
                    public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5);
                }
                #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
            }

            /// <summary>Query callback with 3 writable component(s).</summary>
            public interface Write<T0, T1, T2> where T0 : struct, IComponent where T1 : struct, IComponent where T2 : struct, IComponent {
                public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2);
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                /// <summary>Query callback with 3 writable and 1 readonly component(s).</summary>
                public interface Read<T3> where T3 : struct, IComponent {
                    public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, in T3 comp3);
                }
                /// <summary>Query callback with 3 writable and 2 readonly component(s).</summary>
                public interface Read<T3, T4> where T3 : struct, IComponent where T4 : struct, IComponent {
                    public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, in T3 comp3, in T4 comp4);
                }
                /// <summary>Query callback with 3 writable and 3 readonly component(s).</summary>
                public interface Read<T3, T4, T5> where T3 : struct, IComponent where T4 : struct, IComponent where T5 : struct, IComponent {
                    public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5);
                }
                #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
            }

            /// <summary>Query callback with 4 writable component(s).</summary>
            public interface Write<T0, T1, T2, T3> where T0 : struct, IComponent where T1 : struct, IComponent where T2 : struct, IComponent where T3 : struct, IComponent {
                public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3);
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                /// <summary>Query callback with 4 writable and 1 readonly component(s).</summary>
                public interface Read<T4> where T4 : struct, IComponent {
                    public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, in T4 comp4);
                }
                /// <summary>Query callback with 4 writable and 2 readonly component(s).</summary>
                public interface Read<T4, T5> where T4 : struct, IComponent where T5 : struct, IComponent {
                    public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, in T4 comp4, in T5 comp5);
                }
                #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
            }

            /// <summary>Query callback with 5 writable component(s).</summary>
            public interface Write<T0, T1, T2, T3, T4> where T0 : struct, IComponent where T1 : struct, IComponent where T2 : struct, IComponent where T3 : struct, IComponent where T4 : struct, IComponent {
                public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4);
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                /// <summary>Query callback with 5 writable and 1 readonly component(s).</summary>
                public interface Read<T5> where T5 : struct, IComponent {
                    public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, in T5 comp5);
                }
                #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
            }

            /// <summary>Query callback with 6 writable component(s).</summary>
            public interface Write<T0, T1, T2, T3, T4, T5> where T0 : struct, IComponent where T1 : struct, IComponent where T2 : struct, IComponent where T3 : struct, IComponent where T4 : struct, IComponent where T5 : struct, IComponent {
                public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5);
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            /// <summary>Query callback with 1 readonly component(s).</summary>
            public interface Read<T0> where T0 : struct, IComponent {
                public void Invoke(Entity entity, in T0 comp0);
            }

            /// <summary>Query callback with 2 readonly component(s).</summary>
            public interface Read<T0, T1> where T0 : struct, IComponent where T1 : struct, IComponent {
                public void Invoke(Entity entity, in T0 comp0, in T1 comp1);
            }

            /// <summary>Query callback with 3 readonly component(s).</summary>
            public interface Read<T0, T1, T2> where T0 : struct, IComponent where T1 : struct, IComponent where T2 : struct, IComponent {
                public void Invoke(Entity entity, in T0 comp0, in T1 comp1, in T2 comp2);
            }

            /// <summary>Query callback with 4 readonly component(s).</summary>
            public interface Read<T0, T1, T2, T3> where T0 : struct, IComponent where T1 : struct, IComponent where T2 : struct, IComponent where T3 : struct, IComponent {
                public void Invoke(Entity entity, in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3);
            }

            /// <summary>Query callback with 5 readonly component(s).</summary>
            public interface Read<T0, T1, T2, T3, T4> where T0 : struct, IComponent where T1 : struct, IComponent where T2 : struct, IComponent where T3 : struct, IComponent where T4 : struct, IComponent {
                public void Invoke(Entity entity, in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4);
            }

            /// <summary>Query callback with 6 readonly component(s).</summary>
            public interface Read<T0, T1, T2, T3, T4, T5> where T0 : struct, IComponent where T1 : struct, IComponent where T2 : struct, IComponent where T3 : struct, IComponent where T4 : struct, IComponent where T5 : struct, IComponent {
                public void Invoke(Entity entity, in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5);
            }
            #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
        }

        /// <summary>
        /// Block-based query callback interface hierarchy.
        /// </summary>
        public interface IQueryBlock {
            /// <summary>Called once for each contiguous run of matching entities within a block.</summary>
            public void Invoke(uint count, EntityBlock entitiesBlock);

            /// <summary>Query callback with 1 writable component(s).</summary>
            public interface Write<T0> where T0 : unmanaged, IComponent {
                public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0);
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                /// <summary>Query callback with 1 writable and 1 readonly component(s).</summary>
                public interface Read<T1> where T1 : unmanaged, IComponent {
                    public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, BlockR<T1> block1);
                }
                /// <summary>Query callback with 1 writable and 2 readonly component(s).</summary>
                public interface Read<T1, T2> where T1 : unmanaged, IComponent where T2 : unmanaged, IComponent {
                    public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, BlockR<T1> block1, BlockR<T2> block2);
                }
                /// <summary>Query callback with 1 writable and 3 readonly component(s).</summary>
                public interface Read<T1, T2, T3> where T1 : unmanaged, IComponent where T2 : unmanaged, IComponent where T3 : unmanaged, IComponent {
                    public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, BlockR<T1> block1, BlockR<T2> block2, BlockR<T3> block3);
                }
                /// <summary>Query callback with 1 writable and 4 readonly component(s).</summary>
                public interface Read<T1, T2, T3, T4> where T1 : unmanaged, IComponent where T2 : unmanaged, IComponent where T3 : unmanaged, IComponent where T4 : unmanaged, IComponent {
                    public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, BlockR<T1> block1, BlockR<T2> block2, BlockR<T3> block3, BlockR<T4> block4);
                }
                /// <summary>Query callback with 1 writable and 5 readonly component(s).</summary>
                public interface Read<T1, T2, T3, T4, T5> where T1 : unmanaged, IComponent where T2 : unmanaged, IComponent where T3 : unmanaged, IComponent where T4 : unmanaged, IComponent where T5 : unmanaged, IComponent {
                    public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, BlockR<T1> block1, BlockR<T2> block2, BlockR<T3> block3, BlockR<T4> block4, BlockR<T5> block5);
                }
                #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
            }

            /// <summary>Query callback with 2 writable component(s).</summary>
            public interface Write<T0, T1> where T0 : unmanaged, IComponent where T1 : unmanaged, IComponent {
                public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1);
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                /// <summary>Query callback with 2 writable and 1 readonly component(s).</summary>
                public interface Read<T2> where T2 : unmanaged, IComponent {
                    public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, BlockR<T2> block2);
                }
                /// <summary>Query callback with 2 writable and 2 readonly component(s).</summary>
                public interface Read<T2, T3> where T2 : unmanaged, IComponent where T3 : unmanaged, IComponent {
                    public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, BlockR<T2> block2, BlockR<T3> block3);
                }
                /// <summary>Query callback with 2 writable and 3 readonly component(s).</summary>
                public interface Read<T2, T3, T4> where T2 : unmanaged, IComponent where T3 : unmanaged, IComponent where T4 : unmanaged, IComponent {
                    public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, BlockR<T2> block2, BlockR<T3> block3, BlockR<T4> block4);
                }
                /// <summary>Query callback with 2 writable and 4 readonly component(s).</summary>
                public interface Read<T2, T3, T4, T5> where T2 : unmanaged, IComponent where T3 : unmanaged, IComponent where T4 : unmanaged, IComponent where T5 : unmanaged, IComponent {
                    public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, BlockR<T2> block2, BlockR<T3> block3, BlockR<T4> block4, BlockR<T5> block5);
                }
                #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
            }

            /// <summary>Query callback with 3 writable component(s).</summary>
            public interface Write<T0, T1, T2> where T0 : unmanaged, IComponent where T1 : unmanaged, IComponent where T2 : unmanaged, IComponent {
                public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2);
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                /// <summary>Query callback with 3 writable and 1 readonly component(s).</summary>
                public interface Read<T3> where T3 : unmanaged, IComponent {
                    public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, BlockR<T3> block3);
                }
                /// <summary>Query callback with 3 writable and 2 readonly component(s).</summary>
                public interface Read<T3, T4> where T3 : unmanaged, IComponent where T4 : unmanaged, IComponent {
                    public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, BlockR<T3> block3, BlockR<T4> block4);
                }
                /// <summary>Query callback with 3 writable and 3 readonly component(s).</summary>
                public interface Read<T3, T4, T5> where T3 : unmanaged, IComponent where T4 : unmanaged, IComponent where T5 : unmanaged, IComponent {
                    public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, BlockR<T3> block3, BlockR<T4> block4, BlockR<T5> block5);
                }
                #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
            }

            /// <summary>Query callback with 4 writable component(s).</summary>
            public interface Write<T0, T1, T2, T3> where T0 : unmanaged, IComponent where T1 : unmanaged, IComponent where T2 : unmanaged, IComponent where T3 : unmanaged, IComponent {
                public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, Block<T3> block3);
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                /// <summary>Query callback with 4 writable and 1 readonly component(s).</summary>
                public interface Read<T4> where T4 : unmanaged, IComponent {
                    public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, Block<T3> block3, BlockR<T4> block4);
                }
                /// <summary>Query callback with 4 writable and 2 readonly component(s).</summary>
                public interface Read<T4, T5> where T4 : unmanaged, IComponent where T5 : unmanaged, IComponent {
                    public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, Block<T3> block3, BlockR<T4> block4, BlockR<T5> block5);
                }
                #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
            }

            /// <summary>Query callback with 5 writable component(s).</summary>
            public interface Write<T0, T1, T2, T3, T4> where T0 : unmanaged, IComponent where T1 : unmanaged, IComponent where T2 : unmanaged, IComponent where T3 : unmanaged, IComponent where T4 : unmanaged, IComponent {
                public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, Block<T3> block3, Block<T4> block4);
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                /// <summary>Query callback with 5 writable and 1 readonly component(s).</summary>
                public interface Read<T5> where T5 : unmanaged, IComponent {
                    public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, Block<T3> block3, Block<T4> block4, BlockR<T5> block5);
                }
                #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
            }

            /// <summary>Query callback with 6 writable component(s).</summary>
            public interface Write<T0, T1, T2, T3, T4, T5> where T0 : unmanaged, IComponent where T1 : unmanaged, IComponent where T2 : unmanaged, IComponent where T3 : unmanaged, IComponent where T4 : unmanaged, IComponent where T5 : unmanaged, IComponent {
                public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, Block<T3> block3, Block<T4> block4, Block<T5> block5);
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            /// <summary>Query callback with 1 readonly component(s).</summary>
            public interface Read<T0> where T0 : unmanaged, IComponent {
                public void Invoke(uint count, EntityBlock entitiesBlock, BlockR<T0> block0);
            }

            /// <summary>Query callback with 2 readonly component(s).</summary>
            public interface Read<T0, T1> where T0 : unmanaged, IComponent where T1 : unmanaged, IComponent {
                public void Invoke(uint count, EntityBlock entitiesBlock, BlockR<T0> block0, BlockR<T1> block1);
            }

            /// <summary>Query callback with 3 readonly component(s).</summary>
            public interface Read<T0, T1, T2> where T0 : unmanaged, IComponent where T1 : unmanaged, IComponent where T2 : unmanaged, IComponent {
                public void Invoke(uint count, EntityBlock entitiesBlock, BlockR<T0> block0, BlockR<T1> block1, BlockR<T2> block2);
            }

            /// <summary>Query callback with 4 readonly component(s).</summary>
            public interface Read<T0, T1, T2, T3> where T0 : unmanaged, IComponent where T1 : unmanaged, IComponent where T2 : unmanaged, IComponent where T3 : unmanaged, IComponent {
                public void Invoke(uint count, EntityBlock entitiesBlock, BlockR<T0> block0, BlockR<T1> block1, BlockR<T2> block2, BlockR<T3> block3);
            }

            /// <summary>Query callback with 5 readonly component(s).</summary>
            public interface Read<T0, T1, T2, T3, T4> where T0 : unmanaged, IComponent where T1 : unmanaged, IComponent where T2 : unmanaged, IComponent where T3 : unmanaged, IComponent where T4 : unmanaged, IComponent {
                public void Invoke(uint count, EntityBlock entitiesBlock, BlockR<T0> block0, BlockR<T1> block1, BlockR<T2> block2, BlockR<T3> block3, BlockR<T4> block4);
            }

            /// <summary>Query callback with 6 readonly component(s).</summary>
            public interface Read<T0, T1, T2, T3, T4, T5> where T0 : unmanaged, IComponent where T1 : unmanaged, IComponent where T2 : unmanaged, IComponent where T3 : unmanaged, IComponent where T4 : unmanaged, IComponent where T5 : unmanaged, IComponent {
                public void Invoke(uint count, EntityBlock entitiesBlock, BlockR<T0> block0, BlockR<T1> block1, BlockR<T2> block2, BlockR<T3> block3, BlockR<T4> block4, BlockR<T5> block5);
            }
            #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
        }

        #endregion
    }

    #region QUERY_FUNCTION
    /// <summary>
    /// Delegate for query iteration that receives component references only, without the entity handle.
    /// Used with query <c>For</c> methods for lightweight callbacks when entity identity is not needed.
    /// <para>
    /// This is the simplest and fastest delegate form — use it when your logic only reads/writes
    /// component data and doesn't need to access the entity (e.g., for applying velocity to position).
    /// Since no entity handle is passed, there is no way to destroy or add/remove components from
    /// the entity inside this callback — use <see cref="QueryFunctionWithEntity{TWorld, T0}"/> for that.
    /// </para>
    /// </summary>
    /// <typeparam name="T0">Component type passed by reference.</typeparam>
    public delegate void QueryFunction<T0>(ref T0 comp0)
        where T0 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunction<T0, T1>(ref T0 comp0, ref T1 comp1)
        where T0 : struct, IComponent
        where T1 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunction<T0, T1, T2>(ref T0 comp0, ref T1 comp1, ref T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunction<T0, T1, T2, T3>(ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunction<T0, T1, T2, T3, T4>(ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunction<T0, T1, T2, T3, T4, T5>(ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent;

    #if !FFS_ECS_DISABLE_CHANGED_TRACKING
    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR1<T0>(in T0 comp0)
        where T0 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR1<T0, T1>(ref T0 comp0, in T1 comp1)
        where T0 : struct, IComponent
        where T1 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR2<T0, T1>(in T0 comp0, in T1 comp1)
        where T0 : struct, IComponent
        where T1 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR1<T0, T1, T2>(ref T0 comp0, ref T1 comp1, in T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR2<T0, T1, T2>(ref T0 comp0, in T1 comp1, in T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR3<T0, T1, T2>(in T0 comp0, in T1 comp1, in T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR1<T0, T1, T2, T3>(ref T0 comp0, ref T1 comp1, ref T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR2<T0, T1, T2, T3>(ref T0 comp0, ref T1 comp1, in T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR3<T0, T1, T2, T3>(ref T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR4<T0, T1, T2, T3>(in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR1<T0, T1, T2, T3, T4>(ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR2<T0, T1, T2, T3, T4>(ref T0 comp0, ref T1 comp1, ref T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR3<T0, T1, T2, T3, T4>(ref T0 comp0, ref T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR4<T0, T1, T2, T3, T4>(ref T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR5<T0, T1, T2, T3, T4>(in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR1<T0, T1, T2, T3, T4, T5>(ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR2<T0, T1, T2, T3, T4, T5>(ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR3<T0, T1, T2, T3, T4, T5>(ref T0 comp0, ref T1 comp1, ref T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR4<T0, T1, T2, T3, T4, T5>(ref T0 comp0, ref T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR5<T0, T1, T2, T3, T4, T5>(ref T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent;

    /// <inheritdoc cref="QueryFunction{T0}"/>
    public delegate void QueryFunctionR6<T0, T1, T2, T3, T4, T5>(in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent;

    #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
    #endregion
    
    #region QUERY_FUNCTION_WITH_REF_DATA
    /// <summary>
    /// Delegate for query iteration that receives an additional <c>ref TData</c> parameter alongside
    /// component references. Used with query <c>For</c> methods that accept a <c>ref TData</c> argument,
    /// allowing you to pass mutable state into the callback without closures or static fields.
    /// <para>
    /// This is critical for avoiding delegate allocations in hot paths: instead of capturing local
    /// variables in a lambda (which allocates a closure object), pass them as <typeparamref name="TData"/>
    /// by reference. The data parameter is forwarded from the caller's stack — no heap allocation occurs.
    /// Example: accumulating a sum, collecting results into a list, or passing a delta-time value.
    /// </para>
    /// </summary>
    /// <typeparam name="TData">Arbitrary user data type passed by reference. Can be any type (struct, class, array).</typeparam>
    /// <typeparam name="T0">Component type passed by reference.</typeparam>
    public delegate void QueryFunctionWithRefData<TData, T0>(ref TData data, ref T0 comp0)
        where T0 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefData<TData, T0, T1>(ref TData data, ref T0 comp0, ref T1 comp1)
        where T0 : struct, IComponent
        where T1 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefData<TData, T0, T1, T2>(ref TData data, ref T0 comp0, ref T1 comp1, ref T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefData<TData, T0, T1, T2, T3>(ref TData data, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefData<TData, T0, T1, T2, T3, T4>(ref TData data, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefData<TData, T0, T1, T2, T3, T4, T5>(ref TData data, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent;

    #if !FFS_ECS_DISABLE_CHANGED_TRACKING
    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR1<TData, T0>(ref TData data, in T0 comp0)
        where T0 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR1<TData, T0, T1>(ref TData data, ref T0 comp0, in T1 comp1)
        where T0 : struct, IComponent
        where T1 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR2<TData, T0, T1>(ref TData data, in T0 comp0, in T1 comp1)
        where T0 : struct, IComponent
        where T1 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR1<TData, T0, T1, T2>(ref TData data, ref T0 comp0, ref T1 comp1, in T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR2<TData, T0, T1, T2>(ref TData data, ref T0 comp0, in T1 comp1, in T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR3<TData, T0, T1, T2>(ref TData data, in T0 comp0, in T1 comp1, in T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR1<TData, T0, T1, T2, T3>(ref TData data, ref T0 comp0, ref T1 comp1, ref T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR2<TData, T0, T1, T2, T3>(ref TData data, ref T0 comp0, ref T1 comp1, in T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR3<TData, T0, T1, T2, T3>(ref TData data, ref T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR4<TData, T0, T1, T2, T3>(ref TData data, in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR1<TData, T0, T1, T2, T3, T4>(ref TData data, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR2<TData, T0, T1, T2, T3, T4>(ref TData data, ref T0 comp0, ref T1 comp1, ref T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR3<TData, T0, T1, T2, T3, T4>(ref TData data, ref T0 comp0, ref T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR4<TData, T0, T1, T2, T3, T4>(ref TData data, ref T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR5<TData, T0, T1, T2, T3, T4>(ref TData data, in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR1<TData, T0, T1, T2, T3, T4, T5>(ref TData data, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR2<TData, T0, T1, T2, T3, T4, T5>(ref TData data, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR3<TData, T0, T1, T2, T3, T4, T5>(ref TData data, ref T0 comp0, ref T1 comp1, ref T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR4<TData, T0, T1, T2, T3, T4, T5>(ref TData data, ref T0 comp0, ref T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR5<TData, T0, T1, T2, T3, T4, T5>(ref TData data, ref T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent;

    /// <inheritdoc cref="QueryFunctionWithRefData{TData, T0}"/>
    public delegate void QueryFunctionWithRefDataR6<TData, T0, T1, T2, T3, T4, T5>(ref TData data, in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent;

    #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
    #endregion

    #region QUERY_FUNCTION_WITH_ENTITY
    /// <summary>
    /// Delegate for query iteration that receives the entity handle and optionally component references.
    /// Used with query <c>For</c> methods when you need the entity for operations beyond
    /// component access — such as destroying the entity, adding/removing components, checking tags, etc.
    /// <para>
    /// This is the most versatile delegate form. The entity parameter provides the full
    /// <see cref="World{TWorld}.Entity"/> API surface. Note that structural changes (Add/Delete/Destroy)
    /// on the current entity during iteration are safe, but modifying filtered types on OTHER entities
    /// requires <see cref="QueryMode.Flexible"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TWorld">The world type.</typeparam>
    public delegate void QueryFunctionWithEntity<TWorld>(World<TWorld>.Entity entity)
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntity<TWorld, T0>(World<TWorld>.Entity entity, ref T0 comp0)
        where T0 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntity<TWorld, T0, T1>(World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntity<TWorld, T0, T1, T2>(World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntity<TWorld, T0, T1, T2, T3>(World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntity<TWorld, T0, T1, T2, T3, T4>(World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntity<TWorld, T0, T1, T2, T3, T4, T5>(World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where TWorld : struct, IWorldType;

    #if !FFS_ECS_DISABLE_CHANGED_TRACKING
    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR1<TWorld, T0>(World<TWorld>.Entity entity, in T0 comp0)
        where T0 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR1<TWorld, T0, T1>(World<TWorld>.Entity entity, ref T0 comp0, in T1 comp1)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR2<TWorld, T0, T1>(World<TWorld>.Entity entity, in T0 comp0, in T1 comp1)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR1<TWorld, T0, T1, T2>(World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, in T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR2<TWorld, T0, T1, T2>(World<TWorld>.Entity entity, ref T0 comp0, in T1 comp1, in T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR3<TWorld, T0, T1, T2>(World<TWorld>.Entity entity, in T0 comp0, in T1 comp1, in T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR1<TWorld, T0, T1, T2, T3>(World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR2<TWorld, T0, T1, T2, T3>(World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, in T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR3<TWorld, T0, T1, T2, T3>(World<TWorld>.Entity entity, ref T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR4<TWorld, T0, T1, T2, T3>(World<TWorld>.Entity entity, in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR1<TWorld, T0, T1, T2, T3, T4>(World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR2<TWorld, T0, T1, T2, T3, T4>(World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR3<TWorld, T0, T1, T2, T3, T4>(World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR4<TWorld, T0, T1, T2, T3, T4>(World<TWorld>.Entity entity, ref T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR5<TWorld, T0, T1, T2, T3, T4>(World<TWorld>.Entity entity, in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR1<TWorld, T0, T1, T2, T3, T4, T5>(World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR2<TWorld, T0, T1, T2, T3, T4, T5>(World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR3<TWorld, T0, T1, T2, T3, T4, T5>(World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR4<TWorld, T0, T1, T2, T3, T4, T5>(World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR5<TWorld, T0, T1, T2, T3, T4, T5>(World<TWorld>.Entity entity, ref T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithEntity{TWorld}"/>
    public delegate void QueryFunctionWithEntityR6<TWorld, T0, T1, T2, T3, T4, T5>(World<TWorld>.Entity entity, in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where TWorld : struct, IWorldType;

    #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
    #endregion

    #region QUERY_FUNCTION_WITH_REF_DATA_ENTITY
    /// <summary>
    /// Delegate for query iteration that combines both <c>ref TData</c> user data and the entity handle,
    /// alongside component references. This is the most feature-complete delegate form — use it when
    /// you need the entity handle for structural operations AND want to pass external mutable state
    /// without closure allocations.
    /// <para>
    /// Combines the benefits of <see cref="QueryFunctionWithRefData{TData, T0}"/> (zero-allocation state passing)
    /// and <see cref="QueryFunctionWithEntity{TWorld}"/> (entity access for Add/Delete/Destroy/tag operations).
    /// Ideal for complex iteration logic that both accumulates results and performs entity mutations.
    /// </para>
    /// </summary>
    /// <typeparam name="TData">Arbitrary user data type passed by reference.</typeparam>
    /// <typeparam name="TWorld">The world type.</typeparam>
    public delegate void QueryFunctionWithRefDataEntity<TData, TWorld>(ref TData data, World<TWorld>.Entity entity)
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntity<TData, TWorld, T0>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0)
        where T0 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntity<TData, TWorld, T0, T1>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntity<TData, TWorld, T0, T1, T2>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntity<TData, TWorld, T0, T1, T2, T3>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntity<TData, TWorld, T0, T1, T2, T3, T4>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntity<TData, TWorld, T0, T1, T2, T3, T4, T5>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where TWorld : struct, IWorldType;

    #if !FFS_ECS_DISABLE_CHANGED_TRACKING
    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR1<TData, TWorld, T0>(ref TData data, World<TWorld>.Entity entity, in T0 comp0)
        where T0 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR1<TData, TWorld, T0, T1>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, in T1 comp1)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR2<TData, TWorld, T0, T1>(ref TData data, World<TWorld>.Entity entity, in T0 comp0, in T1 comp1)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR1<TData, TWorld, T0, T1, T2>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, in T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR2<TData, TWorld, T0, T1, T2>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, in T1 comp1, in T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR3<TData, TWorld, T0, T1, T2>(ref TData data, World<TWorld>.Entity entity, in T0 comp0, in T1 comp1, in T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR1<TData, TWorld, T0, T1, T2, T3>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR2<TData, TWorld, T0, T1, T2, T3>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, in T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR3<TData, TWorld, T0, T1, T2, T3>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR4<TData, TWorld, T0, T1, T2, T3>(ref TData data, World<TWorld>.Entity entity, in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR1<TData, TWorld, T0, T1, T2, T3, T4>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR2<TData, TWorld, T0, T1, T2, T3, T4>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR3<TData, TWorld, T0, T1, T2, T3, T4>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR4<TData, TWorld, T0, T1, T2, T3, T4>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR5<TData, TWorld, T0, T1, T2, T3, T4>(ref TData data, World<TWorld>.Entity entity, in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR1<TData, TWorld, T0, T1, T2, T3, T4, T5>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR2<TData, TWorld, T0, T1, T2, T3, T4, T5>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR3<TData, TWorld, T0, T1, T2, T3, T4, T5>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR4<TData, TWorld, T0, T1, T2, T3, T4, T5>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, ref T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR5<TData, TWorld, T0, T1, T2, T3, T4, T5>(ref TData data, World<TWorld>.Entity entity, ref T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="QueryFunctionWithRefDataEntity{TData, TWorld}"/>
    public delegate void QueryFunctionWithRefDataEntityR6<TData, TWorld, T0, T1, T2, T3, T4, T5>(ref TData data, World<TWorld>.Entity entity, in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where TWorld : struct, IWorldType;

    #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
    #endregion
    
    #region SEARCH_FUNCTION_WITH_ENTITY
    /// <summary>
    /// Delegate for query search/find operations that returns <c>bool</c> to indicate whether
    /// the target entity was found. Used with query <c>Search</c> method that
    /// iterate over matching entities and stop at the first one where this delegate returns <c>true</c>.
    /// <para>
    /// Unlike <see cref="QueryFunctionWithEntity{TWorld}"/> which visits all entities,
    /// this delegate enables early termination — the query stops as soon as the callback returns <c>true</c>.
    /// Use for finding a specific entity by condition (e.g., the nearest enemy, first entity with HP below threshold).
    /// Return <c>false</c> to continue searching, <c>true</c> to stop and report the entity as found.
    /// </para>
    /// </summary>
    /// <typeparam name="TWorld">The world type.</typeparam>
    /// <returns><c>true</c> to stop iteration (entity found); <c>false</c> to continue searching.</returns>
    public delegate bool SearchFunctionWithEntity<TWorld>(World<TWorld>.Entity entity)
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="SearchFunctionWithEntity{TWorld}"/>
    public delegate bool SearchFunctionWithEntity<TWorld, T0>(World<TWorld>.Entity entity, in T0 comp0)
        where T0 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="SearchFunctionWithEntity{TWorld}"/>
    public delegate bool SearchFunctionWithEntity<TWorld, T0, T1>(World<TWorld>.Entity entity, in T0 comp0, in T1 comp1)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="SearchFunctionWithEntity{TWorld}"/>
    public delegate bool SearchFunctionWithEntity<TWorld, T0, T1, T2>(World<TWorld>.Entity entity, in T0 comp0, in T1 comp1, in T2 comp2)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="SearchFunctionWithEntity{TWorld}"/>
    public delegate bool SearchFunctionWithEntity<TWorld, T0, T1, T2, T3>(World<TWorld>.Entity entity, in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="SearchFunctionWithEntity{TWorld}"/>
    public delegate bool SearchFunctionWithEntity<TWorld, T0, T1, T2, T3, T4>(World<TWorld>.Entity entity, in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where TWorld : struct, IWorldType;

    /// <inheritdoc cref="SearchFunctionWithEntity{TWorld}"/>
    public delegate bool SearchFunctionWithEntity<TWorld, T0, T1, T2, T3, T4, T5>(World<TWorld>.Entity entity, in T0 comp0, in T1 comp1, in T2 comp2, in T3 comp3, in T4 comp4, in T5 comp5)
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where TWorld : struct, IWorldType;

    #endregion
}
