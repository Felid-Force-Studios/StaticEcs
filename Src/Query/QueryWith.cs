#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

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
        public static With<QM1> Create<QM1>(QM1 qm1) where QM1 : struct, IQueryMethod {
            return new With<QM1>(qm1);
        }

        [MethodImpl(AggressiveInlining)]
        public static With<QM1, QM2> Create<QM1, QM2>(QM1 qm1, QM2 qm2)
            where QM1 : struct, IQueryMethod
            where QM2 : struct, IQueryMethod {
            return new With<QM1, QM2>(qm1, qm2);
        }

        [MethodImpl(AggressiveInlining)]
        public static With<QM1, QM2, QM3> Create<QM1, QM2, QM3>(QM1 qm1, QM2 qm2, QM3 qm3)
            where QM1 : struct, IQueryMethod
            where QM2 : struct, IQueryMethod
            where QM3 : struct, IQueryMethod {
            return new With<QM1, QM2, QM3>(qm1, qm2, qm3);
        }

        [MethodImpl(AggressiveInlining)]
        public static With<QM1, QM2, QM3, QM4> Create<QM1, QM2, QM3, QM4>(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4)
            where QM1 : struct, IQueryMethod
            where QM2 : struct, IQueryMethod
            where QM3 : struct, IQueryMethod
            where QM4 : struct, IQueryMethod {
            return new With<QM1, QM2, QM3, QM4>(qm1, qm2, qm3, qm4);
        }

        [MethodImpl(AggressiveInlining)]
        public static With<QM1, QM2, QM3, QM4, QM5> Create<QM1, QM2, QM3, QM4, QM5>(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5)
            where QM1 : struct, IQueryMethod
            where QM2 : struct, IQueryMethod
            where QM3 : struct, IQueryMethod
            where QM4 : struct, IQueryMethod
            where QM5 : struct, IQueryMethod {
            return new With<QM1, QM2, QM3, QM4, QM5>(qm1, qm2, qm3, qm4, qm5);
        }

        [MethodImpl(AggressiveInlining)]
        public static With<QM1, QM2, QM3, QM4, QM5, QM6> Create<QM1, QM2, QM3, QM4, QM5, QM6>(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5, QM6 qm6)
            where QM1 : struct, IQueryMethod
            where QM2 : struct, IQueryMethod
            where QM3 : struct, IQueryMethod
            where QM4 : struct, IQueryMethod
            where QM5 : struct, IQueryMethod
            where QM6 : struct, IQueryMethod {
            return new With<QM1, QM2, QM3, QM4, QM5, QM6>(qm1, qm2, qm3, qm4, qm5, qm6);
        }

        [MethodImpl(AggressiveInlining)]
        public static With<QM1, QM2, QM3, QM4, QM5, QM6, QM7> Create<QM1, QM2, QM3, QM4, QM5, QM6, QM7>(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5, QM6 qm6, QM7 qm7)
            where QM1 : struct, IQueryMethod
            where QM2 : struct, IQueryMethod
            where QM3 : struct, IQueryMethod
            where QM4 : struct, IQueryMethod
            where QM5 : struct, IQueryMethod
            where QM6 : struct, IQueryMethod
            where QM7 : struct, IQueryMethod {
            return new With<QM1, QM2, QM3, QM4, QM5, QM6, QM7>(qm1, qm2, qm3, qm4, qm5, qm6, qm7);
        }

        [MethodImpl(AggressiveInlining)]
        public static With<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8> Create<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8>(QM1 qm1, QM2 qm2, QM3 qm3, QM4 qm4, QM5 qm5, QM6 qm6, QM7 qm7, QM8 qm8)
            where QM1 : struct, IQueryMethod
            where QM2 : struct, IQueryMethod
            where QM3 : struct, IQueryMethod
            where QM4 : struct, IQueryMethod
            where QM5 : struct, IQueryMethod
            where QM6 : struct, IQueryMethod
            where QM7 : struct, IQueryMethod
            where QM8 : struct, IQueryMethod {
            return new With<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8>(qm1, qm2, qm3, qm4, qm5, qm6, qm7, qm8);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct WithNothing : IQueryMethod {

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType { }

        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType { }

        #if FFS_ECS_DEBUG
        public void Assert<WorldType>() where WorldType : struct, IWorldType { }
        
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType { }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1> : IQueryMethod
        where QM1 : struct, IQueryMethod {
        private QM1 qm1;

        public With(QM1 qm1) {
            this.qm1 = qm1;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            qm1.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            qm1.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            qm1.IncQ<WorldType>(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            qm1.DecQ<WorldType>();
        }
        
        #if FFS_ECS_DEBUG
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            qm1.Assert<WorldType>();
        }
        
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            qm1.BlockQ<WorldType>(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1, QM2> : IQueryMethod
        where QM1 : struct, IQueryMethod
        where QM2 : struct, IQueryMethod {
        private QM1 qm1;
        private QM2 qm2;

        public With(QM1 qm1, QM2 qm2) {
            this.qm1 = qm1;
            this.qm2 = qm2;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            qm1.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm2.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            qm1.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm2.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            qm1.IncQ<WorldType>(data);
            qm2.IncQ<WorldType>(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            qm1.DecQ<WorldType>();
            qm2.DecQ<WorldType>();
        }
        
        #if FFS_ECS_DEBUG
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            qm1.Assert<WorldType>();
            qm2.Assert<WorldType>();
        }
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            qm1.BlockQ<WorldType>(val);
            qm2.BlockQ<WorldType>(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1, QM2, QM3> : IQueryMethod
        where QM1 : struct, IQueryMethod
        where QM2 : struct, IQueryMethod
        where QM3 : struct, IQueryMethod {
        private QM1 qm1;
        private QM2 qm2;
        private QM3 qm3;

        public With(QM1 qm1, QM2 qm2, QM3 qm3) {
            this.qm1 = qm1;
            this.qm2 = qm2;
            this.qm3 = qm3;
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            qm1.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm2.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm3.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            qm1.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm2.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm3.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            qm1.IncQ<WorldType>(data);
            qm2.IncQ<WorldType>(data);
            qm3.IncQ<WorldType>(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            qm1.DecQ<WorldType>();
            qm2.DecQ<WorldType>();
            qm3.DecQ<WorldType>();
        }
        
        #if FFS_ECS_DEBUG
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            qm1.Assert<WorldType>();
            qm2.Assert<WorldType>();
            qm3.Assert<WorldType>();
        }
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            qm1.BlockQ<WorldType>(val);
            qm2.BlockQ<WorldType>(val);
            qm3.BlockQ<WorldType>(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1, QM2, QM3, QM4> : IQueryMethod
        where QM1 : struct, IQueryMethod
        where QM2 : struct, IQueryMethod
        where QM3 : struct, IQueryMethod
        where QM4 : struct, IQueryMethod {
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
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            qm1.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm2.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm3.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm4.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            qm1.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm2.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm3.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm4.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            qm1.IncQ<WorldType>(data);
            qm2.IncQ<WorldType>(data);
            qm3.IncQ<WorldType>(data);
            qm4.IncQ<WorldType>(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            qm1.DecQ<WorldType>();
            qm2.DecQ<WorldType>();
            qm3.DecQ<WorldType>();
            qm4.DecQ<WorldType>();
        }
        
        #if FFS_ECS_DEBUG
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            qm1.Assert<WorldType>();
            qm2.Assert<WorldType>();
            qm3.Assert<WorldType>();
            qm4.Assert<WorldType>();
        }
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            qm1.BlockQ<WorldType>(val);
            qm2.BlockQ<WorldType>(val);
            qm3.BlockQ<WorldType>(val);
            qm4.BlockQ<WorldType>(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1, QM2, QM3, QM4, QM5> : IQueryMethod
        where QM1 : struct, IQueryMethod
        where QM2 : struct, IQueryMethod
        where QM3 : struct, IQueryMethod
        where QM4 : struct, IQueryMethod
        where QM5 : struct, IQueryMethod {
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
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            qm1.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm2.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm3.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm4.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm5.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            qm1.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm2.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm3.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm4.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm5.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            qm1.IncQ<WorldType>(data);
            qm2.IncQ<WorldType>(data);
            qm3.IncQ<WorldType>(data);
            qm4.IncQ<WorldType>(data);
            qm5.IncQ<WorldType>(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            qm1.DecQ<WorldType>();
            qm2.DecQ<WorldType>();
            qm3.DecQ<WorldType>();
            qm4.DecQ<WorldType>();
            qm5.DecQ<WorldType>();
        }
        
        #if FFS_ECS_DEBUG
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            qm1.Assert<WorldType>();
            qm2.Assert<WorldType>();
            qm3.Assert<WorldType>();
            qm4.Assert<WorldType>();
            qm5.Assert<WorldType>();
        }
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            qm1.BlockQ<WorldType>(val);
            qm2.BlockQ<WorldType>(val);
            qm3.BlockQ<WorldType>(val);
            qm4.BlockQ<WorldType>(val);
            qm5.BlockQ<WorldType>(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1, QM2, QM3, QM4, QM5, QM6> : IQueryMethod
        where QM1 : struct, IQueryMethod
        where QM2 : struct, IQueryMethod
        where QM3 : struct, IQueryMethod
        where QM4 : struct, IQueryMethod
        where QM5 : struct, IQueryMethod
        where QM6 : struct, IQueryMethod {
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
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            qm1.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm2.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm3.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm4.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm5.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm6.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            qm1.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm2.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm3.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm4.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm5.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm6.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            qm1.IncQ<WorldType>(data);
            qm2.IncQ<WorldType>(data);
            qm3.IncQ<WorldType>(data);
            qm4.IncQ<WorldType>(data);
            qm5.IncQ<WorldType>(data);
            qm6.IncQ<WorldType>(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            qm1.DecQ<WorldType>();
            qm2.DecQ<WorldType>();
            qm3.DecQ<WorldType>();
            qm4.DecQ<WorldType>();
            qm5.DecQ<WorldType>();
            qm6.DecQ<WorldType>();
        }
        
        #if FFS_ECS_DEBUG
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            qm1.Assert<WorldType>();
            qm2.Assert<WorldType>();
            qm3.Assert<WorldType>();
            qm4.Assert<WorldType>();
            qm5.Assert<WorldType>();
            qm6.Assert<WorldType>();
        }
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            qm1.BlockQ<WorldType>(val);
            qm2.BlockQ<WorldType>(val);
            qm3.BlockQ<WorldType>(val);
            qm4.BlockQ<WorldType>(val);
            qm5.BlockQ<WorldType>(val);
            qm6.BlockQ<WorldType>(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1, QM2, QM3, QM4, QM5, QM6, QM7> : IQueryMethod
        where QM1 : struct, IQueryMethod
        where QM2 : struct, IQueryMethod
        where QM3 : struct, IQueryMethod
        where QM4 : struct, IQueryMethod
        where QM5 : struct, IQueryMethod
        where QM6 : struct, IQueryMethod
        where QM7 : struct, IQueryMethod {
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
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            qm1.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm2.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm3.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm4.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm5.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm6.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm7.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            qm1.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm2.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm3.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm4.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm5.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm6.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm7.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            qm1.IncQ<WorldType>(data);
            qm2.IncQ<WorldType>(data);
            qm3.IncQ<WorldType>(data);
            qm4.IncQ<WorldType>(data);
            qm5.IncQ<WorldType>(data);
            qm6.IncQ<WorldType>(data);
            qm7.IncQ<WorldType>(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            qm1.DecQ<WorldType>();
            qm2.DecQ<WorldType>();
            qm3.DecQ<WorldType>();
            qm4.DecQ<WorldType>();
            qm5.DecQ<WorldType>();
            qm6.DecQ<WorldType>();
            qm7.DecQ<WorldType>();
        }
        
        #if FFS_ECS_DEBUG
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            qm1.Assert<WorldType>();
            qm2.Assert<WorldType>();
            qm3.Assert<WorldType>();
            qm4.Assert<WorldType>();
            qm5.Assert<WorldType>();
            qm6.Assert<WorldType>();
            qm7.Assert<WorldType>();
        }
        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            qm1.BlockQ<WorldType>(val);
            qm2.BlockQ<WorldType>(val);
            qm3.BlockQ<WorldType>(val);
            qm4.BlockQ<WorldType>(val);
            qm5.BlockQ<WorldType>(val);
            qm6.BlockQ<WorldType>(val);
            qm7.BlockQ<WorldType>(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct With<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8> : IQueryMethod
        where QM1 : struct, IQueryMethod
        where QM2 : struct, IQueryMethod
        where QM3 : struct, IQueryMethod
        where QM4 : struct, IQueryMethod
        where QM5 : struct, IQueryMethod
        where QM6 : struct, IQueryMethod
        where QM7 : struct, IQueryMethod
        where QM8 : struct, IQueryMethod {
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
        public void CheckChunk<WorldType>(ref ulong chunkMask, uint chunkIdx) where WorldType : struct, IWorldType {
            qm1.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm2.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm3.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm4.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm5.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm6.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm7.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
            qm8.CheckChunk<WorldType>(ref chunkMask, chunkIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void CheckEntities<WorldType>(ref ulong entitiesMask, uint chunkIdx, int blockIdx) where WorldType : struct, IWorldType {
            qm1.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm2.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm3.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm4.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm5.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm6.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm7.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
            qm8.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, blockIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void IncQ<WorldType>(QueryData data) where WorldType : struct, IWorldType {
            qm1.IncQ<WorldType>(data);
            qm2.IncQ<WorldType>(data);
            qm3.IncQ<WorldType>(data);
            qm4.IncQ<WorldType>(data);
            qm5.IncQ<WorldType>(data);
            qm6.IncQ<WorldType>(data);
            qm7.IncQ<WorldType>(data);
            qm8.IncQ<WorldType>(data);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DecQ<WorldType>() where WorldType : struct, IWorldType {
            qm1.DecQ<WorldType>();
            qm2.DecQ<WorldType>();
            qm3.DecQ<WorldType>();
            qm4.DecQ<WorldType>();
            qm5.DecQ<WorldType>();
            qm6.DecQ<WorldType>();
            qm7.DecQ<WorldType>();
            qm8.DecQ<WorldType>();
        }
        
        #if FFS_ECS_DEBUG
        public void Assert<WorldType>() where WorldType : struct, IWorldType {
            qm1.Assert<WorldType>();
            qm2.Assert<WorldType>();
            qm3.Assert<WorldType>();
            qm4.Assert<WorldType>();
            qm5.Assert<WorldType>();
            qm6.Assert<WorldType>();
            qm7.Assert<WorldType>();
            qm8.Assert<WorldType>();
        }

        public void BlockQ<WorldType>(int val) where WorldType : struct, IWorldType {
            qm1.BlockQ<WorldType>(val);
            qm2.BlockQ<WorldType>(val);
            qm3.BlockQ<WorldType>(val);
            qm4.BlockQ<WorldType>(val);
            qm5.BlockQ<WorldType>(val);
            qm6.BlockQ<WorldType>(val);
            qm7.BlockQ<WorldType>(val);
            qm8.BlockQ<WorldType>(val);
        }
        #endif
    }
}