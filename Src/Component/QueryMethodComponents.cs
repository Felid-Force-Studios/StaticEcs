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
    internal struct AllTypes<TComponents> : IPrimaryQueryMethod, ISealedQueryMethod where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        private byte _incBufId;
        public TComponents _all;

        [MethodImpl(AggressiveInlining)]
        public AllTypes(TComponents all) {
            _all = all;
            _bitMask = null;
            _incBufId = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _all.SetMinData<WorldType>(ref minCount, ref entities);
            _all.SetBitMask<WorldType>(_incBufId);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _all.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAll(entityId, _incBufId);
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
    public struct All<C1> : IPrimaryQueryMethod, ISealedQueryMethod
        where C1 : struct, IComponent {
        private uint[] m1;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyAndDisabledComponentMask) == 0;
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
        }
        #endif
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
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
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

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
        }
        #endif
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
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
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

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
        }
        #endif
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
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
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

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C4>.Value.AddBlocker(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;
        private Types<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5>>.Cache.This(out _bufId, out _count);
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
            default(Types<C1, C2, C3, C4, C5>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;
        private Types<C1, C2, C3, C4, C5, C6> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6>>.Cache.This(out _bufId, out _count);
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
    public struct All<C1, C2, C3, C4, C5, C6, C7> : IPrimaryQueryMethod, ISealedQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;
        private Types<C1, C2, C3, C4, C5, C6, C7> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7>>.Cache.This(out _bufId, out _count);
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
        private uint _bufId;
        private ushort _count;
        private Types<C1, C2, C3, C4, C5, C6, C7, C8> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7, C8>>.Cache.This(out _bufId, out _count);
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

    #region ALL_ONLY_DISABLED
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct AllOnlyDisabledTypes<TComponents> : IPrimaryQueryMethod, ISealedQueryMethod where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        private byte _incBufId;
        public TComponents _all;

        [MethodImpl(AggressiveInlining)]
        public AllOnlyDisabledTypes(TComponents all) {
            _all = all;
            _bitMask = null;
            _incBufId = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _all.SetMinData<WorldType>(ref minCount, ref entities);
            _all.SetBitMask<WorldType>(_incBufId);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _all.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllOnlyDisabled(entityId, _incBufId);
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
    public struct AllOnlyDisabled<C1> : IPrimaryQueryMethod, ISealedQueryMethod
        where C1 : struct, IComponent {
        private uint[] m1;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
            #endif
            m1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return (m1[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask;
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
        }
        #endif
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
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
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

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
        }
        #endif
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
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
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
            return (m1[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask && (m2[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask &&
                   (m3[entityId] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask;
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
        }
        #endif
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
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
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

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
            Ecs<WorldType>.Components<C4>.Value.AddBlocker(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;
        private Types<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllOnlyDisabledIndexed(entityId, _bufId, _count);
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
    public struct AllOnlyDisabled<C1, C2, C3, C4, C5, C6> : IPrimaryQueryMethod, ISealedQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;
        private Types<C1, C2, C3, C4, C5, C6> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllOnlyDisabledIndexed(entityId, _bufId, _count);
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
    public struct AllOnlyDisabled<C1, C2, C3, C4, C5, C6, C7> : IPrimaryQueryMethod, ISealedQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;
        private Types<C1, C2, C3, C4, C5, C6, C7> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllOnlyDisabledIndexed(entityId, _bufId, _count);
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
        private uint _bufId;
        private ushort _count;
        private Types<C1, C2, C3, C4, C5, C6, C7, C8> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7, C8>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllOnlyDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _types.Block<WorldType>(-1);
        }
        #endif
    }
    #endregion

    #region ALL_WITH_DISABLED
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct AllWithDisabledTypes<TComponents> : IPrimaryQueryMethod, ISealedQueryMethod where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        private byte _incBufId;
        public TComponents _all;

        [MethodImpl(AggressiveInlining)]
        public AllWithDisabledTypes(TComponents all) {
            _all = all;
            _bitMask = null;
            _incBufId = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _all.SetMinData<WorldType>(ref minCount, ref entities);
            _all.SetBitMask<WorldType>(_incBufId);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _all.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllWithDisabled(entityId, _incBufId);
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
    public struct AllWithDisabled<C1> : IPrimaryQueryMethod, ISealedQueryMethod
        where C1 : struct, IComponent {
        private uint[] m1;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
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
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
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
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
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
            Ecs<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
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
        private uint _bufId;
        private ushort _count;
        private Types<C1, C2, C3, C4, C5> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllWithDisabledIndexed(entityId, _bufId, _count);
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
    public struct AllWithDisabled<C1, C2, C3, C4, C5, C6> : IPrimaryQueryMethod, ISealedQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;
        private Types<C1, C2, C3, C4, C5, C6> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllWithDisabledIndexed(entityId, _bufId, _count);
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
    public struct AllWithDisabled<C1, C2, C3, C4, C5, C6, C7> : IPrimaryQueryMethod, ISealedQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent {
        private BitMask _bitMask;
        private uint _bufId;
        private ushort _count;
        private Types<C1, C2, C3, C4, C5, C6, C7> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllWithDisabledIndexed(entityId, _bufId, _count);
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
        private uint _bufId;
        private ushort _count;
        private Types<C1, C2, C3, C4, C5, C6, C7, C8> _types;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _types.SetMinData<WorldType>(ref minCount, ref entities);
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7, C8>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _types.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAllWithDisabledIndexed(entityId, _bufId, _count);
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
    internal struct NoneTypes<TComponents> : ISealedQueryMethod
        where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        private byte _incBufId;
        public TComponents _exc;

        [MethodImpl(AggressiveInlining)]
        public NoneTypes(TComponents exc) {
            _exc = exc;
            _bitMask = null;
            _incBufId = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _exc.SetBitMask<WorldType>(_incBufId);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _exc.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAny(entityId, _incBufId);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _exc.Block<WorldType>(-1);
            _bitMask.DropBuf();
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5, C6>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5, C6>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5, C6, C7>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5, C6, C7>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7, C8>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5, C6, C7, C8>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5, C6, C7, C8>).Block<WorldType>(-1);
        }
        #endif
    }
    #endregion

    #region NONE_WITH_DISABLED
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct NoneWithDisabledTypes<TComponents> : ISealedQueryMethod
        where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        private byte _incBufId;
        public TComponents _exc;

        [MethodImpl(AggressiveInlining)]
        public NoneWithDisabledTypes(TComponents exc) {
            _exc = exc;
            _bitMask = null;
            _incBufId = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _exc.SetBitMask<WorldType>(_incBufId);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _exc.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyWithDisabled(entityId, _incBufId);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _exc.Block<WorldType>(-1);
            _bitMask.DropBuf();
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyWithDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyWithDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyWithDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5, C6>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyWithDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5, C6>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5, C6, C7>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyWithDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5, C6, C7>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7, C8>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5, C6, C7, C8>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.NotHasAnyWithDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5, C6, C7, C8>).Block<WorldType>(-1);
        }
        #endif
    }
    #endregion

    #region ANY
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct AnyTypes<TComponents> : ISealedQueryMethod where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        private byte _incBufId;
        public TComponents _any;

        [MethodImpl(AggressiveInlining)]
        public AnyTypes(TComponents any) {
            _any = any;
            _bitMask = null;
            _incBufId = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _any.SetBitMask<WorldType>(_incBufId);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _any.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAny(entityId, _incBufId);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _any.Block<WorldType>(-1);
            _bitMask.DropBuf();
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5, C6>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5, C6>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5, C6, C7>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5, C6, C7>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7, C8>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5, C6, C7, C8>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5, C6, C7, C8>).Block<WorldType>(-1);
        }
        #endif
    }
    #endregion

    #region ANY_ONLY_DISABLED
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct AnyOnlyDisabledTypes<TComponents> : ISealedQueryMethod where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        private byte _incBufId;
        public TComponents _any;

        [MethodImpl(AggressiveInlining)]
        public AnyOnlyDisabledTypes(TComponents any) {
            _any = any;
            _bitMask = null;
            _incBufId = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _any.SetBitMask<WorldType>(_incBufId);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _any.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyOnlyDisabled(entityId, _incBufId);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _any.Block<WorldType>(-1);
            _bitMask.DropBuf();
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyOnlyDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyOnlyDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyOnlyDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5, C6>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyOnlyDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5, C6>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5, C6, C7>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyOnlyDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5, C6, C7>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7, C8>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5, C6, C7, C8>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyOnlyDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5, C6, C7, C8>).Block<WorldType>(-1);
        }
        #endif
    }
    #endregion

    #region ANY_WITH_DISABLED
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct AnyWithDisabledTypes<TComponents> : ISealedQueryMethod where TComponents : struct, IComponentTypes {
        private BitMask _bitMask;
        private byte _incBufId;
        public TComponents _any;

        [MethodImpl(AggressiveInlining)]
        public AnyWithDisabledTypes(TComponents any) {
            _any = any;
            _bitMask = null;
            _incBufId = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            _incBufId = _bitMask.BorrowBuf();
            _any.SetBitMask<WorldType>(_incBufId);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            _any.Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyWithDisabled(entityId, _incBufId);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _any.Block<WorldType>(-1);
            _bitMask.DropBuf();
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyWithDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyWithDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyWithDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5, C6>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyWithDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5, C6>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5, C6, C7>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyWithDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5, C6, C7>).Block<WorldType>(-1);
        }
        #endif
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
        private uint _bufId;
        private ushort _count;

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            _bitMask = Ecs<WorldType>.ModuleComponents.Value.BitMask;
            Ecs<WorldType>.ModuleComponents.MaskCache<Types<C1, C2, C3, C4, C5, C6, C7, C8>>.Cache.This(out _bufId, out _count);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            default(Types<C1, C2, C3, C4, C5, C6, C7, C8>).Block<WorldType>(1);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            return _bitMask.HasAnyWithDisabledIndexed(entityId, _bufId, _count);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            default(Types<C1, C2, C3, C4, C5, C6, C7, C8>).Block<WorldType>(-1);
        }
        #endif
    }
    #endregion
}