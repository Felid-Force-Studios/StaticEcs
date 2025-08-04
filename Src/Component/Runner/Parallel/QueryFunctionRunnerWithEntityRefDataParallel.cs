using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    #region QUERY_FUNCTION_RUNNER
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, P> : AbstractParallelTask
        where D : struct
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private QueryFunctionWithRefDataEntityParallel<D, WorldType, C1> _runner;
        private uint[] _entities;
        private P _with;
        private D _data;
        private EntityStatusType _entitiesParam;
        private byte _componentParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1> runner, P with, EntityStatusType entitiesParam, ComponentStatus componentsParam, uint minChunkSize, uint workersLimit) {
            _data = data;
            _componentParam = (byte) componentsParam;
            _entitiesParam = entitiesParam;
            _with = with;
            _runner = runner;
            var count = World<WorldType>.Components<C1>.Value.Count();
            _entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            with.SetData<WorldType>(ref count, ref _entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
            data = _data;
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
                    && (
                        (_componentParam == 0 && ((i1) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (_componentParam == 1 && ((i1) & Const.EmptyComponentMask) == 0) ||
                        (_componentParam == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && _with.CheckEntity(entity)) {
                    _runner(ref _data,
                            new World<WorldType>.ROEntity(entity),
                            ref data1[i1 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        private static void AddBlocker(int val) {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, P> : AbstractParallelTask
        where D : struct
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2> _runner;
        private uint[] _entities;
        private P _with;
        private D _data;
        private EntityStatusType _entitiesParam;
        private byte _componentParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2> runner, P with, EntityStatusType entitiesParam, ComponentStatus componentsParam, uint minChunkSize, uint workersLimit) {
            _data = data;
            _componentParam = (byte) componentsParam;
            _entitiesParam = entitiesParam;
            _with = with;
            _runner = runner;
            var count = World<WorldType>.Components<C1>.Value.Count();
            _entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref _entities);
            with.SetData<WorldType>(ref count, ref _entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
            data = _data;
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
                    && (
                        (_componentParam == 0 && ((i1 | i2) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (_componentParam == 1 && ((i1 | i2) & Const.EmptyComponentMask) == 0) ||
                        (_componentParam == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && _with.CheckEntity(entity)) {
                    _runner(ref _data,
                            new World<WorldType>.ROEntity(entity),
                            ref data1[i1 & Const.DisabledComponentMaskInv],
                            ref data2[i2 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        private static void AddBlocker(int val) {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
            World<WorldType>.Components<C2>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, P> : AbstractParallelTask
        where D : struct
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3> _runner;
        private uint[] _entities;
        private P _with;
        private D _data;
        private EntityStatusType _entitiesParam;
        private byte _componentParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3> runner, P with, EntityStatusType entitiesParam, ComponentStatus componentsParam, uint minChunkSize, uint workersLimit) {
            _data = data;
            _componentParam = (byte) componentsParam;
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
                AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
            data = _data;
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
                    && (
                        (_componentParam == 0 && ((i1 | i2 | i3) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (_componentParam == 1 && ((i1 | i2 | i3) & Const.EmptyComponentMask) == 0) ||
                        (_componentParam == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && _with.CheckEntity(entity)) {
                    _runner(ref _data,
                            new World<WorldType>.ROEntity(entity),
                            ref data1[i1 & Const.DisabledComponentMaskInv],
                            ref data2[i2 & Const.DisabledComponentMaskInv],
                            ref data3[i3 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        private static void AddBlocker(int val) {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
            World<WorldType>.Components<C2>.Value.AddBlocker(val);
            World<WorldType>.Components<C3>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, P> : AbstractParallelTask
        where D : struct
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4> _runner;
        private uint[] _entities;
        private P _with;
        private D _data;
        private EntityStatusType _entitiesParam;
        private byte _componentParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4> runner, P with, EntityStatusType entitiesParam, ComponentStatus componentsParam, uint minChunkSize, uint workersLimit) {
            _data = data;
            _componentParam = (byte) componentsParam;
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
                AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
            data = _data;
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
                    && (
                        (_componentParam == 0 && ((i1 | i2 | i3 | i4) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (_componentParam == 1 && ((i1 | i2 | i3 | i4) & Const.EmptyComponentMask) == 0) ||
                        (_componentParam == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && _with.CheckEntity(entity)) {
                    _runner(ref _data,
                            new World<WorldType>.ROEntity(entity),
                            ref data1[i1 & Const.DisabledComponentMaskInv],
                            ref data2[i2 & Const.DisabledComponentMaskInv],
                            ref data3[i3 & Const.DisabledComponentMaskInv],
                            ref data4[i4 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        private static void AddBlocker(int val) {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
            World<WorldType>.Components<C2>.Value.AddBlocker(val);
            World<WorldType>.Components<C3>.Value.AddBlocker(val);
            World<WorldType>.Components<C4>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, P> : AbstractParallelTask
        where D : struct
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5> _runner;
        private uint[] _entities;
        private P _with;
        private D _data;
        private EntityStatusType _entitiesParam;
        private byte _componentParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5> runner, P with, EntityStatusType entitiesParam, ComponentStatus componentsParam, uint minChunkSize, uint workersLimit) {
            _data = data;
            _componentParam = (byte) componentsParam;
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
                AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
            data = _data;
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
                    && (
                        (_componentParam == 0 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (_componentParam == 1 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyComponentMask) == 0) ||
                        (_componentParam == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && _with.CheckEntity(entity)) {
                    _runner(ref _data,
                            new World<WorldType>.ROEntity(entity),
                            ref data1[i1 & Const.DisabledComponentMaskInv],
                            ref data2[i2 & Const.DisabledComponentMaskInv],
                            ref data3[i3 & Const.DisabledComponentMaskInv],
                            ref data4[i4 & Const.DisabledComponentMaskInv],
                            ref data5[i5 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        private static void AddBlocker(int val) {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
            World<WorldType>.Components<C2>.Value.AddBlocker(val);
            World<WorldType>.Components<C3>.Value.AddBlocker(val);
            World<WorldType>.Components<C4>.Value.AddBlocker(val);
            World<WorldType>.Components<C5>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, P> : AbstractParallelTask
        where D : struct
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6> _runner;
        private uint[] _entities;
        private P _with;
        private D _data;
        private EntityStatusType _entitiesParam;
        private byte _componentParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6> runner, P with, EntityStatusType entitiesParam, ComponentStatus componentsParam, uint minChunkSize, uint workersLimit) {
            _data = data;
            _componentParam = (byte) componentsParam;
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
                AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
            data = _data;
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
                    && (
                        (_componentParam == 0 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (_componentParam == 1 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyComponentMask) == 0) ||
                        (_componentParam == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && _with.CheckEntity(entity)) {
                    _runner(ref _data,
                            new World<WorldType>.ROEntity(entity),
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
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        private static void AddBlocker(int val) {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
            World<WorldType>.Components<C2>.Value.AddBlocker(val);
            World<WorldType>.Components<C3>.Value.AddBlocker(val);
            World<WorldType>.Components<C4>.Value.AddBlocker(val);
            World<WorldType>.Components<C5>.Value.AddBlocker(val);
            World<WorldType>.Components<C6>.Value.AddBlocker(val);
        }
        #endif
    }


    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, P> : AbstractParallelTask
        where D : struct
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7> _runner;
        private uint[] _entities;
        private P _with;
        private D _data;
        private EntityStatusType _entitiesParam;
        private byte _componentParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7> runner, P with, EntityStatusType entitiesParam, ComponentStatus componentsParam, uint minChunkSize, uint workersLimit) {
            _data = data;
            _componentParam = (byte) componentsParam;
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
                AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
            data = _data;
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
                    && (
                        (_componentParam == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (_componentParam == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyComponentMask) == 0) ||
                        (_componentParam == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && _with.CheckEntity(entity)) {
                    _runner(ref _data,
                            new World<WorldType>.ROEntity(entity),
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
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        private static void AddBlocker(int val) {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
            World<WorldType>.Components<C2>.Value.AddBlocker(val);
            World<WorldType>.Components<C3>.Value.AddBlocker(val);
            World<WorldType>.Components<C4>.Value.AddBlocker(val);
            World<WorldType>.Components<C5>.Value.AddBlocker(val);
            World<WorldType>.Components<C6>.Value.AddBlocker(val);
            World<WorldType>.Components<C7>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public sealed class QueryFunctionRunnerWithEntityRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, P> : AbstractParallelTask
        where D : struct
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
        
        private QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> _runner;
        private uint[] _entities;
        private P _with;
        private D _data;
        private EntityStatusType _entitiesParam;
        private byte _componentParam;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefDataEntityParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> runner, P with, EntityStatusType entitiesParam, ComponentStatus componentsParam, uint minChunkSize, uint workersLimit) {
            _data = data;
            _componentParam = (byte) componentsParam;
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
                AddBlocker(1);
                #endif
                ParallelRunner<WorldType>.Run(this, count, minChunkSize, workersLimit);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
            data = _data;
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
                    && (
                        (_componentParam == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (_componentParam == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyComponentMask) == 0) ||
                        (_componentParam == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                              && (i8 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && _with.CheckEntity(entity)) {
                    _runner(ref _data,
                            new World<WorldType>.ROEntity(entity),
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
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        private static void AddBlocker(int val) {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
            World<WorldType>.Components<C2>.Value.AddBlocker(val);
            World<WorldType>.Components<C3>.Value.AddBlocker(val);
            World<WorldType>.Components<C4>.Value.AddBlocker(val);
            World<WorldType>.Components<C5>.Value.AddBlocker(val);
            World<WorldType>.Components<C6>.Value.AddBlocker(val);
            World<WorldType>.Components<C7>.Value.AddBlocker(val);
            World<WorldType>.Components<C8>.Value.AddBlocker(val);
        }
        #endif
    }
    #endregion
}