#if !FFS_ECS_DISABLE_MASKS
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
    public struct MaskAllTypes<TMasks> : IQueryMethod, ISealedQueryMethod where TMasks : struct, IComponentMasks {
        private BitMask _bitMask;
        public TMasks _all;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public MaskAllTypes(TMasks all) {
            _all = all;
            _incBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _all.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAll(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id = Ecs<WorldType>.Masks<C1>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.Has(entityId, _id);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1, C2> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id1;
        private ushort _id2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id1 = Ecs<WorldType>.Masks<C1>.Value.id;
            _id2 = Ecs<WorldType>.Masks<C2>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.Has(entityId, _id1) && _bitMask.Has(entityId, _id2);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1, C2, C3> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id1;
        private ushort _id2;
        private ushort _id3;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id1 = Ecs<WorldType>.Masks<C1>.Value.id;
            _id2 = Ecs<WorldType>.Masks<C2>.Value.id;
            _id3 = Ecs<WorldType>.Masks<C3>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.Has(entityId, _id1) && _bitMask.Has(entityId, _id2) && _bitMask.Has(entityId, _id3);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1, C2, C3, C4> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask {
        private BitMask _bitMask;
        public Mask<C1, C2, C3, C4> _types;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAll(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1, C2, C3, C4, C5> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask {
        private BitMask _bitMask;
        public Mask<C1, C2, C3, C4, C5> _types;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAll(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1, C2, C3, C4, C5, C6> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask {
        private BitMask _bitMask;
        public Mask<C1, C2, C3, C4, C5, C6> _types;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAll(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask
        where C7 : struct, IMask {
        private BitMask _bitMask;
        public Mask<C1, C2, C3, C4, C5, C6, C7> _types;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAll(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask
        where C7 : struct, IMask
        where C8 : struct, IMask {
        private BitMask _bitMask;
        public Mask<C1, C2, C3, C4, C5, C6, C7, C8> _types;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAll(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }
    #endregion


    #region ALL_AND_NONE
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAllAndNoneTypes<TMasksIncluded, TMasksExcluded> : IQueryMethod, ISealedQueryMethod
        where TMasksIncluded : struct, IComponentMasks
        where TMasksExcluded : struct, IComponentMasks {
        private BitMask _bitMask;
        public TMasksIncluded _all;
        public TMasksExcluded _exc;
        private byte _incBufId;
        private byte _excBufId;

        [MethodImpl(AggressiveInlining)]
        public MaskAllAndNoneTypes(TMasksIncluded all, TMasksExcluded exc) {
            _all = all;
            _exc = exc;
            _incBufId = default;
            _excBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _excBufId = _bitMask.BorrowBuf();

            _all.SetMask<WorldType>(_incBufId);
            _exc.SetMask<WorldType>(_excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAllAndExc(entityId, _incBufId, _excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
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
    public struct MaskNoneTypes<TMasks> : IQueryMethod, ISealedQueryMethod
        where TMasks : struct, IComponentMasks {
        private BitMask _bitMask;
        public TMasks _exc;
        private byte _excBufId;

        [MethodImpl(AggressiveInlining)]
        public MaskNoneTypes(TMasks exc) {
            _exc = exc;
            _excBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _excBufId = _bitMask.BorrowBuf();
            _exc.SetMask<WorldType>(_excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.NotHasAny(entityId, _excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id = Ecs<WorldType>.Masks<C1>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return !_bitMask.Has(entityId, _id);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1, C2> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id1;
        private ushort _id2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id1 = Ecs<WorldType>.Masks<C1>.Value.id;
            _id2 = Ecs<WorldType>.Masks<C2>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return !_bitMask.Has(entityId, _id1) && !_bitMask.Has(entityId, _id2);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1, C2, C3> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id1;
        private ushort _id2;
        private ushort _id3;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id1 = Ecs<WorldType>.Masks<C1>.Value.id;
            _id2 = Ecs<WorldType>.Masks<C2>.Value.id;
            _id3 = Ecs<WorldType>.Masks<C3>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return !_bitMask.Has(entityId, _id1) && !_bitMask.Has(entityId, _id2) && !_bitMask.Has(entityId, _id3);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1, C2, C3, C4> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask {
        private BitMask _bitMask;
        public Mask<C1, C2, C3, C4> _types;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.NotHasAny(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1, C2, C3, C4, C5> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask {
        private BitMask _bitMask;
        public Mask<C1, C2, C3, C4, C5> _types;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.NotHasAny(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1, C2, C3, C4, C5, C6> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask {
        private BitMask _bitMask;
        public Mask<C1, C2, C3, C4, C5, C6> _types;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.NotHasAny(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask
        where C7 : struct, IMask {
        private BitMask _bitMask;
        public Mask<C1, C2, C3, C4, C5, C6, C7> _types;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.NotHasAny(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask
        where C7 : struct, IMask
        where C8 : struct, IMask {
        private BitMask _bitMask;
        public Mask<C1, C2, C3, C4, C5, C6, C7, C8> _types;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.NotHasAny(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }
    #endregion


    #region ANY
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAnyTypes<TMasks> : IQueryMethod, ISealedQueryMethod where TMasks : struct, IComponentMasks {
        private BitMask _bitMask;
        public TMasks _any;
        private byte _anyBufId;

        [MethodImpl(AggressiveInlining)]
        public MaskAnyTypes(TMasks any) {
            _any = any;
            _anyBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _anyBufId = _bitMask.BorrowBuf();
            _any.SetMask<WorldType>(_anyBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAny(entityId, _anyBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAny<C1, C2> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id1;
        private ushort _id2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id1 = Ecs<WorldType>.Masks<C1>.Value.id;
            _id2 = Ecs<WorldType>.Masks<C2>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.Has(entityId, _id1) || _bitMask.Has(entityId, _id2);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAny<C1, C2, C3> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id1;
        private ushort _id2;
        private ushort _id3;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id1 = Ecs<WorldType>.Masks<C1>.Value.id;
            _id2 = Ecs<WorldType>.Masks<C2>.Value.id;
            _id3 = Ecs<WorldType>.Masks<C3>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.Has(entityId, _id1) || _bitMask.Has(entityId, _id2) || _bitMask.Has(entityId, _id3);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAny<C1, C2, C3, C4> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask {
        private BitMask _bitMask;
        public Mask<C1, C2, C3, C4> _types;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAny(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAny<C1, C2, C3, C4, C5> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask {
        private BitMask _bitMask;
        public Mask<C1, C2, C3, C4, C5> _types;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAny(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAny<C1, C2, C3, C4, C5, C6> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask {
        private BitMask _bitMask;
        public Mask<C1, C2, C3, C4, C5, C6> _types;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAny(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAny<C1, C2, C3, C4, C5, C6, C7> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask
        where C7 : struct, IMask {
        private BitMask _bitMask;
        public Mask<C1, C2, C3, C4, C5, C6, C7> _types;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAny(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAny<C1, C2, C3, C4, C5, C6, C7, C8> : IQueryMethod, ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask
        where C7 : struct, IMask
        where C8 : struct, IMask {
        private BitMask _bitMask;
        public Mask<C1, C2, C3, C4, C5, C6, C7, C8> _types;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _bitMask.HasAny(entityId, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
    }
    #endregion
}
#endif