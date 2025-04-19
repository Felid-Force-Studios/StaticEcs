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
    public abstract partial class World<WorldType> {
        #region ABSTRACT_QUERY_FUNCTION
        public interface IQueryFunction<C1>
            where C1 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1);
        }

        public interface IQueryFunction<C1, C2>
            where C1 : struct, IComponent
            where C2 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1, ref C2 c2);
        }

        public interface IQueryFunction<C1, C2, C3>
            where C1 : struct, IComponent
            where C2 : struct, IComponent
            where C3 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1, ref C2 c2, ref C3 c3);
        }

        public interface IQueryFunction<C1, C2, C3, C4>
            where C1 : struct, IComponent
            where C2 : struct, IComponent
            where C3 : struct, IComponent
            where C4 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4);
        }

        public interface IQueryFunction<C1, C2, C3, C4, C5>
            where C1 : struct, IComponent
            where C2 : struct, IComponent
            where C3 : struct, IComponent
            where C4 : struct, IComponent
            where C5 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5);
        }

        public interface IQueryFunction<C1, C2, C3, C4, C5, C6>
            where C1 : struct, IComponent
            where C2 : struct, IComponent
            where C3 : struct, IComponent
            where C4 : struct, IComponent
            where C5 : struct, IComponent
            where C6 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6);
        }

        public interface IQueryFunction<C1, C2, C3, C4, C5, C6, C7>
            where C1 : struct, IComponent
            where C2 : struct, IComponent
            where C3 : struct, IComponent
            where C4 : struct, IComponent
            where C5 : struct, IComponent
            where C6 : struct, IComponent
            where C7 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7);
        }

        public interface IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8>
            where C1 : struct, IComponent
            where C2 : struct, IComponent
            where C3 : struct, IComponent
            where C4 : struct, IComponent
            where C5 : struct, IComponent
            where C6 : struct, IComponent
            where C7 : struct, IComponent
            where C8 : struct, IComponent {
            public void Run(Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7, ref C8 c8);
        }
        #endregion
    }

    #region DELEFATE_QUERY_FUNCTION
    public delegate void DelegateQueryFunction<WorldType, C1>(World<WorldType>.Entity entity, ref C1 c1)
        where C1 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void DelegateQueryFunction<WorldType, C1, C2>(World<WorldType>.Entity entity, ref C1 c1, ref C2 c2)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void DelegateQueryFunction<WorldType, C1, C2, C3>(World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void DelegateQueryFunction<WorldType, C1, C2, C3, C4>(World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5>(World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6>(World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6, C7>(World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7)
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where WorldType : struct, IWorldType;

    public delegate void DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6, C7, C8>(
        World<WorldType>.Entity entity, ref C1 c1, ref C2 c2, ref C3 c3, ref C4 c4, ref C5 c5, ref C6 c6, ref C7 c7, ref C8 c8
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

    #region QUERY_FUNCTION_RUNNER
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct QueryFunctionRunner<WorldType, C1, P>
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where WorldType : struct, IWorldType {
        internal static QueryFunctionRunner<WorldType, C1, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, World<WorldType>.IQueryFunction<C1> {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count)
            where R : struct, World<WorldType>.IQueryFunction<C1> {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();

            var data1 = World<WorldType>.Components<C1>.Value.Data();

            var status = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                    && (i1 & maskLeft) == maskRight
                    && with.CheckEntity(entity)) {
                    runner.Run(new World<WorldType>.Entity(entity),
                               ref data1[i1 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunction<WorldType, C1> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run(DelegateQueryFunction<WorldType, C1> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
            var di1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();

            var data1 = World<WorldType>.Components<C1>.Value.Data();

            var status = World<WorldType>.StandardComponents<EntityStatus>.Value.Data();

            while (count > 0) {
                count--;
                var entity = entities[count];
                var i1 = di1[entity];
                if ((entitiesParam == EntityStatusType.Any || entitiesParam == status[entity].Value)
                    && (i1 & maskLeft) == maskRight
                    && with.CheckEntity(entity)) {
                    runner(new World<WorldType>.Entity(entity),
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
    public readonly struct QueryFunctionRunner<WorldType, C1, C2, P>
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where WorldType : struct, IWorldType {
        internal static QueryFunctionRunner<WorldType, C1, C2, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, World<WorldType>.IQueryFunction<C1, C2> {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(-1);
                World<WorldType>.Components<C2>.Value.AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }
        
        private void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count)
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
                    && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                    && with.CheckEntity(entity)) {
                    runner.Run(new World<WorldType>.Entity(entity),
                               ref data1[i1 & Const.DisabledComponentMaskInv],
                               ref data2[i2 & Const.DisabledComponentMaskInv]
                    );
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run(DelegateQueryFunction<WorldType, C1, C2> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(-1);
                World<WorldType>.Components<C2>.Value.AddBlocker(-1);
                #endif
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            with.Dispose<WorldType>();
            #endif
        }

        private void Run(DelegateQueryFunction<WorldType, C1, C2> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
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
                    && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                    && with.CheckEntity(entity)) {
                    runner(new World<WorldType>.Entity(entity),
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
    public readonly struct QueryFunctionRunner<WorldType, C1, C2, C3, P>
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where WorldType : struct, IWorldType {
        internal static QueryFunctionRunner<WorldType, C1, C2, C3, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3> {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
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
        
        private void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count)
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
                    && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                    && (i3 & maskLeft) == maskRight
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
        public void Run(DelegateQueryFunction<WorldType, C1, C2, C3> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
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

        private void Run(DelegateQueryFunction<WorldType, C1, C2, C3> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
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
                    && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                    && (i3 & maskLeft) == maskRight
                    && with.CheckEntity(entity)) {
                    runner(new World<WorldType>.Entity(entity),
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
    public readonly struct QueryFunctionRunner<WorldType, C1, C2, C3, C4, P>
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where WorldType : struct, IWorldType {
        internal static QueryFunctionRunner<WorldType, C1, C2, C3, C4, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4> {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                World<WorldType>.Components<C4>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
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
        
        private void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count)
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
                    && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                    && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight
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
        public void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                World<WorldType>.Components<C4>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
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

        private void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
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
                    && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                    && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight
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
        internal static QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4, C5> {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                World<WorldType>.Components<C4>.Value.AddBlocker(1);
                World<WorldType>.Components<C5>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
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
        
        private void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count)
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
                    && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                    && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
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
        public void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
            var count = World<WorldType>.Components<C1>.Value.Count();
            var entities = World<WorldType>.Components<C1>.Value.EntitiesData();
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref count, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref count, ref entities);
            with.SetData<WorldType>(ref count, ref entities);

            if (count > 0) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                World<WorldType>.Components<C4>.Value.AddBlocker(1);
                World<WorldType>.Components<C5>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
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

        private void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
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
                    && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                    && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
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
        internal static QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6> {
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
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                World<WorldType>.Components<C4>.Value.AddBlocker(1);
                World<WorldType>.Components<C5>.Value.AddBlocker(1);
                World<WorldType>.Components<C6>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
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
        
        private void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count)
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
                    && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                    && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
                    && (i6 & maskLeft) == maskRight
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
        public void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
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
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                World<WorldType>.Components<C4>.Value.AddBlocker(1);
                World<WorldType>.Components<C5>.Value.AddBlocker(1);
                World<WorldType>.Components<C6>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
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

        private void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
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
                    && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                    && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
                    && (i6 & maskLeft) == maskRight
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
        internal static QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
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
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                World<WorldType>.Components<C4>.Value.AddBlocker(1);
                World<WorldType>.Components<C5>.Value.AddBlocker(1);
                World<WorldType>.Components<C6>.Value.AddBlocker(1);
                World<WorldType>.Components<C7>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
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
        
        private void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count)
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
                    && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                    && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
                    && (i6 & maskLeft) == maskRight && (i7 & maskLeft) == maskRight
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
        public void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6, C7> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
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
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                World<WorldType>.Components<C4>.Value.AddBlocker(1);
                World<WorldType>.Components<C5>.Value.AddBlocker(1);
                World<WorldType>.Components<C6>.Value.AddBlocker(1);
                World<WorldType>.Components<C7>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
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

        private void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6, C7> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
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
                    && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                    && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
                    && (i6 & maskLeft) == maskRight && (i7 & maskLeft) == maskRight
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
        internal static QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, P> Value;

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
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
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                World<WorldType>.Components<C4>.Value.AddBlocker(1);
                World<WorldType>.Components<C5>.Value.AddBlocker(1);
                World<WorldType>.Components<C6>.Value.AddBlocker(1);
                World<WorldType>.Components<C7>.Value.AddBlocker(1);
                World<WorldType>.Components<C8>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
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
        
        private void Run<R>(R runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count)
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
                    && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                    && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
                    && (i6 & maskLeft) == maskRight && (i7 & maskLeft) == maskRight && (i8 & maskLeft) == maskRight
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
        public void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight) {
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
                World<WorldType>.Components<C1>.Value.AddBlocker(1);
                World<WorldType>.Components<C2>.Value.AddBlocker(1);
                World<WorldType>.Components<C3>.Value.AddBlocker(1);
                World<WorldType>.Components<C4>.Value.AddBlocker(1);
                World<WorldType>.Components<C5>.Value.AddBlocker(1);
                World<WorldType>.Components<C6>.Value.AddBlocker(1);
                World<WorldType>.Components<C7>.Value.AddBlocker(1);
                World<WorldType>.Components<C8>.Value.AddBlocker(1);
                #endif
                Run(runner, with, entitiesParam, maskLeft, maskRight, entities, count);
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

        private void Run(DelegateQueryFunction<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> runner, P with, EntityStatusType entitiesParam, uint maskLeft, uint maskRight, uint[] entities, uint count) {
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
                    && (i1 & maskLeft) == maskRight && (i2 & maskLeft) == maskRight
                    && (i3 & maskLeft) == maskRight && (i4 & maskLeft) == maskRight && (i5 & maskLeft) == maskRight
                    && (i6 & maskLeft) == maskRight && (i7 & maskLeft) == maskRight && (i8 & maskLeft) == maskRight
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
    }
    #endregion
}