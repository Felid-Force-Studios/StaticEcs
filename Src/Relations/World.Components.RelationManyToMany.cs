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
            internal void RegisterManyToManyRelationType<L, R>(
                ushort defaultComponentCapacity,
                uint capacity,
                IComponentConfig<L, WorldType> leftConfig,
                IComponentConfig<R, WorldType> rightConfig,
                BiDirectionalDeleteStrategy leftDeleteStrategy = BiDirectionalDeleteStrategy.Default,
                BiDirectionalDeleteStrategy rightDeleteStrategy = BiDirectionalDeleteStrategy.Default,
                CopyStrategy leftCopyStrategy = CopyStrategy.Default,
                CopyStrategy rightCopyStrategy = CopyStrategy.Default,
                bool disableRelationsCheckLeftDebug = false, bool disableRelationsCheckRightDebug = false
            )
                where L : struct, IEntityLinksComponent<L>
                where R : struct, IEntityLinksComponent<R> {
                
                ValidateComponentRegistration<L>();
                RegisterMultiComponentsData<EntityGID>(defaultComponentCapacity, capacity);

                #if (DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_RELATION_CHECK
                CyclicCheck<L>.Value = !disableRelationsCheckLeftDebug;
                #endif
                AddItemsHandlers<L, R>(leftDeleteStrategy);
                
                var actualConfigLeft = new ValueComponentConfig<L, WorldType>(leftConfig) {
                    OnAddWithValueHandler = OnAddHandler(leftConfig.OnAdd()),
                    OnDeleteHandler = OnDeleteHandlerManyToMany<L, R>(leftDeleteStrategy, leftConfig.OnDelete()),
                    OnCopyHandler = OnCopyManyHandler(leftCopyStrategy, leftConfig.OnCopy()),
                    OnAddHandler = OnAddHandler(leftConfig.OnAdd()),
                    Copyable = leftCopyStrategy != CopyStrategy.NotCopy
                };

                RegisterComponentType(
                    capacity: capacity,
                    actualConfigLeft,
                    putNotAllowed: true
                );

                ValidateComponentRegistration<R>();
                RegisterMultiComponentsData<EntityGID>(defaultComponentCapacity, capacity);

                #if (DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_RELATION_CHECK
                CyclicCheck<R>.Value = !disableRelationsCheckRightDebug;
                #endif
                AddItemsHandlers<R, L>(rightDeleteStrategy);
                
                var actualConfigRight = new ValueComponentConfig<R, WorldType>(rightConfig) {
                    OnAddWithValueHandler = OnAddHandler(rightConfig.OnAdd()),
                    OnDeleteHandler = OnDeleteHandlerManyToMany<R, L>(rightDeleteStrategy, rightConfig.OnDelete()),
                    OnCopyHandler = OnCopyManyHandler(rightCopyStrategy, rightConfig.OnCopy()),
                    OnAddHandler = OnAddHandler(rightConfig.OnAdd()),
                    Copyable = rightCopyStrategy != CopyStrategy.NotCopy
                };

                RegisterComponentType(
                    capacity: capacity,
                    actualConfigRight,
                    putNotAllowed: true
                );

                return;
                
                static void AddItemsHandlers<A, B>(BiDirectionalDeleteStrategy strategy) where A : struct, IEntityLinksComponent<A> where B : struct, IEntityLinksComponent<B> {
                    Context.Value.GetOrCreate<LinkManyHandlers<A>>().OnAddLinkItem = SetAnotherLink<A, B>;

                    if (strategy == BiDirectionalDeleteStrategy.DestroyLinkedEntity) {
                        Context<DestroyEntityLoopMultiAccess<B>>.Set(new(false));
                        Context.Value.Get<LinkManyHandlers<A>>().OnDeleteLinkItem = DeepDestroy<A>;
                    } else if (strategy == BiDirectionalDeleteStrategy.DeleteAnotherLink) {
                        Context.Value.Get<LinkManyHandlers<A>>().OnDeleteLinkItem = DeleteAnotherLink<B>;
                    }
                }
                
                static void SetAnotherLink<Current, Another>(Entity e, Entity link) where Current : struct, IEntityLinksComponent<Current> where Another : struct, IEntityLinksComponent<Another> {
                    #if (DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_RELATION_CHECK
                    if (CyclicCheck<Current>.Value) {
                        CheckManyRelation<Current>(e, link);
                    }
                    #endif
                    var entityGid = e.Gid();
                    if (!link.HasAllOf<Another>()) {
                        ref var links = ref Components<Another>.Value.Add(link);
                        links.RefValue(ref links).multi.Add(entityGid);
                        Context.Value.Get<LinkManyHandlers<Another>>().OnAddLinkItem?.Invoke(link, e);
                    } else {
                        ref var links = ref Components<Another>.Value.Ref(link);
                        ref var multi = ref links.RefValue(ref links).multi;
                        if (!multi.Contains(entityGid)) {
                            multi.Add(entityGid);
                            Context.Value.Get<LinkManyHandlers<Another>>().OnAddLinkItem?.Invoke(link, e);
                        }
                    }
                }
                
                static void DeleteAnotherLink<T>(Entity e, EntityGID child) where T : struct, IEntityLinksComponent<T> {
                    if (child.TryUnpack<WorldType>(out var childEntity) && childEntity.HasAllOf<T>()) {
                        ref var link = ref childEntity.Ref<T>();
                        ref var multi = ref link.RefValue(ref link).multi;
                        multi.TryRemove(e.Gid());
                        if (multi.IsEmpty()) {
                            Components<T>.Value.Delete(childEntity);
                        }
                    }
                }
                
                static void DeepDestroy<T>(Entity _, EntityGID entity) where T : struct, IEntityLinksComponent<T> {
                    Context<DestroyEntityLoopMultiAccess<T>>.Get().DeepDestroy(entity);
                }
                
                static OnComponentHandler<T> OnAddHandler<T>(OnComponentHandler<T> handler) where T : struct, IEntityLinksComponent<T> {
                    if (handler != null) {
                        return (Entity entity, ref T component) => {
                            handler(entity, ref component);
                            OnAddMultiLink(entity, ref component);
                        };
                    }

                    return OnAddMultiLink;
                }
                            
                static OnComponentHandler<A> OnDeleteHandlerManyToMany<A, B>(BiDirectionalDeleteStrategy strategy, OnComponentHandler<A> handler)
                    where B : struct, IEntityLinksComponent<B> 
                    where A : struct, IEntityLinksComponent<A> {
                    if (strategy == BiDirectionalDeleteStrategy.Default) {
                        return handler;
                    }

                    if (strategy == BiDirectionalDeleteStrategy.DestroyLinkedEntity) {
                        return DestroyLinkedEntitiesHandler(handler);
                    }

                    if (strategy == BiDirectionalDeleteStrategy.DeleteAnotherLink) {
                        if (handler != null) {
                            return (Entity entity, ref A component) => {
                                handler(entity, ref component);
                                DeleteLink(entity, ref component);
                            };
                        }
                    
                        return DeleteLink;
                    }

                    throw new StaticEcsException("Unsupported onDelete strategy");

                    static void DeleteLink(Entity e, ref A component) {
                        foreach (var gid in component.RefValue(ref component)) {
                            if (gid.TryUnpack<WorldType>(out var unpacked)) {
                                Components<B>.Value.TryDelete(unpacked);
                            }
                        }
                    }
                }
            }
        }
    }
}