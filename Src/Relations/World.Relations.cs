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
            bool disableRelationCheck = false,
            uint basePoolCapacity = 128
        )
            where T : struct, IEntityLinkComponent<T> {
            if (Status != WorldStatus.Created) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: RegisterToOneRelationType<{typeof(T)}>, World not created");
            }

            config ??= DefaultComponentConfig<T, WorldType>.Default;
            ModuleComponents.Value.RegisterToOneRelationType(basePoolCapacity, config, onDeleteStrategy, onCopyStrategy, disableRelationCheck);
        }

        [MethodImpl(AggressiveInlining)]
        public static void RegisterToManyRelationType<T>(
            ushort defaultComponentCapacity,
            OneDirectionalDeleteStrategy deleteStrategy = OneDirectionalDeleteStrategy.Default,
            CopyStrategy copyStrategy = CopyStrategy.Default,
            IComponentConfig<T, WorldType> config = null,
            bool disableRelationsCheckDebug = false,
            uint basePoolCapacity = 128
        ) where T : struct, IEntityLinksComponent<T> {
            if (Status != WorldStatus.Created) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: RegisterToManyRelationType<{typeof(T)}>, World not created");
            }

            config ??= DefaultComponentConfig<T, WorldType>.Default;
            ModuleComponents.Value.RegisterToManyRelationType(defaultComponentCapacity, basePoolCapacity, config, deleteStrategy, copyStrategy, disableRelationsCheckDebug);
        }

        [MethodImpl(AggressiveInlining)]
        public static void RegisterOneToOneRelationType<L, R>(
            BiDirectionalDeleteStrategy leftDeleteStrategy = BiDirectionalDeleteStrategy.DeleteAnotherLink,
            BiDirectionalDeleteStrategy rightDeleteStrategy = BiDirectionalDeleteStrategy.DeleteAnotherLink,
            CopyStrategy leftCopyStrategy = CopyStrategy.Default,
            CopyStrategy rightCopyStrategy = CopyStrategy.Default,
            IComponentConfig<L, WorldType> leftConfig = null,
            IComponentConfig<R, WorldType> rightConfig = null,
            bool disableRelationsCheckLeftDebug = false, bool disableRelationsCheckRightDebug = false,
            uint basePoolCapacity = 128
        )
            where L : struct, IEntityLinkComponent<L>
            where R : struct, IEntityLinkComponent<R> {
            if (Status != WorldStatus.Created) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: RegisterOneToOneRelationType<{typeof(L)}, {typeof(R)}>, World not created");
            }

            leftConfig ??= DefaultComponentConfig<L, WorldType>.Default;
            rightConfig ??= DefaultComponentConfig<R, WorldType>.Default;
            ModuleComponents.Value.RegisterOneToOneRelationType(basePoolCapacity, leftConfig, rightConfig, leftDeleteStrategy, rightDeleteStrategy, leftCopyStrategy, rightCopyStrategy, disableRelationsCheckLeftDebug, disableRelationsCheckRightDebug);
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
            bool disableRelationsCheckLeftDebug = false, bool disableRelationsCheckRightDebug = false,
            uint basePoolCapacity = 128
        )
            where O : struct, IEntityLinkComponent<O>
            where M : struct, IEntityLinksComponent<M> {
            if (Status != WorldStatus.Created) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: RegisterOneToManyRelationType<{typeof(O)}, {typeof(M)}>, World not created");
            }

            leftConfig ??= DefaultComponentConfig<O, WorldType>.Default;
            rightConfig ??= DefaultComponentConfig<M, WorldType>.Default;
            ModuleComponents.Value.RegisterOneToManyRelationType(defaultComponentCapacity, basePoolCapacity, leftConfig, rightConfig, leftDeleteStrategy, rightDeleteStrategy,
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
            bool disableRelationsCheckLeftDebug = false, bool disableRelationsCheckRightDebug = false,
            uint basePoolCapacity = 128
        )
            where L : struct, IEntityLinksComponent<L>
            where R : struct, IEntityLinksComponent<R> {
            if (Status != WorldStatus.Created) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: RegisterManyToManyRelationType<{typeof(L)}, {typeof(R)}>, World not created");
            }

            leftConfig ??= DefaultComponentConfig<L, WorldType>.Default;
            rightConfig ??= DefaultComponentConfig<R, WorldType>.Default;
            ModuleComponents.Value.RegisterManyToManyRelationType(defaultComponentCapacity, basePoolCapacity, leftConfig, rightConfig, leftDeleteStrategy, rightDeleteStrategy,
                                                                  leftCopyStrategy, rightCopyStrategy, disableRelationsCheckLeftDebug, disableRelationsCheckRightDebug);
        }
    }
}