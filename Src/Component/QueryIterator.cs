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
    public ref struct QueryComponentsIterator<WorldID, C> where C : struct, IComponent where WorldID : struct, IWorldId {
        private readonly C[] _data; //8
        private int _count;         //4

        [MethodImpl(AggressiveInlining)]
        public QueryComponentsIterator(byte cs9fake) {
            _data = Ecs<WorldID>.Components<C>.Value.Data();
            _count = Ecs<WorldID>.Components<C>.Value.Count();
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldID>.Components<C>.Value.AddBlocker(1);
            #endif
        }

        public readonly ref C Current {
            [MethodImpl(AggressiveInlining)]
            get {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                ref var val = ref _data[_count];
                if (Ecs<WorldID>.ModuleComponents.Value._debugEventListeners != null) {
                    foreach (var listener in Ecs<WorldID>.ModuleComponents.Value._debugEventListeners) {
                        listener.OnComponentRefMut(new Ecs<WorldID>.Entity(Ecs<WorldID>.Components<C>.Value.EntitiesData()[_count]), ref val);
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
            _count--;
            return _count >= 0;
        }

        [MethodImpl(AggressiveInlining)]
        public QueryComponentsIterator<WorldID, C> GetEnumerator() => this;

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
            [MethodImpl(AggressiveInlining)]
            public void Dispose() {
                Ecs<WorldID>.Components<C>.Value.AddBlocker(-1);
            }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public ref struct QueryComponentsIterator<WorldID, C, QW>
        where C : struct, IComponent
        where QW : struct, IQueryMethod
        where WorldID : struct, IWorldId {
        private readonly C[] _data;                               //8
        private readonly int[] _entities; //8
        private int _count;                                       //4
        private QW _with;                                         //???

        [MethodImpl(AggressiveInlining)]
        public QueryComponentsIterator(QW with) {
            _with = with;
            _data = Ecs<WorldID>.Components<C>.Value.Data();
            _count = Ecs<WorldID>.Components<C>.Value.Count();
            _entities = Ecs<WorldID>.Components<C>.Value.EntitiesData();
            var count = int.MaxValue;
            int[] entities = null;
            with.SetData<WorldID>(ref count, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldID>.Components<C>.Value.AddBlocker(1);
            #endif
        }

        public readonly ref C Current {
            [MethodImpl(AggressiveInlining)]
            get {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                ref var val = ref _data[_count];
                if (Ecs<WorldID>.ModuleComponents.Value._debugEventListeners != null) {
                    foreach (var listener in Ecs<WorldID>.ModuleComponents.Value._debugEventListeners) {
                        listener.OnComponentRefMut(new Ecs<WorldID>.Entity(_entities[_count]), ref val);
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
            while (true) {
                if (_count <= 0) {
                    return false;
                }

                _count--;

                if (_with.CheckEntity(_entities[_count])) {
                    return true;
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public QueryComponentsIterator<WorldID, C, QW> GetEnumerator() => this;

        [MethodImpl(AggressiveInlining)]
        public void Dispose() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldID>.Components<C>.Value.AddBlocker(-1);
            #endif
            _with.Dispose<WorldID>();
        }
    }
}