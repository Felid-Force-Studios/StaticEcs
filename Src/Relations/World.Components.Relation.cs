#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

using System;
using System.Collections.Generic;
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
            private static void ValidateComponentRegistration<L>() where L : struct, IComponent {
                if (Components<L>.Value.IsRegistered()) throw new StaticEcsException($"Component {typeof(L)} already registered");
            }
            
            private static void OnAddMultiLink<T>(Entity _, ref T component) where T : struct, IEntityLinksComponent<T> {
                Context<MultiComponents<EntityGID>>.Get().Add(ref component.RefValue(ref component).multi);
            }
            
            private static OnCopyHandler<T> OnCopyOneHandler<T>(CopyStrategy strategy, OnCopyHandler<T> handler) where T : struct, IEntityLinkComponent<T> {
                if (strategy == CopyStrategy.DeepCopy) {
                    if (handler != null) {
                        return (Entity srcEntity, Entity dstEntity, ref T src, ref T dst) => {
                            handler(srcEntity, dstEntity, ref src, ref dst);
                            DeepCopy(srcEntity, dstEntity, ref src, ref dst);
                        };
                    }

                    return DeepCopy;
                }

                return handler;
                
                static void DeepCopy(Entity srcEntity, Entity dstEntity, ref T src, ref T dst) {
                    if (src.RefValue(ref src).TryUnpack<WorldType>(out var srcE)) {
                        dst.RefValue(ref dst) = srcE.Clone().Gid();
                    }
                }
            }
            
            private static OnCopyHandler<T> OnCopyManyHandler<T>(CopyStrategy strategy, OnCopyHandler<T> handler) where T : struct, IEntityLinksComponent<T> {
                if (strategy == CopyStrategy.DeepCopy) {
                    if (handler != null) {
                        return (Entity srcEntity, Entity dstEntity, ref T src, ref T dst) => {
                            handler(srcEntity, dstEntity, ref src, ref dst);
                            DeepCopyMulti(srcEntity, dstEntity, ref src, ref dst);
                        };
                    }

                    return DeepCopyMulti;
                }

                if (strategy == CopyStrategy.Default) {
                    if (handler != null) {
                        throw new StaticEcsException($"Use CopyStrategy.Custom if a custom method of copying a component is required {typeof(T)}");
                    }

                    return DefaultMultiCopy;
                }

                return handler;
                
                static void DeepCopyMulti(Entity srcEntity, Entity dstEntity, ref T src, ref T dst) {
                    ref var srsItems = ref src.RefValue(ref src).multi;
                    ref var dstItems = ref dst.RefValue(ref dst).multi;
                    dstItems.Clear();
                    foreach (var srsItem in srsItems) {
                        if (srsItem.TryUnpack<WorldType>(out var srcE)) {
                            dstItems.Add(srcE.Clone().Gid());
                        }
                    }
                }
                
                static void DefaultMultiCopy(Entity srcEntity, Entity dstEntity, ref T src, ref T dst) {
                    ref var srsItems = ref src.RefValue(ref src).multi;
                    var dstItemsTemp = dst.RefValue(ref dst).multi;
                    dst = src;
                    ref var dstItems = ref dst.RefValue(ref dst).multi;
                    dstItems = dstItemsTemp;
                    dstItems.Clear();
                    dstItems.Add(ref srsItems);
                }
            }
            
            private static void CheckOneRelation<T>(Entity entity, ref T component) where T : struct, IEntityLinkComponent<T> {
                var gid = component.RefValue(ref component);
                if (!gid.TryUnpack<WorldType>(out var _)) {
                    throw new StaticEcsException($"Not actual entity adding with relation component {typeof(T)} to {entity}");
                }
                
                while (gid.TryUnpack<WorldType>(out var e)) {
                    if (e == entity) {
                        throw new StaticEcsException($"Cyclic relation between when adding a component {typeof(T)} to {entity}");
                    }

                    if (Components<T>.Value.Has(e)) {
                        ref var link = ref Components<T>.Value.Ref(e);
                        gid = link.RefValue(ref link);
                    } else {
                        break;
                    }
                }
            }
            
            private static void CheckManyRelation<T>(Entity entity, Entity link) where T : struct, IEntityLinksComponent<T> {
                if (link == entity) {
                    throw new StaticEcsException($"Cyclic relation between when adding a component {typeof(T)} to {entity}");
                }

                if (Components<T>.Value.Has(link)) {
                    ref var val = ref Components<T>.Value.Ref(link);
                    ref var multi = ref val.RefValue(ref val);
                    foreach (var gid in multi) {
                        if (gid.TryUnpack<WorldType>(out var e)) {
                            CheckManyRelation<T>(entity, e);
                        }
                    }
                }
            }

            private static OnComponentHandler<T> DestroyLinkedEntityHandler<T>(OnComponentHandler<T> handler) where T : struct, IEntityLinkComponent<T> {
                Context<DestroyEntityLoopAccess<T>>.Set(new(false));

                if (handler != null) {
                    return (Entity entity, ref T component) => {
                        handler(entity, ref component);
                        DestroyLinkedEntityOne(entity, ref component);
                    };
                }

                return DestroyLinkedEntityOne;
                
                static void DestroyLinkedEntityOne(Entity e, ref T component) {
                    Context<DestroyEntityLoopAccess<T>>.Get().DeepDestroy(ref component);
                }
            }

            private static OnComponentHandler<T> DestroyLinkedEntitiesHandler<T>(OnComponentHandler<T> handler) where T : struct, IEntityLinksComponent<T> {
                Context<DestroyEntityLoopMultiAccess<T>>.Set(new(false));

                if (handler != null) {
                    return (Entity entity, ref T component) => {
                        handler(entity, ref component);
                        DestroyLinkedEntityMany(entity, ref component);
                    };
                }

                return DestroyLinkedEntityMany;
                
                static void DestroyLinkedEntityMany(Entity e, ref T component) {
                    Context<DestroyEntityLoopMultiAccess<T>>.Get().DeepDestroy(ref component);
                    Context<MultiComponents<EntityGID>>.Get().Delete(ref component.RefValue(ref component).multi);
                }
            }
        }
        
        internal delegate void EntityOnAddLinkAction(Entity a, Entity b);
        internal delegate void EntityOnDeleteLinkAction(Entity a, EntityGID b);
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        internal struct LinkManyHandlers<T> where T : struct, IEntityLinksComponent<T> {
            internal EntityOnAddLinkAction OnAddLinkItem;
            internal EntityOnDeleteLinkAction OnDeleteLinkItem;
        }
        
        #if FFS_ECS_DEBUG && !FFS_ECS_DISABLE_RELATION_CHECK
        internal struct CyclicCheck<T> where T : struct, IComponent {
            internal static bool Value;
        }
        #endif

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        internal struct DestroyEntityLoopMultiAccess<T> where T : struct, IEntityLinksComponent<T> {
            internal readonly Stack<(int, int)> Ranges;
            internal readonly Stack<Entity> ForDelete;
            internal bool Active;

            public DestroyEntityLoopMultiAccess(bool active) {
                Active = active;
                Ranges = new Stack<(int, int)>(64);
                ForDelete = new Stack<Entity>(64);
            }

            public void DeepDestroy(ref T component) {
                if (!Active) {
                    Active = true;

                    ref var multi = ref component.RefValue(ref component).multi;
                    var data = multi.data;
                    Ranges.Push(((int, int)) (multi.offset, multi.count));

                    while (Ranges.Count > 0) {
                        var (from, count) = Ranges.Pop();
                        for (var i = from + count - 1; i >= from; i--) {
                            var gid = data.values[i];
                            if (gid.TryUnpack<WorldType>(out var e)) {
                                if (e.HasAllOf<T>()) {
                                    ref var linksComponent = ref e.Ref<T>();
                                    ref var targets = ref linksComponent.RefValue(ref linksComponent).multi;
                                    Ranges.Push(((int, int)) (targets.offset, targets.count));
                                }

                                ForDelete.Push(e);
                            }
                        }
                    }

                    while (ForDelete.Count > 0) {
                        ForDelete.Pop().Destroy();
                    }

                    Active = false;
                }
            }

            public void DeepDestroy(EntityGID gid) {
                if (!Active) {
                    Active = true;
                    
                    if (gid.TryUnpack<WorldType>(out var ent)) {
                        if (ent.HasAllOf<T>()) {
                            ref var link = ref ent.Ref<T>();
                            ref var multi = ref link.RefValue(ref link).multi;
                            var data = multi.data;
                            Ranges.Push(((int, int)) (multi.offset, multi.count));

                            while (Ranges.Count > 0) {
                                var (from, count) = Ranges.Pop();
                                for (var i = from + count - 1; i >= from; i--) {
                                    gid = data.values[i];
                                    if (gid.TryUnpack<WorldType>(out var e)) {
                                        if (e.HasAllOf<T>()) {
                                            ref var linksComponent = ref e.Ref<T>();
                                            ref var targets = ref linksComponent.RefValue(ref linksComponent).multi;
                                            Ranges.Push(((int, int)) (targets.offset, targets.count));
                                        }

                                        ForDelete.Push(e);
                                    }
                                }
                            }

                            while (ForDelete.Count > 0) {
                                ForDelete.Pop().Destroy();
                            }
                        }
                    } 

                    Active = false;
                }
            }
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        internal struct DestroyEntityLoopAccess<T> where T : struct, IEntityLinkComponent<T> {
            internal bool Active;

            public DestroyEntityLoopAccess(bool active) {
                Active = active;
            }

            [MethodImpl(AggressiveInlining)]
            public void DeepDestroy(ref T component) {
                if (!Active) {
                    Active = true;
                    Process(component.RefValue(ref component));
                    Active = false;
                }
            }

            [MethodImpl(AggressiveInlining)]
            public void DeepDestroy(EntityGID current) {
                if (!Active) {
                    Active = true;
                    Process(current);
                    Active = false;
                }
            }

            [MethodImpl(AggressiveInlining)]
            private static void Process(EntityGID current) {
                while (current.TryUnpack<WorldType>(out var e)) {
                    if (!e.HasAllOf<T>()) {
                        e.Destroy();
                        break;
                    }

                    ref var link = ref e.Ref<T>();
                    current = link.RefValue(ref link);
                    e.Destroy();
                }
            }
        }
    }
    
    #if FFS_ECS_DEBUG && !FFS_ECS_DISABLE_RELATION_CHECK
    internal static class RelationExtension {
        [MethodImpl(AggressiveInlining)]
        internal static EntityGID Link<L>(this ref L value) where L : struct, IEntityLinkComponent<L> {
            return value.RefValue(ref value);
        }
    }
    #endif
    
    public enum OneDirectionalDeleteStrategy {
        Default,
        DestroyLinkedEntity
    }

    public enum BiDirectionalDeleteStrategy {
        Default,
        DestroyLinkedEntity,
        DeleteAnotherLink
    }

    public enum CopyStrategy {
        Default,
        Custom,
        NotCopy,
        DeepCopy,
    }
}