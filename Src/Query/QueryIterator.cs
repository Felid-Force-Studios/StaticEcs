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
    public ref struct QueryEntitiesIterator<WorldID, QM> where QM : struct, IPrimaryQueryMethod where WorldID : struct, IWorldId {
        private readonly int[] _entities; //8
        private int _current;             //4
        private int _count;               //4
        private QM _queryMethod;          //???

        [MethodImpl(AggressiveInlining)]
        public QueryEntitiesIterator(QM queryMethod) {
            _queryMethod = queryMethod;
            _current = default;
            _entities = default;
            _count = int.MaxValue;
            _queryMethod.SetData<WorldID>(ref _count, ref _entities);
        }

        public readonly Ecs<WorldID>.Entity Current {
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
                if (_queryMethod.CheckEntity(_current)) {
                    return true;
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public readonly QueryEntitiesIterator<WorldID, QM> GetEnumerator() => this;

        [MethodImpl(AggressiveInlining)]
        public void Dispose() => _queryMethod.Dispose<WorldID>();
        
        [MethodImpl(AggressiveInlining)]
        public void DestroyAllEntities() {
            while (MoveNext()) {
                new Ecs<WorldID>.Entity(_current).Destroy();
            }
            Dispose();
        }

        [MethodImpl(AggressiveInlining)]
        public bool First(out Ecs<WorldID>.Entity entity) {
            var moveNext = MoveNext();
            entity = new Ecs<WorldID>.Entity(_current);
            Dispose();
            return moveNext;
        }
        
        [MethodImpl(AggressiveInlining)]
        public int EntitiesCount() {
            var count = 0;
            while (MoveNext()) {
                count++;
            }
            Dispose();
            return count;
        }

        #region COMPONENTS
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1>() where T1 : struct, IComponent {
            ref var components = ref Ecs<WorldID>.Components<T1>.Value;
            while (MoveNext()) {
                components.Add(new Ecs<WorldID>.Entity(_current));
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent{
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Add(entity);
                container2.Add(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Components<T3>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Add(entity);
                container2.Add(entity);
                container3.Add(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3, T4>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Components<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Components<T4>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Add(entity);
                container2.Add(entity);
                container3.Add(entity);
                container4.Add(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Components<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Components<T4>.Value;
            ref var container5 = ref Ecs<WorldID>.Components<T5>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Add(entity);
                container2.Add(entity);
                container3.Add(entity);
                container4.Add(entity);
                container5.Add(entity);
            }
            Dispose();
        }
        
                [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1>() where T1 : struct, IComponent {
            ref var components = ref Ecs<WorldID>.Components<T1>.Value;
            while (MoveNext()) {
                components.TryAdd(new Ecs<WorldID>.Entity(_current));
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent{
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.TryAdd(entity);
                container2.TryAdd(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2, T3>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Components<T3>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.TryAdd(entity);
                container2.TryAdd(entity);
                container3.TryAdd(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2, T3, T4>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Components<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Components<T4>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.TryAdd(entity);
                container2.TryAdd(entity);
                container3.TryAdd(entity);
                container4.TryAdd(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Components<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Components<T4>.Value;
            ref var container5 = ref Ecs<WorldID>.Components<T5>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.TryAdd(entity);
                container2.TryAdd(entity);
                container3.TryAdd(entity);
                container4.TryAdd(entity);
                container5.TryAdd(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1>(T1 t1) 
            where T1 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Add(entity) = t1;
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2>(T1 t1, T2 t2) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Add(entity) = t1;
                container2.Add(entity) = t2;
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3>(T1 t1, T2 t2, T3 t3) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Components<T3>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Add(entity) = t1;
                container2.Add(entity) = t2;
                container3.Add(entity) = t3;
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Components<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Components<T4>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Add(entity) = t1;
                container2.Add(entity) = t2;
                container3.Add(entity) = t3;
                container4.Add(entity) = t4;
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Components<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Components<T4>.Value;
            ref var container5 = ref Ecs<WorldID>.Components<T5>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Add(entity) = t1;
                container2.Add(entity) = t2;
                container3.Add(entity) = t3;
                container4.Add(entity) = t4;
                container5.Add(entity) = t5;
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void PutForAll<T1>(T1 t1) 
            where T1 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Put(entity, t1);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void PutForAll<T1, T2>(T1 t1, T2 t2) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Put(entity, t1);
                container2.Put(entity, t2);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void PutForAll<T1, T2, T3>(T1 t1, T2 t2, T3 t3) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Components<T3>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Put(entity, t1);
                container2.Put(entity, t2);
                container3.Put(entity, t3);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void PutForAll<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Components<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Components<T4>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Put(entity, t1);
                container2.Put(entity, t2);
                container3.Put(entity, t3);
                container4.Put(entity, t4);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void PutForAll<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Components<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Components<T4>.Value;
            ref var container5 = ref Ecs<WorldID>.Components<T5>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Put(entity, t1);
                container2.Put(entity, t2);
                container3.Put(entity, t3);
                container4.Put(entity, t4);
                container5.Put(entity, t5);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteForAll<T1>() 
            where T1 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Delete(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteForAll<T1, T2>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Delete(entity);
                container2.Delete(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteForAll<T1, T2, T3>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Components<T3>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Delete(entity);
                container2.Delete(entity);
                container3.Delete(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteForAll<T1, T2, T3, T4>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Components<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Components<T4>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Delete(entity);
                container2.Delete(entity);
                container3.Delete(entity);
                container4.Delete(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            ref var container1 = ref Ecs<WorldID>.Components<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Components<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Components<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Components<T4>.Value;
            ref var container5 = ref Ecs<WorldID>.Components<T5>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Delete(entity);
                container2.Delete(entity);
                container3.Delete(entity);
                container4.Delete(entity);
                container5.Delete(entity);
            }
            Dispose();
        }
        #endregion

        #region TAGS
        #if !FFS_ECS_DISABLE_TAGS
        [MethodImpl(AggressiveInlining)]
        public void SetTagForAll<T1>() where T1 : struct, ITag {
            ref var container = ref Ecs<WorldID>.Tags<T1>.Value;
            while (MoveNext()) {
                container.Set(new Ecs<WorldID>.Entity(_current));
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetTagForAll<T1, T2>() 
            where T1 : struct, ITag
            where T2 : struct, ITag {
            ref var container1 = ref Ecs<WorldID>.Tags<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Tags<T2>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Set(entity);
                container2.Set(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetTagForAll<T1, T2, T3>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag {
            ref var container1 = ref Ecs<WorldID>.Tags<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Tags<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Tags<T3>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Set(entity);
                container2.Set(entity);
                container3.Set(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetTagForAll<T1, T2, T3, T4>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag
            where T4 : struct, ITag {
            ref var container1 = ref Ecs<WorldID>.Tags<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Tags<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Tags<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Tags<T4>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Set(entity);
                container2.Set(entity);
                container3.Set(entity);
                container4.Set(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetTagForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag
            where T4 : struct, ITag
            where T5 : struct, ITag {
            ref var container1 = ref Ecs<WorldID>.Tags<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Tags<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Tags<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Tags<T4>.Value;
            ref var container5 = ref Ecs<WorldID>.Tags<T5>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Set(entity);
                container2.Set(entity);
                container3.Set(entity);
                container4.Set(entity);
                container5.Set(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteTagForAll<T1>() 
            where T1 : struct, ITag {
            ref var container1 = ref Ecs<WorldID>.Tags<T1>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Delete(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteTagForAll<T1, T2>() 
            where T1 : struct, ITag
            where T2 : struct, ITag {
            ref var container1 = ref Ecs<WorldID>.Tags<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Tags<T2>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Delete(entity);
                container2.Delete(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteTagForAll<T1, T2, T3>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag {
            ref var container1 = ref Ecs<WorldID>.Tags<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Tags<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Tags<T3>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Delete(entity);
                container2.Delete(entity);
                container3.Delete(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteTagForAll<T1, T2, T3, T4>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag
            where T4 : struct, ITag {
            ref var container1 = ref Ecs<WorldID>.Tags<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Tags<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Tags<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Tags<T4>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Delete(entity);
                container2.Delete(entity);
                container3.Delete(entity);
                container4.Delete(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteTagForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag
            where T4 : struct, ITag
            where T5 : struct, ITag {
            ref var container1 = ref Ecs<WorldID>.Tags<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Tags<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Tags<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Tags<T4>.Value;
            ref var container5 = ref Ecs<WorldID>.Tags<T5>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Delete(entity);
                container2.Delete(entity);
                container3.Delete(entity);
                container4.Delete(entity);
                container5.Delete(entity);
            }
            Dispose();
        }
        #endif
        #endregion
        
        #region MASKS
        #if !FFS_ECS_DISABLE_MASKS
        [MethodImpl(AggressiveInlining)]
        public void SetMaskForAll<T1>() where T1 : struct, IMask {
            ref var container = ref Ecs<WorldID>.Masks<T1>.Value;
            while (MoveNext()) {
                container.Set(new Ecs<WorldID>.Entity(_current));
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMaskForAll<T1, T2>() 
            where T1 : struct, IMask
            where T2 : struct, IMask {
            ref var container1 = ref Ecs<WorldID>.Masks<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Masks<T2>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Set(entity);
                container2.Set(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMaskForAll<T1, T2, T3>() 
            where T1 : struct, IMask
            where T2 : struct, IMask
            where T3 : struct, IMask {
            ref var container1 = ref Ecs<WorldID>.Masks<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Masks<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Masks<T3>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Set(entity);
                container2.Set(entity);
                container3.Set(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMaskForAll<T1, T2, T3, T4>() 
            where T1 : struct, IMask
            where T2 : struct, IMask
            where T3 : struct, IMask
            where T4 : struct, IMask {
            ref var container1 = ref Ecs<WorldID>.Masks<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Masks<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Masks<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Masks<T4>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Set(entity);
                container2.Set(entity);
                container3.Set(entity);
                container4.Set(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMaskForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IMask
            where T2 : struct, IMask
            where T3 : struct, IMask
            where T4 : struct, IMask
            where T5 : struct, IMask {
            ref var container1 = ref Ecs<WorldID>.Masks<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Masks<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Masks<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Masks<T4>.Value;
            ref var container5 = ref Ecs<WorldID>.Masks<T5>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Set(entity);
                container2.Set(entity);
                container3.Set(entity);
                container4.Set(entity);
                container5.Set(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteMaskForAll<T1>() 
            where T1 : struct, IMask {
            ref var container1 = ref Ecs<WorldID>.Masks<T1>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Delete(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteMaskForAll<T1, T2>() 
            where T1 : struct, IMask
            where T2 : struct, IMask {
            ref var container1 = ref Ecs<WorldID>.Masks<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Masks<T2>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Delete(entity);
                container2.Delete(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteMaskForAll<T1, T2, T3>() 
            where T1 : struct, IMask
            where T2 : struct, IMask
            where T3 : struct, IMask {
            ref var container1 = ref Ecs<WorldID>.Masks<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Masks<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Masks<T3>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Delete(entity);
                container2.Delete(entity);
                container3.Delete(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteMaskForAll<T1, T2, T3, T4>() 
            where T1 : struct, IMask
            where T2 : struct, IMask
            where T3 : struct, IMask
            where T4 : struct, IMask {
            ref var container1 = ref Ecs<WorldID>.Masks<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Masks<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Masks<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Masks<T4>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Delete(entity);
                container2.Delete(entity);
                container3.Delete(entity);
                container4.Delete(entity);
            }
            Dispose();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteMaskForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IMask
            where T2 : struct, IMask
            where T3 : struct, IMask
            where T4 : struct, IMask
            where T5 : struct, IMask {
            ref var container1 = ref Ecs<WorldID>.Masks<T1>.Value;
            ref var container2 = ref Ecs<WorldID>.Masks<T2>.Value;
            ref var container3 = ref Ecs<WorldID>.Masks<T3>.Value;
            ref var container4 = ref Ecs<WorldID>.Masks<T4>.Value;
            ref var container5 = ref Ecs<WorldID>.Masks<T5>.Value;
            while (MoveNext()) {
                var entity = new Ecs<WorldID>.Entity(_current);
                container1.Delete(entity);
                container2.Delete(entity);
                container3.Delete(entity);
                container4.Delete(entity);
                container5.Delete(entity);
            }
            Dispose();
        }
        #endif
        #endregion
    }
}