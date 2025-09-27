using System;
using System.Buffers;
using System.Runtime.CompilerServices;
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
        internal ushort maskLen;

        public ushort MaskLen {
            [MethodImpl(AggressiveInlining)] get => maskLen;
        }

        [MethodImpl(AggressiveInlining)]
        internal void Create(uint entitiesCapacity, ushort maskLen) {
            this.maskLen = maskLen;
            chunks = new ulong[entitiesCapacity >> Const.ENTITIES_IN_CHUNK_SHIFT][];
            for (var i = 0; i < chunks.Length; i++) {
                chunks[i] = new ulong[Const.ENTITIES_IN_CHUNK * maskLen];
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal void Destroy() {
            chunks = null;
            maskLen = 0;
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void ResizeBitMap(uint entitiesCapacity) {
            entitiesCapacity >>= Const.ENTITIES_IN_CHUNK_SHIFT;

            if (chunks.Length < entitiesCapacity) {
                var old = chunks.Length;
                Array.Resize(ref chunks, (int) entitiesCapacity);
                for (var i = old; i < chunks.Length; i++) {
                    chunks[i] = new ulong[Const.ENTITIES_IN_CHUNK * maskLen];
                }
            }
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
        public void DelRange(uint entityIdFrom, uint entityIdTo, ushort poolId) {
            var div = (ushort) (poolId >> Const.LONG_SHIFT);
            var rem = (ushort) (poolId & Const.LONG_OFFSET_MASK);

            for (; entityIdFrom < entityIdTo; entityIdFrom++) {
                var index = (entityIdFrom & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + div;
                chunks[entityIdFrom >> Const.ENTITIES_IN_CHUNK_SHIFT][index] &= ~(1UL << rem);
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Migrate(uint entityIdFrom, uint entityIdTo, ushort poolIdFrom, ushort poolIdTo, ulong[][] srcBitMap) {
            var divFrom = (ushort) (poolIdFrom >> Const.LONG_SHIFT);
            var remFrom = (ushort) (poolIdFrom & Const.LONG_OFFSET_MASK);
            var divTo = (ushort) (poolIdTo >> Const.LONG_SHIFT);
            var remTo = (ushort) (poolIdTo & Const.LONG_OFFSET_MASK);
            
            for (; entityIdFrom < entityIdTo; entityIdFrom++) {
                var indexFrom = (entityIdFrom & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + divFrom;
                var indexTo = (entityIdFrom & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + divTo;
                var i = entityIdFrom >> Const.ENTITIES_IN_CHUNK_SHIFT;
                ref var from = ref srcBitMap[i][indexFrom];
                if ((from & (1UL << remFrom)) != 0UL) {
                    chunks[i][indexTo] |= 1UL << remTo;
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
        public void Clear(int nextChunk) {
            for (var i = 0; i < nextChunk; i++) {
                Array.Clear(chunks[i], 0, Const.ENTITIES_IN_CHUNK);
            }
        }

        [MethodImpl(AggressiveInlining)]
        public ulong[][] CopyBitMapToArrayPool() {
            var res = ArrayPool<ulong[]>.Shared.Rent(chunks.Length);
            
            for (var i = 0; i < chunks.Length; i++) {
                var val = ArrayPool<ulong>.Shared.Rent(Const.ENTITIES_IN_CHUNK);
                Array.Copy(chunks[i], val, Const.ENTITIES_IN_CHUNK);
                res[i] = val;
            }

            return res;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Write(ref BinaryPackWriter writer, int nextActiveChunkIdx, bool empty) {
            writer.WriteUshort(maskLen);
            writer.WriteInt(chunks.Length);
            writer.WriteInt(nextActiveChunkIdx);
            writer.WriteBool(empty);
            if (empty) {
                return;
            }
            
            for (var i = 0; i < nextActiveChunkIdx; i++) {
                writer.WriteArrayUnmanaged(chunks[i]);
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Read(ref BinaryPackReader reader) {
            maskLen = reader.ReadUshort();
            var capacity = reader.ReadInt();
            var count = reader.ReadInt();
            chunks ??= new ulong[capacity][];
            if (chunks.Length < capacity) {
                Array.Resize(ref chunks, capacity);
            }
            var empty = reader.ReadBool();
            if (empty) {
                return;
            }

            for (var i = 0; i < count; i++) {
                reader.ReadArrayUnmanaged(ref chunks[i]);
            }
        }
    }
}