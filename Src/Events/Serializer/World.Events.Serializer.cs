#if !FFS_ECS_DISABLE_EVENTS
using System;
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
            public static void SetEventDeleteMigrator(Guid id, EcsEventDeleteMigrationReader<WorldType> migrator) {
                Events.Serializer.Value.SetDeleteMigrator(id, migrator);
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public abstract partial class Events {
            
            [MethodImpl(AggressiveInlining)]
            public static void CreateSnapshot(ref BinaryPackWriter writer) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Event, Method: CreateSnapshot, World not initialized");
                #endif
                Serializer.Value.Write(ref writer);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static byte[] CreateSnapshot(uint byteSizeHint = 512, bool gzip = false) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Event, Method: CreateSnapshot, World not initialized");
                #endif
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                Serializer.Value.Write(ref writer);
                var result = writer.CopyToBytes(gzip);
                writer.Dispose();
                return result;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void CreateSnapshot(ref byte[] result, bool gzip = false) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Event, Method: CreateSnapshot, World not initialized");
                #endif
                var writer = BinaryPackWriter.CreateFromPool((uint) result.Length);
                Serializer.Value.Write(ref writer);
                writer.CopyToBytes(ref result, gzip);
                writer.Dispose();
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void CreateSnapshot(string filePath, bool gzip = false, bool flushToDisk = false, uint byteSizeHint = 512) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Event, Method: CreateSnapshot, World not initialized");
                #endif
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                Serializer.Value.Write(ref writer);
                writer.FlushToFile(filePath, gzip, flushToDisk);
                writer.Dispose();
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void LoadSnapshot(BinaryPackReader reader) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Event, Method: LoadSnapshot, World not initialized");
                #endif
                Serializer.Value.Read(ref reader);
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadSnapshot(byte[] snapshot, bool gzip = false) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Event, Method: LoadSnapshot, World not initialized");
                #endif
                if (gzip) {
                    var writer = BinaryPackWriter.CreateFromPool((uint) (snapshot.Length * 2));
                    writer.WriteGzipData(snapshot);
                    var reader = writer.AsReader();
                    Serializer.Value.Read(ref reader);
                    writer.Dispose();
                } else {
                    var reader = new BinaryPackReader(snapshot, (uint) snapshot.Length, 0);
                    Serializer.Value.Read(ref reader);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadSnapshot(string worldSnapshotFilePath, bool gzip = false, uint byteSizeHint = 512) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Event, Method: LoadSnapshot, World not initialized");
                #endif
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                writer.WriteFromFile(worldSnapshotFilePath, gzip);
                var reader = writer.AsReader();
                Serializer.Value.Read(ref reader);
                writer.Dispose();
            }

            internal struct Serializer {
                internal static Serializer Value;

                private Dictionary<Guid, IEventPoolWrapper> _poolByGuid;
                private Dictionary<Guid, EcsEventDeleteMigrationReader<WorldType>> _deleteMigratorByGuid;
                private List<(Guid id, uint offset)> _tempDeletedPoolIds;

                [MethodImpl(AggressiveInlining)]
                internal void Create() {
                    _poolByGuid = new Dictionary<Guid, IEventPoolWrapper>();
                    _deleteMigratorByGuid = new Dictionary<Guid, EcsEventDeleteMigrationReader<WorldType>>();
                    _tempDeletedPoolIds = new List<(Guid id, uint offset)>();
                }

                [MethodImpl(AggressiveInlining)]
                internal void Destroy() {
                    _deleteMigratorByGuid = null;
                    _tempDeletedPoolIds = null;
                    _poolByGuid = null;
                }

                [MethodImpl(AggressiveInlining)]
                internal void RegisterEventType<T>(IEventConfig<T, WorldType> config) where T : struct, IEvent {
                    var guid = config.Id();
                    if (guid != Guid.Empty) {
                        if (_poolByGuid.ContainsKey(guid)) throw new StaticEcsException($"Event type {typeof(T)} with guid {guid} already registered");
                        _poolByGuid[guid] = new EventPoolWrapper<WorldType, T>();
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void SetDeleteMigrator(Guid id, EcsEventDeleteMigrationReader<WorldType> migrator) {
                    _deleteMigratorByGuid[id] = migrator;
                }

                [MethodImpl(AggressiveInlining)]
                internal void Write(ref BinaryPackWriter writer) {
                    writer.WriteUshort(_poolsCount);
                    
                    ushort len = 0;
                    var point = writer.MakePoint(sizeof(ushort));
                    for (var i = 0; i < _poolsCount; i++) {
                        var pool = _pools[i];
                        var guid = pool.Guid();

                        if (_poolByGuid.TryGetValue(guid, out var wrapper)) {
                            writer.WriteGuid(guid);
                            var offset = writer.MakePoint(sizeof(uint));
                            wrapper.WriteAll(ref writer);
                            writer.WriteUintAt(offset, writer.Position - (offset + sizeof(uint)));
                            len++;
                        }
                    }
                    writer.WriteUshortAt(point, len);
                }

                [MethodImpl(AggressiveInlining)]
                internal void Read(ref BinaryPackReader reader) {
                    _tempDeletedPoolIds.Clear();
                    
                    var poolsCount = reader.ReadUshort();
                    for (var i = 0; i < poolsCount; i++) {
                        var guid = reader.ReadGuid();
                        var byteSize = reader.ReadUint();
                        if (_poolByGuid.TryGetValue(guid, out var pool)) {
                            pool.ReadAll(ref reader);
                        } else {
                            _tempDeletedPoolIds.Add((guid, reader.Position));
                            reader.SkipNext(byteSize);
                        }
                    }
                    
                    foreach (var (id, offset) in _tempDeletedPoolIds) {
                        if (_deleteMigratorByGuid.TryGetValue(id, out var migration)) {
                            var pReader = reader.AsReader(offset);
                            pReader.DeleteAllEventMigration(migration);
                        }
                    }
                }
            }
        }
    }
}
#endif