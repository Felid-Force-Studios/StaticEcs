﻿#if !FFS_ECS_DISABLE_TAGS
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
    public struct TagAll<TTags> : IPrimaryQueryMethod where TTags : struct, IComponentTags {
        public TTags _all;
        private byte _incBufId;
        
        [MethodImpl(AggressiveInlining)]
        public TagAll(TTags all) {
            _all = all;
            _incBufId = default;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            _incBufId = BitMaskUtils<WorldID, ITag>.BorrowBuf();
            _all.SetMask<WorldID>(_incBufId);
            _all.SetData(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity<WorldID>(Ecs<WorldID>.Entity entity) where WorldID : struct, IWorldId {
            return BitMaskUtils<WorldID, ITag>.HasAll(entity._id, _incBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG
            _all.Dispose<WorldID>();
            #endif
            BitMaskUtils<WorldID, ITag>.DropBuf(_incBufId);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct TagSingle<TTag> : IPrimaryQueryMethod, Stateless 
        where TTag : struct, ITag {
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            #if DEBUG
            Ecs<WorldID>.Tags<TTag>.AddBlocker(1);
            #endif
            Ecs<WorldID>.Tags<TTag>.SetDataIfCountLess(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity<WorldID>(Ecs<WorldID>.Entity entity) where WorldID : struct, IWorldId {
            return Ecs<WorldID>.Tags<TTag>.Has(entity);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG
            Ecs<WorldID>.Tags<TTag>.AddBlocker(-1);
            #endif
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct TagDouble<TTag1, TTag2> : IPrimaryQueryMethod, Stateless
        where TTag1 : struct, ITag 
        where TTag2 : struct, ITag {
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            #if DEBUG
            Ecs<WorldID>.Tags<TTag1>.AddBlocker(1);
            Ecs<WorldID>.Tags<TTag2>.AddBlocker(1);
            #endif
            Ecs<WorldID>.Tags<TTag1>.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Tags<TTag2>.SetDataIfCountLess(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity<WorldID>(Ecs<WorldID>.Entity entity) where WorldID : struct, IWorldId {
            return Ecs<WorldID>.Tags<TTag1>.Has(entity) && Ecs<WorldID>.Tags<TTag2>.Has(entity);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG
            Ecs<WorldID>.Tags<TTag1>.AddBlocker(-1);
            Ecs<WorldID>.Tags<TTag2>.AddBlocker(-1);
            #endif
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAllAndNone<TTagsIncluded, TTagsExcluded> : IPrimaryQueryMethod
        where TTagsIncluded : struct, IComponentTags
        where TTagsExcluded : struct, IComponentTags {
        public TTagsIncluded _all;
        public TTagsExcluded _exc;
        private byte _incBufId;
        private byte _excBufId;
        
        [MethodImpl(AggressiveInlining)]
        public TagAllAndNone(TTagsIncluded all, TTagsExcluded exc) {
            _all = all;
            _exc = exc;
            _incBufId = default;
            _excBufId = default;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            _incBufId = BitMaskUtils<WorldID, ITag>.BorrowBuf();
            _excBufId = BitMaskUtils<WorldID, ITag>.BorrowBuf();
            _all.SetMask<WorldID>(_incBufId);
            _all.SetData(ref minCount, ref entities);
            _exc.SetMask<WorldID>(_excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity<WorldID>(Ecs<WorldID>.Entity entity) where WorldID : struct, IWorldId {
            return BitMaskUtils<WorldID, ITag>.HasAllAndExc(entity._id, _incBufId, _excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG
            _all.Dispose<WorldID>();
            _exc.Dispose<WorldID>();
            #endif
            BitMaskUtils<WorldID, ITag>.DropBuf(_incBufId);
            BitMaskUtils<WorldID, ITag>.DropBuf(_excBufId);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagNone<TTags> : IQueryMethod
        where TTags : struct, IComponentTags {
        public TTags _exc;
        private byte _excBufId;
        
        [MethodImpl(AggressiveInlining)]
        public TagNone(TTags exc) {
            _exc = exc;
            _excBufId = default;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            _excBufId = BitMaskUtils<WorldID, ITag>.BorrowBuf();
            _exc.SetMask<WorldID>(_excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity<WorldID>(Ecs<WorldID>.Entity entity) where WorldID : struct, IWorldId {
            return BitMaskUtils<WorldID, ITag>.NotHasAny(entity._id, _excBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG
            _exc.Dispose<WorldID>();
            #endif
            BitMaskUtils<WorldID, ITag>.DropBuf(_excBufId);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagAny<TTags> : IQueryMethod where TTags : struct, IComponentTags {
        public TTags _any;
        private byte _anyBufId;
        
        [MethodImpl(AggressiveInlining)]
        public TagAny(TTags any) {
            _any = any;
            _anyBufId = default;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            _anyBufId = BitMaskUtils<WorldID, ITag>.BorrowBuf();
            _any.SetMask<WorldID>(_anyBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity<WorldID>(Ecs<WorldID>.Entity entity) where WorldID : struct, IWorldId {
            return BitMaskUtils<WorldID, ITag>.HasAny(entity._id, _anyBufId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            #if DEBUG
            _any.Dispose<WorldID>();
            #endif
            BitMaskUtils<WorldID, ITag>.DropBuf(_anyBufId);
        }
    }
}
#endif