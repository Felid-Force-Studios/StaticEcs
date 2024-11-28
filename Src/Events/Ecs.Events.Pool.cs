#if !FFS_ECS_DISABLE_EVENTS
using System;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public interface IEventPoolWrapper {
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

        internal short Version(int idx);

        internal void PutRaw(int idx, IEvent value);
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct EventPoolWrapper<WorldID, T> : IEventPoolWrapper where T : struct, IEvent where WorldID : struct, IWorldId {
        [MethodImpl(AggressiveInlining)]
        internal ref T Get(int idx) => ref Ecs<WorldID>.Events.Pool<T>.Value.Get(idx);

        [MethodImpl(AggressiveInlining)]
        public bool Add(T value) => Ecs<WorldID>.Events.Pool<T>.Value.Add(value);

        [MethodImpl(AggressiveInlining)]
        IEvent IEventPoolWrapper.GetRaw(int idx) => Ecs<WorldID>.Events.Pool<T>.Value.Get(idx);

        [MethodImpl(AggressiveInlining)]
        void IEventPoolWrapper.Destroy() => Ecs<WorldID>.Events.Pool<T>.Value.Destroy();

        [MethodImpl(AggressiveInlining)]
        public bool AddRaw(IEvent value) => Ecs<WorldID>.Events.Pool<T>.Value.Add((T) value);

        [MethodImpl(AggressiveInlining)]
        void IEventPoolWrapper.PutRaw(int idx, IEvent value) => Ecs<WorldID>.Events.Pool<T>.Value.Get(idx) = (T) value;

        [MethodImpl(AggressiveInlining)]
        public bool Add() => Ecs<WorldID>.Events.Pool<T>.Value.Add();

        [MethodImpl(AggressiveInlining)]
        void IEventPoolWrapper.Del(int idx) => Ecs<WorldID>.Events.Pool<T>.Value.Del(idx, true);

        [MethodImpl(AggressiveInlining)]
        short IEventPoolWrapper.Version(int idx) => Ecs<WorldID>.Events.Pool<T>.Value._versions[idx];

        [MethodImpl(AggressiveInlining)]
        bool IEventPoolWrapper.IsDeleted(int idx) => Ecs<WorldID>.Events.Pool<T>.Value._versions[idx] <= 0;

        [MethodImpl(AggressiveInlining)]
        int IEventPoolWrapper.UnreadCount(int idx) => Ecs<WorldID>.Events.Pool<T>.Value._dataReceiverUnreadCount[idx];

        [MethodImpl(AggressiveInlining)]
        int IEventPoolWrapper.Last() => Ecs<WorldID>.Events.Pool<T>.Value._dataCount - 1;

        [MethodImpl(AggressiveInlining)]
        int IEventPoolWrapper.NotDeletedCount() => Ecs<WorldID>.Events.Pool<T>.Value._notDeletedCount;

        [MethodImpl(AggressiveInlining)]
        int IEventPoolWrapper.Capacity() => Ecs<WorldID>.Events.Pool<T>.Value._data.Length;

        [MethodImpl(AggressiveInlining)]
        int IEventPoolWrapper.ReceiversCount() => Ecs<WorldID>.Events.Pool<T>.Value._receiversCount - Ecs<WorldID>.Events.Pool<T>.Value._deletedReceiversCount;

        [MethodImpl(AggressiveInlining)]
        internal void Del(int idx) => Ecs<WorldID>.Events.Pool<T>.Value.Del(idx, true);
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class Ecs<WorldID> {
        
        public abstract partial class Events {
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            [Il2CppEagerStaticClassConstruction]
            #endif
            internal struct Pool<T> where T : struct, IEvent {
                internal static Pool<T> Value;
                internal T[] _data;
                internal short[] _versions;
                internal int[] _dataReceiverUnreadCount;
                internal int[] _dataReceiverOffsets;
                internal ushort[] _deletedReceivers;
                internal int _dataCount;
                internal int _notDeletedCount;
                internal int _dataFirstIdx;
                internal ushort _deletedReceiversCount;
                internal ushort _receiversCount;
                internal ushort Id;
                internal bool Initialized;
                
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                private int _blockers;
                #endif

                [MethodImpl(AggressiveInlining)]
                internal void Create(ushort id) {
                    Id = id;
                    _data = new T[cfg.BaseEventPoolCount];
                    _versions = new short[cfg.BaseEventPoolCount];
                    _dataReceiverUnreadCount = new int[cfg.BaseEventPoolCount];
                    _dataFirstIdx = 0;
                    _dataCount = 0;
                    _dataReceiverOffsets = new int[8];
                    _deletedReceivers = new ushort[8];
                    _receiversCount = 0;
                    _deletedReceiversCount = 0;
                    _notDeletedCount = 0;
                    Initialized = true;
                }

                [MethodImpl(AggressiveInlining)]
                internal EventReceiver<WorldID, T> CreateReceiver() {
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (_blockers > 0) throw new Exception($"[ Ecs<{typeof(World)}>.Events.Pool<{typeof(T)}>.CreateReceiver ] event pool cannot be changed, it is in read-only mode");
                    #endif
                    for (var i = _dataFirstIdx; i < _dataCount; i++) {
                        _dataReceiverUnreadCount[i]++;
                    }
                    
                    if (_deletedReceiversCount > 0) {
                        var receiver = _deletedReceivers[--_deletedReceiversCount];
                        _dataReceiverOffsets[receiver] = _dataFirstIdx;
                        return new EventReceiver<WorldID, T>(receiver);
                    }
                    
                    if (_receiversCount == _dataReceiverOffsets.Length) {
                        Array.Resize(ref _dataReceiverOffsets, _receiversCount << 1);
                        for (var i = _receiversCount; i < _dataReceiverOffsets.Length; i++) {
                            _dataReceiverOffsets[i] = _dataFirstIdx;
                        }
                    }
                    
                    return new EventReceiver<WorldID, T>(_receiversCount++);
                }
                
                [MethodImpl(AggressiveInlining)]
                internal void DeleteReceiver(ref EventReceiver<WorldID, T> receiver) {
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (_blockers > 0) throw new Exception($"[ Ecs<{typeof(World)}>.Events.Pool<{typeof(T)}>.DeleteReceiver ] event pool cannot be changed, it is in read-only mode");
                    #endif
                    if (_deletedReceiversCount == _deletedReceivers.Length) {
                        Array.Resize(ref _deletedReceivers, _deletedReceiversCount << 1);
                    }
                    
                    MarkAsReadAll(receiver._id);

                    _dataReceiverOffsets[receiver._id] = -1;
                    _deletedReceivers[_deletedReceiversCount++] = (ushort) receiver._id;
                    receiver._id = -1;
                }

                [MethodImpl(AggressiveInlining)]
                internal void Destroy() {
                    Initialized = false;
                    _dataReceiverUnreadCount = default;
                    _deletedReceiversCount = default;
                    _dataReceiverOffsets = default;
                    _deletedReceivers = default;
                    _notDeletedCount = default;
                    _receiversCount = default;
                    _dataFirstIdx = default;
                    _dataCount = default;
                    _versions = default;
                    _data = default;
                    Id = default;
                }
                
                [MethodImpl(AggressiveInlining)]
                internal ref T Get(int idx) {
                    return ref _data[idx];
                }

                [MethodImpl(AggressiveInlining)]
                internal bool Add(T value = default) {
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (_blockers > 0) throw new Exception($"[ Ecs<{typeof(World)}>.Events.Pool<{typeof(T)}>.Add ] event pool cannot be changed, it is in read-only mode");
                    #endif
                    if (_receiversCount > 0) {
                        _notDeletedCount++;
                        if (_dataCount == _data.Length) {
                            Array.Resize(ref _data, _dataCount << 1);
                            Array.Resize(ref _versions, _dataCount << 1);
                            Array.Resize(ref _dataReceiverUnreadCount, _dataCount << 1);
                        }
                        _data[_dataCount] = value;
                        ref var version = ref _versions[_dataCount];
                        version *= -1;
                        if (version == short.MaxValue) {
                            version = 0;
                        }
                        version++;
                        _dataReceiverUnreadCount[_dataCount] = _receiversCount;
                        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                        if (_debugEventListeners != null) {
                            foreach (var listener in _debugEventListeners) {
                                listener.OnEventSent(new Event<T>(_dataCount));
                            }
                        }
                        #endif
                        _dataCount++;
                        return true;
                    }

                    return false;
                }
                
                [MethodImpl(AggressiveInlining)]
                internal bool Del(int idx, bool suppressed) {
                    if (_versions[idx] > 0) {
                        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                        if (_debugEventListeners != null) {
                            foreach (var listener in _debugEventListeners) {
                                if (suppressed) {
                                    listener.OnEventSuppress(new Event<T>(idx));
                                } else {
                                    listener.OnEventReadAll(new Event<T>(idx));
                                }
                            }
                        }
                        #endif
                        _notDeletedCount--;
                        _data[idx] = default;
                        _versions[idx] *= -1;
                        _dataReceiverUnreadCount[idx] = 0;
                    
                        if (idx == _dataCount - 1) {
                            _dataCount--;
                            return TryDropOffsets();
                        }

                        if (_dataFirstIdx == idx) {
                            while (_versions[idx] <= 0 && idx < _dataCount) {
                                _dataFirstIdx++;
                                idx++;
                            }
                        
                            return TryDropOffsets();
                        }
                    }

                    return false;
                }
                
                [MethodImpl(AggressiveInlining)]
                internal bool ShiftReceiverOffset(int receiverId, int previous, out int next) {
                    if (previous >= 0 && MarkAsRead(previous)) {
                        next = -1;
                        return false;
                    }
                    
                    ref var offset = ref _dataReceiverOffsets[receiverId];
                    for (; offset < _dataCount; offset++) {
                        if (_versions[offset] > 0) {
                            next = offset++;
                            return true;
                        }
                    }

                    next = -1;
                    return false;
                }
                
                [MethodImpl(AggressiveInlining)]
                internal void MarkAsReadAll(int receiverId) {
                    ref var offset = ref _dataReceiverOffsets[receiverId];
                    for (; offset < _dataCount; offset++) {
                        if (_versions[offset] > 0 && MarkAsRead(offset)) {
                            return;
                        }
                    }
                }

                [MethodImpl(AggressiveInlining)]
                private bool MarkAsRead(int offset) {
                    ref var unreadCount = ref _dataReceiverUnreadCount[offset];
                    if (unreadCount != 0 && --unreadCount == 0) {
                        if (Del(offset, false)) {
                            return true;
                        }
                    }

                    return false;
                }

                [MethodImpl(AggressiveInlining)]
                internal void ClearEvents(int receiverId) {
                    ref var offset = ref _dataReceiverOffsets[receiverId];
                    for (; offset < _dataCount; offset++) {
                        Del(offset, true);
                    }
                }

                [MethodImpl(AggressiveInlining)]
                private bool TryDropOffsets() {
                    if (_dataFirstIdx == _dataCount) {
                        _dataFirstIdx = 0;
                        _dataCount = 0;
                        for (var i = 0; i < _receiversCount; i++) {
                            ref var offset = ref _dataReceiverOffsets[i];
                            if (offset > 0) {
                                offset = 0;
                            }
                        }

                        return true;
                    }

                    return false;
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
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
#endif