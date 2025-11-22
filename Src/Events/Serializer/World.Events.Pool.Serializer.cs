using System;
using System.Diagnostics.CodeAnalysis;
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
                        if (!BinaryPack.IsRegistered<EventReceiver<WorldType, T>>()) {
                            BinaryPack.RegisterWithCollections<EventReceiver<WorldType, T>, UnmanagedPackArrayStrategy<EventReceiver<WorldType, T>>>(
                                (ref BinaryPackWriter writer, in EventReceiver<WorldType, T> value) => writer.WriteInt(value._id),
                                (ref BinaryPackReader reader) => new EventReceiver<WorldType, T>(reader.ReadInt())
                            );
                        }
                        
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
                        var notEmpty = pool.receiversCount > pool.deletedReceiversCount;
                        writer.WriteByte(version);
                        writer.WriteUlong(pool.sequence);
                        writer.WriteBool(notEmpty);
                        writer.WriteUshort((ushort) pool.receivers.Length);
                        writer.WriteArrayUnmanaged(pool.receivers, 0, pool.receiversCount);
                        
                        if (notEmpty) {
                            var minSeq = pool.sequence;
                            var maxSeq = pool.sequence;
                            for (var i = 0; i < pool.receiversCount; i++) {
                                ref var receiver = ref pool.receivers[i];
                                if (!receiver.Deleted && receiver.Sequence < minSeq) {
                                    minSeq = receiver.Sequence;
                                }
                            }
                            var curPageIdx = (uint) ((minSeq >> EVENT_PAGE_SHIFT) & PAGES_OFFSET_MASK);
                            var maxPageIdx = (uint) ((maxSeq >> EVENT_PAGE_SHIFT) & PAGES_OFFSET_MASK);
                            var maxInPageIdx = (uint) (maxSeq & EVENT_PAGE_OFFSET_MASK);

                            var isUnmanaged = _readWriteArrayStrategy.IsUnmanaged();
                            writer.WriteBool(isUnmanaged);
                            ushort count = 0;
                            var offset = writer.MakePoint(sizeof(ushort));
                            while (curPageIdx <= maxPageIdx) {
                                if (curPageIdx == maxPageIdx && maxInPageIdx == 0) {
                                    break;
                                }
                                
                                ref var page = ref pool.pages[curPageIdx];
                                writer.WriteUint(curPageIdx);
                                writer.WriteUshort(page.Version);
                                writer.WriteArrayUnmanaged(page.Mask);
                                writer.WriteArrayUnmanaged(page.UnreadReceiversCount);
                                if (isUnmanaged) {
                                    _readWriteArrayStrategy.WriteArray(ref writer, page.Data);
                                } else {
                                    for (var eIdx = 0; eIdx < EVENTS_PER_PAGE; eIdx++) {
                                        if ((page.Mask[eIdx >> EVENT_IN_PAGE_MASK_SHIFT] & (1Ul << (eIdx & EVENT_IN_PAGE_OFFSET_MASK))) != 0) {
                                            writer.Write(in page.Data[eIdx]);
                                        }
                                    }
                                }
                                count++;
                                curPageIdx++;
                            }
                            writer.WriteUshortAt(offset, count);
                        }
                    }

                    [MethodImpl(AggressiveInlining)]
                    internal void ReadAll(ref BinaryPackReader reader, ref Pool<T> pool) {
                        var oldVersion = reader.ReadByte();
                        pool.sequence = reader.ReadUlong();
                        var notEmpty = reader.ReadBool();
                        var len = reader.ReadUshort();
                        if (len > pool.receivers.Length) {
                            Array.Resize(ref pool.receivers, len);
                        }
                        reader.ReadArrayUnmanaged(ref pool.receivers);

                        if (notEmpty) {
                            var isUnmanaged = reader.ReadBool();
                            var count = reader.ReadUshort();
                            for (var i = 0; i < count; i++) {
                                var pageIdx = reader.ReadUint();
                                ref var page = ref pool.pages[pageIdx];
                                page.Version = reader.ReadUshort();
                                
                                if (pool.freePagesCount > 0) {
                                    page.FromFree(ref pool.freePages[--pool.freePagesCount]);
                                } else {
                                    page.InitNew();
                                    pool.maxPagesCount++;
                                }
                                reader.ReadArrayUnmanaged(ref page.Mask);
                                reader.ReadArrayUnmanaged(ref page.UnreadReceiversCount);
                                if (version == oldVersion) {
                                    if (isUnmanaged) {
                                        _readWriteArrayStrategy.ReadArray(ref reader, ref page.Data);
                                    } else {
                                        for (var eIdx = 0; eIdx < EVENTS_PER_PAGE; eIdx++) {
                                            if ((page.Mask[eIdx >> EVENT_IN_PAGE_MASK_SHIFT] & (1Ul << (eIdx & EVENT_IN_PAGE_OFFSET_MASK))) != 0) {
                                                page.Data[eIdx] = reader.Read<T>();
                                            }
                                        }
                                    }
                                } else {
                                    uint oneSize = default;
                                    if (isUnmanaged) {
                                        _ = reader.ReadNullFlag();
                                        var size = reader.ReadInt();
                                        var byteSize = reader.ReadUint();
                                        oneSize = (uint) (byteSize / size);
                                    }
                                    for (var eIdx = 0; eIdx < EVENTS_PER_PAGE; eIdx++) {
                                        if ((page.Mask[eIdx >> EVENT_IN_PAGE_MASK_SHIFT] & (1Ul << (eIdx & EVENT_IN_PAGE_OFFSET_MASK))) != 0) {
                                            page.Data[eIdx] = _migrationReader(ref reader, oldVersion);
                                        } else if (isUnmanaged) {
                                            reader.SkipNext(oneSize);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    internal static class EventSerializerUtils {
        [MethodImpl(AggressiveInlining)]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        internal static void DeleteAllEventMigration<WorldType>(this ref BinaryPackReader reader, EcsEventDeleteMigrationReader<WorldType> migration)
            where WorldType : struct, IWorldType {
            var oldVersion = reader.ReadByte();
            var sequence = reader.ReadUlong();
            var notEmpty = reader.ReadBool();
            var len = reader.ReadUshort();

            reader.ReadArrayUnmanagedPooled<ReceiverData>(out var handle);
            handle.Return();
            
            if (notEmpty) {
                var isUnmanaged = reader.ReadBool();
                var count = reader.ReadUshort();
                for (var i = 0; i < count; i++) {
                    var pageIdx = reader.ReadUint();
                    var Version = reader.ReadUshort();

                    var mask = reader.ReadArrayUnmanagedPooled<ulong>(out var maskHandle).Array!;
                    reader.ReadArrayUnmanagedPooled<ushort>(out var unreadReceiversCountHandle);
                    unreadReceiversCountHandle.Return();
                    uint oneSize = default;
                    if (isUnmanaged) {
                        _ = reader.ReadNullFlag();
                        var size = reader.ReadInt();
                        var byteSize = reader.ReadUint();
                        oneSize = (uint) (byteSize / size);
                    }

                    for (var eIdx = 0; eIdx < World<WorldType>.Events.EVENTS_PER_PAGE; eIdx++) {
                        if ((mask[eIdx >> World<WorldType>.Events.EVENT_IN_PAGE_MASK_SHIFT] & (1Ul << (eIdx & World<WorldType>.Events.EVENT_IN_PAGE_OFFSET_MASK))) != 0) {
                            migration(ref reader, oldVersion);
                        } else if (isUnmanaged) {
                            reader.SkipNext(oneSize);
                        }
                    }
                    maskHandle.Return();
                }
            }
        }
    }
}