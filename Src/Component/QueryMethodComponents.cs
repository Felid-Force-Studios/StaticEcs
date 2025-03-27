using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _all.SetAllData<WorldType>(ref minCount, ref entities, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAll(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _all.Dispose<WorldType>();
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
        private uint[] m1;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyAndDisabledComponentMask) == 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
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
        private uint[] m1;
        private uint[] m2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyAndDisabledComponentMask) == 0 && (m2[entityId] & Const.EmptyAndDisabledComponentMask) == 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
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
        private uint[] m1;
        private uint[] m2;
        private uint[] m3;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            m3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyAndDisabledComponentMask) == 0 && (m2[entityId] & Const.EmptyAndDisabledComponentMask) == 0 && (m3[entityId] & Const.EmptyAndDisabledComponentMask) == 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
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
        private uint[] m1;
        private uint[] m2;
        private uint[] m3;
        private uint[] m4;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C4>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            m3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            m4 = Ecs<WorldType>.Components<C4>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyAndDisabledComponentMask) == 0 
                   && (m2[entityId] & Const.EmptyAndDisabledComponentMask) == 0 
                   && (m3[entityId] & Const.EmptyAndDisabledComponentMask) == 0 
                   && (m4[entityId] & Const.EmptyAndDisabledComponentMask) == 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C4>.Value.AddBlocker(-1);
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetAllData<WorldType>(ref minCount, ref entities, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAll(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetAllData<WorldType>(ref minCount, ref entities, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAll(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetAllData<WorldType>(ref minCount, ref entities, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAll(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetAllData<WorldType>(ref minCount, ref entities, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAll(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    #endregion

    #region ALL_ONLY_DISABLED
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabledTypes<TComponents> : IPrimaryQueryMethod, ISealedQueryMethod where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        public TComponents _all;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public AllOnlyDisabledTypes(TComponents all) {
            _all = all;
            _incBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _all.SetAllData<WorldType>(ref minCount, ref entities, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllOnlyDisabled(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _all.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent {
        private uint[] m1;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1, C2> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent {
        private uint[] m1;
        private uint[] m2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask && (m2[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1, C2, C3> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent {
        private uint[] m1;
        private uint[] m2;
        private uint[] m3;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            m3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask && (m2[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask && (m3[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1, C2, C3, C4> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent {
        private uint[] m1;
        private uint[] m2;
        private uint[] m3;
        private uint[] m4;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C4>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            m3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            m4 = Ecs<WorldType>.Components<C4>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask 
                   && (m2[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask 
                   && (m3[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask 
                   && (m4[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C4>.Value.AddBlocker(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1, C2, C3, C4, C5> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetAllData<WorldType>(ref minCount, ref entities, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllOnlyDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1, C2, C3, C4, C5, C6> : IPrimaryQueryMethod, ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetAllData<WorldType>(ref minCount, ref entities, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllOnlyDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1, C2, C3, C4, C5, C6, C7> : IPrimaryQueryMethod, ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetAllData<WorldType>(ref minCount, ref entities, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllOnlyDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllOnlyDisabled<C1, C2, C3, C4, C5, C6, C7, C8> : IPrimaryQueryMethod, ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetAllData<WorldType>(ref minCount, ref entities, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllOnlyDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    #endregion
    
    #region ALL_WITH_DISABLED
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabledTypes<TComponents> : IPrimaryQueryMethod, ISealedQueryMethod where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        public TComponents _all;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public AllWithDisabledTypes(TComponents all) {
            _all = all;
            _incBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _all.SetAllData<WorldType>(ref minCount, ref entities, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllWithDisabled(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _all.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent {
        private uint[] m1;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyComponentMask) == 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1, C2> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent {
        private uint[] m1;
        private uint[] m2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyComponentMask) == 0 && (m2[entityId] & Const.EmptyComponentMask) == 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1, C2, C3> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent {
        private uint[] m1;
        private uint[] m2;
        private uint[] m3;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            m3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyComponentMask) == 0 && (m2[entityId] & Const.EmptyComponentMask) == 0 && (m3[entityId] & Const.EmptyComponentMask) == 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1, C2, C3, C4> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent {
        private uint[] m1;
        private uint[] m2;
        private uint[] m3;
        private uint[] m4;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C4>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            m3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            m4 = Ecs<WorldType>.Components<C4>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyComponentMask) == 0
                   && (m2[entityId] & Const.EmptyComponentMask) == 0
                   && (m3[entityId] & Const.EmptyComponentMask) == 0
                   && (m4[entityId] & Const.EmptyComponentMask) == 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C4>.Value.AddBlocker(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1, C2, C3, C4, C5> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetAllData<WorldType>(ref minCount, ref entities, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1, C2, C3, C4, C5, C6> : IPrimaryQueryMethod, ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetAllData<WorldType>(ref minCount, ref entities, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1, C2, C3, C4, C5, C6, C7> : IPrimaryQueryMethod, ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetAllData<WorldType>(ref minCount, ref entities, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllWithDisabled<C1, C2, C3, C4, C5, C6, C7, C8> : IPrimaryQueryMethod, ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetAllData<WorldType>(ref minCount, ref entities, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _excBufId = _bitMask.BorrowBuf();

            _all.SetAllData<WorldType>(ref minCount, ref entities, _incBufId);
            _exc.SetBitMask<WorldType>(_excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllAndExc(entityId, _incBufId, _excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _all.Dispose<WorldType>();
            _exc.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllAndNoneOnlyDisabledTypes<TComponentsIncluded, TComponentsExcluded> : IPrimaryQueryMethod, ISealedQueryMethod
        where TComponentsIncluded : struct, IComponentTypes
        where TComponentsExcluded : struct, IComponentTypes {
        private BitMask _bitMask;
        public TComponentsIncluded _all;
        public TComponentsExcluded _exc;
        private byte _incBufId;
        private byte _excBufId;
        
        [MethodImpl(AggressiveInlining)]
        public AllAndNoneOnlyDisabledTypes(TComponentsIncluded all, TComponentsExcluded exc) {
            _all = all;
            _exc = exc;
            _incBufId = default;
            _excBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _excBufId = _bitMask.BorrowBuf();

            _all.SetAllData<WorldType>(ref minCount, ref entities, _incBufId);
            _exc.SetBitMask<WorldType>(_excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllAndExcOnlyDisabled(entityId, _incBufId, _excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _all.Dispose<WorldType>();
            _exc.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AllAndNoneWithDisabledTypes<TComponentsIncluded, TComponentsExcluded> : IPrimaryQueryMethod, ISealedQueryMethod
        where TComponentsIncluded : struct, IComponentTypes
        where TComponentsExcluded : struct, IComponentTypes {
        private BitMask _bitMask;
        public TComponentsIncluded _all;
        public TComponentsExcluded _exc;
        private byte _incBufId;
        private byte _excBufId;
        
        [MethodImpl(AggressiveInlining)]
        public AllAndNoneWithDisabledTypes(TComponentsIncluded all, TComponentsExcluded exc) {
            _all = all;
            _exc = exc;
            _incBufId = default;
            _excBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _excBufId = _bitMask.BorrowBuf();

            _all.SetAllData<WorldType>(ref minCount, ref entities, _incBufId);
            _exc.SetBitMask<WorldType>(_excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllAndExcWithDisabled(entityId, _incBufId, _excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _all.Dispose<WorldType>();
            _exc.Dispose<WorldType>();
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
    public struct NoneTypes<TComponents> : ISealedQueryMethod
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _excBufId = _bitMask.BorrowBuf();
            _exc.SetBitMask<WorldType>(_excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAny(entityId, _excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _exc.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1> : ISealedQueryMethod 
        where C1 : struct, IComponent {
        private uint[] m1;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyAndDisabledComponentMask) > 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent {
        private uint[] m1;
        private uint[] m2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyAndDisabledComponentMask) > 0 && (m2[entityId] & Const.EmptyAndDisabledComponentMask) > 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3, C4> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }

    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3, C4, C5> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3, C4, C5, C6> : ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3, C4, C5, C6, C7> : ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct None<C1, C2, C3, C4, C5, C6, C7, C8> : ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    #endregion
    
    #region NONE_WITH_DISABLED
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabledTypes<TComponents> : ISealedQueryMethod
        where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        public TComponents _exc;
        private byte _excBufId;
        
        [MethodImpl(AggressiveInlining)]
        public NoneWithDisabledTypes(TComponents exc) {
            _exc = exc;
            _excBufId = default;
            _bitMask = null;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _excBufId = _bitMask.BorrowBuf();
            _exc.SetBitMask<WorldType>(_excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyWithDisabled(entityId, _excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _exc.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1> : ISealedQueryMethod 
        where C1 : struct, IComponent {
        private uint[] m1;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyComponentMask) == Const.EmptyComponentMask;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1, C2> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent {
        private uint[] m1;
        private uint[] m2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyComponentMask) == Const.EmptyComponentMask && (m2[entityId] & Const.EmptyComponentMask) == Const.EmptyComponentMask;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1, C2, C3> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1, C2, C3, C4> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }

    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1, C2, C3, C4, C5> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1, C2, C3, C4, C5, C6> : ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1, C2, C3, C4, C5, C6, C7> : ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct NoneWithDisabled<C1, C2, C3, C4, C5, C6, C7, C8> : ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
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
    public struct AnyTypes<TComponents> : ISealedQueryMethod where TComponents : struct, IComponentTypes {
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _anyBufId = _bitMask.BorrowBuf();
            _any.SetBitMask<WorldType>(_anyBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAny(entityId, _anyBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _any.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent {
        private uint[] m1;
        private uint[] m2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyAndDisabledComponentMask) == 0 || (m2[entityId] & Const.EmptyAndDisabledComponentMask) == 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3, C4> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }

    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3, C4, C5> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3, C4, C5, C6> : ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3, C4, C5, C6, C7> : ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Any<C1, C2, C3, C4, C5, C6, C7, C8> : ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAny(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    #endregion
    
    #region ANY_ONLY_DISABLED
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyOnlyDisabledTypes<TComponents> : ISealedQueryMethod where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        public TComponents _any;
        private byte _anyBufId;
        
        [MethodImpl(AggressiveInlining)]
        public AnyOnlyDisabledTypes(TComponents any) {
            _any = any;
            _anyBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _anyBufId = _bitMask.BorrowBuf();
            _any.SetBitMask<WorldType>(_anyBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyOnlyDisabled(entityId, _anyBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _any.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyOnlyDisabled<C1, C2> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent {
        private uint[] m1;
        private uint[] m2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask || (m2[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyOnlyDisabled<C1, C2, C3> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyOnlyDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyOnlyDisabled<C1, C2, C3, C4> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyOnlyDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }

    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyOnlyDisabled<C1, C2, C3, C4, C5> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyOnlyDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyOnlyDisabled<C1, C2, C3, C4, C5, C6> : ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyOnlyDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyOnlyDisabled<C1, C2, C3, C4, C5, C6, C7> : ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyOnlyDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyOnlyDisabled<C1, C2, C3, C4, C5, C6, C7, C8> : ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyOnlyDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    #endregion
    
    #region ANY_WITH_DISABLED
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyWithDisabledTypes<TComponents> : ISealedQueryMethod where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        public TComponents _any;
        private byte _anyBufId;
        
        [MethodImpl(AggressiveInlining)]
        public AnyWithDisabledTypes(TComponents any) {
            _any = any;
            _anyBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _anyBufId = _bitMask.BorrowBuf();
            _any.SetBitMask<WorldType>(_anyBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyWithDisabled(entityId, _anyBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _any.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyWithDisabled<C1, C2> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent {
        private uint[] m1;
        private uint[] m2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyComponentMask) == 0 || (m2[entityId] & Const.EmptyComponentMask) == 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyWithDisabled<C1, C2, C3> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyWithDisabled<C1, C2, C3, C4> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }

    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyWithDisabled<C1, C2, C3, C4, C5> : ISealedQueryMethod 
        where C1 : struct, IComponent 
        where C2 : struct, IComponent 
        where C3 : struct, IComponent 
        where C4 : struct, IComponent
        where C5 : struct, IComponent {
        private BitMask _bitMask;
        private byte _bufId;
        private Types<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyWithDisabled<C1, C2, C3, C4, C5, C6> : ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyWithDisabled<C1, C2, C3, C4, C5, C6, C7> : ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct AnyWithDisabled<C1, C2, C3, C4, C5, C6, C7, C8> : ISealedQueryMethod 
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
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyWithDisabled(entityId, _bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Dispose<WorldType>();
            #endif
            _bitMask.DropBuf();
        }
    }
    #endregion
}