#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    public abstract partial class World<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public static partial class Serializer {
            [MethodImpl(AggressiveInlining)]
            public static byte[] CreateGIDStoreSnapshot(bool gzip = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, ReadOnlySpan<ushort> clusters = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                clusters = HandleClustersRange(clusters);

                var tempChunks = TempChunksData.Create();
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                Entities.Value.Write(ref writer, strategy, clusters, ref tempChunks, false);
                var result = writer.CopyToBytes(gzip);
                writer.Dispose();
                tempChunks.Dispose();
                return result;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void CreateGIDStoreSnapshot(ref byte[] result, bool gzip = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, ReadOnlySpan<ushort> clusters = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                clusters = HandleClustersRange(clusters);
                
                var tempChunks = TempChunksData.Create();
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                Entities.Value.Write(ref writer, strategy, clusters, ref tempChunks, false);
                writer.CopyToBytes(ref result, gzip);
                writer.Dispose();
                tempChunks.Dispose();
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void CreateGIDStoreSnapshot(string filePath, bool gzip = false, bool flushToDisk = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, ReadOnlySpan<ushort> clusters = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                clusters = HandleClustersRange(clusters);
                
                var tempChunks = TempChunksData.Create();
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                Entities.Value.Write(ref writer, strategy, clusters, ref tempChunks, false);
                writer.FlushToFile(filePath, gzip, flushToDisk);
                writer.Dispose();
                tempChunks.Dispose();
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void RestoreFromGIDStoreSnapshot(BinaryPackReader reader) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                Entities.Value.Read(ref reader, false);
            }

            [MethodImpl(AggressiveInlining)]
            public static void RestoreFromGIDStoreSnapshot(byte[] snapshot, bool gzip = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                if (gzip) {
                    var writer = BinaryPackWriter.CreateFromPool((uint) (snapshot.Length * 2));
                    writer.WriteGzipData(snapshot);
                    var reader = writer.AsReader();
                    Entities.Value.Read(ref reader, false);
                    writer.Dispose();
                } else {
                    var reader = new BinaryPackReader(snapshot, (uint) snapshot.Length, 0);
                    Entities.Value.Read(ref reader, false);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void RestoreFromGIDStoreSnapshot(string worldSnapshotFilePath, bool gzip = false, uint byteSizeHint = 0) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                CalculateByteSizeHint(ref byteSizeHint);
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                writer.WriteFromFile(worldSnapshotFilePath, gzip);
                var reader = writer.AsReader();
                Entities.Value.Read(ref reader, false);
                writer.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            public static EntitiesWriter CreateEntitiesSnapshotWriter(uint byteSizeHint = 0) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                
                CalculateByteSizeHint(ref byteSizeHint);
                return new EntitiesWriter(
                    byteSizeHint: byteSizeHint
                );
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadEntitiesSnapshot(BinaryPackReader reader, bool entitiesAsNew = false, QueryFunctionWithEntity<WorldType> onLoad = null) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif

                var snapshotParams = new SnapshotReadParams(SnapshotType.Entities, entitiesAsNew);

                BeforeRead(snapshotParams);

                var entitiesCount = reader.ReadUint();
                var componentGuidById = reader.ReadArrayUnmanagedPooled<Guid>(out var h1).Array;
                var tagGuidById = reader.ReadArrayUnmanagedPooled<Guid>(out var h3).Array;
                
                var dynamicComponentsPoolMap = ArrayPool<IComponentsWrapper>.Shared.Rent(componentGuidById!.Length);
                for (var i = 0; i < componentGuidById.Length; i++) {
                    if (ModuleComponents.Serializer.Value.TryGetPool(componentGuidById[i], out var pool)) {
                        dynamicComponentsPoolMap[i] = pool;
                    } else {
                        dynamicComponentsPoolMap[i] = null;
                    }
                }
                
                var dynamicTagsPoolMap = ArrayPool<ITagsWrapper>.Shared.Rent(tagGuidById!.Length);
                for (var i = 0; i < tagGuidById.Length; i++) {
                    if (ModuleTags.Serializer.Value.TryGetPool(tagGuidById[i], out var pool)) {
                        dynamicTagsPoolMap[i] = pool;
                    } else {
                        dynamicTagsPoolMap[i] = null;
                    }
                }

                var entities = ArrayPool<Entity>.Shared.Rent((int) entitiesCount);

                for (var i = 0; i < entitiesCount; i++) {
                    var gid = reader.ReadEntityGID();

                    Entity entity;
                    if (entitiesAsNew) {
                        entity = Entity.New(gid.ClusterId);
                    } else {
                        Entities.Value.LoadEntity(gid, out entity);
                    }
                    entities[i] = entity;
                    var disabled = reader.ReadBool();
                    if (disabled) {
                        entity.Disable();
                    }
                    ModuleComponents.Serializer.Value.ReadEntity(ref reader, componentGuidById, dynamicComponentsPoolMap, entity);
                    ModuleTags.Serializer.Value.ReadEntity(ref reader, tagGuidById, dynamicTagsPoolMap, entity);
                }
                
                ArrayPool<IComponentsWrapper>.Shared.Return(dynamicComponentsPoolMap, true);
                ArrayPool<ITagsWrapper>.Shared.Return(dynamicTagsPoolMap, true);

                h1.Return();
                h3.Return();

                ReadSnapshotData(ref reader, snapshotParams);
                var count = reader.ReadInt();

                for (var i = 0; i < count; i++) {
                    var key = reader.ReadGuid();
                    var version = reader.ReadUshort();
                    var byteSize = reader.ReadUint();
                    if (SnapshotDataEntitySerializers.TryGetValue(key, out var val)) {
                        for (var j = 0; j < entitiesCount; j++) {
                            val.reader(ref reader, entities[j], version, snapshotParams);
                        }
                    } else {
                        reader.SkipNext(byteSize);
                    }
                }

                for (var i = 0; i < PostLoadSnapshotCallbacksType.Count; i++) {
                    PostLoadSnapshotCallbacksType[i](snapshotParams);
                }

                if (OnRestoreEntityFromSnapshotActions.Count > 0) {
                    for (var i = 0; i < entitiesCount; i++) {
                        for (var j = 0; j < OnRestoreEntityFromSnapshotActions.Count; j++) {
                            OnRestoreEntityFromSnapshotActions[j](entities[i], snapshotParams);
                        }
                    }
                }

                if (onLoad != null) {
                    for (var j = 0; j < entitiesCount; j++) {
                        onLoad.Invoke(entities[j]);
                    }
                }

                ArrayPool<Entity>.Shared.Return(entities);
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadEntitiesSnapshot(byte[] data, bool entitiesAsNew = false, QueryFunctionWithEntity<WorldType> onLoad = null, bool gzip = false, uint gzipByteSizeHint = 0) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                if (gzip) {
                    CalculateByteSizeHint(ref gzipByteSizeHint);
                    var writer = BinaryPackWriter.CreateFromPool(gzipByteSizeHint);
                    writer.WriteGzipData(data);
                    LoadEntitiesSnapshot(writer.AsReader(), entitiesAsNew, onLoad);
                    writer.Dispose();
                } else {
                    var reader = new BinaryPackReader(data, (uint) data.Length, 0);
                    LoadEntitiesSnapshot(reader, entitiesAsNew);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadEntitiesSnapshot(string filePath, bool entitiesAsNew = false, QueryFunctionWithEntity<WorldType> onLoad = null, bool gzip = false, uint byteSizeHint = 0) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                CalculateByteSizeHint(ref byteSizeHint);
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                writer.WriteFromFile(filePath, gzip);
                var reader = writer.AsReader();
                LoadEntitiesSnapshot(reader, entitiesAsNew, onLoad);
                writer.Dispose();
            }

            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            public struct EntitiesWriter : IDisposable {
                internal BinaryPackWriter Writer;
                internal Entity[] EntityIds;
                internal uint EntitiesCount;
                internal uint StartWriterPosition;
                #if FFS_ECS_DEBUG
                internal bool Destroyed;
                #endif

                [MethodImpl(AggressiveInlining)]
                public EntitiesWriter(uint byteSizeHint) {
                    EntitiesCount = 0;
                    EntityIds = ArrayPool<Entity>.Shared.Rent(Entities.Value.chunks.Length << Const.ENTITIES_IN_CHUNK_SHIFT);
                    Writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                    Writer.Skip(sizeof(uint));
                    ModuleComponents.Value.WriteGuids(ref Writer);
                    ModuleTags.Value.WriteGuids(ref Writer);
                    StartWriterPosition = Writer.Position;
                    
                    BeforeWrite(new SnapshotWriteParams(SnapshotType.Entities));
                    
                    #if FFS_ECS_DEBUG
                    Destroyed = false;
                    #endif
                }

                [MethodImpl(AggressiveInlining)]
                public void Write(Entity entity) {
                    #if FFS_ECS_DEBUG
                    Assert("EntitiesWriter", !Destroyed, "EntitiesWriter is destroyed");
                    #endif
  
                    Writer.WriteUlong(entity.Gid().Raw);
                    Writer.WriteBool(entity.IsDisabled());
                    ModuleComponents.Serializer.Value.WriteEntity(ref Writer, entity);
                    ModuleTags.Serializer.Value.WriteEntity(ref Writer, entity);
                    EntityIds[EntitiesCount++] = entity;
                }

                [MethodImpl(AggressiveInlining)]
                public void WriteAndUnload(Entity entity) {
                    #if FFS_ECS_DEBUG
                    Assert("EntitiesWriter", !Destroyed, "EntitiesWriter is destroyed");
                    #endif
  
                    Writer.WriteUlong(entity.Gid().Raw);
                    Writer.WriteBool(entity.IsDisabled());
                    ModuleComponents.Serializer.Value.WriteEntityAndDestroy(ref Writer, entity);
                    ModuleTags.Serializer.Value.WriteEntityAndDestroy(ref Writer, entity);
                    Entities.Value.UnloadEntity(entity);
                    EntityIds[EntitiesCount++] = entity;
                }

                [MethodImpl(AggressiveInlining)]
                public void WriteAllEntities() {
                    #if FFS_ECS_DEBUG
                    Assert("EntitiesWriter", !Destroyed, "EntitiesWriter is destroyed");
                    #endif
                    
                    Query.For(ref this, (ref EntitiesWriter self, Entity entity) => self.Write(entity), EntityStatusType.Any);
                }

                [MethodImpl(AggressiveInlining)]
                public void WriteAndUnloadAllEntities() {
                    #if FFS_ECS_DEBUG
                    Assert("EntitiesWriter", !Destroyed, "EntitiesWriter is destroyed");
                    #endif
                    
                    Query.For(ref this, (ref EntitiesWriter self, Entity entity) => self.WriteAndUnload(entity), EntityStatusType.Any);
                }

                [MethodImpl(AggressiveInlining)]
                public byte[] CreateSnapshot(bool withCustomSnapshotData = true, bool gzip = false) {
                    #if FFS_ECS_DEBUG
                    Assert("EntitiesWriter", !Destroyed, "EntitiesWriter is destroyed");
                    Destroyed = true;
                    #endif
                    Writer.WriteUintAt(0, EntitiesCount);
                    CreateCustomSnapshot(withCustomSnapshotData);
                    var result = Writer.CopyToBytes(gzip);
                    Writer.Position = StartWriterPosition;
                    EntitiesCount = 0;
                    return result;
                }

                [MethodImpl(AggressiveInlining)]
                public void CreateSnapshot(ref byte[] result, bool withCustomSnapshotData = true, bool gzip = false) {
                    #if FFS_ECS_DEBUG
                    Assert("EntitiesWriter", !Destroyed, "EntitiesWriter is destroyed");
                    Destroyed = true;
                    #endif
                    Writer.WriteUintAt(0, EntitiesCount);
                    CreateCustomSnapshot(withCustomSnapshotData);
                    Writer.CopyToBytes(ref result, gzip);
                    Writer.Position = StartWriterPosition;
                    EntitiesCount = 0;
                }

                [MethodImpl(AggressiveInlining)]
                public void CreateSnapshot(string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false) {
                    #if FFS_ECS_DEBUG
                    Assert("EntitiesWriter", !Destroyed, "EntitiesWriter is destroyed");
                    Destroyed = true;
                    #endif
                    Writer.WriteUintAt(0, EntitiesCount);
                    CreateCustomSnapshot(withCustomSnapshotData);
                    Writer.FlushToFile(filePath, gzip, flushToDisk);
                    Writer.Position = StartWriterPosition;
                    EntitiesCount = 0;
                }

                [MethodImpl(AggressiveInlining)]
                private void CreateCustomSnapshot(bool withCustomSnapshotData) {
                    var snapshotParams = new SnapshotWriteParams(SnapshotType.Entities);
                    
                    if (withCustomSnapshotData) {
                        WriteSnapshotData(ref Writer, snapshotParams);
                        Writer.WriteInt(SnapshotDataEntitySerializers.Count);
                        foreach (var (key, (writer, _, version)) in SnapshotDataEntitySerializers) {
                            Writer.WriteGuid(key);
                            Writer.WriteUshort(version);
                            var point = Writer.MakePoint(sizeof(uint));
                            for (var i = 0; i < EntitiesCount; i++) {
                                writer(ref Writer, EntityIds[i], snapshotParams);
                            }

                            Writer.WriteUintAt(point, Writer.Position - (point + sizeof(uint)));
                        }
                    } else {
                        Writer.WriteInt(0);
                        Writer.WriteInt(0);
                    }

                    for (var i = 0; i < PostCreateSnapshotCallbacks.Count; i++) {
                        PostCreateSnapshotCallbacks[i](snapshotParams);
                    }

                    if (OnCreateEntitySnapshotActions.Count > 0) {
                        for (var i = 0; i < EntitiesCount; i++) {
                            for (var j = 0; j < OnCreateEntitySnapshotActions.Count; j++) {
                                OnCreateEntitySnapshotActions[j](EntityIds[i], snapshotParams);
                            }
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                public void Dispose() {
                    Writer.Dispose();
                    if (EntityIds != null) {
                        ArrayPool<Entity>.Shared.Return(EntityIds);
                    }
                }
            }
        }
    }
}