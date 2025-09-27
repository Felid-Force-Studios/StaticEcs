using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    public delegate void CustomSnapshotDataReader(ref BinaryPackReader reader, ushort version);

    public delegate void CustomSnapshotDataWriter(ref BinaryPackWriter writer);

    public delegate void CustomSnapshotEntityDataReader<WorldType>(ref BinaryPackReader reader, World<WorldType>.Entity entity, ushort version) where WorldType : struct, IWorldType;

    public delegate void CustomSnapshotEntityDataWriter<WorldType>(ref BinaryPackWriter writer, World<WorldType>.Entity entity) where WorldType : struct, IWorldType;

    public enum SnapshotActionType {
        Entities,
        World,
        All
    }

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
            internal static Dictionary<Guid, (CustomSnapshotDataWriter writer, CustomSnapshotDataReader reader, ushort version)>[] SnapshotDataSerializersBySnapshotType;

            internal static Dictionary<Guid, (CustomSnapshotEntityDataWriter<WorldType> writer, CustomSnapshotEntityDataReader<WorldType> reader, ushort version)>[]
                SnapshotDataEntitySerializersBySnapshotType;

            internal static List<Action>[] PreCreateSnapshotCallbacksBySnapshotType;
            internal static List<Action>[] PreLoadSnapshotCallbacksBySnapshotType;
            internal static List<Action>[] PostCreateSnapshotCallbacksBySnapshotType;
            internal static List<Action>[] PostLoadSnapshotCallbacksTypeBySnapshotType;
            internal static List<QueryFunctionWithEntity<WorldType>>[] OnCreateEntitySnapshotActionsBySnapshotType;
            internal static List<QueryFunctionWithEntity<WorldType>>[] OnRestoreEntityFromSnapshotActionsBySnapshotType;

            [MethodImpl(AggressiveInlining)]
            internal static void Create() {
                var snapshotActionTypeLength = 2;

                SnapshotDataSerializersBySnapshotType = new Dictionary<Guid, (CustomSnapshotDataWriter, CustomSnapshotDataReader, ushort)>[snapshotActionTypeLength];
                SnapshotDataEntitySerializersBySnapshotType =
                    new Dictionary<Guid, (CustomSnapshotEntityDataWriter<WorldType>, CustomSnapshotEntityDataReader<WorldType>, ushort)>[snapshotActionTypeLength];
                PreCreateSnapshotCallbacksBySnapshotType = new List<Action>[snapshotActionTypeLength];
                PreLoadSnapshotCallbacksBySnapshotType = new List<Action>[snapshotActionTypeLength];
                PostCreateSnapshotCallbacksBySnapshotType = new List<Action>[snapshotActionTypeLength];
                PostLoadSnapshotCallbacksTypeBySnapshotType = new List<Action>[snapshotActionTypeLength];
                OnCreateEntitySnapshotActionsBySnapshotType = new List<QueryFunctionWithEntity<WorldType>>[snapshotActionTypeLength];
                OnRestoreEntityFromSnapshotActionsBySnapshotType = new List<QueryFunctionWithEntity<WorldType>>[snapshotActionTypeLength];

                for (var i = 0; i < snapshotActionTypeLength; i++) {
                    PreCreateSnapshotCallbacksBySnapshotType[i] = new();
                    PreLoadSnapshotCallbacksBySnapshotType[i] = new();
                    SnapshotDataSerializersBySnapshotType[i] = new();
                    SnapshotDataEntitySerializersBySnapshotType[i] = new();
                    PostCreateSnapshotCallbacksBySnapshotType[i] = new();
                    PostLoadSnapshotCallbacksTypeBySnapshotType[i] = new();
                    OnCreateEntitySnapshotActionsBySnapshotType[i] = new();
                    OnRestoreEntityFromSnapshotActionsBySnapshotType[i] = new();
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal static void Destroy() {
                SnapshotDataSerializersBySnapshotType = default;
                PostCreateSnapshotCallbacksBySnapshotType = default;
                PostLoadSnapshotCallbacksTypeBySnapshotType = default;
                OnCreateEntitySnapshotActionsBySnapshotType = default;
                OnRestoreEntityFromSnapshotActionsBySnapshotType = default;
                SnapshotDataEntitySerializersBySnapshotType = default;
                PreCreateSnapshotCallbacksBySnapshotType = default;
                PreLoadSnapshotCallbacksBySnapshotType = default;
            }

            [MethodImpl(AggressiveInlining)]
            public static void RegisterPreCreateSnapshotCallback(Action action, SnapshotActionType snapshotType = SnapshotActionType.All) {
                if (Status == WorldStatus.NotCreated) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: RegisterPreCreateSnapshotCallback, World not created");
                }

                if (snapshotType == SnapshotActionType.All) {
                    PreCreateSnapshotCallbacksBySnapshotType[(int) SnapshotActionType.Entities].Add(action);
                    PreCreateSnapshotCallbacksBySnapshotType[(int) SnapshotActionType.World].Add(action);
                } else {
                    PreCreateSnapshotCallbacksBySnapshotType[(int) snapshotType].Add(action);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void RegisterPostCreateSnapshotCallback(Action action, SnapshotActionType snapshotType = SnapshotActionType.All) {
                if (Status == WorldStatus.NotCreated) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: RegisterPostCreateSnapshotCallback, World not created");
                }

                if (snapshotType == SnapshotActionType.All) {
                    PostCreateSnapshotCallbacksBySnapshotType[(int) SnapshotActionType.Entities].Add(action);
                    PostCreateSnapshotCallbacksBySnapshotType[(int) SnapshotActionType.World].Add(action);
                } else {
                    PostCreateSnapshotCallbacksBySnapshotType[(int) snapshotType].Add(action);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void RegisterPreLoadSnapshotCallback(Action action, SnapshotActionType snapshotType = SnapshotActionType.All) {
                if (Status == WorldStatus.NotCreated) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: RegisterPreLoadSnapshotCallback, World not created");
                }

                if (snapshotType == SnapshotActionType.All) {
                    PreLoadSnapshotCallbacksBySnapshotType[(int) SnapshotActionType.Entities].Add(action);
                    PreLoadSnapshotCallbacksBySnapshotType[(int) SnapshotActionType.World].Add(action);
                } else {
                    PreLoadSnapshotCallbacksBySnapshotType[(int) snapshotType].Add(action);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void RegisterPostLoadSnapshotCallback(Action action, SnapshotActionType snapshotType = SnapshotActionType.All) {
                if (Status == WorldStatus.NotCreated) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: RegisterPostLoadSnapshotCallback, World not created");
                }

                if (snapshotType == SnapshotActionType.All) {
                    PostLoadSnapshotCallbacksTypeBySnapshotType[(int) SnapshotActionType.Entities].Add(action);
                    PostLoadSnapshotCallbacksTypeBySnapshotType[(int) SnapshotActionType.World].Add(action);
                } else {
                    PostLoadSnapshotCallbacksTypeBySnapshotType[(int) snapshotType].Add(action);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void RegisterPostCreateSnapshotEachEntityCallback(QueryFunctionWithEntity<WorldType> action, SnapshotActionType snapshotType = SnapshotActionType.All) {
                if (Status == WorldStatus.NotCreated) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: RegisterPostCreateSnapshotEachEntityCallback, World not created");
                }

                if (snapshotType == SnapshotActionType.All) {
                    OnCreateEntitySnapshotActionsBySnapshotType[(int) SnapshotActionType.Entities].Add(action);
                    OnCreateEntitySnapshotActionsBySnapshotType[(int) SnapshotActionType.World].Add(action);
                } else {
                    OnCreateEntitySnapshotActionsBySnapshotType[(int) snapshotType].Add(action);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void RegisterPostLoadSnapshotEachEntityCallback(QueryFunctionWithEntity<WorldType> action, SnapshotActionType snapshotType = SnapshotActionType.All) {
                if (Status == WorldStatus.NotCreated) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: OnRestoreFromSnapshot, World not created");
                }

                if (snapshotType == SnapshotActionType.All) {
                    OnRestoreEntityFromSnapshotActionsBySnapshotType[(int) SnapshotActionType.Entities].Add(action);
                    OnRestoreEntityFromSnapshotActionsBySnapshotType[(int) SnapshotActionType.World].Add(action);
                } else {
                    OnRestoreEntityFromSnapshotActionsBySnapshotType[(int) snapshotType].Add(action);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void SetSnapshotHandler(
                Guid guid, ushort version, CustomSnapshotDataWriter writer, CustomSnapshotDataReader reader, SnapshotActionType snapshotType = SnapshotActionType.All
            ) {
                if (Status == WorldStatus.NotCreated) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: SetCustomSnapshotDataSerializer, World not created");
                }

                if (snapshotType == SnapshotActionType.All) {
                    SnapshotDataSerializersBySnapshotType[(int) SnapshotActionType.Entities][guid] = (writer, reader, version);
                    SnapshotDataSerializersBySnapshotType[(int) SnapshotActionType.World][guid] = (writer, reader, version);
                } else {
                    SnapshotDataSerializersBySnapshotType[(int) snapshotType][guid] = (writer, reader, version);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void SetSnapshotHandlerEachEntity(
                Guid guid, ushort version, CustomSnapshotEntityDataWriter<WorldType> writer, CustomSnapshotEntityDataReader<WorldType> reader, SnapshotActionType snapshotType = SnapshotActionType.All
            ) {
                if (Status == WorldStatus.NotCreated) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: SetCustomSnapshotEntityDataSerializer, World not created");
                }

                if (snapshotType == SnapshotActionType.All) {
                    SnapshotDataEntitySerializersBySnapshotType[(int) SnapshotActionType.Entities][guid] = (writer, reader, version);
                    SnapshotDataEntitySerializersBySnapshotType[(int) SnapshotActionType.World][guid] = (writer, reader, version);
                } else {
                    SnapshotDataEntitySerializersBySnapshotType[(int) snapshotType][guid] = (writer, reader, version);
                }
            }


            [MethodImpl(AggressiveInlining)]
            public static void WriteWorldSnapshotData(ref BinaryPackWriter writer) {
                WriteSnapshotData(ref writer, SnapshotActionType.World);
            }

            [MethodImpl(AggressiveInlining)]
            public static void ReadWorldSnapshotData(ref BinaryPackReader reader) {
                ReadSnapshotData(ref reader, SnapshotActionType.World);
            }

            [MethodImpl(AggressiveInlining)]
            public static void WriteEntitiesSnapshotData(ref BinaryPackWriter writer) {
                WriteSnapshotData(ref writer, SnapshotActionType.Entities);
            }

            [MethodImpl(AggressiveInlining)]
            public static void ReadEntitiesSnapshotData(ref BinaryPackReader reader) {
                ReadSnapshotData(ref reader, SnapshotActionType.Entities);
            }

            [MethodImpl(AggressiveInlining)]
            public static byte[] CreateWorldSnapshot(bool withCustomSnapshotData = true, bool gzip = false) {
                var writer = BinaryPackWriter.CreateFromPool(1024 * 16);
                Write(ref writer, withCustomSnapshotData);
                var result = writer.CopyToBytes(gzip);
                writer.Dispose();
                return result;
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateWorldSnapshot(ref byte[] result, bool withCustomSnapshotData = true, bool gzip = false) {
                var writer = BinaryPackWriter.CreateFromPool(1024 * 16);
                Write(ref writer, withCustomSnapshotData);
                writer.CopyToBytes(ref result, gzip);
                writer.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateWorldSnapshot(string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false) {
                var writer = BinaryPackWriter.CreateFromPool(1024 * 16);
                CreateWorldSnapshot(ref writer, filePath, withCustomSnapshotData, gzip, flushToDisk);
                writer.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateWorldSnapshot(ref BinaryPackWriter writer, bool withCustomSnapshotData = true) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: WriteToBytes, World not initialized");
                #endif
                Write(ref writer, withCustomSnapshotData);
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateWorldSnapshot(ref BinaryPackWriter writer, string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: WriteToFile, World not initialized");
                #endif
                Write(ref writer, withCustomSnapshotData);
                writer.FlushToFile(filePath, gzip, flushToDisk);
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadWorldSnapshot(BinaryPackReader reader, OnLoadWorldAction loadWorldAction = OnLoadWorldAction.DestroyAllEntitiesAndClearWorld) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: ReadFromBytes, World not initialized");
                #endif
                Read(ref reader, loadWorldAction);
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadWorldSnapshot(byte[] snapshot, bool gzip = false, OnLoadWorldAction loadWorldAction = OnLoadWorldAction.DestroyAllEntitiesAndClearWorld) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: ReadFromBytes, World not initialized");
                #endif
                if (gzip) {
                    var writer = BinaryPackWriter.CreateFromPool((uint) (snapshot.Length * 2));
                    writer.WriteGzipData(snapshot);
                    var reader = writer.AsReader();
                    Read(ref reader, loadWorldAction);
                    writer.Dispose();
                } else {
                    var reader = new BinaryPackReader(snapshot, (uint) snapshot.Length, 0);
                    Read(ref reader, loadWorldAction);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadWorldSnapshot(string worldSnapshotFilePath, bool gzip = false, uint byteSizeHint = 16384, OnLoadWorldAction loadWorldAction = OnLoadWorldAction.DestroyAllEntitiesAndClearWorld) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: ReadFromFile, World not initialized");
                #endif
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                writer.WriteFromFile(worldSnapshotFilePath, gzip);
                var reader = writer.AsReader();
                Read(ref reader, loadWorldAction);
                writer.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            internal static void WriteSnapshotData(ref BinaryPackWriter writer, SnapshotActionType type) {
                var serializers = SnapshotDataSerializersBySnapshotType[(int) type];
                writer.WriteInt(serializers.Count);
                foreach (var (key, (snapshotDataWriter, _, version)) in serializers) {
                    writer.WriteGuid(key);
                    writer.WriteUshort(version);
                    var point = writer.MakePoint(sizeof(uint));
                    snapshotDataWriter(ref writer);
                    writer.WriteUintAt(point, writer.Position - (point + sizeof(uint)));
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal static void ReadSnapshotData(ref BinaryPackReader reader, SnapshotActionType type) {
                var count = reader.ReadInt();
                var serializers = SnapshotDataSerializersBySnapshotType[(int) type];

                for (var i = 0; i < count; i++) {
                    var key = reader.ReadGuid();
                    var version = reader.ReadUshort();
                    var byteSize = reader.ReadUint();
                    if (serializers.TryGetValue(key, out var val)) {
                        val.reader(ref reader, version);
                    } else {
                        reader.SkipNext(byteSize);
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal static void WriteEntitySnapshotData(ref BinaryPackWriter writer, SnapshotActionType type) {
                var serializers = SnapshotDataEntitySerializersBySnapshotType[(int) type];
                writer.WriteInt(serializers.Count);
                foreach (var (key, (snapshotDataEntityWriter, _, version)) in serializers) {
                    writer.WriteGuid(key);
                    writer.WriteUshort(version);
                    var point = writer.MakePoint(sizeof(uint));
                    QueryEntities.For(EntityStatusType.Any).WriteEntitySnapshotData(ref writer, snapshotDataEntityWriter);
                    writer.WriteUintAt(point, writer.Position - (point + sizeof(uint)));
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal static void ReadEntitySnapshotData(ref BinaryPackReader reader, SnapshotActionType type) {
                var count = reader.ReadInt();
                var serializers = SnapshotDataEntitySerializersBySnapshotType[(int) type];

                for (var i = 0; i < count; i++) {
                    var key = reader.ReadGuid();
                    var version = reader.ReadUshort();
                    var byteSize = reader.ReadUint();
                    if (serializers.TryGetValue(key, out var val)) {
                        QueryEntities.For(EntityStatusType.Any).ReadEntitySnapshotData(ref reader, val.reader, version);
                    } else {
                        reader.SkipNext(byteSize);
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            private static void Write(ref BinaryPackWriter writer, bool withCustomSnapshotData) {
                var actions = PreCreateSnapshotCallbacksBySnapshotType[(int) SnapshotActionType.World];
                for (var i = 0; i < actions.Count; i++) {
                    actions[i]();
                }
                
                Entities.Value.Write(ref writer);
                ModuleComponents.Serializer.Value.Write(ref writer);
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Serializer.Value.Write(ref writer);
                #endif
                #if !FFS_ECS_DISABLE_EVENTS
                Events.Serializer.Value.Write(ref writer);
                #endif

                if (withCustomSnapshotData) {
                    WriteWorldSnapshotData(ref writer);
                    WriteEntitySnapshotData(ref writer, SnapshotActionType.World);
                } else {
                    writer.WriteInt(0);
                    writer.WriteInt(0);
                }

                actions = PostCreateSnapshotCallbacksBySnapshotType[(int) SnapshotActionType.World];
                for (var i = 0; i < actions.Count; i++) {
                    actions[i]();
                }

                if (OnCreateEntitySnapshotActionsBySnapshotType[(int) SnapshotActionType.World].Count > 0) {
                    Query.For(entity => {
                        var entActions = OnCreateEntitySnapshotActionsBySnapshotType[(int) SnapshotActionType.World];
                        for (var j = 0; j < entActions.Count; j++) {
                            entActions[j](entity);
                        }
                    }, EntityStatusType.Any);
                }
            }

            [MethodImpl(AggressiveInlining)]
            private static void Read(ref BinaryPackReader reader, OnLoadWorldAction loadWorldAction) {
                if (loadWorldAction == OnLoadWorldAction.DestroyAllEntitiesAndClearWorld) {
                    DestroyAllEntities();
                    #if !FFS_ECS_DISABLE_EVENTS
                    Events.Clear();
                    #endif
                } else if (loadWorldAction == OnLoadWorldAction.ClearWorld) {
                    Clear();
                }
                
                var actions = PreLoadSnapshotCallbacksBySnapshotType[(int) SnapshotActionType.World];
                for (var i = 0; i < actions.Count; i++) {
                    actions[i]();
                }

                Entities.Value.Read(ref reader);
                ModuleComponents.Serializer.Value.Read(ref reader);
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Serializer.Value.Read(ref reader);
                #endif
                #if !FFS_ECS_DISABLE_EVENTS
                Events.Serializer.Value.Read(ref reader);
                #endif

                ReadWorldSnapshotData(ref reader);
                ReadEntitySnapshotData(ref reader, SnapshotActionType.World);

                actions = PostLoadSnapshotCallbacksTypeBySnapshotType[(int) SnapshotActionType.World];
                for (var i = 0; i < actions.Count; i++) {
                    actions[i]();
                }

                if (OnRestoreEntityFromSnapshotActionsBySnapshotType[(int) SnapshotActionType.World].Count > 0) {
                    Query.For(entity => {
                        var entActions = OnRestoreEntityFromSnapshotActionsBySnapshotType[(int) SnapshotActionType.World];
                        for (var j = 0; j < entActions.Count; j++) {
                            entActions[j](entity);
                        }
                    }, EntityStatusType.Any);
                }
            }
        }
    }
    
    public enum OnLoadWorldAction {
        DestroyAllEntitiesAndClearWorld,
        ClearWorld
    }
}