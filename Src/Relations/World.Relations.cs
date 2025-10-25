#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

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

        [MethodImpl(AggressiveInlining)]
        public static void RegisterToOneRelationType<T>(
            OneDirectionalDeleteStrategy onDeleteStrategy = OneDirectionalDeleteStrategy.Default,
            CopyStrategy onCopyStrategy = CopyStrategy.Default,
            IComponentConfig<T, WorldType> config = null,
            bool disableRelationCheck = false
        )
            where T : struct, IEntityLinkComponent<T> {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            config ??= DefaultComponentConfig<T, WorldType>.Default;
            ModuleComponents.Value.RegisterToOneRelationType(config, onDeleteStrategy, onCopyStrategy, disableRelationCheck);
        }

        [MethodImpl(AggressiveInlining)]
        public static void RegisterToManyRelationType<T>(
            ushort defaultComponentCapacity,
            OneDirectionalDeleteStrategy deleteStrategy = OneDirectionalDeleteStrategy.Default,
            CopyStrategy copyStrategy = CopyStrategy.Default,
            IComponentConfig<T, WorldType> config = null,
            bool disableRelationsCheckDebug = false
        ) where T : struct, IEntityLinksComponent<T> {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            config ??= DefaultComponentConfig<T, WorldType>.Default;
            ModuleComponents.Value.RegisterToManyRelationType(defaultComponentCapacity, config, deleteStrategy, copyStrategy, disableRelationsCheckDebug);
        }

        [MethodImpl(AggressiveInlining)]
        public static void RegisterOneToOneRelationType<L, R>(
            BiDirectionalDeleteStrategy leftDeleteStrategy = BiDirectionalDeleteStrategy.DeleteAnotherLink,
            BiDirectionalDeleteStrategy rightDeleteStrategy = BiDirectionalDeleteStrategy.DeleteAnotherLink,
            CopyStrategy leftCopyStrategy = CopyStrategy.Default,
            CopyStrategy rightCopyStrategy = CopyStrategy.Default,
            IComponentConfig<L, WorldType> leftConfig = null,
            IComponentConfig<R, WorldType> rightConfig = null,
            bool disableRelationsCheckLeftDebug = false, bool disableRelationsCheckRightDebug = false
        )
            where L : struct, IEntityLinkComponent<L>
            where R : struct, IEntityLinkComponent<R> {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            leftConfig ??= DefaultComponentConfig<L, WorldType>.Default;
            rightConfig ??= DefaultComponentConfig<R, WorldType>.Default;
            ModuleComponents.Value.RegisterOneToOneRelationType(leftConfig, rightConfig, leftDeleteStrategy, rightDeleteStrategy, leftCopyStrategy, rightCopyStrategy, disableRelationsCheckLeftDebug, disableRelationsCheckRightDebug);
        }

        [MethodImpl(AggressiveInlining)]
        public static void RegisterOneToManyRelationType<O, M>(
            ushort defaultComponentCapacity,
            BiDirectionalDeleteStrategy leftDeleteStrategy = BiDirectionalDeleteStrategy.DeleteAnotherLink,
            BiDirectionalDeleteStrategy rightDeleteStrategy = BiDirectionalDeleteStrategy.DeleteAnotherLink,
            CopyStrategy leftCopyStrategy = CopyStrategy.Default,
            CopyStrategy rightCopyStrategy = CopyStrategy.Default,
            IComponentConfig<O, WorldType> leftConfig = null,
            IComponentConfig<M, WorldType> rightConfig = null,
            bool disableRelationsCheckLeftDebug = false, bool disableRelationsCheckRightDebug = false
        )
            where O : struct, IEntityLinkComponent<O>
            where M : struct, IEntityLinksComponent<M> {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            leftConfig ??= DefaultComponentConfig<O, WorldType>.Default;
            rightConfig ??= DefaultComponentConfig<M, WorldType>.Default;
            ModuleComponents.Value.RegisterOneToManyRelationType(defaultComponentCapacity, leftConfig, rightConfig, leftDeleteStrategy, rightDeleteStrategy,
                                                                 leftCopyStrategy, rightCopyStrategy, disableRelationsCheckLeftDebug, disableRelationsCheckRightDebug);
        }

        [MethodImpl(AggressiveInlining)]
        public static void RegisterManyToManyRelationType<L, R>(
            ushort defaultComponentCapacity,
            BiDirectionalDeleteStrategy leftDeleteStrategy = BiDirectionalDeleteStrategy.DeleteAnotherLink,
            BiDirectionalDeleteStrategy rightDeleteStrategy = BiDirectionalDeleteStrategy.DeleteAnotherLink,
            CopyStrategy leftCopyStrategy = CopyStrategy.Default,
            CopyStrategy rightCopyStrategy = CopyStrategy.Default,
            IComponentConfig<L, WorldType> leftConfig = null,
            IComponentConfig<R, WorldType> rightConfig = null,
            bool disableRelationsCheckLeftDebug = false, bool disableRelationsCheckRightDebug = false
        )
            where L : struct, IEntityLinksComponent<L>
            where R : struct, IEntityLinksComponent<R> {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            leftConfig ??= DefaultComponentConfig<L, WorldType>.Default;
            rightConfig ??= DefaultComponentConfig<R, WorldType>.Default;
            ModuleComponents.Value.RegisterManyToManyRelationType(defaultComponentCapacity, leftConfig, rightConfig, leftDeleteStrategy, rightDeleteStrategy,
                                                                  leftCopyStrategy, rightCopyStrategy, disableRelationsCheckLeftDebug, disableRelationsCheckRightDebug);
        }
    }
}