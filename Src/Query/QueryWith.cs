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
    public static class With {
        [MethodImpl(AggressiveInlining)]
        public static With<QM1> Create<QM1>(QM1 qm1) where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod {
            return new With<QM1>(qm1);
        }

        [MethodImpl(AggressiveInlining)]
        public static With<QM1, QM2> Create<QM1, QM2>(QM1 qm1, QM2 qm2)
            where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
            where QM2 : struct, ISealedQueryMethod {
            return new With<QM1, QM2>(qm1, qm2);
        }

        [MethodImpl(AggressiveInlining)]
        public static With<QM1, QM2, QM3> Create<QM1, QM2, QM3>(QM1 qm1, QM2 qm2, QM3 qm3)
            where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
            where QM2 : struct, ISealedQueryMethod
            where QM3 : struct, ISealedQueryMethod {
            return new With<QM1, QM2, QM3>(qm1, qm2, qm3);
        }

        [MethodImpl(AggressiveInlining)]
        public static With<QM1, QM2, QM3, QM4> Create<QM1, QM2, QM3, QM4>(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4)
            where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
            where QM2 : struct, ISealedQueryMethod
            where QM3 : struct, ISealedQueryMethod
            where QM4 : struct, ISealedQueryMethod {
            return new With<QM1, QM2, QM3, QM4>(qm1, qm2, qm3, qm4);
        }

        [MethodImpl(AggressiveInlining)]
        public static With<QM1, QM2, QM3, QM4, QM5> Create<QM1, QM2, QM3, QM4, QM5>(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5)
            where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
            where QM2 : struct, ISealedQueryMethod
            where QM3 : struct, ISealedQueryMethod
            where QM4 : struct, ISealedQueryMethod
            where QM5 : struct, ISealedQueryMethod {
            return new With<QM1, QM2, QM3, QM4, QM5>(qm1, qm2, qm3, qm4, qm5);
        }

        [MethodImpl(AggressiveInlining)]
        public static With<QM1, QM2, QM3, QM4, QM5, QM6> Create<QM1, QM2, QM3, QM4, QM5, QM6>(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5, QM6 qm6)
            where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
            where QM2 : struct, ISealedQueryMethod
            where QM3 : struct, ISealedQueryMethod
            where QM4 : struct, ISealedQueryMethod
            where QM5 : struct, ISealedQueryMethod
            where QM6 : struct, ISealedQueryMethod {
            return new With<QM1, QM2, QM3, QM4, QM5, QM6>(qm1, qm2, qm3, qm4, qm5, qm6);
        }

        [MethodImpl(AggressiveInlining)]
        public static With<QM1, QM2, QM3, QM4, QM5, QM6, QM7> Create<QM1, QM2, QM3, QM4, QM5, QM6, QM7>(
            QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5, QM6 qm6, QM7 qm7
        )
            where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
            where QM2 : struct, ISealedQueryMethod
            where QM3 : struct, ISealedQueryMethod
            where QM4 : struct, ISealedQueryMethod
            where QM5 : struct, ISealedQueryMethod
            where QM6 : struct, ISealedQueryMethod
            where QM7 : struct, ISealedQueryMethod {
            return new With<QM1, QM2, QM3, QM4, QM5, QM6, QM7>(qm1, qm2, qm3, qm4, qm5, qm6, qm7);
        }

        [MethodImpl(AggressiveInlining)]
        public static With<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8> Create<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8>(
            QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5, QM6 qm6, QM7 qm7, QM8 qm8
        )
            where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
            where QM2 : struct, ISealedQueryMethod
            where QM3 : struct, ISealedQueryMethod
            where QM4 : struct, ISealedQueryMethod
            where QM5 : struct, ISealedQueryMethod
            where QM6 : struct, ISealedQueryMethod
            where QM7 : struct, ISealedQueryMethod
            where QM8 : struct, ISealedQueryMethod {
            return new With<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8>(qm1, qm2, qm3, qm4, qm5, qm6, qm7, qm8);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct WithNothing : IPrimaryQueryMethod {
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minComponentsCount, ref int[] minEntities) where WorldType : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return true;
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType { }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1> : IPrimaryQueryMethod
        where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod {
        private QM1 _qm1;

        public With(QM1 qm1) {
            _qm1 = qm1;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minComponentsCount, ref int[] minEntities) where WorldType : struct, IWorldType {
            _qm1.SetData<WorldType>(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return _qm1.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            _qm1.Dispose<WorldType>();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1, QM2> : IPrimaryQueryMethod
        where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
        where QM2 : struct, ISealedQueryMethod {
        private QM1 qm1;
        private QM2 qm2;

        public With(QM1 qm1, QM2 qm2) {
            this.qm1 = qm1;
            this.qm2 = qm2;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minComponentsCount, ref int[] minEntities) where WorldType : struct, IWorldType {
            qm1.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm2.SetData<WorldType>(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return qm1.CheckEntity(entityId) && qm2.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            qm1.Dispose<WorldType>();
            qm2.Dispose<WorldType>();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1, QM2, QM3> : IPrimaryQueryMethod
        where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
        where QM2 : struct, ISealedQueryMethod
        where QM3 : struct, ISealedQueryMethod {
        private QM1 qm1;
        private QM2 qm2;
        private QM3 qm3;

        public With(QM1 qm1, QM2 qm2, QM3 qm3) {
            this.qm1 = qm1;
            this.qm2 = qm2;
            this.qm3 = qm3;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minComponentsCount, ref int[] minEntities) where WorldType : struct, IWorldType {
            qm1.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm2.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm3.SetData<WorldType>(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return qm1.CheckEntity(entityId) && qm2.CheckEntity(entityId) && qm3.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            qm1.Dispose<WorldType>();
            qm2.Dispose<WorldType>();
            qm3.Dispose<WorldType>();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1, QM2, QM3, QM4> : IPrimaryQueryMethod
        where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
        where QM2 : struct, ISealedQueryMethod
        where QM3 : struct, ISealedQueryMethod
        where QM4 : struct, ISealedQueryMethod {
        private QM1 qm1;
        private QM2 qm2;
        private QM3 qm3;
        private QM4 qm4;

        public With(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4) {
            this.qm1 = qm1;
            this.qm2 = qm2;
            this.qm3 = qm3;
            this.qm4 = qm4;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minComponentsCount, ref int[] minEntities) where WorldType : struct, IWorldType {
            qm1.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm2.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm3.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm4.SetData<WorldType>(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return qm1.CheckEntity(entityId) && qm2.CheckEntity(entityId) && qm3.CheckEntity(entityId) && qm4.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            qm1.Dispose<WorldType>();
            qm2.Dispose<WorldType>();
            qm3.Dispose<WorldType>();
            qm4.Dispose<WorldType>();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1, QM2, QM3, QM4, QM5> : IPrimaryQueryMethod
        where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
        where QM2 : struct, ISealedQueryMethod
        where QM3 : struct, ISealedQueryMethod
        where QM4 : struct, ISealedQueryMethod
        where QM5 : struct, ISealedQueryMethod {
        private QM1 qm1;
        private QM2 qm2;
        private QM3 qm3;
        private QM4 qm4;
        private QM5 qm5;

        public With(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5) {
            this.qm1 = qm1;
            this.qm2 = qm2;
            this.qm3 = qm3;
            this.qm4 = qm4;
            this.qm5 = qm5;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minComponentsCount, ref int[] minEntities) where WorldType : struct, IWorldType {
            qm1.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm2.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm3.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm4.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm5.SetData<WorldType>(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return qm1.CheckEntity(entityId) && qm2.CheckEntity(entityId) && qm3.CheckEntity(entityId) && qm4.CheckEntity(entityId) && qm5.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            qm1.Dispose<WorldType>();
            qm2.Dispose<WorldType>();
            qm3.Dispose<WorldType>();
            qm4.Dispose<WorldType>();
            qm5.Dispose<WorldType>();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1, QM2, QM3, QM4, QM5, QM6> : IPrimaryQueryMethod
        where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
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

        public With(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5, QM6 qm6) {
            this.qm1 = qm1;
            this.qm2 = qm2;
            this.qm3 = qm3;
            this.qm4 = qm4;
            this.qm5 = qm5;
            this.qm6 = qm6;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minComponentsCount, ref int[] minEntities) where WorldType : struct, IWorldType {
            qm1.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm2.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm3.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm4.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm5.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm6.SetData<WorldType>(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return qm1.CheckEntity(entityId) && qm2.CheckEntity(entityId) && qm3.CheckEntity(entityId) && qm4.CheckEntity(entityId) && qm5.CheckEntity(entityId) && qm6.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            qm1.Dispose<WorldType>();
            qm2.Dispose<WorldType>();
            qm3.Dispose<WorldType>();
            qm4.Dispose<WorldType>();
            qm5.Dispose<WorldType>();
            qm6.Dispose<WorldType>();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1, QM2, QM3, QM4, QM5, QM6, QM7> : IPrimaryQueryMethod
        where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
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

        public With(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5, QM6 qm6, QM7 qm7) {
            this.qm1 = qm1;
            this.qm2 = qm2;
            this.qm3 = qm3;
            this.qm4 = qm4;
            this.qm5 = qm5;
            this.qm6 = qm6;
            this.qm7 = qm7;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minComponentsCount, ref int[] minEntities) where WorldType : struct, IWorldType {
            qm1.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm2.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm3.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm4.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm5.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm6.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm7.SetData<WorldType>(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return qm1.CheckEntity(entityId) && qm2.CheckEntity(entityId) && qm3.CheckEntity(entityId) && qm4.CheckEntity(entityId) && qm5.CheckEntity(entityId) && qm6.CheckEntity(entityId) && qm7.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            qm1.Dispose<WorldType>();
            qm2.Dispose<WorldType>();
            qm3.Dispose<WorldType>();
            qm4.Dispose<WorldType>();
            qm5.Dispose<WorldType>();
            qm6.Dispose<WorldType>();
            qm7.Dispose<WorldType>();
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8> : IPrimaryQueryMethod
        where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
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

        public With(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5, QM6 qm6, QM7 qm7, QM8 qm8) {
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
        public void SetData<WorldType>(ref int minComponentsCount, ref int[] minEntities) where WorldType : struct, IWorldType {
            qm1.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm2.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm3.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm4.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm5.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm6.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm7.SetData<WorldType>(ref minComponentsCount, ref minEntities);
            qm8.SetData<WorldType>(ref minComponentsCount, ref minEntities);
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(int entityId) {
            return qm1.CheckEntity(entityId) && qm2.CheckEntity(entityId) && qm3.CheckEntity(entityId) && qm4.CheckEntity(entityId) && qm5.CheckEntity(entityId) && qm6.CheckEntity(entityId) && qm7.CheckEntity(entityId) && qm8.CheckEntity(entityId);
        }

        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            qm1.Dispose<WorldType>();
            qm2.Dispose<WorldType>();
            qm3.Dispose<WorldType>();
            qm4.Dispose<WorldType>();
            qm5.Dispose<WorldType>();
            qm6.Dispose<WorldType>();
            qm7.Dispose<WorldType>();
            qm8.Dispose<WorldType>();
        }
    }
}