using System;
using System.Buffers;
using System.Collections.Generic;
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
        public static partial class Serializer {
            [MethodImpl(AggressiveInlining)]
            public static void SetComponentDeleteMigrator(Guid id, EcsComponentDeleteMigrationReader<WorldType> migrator) {
                ModuleComponents.Serializer.Value.SetDeleteMigrator(id, migrator);
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        internal partial struct ModuleComponents {
            
            [MethodImpl(AggressiveInlining)]
            internal void WriteGuids(ref BinaryPackWriter writer) {
                writer.WriteCollection(0, _poolsCount, (ref BinaryPackWriter w, int i) => {
                    var guid = Value._pools[i].Guid();
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (guid == Guid.Empty) throw new StaticEcsException($"Component type {Value._pools[i].GetElementType()} guid not registered");
                    #endif
                    w.WriteGuid(guid);
                });
            }
            
            internal struct Serializer {
                internal static Serializer Value;

                private Dictionary<Guid, IComponentsWrapper> _poolByGuid;
                private Dictionary<Guid, EcsComponentDeleteMigrationReader<WorldType>> _deleteMigratorByGuid;
                private List<(Guid id, uint offset)> _tempDeletedPoolIds;
                private List<(Guid guid, ushort id)> _tempLoadedPools;

                [MethodImpl(AggressiveInlining)]
                internal void Create() {
                    _poolByGuid = new Dictionary<Guid, IComponentsWrapper>();
                    _deleteMigratorByGuid = new Dictionary<Guid, EcsComponentDeleteMigrationReader<WorldType>>();
                    _tempDeletedPoolIds = new List<(Guid id, uint offset)>();
                    _tempLoadedPools = new List<(Guid guid, ushort id)>();
                }

                [MethodImpl(AggressiveInlining)]
                internal void Destroy() {
                    _deleteMigratorByGuid = null;
                    _tempDeletedPoolIds = null;
                    _tempLoadedPools = null;
                    _poolByGuid = null;
                }

                [MethodImpl(AggressiveInlining)]
                internal void RegisterComponentType<T>(IComponentConfig<T, WorldType> config) where T : struct, IComponent {
                    var guid = config.Id();
                    if (guid != Guid.Empty) {
                        if (_poolByGuid.ContainsKey(guid)) throw new StaticEcsException($"Component type {typeof(T)} with guid {guid} already registered");
                        _poolByGuid[guid] = new ComponentsWrapper<T>();
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void SetDeleteMigrator(Guid id, EcsComponentDeleteMigrationReader<WorldType> migrator) {
                    _deleteMigratorByGuid[id] = migrator;
                }

                [MethodImpl(AggressiveInlining)]
                internal void Write(ref BinaryPackWriter writer) {
                    ref var components = ref ModuleComponents.Value;
                    
                    components.BitMask.WriteWithDisabled(ref writer, Entity.entitiesCount);
                    writer.WriteUshort(components._poolsCount);
                    for (var i = 0; i < components._poolsCount; i++) {
                        var pool = components._pools[i];
                        var guid = pool.Guid();

                        #if DEBUG || FFS_ECS_ENABLE_DEBUG
                        if (!_poolByGuid.ContainsKey(guid)) {
                            throw new StaticEcsException($"Serializer for component type {pool.GetElementType()} not registered");
                        }
                        #endif
                        writer.WriteGuid(guid);
                        writer.WriteUshort(pool.DynamicId());
                        var offset = writer.MakePoint(sizeof(uint));
                        _poolByGuid[guid].WriteAll(ref writer);
                        writer.WriteUintAt(offset, writer.Position - (offset + sizeof(uint)));
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void Write(ref BinaryPackWriter writer, Entity entity) {
                    ref var components = ref ModuleComponents.Value;
                    
                    ref var bitMask = ref components.BitMask;
                    var len = bitMask.LenWithDisabled(entity._id);
                    writer.WriteUshort(len);
                    var bufId = bitMask.BorrowBuf();
                    bitMask.CopyWithDisabledToBuffer(entity._id, bufId);
                    while (bitMask.GetMinIndexBuffer(bufId, out var id)) {
                        writer.WriteUshort((ushort) id);
                        components._pools[id].Write(ref writer, entity);
                        bitMask.DelInBuffer(bufId, (ushort) id);
                    }
                    bitMask.DropBuf();
                }

                [MethodImpl(AggressiveInlining)]
                internal void Read(ref BinaryPackReader reader, Guid[] componentGuidById, Entity entity) {
                    var len = reader.ReadUshort();

                    for (int i = 0; i < len; i++) {
                        var id = reader.ReadUshort();
                        var guid = componentGuidById[id];
                        if (_poolByGuid.TryGetValue(guid, out var pool)) {
                            pool.Read(ref reader, entity);
                        } else {
                            if (_deleteMigratorByGuid.TryGetValue(guid, out var migration)) {
                                reader.DeleteOneComponentMigration(entity, migration);
                            } else {
                                reader.SkipOneComponent();
                            }
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void Read(ref BinaryPackReader reader) {
                    _tempDeletedPoolIds.Clear();
                    _tempLoadedPools.Clear();
                    
                    ref var components = ref ModuleComponents.Value;

                    components.BitMask.ReadWithDisabled(ref reader);
                    var poolsCount = reader.ReadUshort();
                    for (var i = 0; i < poolsCount; i++) {
                        var guid = reader.ReadGuid();
                        var id = reader.ReadUshort();
                        var byteSize = reader.ReadUint();
                        if (_poolByGuid.TryGetValue(guid, out var pool)) {
                            pool.ReadAll(ref reader);
                            _tempLoadedPools.Add((guid, id));
                        } else {
                            _tempDeletedPoolIds.Add((guid, reader.Position));
                            components.BitMask.DelRangeWithDisabled(0, Entity.entitiesCount, id);
                            reader.SkipNext(byteSize);
                        }
                    }

                    Migration(ref components);
                    
                    foreach (var (id, offset) in _tempDeletedPoolIds) {
                        if (_deleteMigratorByGuid.TryGetValue(id, out var migration)) {
                            var pReader = reader.AsReader(offset);
                            pReader.DeleteAllComponentMigration(migration);
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                private void Migration(ref ModuleComponents value) {
                    if (HasMigration(ref value)) {
                        var bitMap = value.BitMask.CopyBitMapToArrayPool();
                        var bitMapDisabled = value.BitMask.CopyDisabledBitMapToArrayPool();
                        value.BitMask.Clear();
                        
                        for (var i = 0; i < value._poolsCount; i++) {
                            MigrateLoadedPool(value._pools[i], i, ref value, bitMap, bitMapDisabled);
                        }
                        
                        ArrayPool<ulong>.Shared.Return(bitMap);
                        ArrayPool<ulong>.Shared.Return(bitMapDisabled);
                    }
                }

                private bool HasMigration(ref ModuleComponents value) {
                    if (value._poolsCount != _tempLoadedPools.Count) return true;
                    
                    for (var i = 0; i < value._poolsCount; i++) {
                        var cur = value._pools[i];
                        var (guid, id) = _tempLoadedPools[i];

                        if (cur.Guid() != guid || cur.DynamicId() != id) {
                            return true;
                        }
                    }

                    return false;
                }

                private void MigrateLoadedPool(IComponentsWrapper pool, int poolId, ref ModuleComponents components, ulong[] bitMap, ulong[] bitMapDisabled) {
                    foreach (var (guid, id) in _tempLoadedPools) {
                        if (pool.Guid() == guid) {
                            components.BitMask.MigrateWithDisabled(0, Entity.entitiesCount, id, (ushort) poolId, bitMap, bitMapDisabled);
                            return;
                        }
                    }
                }
            }
        }
    }
}