#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public abstract partial class World<TWorld> {
        #region ADAPTERS
        internal interface IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {

            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5);

            public void InvokeBlock(Entity entity,
                                    #if NET6_0_OR_GREATER
                                    int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5);
                                    #else
                                    uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5);
                                    #endif
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionStructAdapter<T0, T1, T2, T3, T4, T5, TFunction> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where TFunction : struct, IQuery.Write<T0, T1, T2, T3, T4, T5> {
            public TFunction Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function.Invoke(entity, ref comp0, ref comp1, ref comp2, ref comp3, ref comp4, ref comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(entity, ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], ref comp4[start], ref comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionAdapter<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunction<T0, T1, T2, T3, T4, T5> Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref comp0, ref comp1, ref comp2, ref comp3, ref comp4, ref comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset++);
                    #endif
                    Function.Invoke(ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], ref comp4[start], ref comp5[start]);
                    start++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithEntityAdapter<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithEntity<TWorld, T0, T1, T2, T3, T4, T5> Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(entity, ref comp0, ref comp1, ref comp2, ref comp3, ref comp4, ref comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(entity, ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], ref comp4[start], ref comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithDataEntityAdapter<TData, T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithRefDataEntity<TData, TWorld, T0, T1, T2, T3, T4, T5> Function;
            public TData UserData;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref UserData, entity, ref comp0, ref comp1, ref comp2, ref comp3, ref comp4, ref comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(ref UserData, entity, ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], ref comp4[start], ref comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithDataAdapter<TData, T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithRefData<TData, T0, T1, T2, T3, T4, T5> Function;
            public TData UserData;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref UserData, ref comp0, ref comp1, ref comp2, ref comp3, ref comp4, ref comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset++);
                    #endif
                    Function.Invoke(ref UserData, ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], ref comp4[start], ref comp5[start]);
                    start++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryUnsafeFunctionAdapter<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public unsafe delegate*<ref T0, ref T1, ref T2, ref T3, ref T4, ref T5, void> Function;

            [MethodImpl(AggressiveInlining)]
            public unsafe void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref comp0, ref comp1, ref comp2, ref comp3, ref comp4, ref comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public unsafe void InvokeBlock(Entity entity,
                                           #if NET6_0_OR_GREATER
                                           int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                                           #else
                                           uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                                           #endif
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset++);
                    #endif
                    Function(ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], ref comp4[start], ref comp5[start]);
                    start++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryUnsafeFunctionWithEntityAdapter<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public unsafe delegate*<Entity, ref T0, ref T1, ref T2, ref T3, ref T4, ref T5, void> Function;

            [MethodImpl(AggressiveInlining)]
            public unsafe void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(entity, ref comp0, ref comp1, ref comp2, ref comp3, ref comp4, ref comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public unsafe void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function(entity, ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], ref comp4[start], ref comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if !FFS_ECS_DISABLE_CHANGED_TRACKING
        // ---- Write5Read1 — T5 is in ----
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionStructAdapterW5R1<T0, T1, T2, T3, T4, T5, TFunction> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where TFunction : struct, IQuery.Write<T0, T1, T2, T3, T4>.Read<T5> {
            public TFunction Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function.Invoke(entity, ref comp0, ref comp1, ref comp2, ref comp3, ref comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(entity, ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], ref comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionAdapterWrite5Read1<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionR1<T0, T1, T2, T3, T4, T5> Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref comp0, ref comp1, ref comp2, ref comp3, ref comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset++);
                    #endif
                    Function.Invoke(ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], ref comp4[start], in comp5[start]);
                    start++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithEntityAdapterWrite5Read1<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithEntityR1<TWorld, T0, T1, T2, T3, T4, T5> Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(entity, ref comp0, ref comp1, ref comp2, ref comp3, ref comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(entity, ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], ref comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithDataEntityAdapterWrite5Read1<TData, T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithRefDataEntityR1<TData, TWorld, T0, T1, T2, T3, T4, T5> Function;
            public TData UserData;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref UserData, entity, ref comp0, ref comp1, ref comp2, ref comp3, ref comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(ref UserData, entity, ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], ref comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithDataAdapterWrite5Read1<TData, T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithRefDataR1<TData, T0, T1, T2, T3, T4, T5> Function;
            public TData UserData;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref UserData, ref comp0, ref comp1, ref comp2, ref comp3, ref comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset++);
                    #endif
                    Function.Invoke(ref UserData, ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], ref comp4[start], in comp5[start]);
                    start++;
                }
            }
        }
        
        // ---- Write4Read2 — T4..T5 are in ----
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionStructAdapterW4R2<T0, T1, T2, T3, T4, T5, TFunction> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where TFunction : struct, IQuery.Write<T0, T1, T2, T3>.Read<T4, T5> {
            public TFunction Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function.Invoke(entity, ref comp0, ref comp1, ref comp2, ref comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(entity, ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], in comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionAdapterWrite4Read2<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionR2<T0, T1, T2, T3, T4, T5> Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref comp0, ref comp1, ref comp2, ref comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset++);
                    #endif
                    Function.Invoke(ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], in comp4[start], in comp5[start]);
                    start++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithEntityAdapterWrite4Read2<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithEntityR2<TWorld, T0, T1, T2, T3, T4, T5> Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(entity, ref comp0, ref comp1, ref comp2, ref comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(entity, ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], in comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithDataEntityAdapterWrite4Read2<TData, T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithRefDataEntityR2<TData, TWorld, T0, T1, T2, T3, T4, T5> Function;
            public TData UserData;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref UserData, entity, ref comp0, ref comp1, ref comp2, ref comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(ref UserData, entity, ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], in comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithDataAdapterWrite4Read2<TData, T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithRefDataR2<TData, T0, T1, T2, T3, T4, T5> Function;
            public TData UserData;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref UserData, ref comp0, ref comp1, ref comp2, ref comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset++);
                    #endif
                    Function.Invoke(ref UserData, ref comp0[start], ref comp1[start], ref comp2[start], ref comp3[start], in comp4[start], in comp5[start]);
                    start++;
                }
            }
        }
        
        // ---- Write3Read3 — T3..T5 are in ----
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionStructAdapterW3R3<T0, T1, T2, T3, T4, T5, TFunction> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where TFunction : struct, IQuery.Write<T0, T1, T2>.Read<T3, T4, T5> {
            public TFunction Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function.Invoke(entity, ref comp0, ref comp1, ref comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(entity, ref comp0[start], ref comp1[start], ref comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionAdapterWrite3Read3<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionR3<T0, T1, T2, T3, T4, T5> Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref comp0, ref comp1, ref comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset++);
                    #endif
                    Function.Invoke(ref comp0[start], ref comp1[start], ref comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithEntityAdapterWrite3Read3<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithEntityR3<TWorld, T0, T1, T2, T3, T4, T5> Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(entity, ref comp0, ref comp1, ref comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(entity, ref comp0[start], ref comp1[start], ref comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithDataEntityAdapterWrite3Read3<TData, T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithRefDataEntityR3<TData, TWorld, T0, T1, T2, T3, T4, T5> Function;
            public TData UserData;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref UserData, entity, ref comp0, ref comp1, ref comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(ref UserData, entity, ref comp0[start], ref comp1[start], ref comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithDataAdapterWrite3Read3<TData, T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithRefDataR3<TData, T0, T1, T2, T3, T4, T5> Function;
            public TData UserData;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref UserData, ref comp0, ref comp1, ref comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset++);
                    #endif
                    Function.Invoke(ref UserData, ref comp0[start], ref comp1[start], ref comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                }
            }
        }
        
        // ---- Write2Read4 — T2..T5 are in ----
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionStructAdapterW2R4<T0, T1, T2, T3, T4, T5, TFunction> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where TFunction : struct, IQuery.Write<T0, T1>.Read<T2, T3, T4, T5> {
            public TFunction Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function.Invoke(entity, ref comp0, ref comp1, in comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(entity, ref comp0[start], ref comp1[start], in comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionAdapterWrite2Read4<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionR4<T0, T1, T2, T3, T4, T5> Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref comp0, ref comp1, in comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset++);
                    #endif
                    Function.Invoke(ref comp0[start], ref comp1[start], in comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithEntityAdapterWrite2Read4<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithEntityR4<TWorld, T0, T1, T2, T3, T4, T5> Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(entity, ref comp0, ref comp1, in comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(entity, ref comp0[start], ref comp1[start], in comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithDataEntityAdapterWrite2Read4<TData, T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithRefDataEntityR4<TData, TWorld, T0, T1, T2, T3, T4, T5> Function;
            public TData UserData;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref UserData, entity, ref comp0, ref comp1, in comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(ref UserData, entity, ref comp0[start], ref comp1[start], in comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithDataAdapterWrite2Read4<TData, T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithRefDataR4<TData, T0, T1, T2, T3, T4, T5> Function;
            public TData UserData;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref UserData, ref comp0, ref comp1, in comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset++);
                    #endif
                    Function.Invoke(ref UserData, ref comp0[start], ref comp1[start], in comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                }
            }
        }
        
        // ---- Write1Read5 — T1..T5 are in ----
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionStructAdapterW1R5<T0, T1, T2, T3, T4, T5, TFunction> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where TFunction : struct, IQuery.Write<T0>.Read<T1, T2, T3, T4, T5> {
            public TFunction Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function.Invoke(entity, ref comp0, in comp1, in comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(entity, ref comp0[start], in comp1[start], in comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionAdapterWrite1Read5<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionR5<T0, T1, T2, T3, T4, T5> Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref comp0, in comp1, in comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset++);
                    #endif
                    Function.Invoke(ref comp0[start], in comp1[start], in comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithEntityAdapterWrite1Read5<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithEntityR5<TWorld, T0, T1, T2, T3, T4, T5> Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(entity, ref comp0, in comp1, in comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(entity, ref comp0[start], in comp1[start], in comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithDataEntityAdapterWrite1Read5<TData, T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithRefDataEntityR5<TData, TWorld, T0, T1, T2, T3, T4, T5> Function;
            public TData UserData;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref UserData, entity, ref comp0, in comp1, in comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(ref UserData, entity, ref comp0[start], in comp1[start], in comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithDataAdapterWrite1Read5<TData, T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithRefDataR5<TData, T0, T1, T2, T3, T4, T5> Function;
            public TData UserData;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref UserData, ref comp0, in comp1, in comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset++);
                    #endif
                    Function.Invoke(ref UserData, ref comp0[start], in comp1[start], in comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                }
            }
        }
        
        // ---- Read — all read ----
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionStructAdapterRead<T0, T1, T2, T3, T4, T5, TFunction> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where TFunction : struct, IQuery.Read<T0, T1, T2, T3, T4, T5> {
            public TFunction Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function.Invoke(entity, in comp0, in comp1, in comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(entity, in comp0[start], in comp1[start], in comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionAdapterRead<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionR6<T0, T1, T2, T3, T4, T5> Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(in comp0, in comp1, in comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset++);
                    #endif
                    Function.Invoke(in comp0[start], in comp1[start], in comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithEntityAdapterRead<T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithEntityR6<TWorld, T0, T1, T2, T3, T4, T5> Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(entity, in comp0, in comp1, in comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(entity, in comp0[start], in comp1[start], in comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithDataEntityAdapterRead<TData, T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithRefDataEntityR6<TData, TWorld, T0, T1, T2, T3, T4, T5> Function;
            public TData UserData;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref UserData, entity, in comp0, in comp1, in comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                ref var entityId = ref entity.IdWithOffset;
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                    #endif
                    Function.Invoke(ref UserData, entity, in comp0[start], in comp1[start], in comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                    entityId++;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct QueryFunctionWithDataAdapterRead<TData, T0, T1, T2, T3, T4, T5> : IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
            public QueryFunctionWithRefDataR6<TData, T0, T1, T2, T3, T4, T5> Function;
            public TData UserData;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(Entity entity, ref T0 comp0, ref T1 comp1, ref T2 comp2, ref T3 comp3, ref T4 comp4, ref T5 comp5) {
                Function(ref UserData, in comp0, in comp1, in comp2, in comp3, in comp4, in comp5);
            }

            [MethodImpl(AggressiveInlining)]
            public void InvokeBlock(
                Entity entity,
                #if NET6_0_OR_GREATER
                int start, int end, Span<T0> comp0, Span<T1> comp1, Span<T2> comp2, Span<T3> comp3, Span<T4> comp4, Span<T5> comp5) {
                #else
                uint start, uint end, T0[] comp0, T1[] comp1, T2[] comp2, T3[] comp3, T4[] comp4, T5[] comp5) {
                #endif
                while (start < end) {
                    #if FFS_ECS_DEBUG
                    Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset++);
                    #endif
                    Function.Invoke(ref UserData, in comp0[start], in comp1[start], in comp2[start], in comp3[start], in comp4[start], in comp5[start]);
                    start++;
                }
            }
        }
        
        #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING

        // ---- Block adapters ----
        internal interface IBlockQueryAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent {

            public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, Block<T3> block3, Block<T4> block4, Block<T5> block5);
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct BlockAdapterWrite<T0, T1, T2, T3, T4, T5, TFunction> : IBlockQueryAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent
                where TFunction : struct, IQueryBlock.Write<T0, T1, T2, T3, T4, T5> {
            public TFunction Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, Block<T3> block3, Block<T4> block4, Block<T5> block5) {
                Function.Invoke(count, entitiesBlock, block0, block1, block2, block3, block4, block5);
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct BlockUnsafeAdapter<T0, T1, T2, T3, T4, T5> : IBlockQueryAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent {
            public unsafe delegate*<uint, EntityBlock, Block<T0>, Block<T1>, Block<T2>, Block<T3>, Block<T4>, Block<T5>, void> Function;

            [MethodImpl(AggressiveInlining)]
            public unsafe void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, Block<T3> block3, Block<T4> block4, Block<T5> block5) {
                Function(count, entitiesBlock, block0, block1, block2, block3, block4, block5);
            }
        }
        
        #if !FFS_ECS_DISABLE_CHANGED_TRACKING
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct BlockAdapterW5R1<T0, T1, T2, T3, T4, T5, TFunction> : IBlockQueryAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent
                where TFunction : struct, IQueryBlock.Write<T0, T1, T2, T3, T4>.Read<T5> {
            public TFunction Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, Block<T3> block3, Block<T4> block4, Block<T5> block5) {
                Function.Invoke(count, entitiesBlock, block0, block1, block2, block3, block4, new BlockR<T5>(block5));
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct BlockAdapterW4R2<T0, T1, T2, T3, T4, T5, TFunction> : IBlockQueryAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent
                where TFunction : struct, IQueryBlock.Write<T0, T1, T2, T3>.Read<T4, T5> {
            public TFunction Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, Block<T3> block3, Block<T4> block4, Block<T5> block5) {
                Function.Invoke(count, entitiesBlock, block0, block1, block2, block3, new BlockR<T4>(block4), new BlockR<T5>(block5));
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct BlockAdapterW3R3<T0, T1, T2, T3, T4, T5, TFunction> : IBlockQueryAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent
                where TFunction : struct, IQueryBlock.Write<T0, T1, T2>.Read<T3, T4, T5> {
            public TFunction Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, Block<T3> block3, Block<T4> block4, Block<T5> block5) {
                Function.Invoke(count, entitiesBlock, block0, block1, block2, new BlockR<T3>(block3), new BlockR<T4>(block4), new BlockR<T5>(block5));
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct BlockAdapterW2R4<T0, T1, T2, T3, T4, T5, TFunction> : IBlockQueryAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent
                where TFunction : struct, IQueryBlock.Write<T0, T1>.Read<T2, T3, T4, T5> {
            public TFunction Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, Block<T3> block3, Block<T4> block4, Block<T5> block5) {
                Function.Invoke(count, entitiesBlock, block0, block1, new BlockR<T2>(block2), new BlockR<T3>(block3), new BlockR<T4>(block4), new BlockR<T5>(block5));
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct BlockAdapterW1R5<T0, T1, T2, T3, T4, T5, TFunction> : IBlockQueryAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent
                where TFunction : struct, IQueryBlock.Write<T0>.Read<T1, T2, T3, T4, T5> {
            public TFunction Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, Block<T3> block3, Block<T4> block4, Block<T5> block5) {
                Function.Invoke(count, entitiesBlock, block0, new BlockR<T1>(block1), new BlockR<T2>(block2), new BlockR<T3>(block3), new BlockR<T4>(block4), new BlockR<T5>(block5));
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct BlockAdapterRead<T0, T1, T2, T3, T4, T5, TFunction> : IBlockQueryAdapter<T0, T1, T2, T3, T4, T5>
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent
                where TFunction : struct, IQueryBlock.Read<T0, T1, T2, T3, T4, T5> {
            public TFunction Function;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(uint count, EntityBlock entitiesBlock, Block<T0> block0, Block<T1> block1, Block<T2> block2, Block<T3> block3, Block<T4> block4, Block<T5> block5) {
                Function.Invoke(count, entitiesBlock, new BlockR<T0>(block0), new BlockR<T1>(block1), new BlockR<T2>(block2), new BlockR<T3>(block3), new BlockR<T4>(block4), new BlockR<T5>(block5));
            }
        }
        
        #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
        #endregion
        /// <summary>Fluent query builder for 6 writable component(s). Call <c>For</c>/<c>ForParallel</c> to execute.</summary>
        public readonly struct WriteQuery<TFilter, T0, T1, T2, T3, T4, T5>
            where T0 : struct, IComponent
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent
            where TFilter : struct, IQueryFilter {
            internal readonly TFilter Filter;
            [MethodImpl(AggressiveInlining)]
            internal WriteQuery(TFilter filter) { Filter = filter; }

            /// <inheritdoc cref="For{TFunction}(ref TFunction, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TFunction>(TFunction function = default,
                                       EntityStatusType entities = EntityStatusType.Enabled,
                                       ComponentStatus components = ComponentStatus.Enabled,
                                       QueryMode queryMode = QueryMode.Strict,
                                       ReadOnlySpan<ushort> clusters = default)
                where TFunction : struct, IQuery.Write<T0, T1, T2, T3, T4, T5> {
                QueryFunctionStructAdapter<T0, T1, T2, T3, T4, T5, TFunction> adapter;
                adapter.Function = function;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                new WorldQuery<TFilter>(Filter).ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionStructAdapter<T0, T1, T2, T3, T4, T5, TFunction>
                    , ChangedTracker<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4, T5>(0),
                    entities, components, queryMode, clusters);
                #else
                new WorldQuery<TFilter>(Filter).ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionStructAdapter<T0, T1, T2, T3, T4, T5, TFunction>
                >(ref adapter,
                    entities, components, queryMode, clusters);
                #endif
            }

            /// <summary>Iterates over matching entities, invoking the struct function for each.</summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TFunction>(ref TFunction function,
                                       EntityStatusType entities = EntityStatusType.Enabled,
                                       ComponentStatus components = ComponentStatus.Enabled,
                                       QueryMode queryMode = QueryMode.Strict,
                                       ReadOnlySpan<ushort> clusters = default)
                where TFunction : struct, IQuery.Write<T0, T1, T2, T3, T4, T5> {
                QueryFunctionStructAdapter<T0, T1, T2, T3, T4, T5, TFunction> adapter;
                adapter.Function = function;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                new WorldQuery<TFilter>(Filter).ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionStructAdapter<T0, T1, T2, T3, T4, T5, TFunction>
                    , ChangedTracker<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4, T5>(0),
                    entities, components, queryMode, clusters);
                #else
                new WorldQuery<TFilter>(Filter).ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionStructAdapter<T0, T1, T2, T3, T4, T5, TFunction>
                >(ref adapter,
                    entities, components, queryMode, clusters);
                #endif
                function = adapter.Function;
            }

            /// <inheritdoc cref="ForParallel{TFunction}(ref TFunction, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TFunction>(TFunction function = default,
                                               EntityStatusType entities = EntityStatusType.Enabled,
                                               ComponentStatus components = ComponentStatus.Enabled,
                                               ReadOnlySpan<ushort> clusters = default,
                                               uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                               uint workersLimit = 0)
                where TFunction : struct, IQuery.Write<T0, T1, T2, T3, T4, T5> {
                ForParallel(ref function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <summary>Parallel iteration over matching entities, distributing work across threads.</summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TFunction>(ref TFunction function,
                                               EntityStatusType entities = EntityStatusType.Enabled,
                                               ComponentStatus components = ComponentStatus.Enabled,
                                               ReadOnlySpan<ushort> clusters = default,
                                               uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                               uint workersLimit = 0)
                where TFunction : struct, IQuery.Write<T0, T1, T2, T3, T4, T5> {
                if (new WorldQuery<TFilter>(Filter).PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionStructAdapter<T0, T1, T2, T3, T4, T5, TFunction>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&WorldQuery<TFilter>.ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionStructAdapter<T0, T1, T2, T3, T4, T5, TFunction>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        function = data.Value.Function;
                        data = default;
                    }
                }
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
        }

        #if !FFS_ECS_DISABLE_CHANGED_TRACKING
        /// <summary>Fluent query builder for 6 readonly component(s). Call <c>For</c>/<c>ForParallel</c> to execute.</summary>
        public readonly struct ReadQuery<TFilter, T0, T1, T2, T3, T4, T5>
            where T0 : struct, IComponent
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent
            where TFilter : struct, IQueryFilter {
            internal readonly TFilter Filter;
            [MethodImpl(AggressiveInlining)]
            internal ReadQuery(TFilter filter) { Filter = filter; }

            /// <inheritdoc cref="For{TFunction}(ref TFunction, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TFunction>(TFunction function = default,
                                       EntityStatusType entities = EntityStatusType.Enabled,
                                       ComponentStatus components = ComponentStatus.Enabled,
                                       QueryMode queryMode = QueryMode.Strict,
                                       ReadOnlySpan<ushort> clusters = default)
                where TFunction : struct, IQuery.Read<T0, T1, T2, T3, T4, T5> {
                QueryFunctionStructAdapterRead<T0, T1, T2, T3, T4, T5, TFunction> adapter;
                adapter.Function = function;
                new WorldQuery<TFilter>(Filter).ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionStructAdapterRead<T0, T1, T2, T3, T4, T5, TFunction>
                >(ref adapter,
                    entities, components, queryMode, clusters);
            }

            /// <summary>Iterates over matching entities, invoking the struct function for each.</summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TFunction>(ref TFunction function,
                                       EntityStatusType entities = EntityStatusType.Enabled,
                                       ComponentStatus components = ComponentStatus.Enabled,
                                       QueryMode queryMode = QueryMode.Strict,
                                       ReadOnlySpan<ushort> clusters = default)
                where TFunction : struct, IQuery.Read<T0, T1, T2, T3, T4, T5> {
                QueryFunctionStructAdapterRead<T0, T1, T2, T3, T4, T5, TFunction> adapter;
                adapter.Function = function;
                new WorldQuery<TFilter>(Filter).ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionStructAdapterRead<T0, T1, T2, T3, T4, T5, TFunction>
                >(ref adapter,
                    entities, components, queryMode, clusters);
                function = adapter.Function;
            }

            /// <inheritdoc cref="ForParallel{TFunction}(ref TFunction, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TFunction>(TFunction function = default,
                                               EntityStatusType entities = EntityStatusType.Enabled,
                                               ComponentStatus components = ComponentStatus.Enabled,
                                               ReadOnlySpan<ushort> clusters = default,
                                               uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                               uint workersLimit = 0)
                where TFunction : struct, IQuery.Read<T0, T1, T2, T3, T4, T5> {
                ForParallel(ref function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <summary>Parallel iteration over matching entities, distributing work across threads.</summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TFunction>(ref TFunction function,
                                               EntityStatusType entities = EntityStatusType.Enabled,
                                               ComponentStatus components = ComponentStatus.Enabled,
                                               ReadOnlySpan<ushort> clusters = default,
                                               uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                               uint workersLimit = 0)
                where TFunction : struct, IQuery.Read<T0, T1, T2, T3, T4, T5> {
                if (new WorldQuery<TFilter>(Filter).PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionStructAdapterRead<T0, T1, T2, T3, T4, T5, TFunction>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&WorldQuery<TFilter>.ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionStructAdapterRead<T0, T1, T2, T3, T4, T5, TFunction>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        function = data.Value.Function;
                        data = default;
                    }
                }
            }

        }

        #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
        /// <summary>Fluent block query builder for 6 writable component(s). Call <c>For</c>/<c>ForParallel</c> to execute.</summary>
        public readonly struct BlockWriteQuery<TFilter, T0, T1, T2, T3, T4, T5>
            where T0 : unmanaged, IComponent
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where TFilter : struct, IQueryFilter {
            internal readonly TFilter Filter;
            [MethodImpl(AggressiveInlining)]
            internal BlockWriteQuery(TFilter filter) { Filter = filter; }

            /// <inheritdoc cref="For{TFunction}(ref TFunction, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TFunction>(TFunction function = default,
                                       EntityStatusType entities = EntityStatusType.Enabled,
                                       ComponentStatus components = ComponentStatus.Enabled,
                                       ReadOnlySpan<ushort> clusters = default)
                where TFunction : struct, IQueryBlock.Write<T0, T1, T2, T3, T4, T5> {
                BlockAdapterWrite<T0, T1, T2, T3, T4, T5, TFunction> adapter;
                adapter.Function = function;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                new WorldQuery<TFilter>(Filter).ForBlockInternalTracked<T0, T1, T2, T3, T4, T5, BlockAdapterWrite<T0, T1, T2, T3, T4, T5, TFunction>
                    , ChangedTracker<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4, T5>(0),
                    entities, components, clusters);
                #else
                new WorldQuery<TFilter>(Filter).ForBlockInternal<T0, T1, T2, T3, T4, T5, BlockAdapterWrite<T0, T1, T2, T3, T4, T5, TFunction>
                >(ref adapter,
                    entities, components, clusters);
                #endif
            }

            /// <summary>Iterates over matching entities, invoking the struct function for each.</summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TFunction>(ref TFunction function,
                                       EntityStatusType entities = EntityStatusType.Enabled,
                                       ComponentStatus components = ComponentStatus.Enabled,
                                       ReadOnlySpan<ushort> clusters = default)
                where TFunction : struct, IQueryBlock.Write<T0, T1, T2, T3, T4, T5> {
                BlockAdapterWrite<T0, T1, T2, T3, T4, T5, TFunction> adapter;
                adapter.Function = function;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                new WorldQuery<TFilter>(Filter).ForBlockInternalTracked<T0, T1, T2, T3, T4, T5, BlockAdapterWrite<T0, T1, T2, T3, T4, T5, TFunction>
                    , ChangedTracker<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4, T5>(0),
                    entities, components, clusters);
                #else
                new WorldQuery<TFilter>(Filter).ForBlockInternal<T0, T1, T2, T3, T4, T5, BlockAdapterWrite<T0, T1, T2, T3, T4, T5, TFunction>
                >(ref adapter,
                    entities, components, clusters);
                #endif
                function = adapter.Function;
            }

            /// <inheritdoc cref="ForParallel{TFunction}(ref TFunction, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TFunction>(TFunction function = default,
                                               EntityStatusType entities = EntityStatusType.Enabled,
                                               ComponentStatus components = ComponentStatus.Enabled,
                                               ReadOnlySpan<ushort> clusters = default,
                                               uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                               uint workersLimit = 0)
                where TFunction : struct, IQueryBlock.Write<T0, T1, T2, T3, T4, T5> {
                ForParallel(ref function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <summary>Parallel iteration over matching entities, distributing work across threads.</summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TFunction>(ref TFunction function,
                                               EntityStatusType entities = EntityStatusType.Enabled,
                                               ComponentStatus components = ComponentStatus.Enabled,
                                               ReadOnlySpan<ushort> clusters = default,
                                               uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                               uint workersLimit = 0)
                where TFunction : struct, IQueryBlock.Write<T0, T1, T2, T3, T4, T5> {
                if (new WorldQuery<TFilter>(Filter).PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<BlockAdapterWrite<T0, T1, T2, T3, T4, T5, TFunction>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&WorldQuery<TFilter>.ForBlockParallelInternal<T0, T1, T2, T3, T4, T5, BlockAdapterWrite<T0, T1, T2, T3, T4, T5, TFunction>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        function = data.Value.Function;
                        data = default;
                    }
                }
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
        }

        #if !FFS_ECS_DISABLE_CHANGED_TRACKING
        /// <summary>Fluent block query builder for 6 readonly component(s). Call <c>For</c>/<c>ForParallel</c> to execute.</summary>
        public readonly struct BlockReadQuery<TFilter, T0, T1, T2, T3, T4, T5>
            where T0 : unmanaged, IComponent
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where TFilter : struct, IQueryFilter {
            internal readonly TFilter Filter;
            [MethodImpl(AggressiveInlining)]
            internal BlockReadQuery(TFilter filter) { Filter = filter; }

            /// <inheritdoc cref="For{TFunction}(ref TFunction, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TFunction>(TFunction function = default,
                                       EntityStatusType entities = EntityStatusType.Enabled,
                                       ComponentStatus components = ComponentStatus.Enabled,
                                       ReadOnlySpan<ushort> clusters = default)
                where TFunction : struct, IQueryBlock.Read<T0, T1, T2, T3, T4, T5> {
                BlockAdapterRead<T0, T1, T2, T3, T4, T5, TFunction> adapter;
                adapter.Function = function;
                new WorldQuery<TFilter>(Filter).ForBlockInternal<T0, T1, T2, T3, T4, T5, BlockAdapterRead<T0, T1, T2, T3, T4, T5, TFunction>
                >(ref adapter,
                    entities, components, clusters);
            }

            /// <summary>Iterates over matching entities, invoking the struct function for each.</summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TFunction>(ref TFunction function,
                                       EntityStatusType entities = EntityStatusType.Enabled,
                                       ComponentStatus components = ComponentStatus.Enabled,
                                       ReadOnlySpan<ushort> clusters = default)
                where TFunction : struct, IQueryBlock.Read<T0, T1, T2, T3, T4, T5> {
                BlockAdapterRead<T0, T1, T2, T3, T4, T5, TFunction> adapter;
                adapter.Function = function;
                new WorldQuery<TFilter>(Filter).ForBlockInternal<T0, T1, T2, T3, T4, T5, BlockAdapterRead<T0, T1, T2, T3, T4, T5, TFunction>
                >(ref adapter,
                    entities, components, clusters);
                function = adapter.Function;
            }

            /// <inheritdoc cref="ForParallel{TFunction}(ref TFunction, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TFunction>(TFunction function = default,
                                               EntityStatusType entities = EntityStatusType.Enabled,
                                               ComponentStatus components = ComponentStatus.Enabled,
                                               ReadOnlySpan<ushort> clusters = default,
                                               uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                               uint workersLimit = 0)
                where TFunction : struct, IQueryBlock.Read<T0, T1, T2, T3, T4, T5> {
                ForParallel(ref function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <summary>Parallel iteration over matching entities, distributing work across threads.</summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TFunction>(ref TFunction function,
                                               EntityStatusType entities = EntityStatusType.Enabled,
                                               ComponentStatus components = ComponentStatus.Enabled,
                                               ReadOnlySpan<ushort> clusters = default,
                                               uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                               uint workersLimit = 0)
                where TFunction : struct, IQueryBlock.Read<T0, T1, T2, T3, T4, T5> {
                if (new WorldQuery<TFilter>(Filter).PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<BlockAdapterRead<T0, T1, T2, T3, T4, T5, TFunction>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&WorldQuery<TFilter>.ForBlockParallelInternal<T0, T1, T2, T3, T4, T5, BlockAdapterRead<T0, T1, T2, T3, T4, T5, TFunction>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        function = data.Value.Function;
                        data = default;
                    }
                }
            }

        }

        #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public readonly ref partial struct WorldQuery<TFilter> where TFilter : struct, IQueryFilter {

            /// <summary>Creates a query builder for 6 writable component(s).</summary>
            [MethodImpl(AggressiveInlining)]
            public WriteQuery<TFilter, T0, T1, T2, T3, T4, T5> Write<T0, T1, T2, T3, T4, T5>()
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                return new WriteQuery<TFilter, T0, T1, T2, T3, T4, T5>(Filter);
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            /// <summary>Creates a query builder for 6 readonly component(s).</summary>
            [MethodImpl(AggressiveInlining)]
            public ReadQuery<TFilter, T0, T1, T2, T3, T4, T5> Read<T0, T1, T2, T3, T4, T5>()
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                return new ReadQuery<TFilter, T0, T1, T2, T3, T4, T5>(Filter);
            }
            #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING

            /// <summary>Creates a block query builder for 6 writable component(s).</summary>
            [MethodImpl(AggressiveInlining)]
            public BlockWriteQuery<TFilter, T0, T1, T2, T3, T4, T5> WriteBlock<T0, T1, T2, T3, T4, T5>()
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent {
                return new BlockWriteQuery<TFilter, T0, T1, T2, T3, T4, T5>(Filter);
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            /// <summary>Creates a block query builder for 6 readonly component(s).</summary>
            [MethodImpl(AggressiveInlining)]
            public BlockReadQuery<TFilter, T0, T1, T2, T3, T4, T5> ReadBlock<T0, T1, T2, T3, T4, T5>()
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent {
                return new BlockReadQuery<TFilter, T0, T1, T2, T3, T4, T5>(Filter);
            }
            #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING

            #region DELEGATE SEARCH
            /// <inheritdoc cref="Search{T0}(out Entity, SearchFunctionWithEntity{TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public bool Search<T0, T1, T2, T3, T4, T5>(out Entity entity,
                                                       SearchFunctionWithEntity<TWorld, T0, T1, T2, T3, T4, T5> function,
                                                       EntityStatusType entities = EntityStatusType.Enabled,
                                                       ComponentStatus components = ComponentStatus.Enabled,
                                                       QueryMode queryMode = QueryMode.Strict,
                                                       ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ref var world = ref Data.Instance;
                var strict = queryMode == QueryMode.Strict;

                var result = false;
                entity = new Entity();
                ref var entityId = ref entity.IdWithOffset;

                if (Prepare<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        var segments0 = Components<T0>.Instance.ComponentSegments;
                        var segments1 = Components<T1>.Instance.ComponentSegments;
                        var segments2 = Components<T2>.Instance.ComponentSegments;
                        var segments3 = Components<T3>.Instance.ComponentSegments;
                        var segments4 = Components<T4>.Instance.ComponentSegments;
                        var segments5 = Components<T5>.Instance.ComponentSegments;

                        #if !NET6_0_OR_GREATER
                        var deBruijn = Utils.DeBruijn;
                        #endif

                        #if NET6_0_OR_GREATER
                        Span<T0> components0 = default;
                        Span<T1> components1 = default;
                        Span<T2> components2 = default;
                        Span<T3> components3 = default;
                        Span<T4> components4 = default;
                        Span<T5> components5 = default;
                        #else
                        T0[] components0 = null;
                        T1[] components1 = null;
                        T2[] components2 = null;
                        T3[] components3 = null;
                        T4[] components4 = null;
                        T5[] components5 = null;
                        #endif

                        var blocks = queryData.Blocks;
                        var segmentIdx = uint.MaxValue;

                        do {
                            var curSegmentIdx = firstGlobalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                            if (curSegmentIdx != segmentIdx) {
                                segmentIdx = (uint)curSegmentIdx;
                                #if NET6_0_OR_GREATER
                                components0 = new(segments0[segmentIdx]);
                                components1 = new(segments1[segmentIdx]);
                                components2 = new(segments2[segmentIdx]);
                                components3 = new(segments3[segmentIdx]);
                                components4 = new(segments4[segmentIdx]);
                                components5 = new(segments5[segmentIdx]);
                                #else
                                components0 = segments0[segmentIdx];
                                components1 = segments1[segmentIdx];
                                components2 = segments2[segmentIdx];
                                components3 = segments3[segmentIdx];
                                components4 = segments4[segmentIdx];
                                components5 = segments5[segmentIdx];
                                #endif
                            }

                            var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT);
                            ref var block = ref blocks[firstGlobalBlockIdx];
                            ref var entitiesMaskRef = ref block.EntitiesMask;
                            firstGlobalBlockIdx = block.NextGlobalBlock;
                            var entitiesMask = entitiesMaskRef;
                            #if NET6_0_OR_GREATER
                            var componentOffset = (int)(chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK);
                            #else
                            var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                            #endif
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            if (strict) {
                                if (entitiesMask == ulong.MaxValue) {
                                    var componentEnd = componentOffset + Const.U64_BITS;
                                    entityId = chunkBlockEntityId;
                                    while (componentOffset < componentEnd) {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        if (function.Invoke(
                                                entity,
                                                in components0[componentOffset],
                                                in components1[componentOffset],
                                                in components2[componentOffset],
                                                in components3[componentOffset],
                                                in components4[componentOffset],
                                                in components5[componentOffset]
                                            )) {
                                            result = true;
                                            goto EXIT;
                                        }

                                        componentOffset++;
                                        entityId++;
                                    }
                                }
                                else {
                                    var runStarts = entitiesMask & ~(entitiesMask << 1);
                                    var runEnds = entitiesMask & ~(entitiesMask >> 1);
                                    do {
                                        #if NET6_0_OR_GREATER
                                        var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(runStarts);
                                        var runEnd = (byte)System.Numerics.BitOperations.TrailingZeroCount(runEnds);
                                        #else
                                        var runStart = deBruijn[(uint)(((runStarts & (ulong)-(long)runStarts) * 0x37E84A99DAE458FUL) >> 58)];
                                        var runEnd = deBruijn[(uint)(((runEnds & (ulong)-(long)runEnds) * 0x37E84A99DAE458FUL) >> 58)];
                                        #endif
                                        runStarts &= runStarts - 1UL;
                                        runEnds &= runEnds - 1UL;
                                        var componentIdx = runStart + componentOffset;
                                        var componentEnd = runEnd + componentOffset;
                                        entityId = chunkBlockEntityId + runStart;
                                        while (componentIdx <= componentEnd) {
                                            #if FFS_ECS_DEBUG
                                            world.SetCurrentQueryEntity(entityId);
                                            #endif
                                            if (function.Invoke(
                                                    entity,
                                                    in components0[componentIdx],
                                                    in components1[componentIdx],
                                                    in components2[componentIdx],
                                                    in components3[componentIdx],
                                                    in components4[componentIdx],
                                                    in components5[componentIdx]
                                                )) {
                                                result = true;
                                                goto EXIT;
                                            }

                                            componentIdx++;
                                            entityId++;
                                        }
                                    } while (runStarts != 0);
                                }
                            }
                            else {
                                do {
                                    var isolatedBit = entitiesMask & (ulong)-(long)entitiesMask;
                                    #if NET6_0_OR_GREATER
                                    var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                                    #else
                                    var runStart = deBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                                    #endif

                                    var componentIdx = runStart + componentOffset;
                                    entityId = chunkBlockEntityId + runStart;

                                    do {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        if (function.Invoke(
                                                entity,
                                                in components0[componentIdx],
                                                in components1[componentIdx],
                                                in components2[componentIdx],
                                                in components3[componentIdx],
                                                in components4[componentIdx],
                                                in components5[componentIdx]
                                            )) {
                                            result = true;
                                            goto EXIT;
                                        }

                                        isolatedBit <<= 1;
                                        componentIdx++;
                                        entityId++;
                                    } while ((entitiesMaskRef & isolatedBit) != 0);

                                    entitiesMask = entitiesMaskRef & ~(isolatedBit - 1);
                                } while (entitiesMask != 0);
                            }
                        } while (firstGlobalBlockIdx >= 0);

                        EXIT: ;
                    }

                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        Dispose<T0, T1, T2, T3, T4, T5>(Filter, entities, components, strict, queryData);
                    }
                }

                return result;
            }
            #endregion

            #region DELEGATE WITH DATA AND ENTITY
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                           QueryFunctionWithRefDataEntity<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataEntityAdapter<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapter<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4, T5>(0),
                    entities, components, queryMode, clusters);
                #else
                ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapter<TData, T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    entities, components, queryMode, clusters);
                #endif
            }

            /// <inheritdoc cref="For{TData,T0}(TData, QueryFunctionWithRefDataEntity{TData,TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                           QueryFunctionWithRefDataEntity<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataEntityAdapter<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapter<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4, T5>(0),
                    entities, components, queryMode, clusters);
                #else
                ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapter<TData, T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    entities, components, queryMode, clusters);
                #endif
                userData = adapter.UserData;
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                                   QueryFunctionWithRefDataEntity<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ForParallel(ref userData, function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <inheritdoc cref="ForParallel{TData,T0, T1, T2, T3, T4, T5}(ref TData, QueryFunctionWithRefDataEntity{TData,TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                                   QueryFunctionWithRefDataEntity<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithDataEntityAdapter<TData, T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    data.Value.UserData = userData;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapter<TData, T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        userData = data.Value.UserData;
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH ENTITY
            /// <inheritdoc cref="Search{T0, T1, T2, T3, T4, T5}(out Entity, SearchFunctionWithEntity{TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<T0, T1, T2, T3, T4, T5>(QueryFunctionWithEntity<TWorld, T0, T1, T2, T3, T4, T5> function,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ComponentStatus components = ComponentStatus.Enabled,
                                                    QueryMode queryMode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithEntityAdapter<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithEntityAdapter<T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4, T5>(0),
                    entities, components, queryMode, clusters);
                #else
                ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithEntityAdapter<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    entities, components, queryMode, clusters);
                #endif
            }
            
            /// <inheritdoc cref="ForParallel{T0, T1, T2, T3, T4, T5}(QueryFunctionWithEntity{TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<T0, T1, T2, T3, T4, T5>(QueryFunctionWithEntity<TWorld, T0, T1, T2, T3, T4, T5> function,
                                                            EntityStatusType entities = EntityStatusType.Enabled,
                                                            ComponentStatus components = ComponentStatus.Enabled,
                                                            ReadOnlySpan<ushort> clusters = default,
                                                            uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                            uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithEntityAdapter<T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithEntityAdapter<T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH DATA
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                           QueryFunctionWithRefData<TData, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataAdapter<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapter<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4, T5>(0),
                    entities, components, queryMode, clusters);
                #else
                ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapter<TData, T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    entities, components, queryMode, clusters);
                #endif
            }

            /// <inheritdoc cref="For{TData,T0}(TData, QueryFunctionWithRefData{TData,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                           QueryFunctionWithRefData<TData, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataAdapter<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapter<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4, T5>(0),
                    entities, components, queryMode, clusters);
                #else
                ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapter<TData, T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    entities, components, queryMode, clusters);
                #endif
                userData = adapter.UserData;
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                                   QueryFunctionWithRefData<TData, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ForParallel(ref userData, function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <inheritdoc cref="ForParallel{TData,T0, T1, T2, T3, T4, T5}(ref TData, QueryFunctionWithRefData{TData,TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                                   QueryFunctionWithRefData<TData, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithDataAdapter<TData, T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    data.Value.UserData = userData;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapter<TData, T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        userData = data.Value.UserData;
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE
            /// <inheritdoc cref="For{T0}(QueryFunctionWithEntity{TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<T0, T1, T2, T3, T4, T5>(QueryFunction<T0, T1, T2, T3, T4, T5> function,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ComponentStatus components = ComponentStatus.Enabled,
                                                    QueryMode queryMode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionAdapter<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4, T5>(0),
                    entities, components, queryMode, clusters);
                #else
                ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    entities, components, queryMode, clusters);
                #endif
            }
            
            /// <inheritdoc cref="ForParallel{T0, T1, T2, T3, T4, T5}(QueryFunction{T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<T0, T1, T2, T3, T4, T5>(QueryFunction<T0, T1, T2, T3, T4, T5> function,
                                                            EntityStatusType entities = EntityStatusType.Enabled,
                                                            ComponentStatus components = ComponentStatus.Enabled,
                                                            ReadOnlySpan<ushort> clusters = default,
                                                            uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                            uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionAdapter<T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionAdapter<T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        data = default;
                    }
                }
            }
            #endregion

            #region UNSAFE DELEGATE
            /// <inheritdoc cref="For{T0}(QueryFunction{T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public unsafe void For<T0, T1, T2, T3, T4, T5>(delegate*<ref T0, ref T1, ref T2, ref T3, ref T4, ref T5, void> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryUnsafeFunctionAdapter<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryUnsafeFunctionAdapter<T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4, T5>(0),
                    entities, components, queryMode, clusters);
                #else
                ForInternal<T0, T1, T2, T3, T4, T5, QueryUnsafeFunctionAdapter<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    entities, components, queryMode, clusters);
                #endif
            }

            /// <inheritdoc cref="For{T0}(QueryFunctionWithEntity{TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public unsafe void For<T0, T1, T2, T3, T4, T5>(delegate*<Entity, ref T0, ref T1, ref T2, ref T3, ref T4, ref T5, void> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryUnsafeFunctionWithEntityAdapter<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryUnsafeFunctionWithEntityAdapter<T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4, T5>(0),
                    entities, components, queryMode, clusters);
                #else
                ForInternal<T0, T1, T2, T3, T4, T5, QueryUnsafeFunctionWithEntityAdapter<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    entities, components, queryMode, clusters);
                #endif
            }
            #endregion
            
        #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            #region DELEGATE WITH DATA AND ENTITY
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                           QueryFunctionWithRefDataEntityR1<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataEntityAdapterWrite5Read1<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterWrite5Read1<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3, T4>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4>(0),
                    entities, components, queryMode, clusters);
            }

            /// <inheritdoc cref="For{TData,T0}(TData, QueryFunctionWithRefDataEntity{TData,TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                           QueryFunctionWithRefDataEntityR1<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataEntityAdapterWrite5Read1<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterWrite5Read1<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3, T4>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4>(0),
                    entities, components, queryMode, clusters);
                userData = adapter.UserData;
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                                   QueryFunctionWithRefDataEntityR1<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ForParallel(ref userData, function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <inheritdoc cref="ForParallel{TData,T0, T1, T2, T3, T4, T5}(ref TData, QueryFunctionWithRefDataEntityR1{TData,TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                                   QueryFunctionWithRefDataEntityR1<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithDataEntityAdapterWrite5Read1<TData, T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    data.Value.UserData = userData;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterWrite5Read1<TData, T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        userData = data.Value.UserData;
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH ENTITY
            /// <inheritdoc cref="Search{T0, T1, T2, T3, T4, T5}(out Entity, SearchFunctionWithEntity{TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<T0, T1, T2, T3, T4, T5>(QueryFunctionWithEntityR1<TWorld, T0, T1, T2, T3, T4, T5> function,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ComponentStatus components = ComponentStatus.Enabled,
                                                    QueryMode queryMode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithEntityAdapterWrite5Read1<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithEntityAdapterWrite5Read1<T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3, T4>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4>(0),
                    entities, components, queryMode, clusters);
            }
            
            /// <inheritdoc cref="ForParallel{T0, T1, T2, T3, T4, T5}(QueryFunctionWithEntityR1{TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<T0, T1, T2, T3, T4, T5>(QueryFunctionWithEntityR1<TWorld, T0, T1, T2, T3, T4, T5> function,
                                                            EntityStatusType entities = EntityStatusType.Enabled,
                                                            ComponentStatus components = ComponentStatus.Enabled,
                                                            ReadOnlySpan<ushort> clusters = default,
                                                            uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                            uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithEntityAdapterWrite5Read1<T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithEntityAdapterWrite5Read1<T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH DATA
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                           QueryFunctionWithRefDataR1<TData, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataAdapterWrite5Read1<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterWrite5Read1<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3, T4>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4>(0),
                    entities, components, queryMode, clusters);
            }

            /// <inheritdoc cref="For{TData,T0}(TData, QueryFunctionWithRefData{TData,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                           QueryFunctionWithRefDataR1<TData, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataAdapterWrite5Read1<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterWrite5Read1<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3, T4>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4>(0),
                    entities, components, queryMode, clusters);
                userData = adapter.UserData;
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                                   QueryFunctionWithRefDataR1<TData, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ForParallel(ref userData, function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <inheritdoc cref="ForParallel{TData,T0, T1, T2, T3, T4, T5}(ref TData, QueryFunctionWithRefDataR1{TData,TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                                   QueryFunctionWithRefDataR1<TData, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithDataAdapterWrite5Read1<TData, T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    data.Value.UserData = userData;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterWrite5Read1<TData, T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        userData = data.Value.UserData;
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE
            /// <inheritdoc cref="For{T0}(QueryFunctionWithEntity{TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<T0, T1, T2, T3, T4, T5>(QueryFunctionR1<T0, T1, T2, T3, T4, T5> function,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ComponentStatus components = ComponentStatus.Enabled,
                                                    QueryMode queryMode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionAdapterWrite5Read1<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionAdapterWrite5Read1<T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3, T4>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4>(0),
                    entities, components, queryMode, clusters);
            }
            
            /// <inheritdoc cref="ForParallel{T0, T1, T2, T3, T4, T5}(QueryFunctionR1{T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<T0, T1, T2, T3, T4, T5>(QueryFunctionR1<T0, T1, T2, T3, T4, T5> function,
                                                            EntityStatusType entities = EntityStatusType.Enabled,
                                                            ComponentStatus components = ComponentStatus.Enabled,
                                                            ReadOnlySpan<ushort> clusters = default,
                                                            uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                            uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionAdapterWrite5Read1<T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionAdapterWrite5Read1<T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH DATA AND ENTITY
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                           QueryFunctionWithRefDataEntityR2<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataEntityAdapterWrite4Read2<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterWrite4Read2<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3>(0),
                    entities, components, queryMode, clusters);
            }

            /// <inheritdoc cref="For{TData,T0}(TData, QueryFunctionWithRefDataEntity{TData,TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                           QueryFunctionWithRefDataEntityR2<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataEntityAdapterWrite4Read2<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterWrite4Read2<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3>(0),
                    entities, components, queryMode, clusters);
                userData = adapter.UserData;
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                                   QueryFunctionWithRefDataEntityR2<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ForParallel(ref userData, function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <inheritdoc cref="ForParallel{TData,T0, T1, T2, T3, T4, T5}(ref TData, QueryFunctionWithRefDataEntityR2{TData,TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                                   QueryFunctionWithRefDataEntityR2<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithDataEntityAdapterWrite4Read2<TData, T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    data.Value.UserData = userData;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterWrite4Read2<TData, T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        userData = data.Value.UserData;
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH ENTITY
            /// <inheritdoc cref="Search{T0, T1, T2, T3, T4, T5}(out Entity, SearchFunctionWithEntity{TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<T0, T1, T2, T3, T4, T5>(QueryFunctionWithEntityR2<TWorld, T0, T1, T2, T3, T4, T5> function,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ComponentStatus components = ComponentStatus.Enabled,
                                                    QueryMode queryMode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithEntityAdapterWrite4Read2<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithEntityAdapterWrite4Read2<T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3>(0),
                    entities, components, queryMode, clusters);
            }
            
            /// <inheritdoc cref="ForParallel{T0, T1, T2, T3, T4, T5}(QueryFunctionWithEntityR2{TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<T0, T1, T2, T3, T4, T5>(QueryFunctionWithEntityR2<TWorld, T0, T1, T2, T3, T4, T5> function,
                                                            EntityStatusType entities = EntityStatusType.Enabled,
                                                            ComponentStatus components = ComponentStatus.Enabled,
                                                            ReadOnlySpan<ushort> clusters = default,
                                                            uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                            uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithEntityAdapterWrite4Read2<T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithEntityAdapterWrite4Read2<T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH DATA
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                           QueryFunctionWithRefDataR2<TData, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataAdapterWrite4Read2<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterWrite4Read2<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3>(0),
                    entities, components, queryMode, clusters);
            }

            /// <inheritdoc cref="For{TData,T0}(TData, QueryFunctionWithRefData{TData,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                           QueryFunctionWithRefDataR2<TData, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataAdapterWrite4Read2<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterWrite4Read2<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3>(0),
                    entities, components, queryMode, clusters);
                userData = adapter.UserData;
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                                   QueryFunctionWithRefDataR2<TData, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ForParallel(ref userData, function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <inheritdoc cref="ForParallel{TData,T0, T1, T2, T3, T4, T5}(ref TData, QueryFunctionWithRefDataR2{TData,TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                                   QueryFunctionWithRefDataR2<TData, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithDataAdapterWrite4Read2<TData, T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    data.Value.UserData = userData;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterWrite4Read2<TData, T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        userData = data.Value.UserData;
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE
            /// <inheritdoc cref="For{T0}(QueryFunctionWithEntity{TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<T0, T1, T2, T3, T4, T5>(QueryFunctionR2<T0, T1, T2, T3, T4, T5> function,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ComponentStatus components = ComponentStatus.Enabled,
                                                    QueryMode queryMode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionAdapterWrite4Read2<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionAdapterWrite4Read2<T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3>(0),
                    entities, components, queryMode, clusters);
            }
            
            /// <inheritdoc cref="ForParallel{T0, T1, T2, T3, T4, T5}(QueryFunctionR2{T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<T0, T1, T2, T3, T4, T5>(QueryFunctionR2<T0, T1, T2, T3, T4, T5> function,
                                                            EntityStatusType entities = EntityStatusType.Enabled,
                                                            ComponentStatus components = ComponentStatus.Enabled,
                                                            ReadOnlySpan<ushort> clusters = default,
                                                            uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                            uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionAdapterWrite4Read2<T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionAdapterWrite4Read2<T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH DATA AND ENTITY
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                           QueryFunctionWithRefDataEntityR3<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataEntityAdapterWrite3Read3<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterWrite3Read3<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2>(0),
                    entities, components, queryMode, clusters);
            }

            /// <inheritdoc cref="For{TData,T0}(TData, QueryFunctionWithRefDataEntity{TData,TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                           QueryFunctionWithRefDataEntityR3<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataEntityAdapterWrite3Read3<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterWrite3Read3<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2>(0),
                    entities, components, queryMode, clusters);
                userData = adapter.UserData;
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                                   QueryFunctionWithRefDataEntityR3<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ForParallel(ref userData, function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <inheritdoc cref="ForParallel{TData,T0, T1, T2, T3, T4, T5}(ref TData, QueryFunctionWithRefDataEntityR3{TData,TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                                   QueryFunctionWithRefDataEntityR3<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithDataEntityAdapterWrite3Read3<TData, T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    data.Value.UserData = userData;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterWrite3Read3<TData, T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        userData = data.Value.UserData;
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH ENTITY
            /// <inheritdoc cref="Search{T0, T1, T2, T3, T4, T5}(out Entity, SearchFunctionWithEntity{TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<T0, T1, T2, T3, T4, T5>(QueryFunctionWithEntityR3<TWorld, T0, T1, T2, T3, T4, T5> function,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ComponentStatus components = ComponentStatus.Enabled,
                                                    QueryMode queryMode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithEntityAdapterWrite3Read3<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithEntityAdapterWrite3Read3<T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2>(0),
                    entities, components, queryMode, clusters);
            }
            
            /// <inheritdoc cref="ForParallel{T0, T1, T2, T3, T4, T5}(QueryFunctionWithEntityR3{TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<T0, T1, T2, T3, T4, T5>(QueryFunctionWithEntityR3<TWorld, T0, T1, T2, T3, T4, T5> function,
                                                            EntityStatusType entities = EntityStatusType.Enabled,
                                                            ComponentStatus components = ComponentStatus.Enabled,
                                                            ReadOnlySpan<ushort> clusters = default,
                                                            uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                            uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithEntityAdapterWrite3Read3<T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithEntityAdapterWrite3Read3<T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH DATA
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                           QueryFunctionWithRefDataR3<TData, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataAdapterWrite3Read3<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterWrite3Read3<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2>(0),
                    entities, components, queryMode, clusters);
            }

            /// <inheritdoc cref="For{TData,T0}(TData, QueryFunctionWithRefData{TData,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                           QueryFunctionWithRefDataR3<TData, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataAdapterWrite3Read3<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterWrite3Read3<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2>(0),
                    entities, components, queryMode, clusters);
                userData = adapter.UserData;
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                                   QueryFunctionWithRefDataR3<TData, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ForParallel(ref userData, function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <inheritdoc cref="ForParallel{TData,T0, T1, T2, T3, T4, T5}(ref TData, QueryFunctionWithRefDataR3{TData,TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                                   QueryFunctionWithRefDataR3<TData, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithDataAdapterWrite3Read3<TData, T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    data.Value.UserData = userData;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterWrite3Read3<TData, T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        userData = data.Value.UserData;
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE
            /// <inheritdoc cref="For{T0}(QueryFunctionWithEntity{TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<T0, T1, T2, T3, T4, T5>(QueryFunctionR3<T0, T1, T2, T3, T4, T5> function,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ComponentStatus components = ComponentStatus.Enabled,
                                                    QueryMode queryMode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionAdapterWrite3Read3<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionAdapterWrite3Read3<T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2>(0),
                    entities, components, queryMode, clusters);
            }
            
            /// <inheritdoc cref="ForParallel{T0, T1, T2, T3, T4, T5}(QueryFunctionR3{T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<T0, T1, T2, T3, T4, T5>(QueryFunctionR3<T0, T1, T2, T3, T4, T5> function,
                                                            EntityStatusType entities = EntityStatusType.Enabled,
                                                            ComponentStatus components = ComponentStatus.Enabled,
                                                            ReadOnlySpan<ushort> clusters = default,
                                                            uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                            uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionAdapterWrite3Read3<T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionAdapterWrite3Read3<T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH DATA AND ENTITY
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                           QueryFunctionWithRefDataEntityR4<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataEntityAdapterWrite2Read4<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterWrite2Read4<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1>
                >(ref adapter,
                    new ChangedTracker<T0, T1>(0),
                    entities, components, queryMode, clusters);
            }

            /// <inheritdoc cref="For{TData,T0}(TData, QueryFunctionWithRefDataEntity{TData,TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                           QueryFunctionWithRefDataEntityR4<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataEntityAdapterWrite2Read4<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterWrite2Read4<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1>
                >(ref adapter,
                    new ChangedTracker<T0, T1>(0),
                    entities, components, queryMode, clusters);
                userData = adapter.UserData;
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                                   QueryFunctionWithRefDataEntityR4<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ForParallel(ref userData, function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <inheritdoc cref="ForParallel{TData,T0, T1, T2, T3, T4, T5}(ref TData, QueryFunctionWithRefDataEntityR4{TData,TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                                   QueryFunctionWithRefDataEntityR4<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithDataEntityAdapterWrite2Read4<TData, T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    data.Value.UserData = userData;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterWrite2Read4<TData, T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        userData = data.Value.UserData;
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH ENTITY
            /// <inheritdoc cref="Search{T0, T1, T2, T3, T4, T5}(out Entity, SearchFunctionWithEntity{TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<T0, T1, T2, T3, T4, T5>(QueryFunctionWithEntityR4<TWorld, T0, T1, T2, T3, T4, T5> function,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ComponentStatus components = ComponentStatus.Enabled,
                                                    QueryMode queryMode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithEntityAdapterWrite2Read4<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithEntityAdapterWrite2Read4<T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1>
                >(ref adapter,
                    new ChangedTracker<T0, T1>(0),
                    entities, components, queryMode, clusters);
            }
            
            /// <inheritdoc cref="ForParallel{T0, T1, T2, T3, T4, T5}(QueryFunctionWithEntityR4{TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<T0, T1, T2, T3, T4, T5>(QueryFunctionWithEntityR4<TWorld, T0, T1, T2, T3, T4, T5> function,
                                                            EntityStatusType entities = EntityStatusType.Enabled,
                                                            ComponentStatus components = ComponentStatus.Enabled,
                                                            ReadOnlySpan<ushort> clusters = default,
                                                            uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                            uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithEntityAdapterWrite2Read4<T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithEntityAdapterWrite2Read4<T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH DATA
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                           QueryFunctionWithRefDataR4<TData, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataAdapterWrite2Read4<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterWrite2Read4<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1>
                >(ref adapter,
                    new ChangedTracker<T0, T1>(0),
                    entities, components, queryMode, clusters);
            }

            /// <inheritdoc cref="For{TData,T0}(TData, QueryFunctionWithRefData{TData,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                           QueryFunctionWithRefDataR4<TData, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataAdapterWrite2Read4<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterWrite2Read4<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1>
                >(ref adapter,
                    new ChangedTracker<T0, T1>(0),
                    entities, components, queryMode, clusters);
                userData = adapter.UserData;
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                                   QueryFunctionWithRefDataR4<TData, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ForParallel(ref userData, function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <inheritdoc cref="ForParallel{TData,T0, T1, T2, T3, T4, T5}(ref TData, QueryFunctionWithRefDataR4{TData,TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                                   QueryFunctionWithRefDataR4<TData, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithDataAdapterWrite2Read4<TData, T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    data.Value.UserData = userData;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterWrite2Read4<TData, T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        userData = data.Value.UserData;
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE
            /// <inheritdoc cref="For{T0}(QueryFunctionWithEntity{TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<T0, T1, T2, T3, T4, T5>(QueryFunctionR4<T0, T1, T2, T3, T4, T5> function,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ComponentStatus components = ComponentStatus.Enabled,
                                                    QueryMode queryMode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionAdapterWrite2Read4<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionAdapterWrite2Read4<T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1>
                >(ref adapter,
                    new ChangedTracker<T0, T1>(0),
                    entities, components, queryMode, clusters);
            }
            
            /// <inheritdoc cref="ForParallel{T0, T1, T2, T3, T4, T5}(QueryFunctionR4{T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<T0, T1, T2, T3, T4, T5>(QueryFunctionR4<T0, T1, T2, T3, T4, T5> function,
                                                            EntityStatusType entities = EntityStatusType.Enabled,
                                                            ComponentStatus components = ComponentStatus.Enabled,
                                                            ReadOnlySpan<ushort> clusters = default,
                                                            uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                            uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionAdapterWrite2Read4<T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionAdapterWrite2Read4<T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH DATA AND ENTITY
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                           QueryFunctionWithRefDataEntityR5<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataEntityAdapterWrite1Read5<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterWrite1Read5<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0>
                >(ref adapter,
                    new ChangedTracker<T0>(0),
                    entities, components, queryMode, clusters);
            }

            /// <inheritdoc cref="For{TData,T0}(TData, QueryFunctionWithRefDataEntity{TData,TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                           QueryFunctionWithRefDataEntityR5<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataEntityAdapterWrite1Read5<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterWrite1Read5<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0>
                >(ref adapter,
                    new ChangedTracker<T0>(0),
                    entities, components, queryMode, clusters);
                userData = adapter.UserData;
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                                   QueryFunctionWithRefDataEntityR5<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ForParallel(ref userData, function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <inheritdoc cref="ForParallel{TData,T0, T1, T2, T3, T4, T5}(ref TData, QueryFunctionWithRefDataEntityR5{TData,TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                                   QueryFunctionWithRefDataEntityR5<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithDataEntityAdapterWrite1Read5<TData, T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    data.Value.UserData = userData;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterWrite1Read5<TData, T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        userData = data.Value.UserData;
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH ENTITY
            /// <inheritdoc cref="Search{T0, T1, T2, T3, T4, T5}(out Entity, SearchFunctionWithEntity{TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<T0, T1, T2, T3, T4, T5>(QueryFunctionWithEntityR5<TWorld, T0, T1, T2, T3, T4, T5> function,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ComponentStatus components = ComponentStatus.Enabled,
                                                    QueryMode queryMode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithEntityAdapterWrite1Read5<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithEntityAdapterWrite1Read5<T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0>
                >(ref adapter,
                    new ChangedTracker<T0>(0),
                    entities, components, queryMode, clusters);
            }
            
            /// <inheritdoc cref="ForParallel{T0, T1, T2, T3, T4, T5}(QueryFunctionWithEntityR5{TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<T0, T1, T2, T3, T4, T5>(QueryFunctionWithEntityR5<TWorld, T0, T1, T2, T3, T4, T5> function,
                                                            EntityStatusType entities = EntityStatusType.Enabled,
                                                            ComponentStatus components = ComponentStatus.Enabled,
                                                            ReadOnlySpan<ushort> clusters = default,
                                                            uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                            uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithEntityAdapterWrite1Read5<T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithEntityAdapterWrite1Read5<T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH DATA
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                           QueryFunctionWithRefDataR5<TData, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataAdapterWrite1Read5<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterWrite1Read5<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0>
                >(ref adapter,
                    new ChangedTracker<T0>(0),
                    entities, components, queryMode, clusters);
            }

            /// <inheritdoc cref="For{TData,T0}(TData, QueryFunctionWithRefData{TData,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                           QueryFunctionWithRefDataR5<TData, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataAdapterWrite1Read5<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterWrite1Read5<TData, T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0>
                >(ref adapter,
                    new ChangedTracker<T0>(0),
                    entities, components, queryMode, clusters);
                userData = adapter.UserData;
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                                   QueryFunctionWithRefDataR5<TData, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ForParallel(ref userData, function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <inheritdoc cref="ForParallel{TData,T0, T1, T2, T3, T4, T5}(ref TData, QueryFunctionWithRefDataR5{TData,TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                                   QueryFunctionWithRefDataR5<TData, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithDataAdapterWrite1Read5<TData, T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    data.Value.UserData = userData;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterWrite1Read5<TData, T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        userData = data.Value.UserData;
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE
            /// <inheritdoc cref="For{T0}(QueryFunctionWithEntity{TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<T0, T1, T2, T3, T4, T5>(QueryFunctionR5<T0, T1, T2, T3, T4, T5> function,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ComponentStatus components = ComponentStatus.Enabled,
                                                    QueryMode queryMode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionAdapterWrite1Read5<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                ForInternalTracked<T0, T1, T2, T3, T4, T5, QueryFunctionAdapterWrite1Read5<T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0>
                >(ref adapter,
                    new ChangedTracker<T0>(0),
                    entities, components, queryMode, clusters);
            }
            
            /// <inheritdoc cref="ForParallel{T0, T1, T2, T3, T4, T5}(QueryFunctionR5{T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<T0, T1, T2, T3, T4, T5>(QueryFunctionR5<T0, T1, T2, T3, T4, T5> function,
                                                            EntityStatusType entities = EntityStatusType.Enabled,
                                                            ComponentStatus components = ComponentStatus.Enabled,
                                                            ReadOnlySpan<ushort> clusters = default,
                                                            uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                            uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionAdapterWrite1Read5<T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionAdapterWrite1Read5<T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH DATA AND ENTITY
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                           QueryFunctionWithRefDataEntityR6<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataEntityAdapterRead<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterRead<TData, T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    entities, components, queryMode, clusters);
            }

            /// <inheritdoc cref="For{TData,T0}(TData, QueryFunctionWithRefDataEntity{TData,TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                           QueryFunctionWithRefDataEntityR6<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataEntityAdapterRead<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterRead<TData, T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    entities, components, queryMode, clusters);
                userData = adapter.UserData;
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                                   QueryFunctionWithRefDataEntityR6<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ForParallel(ref userData, function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <inheritdoc cref="ForParallel{TData,T0, T1, T2, T3, T4, T5}(ref TData, QueryFunctionWithRefDataEntityR6{TData,TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                                   QueryFunctionWithRefDataEntityR6<TData, TWorld, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithDataEntityAdapterRead<TData, T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    data.Value.UserData = userData;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataEntityAdapterRead<TData, T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        userData = data.Value.UserData;
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH ENTITY
            /// <inheritdoc cref="Search{T0, T1, T2, T3, T4, T5}(out Entity, SearchFunctionWithEntity{TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<T0, T1, T2, T3, T4, T5>(QueryFunctionWithEntityR6<TWorld, T0, T1, T2, T3, T4, T5> function,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ComponentStatus components = ComponentStatus.Enabled,
                                                    QueryMode queryMode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithEntityAdapterRead<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithEntityAdapterRead<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    entities, components, queryMode, clusters);
            }
            
            /// <inheritdoc cref="ForParallel{T0, T1, T2, T3, T4, T5}(QueryFunctionWithEntityR6{TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<T0, T1, T2, T3, T4, T5>(QueryFunctionWithEntityR6<TWorld, T0, T1, T2, T3, T4, T5> function,
                                                            EntityStatusType entities = EntityStatusType.Enabled,
                                                            ComponentStatus components = ComponentStatus.Enabled,
                                                            ReadOnlySpan<ushort> clusters = default,
                                                            uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                            uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithEntityAdapterRead<T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithEntityAdapterRead<T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE WITH DATA
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                           QueryFunctionWithRefDataR6<TData, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataAdapterRead<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterRead<TData, T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    entities, components, queryMode, clusters);
            }

            /// <inheritdoc cref="For{TData,T0}(TData, QueryFunctionWithRefData{TData,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                           QueryFunctionWithRefDataR6<TData, T0, T1, T2, T3, T4, T5> function,
                                                           EntityStatusType entities = EntityStatusType.Enabled,
                                                           ComponentStatus components = ComponentStatus.Enabled,
                                                           QueryMode queryMode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionWithDataAdapterRead<TData, T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                adapter.UserData = userData;
                ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterRead<TData, T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    entities, components, queryMode, clusters);
                userData = adapter.UserData;
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(TData userData,
                                                                   QueryFunctionWithRefDataR6<TData, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where TData : struct
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ForParallel(ref userData, function, entities, components, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <inheritdoc cref="ForParallel{TData,T0, T1, T2, T3, T4, T5}(ref TData, QueryFunctionWithRefDataR6{TData,TWorld,T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData, T0, T1, T2, T3, T4, T5>(ref TData userData,
                                                                   QueryFunctionWithRefDataR6<TData, T0, T1, T2, T3, T4, T5> function,
                                                                   EntityStatusType entities = EntityStatusType.Enabled,
                                                                   ComponentStatus components = ComponentStatus.Enabled,
                                                                   ReadOnlySpan<ushort> clusters = default,
                                                                   uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                                   uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionWithDataAdapterRead<TData, T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    data.Value.UserData = userData;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionWithDataAdapterRead<TData, T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        userData = data.Value.UserData;
                        data = default;
                    }
                }
            }
            #endregion

            #region DELEGATE
            /// <inheritdoc cref="For{T0}(QueryFunctionWithEntity{TWorld,T0}, EntityStatusType, ComponentStatus, QueryMode, ReadOnlySpan{ushort})"/>
            [MethodImpl(AggressiveInlining)]
            public void For<T0, T1, T2, T3, T4, T5>(QueryFunctionR6<T0, T1, T2, T3, T4, T5> function,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ComponentStatus components = ComponentStatus.Enabled,
                                                    QueryMode queryMode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                QueryFunctionAdapterRead<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                ForInternal<T0, T1, T2, T3, T4, T5, QueryFunctionAdapterRead<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    entities, components, queryMode, clusters);
            }
            
            /// <inheritdoc cref="ForParallel{T0, T1, T2, T3, T4, T5}(QueryFunctionR6{T0, T1, T2, T3, T4, T5}, EntityStatusType, ComponentStatus, ReadOnlySpan{ushort}, uint, uint)"/>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<T0, T1, T2, T3, T4, T5>(QueryFunctionR6<T0, T1, T2, T3, T4, T5> function,
                                                            EntityStatusType entities = EntityStatusType.Enabled,
                                                            ComponentStatus components = ComponentStatus.Enabled,
                                                            ReadOnlySpan<ushort> clusters = default,
                                                            uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                            uint workersLimit = 0)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                if (PrepareParallel<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, out var count, out var jobs, out var jobIndexes)) {
                    ref var data = ref Resources<ParallelData<QueryFunctionAdapterRead<T0, T1, T2, T3, T4, T5>>>.Value;
                    data.Value.Function = function;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ForParallelInternal<T0, T1, T2, T3, T4, T5, QueryFunctionAdapterRead<T0, T1, T2, T3, T4, T5>>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        data = default;
                    }
                }
            }
            #endregion

        #endif // !FFS_ECS_DISABLE_CHANGED_TRACKING
            #region BLOCKS
            [MethodImpl(AggressiveInlining)]
            public unsafe void ForBlock<T0, T1, T2, T3, T4, T5>(delegate*<uint, EntityBlock, Block<T0>, Block<T1>, Block<T2>, Block<T3>, Block<T4>, Block<T5>, void> function,
                                                                EntityStatusType entities = EntityStatusType.Enabled,
                                                                ComponentStatus components = ComponentStatus.Enabled,
                                                                ReadOnlySpan<ushort> clusters = default)
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent {
                BlockUnsafeAdapter<T0, T1, T2, T3, T4, T5> adapter;
                adapter.Function = function;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                ForBlockInternalTracked<T0, T1, T2, T3, T4, T5, BlockUnsafeAdapter<T0, T1, T2, T3, T4, T5>
                    , ChangedTracker<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    new ChangedTracker<T0, T1, T2, T3, T4, T5>(0),
                    entities, components, clusters);
                #else
                ForBlockInternal<T0, T1, T2, T3, T4, T5, BlockUnsafeAdapter<T0, T1, T2, T3, T4, T5>
                >(ref adapter,
                    entities, components, clusters);
                #endif
            }
            #endregion

            internal unsafe void ForBlockInternal<T0, T1, T2, T3, T4, T5, TAdapter>(ref TAdapter adapter,
                EntityStatusType entities,
                ComponentStatus components,
                ReadOnlySpan<ushort> clusters)
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent
                where TAdapter : struct, IBlockQueryAdapter<T0, T1, T2, T3, T4, T5>
                {
                if (Prepare<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, true, out var queryData, out var firstGlobalBlockIdx)) {
                    ref var world = ref Data.Instance;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        var segments0 = Components<T0>.Instance.ComponentSegments;
                        var segments1 = Components<T1>.Instance.ComponentSegments;
                        var segments2 = Components<T2>.Instance.ComponentSegments;
                        var segments3 = Components<T3>.Instance.ComponentSegments;
                        var segments4 = Components<T4>.Instance.ComponentSegments;
                        var segments5 = Components<T5>.Instance.ComponentSegments;
                        var b0 = new Block<T0>();
                        var b1 = new Block<T1>();
                        var b2 = new Block<T2>();
                        var b3 = new Block<T3>();
                        var b4 = new Block<T4>();
                        var b5 = new Block<T5>();

                        #if !NET6_0_OR_GREATER
                        var deBruijn = Utils.DeBruijn;
                        #endif

                        EntityBlock entityBlock = default;
                        ref var entityBlockOffset = ref entityBlock.Offset;
                        var blocks = queryData.Blocks;
                        do {
                            var segmentIdx = firstGlobalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                            var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT);
                            ref var block = ref blocks[firstGlobalBlockIdx];
                            ref var entitiesMaskRef = ref block.EntitiesMask;
                            firstGlobalBlockIdx = block.NextGlobalBlock;
                            var entitiesMask = entitiesMaskRef;
                            var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            fixed (T0* components0 = &segments0[segmentIdx][componentOffset]) {
                                fixed (T1* components1 = &segments1[segmentIdx][componentOffset]) {
                                    fixed (T2* components2 = &segments2[segmentIdx][componentOffset]) {
                                        fixed (T3* components3 = &segments3[segmentIdx][componentOffset]) {
                                            fixed (T4* components4 = &segments4[segmentIdx][componentOffset]) {
                                                fixed (T5* components5 = &segments5[segmentIdx][componentOffset]) {
                                                    if (entitiesMask == ulong.MaxValue) {
                                                        b0.Ptr = components0;
                                                        b1.Ptr = components1;
                                                        b2.Ptr = components2;
                                                        b3.Ptr = components3;
                                                        b4.Ptr = components4;
                                                        b5.Ptr = components5;

                                                        #if FFS_ECS_DEBUG
                                                        b0.Count = Const.U64_BITS;
                                                        b1.Count = Const.U64_BITS;
                                                        b2.Count = Const.U64_BITS;
                                                        b3.Count = Const.U64_BITS;
                                                        b4.Count = Const.U64_BITS;
                                                        b5.Count = Const.U64_BITS;
                                                        world.SetCurrentQueryEntity(chunkBlockEntityId, chunkBlockEntityId + Const.U64_BITS - 1);
                                                        #endif

                                                        entityBlockOffset = chunkBlockEntityId;
                                                        adapter.Invoke(
                                                            Const.U64_BITS,
                                                            entityBlock,
                                                            b0,
                                                            b1,
                                                            b2,
                                                            b3,
                                                            b4,
                                                            b5
                                                        );
                                                    }
                                                    else {
                                                        var runStarts = entitiesMask & ~(entitiesMask << 1);
                                                        var runEnds = entitiesMask & ~(entitiesMask >> 1);
                                                        do {
                                                            #if NET6_0_OR_GREATER
                                                            var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(runStarts);
                                                            var runEnd = (byte)System.Numerics.BitOperations.TrailingZeroCount(runEnds);
                                                            #else
                                                            var runStart = deBruijn[(uint)(((runStarts & (ulong)-(long)runStarts) * 0x37E84A99DAE458FUL) >> 58)];
                                                            var runEnd = deBruijn[(uint)(((runEnds & (ulong)-(long)runEnds) * 0x37E84A99DAE458FUL) >> 58)];
                                                            #endif
                                                            runStarts &= runStarts - 1UL;
                                                            runEnds &= runEnds - 1UL;

                                                            b0.Ptr = components0 + runStart;
                                                            b1.Ptr = components1 + runStart;
                                                            b2.Ptr = components2 + runStart;
                                                            b3.Ptr = components3 + runStart;
                                                            b4.Ptr = components4 + runStart;
                                                            b5.Ptr = components5 + runStart;
                                                            var blockSize = (uint)(runEnd - runStart + 1);

                                                            #if FFS_ECS_DEBUG
                                                            b0.Count = blockSize;
                                                            b1.Count = blockSize;
                                                            b2.Count = blockSize;
                                                            b3.Count = blockSize;
                                                            b4.Count = blockSize;
                                                            b5.Count = blockSize;
                                                            world.SetCurrentQueryEntity(chunkBlockEntityId + runStart, chunkBlockEntityId + runEnd);
                                                            #endif
                                                            entityBlockOffset = chunkBlockEntityId + runStart;
                                                            adapter.Invoke(
                                                                blockSize,
                                                                entityBlock,
                                                                b0,
                                                            b1,
                                                            b2,
                                                            b3,
                                                            b4,
                                                            b5
                                                            );
                                                        } while (runStarts != 0);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        } while (firstGlobalBlockIdx >= 0);
                    }

                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        Dispose<T0, T1, T2, T3, T4, T5>(Filter, entities, components, true, queryData);
                    }
                }
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            [MethodImpl(AggressiveInlining)]
            internal unsafe void ForBlockInternal<T0, T1, T2, T3, T4, T5, TAdapter, TTracker>(ref TAdapter adapter,
                TTracker tracker,
                EntityStatusType entities,
                ComponentStatus components,
                ReadOnlySpan<ushort> clusters)
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent
                where TAdapter : struct, IBlockQueryAdapter<T0, T1, T2, T3, T4, T5>
                where TTracker : struct, IChangedTracker
                {
                if (Prepare<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, true, out var queryData, out var firstGlobalBlockIdx)) {
                    ref var world = ref Data.Instance;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        var segments0 = Components<T0>.Instance.ComponentSegments;
                        var segments1 = Components<T1>.Instance.ComponentSegments;
                        var segments2 = Components<T2>.Instance.ComponentSegments;
                        var segments3 = Components<T3>.Instance.ComponentSegments;
                        var segments4 = Components<T4>.Instance.ComponentSegments;
                        var segments5 = Components<T5>.Instance.ComponentSegments;
                        var b0 = new Block<T0>();
                        var b1 = new Block<T1>();
                        var b2 = new Block<T2>();
                        var b3 = new Block<T3>();
                        var b4 = new Block<T4>();
                        var b5 = new Block<T5>();

                        #if !NET6_0_OR_GREATER
                        var deBruijn = Utils.DeBruijn;
                        #endif

                        EntityBlock entityBlock = default;
                        ref var entityBlockOffset = ref entityBlock.Offset;
                        var blocks = queryData.Blocks;
                        do {
                            var segmentIdx = firstGlobalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                            var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT);
                            ref var block = ref blocks[firstGlobalBlockIdx];
                            ref var entitiesMaskRef = ref block.EntitiesMask;
                            firstGlobalBlockIdx = block.NextGlobalBlock;
                            var entitiesMask = entitiesMaskRef;
                            var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                            var trackSegmentBlockIdx = (byte)((chunkBlockEntityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                            var trackChunkBlockIdx = (byte)((chunkBlockEntityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                            var trackChunkIdx = chunkBlockEntityId >> Const.ENTITIES_IN_CHUNK_SHIFT;
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            fixed (T0* components0 = &segments0[segmentIdx][componentOffset]) {
                                fixed (T1* components1 = &segments1[segmentIdx][componentOffset]) {
                                    fixed (T2* components2 = &segments2[segmentIdx][componentOffset]) {
                                        fixed (T3* components3 = &segments3[segmentIdx][componentOffset]) {
                                            fixed (T4* components4 = &segments4[segmentIdx][componentOffset]) {
                                                fixed (T5* components5 = &segments5[segmentIdx][componentOffset]) {
                                                    if (entitiesMask == ulong.MaxValue) {
                                                        b0.Ptr = components0;
                                                        b1.Ptr = components1;
                                                        b2.Ptr = components2;
                                                        b3.Ptr = components3;
                                                        b4.Ptr = components4;
                                                        b5.Ptr = components5;

                                                        #if FFS_ECS_DEBUG
                                                        b0.Count = Const.U64_BITS;
                                                        b1.Count = Const.U64_BITS;
                                                        b2.Count = Const.U64_BITS;
                                                        b3.Count = Const.U64_BITS;
                                                        b4.Count = Const.U64_BITS;
                                                        b5.Count = Const.U64_BITS;
                                                        world.SetCurrentQueryEntity(chunkBlockEntityId, chunkBlockEntityId + Const.U64_BITS - 1);
                                                        #endif

                                                        entityBlockOffset = chunkBlockEntityId;
                                                        adapter.Invoke(
                                                            Const.U64_BITS,
                                                            entityBlock,
                                                            b0,
                                                            b1,
                                                            b2,
                                                            b3,
                                                            b4,
                                                            b5
                                                        );
                                                    }
                                                    else {
                                                        var runStarts = entitiesMask & ~(entitiesMask << 1);
                                                        var runEnds = entitiesMask & ~(entitiesMask >> 1);
                                                        do {
                                                            #if NET6_0_OR_GREATER
                                                            var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(runStarts);
                                                            var runEnd = (byte)System.Numerics.BitOperations.TrailingZeroCount(runEnds);
                                                            #else
                                                            var runStart = deBruijn[(uint)(((runStarts & (ulong)-(long)runStarts) * 0x37E84A99DAE458FUL) >> 58)];
                                                            var runEnd = deBruijn[(uint)(((runEnds & (ulong)-(long)runEnds) * 0x37E84A99DAE458FUL) >> 58)];
                                                            #endif
                                                            runStarts &= runStarts - 1UL;
                                                            runEnds &= runEnds - 1UL;

                                                            b0.Ptr = components0 + runStart;
                                                            b1.Ptr = components1 + runStart;
                                                            b2.Ptr = components2 + runStart;
                                                            b3.Ptr = components3 + runStart;
                                                            b4.Ptr = components4 + runStart;
                                                            b5.Ptr = components5 + runStart;
                                                            var blockSize = (uint)(runEnd - runStart + 1);

                                                            #if FFS_ECS_DEBUG
                                                            b0.Count = blockSize;
                                                            b1.Count = blockSize;
                                                            b2.Count = blockSize;
                                                            b3.Count = blockSize;
                                                            b4.Count = blockSize;
                                                            b5.Count = blockSize;
                                                            world.SetCurrentQueryEntity(chunkBlockEntityId + runStart, chunkBlockEntityId + runEnd);
                                                            #endif
                                                            entityBlockOffset = chunkBlockEntityId + runStart;
                                                            adapter.Invoke(
                                                                blockSize,
                                                                entityBlock,
                                                                b0,
                                                            b1,
                                                            b2,
                                                            b3,
                                                            b4,
                                                            b5
                                                            );
                                                        } while (runStarts != 0);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            tracker.ApplyBlock((uint)segmentIdx, trackSegmentBlockIdx, entitiesMask, trackChunkBlockIdx, trackChunkIdx);
                        } while (firstGlobalBlockIdx >= 0);
                    }

                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        Dispose<T0, T1, T2, T3, T4, T5>(Filter, entities, components, true, queryData);
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal unsafe void ForBlockInternalTracked<T0, T1, T2, T3, T4, T5, TAdapter, TTracker>(ref TAdapter adapter,
                TTracker tracker,
                EntityStatusType entities,
                ComponentStatus components,
                ReadOnlySpan<ushort> clusters)
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent
                where TAdapter : struct, IBlockQueryAdapter<T0, T1, T2, T3, T4, T5>
                where TTracker : struct, IChangedTracker
            {
                if (tracker.IsActive) {
                    ForBlockInternal<T0, T1, T2, T3, T4, T5, TAdapter, TTracker>(ref adapter, tracker, entities, components, clusters);
                } else {
                    ForBlockInternal<T0, T1, T2, T3, T4, T5, TAdapter>(ref adapter, entities, components, clusters);
                }
            }
            #endif


            internal static unsafe void ForBlockParallelInternal<T0, T1, T2, T3, T4, T5, TAdapter>(Job[] jobs, uint[] jobIndexes, uint from, uint to, int worker)
                where T0 : unmanaged, IComponent
                where T1 : unmanaged, IComponent
                where T2 : unmanaged, IComponent
                where T3 : unmanaged, IComponent
                where T4 : unmanaged, IComponent
                where T5 : unmanaged, IComponent
                where TAdapter : struct, IBlockQueryAdapter<T0, T1, T2, T3, T4, T5> {
                ref var world = ref Data.Instance;
                var segments0 = Components<T0>.Instance.ComponentSegments;
                var segments1 = Components<T1>.Instance.ComponentSegments;
                var segments2 = Components<T2>.Instance.ComponentSegments;
                var segments3 = Components<T3>.Instance.ComponentSegments;
                var segments4 = Components<T4>.Instance.ComponentSegments;
                var segments5 = Components<T5>.Instance.ComponentSegments;
                var b0 = new Block<T0>();
                var b1 = new Block<T1>();
                var b2 = new Block<T2>();
                var b3 = new Block<T3>();
                var b4 = new Block<T4>();
                var b5 = new Block<T5>();

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                ref var adapter = ref Resources<ParallelData<TAdapter>>.Value.Value;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                var tracker = new ChangedTracker<T0, T1, T2, T3, T4, T5>(0);
                var hasTracking = tracker.IsActive;
                #endif
                EntityBlock entityBlock = default;
                ref var entityBlockOffset = ref entityBlock.Offset;

                while (from < to) {
                    ref var job = ref jobs[jobIndexes[from++]];
                    var count = job.Count;
                    job.Count = 0;

                    var segmentIdx = job.GlobalBlockIdx[0] >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                    fixed (T0* components0 = &segments0[segmentIdx][0]) {
                        fixed (T1* components1 = &segments1[segmentIdx][0]) {
                            fixed (T2* components2 = &segments2[segmentIdx][0]) {
                                fixed (T3* components3 = &segments3[segmentIdx][0]) {
                                    fixed (T4* components4 = &segments4[segmentIdx][0]) {
                                        fixed (T5* components5 = &segments5[segmentIdx][0]) {
                                            for (uint i = 0; i < count; i++) {
                                                var chunkBlockEntityId = job.GlobalBlockIdx[i] << Const.ENTITIES_IN_BLOCK_SHIFT;
                                                var entitiesMask = job.Masks[i];
                                                var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                                                chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                                                if (entitiesMask == ulong.MaxValue) {
                                                    b0.Ptr = components0 + componentOffset;
                                                    b1.Ptr = components1 + componentOffset;
                                                    b2.Ptr = components2 + componentOffset;
                                                    b3.Ptr = components3 + componentOffset;
                                                    b4.Ptr = components4 + componentOffset;
                                                    b5.Ptr = components5 + componentOffset;

                                                    #if FFS_ECS_DEBUG
                                                    b0.Count = Const.U64_BITS;
                                                    b1.Count = Const.U64_BITS;
                                                    b2.Count = Const.U64_BITS;
                                                    b3.Count = Const.U64_BITS;
                                                    b4.Count = Const.U64_BITS;
                                                    b5.Count = Const.U64_BITS;
                                                    world.SetCurrentQueryEntity(chunkBlockEntityId, chunkBlockEntityId + Const.U64_BITS - 1);
                                                    #endif

                                                    entityBlockOffset = chunkBlockEntityId;
                                                    adapter.Invoke(
                                                        Const.U64_BITS,
                                                        entityBlock,
                                                        b0,
                                                        b1,
                                                        b2,
                                                        b3,
                                                        b4,
                                                        b5
                                                    );
                                                }
                                                else {
                                                    var runStarts = entitiesMask & ~(entitiesMask << 1);
                                                    var runEnds = entitiesMask & ~(entitiesMask >> 1);
                                                    do {
                                                        #if NET6_0_OR_GREATER
                                                        var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(runStarts);
                                                        var runEnd = (byte)System.Numerics.BitOperations.TrailingZeroCount(runEnds);
                                                        #else
                                                        var runStart = deBruijn[(uint)(((runStarts & (ulong)-(long)runStarts) * 0x37E84A99DAE458FUL) >> 58)];
                                                        var runEnd = deBruijn[(uint)(((runEnds & (ulong)-(long)runEnds) * 0x37E84A99DAE458FUL) >> 58)];
                                                        #endif
                                                        runStarts &= runStarts - 1UL;
                                                        runEnds &= runEnds - 1UL;

                                                        b0.Ptr = components0 + (componentOffset + runStart);
                                                        b1.Ptr = components1 + (componentOffset + runStart);
                                                        b2.Ptr = components2 + (componentOffset + runStart);
                                                        b3.Ptr = components3 + (componentOffset + runStart);
                                                        b4.Ptr = components4 + (componentOffset + runStart);
                                                        b5.Ptr = components5 + (componentOffset + runStart);
                                                        var blockSize = (uint)(runEnd - runStart + 1);

                                                        #if FFS_ECS_DEBUG
                                                        b0.Count = blockSize;
                                                        b1.Count = blockSize;
                                                        b2.Count = blockSize;
                                                        b3.Count = blockSize;
                                                        b4.Count = blockSize;
                                                        b5.Count = blockSize;
                                                        world.SetCurrentQueryEntity(chunkBlockEntityId + runStart, chunkBlockEntityId + runEnd);
                                                        #endif
                                                        entityBlockOffset = chunkBlockEntityId + runStart;
                                                        adapter.Invoke(
                                                            blockSize,
                                                            entityBlock,
                                                            b0,
                                                        b1,
                                                        b2,
                                                        b3,
                                                        b4,
                                                        b5
                                                        );
                                                    } while (runStarts != 0);
                                                }
                                                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                                                if (hasTracking) {
                                                    var origChunkBlockEntityId = chunkBlockEntityId - Const.ENTITY_ID_OFFSET;
                                                    var trackSegmentBlockIdx = (byte)((origChunkBlockEntityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                                                    var trackChunkBlockIdx = (byte)((origChunkBlockEntityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                                                    var trackChunkIdx = origChunkBlockEntityId >> Const.ENTITIES_IN_CHUNK_SHIFT;
                                                    tracker.ApplyBlock((uint)(segmentIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT), trackSegmentBlockIdx, entitiesMask, trackChunkBlockIdx, trackChunkIdx);
                                                }
                                                #endif
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void ForInternal<T0, T1, T2, T3, T4, T5, TFunction>(ref TFunction function,
                                                                         EntityStatusType entities = EntityStatusType.Enabled,
                                                                         ComponentStatus components = ComponentStatus.Enabled,
                                                                         QueryMode queryMode = QueryMode.Strict,
                                                                         ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where TFunction : struct, IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
            {
                ref var world = ref Data.Instance;
                var strict = queryMode == QueryMode.Strict;

                if (Prepare<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        var segments0 = Components<T0>.Instance.ComponentSegments;
                        var segments1 = Components<T1>.Instance.ComponentSegments;
                        var segments2 = Components<T2>.Instance.ComponentSegments;
                        var segments3 = Components<T3>.Instance.ComponentSegments;
                        var segments4 = Components<T4>.Instance.ComponentSegments;
                        var segments5 = Components<T5>.Instance.ComponentSegments;

                        #if !NET6_0_OR_GREATER
                        var deBruijn = Utils.DeBruijn;
                        #endif

                        #if NET6_0_OR_GREATER
                        Span<T0> comp0 = default;
                        Span<T1> comp1 = default;
                        Span<T2> comp2 = default;
                        Span<T3> comp3 = default;
                        Span<T4> comp4 = default;
                        Span<T5> comp5 = default;
                        #else
                        T0[] comp0 = null;
                        T1[] comp1 = null;
                        T2[] comp2 = null;
                        T3[] comp3 = null;
                        T4[] comp4 = null;
                        T5[] comp5 = null;
                        #endif

                        var blocks = queryData.Blocks;
                        var segmentIdx = uint.MaxValue;
                        var entity = new Entity();
                        ref var entityId = ref entity.IdWithOffset;

                        do {
                            var curSegmentIdx = firstGlobalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                            if (curSegmentIdx != segmentIdx) {
                                segmentIdx = (uint)curSegmentIdx;
                                #if NET6_0_OR_GREATER
                                comp0 = new(segments0[segmentIdx]);
                                comp1 = new(segments1[segmentIdx]);
                                comp2 = new(segments2[segmentIdx]);
                                comp3 = new(segments3[segmentIdx]);
                                comp4 = new(segments4[segmentIdx]);
                                comp5 = new(segments5[segmentIdx]);
                                #else
                                comp0 = segments0[segmentIdx];
                                comp1 = segments1[segmentIdx];
                                comp2 = segments2[segmentIdx];
                                comp3 = segments3[segmentIdx];
                                comp4 = segments4[segmentIdx];
                                comp5 = segments5[segmentIdx];
                                #endif
                            }

                            var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT);
                            ref var block = ref blocks[firstGlobalBlockIdx];
                            ref var entitiesMaskRef = ref block.EntitiesMask;
                            firstGlobalBlockIdx = block.NextGlobalBlock;
                            var entitiesMask = entitiesMaskRef;
                            #if NET6_0_OR_GREATER
                            var componentOffset = (int)(chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK);
                            #else
                            var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                            #endif
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            if (strict) {
                                if (entitiesMask == ulong.MaxValue) {
                                    var componentEnd = componentOffset + Const.U64_BITS;
                                    entityId = chunkBlockEntityId;
                                    function.InvokeBlock(entity, componentOffset, componentEnd, comp0, comp1, comp2, comp3, comp4, comp5);
                                }
                                else {
                                    var runStarts = entitiesMask & ~(entitiesMask << 1);
                                    var runEnds = entitiesMask & ~(entitiesMask >> 1);
                                    do {
                                        #if NET6_0_OR_GREATER
                                        var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(runStarts);
                                        var runEnd = (byte)System.Numerics.BitOperations.TrailingZeroCount(runEnds);
                                        #else
                                        var runStart = deBruijn[(uint)(((runStarts & (ulong)-(long)runStarts) * 0x37E84A99DAE458FUL) >> 58)];
                                        var runEnd = deBruijn[(uint)(((runEnds & (ulong)-(long)runEnds) * 0x37E84A99DAE458FUL) >> 58)];
                                        #endif
                                        runStarts &= runStarts - 1UL;
                                        runEnds &= runEnds - 1UL;
                                        var componentIdx = runStart + componentOffset;
                                        var componentEnd = runEnd + componentOffset;
                                        entityId = chunkBlockEntityId + runStart;
                                        function.InvokeBlock(entity, componentIdx, componentEnd + 1, comp0, comp1, comp2, comp3, comp4, comp5);
                                    } while (runStarts != 0);
                                }
                            }
                            else {
                                do {
                                    var isolatedBit = entitiesMask & (ulong)-(long)entitiesMask;
                                    #if NET6_0_OR_GREATER
                                    var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                                    #else
                                    var runStart = deBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                                    #endif

                                    var idx = runStart + componentOffset;
                                    entityId = chunkBlockEntityId + runStart;

                                    do {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        function.Invoke(entity, ref comp0[idx], ref comp1[idx], ref comp2[idx], ref comp3[idx], ref comp4[idx], ref comp5[idx]);
                                        isolatedBit <<= 1;
                                        idx++;
                                        entityId++;
                                    } while ((entitiesMaskRef & isolatedBit) != 0);

                                    entitiesMask = entitiesMaskRef & ~(isolatedBit - 1);
                                } while (entitiesMask != 0);
                            }
                        } while (firstGlobalBlockIdx >= 0);
                    }

                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        Dispose<T0, T1, T2, T3, T4, T5>(Filter, entities, components, strict, queryData);
                    }
                }
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            [MethodImpl(AggressiveInlining)]
            internal void ForInternal<T0, T1, T2, T3, T4, T5, TFunction, TTracker>(ref TFunction function,
                TTracker tracker,
                                                                         EntityStatusType entities = EntityStatusType.Enabled,
                                                                         ComponentStatus components = ComponentStatus.Enabled,
                                                                         QueryMode queryMode = QueryMode.Strict,
                                                                         ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where TFunction : struct, IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where TTracker : struct, IChangedTracker
            {
                ref var world = ref Data.Instance;
                var strict = queryMode == QueryMode.Strict;

                if (Prepare<T0, T1, T2, T3, T4, T5>(Filter, clusters, entities, components, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        var segments0 = Components<T0>.Instance.ComponentSegments;
                        var segments1 = Components<T1>.Instance.ComponentSegments;
                        var segments2 = Components<T2>.Instance.ComponentSegments;
                        var segments3 = Components<T3>.Instance.ComponentSegments;
                        var segments4 = Components<T4>.Instance.ComponentSegments;
                        var segments5 = Components<T5>.Instance.ComponentSegments;

                        #if !NET6_0_OR_GREATER
                        var deBruijn = Utils.DeBruijn;
                        #endif

                        #if NET6_0_OR_GREATER
                        Span<T0> comp0 = default;
                        Span<T1> comp1 = default;
                        Span<T2> comp2 = default;
                        Span<T3> comp3 = default;
                        Span<T4> comp4 = default;
                        Span<T5> comp5 = default;
                        #else
                        T0[] comp0 = null;
                        T1[] comp1 = null;
                        T2[] comp2 = null;
                        T3[] comp3 = null;
                        T4[] comp4 = null;
                        T5[] comp5 = null;
                        #endif

                        var blocks = queryData.Blocks;
                        var segmentIdx = uint.MaxValue;
                        var entity = new Entity();
                        ref var entityId = ref entity.IdWithOffset;

                        do {
                            var curSegmentIdx = firstGlobalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                            if (curSegmentIdx != segmentIdx) {
                                segmentIdx = (uint)curSegmentIdx;
                                #if NET6_0_OR_GREATER
                                comp0 = new(segments0[segmentIdx]);
                                comp1 = new(segments1[segmentIdx]);
                                comp2 = new(segments2[segmentIdx]);
                                comp3 = new(segments3[segmentIdx]);
                                comp4 = new(segments4[segmentIdx]);
                                comp5 = new(segments5[segmentIdx]);
                                #else
                                comp0 = segments0[segmentIdx];
                                comp1 = segments1[segmentIdx];
                                comp2 = segments2[segmentIdx];
                                comp3 = segments3[segmentIdx];
                                comp4 = segments4[segmentIdx];
                                comp5 = segments5[segmentIdx];
                                #endif
                            }

                            var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT);
                            ref var block = ref blocks[firstGlobalBlockIdx];
                            ref var entitiesMaskRef = ref block.EntitiesMask;
                            firstGlobalBlockIdx = block.NextGlobalBlock;
                            var entitiesMask = entitiesMaskRef;
                            #if NET6_0_OR_GREATER
                            var componentOffset = (int)(chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK);
                            #else
                            var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                            #endif
                            var trackSegmentBlockIdx = (byte)((chunkBlockEntityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                            var trackChunkBlockIdx = (byte)((chunkBlockEntityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                            var trackChunkIdx = chunkBlockEntityId >> Const.ENTITIES_IN_CHUNK_SHIFT;
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            if (strict) {
                                if (entitiesMask == ulong.MaxValue) {
                                    var componentEnd = componentOffset + Const.U64_BITS;
                                    entityId = chunkBlockEntityId;
                                    function.InvokeBlock(entity, componentOffset, componentEnd, comp0, comp1, comp2, comp3, comp4, comp5);
                                }
                                else {
                                    var runStarts = entitiesMask & ~(entitiesMask << 1);
                                    var runEnds = entitiesMask & ~(entitiesMask >> 1);
                                    do {
                                        #if NET6_0_OR_GREATER
                                        var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(runStarts);
                                        var runEnd = (byte)System.Numerics.BitOperations.TrailingZeroCount(runEnds);
                                        #else
                                        var runStart = deBruijn[(uint)(((runStarts & (ulong)-(long)runStarts) * 0x37E84A99DAE458FUL) >> 58)];
                                        var runEnd = deBruijn[(uint)(((runEnds & (ulong)-(long)runEnds) * 0x37E84A99DAE458FUL) >> 58)];
                                        #endif
                                        runStarts &= runStarts - 1UL;
                                        runEnds &= runEnds - 1UL;
                                        var componentIdx = runStart + componentOffset;
                                        var componentEnd = runEnd + componentOffset;
                                        entityId = chunkBlockEntityId + runStart;
                                        function.InvokeBlock(entity, componentIdx, componentEnd + 1, comp0, comp1, comp2, comp3, comp4, comp5);
                                    } while (runStarts != 0);
                                }
                                tracker.ApplyBlock(segmentIdx, trackSegmentBlockIdx, entitiesMask, trackChunkBlockIdx, trackChunkIdx);
                            }
                            else {
                                ulong trackedEntities = 0;
                                do {
                                    var isolatedBit = entitiesMask & (ulong)-(long)entitiesMask;
                                    #if NET6_0_OR_GREATER
                                    var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                                    #else
                                    var runStart = deBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                                    #endif

                                    var idx = runStart + componentOffset;
                                    entityId = chunkBlockEntityId + runStart;

                                    do {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        trackedEntities |= isolatedBit;
                                        function.Invoke(entity, ref comp0[idx], ref comp1[idx], ref comp2[idx], ref comp3[idx], ref comp4[idx], ref comp5[idx]);
                                        isolatedBit <<= 1;
                                        idx++;
                                        entityId++;
                                    } while ((entitiesMaskRef & isolatedBit) != 0);

                                    entitiesMask = entitiesMaskRef & ~(isolatedBit - 1);
                                } while (entitiesMask != 0);
                                tracker.ApplyBlock(segmentIdx, trackSegmentBlockIdx, trackedEntities, trackChunkBlockIdx, trackChunkIdx);
                            }
                        } while (firstGlobalBlockIdx >= 0);
                    }

                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        Dispose<T0, T1, T2, T3, T4, T5>(Filter, entities, components, strict, queryData);
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void ForInternalTracked<T0, T1, T2, T3, T4, T5, TFunction, TTracker>(ref TFunction function,
                TTracker tracker,
                EntityStatusType entities = EntityStatusType.Enabled,
                ComponentStatus components = ComponentStatus.Enabled,
                QueryMode queryMode = QueryMode.Strict,
                ReadOnlySpan<ushort> clusters = default)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where TFunction : struct, IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5>
                where TTracker : struct, IChangedTracker
            {
                if (tracker.IsActive) {
                    ForInternal<T0, T1, T2, T3, T4, T5, TFunction, TTracker>(ref function, tracker, entities, components, queryMode, clusters);
                } else {
                    ForInternal<T0, T1, T2, T3, T4, T5, TFunction>(ref function, entities, components, queryMode, clusters);
                }
            }
            #endif


            internal static unsafe void ForParallelInternal<T0, T1, T2, T3, T4, T5, TFunction>(Job[] jobs, uint[] jobIndexes, uint from, uint to, int worker)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where TFunction : struct, IQueryFunctionAdapter<T0, T1, T2, T3, T4, T5> {
                var segments0 = Components<T0>.Instance.ComponentSegments;
                var segments1 = Components<T1>.Instance.ComponentSegments;
                var segments2 = Components<T2>.Instance.ComponentSegments;
                var segments3 = Components<T3>.Instance.ComponentSegments;
                var segments4 = Components<T4>.Instance.ComponentSegments;
                var segments5 = Components<T5>.Instance.ComponentSegments;

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                #if NET6_0_OR_GREATER
                Span<T0> comp0;
                Span<T1> comp1;
                Span<T2> comp2;
                Span<T3> comp3;
                Span<T4> comp4;
                Span<T5> comp5;
                #else
                T0[] comp0;
                T1[] comp1;
                T2[] comp2;
                T3[] comp3;
                T4[] comp4;
                T5[] comp5;
                #endif

                ref var function = ref Resources<ParallelData<TFunction>>.Value.Value;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                var tracker = new ChangedTracker<T0, T1, T2, T3, T4, T5>(0);
                var hasTracking = tracker.IsActive;
                #endif
                var entity = new Entity();
                ref var entityId = ref entity.IdWithOffset;

                while (from < to) {
                    ref var job = ref jobs[jobIndexes[from++]];
                    var count = job.Count;
                    job.Count = 0;

                    var segmentIdx = job.GlobalBlockIdx[0] >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                    #if NET6_0_OR_GREATER
                    comp0 = new(segments0[segmentIdx]);
                    comp1 = new(segments1[segmentIdx]);
                    comp2 = new(segments2[segmentIdx]);
                    comp3 = new(segments3[segmentIdx]);
                    comp4 = new(segments4[segmentIdx]);
                    comp5 = new(segments5[segmentIdx]);
                    #else
                    comp0 = segments0[segmentIdx];
                    comp1 = segments1[segmentIdx];
                    comp2 = segments2[segmentIdx];
                    comp3 = segments3[segmentIdx];
                    comp4 = segments4[segmentIdx];
                    comp5 = segments5[segmentIdx];
                    #endif

                    for (uint i = 0; i < count; i++) {
                        var chunkBlockEntityId = job.GlobalBlockIdx[i] << Const.ENTITIES_IN_BLOCK_SHIFT;
                        var entitiesMask = job.Masks[i];
                        #if NET6_0_OR_GREATER
                        var componentOffset = (int)(chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK);
                        #else
                        var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                        #endif
                        chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                        if (entitiesMask == ulong.MaxValue) {
                            var componentEnd = componentOffset + Const.U64_BITS;
                            entityId = chunkBlockEntityId;
                            function.InvokeBlock(entity, componentOffset, componentEnd, comp0, comp1, comp2, comp3, comp4, comp5);
                        }
                        else {
                            var runStarts = entitiesMask & ~(entitiesMask << 1);
                            var runEnds = entitiesMask & ~(entitiesMask >> 1);
                            do {
                                #if NET6_0_OR_GREATER
                                var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(runStarts);
                                var runEnd = (byte)System.Numerics.BitOperations.TrailingZeroCount(runEnds);
                                #else
                                var runStart = deBruijn[(uint)(((runStarts & (ulong)-(long)runStarts) * 0x37E84A99DAE458FUL) >> 58)];
                                var runEnd = deBruijn[(uint)(((runEnds & (ulong)-(long)runEnds) * 0x37E84A99DAE458FUL) >> 58)];
                                #endif
                                runStarts &= runStarts - 1UL;
                                runEnds &= runEnds - 1UL;
                                var componentIdx = runStart + componentOffset;
                                var componentEnd = runEnd + componentOffset;
                                entityId = chunkBlockEntityId + runStart;
                                function.InvokeBlock(entity, componentIdx, componentEnd + 1, comp0, comp1, comp2, comp3, comp4, comp5);
                            } while (runStarts != 0);
                        }
                        #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                        if (hasTracking) {
                            var origChunkBlockEntityId = chunkBlockEntityId - Const.ENTITY_ID_OFFSET;
                            var trackSegmentBlockIdx = (byte)((origChunkBlockEntityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                            var trackChunkBlockIdx = (byte)((origChunkBlockEntityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                            var trackChunkIdx = origChunkBlockEntityId >> Const.ENTITIES_IN_CHUNK_SHIFT;
                            tracker.ApplyBlock((uint)(segmentIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT), trackSegmentBlockIdx, entitiesMask, trackChunkBlockIdx, trackChunkIdx);
                        }
                        #endif
                    }
                }
            }

            #region PREPARE AND DISPOSE
            [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
            [MethodImpl(AggressiveInlining)]
            internal bool Prepare<T0, T1, T2, T3, T4, T5>(TFilter filter, ReadOnlySpan<ushort> clusters, EntityStatusType entities, ComponentStatus components,
                                                          bool strict, out QueryData queryData, out int firstGlobalBlockIdx)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertNotNestedParallelQuery(WorldTypeName);
                AssertRegisteredComponent<T0>(Components<T0>.ComponentsTypeName);
                AssertRegisteredComponent<T1>(Components<T1>.ComponentsTypeName);
                AssertRegisteredComponent<T2>(Components<T2>.ComponentsTypeName);
                AssertRegisteredComponent<T3>(Components<T3>.ComponentsTypeName);
                AssertRegisteredComponent<T4>(Components<T4>.ComponentsTypeName);
                AssertRegisteredComponent<T5>(Components<T5>.ComponentsTypeName);
                filter.Assert<TWorld>();
                #endif


                ref var world = ref Data.Instance;
                ref var pool0 = ref Components<T0>.Instance;
                ref var pool1 = ref Components<T1>.Instance;
                ref var pool2 = ref Components<T2>.Instance;
                ref var pool3 = ref Components<T3>.Instance;
                ref var pool4 = ref Components<T4>.Instance;
                ref var pool5 = ref Components<T5>.Instance;

                clusters = world.GetActiveClustersIfEmpty(clusters);
                queryData = default;
                BlockMaskCache[] filteredBlocks = null;

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                var previousGlobalBlockIdx = -1;
                firstGlobalBlockIdx = -1;

                for (var i = 0; i < clusters.Length; i++) {
                    var clusterIdx = clusters[i];
                    ref var cluster = ref world.Clusters[clusterIdx];
                    if (cluster.Disabled) {
                        continue;
                    }

                    for (uint chunkMapIdx = 0; chunkMapIdx < cluster.LoadedChunksCount; chunkMapIdx++) {
                        var chunkIdx = cluster.LoadedChunks[chunkMapIdx];
                        var chunkMask = world.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                                        & pool0.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                                        & pool1.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                                        & pool2.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                                        & pool3.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                                        & pool4.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                                        & pool5.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
                        chunkMask &= filter.FilterChunk<TWorld>(chunkIdx);

                        if (chunkMask != 0) {
                            var segmentIdx = uint.MaxValue;

                            ulong[] worldMasks = null;
                            ulong[] pool0Masks = null;
                            ulong[] pool1Masks = null;
                            ulong[] pool2Masks = null;
                            ulong[] pool3Masks = null;
                            ulong[] pool4Masks = null;
                            ulong[] pool5Masks = null;

                            do {
                                #if NET6_0_OR_GREATER
                                var chunkBlockIdx = (uint)System.Numerics.BitOperations.TrailingZeroCount(chunkMask);
                                #else
                                var chunkBlockIdx = (uint)deBruijn[(uint)(((chunkMask & (ulong)-(long)chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                                #endif
                                chunkMask &= chunkMask - 1;
                                var globalBlockIdx = chunkBlockIdx + (chunkIdx << Const.BLOCKS_IN_CHUNK_SHIFT);

                                var curSegmentIdx = (chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT) + (chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT);
                                if (curSegmentIdx != segmentIdx) {
                                    segmentIdx = curSegmentIdx;
                                    worldMasks = world.EntitiesSegments[segmentIdx].Masks;
                                    pool0Masks = pool0.EntitiesMaskSegments[segmentIdx];
                                    pool1Masks = pool1.EntitiesMaskSegments[segmentIdx];
                                    pool2Masks = pool2.EntitiesMaskSegments[segmentIdx];
                                    pool3Masks = pool3.EntitiesMaskSegments[segmentIdx];
                                    pool4Masks = pool4.EntitiesMaskSegments[segmentIdx];
                                    pool5Masks = pool5.EntitiesMaskSegments[segmentIdx];
                                }

                                var blockIdx = (byte)(chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);
                                var disabledBlockIdx = blockIdx + Const.BLOCKS_IN_SEGMENT;
                                var loadedBlockIdx = disabledBlockIdx + Const.BLOCKS_IN_SEGMENT;

                                var entitiesMask = entities switch {
                                    EntityStatusType.Enabled => worldMasks[loadedBlockIdx] & worldMasks[blockIdx] & ~worldMasks[disabledBlockIdx],
                                    EntityStatusType.Disabled => worldMasks[loadedBlockIdx] & worldMasks[disabledBlockIdx],
                                    _ => worldMasks[loadedBlockIdx] & worldMasks[blockIdx]
                                };
                                entitiesMask &= components switch {
                                    ComponentStatus.Enabled => pool0Masks[blockIdx] & ~pool0Masks[disabledBlockIdx]
                                                                & pool1Masks[blockIdx] & ~pool1Masks[disabledBlockIdx]
                                                                & pool2Masks[blockIdx] & ~pool2Masks[disabledBlockIdx]
                                                                & pool3Masks[blockIdx] & ~pool3Masks[disabledBlockIdx]
                                                                & pool4Masks[blockIdx] & ~pool4Masks[disabledBlockIdx]
                                                                & pool5Masks[blockIdx] & ~pool5Masks[disabledBlockIdx],
                                    ComponentStatus.Disabled => pool0Masks[disabledBlockIdx]
                                                                & pool1Masks[disabledBlockIdx]
                                                                & pool2Masks[disabledBlockIdx]
                                                                & pool3Masks[disabledBlockIdx]
                                                                & pool4Masks[disabledBlockIdx]
                                                                & pool5Masks[disabledBlockIdx],
                                    _ => pool0Masks[blockIdx]
                                         & pool1Masks[blockIdx]
                                         & pool2Masks[blockIdx]
                                         & pool3Masks[blockIdx]
                                         & pool4Masks[blockIdx]
                                         & pool5Masks[blockIdx]
                                };
                                entitiesMask &= filter.FilterEntities<TWorld>(segmentIdx, blockIdx);

                                if (entitiesMask != 0) {
                                    if (previousGlobalBlockIdx >= 0) {
                                        filteredBlocks[previousGlobalBlockIdx].NextGlobalBlock = (int)globalBlockIdx;
                                    }
                                    else {
                                        #if FFS_ECS_DEBUG
                                        var queryMode = strict ? (byte)1 : (byte)2;
                                        AssertSameQueryMode(WorldTypeName, queryMode);
                                        world.QueryMode = queryMode;
                                        #endif

                                        queryData = world.PushCurrentQuery();

                                        if (!strict) {
                                            filter.PushQueryData<TWorld>(queryData);
                                            world.PushQueryDataForDestroy(queryData);

                                            switch (entities) {
                                                case EntityStatusType.Enabled: world.PushQueryDataForDisable(queryData); break;
                                                case EntityStatusType.Disabled: world.PushQueryDataForEnable(queryData); break;
                                            }

                                            switch (components) {
                                                case ComponentStatus.Enabled:
                                                    pool0.PushQueryDataForDeleteDisable(queryData);
                                                    pool1.PushQueryDataForDeleteDisable(queryData);
                                                    pool2.PushQueryDataForDeleteDisable(queryData);
                                                    pool3.PushQueryDataForDeleteDisable(queryData);
                                                    pool4.PushQueryDataForDeleteDisable(queryData);
                                                    pool5.PushQueryDataForDeleteDisable(queryData);
                                                    break;
                                                case ComponentStatus.Disabled:
                                                    pool0.PushQueryDataForDeleteEnable(queryData);
                                                    pool1.PushQueryDataForDeleteEnable(queryData);
                                                    pool2.PushQueryDataForDeleteEnable(queryData);
                                                    pool3.PushQueryDataForDeleteEnable(queryData);
                                                    pool4.PushQueryDataForDeleteEnable(queryData);
                                                    pool5.PushQueryDataForDeleteEnable(queryData);
                                                    break;
                                                default:
                                                    pool0.PushQueryDataForDelete(queryData);
                                                    pool1.PushQueryDataForDelete(queryData);
                                                    pool2.PushQueryDataForDelete(queryData);
                                                    pool3.PushQueryDataForDelete(queryData);
                                                    pool4.PushQueryDataForDelete(queryData);
                                                    pool5.PushQueryDataForDelete(queryData);
                                                    break;
                                            }
                                        }
                                        #if FFS_ECS_DEBUG
                                        else {
                                            const int block = 1;
                                            filter.Block<TWorld>(block);
                                            world.BlockDestroy(block);

                                            switch (entities) {
                                                case EntityStatusType.Enabled: world.BlockDisable(block); break;
                                                case EntityStatusType.Disabled: world.BlockEnable(block); break;
                                            }

                                            switch (components) {
                                                case ComponentStatus.Enabled:
                                                    pool0.BlockDeleteDisable(block);
                                                    pool1.BlockDeleteDisable(block);
                                                    pool2.BlockDeleteDisable(block);
                                                    pool3.BlockDeleteDisable(block);
                                                    pool4.BlockDeleteDisable(block);
                                                    pool5.BlockDeleteDisable(block);
                                                    break;
                                                case ComponentStatus.Disabled:
                                                    pool0.BlockDeleteEnable(block);
                                                    pool1.BlockDeleteEnable(block);
                                                    pool2.BlockDeleteEnable(block);
                                                    pool3.BlockDeleteEnable(block);
                                                    pool4.BlockDeleteEnable(block);
                                                    pool5.BlockDeleteEnable(block);
                                                    break;
                                                default:
                                                    pool0.BlockDelete(block);
                                                    pool1.BlockDelete(block);
                                                    pool2.BlockDelete(block);
                                                    pool3.BlockDelete(block);
                                                    pool4.BlockDelete(block);
                                                    pool5.BlockDelete(block);
                                                    break;
                                            }
                                        }
                                        #endif

                                        filteredBlocks = queryData.Blocks;
                                        firstGlobalBlockIdx = (int)globalBlockIdx;
                                    }

                                    filteredBlocks[globalBlockIdx].EntitiesMask = entitiesMask;
                                    filteredBlocks[globalBlockIdx].NextGlobalBlock = -1;
                                    previousGlobalBlockIdx = (int)globalBlockIdx;
                                }
                            } while (chunkMask != 0);
                        }
                    }
                }

                return filteredBlocks != null;
            }

            [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
            [MethodImpl(AggressiveInlining)]
            internal unsafe bool PrepareParallel<T0, T1, T2, T3, T4, T5>(TFilter filter, ReadOnlySpan<ushort> clusters,
                                                                         EntityStatusType entities,
                                                                         ComponentStatus components,
                                                                         out uint jobsCount, out Job[] jobs, out uint[] jobIndexes)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertNotNestedParallelQuery(WorldTypeName);
                AssertNotMoreThanOneParallelQuery(WorldTypeName);
                AssertParallelAvailable(WorldTypeName);
                AssertRegisteredComponent<T0>(Components<T0>.ComponentsTypeName);
                AssertRegisteredComponent<T1>(Components<T1>.ComponentsTypeName);
                AssertRegisteredComponent<T2>(Components<T2>.ComponentsTypeName);
                AssertRegisteredComponent<T3>(Components<T3>.ComponentsTypeName);
                AssertRegisteredComponent<T4>(Components<T4>.ComponentsTypeName);
                AssertRegisteredComponent<T5>(Components<T5>.ComponentsTypeName);
                filter.Assert<TWorld>();
                #endif

                ref var world = ref Data.Instance;
                ref var pool0 = ref Components<T0>.Instance;
                ref var pool1 = ref Components<T1>.Instance;
                ref var pool2 = ref Components<T2>.Instance;
                ref var pool3 = ref Components<T3>.Instance;
                ref var pool4 = ref Components<T4>.Instance;
                ref var pool5 = ref Components<T5>.Instance;

                clusters = world.GetActiveClustersIfEmpty(clusters);
                jobsCount = 0;
                jobs = default;
                jobIndexes = default;

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                for (var i = 0; i < clusters.Length; i++) {
                    var clusterIdx = clusters[i];
                    ref var cluster = ref world.Clusters[clusterIdx];
                    if (cluster.Disabled) {
                        continue;
                    }

                    for (uint chunkMapIdx = 0; chunkMapIdx < cluster.LoadedChunksCount; chunkMapIdx++) {
                        var chunkIdx = cluster.LoadedChunks[chunkMapIdx];
                        var chunkMask = world.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                                        & pool0.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                                        & pool1.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                                        & pool2.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                                        & pool3.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                                        & pool4.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value
                                        & pool5.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
                        chunkMask &= filter.FilterChunk<TWorld>(chunkIdx);

                        if (chunkMask != 0) {
                            var segmentIdx = uint.MaxValue;

                            ulong[] worldMasks = null;
                            ulong[] pool0Masks = null;
                            ulong[] pool1Masks = null;
                            ulong[] pool2Masks = null;
                            ulong[] pool3Masks = null;
                            ulong[] pool4Masks = null;
                            ulong[] pool5Masks = null;

                            do {
                                #if NET6_0_OR_GREATER
                                var chunkBlockIdx = (uint)System.Numerics.BitOperations.TrailingZeroCount(chunkMask);
                                #else
                                var chunkBlockIdx = (uint)deBruijn[(uint)(((chunkMask & (ulong)-(long)chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                                #endif
                                chunkMask &= chunkMask - 1;
                                var globalBlockIdx = chunkBlockIdx + (chunkIdx << Const.BLOCKS_IN_CHUNK_SHIFT);

                                var curSegmentIdx = (chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT) + (chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT);
                                if (curSegmentIdx != segmentIdx) {
                                    segmentIdx = curSegmentIdx;
                                    worldMasks = world.EntitiesSegments[segmentIdx].Masks;
                                    pool0Masks = pool0.EntitiesMaskSegments[segmentIdx];
                                    pool1Masks = pool1.EntitiesMaskSegments[segmentIdx];
                                    pool2Masks = pool2.EntitiesMaskSegments[segmentIdx];
                                    pool3Masks = pool3.EntitiesMaskSegments[segmentIdx];
                                    pool4Masks = pool4.EntitiesMaskSegments[segmentIdx];
                                    pool5Masks = pool5.EntitiesMaskSegments[segmentIdx];
                                }

                                var blockIdx = (byte)(chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);
                                var disabledBlockIdx = blockIdx + Const.BLOCKS_IN_SEGMENT;
                                var loadedBlockIdx = disabledBlockIdx + Const.BLOCKS_IN_SEGMENT;

                                var entitiesMask = entities switch {
                                    EntityStatusType.Enabled => worldMasks[loadedBlockIdx] & worldMasks[blockIdx] & ~worldMasks[disabledBlockIdx],
                                    EntityStatusType.Disabled => worldMasks[loadedBlockIdx] & worldMasks[disabledBlockIdx],
                                    _ => worldMasks[loadedBlockIdx] & worldMasks[blockIdx]
                                };
                                entitiesMask &= components switch {
                                    ComponentStatus.Enabled => pool0Masks[blockIdx] & ~pool0Masks[disabledBlockIdx]
                                                                & pool1Masks[blockIdx] & ~pool1Masks[disabledBlockIdx]
                                                                & pool2Masks[blockIdx] & ~pool2Masks[disabledBlockIdx]
                                                                & pool3Masks[blockIdx] & ~pool3Masks[disabledBlockIdx]
                                                                & pool4Masks[blockIdx] & ~pool4Masks[disabledBlockIdx]
                                                                & pool5Masks[blockIdx] & ~pool5Masks[disabledBlockIdx],
                                    ComponentStatus.Disabled => pool0Masks[disabledBlockIdx]
                                                                & pool1Masks[disabledBlockIdx]
                                                                & pool2Masks[disabledBlockIdx]
                                                                & pool3Masks[disabledBlockIdx]
                                                                & pool4Masks[disabledBlockIdx]
                                                                & pool5Masks[disabledBlockIdx],
                                    _ => pool0Masks[blockIdx]
                                         & pool1Masks[blockIdx]
                                         & pool2Masks[blockIdx]
                                         & pool3Masks[blockIdx]
                                         & pool4Masks[blockIdx]
                                         & pool5Masks[blockIdx]
                                };
                                entitiesMask &= filter.FilterEntities<TWorld>(segmentIdx, blockIdx);

                                if (entitiesMask != 0) {
                                    if (jobsCount == 0) {
                                        var size = world.EntitiesSegments.Length;
                                        if (ParallelRunner<TWorld>.CachedSize < size) {
                                            ParallelRunner<TWorld>.CachedJobs = new Job[size];
                                            ParallelRunner<TWorld>.CachedJobIndexes = new uint[size];
                                            ParallelRunner<TWorld>.CachedSize = size;
                                        }
                                        jobs = ParallelRunner<TWorld>.CachedJobs;
                                        jobIndexes = ParallelRunner<TWorld>.CachedJobIndexes;
                                        world.QueryDataCount++;
                                    }

                                    ref var job = ref jobs[segmentIdx];
                                    if (job.Count == 0) {
                                        jobIndexes[jobsCount++] = segmentIdx;
                                    }

                                    job.Masks[job.Count] = entitiesMask;
                                    job.GlobalBlockIdx[job.Count++] = globalBlockIdx;
                                }
                            } while (chunkMask != 0);
                        }
                    }
                }

                return jobsCount != 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal void Dispose<T0, T1, T2, T3, T4, T5>(TFilter filter, EntityStatusType entities, ComponentStatus components, bool strict, QueryData queryData)
                where T0 : struct, IComponent
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                ref var world = ref Data.Instance;
                ref var pool0 = ref Components<T0>.Instance;
                ref var pool1 = ref Components<T1>.Instance;
                ref var pool2 = ref Components<T2>.Instance;
                ref var pool3 = ref Components<T3>.Instance;
                ref var pool4 = ref Components<T4>.Instance;
                ref var pool5 = ref Components<T5>.Instance;

                world.PopCurrentQuery(queryData);

                if (!strict) {
                    filter.PopQueryData<TWorld>();
                    world.PopQueryDataForDestroy();

                    switch (entities) {
                        case EntityStatusType.Enabled: world.PopQueryDataForDisable(); break;
                        case EntityStatusType.Disabled: world.PopQueryDataForEnable(); break;
                    }

                    switch (components) {
                        case ComponentStatus.Enabled:
                            pool0.PopQueryDataForDeleteDisable();
                            pool1.PopQueryDataForDeleteDisable();
                            pool2.PopQueryDataForDeleteDisable();
                            pool3.PopQueryDataForDeleteDisable();
                            pool4.PopQueryDataForDeleteDisable();
                            pool5.PopQueryDataForDeleteDisable();
                            break;
                        case ComponentStatus.Disabled:
                            pool0.PopQueryDataForDeleteEnable();
                            pool1.PopQueryDataForDeleteEnable();
                            pool2.PopQueryDataForDeleteEnable();
                            pool3.PopQueryDataForDeleteEnable();
                            pool4.PopQueryDataForDeleteEnable();
                            pool5.PopQueryDataForDeleteEnable();
                            break;
                        default:
                            pool0.PopQueryDataForDelete();
                            pool1.PopQueryDataForDelete();
                            pool2.PopQueryDataForDelete();
                            pool3.PopQueryDataForDelete();
                            pool4.PopQueryDataForDelete();
                            pool5.PopQueryDataForDelete();
                            break;
                    }
                }
                #if FFS_ECS_DEBUG
                else {
                    const int unblock = -1;
                    filter.Block<TWorld>(unblock);
                    world.BlockDestroy(unblock);

                    switch (entities) {
                        case EntityStatusType.Enabled: world.BlockDisable(unblock); break;
                        case EntityStatusType.Disabled: world.BlockEnable(unblock); break;
                    }

                    switch (components) {
                        case ComponentStatus.Enabled:
                            pool0.BlockDeleteDisable(unblock);
                            pool1.BlockDeleteDisable(unblock);
                            pool2.BlockDeleteDisable(unblock);
                            pool3.BlockDeleteDisable(unblock);
                            pool4.BlockDeleteDisable(unblock);
                            pool5.BlockDeleteDisable(unblock);
                            break;
                        case ComponentStatus.Disabled:
                            pool0.BlockDeleteEnable(unblock);
                            pool1.BlockDeleteEnable(unblock);
                            pool2.BlockDeleteEnable(unblock);
                            pool3.BlockDeleteEnable(unblock);
                            pool4.BlockDeleteEnable(unblock);
                            pool5.BlockDeleteEnable(unblock);
                            break;
                        default:
                            pool0.BlockDelete(unblock);
                            pool1.BlockDelete(unblock);
                            pool2.BlockDelete(unblock);
                            pool3.BlockDelete(unblock);
                            pool4.BlockDelete(unblock);
                            pool5.BlockDelete(unblock);
                            break;
                    }
                }

                if (world.QueryDataCount == 0) {
                    world.QueryMode = 0;
                }
                #endif
            }
            #endregion

        }
    }
}