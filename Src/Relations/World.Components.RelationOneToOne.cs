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
        internal partial struct ModuleComponents {
            [MethodImpl(AggressiveInlining)]
            internal void RegisterOneToOneRelationType<L, R>(
                IComponentConfig<L, WorldType> leftConfig,
                IComponentConfig<R, WorldType> rightConfig,
                BiDirectionalDeleteStrategy leftDeleteStrategy = BiDirectionalDeleteStrategy.Default,
                BiDirectionalDeleteStrategy rightDeleteStrategy = BiDirectionalDeleteStrategy.Default,
                CopyStrategy leftCopyStrategy = CopyStrategy.Default,
                CopyStrategy rightCopyStrategy = CopyStrategy.Default,
                bool disableRelationsCheckLeftDebug = false, bool disableRelationsCheckRightDebug = false
            )
                where L : struct, IEntityLinkComponent<L>
                where R : struct, IEntityLinkComponent<R> {
                ValidateComponentRegistration<L>();
                ValidateComponentRegistration<R>();
                
                #if (((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)) && !FFS_ECS_DISABLE_RELATION_CHECK
                CyclicCheck<L>.Value = !disableRelationsCheckLeftDebug && typeof(L) != typeof(R);
                #endif
                
                var actualConfigLeft = new ValueComponentConfig<L, WorldType>(leftConfig) {
                    OnPutHandler = OnPutHandler<L, R>(leftConfig.OnPut(), disableRelationsCheckLeftDebug),
                    OnDeleteHandler = OnDeleteHandler<L, R>(leftDeleteStrategy, leftConfig.OnDelete()),
                    OnCopyHandler = OnCopyOneHandler(leftCopyStrategy, leftConfig.OnCopy()),
                    OnAddHandler = OnPutHandler<L, R>(leftConfig.OnAdd(), disableRelationsCheckLeftDebug),
                    Copyable = leftCopyStrategy != CopyStrategy.NotCopy
                };

                RegisterComponentType(
                    actualConfigLeft
                );
                Components<L>.Value.addWithoutValueError = "not allowed for relation components, use entity.SetLink() or entity.Put(value) or entity.Add(value)";
                

                if (Components<R>.Value.IsRegistered()) {
                    // Same types
                    return;
                }
                
                #if (((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)) && !FFS_ECS_DISABLE_RELATION_CHECK
                CyclicCheck<R>.Value = !disableRelationsCheckRightDebug;
                #endif
                
                var actualConfigRight = new ValueComponentConfig<R, WorldType>(rightConfig) {
                    OnPutHandler = OnPutHandler<R, L>(rightConfig.OnPut(), disableRelationsCheckRightDebug),
                    OnDeleteHandler = OnDeleteHandler<R, L>(rightDeleteStrategy, rightConfig.OnDelete()),
                    OnCopyHandler = OnCopyOneHandler(rightCopyStrategy, rightConfig.OnCopy()),
                    OnAddHandler = OnPutHandler<R, L>(rightConfig.OnAdd(), disableRelationsCheckRightDebug),
                    Copyable = rightCopyStrategy != CopyStrategy.NotCopy
                };

                RegisterComponentType(
                    actualConfigRight
                );
                Components<R>.Value.addWithoutValueError = "not allowed for relation components, use entity.SetLink() or entity.Put(value) or entity.Add(value)";
                return;

                static OnComponentHandler<A> OnPutHandler<A, B>(OnComponentHandler<A> handler, bool disableRelationsCheck)
                    where A : struct, IEntityLinkComponent<A>
                    where B : struct, IEntityLinkComponent<B> {

                    if (handler != null) {
                        return (Entity entity, ref A component) => {
                            handler(entity, ref component);
                            SetAnotherLink(entity, ref component);
                        };
                    }

                    return SetAnotherLink;

                    [MethodImpl(AggressiveInlining)]
                    static void SetAnotherLink(Entity e, ref A component) {
                        #if (((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)) && !FFS_ECS_DISABLE_RELATION_CHECK
                        if (CyclicCheck<A>.Value) {
                            CheckOneRelation(e, ref component);
                        }
                        #endif
                        var unpacked = component.RefValue(ref component).Unpack<WorldType>();
                        var gid = e.Gid();
                        if (Components<B>.Value.Has(unpacked)) {
                            ref var link = ref Components<B>.Value.Ref(unpacked);
                            if (link.RefValue(ref link).Equals(gid)) {
                                return;
                            }
                            if (Components<B>.Value.onDeleteHandler != null) {
                                Components<B>.Value.onDeleteHandler(unpacked, ref link);
                            } else {
                                link = default;
                            }

                            link.RefValue(ref link) = gid;
                            Components<B>.Value.onPutHandler?.Invoke(unpacked, ref link);
                            return;
                        }
                        var value = default(B);
                        value.RefValue(ref value) = gid;
                        Components<B>.Value.Add(unpacked, value);
                    }
                }

                static OnComponentHandler<A> OnDeleteHandler<A, B>(BiDirectionalDeleteStrategy strategy, OnComponentHandler<A> handler)
                    where A : struct, IEntityLinkComponent<A>
                    where B : struct, IEntityLinkComponent<B> {
                    if (strategy == BiDirectionalDeleteStrategy.Default) {
                        return handler;
                    }

                    if (strategy == BiDirectionalDeleteStrategy.DestroyLinkedEntity) {
                        return DestroyLinkedEntityHandler(handler);
                    }

                    if (strategy == BiDirectionalDeleteStrategy.DeleteAnotherLink) {
                        if (handler != null) {
                            return (Entity entity, ref A component) => {
                                handler(entity, ref component);
                                DeleteAnotherLink(entity, ref component);
                            };
                        }

                        return DeleteAnotherLink;
                    }

                    throw new StaticEcsException("Unsupported onDelete strategy");

                    static void DeleteAnotherLink(Entity e, ref A component) {
                        if (component.RefValue(ref component).TryUnpack<WorldType>(out var ent) && ent.HasAllOf<B>()) {
                            #if (DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_RELATION_CHECK
                            var link = ent.Ref<B>().Link();
                            if (link != e.Gid()) {
                                throw new StaticEcsException($"When another link of type {typeof(B)} is deleted, the link refers to another entity {link}, not {e}");
                            }
                            #endif
                            Components<B>.Value.Delete(ent);
                        }
                    }
                }
            }
        }
    }
}
