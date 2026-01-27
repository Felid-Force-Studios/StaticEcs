#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    internal struct ReceiverData {
        internal ulong Sequence;
        internal bool Deleted;
    }
    
    /// <summary>
    /// Marker interface for event structs used with the ECS event system.
    /// <para>
    /// Implement this interface on any <c>struct</c> to make it usable as an event type with
    /// <see cref="World{TWorld}.SendEvent{TEvent}"/> and <see cref="EventReceiver{TWorld, TEvent}"/>.
    /// </para>
    /// <para>
    /// Events are stored in a ring buffer (256 pages × 512 events = 131 072 max in-flight).
    /// Each receiver independently tracks its read position; an event's data is released only
    /// after <em>all</em> active receivers have consumed or skipped it.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Override <see cref="Write"/> and <see cref="Read"/> only when the event type requires
    /// custom binary serialization (e.g., contains managed references or version-specific migration).
    /// For plain unmanaged structs the default (no-op) implementations are sufficient —
    /// the framework uses <see cref="IPackArrayStrategy{T}"/> for bulk serialization.
    /// </remarks>
    public interface IEvent {
        /// <summary>
        /// Serializes this event instance into a binary stream.
        /// Override to provide custom serialization for non-trivially-copyable event types.
        /// The default implementation is a no-op (unmanaged events are bulk-copied).
        /// </summary>
        /// <param name="writer">Binary writer to write event data into.</param>
        public void Write(ref BinaryPackWriter writer) {}

        /// <summary>
        /// Deserializes this event instance from a binary stream.
        /// Override to provide custom deserialization or version migration logic.
        /// The default implementation is a no-op (unmanaged events are bulk-copied).
        /// </summary>
        /// <param name="reader">Binary reader to read event data from.</param>
        /// <param name="version">
        /// The schema version that was active when the data was serialized.
        /// Compare against the current <see cref="EventTypeConfig{T}.Version"/> to perform migration.
        /// </param>
        public void Read(ref BinaryPackReader reader, byte version) {}

    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    /// <summary>
    /// Configuration for registering an event type with
    /// <c>World&lt;TWorld&gt;.Types().Event&lt;T&gt;()</c>.
    /// </summary>
    /// <typeparam name="T">The event struct type implementing <see cref="IEvent"/>.</typeparam>
    public readonly struct EventTypeConfig<T> where T : struct, IEvent {
        /// <summary>
        /// Stable GUID used to identify this event type during serialization and deserialization.
        /// When set to <see cref="Guid.Empty"/> (default), the event type is excluded from serialization.
        /// Assign a non-empty GUID to enable persistent save/load of in-flight events.
        /// </summary>
        public readonly Guid Guid;

        /// <summary>
        /// Schema version number for data migration.
        /// When loading serialized data whose stored version differs from this value,
        /// <see cref="IEvent.Read"/> receives the old version so the event can migrate its fields.
        /// </summary>
        public readonly byte Version;

        /// <summary>
        /// Strategy for binary array serialization of event pages.
        /// Defaults to <see cref="StructPackArrayStrategy{T}"/> which performs unmanaged bulk copy
        /// for blittable types and falls back to per-element <see cref="IEvent.Write"/>/<see cref="IEvent.Read"/>
        /// for managed types.
        /// </summary>
        public readonly IPackArrayStrategy<T> ReadWriteStrategy;

        /// <summary>
        /// Creates a new event type configuration.
        /// </summary>
        /// <param name="guid">
        /// Stable serialization identifier. Set to a non-empty GUID to enable serialization of in-flight events.
        /// Default is <see cref="Guid.Empty"/> (serialization disabled).
        /// </param>
        /// <param name="version">Schema version for data migration. Default is 0.</param>
        /// <param name="readWriteStrategy">
        /// Custom binary serialization strategy. Default is <see cref="StructPackArrayStrategy{T}"/>.
        /// </param>
        public EventTypeConfig(Guid guid = default,
                               byte version = 0,
                               IPackArrayStrategy<T> readWriteStrategy = null) {
            Guid = guid;
            Version = version;
            ReadWriteStrategy = readWriteStrategy ?? new StructPackArrayStrategy<T>();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public abstract partial class World<TWorld> {
        /// <summary>
        /// Delegate invoked for each event during bulk reading via
        /// <see cref="EventReceiver{TWorld, TEvent}.ReadAll"/>.
        /// The delegate receives an <see cref="Event{T}"/> wrapper that provides access to the
        /// event's data and metadata.
        /// </summary>
        /// <typeparam name="T">The event struct type.</typeparam>
        /// <param name="eventValue">Wrapper around the current event, providing access to <see cref="Event{T}.Value"/>.</param>
        public delegate void EventAction<T>(Event<T> eventValue) where T : struct, IEvent;
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        /// <summary>
        /// Lightweight wrapper around a single event instance, providing access to the event's
        /// data and metadata during iteration or bulk reading.
        /// <para>
        /// Obtained from <see cref="EventIterator{TEvent}.Current"/> during <c>foreach</c> iteration
        /// over an <see cref="EventReceiver{TWorld, TEvent}"/>, or from the delegate parameter in
        /// <see cref="EventReceiver{TWorld, TEvent}.ReadAll"/>.
        /// </para>
        /// </summary>
        /// <typeparam name="TEvent">The event struct type.</typeparam>
        public ref struct Event<TEvent> where TEvent : struct, IEvent {
            internal int EventIdx;

            [MethodImpl(AggressiveInlining)]
            public Event(int eventIdx) => EventIdx = eventIdx;

            /// <summary>
            /// Returns a reference to the event data stored in the ring buffer.
            /// The reference is valid only while the event is alive (not yet suppressed and not
            /// consumed by all receivers).
            /// </summary>
            /// <exception cref="StaticEcsException">Thrown in debug builds if the event has been suppressed.</exception>
            public ref TEvent Value {
                [MethodImpl(AggressiveInlining)]
                get {
                    #if FFS_ECS_DEBUG
                    if (EventIdx < 0) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Event<{typeof(TEvent)}>.Value ] event is deleted");
                    #endif
                    return ref Events<TEvent>.Instance.Get(EventIdx);
                }
            }

            /// <summary>
            /// Suppresses (cancels) this event for <b>all</b> receivers, not just the current one.
            /// <para>
            /// After suppression the event's data is cleared (if the type contains managed references),
            /// its mask bit is removed, and no other receiver will see it. The <see cref="Event{TEvent}"/>
            /// wrapper becomes invalid (further access throws in debug builds).
            /// </para>
            /// </summary>
            /// <remarks>
            /// <b>Warning:</b> this is a global operation — it removes the event from every receiver's
            /// unread queue. Use with care in multi-receiver scenarios.
            /// </remarks>
            /// <exception cref="StaticEcsException">Thrown in debug builds if the event has already been suppressed.</exception>
            [MethodImpl(AggressiveInlining)]
            public void Suppress() {
                #if FFS_ECS_DEBUG
                if (EventIdx < 0) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Event<{typeof(TEvent)}>.Suppress ] event is deleted");
                #endif
                Events<TEvent>.Instance.SuppressOne(EventIdx);
                EventIdx = -1;
            }

            /// <summary>
            /// Returns <c>true</c> if this receiver is the last one that has not yet consumed this event.
            /// <para>
            /// Useful for performing cleanup or side effects that should happen exactly once,
            /// when the last reader processes the event.
            /// </para>
            /// </summary>
            /// <exception cref="StaticEcsException">Thrown in debug builds if the event has been suppressed.</exception>
            [MethodImpl(AggressiveInlining)]
            public bool IsLastReading() {
                #if FFS_ECS_DEBUG
                if (EventIdx < 0) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Event<{typeof(TEvent)}>.IsLastReading ] event is deleted");
                #endif
                return Events<TEvent>.Instance.IsLastReading(EventIdx);
            }

            /// <summary>
            /// Returns the number of <b>other</b> receivers that have not yet consumed this event
            /// (excluding the current reader). A return value of <c>0</c> means this is the last reader
            /// (equivalent to <see cref="IsLastReading"/> returning <c>true</c>).
            /// </summary>
            /// <exception cref="StaticEcsException">Thrown in debug builds if the event has been suppressed.</exception>
            [MethodImpl(AggressiveInlining)]
            public int UnreadCount() {
                #if FFS_ECS_DEBUG
                if (EventIdx < 0) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Event<{typeof(TEvent)}>.UnreadCount ] event is deleted");
                #endif
                return Events<TEvent>.Instance.UnreadCount(EventIdx) - 1;
            }
        }
        
        // ReSharper disable InconsistentNaming
        internal const int MAX_PAGES = 256;
        internal const int PAGES_OFFSET_MASK = MAX_PAGES - 1;
        internal const int EVENTS_PER_PAGE = 512;
        internal const int EVENT_PAGE_SHIFT = 9;
        internal const int EVENT_PAGE_OFFSET_MASK = EVENTS_PER_PAGE - 1;
        internal const int MASKS_IN_PAGE = EVENTS_PER_PAGE / Const.U64_BITS;
        internal const int EVENT_IN_PAGE_MASK_SHIFT = Const.U64_SHIFT;
        internal const int EVENT_IN_PAGE_OFFSET_MASK = Const.U64_MASK;
        internal const int MAX_EVENTS = MAX_PAGES * EVENTS_PER_PAGE;
        internal const int MAX_EVENTS_OFFSET_MASK = MAX_EVENTS - 1;
        // ReSharper restore InconsistentNaming
        
        #if FFS_ECS_DEBUG
        public interface IEventsDebugEventListener {
            void OnEventSent<T>(Event<T> value) where T : struct, IEvent;
            void OnEventReadAll<T>(Event<T> value) where T : struct, IEvent;
            void OnEventSuppress<T>(Event<T> value) where T : struct, IEvent;
        }
        #endif

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        internal struct Events<T> where T : struct, IEvent {
            internal static Events<T> Instance;
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
            #endif
            internal struct Page {
                internal T[] EventsData;
                internal long[] Mask;
                internal ushort[] UnreadReceiversCount;
                internal ushort Version;

                [MethodImpl(AggressiveInlining)]
                internal void Free(ref FreePage freePage) {
                    freePage.EventsData = EventsData;
                    freePage.Mask = Mask;
                    freePage.UnreadReceiversCount = UnreadReceiversCount;
                    EventsData = null;
                    Mask = null;
                    UnreadReceiversCount = null;
                    Version++;
                }
                
                [MethodImpl(AggressiveInlining)]
                internal void FromFree(ref FreePage freePage) {
                    EventsData = freePage.EventsData;
                    Mask = freePage.Mask;
                    UnreadReceiversCount = freePage.UnreadReceiversCount;
                    freePage = default;
                }
                
                [MethodImpl(AggressiveInlining)]
                internal void InitNew() {
                    EventsData = new T[EVENTS_PER_PAGE];
                    Mask = new long[MASKS_IN_PAGE];
                    UnreadReceiversCount = new ushort[EVENTS_PER_PAGE];
                }
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
            #endif
            internal struct FreePage {
                internal T[] EventsData;
                internal long[] Mask;
                internal ushort[] UnreadReceiversCount;
            }
            
            private readonly Page[] _pages;
            private readonly FreePage[] _freePages;
            private ReceiverData[] _receiverDatas;
            
            private readonly IPackArrayStrategy<T> _readWriteArrayStrategy;
            
            internal readonly Guid Guid;
            internal readonly ushort Id;
            internal readonly byte Version;
            internal readonly bool Initialized;
            
            private long _sequence;
            private ushort _maxPagesCount;
            private ushort _freePagesCount;
            private ushort _receiversCount;
            private ushort _deletedReceiversCount;
            private readonly bool _clearDataOnRead;
            private SpinLock _pagePoolLock;

            #if FFS_ECS_DEBUG
            private int _blockers;
            #endif

            [MethodImpl(AggressiveInlining)]
            internal Events(ushort id, EventTypeConfig<T> config) {
                this = default;
                Id = id;
                _pages = new Page[MAX_PAGES];
                _freePages = new FreePage[MAX_PAGES];
                _receiverDatas = new ReceiverData[32];
                _clearDataOnRead = RuntimeHelpers.IsReferenceOrContainsReferences<T>();
                if (!BinaryPack.IsRegistered<EventReceiver<TWorld, T>>()) {
                    BinaryPack.RegisterWithCollections<EventReceiver<TWorld, T>, UnmanagedPackArrayStrategy<EventReceiver<TWorld, T>>>(
                        (ref BinaryPackWriter writer, in EventReceiver<TWorld, T> value) => writer.WriteInt(value.Id),
                        (ref BinaryPackReader reader) => new EventReceiver<TWorld, T>(reader.ReadInt())
                    );
                }

                Guid = config.Guid;
                Version = config.Version;
                if (Guid != Guid.Empty) {
                    _readWriteArrayStrategy = config.ReadWriteStrategy;
                    _readWriteArrayStrategy ??= new StructPackArrayStrategy<T>();
                }
                _pagePoolLock = new SpinLock(false);
                Initialized = true;
            }

            [MethodImpl(AggressiveInlining)]
            internal EventReceiver<TWorld, T> CreateReceiver() {
                #if FFS_ECS_DEBUG
                if (_blockers > 0) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Events.Pool<{typeof(T)}>.CreateReceiver ] event pool cannot be changed, it is in read-only mode");
                #endif
                if (_deletedReceiversCount > 0) {
                    for (int i = 0; i < _receiversCount; i++) {
                        ref var receiver = ref _receiverDatas[i];
                        if (receiver.Deleted) {
                            _deletedReceiversCount--;
                            receiver.Deleted = false;
                            receiver.Sequence = (ulong) _sequence;
                            return new EventReceiver<TWorld, T>(i);
                        }
                    }
                }

                if (_receiversCount == _receiverDatas.Length) {
                    Array.Resize(ref _receiverDatas, _receiversCount << 1);
                }

                _receiverDatas[_receiversCount].Sequence = (ulong) _sequence;
                return new EventReceiver<TWorld, T>(_receiversCount++);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void DeleteReceiver(ref EventReceiver<TWorld, T> receiver) {
                #if FFS_ECS_DEBUG
                if (_blockers > 0) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Events.Pool<{typeof(T)}>.DeleteReceiver ] event pool cannot be changed, it is in read-only mode");
                #endif
                ref var receiverData = ref _receiverDatas[receiver.Id];
                if (!receiverData.Deleted) {
                    ReadAll(receiver.Id);
                    _deletedReceiversCount++;
                    receiverData.Deleted = true;
                    receiver.Id = -1;
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly void DestroyEvents() {
                Instance = default;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal readonly ref T Get(int idx) {
                return ref _pages[idx >> EVENT_PAGE_SHIFT].EventsData[idx & EVENT_PAGE_OFFSET_MASK];
            }

            [MethodImpl(AggressiveInlining)]
            internal bool Add(T value = default) {
                #if FFS_ECS_DEBUG
                if (_blockers > 0) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Events.Pool<{typeof(T)}>.Add ] event pool cannot be changed, it is in read-only mode");
                #endif
                if (_receiversCount > _deletedReceiversCount) {
                    var seq = (ulong) (Interlocked.Increment(ref _sequence) - 1);

                    #if FFS_ECS_DEBUG
                    {
                        var minReceiverSeq = seq + 1;
                        for (var ri = 0; ri < _receiversCount; ri++) {
                            ref var r = ref _receiverDatas[ri];
                            if (!r.Deleted && r.Sequence < minReceiverSeq) {
                                minReceiverSeq = r.Sequence;
                            }
                        }
                        if (seq - minReceiverSeq >= MAX_EVENTS) {
                            throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Events<{typeof(T)}>.Add ] Ring buffer overflow: sequence {seq} is {seq - minReceiverSeq} ahead of slowest receiver (max {MAX_EVENTS}). Unread events will be overwritten.");
                        }
                    }
                    #endif

                    var pageIdx = (uint) ((seq >> EVENT_PAGE_SHIFT) & PAGES_OFFSET_MASK);
                    var inPageIdx = (uint) (seq & EVENT_PAGE_OFFSET_MASK);
                    var maskIdx = (byte) (inPageIdx >> EVENT_IN_PAGE_MASK_SHIFT);
                    var inMaskBit = (byte) (inPageIdx & EVENT_IN_PAGE_OFFSET_MASK);

                    ref var page = ref _pages[pageIdx];
                    if (page.EventsData == null) {
                        var poolLockTaken = false;
                        _pagePoolLock.Enter(ref poolLockTaken);
                        #if FFS_ECS_DEBUG
                        if (!poolLockTaken) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Events<{typeof(T)}>.Add ] Failed to acquire page pool lock");
                        #endif
                        if (page.EventsData == null) {
                            if (_freePagesCount > 0) {
                                page.FromFree(ref _freePages[--_freePagesCount]);
                            } else {
                                page.InitNew();
                                _maxPagesCount++;
                            }
                        }
                        _pagePoolLock.Exit();
                    }

                    page.EventsData[inPageIdx] = value;
                    page.UnreadReceiversCount[inPageIdx] = (ushort)(_receiversCount - _deletedReceiversCount);

                    var bit = 1L << inMaskBit;
                    long orig, newVal;
                    do {
                        orig = page.Mask[maskIdx];
                        newVal = orig | bit;
                    } while (Interlocked.CompareExchange(ref page.Mask[maskIdx], newVal, orig) != orig);
                    
                    #if FFS_ECS_DEBUG
                    Data.Instance.EventListener?.OnEventSent(new Event<T>((int) ((pageIdx << EVENT_PAGE_SHIFT) + inPageIdx)));
                    #endif
                    return true;
                }

                return false;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void CleanupEvent(int eventIdx) {
                if (eventIdx == -1) return;
                var pageIdx = (uint) (eventIdx >> EVENT_PAGE_SHIFT);
                var inPageIdx = (uint) (eventIdx & EVENT_PAGE_OFFSET_MASK);
                var maskIdx = (byte) (inPageIdx >> EVENT_IN_PAGE_MASK_SHIFT);
                var inMaskBit = (byte) (inPageIdx & EVENT_IN_PAGE_OFFSET_MASK);

                ref var page = ref _pages[pageIdx];
                ref var unreadCount = ref page.UnreadReceiversCount[inPageIdx];
                if (unreadCount != 0) {
                    unreadCount--;
                    if (unreadCount == 0) {
                        page.Mask[maskIdx] &= ~(1L << inMaskBit);
                        #if FFS_ECS_DEBUG
                        Data.Instance.EventListener?.OnEventReadAll(new Event<T>((int) ((pageIdx << EVENT_PAGE_SHIFT) + inPageIdx)));
                        #endif
                        if (_clearDataOnRead) {
                            page.EventsData[inPageIdx] = default;
                        }

                        if (inPageIdx == EVENT_PAGE_OFFSET_MASK) {
                            #if FFS_ECS_DEBUG
                            if (_freePagesCount + 1 >= _freePages.Length) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Events<{typeof(T)}> ] Active events buffer overflow, maximum {MAX_EVENTS} events");
                            #endif
                            var poolLockTaken = false;
                            _pagePoolLock.Enter(ref poolLockTaken);
                            #if FFS_ECS_DEBUG
                            if (!poolLockTaken) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Events<{typeof(T)}> ] Failed to acquire page pool lock");
                            #endif
                            page.Free(ref _freePages[_freePagesCount++]);
                            _pagePoolLock.Exit();
                        }
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal bool ReadOne(int receiverId, int previous, out int next) {
                ref var receiver = ref _receiverDatas[receiverId];

                CleanupEvent(previous);

                while (receiver.Sequence != (ulong) _sequence) {
                    next = (int) (receiver.Sequence++ & MAX_EVENTS_OFFSET_MASK);
                    var pageIdx = (uint) (next >> EVENT_PAGE_SHIFT);
                    var inPageIdx = (uint) (next & EVENT_PAGE_OFFSET_MASK);
                    var maskIdx = (byte) (inPageIdx >> EVENT_IN_PAGE_MASK_SHIFT);
                    var inMaskBit = (byte) (inPageIdx & EVENT_IN_PAGE_OFFSET_MASK);
                    if ((_pages[pageIdx].Mask[maskIdx] & (1L << inMaskBit)) != 0) {
                        return true;
                    }
                }

                next = -1;
                return false;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void ReadAll(int receiverId, EventAction<T> action) {
                ref var receiver = ref _receiverDatas[receiverId];
                var ev = new Event<T>();
                ref var next = ref ev.EventIdx;

                while (receiver.Sequence != (ulong) _sequence) {
                    next = (int) (receiver.Sequence++ & MAX_EVENTS_OFFSET_MASK);
                    var pageIdx = (uint) (next >> EVENT_PAGE_SHIFT);
                    var inPageIdx = (uint) (next & EVENT_PAGE_OFFSET_MASK);
                    var maskIdx = (byte) (inPageIdx >> EVENT_IN_PAGE_MASK_SHIFT);
                    var inMaskBit = (byte) (inPageIdx & EVENT_IN_PAGE_OFFSET_MASK);
                    ref var page = ref _pages[pageIdx];
                    var mask = page.Mask;

                    if ((mask[maskIdx] & (1L << inMaskBit)) != 0) {
                        action(ev);
                    }

                    ref var unreadCount = ref page.UnreadReceiversCount[inPageIdx];
                    if (unreadCount != 0) {
                        unreadCount--;
                        if (unreadCount == 0) {
                            mask[maskIdx] &= ~(1L << inMaskBit);
                            #if FFS_ECS_DEBUG
                            Data.Instance.EventListener?.OnEventReadAll(ev);
                            #endif
                            if (_clearDataOnRead) {
                                page.EventsData[inPageIdx] = default;
                            }

                            if (inPageIdx == EVENT_PAGE_OFFSET_MASK) {
                                #if FFS_ECS_DEBUG
                                if (_freePagesCount + 1 >= _freePages.Length) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Events<{typeof(T)}> ] Active events buffer overflow, maximum {MAX_EVENTS} events");
                                #endif
                                var poolLockTaken = false;
                                _pagePoolLock.Enter(ref poolLockTaken);
                                #if FFS_ECS_DEBUG
                                if (!poolLockTaken) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Events<{typeof(T)}> ] Failed to acquire page pool lock");
                                #endif
                                page.Free(ref _freePages[_freePagesCount++]);
                                _pagePoolLock.Exit();
                            }
                        }
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void ReadAll(int receiverId) {
                ref var receiver = ref _receiverDatas[receiverId];
                var ev = new Event<T>();
                ref var next = ref ev.EventIdx;

                while (receiver.Sequence != (ulong) _sequence) {
                    next = (int) (receiver.Sequence++ & MAX_EVENTS_OFFSET_MASK);
                    var pageIdx = (uint) (next >> EVENT_PAGE_SHIFT);
                    var inPageIdx = (uint) (next & EVENT_PAGE_OFFSET_MASK);
                    var maskIdx = (byte) (inPageIdx >> EVENT_IN_PAGE_MASK_SHIFT);
                    var inMaskBit = (byte) (inPageIdx & EVENT_IN_PAGE_OFFSET_MASK);
                    ref var page = ref _pages[pageIdx];
                    var mask = page.Mask;
                    ref var unreadCount = ref page.UnreadReceiversCount[inPageIdx];
                    if (unreadCount != 0) {
                        unreadCount--;
                        if (unreadCount == 0) {
                            mask[maskIdx] &= ~(1L << inMaskBit);
                            #if FFS_ECS_DEBUG
                            Data.Instance.EventListener?.OnEventReadAll(ev);
                            #endif
                            
                            if (_clearDataOnRead) {
                                page.EventsData[inPageIdx] = default;
                            }

                            if (inPageIdx == EVENT_PAGE_OFFSET_MASK) {
                                #if FFS_ECS_DEBUG
                                if (_freePagesCount + 1 >= _freePages.Length) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Events<{typeof(T)}> ] Active events buffer overflow, maximum {MAX_EVENTS} events");
                                #endif
                                var poolLockTaken = false;
                                _pagePoolLock.Enter(ref poolLockTaken);
                                #if FFS_ECS_DEBUG
                                if (!poolLockTaken) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Events<{typeof(T)}> ] Failed to acquire page pool lock");
                                #endif
                                page.Free(ref _freePages[_freePagesCount++]);
                                _pagePoolLock.Exit();
                            }
                        }
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void SuppressOne(int eventIdx) {
                var pageIdx = (uint) (eventIdx >> EVENT_PAGE_SHIFT);
                var inPageIdx = (uint) (eventIdx & EVENT_PAGE_OFFSET_MASK);
                var maskIdx = (byte) (inPageIdx >> EVENT_IN_PAGE_MASK_SHIFT);
                var inMaskBit = (byte) (inPageIdx & EVENT_IN_PAGE_OFFSET_MASK);

                ref var page = ref _pages[pageIdx];
                ref var unreadCount = ref page.UnreadReceiversCount[inPageIdx];
                if (unreadCount != 0) {
                    unreadCount = 0;
                    page.Mask[maskIdx] &= ~(1L << inMaskBit);
                    
                    #if FFS_ECS_DEBUG
                    Data.Instance.EventListener?.OnEventSuppress(new Event<T>((int) ((pageIdx << EVENT_PAGE_SHIFT) + inPageIdx)));
                    #endif
                    
                    if (_clearDataOnRead) {
                        page.EventsData[inPageIdx] = default;
                    }

                    if (inPageIdx == EVENT_PAGE_OFFSET_MASK) {
                        #if FFS_ECS_DEBUG
                        if (_freePagesCount + 1 >= _freePages.Length) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Events<{typeof(T)}> ] Active events buffer overflow, maximum {MAX_EVENTS} events");
                        #endif
                        var poolLockTaken = false;
                        _pagePoolLock.Enter(ref poolLockTaken);
                        #if FFS_ECS_DEBUG
                        if (!poolLockTaken) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Events<{typeof(T)}> ] Failed to acquire page pool lock");
                        #endif
                        page.Free(ref _freePages[_freePagesCount++]);
                        _pagePoolLock.Exit();
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void SuppressAll(int receiverId) {
                ref var receiver = ref _receiverDatas[receiverId];
                var ev = new Event<T>();
                ref var next = ref ev.EventIdx;

                while (receiver.Sequence != (ulong) _sequence) {
                    next = (int) (receiver.Sequence++ & MAX_EVENTS_OFFSET_MASK);
                    var pageIdx = (uint) (next >> EVENT_PAGE_SHIFT);
                    var inPageIdx = (uint) (next & EVENT_PAGE_OFFSET_MASK);
                    var maskIdx = (byte) (inPageIdx >> EVENT_IN_PAGE_MASK_SHIFT);
                    var inMaskBit = (byte) (inPageIdx & EVENT_IN_PAGE_OFFSET_MASK);
                    ref var page = ref _pages[pageIdx];
                    var mask = page.Mask;

                    ref var unreadCount = ref page.UnreadReceiversCount[inPageIdx];
                    if (unreadCount != 0) {
                        unreadCount = 0;
                        mask[maskIdx] &= ~(1L << inMaskBit);
                        
                        #if FFS_ECS_DEBUG
                        Data.Instance.EventListener?.OnEventSuppress(ev);
                        #endif
                        
                        if (_clearDataOnRead) {
                            page.EventsData[inPageIdx] = default;
                        }

                        if (inPageIdx == EVENT_PAGE_OFFSET_MASK) {
                            #if FFS_ECS_DEBUG
                            if (_freePagesCount + 1 >= _freePages.Length) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Events<{typeof(T)}> ] Active events buffer overflow, maximum {MAX_EVENTS} events");
                            #endif
                            var poolLockTaken = false;
                            _pagePoolLock.Enter(ref poolLockTaken);
                            #if FFS_ECS_DEBUG
                            if (!poolLockTaken) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.Events<{typeof(T)}> ] Failed to acquire page pool lock");
                            #endif
                            page.Free(ref _freePages[_freePagesCount++]);
                            _pagePoolLock.Exit();
                        }
                    }
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal readonly ushort EventVersion(int eventIdx) {
                var pageIdx = (uint) (eventIdx >> EVENT_PAGE_SHIFT);
                return _pages[pageIdx].Version;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly bool IsLastReading(int eventIdx) {
                var pageIdx = (uint) (eventIdx >> EVENT_PAGE_SHIFT);
                var inPageIdx = (uint) (eventIdx & EVENT_PAGE_OFFSET_MASK);
                return _pages[pageIdx].UnreadReceiversCount[inPageIdx] == 1;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly int UnreadCount(int eventIdx) {
                var pageIdx = (uint) (eventIdx >> EVENT_PAGE_SHIFT);
                var inPageIdx = (uint) (eventIdx & EVENT_PAGE_OFFSET_MASK);
                return _pages[pageIdx].UnreadReceiversCount[inPageIdx];
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly int Last() {
                if (_sequence == 0) return 0;
                var seq = (ulong) _sequence - 1;
                var pageIdx = (uint) ((seq >> EVENT_PAGE_SHIFT) & PAGES_OFFSET_MASK);
                var inPageIdx = (uint) (seq & EVENT_PAGE_OFFSET_MASK);

                return _pages[pageIdx].UnreadReceiversCount[inPageIdx];
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly int NotDeletedCount() {
                if (_receiversCount > _deletedReceiversCount) {
                    var seq = (ulong) _sequence;
                    var minSeq = seq;
                    for (var i = 0; i < _receiversCount; i++) {
                        ref var receiver = ref _receiverDatas[i];
                        if (!receiver.Deleted && receiver.Sequence < minSeq) {
                            minSeq = receiver.Sequence;
                        }
                    }

                    return (int) (seq - minSeq);
                }

                return 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly int Capacity() {
                return _maxPagesCount << EVENT_PAGE_SHIFT;
            }

            [MethodImpl(AggressiveInlining)]
            internal void Reset() {
                for (int i = 0; i < _pages.Length; i++) {
                    ref var page = ref _pages[i];
                    if (page.EventsData != null) {
                        Array.Clear(page.EventsData, 0, page.EventsData.Length);
                        Array.Clear(page.Mask, 0, page.Mask.Length);
                        Array.Clear(page.UnreadReceiversCount, 0, page.UnreadReceiversCount.Length);
                        page.Free(ref _freePages[_freePagesCount++]);
                    }

                    page.Version = 0;
                }

                for (int i = 0; i < _receiversCount; i++) {
                    ref var receiver = ref _receiverDatas[i];
                    receiver.Sequence = 0;
                    receiver.Deleted = false;
                }

                _receiversCount = 0;
                _deletedReceiversCount = 0;
                _sequence = 0;
            }

            #if FFS_ECS_DEBUG
            [MethodImpl(AggressiveInlining)]
            internal void AddBlocker(int val) {
                _blockers += val;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly bool IsBlocked() {
                return _blockers > 0;
            }
            #endif

            [MethodImpl(AggressiveInlining)]
            internal readonly void WriteAll(ref BinaryPackWriter writer, ref Events<T> pool) {
                var notEmpty = pool._receiversCount > pool._deletedReceiversCount;
                writer.WriteByte(Version);
                writer.WriteUlong((ulong) pool._sequence);
                writer.WriteBool(notEmpty);
                writer.WriteUshort((ushort)pool._receiverDatas.Length);
                writer.WriteArrayUnmanaged(pool._receiverDatas, 0, pool._receiversCount);
                writer.WriteUshort(pool._receiversCount);
                writer.WriteUshort(pool._deletedReceiversCount);

                if (notEmpty) {
                    var minSeq = (ulong) pool._sequence;
                    var maxSeq = (ulong) pool._sequence;
                    for (var i = 0; i < pool._receiversCount; i++) {
                        ref var receiver = ref pool._receiverDatas[i];
                        if (!receiver.Deleted && receiver.Sequence < minSeq) {
                            minSeq = receiver.Sequence;
                        }
                    }

                    var curPageIdx = (uint)((minSeq >> EVENT_PAGE_SHIFT) & PAGES_OFFSET_MASK);
                    var maxPageIdx = (uint)((maxSeq >> EVENT_PAGE_SHIFT) & PAGES_OFFSET_MASK);
                    var maxInPageIdx = (uint)(maxSeq & EVENT_PAGE_OFFSET_MASK);

                    var isUnmanaged = _readWriteArrayStrategy.IsUnmanaged();
                    writer.WriteBool(isUnmanaged);
                    ushort count = 0;
                    var offset = writer.MakePoint(sizeof(ushort));
                    while (true) {
                        if (curPageIdx == maxPageIdx && maxInPageIdx == 0) {
                            break;
                        }

                        ref var page = ref pool._pages[curPageIdx];
                        writer.WriteUint(curPageIdx);
                        writer.WriteUshort(page.Version);
                        writer.WriteArrayUnmanaged(page.Mask);
                        writer.WriteArrayUnmanaged(page.UnreadReceiversCount);
                        if (isUnmanaged) {
                            _readWriteArrayStrategy.WriteArray(ref writer, page.EventsData);
                        }
                        else {
                            for (var eIdx = 0; eIdx < EVENTS_PER_PAGE; eIdx++) {
                                if ((page.Mask[eIdx >> EVENT_IN_PAGE_MASK_SHIFT] & (1L << (eIdx & EVENT_IN_PAGE_OFFSET_MASK))) != 0) {
                                    writer.Write(in page.EventsData[eIdx]);
                                }
                            }
                        }

                        count++;

                        if (curPageIdx == maxPageIdx) {
                            break;
                        }

                        curPageIdx = ((curPageIdx + 1) & PAGES_OFFSET_MASK);
                    }

                    writer.WriteUshortAt(offset, count);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void ReadAll(ref BinaryPackReader reader, ref Events<T> pool) {
                var oldVersion = reader.ReadByte();
                pool._sequence = (long) reader.ReadUlong();
                var notEmpty = reader.ReadBool();
                var len = reader.ReadUshort();
                if (len > pool._receiverDatas.Length) {
                    Array.Resize(ref pool._receiverDatas, len);
                }

                reader.ReadArrayUnmanaged(ref pool._receiverDatas);
                pool._receiversCount = reader.ReadUshort();
                pool._deletedReceiversCount = reader.ReadUshort();

                if (notEmpty) {
                    var isUnmanaged = reader.ReadBool();
                    var count = reader.ReadUshort();
                    for (var i = 0; i < count; i++) {
                        var pageIdx = reader.ReadUint();
                        ref var page = ref pool._pages[pageIdx];
                        page.Version = reader.ReadUshort();

                        if (pool._freePagesCount > 0) {
                            page.FromFree(ref pool._freePages[--pool._freePagesCount]);
                        }
                        else {
                            page.InitNew();
                            pool._maxPagesCount++;
                        }

                        reader.ReadArrayUnmanaged(ref page.Mask);
                        reader.ReadArrayUnmanaged(ref page.UnreadReceiversCount);
                        if (Version == oldVersion) {
                            if (isUnmanaged) {
                                _readWriteArrayStrategy.ReadArray(ref reader, ref page.EventsData);
                            }
                            else {
                                for (var eIdx = 0; eIdx < EVENTS_PER_PAGE; eIdx++) {
                                    if ((page.Mask[eIdx >> EVENT_IN_PAGE_MASK_SHIFT] & (1L << (eIdx & EVENT_IN_PAGE_OFFSET_MASK))) != 0) {
                                        page.EventsData[eIdx].Read(ref reader, oldVersion);
                                    }
                                }
                            }
                        }
                        else {
                            uint oneSize = default;
                            if (isUnmanaged) {
                                _ = reader.ReadNullFlag();
                                var size = reader.ReadInt();
                                var byteSize = reader.ReadUint();
                                oneSize = (uint)(byteSize / size);
                            }

                            for (var eIdx = 0; eIdx < EVENTS_PER_PAGE; eIdx++) {
                                if ((page.Mask[eIdx >> EVENT_IN_PAGE_MASK_SHIFT] & (1L << (eIdx & EVENT_IN_PAGE_OFFSET_MASK))) != 0) {
                                    page.EventsData[eIdx].Read(ref reader, oldVersion);
                                }
                                else if (isUnmanaged) {
                                    reader.SkipNext(oneSize);
                                }
                            }
                        }
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal static IEvent _GetRaw(int idx) => Instance.Get(idx);

            [MethodImpl(AggressiveInlining)]
            internal static void _Del(int idx) => Instance.SuppressOne(idx);

            [MethodImpl(AggressiveInlining)]
            internal static void _Destroy() => Instance.DestroyEvents();

            [MethodImpl(AggressiveInlining)]
            internal static bool _AddRaw(IEvent value) => Instance.Add((T) value);

            [MethodImpl(AggressiveInlining)]
            internal static bool _Add() => Instance.Add();

            [MethodImpl(AggressiveInlining)]
            internal static bool _IsDeleted(int idx) => Instance.UnreadCount(idx) == 0;

            [MethodImpl(AggressiveInlining)]
            internal static int _UnreadCount(int idx) => Instance.UnreadCount(idx);

            [MethodImpl(AggressiveInlining)]
            internal static int _NotDeletedCount() => Instance.NotDeletedCount();

            [MethodImpl(AggressiveInlining)]
            internal static int _Capacity() => Instance.Capacity();

            [MethodImpl(AggressiveInlining)]
            internal static int _ReceiversCount() => Instance._receiversCount - Instance._deletedReceiversCount;

            [MethodImpl(AggressiveInlining)]
            internal static int _Last() => Instance.Last();

            [MethodImpl(AggressiveInlining)]
            internal static ushort _Version(int idx) => Instance.EventVersion(idx);

            [MethodImpl(AggressiveInlining)]
            internal static void _PutRaw(int idx, IEvent value) => Instance.Get(idx) = (T) value;

            [MethodImpl(AggressiveInlining)]
            internal static void _Reset() => Instance.Reset();

            [MethodImpl(AggressiveInlining)]
            internal static void _WriteAll(ref BinaryPackWriter writer) => Instance.WriteAll(ref writer, ref Instance);

            [MethodImpl(AggressiveInlining)]
            internal static void _ReadAll(ref BinaryPackReader reader) => Instance.ReadAll(ref reader, ref Instance);
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        /// <summary>
        /// Zero-allocation enumerator for consuming events from an <see cref="EventReceiver{TWorld, TEvent}"/>
        /// via <c>foreach</c>.
        /// <para>
        /// Each call to <see cref="MoveNext"/> advances to the next unread event and marks the
        /// previous event as consumed by this receiver. When the enumerator is disposed (at the end
        /// of <c>foreach</c> or on early <c>break</c>), the last consumed event is properly cleaned up.
        /// </para>
        /// <para>
        /// During iteration the event pool is locked in read-only mode (debug builds only) —
        /// sending new events or modifying receivers will throw.
        /// </para>
        /// </summary>
        /// <typeparam name="TEvent">The event struct type.</typeparam>
        public ref struct EventIterator<TEvent> where TEvent : struct, IEvent {
            private Event<TEvent> _current;
            private readonly int _id;

            [MethodImpl(AggressiveInlining)]
            internal EventIterator(int id) {
                _id = id;
                _current = new Event<TEvent>(-1);
                #if FFS_ECS_DEBUG
                Events<TEvent>.Instance.AddBlocker(1);
                #endif
            }

            /// <summary>
            /// Gets the <see cref="Event{TEvent}"/> wrapper for the event at the current enumerator position.
            /// </summary>
            public Event<TEvent> Current {
                [MethodImpl(AggressiveInlining)] get => _current;
            }

            /// <summary>
            /// Advances to the next unread event. The previous event is marked as consumed by this receiver.
            /// Returns <c>false</c> when all events have been consumed.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public bool MoveNext() => Events<TEvent>.Instance.ReadOne(_id, _current.EventIdx, out _current.EventIdx);

            /// <summary>
            /// Cleans up the last consumed event and releases the read-only lock (debug builds).
            /// Called automatically at the end of <c>foreach</c> or on early <c>break</c>.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void Dispose() {
                Events<TEvent>.Instance.CleanupEvent(_current.EventIdx);
                #if FFS_ECS_DEBUG
                Events<TEvent>.Instance.AddBlocker(-1);
                #endif
            }
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    /// <summary>
    /// Handle for consuming events of type <typeparamref name="TEvent"/> from
    /// <c>World&lt;<typeparamref name="TWorld"/>&gt;</c>.
    /// <para>
    /// Each receiver maintains an independent read cursor (sequence number) into the shared ring
    /// buffer. Multiple receivers can consume the same event stream at their own pace.
    /// An event's data is released only after <em>every</em> active receiver has consumed it.
    /// </para>
    /// <para>
    /// Create via <see cref="World{TWorld}.RegisterEventReceiver{TEvent}"/>,
    /// delete via <see cref="World{TWorld}.DeleteEventReceiver{TEvent}"/>.
    /// </para>
    /// <para>
    /// Supports <c>foreach</c> iteration — <see cref="GetEnumerator"/> returns a zero-allocation
    /// <see cref="World{TWorld}.EventIterator{TEvent}"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TWorld">The world type.</typeparam>
    /// <typeparam name="TEvent">The event struct type.</typeparam>
    public struct EventReceiver<TWorld, TEvent>
        where TEvent : struct, IEvent
        where TWorld : struct, IWorldType {

        internal int Id;

        [MethodImpl(AggressiveInlining)]
        internal EventReceiver(int id) => Id = id;

        /// <summary>
        /// Reads all unread events, invoking <paramref name="action"/> for each one, and marks them
        /// as consumed by this receiver.
        /// <para>
        /// Events are delivered in FIFO order. Each event is marked as consumed immediately
        /// after the delegate returns. If all receivers have consumed an event, its data is released.
        /// </para>
        /// </summary>
        /// <param name="action">
        /// Delegate called for each unread event. Receives an <see cref="World{TWorld}.Event{TEvent}"/>
        /// wrapper providing access to <see cref="World{TWorld}.Event{TEvent}.Value"/>.
        /// </param>
        /// <exception cref="StaticEcsException">Thrown in debug builds if the receiver has been deleted or called from a multithreaded context.</exception>
        [MethodImpl(AggressiveInlining)]
        public void ReadAll(World<TWorld>.EventAction<TEvent> action) {
            #if FFS_ECS_DEBUG
            if (Id < 0) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.EventReceiver<{typeof(TEvent)}>.ReadAll ] receiver is deleted");
            if (World<TWorld>.Data.Instance.MultiThreadActive) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.EventReceiver<{typeof(TEvent)}>.ReadAll ] this operation is not supported in multithreaded mode");
            #endif
            World<TWorld>.Events<TEvent>.Instance.ReadAll(Id, action);
        }

        /// <summary>
        /// Advances this receiver's read cursor past all unread events without processing them.
        /// Events are marked as consumed by this receiver; their data is released if all
        /// receivers have now consumed them.
        /// <para>
        /// Use this to skip events that are no longer relevant (e.g., after a state reset)
        /// without iterating through each one.
        /// </para>
        /// </summary>
        /// <exception cref="StaticEcsException">Thrown in debug builds if the receiver has been deleted or called from a multithreaded context.</exception>
        [MethodImpl(AggressiveInlining)]
        public void MarkAsReadAll() {
            #if FFS_ECS_DEBUG
            if (Id < 0) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.EventReceiver<{typeof(TEvent)}>.MarkAsReadAll ] receiver is deleted");
            if (World<TWorld>.Data.Instance.MultiThreadActive) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.EventReceiver<{typeof(TEvent)}>.MarkAsReadAl ] this operation is not supported in multithreaded mode");
            #endif
            World<TWorld>.Events<TEvent>.Instance.ReadAll(Id);
        }

        /// <summary>
        /// Suppresses (cancels) all unread events for <b>all</b> receivers, not just this one.
        /// <para>
        /// Each unread event visible to this receiver is forcibly removed from the ring buffer:
        /// its mask bit is cleared, its unread counter is zeroed, and its data is released.
        /// Other receivers that have not yet read these events will never see them.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <b>Warning:</b> this is a destructive global operation. One receiver can silently
        /// remove events from every other receiver's queue. Use only when you need to cancel
        /// events globally (e.g., invalidating a batch of obsolete notifications).
        /// </remarks>
        /// <exception cref="StaticEcsException">Thrown in debug builds if the receiver has been deleted or called from a multithreaded context.</exception>
        [MethodImpl(AggressiveInlining)]
        public void SuppressAll() {
            #if FFS_ECS_DEBUG
            if (Id < 0) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.EventReceiver<{typeof(TEvent)}>.SuppressAll ] receiver is deleted");
            if (World<TWorld>.Data.Instance.MultiThreadActive) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.EventReceiver<{typeof(TEvent)}>.SuppressAll ] this operation is not supported in multithreaded mode");
            #endif
            World<TWorld>.Events<TEvent>.Instance.SuppressAll(Id);
        }

        /// <summary>
        /// Returns a zero-allocation enumerator for consuming events via <c>foreach</c>.
        /// <para>
        /// Each iteration yields an <see cref="World{TWorld}.Event{TEvent}"/> wrapper. Events are
        /// consumed in FIFO order and marked as read when the enumerator advances past them.
        /// Early <c>break</c> is safe — only the events actually iterated are marked as consumed;
        /// remaining events stay in the buffer for the next read.
        /// </para>
        /// </summary>
        /// <returns>An <see cref="World{TWorld}.EventIterator{TEvent}"/> for <c>foreach</c> use.</returns>
        /// <exception cref="StaticEcsException">Thrown in debug builds if the receiver has been deleted, called from a multithreaded context, or the pool is already being iterated.</exception>
        [MethodImpl(AggressiveInlining)]
        public World<TWorld>.EventIterator<TEvent> GetEnumerator() {
            #if FFS_ECS_DEBUG
            if (Id < 0) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.EventReceiver<{typeof(TEvent)}>.GetEnumerator ] receiver is deleted");
            if (World<TWorld>.Data.Instance.MultiThreadActive) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.EventReceiver<{typeof(TEvent)}>.GetEnumerator ] this operation is not supported in multithreaded mode");
            if (World<TWorld>.Events<TEvent>.Instance.IsBlocked()) throw new StaticEcsException($"[ World<{typeof(TWorld)}>.EventReceiver<{typeof(TEvent)}>.GetEnumerator ] event pool is blocked");
            #endif
            return new World<TWorld>.EventIterator<TEvent>(Id);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    /// <summary>
    /// Extension methods for binary serialization and deserialization of
    /// <see cref="EventReceiver{TWorld, TEvent}"/> handles.
    /// <para>
    /// Only the receiver's internal ID is serialized — the receiver must already exist in the
    /// target world with the same ID for deserialization to produce a valid handle.
    /// </para>
    /// </summary>
    public static class EventReceiverSerializer {
        /// <summary>
        /// Writes the receiver's internal ID to the binary stream.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static void WriteEventReceiver<TWorld, TEvent>(this ref BinaryPackWriter writer, in EventReceiver<TWorld, TEvent> value)
            where TWorld : struct, IWorldType
            where TEvent : struct, IEvent {

            writer.WriteInt(value.Id);
        }

        /// <summary>
        /// Reads a receiver ID from the binary stream and wraps it in an
        /// <see cref="EventReceiver{TWorld, TEvent}"/> handle.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static EventReceiver<TWorld, TEvent> ReadEventReceiver<TWorld, TEvent>(this ref BinaryPackReader reader)
            where TWorld : struct, IWorldType
            where TEvent : struct, IEvent {

            return new EventReceiver<TWorld, TEvent>(reader.ReadInt());
        }
    }

    /// <summary>
    /// Delegate used during event type migration to read and discard serialized event data
    /// when the event type has been removed from the world.
    /// </summary>
    /// <param name="reader">Binary reader positioned at the start of a single event's data.</param>
    /// <param name="version">The schema version that was active when the event was serialized.</param>
    public delegate void EcsEventDeleteMigrationReader(ref BinaryPackReader reader, byte version);

    internal static class EventSerializerUtils {
        [MethodImpl(AggressiveInlining)]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        internal static void DeleteAllEventMigration<TWorld>(this ref BinaryPackReader reader, EcsEventDeleteMigrationReader migration)
            where TWorld : struct, IWorldType {
            var oldVersion = reader.ReadByte();
            var sequence = reader.ReadUlong();
            var notEmpty = reader.ReadBool();
            var len = reader.ReadUshort();

            reader.ReadArrayUnmanagedPooled<ReceiverData>(out var handle);
            handle.Return();
            var receiversCount = reader.ReadUshort();
            var deletedReceiversCount = reader.ReadUshort();

            if (notEmpty) {
                var isUnmanaged = reader.ReadBool();
                var count = reader.ReadUshort();
                for (var i = 0; i < count; i++) {
                    var pageIdx = reader.ReadUint();
                    var version = reader.ReadUshort();

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

                    for (var eIdx = 0; eIdx < World<TWorld>.EVENTS_PER_PAGE; eIdx++) {
                        if ((mask[eIdx >> World<TWorld>.EVENT_IN_PAGE_MASK_SHIFT] & (1Ul << (eIdx & World<TWorld>.EVENT_IN_PAGE_OFFSET_MASK))) != 0) {
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
