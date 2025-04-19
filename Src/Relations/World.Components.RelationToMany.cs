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
            internal void RegisterToManyRelationType<T>(
                ushort defaultComponentCapacity, uint capacity,
                IComponentConfig<T, WorldType> config,
                OneDirectionalDeleteStrategy deleteStrategy = OneDirectionalDeleteStrategy.Default,
                CopyStrategy copyStrategy = CopyStrategy.Default,
                bool disableRelationsCheckDebug = false
            ) where T : struct, IEntityLinksComponent<T> {
                ValidateComponentRegistration<T>();
                RegisterMultiComponentsData<EntityGID>(defaultComponentCapacity, capacity);
                
                Context.Value.GetOrCreate<LinkManyHandlers<T>>();
                
                #if (DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_RELATION_CHECK
                if (!disableRelationsCheckDebug) {
                    Context.Value.Get<LinkManyHandlers<T>>().OnAddLinkItem = CheckManyRelation<T>;
                }
                #endif
                
                var actualConfig = new ValueComponentConfig<T, WorldType>(config) {
                    OnAddWithValueHandler = OnAddHandler(config.OnAdd(), disableRelationsCheckDebug),
                    OnDeleteHandler = OnDeleteHandler(deleteStrategy, config.OnDelete()),
                    OnCopyHandler = OnCopyManyHandler(copyStrategy, config.OnCopy()),
                    OnAddHandler = OnAddHandler(config.OnAdd(), disableRelationsCheckDebug),
                    Copyable = copyStrategy != CopyStrategy.NotCopy
                };

                RegisterComponentType(
                    capacity: capacity,
                    actualConfig,
                    putNotAllowed: true
                );
                return;

                [MethodImpl(AggressiveInlining)]
                static OnComponentHandler<T> OnAddHandler(OnComponentHandler<T> handler, bool disableRelationCheck) {
                    if (handler != null) {
                        return (Entity entity, ref T component) => {
                            OnAddMultiLink(entity, ref component);
                            handler(entity, ref component);
                        };
                    }

                    return OnAddMultiLink;
                }
                
                [MethodImpl(AggressiveInlining)]
                static OnComponentHandler<T> OnDeleteHandler(OneDirectionalDeleteStrategy strategy, OnComponentHandler<T> handler) {
                    if (strategy == OneDirectionalDeleteStrategy.Default) {
                        return handler;
                    }
                    
                    if (strategy == OneDirectionalDeleteStrategy.DestroyLinkedEntity) {
                        return DestroyLinkedEntitiesHandler(handler);
                    }

                    throw new StaticEcsException("Unsupported onDelete strategy");
                }
            }
        }
    }
}