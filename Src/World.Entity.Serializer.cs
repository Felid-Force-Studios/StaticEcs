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
            public static byte[] CreateGIDStoreSnapshot() {
                return GIDStore.Value.AsSnapshot().WriteToBytes();
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void CreateGIDStoreSnapshot(ref byte[] result) {
                GIDStore.Value.AsSnapshot().WriteToBytes(ref result);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void CreateGIDStoreSnapshot(string filePath, bool gzip = false, bool flushToDisk = false) {
                GIDStore.Value.AsSnapshot().WriteToFile(filePath, gzip, flushToDisk);
            }

            [MethodImpl(AggressiveInlining)]
            public static EntitiesWriter CreateEntitiesSnapshotWriter(uint byteSizeHint = 16384) {
                return new EntitiesWriter(
                    byteSizeHint: byteSizeHint
                );
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadEntitiesSnapshot(BinaryPackReader reader, bool entitiesAsNew, Action<Entity> onLoad = null) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: LoadEntitiesSnapshot, World not initialized");
                if (!GidStoreLoaded && !entitiesAsNew)
                    throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: LoadEntities, to load entitiesAsNew == false, initialize the world with GIDStoreSnapshot");
                #endif

                var actions = PreLoadSnapshotCallbacksBySnapshotType[(int) SnapshotActionType.Entities];
                for (var i = 0; i < actions.Count; i++) {
                    actions[i]();
                }

                var entitiesCount = reader.ReadUint();
                var standardComponentGuidById = reader.ReadArrayUnmanagedPooled<Guid>(out var h2).Array;
                var componentGuidById = reader.ReadArrayUnmanagedPooled<Guid>(out var h1).Array;
                var tagGuidById = reader.ReadArrayUnmanagedPooled<Guid>(out var h3).Array;
                var maskGuidById = reader.ReadArrayUnmanagedPooled<Guid>(out var h4).Array;

                var entities = ArrayPool<Entity>.Shared.Rent((int) entitiesCount);

                for (var i = 0; i < entitiesCount; i++) {
                    var gid = new EntityGID(reader.ReadUint());
                    var entity = entitiesAsNew
                        ? Entity.New()
                        : Entity.New(gid);
                    entities[i] = entity;
                    ModuleStandardComponents.Serializer.Value.Read(ref reader, standardComponentGuidById, entity);
                    ModuleComponents.Serializer.Value.Read(ref reader, componentGuidById, entity);
                    #if !FFS_ECS_DISABLE_TAGS
                    ModuleTags.Serializer.Value.Read(ref reader, tagGuidById, entity);
                    #endif
                    #if !FFS_ECS_DISABLE_MASKS
                    ModuleMasks.Serializer.Value.Read(ref reader, maskGuidById, entity);
                    #endif
                }

                h1.Return();
                h2.Return();
                h3.Return();
                h4.Return();

                ReadEntitiesSnapshotData(ref reader);
                var count = reader.ReadInt();
                var serializers = SnapshotDataEntitySerializersBySnapshotType[(int) SnapshotActionType.Entities];

                for (var i = 0; i < count; i++) {
                    var key = reader.ReadGuid();
                    var version = reader.ReadUshort();
                    var byteSize = reader.ReadUint();
                    if (serializers.TryGetValue(key, out var val)) {
                        for (var j = 0; j < entitiesCount; j++) {
                            val.reader(ref reader, entities[j], version);
                        }
                    } else {
                        reader.SkipNext(byteSize);
                    }
                }

                actions = PostLoadSnapshotCallbacksTypeBySnapshotType[(int) SnapshotActionType.Entities];
                for (var i = 0; i < actions.Count; i++) {
                    actions[i]();
                }

                var entityActions = OnRestoreEntityFromSnapshotActionsBySnapshotType[(int) SnapshotActionType.Entities];
                if (entityActions.Count > 0) {
                    for (var i = 0; i < entitiesCount; i++) {
                        for (var j = 0; j < entityActions.Count; j++) {
                            entityActions[j](entities[i]);
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
            public static void LoadEntitiesSnapshot(byte[] data, bool entitiesAsNew, Action<Entity> onLoad = null, bool gzip = false, uint gzipByteSizeHint = 16384) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: LoadEntitiesSnapshot, World not initialized");
                #endif
                if (gzip) {
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
            public static void LoadEntitiesSnapshot(string filePath, bool entitiesAsNew, Action<Entity> onLoad = null, bool gzip = false, uint byteSizeHint = 16384) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: LoadEntitiesSnapshot, World not initialized");
                #endif
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
                internal Entity[] Entities;
                internal uint EntitiesCount;
                internal uint StartWriterPosition;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                internal bool Destroyed;
                #endif

                [MethodImpl(AggressiveInlining)]
                public EntitiesWriter(uint byteSizeHint = 16384) {
                    EntitiesCount = 0;
                    Entities = ArrayPool<Entity>.Shared.Rent((int) Entity.entitiesCapacity);
                    Writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                    Writer.Skip(sizeof(uint));
                    ModuleStandardComponents.Value.WriteGuids(ref Writer);
                    ModuleComponents.Value.WriteGuids(ref Writer);
                    #if !FFS_ECS_DISABLE_TAGS
                    ModuleTags.Value.WriteGuids(ref Writer);
                    #else
                    Writer.WriteNotNullFlag(null);
                    #endif
                    #if !FFS_ECS_DISABLE_MASKS
                    ModuleMasks.Value.WriteGuids(ref Writer);
                    #else
                    Writer.WriteNotNullFlag(null);
                    #endif
                    StartWriterPosition = Writer.Position;
                    
                    var actions = PreCreateSnapshotCallbacksBySnapshotType[(int) SnapshotActionType.Entities];
                    for (var i = 0; i < actions.Count; i++) {
                        actions[i]();
                    }
                    
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    Destroyed = false;
                    #endif
                }

                [MethodImpl(AggressiveInlining)]
                public void Write(Entity entity) {
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (Destroyed) throw new StaticEcsException($"World<{typeof(WorldType)}>.EntitiesWriter, Method: Write, SnapshotWriter is destroyed");
                    #endif

                    Writer.WriteUint(GIDStore.Value.Get(entity).id);
                    ModuleStandardComponents.Serializer.Value.Write(ref Writer, entity);
                    ModuleComponents.Serializer.Value.Write(ref Writer, entity);
                    #if !FFS_ECS_DISABLE_TAGS
                    ModuleTags.Serializer.Value.Write(ref Writer, entity);
                    #endif
                    #if !FFS_ECS_DISABLE_MASKS
                    ModuleMasks.Serializer.Value.Write(ref Writer, entity);
                    #endif
                    Entities[EntitiesCount++] = entity;
                }

                [MethodImpl(AggressiveInlining)]
                public byte[] CreateSnapshot(bool withCustomSnapshotData = true, bool gzip = false) {
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (Destroyed) throw new StaticEcsException($"World<{typeof(WorldType)}>.EntitiesWriter, Method: CreateSnapshot, SnapshotWriter is destroyed");
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
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (Destroyed) throw new StaticEcsException($"World<{typeof(WorldType)}>.EntitiesWriter, Method: CreateSnapshot, SnapshotWriter is destroyed");
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
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (Destroyed) throw new StaticEcsException($"World<{typeof(WorldType)}>.EntitiesWriter, Method: CreateSnapshot, SnapshotWriter is destroyed");
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
                    if (withCustomSnapshotData) {
                        WriteEntitiesSnapshotData(ref Writer);
                        var serializers = SnapshotDataEntitySerializersBySnapshotType[(int) SnapshotActionType.Entities];
                        Writer.WriteInt(serializers.Count);
                        foreach (var (key, (writer, _, version)) in serializers) {
                            Writer.WriteGuid(key);
                            Writer.WriteUshort(version);
                            var point = Writer.MakePoint(sizeof(uint));
                            for (var i = 0; i < EntitiesCount; i++) {
                                writer(ref Writer, Entities[i]);
                            }

                            Writer.WriteUintAt(point, Writer.Position - (point + sizeof(uint)));
                        }
                    } else {
                        Writer.WriteInt(0);
                        Writer.WriteInt(0);
                    }

                    var actions = PostCreateSnapshotCallbacksBySnapshotType[(int) SnapshotActionType.Entities];
                    for (var i = 0; i < actions.Count; i++) {
                        actions[i]();
                    }

                    var entityActions = OnCreateEntitySnapshotActionsBySnapshotType[(int) SnapshotActionType.Entities];
                    if (entityActions.Count > 0) {
                        for (var i = 0; i < EntitiesCount; i++) {
                            for (var j = 0; j < entityActions.Count; j++) {
                                entityActions[j](Entities[i]);
                            }
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                public void Dispose() {
                    Writer.Dispose();
                    if (Entities != null) {
                        ArrayPool<Entity>.Shared.Return(Entities);
                    }
                }
            }
        }
    }
}