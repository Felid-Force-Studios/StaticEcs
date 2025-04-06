#if !FFS_ECS_DISABLE_MASKS
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
    internal struct MaskAllTypes<TMasks> : ISealedQueryMethod where TMasks : struct, IComponentMasks {
        private BitMask _bitMask;
        private byte _bufId;
        public TMasks _all;

        [MethodImpl(AggressiveInlining)]
        public MaskAllTypes(TMasks all) {
            _all = all;
            _bitMask = null;
            _bufId = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _all.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAll(entityId, _bufId);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id = Ecs<WorldType>.Masks<C1>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.Has(entityId, _id);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1, C2> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id1;
        private ushort _id2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id1 = Ecs<WorldType>.Masks<C1>.Value.id;
            _id2 = Ecs<WorldType>.Masks<C2>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.Has(entityId, _id1) && _bitMask.Has(entityId, _id2);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1, C2, C3> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id1;
        private ushort _id2;
        private ushort _id3;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id1 = Ecs<WorldType>.Masks<C1>.Value.id;
            _id2 = Ecs<WorldType>.Masks<C2>.Value.id;
            _id3 = Ecs<WorldType>.Masks<C3>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.Has(entityId, _id1) && _bitMask.Has(entityId, _id2) && _bitMask.Has(entityId, _id3);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1, C2, C3, C4> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            Ecs<WorldType>.ModuleMasks.MaskCache<Mask<C1, C2, C3, C4>>.Cache.This(out _bufId, out _count);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1, C2, C3, C4, C5> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            Ecs<WorldType>.ModuleMasks.MaskCache<Mask<C1, C2, C3, C4, C5>>.Cache.This(out _bufId, out _count);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1, C2, C3, C4, C5, C6> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            Ecs<WorldType>.ModuleMasks.MaskCache<Mask<C1, C2, C3, C4, C5, C6>>.Cache.This(out _bufId, out _count);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1, C2, C3, C4, C5, C6, C7> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask
        where C7 : struct, IMask {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            Ecs<WorldType>.ModuleMasks.MaskCache<Mask<C1, C2, C3, C4, C5, C6, C7>>.Cache.This(out _bufId, out _count);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAll<C1, C2, C3, C4, C5, C6, C7, C8> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask
        where C7 : struct, IMask
        where C8 : struct, IMask {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            Ecs<WorldType>.ModuleMasks.MaskCache<Mask<C1, C2, C3, C4, C5, C6, C7, C8>>.Cache.This(out _bufId, out _count);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }
    #endregion

    #region NONE
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct MaskNoneTypes<TMasks> : ISealedQueryMethod
        where TMasks : struct, IComponentMasks {
        private BitMask _bitMask;
        private byte _bufId;
        public TMasks _all;

        [MethodImpl(AggressiveInlining)]
        public MaskNoneTypes(TMasks all) {
            _all = all;
            _bitMask = null;
            _bufId = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _all.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAny(entityId, _bufId);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id = Ecs<WorldType>.Masks<C1>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return !_bitMask.Has(entityId, _id);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1, C2> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id1;
        private ushort _id2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id1 = Ecs<WorldType>.Masks<C1>.Value.id;
            _id2 = Ecs<WorldType>.Masks<C2>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return !_bitMask.Has(entityId, _id1) && !_bitMask.Has(entityId, _id2);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1, C2, C3> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id1;
        private ushort _id2;
        private ushort _id3;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id1 = Ecs<WorldType>.Masks<C1>.Value.id;
            _id2 = Ecs<WorldType>.Masks<C2>.Value.id;
            _id3 = Ecs<WorldType>.Masks<C3>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return !_bitMask.Has(entityId, _id1) && !_bitMask.Has(entityId, _id2) && !_bitMask.Has(entityId, _id3);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1, C2, C3, C4> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            Ecs<WorldType>.ModuleMasks.MaskCache<Mask<C1, C2, C3, C4>>.Cache.This(out _bufId, out _count);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1, C2, C3, C4, C5> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            Ecs<WorldType>.ModuleMasks.MaskCache<Mask<C1, C2, C3, C4, C5>>.Cache.This(out _bufId, out _count);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1, C2, C3, C4, C5, C6> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            Ecs<WorldType>.ModuleMasks.MaskCache<Mask<C1, C2, C3, C4, C5, C6>>.Cache.This(out _bufId, out _count);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1, C2, C3, C4, C5, C6, C7> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask
        where C7 : struct, IMask {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            Ecs<WorldType>.ModuleMasks.MaskCache<Mask<C1, C2, C3, C4, C5, C6, C7>>.Cache.This(out _bufId, out _count);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskNone<C1, C2, C3, C4, C5, C6, C7, C8> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask
        where C7 : struct, IMask
        where C8 : struct, IMask {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            Ecs<WorldType>.ModuleMasks.MaskCache<Mask<C1, C2, C3, C4, C5, C6, C7, C8>>.Cache.This(out _bufId, out _count);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }
    #endregion

    #region ANY
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct MaskAnyTypes<TMasks> : ISealedQueryMethod where TMasks : struct, IComponentMasks {
        private BitMask _bitMask;
        private byte _bufId;
        public TMasks _all;

        [MethodImpl(AggressiveInlining)]
        public MaskAnyTypes(TMasks all) {
            _all = all;
            _bitMask = null;
            _bufId = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _all.SetBitMask<WorldType>(_bufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAny(entityId, _bufId);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _bitMask.DropBuf();
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAny<C1, C2> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id1;
        private ushort _id2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id1 = Ecs<WorldType>.Masks<C1>.Value.id;
            _id2 = Ecs<WorldType>.Masks<C2>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.Has(entityId, _id1) || _bitMask.Has(entityId, _id2);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAny<C1, C2, C3> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask {
        private BitMask _bitMask;
        private ushort _id1;
        private ushort _id2;
        private ushort _id3;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            _id1 = Ecs<WorldType>.Masks<C1>.Value.id;
            _id2 = Ecs<WorldType>.Masks<C2>.Value.id;
            _id3 = Ecs<WorldType>.Masks<C3>.Value.id;
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.Has(entityId, _id1) || _bitMask.Has(entityId, _id2) || _bitMask.Has(entityId, _id3);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAny<C1, C2, C3, C4> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            Ecs<WorldType>.ModuleMasks.MaskCache<Mask<C1, C2, C3, C4>>.Cache.This(out _bufId, out _count);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAny<C1, C2, C3, C4, C5> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            Ecs<WorldType>.ModuleMasks.MaskCache<Mask<C1, C2, C3, C4, C5>>.Cache.This(out _bufId, out _count);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAny<C1, C2, C3, C4, C5, C6> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            Ecs<WorldType>.ModuleMasks.MaskCache<Mask<C1, C2, C3, C4, C5, C6>>.Cache.This(out _bufId, out _count);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAny<C1, C2, C3, C4, C5, C6, C7> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask
        where C7 : struct, IMask {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            Ecs<WorldType>.ModuleMasks.MaskCache<Mask<C1, C2, C3, C4, C5, C6, C7>>.Cache.This(out _bufId, out _count);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct MaskAny<C1, C2, C3, C4, C5, C6, C7, C8> : ISealedQueryMethod, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask
        where C7 : struct, IMask
        where C8 : struct, IMask {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleMasks.Value.BitMask;
            Ecs<WorldType>.ModuleMasks.MaskCache<Mask<C1, C2, C3, C4, C5, C6, C7, C8>>.Cache.This(out _bufId, out _count);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
        #endif
    }
    #endregion
}
#endif