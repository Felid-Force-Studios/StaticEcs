using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    #region ALL
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllTypes<TComponents> : IPrimaryQueryMethod, ISealedQueryMethod where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        public TComponents _all;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public AllTypes(TComponents all) {
            _all = all;
            _incBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _all.SetBitMask<WorldID>(_incBufId);
            _all.SetData<WorldID>(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAll(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _all.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent {
        private int[] m1;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            var types = default(Types<C1>);
            types.SetData<WorldID>(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            types.BlockComponents<WorldID>(1);
            #endif
            m1 = Ecs<WorldID>.Components<C1>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return m1[entityId] >= 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            var types = default(Types<C1>);
            types.BlockComponents<WorldID>(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1, C2> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent {
        private int[] m1;
        private int[] m2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            var types = default(Types<C1, C2>);
            types.SetData<WorldID>(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            types.BlockComponents<WorldID>(1);
            #endif
            m1 = Ecs<WorldID>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldID>.Components<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return m1[entityId] >= 0 && m2[entityId] >= 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            var types = default(Types<C1, C2>);
            types.BlockComponents<WorldID>(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1, C2, C3> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent {
        private int[] m1;
        private int[] m2;
        private int[] m3;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            var types = default(Types<C1, C2, C3>);
            types.SetData<WorldID>(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            types.BlockComponents<WorldID>(1);
            #endif
            m1 = Ecs<WorldID>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldID>.Components<C2>.Value.GetDataIdxByEntityId();
            m3 = Ecs<WorldID>.Components<C3>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return m1[entityId] >= 0 && m2[entityId] >= 0 && m3[entityId] >= 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            var types = default(Types<C1, C2, C3>);
            types.BlockComponents<WorldID>(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1, C2, C3, C4> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent {
        private int[] m1;
        private int[] m2;
        private int[] m3;
        private int[] m4;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            var types = default(Types<C1, C2, C3, C4>);
            types.SetData<WorldID>(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            types.BlockComponents<WorldID>(1);
            #endif
            m1 = Ecs<WorldID>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldID>.Components<C2>.Value.GetDataIdxByEntityId();
            m3 = Ecs<WorldID>.Components<C3>.Value.GetDataIdxByEntityId();
            m4 = Ecs<WorldID>.Components<C4>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return m1[entityId] >= 0 && m2[entityId] >= 0 && m3[entityId] >= 0 && m4[entityId] >= 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            var types = default(Types<C1, C2, C3, C4>);
            types.BlockComponents<WorldID>(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1, C2, C3, C4, C5> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
            _types.SetData<WorldID>(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAll(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1, C2, C3, C4, C5, C6> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5, C6> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
            _types.SetData<WorldID>(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAll(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1, C2, C3, C4, C5, C6, C7> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5, C6, C7> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
            _types.SetData<WorldID>(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAll(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct All<C1, C2, C3, C4, C5, C6, C7, C8> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5, C6, C7, C8> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
            _types.SetData<WorldID>(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAll(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    #endregion


    #region ALL_AND_NONE
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllAndNoneTypes<TComponentsIncluded, TComponentsExcluded> : IPrimaryQueryMethod, ISealedQueryMethod
        where TComponentsIncluded : struct, IComponentTypes
        where TComponentsExcluded : struct, IComponentTypes {
        private BitMask _bitMask;
        public TComponentsIncluded _all;
        public TComponentsExcluded _exc;
        private byte _incBufId;
        private byte _excBufId;
        
        [MethodImpl(AggressiveInlining)]
        public AllAndNoneTypes(TComponentsIncluded all, TComponentsExcluded exc) {
            _all = all;
            _exc = exc;
            _incBufId = default;
            _excBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _excBufId = _bitMask.BorrowBuf();

            _all.SetBitMask<WorldID>(_incBufId);
            _all.SetData<WorldID>(ref minCount, ref entities);

            _exc.SetBitMask<WorldID>(_excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAllAndExc(entityId, _incBufId, _excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _all.Dispose<WorldID>();
            _exc.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
            _bitMask.DropBuf();
        }
    }
    #endregion


    #region NONE
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneTypes<TComponents> : IQueryMethod, ISealedQueryMethod
        where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        public TComponents _exc;
        private byte _excBufId;
        
        [MethodImpl(AggressiveInlining)]
        public NoneTypes(TComponents exc) {
            _exc = exc;
            _excBufId = default;
            _bitMask = null;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _excBufId = _bitMask.BorrowBuf();
            _exc.SetBitMask<WorldID>(_excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.NotHasAny(entityId, _excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _exc.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1> : IQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent {
        private int[] m1;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            var types = default(Types<C1>);
            types.BlockComponents<WorldID>(1);
            #endif
            m1 = Ecs<WorldID>.Components<C1>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return m1[entityId] < 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            var types = default(Types<C1>);
            types.BlockComponents<WorldID>(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2> : IQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent {
        private int[] m1;
        private int[] m2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            var types = default(Types<C1, C2>);
            types.BlockComponents<WorldID>(1);
            #endif
            m1 = Ecs<WorldID>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldID>.Components<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return m1[entityId] < 0 && m2[entityId] < 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            var types = default(Types<C1, C2>);
            types.BlockComponents<WorldID>(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3> : IQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
            _types.SetData<WorldID>(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.NotHasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3, C4> : IQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
            _types.SetData<WorldID>(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.NotHasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }

    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3, C4, C5> : IQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
            _types.SetData<WorldID>(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.NotHasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3, C4, C5, C6> : IQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5, C6> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
            _types.SetData<WorldID>(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.NotHasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5, C6, C7> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
            _types.SetData<WorldID>(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.NotHasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5, C6, C7, C8> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
            _types.SetData<WorldID>(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.NotHasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    #endregion


    #region ANY
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyTypes<TComponents> : IQueryMethod, ISealedQueryMethod where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        public TComponents _any;
        private byte _anyBufId;
        
        [MethodImpl(AggressiveInlining)]
        public AnyTypes(TComponents any) {
            _any = any;
            _anyBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _anyBufId = _bitMask.BorrowBuf();
            _any.SetBitMask<WorldID>(_anyBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAny(entityId, _anyBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _any.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2> : IQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent {
        private int[] m1;
        private int[] m2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            var types = default(Types<C1, C2>);
            types.BlockComponents<WorldID>(1);
            #endif
            m1 = Ecs<WorldID>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldID>.Components<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return m1[entityId] >= 0 || m2[entityId] >= 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            var types = default(Types<C1, C2>);
            types.BlockComponents<WorldID>(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3> : IQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3, C4> : IQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }

    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3, C4, C5> : IQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3, C4, C5, C6> : IQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5, C6> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5, C6, C7> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5, C6, C7, C8> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            _bitMask = Ecs<WorldID>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldID>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldID>();
            #endif
            _bitMask.DropBuf();
        }
    }
    #endregion
}