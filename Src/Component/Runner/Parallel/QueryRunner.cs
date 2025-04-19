using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    #region DELEFATE_QUERY_FUNCTION
    public delegate void DelegateQueryFunctionParallel<WorldType, C1>(World<WorldType>.ROEntity entity, ref C1 c1)
        where C1 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void DelegateQueryFunctionParallel<WorldType, C1, C2>(World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void DelegateQueryFunctionParallel<WorldType, C1, C2, C3>(World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4>(World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5>(World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5, C6>(World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5, C6, C7>(World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, C8>(
        World<WorldType>.ROEntity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7, ref C8 c8
    )
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent
        where WorldType : struct, IWorldType;
    #endregion

    public abstract class AbstractParallelTask {
        public abstract void Run(uint from, uint to);
    }

    #region QUERY_FUNCTION_RUNNER
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerParallel<WorldType, C1, P> : AbstractParallelTask
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private DelegateQueryFunctionParallel<WorldType, C1> _runner;
        private uint[] _entities;
        private uint _maskLeft;
        private uint _maskRight;
        private P _with;
        private EntityStatusType _entitiesParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunctionParallel<WorldType, C1> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint minChunkSize, uint workersLimit) {
            _maskRight = maskRight;
            _maskLeft = maskLeft;
            _entitiesParam = entitiesParam;
            _with = with;
            _runner = runner;
            var count = World<WorldType>.Components<C1>.Value.Count();
            _entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            with.SetData<WorldType>(ref count, ref _entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        public override void Run(uint from, uint to) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();

            var data1 = World<WorldType>.Components<C1>.Value.Data();

            var status = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (to > from) {
                to--;
                var entity = _entities[to];
                var i1 = di1[entity];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == status[entity].Value)
                    && (i1 & _maskLeft) == _maskRight
                    && _with.CheckEntity(entity)) {
                    _runner(new World<WorldType>.ROEntity(entity),
                            ref data1[i1 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerParallel<WorldType, C1, C2, P> : AbstractParallelTask
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private DelegateQueryFunctionParallel<WorldType, C1, C2> _runner;
        private uint[] _entities;
        private uint _maskLeft;
        private uint _maskRight;
        private P _with;
        private EntityStatusType _entitiesParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunctionParallel<WorldType, C1, C2> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint minChunkSize, uint workersLimit) {
            _maskRight = maskRight;
            _maskLeft = maskLeft;
            _entitiesParam = entitiesParam;
            _with = with;
            _runner = runner;
            var count = World<WorldType>.Components<C1>.Value.Count();
            _entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref _entities);
            with.SetData<WorldType>(ref count, ref _entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(-1);
                World<WorldType>.Components<C2>.Value.AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        public override void Run(uint from, uint to) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();

            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();

            var status = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (to > from) {
                to--;
                var entity = _entities[to];
                var i1 = di1[entity];
                var i2 = di2[entity];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == status[entity].Value)
                    && (i1 & _maskLeft) == _maskRight && (i2 & _maskLeft) ==_maskRight
                    && _with.CheckEntity(entity)) {
                    _runner(new World<WorldType>.ROEntity(entity),
                            ref data1[i1 & Const.DisabledComponentMaskInv],
                            ref data2[i2 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerParallel<WorldType, C1, C2, C3, P> : AbstractParallelTask
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private DelegateQueryFunctionParallel<WorldType, C1, C2, C3> _runner;
        private uint[] _entities;
        private uint _maskLeft;
        private uint _maskRight;
        private P _with;
        private EntityStatusType _entitiesParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunctionParallel<WorldType, C1, C2, C3> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint minChunkSize, uint workersLimit) {
            _maskRight = maskRight;
            _maskLeft = maskLeft;
            _entitiesParam = entitiesParam;
            _with = with;
            _runner = runner;
            var count = World<WorldType>.Components<C1>.Value.Count();
            _entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref _entities);
            with.SetData<WorldType>(ref count, ref _entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(-1);
                World<WorldType>.Components<C2>.Value.AddBlocker(-1);
                World<WorldType>.Components<C3>.Value.AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        public override void Run(uint from, uint to) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();

            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();

            var status = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (to > from) {
                to--;
                var entity = _entities[to];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == status[entity].Value)
                    && (i1 & _maskLeft) == _maskRight && (i2 & _maskLeft) ==_maskRight
                    && (i3 & _maskLeft) == _maskRight
                    && _with.CheckEntity(entity)) {
                    _runner(new World<WorldType>.ROEntity(entity),
                            ref data1[i1 & Const.DisabledComponentMaskInv],
                            ref data2[i2 & Const.DisabledComponentMaskInv],
                            ref data3[i3 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, P> : AbstractParallelTask
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4> _runner;
        private uint[] _entities;
        private uint _maskLeft;
        private uint _maskRight;
        private P _with;
        private EntityStatusType _entitiesParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint minChunkSize, uint workersLimit) {
            _maskRight = maskRight;
            _maskLeft = maskLeft;
            _entitiesParam = entitiesParam;
            _with = with;
            _runner = runner;
            var count = World<WorldType>.Components<C1>.Value.Count();
            _entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref _entities);
            with.SetData<WorldType>(ref count, ref _entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                World<WorldType>.Components<C4>.Value.AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(-1);
                World<WorldType>.Components<C2>.Value.AddBlocker(-1);
                World<WorldType>.Components<C3>.Value.AddBlocker(-1);
                World<WorldType>.Components<C4>.Value.AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        public override void Run(uint from, uint to) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var di4 = World<WorldType>.Components<C4>.Value.GetDataIdxByEntityId();

            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();
            var data4 = World<WorldType>.Components<C4>.Value.Data();

            var status = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (to > from) {
                to--;
                var entity = _entities[to];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == status[entity].Value)
                    && (i1 & _maskLeft) == _maskRight && (i2 & _maskLeft) ==_maskRight
                    && (i3 & _maskLeft) == _maskRight && (i4 & _maskLeft) ==_maskRight
                    && _with.CheckEntity(entity)) {
                    _runner(new World<WorldType>.ROEntity(entity),
                            ref data1[i1 & Const.DisabledComponentMaskInv],
                            ref data2[i2 & Const.DisabledComponentMaskInv],
                            ref data3[i3 & Const.DisabledComponentMaskInv],
                            ref data4[i4 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, P> : AbstractParallelTask
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5> _runner;
        private uint[] _entities;
        private uint _maskLeft;
        private uint _maskRight;
        private P _with;
        private EntityStatusType _entitiesParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint minChunkSize, uint workersLimit) {
            _maskRight = maskRight;
            _maskLeft = maskLeft;
            _entitiesParam = entitiesParam;
            _with = with;
            _runner = runner;
            var count = World<WorldType>.Components<C1>.Value.Count();
            _entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref _entities);
            with.SetData<WorldType>(ref count, ref _entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                World<WorldType>.Components<C4>.Value.AddBlocker(1);
                World<WorldType>.Components<C5>.Value.AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(-1);
                World<WorldType>.Components<C2>.Value.AddBlocker(-1);
                World<WorldType>.Components<C3>.Value.AddBlocker(-1);
                World<WorldType>.Components<C4>.Value.AddBlocker(-1);
                World<WorldType>.Components<C5>.Value.AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        public override void Run(uint from, uint to) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var di4 = World<WorldType>.Components<C4>.Value.GetDataIdxByEntityId();
            var di5 = World<WorldType>.Components<C5>.Value.GetDataIdxByEntityId();

            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();
            var data4 = World<WorldType>.Components<C4>.Value.Data();
            var data5 = World<WorldType>.Components<C5>.Value.Data();

            var status = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (to > from) {
                to--;
                var entity = _entities[to];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == status[entity].Value)
                    && (i1 & _maskLeft) == _maskRight && (i2 & _maskLeft) ==_maskRight
                    && (i3 & _maskLeft) == _maskRight && (i4 & _maskLeft) ==_maskRight && (i5 & _maskLeft) == _maskRight
                    && _with.CheckEntity(entity)) {
                    _runner(new World<WorldType>.ROEntity(entity),
                            ref data1[i1 & Const.DisabledComponentMaskInv],
                            ref data2[i2 & Const.DisabledComponentMaskInv],
                            ref data3[i3 & Const.DisabledComponentMaskInv],
                            ref data4[i4 & Const.DisabledComponentMaskInv],
                            ref data5[i5 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, C6, P> : AbstractParallelTask
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5, C6> _runner;
        private uint[] _entities;
        private uint _maskLeft;
        private uint _maskRight;
        private P _with;
        private EntityStatusType _entitiesParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5, C6> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint minChunkSize, uint workersLimit) {
            _maskRight = maskRight;
            _maskLeft = maskLeft;
            _entitiesParam = entitiesParam;
            _with = with;
            _runner = runner;
            var count = World<WorldType>.Components<C1>.Value.Count();
            _entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref _entities);
            with.SetData<WorldType>(ref count, ref _entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                World<WorldType>.Components<C4>.Value.AddBlocker(1);
                World<WorldType>.Components<C5>.Value.AddBlocker(1);
                World<WorldType>.Components<C6>.Value.AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(-1);
                World<WorldType>.Components<C2>.Value.AddBlocker(-1);
                World<WorldType>.Components<C3>.Value.AddBlocker(-1);
                World<WorldType>.Components<C4>.Value.AddBlocker(-1);
                World<WorldType>.Components<C5>.Value.AddBlocker(-1);
                World<WorldType>.Components<C6>.Value.AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        public override void Run(uint from, uint to) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var di4 = World<WorldType>.Components<C4>.Value.GetDataIdxByEntityId();
            var di5 = World<WorldType>.Components<C5>.Value.GetDataIdxByEntityId();
            var di6 = World<WorldType>.Components<C6>.Value.GetDataIdxByEntityId();

            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();
            var data4 = World<WorldType>.Components<C4>.Value.Data();
            var data5 = World<WorldType>.Components<C5>.Value.Data();
            var data6 = World<WorldType>.Components<C6>.Value.Data();

            var status = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (to > from) {
                to--;
                var entity = _entities[to];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == status[entity].Value)
                    && (i1 & _maskLeft) == _maskRight && (i2 & _maskLeft) ==_maskRight
                    && (i3 & _maskLeft) == _maskRight && (i4 & _maskLeft) ==_maskRight && (i5 & _maskLeft) == _maskRight
                    && (i6 & _maskLeft) == _maskRight
                    && _with.CheckEntity(entity)) {
                    _runner(new World<WorldType>.ROEntity(entity),
                            ref data1[i1 & Const.DisabledComponentMaskInv],
                            ref data2[i2 & Const.DisabledComponentMaskInv],
                            ref data3[i3 & Const.DisabledComponentMaskInv],
                            ref data4[i4 & Const.DisabledComponentMaskInv],
                            ref data5[i5 & Const.DisabledComponentMaskInv],
                            ref data6[i6 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
    }


    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, P> : AbstractParallelTask
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5, C6, C7> _runner;
        private uint[] _entities;
        private uint _maskLeft;
        private uint _maskRight;
        private P _with;
        private EntityStatusType _entitiesParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5, C6, C7> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint minChunkSize, uint workersLimit) {
            _maskRight = maskRight;
            _maskLeft = maskLeft;
            _entitiesParam = entitiesParam;
            _with = with;
            _runner = runner;
            var count = World<WorldType>.Components<C1>.Value.Count();
            _entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref _entities);
            with.SetData<WorldType>(ref count, ref _entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                World<WorldType>.Components<C4>.Value.AddBlocker(1);
                World<WorldType>.Components<C5>.Value.AddBlocker(1);
                World<WorldType>.Components<C6>.Value.AddBlocker(1);
                World<WorldType>.Components<C7>.Value.AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(-1);
                World<WorldType>.Components<C2>.Value.AddBlocker(-1);
                World<WorldType>.Components<C3>.Value.AddBlocker(-1);
                World<WorldType>.Components<C4>.Value.AddBlocker(-1);
                World<WorldType>.Components<C5>.Value.AddBlocker(-1);
                World<WorldType>.Components<C6>.Value.AddBlocker(-1);
                World<WorldType>.Components<C7>.Value.AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        public override void Run(uint from, uint to) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var di4 = World<WorldType>.Components<C4>.Value.GetDataIdxByEntityId();
            var di5 = World<WorldType>.Components<C5>.Value.GetDataIdxByEntityId();
            var di6 = World<WorldType>.Components<C6>.Value.GetDataIdxByEntityId();
            var di7 = World<WorldType>.Components<C7>.Value.GetDataIdxByEntityId();

            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();
            var data4 = World<WorldType>.Components<C4>.Value.Data();
            var data5 = World<WorldType>.Components<C5>.Value.Data();
            var data6 = World<WorldType>.Components<C6>.Value.Data();
            var data7 = World<WorldType>.Components<C7>.Value.Data();

            var status = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (to > from) {
                to--;
                var entity = _entities[to];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == status[entity].Value)
                    && (i1 & _maskLeft) == _maskRight && (i2 & _maskLeft) ==_maskRight
                    && (i3 & _maskLeft) == _maskRight && (i4 & _maskLeft) ==_maskRight && (i5 & _maskLeft) == _maskRight
                    && (i6 & _maskLeft) == _maskRight && (i7 & _maskLeft) ==_maskRight
                    && _with.CheckEntity(entity)) {
                    _runner(new World<WorldType>.ROEntity(entity),
                            ref data1[i1 & Const.DisabledComponentMaskInv],
                            ref data2[i2 & Const.DisabledComponentMaskInv],
                            ref data3[i3 & Const.DisabledComponentMaskInv],
                            ref data4[i4 & Const.DisabledComponentMaskInv],
                            ref data5[i5 & Const.DisabledComponentMaskInv],
                            ref data6[i6 & Const.DisabledComponentMaskInv],
                            ref data7[i7 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, P> : AbstractParallelTask
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> _runner;
        private uint[] _entities;
        private uint _maskLeft;
        private uint _maskRight;
        private P _with;
        private EntityStatusType _entitiesParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunctionParallel<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint minChunkSize, uint workersLimit) {
            _maskRight = maskRight;
            _maskLeft = maskLeft;
            _entitiesParam = entitiesParam;
            _with = with;
            _runner = runner;
            var count = World<WorldType>.Components<C1>.Value.Count();
            _entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref _entities);
            World<WorldType>.Components<C8>.Value.SetDataIfCountLess(ref count, ref _entities);
            with.SetData<WorldType>(ref count, ref _entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                World<WorldType>.Components<C4>.Value.AddBlocker(1);
                World<WorldType>.Components<C5>.Value.AddBlocker(1);
                World<WorldType>.Components<C6>.Value.AddBlocker(1);
                World<WorldType>.Components<C7>.Value.AddBlocker(1);
                World<WorldType>.Components<C8>.Value.AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(-1);
                World<WorldType>.Components<C2>.Value.AddBlocker(-1);
                World<WorldType>.Components<C3>.Value.AddBlocker(-1);
                World<WorldType>.Components<C4>.Value.AddBlocker(-1);
                World<WorldType>.Components<C5>.Value.AddBlocker(-1);
                World<WorldType>.Components<C6>.Value.AddBlocker(-1);
                World<WorldType>.Components<C7>.Value.AddBlocker(-1);
                World<WorldType>.Components<C8>.Value.AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        public override void Run(uint from, uint to) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var di4 = World<WorldType>.Components<C4>.Value.GetDataIdxByEntityId();
            var di5 = World<WorldType>.Components<C5>.Value.GetDataIdxByEntityId();
            var di6 = World<WorldType>.Components<C6>.Value.GetDataIdxByEntityId();
            var di7 = World<WorldType>.Components<C7>.Value.GetDataIdxByEntityId();
            var di8 = World<WorldType>.Components<C8>.Value.GetDataIdxByEntityId();

            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();
            var data4 = World<WorldType>.Components<C4>.Value.Data();
            var data5 = World<WorldType>.Components<C5>.Value.Data();
            var data6 = World<WorldType>.Components<C6>.Value.Data();
            var data7 = World<WorldType>.Components<C7>.Value.Data();
            var data8 = World<WorldType>.Components<C8>.Value.Data();

            var status = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (to > from) {
                to--;
                var entity = _entities[to];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                var i8 = di8[entity];
                if ((_entitiesParam == EntityStatusType.Any || _entitiesParam == status[entity].Value)
                    && (i1 & _maskLeft) == _maskRight && (i2 & _maskLeft) ==_maskRight
                    && (i3 & _maskLeft) == _maskRight && (i4 & _maskLeft) ==_maskRight && (i5 & _maskLeft) == _maskRight
                    && (i6 & _maskLeft) == _maskRight && (i7 & _maskLeft) ==_maskRight && (i8 & _maskLeft) == _maskRight
                    && _with.CheckEntity(entity)) {
                    _runner(new World<WorldType>.ROEntity(entity),
                            ref data1[i1 & Const.DisabledComponentMaskInv],
                            ref data2[i2 & Const.DisabledComponentMaskInv],
                            ref data3[i3 & Const.DisabledComponentMaskInv],
                            ref data4[i4 & Const.DisabledComponentMaskInv],
                            ref data5[i5 & Const.DisabledComponentMaskInv],
                            ref data6[i6 & Const.DisabledComponentMaskInv],
                            ref data7[i7 & Const.DisabledComponentMaskInv],
                            ref data8[i8 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
    }
    #endregion
}