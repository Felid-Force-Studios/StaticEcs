using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    public interface IWorld {
        
        public Type GetWorldType();
        
        public bool TryGetEntity(EntityGID gid, out IEntity entity);
        
        public IEntity NewEntity(Type componentType);

        public IEntity NewEntity(IComponent component);

        public IEntity NewEntity<T>(T component) where T : struct, IComponent;

        public uint EntitiesCountWithoutDestroyed();

        public uint EntitiesCount();

        public uint EntitiesCapacity();

        public void Clear();

        public WorldStatus Status();

        public IContext Context();

        #if !FFS_ECS_DISABLE_EVENTS
        public IEvents Events();
        public List<IEventPoolWrapper> GetAllEventPools();
        #endif

        internal bool TryGetStandardComponentsRawPool(Type type, out IStandardRawPool pool);

        internal List<IStandardRawPool> GetAllStandardComponentsRawPools();

        internal bool TryGetComponentsRawPool(Type type, out IRawComponentPool pool);

        internal List<IRawPool> GetAllComponentsRawPools();


        #if !FFS_ECS_DISABLE_TAGS
        internal bool TryGetTagsRawPool(Type type, out IRawPool pool);

        internal List<IRawPool> GetAllTagsRawPools();
        #endif

        #if !FFS_ECS_DISABLE_MASKS
        internal bool TryGetMasksRawPool(Type type, out IRawPool pool);

        internal List<IRawPool> GetAllMasksRawPools();
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct WorldWrapper<WorldType> : IWorld where WorldType : struct, IWorldType {

        [MethodImpl(AggressiveInlining)]
        public Type GetWorldType() => typeof(WorldType);

        [MethodImpl(AggressiveInlining)]
        public bool TryGetEntity(EntityGID gid, out IEntity entity) {
            if (gid.TryUnpack<WorldType>(out var e)) {
                entity = e;
                return true;
            }

            entity = default;
            return false;
        }

        [MethodImpl(AggressiveInlining)]
        public IEntity NewEntity(Type componentType) => World<WorldType>.Entity.New(componentType);

        [MethodImpl(AggressiveInlining)]
        public IEntity NewEntity(IComponent component) => World<WorldType>.Entity.New(component);

        [MethodImpl(AggressiveInlining)]
        public IEntity NewEntity<T>(T component) where T : struct, IComponent => World<WorldType>.Entity.New(component);

        [MethodImpl(AggressiveInlining)]
        public uint EntitiesCountWithoutDestroyed() => World<WorldType>.EntitiesCountWithoutDestroyed();

        [MethodImpl(AggressiveInlining)]
        public uint EntitiesCount() => World<WorldType>.EntitiesCount();

        [MethodImpl(AggressiveInlining)]
        public uint EntitiesCapacity() => World<WorldType>.EntitiesCapacity();

        [MethodImpl(AggressiveInlining)]
        public void Clear() => World<WorldType>.Clear();

        [MethodImpl(AggressiveInlining)]
        public WorldStatus Status() => World<WorldType>.Status;

        [MethodImpl(AggressiveInlining)]
        public IContext Context() => World<WorldType>.Context.Value;

        #if !FFS_ECS_DISABLE_EVENTS
        [MethodImpl(AggressiveInlining)]
        public IEvents Events() => new EventsWrapper<WorldType>();

        public List<IEventPoolWrapper> GetAllEventPools() {
            return World<WorldType>.Events.GetAllRawsPools();
        }
        #endif

        bool IWorld.TryGetStandardComponentsRawPool(Type type, out IStandardRawPool pool) {
            if (World<WorldType>.TryGetStandardComponentsPool(type, out var p)) {
                pool = p;
                return true;
            }

            pool = default;
            return false;
        }

        List<IStandardRawPool> IWorld.GetAllStandardComponentsRawPools() {
            return World<WorldType>.ModuleStandardComponents.Value.GetAllRawsPools();
        }

        bool IWorld.TryGetComponentsRawPool(Type type, out IRawComponentPool pool) {
            if (World<WorldType>.TryGetComponentsPool(type, out var p)) {
                pool = p;
                return true;
            }

            pool = default;
            return false;
        }

        List<IRawPool> IWorld.GetAllComponentsRawPools() {
            return World<WorldType>.ModuleComponents.Value.GetAllRawsPools();
        }

        #if !FFS_ECS_DISABLE_TAGS
        bool IWorld.TryGetTagsRawPool(Type type, out IRawPool pool) {
            if (World<WorldType>.TryGetTagsPool(type, out var p)) {
                pool = p;
                return true;
            }

            pool = default;
            return false;
        }

        List<IRawPool> IWorld.GetAllTagsRawPools() {
            return World<WorldType>.ModuleTags.Value.GetAllRawsPools();
        }
        #endif

        #if !FFS_ECS_DISABLE_MASKS
        bool IWorld.TryGetMasksRawPool(Type type, out IRawPool pool) {
            if (World<WorldType>.TryGetMasksPool(type, out var p)) {
                pool = p;
                return true;
            }

            pool = default;
            return false;
        }

        List<IRawPool> IWorld.GetAllMasksRawPools() {
            return World<WorldType>.ModuleMasks.Value.GetAllRawsPools();
        }
        #endif
    }
}