#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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
                internal void WriteChunk(ref BinaryPackWriter writer, ref Components<T> pool, uint chunkIdx) {
                    ref var chunk = ref pool.chunks[chunkIdx];
                    
                    writer.WriteBool(pool._clearable);
                    writer.WriteUlong(chunk.notEmptyBlocks);
                    if (chunk.notEmptyBlocks != 0) {
                        var entities = chunk.entities;
                        
                        writer.WriteByte(version);
                        writer.WriteUlong(chunk.fullBlocks);
                        writer.WriteArrayUnmanaged(entities);
                        writer.WriteArrayUnmanaged(chunk.disabledEntities);

                        var unmanagedStrategy = _readWriteArrayStrategy.IsUnmanaged();
                        writer.WriteBool(unmanagedStrategy);

                        if (unmanagedStrategy) {
                            var eidChunk = chunkIdx << Const.ENTITIES_IN_CHUNK_SHIFT;
                            var dataIdxStart = (ushort) (eidChunk >> Const.DATA_SHIFT);
                            var dataIdxEnd = dataIdxStart + Const.DATA_BLOCKS_IN_CHUNK;
                            var blockIdxStart = (byte) ((eidChunk >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);

                            const int blockStep = Const.BLOCK_IN_CHUNK / Const.DATA_BLOCKS_IN_CHUNK; 

                            for (var dataIdx = dataIdxStart; dataIdx < dataIdxEnd; dataIdx++) {
                                ref var components = ref pool.data[dataIdx];
                                writer.WriteNotNullFlag(components);
                                if (components != null) {
                                    var curBlockIdx = 0;
                                    ulong mask;
                                    do {
                                        mask = entities[blockIdxStart + curBlockIdx];
                                    } while (mask == 0 && ++curBlockIdx < blockStep);
                                    var start = Utils.Lsb(mask) + Const.ENTITIES_IN_BLOCK * curBlockIdx;

                                    curBlockIdx = blockStep - 1;
                                    do {
                                        mask = entities[blockIdxStart + curBlockIdx];
                                    } while (mask == 0 && --curBlockIdx >= 0);
                                    
                                    var end = Utils.Msb(mask) + Const.ENTITIES_IN_BLOCK * curBlockIdx + 1;
                                    var count = end - start;
                                    writer.WriteInt(start);
                                    writer.WriteInt(count);
                                    _readWriteArrayStrategy.WriteArray(ref writer, components, start, count);
                                }

                                blockIdxStart += blockStep;
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
                            var chunkMask = chunk.notEmptyBlocks;

                            while (chunkMask > 0) {
                                var blockIdx = (uint) deBruijn[(int) (((chunkMask & (ulong) -(long) chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                                chunkMask &= chunkMask - 1;
                                var entitiesMask = entities[blockIdx];

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
                [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
                internal void ReadChunk(ref BinaryPackReader reader, ref Components<T> pool, uint chunkIdx) {
                    ref var chunk = ref pool.chunks[chunkIdx];

                    var clearable = reader.ReadBool(); // TODO  
                    chunk.notEmptyBlocks = reader.ReadUlong();

                    if (chunk.notEmptyBlocks != 0) {
                        pool.InitChunkSimple(ref chunk, chunkIdx);
                        
                        var oldVersion = reader.ReadByte();
                        chunk.fullBlocks = reader.ReadUlong();
                        reader.ReadArrayUnmanaged(ref chunk.entities);
                        reader.ReadArrayUnmanaged(ref chunk.disabledEntities);

                        var unmanaged = reader.ReadBool(); // TODO validate strategy

                        if (unmanaged) {
                            var dataIdxStart = (ushort) ((chunkIdx << Const.ENTITIES_IN_CHUNK_SHIFT) >> Const.DATA_SHIFT);
                            var dataIdxEnd = dataIdxStart + Const.DATA_BLOCKS_IN_CHUNK;

                            for (var dataIdx = dataIdxStart; dataIdx < dataIdxEnd; dataIdx++) {
                                ref var components = ref pool.data[dataIdx];
                                
                                var notEmpty = !reader.ReadNullFlag();
                                if (notEmpty) {
                                    pool.InitializeData(ref components);
                                    var start = reader.ReadInt();
                                    var count = reader.ReadInt();
                                    
                                    if (version == oldVersion) {
                                        _readWriteArrayStrategy.ReadArray(ref reader, ref components, start);
                                    } else {
                                        _ = reader.ReadNullFlag();
                                        _ = reader.ReadInt(); // count
                                        var byteSize = reader.ReadUint();
                                        var oneSize = byteSize / count;
                                        var dataEntity = (uint) (dataIdx * Const.DATA_BLOCK_SIZE);
                                        var entity = new Entity();
                                        ref var eid = ref entity.id;
                                        for (var i = start; i < start + count; i++) {
                                            eid = (uint) (dataEntity + i);
                                            var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                                            var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                                            if ((chunk.entities[blockIdx] & (1UL << blockEntityIdx)) != 0) {
                                                eid += Const.ENTITY_ID_OFFSET;
                                                components[i] = _migrationReader(ref reader, entity, oldVersion, (chunk.disabledEntities[blockIdx] & (1UL << blockEntityIdx)) != 0);
                                            } else {
                                                reader.SkipNext((uint) oneSize);
                                            }
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
                            var chunkMask = chunk.notEmptyBlocks;

                            var entity = new Entity();
                            ref var eid = ref entity.id;
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
                                        ref var components = ref pool.data[dataIdx];
                                        pool.InitializeData(ref components);
                                        #if NET6_0_OR_GREATER
                                        data = new(components);
                                        #else
                                        data = components;
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
                                                    eid = blockEntity + idx + Const.ENTITY_ID_OFFSET;
                                                    data[idx + dOffset] = _migrationReader(ref reader, entity, oldVersion, (disabledMask & (1UL << idx)) != 0);
                                                }
                                            }
                                        } else {
                                            do {
                                                eid = blockEntity + idx + Const.ENTITY_ID_OFFSET;
                                                data[idx + dOffset] = _migrationReader(ref reader, entity, oldVersion, (disabledMask & (1UL << idx)) != 0);
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
                    var offset = writer.MakePoint(sizeof(ushort));
                    writer.WriteByte(version);
                    writer.Write(in pool.RefInternal(entity));
                    var size = writer.Position - (offset + sizeof(short));
                    #if FFS_ECS_DEBUG
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
        internal static void DeleteAllComponentMigration<WorldType>(this ref BinaryPackReader reader, EcsComponentDeleteMigrationReader<WorldType> migration, uint chunkIdx) 
            where WorldType : struct, IWorldType {
            var clearable = reader.ReadBool();
            var notEmptyBlocks = reader.ReadUlong();

            if (notEmptyBlocks != 0) {
                var entities = ArrayPool<ulong>.Shared.Rent(Const.BLOCK_IN_CHUNK);
                var disabledEntities = ArrayPool<ulong>.Shared.Rent(Const.BLOCK_IN_CHUNK);

                var oldVersion = reader.ReadByte();
                var fullBlocks = reader.ReadUlong();
                reader.ReadArrayUnmanaged(ref entities);
                reader.ReadArrayUnmanaged(ref disabledEntities);

                var unmanaged = reader.ReadBool();

                if (unmanaged) {
                    var dataIdxStart = (ushort) ((chunkIdx << Const.ENTITIES_IN_CHUNK_SHIFT) >> Const.DATA_SHIFT);
                    var dataIdxEnd = dataIdxStart + Const.DATA_BLOCKS_IN_CHUNK;

                    for (var dataIdx = dataIdxStart; dataIdx < dataIdxEnd; dataIdx++) {
                        var notEmpty = !reader.ReadNullFlag();
                        if (notEmpty) {
                            var start = reader.ReadInt();
                            var count = reader.ReadInt();

                            _ = reader.ReadNullFlag();
                            _ = reader.ReadInt(); // count
                            var byteSize = reader.ReadUint();
                            var oneSize = byteSize / count;
                            var dataEntity = (uint) (dataIdx * Const.DATA_BLOCK_SIZE);
                            var entity = new World<WorldType>.Entity();
                            ref var eid = ref entity.id;
                            for (var i = start; i < start + count; i++) {
                                eid = (uint) (dataEntity + i);
                                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                                if ((entities[blockIdx] & (1UL << blockEntityIdx)) != 0) {
                                    var disabled = (disabledEntities[blockIdx] & (1UL << blockEntityIdx)) != 0;
                                    eid += Const.ENTITY_ID_OFFSET;
                                    migration(ref reader, entity, oldVersion, disabled);
                                } else {
                                    reader.SkipNext((uint) oneSize);
                                }
                            }
                        }
                    }
                } else {
                    #if NET6_0_OR_GREATER
                    ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
                    #else
                    var deBruijn = Utils.DeBruijn;
                    #endif
                    var chunkMask = notEmptyBlocks;

                    var entity = new World<WorldType>.Entity();
                    ref var eid = ref entity.id;
                    while (chunkMask > 0) {
                        var blockIdx = (uint) deBruijn[(int) (((chunkMask & (ulong) -(long) chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                        chunkMask &= chunkMask - 1;
                        var entitiesMask = entities[blockIdx];

                        if (entitiesMask > 0) {
                            var globalBlockIdx = blockIdx + (chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                            var blockEntity = (uint) (globalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT);
       
                            var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                            var end = Utils.ApproximateMSB(entitiesMask);
                            var total = Utils.PopCnt(entitiesMask);
                            var disabledMask = disabledEntities[blockIdx];
                            if (total >= (end - idx) >> 1) {
                                for (; idx < end; idx++) {
                                    if ((entitiesMask & (1UL << idx)) > 0) {
                                        eid = blockEntity + idx + Const.ENTITY_ID_OFFSET;
                                        migration(ref reader, entity, oldVersion, (disabledMask & (1UL << idx)) != 0);
                                    }
                                }
                            } else {
                                do {
                                    eid = blockEntity + idx + Const.ENTITY_ID_OFFSET;
                                    migration(ref reader, entity, oldVersion, (disabledMask & (1UL << idx)) != 0);
                                    entitiesMask &= entitiesMask - 1UL;
                                    idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                                } while (entitiesMask > 0);
                            }
                        }
                    }
                }
                
                ArrayPool<ulong>.Shared.Return(entities, true);
                ArrayPool<ulong>.Shared.Return(disabledEntities, true);
            }
        }
    }
}