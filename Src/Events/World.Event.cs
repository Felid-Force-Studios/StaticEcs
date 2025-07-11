﻿using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

#if !FFS_ECS_DISABLE_EVENTS
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public ref struct Event<E> where E : struct, IEvent {
            internal int _idx;
                
            [MethodImpl(AggressiveInlining)]
            public Event(int idx) {
                _idx = idx;
            }

            public ref E Value {
                [MethodImpl(AggressiveInlining)]
                get {
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (_idx < 0) throw new StaticEcsException($"[ Ecs<{typeof(WorldType)}>.Event<{typeof(E)}>.Value ] event is deleted");
                    #endif
                    return ref Events.Pool<E>.Value.Get(_idx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public void Suppress() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (_idx < 0) throw new StaticEcsException($"[ Ecs<{typeof(WorldType)}>.Event<{typeof(E)}>.Suppress ] event is deleted");
                #endif
                Events.Pool<E>.Value.Del((ushort) _idx, true);
                _idx = -1;
            }

            [MethodImpl(AggressiveInlining)]
            public bool IsLastReading() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (_idx < 0) throw new StaticEcsException($"[ Ecs<{typeof(WorldType)}>.Event<{typeof(E)}>.IsLastReading ] event is deleted");
                #endif
                return Events.Pool<E>.Value._dataReceiverUnreadCount[_idx] == 1;
            }
            
            [MethodImpl(AggressiveInlining)]
            public int UnreadCount() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (_idx < 0) throw new StaticEcsException($"[ Ecs<{typeof(WorldType)}>.Event<{typeof(E)}>.UnreadCount ] event is deleted");
                #endif
                return Events.Pool<E>.Value._dataReceiverUnreadCount[_idx] - 1;
            }
        }
    }
}
#endif

namespace FFS.Libraries.StaticEcs {
    public interface IEvent { }
    
}