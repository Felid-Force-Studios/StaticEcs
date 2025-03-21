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
    public ref struct QueryComponentsIterator<WorldType, C> where C : struct, IComponent where WorldType : struct, IWorldType {
        private readonly C[] _data; //8
        private uint _count;         //4

        [MethodImpl(AggressiveInlining)]
        public QueryComponentsIterator(byte _) {
            _data = Ecs<WorldType>.Components<C>.Value.Data();
            _count = Ecs<WorldType>.Components<C>.Value.Count();
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C>.Value.AddBlocker(1);
            #endif
        }

        public readonly ref C Current {
            [MethodImpl(AggressiveInlining)]
            get {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                ref var val = ref _data[_count];
                if (Ecs<WorldType>.Components<C>.Value.debugEventListeners != null) {
                    foreach (var listener in Ecs<WorldType>.Components<C>.Value.debugEventListeners) {
                        listener.OnComponentRefMut(new Ecs<WorldType>.Entity(Ecs<WorldType>.Components<C>.Value.EntitiesData()[_count]), ref val);
                    }
                }
                return ref val;
                #else
                return ref _data[_count];
                #endif
            }
        }

        [MethodImpl(AggressiveInlining)]
        public bool MoveNext() {
            if (_count == 0) {
                return false;
            }
            _count--;
            return true;
        }

        [MethodImpl(AggressiveInlining)]
        public QueryComponentsIterator<WorldType, C> GetEnumerator() => this;

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
            [MethodImpl(AggressiveInlining)]
            public void Dispose() {
                Ecs<WorldType>.Components<C>.Value.AddBlocker(-1);
            }
        #endif
    }
}