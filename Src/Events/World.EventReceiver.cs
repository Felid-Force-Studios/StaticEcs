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
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct EventReceiver<WorldType, T> where T : struct, IEvent where WorldType : struct, IWorldType {
        internal int _id;

        [MethodImpl(AggressiveInlining)]
        internal EventReceiver(int id) => _id = id;
        
        [MethodImpl(AggressiveInlining)]
        public void ReadAll(World<WorldType>.EventAction<T> action) {
            #if FFS_ECS_DEBUG
            if (_id < 0) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.EventReceiver<{typeof(T)}>.ReadAll ] receiver is deleted");
            if (World<WorldType>.MultiThreadActive) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.EventReceiver<{typeof(T)}>.ReadAll ] this operation is not supported in multithreaded mode");
            #endif
            World<WorldType>.Events.Pool<T>.Value.ReadAll(_id, action);
        }

        [MethodImpl(AggressiveInlining)]
        public void MarkAsReadAll() {
            #if FFS_ECS_DEBUG
            if (_id < 0) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.EventReceiver<{typeof(T)}>.MarkAsReadAll ] receiver is deleted");
            if (World<WorldType>.MultiThreadActive) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.EventReceiver<{typeof(T)}>.MarkAsReadAl ] this operation is not supported in multithreaded mode");
            #endif
            World<WorldType>.Events.Pool<T>.Value.ReadAll(_id);
        }
            
        [MethodImpl(AggressiveInlining)]
        public void SuppressAll() {
            #if FFS_ECS_DEBUG
            if (_id < 0) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.EventReceiver<{typeof(T)}>.SuppressAll ] receiver is deleted");
            if (World<WorldType>.MultiThreadActive) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.EventReceiver<{typeof(T)}>.SuppressAll ] this operation is not supported in multithreaded mode");
            #endif
            World<WorldType>.Events.Pool<T>.Value.SuppressAll(_id);
        }

        [MethodImpl(AggressiveInlining)]
        public World<WorldType>.EventIterator<T> GetEnumerator() {
            #if FFS_ECS_DEBUG
            if (_id < 0) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.EventReceiver<{typeof(T)}>.GetEnumerator ] receiver is deleted");
            if (World<WorldType>.MultiThreadActive) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.EventReceiver<{typeof(T)}>.GetEnumerator ] this operation is not supported in multithreaded mode");
            if (World<WorldType>.Events.Pool<T>.Value.IsBlocked()) throw new StaticEcsException($"[ World<{typeof(WorldType)}>.EventReceiver<{typeof(T)}>.GetEnumerator ] event pool is blocked");
            #endif
            return new World<WorldType>.EventIterator<T>(_id);
        }
    }
    
    #if ENABLE_IL2CPP
   [Il2CppSetOption(Option.NullChecks, false)]
   [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public static class EventReceiverSerializer {
        [MethodImpl(AggressiveInlining)]
        public static void WriteEventReceiver<WorldType, T>(this ref BinaryPackWriter writer, in EventReceiver<WorldType, T> value) where WorldType : struct, IWorldType where T : struct, IEvent => writer.WriteInt(value._id);

        [MethodImpl(AggressiveInlining)]
        public static EventReceiver<WorldType, T> ReadEventReceiver<WorldType, T>(this ref BinaryPackReader reader) where WorldType : struct, IWorldType where T : struct, IEvent => new(reader.ReadInt());
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
                #if FFS_ECS_DEBUG
                Events.Pool<T>.Value.AddBlocker(1);
                #endif
            }

            public Event<T> Current {
                [MethodImpl(AggressiveInlining)] get => _current;
            }

            [MethodImpl(AggressiveInlining)]
            public bool MoveNext() => Events.Pool<T>.Value.ReadOne(_id, _current._eventIdx, out _current._eventIdx);
            
            [MethodImpl(AggressiveInlining)]
            public void Dispose() {
                Events.Pool<T>.Value.ReadOne(_id, _current._eventIdx, out var _);
                #if FFS_ECS_DEBUG
                Events.Pool<T>.Value.AddBlocker(-1);
                #endif
            }
        }
    }
}
