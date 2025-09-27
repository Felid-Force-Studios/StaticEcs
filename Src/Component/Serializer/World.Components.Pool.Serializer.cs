using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    public delegate T EcsComponentMigrationReader<T, WorldType>(ref BinaryPackReader reader, World<WorldType>.Entity entity, byte version, bool disabled)
        where T : struct
        where WorldType : struct, IWorldType;
    
    public delegate void EcsComponentDeleteMigrationReader<WorldType>(ref BinaryPackReader reader, World<WorldType>.Entity entity, byte version, bool disabled)
        where WorldType : struct, IWorldType;

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public partial struct Components<T> where T : struct, IComponent {
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            [Il2CppEagerStaticClassConstruction]
            #endif
            public struct Serializer {
                internal static Serializer Value;
                
                private IPackArrayStrategy<T> _readWriteArrayStrategy;
                private EcsComponentMigrationReader<T, WorldType> _migrationReader;
                internal Guid guid;
                internal byte version;
                
                [MethodImpl(AggressiveInlining)]
                public void Create(IComponentConfig<T, WorldType> config) {
                    guid = config.Id();
                    version = config.Version();
                    if (guid != System.Guid.Empty) {
                        _readWriteArrayStrategy = config.ReadWriteStrategy();
                        _migrationReader = config.MigrationReader();
                        BinaryPack.RegisterWithCollections(config.Writer(), config.Reader(), _readWriteArrayStrategy);
                    }
                }

                [MethodImpl(AggressiveInlining)]
                public void Destroy() {
                    guid = default;
                    version = default;
                    _migrationReader = default;
                    _readWriteArrayStrategy = default;
                }

                [MethodImpl(AggressiveInlining)]
                [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
                internal void WriteAll(ref BinaryPackWriter writer, ref Components<T> pool) {
                    writer.WriteByte(version);
                    writer.WriteInt(pool.chunks.Length);
                    writer.WriteBool(pool._clearable);
                    writer.WriteInt(Entities.Value.nextActiveChunkIdx);
                    for (var i = 0; i < Entities.Value.nextActiveChunkIdx; i++) {
                        ref var chunk = ref pool.chunks[i];
                        writer.WriteUlong(chunk.notEmptyBlocks);
                        writer.WriteUlong(chunk.fullBlocks);
                        writer.WriteArrayUnmanaged(chunk.entities);
                        writer.WriteArrayUnmanaged(chunk.disabledEntities);
                    }
                    writer.WriteBool(_readWriteArrayStrategy.IsUnmanaged());
                    if (_readWriteArrayStrategy.IsUnmanaged()) {
                        var dataCount = Entities.Value.nextActiveChunkIdx * Const.DATA_BLOCKS_IN_CHUNK;
                        var offset = writer.MakePoint(sizeof(uint));
                        uint len = 0;
                        for (var idx = 0; idx < dataCount; idx++) {
                            if (pool.data[idx] != null) {
                                writer.WriteInt(idx);
                                len++;
                                _readWriteArrayStrategy.WriteArray(ref writer, pool.data[idx], 0, Const.DATA_BLOCK_SIZE);
                            }
                        }
                        writer.WriteUintAt(offset, len); 
                    } else {
                        #if NET6_0_OR_GREATER
                        ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
                        Span<T> data = default;
                        #else
                        var deBruijn = Utils.DeBruijn;
                        T[] data = null;
                        #endif
                        var dataIdx = uint.MaxValue;
                        for (var chunkIdx = 0; chunkIdx < Entities.Value.nextActiveChunkIdx; chunkIdx++) {
                            ref var chunk = ref pool.chunks[chunkIdx];
                            var chunkMask = chunk.notEmptyBlocks;

                            while (chunkMask > 0) {
                                var blockIdx = (uint) deBruijn[(int) (((chunkMask & (ulong) -(long) chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                                chunkMask &= chunkMask - 1;
                                var entitiesMask = chunk.entities[blockIdx];

                                if (entitiesMask > 0) {
                                    var globalBlockIdx = blockIdx + (chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                                    var blockEntity = (uint) (globalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                                    #if NET6_0_OR_GREATER
                                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                                    #else
                                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                                    #endif
                                    
                                    if (globalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                                        dataIdx = (uint) (globalBlockIdx >> Const.DATA_QUERY_SHIFT);
                                        #if NET6_0_OR_GREATER
                                        data = new(pool.data[dataIdx]);
                                        #else
                                        data = pool.data[dataIdx];
                                        #endif
                                    }
                                    
                                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                                    var end = Utils.ApproximateMSB(entitiesMask);
                                    var total = Utils.PopCnt(entitiesMask);
                                    if (total >= (end - idx) >> 1) {
                                        for (; idx < end; idx++) {
                                            if ((entitiesMask & (1UL << idx)) > 0) {
                                                writer.Write(in data[idx + dOffset]);
                                            }
                                        }
                                    } else {
                                        do {
                                            writer.Write(in data[idx + dOffset]);
                                            entitiesMask &= entitiesMask - 1UL;
                                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                                        } while (entitiesMask > 0);
                                    }
                                }
                            }
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void ReadAll(ref BinaryPackReader reader, ref Components<T> pool) {
                    var oldVersion = reader.ReadByte();
                    var chunkCapacity = reader.ReadInt();
                    var clearable = reader.ReadBool(); // TODO  
                    var chunkCount = reader.ReadInt();

                    pool.chunks ??= new ComponentsChunk[chunkCapacity];
                    if (pool.chunks.Length < chunkCapacity) {
                        Array.Resize(ref pool.chunks, chunkCapacity);
                    }
                    
                    for (uint i = 0; i < chunkCount; i++) {
                        ref var chunk = ref pool.chunks[i];
                        chunk.notEmptyBlocks =  reader.ReadUlong();
                        chunk.fullBlocks =  reader.ReadUlong();
                        reader.ReadArrayUnmanaged(ref chunk.entities);
                        reader.ReadArrayUnmanaged(ref chunk.disabledEntities);
                    }
                    
                    var dataCapacity = chunkCapacity * Const.DATA_BLOCKS_IN_CHUNK;
                    pool.data ??= new T[dataCapacity][];
                    pool.dataPool ??= new T[dataCapacity][];
                    if (pool.data.Length < dataCapacity) {
                        Array.Resize(ref pool.data, dataCapacity);
                        Array.Resize(ref pool.dataPool, dataCapacity);
                    }
                    var unmanaged = reader.ReadBool(); // TODO validate strategy
                    
                    if (unmanaged) {
                        var dataCount = reader.ReadUint();
                        
                        for (uint j = 0; j < dataCount; j++) {
                            var idx = reader.ReadInt();
                            ref var components = ref pool.data[idx];
                            if (components == null) {
                                var count = Interlocked.Decrement(ref pool.dataPoolCount);
                                if (count >= 0) {
                                    components = pool.dataPool[count];
                                } else {
                                    Interlocked.Increment(ref pool.dataPoolCount);    
                                    components = new T[Const.DATA_BLOCK_SIZE];
                                }
                            }
                            
                            if (version == oldVersion) {
                                _readWriteArrayStrategy.ReadArray(ref reader, ref components);
                            } else {
                                _ = reader.ReadNullFlag();
                                var count = reader.ReadInt();
                                var byteSize = reader.ReadUint();
                                var oneSize = byteSize / count; 
                                var dataEntity = (uint) (idx * Const.DATA_BLOCK_SIZE);
                                var entity = new Entity(dataEntity);
                                ref var eid = ref entity._id;
                                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                                ref var chunk = ref pool.chunks[chunkIdx];
                                for (var i = 0; i < count; i++) {
                                    eid = (uint) (dataEntity + i);
                                    var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                                    var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                                    if ((chunk.entities[blockIdx] & (1UL << blockEntityIdx)) != 0) {
                                        components[i] = _migrationReader(ref reader, entity, oldVersion, (chunk.disabledEntities[blockIdx] & (1UL << blockEntityIdx)) != 0);
                                    } else {
                                        reader.SkipNext((uint) oneSize);
                                    }
                                }
                            }
                        }
                    } else {
                        #if NET6_0_OR_GREATER
                        ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
                        Span<T> data = default;
                        #else
                        var deBruijn = Utils.DeBruijn;
                        T[] data = null;
                        #endif
                        var dataIdx = uint.MaxValue;
                        for (var chunkIdx = 0; chunkIdx < chunkCount; chunkIdx++) {
                            ref var chunk = ref pool.chunks[chunkIdx];
                            var chunkMask = chunk.notEmptyBlocks;

                            while (chunkMask > 0) {
                                var blockIdx = (uint) deBruijn[(int) (((chunkMask & (ulong) -(long) chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                                chunkMask &= chunkMask - 1;
                                var entitiesMask = chunk.entities[blockIdx];

                                if (entitiesMask > 0) {
                                    var globalBlockIdx = blockIdx + (chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                                    var blockEntity = (uint) (globalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                                    #if NET6_0_OR_GREATER
                                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                                    #else
                                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                                    #endif
                                    
                                    if (globalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                                        dataIdx = (uint) (globalBlockIdx >> Const.DATA_QUERY_SHIFT);
                                        pool.data[dataIdx] ??= new T[Const.DATA_BLOCK_SIZE];
                                        #if NET6_0_OR_GREATER
                                        data = new(pool.data[dataIdx]);
                                        #else
                                        data = pool.data[dataIdx];
                                        #endif
                                    }
                                    
                                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                                    var end = Utils.ApproximateMSB(entitiesMask);
                                    var total = Utils.PopCnt(entitiesMask);
                                    if (version == oldVersion) {
                                        if (total >= (end - idx) >> 1) {
                                            for (; idx < end; idx++) {
                                                if ((entitiesMask & (1UL << idx)) > 0) {
                                                    data[idx + dOffset] = reader.Read<T>();
                                                }
                                            }
                                        } else {
                                            do {
                                                data[idx + dOffset] = reader.Read<T>();
                                                entitiesMask &= entitiesMask - 1UL;
                                                idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                                            } while (entitiesMask > 0);
                                        }
                                    } else {
                                        var disabledMask = chunk.disabledEntities[blockIdx];
                                        if (total >= (end - idx) >> 1) {
                                            for (; idx < end; idx++) {
                                                if ((entitiesMask & (1UL << idx)) > 0) {
                                                    data[idx + dOffset] = _migrationReader(ref reader, new (blockEntity + idx), oldVersion, (disabledMask & (1UL << idx)) != 0);
                                                }
                                            }
                                        } else {
                                            do {
                                                data[idx + dOffset] = _migrationReader(ref reader, new (blockEntity + idx), oldVersion, (disabledMask & (1UL << idx)) != 0);
                                                entitiesMask &= entitiesMask - 1UL;
                                                idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                                            } while (entitiesMask > 0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void Write(ref BinaryPackWriter writer, ref Components<T> pool, Entity entity) {
                    var offset = writer.MakePoint(sizeof(short));
                    writer.WriteByte(version);
                    writer.Write(in pool.RefInternal(entity));
                    var size = writer.Position - (offset + sizeof(short));
                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                    if (size > short.MaxValue) throw new StaticEcsException($"Size of component {typeof(T)} more than {short.MaxValue} bytes");
                    #endif
                    var disabled = pool.HasDisabled(entity);
                    if (disabled) {
                        writer.WriteUshortAt(offset, (ushort) (size | ComponentSerializerUtils.disabledBit));
                    } else {
                        writer.WriteUshortAt(offset, (ushort) size);
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void Read(ref BinaryPackReader reader, ref Components<T> pool, Entity entity) {
                    var disabled = (reader.ReadUshort() & ComponentSerializerUtils.disabledBit) == ComponentSerializerUtils.disabledBit;
                    var oldVersion = reader.ReadByte();

                    pool.AddInternal(entity, version == oldVersion 
                                         ? reader.Read<T>() 
                                         : _migrationReader(ref reader, entity, oldVersion, disabled));

                    if (disabled) {
                        pool.Disable(entity);
                    }
                }
            }
        }
    }
    
    internal static class ComponentSerializerUtils {
        internal const int disabledBit = 0b_10000000_00000000;
        internal const int disabledBitInv = ~disabledBit;
        
        [MethodImpl(AggressiveInlining)]
        internal static void SkipOneComponent(this ref BinaryPackReader reader) {
            var size = reader.ReadUshort() & disabledBitInv;
            reader.SkipNext((uint) size);
        }
        
        [MethodImpl(AggressiveInlining)]
        internal static void DeleteOneComponentMigration<WorldType>(this ref BinaryPackReader reader, World<WorldType>.Entity entity, EcsComponentDeleteMigrationReader<WorldType> migration) 
            where WorldType : struct, IWorldType {
            var disabled = (reader.ReadUshort() & disabledBit) == disabledBit;
            var oldVersion = reader.ReadByte();
                    
            migration(ref reader, entity, oldVersion, disabled);
        }
        
        [MethodImpl(AggressiveInlining)]
        internal static void DeleteAllComponentMigration<WorldType>(this ref BinaryPackReader reader, EcsComponentDeleteMigrationReader<WorldType> migration) 
            where WorldType : struct, IWorldType {
            var oldVersion = reader.ReadByte();
            var chunkCapacity = reader.ReadInt();
            var clearable = reader.ReadBool();
            var chunkCount = reader.ReadInt();

            var chunks = ArrayPool<ComponentsChunk>.Shared.Rent(chunkCapacity);
            for (uint i = 0; i < chunkCount; i++) {
                ref var chunk = ref chunks[i];
                chunk.notEmptyBlocks = reader.ReadUlong();
                chunk.fullBlocks = reader.ReadUlong();
                chunk.entities = ArrayPool<ulong>.Shared.Rent(Const.BLOCK_IN_CHUNK);
                chunk.disabledEntities = ArrayPool<ulong>.Shared.Rent(Const.BLOCK_IN_CHUNK);
                reader.ReadArrayUnmanaged(ref chunk.entities);
                reader.ReadArrayUnmanaged(ref chunk.disabledEntities);
            }

            var unmanaged = reader.ReadBool(); // TODO validate strategy

            if (unmanaged) {
                var dataCount = reader.ReadUint();

                for (uint j = 0; j < dataCount; j++) {
                    var idx = reader.ReadInt();
                    _ = reader.ReadNullFlag();
                    var count = reader.ReadInt();
                    var byteSize = reader.ReadUint();
                    var oneSize = byteSize / count;
                    var dataEntity = (uint) (idx * Const.DATA_BLOCK_SIZE);
                    var entity = new World<WorldType>.Entity(dataEntity);
                    ref var eid = ref entity._id;
                    for (var i = 0; i < Const.DATA_BLOCK_SIZE; i++) {
                        eid = (uint) (dataEntity + i);
                        var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                        var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                        var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                        var has = (chunks[chunkIdx].entities[blockIdx] & (1UL << blockEntityIdx)) != 0;
                        if (has) {
                            var disabled = (chunks[chunkIdx].disabledEntities[blockIdx] & (1UL << blockEntityIdx)) != 0;
                            migration(ref reader, entity, oldVersion, disabled);
                        } else {
                            reader.SkipNext((uint) oneSize);
                        }
                    }
                }
            } else {
                #if NET6_0_OR_GREATER
                ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
                #else
                var deBruijn = Utils.DeBruijn;
                #endif
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity._id;
                for (var chunkIdx = 0; chunkIdx < chunkCount; chunkIdx++) {
                    ref var chunk = ref chunks[chunkIdx];
                    var chunkMask = chunk.notEmptyBlocks;

                    while (chunkMask > 0) {
                        var blockIdx = (uint) deBruijn[(int) (((chunkMask & (ulong) -(long) chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                        chunkMask &= chunkMask - 1;
                        var entitiesMask = chunk.entities[blockIdx];

                        if (entitiesMask > 0) {
                            var globalBlockIdx = blockIdx + (chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                            var blockEntity = (uint) (globalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT);

                            var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                            var end = Utils.ApproximateMSB(entitiesMask);
                            var total = Utils.PopCnt(entitiesMask);
                            var disabledMask = chunk.disabledEntities[blockIdx];
                            if (total >= (end - idx) >> 1) {
                                for (; idx < end; idx++) {
                                    if ((entitiesMask & (1UL << idx)) > 0) {
                                        eid = blockEntity + idx;
                                        migration(ref reader, entity, oldVersion, (disabledMask & (1UL << idx)) != 0);
                                    }
                                }
                            } else {
                                do {
                                    eid = blockEntity + idx;
                                    migration(ref reader, entity, oldVersion, (disabledMask & (1UL << idx)) != 0);
                                    entitiesMask &= entitiesMask - 1UL;
                                    idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                                } while (entitiesMask > 0);
                            }
                        }
                    }
                }
            }
            
            for (uint i = 0; i < chunkCount; i++) {
                ref var chunk = ref chunks[i];
                ArrayPool<ulong>.Shared.Return(chunk.entities, true);
                ArrayPool<ulong>.Shared.Return(chunk.disabledEntities, true);
            }
            
            ArrayPool<ComponentsChunk>.Shared.Return(chunks, true);
        }
    }
}