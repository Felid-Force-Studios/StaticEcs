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

    public interface IWorld {
        
        public Type GetWorldType();
        
        public bool TryGetEntity(EntityGID gid, out IEntity entity);
        
        public IEntity NewEntity(Type componentType);

        public IEntity NewEntity(IComponent component);

        public IEntity NewEntity<T>(T component) where T : struct, IComponent;

        public uint CalculateEntitiesCount();

        public uint CalculateEntitiesCapacity();

        public void DestroyAllEntities();

        public void DestroyAllEntitiesInCluster(ushort clusterId);

        public void DestroyAllEntitiesInChunk(uint chunkIdx);

        public WorldStatus Status();

        public IContext Context();

        public IEvents Events();
        public List<IEventPoolWrapper> GetAllEventPools();

        internal bool TryGetComponentsRawPool(Type type, out IRawComponentPool pool);

        internal List<IRawPool> GetAllComponentsRawPools();


        internal bool TryGetTagsRawPool(Type type, out IRawPool pool);

        internal List<IRawPool> GetAllTagsRawPools();
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
                entity = e.Box();
                return true;
            }

            entity = default;
            return false;
        }

        [MethodImpl(AggressiveInlining)]
        public IEntity NewEntity(Type componentType) => World<WorldType>.Entity.New(componentType).Box();

        [MethodImpl(AggressiveInlining)]
        public IEntity NewEntity(IComponent component) => World<WorldType>.Entity.New(component).Box();

        [MethodImpl(AggressiveInlining)]
        public IEntity NewEntity<T>(T component) where T : struct, IComponent => World<WorldType>.Entity.New(component).Box();

        [MethodImpl(AggressiveInlining)]
        public uint CalculateEntitiesCount() => World<WorldType>.CalculateEntitiesCount();

        [MethodImpl(AggressiveInlining)]
        public uint CalculateEntitiesCapacity() => World<WorldType>.CalculateEntitiesCapacity();

        [MethodImpl(AggressiveInlining)]
        public void DestroyAllEntities() => World<WorldType>.DestroyAllEntities();

        [MethodImpl(AggressiveInlining)]
        public void DestroyAllEntitiesInCluster(ushort clusterId) => World<WorldType>.DestroyAllEntitiesInCluster(clusterId);

        [MethodImpl(AggressiveInlining)]
        public void DestroyAllEntitiesInChunk(uint chunkIdx) => World<WorldType>.DestroyAllEntitiesInChunk(chunkIdx);

        [MethodImpl(AggressiveInlining)]
        public WorldStatus Status() => World<WorldType>.Status;

        [MethodImpl(AggressiveInlining)]
        public IContext Context() => World<WorldType>.Context.Value;

        [MethodImpl(AggressiveInlining)]
        public IEvents Events() => new EventsWrapper<WorldType>();

        public List<IEventPoolWrapper> GetAllEventPools() {
            return World<WorldType>.Events.GetAllRawsPools();
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
    }
}