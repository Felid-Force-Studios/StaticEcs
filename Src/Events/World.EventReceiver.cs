#if !FFS_ECS_DISABLE_EVENTS
using System;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif


namespace FFS.Libraries.StaticEcs {
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct EventReceiver<WorldType, T> where T : struct, IEvent where WorldType : struct, IWorldType {
        internal int _id;

        [MethodImpl(AggressiveInlining)]
        internal EventReceiver(int id) {
            _id = id;
        }
            
        [MethodImpl(AggressiveInlining)]
        public void MarkAsReadAll() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (_id < 0) throw new Exception($"[ Ecs<{typeof(WorldType)}>.EventReceiver<{typeof(T)}>.MarkAsReadAll ] receiver is deleted");
            #endif
            World<WorldType>.Events.Pool<T>.Value.MarkAsReadAll(_id);
        }
            
        [MethodImpl(AggressiveInlining)]
        public void SuppressAll() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (_id < 0) throw new Exception($"[ Ecs<{typeof(WorldType)}>.EventReceiver<{typeof(T)}>.SuppressAll ] receiver is deleted");
            #endif
            World<WorldType>.Events.Pool<T>.Value.ClearEvents(_id);
        }

        [MethodImpl(AggressiveInlining)]
        public World<WorldType>.EventIterator<T> GetEnumerator() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (_id < 0) throw new Exception($"[ Ecs<{typeof(WorldType)}>.EventReceiver<{typeof(T)}>.GetEnumerator ] receiver is deleted");
            if (World<WorldType>.Events.Pool<T>.Value.IsBlocked()) throw new Exception($"[ Ecs<{typeof(WorldType)}>.EventReceiver<{typeof(T)}>.GetEnumerator ] event pool is blocked");
            #endif
            return new World<WorldType>.EventIterator<T>(_id);
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
        #endif
        public ref struct EventIterator<T> where T : struct, IEvent {
            private Event<T> _current;
            internal readonly int _id;

            [MethodImpl(AggressiveInlining)]
            internal EventIterator(int id) {
                _id = id;
                _current = new Event<T>(-1);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Events.Pool<T>.Value.AddBlocker(1);
                #endif
            }

            public Event<T> Current {
                [MethodImpl(AggressiveInlining)] get => _current;
            }

            [MethodImpl(AggressiveInlining)]
            public bool MoveNext() => Events.Pool<T>.Value.ShiftReceiverOffset(_id, _current._idx, out _current._idx);
            
            [MethodImpl(AggressiveInlining)]
            public void Dispose() {
                if (_current._idx >= 0) {
                    Events.Pool<T>.Value.MarkAsRead(_current._idx);
                }
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Events.Pool<T>.Value.AddBlocker(-1);
                #endif
            }
        }
    }
}
#endif