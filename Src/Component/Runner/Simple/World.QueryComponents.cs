#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    public enum QueryMode {
        Default,
        Strict,
        Flexible
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct QueryData {
        public BlockMaskCache[] Blocks;
        public OnCacheUpdate OnCacheUpdate;
        
        [MethodImpl(AggressiveInlining)]
        internal void Update(ulong mask, uint eid) {
            if (OnCacheUpdate == null) {
                Blocks[eid >> Const.ENTITIES_IN_BLOCK_SHIFT].EntitiesMask &= mask;
            } else {
                OnCacheUpdate(Blocks,
                              eid >> Const.ENTITIES_IN_CHUNK_SHIFT,
                              (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) >> Const.ENTITIES_IN_BLOCK_SHIFT,
                              (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
            }
        }
    }

    public delegate void OnCacheUpdate(BlockMaskCache[] cache, uint chunkIdx, int blockIdx, int blockEntityIdx);

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        internal struct CurrentQuery {
            internal static uint QueryDataCount;

            #if FFS_ECS_DEBUG
            internal static byte QueryMode; // 0 - None, 1 - Strict, 2 - Flexible
            private static uint[] CurrentEntitiesMainThread;
            
            [ThreadStatic]
            private static uint[] CurrentEntitiesOtherThread;
            
            [MethodImpl(AggressiveInlining)]
            internal static void SetCurrentEntity(uint entity) {
                if (MultiThreadActive) {
                    CurrentEntitiesOtherThread ??= new uint[Environment.ProcessorCount * Const.MAX_NESTED_QUERY + 1];
                    CurrentEntitiesOtherThread[QueryDataCount - 1] = entity;
                } else {
                    CurrentEntitiesMainThread ??= new uint[Const.MAX_NESTED_QUERY + 1];
                    CurrentEntitiesMainThread[QueryDataCount - 1] = entity;
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal static bool IsNotCurrentEntity(Entity entity) {
                for (var i = 0; i < QueryDataCount; i++) {
                    if (MultiThreadActive) {
                        if (CurrentEntitiesOtherThread[i] != entity.id) return true;
                    } else {
                        if (CurrentEntitiesMainThread[i] != entity.id) return true;
                    }
                }

                return false;
            }
            #endif
            
            [MethodImpl(AggressiveInlining)]
            internal static QueryData RegisterQuery() {
                #if FFS_ECS_DEBUG
                if (QueryDataCount == Const.MAX_NESTED_QUERY) throw new StaticEcsException($"The maximum number of nested Query is {Const.MAX_NESTED_QUERY - 1}");
                #endif

                QueryDataCount++;
                var data = new QueryData {
                    Blocks = ArrayPool<BlockMaskCache>.Shared.Rent(Entities.Value.chunks.Length << Const.BLOCK_IN_CHUNK_SHIFT),
                };
                return data;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal static void UnregisterQuery(QueryData data) {
                #if FFS_ECS_DEBUG
                if (QueryDataCount == 0) throw new StaticEcsException("Unexpected error");
                #endif
                ArrayPool<BlockMaskCache>.Shared.Return(data.Blocks);
                QueryDataCount--;
            }
        }

        [MethodImpl(AggressiveInlining)]
        public static ReadOnlySpan<ushort> HandleClustersRange(ReadOnlySpan<ushort> clusters) {
            return clusters.Length == 0
                ? new ReadOnlySpan<ushort>(Entities.Value.activeClusters, 0, Entities.Value.activeClustersCount)
                : clusters;
        }
        
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public readonly ref struct WithComponents<W> where W : struct, IQueryMethod {
            private readonly W With;
        
            [MethodImpl(AggressiveInlining)]
            public WithComponents(W with) {
                With = with;
            }
        
            #region BY_RUNNER
            [MethodImpl(AggressiveInlining)]
            public void For<R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where R : struct, IQueryFunction {
                QueryFunctionRunner<WorldType, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where R : struct, IQueryFunction {
                QueryFunctionRunner<WorldType, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void For<C1, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where R : struct, IQueryFunction<C1> {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where R : struct, IQueryFunction<C1> {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2> {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2> {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3> {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3> {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, C8, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, C8, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(HandleClustersRange(clusters), ref runner, With, entities, components, queryMode);
            }
            #endregion
        
            #region QUERY_FUNCTION
            [MethodImpl(AggressiveInlining)]
            public void For<C1>(QueryFunction<C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2>(QueryFunction<C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3>(QueryFunction<C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4>(QueryFunction<C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5>(QueryFunction<C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6>(QueryFunction<C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7>(QueryFunction<C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, C8>(QueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
            #endregion
        
            #region QUERY_FUNCTION_WITH_DATA
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(D data, QueryFunctionWithRefData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(D data, QueryFunctionWithRefData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(D data, QueryFunctionWithRefData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(D data, QueryFunctionWithRefData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
            #endregion
        
            #region QUERY_FUNCTION_WITH_REF_DATA
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(ref D data, QueryFunctionWithRefData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(ref D data, QueryFunctionWithRefData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
            #endregion
        
            #region QUERY_FUNCTION_WITH_ENTITY
            [MethodImpl(AggressiveInlining)]
            public void For(QueryFunctionWithEntity<WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default) {
                QueryFunctionRunner<WorldType, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void For<C1>(QueryFunctionWithEntity<WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2>(QueryFunctionWithEntity<WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3>(QueryFunctionWithEntity<WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<C1, C2, C3, C4, C5, C6, C7, C8>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(HandleClustersRange(clusters), function, With, entities, components, queryMode);
            }
            #endregion
        
            #region QUERY_FUNCTION_WITH_DATA_ENTITY
            [MethodImpl(AggressiveInlining)]
            public void For<D>(D data, QueryFunctionWithRefDataEntity<D, WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default) {
                QueryFunctionRunner<WorldType, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
            #endregion
        
            #region QUERY_FUNCTION_WITH_DATA_ENTITY
            [MethodImpl(AggressiveInlining)]
            public void For<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct {
                QueryFunctionRunner<WorldType, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
        
            [MethodImpl(AggressiveInlining)]
            public void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Run(HandleClustersRange(clusters), ref data, function, With, entities, components, queryMode);
            }
            #endregion
            
            #region SEARCH_FUNCTION_WITH_ENTITY
            [MethodImpl(AggressiveInlining)]
            public bool Search(out Entity entity, SearchFunctionWithEntity<WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default) {
                return QueryFunctionRunner<WorldType, W>.Value.Search(HandleClustersRange(clusters), function, With, entities, queryMode, out entity);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool Search<C1>(out Entity entity, SearchFunctionWithEntity<WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, W>.Value.Search(HandleClustersRange(clusters), function, With, entities, components, queryMode, out entity);
            }
        
            [MethodImpl(AggressiveInlining)]
            public bool Search<C1, C2>(out Entity entity, SearchFunctionWithEntity<WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, C2, W>.Value.Search(HandleClustersRange(clusters), function, With, entities, components, queryMode, out entity);
            }
        
            [MethodImpl(AggressiveInlining)]
            public bool Search<C1, C2, C3>(out Entity entity, SearchFunctionWithEntity<WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, C2, C3, W>.Value.Search(HandleClustersRange(clusters), function, With, entities, components, queryMode, out entity);
            }
        
            [MethodImpl(AggressiveInlining)]
            public bool Search<C1, C2, C3, C4>(out Entity entity, SearchFunctionWithEntity<WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, C2, C3, C4, W>.Value.Search(HandleClustersRange(clusters), function, With, entities, components, queryMode, out entity);
            }
        
            [MethodImpl(AggressiveInlining)]
            public bool Search<C1, C2, C3, C4, C5>(out Entity entity, SearchFunctionWithEntity<WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, W>.Value.Search(HandleClustersRange(clusters), function, With, entities, components, queryMode, out entity);
            }
        
            [MethodImpl(AggressiveInlining)]
            public bool Search<C1, C2, C3, C4, C5, C6>(out Entity entity, SearchFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, W>.Value.Search(HandleClustersRange(clusters), function, With, entities, components, queryMode, out entity);
            }
        
            [MethodImpl(AggressiveInlining)]
            public bool Search<C1, C2, C3, C4, C5, C6, C7>(out Entity entity, SearchFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, W>.Value.Search(HandleClustersRange(clusters), function, With, entities, components, queryMode, out entity);
            }
        
            [MethodImpl(AggressiveInlining)]
            public bool Search<C1, C2, C3, C4, C5, C6, C7, C8>(out Entity entity, SearchFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, W>.Value.Search(HandleClustersRange(clusters), function, With, entities, components, queryMode, out entity);
            }
            #endregion
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public partial struct Query {
            
            [MethodImpl(AggressiveInlining)]
            public static WithComponents<W> With<W>(W with = default)
                where W : struct, IQueryMethod {
                return new (with);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static WithComponents<With<W1, W2>> With<W1, W2>(W1 with1 = default, W2 with2 = default)
                where W1 : struct, IQueryMethod
                where W2 : struct, IQueryMethod {
                return new (new (with1, with2));
            }
            
            [MethodImpl(AggressiveInlining)]
            public static WithComponents<With<W1, W2, W3>> With<W1, W2, W3>(W1 with1 = default, W2 with2 = default, W3 with3 = default)
                where W1 : struct, IQueryMethod
                where W2 : struct, IQueryMethod
                where W3 : struct, IQueryMethod {
                return new (new (with1, with2, with3));
            }
            
            [MethodImpl(AggressiveInlining)]
            public static WithComponents<With<W1, W2, W3, W4>> With<W1, W2, W3, W4>(W1 with1 = default, W2 with2 = default, W3 with3 = default, W4 with4 = default)
                where W1 : struct, IQueryMethod
                where W2 : struct, IQueryMethod
                where W3 : struct, IQueryMethod
                where W4 : struct, IQueryMethod {
                return new (new (with1, with2, with3, with4));
            }

            #region BY_STRUCT_FUNCTION
            [MethodImpl(AggressiveInlining)]
            public static void For<R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where R : struct, IQueryFunction {
                QueryFunctionRunner<WorldType, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where R : struct, IQueryFunction {
                QueryFunctionRunner<WorldType, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where R : struct, IQueryFunction<C1> {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where R : struct, IQueryFunction<C1> {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2> {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2> {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3> {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3> {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7, C8, R>(R runner = default, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7, C8, R>(ref R runner, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent
                where R : struct, IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(HandleClustersRange(clusters), ref runner, default, entities, components, queryMode);
            }
            #endregion
            
            #region QUERY_FUNCTION
            [MethodImpl(AggressiveInlining)]
            public static void For<C1>(QueryFunction<C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2>(QueryFunction<C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3>(QueryFunction<C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4>(QueryFunction<C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5>(QueryFunction<C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6>(QueryFunction<C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7>(QueryFunction<C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7, C8>(QueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            #endregion
            
            #region QUERY_FUNCTION_WITH_DATA
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1>(D data, QueryFunctionWithRefData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2>(D data, QueryFunctionWithRefData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3>(D data, QueryFunctionWithRefData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4>(D data, QueryFunctionWithRefData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5>(D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6>(D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7>(D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_REF_DATA
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1>(ref D data, QueryFunctionWithRefData<D, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2>(ref D data, QueryFunctionWithRefData<D, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            #endregion
            
            #region QUERY_FUNCTION_WITH_ENTITY
            [MethodImpl(AggressiveInlining)]
            internal static void For(ReadOnlySpan<uint> chunks, QueryFunctionWithEntity<WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, QueryMode queryMode = QueryMode.Default) {
                QueryFunctionRunner<WorldType, WithNothing>.Value.Run(chunks, function, default, entities, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal static void For<D>(ReadOnlySpan<uint> chunks, ref D data, QueryFunctionWithRefDataEntity<D, WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, QueryMode queryMode = QueryMode.Default) {
                QueryFunctionRunner<WorldType, WithNothing>.Value.Run(chunks, ref data, function, default, entities, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For(QueryFunctionWithEntity<WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default) {
                QueryFunctionRunner<WorldType, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1>(QueryFunctionWithEntity<WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2>(QueryFunctionWithEntity<WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3>(QueryFunctionWithEntity<WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<C1, C2, C3, C4, C5, C6, C7, C8>(QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(HandleClustersRange(clusters), function, default, entities, components, queryMode);
            }
            #endregion
            
            #region QUERY_FUNCTION_WITH_DATA_ENTITY
            [MethodImpl(AggressiveInlining)]
            public static void For<D>(D data, QueryFunctionWithRefDataEntity<D, WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default) {
                QueryFunctionRunner<WorldType, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            #endregion

            #region QUERY_FUNCTION_WITH_DATA_ENTITY
            [MethodImpl(AggressiveInlining)]
            public static void For<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct {
                QueryFunctionRunner<WorldType, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }

            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void For<D, C1, C2, C3, C4, C5, C6, C7, C8>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where D : struct
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Run(HandleClustersRange(clusters), ref data, function, default, entities, components, queryMode);
            }
            #endregion
            
            #region SEARCH_FUNCTION_WITH_ENTITY
            [MethodImpl(AggressiveInlining)]
            public static bool Search(out Entity entity, SearchFunctionWithEntity<WorldType> function, EntityStatusType entities = EntityStatusType.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default) {
                return QueryFunctionRunner<WorldType, WithNothing>.Value.Search(HandleClustersRange(clusters), function, default, entities, queryMode, out entity);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static bool Search<C1>(out Entity entity, SearchFunctionWithEntity<WorldType, C1> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, WithNothing>.Value.Search(HandleClustersRange(clusters), function, default, entities, components, queryMode, out entity);
            }
        
            [MethodImpl(AggressiveInlining)]
            public static bool Search<C1, C2>(out Entity entity, SearchFunctionWithEntity<WorldType, C1, C2> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, C2, WithNothing>.Value.Search(HandleClustersRange(clusters), function, default, entities, components, queryMode, out entity);
            }
        
            [MethodImpl(AggressiveInlining)]
            public static bool Search<C1, C2, C3>(out Entity entity, SearchFunctionWithEntity<WorldType, C1, C2, C3> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, C2, C3, WithNothing>.Value.Search(HandleClustersRange(clusters), function, default, entities, components, queryMode, out entity);
            }
        
            [MethodImpl(AggressiveInlining)]
            public static bool Search<C1, C2, C3, C4>(out Entity entity, SearchFunctionWithEntity<WorldType, C1, C2, C3, C4> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, C2, C3, C4, WithNothing>.Value.Search(HandleClustersRange(clusters), function, default, entities, components, queryMode, out entity);
            }
        
            [MethodImpl(AggressiveInlining)]
            public static bool Search<C1, C2, C3, C4, C5>(out Entity entity, SearchFunctionWithEntity<WorldType, C1, C2, C3, C4, C5> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, WithNothing>.Value.Search(HandleClustersRange(clusters), function, default, entities, components, queryMode, out entity);
            }
        
            [MethodImpl(AggressiveInlining)]
            public static bool Search<C1, C2, C3, C4, C5, C6>(out Entity entity, SearchFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, WithNothing>.Value.Search(HandleClustersRange(clusters), function, default, entities, components, queryMode, out entity);
            }
        
            [MethodImpl(AggressiveInlining)]
            public static bool Search<C1, C2, C3, C4, C5, C6, C7>(out Entity entity, SearchFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, WithNothing>.Value.Search(HandleClustersRange(clusters), function, default, entities, components, queryMode, out entity);
            }
        
            [MethodImpl(AggressiveInlining)]
            public static bool Search<C1, C2, C3, C4, C5, C6, C7, C8>(out Entity entity, SearchFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, EntityStatusType entities = EntityStatusType.Enabled, ComponentStatus components = ComponentStatus.Enabled, QueryMode queryMode = QueryMode.Default, ReadOnlySpan<ushort> clusters = default)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                return QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, WithNothing>.Value.Search(HandleClustersRange(clusters), function, default, entities, components, queryMode, out entity);
            }
            #endregion
        }
    }
}