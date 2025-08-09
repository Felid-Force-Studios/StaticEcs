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
            public static void SetStandardComponentDeleteMigrator(Guid id, EcsStandardComponentDeleteMigrationReader<WorldType> migrator) {
                ModuleStandardComponents.Serializer.Value.SetDeleteMigrator(id, migrator);
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        internal partial struct ModuleStandardComponents {
            
            [MethodImpl(AggressiveInlining)]
            internal void WriteGuids(ref BinaryPackWriter writer) {
                writer.WriteCollection(0, _poolsCount, (ref BinaryPackWriter w, int i) => {
                    w.WriteGuid(Value._pools[i].Guid());
                });
            }
            
            internal struct Serializer {
                internal static Serializer Value;

                private Dictionary<Guid, IStandardComponentsWrapper> _poolByGuid;
                private Dictionary<Guid, EcsStandardComponentDeleteMigrationReader<WorldType>> _deleteMigratorByGuid;
                private List<(Guid id, uint offset)> _tempDeletedPoolIds;

                [MethodImpl(AggressiveInlining)]
                internal void Create() {
                    _poolByGuid = new Dictionary<Guid, IStandardComponentsWrapper>();
                    _deleteMigratorByGuid = new Dictionary<Guid, EcsStandardComponentDeleteMigrationReader<WorldType>>();
                    _tempDeletedPoolIds = new List<(Guid id, uint offset)>();
                }

                [MethodImpl(AggressiveInlining)]
                internal void Destroy() {
                    _deleteMigratorByGuid = null;
                    _tempDeletedPoolIds = null;
                    _poolByGuid = null;
                }

                [MethodImpl(AggressiveInlining)]
                internal void RegisterComponentType<T>(IStandardComponentConfig<T, WorldType> config) where T : struct, IStandardComponent {
                    var guid = config.Id();
                    if (guid != Guid.Empty) {
                        if (_poolByGuid.ContainsKey(guid)) throw new StaticEcsException($"Component type {typeof(T)} with guid {guid} already registered");
                        _poolByGuid[guid] = new StandardComponentsWrapper<T>();
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void SetDeleteMigrator(Guid id, EcsStandardComponentDeleteMigrationReader<WorldType> migrator) {
                    _deleteMigratorByGuid[id] = migrator;
                }

                [MethodImpl(AggressiveInlining)]
                internal void Write(ref BinaryPackWriter writer) {
                    ref var components = ref ModuleStandardComponents.Value;
                    
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
                    ref var components = ref ModuleStandardComponents.Value;
                    
                    ushort len = 0;
                    var offset = writer.MakePoint(sizeof(ushort));
                    for (int id = 0; id < components._poolsCount; id++) {
                        var pool = components._pools[id];
                        if (!pool.Guid().Equals(Guid.Empty)) {
                            writer.WriteUshort((ushort) id);
                            pool.Write(ref writer, entity);
                            len++;
                        }
                    }
                    writer.WriteUshortAt(offset, len);
                }

                [MethodImpl(AggressiveInlining)]
                internal void Read(ref BinaryPackReader reader, Guid[] standardComponentGuidById, Entity entity) {
                    var len = reader.ReadUshort();

                    for (int i = 0; i < len; i++) {
                        var id = reader.ReadUshort();
                        var guid = standardComponentGuidById[id];
                        if (_poolByGuid.TryGetValue(guid, out var pool)) {
                            pool.Read(ref reader, entity);
                        } else {
                            if (_deleteMigratorByGuid.TryGetValue(guid, out var migration)) {
                                reader.DeleteOneStandardComponentMigration(entity, migration);
                            }
                            
                            reader.SkipOneStandardComponent();
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void Read(ref BinaryPackReader reader) {
                    _tempDeletedPoolIds.Clear();
                    
                    var poolsCount = reader.ReadUshort();
                    for (var i = 0; i < poolsCount; i++) {
                        var guid = reader.ReadGuid();
                        reader.SkipNext(sizeof(ushort)); // ID
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
                            pReader.DeleteAllStandardComponentMigration(migration);
                        }
                    }
                }
            }
        }
    }
}