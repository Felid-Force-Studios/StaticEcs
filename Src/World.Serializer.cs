#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public delegate void SnapshotOnWriteAction(SnapshotWriteParams snapshotParams);
    
    public delegate void SnapshotOnReadAction(SnapshotReadParams snapshotParams);
    
    public delegate void SnapshotOnWriteEntityAction<WorldType>(World<WorldType>.Entity entity, SnapshotWriteParams snapshotParams)
        where WorldType : struct, IWorldType;

    public delegate void SnapshotOnReadEntityAction<WorldType>(World<WorldType>.Entity entity, SnapshotReadParams snapshotParams)
        where WorldType : struct, IWorldType;
    
    public delegate void CustomSnapshotDataReader(ref BinaryPackReader reader, ushort version, SnapshotReadParams snapshotParams);

    public delegate void CustomSnapshotDataWriter(ref BinaryPackWriter writer, SnapshotWriteParams snapshotParams);

    public delegate void CustomSnapshotEntityDataReader<WorldType>(ref BinaryPackReader reader, World<WorldType>.Entity entity, ushort version, SnapshotReadParams snapshotParams) where WorldType : struct, IWorldType;

    public delegate void CustomSnapshotEntityDataWriter<WorldType>(ref BinaryPackWriter writer, World<WorldType>.Entity entity, SnapshotWriteParams snapshotParams) where WorldType : struct, IWorldType;

    public enum SnapshotType: byte {
        GIDStore,
        Entities,
        World,
        Cluster,
        Chunk
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct SnapshotWriteParams {
        public readonly SnapshotType Type;

        [MethodImpl(AggressiveInlining)]
        public SnapshotWriteParams(SnapshotType type) {
            Type = type;
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct SnapshotReadParams {
        public readonly SnapshotType Type;
        public readonly bool EntitiesAsNew;

        [MethodImpl(AggressiveInlining)]
        public SnapshotReadParams(SnapshotType type, bool entitiesAsNew) {
            Type = type;
            EntitiesAsNew = entitiesAsNew;
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct EntitiesAsNewParams {
        public readonly ushort ClusterId;
        public readonly bool EntitiesAsNew;

        [MethodImpl(AggressiveInlining)]
        public EntitiesAsNewParams(bool entitiesAsNew, ushort clusterId = 0) {
            ClusterId = clusterId;
            EntitiesAsNew = entitiesAsNew;
        }
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
            internal static Dictionary<Guid, (CustomSnapshotDataWriter writer, CustomSnapshotDataReader reader, ushort version)> SnapshotDataSerializers;
            internal static Dictionary<Guid, (CustomSnapshotEntityDataWriter<WorldType> writer, CustomSnapshotEntityDataReader<WorldType> reader, ushort version)> SnapshotDataEntitySerializers;

            internal static List<SnapshotOnWriteAction> PreCreateSnapshotCallbacks;
            internal static List<SnapshotOnReadAction> PreLoadSnapshotCallbacks;
            internal static List<SnapshotOnWriteAction> PostCreateSnapshotCallbacks;
            internal static List<SnapshotOnReadAction> PostLoadSnapshotCallbacksType;
            internal static List<SnapshotOnWriteEntityAction<WorldType>> OnCreateEntitySnapshotActions;
            internal static List<SnapshotOnReadEntityAction<WorldType>> OnRestoreEntityFromSnapshotActions;

            [MethodImpl(AggressiveInlining)]
            internal static void Create() {
                SnapshotDataSerializers = new Dictionary<Guid, (CustomSnapshotDataWriter, CustomSnapshotDataReader, ushort)>();
                SnapshotDataEntitySerializers = new Dictionary<Guid, (CustomSnapshotEntityDataWriter<WorldType>, CustomSnapshotEntityDataReader<WorldType>, ushort)>();
                PreCreateSnapshotCallbacks = new List<SnapshotOnWriteAction>(16);
                PreLoadSnapshotCallbacks = new List<SnapshotOnReadAction>(16);
                PostCreateSnapshotCallbacks = new List<SnapshotOnWriteAction>(16);
                PostLoadSnapshotCallbacksType = new List<SnapshotOnReadAction>(16);
                OnCreateEntitySnapshotActions = new List<SnapshotOnWriteEntityAction<WorldType>>(16);
                OnRestoreEntityFromSnapshotActions = new List<SnapshotOnReadEntityAction<WorldType>>(16);
            }

            [MethodImpl(AggressiveInlining)]
            internal static void Destroy() {
                SnapshotDataSerializers = default;
                PostCreateSnapshotCallbacks = default;
                PostLoadSnapshotCallbacksType = default;
                OnCreateEntitySnapshotActions = default;
                OnRestoreEntityFromSnapshotActions = default;
                SnapshotDataEntitySerializers = default;
                PreCreateSnapshotCallbacks = default;
                PreLoadSnapshotCallbacks = default;
            }

            [MethodImpl(AggressiveInlining)]
            public static void RegisterPreCreateSnapshotCallback(SnapshotOnWriteAction action) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                PreCreateSnapshotCallbacks.Add(action);
            }

            [MethodImpl(AggressiveInlining)]
            public static void RegisterPostCreateSnapshotCallback(SnapshotOnWriteAction action) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                PostCreateSnapshotCallbacks.Add(action);
            }

            [MethodImpl(AggressiveInlining)]
            public static void RegisterPreLoadSnapshotCallback(SnapshotOnReadAction action) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                PreLoadSnapshotCallbacks.Add(action);
            }

            [MethodImpl(AggressiveInlining)]
            public static void RegisterPostLoadSnapshotCallback(SnapshotOnReadAction action) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                PostLoadSnapshotCallbacksType.Add(action);
            }

            [MethodImpl(AggressiveInlining)]
            public static void RegisterPostCreateSnapshotEachEntityCallback(SnapshotOnWriteEntityAction<WorldType> action) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                OnCreateEntitySnapshotActions.Add(action);
            }

            [MethodImpl(AggressiveInlining)]
            public static void RegisterPostLoadSnapshotEachEntityCallback(SnapshotOnReadEntityAction<WorldType> action) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                OnRestoreEntityFromSnapshotActions.Add(action);
            }

            [MethodImpl(AggressiveInlining)]
            public static void SetSnapshotHandler(Guid guid, ushort version, CustomSnapshotDataWriter writer, CustomSnapshotDataReader reader) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                SnapshotDataSerializers[guid] = (writer, reader, version);
            }

            [MethodImpl(AggressiveInlining)]
            public static void SetSnapshotHandlerEachEntity(Guid guid, ushort version, CustomSnapshotEntityDataWriter<WorldType> writer, CustomSnapshotEntityDataReader<WorldType> reader) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                SnapshotDataEntitySerializers[guid] = (writer, reader, version);
            }

            [MethodImpl(AggressiveInlining)]
            public static void WriteSnapshotData(ref BinaryPackWriter writer, SnapshotWriteParams snapshotParams) {
                writer.WriteInt(SnapshotDataSerializers.Count);
                foreach (var (key, (snapshotDataWriter, _, version)) in SnapshotDataSerializers) {
                    writer.WriteGuid(key);
                    writer.WriteUshort(version);
                    var point = writer.MakePoint(sizeof(uint));
                    snapshotDataWriter(ref writer, snapshotParams);
                    writer.WriteUintAt(point, writer.Position - (point + sizeof(uint)));
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void ReadSnapshotData(ref BinaryPackReader reader, SnapshotReadParams snapshotParams) {
                var count = reader.ReadInt();

                for (var i = 0; i < count; i++) {
                    var key = reader.ReadGuid();
                    var version = reader.ReadUshort();
                    var byteSize = reader.ReadUint();
                    if (SnapshotDataSerializers.TryGetValue(key, out var val)) {
                        val.reader(ref reader, version, snapshotParams);
                    } else {
                        reader.SkipNext(byteSize);
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void WriteEntitySnapshotData(ref BinaryPackWriter writer, SnapshotWriteParams snapshotParams, ReadOnlySpan<uint> chunks) {
                writer.WriteInt(SnapshotDataEntitySerializers.Count);
                foreach (var (key, (snapshotDataEntityWriter, _, version)) in SnapshotDataEntitySerializers) {
                    writer.WriteGuid(key);
                    writer.WriteUshort(version);
                    var point = writer.MakePoint(sizeof(uint));
                    Query.Entities(chunks, EntityStatusType.Any).WriteEntitySnapshotData(ref writer, snapshotDataEntityWriter, snapshotParams);
                    writer.WriteUintAt(point, writer.Position - (point + sizeof(uint)));
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void ReadEntitySnapshotData(ref BinaryPackReader reader, SnapshotReadParams snapshotParams, ReadOnlySpan<uint> chunks) {
                var count = reader.ReadInt();

                for (var i = 0; i < count; i++) {
                    var key = reader.ReadGuid();
                    var version = reader.ReadUshort();
                    var byteSize = reader.ReadUint();
                    if (SnapshotDataEntitySerializers.TryGetValue(key, out var val)) {
                        Query.Entities(chunks, EntityStatusType.Any).ReadEntitySnapshotData(ref reader, val.reader, version, snapshotParams);
                    } else {
                        reader.SkipNext(byteSize);
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static byte[] CreateWorldSnapshot(bool withCustomSnapshotData = true, bool gzip = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, ReadOnlySpan<ushort> clusters = default, bool writeEvents = true) {
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                WriteWorld(ref writer, withCustomSnapshotData, strategy, clusters, writeEvents);
                var result = writer.CopyToBytes(gzip);
                writer.Dispose();
                return result;
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateWorldSnapshot(ref byte[] result, bool withCustomSnapshotData = true, bool gzip = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, ReadOnlySpan<ushort> clusters = default, bool writeEvents = true) {
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                WriteWorld(ref writer, withCustomSnapshotData, strategy, clusters, writeEvents);
                writer.CopyToBytes(ref result, gzip);
                writer.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateWorldSnapshot(string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false) {
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                CreateWorldSnapshot(ref writer, filePath, withCustomSnapshotData, gzip, flushToDisk);
                writer.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateWorldSnapshot(ref BinaryPackWriter writer, bool withCustomSnapshotData = true, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, ReadOnlySpan<ushort> clusters = default, bool writeEvents = true) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                WriteWorld(ref writer, withCustomSnapshotData, strategy, clusters, writeEvents);
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateWorldSnapshot(ref BinaryPackWriter writer, string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, ReadOnlySpan<ushort> clusters = default, bool writeEvents = true) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                WriteWorld(ref writer, withCustomSnapshotData, strategy, clusters, writeEvents);
                writer.FlushToFile(filePath, gzip, flushToDisk);
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadWorldSnapshot(BinaryPackReader reader) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                ReadWorld(ref reader);
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadWorldSnapshot(byte[] snapshot, bool gzip = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                if (gzip) {
                    var writer = BinaryPackWriter.CreateFromPool((uint) (snapshot.Length * 2));
                    writer.WriteGzipData(snapshot);
                    var reader = writer.AsReader();
                    ReadWorld(ref reader);
                    writer.Dispose();
                } else {
                    var reader = new BinaryPackReader(snapshot, (uint) snapshot.Length, 0);
                    ReadWorld(ref reader);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadWorldSnapshot(string worldSnapshotFilePath, bool gzip = false, uint byteSizeHint = 0) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                CalculateByteSizeHint(ref byteSizeHint);
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                writer.WriteFromFile(worldSnapshotFilePath, gzip);
                var reader = writer.AsReader();
                ReadWorld(ref reader);
                writer.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            public static byte[] CreateClusterSnapshot(ushort clusterId, bool withCustomSnapshotData = true, bool gzip = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, bool withEntitiesData = false) {
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                WriteCluster(ref writer, withCustomSnapshotData, strategy, clusterId, withEntitiesData);
                var result = writer.CopyToBytes(gzip);
                writer.Dispose();
                return result;
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateClusterSnapshot(ushort clusterId, ref byte[] result, bool withCustomSnapshotData = true, bool gzip = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, bool withEntitiesData = false) {
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                WriteCluster(ref writer, withCustomSnapshotData, strategy, clusterId, withEntitiesData);
                writer.CopyToBytes(ref result, gzip);
                writer.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateClusterSnapshot(ushort clusterId, string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, bool withEntitiesData = false) {
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                CreateClusterSnapshot(clusterId, ref writer, filePath, withCustomSnapshotData, gzip, flushToDisk, strategy, withEntitiesData);
                writer.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateClusterSnapshot(ushort clusterId, ref BinaryPackWriter writer, bool withCustomSnapshotData = true, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, bool withEntitiesData = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                WriteCluster(ref writer, withCustomSnapshotData, strategy, clusterId, withEntitiesData);
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateClusterSnapshot(ushort clusterId, ref BinaryPackWriter writer, string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, bool withEntitiesData = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                WriteCluster(ref writer, withCustomSnapshotData, strategy, clusterId, withEntitiesData);
                writer.FlushToFile(filePath, gzip, flushToDisk);
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadClusterSnapshot(BinaryPackReader reader, EntitiesAsNewParams entitiesAsNew = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                ReadCluster(ref reader, entitiesAsNew);
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadClusterSnapshot(byte[] snapshot, bool gzip = false, EntitiesAsNewParams entitiesAsNew = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                if (gzip) {
                    var writer = BinaryPackWriter.CreateFromPool((uint) (snapshot.Length * 2));
                    writer.WriteGzipData(snapshot);
                    var reader = writer.AsReader();
                    ReadCluster(ref reader, entitiesAsNew);
                    writer.Dispose();
                } else {
                    var reader = new BinaryPackReader(snapshot, (uint) snapshot.Length, 0);
                    ReadCluster(ref reader, entitiesAsNew);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadClusterSnapshot(string worldSnapshotFilePath, bool gzip = false, uint byteSizeHint = 0, EntitiesAsNewParams entitiesAsNew = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                CalculateByteSizeHint(ref byteSizeHint);
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                writer.WriteFromFile(worldSnapshotFilePath, gzip);
                var reader = writer.AsReader();
                ReadCluster(ref reader, entitiesAsNew);
                writer.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            public static byte[] CreateChunkSnapshot(uint chunkIdx, bool withCustomSnapshotData = true, bool gzip = false, bool withEntitiesData = false) {
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                WriteChunk(ref writer, withCustomSnapshotData, chunkIdx, withEntitiesData);
                var result = writer.CopyToBytes(gzip);
                writer.Dispose();
                return result;
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateChunkSnapshot(uint chunkIdx, ref byte[] result, bool withCustomSnapshotData = true, bool gzip = false, bool withEntitiesData = false) {
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                WriteChunk(ref writer, withCustomSnapshotData, chunkIdx, withEntitiesData);
                writer.CopyToBytes(ref result, gzip);
                writer.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateChunkSnapshot(uint chunkIdx, string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false, bool withEntitiesData = false) {
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                CreateChunkSnapshot(chunkIdx, ref writer, filePath, withCustomSnapshotData, gzip, flushToDisk, withEntitiesData);
                writer.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateChunkSnapshot(uint chunkIdx, ref BinaryPackWriter writer, bool withCustomSnapshotData = true, bool withEntitiesData = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                WriteChunk(ref writer, withCustomSnapshotData, chunkIdx, withEntitiesData);
            }

            [MethodImpl(AggressiveInlining)]
            public static void CreateChunkSnapshot(uint chunkIdx, ref BinaryPackWriter writer, string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false, bool withEntitiesData = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                WriteChunk(ref writer, withCustomSnapshotData, chunkIdx, withEntitiesData);
                writer.FlushToFile(filePath, gzip, flushToDisk);
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadChunkSnapshot(BinaryPackReader reader, EntitiesAsNewParams entitiesAsNew = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                ReadChunk(ref reader, entitiesAsNew);
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadChunkSnapshot(byte[] snapshot, bool gzip = false, EntitiesAsNewParams entitiesAsNew = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                if (gzip) {
                    var writer = BinaryPackWriter.CreateFromPool((uint) (snapshot.Length * 2));
                    writer.WriteGzipData(snapshot);
                    var reader = writer.AsReader();
                    ReadChunk(ref reader, entitiesAsNew);
                    writer.Dispose();
                } else {
                    var reader = new BinaryPackReader(snapshot, (uint) snapshot.Length, 0);
                    ReadChunk(ref reader, entitiesAsNew);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void LoadChunkSnapshot(string worldSnapshotFilePath, bool gzip = false, uint byteSizeHint = 0, EntitiesAsNewParams entitiesAsNew = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                CalculateByteSizeHint(ref byteSizeHint);
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                writer.WriteFromFile(worldSnapshotFilePath, gzip);
                var reader = writer.AsReader();
                ReadChunk(ref reader, entitiesAsNew);
                writer.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            private static void WriteWorld(ref BinaryPackWriter writer, bool withCustomSnapshotData, ChunkWritingStrategy strategy, ReadOnlySpan<ushort> clusters, bool writeEvents) {
                clusters = HandleClustersRange(clusters);

                var snapshotParams = new SnapshotWriteParams(SnapshotType.World);
                BeforeWrite(snapshotParams);
                
                var tempChunks = TempChunksData.Create();
                writer.WriteBool(writeEvents);
                Entities.Value.Write(ref writer, strategy, clusters, ref tempChunks, true);
                writer.WriteUint(tempChunks.ChunksCount);
                writer.WriteArrayUnmanaged(tempChunks.Chunks, 0, (int) tempChunks.ChunksCount);
                ModuleComponents.Serializer.Value.WriteWorld(ref writer, tempChunks);
                ModuleTags.Serializer.Value.WriteWorld(ref writer, tempChunks);
                if (writeEvents) {
                    Events.Serializer.Value.Write(ref writer);
                }
                
                var chunks = new ReadOnlySpan<uint>(tempChunks.Chunks, 0, (int) tempChunks.ChunksCount);

                WriteCustomSnapshotData(ref writer, withCustomSnapshotData, snapshotParams, chunks);
                AfterWrite(snapshotParams, chunks);
                tempChunks.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            private static void ReadWorld(ref BinaryPackReader reader) {
                var snapshotParams = new SnapshotReadParams(SnapshotType.World, false);
                BeforeRead(snapshotParams);

                var readEvents = reader.ReadBool();
                Entities.Value.Read(ref reader, true);
                var chunksCount = reader.ReadUint();
                var tempChunks = reader.ReadArrayUnmanagedPooled<uint>(out var h).Array;
                ModuleComponents.Serializer.Value.ReadWorld(ref reader);
                ModuleTags.Serializer.Value.ReadWorld(ref reader);
                if (readEvents) {
                    Events.Clear();
                    Events.Serializer.Value.Read(ref reader);
                }
                
                var chunks = new ReadOnlySpan<uint>(tempChunks, 0, (int) chunksCount);

                ReadCustomSnapshotData(ref reader, snapshotParams, chunks);
                AfterRead(snapshotParams, chunks);
                
                h.Return();
            }

            [MethodImpl(AggressiveInlining)]
            private static void WriteCluster(ref BinaryPackWriter writer, bool withCustomSnapshotData, ChunkWritingStrategy strategy, ushort clusterId, bool withEntitiesData) {
                var snapshotParams = new SnapshotWriteParams(SnapshotType.Cluster);
                BeforeWrite(snapshotParams);
                
                var tempChunks = TempChunksData.Create();
                Entities.Value.FillClusterChunks(strategy, clusterId, ref tempChunks);
                
                writer.WriteBool(withEntitiesData);
                writer.WriteUshort(clusterId);
                writer.WriteUshort(ModuleComponents.Value.poolsCount);
                writer.WriteUshort(ModuleTags.Value.poolsCount);
                writer.WriteUint(tempChunks.ChunksCount);
                writer.WriteArrayUnmanaged(tempChunks.Chunks, 0, (int) tempChunks.ChunksCount);
                for (uint i = 0; i < tempChunks.ChunksCount; i++) {
                    var chunkIdx = tempChunks.Chunks[i];
                    writer.WriteUint(chunkIdx);
                    if (withEntitiesData) {
                        Entities.Value.WriteChunk(ref writer, ref Entities.Value.chunks[chunkIdx], false);
                    }
                    ModuleComponents.Serializer.Value.WriteChunk(ref writer, chunkIdx);
                    ModuleTags.Serializer.Value.WriteChunk(ref writer, chunkIdx);
                }
                
                var chunks = new ReadOnlySpan<uint>(tempChunks.Chunks, 0, (int) tempChunks.ChunksCount);

                WriteCustomSnapshotData(ref writer, withCustomSnapshotData, snapshotParams, chunks);
                AfterWrite(snapshotParams, chunks);
                tempChunks.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            private static void ReadCluster(ref BinaryPackReader reader, EntitiesAsNewParams entitiesAsNewParams) {
                var snapshotParams = new SnapshotReadParams(SnapshotType.Cluster, entitiesAsNewParams.EntitiesAsNew);
                BeforeRead(snapshotParams);

                var withEntitiesData = reader.ReadBool();
                var clusterId = reader.ReadUshort();
                
                if (!withEntitiesData && entitiesAsNewParams.EntitiesAsNew) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>", "ReadChunk", $"Cluster {clusterId} does not have information about entities, use withEntitiesData = true when saving a cluster");
                }
                
                if (entitiesAsNewParams.EntitiesAsNew) {
                    if (Entities.Value.ClusterIsRegistered(clusterId)) {
                        throw new StaticEcsException($"World<{typeof(WorldType)}>", "ReadCluster", $"Cluster {clusterId} already registered");
                    }
                    
                    RegisterCluster(entitiesAsNewParams.ClusterId);
                } else if (!Entities.Value.ClusterIsRegistered(clusterId)) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>", "ReadCluster", $"Cluster {clusterId} is not registered");
                }
                
                var componentsPoolCount = reader.ReadUshort();
                var tagsPoolCount = reader.ReadUshort();
                var chunksCount = reader.ReadUint();
                var tempChunks = reader.ReadArrayUnmanagedPooled<uint>(out var h).Array!;
                for (var i = 0; i < chunksCount; i++) {
                    var chunkIdx = reader.ReadUint();
                    if (entitiesAsNewParams.EntitiesAsNew) {
                        chunkIdx = FindNextSelfFreeChunk().ChunkIdx;
                        tempChunks[i] = chunkIdx;
                        RegisterChunk(chunkIdx, ChunkOwnerType.Self, entitiesAsNewParams.ClusterId);
                        Entities.Value.ReadChunk(ref reader, ref Entities.Value.chunks[chunkIdx], false);
                    }
                    ModuleComponents.Serializer.Value.ReadChunk(ref reader, chunkIdx, componentsPoolCount);
                    ModuleTags.Serializer.Value.ReadChunk(ref reader, chunkIdx, tagsPoolCount);
                }
                
                Entities.Value.LoadCluster(clusterId);
                
                var chunks = new ReadOnlySpan<uint>(tempChunks, 0, (int) chunksCount);

                ReadCustomSnapshotData(ref reader, snapshotParams, chunks);
                AfterRead(snapshotParams, chunks);
                h.Return();
            }

            [MethodImpl(AggressiveInlining)]
            private static void WriteChunk(ref BinaryPackWriter writer, bool withCustomSnapshotData, uint chunkIdx, bool withEntitiesData) {
                if (!Entities.Value.ChunkIsRegistered(chunkIdx)) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>", "WriteChunk", $"Chunk {chunkIdx} is not registered");
                }
                
                var snapshotParams = new SnapshotWriteParams(SnapshotType.Chunk);
                BeforeWrite(snapshotParams);
                
                writer.WriteUint(chunkIdx);
                writer.WriteBool(withEntitiesData);

                if (withEntitiesData) {
                    Entities.Value.WriteChunk(ref writer, ref Entities.Value.chunks[chunkIdx], false);
                }
                
                writer.WriteUshort(ModuleComponents.Value.poolsCount);
                ModuleComponents.Serializer.Value.WriteChunk(ref writer, chunkIdx);
                writer.WriteUshort(ModuleTags.Value.poolsCount);
                ModuleTags.Serializer.Value.WriteChunk(ref writer, chunkIdx);
                
                ReadOnlySpan<uint> chunks = stackalloc uint[1] { chunkIdx };

                WriteCustomSnapshotData(ref writer, withCustomSnapshotData, snapshotParams, chunks);
                AfterWrite(snapshotParams, chunks);
            }

            [MethodImpl(AggressiveInlining)]
            private static void ReadChunk(ref BinaryPackReader reader, EntitiesAsNewParams entitiesAsNewParams) {
                var snapshotParams = new SnapshotReadParams(SnapshotType.Chunk, entitiesAsNewParams.EntitiesAsNew);
                BeforeRead(snapshotParams);

                var chunkIdx = reader.ReadUint();
                var withEntitiesData = reader.ReadBool();

                if (!withEntitiesData && entitiesAsNewParams.EntitiesAsNew) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>", "ReadChunk", $"Chunk {chunkIdx} does not have information about entities, use withEntitiesData = true when saving a chunk");
                }

                if (entitiesAsNewParams.EntitiesAsNew) {
                    if (!ClusterIsRegistered(entitiesAsNewParams.ClusterId)) {
                        RegisterCluster(entitiesAsNewParams.ClusterId);
                    }

                    chunkIdx = FindNextSelfFreeChunk().ChunkIdx;
                    RegisterChunk(chunkIdx, ChunkOwnerType.Self, entitiesAsNewParams.ClusterId);
                    Entities.Value.ReadChunk(ref reader, ref Entities.Value.chunks[chunkIdx], false);
                } else if (!Entities.Value.ChunkIsRegistered(chunkIdx)) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>", "ReadChunk", $"Chunk {chunkIdx} is not registered");
                }
                
                var componentsPoolCount = reader.ReadUshort();
                ModuleComponents.Serializer.Value.ReadChunk(ref reader, chunkIdx,  componentsPoolCount);
                var tagsPoolCount = reader.ReadUshort();
                ModuleTags.Serializer.Value.ReadChunk(ref reader, chunkIdx, tagsPoolCount);
                Entities.Value.LoadChunk(chunkIdx);
                
                ReadOnlySpan<uint> chunks = stackalloc uint[1] { chunkIdx };
                
                ReadCustomSnapshotData(ref reader, snapshotParams, chunks);
                AfterRead(snapshotParams, chunks);
            }

            [MethodImpl(AggressiveInlining)]
            private static void BeforeWrite(SnapshotWriteParams snapshotParams) {
                for (var i = 0; i < PreCreateSnapshotCallbacks.Count; i++) {
                    PreCreateSnapshotCallbacks[i](snapshotParams);
                }
            }

            [MethodImpl(AggressiveInlining)]
            private static void BeforeRead(SnapshotReadParams snapshotParams) {
                for (var i = 0; i < PreLoadSnapshotCallbacks.Count; i++) {
                    PreLoadSnapshotCallbacks[i](snapshotParams);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            private static void WriteCustomSnapshotData(ref BinaryPackWriter writer, bool withCustomSnapshotData, SnapshotWriteParams snapshotParams, ReadOnlySpan<uint> chunks) {
                if (withCustomSnapshotData) {
                    WriteSnapshotData(ref writer, snapshotParams);
                    WriteEntitySnapshotData(ref writer, snapshotParams, chunks);
                } else {
                    writer.WriteInt(0);
                    writer.WriteInt(0);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            private static void ReadCustomSnapshotData(ref BinaryPackReader reader, SnapshotReadParams snapshotParams, ReadOnlySpan<uint> chunks) {
                ReadSnapshotData(ref reader, snapshotParams);
                ReadEntitySnapshotData(ref reader, snapshotParams, chunks);
            }
            
            [MethodImpl(AggressiveInlining)]
            private static void AfterWrite(SnapshotWriteParams snapshotParams, ReadOnlySpan<uint> chunks) {
                for (var i = 0; i < PostCreateSnapshotCallbacks.Count; i++) {
                    PostCreateSnapshotCallbacks[i](snapshotParams);
                }

                if (OnCreateEntitySnapshotActions.Count > 0) {
                    Query.For(chunks, ref snapshotParams, static (ref SnapshotWriteParams p, Entity entity) => {
                        for (var j = 0; j < OnCreateEntitySnapshotActions.Count; j++) {
                            OnCreateEntitySnapshotActions[j](entity, p);
                        }
                    }, EntityStatusType.Any, QueryMode.Flexible);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            private static void AfterRead(SnapshotReadParams snapshotParams, ReadOnlySpan<uint> chunks) {
                for (var i = 0; i < PostLoadSnapshotCallbacksType.Count; i++) {
                    PostLoadSnapshotCallbacksType[i](snapshotParams);
                }
                
                if (OnRestoreEntityFromSnapshotActions.Count > 0) {
                    Query.For(chunks, ref snapshotParams, static (ref SnapshotReadParams p, Entity entity) => {
                        for (var j = 0; j < OnRestoreEntityFromSnapshotActions.Count; j++) {
                            OnRestoreEntityFromSnapshotActions[j](entity, p);
                        }
                    }, EntityStatusType.Any, QueryMode.Flexible);
                }
            }
        }
    }
}