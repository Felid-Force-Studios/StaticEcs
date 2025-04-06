#if !FFS_ECS_DISABLE_TAGS
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
    internal struct TagAllTypes<TComponents> : IPrimaryQueryMethod, ISealedQueryMethod where TComponents : struct, IComponentTags {
        private BitMask _bitMask;
        public TComponents _all;
        private byte _bufId;

        [MethodImpl(AggressiveInlining)]
        public TagAllTypes(TComponents all) {
            _all = all;
            _bitMask = null;
            _bufId = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _all.SetMinData<WorldType>(ref minCount, ref entities);
            _all.SetBitMask<WorldType>(_bufId);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _all.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAll(entityId, _bufId);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _all.Block<WorldType>(-1);
            _bitMask.DropBuf();
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAll<C1> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, ITag {
        private uint[] m1;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Tags<C1>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return m1[entityId] != Const.EmptyComponentMask;
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAll<C1, C2> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag {
        private uint[] m1;
        private uint[] m2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Tags<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Tags<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return m1[entityId] != Const.EmptyComponentMask && m2[entityId] != Const.EmptyComponentMask;
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAll<C1, C2, C3> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag {
        private uint[] m1;
        private uint[] m2;
        private uint[] m3;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(1);
            Ecs<WorldType>.Tags<C3>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Tags<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Tags<C2>.Value.GetDataIdxByEntityId();
            m3 = Ecs<WorldType>.Tags<C3>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return m1[entityId] != Const.EmptyComponentMask && m2[entityId] != Const.EmptyComponentMask && m3[entityId] != Const.EmptyComponentMask;
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(-1);
            Ecs<WorldType>.Tags<C3>.Value.AddBlocker(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAll<C1, C2, C3, C4> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag {
        private uint[] m1;
        private uint[] m2;
        private uint[] m3;
        private uint[] m4;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(1);
            Ecs<WorldType>.Tags<C3>.Value.AddBlocker(1);
            Ecs<WorldType>.Tags<C4>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Tags<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Tags<C2>.Value.GetDataIdxByEntityId();
            m3 = Ecs<WorldType>.Tags<C3>.Value.GetDataIdxByEntityId();
            m4 = Ecs<WorldType>.Tags<C4>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return m1[entityId] != Const.EmptyComponentMask && m2[entityId] != Const.EmptyComponentMask && m3[entityId] != Const.EmptyComponentMask && m4[entityId] != Const.EmptyComponentMask;
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(-1);
            Ecs<WorldType>.Tags<C3>.Value.AddBlocker(-1);
            Ecs<WorldType>.Tags<C4>.Value.AddBlocker(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAll<C1, C2, C3, C4, C5> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;
        private Tag<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3, C4, C5>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _types.Block<WorldType>(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAll<C1, C2, C3, C4, C5, C6> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;
        private Tag<C1, C2, C3, C4, C5, C6> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3, C4, C5, C6>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _types.Block<WorldType>(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAll<C1, C2, C3, C4, C5, C6, C7> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;
        private Tag<C1, C2, C3, C4, C5, C6, C7> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3, C4, C5, C6, C7>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _types.Block<WorldType>(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAll<C1, C2, C3, C4, C5, C6, C7, C8> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag
        where C8 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;
        private Tag<C1, C2, C3, C4, C5, C6, C7, C8> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3, C4, C5, C6, C7, C8>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _types.Block<WorldType>(-1);
        }
        #endif
    }
    #endregion

    #region NONE
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct TagNoneTypes<TTags> : ISealedQueryMethod
        where TTags : struct, IComponentTags {
        private BitMask _bitMask;
        public TTags _all;
        private byte _bufId;

        [MethodImpl(AggressiveInlining)]
        public TagNoneTypes(TTags all) {
            _all = all;
            _bitMask = null;
            _bufId = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _all.SetBitMask<WorldType>(_bufId);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _all.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAny(entityId, _bufId);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _all.Block<WorldType>(-1);
            _bitMask.DropBuf();
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1> : ISealedQueryMethod 
        where C1 : struct, ITag {
        private uint[] m1;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Tag<C1>).Block<WorldType>(1);
            #endif
            m1 = Ecs<WorldType>.Tags<C1>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return m1[entityId] == Const.EmptyComponentMask;
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Tag<C1>).Block<WorldType>(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1, C2> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag {
        private uint[] m1;
        private uint[] m2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Tag<C1, C2>).Block<WorldType>(1);
            #endif
            m1 = Ecs<WorldType>.Tags<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Tags<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return m1[entityId] == Const.EmptyComponentMask && m2[entityId] == Const.EmptyComponentMask;
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Tag<C1, C2>).Block<WorldType>(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1, C2, C3> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Tag<C1, C2, C3>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Tag<C1, C2, C3>).Block<WorldType>(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1, C2, C3, C4> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3, C4>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Tag<C1, C2, C3, C4>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Tag<C1, C2, C3, C4>).Block<WorldType>(-1);
        }
        #endif
    }

    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1, C2, C3, C4, C5> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3, C4, C5>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Tag<C1, C2, C3, C4, C5>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Tag<C1, C2, C3, C4, C5>).Block<WorldType>(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1, C2, C3, C4, C5, C6> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3, C4, C5, C6>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Tag<C1, C2, C3, C4, C5, C6>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Tag<C1, C2, C3, C4, C5, C6>).Block<WorldType>(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1, C2, C3, C4, C5, C6, C7> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3, C4, C5, C6, C7>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Tag<C1, C2, C3, C4, C5, C6, C7>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Tag<C1, C2, C3, C4, C5, C6, C7>).Block<WorldType>(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<C1, C2, C3, C4, C5, C6, C7, C8> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag
        where C8 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3, C4, C5, C6, C7, C8>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Tag<C1, C2, C3, C4, C5, C6, C7, C8>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Tag<C1, C2, C3, C4, C5, C6, C7, C8>).Block<WorldType>(-1);
        }
        #endif
    }
    #endregion

    #region ANY
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct TagAnyTypes<TTags> : ISealedQueryMethod where TTags : struct, IComponentTags {
        private BitMask _bitMask;
        public TTags _all;
        private byte _bufId;

        [MethodImpl(AggressiveInlining)]
        public TagAnyTypes(TTags all) {
            _all = all;
            _bitMask = null;
            _bufId = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _all.SetBitMask<WorldType>(_bufId);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _all.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAny(entityId, _bufId);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _all.Block<WorldType>(-1);
            _bitMask.DropBuf();
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAny<C1, C2> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag {
        private uint[] m1;
        private uint[] m2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Tag<C1, C2>).Block<WorldType>(1);
            #endif
            m1 = Ecs<WorldType>.Tags<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Tags<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return m1[entityId] != Const.EmptyComponentMask || m2[entityId] != Const.EmptyComponentMask;
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Tag<C1, C2>).Block<WorldType>(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAny<C1, C2, C3> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Tag<C1, C2, C3>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Tag<C1, C2, C3>).Block<WorldType>(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAny<C1, C2, C3, C4> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3, C4>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Tag<C1, C2, C3, C4>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Tag<C1, C2, C3, C4>).Block<WorldType>(-1);
        }
        #endif
    }

    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAny<C1, C2, C3, C4, C5> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3, C4, C5>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Tag<C1, C2, C3, C4, C5>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Tag<C1, C2, C3, C4, C5>).Block<WorldType>(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAny<C1, C2, C3, C4, C5, C6> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3, C4, C5, C6>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Tag<C1, C2, C3, C4, C5, C6>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Tag<C1, C2, C3, C4, C5, C6>).Block<WorldType>(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAny<C1, C2, C3, C4, C5, C6, C7> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3, C4, C5, C6, C7>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Tag<C1, C2, C3, C4, C5, C6, C7>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Tag<C1, C2, C3, C4, C5, C6, C7>).Block<WorldType>(-1);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAny<C1, C2, C3, C4, C5, C6, C7, C8> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag
        where C8 : struct, ITag {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            Ecs<WorldType>.ModuleTags.MaskCache<Tag<C1, C2, C3, C4, C5, C6, C7, C8>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Tag<C1, C2, C3, C4, C5, C6, C7, C8>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Tag<C1, C2, C3, C4, C5, C6, C7, C8>).Block<WorldType>(-1);
        }
        #endif
    }
    #endregion
}
#endif