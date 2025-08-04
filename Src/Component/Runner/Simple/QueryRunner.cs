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
    public readonly struct QueryFunctionRunner<WorldType, C1, P>
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where WorldType : struct, IWorldType {
        internal static readonly QueryFunctionRunner<WorldType, C1, P> Value = default;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where R : struct, World<WorldType>.IQueryFunction<C1> {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<R>(R runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components)
            where R : struct, World<WorldType>.IQueryFunction<C1> {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();

            var status = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                    && (
                        (components == 0 && ((i1) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner.Run(new World<WorldType>.Entity(entity),
                               ref data1[i1 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunction<C1> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run(QueryFunction<C1> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data1[i1 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithData<D, C1> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(D data, QueryFunctionWithData<D, C1> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
                           ref data1[i1 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefData<D, C1> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(ref D data, QueryFunctionWithRefData<D, C1> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
                           ref data1[i1 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunctionWithEntity<WorldType, C1> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run(QueryFunctionWithEntity<WorldType, C1> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(new World<WorldType>.Entity(entity),
                           ref data1[i1 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
                           new World<WorldType>.Entity(entity),
                           ref data1[i1 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
                           new World<WorldType>.Entity(entity),
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
    public readonly struct QueryFunctionRunner<WorldType, C1, C2, P>
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where WorldType : struct, IWorldType {
        internal static readonly QueryFunctionRunner<WorldType, C1, C2, P> Value = default;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where R : struct, World<WorldType>.IQueryFunction<C1, C2> {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<R>(R runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components)
            where R : struct, World<WorldType>.IQueryFunction<C1, C2> {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();

            var status = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner.Run(new World<WorldType>.Entity(entity),
                               ref data1[i1 & Const.DisabledComponentMaskInv],
                               ref data2[i2 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunction<C1, C2> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run(QueryFunction<C1, C2> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithData<D, C1, C2> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(D data, QueryFunctionWithData<D, C1, C2> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefData<D, C1, C2> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(ref D data, QueryFunctionWithRefData<D, C1, C2> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunctionWithEntity<WorldType, C1, C2> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run(QueryFunctionWithEntity<WorldType, C1, C2> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(new World<WorldType>.Entity(entity),
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
                           new World<WorldType>.Entity(entity),
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
                           new World<WorldType>.Entity(entity),
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
    public readonly struct QueryFunctionRunner<WorldType, C1, C2, C3, P>
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where WorldType : struct, IWorldType {
        internal static readonly QueryFunctionRunner<WorldType, C1, C2, C3, P> Value = default;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3> {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<R>(R runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components)
            where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3> {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();

            var status = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner.Run(new World<WorldType>.Entity(entity),
                               ref data1[i1 & Const.DisabledComponentMaskInv],
                               ref data2[i2 & Const.DisabledComponentMaskInv],
                               ref data3[i3 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunction<C1, C2, C3> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run(QueryFunction<C1, C2, C3> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithData<D, C1, C2, C3> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(D data, QueryFunctionWithData<D, C1, C2, C3> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunctionWithEntity<WorldType, C1, C2, C3> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run(QueryFunctionWithEntity<WorldType, C1, C2, C3> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(new World<WorldType>.Entity(entity),
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
                           new World<WorldType>.Entity(entity),
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
                           new World<WorldType>.Entity(entity),
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
    public readonly struct QueryFunctionRunner<WorldType, C1, C2, C3, C4, P>
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where WorldType : struct, IWorldType {
        internal static readonly QueryFunctionRunner<WorldType, C1, C2, C3, C4, P> Value = default;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4> {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<R>(R runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components)
            where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4> {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var di4 = World<WorldType>.Components<C4>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();
            var data4 = World<WorldType>.Components<C4>.Value.Data();

            var status = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner.Run(new World<WorldType>.Entity(entity),
                               ref data1[i1 & Const.DisabledComponentMaskInv],
                               ref data2[i2 & Const.DisabledComponentMaskInv],
                               ref data3[i3 & Const.DisabledComponentMaskInv],
                               ref data4[i4 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunction<C1, C2, C3, C4> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run(QueryFunction<C1, C2, C3, C4> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var di4 = World<WorldType>.Components<C4>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();
            var data4 = World<WorldType>.Components<C4>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv],
                           ref data4[i4 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithData<D, C1, C2, C3, C4> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(D data, QueryFunctionWithData<D, C1, C2, C3, C4> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var di4 = World<WorldType>.Components<C4>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();
            var data4 = World<WorldType>.Components<C4>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv],
                           ref data4[i4 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var di4 = World<WorldType>.Components<C4>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();
            var data4 = World<WorldType>.Components<C4>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv],
                           ref data4[i4 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var di4 = World<WorldType>.Components<C4>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();
            var data4 = World<WorldType>.Components<C4>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(new World<WorldType>.Entity(entity),
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv],
                           ref data4[i4 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var di4 = World<WorldType>.Components<C4>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();
            var data4 = World<WorldType>.Components<C4>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
                           new World<WorldType>.Entity(entity),
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv],
                           ref data4[i4 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            var di2 = World<WorldType>.Components<C2>.Value.GetDataIdxByEntityId();
            var di3 = World<WorldType>.Components<C3>.Value.GetDataIdxByEntityId();
            var di4 = World<WorldType>.Components<C4>.Value.GetDataIdxByEntityId();
            var data1 = World<WorldType>.Components<C1>.Value.Data();
            var data2 = World<WorldType>.Components<C2>.Value.Data();
            var data3 = World<WorldType>.Components<C3>.Value.Data();
            var data4 = World<WorldType>.Components<C4>.Value.Data();

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
                           new World<WorldType>.Entity(entity),
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
    public readonly struct QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, P>
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where WorldType : struct, IWorldType {
        internal static readonly QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, P> Value = default;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4, C5> {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<R>(R runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components)
            where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4, C5> {
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

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner.Run(new World<WorldType>.Entity(entity),
                               ref data1[i1 & Const.DisabledComponentMaskInv],
                               ref data2[i2 & Const.DisabledComponentMaskInv],
                               ref data3[i3 & Const.DisabledComponentMaskInv],
                               ref data4[i4 & Const.DisabledComponentMaskInv],
                               ref data5[i5 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunction<C1, C2, C3, C4, C5> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run(QueryFunction<C1, C2, C3, C4, C5> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv],
                           ref data4[i4 & Const.DisabledComponentMaskInv],
                           ref data5[i5 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv],
                           ref data4[i4 & Const.DisabledComponentMaskInv],
                           ref data5[i5 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv],
                           ref data4[i4 & Const.DisabledComponentMaskInv],
                           ref data5[i5 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(new World<WorldType>.Entity(entity),
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv],
                           ref data4[i4 & Const.DisabledComponentMaskInv],
                           ref data5[i5 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
                           new World<WorldType>.Entity(entity),
                           ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv],
                           ref data4[i4 & Const.DisabledComponentMaskInv],
                           ref data5[i5 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
                           new World<WorldType>.Entity(entity),
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
    public readonly struct QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, P>
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where WorldType : struct, IWorldType {
        internal static readonly QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, P> Value = default;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6> {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<R>(R runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components)
            where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6> {
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

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner.Run(new World<WorldType>.Entity(entity),
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
        
        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunction<C1, C2, C3, C4, C5, C6> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run(QueryFunction<C1, C2, C3, C4, C5, C6> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data1[i1 & Const.DisabledComponentMaskInv],
                           ref data2[i2 & Const.DisabledComponentMaskInv],
                           ref data3[i3 & Const.DisabledComponentMaskInv],
                           ref data4[i4 & Const.DisabledComponentMaskInv],
                           ref data5[i5 & Const.DisabledComponentMaskInv],
                           ref data6[i6 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
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
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
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

        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(new World<WorldType>.Entity(entity),
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

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
                           new World<WorldType>.Entity(entity),
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

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
                           new World<WorldType>.Entity(entity),
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
    public readonly struct QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, P>
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where WorldType : struct, IWorldType {
        internal static readonly QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, P> Value = default;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<R>(R runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components)
            where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
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

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner.Run(new World<WorldType>.Entity(entity),
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
        
        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunction<C1, C2, C3, C4, C5, C6, C7> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run(QueryFunction<C1, C2, C3, C4, C5, C6, C7> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data1[i1 & Const.DisabledComponentMaskInv],
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
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6, C7> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6, C7> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
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
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
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

        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(new World<WorldType>.Entity(entity),
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

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
                           new World<WorldType>.Entity(entity),
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

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
                           new World<WorldType>.Entity(entity),
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
    public readonly struct QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, P>
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
        internal static readonly QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, P> Value = default;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C8>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<R>(R runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components)
            where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
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

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                var i8 = di8[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i8 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner.Run(new World<WorldType>.Entity(entity),
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
        
        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C8>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run(QueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                var i8 = di8[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i8 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data1[i1 & Const.DisabledComponentMaskInv],
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
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6, C7, C8> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C8>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(D data, QueryFunctionWithData<D, C1, C2, C3, C4, C5, C6, C7, C8> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                var i8 = di8[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i8 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
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
        
        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C8>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<D>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                var i8 = di8[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i8 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
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

        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C8>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                var i8 = di8[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i8 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(new World<WorldType>.Entity(entity),
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

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C8>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(D data, QueryFunctionWithDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                var i8 = di8[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i8 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(data,
                           new World<WorldType>.Entity(entity),
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

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> runner, P with, EntityStatusType entitiesParam, ComponentStatus components) where D : struct {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C8>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(1);
                #endif
                Run(ref data, runner, entities, count, with, entitiesParam, (byte) components);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> runner, uint[] entities, uint count, P with, EntityStatusType entitiesParam, byte components) where D : struct {
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

            var statuses = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                var i2 = di2[entity];
                var i3 = di3[entity];
                var i4 = di4[entity];
                var i5 = di5[entity];
                var i6 = di6[entity];
                var i7 = di7[entity];
                var i8 = di8[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == statuses[entity].Value)
                    && (
                        (components == 0 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyAndDisabledComponentMask) == 0) ||
                        (components == 1 && ((i1 | i2 | i3 | i4 | i5 | i6 | i7 | i8) & Const.EmptyComponentMask) == 0) ||
                        (components == 2 && (i1 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i2 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i3 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i4 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i5 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i6 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i7 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                                         && (i8 & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask
                        )
                    )
                    && with.CheckEntity(entity)) {
                    runner(ref data,
                           new World<WorldType>.Entity(entity),
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