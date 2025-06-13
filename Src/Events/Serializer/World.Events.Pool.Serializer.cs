using System;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    public delegate T EcsEventMigrationReader<T, WorldType>(ref BinaryPackReader reader, byte version)
        where T : struct
        where WorldType : struct, IWorldType;

    public delegate void EcsEventDeleteMigrationReader<WorldType>(ref BinaryPackReader reader, byte version)
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
        public abstract partial class Events {
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal partial struct Pool<T> where T : struct, IEvent {
                #if ENABLE_IL2CPP
                [Il2CppSetOption(Option.NullChecks, false)]
                [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
                [Il2CppEagerStaticClassConstruction]
                #endif
                public struct Serializer {
                    internal static Serializer Value;

                    private IPackArrayStrategy<T> _readWriteArrayStrategy;
                    private EcsEventMigrationReader<T, WorldType> _migrationReader;
                    internal Guid guid;
                    internal byte version;

                    [MethodImpl(AggressiveInlining)]
                    public void Create(IEventConfig<T, WorldType> config) {
                        guid = config.Id();
                        version = config.Version();
                        if (guid != Guid.Empty) {
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
                    internal void WriteAll(ref BinaryPackWriter writer, ref Pool<T> pool) {
                        var count = (pool._dataCount - pool._dataFirstIdx);
                        writer.WriteByte(version);
                        writer.WriteInt(count);
                        writer.WriteInt(pool._data.Length);
                        _readWriteArrayStrategy.WriteArray(ref writer, pool._data, pool._dataFirstIdx, count);
                    }

                    [MethodImpl(AggressiveInlining)]
                    internal void ReadAll(ref BinaryPackReader reader, ref Pool<T> pool) {
                        var oldVersion = reader.ReadByte();
                        var dataCount = reader.ReadInt();

                        var dataCapacity = reader.ReadInt();
                        if (pool._data == null || dataCapacity > pool._data.Length) {
                            pool._data = new T[dataCapacity];
                            pool._versions = new short[dataCapacity];
                            pool._dataReceiverUnreadCount = new int[dataCapacity];
                        }

                        if (version == oldVersion) {
                            reader.SkipArrayHeaders();
                            for (var i = 0; i < dataCount; i++) {
                                pool.Add(reader.Read<T>());
                            }
                        } else {
                            reader.SkipArrayHeaders();
                            for (var i = 0; i < dataCount; i++) {
                                pool.Add(_migrationReader(ref reader, oldVersion));
                            }
                        }
                    }
                }
            }
        }
    }

    internal static class EventSerializerUtils {
        [MethodImpl(AggressiveInlining)]
        internal static void DeleteAllEventMigration<WorldType>(this ref BinaryPackReader reader, EcsEventDeleteMigrationReader<WorldType> migration)
            where WorldType : struct, IWorldType {
            var oldVersion = reader.ReadByte();
            var dataCount = reader.ReadInt();
            reader.SkipNext(sizeof(int)); // capacity

            reader.SkipArrayHeaders();
            for (var i = 0; i < dataCount; i++) {
                migration(ref reader, oldVersion);
            }
        }
    }
}