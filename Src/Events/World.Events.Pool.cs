#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

using System;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public interface IEventPoolWrapper {
        internal Guid Guid();

        internal ushort DynamicId();

        internal Type GetEventType();
        
        internal IEvent GetRaw(int idx);

        internal void Del(int idx);
        
        internal void Destroy();
        
        public bool AddRaw(IEvent value);
        
        public bool Add();

        internal bool IsDeleted(int idx);

        internal int UnreadCount(int idx);

        internal int NotDeletedCount();

        internal int Capacity();

        internal int ReceiversCount();

        internal int Last();

        internal ushort Version(int idx);

        internal void PutRaw(int idx, IEvent value);

        internal void Clear();
        
        internal void WriteAll(ref BinaryPackWriter writer);

        internal void ReadAll(ref BinaryPackReader reader);
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct EventPoolWrapper<WorldType, T> : IEventPoolWrapper where T : struct, IEvent where WorldType : struct, IWorldType {
        [MethodImpl(AggressiveInlining)]
        internal ref T Get(int idx) => ref World<WorldType>.Events.Pool<T>.Value.Get(idx);

        [MethodImpl(AggressiveInlining)]
        public bool Add(T value) => World<WorldType>.Events.Pool<T>.Value.Add(value);

        [MethodImpl(AggressiveInlining)]
        Guid IEventPoolWrapper.Guid() => World<WorldType>.Events.Pool<T>.Serializer.Value.guid;

        [MethodImpl(AggressiveInlining)]
        ushort IEventPoolWrapper.DynamicId() => World<WorldType>.Events.Pool<T>.Value.Id;

        [MethodImpl(AggressiveInlining)]
        Type IEventPoolWrapper.GetEventType() => typeof(T);

        [MethodImpl(AggressiveInlining)]
        IEvent IEventPoolWrapper.GetRaw(int idx) => World<WorldType>.Events.Pool<T>.Value.Get(idx);

        [MethodImpl(AggressiveInlining)]
        void IEventPoolWrapper.Destroy() => World<WorldType>.Events.Pool<T>.Value.Destroy();

        [MethodImpl(AggressiveInlining)]
        public bool AddRaw(IEvent value) => World<WorldType>.Events.Pool<T>.Value.Add((T) value);

        [MethodImpl(AggressiveInlining)]
        void IEventPoolWrapper.PutRaw(int idx, IEvent value) => World<WorldType>.Events.Pool<T>.Value.Get(idx) = (T) value;

        [MethodImpl(AggressiveInlining)]
        void IEventPoolWrapper.Clear() => World<WorldType>.Events.Pool<T>.Value.Clear();

        [MethodImpl(AggressiveInlining)]
        void IEventPoolWrapper.WriteAll(ref BinaryPackWriter writer) => World<WorldType>.Events.Pool<T>.Serializer.Value.WriteAll(ref writer, ref World<WorldType>.Events.Pool<T>.Value);

        [MethodImpl(AggressiveInlining)]
        void IEventPoolWrapper.ReadAll(ref BinaryPackReader reader) => World<WorldType>.Events.Pool<T>.Serializer.Value.ReadAll(ref reader, ref World<WorldType>.Events.Pool<T>.Value);

        [MethodImpl(AggressiveInlining)]
        public bool Add() => World<WorldType>.Events.Pool<T>.Value.Add();

        [MethodImpl(AggressiveInlining)]
        void IEventPoolWrapper.Del(int idx) => World<WorldType>.Events.Pool<T>.Value.SuppressOne(idx);

        [MethodImpl(AggressiveInlining)]
        ushort IEventPoolWrapper.Version(int idx) => World<WorldType>.Events.Pool<T>.Value.Version(idx);

        [MethodImpl(AggressiveInlining)]
        bool IEventPoolWrapper.IsDeleted(int idx) => World<WorldType>.Events.Pool<T>.Value.UnreadCount(idx) == 0;

        [MethodImpl(AggressiveInlining)]
        int IEventPoolWrapper.UnreadCount(int idx) => World<WorldType>.Events.Pool<T>.Value.UnreadCount(idx);

        [MethodImpl(AggressiveInlining)]
        int IEventPoolWrapper.Last() => World<WorldType>.Events.Pool<T>.Value.Last();

        [MethodImpl(AggressiveInlining)]
        int IEventPoolWrapper.NotDeletedCount() => World<WorldType>.Events.Pool<T>.Value.NotDeletedCount();

        [MethodImpl(AggressiveInlining)]
        int IEventPoolWrapper.Capacity() => World<WorldType>.Events.Pool<T>.Value.Capacity();

        [MethodImpl(AggressiveInlining)]
        int IEventPoolWrapper.ReceiversCount() => World<WorldType>.Events.Pool<T>.Value.receiversCount - World<WorldType>.Events.Pool<T>.Value.deletedReceiversCount;
    }
    
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct ReceiverData {
        internal ulong Sequence;
        internal bool Deleted;
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        
        public delegate void EventAction<T>(Event<T> eventValue) where T : struct, IEvent;
        
        public abstract partial class Events {
            
            internal const int MAX_PAGES = 256;
            internal const int PAGES_OFFSET_MASK = MAX_PAGES - 1;
            internal const int EVENTS_PER_PAGE = 512;
            internal const int EVENT_PAGE_SHIFT = 9;
            internal const int EVENT_PAGE_OFFSET_MASK = EVENTS_PER_PAGE - 1;
            internal const int MASKS_IN_PAGE = EVENTS_PER_PAGE / Const.BITS_PER_LONG;
            internal const int EVENT_IN_PAGE_MASK_SHIFT = Const.LONG_SHIFT;
            internal const int EVENT_IN_PAGE_OFFSET_MASK = Const.LONG_OFFSET_MASK;
            internal const int MAX_EVENTS = MAX_PAGES * EVENTS_PER_PAGE;
            internal const int MAX_EVENTS_OFFSET_MASK = MAX_EVENTS - 1;

            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            [Il2CppEagerStaticClassConstruction]
            #endif
            internal partial struct Pool<T> where T : struct, IEvent {
                internal static Pool<T> Value;
                
                #if ENABLE_IL2CPP
                [Il2CppSetOption(Option.NullChecks, false)]
                [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
                #endif
                internal struct Page {
                    internal T[] Data;
                    internal ulong[] Mask;
                    internal ushort[] UnreadReceiversCount;
                    internal ushort Version;

                    [MethodImpl(AggressiveInlining)]
                    public void Free(ref FreePage freePage) {
                        freePage.Data = Data;
                        freePage.Mask = Mask;
                        freePage.UnreadReceiversCount = UnreadReceiversCount;
                        Data = null;
                        Mask = null;
                        UnreadReceiversCount = null;
                        Version++;
                    }
                    
                    [MethodImpl(AggressiveInlining)]
                    public void FromFree(ref FreePage freePage) {
                        Data = freePage.Data;
                        Mask = freePage.Mask;
                        UnreadReceiversCount = freePage.UnreadReceiversCount;
                        freePage = default;
                    }
                    
                    [MethodImpl(AggressiveInlining)]
                    public void InitNew() {
                        Data = new T[EVENTS_PER_PAGE];
                        Mask = new ulong[MASKS_IN_PAGE];
                        UnreadReceiversCount = new ushort[EVENTS_PER_PAGE];
                    }
                }
                
                #if ENABLE_IL2CPP
                [Il2CppSetOption(Option.NullChecks, false)]
                [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
                #endif
                internal struct FreePage {
                    internal T[] Data;
                    internal ulong[] Mask;
                    internal ushort[] UnreadReceiversCount;
                }
                
                internal Page[] pages;
                internal FreePage[] freePages;
                internal ReceiverData[] receivers;
                
                internal ulong sequence;
                internal ushort maxPagesCount;
                internal ushort freePagesCount;
                internal ushort receiversCount;
                internal ushort deletedReceiversCount;
                internal ushort Id;
                internal bool clearable;
                internal bool initialized;
                
                #if FFS_ECS_DEBUG
                private int _blockers;
                #endif

                [MethodImpl(AggressiveInlining)]
                internal void Create(ushort id, IEventConfig<T, WorldType> config) {
                    Id = id;
                    pages = new Page[MAX_PAGES];
                    freePages = new FreePage[MAX_PAGES];
                    receivers = new ReceiverData[32];

                    maxPagesCount = 0;
                    freePagesCount = 0;
                    receiversCount = 0;
                    deletedReceiversCount = 0;
                    sequence = 0;
                    clearable = config.IsClearable();
                    Serializer.Value.Create(config);
                    initialized = true;
                }

                [MethodImpl(AggressiveInlining)]
                internal EventReceiver<WorldType, T> CreateReceiver() {
                    #if FFS_ECS_DEBUG
                    if (_blockers > 0) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.Events.Pool<{typeof(T)}>.CreateReceiver ] event pool cannot be changed, it is in read-only mode");
                    #endif
                    if (deletedReceiversCount > 0) {
                        for (int i = 0; i < receiversCount; i++) {
                            ref var receiver = ref receivers[i];
                            if (receiver.Deleted) {
                                deletedReceiversCount--;
                                receiver.Deleted = false;
                                receiver.Sequence = sequence;
                                return new EventReceiver<WorldType, T>(i);
                            }
                        }
                    }

                    if (receiversCount == receivers.Length) {
                        Array.Resize(ref receivers, receiversCount << 1);
                    }

                    receivers[receiversCount].Sequence = sequence;
                    return new EventReceiver<WorldType, T>(receiversCount++);
                }
                
                [MethodImpl(AggressiveInlining)]
                internal void DeleteReceiver(ref EventReceiver<WorldType, T> receiver) {
                    #if FFS_ECS_DEBUG
                    if (_blockers > 0) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.Events.Pool<{typeof(T)}>.DeleteReceiver ] event pool cannot be changed, it is in read-only mode");
                    #endif
                    ref var receiverData = ref receivers[receiver._id];
                    if (!receiverData.Deleted) {
                        ReadAll(receiver._id);
                        deletedReceiversCount++;
                        receiverData.Deleted = true;
                        receiver._id = -1;
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void Destroy() {
                    initialized = false;
                    Serializer.Value.Destroy();
                    freePages = default;
                    pages = default;
                    maxPagesCount = default;
                    freePagesCount = default;
                    receiversCount = default;
                    sequence = default;
                    clearable = default;
                    Id = default;
                }
                
                [MethodImpl(AggressiveInlining)]
                internal ref T Get(int idx) {
                    return ref pages[idx >> EVENT_PAGE_SHIFT].Data[idx & EVENT_PAGE_OFFSET_MASK];
                }

                [MethodImpl(AggressiveInlining)]
                internal bool Add(T value = default) {
                    #if FFS_ECS_DEBUG
                    if (_blockers > 0) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.Events.Pool<{typeof(T)}>.Add ] event pool cannot be changed, it is in read-only mode");
                    #endif
                    if (receiversCount > deletedReceiversCount) {
                        var pageIdx = (uint) ((sequence >> EVENT_PAGE_SHIFT) & PAGES_OFFSET_MASK);
                        var inPageIdx = (uint) (sequence & EVENT_PAGE_OFFSET_MASK);
                        var maskIdx = (byte) (inPageIdx >> EVENT_IN_PAGE_MASK_SHIFT);
                        var inMaskBit = (byte) (inPageIdx & EVENT_IN_PAGE_OFFSET_MASK);
                        
                        ref var page = ref pages[pageIdx];
                        if (page.Data == null) {
                            if (freePagesCount > 0) {
                                page.FromFree(ref freePages[--freePagesCount]);
                            } else {
                                page.InitNew();
                                maxPagesCount++;
                            }
                        }

                        page.Data[inPageIdx] = value;
                        page.Mask[maskIdx] |= 1UL << inMaskBit;
                        page.UnreadReceiversCount[inPageIdx] = receiversCount;
                        sequence++;

                        #if FFS_ECS_EVENTS
                        if (_debugEventListeners != null) {
                            foreach (var listener in _debugEventListeners) {
                                listener.OnEventSent(new Event<T>((int) ((pageIdx << EVENT_PAGE_SHIFT) + inPageIdx)));
                            }
                        }
                        #endif
                        return true;
                    }

                    return false;
                }
                
                [MethodImpl(AggressiveInlining)]
                internal bool ReadOne(int receiverId, int previous, out int next) {
                    ref var receiver = ref receivers[receiverId];

                    if (previous != -1) {
                        var pageIdx = (uint) (previous >> EVENT_PAGE_SHIFT);
                        var inPageIdx = (uint) (previous & EVENT_PAGE_OFFSET_MASK);
                        var maskIdx = (byte) (inPageIdx >> EVENT_IN_PAGE_MASK_SHIFT);
                        var inMaskBit = (byte) (inPageIdx & EVENT_IN_PAGE_OFFSET_MASK);
                        
                        ref var page = ref pages[pageIdx];
                        ref var unreadCount = ref page.UnreadReceiversCount[inPageIdx];
                        if (unreadCount != 0) {
                            unreadCount--;
                            if (unreadCount == 0) {
                                page.Mask[maskIdx] &= ~(1UL << inMaskBit);
                                #if FFS_ECS_EVENTS
                                if (_debugEventListeners != null) {
                                    foreach (var listener in _debugEventListeners) {
                                        listener.OnEventReadAll(new Event<T>(previous));
                                    }
                                }
                                #endif
                                if (clearable) {
                                    page.Data[inPageIdx] = default;
                                }

                                if (inPageIdx == EVENT_PAGE_OFFSET_MASK) {
                                    #if FFS_ECS_DEBUG
                                    if (freePagesCount + 1 >= freePages.Length) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.Events<{typeof(T)}> ] Active events buffer overflow, maximum {MAX_EVENTS} events");
                                    #endif
                                    page.Free(ref freePages[freePagesCount++]);
                                }
                            }
                        }
                    }

                    while (receiver.Sequence != sequence) {
                        next = (int) (receiver.Sequence++ & MAX_EVENTS_OFFSET_MASK);
                        var pageIdx = (uint) (next >> EVENT_PAGE_SHIFT);
                        var inPageIdx = (uint) (next & EVENT_PAGE_OFFSET_MASK);
                        var maskIdx = (byte) (inPageIdx >> EVENT_IN_PAGE_MASK_SHIFT);
                        var inMaskBit = (byte) (inPageIdx & EVENT_IN_PAGE_OFFSET_MASK);
                        if ((pages[pageIdx].Mask[maskIdx] & (1UL << inMaskBit)) != 0) {
                            return true;
                        }
                    }

                    next = -1;
                    return false;
                }
                
                [MethodImpl(AggressiveInlining)]
                internal void ReadAll(int receiverId, EventAction<T> action) {
                    ref var receiver = ref receivers[receiverId];
                    var ev = new Event<T>();
                    ref var next = ref ev._eventIdx;

                    while (receiver.Sequence != sequence) {
                        next = (int) (receiver.Sequence++ & MAX_EVENTS_OFFSET_MASK);
                        var pageIdx = (uint) (next >> EVENT_PAGE_SHIFT);
                        var inPageIdx = (uint) (next & EVENT_PAGE_OFFSET_MASK);
                        var maskIdx = (byte) (inPageIdx >> EVENT_IN_PAGE_MASK_SHIFT);
                        var inMaskBit = (byte) (inPageIdx & EVENT_IN_PAGE_OFFSET_MASK);
                        ref var page = ref pages[pageIdx];
                        var mask = page.Mask;
                        
                        if ((mask[maskIdx] & (1UL << inMaskBit)) != 0) {
                            action(ev);
                        }

                        ref var unreadCount = ref page.UnreadReceiversCount[inPageIdx];
                        if (unreadCount != 0) {
                            unreadCount--;
                            if (unreadCount == 0) {
                                mask[maskIdx] &= ~(1UL << inMaskBit);
                                #if FFS_ECS_EVENTS
                                if (_debugEventListeners != null) {
                                    foreach (var listener in _debugEventListeners) {
                                        listener.OnEventReadAll(ev);
                                    }
                                }
                                #endif
                                if (clearable) {
                                    page.Data[inPageIdx] = default;
                                }

                                if (inPageIdx == EVENT_PAGE_OFFSET_MASK) {
                                    #if FFS_ECS_DEBUG
                                    if (freePagesCount + 1 >= freePages.Length) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.Events<{typeof(T)}> ] Active events buffer overflow, maximum {MAX_EVENTS} events");
                                    #endif
                                    page.Free(ref freePages[freePagesCount++]);
                                }
                            }
                        }
                    }
                }
                
                [MethodImpl(AggressiveInlining)]
                internal void ReadAll(int receiverId) {
                    ref var receiver = ref receivers[receiverId];
                    var ev = new Event<T>();
                    ref var next = ref ev._eventIdx;

                    while (receiver.Sequence != sequence) {
                        next = (int) (receiver.Sequence++ & MAX_EVENTS_OFFSET_MASK);
                        var pageIdx = (uint) (next >> EVENT_PAGE_SHIFT);
                        var inPageIdx = (uint) (next & EVENT_PAGE_OFFSET_MASK);
                        var maskIdx = (byte) (inPageIdx >> EVENT_IN_PAGE_MASK_SHIFT);
                        var inMaskBit = (byte) (inPageIdx & EVENT_IN_PAGE_OFFSET_MASK);
                        ref var page = ref pages[pageIdx];
                        var mask = page.Mask;
                        ref var unreadCount = ref page.UnreadReceiversCount[inPageIdx];
                        if (unreadCount != 0) {
                            unreadCount--;
                            if (unreadCount == 0) {
                                mask[maskIdx] &= ~(1UL << inMaskBit);
                                #if FFS_ECS_EVENTS
                                if (_debugEventListeners != null) {
                                    foreach (var listener in _debugEventListeners) {
                                        listener.OnEventReadAll(ev);
                                    }
                                }
                                #endif
                                if (clearable) {
                                    page.Data[inPageIdx] = default;
                                }

                                if (inPageIdx == EVENT_PAGE_OFFSET_MASK) {
                                    #if FFS_ECS_DEBUG
                                    if (freePagesCount + 1 >= freePages.Length) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.Events<{typeof(T)}> ] Active events buffer overflow, maximum {MAX_EVENTS} events");
                                    #endif
                                    page.Free(ref freePages[freePagesCount++]);
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
                        
                    ref var page = ref pages[pageIdx];
                    ref var unreadCount = ref page.UnreadReceiversCount[inPageIdx];
                    if (unreadCount != 0) {
                        unreadCount = 0;
                        page.Mask[maskIdx] &= ~(1UL << inMaskBit);
                        #if FFS_ECS_EVENTS
                        if (_debugEventListeners != null) {
                            foreach (var listener in _debugEventListeners) {
                                listener.OnEventSuppress(new Event<T>(eventIdx));
                            }
                        }
                        #endif
                        if (clearable) {
                            page.Data[inPageIdx] = default;
                        }

                        if (inPageIdx == EVENT_PAGE_OFFSET_MASK) {
                            #if FFS_ECS_DEBUG
                            if (freePagesCount + 1 >= freePages.Length) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.Events<{typeof(T)}> ] Active events buffer overflow, maximum {MAX_EVENTS} events");
                            #endif
                            page.Free(ref freePages[freePagesCount++]);
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void SuppressAll(int receiverId) {
                    ref var receiver = ref receivers[receiverId];
                    var ev = new Event<T>();
                    ref var next = ref ev._eventIdx;

                    while (receiver.Sequence != sequence) {
                        next = (int) (receiver.Sequence++ & MAX_EVENTS_OFFSET_MASK);
                        var pageIdx = (uint) (next >> EVENT_PAGE_SHIFT);
                        var inPageIdx = (uint) (next & EVENT_PAGE_OFFSET_MASK);
                        var maskIdx = (byte) (inPageIdx >> EVENT_IN_PAGE_MASK_SHIFT);
                        var inMaskBit = (byte) (inPageIdx & EVENT_IN_PAGE_OFFSET_MASK);
                        ref var page = ref pages[pageIdx];
                        var mask = page.Mask;

                        ref var unreadCount = ref page.UnreadReceiversCount[inPageIdx];
                        if (unreadCount != 0) {
                            unreadCount = 0;
                            mask[maskIdx] &= ~(1UL << inMaskBit);
                            #if FFS_ECS_EVENTS
                            if (_debugEventListeners != null) {
                                foreach (var listener in _debugEventListeners) {
                                    listener.OnEventReadAll(ev);
                                }
                            }
                            #endif
                            if (clearable) {
                                page.Data[inPageIdx] = default;
                            }

                            if (inPageIdx == EVENT_PAGE_OFFSET_MASK) {
                                #if FFS_ECS_DEBUG
                                if (freePagesCount + 1 >= freePages.Length) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.Events<{typeof(T)}> ] Active events buffer overflow, maximum {MAX_EVENTS} events");
                                #endif
                                page.Free(ref freePages[freePagesCount++]);
                            }
                        }
                    }
                }
                
                [MethodImpl(AggressiveInlining)]
                internal ushort Version(int eventIdx) {
                    var pageIdx = (uint) (eventIdx >> EVENT_PAGE_SHIFT);
                    return pages[pageIdx].Version;
                }

                [MethodImpl(AggressiveInlining)]
                internal bool IsLastReading(int eventIdx) {
                    var pageIdx = (uint) (eventIdx >> EVENT_PAGE_SHIFT);
                    var inPageIdx = (uint) (eventIdx & EVENT_PAGE_OFFSET_MASK);
                    return pages[pageIdx].UnreadReceiversCount[inPageIdx] == 1;
                }

                [MethodImpl(AggressiveInlining)]
                internal int UnreadCount(int eventIdx) {
                    var pageIdx = (uint) (eventIdx >> EVENT_PAGE_SHIFT);
                    var inPageIdx = (uint) (eventIdx & EVENT_PAGE_OFFSET_MASK);
                    return pages[pageIdx].UnreadReceiversCount[inPageIdx];
                }

                [MethodImpl(AggressiveInlining)]
                internal int Last() {
                    var seq = sequence - 1;
                    var pageIdx = (uint) ((seq >> EVENT_PAGE_SHIFT) & PAGES_OFFSET_MASK);
                    var inPageIdx = (uint) (seq & EVENT_PAGE_OFFSET_MASK);
                    
                    return pages[pageIdx].UnreadReceiversCount[inPageIdx];
                }

                [MethodImpl(AggressiveInlining)]
                internal int NotDeletedCount() {
                    if (receiversCount > deletedReceiversCount) {
                        var minSeq = sequence;
                        for (var i = 0; i < receiversCount; i++) {
                            ref var receiver = ref receivers[i];
                            if (!receiver.Deleted && receiver.Sequence < minSeq) {
                                minSeq = receiver.Sequence;
                            }
                        }

                        return (int) (sequence - minSeq);
                    }

                    return 0;
                }

                [MethodImpl(AggressiveInlining)]
                internal int Capacity() {
                    return maxPagesCount << EVENT_PAGE_SHIFT;
                }

                [MethodImpl(AggressiveInlining)]
                internal void Clear() {
                    for (int i = 0; i < pages.Length; i++) {
                        ref var page = ref pages[i];
                        if (page.Data != null) {
                            Array.Clear(page.Data, 0, page.Data.Length);
                            Array.Clear(page.Mask, 0, page.Mask.Length);
                            Array.Clear(page.UnreadReceiversCount, 0, page.UnreadReceiversCount.Length);
                            page.Free(ref freePages[freePagesCount++]);
                        }

                        page.Version = 0;
                    }

                    for (int i = 0; i < receiversCount; i++) {
                        ref var receiver = ref receivers[i];
                        receiver.Sequence = 0;
                        receiver.Deleted = false;
                    }

                    receiversCount = 0;
                    deletedReceiversCount = 0;
                    sequence = 0;
                }

                #if FFS_ECS_DEBUG
                [MethodImpl(AggressiveInlining)]
                internal void AddBlocker(int val) {
                    _blockers += val;
                }
                
                [MethodImpl(AggressiveInlining)]
                internal bool IsBlocked() {
                    return _blockers > 0;
                }
                #endif
            }
        }
    }
}
