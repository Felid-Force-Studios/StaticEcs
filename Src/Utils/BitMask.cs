#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
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
    internal class BitMask {
        internal ulong[][] chunks;
        internal ulong[][] freeChunks;
        internal int freeChunksCount;
        internal ushort maskLen;

        public ushort MaskLen {
            [MethodImpl(AggressiveInlining)] get => maskLen;
        }

        [MethodImpl(AggressiveInlining)]
        internal void Create(uint chunksCapacity, ushort maskLen) {
            this.maskLen = maskLen;
            freeChunksCount = 0;
            freeChunks = new ulong[chunksCapacity][];
            chunks = new ulong[chunksCapacity][];
        }

        [MethodImpl(AggressiveInlining)]
        internal void Destroy() {
            chunks = null;
            freeChunks = null;
            maskLen = 0;
            freeChunksCount = 0;
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void ResizeBitMap(uint chunksCapacity) {
            Array.Resize(ref freeChunks, (int) chunksCapacity);
            Array.Resize(ref chunks, (int) chunksCapacity);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void InitChunk(uint chunkIdx) {
            if (freeChunksCount > 0) {
                ref var freeChunk = ref freeChunks[--freeChunksCount];
                chunks[chunkIdx] = freeChunk;
                freeChunk = null;
            } else {
                chunks[chunkIdx] = new ulong[Const.ENTITIES_IN_CHUNK * maskLen];
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void FreeChunk(uint chunkIdx) {
            ref var chunk = ref chunks[chunkIdx];
            freeChunks[Interlocked.Increment(ref freeChunksCount) - 1] = chunk;
            chunk = null;
        }
        
        [MethodImpl(AggressiveInlining)]
        public ulong[] Chunk(uint entityId) {
            return chunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT];
        }
        
        [MethodImpl(AggressiveInlining)]
        public ref ulong Ref(uint entityId, ushort index) {
            return ref chunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT][(entityId & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + index];
        }

        [MethodImpl(AggressiveInlining)]
        public void Set(uint entityId, ushort poolId) {
            var div = (ushort) (poolId >> Const.LONG_SHIFT);
            var rem = (ushort) (poolId & Const.LONG_OFFSET_MASK);
            var index = (entityId & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + div;
            chunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT][index] |= 1UL << rem;
        }

        [MethodImpl(AggressiveInlining)]
        public void Del(uint entityId, ushort poolId) {
            var div = (ushort) (poolId >> Const.LONG_SHIFT);
            var rem = (ushort) (poolId & Const.LONG_OFFSET_MASK);
            var index = (entityId & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + div;
            chunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT][index] &= ~(1UL << rem);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DelInChunk(uint chunkIdx, ushort poolId) {
            var div = (ushort) (poolId >> Const.LONG_SHIFT);
            var rem = (ushort) (poolId & Const.LONG_OFFSET_MASK);

            var chunk = chunks[chunkIdx];

            for (var i = 0; i < Const.ENTITIES_IN_CHUNK; i++) {
                var index = i * maskLen + div;
                chunk[index] &= ~(1UL << rem);
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void MigrateChunk(uint chunkIdx, ushort poolIdFrom, ushort poolIdTo, ulong[] srcBitMap) {
            var divFrom = (ushort) (poolIdFrom >> Const.LONG_SHIFT);
            var remFrom = (ushort) (poolIdFrom & Const.LONG_OFFSET_MASK);
            var divTo = (ushort) (poolIdTo >> Const.LONG_SHIFT);
            var remTo = (ushort) (poolIdTo & Const.LONG_OFFSET_MASK);

            var chunk = chunks[chunkIdx];

            for (var i = 0; i < Const.ENTITIES_IN_CHUNK; i++) {
                var indexFrom = i * maskLen + divFrom;
                var indexTo = i * maskLen + divTo;
                ref var from = ref srcBitMap[indexFrom];
                if ((from & (1UL << remFrom)) != 0UL) {
                    chunk[indexTo] |= 1UL << remTo;
                }
                
                from &= ~(1UL << remFrom);
            }
        }

        [MethodImpl(AggressiveInlining)]
        public bool Has(uint entityId, ushort poolId) {
            var div = (ushort) (poolId >> Const.LONG_SHIFT);
            var rem = (ushort) (poolId & Const.LONG_OFFSET_MASK);
            var index = (entityId & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + div;
            return (chunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT][index] & (1UL << rem)) != 0UL;
        }

        [MethodImpl(AggressiveInlining)]
        public bool IsEmpty(uint entityId) {
            var offset = (entityId & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen;
            var endOffset = offset + maskLen;
            var data = chunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT];
            for (; offset < endOffset; offset++) {
                if (data[offset] != 0UL) {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(AggressiveInlining)]
        public ushort Len(uint entityId) {
            ushort count = 0;
            var offset = (entityId & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen;
            var endOffset = offset + maskLen;
            var data = chunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT];
            for (; offset < endOffset; offset++) {
                count += (ushort) data[offset].PopCnt();
            }

            return count;
        }

        [MethodImpl(AggressiveInlining)]
        public void ClearChunk(uint chunkIdx) {
            if (chunks[chunkIdx] != null) {
                Array.Clear(chunks[chunkIdx], 0, Const.ENTITIES_IN_CHUNK);
            }
        }

        [MethodImpl(AggressiveInlining)]
        public ulong[] CopyBitMapChunkToArrayPool(uint chunkIdx) {
            var res = ArrayPool<ulong>.Shared.Rent(Const.ENTITIES_IN_CHUNK);
            Array.Copy(chunks[chunkIdx], res, Const.ENTITIES_IN_CHUNK);
            return res;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void WriteChunk(ref BinaryPackWriter writer, uint chunkIdx) {
            writer.WriteArrayUnmanaged(chunks[chunkIdx]);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void ReadChunk(ref BinaryPackReader reader, uint chunkIdx) {
            reader.ReadArrayUnmanaged(ref chunks[chunkIdx]);
        }
    }
}