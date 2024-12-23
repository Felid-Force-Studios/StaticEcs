﻿using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public static class WithAdds {
        [MethodImpl(AggressiveInlining)]
        public static WithAdds<QM1> Create<QM1>(QM1 qm1) where QM1 : struct, ISealedQueryMethod {
            return new WithAdds<QM1>(qm1);
        }

        [MethodImpl(AggressiveInlining)]
        public static WithAdds<QM1, QM2> Create<QM1, QM2>(QM1 qm1, QM2 qm2)
            where QM1 : struct, ISealedQueryMethod
            where QM2 : struct, ISealedQueryMethod {
            return new WithAdds<QM1, QM2>(qm1, qm2);
        }

        [MethodImpl(AggressiveInlining)]
        public static WithAdds<QM1, QM2, QM3> Create<QM1, QM2, QM3>(QM1 qm1, QM2 qm2, QM3 qm3)
            where QM1 : struct, ISealedQueryMethod
            where QM2 : struct, ISealedQueryMethod
            where QM3 : struct, ISealedQueryMethod {
            return new WithAdds<QM1, QM2, QM3>(qm1, qm2, qm3);
        }

        [MethodImpl(AggressiveInlining)]
        public static WithAdds<QM1, QM2, QM3, QM4> Create<QM1, QM2, QM3, QM4>(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4)
            where QM1 : struct, ISealedQueryMethod
            where QM2 : struct, ISealedQueryMethod
            where QM3 : struct, ISealedQueryMethod
            where QM4 : struct, ISealedQueryMethod {
            return new WithAdds<QM1, QM2, QM3, QM4>(qm1, qm2, qm3, qm4);
        }

        [MethodImpl(AggressiveInlining)]
        public static WithAdds<QM1, QM2, QM3, QM4, QM5> Create<QM1, QM2, QM3, QM4, QM5>(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5)
            where QM1 : struct, ISealedQueryMethod
            where QM2 : struct, ISealedQueryMethod
            where QM3 : struct, ISealedQueryMethod
            where QM4 : struct, ISealedQueryMethod
            where QM5 : struct, ISealedQueryMethod {
            return new WithAdds<QM1, QM2, QM3, QM4, QM5>(qm1, qm2, qm3, qm4, qm5);
        }

        [MethodImpl(AggressiveInlining)]
        public static WithAdds<QM1, QM2, QM3, QM4, QM5, QM6> Create<QM1, QM2, QM3, QM4, QM5, QM6>(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5, QM6 qm6)
            where QM1 : struct, ISealedQueryMethod
            where QM2 : struct, ISealedQueryMethod
            where QM3 : struct, ISealedQueryMethod
            where QM4 : struct, ISealedQueryMethod
            where QM5 : struct, ISealedQueryMethod
            where QM6 : struct, ISealedQueryMethod {
            return new WithAdds<QM1, QM2, QM3, QM4, QM5, QM6>(qm1, qm2, qm3, qm4, qm5, qm6);
        }

        [MethodImpl(AggressiveInlining)]
        public static WithAdds<QM1, QM2, QM3, QM4, QM5, QM6, QM7> Create<QM1, QM2, QM3, QM4, QM5, QM6, QM7>(
            QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5, QM6 qm6, QM7 qm7
        )
            where QM1 : struct, ISealedQueryMethod
            where QM2 : struct, ISealedQueryMethod
            where QM3 : struct, ISealedQueryMethod
            where QM4 : struct, ISealedQueryMethod
            where QM5 : struct, ISealedQueryMethod
            where QM6 : struct, ISealedQueryMethod
            where QM7 : struct, ISealedQueryMethod {
            return new WithAdds<QM1, QM2, QM3, QM4, QM5, QM6, QM7>(qm1, qm2, qm3, qm4, qm5, qm6, qm7);
        }

        [MethodImpl(AggressiveInlining)]
        public static WithAdds<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8> Create<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8>(
            QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5, QM6 qm6, QM7 qm7, QM8 qm8
        )
            where QM1 : struct, ISealedQueryMethod
            where QM2 : struct, ISealedQueryMethod
            where QM3 : struct, ISealedQueryMethod
            where QM4 : struct, ISealedQueryMethod
            where QM5 : struct, ISealedQueryMethod
            where QM6 : struct, ISealedQueryMethod
            where QM7 : struct, ISealedQueryMethod
            where QM8 : struct, ISealedQueryMethod {
            return new WithAdds<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8>(qm1, qm2, qm3, qm4, qm5, qm6, qm7, qm8);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct WithAddsNothing : IQueryMethod {
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minComponentsCount, ref Ecs<WorldID>.Entity[] minEntities) where WorldID : struct, IWorldId { }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return true;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId { }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct WithAdds<QM1> : IQueryMethod
        where QM1 : struct, ISealedQueryMethod {
        private QM1 _qm1;

        public WithAdds(QM1 qm1) {
            _qm1 = qm1;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minComponentsCount, ref Ecs<WorldID>.Entity[] minEntities) where WorldID : struct, IWorldId {
            _qm1.SetData(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _qm1.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            _qm1.Dispose<WorldID>();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct WithAdds<QM1, QM2> : IQueryMethod
        where QM1 : struct, ISealedQueryMethod
        where QM2 : struct, ISealedQueryMethod {
        private QM1 qm1;
        private QM2 qm2;

        public WithAdds(QM1 qm1, QM2 qm2) {
            this.qm1 = qm1;
            this.qm2 = qm2;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minComponentsCount, ref Ecs<WorldID>.Entity[] minEntities) where WorldID : struct, IWorldId {
            qm1.SetData(ref minComponentsCount, ref minEntities);
            qm2.SetData(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return qm1.CheckEntity(entityId) && qm2.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            qm1.Dispose<WorldID>();
            qm2.Dispose<WorldID>();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct WithAdds<QM1, QM2, QM3> : IQueryMethod
        where QM1 : struct, ISealedQueryMethod
        where QM2 : struct, ISealedQueryMethod
        where QM3 : struct, ISealedQueryMethod {
        private QM1 qm1;
        private QM2 qm2;
        private QM3 qm3;

        public WithAdds(QM1 qm1, QM2 qm2, QM3 qm3) {
            this.qm1 = qm1;
            this.qm2 = qm2;
            this.qm3 = qm3;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minComponentsCount, ref Ecs<WorldID>.Entity[] minEntities) where WorldID : struct, IWorldId {
            qm1.SetData(ref minComponentsCount, ref minEntities);
            qm2.SetData(ref minComponentsCount, ref minEntities);
            qm3.SetData(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return qm1.CheckEntity(entityId) && qm2.CheckEntity(entityId) && qm3.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            qm1.Dispose<WorldID>();
            qm2.Dispose<WorldID>();
            qm3.Dispose<WorldID>();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct WithAdds<QM1, QM2, QM3, QM4> : IQueryMethod
        where QM1 : struct, ISealedQueryMethod
        where QM2 : struct, ISealedQueryMethod
        where QM3 : struct, ISealedQueryMethod
        where QM4 : struct, ISealedQueryMethod {
        private QM1 qm1;
        private QM2 qm2;
        private QM3 qm3;
        private QM4 qm4;

        public WithAdds(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4) {
            this.qm1 = qm1;
            this.qm2 = qm2;
            this.qm3 = qm3;
            this.qm4 = qm4;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minComponentsCount, ref Ecs<WorldID>.Entity[] minEntities) where WorldID : struct, IWorldId {
            qm1.SetData(ref minComponentsCount, ref minEntities);
            qm2.SetData(ref minComponentsCount, ref minEntities);
            qm3.SetData(ref minComponentsCount, ref minEntities);
            qm4.SetData(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return qm1.CheckEntity(entityId) && qm2.CheckEntity(entityId) && qm3.CheckEntity(entityId) && qm4.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            qm1.Dispose<WorldID>();
            qm2.Dispose<WorldID>();
            qm3.Dispose<WorldID>();
            qm4.Dispose<WorldID>();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct WithAdds<QM1, QM2, QM3, QM4, QM5> : IQueryMethod
        where QM1 : struct, ISealedQueryMethod
        where QM2 : struct, ISealedQueryMethod
        where QM3 : struct, ISealedQueryMethod
        where QM4 : struct, ISealedQueryMethod
        where QM5 : struct, ISealedQueryMethod {
        private QM1 qm1;
        private QM2 qm2;
        private QM3 qm3;
        private QM4 qm4;
        private QM5 qm5;

        public WithAdds(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5) {
            this.qm1 = qm1;
            this.qm2 = qm2;
            this.qm3 = qm3;
            this.qm4 = qm4;
            this.qm5 = qm5;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minComponentsCount, ref Ecs<WorldID>.Entity[] minEntities) where WorldID : struct, IWorldId {
            qm1.SetData(ref minComponentsCount, ref minEntities);
            qm2.SetData(ref minComponentsCount, ref minEntities);
            qm3.SetData(ref minComponentsCount, ref minEntities);
            qm4.SetData(ref minComponentsCount, ref minEntities);
            qm5.SetData(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return qm1.CheckEntity(entityId) && qm2.CheckEntity(entityId) && qm3.CheckEntity(entityId) && qm4.CheckEntity(entityId) && qm5.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            qm1.Dispose<WorldID>();
            qm2.Dispose<WorldID>();
            qm3.Dispose<WorldID>();
            qm4.Dispose<WorldID>();
            qm5.Dispose<WorldID>();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct WithAdds<QM1, QM2, QM3, QM4, QM5, QM6> : IQueryMethod
        where QM1 : struct, ISealedQueryMethod
        where QM2 : struct, ISealedQueryMethod
        where QM3 : struct, ISealedQueryMethod
        where QM4 : struct, ISealedQueryMethod
        where QM5 : struct, ISealedQueryMethod
        where QM6 : struct, ISealedQueryMethod {
        private QM1 qm1;
        private QM2 qm2;
        private QM3 qm3;
        private QM4 qm4;
        private QM5 qm5;
        private QM6 qm6;

        public WithAdds(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5, QM6 qm6) {
            this.qm1 = qm1;
            this.qm2 = qm2;
            this.qm3 = qm3;
            this.qm4 = qm4;
            this.qm5 = qm5;
            this.qm6 = qm6;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minComponentsCount, ref Ecs<WorldID>.Entity[] minEntities) where WorldID : struct, IWorldId {
            qm1.SetData(ref minComponentsCount, ref minEntities);
            qm2.SetData(ref minComponentsCount, ref minEntities);
            qm3.SetData(ref minComponentsCount, ref minEntities);
            qm4.SetData(ref minComponentsCount, ref minEntities);
            qm5.SetData(ref minComponentsCount, ref minEntities);
            qm6.SetData(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return qm1.CheckEntity(entityId) && qm2.CheckEntity(entityId) && qm3.CheckEntity(entityId) && qm4.CheckEntity(entityId) && qm5.CheckEntity(entityId) && qm6.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            qm1.Dispose<WorldID>();
            qm2.Dispose<WorldID>();
            qm3.Dispose<WorldID>();
            qm4.Dispose<WorldID>();
            qm5.Dispose<WorldID>();
            qm6.Dispose<WorldID>();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct WithAdds<QM1, QM2, QM3, QM4, QM5, QM6, QM7> : IQueryMethod
        where QM1 : struct, ISealedQueryMethod
        where QM2 : struct, ISealedQueryMethod
        where QM3 : struct, ISealedQueryMethod
        where QM4 : struct, ISealedQueryMethod
        where QM5 : struct, ISealedQueryMethod
        where QM6 : struct, ISealedQueryMethod
        where QM7 : struct, ISealedQueryMethod {
        private QM1 qm1;
        private QM2 qm2;
        private QM3 qm3;
        private QM4 qm4;
        private QM5 qm5;
        private QM6 qm6;
        private QM7 qm7;

        public WithAdds(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5, QM6 qm6, QM7 qm7) {
            this.qm1 = qm1;
            this.qm2 = qm2;
            this.qm3 = qm3;
            this.qm4 = qm4;
            this.qm5 = qm5;
            this.qm6 = qm6;
            this.qm7 = qm7;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minComponentsCount, ref Ecs<WorldID>.Entity[] minEntities) where WorldID : struct, IWorldId {
            qm1.SetData(ref minComponentsCount, ref minEntities);
            qm2.SetData(ref minComponentsCount, ref minEntities);
            qm3.SetData(ref minComponentsCount, ref minEntities);
            qm4.SetData(ref minComponentsCount, ref minEntities);
            qm5.SetData(ref minComponentsCount, ref minEntities);
            qm6.SetData(ref minComponentsCount, ref minEntities);
            qm7.SetData(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return qm1.CheckEntity(entityId) && qm2.CheckEntity(entityId) && qm3.CheckEntity(entityId) && qm4.CheckEntity(entityId) && qm5.CheckEntity(entityId) && qm6.CheckEntity(entityId) && qm7.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            qm1.Dispose<WorldID>();
            qm2.Dispose<WorldID>();
            qm3.Dispose<WorldID>();
            qm4.Dispose<WorldID>();
            qm5.Dispose<WorldID>();
            qm6.Dispose<WorldID>();
            qm7.Dispose<WorldID>();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct WithAdds<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8> : IQueryMethod
        where QM1 : struct, ISealedQueryMethod
        where QM2 : struct, ISealedQueryMethod
        where QM3 : struct, ISealedQueryMethod
        where QM4 : struct, ISealedQueryMethod
        where QM5 : struct, ISealedQueryMethod
        where QM6 : struct, ISealedQueryMethod
        where QM7 : struct, ISealedQueryMethod
        where QM8 : struct, ISealedQueryMethod {
        private QM1 qm1;
        private QM2 qm2;
        private QM3 qm3;
        private QM4 qm4;
        private QM5 qm5;
        private QM6 qm6;
        private QM7 qm7;
        private QM8 qm8;

        public WithAdds(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5, QM6 qm6, QM7 qm7, QM8 qm8) {
            this.qm1 = qm1;
            this.qm2 = qm2;
            this.qm3 = qm3;
            this.qm4 = qm4;
            this.qm5 = qm5;
            this.qm6 = qm6;
            this.qm7 = qm7;
            this.qm8 = qm8;
        }


        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minComponentsCount, ref Ecs<WorldID>.Entity[] minEntities) where WorldID : struct, IWorldId {
            qm1.SetData(ref minComponentsCount, ref minEntities);
            qm2.SetData(ref minComponentsCount, ref minEntities);
            qm3.SetData(ref minComponentsCount, ref minEntities);
            qm4.SetData(ref minComponentsCount, ref minEntities);
            qm5.SetData(ref minComponentsCount, ref minEntities);
            qm6.SetData(ref minComponentsCount, ref minEntities);
            qm7.SetData(ref minComponentsCount, ref minEntities);
            qm8.SetData(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return qm1.CheckEntity(entityId) && qm2.CheckEntity(entityId) && qm3.CheckEntity(entityId) && qm4.CheckEntity(entityId) && qm5.CheckEntity(entityId) && qm6.CheckEntity(entityId) && qm7.CheckEntity(entityId) && qm8.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            qm1.Dispose<WorldID>();
            qm2.Dispose<WorldID>();
            qm3.Dispose<WorldID>();
            qm4.Dispose<WorldID>();
            qm5.Dispose<WorldID>();
            qm6.Dispose<WorldID>();
            qm7.Dispose<WorldID>();
            qm8.Dispose<WorldID>();
        }
    }
}