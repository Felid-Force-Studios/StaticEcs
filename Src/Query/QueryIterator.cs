using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    #if ENABLE_IL2CPP
    [Il2CppSetOption (Option.NullChecks, false)]
    [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
    #endif
    public ref struct QueryEntitiesIterator<WorldType, QM> where QM : struct, IPrimaryQueryMethod where WorldType : struct, IWorldType {
        private readonly uint[] _entities;                //8
        private readonly EntityStatus[] _entitiesStatus;  //8
        private uint _current;                            //4
        private uint _count;                              //4
        private QM _queryMethod;                          //???
        private readonly EntityStatusType _entitiesParam; //1

        [MethodImpl(AggressiveInlining)]
        public QueryEntitiesIterator(QM queryMethod, EntityStatusType entities = EntityStatusType.Enabled) {
            _entitiesParam = entities;
            _queryMethod = queryMethod;
            _current = default;
            _entities = default;
            _count = int.MaxValue;
            _queryMethod.SetData<WorldType>(ref _count, ref _entities);
            _entitiesStatus = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();
        }

        public readonly World<WorldType>.Entity Current {
            [MethodImpl(AggressiveInlining)] get => new (_current);
        }

        [MethodImpl(AggressiveInlining)]
        public bool MoveNext() {
            while (true) {
                if (_count == 0) {
                    return false;
                }

                _count--;
                _current = _entities[_count];
                
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    return true;
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public readonly QueryEntitiesIterator<WorldType, QM> GetEnumerator() => this;

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose() {
            _queryMethod.Dispose<WorldType>();
        }
        #endif

        [MethodImpl(AggressiveInlining)]
        public void DestroyAllEntities() {
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    new World<WorldType>.Entity(_current).Destroy();
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool First(out World<WorldType>.Entity entity) {
            var moveNext = MoveNext();
            entity = new World<WorldType>.Entity(_current);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
            return moveNext;
        }
        
        [MethodImpl(AggressiveInlining)]
        public int EntitiesCount() {
            var count = 0;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    count++;
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
            return count;
        }

        #region COMPONENTS
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1>() where T1 : struct, IComponent {
            ref var components = ref World<WorldType>.Components<T1>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    components.AddDefault(new World<WorldType>.Entity(_current));
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent{
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.AddDefault(entity);
                    container2.AddDefault(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.AddDefault(entity);
                    container2.AddDefault(entity);
                    container3.AddDefault(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3, T4>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.AddDefault(entity);
                    container2.AddDefault(entity);
                    container3.AddDefault(entity);
                    container4.AddDefault(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            ref var container5 = ref World<WorldType>.Components<T5>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.AddDefault(entity);
                    container2.AddDefault(entity);
                    container3.AddDefault(entity);
                    container4.AddDefault(entity);
                    container5.AddDefault(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1>() where T1 : struct, IComponent {
            ref var components = ref World<WorldType>.Components<T1>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    components.TryAddDefault(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent{
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryAddDefault(entity);
                    container2.TryAddDefault(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2, T3>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryAddDefault(entity);
                    container2.TryAddDefault(entity);
                    container3.TryAddDefault(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2, T3, T4>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryAddDefault(entity);
                    container2.TryAddDefault(entity);
                    container3.TryAddDefault(entity);
                    container4.TryAddDefault(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            ref var container5 = ref World<WorldType>.Components<T5>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryAddDefault(entity);
                    container2.TryAddDefault(entity);
                    container3.TryAddDefault(entity);
                    container4.TryAddDefault(entity);
                    container5.TryAddDefault(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1>(T1 t1) 
            where T1 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Add(entity) = t1;
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2>(T1 t1, T2 t2) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Add(entity) = t1;
                    container2.Add(entity) = t2;
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3>(T1 t1, T2 t2, T3 t3) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Add(entity) = t1;
                    container2.Add(entity) = t2;
                    container3.Add(entity) = t3;
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Add(entity) = t1;
                    container2.Add(entity) = t2;
                    container3.Add(entity) = t3;
                    container4.Add(entity) = t4;
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            ref var container5 = ref World<WorldType>.Components<T5>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Add(entity) = t1;
                    container2.Add(entity) = t2;
                    container3.Add(entity) = t3;
                    container4.Add(entity) = t4;
                    container5.Add(entity) = t5;
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1>(T1 t1) 
            where T1 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryAdd(entity) = t1;
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2>(T1 t1, T2 t2) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryAdd(entity) = t1;
                    container2.TryAdd(entity) = t2;
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2, T3>(T1 t1, T2 t2, T3 t3) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryAdd(entity) = t1;
                    container2.TryAdd(entity) = t2;
                    container3.TryAdd(entity) = t3;
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryAdd(entity) = t1;
                    container2.TryAdd(entity) = t2;
                    container3.TryAdd(entity) = t3;
                    container4.TryAdd(entity) = t4;
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            ref var container5 = ref World<WorldType>.Components<T5>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryAdd(entity) = t1;
                    container2.TryAdd(entity) = t2;
                    container3.TryAdd(entity) = t3;
                    container4.TryAdd(entity) = t4;
                    container5.TryAdd(entity) = t5;
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void PutForAll<T1>(T1 t1) 
            where T1 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Put(entity, t1);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void PutForAll<T1, T2>(T1 t1, T2 t2) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Put(entity, t1);
                    container2.Put(entity, t2);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void PutForAll<T1, T2, T3>(T1 t1, T2 t2, T3 t3) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Put(entity, t1);
                    container2.Put(entity, t2);
                    container3.Put(entity, t3);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void PutForAll<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Put(entity, t1);
                    container2.Put(entity, t2);
                    container3.Put(entity, t3);
                    container4.Put(entity, t4);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void PutForAll<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            ref var container5 = ref World<WorldType>.Components<T5>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Put(entity, t1);
                    container2.Put(entity, t2);
                    container3.Put(entity, t3);
                    container4.Put(entity, t4);
                    container5.Put(entity, t5);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteForAll<T1>() 
            where T1 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Delete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteForAll<T1, T2>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Delete(entity);
                    container2.Delete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteForAll<T1, T2, T3>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Delete(entity);
                    container2.Delete(entity);
                    container3.Delete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteForAll<T1, T2, T3, T4>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Delete(entity);
                    container2.Delete(entity);
                    container3.Delete(entity);
                    container4.Delete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            ref var container5 = ref World<WorldType>.Components<T5>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Delete(entity);
                    container2.Delete(entity);
                    container3.Delete(entity);
                    container4.Delete(entity);
                    container5.Delete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteForAll<T1>() 
            where T1 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryDelete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteForAll<T1, T2>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryDelete(entity);
                    container2.TryDelete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteForAll<T1, T2, T3>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryDelete(entity);
                    container2.TryDelete(entity);
                    container3.TryDelete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteForAll<T1, T2, T3, T4>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryDelete(entity);
                    container2.TryDelete(entity);
                    container3.TryDelete(entity);
                    container4.TryDelete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            ref var container5 = ref World<WorldType>.Components<T5>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryDelete(entity);
                    container2.TryDelete(entity);
                    container3.TryDelete(entity);
                    container4.TryDelete(entity);
                    container5.TryDelete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        #endregion

        #region TAGS
        #if !FFS_ECS_DISABLE_TAGS
        [MethodImpl(AggressiveInlining)]
        public void SetTagForAll<T1>() where T1 : struct, ITag {
            ref var container = ref World<WorldType>.Tags<T1>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container.Set(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetTagForAll<T1, T2>() 
            where T1 : struct, ITag
            where T2 : struct, ITag {
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Set(entity);
                    container2.Set(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetTagForAll<T1, T2, T3>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag {
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            ref var container3 = ref World<WorldType>.Tags<T3>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Set(entity);
                    container2.Set(entity);
                    container3.Set(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetTagForAll<T1, T2, T3, T4>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag
            where T4 : struct, ITag {
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            ref var container3 = ref World<WorldType>.Tags<T3>.Value;
            ref var container4 = ref World<WorldType>.Tags<T4>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Set(entity);
                    container2.Set(entity);
                    container3.Set(entity);
                    container4.Set(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetTagForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag
            where T4 : struct, ITag
            where T5 : struct, ITag {
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            ref var container3 = ref World<WorldType>.Tags<T3>.Value;
            ref var container4 = ref World<WorldType>.Tags<T4>.Value;
            ref var container5 = ref World<WorldType>.Tags<T5>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Set(entity);
                    container2.Set(entity);
                    container3.Set(entity);
                    container4.Set(entity);
                    container5.Set(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteTagForAll<T1>() 
            where T1 : struct, ITag {
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Delete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteTagForAll<T1, T2>() 
            where T1 : struct, ITag
            where T2 : struct, ITag {
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Delete(entity);
                    container2.Delete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteTagForAll<T1, T2, T3>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag {
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            ref var container3 = ref World<WorldType>.Tags<T3>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Delete(entity);
                    container2.Delete(entity);
                    container3.Delete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteTagForAll<T1, T2, T3, T4>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag
            where T4 : struct, ITag {
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            ref var container3 = ref World<WorldType>.Tags<T3>.Value;
            ref var container4 = ref World<WorldType>.Tags<T4>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Delete(entity);
                    container2.Delete(entity);
                    container3.Delete(entity);
                    container4.Delete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteTagForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag
            where T4 : struct, ITag
            where T5 : struct, ITag {
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            ref var container3 = ref World<WorldType>.Tags<T3>.Value;
            ref var container4 = ref World<WorldType>.Tags<T4>.Value;
            ref var container5 = ref World<WorldType>.Tags<T5>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Delete(entity);
                    container2.Delete(entity);
                    container3.Delete(entity);
                    container4.Delete(entity);
                    container5.Delete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteTagForAll<T1>() 
            where T1 : struct, ITag {
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryDelete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteTagForAll<T1, T2>() 
            where T1 : struct, ITag
            where T2 : struct, ITag {
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryDelete(entity);
                    container2.TryDelete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteTagForAll<T1, T2, T3>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag {
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            ref var container3 = ref World<WorldType>.Tags<T3>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryDelete(entity);
                    container2.TryDelete(entity);
                    container3.TryDelete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteTagForAll<T1, T2, T3, T4>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag
            where T4 : struct, ITag {
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            ref var container3 = ref World<WorldType>.Tags<T3>.Value;
            ref var container4 = ref World<WorldType>.Tags<T4>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryDelete(entity);
                    container2.TryDelete(entity);
                    container3.TryDelete(entity);
                    container4.TryDelete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteTagForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag
            where T4 : struct, ITag
            where T5 : struct, ITag {
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            ref var container3 = ref World<WorldType>.Tags<T3>.Value;
            ref var container4 = ref World<WorldType>.Tags<T4>.Value;
            ref var container5 = ref World<WorldType>.Tags<T5>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryDelete(entity);
                    container2.TryDelete(entity);
                    container3.TryDelete(entity);
                    container4.TryDelete(entity);
                    container5.TryDelete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        #endif
        #endregion
        
        #region MASKS
        #if !FFS_ECS_DISABLE_MASKS
        [MethodImpl(AggressiveInlining)]
        public void SetMaskForAll<T1>() where T1 : struct, IMask {
            ref var container = ref World<WorldType>.Masks<T1>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container.Set(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMaskForAll<T1, T2>() 
            where T1 : struct, IMask
            where T2 : struct, IMask {
            ref var container1 = ref World<WorldType>.Masks<T1>.Value;
            ref var container2 = ref World<WorldType>.Masks<T2>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Set(entity);
                    container2.Set(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMaskForAll<T1, T2, T3>() 
            where T1 : struct, IMask
            where T2 : struct, IMask
            where T3 : struct, IMask {
            ref var container1 = ref World<WorldType>.Masks<T1>.Value;
            ref var container2 = ref World<WorldType>.Masks<T2>.Value;
            ref var container3 = ref World<WorldType>.Masks<T3>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Set(entity);
                    container2.Set(entity);
                    container3.Set(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMaskForAll<T1, T2, T3, T4>() 
            where T1 : struct, IMask
            where T2 : struct, IMask
            where T3 : struct, IMask
            where T4 : struct, IMask {
            ref var container1 = ref World<WorldType>.Masks<T1>.Value;
            ref var container2 = ref World<WorldType>.Masks<T2>.Value;
            ref var container3 = ref World<WorldType>.Masks<T3>.Value;
            ref var container4 = ref World<WorldType>.Masks<T4>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Set(entity);
                    container2.Set(entity);
                    container3.Set(entity);
                    container4.Set(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMaskForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IMask
            where T2 : struct, IMask
            where T3 : struct, IMask
            where T4 : struct, IMask
            where T5 : struct, IMask {
            ref var container1 = ref World<WorldType>.Masks<T1>.Value;
            ref var container2 = ref World<WorldType>.Masks<T2>.Value;
            ref var container3 = ref World<WorldType>.Masks<T3>.Value;
            ref var container4 = ref World<WorldType>.Masks<T4>.Value;
            ref var container5 = ref World<WorldType>.Masks<T5>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Set(entity);
                    container2.Set(entity);
                    container3.Set(entity);
                    container4.Set(entity);
                    container5.Set(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteMaskForAll<T1>() 
            where T1 : struct, IMask {
            ref var container1 = ref World<WorldType>.Masks<T1>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Delete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteMaskForAll<T1, T2>() 
            where T1 : struct, IMask
            where T2 : struct, IMask {
            ref var container1 = ref World<WorldType>.Masks<T1>.Value;
            ref var container2 = ref World<WorldType>.Masks<T2>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Delete(entity);
                    container2.Delete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteMaskForAll<T1, T2, T3>() 
            where T1 : struct, IMask
            where T2 : struct, IMask
            where T3 : struct, IMask {
            ref var container1 = ref World<WorldType>.Masks<T1>.Value;
            ref var container2 = ref World<WorldType>.Masks<T2>.Value;
            ref var container3 = ref World<WorldType>.Masks<T3>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Delete(entity);
                    container2.Delete(entity);
                    container3.Delete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteMaskForAll<T1, T2, T3, T4>() 
            where T1 : struct, IMask
            where T2 : struct, IMask
            where T3 : struct, IMask
            where T4 : struct, IMask {
            ref var container1 = ref World<WorldType>.Masks<T1>.Value;
            ref var container2 = ref World<WorldType>.Masks<T2>.Value;
            ref var container3 = ref World<WorldType>.Masks<T3>.Value;
            ref var container4 = ref World<WorldType>.Masks<T4>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Delete(entity);
                    container2.Delete(entity);
                    container3.Delete(entity);
                    container4.Delete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteMaskForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IMask
            where T2 : struct, IMask
            where T3 : struct, IMask
            where T4 : struct, IMask
            where T5 : struct, IMask {
            ref var container1 = ref World<WorldType>.Masks<T1>.Value;
            ref var container2 = ref World<WorldType>.Masks<T2>.Value;
            ref var container3 = ref World<WorldType>.Masks<T3>.Value;
            ref var container4 = ref World<WorldType>.Masks<T4>.Value;
            ref var container5 = ref World<WorldType>.Masks<T5>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.Delete(entity);
                    container2.Delete(entity);
                    container3.Delete(entity);
                    container4.Delete(entity);
                    container5.Delete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteMaskForAll<T1>() 
            where T1 : struct, IMask {
            ref var container1 = ref World<WorldType>.Masks<T1>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryDelete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteMaskForAll<T1, T2>() 
            where T1 : struct, IMask
            where T2 : struct, IMask {
            ref var container1 = ref World<WorldType>.Masks<T1>.Value;
            ref var container2 = ref World<WorldType>.Masks<T2>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryDelete(entity);
                    container2.TryDelete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteMaskForAll<T1, T2, T3>() 
            where T1 : struct, IMask
            where T2 : struct, IMask
            where T3 : struct, IMask {
            ref var container1 = ref World<WorldType>.Masks<T1>.Value;
            ref var container2 = ref World<WorldType>.Masks<T2>.Value;
            ref var container3 = ref World<WorldType>.Masks<T3>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryDelete(entity);
                    container2.TryDelete(entity);
                    container3.TryDelete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteMaskForAll<T1, T2, T3, T4>() 
            where T1 : struct, IMask
            where T2 : struct, IMask
            where T3 : struct, IMask
            where T4 : struct, IMask {
            ref var container1 = ref World<WorldType>.Masks<T1>.Value;
            ref var container2 = ref World<WorldType>.Masks<T2>.Value;
            ref var container3 = ref World<WorldType>.Masks<T3>.Value;
            ref var container4 = ref World<WorldType>.Masks<T4>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryDelete(entity);
                    container2.TryDelete(entity);
                    container3.TryDelete(entity);
                    container4.TryDelete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteMaskForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IMask
            where T2 : struct, IMask
            where T3 : struct, IMask
            where T4 : struct, IMask
            where T5 : struct, IMask {
            ref var container1 = ref World<WorldType>.Masks<T1>.Value;
            ref var container2 = ref World<WorldType>.Masks<T2>.Value;
            ref var container3 = ref World<WorldType>.Masks<T3>.Value;
            ref var container4 = ref World<WorldType>.Masks<T4>.Value;
            ref var container5 = ref World<WorldType>.Masks<T5>.Value;
            while (_count > 0) {
                _current = _entities[--_count];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == _entitiesStatus[_current].Value) && _queryMethod.CheckEntity(_current)) {
                    var entity = new World<WorldType>.Entity(_current);
                    container1.TryDelete(entity);
                    container2.TryDelete(entity);
                    container3.TryDelete(entity);
                    container4.TryDelete(entity);
                    container5.TryDelete(entity);
                }
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Dispose();
            #endif
        }
        #endif
        #endregion
    }
}