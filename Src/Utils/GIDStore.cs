using System;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
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
        public struct GIDStore {
            internal static GIDStore Value;

            private EntityGID[] _freeIds;
            private short[] _versions;
            private int _freeIdsCount;
            private int _nextId;

            // Dynamic data
            private uint[] _entityByGlobalId;
            private EntityGID[] _globalIdByEntity;
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            private byte[] _newEntities;
            #endif

            [MethodImpl(AggressiveInlining)]
            public GIDStore(GIDStoreSnapshot snapshot) {
                var size = snapshot.Versions.Length;

                _freeIds = snapshot.FreeIds;
                _versions = snapshot.Versions;
                _entityByGlobalId = new uint[size];
                _globalIdByEntity = new EntityGID[size];
                _freeIdsCount = snapshot.FreeIdsCount;
                _nextId = snapshot.NextId;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                _newEntities = new byte[size];
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public GIDStore Create(WorldConfig config) {
                _freeIds = new EntityGID[128];
                _versions = new short[config.BaseEntitiesCount];
                _entityByGlobalId = new uint[config.BaseEntitiesCount];
                _globalIdByEntity = new EntityGID[config.BaseEntitiesCount];
                _freeIdsCount = 0;
                _nextId = 0;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                _newEntities = new byte[config.BaseEntitiesCount];
                #endif
                return this;
            }

            [MethodImpl(AggressiveInlining)]
            public void Clear() {
                Utils.LoopFallbackClear(_freeIds, 0, _freeIdsCount);
                Utils.LoopFallbackClear(_versions, 0, _nextId);
                Utils.LoopFallbackClear(_entityByGlobalId, 0, _nextId);
                Utils.LoopFallbackClear(_globalIdByEntity, 0, (int) Entity.entitiesCount);
                _freeIdsCount = 0;
                _nextId = 0;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Utils.LoopFallbackClear(_newEntities, 0, _nextId);
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public void Destroy() {
                _entityByGlobalId = default;
                _globalIdByEntity = default;
                _freeIds = default;
                _versions = default;
                _freeIdsCount = default;
                _nextId = default;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                _newEntities = default;
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public EntityGID Get(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!Has(entity)) throw new StaticEcsException($"[GlobalIdStore] Method: Get, Dont have {entity}");
                #endif
                return _globalIdByEntity[entity._id];
            }

            [MethodImpl(AggressiveInlining)]
            public bool Has(Entity entity) => !_globalIdByEntity[entity._id].IsEmpty();

            [MethodImpl(AggressiveInlining)]
            internal void New(Entity entity) {
                var gid = _freeIdsCount > 0
                    ? _freeIds[--_freeIdsCount]
                    : new EntityGID((uint) _nextId++, 1);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Set(entity, gid, true);
                #else
                Set(entity, gid);
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            internal void Set(Entity entity, EntityGID gid, bool asNew) {
                #else
            internal void Set(Entity entity, EntityGID gid) {
                #endif
                var id = gid.Id();
                if (id >= _entityByGlobalId.Length) {
                    Resize(id);
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (Has(entity)) throw new StaticEcsException($"[GlobalIdStore] Method: Set, Already have {entity}");
                if (_versions[id] > 0 && _versions[id] != gid.Version()) throw new StaticEcsException($"[GlobalIdStore] Method: Set, Already have {gid}");
                if (!asNew && _newEntities[id] == 1)
                    throw new StaticEcsException($"[GlobalIdStore] Method: Set, ({gid}) was used to create a new entity," +
                                                 $" initialize the world using the saved GlobalIdStore or create/load entity as new");
                if (asNew) {
                    _newEntities[id] = 1;
                }
                #endif

                _versions[id] = gid.Version();
                _entityByGlobalId[id] = entity._id + 1;
                _globalIdByEntity[entity._id] = gid;
            }

            [MethodImpl(AggressiveInlining)]
            internal void DestroyEntity(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!Has(entity)) throw new StaticEcsException($"[GlobalIdStore] Method: DestroyEntity, Dont have {entity}");
                #endif

                if (_freeIdsCount == _freeIds.Length) {
                    Array.Resize(ref _freeIds, _freeIds.Length << 1);
                }

                ref var gid = ref _globalIdByEntity[entity._id];
                gid.IncrementVersion();

                _freeIds[_freeIdsCount++] = gid;
                _versions[gid.Id()] = (short) -gid.Version();
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                _newEntities[gid.Id()] = 0;
                #endif
                gid = default;
            }

            [MethodImpl(AggressiveInlining)]
            internal void UnloadEntity(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!Has(entity)) throw new StaticEcsException($"[GlobalIdStore] Method: UnloadEntity, Dont have {entity}");
                #endif
                ref var gid = ref _globalIdByEntity[entity._id];
                _entityByGlobalId[gid.Id()] = default;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                _newEntities[gid.Id()] = 0;
                #endif
                gid = default;
            }

            [MethodImpl(AggressiveInlining)]
            public Entity GetEntity(EntityGID gid) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsLoaded(gid)) throw new StaticEcsException($"[GlobalIdStore] Method: GetEntity, Dont have id = {gid.Id()}, version = {gid.Version()}");
                #endif
                return new Entity(_entityByGlobalId[gid.Id()] - 1);
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryGetEntity(EntityGID gid, out Entity entity) {
                if (IsLoaded(gid)) {
                    entity = new Entity(_entityByGlobalId[gid.Id()] - 1);
                    return true;
                }

                entity = default;
                return false;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryGetEntity(uint gidId, out Entity entity) {
                if (_entityByGlobalId.Length > gidId) {
                    var val = _entityByGlobalId[gidId];
                    if (val > 0) {
                        entity = new Entity(val - 1);
                        return true;
                    }
                }

                entity = default;
                return false;
            }

            [MethodImpl(AggressiveInlining)]
            public bool IsRegistered(EntityGID gid) {
                return _versions[gid.Id()] == gid.Version();
            }

            [MethodImpl(AggressiveInlining)]
            public bool IsLoaded(EntityGID gid) {
                var id = gid.Id();
                return _versions[gid.Id()] == gid.Version() && _entityByGlobalId[id] > 0;
            }

            private void Resize(uint len) {
                var size = (int) Utils.CalculateSize(len + 1);
                Array.Resize(ref _entityByGlobalId, size);
                Array.Resize(ref _versions, size);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Array.Resize(ref _newEntities, size);
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal void ResizeEntities(uint len) {
                Array.Resize(ref _globalIdByEntity, (int) len);
            }

            [MethodImpl(AggressiveInlining)]
            internal void Write(ref BinaryPackWriter writer) {
                writer.WriteInt(_nextId);
                writer.WriteInt(_freeIdsCount);
                writer.WriteInt(_freeIds.Length);
                writer.WriteInt(_versions.Length);
                writer.WriteInt((int) Entity.entitiesCapacity);
                writer.WriteArrayUnmanaged(_freeIds, 0, _freeIdsCount);
                writer.WriteArrayUnmanaged(_versions, 0, _nextId);
                writer.WriteArrayUnmanaged(_entityByGlobalId, 0, _nextId);
                writer.WriteArrayUnmanaged(_globalIdByEntity, 0, (int) Entity.entitiesCount);
            }

            [MethodImpl(AggressiveInlining)]
            internal void Read(ref BinaryPackReader reader) {
                _nextId = reader.ReadInt();
                _freeIdsCount = reader.ReadInt();
                var freeIdsCapacity = reader.ReadInt();
                var dataCapacity = reader.ReadInt();
                var entitiesCapacity = reader.ReadInt();
                if (entitiesCapacity > _globalIdByEntity.Length) {
                    Array.Resize(ref _globalIdByEntity, entitiesCapacity);
                }

                if (freeIdsCapacity > _freeIds.Length) {
                    Array.Resize(ref _freeIds, freeIdsCapacity);
                }

                if (dataCapacity > _versions.Length) {
                    Array.Resize(ref _versions, dataCapacity);
                    Array.Resize(ref _entityByGlobalId, dataCapacity);
                }

                reader.ReadArrayUnmanaged(ref _freeIds);
                reader.ReadArrayUnmanaged(ref _versions);
                reader.ReadArrayUnmanaged(ref _entityByGlobalId);
                reader.ReadArrayUnmanaged(ref _globalIdByEntity);
            }

            internal GIDStoreSnapshot AsSnapshot() {
                return new GIDStoreSnapshot {
                    NextId = _nextId,
                    FreeIdsCount = _freeIdsCount,
                    Versions = _versions,
                    FreeIds = _freeIds
                };
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct GIDStoreSnapshot {
        internal EntityGID[] FreeIds;
        internal short[] Versions;
        internal int FreeIdsCount;
        internal int NextId;

        [MethodImpl(AggressiveInlining)]
        internal static void Write(ref BinaryPackWriter writer, in GIDStoreSnapshot snapshot) {
            writer.WriteInt(snapshot.NextId);
            writer.WriteInt(snapshot.FreeIdsCount);
            writer.WriteInt(snapshot.FreeIds.Length);
            writer.WriteInt(snapshot.Versions.Length);
            writer.WriteArrayUnmanaged(snapshot.FreeIds, 0, snapshot.FreeIdsCount);
            writer.WriteArrayUnmanaged(snapshot.Versions, 0, snapshot.NextId);
        }

        [MethodImpl(AggressiveInlining)]
        internal static GIDStoreSnapshot Read(ref BinaryPackReader reader) {
            var snapshot = new GIDStoreSnapshot {
                NextId = reader.ReadInt(),
                FreeIdsCount = reader.ReadInt(),
            };

            var freeIdsCapacity = reader.ReadInt();
            var dataCapacity = reader.ReadInt();

            snapshot.FreeIds = new EntityGID[freeIdsCapacity];
            snapshot.Versions = new short[dataCapacity];
            reader.ReadArrayUnmanaged(ref snapshot.FreeIds);
            reader.ReadArrayUnmanaged(ref snapshot.Versions);

            return snapshot;
        }
    }
}