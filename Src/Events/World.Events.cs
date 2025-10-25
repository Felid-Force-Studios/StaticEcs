#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

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

        public bool TryGetPool(Type eventType, out IEventPoolWrapper pool);
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct EventsWrapper<WorldType> : IEvents where WorldType : struct, IWorldType {
        [MethodImpl(AggressiveInlining)]
        public void Send<E>(E value = default) where E : struct, IEvent {
            World<WorldType>.Events.Send(value);
        }

        [MethodImpl(AggressiveInlining)]
        public bool TryGetPool(Type eventType, out IEventPoolWrapper pool) {
            return World<WorldType>.Events.TryGetPool(eventType, out pool);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        public abstract partial class Events {
            #if FFS_ECS_EVENTS
            internal static List<IEventsDebugEventListener> _debugEventListeners;
            #endif

            private static Dictionary<Type, int> _poolIdxByType;
            private static IEventPoolWrapper[] _pools;
            private static ushort _poolsCount;

            #region PUBLIC
            [MethodImpl(AggressiveInlining)]
            public static bool Send<E>(E value = default) where E : struct, IEvent {
                #if FFS_ECS_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.Events.Send<{typeof(E)}> ] Ecs not initialized");
                if (MultiThreadActive) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.Events.Send<{typeof(E)}> ] this operation is not supported in multithreaded mode");
                #endif
                return Pool<E>.Value.Initialized && Pool<E>.Value.Add(value);
            }

            [MethodImpl(AggressiveInlining)]
            public static EventReceiver<WorldType, E> RegisterEventReceiver<E>() where E : struct, IEvent {
                #if FFS_ECS_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.Events.RegisterEventReceiver<{typeof(E)}> ] Ecs not initialized");
                if (!Pool<E>.Value.Initialized) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.Events.RegisterEventReceiver<{typeof(E)}> ] Event type {typeof(E)} not registered");
                #endif
                return Pool<E>.Value.CreateReceiver();
            }

            [MethodImpl(AggressiveInlining)]
            public static void DeleteEventReceiver<E>(ref EventReceiver<WorldType, E> receiver) where E : struct, IEvent {
                #if FFS_ECS_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.Events.DeleteEventReceiver<{typeof(E)}> ] Ecs not initialized");
                #endif
                if (Pool<E>.Value.Initialized) {
                    Pool<E>.Value.DeleteReceiver(ref receiver);
                }
            }

            #if FFS_ECS_EVENTS
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
            public static IEventPoolWrapper GetPool(Type eventType) {
                #if FFS_ECS_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"Events<{typeof(WorldType)}>, Method: GetPool, World not initialized");
                if (!_poolIdxByType.ContainsKey(eventType)) throw new StaticEcsException($"Events<{typeof(WorldType)}>, Method: GetPool, Event type {eventType} not registered");
                #endif
                return _pools[_poolIdxByType[eventType]];
            }

            [MethodImpl(AggressiveInlining)]
            public static EventPoolWrapper<WorldType, T> GetPool<T>() where T : struct, IEvent {
                #if FFS_ECS_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"Events<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, World not initialized");
                if (!_poolIdxByType.ContainsKey(typeof(T))) throw new StaticEcsException($"Events<{typeof(WorldType)}>, Method: GetPool, Event type {typeof(T)} not registered");
                #endif
                return default;
            }

            [MethodImpl(AggressiveInlining)]
            public static bool TryGetPool(Type eventType, out IEventPoolWrapper pool) {
                #if FFS_ECS_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"Events<{typeof(WorldType)}>, Method: GetPool, World not initialized");
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
                #if FFS_ECS_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"Events<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, World not initialized");
                #endif
                pool = default;
                return _poolIdxByType.ContainsKey(typeof(T));
            }

            [MethodImpl(AggressiveInlining)]
            public static void RegisterEventType<T>(IEventConfig<T, WorldType> config = null, uint baseCapacity = 64) where T : struct, IEvent {
                #if FFS_ECS_DEBUG
                if (Status != WorldStatus.Created) {
                    throw new StaticEcsException($"Events<{typeof(WorldType)}>, Method: RegisterEventType<{typeof(T)}>, World not initialized");
                }
                #endif

                if (Pool<T>.Value.Initialized) throw new StaticEcsException($"Event {typeof(T)} already registered");
                
                config ??= DefaultEventConfig<T, WorldType>.Default;
                Pool<T>.Value.Create(_poolsCount, config, baseCapacity);

                if (_poolsCount == _pools.Length) {
                    Array.Resize(ref _pools, _poolsCount << 1);
                }

                _pools[_poolsCount] = new EventPoolWrapper<WorldType, T>();
                _poolIdxByType[typeof(T)] = _poolsCount;
                _poolsCount++;

                Serializer.Value.RegisterEventType(config);
            }
            #endregion

            #region INTERNAL
            [MethodImpl(AggressiveInlining)]
            internal static void Create() {
                _pools = new IEventPoolWrapper[32];
                _poolIdxByType = new Dictionary<Type, int>();
                Serializer.Value.Create();
            }

            [MethodImpl(AggressiveInlining)]
            internal static void Destroy() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Destroy();
                }

                _poolsCount = default;
                _pools = default;
                _poolIdxByType = default;
                Serializer.Value.Destroy();
                #if FFS_ECS_EVENTS
                _debugEventListeners = default;
                #endif
            }
            #endregion

            [MethodImpl(AggressiveInlining)]
            public static List<IEventPoolWrapper> GetAllRawsPools() {
                var pools = new List<IEventPoolWrapper>();
                for (var i = 0; i < _poolsCount; i++) {
                    pools.Add(_pools[i]);
                }

                return pools;
            }

            [MethodImpl(AggressiveInlining)]
            public static void Clear() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Clear();
                }
            }
        }

        #if FFS_ECS_EVENTS
        public interface IEventsDebugEventListener {
            void OnEventSent<T>(Event<T> value) where T : struct, IEvent;
            void OnEventReadAll<T>(Event<T> value) where T : struct, IEvent;
            void OnEventSuppress<T>(Event<T> value) where T : struct, IEvent;
        }
        #endif
    }
}
