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
            internal void RegisterOneToManyRelationType<O, M>(
                ushort defaultComponentCapacity,
                IComponentConfig<O, WorldType> leftConfig,
                IComponentConfig<M, WorldType> rightConfig,
                BiDirectionalDeleteStrategy leftDeleteStrategy = BiDirectionalDeleteStrategy.Default,
                BiDirectionalDeleteStrategy rightDeleteStrategy = BiDirectionalDeleteStrategy.Default,
                CopyStrategy leftCopyStrategy = CopyStrategy.Default,
                CopyStrategy rightCopyStrategy = CopyStrategy.Default,
                bool disableRelationsCheckLeftDebug = false, bool disableRelationsCheckRightDebug = false
            )
                where O : struct, IEntityLinkComponent<O>
                where M : struct, IEntityLinksComponent<M> {
                ValidateComponentRegistration<O>();
                
                var actualConfigLeft = new ValueComponentConfig<O, WorldType>(leftConfig) {
                    OnPutHandler = OnPutLeftHandle(leftConfig.OnPut(), disableRelationsCheckLeftDebug),
                    OnDeleteHandler = OnDeleteHandlerOneToMany(leftDeleteStrategy, leftConfig.OnDelete()),
                    OnCopyHandler = OnCopyOneHandler(leftCopyStrategy, leftConfig.OnCopy()),
                    OnAddHandler = OnPutLeftHandle(leftConfig.OnAdd(), disableRelationsCheckLeftDebug),
                    Copyable = leftCopyStrategy != CopyStrategy.NotCopy
                };

                RegisterComponentType(
                    actualConfigLeft
                );
                Components<O>.Value.addWithoutValueError = "not allowed for relation components, use entity.SetLink() or entity.Put(value) or entity.Add(value)";

                ValidateComponentRegistration<M>();

                Context.Value.GetOrCreate<LinkManyHandlers<M>>();
                Context.Value.Get<LinkManyHandlers<M>>().OnAddLinkItem = SetLeftLink;

                if (rightDeleteStrategy == BiDirectionalDeleteStrategy.DestroyLinkedEntity) {
                    Context.Value.Get<LinkManyHandlers<M>>().OnDeleteLinkItem = DeepLeftDestroy;
                } else if (rightDeleteStrategy == BiDirectionalDeleteStrategy.DeleteAnotherLink) {
                    Context.Value.Get<LinkManyHandlers<M>>().OnDeleteLinkItem = DeleteLeftLink;
                }

                RegisterMultiComponentsData<EntityGID>(defaultComponentCapacity);
                
                var actualConfigRight = new ValueComponentConfig<M, WorldType>(rightConfig) {
                    OnPutHandler = OnAddManyHandler(rightConfig.OnPut(), disableRelationsCheckRightDebug),
                    OnDeleteHandler = OnDeleteHandlerManyToOne(rightDeleteStrategy, rightConfig.OnDelete()),
                    OnCopyHandler = OnCopyManyHandler(rightCopyStrategy, rightConfig.OnCopy()),
                    OnAddHandler = OnAddManyHandler(rightConfig.OnAdd(), disableRelationsCheckRightDebug),
                    Copyable = rightCopyStrategy != CopyStrategy.NotCopy
                };

                RegisterComponentType(
                    actualConfigRight
                );

                return;

                static void SetLeftLink(Entity e, Entity link) {
                    #if (DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_RELATION_CHECK
                    if (CyclicCheck<M>.Value) {
                        CheckManyRelation<M>(e, link);
                    }
                    #endif
                    var gid = e.Gid();
                    if (Components<O>.Value.Has(link)) {
                        ref var val = ref Components<O>.Value.Ref(link);
                        if (val.RefValue(ref val).Equals(gid)) {
                            return;
                        }
                        if (Components<O>.Value.onDeleteHandler != null) {
                            Components<O>.Value.onDeleteHandler(link, ref val);
                        } else {
                            val = default;
                        }

                        val.RefValue(ref val) = e.Gid();
                        Components<O>.Value.onPutHandler?.Invoke(link, ref val);
                        return;
                    }
                    var value = default(O);
                    value.RefValue(ref value) = e.Gid();
                    Components<O>.Value.Add(link, value);
                }

                static void DeepLeftDestroy(Entity _, EntityGID entity) {
                    Context<DestroyEntityLoopAccess<O>>.Get().DeepDestroy(entity);
                }

                static void DeleteLeftLink(Entity e, EntityGID child) {
                    if (child.TryUnpack<WorldType>(out var ent) && ent.HasAllOf<O>()) {
                        #if (DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_RELATION_CHECK
                        var link = ent.Ref<O>().Link();
                        if (link != e.Gid()) {
                            throw new StaticEcsException($"When another link of type {typeof(O)} is deleted, the link refers to another entity {link}, not {e}");
                        }
                        #endif
                        Components<O>.Value.Delete(ent);
                    }
                }

                static OnComponentHandler<O> OnPutLeftHandle(OnComponentHandler<O> handler, bool disableRelationsCheck) {
                    #if (DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_RELATION_CHECK
                    CyclicCheck<O>.Value = !disableRelationsCheck;
                    #endif

                    if (handler != null) {
                        return (Entity entity, ref O component) => {
                            handler(entity, ref component);
                            SetRightLink(entity, ref component);
                        };
                    }

                    return SetRightLink;

                    static void SetRightLink(Entity e, ref O component) {
                        #if (DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_RELATION_CHECK
                        if (CyclicCheck<O>.Value) {
                            CheckOneRelation(e, ref component);
                        }
                        #endif
                        var unpacked = component.RefValue(ref component).Unpack<WorldType>();
                        var entityGid = e.Gid();
                        if (!unpacked.HasAllOf<M>()) {
                            ref var links = ref Components<M>.Value.Add(unpacked);
                            links.RefValue(ref links).multi.Add(entityGid);
                            Context.Value.Get<LinkManyHandlers<M>>().OnAddLinkItem?.Invoke(unpacked, e);
                        } else {
                            ref var links = ref Components<M>.Value.Ref(unpacked);
                            ref var multi = ref links.RefValue(ref links).multi;
                            if (!multi.Contains(entityGid)) {
                                multi.Add(entityGid);
                                Context.Value.Get<LinkManyHandlers<M>>().OnAddLinkItem?.Invoke(unpacked, e);
                            }
                        }
                    }
                }

                static OnComponentHandler<M> OnAddManyHandler(OnComponentHandler<M> handler, bool disableRelationsCheckRightDebug) {
                    #if (DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_RELATION_CHECK
                    CyclicCheck<M>.Value = !disableRelationsCheckRightDebug;
                    #endif

                    if (handler != null) {
                        return (Entity entity, ref M component) => {
                            handler(entity, ref component);
                            OnAddMultiLink(entity, ref component);
                        };
                    }

                    return OnAddMultiLink;
                }

                static OnComponentHandler<O> OnDeleteHandlerOneToMany(BiDirectionalDeleteStrategy strategy, OnComponentHandler<O> handler) {
                    if (strategy == BiDirectionalDeleteStrategy.Default) {
                        return handler;
                    }

                    if (strategy == BiDirectionalDeleteStrategy.DestroyLinkedEntity) {
                        return DestroyLinkedEntityHandler(handler);
                    }

                    if (strategy == BiDirectionalDeleteStrategy.DeleteAnotherLink) {
                        if (handler != null) {
                            return (Entity entity, ref O component) => {
                                handler(entity, ref component);
                                DeleteLink(entity, ref component);
                            };
                        }

                        return DeleteLink;
                    }

                    throw new StaticEcsException("Unsupported onDelete strategy");

                    static void DeleteLink(Entity e, ref O component) {
                        if (component.RefValue(ref component).TryUnpack<WorldType>(out var ent) && ent.HasAllOf<M>()) {
                            ref var comp = ref ent.Ref<M>();
                            ref var multi = ref comp.RefValue(ref comp).multi;
                            multi.TryRemove(e.Gid());
                            if (multi.IsEmpty()) {
                                Components<M>.Value.Delete(ent);
                            }
                        }
                    }
                }

                static OnComponentHandler<M> OnDeleteHandlerManyToOne(BiDirectionalDeleteStrategy strategy, OnComponentHandler<M> handler) {
                    if (strategy == BiDirectionalDeleteStrategy.Default) {
                        return handler;
                    }

                    if (strategy == BiDirectionalDeleteStrategy.DestroyLinkedEntity) {
                        return DestroyLinkedEntitiesHandler(handler);
                    }

                    if (strategy == BiDirectionalDeleteStrategy.DeleteAnotherLink) {
                        if (handler != null) {
                            return (Entity entity, ref M component) => {
                                handler(entity, ref component);
                                DeleteLink(entity, ref component);
                            };
                        }

                        return DeleteLink;
                    }

                    throw new StaticEcsException("Unsupported onDelete strategy");
                    
                    static void DeleteLink(Entity e, ref M component) {
                        foreach (var gid in component.RefValue(ref component)) {
                            if (gid.TryUnpack<WorldType>(out var unpacked) && unpacked.HasAllOf<O>()) {
                                #if (DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_RELATION_CHECK
                                var link = unpacked.Ref<O>().Link();
                                if (link != e.Gid()) {
                                    throw new StaticEcsException($"When another link of type {typeof(O)} is deleted, the link refers to another entity {link}, not {e}");
                                }
                                #endif
                                Components<O>.Value.Delete(unpacked);
                            }
                        }
                    }
                }
            }
        }
    }
}
