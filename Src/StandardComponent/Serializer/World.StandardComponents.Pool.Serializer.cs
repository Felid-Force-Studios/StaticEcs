using System;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    public delegate T EcsStandardComponentMigrationReader<T, WorldType>(ref BinaryPackReader reader, World<WorldType>.Entity entity, byte version)
        where T : struct
        where WorldType : struct, IWorldType;
    
    public delegate void EcsStandardComponentDeleteMigrationReader<WorldType>(ref BinaryPackReader reader, World<WorldType>.Entity entity, byte version)
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
        public partial struct StandardComponents<T> where T : struct, IStandardComponent {
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            [Il2CppEagerStaticClassConstruction]
            #endif
            public struct Serializer {
                internal static Serializer Value;
                
                private IPackArrayStrategy<T> _readWriteArrayStrategy;
                private EcsStandardComponentMigrationReader<T, WorldType> _migrationReader;
                internal Guid guid;
                internal byte version;
                
                [MethodImpl(AggressiveInlining)]
                public void Create(IStandardComponentConfig<T, WorldType> config) {
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
                internal void WriteAll(ref BinaryPackWriter writer, ref StandardComponents<T> pool) {
                    writer.WriteByte(version);
                    writer.WriteInt(pool._data.Length);
                    writer.WriteUint(Entity.entitiesCount);
                    _readWriteArrayStrategy.WriteArray(ref writer, pool._data, 0, (int) Entity.entitiesCount);
                }

                [MethodImpl(AggressiveInlining)]
                internal void ReadAll(ref BinaryPackReader reader, ref StandardComponents<T> pool) {
                    var oldVersion = reader.ReadByte();
                    
                    var capacity = reader.ReadInt();
                    var entitiesCount = reader.ReadUint();
                    if (pool._data == null || capacity > pool._data.Length) {
                        pool._data = new T[capacity];
                    }
                    
                    if (version == oldVersion) {
                        _readWriteArrayStrategy.ReadArray(ref reader, ref pool._data);
                    } else {
                        reader.SkipArrayHeaders();
                        for (var i = 0; i < entitiesCount; i++) {
                            var entity = new Entity((uint) i);
                            if (entity.IsActual()) {
                                pool._data[i] = _migrationReader(ref reader, entity, oldVersion);
                            }
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void Write(ref BinaryPackWriter writer, ref StandardComponents<T> pool, Entity entity) {
                    var offset = writer.MakePoint(sizeof(short));
                    writer.WriteByte(version);
                    writer.Write(in pool._data[entity._id]);
                    var size = writer.Position - (offset + sizeof(short));
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (size > short.MaxValue) throw new StaticEcsException($"Size of component {typeof(T)} more than {short.MaxValue} bytes");
                    #endif
                    writer.WriteUshortAt(offset, (ushort) size);
                }

                [MethodImpl(AggressiveInlining)]
                internal void Read(ref BinaryPackReader reader, ref StandardComponents<T> pool, Entity entity) {
                    reader.SkipNext(sizeof(ushort)); // size
                    var oldVersion = reader.ReadByte();

                    pool._data[entity._id] = version == oldVersion
                        ? reader.Read<T>()
                        : _migrationReader(ref reader, entity, oldVersion);
                }
            }
        }
    }
    
    internal static class StandardComponentSerializerUtils {
        [MethodImpl(AggressiveInlining)]
        internal static void SkipOneStandardComponent(this ref BinaryPackReader reader) {
            reader.SkipNext(reader.ReadUshort());
        }
        
        [MethodImpl(AggressiveInlining)]
        internal static void DeleteOneStandardComponentMigration<WorldType>(this ref BinaryPackReader reader, World<WorldType>.Entity entity, EcsStandardComponentDeleteMigrationReader<WorldType> migration) 
            where WorldType : struct, IWorldType {
            reader.SkipNext(sizeof(ushort)); // size
            var oldVersion = reader.ReadByte();
                    
            migration(ref reader, entity, oldVersion);
        }
        
        [MethodImpl(AggressiveInlining)]
        internal static void DeleteAllStandardComponentMigration<WorldType>(this ref BinaryPackReader reader, EcsStandardComponentDeleteMigrationReader<WorldType> migration) 
            where WorldType : struct, IWorldType {
            var oldVersion = reader.ReadByte();
            reader.SkipNext(sizeof(ushort)); // id
            reader.SkipNext(sizeof(int));    // capacity
            var entitiesCount = reader.ReadUint();

            reader.SkipArrayHeaders();
            for (var i = 0; i < entitiesCount; i++) {
                var entity = new World<WorldType>.Entity((uint) i);
                if (entity.IsActual()) {
                    migration(ref reader, entity, oldVersion);
                }
            }
        }
    }
}