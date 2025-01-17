#if !FFS_ECS_DISABLE_EVENTS
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    public interface IEvents {
        public void Send<E>(E value = default) where E : struct, IEvent;

        public void SendDefault(EventDynId value);

        public EventDynId DynamicId<E>() where E : struct, IEvent;

        public bool TryGetPool(EventDynId id, out IEventPoolWrapper pool);

        public bool TryGetPool(Type eventType, out IEventPoolWrapper pool);
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct EventsWrapper<WorldType> : IEvents where WorldType : struct, IWorldType {
        [MethodImpl(AggressiveInlining)]
        public void Send<E>(E value = default) where E : struct, IEvent {
            Ecs<WorldType>.Events.Send(value);
        }

        [MethodImpl(AggressiveInlining)]
        public void SendDefault(EventDynId value) {
            Ecs<WorldType>.Events.SendDefault(value);
        }

        [MethodImpl(AggressiveInlining)]
        public EventDynId DynamicId<E>() where E : struct, IEvent {
            return Ecs<WorldType>.Events.DynamicId<E>();
        }

        [MethodImpl(AggressiveInlining)]
        public bool TryGetPool(EventDynId id, out IEventPoolWrapper pool) {
            return Ecs<WorldType>.Events.TryGetPool(id, out pool);
        }

        [MethodImpl(AggressiveInlining)]
        public bool TryGetPool(Type eventType, out IEventPoolWrapper pool) {
            return Ecs<WorldType>.Events.TryGetPool(eventType, out pool);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class Ecs<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        public abstract partial class Events {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal static List<IEventsDebugEventListener> _debugEventListeners;
            #endif

            private static Dictionary<Type, int> _poolIdxByType;
            private static IEventPoolWrapper[] _pools;
            private static ushort _poolsCount;

            #region PUBLIC
            [MethodImpl(AggressiveInlining)]
            public static bool Send<E>(E value = default) where E : struct, IEvent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"[ Ecs<{typeof(World)}>.Events.Send<{typeof(E)}> ] Ecs not initialized");
                #endif
                return Pool<E>.Value.Initialized && Pool<E>.Value.Add(value);
            }

            [MethodImpl(AggressiveInlining)]
            public static bool SendDefault(EventDynId value) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"[ Ecs<{typeof(World)}>.Events.Send ] Ecs not initialized");
                #endif
                return value.Value < _poolsCount && _pools[value.Value].Add();
            }

            [MethodImpl(AggressiveInlining)]
            public static EventReceiver<WorldType, E> RegisterEventReceiver<E>() where E : struct, IEvent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"[ Ecs<{typeof(World)}>.Events.RegisterEventReceiver<{typeof(E)}> ] Ecs not initialized");
                if (!Pool<E>.Value.Initialized) throw new Exception($"[ Ecs<{typeof(World)}>.Events.RegisterEventReceiver<{typeof(E)}> ] Event type {typeof(E)} not registered");
                #endif
                return Pool<E>.Value.CreateReceiver();
            }

            [MethodImpl(AggressiveInlining)]
            public static void DeleteEventReceiver<E>(ref EventReceiver<WorldType, E> receiver) where E : struct, IEvent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"[ Ecs<{typeof(World)}>.Events.DeleteEventReceiver<{typeof(E)}> ] Ecs not initialized");
                #endif
                if (Pool<E>.Value.Initialized) {
                    Pool<E>.Value.DeleteReceiver(ref receiver);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static EventDynId DynamicId<E>() where E : struct, IEvent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"[ Ecs<{typeof(World)}>.Events.Send<{typeof(E)}> ] Ecs not initialized");
                #endif
                if (Pool<E>.Value.Initialized) {
                    return new EventDynId(Pool<E>.Value.Id);
                }

                throw new Exception($"[ Ecs<{typeof(World)}>.Events.DynamicId<{typeof(E)}> ] Pool not initialized");
            }

            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            [MethodImpl(AggressiveInlining)]
            public static void AddEventsDebugEventListener(IEventsDebugEventListener listener) {
                _debugEventListeners ??= new List<IEventsDebugEventListener>();
                _debugEventListeners.Add(listener);
            }

            [MethodImpl(AggressiveInlining)]
            public static void RemoveEventsDebugEventListener(IEventsDebugEventListener listener) {
                _debugEventListeners?.Remove(listener);
            }
            #endif

            [MethodImpl(AggressiveInlining)]
            public static IEventPoolWrapper GetPool(EventDynId id) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Events<{typeof(WorldType)}>, Method: GetPool, World not initialized");
                if (id.Value >= _poolsCount) throw new Exception($"Events<{typeof(WorldType)}>, Method: GetPool, Event type for dyn id {id.Value} not registered");
                #endif
                return _pools[id.Value];
            }

            [MethodImpl(AggressiveInlining)]
            public static IEventPoolWrapper GetPool(Type eventType) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Events<{typeof(WorldType)}>, Method: GetPool, World not initialized");
                if (!_poolIdxByType.ContainsKey(eventType)) throw new Exception($"Events<{typeof(WorldType)}>, Method: GetPool, Event type {eventType} not registered");
                #endif
                return _pools[_poolIdxByType[eventType]];
            }

            [MethodImpl(AggressiveInlining)]
            public static EventPoolWrapper<WorldType, T> GetPool<T>() where T : struct, IEvent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Events<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, World not initialized");
                if (!_poolIdxByType.ContainsKey(typeof(T))) throw new Exception($"Events<{typeof(WorldType)}>, Method: GetPool, Event type {typeof(T)} not registered");
                #endif
                return default;
            }

            [MethodImpl(AggressiveInlining)]
            public static bool TryGetPool(EventDynId id, out IEventPoolWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Events<{typeof(WorldType)}>, Method: GetPool, World not initialized");
                #endif
                if (id.Value >= _poolsCount) {
                    pool = default;
                    return false;
                }

                pool = _pools[id.Value];
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            public static bool TryGetPool(Type eventType, out IEventPoolWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Events<{typeof(WorldType)}>, Method: GetPool, World not initialized");
                #endif
                if (!_poolIdxByType.TryGetValue(eventType, out var idx)) {
                    pool = default;
                    return false;
                }

                pool = _pools[idx];
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            public static bool TryGetPool<T>(out EventPoolWrapper<WorldType, T> pool) where T : struct, IEvent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Events<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, World not initialized");
                #endif
                pool = default;
                return _poolIdxByType.ContainsKey(typeof(T));
            }

            [MethodImpl(AggressiveInlining)]
            public static EventDynId RegisterEventType<T>() where T : struct, IEvent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (World.Status != WorldStatus.Created) {
                    throw new Exception($"Events<{typeof(WorldType)}>, Method: RegisterEventType<{typeof(T)}>, World not initialized");
                }
                #endif

                if (Pool<T>.Value.Initialized) {
                    return new EventDynId(Pool<T>.Value.Id);
                }
                
                Pool<T>.Value.Create(_poolsCount);

                if (_poolsCount == _pools.Length) {
                    Array.Resize(ref _pools, _poolsCount << 1);
                }

                _pools[_poolsCount] = new EventPoolWrapper<WorldType, T>();
                _poolIdxByType[typeof(T)] = _poolsCount;
                _poolsCount++;
                return new EventDynId(Pool<T>.Value.Id);
            }
            #endregion

            #region INTERNAL
            [MethodImpl(AggressiveInlining)]
            internal static void Create() {
                _pools = new IEventPoolWrapper[32];
                _poolIdxByType = new Dictionary<Type, int>();
            }

            [MethodImpl(AggressiveInlining)]
            internal static void Destroy() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Destroy();
                }

                _poolsCount = default;
                _pools = default;
                _poolIdxByType = default;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners = default;
                #endif
            }
            #endregion
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
        public interface IEventsDebugEventListener {
            void OnEventSent<T>(Event<T> value) where T : struct, IEvent;
            void OnEventReadAll<T>(Event<T> value) where T : struct, IEvent;
            void OnEventSuppress<T>(Event<T> value) where T : struct, IEvent;
        }
        #endif
    }
}
#endif