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
    public abstract partial class Ecs<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public abstract partial class World {
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            public abstract partial class QueryEntities {

                [MethodImpl(AggressiveInlining)]
                public static QueryEntitiesIterator<WorldType, QM1> For<QM1>(QM1 qm1 = default)
                    where QM1 : struct, IQueryMethod, IPrimaryQueryMethod {
                    return new QueryEntitiesIterator<WorldType, QM1>(qm1);
                }

                [MethodImpl(AggressiveInlining)]
                public static QueryEntitiesIterator<WorldType, With<QM1, QM2>> For<QM1, QM2>(QM1 qm1 = default, QM2 qm2 = default)
                    where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
                    where QM2 : struct, ISealedQueryMethod {
                    return new QueryEntitiesIterator<WorldType, With<QM1, QM2>>(new With<QM1, QM2>(qm1, qm2));
                }

                [MethodImpl(AggressiveInlining)]
                public static QueryEntitiesIterator<WorldType, With<QM1, QM2, QM3>> For<QM1, QM2, QM3>(
                    QM1 qm1 = default, QM2 qm2 = default, QM3 qm3 = default
                )
                    where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
                    where QM2 : struct, ISealedQueryMethod
                    where QM3 : struct, ISealedQueryMethod {
                    return new QueryEntitiesIterator<WorldType, With<QM1, QM2, QM3>>(new With<QM1, QM2, QM3>(qm1, qm2, qm3));
                }

                [MethodImpl(AggressiveInlining)]
                public static QueryEntitiesIterator<WorldType, With<QM1, QM2, QM3, QM4>> For<QM1, QM2, QM3, QM4>(
                    QM1 qm1 = default, QM2 qm2 = default, QM3 qm3 = default, QM4 qm4 = default
                )
                    where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
                    where QM2 : struct, ISealedQueryMethod
                    where QM3 : struct, ISealedQueryMethod
                    where QM4 : struct, ISealedQueryMethod {
                    return new QueryEntitiesIterator<WorldType, With<QM1, QM2, QM3, QM4>>(new With<QM1, QM2, QM3, QM4>(qm1, qm2, qm3, qm4));
                }

                [MethodImpl(AggressiveInlining)]
                public static QueryEntitiesIterator<WorldType, With<QM1, QM2, QM3, QM4, QM5>> For<QM1, QM2, QM3, QM4, QM5>(
                    QM1 qm1 = default, QM2 qm2 = default, QM3 qm3 = default, QM4 qm4 = default, QM5 qm5 = default
                )
                    where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
                    where QM2 : struct, ISealedQueryMethod
                    where QM3 : struct, ISealedQueryMethod
                    where QM4 : struct, ISealedQueryMethod
                    where QM5 : struct, ISealedQueryMethod {
                    return new QueryEntitiesIterator<WorldType, With<QM1, QM2, QM3, QM4, QM5>>(new With<QM1, QM2, QM3, QM4, QM5>(qm1, qm2, qm3, qm4, qm5));
                }

                [MethodImpl(AggressiveInlining)]
                public static QueryEntitiesIterator<WorldType, With<QM1, QM2, QM3, QM4, QM5, QM6>> For<QM1, QM2, QM3, QM4, QM5, QM6>(
                    QM1 qm1 = default, QM2 qm2 = default, QM3 qm3 = default, QM4 qm4 = default, QM5 qm5 = default, QM6 qm6 = default
                )
                    where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
                    where QM2 : struct, ISealedQueryMethod
                    where QM3 : struct, ISealedQueryMethod
                    where QM4 : struct, ISealedQueryMethod
                    where QM5 : struct, ISealedQueryMethod
                    where QM6 : struct, ISealedQueryMethod {
                    return new QueryEntitiesIterator<WorldType, With<QM1, QM2, QM3, QM4, QM5, QM6>>(
                        new With<QM1, QM2, QM3, QM4, QM5, QM6>(qm1, qm2, qm3, qm4, qm5, qm6));
                }

                [MethodImpl(AggressiveInlining)]
                public static QueryEntitiesIterator<WorldType, With<QM1, QM2, QM3, QM4, QM5, QM6, QM7>> For<QM1, QM2, QM3, QM4, QM5, QM6, QM7>(
                    QM1 qm1 = default, QM2 qm2 = default, QM3 qm3 = default, QM4 qm4 = default, QM5 qm5 = default, QM6 qm6 = default, QM7 qm7 = default
                )
                    where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
                    where QM2 : struct, ISealedQueryMethod
                    where QM3 : struct, ISealedQueryMethod
                    where QM4 : struct, ISealedQueryMethod
                    where QM5 : struct, ISealedQueryMethod
                    where QM6 : struct, ISealedQueryMethod
                    where QM7 : struct, ISealedQueryMethod {
                    return new QueryEntitiesIterator<WorldType, With<QM1, QM2, QM3, QM4, QM5, QM6, QM7>>(
                        new With<QM1, QM2, QM3, QM4, QM5, QM6, QM7>(qm1, qm2, qm3, qm4, qm5, qm6, qm7));
                }

                [MethodImpl(AggressiveInlining)]
                public static QueryEntitiesIterator<WorldType, With<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8>> For<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8>(
                        QM1 qm1 = default, QM2 qm2 = default, QM3 qm3 = default, QM4 qm4 = default, QM5 qm5 = default, QM6 qm6 = default, QM7 qm7 = default, QM8 qm8 = default
                    )
                    where QM1 : struct, ISealedQueryMethod, IPrimaryQueryMethod
                    where QM2 : struct, ISealedQueryMethod
                    where QM3 : struct, ISealedQueryMethod
                    where QM4 : struct, ISealedQueryMethod
                    where QM5 : struct, ISealedQueryMethod
                    where QM6 : struct, ISealedQueryMethod
                    where QM7 : struct, ISealedQueryMethod
                    where QM8 : struct, ISealedQueryMethod {
                    return new QueryEntitiesIterator<WorldType, With<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8>>(
                        new With<QM1, QM2, QM3, QM4, QM5, QM6, QM7, QM8>(qm1, qm2, qm3, qm4, qm5, qm6, qm7, qm8));
                }
            }
        }
    }
}