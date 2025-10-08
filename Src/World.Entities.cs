using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [StructLayout(LayoutKind.Explicit)]
    public struct EntitiesChunk {
        [FieldOffset(0)] internal ulong notEmptyBlocks;
        [FieldOffset(0)] private long notEmptyBlocksSigned;
        [FieldOffset(64)] internal ulong[] entities;
        [FieldOffset(72)] internal ulong[] disabledEntities;
        
        [MethodImpl(AggressiveInlining)]
        public void SetNotEmptyBit(byte index) {
            var mask = 1UL << index;
            #if NET6_0_OR_GREATER
            Interlocked.Or(ref notEmptyBlocksSigned, (long) mask);
            #else
            long orig, newVal;
            do {
                orig = notEmptyBlocksSigned;
                newVal = orig | (long) mask;
            } while (Interlocked.CompareExchange(ref notEmptyBlocksSigned, newVal, orig) != orig);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public ulong ClearNotEmptyBit(byte index) {
            var mask = ~(1UL << index);
            #if NET6_0_OR_GREATER
            return (ulong) Interlocked.And(ref notEmptyBlocksSigned, (long) mask);
            #else
            long orig, newVal;
            do {
                orig = notEmptyBlocksSigned;
                newVal = orig & (long) mask;
            } while (Interlocked.CompareExchange(ref notEmptyBlocksSigned, newVal, orig) != orig);

            return (ulong) newVal;
            #endif
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {

        [MethodImpl(AggressiveInlining)]
        public static uint CalculateEntitiesCount() {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: EntitiesCount, World not initialized");
            #endif
            var count = 0;
            var chunks = Entities.Value.chunks;
            for (var i = 0; i < chunks.Length; i++) {
                ref var chunk = ref chunks[i];
                var notEmptyBlocks = chunk.notEmptyBlocks;
                while (notEmptyBlocks > 0) {
                    var blockIdx = Utils.PopLsb(ref notEmptyBlocks);
                    count += chunk.entities[blockIdx].PopCnt();
                }
            }

            return (uint) count;
        }

        [MethodImpl(AggressiveInlining)]
        public static uint CalculateEntitiesCapacity() {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (Status == WorldStatus.NotCreated) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetEntitiesCapacity, World not initialized");
            #endif
            return (uint) (Entities.Value.chunks.Length << Const.ENTITIES_IN_CHUNK_SHIFT);
        }

        [MethodImpl(AggressiveInlining)]
        public static void OnCreateEntity(QueryFunctionWithEntity<WorldType> function) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (Status != WorldStatus.Created) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: OnCreateEntity, World not created or already initialized");
            #endif
            Entities.Value.OnCreateEntity(function);
        }
        

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        public struct Entities {
            public static Entities Value;
            
            internal EntitiesChunk[] chunks;
            internal uint[] deletedEntities;
            internal QueryFunctionWithEntity<WorldType>[] onCreate;
            internal QueryData[] queriesToUpdateOnDisable;
            internal QueryData[] queriesToUpdateOnEnable;
            internal QueryData[] queriesToUpdateOnDestroy;
            internal uint onCreateCount;
            internal int deletedEntitiesCount;
            internal uint entityIdSeq;
            internal int nextActiveChunkIdx;
            internal byte queriesToUpdateOnDestroyCount;
            internal byte queriesToUpdateOnDisableCount;
            internal byte queriesToUpdateOnEnableCount;
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            internal int _blockerDestroy;
            internal int _blockerDisable;
            internal int _blockerEnable;
            #endif
            
            internal void Create(uint baseEntitiesCapacity) {
                chunks = new EntitiesChunk[baseEntitiesCapacity >> Const.ENTITIES_IN_CHUNK_SHIFT];
                onCreate = new QueryFunctionWithEntity<WorldType>[16];
                onCreateCount = 0;
                for (var i = 0; i < chunks.Length; i++) {
                    chunks[i].entities = new ulong[Const.BLOCK_IN_CHUNK];
                    chunks[i].disabledEntities = new ulong[Const.BLOCK_IN_CHUNK];
                }
                deletedEntities = new uint[chunks.Length << Const.ENTITIES_IN_CHUNK_SHIFT];
                nextActiveChunkIdx = 0;
                deletedEntitiesCount = 0;
                entityIdSeq = 0;
                queriesToUpdateOnDisable = new QueryData[Const.MAX_NESTED_QUERY];
                queriesToUpdateOnEnable = new QueryData[Const.MAX_NESTED_QUERY];
                queriesToUpdateOnDestroy = new QueryData[Const.MAX_NESTED_QUERY];
                queriesToUpdateOnDestroyCount = 0;
                queriesToUpdateOnDisableCount = 0;
                queriesToUpdateOnEnableCount = 0;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                _blockerDestroy = 0;
                _blockerDisable = 0;
                _blockerEnable = 0;
                #endif
                GIDStore.Value.Create(baseEntitiesCapacity);
            }

            internal void Initialize(ref GIDStore globalIdStore) {
                GIDStore.Value = globalIdStore;
            }

            internal void Destroy() {
                DestroyAllEntities();
                chunks = null;
                onCreate = null;
                onCreateCount = 0;
                deletedEntities = null;
                queriesToUpdateOnDisable = null;
                queriesToUpdateOnEnable = null;
                queriesToUpdateOnDestroy = null;
                deletedEntitiesCount = 0;
                entityIdSeq = 0;
                nextActiveChunkIdx = 0;
                queriesToUpdateOnDestroyCount = 0;
                queriesToUpdateOnDisableCount = 0;
                queriesToUpdateOnEnableCount = 0;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                _blockerDestroy = 0;
                _blockerDisable = 0;
                _blockerEnable = 0;
                #endif
                GIDStore.Value.Destroy();
            }

            internal void Clear() {
                GIDStore.Value.Clear();
                for (var i = 0; i < chunks.Length; i++) {
                    Array.Clear(chunks[i].entities, 0, chunks[i].entities.Length);
                    Array.Clear(chunks[i].disabledEntities, 0, chunks[i].disabledEntities.Length);
                }
                Array.Clear(deletedEntities, 0, (int) deletedEntitiesCount);
                entityIdSeq = 0;
                nextActiveChunkIdx = 0;
            }

            private void Resize(uint newEntitiesCount) {
                var old = chunks.Length;
                Array.Resize(ref chunks, (int) (newEntitiesCount >> Const.ENTITIES_IN_CHUNK_SHIFT));
                Array.Resize(ref deletedEntities, chunks.Length << Const.ENTITIES_IN_CHUNK_SHIFT);
                for (var i = old; i < chunks.Length; i++) {
                    chunks[i].entities = new ulong[Const.BLOCK_IN_CHUNK];
                    chunks[i].disabledEntities = new ulong[Const.BLOCK_IN_CHUNK];
                }
                GIDStore.Value.ResizeEntities(newEntitiesCount);
                ModuleComponents.Value.Resize(newEntitiesCount);
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.Resize(newEntitiesCount);
                #endif

                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (_debugEventListeners != null) {
                    foreach (var listener in _debugEventListeners) {
                        listener.OnWorldResized(newEntitiesCount);
                    }
                }
                #endif
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void OnCreateEntity(QueryFunctionWithEntity<WorldType> function) {
                if (onCreateCount == onCreate.Length) {
                    Array.Resize(ref onCreate, (int) (onCreateCount << 1));
                }
                onCreate[onCreateCount++] = function;
            }

            [MethodImpl(AggressiveInlining)]
            internal uint CreateEntity() {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CreateEntity, World not initialized");
                if (MultiThreadActive) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CreateEntity, this operation is not supported in multithreaded mode");
                #endif

                var entity = CreateEntityInternal();
                GIDStore.Value.New(entity);

                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (_debugEventListeners != null) {
                    foreach (var listener in _debugEventListeners) {
                        listener.OnEntityCreated(new (entity));
                    }
                }
                #endif
                
                for (var i = 0; i < onCreateCount; i++) {
                    onCreate[i](new (entity));
                }
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            internal uint CreateEntity(EntityGID gid) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CreateEntity, World not initialized");
                if (MultiThreadActive) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CreateEntity, this operation is not supported in multithreaded mode");
                #endif
                var entity = CreateEntityInternal();
                GIDStore.Value.Set(entity, gid);

                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (_debugEventListeners != null) {
                    foreach (var val in _debugEventListeners) val.OnEntityCreated(new (entity));
                }
                #endif
                
                for (var i = 0; i < onCreateCount; i++) {
                    onCreate[i](new (entity));
                }
                return entity;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal uint CreateEntityInternal() {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (MultiThreadActive) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity, Method: New, is blocked, it is forbidden to create new entity in a parallel query (Will be fixed in new versions)");
                #endif
                uint eid;
                if (deletedEntitiesCount > 0) {
                    eid = deletedEntities[(uint)--deletedEntitiesCount];
                } else {
                    eid = entityIdSeq++;
                    if (entityIdSeq > chunks.Length << Const.ENTITIES_IN_CHUNK_SHIFT) {
                        Resize((uint) (chunks.Length << Const.ENTITIES_IN_CHUNK_SHIFT) + Const.ENTITIES_IN_CHUNK);
                    }
                }

                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityIdx = chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK;
                ref var chunk = ref chunks[chunkIdx];
                ref var entitiesMask = ref chunk.entities[blockIdx];
                if (entitiesMask == 0) {
                    if (chunk.notEmptyBlocks == 0 && chunkIdx >= nextActiveChunkIdx) {
                        nextActiveChunkIdx = (int) (chunkIdx + 1);
                    }
                    chunk.notEmptyBlocks |= 1UL << blockIdx;
                }
                entitiesMask |= 1UL << blockEntityIdx;

                return eid;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void DestroyEntity(Entity entity) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity, Method: Destroy, World not initialized");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity, Method: Destroy, Entity already destroyed");
                if (MultiThreadActive && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity, Method: Destroy, is blocked, it is forbidden to modify a non-current entity in a parallel query");
                if (_blockerDestroy > 0 && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity, Method: Destroy, is blocked, use QueryMode.Flexible");
                #endif
                
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.DestroyEntity(entity);
                #endif
                ModuleComponents.Value.DestroyEntity(entity);
                GIDStore.Value.DestroyEntity(entity);

                var eid = entity._id;
                deletedEntities[(uint) Interlocked.Increment(ref deletedEntitiesCount) - 1] = eid;
                
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = (byte) (chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT);
                var blockEntityInvMask = ~(1UL << (chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK));
                
                ref var chunk = ref chunks[chunkIdx];
                ref var mask = ref chunk.entities[blockIdx];
                
                mask &= blockEntityInvMask;
                chunk.disabledEntities[blockIdx] &= blockEntityInvMask;

                if (mask == 0) {
                    if (chunk.ClearNotEmptyBit(blockIdx) == 0) {
                        int oldValue, newValue;
                        do {
                            oldValue = Volatile.Read(ref nextActiveChunkIdx);
                            if (chunkIdx != oldValue - 1) {
                                break;
                            }
                            newValue = oldValue - 1;
                        } while (Interlocked.CompareExchange(ref nextActiveChunkIdx, newValue, oldValue) != oldValue);
                    }
                }
                
                for (uint i = 0; i < queriesToUpdateOnDestroyCount; i++) {
                    queriesToUpdateOnDestroy[i].Update(blockEntityInvMask, eid);
                }

                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (_debugEventListeners != null) {
                    foreach (var listener in _debugEventListeners) {
                        listener.OnEntityDestroyed(entity);
                    }
                }
                #endif
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Write(ref BinaryPackWriter writer) {
                writer.WriteInt(chunks.Length);
                writer.WriteUint(entityIdSeq);
                writer.WriteInt(deletedEntitiesCount);
                writer.WriteInt(nextActiveChunkIdx);
                writer.WriteArrayUnmanaged(deletedEntities, 0, deletedEntitiesCount);
                for (var i = 0; i < nextActiveChunkIdx; i++) {
                    ref var chunk = ref chunks[i];
                    writer.WriteUlong(chunk.notEmptyBlocks);
                    writer.WriteArrayUnmanaged(chunk.entities, 0, Const.BLOCK_IN_CHUNK);
                    writer.WriteArrayUnmanaged(chunk.disabledEntities, 0, Const.BLOCK_IN_CHUNK);
                }
                GIDStore.Value.Write(ref writer);
            }
                
            [MethodImpl(AggressiveInlining)]
            internal void Read(ref BinaryPackReader reader) {
                var chunkCapacity = reader.ReadInt();
                entityIdSeq = reader.ReadUint();
                deletedEntitiesCount = reader.ReadInt();
                nextActiveChunkIdx = reader.ReadInt();

                deletedEntities ??= new uint[chunkCapacity << Const.ENTITIES_IN_CHUNK_SHIFT];
                if (deletedEntities.Length < chunkCapacity << Const.ENTITIES_IN_CHUNK_SHIFT) {
                    Array.Resize(ref deletedEntities, chunkCapacity << Const.ENTITIES_IN_BLOCK_SHIFT);
                }
                reader.ReadArrayUnmanaged(ref deletedEntities);

                chunks ??= new EntitiesChunk[chunkCapacity];
                if (chunks.Length < chunkCapacity) {
                    Array.Resize(ref chunks, chunkCapacity);
                }

                for (uint i = 0; i < nextActiveChunkIdx; i++) {
                    ref var chunk = ref chunks[i];
                    chunk.notEmptyBlocks =  reader.ReadUlong();
                    reader.ReadArrayUnmanaged(ref chunk.entities);
                    reader.ReadArrayUnmanaged(ref chunk.disabledEntities);
                }

                GIDStore.Value.Read(ref reader);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void IncQDestroy(QueryData qData) {
                queriesToUpdateOnDestroy[queriesToUpdateOnDestroyCount++] = qData;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void DecQDestroy() {
                queriesToUpdateOnDestroy[--queriesToUpdateOnDestroyCount] = default;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void IncQDisable(QueryData qData) {
                queriesToUpdateOnDisable[queriesToUpdateOnDisableCount++] = qData;
            }

            internal void DecQDisable() {
                queriesToUpdateOnDisable[--queriesToUpdateOnDisableCount] = default;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void IncQEnable(QueryData qData) {
                queriesToUpdateOnEnable[queriesToUpdateOnEnableCount++] = qData;
            }

            [MethodImpl(AggressiveInlining)]
            internal void DecQEnable() {
                queriesToUpdateOnEnable[--queriesToUpdateOnEnableCount] = default;
            }
            
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            [MethodImpl(AggressiveInlining)]
            internal void BlockDestroy(int val) {
                _blockerDisable += val;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void BlockDisable(int val) {
                _blockerDisable += val;
            }

            [MethodImpl(AggressiveInlining)]
            internal void BlockEnable(int val) {
                _blockerEnable += val;
            }
            #endif
        }
    }
}