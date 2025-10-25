#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

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
                writer.WriteCollection(0, poolsCount, (ref BinaryPackWriter w, int i) => {
                    w.WriteGuid(Value._pools[i].Guid());
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
                internal bool TryGetPool(in Guid guid, out IComponentsWrapper pool) {
                    return _poolByGuid.TryGetValue(guid, out pool);
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
                internal void WriteWorld(ref BinaryPackWriter writer, TempChunksData tempChunks) {
                    ref var components = ref ModuleComponents.Value;
                    writer.WriteUshort(components.poolsCount);
                    writer.WriteUshort(components.bitMask.maskLen);
                    writer.WriteUint(tempChunks.ChunksCount);
                    for (var i = 0; i < tempChunks.ChunksCount; i++) {
                        var chunkIdx = tempChunks.Chunks[i];
                        writer.WriteUint(chunkIdx);
                        WriteChunk(ref writer, chunkIdx);
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void WriteChunk(ref BinaryPackWriter writer, uint chunkIdx) {
                    ref var components = ref ModuleComponents.Value;
                    
                    var hasEntities = HasEntitiesInChunk(chunkIdx);
                    writer.WriteBool(hasEntities);
                        
                    if (hasEntities) {
                        components.bitMask.WriteChunk(ref writer, chunkIdx);
                    }
                            
                    for (var j = 0; j < components.poolsCount; j++) {
                        var pool = components._pools[j];
                        var guid = pool.Guid();
                    
                        #if FFS_ECS_DEBUG
                        if (!_poolByGuid.ContainsKey(guid)) {
                            throw new StaticEcsException($"Serializer for component type {pool.GetElementType()} not registered");
                        }
                        #endif
                                
                        writer.WriteGuid(guid);
                        writer.WriteUshort(pool.DynamicId());
                            
                        if (hasEntities) {
                            var sizePosition = writer.MakePoint(sizeof(uint));
                            _poolByGuid[guid].WriteChunk(ref writer, chunkIdx);
                            writer.WriteUintAt(sizePosition, writer.Position - (sizePosition + sizeof(uint)));
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void ReadWorld(ref BinaryPackReader reader) {
                    ref var components = ref ModuleComponents.Value;
                    
                    var poolsCount = reader.ReadUshort();
                    var maskLen = reader.ReadUshort();
                    var chunksCount = reader.ReadUint();
                    
                    if (maskLen > components.bitMask.maskLen) {
                        throw new StaticEcsException($"In the saved data, the mask is {maskLen * 64} components in size, and in the current world {components.bitMask.maskLen * 64}, register at least {maskLen * 64 - 63} components in the world.");
                    }
                    
                    for (var i = 0; i < chunksCount; i++) {
                        var chunkIdx = reader.ReadUint();
                        
                        ReadChunk(ref reader, chunkIdx, poolsCount);
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void ReadChunk(ref BinaryPackReader reader, uint chunkIdx, uint poolsCount) {
                    ref var components = ref ModuleComponents.Value;

                    _tempDeletedPoolIds.Clear();
                    _tempLoadedPools.Clear();

                    var hasEntities = reader.ReadBool();

                    if (hasEntities) {
                        components.bitMask.InitChunk(chunkIdx);
                        components.bitMask.ReadChunk(ref reader, chunkIdx);
                    }

                    for (var j = 0; j < poolsCount; j++) {
                        var guid = reader.ReadGuid();
                        var id = reader.ReadUshort();

                        if (hasEntities) {
                            var byteSize = reader.ReadUint();
                            if (_poolByGuid.TryGetValue(guid, out var pool)) {
                                pool.ReadChunk(ref reader, chunkIdx);
                                _tempLoadedPools.Add((guid, id));
                            } else {
                                _tempDeletedPoolIds.Add((guid, reader.Position));
                                components.bitMask.DelInChunk(chunkIdx, id);
                                reader.SkipNext(byteSize);
                            }
                        }
                    }

                    Migration(chunkIdx);

                    if (_deleteMigratorByGuid.Count > 0) {
                        foreach (var (id, offset) in _tempDeletedPoolIds) {
                            if (_deleteMigratorByGuid.TryGetValue(id, out var migrator)) {
                                var pReader = reader.AsReader(offset);
                                pReader.DeleteAllComponentMigration(migrator, chunkIdx);
                            }
                        }
                    }

                    for (var p = 0; p < components.poolsCount; p++) {
                        components._pools[p].UpdateBitMask(chunkIdx);
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void WriteEntity(ref BinaryPackWriter writer, Entity entity) {
                    ref var components = ref ModuleComponents.Value;
                    ref var bitMask = ref components.bitMask;
                    
                    ushort len = 0;
                    var point = writer.MakePoint(sizeof(ushort));
                    
                    var maskLen = bitMask.MaskLen;
                    var eid = entity.id - Const.ENTITY_ID_OFFSET;
                    var masks = bitMask.Chunk(eid);
                    var start = (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen;
                    for (ushort i = 0; i < maskLen; i++) {
                        var mask = masks[start + i];
                        var offset = i << Const.LONG_SHIFT;
                        while (mask > 0) {
                            var id = Utils.PopLsb(ref mask) + offset;
                            var pool = components._pools[id];
                            if (!pool.Guid().Equals(Guid.Empty)) {
                                writer.WriteUshort((ushort) id);
                                pool.Write(ref writer, entity);
                                len++;
                            }
                        }
                    }
                    writer.WriteUshortAt(point, len);
                }

                [MethodImpl(AggressiveInlining)]
                internal void WriteEntityAndDestroy(ref BinaryPackWriter writer, Entity entity) {
                    ref var components = ref ModuleComponents.Value;
                    ref var bitMask = ref components.bitMask;
                    
                    ushort len = 0;
                    var point = writer.MakePoint(sizeof(ushort));
                    
                    var maskLen = bitMask.MaskLen;
                    var eid = entity.id - Const.ENTITY_ID_OFFSET;
                    var masks = bitMask.Chunk(eid);
                    var start = (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen;
                    for (ushort i = 0; i < maskLen; i++) {
                        ref var mask = ref masks[start + i];
                        var offset = i << Const.LONG_SHIFT;
                        while (mask > 0) {
                            var id = Utils.PopLsb(ref mask) + offset;
                            var pool = components._pools[id];
                            if (!pool.Guid().Equals(Guid.Empty)) {
                                writer.WriteUshort((ushort) id);
                                pool.Write(ref writer, entity);
                                len++;
                            }
                            pool.DeleteInternalWithoutMask(entity);
                        }
                    }
                    writer.WriteUshortAt(point, len);
                }

                [MethodImpl(AggressiveInlining)]
                internal void ReadEntity(ref BinaryPackReader reader, Guid[] componentGuidById, IComponentsWrapper[] dynamicPoolMap, Entity entity) {
                    var len = reader.ReadUshort();

                    for (var i = 0; i < len; i++) {
                        var id = reader.ReadUshort();
                        var pool = dynamicPoolMap[id];
                        if (pool != null) {
                            pool.Read(ref reader, entity);
                        } else {
                            if (_deleteMigratorByGuid.TryGetValue(componentGuidById[id], out var migration)) {
                                reader.DeleteOneComponentMigration(entity, migration);
                            } else {
                                reader.SkipOneComponent();
                            }
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                private void Migration(uint chunkIdx) {
                    ref var value = ref ModuleComponents.Value;
                    
                    if (HasMigration(ref value)) {
                        var bitMapChunk = value.bitMask.CopyBitMapChunkToArrayPool(chunkIdx);
                        value.bitMask.ClearChunk(chunkIdx);
                        
                        for (var i = 0; i < value.poolsCount; i++) {
                            MigrateChunkMask(value._pools[i], i, ref value, bitMapChunk, chunkIdx);
                        }
                        
                        ArrayPool<ulong>.Shared.Return(bitMapChunk);
                    }
                }
                
                private bool HasMigration(ref ModuleComponents value) {
                    if (value.poolsCount != _tempLoadedPools.Count) return true;
                    
                    for (var i = 0; i < value.poolsCount; i++) {
                        var cur = value._pools[i];
                        var (guid, id) = _tempLoadedPools[i];
                
                        if (cur.Guid() != guid || cur.DynamicId() != id) {
                            return true;
                        }
                    }
                
                    return false;
                }
                
                private void MigrateChunkMask(IComponentsWrapper pool, int poolId, ref ModuleComponents value, ulong[] bitMap, uint chunkIdx) {
                    foreach (var (guid, id) in _tempLoadedPools) {
                        if (pool.Guid() == guid) {
                            value.bitMask.MigrateChunk(chunkIdx, id, (ushort) poolId, bitMap);
                            return;
                        }
                    }
                }
            }
        }
    }
}