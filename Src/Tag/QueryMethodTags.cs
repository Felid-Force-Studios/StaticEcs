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
    public struct TagAllTypes<TComponents> : IPrimaryQueryMethod, ISealedQueryMethod where TComponents : struct, IComponentTags {
        private BitMask _bitMask;
        public TComponents _all;
        private byte _incBufId;

        [MethodImpl(AggressiveInlining)]
        public TagAllTypes(TComponents all) {
            _all = all;
            _incBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
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
    public struct TagAll<C1> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, ITag {
        private uint[] m1;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Tags<C1>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return m1[entityId] != Utils.EmptyComponent;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(-1);
            #endif
        }
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
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Tags<C2>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(1);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Tags<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Tags<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return m1[entityId] != Utils.EmptyComponent && m2[entityId] != Utils.EmptyComponent;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(-1);
            #endif
        }
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
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Tags<C2>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Tags<C3>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
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
            return m1[entityId] != Utils.EmptyComponent && m2[entityId] != Utils.EmptyComponent && m3[entityId] != Utils.EmptyComponent;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(-1);
            Ecs<WorldType>.Tags<C3>.Value.AddBlocker(-1);
            #endif
        }
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
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Tags<C2>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Tags<C3>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
            Ecs<WorldType>.Tags<C4>.Value.SetDataIfCountLess(ref minCount, ref entities, out var _);
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
            return m1[entityId] != Utils.EmptyComponent && m2[entityId] != Utils.EmptyComponent && m3[entityId] != Utils.EmptyComponent && m4[entityId] != Utils.EmptyComponent;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(-1);
            Ecs<WorldType>.Tags<C3>.Value.AddBlocker(-1);
            Ecs<WorldType>.Tags<C4>.Value.AddBlocker(-1);
            #endif
        }
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
        private byte _bufId;
        private Tag<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
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
    public struct TagAll<C1, C2, C3, C4, C5, C6> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag {
        private BitMask _bitMask;
        private byte _bufId;
        private Tag<C1, C2, C3, C4, C5, C6> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
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
    public struct TagAll<C1, C2, C3, C4, C5, C6, C7> : IPrimaryQueryMethod, ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag {
        private BitMask _bitMask;
        private byte _bufId;
        private Tag<C1, C2, C3, C4, C5, C6, C7> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
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
        private byte _bufId;
        private Tag<C1, C2, C3, C4, C5, C6, C7, C8> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
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


    #region ALL_AND_NONE
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAllAndNoneTypes<TTagsIncluded, TTagsExcluded> : IPrimaryQueryMethod, ISealedQueryMethod
        where TTagsIncluded : struct, IComponentTags
        where TTagsExcluded : struct, IComponentTags {
        private BitMask _bitMask;
        public TTagsIncluded _all;
        public TTagsExcluded _exc;
        private byte _incBufId;
        private byte _excBufId;
        
        [MethodImpl(AggressiveInlining)]
        public TagAllAndNoneTypes(TTagsIncluded all, TTagsExcluded exc) {
            _all = all;
            _exc = exc;
            _incBufId = default;
            _excBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _excBufId = _bitMask.BorrowBuf();

            _all.SetAllData<WorldType>(ref minCount, ref entities, _incBufId);

            _exc.SetMask<WorldType>(_excBufId);
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
    #endregion


    #region NONE
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNoneTypes<TTags> : ISealedQueryMethod
        where TTags : struct, IComponentTags {
        private BitMask _bitMask;
        public TTags _exc;
        private byte _excBufId;
        
        [MethodImpl(AggressiveInlining)]
        public TagNoneTypes(TTags exc) {
            _exc = exc;
            _excBufId = default;
            _bitMask = null;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _excBufId = _bitMask.BorrowBuf();
            _exc.SetMask<WorldType>(_excBufId);
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
    public struct TagNone<C1> : ISealedQueryMethod 
        where C1 : struct, ITag {
        private uint[] m1;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            var types = default(Tag<C1>);
            types.BlockTags<WorldType>(1);
            #endif
            m1 = Ecs<WorldType>.Tags<C1>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return m1[entityId] == Utils.EmptyComponent;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            var types = default(Tag<C1>);
            types.BlockTags<WorldType>(-1);
            #endif
        }
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
            var types = default(Tag<C1, C2>);
            types.BlockTags<WorldType>(1);
            #endif
            m1 = Ecs<WorldType>.Tags<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Tags<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return m1[entityId] == Utils.EmptyComponent && m2[entityId] == Utils.EmptyComponent;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            var types = default(Tag<C1, C2>);
            types.BlockTags<WorldType>(-1);
            #endif
        }
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
        private byte _bufId;
        private Tag<C1, C2, C3> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_bufId);
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
    public struct TagNone<C1, C2, C3, C4> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag {
        private BitMask _bitMask;
        private byte _bufId;
        private Tag<C1, C2, C3, C4> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_bufId);
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
    public struct TagNone<C1, C2, C3, C4, C5> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag {
        private BitMask _bitMask;
        private byte _bufId;
        private Tag<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_bufId);
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
    public struct TagNone<C1, C2, C3, C4, C5, C6> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag {
        private BitMask _bitMask;
        private byte _bufId;
        private Tag<C1, C2, C3, C4, C5, C6> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_bufId);
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
    public struct TagNone<C1, C2, C3, C4, C5, C6, C7> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag {
        private BitMask _bitMask;
        private byte _bufId;
        private Tag<C1, C2, C3, C4, C5, C6, C7> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_bufId);
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
        private byte _bufId;
        private Tag<C1, C2, C3, C4, C5, C6, C7, C8> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_bufId);
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


    #region ANY
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAnyTypes<TTags> : ISealedQueryMethod where TTags : struct, IComponentTags {
        private BitMask _bitMask;
        public TTags _any;
        private byte _anyBufId;
        
        [MethodImpl(AggressiveInlining)]
        public TagAnyTypes(TTags any) {
            _any = any;
            _anyBufId = default;
            _bitMask = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _anyBufId = _bitMask.BorrowBuf();
            _any.SetMask<WorldType>(_anyBufId);
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
    public struct TagAny<C1, C2> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag {
        private uint[] m1;
        private uint[] m2;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            var types = default(Tag<C1, C2>);
            types.BlockTags<WorldType>(1);
            #endif
            m1 = Ecs<WorldType>.Tags<C1>.Value.GetDataIdxByEntityId();
            m2 = Ecs<WorldType>.Tags<C2>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return m1[entityId] != Utils.EmptyComponent || m2[entityId] != Utils.EmptyComponent;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            var types = default(Tag<C1, C2>);
            types.BlockTags<WorldType>(-1);
            #endif
        }
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
        private byte _bufId;
        private Tag<C1, C2, C3> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_bufId);
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
    public struct TagAny<C1, C2, C3, C4> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag {
        private BitMask _bitMask;
        private byte _bufId;
        private Tag<C1, C2, C3, C4> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_bufId);
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
    public struct TagAny<C1, C2, C3, C4, C5> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag {
        private BitMask _bitMask;
        private byte _bufId;
        private Tag<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_bufId);
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
    public struct TagAny<C1, C2, C3, C4, C5, C6> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag {
        private BitMask _bitMask;
        private byte _bufId;
        private Tag<C1, C2, C3, C4, C5, C6> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_bufId);
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
    public struct TagAny<C1, C2, C3, C4, C5, C6, C7> : ISealedQueryMethod 
        where C1 : struct, ITag 
        where C2 : struct, ITag 
        where C3 : struct, ITag 
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag {
        private BitMask _bitMask;
        private byte _bufId;
        private Tag<C1, C2, C3, C4, C5, C6, C7> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_bufId);
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
        private byte _bufId;
        private Tag<C1, C2, C3, C4, C5, C6, C7, C8> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleTags.Value.BitMask;
            _bufId = _bitMask.BorrowBuf();
            _types.SetMask<WorldType>(_bufId);
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
}
#endif