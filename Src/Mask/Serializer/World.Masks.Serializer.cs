#if !FFS_ECS_DISABLE_MASKS
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
            public static void SetMaskDeleteMigrator(Guid id, EcsMaskDeleteMigrationReader<WorldType> migrator) {
                ModuleMasks.Serializer.Value.SetDeleteMigrator(id, migrator);
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        internal partial struct ModuleMasks {
            
            [MethodImpl(AggressiveInlining)]
            internal void WriteGuids(ref BinaryPackWriter writer) {
                writer.WriteCollection(0, _poolsCount, (ref BinaryPackWriter w, int i) => {
                    var guid = Value._pools[i].Guid();
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (guid == Guid.Empty) throw new StaticEcsException($"Mask type {Value._pools[i].GetElementType()} guid not registered");
                    #endif
                    w.WriteGuid(guid);
                });
            }
            
            internal struct Serializer {
                internal static Serializer Value;

                private Dictionary<Guid, IMasksWrapper> _poolByGuid;
                private Dictionary<Guid, EcsMaskDeleteMigrationReader<WorldType>> _deleteMigratorByGuid;
                private List<(Guid id, List<Entity> entities)> _tempDeletedPoolIds;
                private List<(Guid guid, ushort id)> _tempLoadedPools;

                [MethodImpl(AggressiveInlining)]
                internal void Create() {
                    _poolByGuid = new Dictionary<Guid, IMasksWrapper>();
                    _deleteMigratorByGuid = new Dictionary<Guid, EcsMaskDeleteMigrationReader<WorldType>>();
                    _tempDeletedPoolIds = new List<(Guid id, List<Entity> offset)>();
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
                internal void RegisterMaskType<T>(Guid guid) where T : struct, IMask {
                    if (guid != Guid.Empty) {
                        if (_poolByGuid.ContainsKey(guid)) throw new StaticEcsException($"Mask type {typeof(T)} with guid {guid} already registered");
                        _poolByGuid[guid] = new MasksWrapper<T>();
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void SetDeleteMigrator(Guid id, EcsMaskDeleteMigrationReader<WorldType> migrator) {
                    _deleteMigratorByGuid[id] = migrator;
                }

                [MethodImpl(AggressiveInlining)]
                internal void Write(ref BinaryPackWriter writer) {
                    ref var tags = ref ModuleMasks.Value;
                    
                    tags.BitMask.Write(ref writer, Entity.entitiesCount);
                    writer.WriteUshort(tags._poolsCount);
                    for (var i = 0; i < tags._poolsCount; i++) {
                        var pool = tags._pools[i];
                        var guid = pool.Guid();

                        #if DEBUG || FFS_ECS_ENABLE_DEBUG
                        if (!_poolByGuid.ContainsKey(guid)) {
                            throw new StaticEcsException($"Serializer for mask type {pool.GetElementType()} not registered");
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
                    ref var components = ref ModuleMasks.Value;
                    
                    ref var bitMask = ref components.BitMask;
                    var len = bitMask.Len(entity._id);
                    writer.WriteUshort(len);
                    var bufId = bitMask.BorrowBuf();
                    bitMask.CopyToBuffer(entity._id, bufId);
                    while (bitMask.GetMinIndexBuffer(bufId, out var id)) {
                        writer.WriteUshort((ushort) id);
                        bitMask.DelInBuffer(bufId, (ushort) id);
                    }
                    bitMask.DropBuf();
                }

                [MethodImpl(AggressiveInlining)]
                internal void Read(ref BinaryPackReader reader, Guid[] maskGuidById, Entity entity) {
                    var len = reader.ReadUshort();

                    for (int i = 0; i < len; i++) {
                        var id = reader.ReadUshort();
                        var guid = maskGuidById[id];
                        if (_poolByGuid.TryGetValue(guid, out var pool)) {
                            pool.Set(entity);
                        } else if (_deleteMigratorByGuid.TryGetValue(guid, out var migration)) {
                            migration(entity);
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void Read(ref BinaryPackReader reader) {
                    _tempDeletedPoolIds.Clear();
                    _tempLoadedPools.Clear();
                    
                    ref var value = ref ModuleMasks.Value;

                    value.BitMask.Read(ref reader);
                    var poolsCount = reader.ReadUshort();
                    for (var i = 0; i < poolsCount; i++) {
                        var guid = reader.ReadGuid();
                        var id = reader.ReadUshort();
                        var byteSize = reader.ReadUint();
                        if (_poolByGuid.TryGetValue(guid, out var pool)) {
                            pool.ReadAll(ref reader);
                            _tempLoadedPools.Add((guid, id));
                        } else {
                            if (_deleteMigratorByGuid.ContainsKey(guid)) {
                                var entities = new List<Entity>();
                                _tempDeletedPoolIds.Add((guid, entities));

                                for (uint ent = 0; ent < Entity.entitiesCount; ent++) {
                                    if (value.BitMask.Has(ent, id)) {
                                        entities.Add(new Entity(ent));
                                    }
                                }
                            } else {
                                _tempDeletedPoolIds.Add((guid, null));
                            }
                            
                            value.BitMask.DelRange(0, Entity.entitiesCount, id);
                            reader.SkipNext(byteSize);
                        }
                    }

                    Migration();
                    
                    foreach (var (id, entities) in _tempDeletedPoolIds) {
                        if (_deleteMigratorByGuid.TryGetValue(id, out var migration)) {
                            foreach (var entity in entities) {
                                migration(entity);
                            }
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                private void Migration() {
                    ref var value = ref ModuleMasks.Value;
                    
                    if (HasMigration(ref value)) {
                        var bitMap = value.BitMask.CopyBitMapToArrayPool();
                        value.BitMask.Clear();
                        
                        for (var i = 0; i < value._poolsCount; i++) {
                            MigrateLoaded(value._pools[i], i, ref value, bitMap);
                        }
                        
                        ArrayPool<ulong>.Shared.Return(bitMap);
                    }
                }

                private bool HasMigration(ref ModuleMasks value) {
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

                private bool MigrateLoaded(IMasksWrapper pool, int poolId, ref ModuleMasks value, ulong[] bitMap) {
                    foreach (var (guid, id) in _tempLoadedPools) {
                        if (pool.Guid() == guid) {
                            value.BitMask.Migrate(0, Entity.entitiesCount, id, (ushort) poolId, bitMap);
                            return true;
                        }
                    }

                    return false;
                }
            }
        }
    }
}
#endif