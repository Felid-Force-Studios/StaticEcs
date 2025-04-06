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
    public readonly struct UQueryFunctionRunner<WorldType, C1, P>
        where P : struct, IQueryMethod
        where C1 : unmanaged, IComponent
        where WorldType : struct, IWorldType {
        internal static UQueryFunctionRunner<WorldType, C1, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, Ecs<WorldType>.IQueryFunction<C1> {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                #endif
            }
        }
        private unsafe void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) where R : struct, Ecs<WorldType>.IQueryFunction<C1> {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner.Run(new Ecs<WorldType>.Entity(entity),
                                   ref data1[i1 & Const.DisabledComponentMaskInv]
                        );
                    }
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunction<WorldType, C1> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                #endif
            }
        }

        private unsafe void Run(DelegateQueryFunction<WorldType, C1> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner(new Ecs<WorldType>.Entity(entity),
                               ref data1[i1 & Const.DisabledComponentMaskInv]
                        );
                    }
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct UQueryFunctionRunner<WorldType, C1, C2, P>
        where P : struct, IQueryMethod
        where C1 : unmanaged, IComponent
        where C2 : unmanaged, IComponent
        where WorldType : struct, IWorldType {
        internal static UQueryFunctionRunner<WorldType, C1, C2, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, Ecs<WorldType>.IQueryFunction<C1, C2> {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
                #endif
            }
        }

        private unsafe void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) where R : struct, Ecs<WorldType>.IQueryFunction<C1, C2> {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (uint* di2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (C2* data2 = Ecs<WorldType>.Components<C2>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    var i2 = di2[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner.Run(new Ecs<WorldType>.Entity(entity),
                                   ref data1[i1 & Const.DisabledComponentMaskInv],
                                   ref data2[i2 & Const.DisabledComponentMaskInv]
                        );
                    }
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunction<WorldType, C1, C2> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
                #endif
            }
        }

        private unsafe void Run(DelegateQueryFunction<WorldType, C1, C2> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (uint* di2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (C2* data2 = Ecs<WorldType>.Components<C2>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    var i2 = di2[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner(new Ecs<WorldType>.Entity(entity),
                               ref data1[i1 & Const.DisabledComponentMaskInv],
                               ref data2[i2 & Const.DisabledComponentMaskInv]
                        );
                    }
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct UQueryFunctionRunner<WorldType, C1, C2, C3, P>
        where P : struct, IQueryMethod
        where C1 : unmanaged, IComponent
        where C2 : unmanaged, IComponent
        where C3 : unmanaged, IComponent
        where WorldType : struct, IWorldType {
        internal static UQueryFunctionRunner<WorldType, C1, C2, C3, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, Ecs<WorldType>.IQueryFunction<C1, C2, C3> {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
                #endif
            }
        }

        private unsafe void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) where R : struct, Ecs<WorldType>.IQueryFunction<C1, C2, C3> {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (uint* di2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId())
            fixed (uint* di3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (C2* data2 = Ecs<WorldType>.Components<C2>.Value.Data())
            fixed (C3* data3 = Ecs<WorldType>.Components<C3>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    var i2 = di2[entity];
                    var i3 = di3[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                        && (i3 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner.Run(new Ecs<WorldType>.Entity(entity),
                                   ref data1[i1 & Const.DisabledComponentMaskInv],
                                   ref data2[i2 & Const.DisabledComponentMaskInv],
                                   ref data3[i3 & Const.DisabledComponentMaskInv]
                        );
                    }
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunction<WorldType, C1, C2, C3> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
                #endif
            }
        }

        private unsafe void Run(DelegateQueryFunction<WorldType, C1, C2, C3> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (uint* di2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId())
            fixed (uint* di3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (C2* data2 = Ecs<WorldType>.Components<C2>.Value.Data())
            fixed (C3* data3 = Ecs<WorldType>.Components<C3>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    var i2 = di2[entity];
                    var i3 = di3[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                        && (i3 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner(new Ecs<WorldType>.Entity(entity),
                               ref data1[i1 & Const.DisabledComponentMaskInv],
                               ref data2[i2 & Const.DisabledComponentMaskInv],
                               ref data3[i3 & Const.DisabledComponentMaskInv]
                        );
                    }
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct UQueryFunctionRunner<WorldType, C1, C2, C3, C4, P>
        where P : struct, IQueryMethod
        where C1 : unmanaged, IComponent
        where C2 : unmanaged, IComponent
        where C3 : unmanaged, IComponent
        where C4 : unmanaged, IComponent
        where WorldType : struct, IWorldType {
        internal static UQueryFunctionRunner<WorldType, C1, C2, C3, C4, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, Ecs<WorldType>.IQueryFunction<C1, C2, C3, C4> {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(-1);
                #endif
            }
        }

        private unsafe void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) where R : struct, Ecs<WorldType>.IQueryFunction<C1, C2, C3, C4> {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (uint* di2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId())
            fixed (uint* di3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId())
            fixed (uint* di4 = Ecs<WorldType>.Components<C4>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (C2* data2 = Ecs<WorldType>.Components<C2>.Value.Data())
            fixed (C3* data3 = Ecs<WorldType>.Components<C3>.Value.Data())
            fixed (C4* data4 = Ecs<WorldType>.Components<C4>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    var i2 = di2[entity];
                    var i3 = di3[entity];
                    var i4 = di4[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                        && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner.Run(new Ecs<WorldType>.Entity(entity),
                                   ref data1[i1 & Const.DisabledComponentMaskInv],
                                   ref data2[i2 & Const.DisabledComponentMaskInv],
                                   ref data3[i3 & Const.DisabledComponentMaskInv],
                                   ref data4[i4 & Const.DisabledComponentMaskInv]
                        );
                    }
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(-1);
                #endif
            }
        }

        private unsafe void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (uint* di2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId())
            fixed (uint* di3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId())
            fixed (uint* di4 = Ecs<WorldType>.Components<C4>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (C2* data2 = Ecs<WorldType>.Components<C2>.Value.Data())
            fixed (C3* data3 = Ecs<WorldType>.Components<C3>.Value.Data())
            fixed (C4* data4 = Ecs<WorldType>.Components<C4>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    var i2 = di2[entity];
                    var i3 = di3[entity];
                    var i4 = di4[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                        && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner(new Ecs<WorldType>.Entity(entity),
                               ref data1[i1 & Const.DisabledComponentMaskInv],
                               ref data2[i2 & Const.DisabledComponentMaskInv],
                               ref data3[i3 & Const.DisabledComponentMaskInv],
                               ref data4[i4 & Const.DisabledComponentMaskInv]
                        );
                    }
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct UQueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, P>
        where P : struct, IQueryMethod
        where C1 : unmanaged, IComponent
        where C2 : unmanaged, IComponent
        where C3 : unmanaged, IComponent
        where C4 : unmanaged, IComponent
        where C5 : unmanaged, IComponent
        where WorldType : struct, IWorldType {
        internal static UQueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, Ecs<WorldType>.IQueryFunction<C1, C2, C3, C4, C5> {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(-1);
                #endif
            }
        }

        private unsafe void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) where R : struct, Ecs<WorldType>.IQueryFunction<C1, C2, C3, C4, C5> {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (uint* di2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId())
            fixed (uint* di3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId())
            fixed (uint* di4 = Ecs<WorldType>.Components<C4>.Value.GetDataIdxByEntityId())
            fixed (uint* di5 = Ecs<WorldType>.Components<C5>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (C2* data2 = Ecs<WorldType>.Components<C2>.Value.Data())
            fixed (C3* data3 = Ecs<WorldType>.Components<C3>.Value.Data())
            fixed (C4* data4 = Ecs<WorldType>.Components<C4>.Value.Data())
            fixed (C5* data5 = Ecs<WorldType>.Components<C5>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    var i2 = di2[entity];
                    var i3 = di3[entity];
                    var i4 = di4[entity];
                    var i5 = di5[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                        && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner.Run(new Ecs<WorldType>.Entity(entity),
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

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(-1);
                #endif
            }
        }

        private unsafe void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (uint* di2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId())
            fixed (uint* di3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId())
            fixed (uint* di4 = Ecs<WorldType>.Components<C4>.Value.GetDataIdxByEntityId())
            fixed (uint* di5 = Ecs<WorldType>.Components<C5>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (C2* data2 = Ecs<WorldType>.Components<C2>.Value.Data())
            fixed (C3* data3 = Ecs<WorldType>.Components<C3>.Value.Data())
            fixed (C4* data4 = Ecs<WorldType>.Components<C4>.Value.Data())
            fixed (C5* data5 = Ecs<WorldType>.Components<C5>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    var i2 = di2[entity];
                    var i3 = di3[entity];
                    var i4 = di4[entity];
                    var i5 = di5[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                        && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner(new Ecs<WorldType>.Entity(entity),
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
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct UQueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, P>
        where P : struct, IQueryMethod
        where C1 : unmanaged, IComponent
        where C2 : unmanaged, IComponent
        where C3 : unmanaged, IComponent
        where C4 : unmanaged, IComponent
        where C5 : unmanaged, IComponent
        where C6 : unmanaged, IComponent
        where WorldType : struct, IWorldType {
        internal static UQueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, Ecs<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6> {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C6>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C6>.Value.AddBlocker(-1);
                #endif
            }
        }

        private unsafe void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) where R : struct, Ecs<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6> {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (uint* di2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId())
            fixed (uint* di3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId())
            fixed (uint* di4 = Ecs<WorldType>.Components<C4>.Value.GetDataIdxByEntityId())
            fixed (uint* di5 = Ecs<WorldType>.Components<C5>.Value.GetDataIdxByEntityId())
            fixed (uint* di6 = Ecs<WorldType>.Components<C6>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (C2* data2 = Ecs<WorldType>.Components<C2>.Value.Data())
            fixed (C3* data3 = Ecs<WorldType>.Components<C3>.Value.Data())
            fixed (C4* data4 = Ecs<WorldType>.Components<C4>.Value.Data())
            fixed (C5* data5 = Ecs<WorldType>.Components<C5>.Value.Data())
            fixed (C6* data6 = Ecs<WorldType>.Components<C6>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    var i2 = di2[entity];
                    var i3 = di3[entity];
                    var i4 = di4[entity];
                    var i5 = di5[entity];
                    var i6 = di6[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                        && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
                        && (i6 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner.Run(new Ecs<WorldType>.Entity(entity),
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

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C6>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C6>.Value.AddBlocker(-1);
                #endif
            }
        }

        private unsafe void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (uint* di2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId())
            fixed (uint* di3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId())
            fixed (uint* di4 = Ecs<WorldType>.Components<C4>.Value.GetDataIdxByEntityId())
            fixed (uint* di5 = Ecs<WorldType>.Components<C5>.Value.GetDataIdxByEntityId())
            fixed (uint* di6 = Ecs<WorldType>.Components<C6>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (C2* data2 = Ecs<WorldType>.Components<C2>.Value.Data())
            fixed (C3* data3 = Ecs<WorldType>.Components<C3>.Value.Data())
            fixed (C4* data4 = Ecs<WorldType>.Components<C4>.Value.Data())
            fixed (C5* data5 = Ecs<WorldType>.Components<C5>.Value.Data())
            fixed (C6* data6 = Ecs<WorldType>.Components<C6>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    var i2 = di2[entity];
                    var i3 = di3[entity];
                    var i4 = di4[entity];
                    var i5 = di5[entity];
                    var i6 = di6[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                        && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
                        && (i6 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner(new Ecs<WorldType>.Entity(entity),
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
    }


    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct UQueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, P>
        where P : struct, IQueryMethod
        where C1 : unmanaged, IComponent
        where C2 : unmanaged, IComponent
        where C3 : unmanaged, IComponent
        where C4 : unmanaged, IComponent
        where C5 : unmanaged, IComponent
        where C6 : unmanaged, IComponent
        where C7 : unmanaged, IComponent
        where WorldType : struct, IWorldType {
        internal static UQueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, Ecs<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C6>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C7>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C6>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C7>.Value.AddBlocker(-1);
                #endif
            }
        }

        private unsafe void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) where R : struct, Ecs<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (uint* di2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId())
            fixed (uint* di3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId())
            fixed (uint* di4 = Ecs<WorldType>.Components<C4>.Value.GetDataIdxByEntityId())
            fixed (uint* di5 = Ecs<WorldType>.Components<C5>.Value.GetDataIdxByEntityId())
            fixed (uint* di6 = Ecs<WorldType>.Components<C6>.Value.GetDataIdxByEntityId())
            fixed (uint* di7 = Ecs<WorldType>.Components<C7>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (C2* data2 = Ecs<WorldType>.Components<C2>.Value.Data())
            fixed (C3* data3 = Ecs<WorldType>.Components<C3>.Value.Data())
            fixed (C4* data4 = Ecs<WorldType>.Components<C4>.Value.Data())
            fixed (C5* data5 = Ecs<WorldType>.Components<C5>.Value.Data())
            fixed (C6* data6 = Ecs<WorldType>.Components<C6>.Value.Data())
            fixed (C7* data7 = Ecs<WorldType>.Components<C7>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    var i2 = di2[entity];
                    var i3 = di3[entity];
                    var i4 = di4[entity];
                    var i5 = di5[entity];
                    var i6 = di6[entity];
                    var i7 = di7[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                        && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
                        && (i6 & maskLeft) == maskRight && (i7 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner.Run(new Ecs<WorldType>.Entity(entity),
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

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6, C7> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C6>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C7>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C6>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C7>.Value.AddBlocker(-1);
                #endif
            }
        }

        private unsafe void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6, C7> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (uint* di2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId())
            fixed (uint* di3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId())
            fixed (uint* di4 = Ecs<WorldType>.Components<C4>.Value.GetDataIdxByEntityId())
            fixed (uint* di5 = Ecs<WorldType>.Components<C5>.Value.GetDataIdxByEntityId())
            fixed (uint* di6 = Ecs<WorldType>.Components<C6>.Value.GetDataIdxByEntityId())
            fixed (uint* di7 = Ecs<WorldType>.Components<C7>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (C2* data2 = Ecs<WorldType>.Components<C2>.Value.Data())
            fixed (C3* data3 = Ecs<WorldType>.Components<C3>.Value.Data())
            fixed (C4* data4 = Ecs<WorldType>.Components<C4>.Value.Data())
            fixed (C5* data5 = Ecs<WorldType>.Components<C5>.Value.Data())
            fixed (C6* data6 = Ecs<WorldType>.Components<C6>.Value.Data())
            fixed (C7* data7 = Ecs<WorldType>.Components<C7>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    var i2 = di2[entity];
                    var i3 = di3[entity];
                    var i4 = di4[entity];
                    var i5 = di5[entity];
                    var i6 = di6[entity];
                    var i7 = di7[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                        && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
                        && (i6 & maskLeft) == maskRight && (i7 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner(new Ecs<WorldType>.Entity(entity),
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
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct UQueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, P>
        where P : struct, IQueryMethod
        where C1 : unmanaged, IComponent
        where C2 : unmanaged, IComponent
        where C3 : unmanaged, IComponent
        where C4 : unmanaged, IComponent
        where C5 : unmanaged, IComponent
        where C6 : unmanaged, IComponent
        where C7 : unmanaged, IComponent
        where C8 : unmanaged, IComponent
        where WorldType : struct, IWorldType {
        internal static UQueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, Ecs<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C8>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C6>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C7>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C8>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C6>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C7>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C8>.Value.AddBlocker(-1);
                #endif
            }
        }

        private unsafe void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) where R : struct, Ecs<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (uint* di2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId())
            fixed (uint* di3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId())
            fixed (uint* di4 = Ecs<WorldType>.Components<C4>.Value.GetDataIdxByEntityId())
            fixed (uint* di5 = Ecs<WorldType>.Components<C5>.Value.GetDataIdxByEntityId())
            fixed (uint* di6 = Ecs<WorldType>.Components<C6>.Value.GetDataIdxByEntityId())
            fixed (uint* di7 = Ecs<WorldType>.Components<C7>.Value.GetDataIdxByEntityId())
            fixed (uint* di8 = Ecs<WorldType>.Components<C8>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (C2* data2 = Ecs<WorldType>.Components<C2>.Value.Data())
            fixed (C3* data3 = Ecs<WorldType>.Components<C3>.Value.Data())
            fixed (C4* data4 = Ecs<WorldType>.Components<C4>.Value.Data())
            fixed (C5* data5 = Ecs<WorldType>.Components<C5>.Value.Data())
            fixed (C6* data6 = Ecs<WorldType>.Components<C6>.Value.Data())
            fixed (C7* data7 = Ecs<WorldType>.Components<C7>.Value.Data())
            fixed (C8* data8 = Ecs<WorldType>.Components<C8>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    var i2 = di2[entity];
                    var i3 = di3[entity];
                    var i4 = di4[entity];
                    var i5 = di5[entity];
                    var i6 = di6[entity];
                    var i7 = di7[entity];
                    var i8 = di8[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                        && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
                        && (i6 & maskLeft) == maskRight && (i7 & maskLeft) == maskRight && (i8 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner.Run(new Ecs<WorldType>.Entity(entity),
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

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
            var count = Ecs<WorldType>.Components<C1>.Value.Count();
            var entities = Ecs<WorldType>.Components<C1>.Value.EntitiesData();
            Ecs<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref count, ref entities);
            Ecs<WorldType>.Components<C8>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C6>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C7>.Value.AddBlocker(1);
                Ecs<WorldType>.Components<C8>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                with.Dispose<WorldType>();
                Ecs<WorldType>.Components<C1>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C2>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C3>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C4>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C5>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C6>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C7>.Value.AddBlocker(-1);
                Ecs<WorldType>.Components<C8>.Value.AddBlocker(-1);
                #endif
            }
        }

        private unsafe void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
            fixed (uint* di1 = Ecs<WorldType>.Components<C1>.Value.GetDataIdxByEntityId())
            fixed (uint* di2 = Ecs<WorldType>.Components<C2>.Value.GetDataIdxByEntityId())
            fixed (uint* di3 = Ecs<WorldType>.Components<C3>.Value.GetDataIdxByEntityId())
            fixed (uint* di4 = Ecs<WorldType>.Components<C4>.Value.GetDataIdxByEntityId())
            fixed (uint* di5 = Ecs<WorldType>.Components<C5>.Value.GetDataIdxByEntityId())
            fixed (uint* di6 = Ecs<WorldType>.Components<C6>.Value.GetDataIdxByEntityId())
            fixed (uint* di7 = Ecs<WorldType>.Components<C7>.Value.GetDataIdxByEntityId())
            fixed (uint* di8 = Ecs<WorldType>.Components<C8>.Value.GetDataIdxByEntityId())
            fixed (C1* data1 = Ecs<WorldType>.Components<C1>.Value.Data())
            fixed (C2* data2 = Ecs<WorldType>.Components<C2>.Value.Data())
            fixed (C3* data3 = Ecs<WorldType>.Components<C3>.Value.Data())
            fixed (C4* data4 = Ecs<WorldType>.Components<C4>.Value.Data())
            fixed (C5* data5 = Ecs<WorldType>.Components<C5>.Value.Data())
            fixed (C6* data6 = Ecs<WorldType>.Components<C6>.Value.Data())
            fixed (C7* data7 = Ecs<WorldType>.Components<C7>.Value.Data())
            fixed (C8* data8 = Ecs<WorldType>.Components<C8>.Value.Data())
            fixed (uint* en = entities)
            fixed (EntityStatus* status = Ecs<WorldType>.StandardComponents<EntityStatus>.Value.Data())
            {
                while (count > 0) {
                    count--;
                    var entity = en[count];
                    var i1 = di1[entity];
                    var i2 = di2[entity];
                    var i3 = di3[entity];
                    var i4 = di4[entity];
                    var i5 = di5[entity];
                    var i6 = di6[entity];
                    var i7 = di7[entity];
                    var i8 = di8[entity];
                    if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                        && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                        && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
                        && (i6 & maskLeft) == maskRight && (i7 & maskLeft) == maskRight && (i8 & maskLeft) == maskRight
                        && with.CheckEntity(entity)) {
                        runner(new Ecs<WorldType>.Entity(entity),
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
    }
    #endregion
}