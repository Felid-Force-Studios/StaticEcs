using System;
using System.Buffers;
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
                internal void WriteAll(ref BinaryPackWriter writer, ref Components<T> pool) {
                    writer.WriteByte(version);
                    writer.WriteUint(pool._componentsCount);
                    writer.WriteInt(pool._entities.Length);
                    writer.WriteInt(pool._dataIdxByEntityId.Length);
                    writer.WriteArrayUnmanaged(pool._entities, 0, (int) pool._componentsCount);
                    writer.WriteArrayUnmanaged(pool._dataIdxByEntityId, 0, (int) Entity.entitiesCount);
                    _readWriteArrayStrategy.WriteArray(ref writer, pool._data, 0, (int) pool._componentsCount);
                }

                [MethodImpl(AggressiveInlining)]
                internal void ReadAll(ref BinaryPackReader reader, ref Components<T> pool) {
                    var oldVersion = reader.ReadByte();
                    pool._componentsCount = reader.ReadUint();

                    var dataCapacity = reader.ReadInt();
                    if (pool._entities == null || dataCapacity > pool._data.Length) {
                        pool._entities = new uint[dataCapacity];
                        pool._data = new T[dataCapacity];
                    }

                    var entitiesCapacity = reader.ReadInt();
                    if (pool._dataIdxByEntityId == null || entitiesCapacity > pool._dataIdxByEntityId.Length) {
                        pool._dataIdxByEntityId = new uint[entitiesCapacity];
                        for (uint i = 0; i < entitiesCapacity; i++) {
                            pool._dataIdxByEntityId[i] = Const.EmptyComponentMask;
                        }
                    }

                    reader.ReadArrayUnmanaged(ref pool._entities);
                    reader.ReadArrayUnmanaged(ref pool._dataIdxByEntityId);
                    if (version == oldVersion) {
                        _readWriteArrayStrategy.ReadArray(ref reader, ref pool._data);
                    } else {
                        reader.SkipArrayHeaders();
                        for (var i = 0; i < pool._componentsCount; i++) {
                            var entity = pool._entities[i];
                            var disabled = (pool._dataIdxByEntityId[entity] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask;
                            pool._data[i] = _migrationReader(ref reader, new Entity(entity), oldVersion, disabled);
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void Write(ref BinaryPackWriter writer, ref Components<T> pool, Entity entity) {
                    var offset = writer.MakePoint(sizeof(short));
                    writer.WriteByte(version);
                    var idx = pool._dataIdxByEntityId[entity._id];
                    writer.Write(in pool._data[idx & Const.DisabledComponentMaskInv]);
                    var size = writer.Position - (offset + sizeof(short));
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (size > short.MaxValue) throw new StaticEcsException($"Size of component {typeof(T)} more than {short.MaxValue} bytes");
                    #endif
                    var disabled = (idx & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask;
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
            reader.SkipNext(sizeof(ushort)); // id
            var componentsCount = reader.ReadUint();

            var dataCapacity = reader.ReadInt();
            var entitiesCapacity = reader.ReadInt();

            var entities = ArrayPool<uint>.Shared.Rent(dataCapacity);
            var dataIdxByEntityId = ArrayPool<uint>.Shared.Rent(entitiesCapacity);

            reader.ReadArrayUnmanaged(ref entities);
            reader.ReadArrayUnmanaged(ref dataIdxByEntityId);
            reader.SkipArrayHeaders();
            for (var i = 0; i < componentsCount; i++) {
                var entity = entities[i];
                var disabled = (dataIdxByEntityId[entity] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask;
                migration(ref reader, new World<WorldType>.Entity(entity), oldVersion, disabled);
            }
            
            ArrayPool<uint>.Shared.Return(entities);
            ArrayPool<uint>.Shared.Return(dataIdxByEntityId);
        }
    }
}