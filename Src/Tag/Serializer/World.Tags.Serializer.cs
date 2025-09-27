#if !FFS_ECS_DISABLE_TAGS
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
            public static void SetTagDeleteMigrator(Guid id, EcsTagDeleteMigrationReader<WorldType> migrator) {
                ModuleTags.Serializer.Value.SetDeleteMigrator(id, migrator);
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        internal partial struct ModuleTags {
            
            [MethodImpl(AggressiveInlining)]
            internal void WriteGuids(ref BinaryPackWriter writer) {
                writer.WriteCollection(0, _poolsCount, (ref BinaryPackWriter w, int i) => {
                    w.WriteGuid(Value._pools[i].Guid());
                });
            }
            
            internal struct Serializer {
                internal static Serializer Value;

                private Dictionary<Guid, ITagsWrapper> _poolByGuid;
                private Dictionary<Guid, EcsTagDeleteMigrationReader<WorldType>> _deleteMigratorByGuid;
                private List<(Guid id, uint offset)> _tempDeletedPoolIds;
                private List<(Guid guid, ushort id)> _tempLoadedPools;

                [MethodImpl(AggressiveInlining)]
                internal void Create() {
                    _poolByGuid = new Dictionary<Guid, ITagsWrapper>();
                    _deleteMigratorByGuid = new Dictionary<Guid, EcsTagDeleteMigrationReader<WorldType>>();
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
                internal void RegisterTagType<T>(Guid guid) where T : struct, ITag {
                    if (guid != Guid.Empty) {
                        if (_poolByGuid.ContainsKey(guid)) throw new StaticEcsException($"Tag type {typeof(T)} with guid {guid} already registered");
                        _poolByGuid[guid] = new TagsWrapper<T>();
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void SetDeleteMigrator(Guid id, EcsTagDeleteMigrationReader<WorldType> migrator) {
                    _deleteMigratorByGuid[id] = migrator;
                }

                [MethodImpl(AggressiveInlining)]
                internal void Write(ref BinaryPackWriter writer) {
                    ref var tags = ref ModuleTags.Value;
                    tags._bitMask.Write(ref writer, Entities.Value.nextActiveChunkIdx, tags._poolsCount == 0);
                    
                    writer.WriteUshort(tags._poolsCount);
                    for (var i = 0; i < tags._poolsCount; i++) {
                        var pool = tags._pools[i];
                        var guid = pool.Guid();

                        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                        if (!_poolByGuid.ContainsKey(guid)) {
                            throw new StaticEcsException($"Serializer for tag type {pool.GetElementType()} not registered");
                        }
                        #endif
                        writer.WriteGuid(guid);
                        writer.WriteUshort(pool.DynamicId());
                        var sizePosition = writer.MakePoint(sizeof(uint));
                        _poolByGuid[guid].WriteAll(ref writer);
                        writer.WriteUintAt(sizePosition, writer.Position - (sizePosition + sizeof(uint)));
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void Read(ref BinaryPackReader reader) {
                    _tempDeletedPoolIds.Clear();
                    _tempLoadedPools.Clear();
                    ref var components = ref ModuleTags.Value;
                    
                    components._bitMask.Read(ref reader);
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
                            components._bitMask.DelRange(0, Entities.Value.entityIdSeq, id);
                            reader.SkipNext(byteSize);
                        }
                    }

                    Migration();
                    
                    foreach (var (id, offset) in _tempDeletedPoolIds) {
                        if (_deleteMigratorByGuid.TryGetValue(id, out var migration)) {
                            var pReader = reader.AsReader(offset);
                            pReader.DeleteAllTagMigration(migration);
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void Write(ref BinaryPackWriter writer, Entity entity) {
                    ref var tags = ref ModuleTags.Value;
                    ref var bitMask = ref tags._bitMask;
                    
                    ushort len = 0;
                    var point = writer.MakePoint(sizeof(ushort));
                    
                    var maskLen = bitMask.MaskLen;
                    var masks = bitMask.Chunk(entity._id);
                    var start = (entity._id & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen;
                    for (ushort i = 0; i < maskLen; i++) {
                        var mask = masks[start + i];
                        var offset = i << Const.LONG_SHIFT;
                        while (mask > 0) {
                            var id = Utils.PopLsb(ref mask) + offset;
                            var pool = tags._pools[id];
                            if (!pool.Guid().Equals(Guid.Empty)) {
                                writer.WriteUshort((ushort) id);
                                len++;
                            }
                        }
                    }
   
                    writer.WriteUshortAt(point, len);
                }

                [MethodImpl(AggressiveInlining)]
                internal void Read(ref BinaryPackReader reader, Guid[] tagGuidById, Entity entity) {
                    var len = reader.ReadUshort();

                    for (int i = 0; i < len; i++) {
                        var id = reader.ReadUshort();
                        var guid = tagGuidById[id];
                        if (_poolByGuid.TryGetValue(guid, out var pool)) {
                            pool.Set(entity);
                        } else if (_deleteMigratorByGuid.TryGetValue(guid, out var migration)) {
                            migration(entity);
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                private void Migration() {
                    ref var value = ref ModuleTags.Value;
                    
                    if (HasMigration(ref value)) {
                        var bitMap = value._bitMask.CopyBitMapToArrayPool();
                        value._bitMask.Clear(Entities.Value.nextActiveChunkIdx);
                        
                        for (var i = 0; i < value._poolsCount; i++) {
                            MigrateLoaded(value._pools[i], i, ref value, bitMap);
                        }
                        
                        for (var i = 0; i < value._bitMask.chunks.Length; i++) {
                            ArrayPool<ulong>.Shared.Return(bitMap[i]);
                        }
                        
                        ArrayPool<ulong[]>.Shared.Return(bitMap);
                    }
                }

                private bool HasMigration(ref ModuleTags value) {
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

                private bool MigrateLoaded(ITagsWrapper pool, int poolId, ref ModuleTags value, ulong[][] bitMap) {
                    foreach (var (guid, id) in _tempLoadedPools) {
                        if (pool.Guid() == guid) {
                            value._bitMask.Migrate(0, Entities.Value.entityIdSeq, id, (ushort) poolId, bitMap);
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