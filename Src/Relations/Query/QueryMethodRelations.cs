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
    public struct WithLink<L, WT, QM> : IPrimaryQueryMethod, ISealedQueryMethod
        where L : struct, IEntityLinkComponent<L>
        where WT : struct, IWorldType
        where QM : struct, ISealedQueryMethod, IPrimaryQueryMethod {
        private uint[] m1;
        private L[] d1;
        private QM _qm;

        public WithLink(QM qm) {
            _qm = qm;
            m1 = null;
            d1 = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            World<WorldType>.Components<L>.Value.SetDataIfCountLess(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            World<WorldType>.Components<L>.Value.AddBlocker(1);
            #endif
            m1 = World<WorldType>.Components<L>.Value.GetDataIdxByEntityId();
            d1 = World<WorldType>.Components<L>.Value.Data();
            uint _count = default;
            uint[] _entities = default;
            _qm.SetData<WT>(ref _count, ref _entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            var i = m1[entityId];
            if ((i & Const.EmptyAndDisabledComponentMask) == 0) {
                ref var component = ref d1[i & Const.DisabledComponentMaskInv];
                return component.RefValue(ref component).TryUnpack<WT>(out var e)
                       && _qm.CheckEntity(e._id);
            }
            
            return false;
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<L>.Value.AddBlocker(-1);
            _qm.Dispose<WT>();
        }
        #endif
    }


    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct WithLinksAny<L, WT, QM> : IPrimaryQueryMethod
        where L : struct, IEntityLinksComponent<L>
        where WT : struct, IWorldType
        where QM : struct, ISealedQueryMethod, IPrimaryQueryMethod {
        private uint[] m1;
        private L[] d1;
        private QM _qm;

        public WithLinksAny(QM qm) {
            _qm = qm;
            m1 = null;
            d1 = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            World<WorldType>.Components<L>.Value.SetDataIfCountLess(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            World<WorldType>.Components<L>.Value.AddBlocker(1);
            #endif
            m1 = World<WorldType>.Components<L>.Value.GetDataIdxByEntityId();
            d1 = World<WorldType>.Components<L>.Value.Data();
            uint _count = default;
            uint[] _entities = default;
            _qm.SetData<WT>(ref _count, ref _entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            var i = m1[entityId];
            return (i & Const.EmptyAndDisabledComponentMask) == 0 && CheckEntity(ref d1[i & Const.DisabledComponentMaskInv]);
        }

        [MethodImpl(AggressiveInlining)]
        private bool CheckEntity(ref L component) {
            ref var multi = ref component.RefValue(ref component);
            var count = multi.Count;
            for (var i = 0; i < count; i++) {
                if (multi[i].TryUnpack<WT>(out var e) && _qm.CheckEntity(e._id)) {
                    return true;
                }
            }

            return false;
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<L>.Value.AddBlocker(-1);
            _qm.Dispose<WT>();
        }
        #endif
    }


    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct WithLinksAll<L, WT, QM> : IPrimaryQueryMethod
        where L : struct, IEntityLinksComponent<L>
        where WT : struct, IWorldType
        where QM : struct, ISealedQueryMethod, IPrimaryQueryMethod {
        private uint[] m1;
        private L[] d1;
        private QM _qm;

        public WithLinksAll(QM qm) {
            _qm = qm;
            m1 = null;
            d1 = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            World<WorldType>.Components<L>.Value.SetDataIfCountLess(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            World<WorldType>.Components<L>.Value.AddBlocker(1);
            #endif
            m1 = World<WorldType>.Components<L>.Value.GetDataIdxByEntityId();
            d1 = World<WorldType>.Components<L>.Value.Data();
            uint _count = default;
            uint[] _entities = default;
            _qm.SetData<WT>(ref _count, ref _entities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            var i = m1[entityId];
            return (i & Const.EmptyAndDisabledComponentMask) == 0 && CheckEntity(ref d1[i & Const.DisabledComponentMaskInv]);
        }

        [MethodImpl(AggressiveInlining)]
        private bool CheckEntity(ref L component) {
            ref var multi = ref component.RefValue(ref component);
            var count = multi.Count;
            for (var i = 0; i < count; i++) {
                if (multi[i].TryUnpack<WT>(out var e) && !_qm.CheckEntity(e._id)) {
                    return false;
                }
            }

            return true;
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<L>.Value.AddBlocker(-1);
            _qm.Dispose<WT>();
        }
        #endif
    }
}