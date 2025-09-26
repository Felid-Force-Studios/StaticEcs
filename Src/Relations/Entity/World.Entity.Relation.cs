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
        public partial struct Entity {
            
            [MethodImpl(AggressiveInlining)]
            public ref L SetLink<L>(EntityGID link) where L : struct, IEntityLinkComponent<L> {
                if (Components<L>.Value.Has(this)) {
                    Components<L>.Value.Delete(this);
                }
                
                var component = default(L);
                component.RefValue(ref component) = link;
                return ref Components<L>.Value.Add(this, component);
            }
            
            [MethodImpl(AggressiveInlining)]
            public ref L SetLink<L>(EntityGID link, out bool newLink) where L : struct, IEntityLinkComponent<L> {
                if (Components<L>.Value.Has(this)) {
                    ref var val = ref Components<L>.Value.Ref(this);
                    if (val.RefValue(ref val) == link) {
                        newLink = false;
                        return ref val;
                    }
                    
                    Components<L>.Value.Delete(this);
                }

                newLink = true;
                var component = default(L);
                component.RefValue(ref component) = link;
                return ref Components<L>.Value.Add(this, component);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void TryDeleteLink<L>() where L : struct, IEntityLinkComponent<L> {
                Components<L>.Value.TryDelete(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void TryDeleteLinks<L>() where L : struct, IEntityLinksComponent<L> {
                Components<L>.Value.TryDelete(this);
            }
            
            #region MULTI_LINKS
            [MethodImpl(AggressiveInlining)]
            public void TryDeleteLinks<L>(EntityGID link1) where L : struct, IEntityLinksComponent<L> {
                ref var component = ref Components<L>.Value.TryAdd(this);
                ref var multi = ref component.RefValue(ref component).multi;
                if (multi.TryRemoveSwap(link1)) {
                    Context.Value.Get<LinkManyHandlers<L>>().OnDeleteLinkItem?.Invoke(this, link1);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public void TryDeleteLinks<L>(EntityGID link1, EntityGID link2) where L : struct, IEntityLinksComponent<L> {
                ref var component = ref Components<L>.Value.TryAdd(this);
                ref var multi = ref component.RefValue(ref component).multi;

                var handler = Context.Value.Get<LinkManyHandlers<L>>().OnDeleteLinkItem;
                if (multi.TryRemoveSwap(link1)) {
                    handler?.Invoke(this, link1);
                }

                if (multi.TryRemoveSwap(link2)) {
                    handler?.Invoke(this, link2);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public void TryDeleteLinks<L>(EntityGID link1, EntityGID link2, EntityGID link3) where L : struct, IEntityLinksComponent<L> {
                ref var component = ref Components<L>.Value.TryAdd(this);
                ref var multi = ref component.RefValue(ref component).multi;
                var handler = Context.Value.Get<LinkManyHandlers<L>>().OnDeleteLinkItem;
                if (multi.TryRemoveSwap(link1)) {
                    handler?.Invoke(this, link1);
                }

                if (multi.TryRemoveSwap(link2)) {
                    handler?.Invoke(this, link2);
                }

                if (multi.TryRemoveSwap(link3)) {
                    handler?.Invoke(this, link3);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public void TryDeleteLinks<L>(EntityGID link1, EntityGID link2, EntityGID link3, EntityGID link4) where L : struct, IEntityLinksComponent<L> {
                ref var component = ref Components<L>.Value.TryAdd(this);
                ref var multi = ref component.RefValue(ref component).multi;
                var handler = Context.Value.Get<LinkManyHandlers<L>>().OnDeleteLinkItem;

                if (multi.TryRemoveSwap(link1)) {
                    handler?.Invoke(this, link1);
                }

                if (multi.TryRemoveSwap(link2)) {
                    handler?.Invoke(this, link2);
                }

                if (multi.TryRemoveSwap(link3)) {
                    handler?.Invoke(this, link3);
                }

                if (multi.TryRemoveSwap(link4)) {
                    handler?.Invoke(this, link4);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public ref L SetLinks<L>(EntityGID link1) where L : struct, IEntityLinksComponent<L> {
                ref var component = ref Components<L>.Value.TryAdd(this);
                ref var multi = ref component.RefValue(ref component).multi;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (multi.Contains(link1)) throw new StaticEcsException($"Link {link1} already contains in relations type {typeof(L)}, for {this}");
                #endif
                
                multi.Add(link1);
                Context.Value.Get<LinkManyHandlers<L>>().OnAddLinkItem?.Invoke(this, link1.Unpack<WorldType>());
                
                return ref component;
            }

            [MethodImpl(AggressiveInlining)]
            public ref L SetLinks<L>(EntityGID link1, EntityGID link2) where L : struct, IEntityLinksComponent<L> {
                ref var component = ref Components<L>.Value.TryAdd(this);
                ref var multi = ref component.RefValue(ref component).multi;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (multi.Contains(link1)) throw new StaticEcsException($"Link {link1} already contains in relations type {typeof(L)}, for {this}");
                multi.Add(link1);
                if (multi.Contains(link2)) throw new StaticEcsException($"Link {link2} already contains in relations type {typeof(L)}, for {this}");
                multi.Add(link2);
                #else
                multi.Add(link1, link2);
                #endif
                var handler = Context.Value.Get<LinkManyHandlers<L>>().OnAddLinkItem;
                if (handler != null) {
                    handler(this, link1.Unpack<WorldType>());
                    handler(this, link2.Unpack<WorldType>());
                }
                
                return ref component;
            }

            [MethodImpl(AggressiveInlining)]
            public ref L SetLinks<L>(EntityGID link1, EntityGID link2, EntityGID link3) where L : struct, IEntityLinksComponent<L> {
                ref var component = ref Components<L>.Value.TryAdd(this);
                ref var multi = ref component.RefValue(ref component).multi;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (multi.Contains(link1)) throw new StaticEcsException($"Link {link1} already contains in relations type {typeof(L)}, for {this}");
                multi.Add(link1);
                if (multi.Contains(link2)) throw new StaticEcsException($"Link {link2} already contains in relations type {typeof(L)}, for {this}");
                multi.Add(link2);
                if (multi.Contains(link3)) throw new StaticEcsException($"Link {link3} already contains in relations type {typeof(L)}, for {this}");
                multi.Add(link3);
                #else
                multi.Add(link1, link2, link3);
                #endif
                var handler = Context.Value.Get<LinkManyHandlers<L>>().OnAddLinkItem;
                if (handler != null) {
                    handler(this, link1.Unpack<WorldType>());
                    handler(this, link2.Unpack<WorldType>());
                    handler(this, link3.Unpack<WorldType>());
                }
                
                return ref component;
            }

            [MethodImpl(AggressiveInlining)]
            public ref L SetLinks<L>(EntityGID link1, EntityGID link2, EntityGID link3, EntityGID link4) where L : struct, IEntityLinksComponent<L> {
                ref var component = ref Components<L>.Value.TryAdd(this);
                ref var multi = ref component.RefValue(ref component).multi;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (multi.Contains(link1)) throw new StaticEcsException($"Link {link1} already contains in relations type {typeof(L)}, for {this}");
                multi.Add(link1);
                if (multi.Contains(link2)) throw new StaticEcsException($"Link {link2} already contains in relations type {typeof(L)}, for {this}");
                multi.Add(link2);
                if (multi.Contains(link3)) throw new StaticEcsException($"Link {link3} already contains in relations type {typeof(L)}, for {this}");
                multi.Add(link3);
                if (multi.Contains(link4)) throw new StaticEcsException($"Link {link4} already contains in relations type {typeof(L)}, for {this}");
                multi.Add(link4);
                #else
                multi.Add(link1, link2, link3, link4);
                #endif
                var handler = Context.Value.Get<LinkManyHandlers<L>>().OnAddLinkItem;
                if (handler != null) {
                    handler(this, link1.Unpack<WorldType>());
                    handler(this, link2.Unpack<WorldType>());
                    handler(this, link3.Unpack<WorldType>());
                    handler(this, link4.Unpack<WorldType>());
                }
                
                return ref component;
            }
            #endregion
        }
    }
}
