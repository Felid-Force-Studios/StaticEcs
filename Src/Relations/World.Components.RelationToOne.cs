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
            internal void RegisterToOneRelationType<T>(
                uint capacity,
                IComponentConfig<T, WorldType> config,
                OneDirectionalDeleteStrategy onDeleteStrategy = OneDirectionalDeleteStrategy.Default,
                CopyStrategy onCopyStrategy = CopyStrategy.Default,
                bool disableRelationCheck = false
            ) where T : struct, IEntityLinkComponent<T> {
                ValidateComponentRegistration<T>();
                
                var actualConfig = new ValueComponentConfig<T, WorldType>(config) {
                    OnAddWithValueHandler = OnAddHandler(config.OnAdd(), disableRelationCheck),
                    OnDeleteHandler = OnDeleteHandler(onDeleteStrategy, config.OnDelete()),
                    OnCopyHandler = OnCopyOneHandler(onCopyStrategy, config.OnCopy()),
                    OnAddHandler = UseAddLinkMethodException,
                    Copyable = onCopyStrategy != CopyStrategy.NotCopy
                };

                RegisterComponentType(
                    capacity: capacity,
                    actualConfig,
                    putNotAllowed: true
                );
                return;

                static OnComponentHandler<T> OnAddHandler(OnComponentHandler<T> handler, bool disableRelationsCheck) {
                    #if (DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_RELATION_CHECK
                    if (!disableRelationsCheck) {
                        if (handler != null) {
                            return (Entity entity, ref T component) => {
                                handler(entity, ref component);
                                CheckOneRelation(entity, ref component);
                            };
                        }

                        return CheckOneRelation;
                    }
                    #endif
                    return handler;
                }

                static OnComponentHandler<T> OnDeleteHandler(OneDirectionalDeleteStrategy strategy, OnComponentHandler<T> handler) {
                    if (strategy == OneDirectionalDeleteStrategy.Default) {
                        return handler;
                    }
                    
                    if (strategy == OneDirectionalDeleteStrategy.DestroyLinkedEntity) {
                        return DestroyLinkedEntityHandler(handler);
                    }

                    throw new StaticEcsException("Unsupported onDelete strategy");
                }
            }
        }
    }
}