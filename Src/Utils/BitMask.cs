using System;
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
    internal class BitMask {
        private static readonly ushort[] BitsLut = {
            0, 1, 17, 2, 18, 50, 3, 57, 47, 19, 22, 51, 29, 4, 33, 58,
            15, 48, 20, 27, 25, 23, 52, 41, 54, 30, 38, 5, 43, 34, 59, 8,
            63, 16, 49, 56, 46, 21, 28, 32, 14, 26, 24, 40, 53, 37, 42, 7,
            62, 55, 45, 31, 13, 39, 36, 6, 61, 44, 12, 35, 60, 11, 10, 9,
        };

        struct IndexedMask {
            public ulong Mask;
            public byte Index;
        }
        
        private ulong[] _bitMap;
        private ulong[] _bitMapDisabled;
        private ushort _maskLen;
        
        private ulong[] _tempBuffer;
        private byte _tempBufferFreeCount;
        private byte _tempBufferBorrowed;
        private byte _tempBufferCount;
        
        private IndexedMask[] _indexedBuffer;
        private byte _indexedBufferCount;

        [MethodImpl(AggressiveInlining)]
        internal void Create(uint worldCapacity, byte tempBuffersCount, ushort maskLen, bool bitMaskDisabled) {
            _maskLen = maskLen;
            _tempBufferCount = tempBuffersCount;
            _bitMap = new ulong[worldCapacity * _maskLen];
            if (bitMaskDisabled) {
                _bitMapDisabled = new ulong[worldCapacity * _maskLen];
            }
            _tempBuffer = new ulong[_tempBufferCount * _maskLen];
            _tempBufferFreeCount = _tempBufferCount;
            _tempBufferBorrowed = 0;
            _indexedBufferCount = 0;
            _indexedBuffer = new IndexedMask[32 * _maskLen];
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void ResizeBitMap(uint size) {
            Array.Resize(ref _bitMap, (int) (size * _maskLen));
            if (_bitMapDisabled != null) {
                Array.Resize(ref _bitMapDisabled, (int) (size * _maskLen));
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal byte BorrowBuf() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (_tempBufferFreeCount == 0) throw new Exception("Bitmask buffer is empty");
            #endif
            
            var maskBufId = --_tempBufferFreeCount;
            var offset = maskBufId * _maskLen;
            for (var i = 0; i < _maskLen; i++) {
                _tempBuffer[offset + i] = 0UL;
            }

            _tempBufferBorrowed++;
            return maskBufId;
        }

        [MethodImpl(AggressiveInlining)]
        internal void DropBuf() {
            _tempBufferBorrowed--;
            if (_tempBufferBorrowed == 0) {
                _tempBufferFreeCount = _tempBufferCount;
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal void Destroy() {
            _tempBuffer = null;
            _bitMap = null;
            _indexedBuffer = null;
            _bitMapDisabled = null;
            _maskLen = 0;
            _tempBufferCount = 0;
            _tempBufferBorrowed = 0;
            _tempBufferFreeCount = 0;
            _indexedBufferCount = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Set(uint bitMapIdx, ushort bitPos) {
            var div = (ushort) (bitPos >> 6);
            var rem = (ushort) (bitPos & 63);
            var index = bitMapIdx * _maskLen + div;
            _bitMap[index] |= 1UL << rem;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetInBuffer(byte bufId, ushort bitPos) {
            var div = (ushort) (bitPos >> 6);
            var rem = (ushort) (bitPos & 63);
            var index = bufId * _maskLen + div;
            _tempBuffer[index] |= 1UL << rem;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetInBuffer(byte bufId, ushort bit1, ushort bit2) {
            var offset = bufId * _maskLen;
            _tempBuffer[offset + (ushort) (bit1 >> 6)] |= 1UL << (ushort) (bit1 & 63);
            _tempBuffer[offset + (ushort) (bit2 >> 6)] |= 1UL << (ushort) (bit2 & 63);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetInBuffer(byte bufId, ushort bit1, ushort bit2, ushort bit3) {
            var offset = bufId * _maskLen;
            _tempBuffer[offset + (ushort) (bit1 >> 6)] |= 1UL << (ushort) (bit1 & 63);
            _tempBuffer[offset + (ushort) (bit2 >> 6)] |= 1UL << (ushort) (bit2 & 63);
            _tempBuffer[offset + (ushort) (bit3 >> 6)] |= 1UL << (ushort) (bit3 & 63);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetInBuffer(byte bufId, ushort bit1, ushort bit2, ushort bit3, ushort bit4) {
            var offset = bufId * _maskLen;
            _tempBuffer[offset + (ushort) (bit1 >> 6)] |= 1UL << (ushort) (bit1 & 63);
            _tempBuffer[offset + (ushort) (bit2 >> 6)] |= 1UL << (ushort) (bit2 & 63);
            _tempBuffer[offset + (ushort) (bit3 >> 6)] |= 1UL << (ushort) (bit3 & 63);
            _tempBuffer[offset + (ushort) (bit4 >> 6)] |= 1UL << (ushort) (bit4 & 63);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetInBuffer(byte bufId, ushort bit1, ushort bit2, ushort bit3, ushort bit4, ushort bit5) {
            var offset = bufId * _maskLen;
            _tempBuffer[offset + (ushort) (bit1 >> 6)] |= 1UL << (ushort) (bit1 & 63);
            _tempBuffer[offset + (ushort) (bit2 >> 6)] |= 1UL << (ushort) (bit2 & 63);
            _tempBuffer[offset + (ushort) (bit3 >> 6)] |= 1UL << (ushort) (bit3 & 63);
            _tempBuffer[offset + (ushort) (bit4 >> 6)] |= 1UL << (ushort) (bit4 & 63);
            _tempBuffer[offset + (ushort) (bit5 >> 6)] |= 1UL << (ushort) (bit5 & 63);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetInBuffer(byte bufId, ushort bit1, ushort bit2, ushort bit3, ushort bit4, ushort bit5, ushort bit6) {
            var offset = bufId * _maskLen;
            _tempBuffer[offset + (ushort) (bit1 >> 6)] |= 1UL << (ushort) (bit1 & 63);
            _tempBuffer[offset + (ushort) (bit2 >> 6)] |= 1UL << (ushort) (bit2 & 63);
            _tempBuffer[offset + (ushort) (bit3 >> 6)] |= 1UL << (ushort) (bit3 & 63);
            _tempBuffer[offset + (ushort) (bit4 >> 6)] |= 1UL << (ushort) (bit4 & 63);
            _tempBuffer[offset + (ushort) (bit5 >> 6)] |= 1UL << (ushort) (bit5 & 63);
            _tempBuffer[offset + (ushort) (bit6 >> 6)] |= 1UL << (ushort) (bit6 & 63);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetInBuffer(byte bufId, ushort bit1, ushort bit2, ushort bit3, ushort bit4, ushort bit5, ushort bit6, ushort bit7) {
            var offset = bufId * _maskLen;
            _tempBuffer[offset + (ushort) (bit1 >> 6)] |= 1UL << (ushort) (bit1 & 63);
            _tempBuffer[offset + (ushort) (bit2 >> 6)] |= 1UL << (ushort) (bit2 & 63);
            _tempBuffer[offset + (ushort) (bit3 >> 6)] |= 1UL << (ushort) (bit3 & 63);
            _tempBuffer[offset + (ushort) (bit4 >> 6)] |= 1UL << (ushort) (bit4 & 63);
            _tempBuffer[offset + (ushort) (bit5 >> 6)] |= 1UL << (ushort) (bit5 & 63);
            _tempBuffer[offset + (ushort) (bit6 >> 6)] |= 1UL << (ushort) (bit6 & 63);
            _tempBuffer[offset + (ushort) (bit7 >> 6)] |= 1UL << (ushort) (bit7 & 63);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetInBuffer(byte bufId, ushort bit1, ushort bit2, ushort bit3, ushort bit4, ushort bit5, ushort bit6, ushort bit7, ushort bit8) {
            var offset = bufId * _maskLen;
            _tempBuffer[offset + (ushort) (bit1 >> 6)] |= 1UL << (ushort) (bit1 & 63);
            _tempBuffer[offset + (ushort) (bit2 >> 6)] |= 1UL << (ushort) (bit2 & 63);
            _tempBuffer[offset + (ushort) (bit3 >> 6)] |= 1UL << (ushort) (bit3 & 63);
            _tempBuffer[offset + (ushort) (bit4 >> 6)] |= 1UL << (ushort) (bit4 & 63);
            _tempBuffer[offset + (ushort) (bit5 >> 6)] |= 1UL << (ushort) (bit5 & 63);
            _tempBuffer[offset + (ushort) (bit6 >> 6)] |= 1UL << (ushort) (bit6 & 63);
            _tempBuffer[offset + (ushort) (bit7 >> 6)] |= 1UL << (ushort) (bit7 & 63);
            _tempBuffer[offset + (ushort) (bit8 >> 6)] |= 1UL << (ushort) (bit8 & 63);
        }

        [MethodImpl(AggressiveInlining)]
        public void Del(uint bitMapIdx, ushort bitPos) {
            var div = (ushort) (bitPos >> 6);
            var rem = (ushort) (bitPos & 63);
            var index = bitMapIdx * _maskLen + div;
            _bitMap[index] &= ~(1UL << rem);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DelWithDisabled(uint bitMapIdx, ushort bitPos) {
            var div = (ushort) (bitPos >> 6);
            var rem = (ushort) (bitPos & 63);
            var index = bitMapIdx * _maskLen + div;
            _bitMap[index] &= ~(1UL << rem);
            _bitMapDisabled[index] &= ~(1UL << rem);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void MoveToDisabled(uint bitMapIdx, ushort bitPos) {
            var div = (ushort) (bitPos >> 6);
            var rem = (ushort) (bitPos & 63);
            var index = bitMapIdx * _maskLen + div;
            _bitMap[index] &= ~(1UL << rem);
            _bitMapDisabled[index] |= 1UL << rem;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void MoveToEnabled(uint bitMapIdx, ushort bitPos) {
            var div = (ushort) (bitPos >> 6);
            var rem = (ushort) (bitPos & 63);
            var index = bitMapIdx * _maskLen + div;
            _bitMapDisabled[index] &= ~(1UL << rem);
            _bitMap[index] |= 1UL << rem;
        }

        [MethodImpl(AggressiveInlining)]
        public void DelInBuffer(byte bufId, ushort bitPos) {
            var div = (ushort) (bitPos >> 6);
            var rem = (ushort) (bitPos & 63);
            var index = bufId * _maskLen + div;
            _tempBuffer[index] &= ~(1UL << rem);
        }

        [MethodImpl(AggressiveInlining)]
        public bool Has(uint bitMapIdx, ushort bitPos) {
            var div = (ushort) (bitPos >> 6);
            var rem = (ushort) (bitPos & 63);
            var index = bitMapIdx * _maskLen + div;
            return (_bitMap[index] & (1UL << rem)) != 0UL;
        }

        [MethodImpl(AggressiveInlining)]
        public bool HasInBuffer(byte bufId, ushort bitPos) {
            var div = (ushort) (bitPos >> 6);
            var rem = (ushort) (bitPos & 63);
            var index = bufId * _maskLen + div;
            return (_tempBuffer[index] & (1UL << rem)) != 0UL;
        }

        [MethodImpl(AggressiveInlining)]
        public bool IsEmpty(uint bitMapIdx) {
            var offset = bitMapIdx * _maskLen;
            var endOffset = offset + _maskLen;
            for (; offset < endOffset; offset++) {
                if (_bitMap[offset] != 0UL) {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(AggressiveInlining)]
        public bool IsEmptyWithDisabled(uint bitMapIdx) {
            var offset = bitMapIdx * _maskLen;
            var endOffset = offset + _maskLen;
            for (; offset < endOffset; offset++) {
                if ((_bitMap[offset] | _bitMapDisabled[offset]) != 0UL) {
                    return false;
                }
            }

            return true;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool IsEmptyBuffer(byte bitMapIdx) {
            var offset = bitMapIdx * _maskLen;
            var endOffset = offset + _maskLen;
            for (; offset < endOffset; offset++) {
                if (_tempBuffer[offset] != 0UL) {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(AggressiveInlining)]
        public ushort Len(uint bitMapIdx) {
            ushort count = 0;
            var offset = bitMapIdx * _maskLen;
            var endOffset = offset + _maskLen;
            for (; offset < endOffset; offset++) {
                var v = _bitMap[offset];
                v -= (v >> 1) & 0x5555555555555555;
                v = (v & 0x3333333333333333) + ((v >> 2) & 0x3333333333333333);
                count += (ushort) ((((v + (v >> 4)) & 0xF0F0F0F0F0F0F0F) * 0x101010101010101) >> 56);
            }

            return count;
        }
        
        [MethodImpl(AggressiveInlining)]
        public ushort LenWithDisabled(uint bitMapIdx) {
            ushort count = 0;
            var offset = bitMapIdx * _maskLen;
            var endOffset = offset + _maskLen;
            for (; offset < endOffset; offset++) {
                var v = _bitMap[offset] | _bitMapDisabled[offset];
                v -= (v >> 1) & 0x5555555555555555;
                v = (v & 0x3333333333333333) + ((v >> 2) & 0x3333333333333333);
                count += (ushort) ((((v + (v >> 4)) & 0xF0F0F0F0F0F0F0F) * 0x101010101010101) >> 56);
            }

            return count;
        }

        [MethodImpl(AggressiveInlining)]
        public ushort LenBuffer(byte bufId) {
            ushort count = 0;
            var offset = bufId * _maskLen;
            var endOffset = offset * _maskLen;
            for (; offset < endOffset; offset++) {
                var v = _tempBuffer[offset];
                v -= (v >> 1) & 0x5555555555555555;
                v = (v & 0x3333333333333333) + ((v >> 2) & 0x3333333333333333);
                count += (ushort) ((((v + (v >> 4)) & 0xF0F0F0F0F0F0F0F) * 0x101010101010101) >> 56);
            }

            return count;
        }

        [MethodImpl(AggressiveInlining)]
        public bool GetMinIndex(uint bitMapIdx, out int idx) {
            var offset = bitMapIdx * _maskLen;
            for (var i = 0; i < _maskLen; i++, offset++) {
                var v = _bitMap[offset];
                if (v != 0UL) {
                    idx = (i << 6) + BitsLut[((ulong) ((long) v & -(long) v) * 0x37E84A99DAE458F) >> 58];
                    return true;
                }
            }

            idx = -1;
            return false;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool GetMinIndexWithDisabled(uint bitMapIdx, out int idx) {
            var offset = bitMapIdx * _maskLen;
            for (var i = 0; i < _maskLen; i++, offset++) {
                var v = _bitMap[offset] | _bitMapDisabled[offset];
                if (v != 0UL) {
                    idx = (i << 6) + BitsLut[((ulong) ((long) v & -(long) v) * 0x37E84A99DAE458F) >> 58];
                    return true;
                }
            }

            idx = -1;
            return false;
        }

        [MethodImpl(AggressiveInlining)]
        public bool GetMinIndexBuffer(byte BufId, out int idx) {
            var offset = BufId * _maskLen;
            for (var i = 0; i < _maskLen; i++, offset++) {
                var v = _tempBuffer[offset];
                if (v != 0UL) {
                    idx = (i << 6) + BitsLut[((ulong) ((long) v & -(long) v) * 0x37E84A99DAE458F) >> 58];
                    return true;
                }
            }

            idx = -1;
            return false;
        }

        [MethodImpl(AggressiveInlining)]
        public void ClearAll(int bitMapIdx) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEmd = srcOffset + _maskLen;
            for (; srcOffset < srcOffsetEmd; srcOffset++) {
                _bitMap[srcOffset] = 0UL;
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void ClearAllInBuffer(byte BufId) {
            var srcOffset = BufId * _maskLen;
            var srcOffsetEmd = srcOffset + _maskLen;
            for (; srcOffset < srcOffsetEmd; srcOffset++) {
                _tempBuffer[srcOffset] = 0UL;
            }
        }

        [MethodImpl(AggressiveInlining)]
        public bool HasAll(uint bitMapIdx, byte bufId) {
            if (_maskLen < 2) {
                var rhs = _tempBuffer[bufId];
                return (_bitMap[bitMapIdx] & rhs) == rhs;
            }

            return HasAllArray(bitMapIdx, bufId);
        }
        
        private bool HasAllArray(uint bitMapIdx, byte bufId) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEnd = srcOffset + _maskLen;
            var bufOffset = (uint) (bufId * _maskLen);
            for (; srcOffset < srcOffsetEnd; srcOffset++, bufOffset++) {
                var rhs = _tempBuffer[bufOffset];
                if ((_bitMap[srcOffset] & rhs) != rhs) {
                    return false;
                }
            }

            return true;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool HasAllIndexed(uint bitMapIdx, uint bufId, ushort count) {
            var end = bufId + count;
            var srcOffset = bitMapIdx * _maskLen;
            for (; bufId < end; bufId++) {
                var rhs = _indexedBuffer[bufId];
                if ((_bitMap[srcOffset + rhs.Index] & rhs.Mask) != rhs.Mask) {
                    return false;
                }
            }

            return true;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool HasAllOnlyDisabled(uint bitMapIdx, byte bufId) {
            if (_maskLen < 2) {
                var rhs = _tempBuffer[bufId];
                return (_bitMapDisabled[bitMapIdx] & rhs) == rhs;
            }

            return HasAllArrayOnlyDisabled(bitMapIdx, bufId);
        }
        
        private bool HasAllArrayOnlyDisabled(uint bitMapIdx, byte bufId) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEnd = srcOffset + _maskLen;
            var bufOffset = (uint) (bufId * _maskLen);
            for (; srcOffset < srcOffsetEnd; srcOffset++, bufOffset++) {
                var rhs = _tempBuffer[bufOffset];
                if ((_bitMapDisabled[srcOffset] & rhs) != rhs) {
                    return false;
                }
            }

            return true;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool HasAllOnlyDisabledIndexed(uint bitMapIdx, uint bufId, ushort count) {
            var end = bufId + count;
            var srcOffset = bitMapIdx * _maskLen;
            for (; bufId < end; bufId++) {
                var rhs = _indexedBuffer[bufId];
                if ((_bitMapDisabled[srcOffset + rhs.Index] & rhs.Mask) != rhs.Mask) {
                    return false;
                }
            }

            return true;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool HasAllWithDisabled(uint bitMapIdx, byte bufId) {
            if (_maskLen < 2) {
                var rhs = _tempBuffer[bufId];
                return ((_bitMap[bitMapIdx] | _bitMapDisabled[bitMapIdx]) & rhs) == rhs;
            }

            return HasAllArrayWithDisabled(bitMapIdx, bufId);
        }
        
        private bool HasAllArrayWithDisabled(uint bitMapIdx, byte bufId) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEnd = srcOffset + _maskLen;
            var bufOffset = (uint) (bufId * _maskLen);
            for (; srcOffset < srcOffsetEnd; srcOffset++, bufOffset++) {
                var rhs = _tempBuffer[bufOffset];
                if (((_bitMap[srcOffset] | _bitMapDisabled[srcOffset]) & rhs) != rhs) {
                    return false;
                }
            }

            return true;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool HasAllWithDisabledIndexed(uint bitMapIdx, uint bufId, ushort count) {
            var end = bufId + count;
            var srcOffset = bitMapIdx * _maskLen;
            for (; bufId < end; bufId++) {
                var rhs = _indexedBuffer[bufId];
                if (((_bitMap[srcOffset + rhs.Index] | _bitMapDisabled[srcOffset + rhs.Index]) & rhs.Mask) != rhs.Mask) {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(AggressiveInlining)]
        public bool HasAny(uint bitMapIdx, byte bufId) {
            if (_maskLen < 2) {
                return (_bitMap[bitMapIdx] & _tempBuffer[bufId]) != 0UL;
            }

            return HasAnyArray(bitMapIdx, bufId);
        }

        private bool HasAnyArray(uint bitMapIdx, byte bufId) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEnd = srcOffset + _maskLen;
            var bufOffset = bufId * _maskLen;
            for (; srcOffset < srcOffsetEnd; srcOffset++, bufOffset++) {
                if ((_bitMap[srcOffset] & _tempBuffer[bufOffset]) != 0UL) {
                    return true;
                }
            }

            return false;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool HasAnyIndexed(uint bitMapIdx, uint bufId, ushort count) {
            var end = bufId + count;
            var srcOffset = bitMapIdx * _maskLen;
            for (; bufId < end; bufId++) {
                var rhs = _indexedBuffer[bufId];
                if ((_bitMap[srcOffset + rhs.Index] & rhs.Mask) != 0UL) {
                    return true;
                }
            }

            return false;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool HasAnyOnlyDisabled(uint bitMapIdx, byte bufId) {
            if (_maskLen < 2) {
                return (_bitMapDisabled[bitMapIdx] & _tempBuffer[bufId]) != 0UL;
            }

            return HasAnyArrayOnlyDisabled(bitMapIdx, bufId);
        }

        private bool HasAnyArrayOnlyDisabled(uint bitMapIdx, byte bufId) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEnd = srcOffset + _maskLen;
            var bufOffset = bufId * _maskLen;
            for (; srcOffset < srcOffsetEnd; srcOffset++, bufOffset++) {
                if ((_bitMapDisabled[srcOffset] & _tempBuffer[bufOffset]) != 0UL) {
                    return true;
                }
            }

            return false;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool HasAnyOnlyDisabledIndexed(uint bitMapIdx, uint bufId, ushort count) {
            var end = bufId + count;
            var srcOffset = bitMapIdx * _maskLen;
            for (; bufId < end; bufId++) {
                var rhs = _indexedBuffer[bufId];
                if ((_bitMapDisabled[srcOffset + rhs.Index] & rhs.Mask) != 0UL) {
                    return true;
                }
            }

            return false;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool HasAnyWithDisabled(uint bitMapIdx, byte bufId) {
            if (_maskLen < 2) {
                return ((_bitMapDisabled[bitMapIdx] | _bitMap[bitMapIdx]) & _tempBuffer[bufId]) != 0UL;
            }

            return HasAnyArrayWithDisabled(bitMapIdx, bufId);
        }

        private bool HasAnyArrayWithDisabled(uint bitMapIdx, byte bufId) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEnd = srcOffset + _maskLen;
            var bufOffset = bufId * _maskLen;
            for (; srcOffset < srcOffsetEnd; srcOffset++, bufOffset++) {
                if (((_bitMapDisabled[bitMapIdx] | _bitMap[bitMapIdx]) & _tempBuffer[bufOffset]) != 0UL) {
                    return true;
                }
            }

            return false;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool HasAnyWithDisabledIndexed(uint bitMapIdx, uint bufId, ushort count) {
            var end = bufId + count;
            var srcOffset = bitMapIdx * _maskLen;
            for (; bufId < end; bufId++) {
                var rhs = _indexedBuffer[bufId];
                if (((_bitMap[srcOffset + rhs.Index] | _bitMapDisabled[srcOffset + rhs.Index]) & rhs.Mask) != 0UL) {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(AggressiveInlining)]
        public bool HasAllAndExc(uint bitMapIdx, byte bufIdAll, byte bufIdExc) {
            if (_maskLen < 2) {
                var lhs = _bitMap[bitMapIdx];
                var rhs = _tempBuffer[bufIdAll];
                return (lhs & rhs) == rhs && (lhs & _tempBuffer[bufIdExc]) == 0UL;
            }

            return HasAllAndExcArray(bitMapIdx, bufIdAll, bufIdExc);
        }
        
        private bool HasAllAndExcArray(uint bitMapIdx, byte bufIdAll, byte bufIdExc) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEnd = srcOffset + _maskLen;
            var bufIdAllOffset = bufIdAll * _maskLen;
            var bufIdExcOffset = bufIdExc * _maskLen;
            for (; srcOffset < srcOffsetEnd; srcOffset++, bufIdAllOffset++, bufIdExcOffset++) {
                var lhs = _bitMap[srcOffset];
                var rhs = _tempBuffer[bufIdAllOffset];
                if ((lhs & rhs) != rhs || (lhs & _tempBuffer[bufIdExcOffset]) != 0UL) {
                    return false;
                }
            }

            return true;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool HasAllAndExcOnlyDisabled(uint bitMapIdx, byte bufIdAll, byte bufIdExc) {
            if (_maskLen < 2) {
                var lhs = _bitMapDisabled[bitMapIdx];
                var rhs = _tempBuffer[bufIdAll];
                return (lhs & rhs) == rhs && (lhs & _tempBuffer[bufIdExc]) == 0UL;
            }

            return HasAllAndExcArrayOnlyDisabled(bitMapIdx, bufIdAll, bufIdExc);
        }
        
        private bool HasAllAndExcArrayOnlyDisabled(uint bitMapIdx, byte bufIdAll, byte bufIdExc) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEnd = srcOffset + _maskLen;
            var bufIdAllOffset = bufIdAll * _maskLen;
            var bufIdExcOffset = bufIdExc * _maskLen;
            for (; srcOffset < srcOffsetEnd; srcOffset++, bufIdAllOffset++, bufIdExcOffset++) {
                var lhs = _bitMapDisabled[srcOffset];
                var rhs = _tempBuffer[bufIdAllOffset];
                if ((lhs & rhs) != rhs || (lhs & _tempBuffer[bufIdExcOffset]) != 0UL) {
                    return false;
                }
            }

            return true;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool HasAllAndExcWithDisabled(uint bitMapIdx, byte bufIdAll, byte bufIdExc) {
            if (_maskLen < 2) {
                var lhs = _bitMapDisabled[bitMapIdx] | _bitMap[bitMapIdx];
                var rhs = _tempBuffer[bufIdAll];
                return (lhs & rhs) == rhs && (lhs & _tempBuffer[bufIdExc]) == 0UL;
            }

            return HasAllAndExcArrayWithDisabled(bitMapIdx, bufIdAll, bufIdExc);
        }
        
        private bool HasAllAndExcArrayWithDisabled(uint bitMapIdx, byte bufIdAll, byte bufIdExc) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEnd = srcOffset + _maskLen;
            var bufIdAllOffset = bufIdAll * _maskLen;
            var bufIdExcOffset = bufIdExc * _maskLen;
            for (; srcOffset < srcOffsetEnd; srcOffset++, bufIdAllOffset++, bufIdExcOffset++) {
                var lhs = _bitMapDisabled[srcOffset] | _bitMap[srcOffset];
                var rhs = _tempBuffer[bufIdAllOffset];
                if ((lhs & rhs) != rhs || (lhs & _tempBuffer[bufIdExcOffset]) != 0UL) {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(AggressiveInlining)]
        public bool NotHasAny(uint bitMapIdx, byte bufId) {
            if (_maskLen < 2) {
                return (_bitMap[bitMapIdx] & _tempBuffer[bufId]) == 0UL;
            }

            return NotHasAnyArray(bitMapIdx, bufId);
        }
        
        private bool NotHasAnyArray(uint bitMapIdx, byte bufId) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEnd = srcOffset + _maskLen;
            var bufOffset = bufId * _maskLen;
            for (; srcOffset < srcOffsetEnd; srcOffset++, bufOffset++) {
                if ((_bitMap[srcOffset] & _tempBuffer[bufOffset]) != 0UL) {
                    return false;
                }
            }

            return true;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool NotHasAnyIndexed(uint bitMapIdx, uint bufId, ushort count) {
            var end = bufId + count;
            var srcOffset = bitMapIdx * _maskLen;
            for (; bufId < end; bufId++) {
                var rhs = _indexedBuffer[bufId];
                if ((_bitMap[srcOffset + rhs.Index] & rhs.Mask) != 0UL) {
                    return false;
                }
            }

            return true;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool NotHasAnyOnlyDisabled(uint bitMapIdx, byte bufId) {
            if (_maskLen < 2) {
                return (_bitMapDisabled[bitMapIdx] & _tempBuffer[bufId]) == 0UL;
            }

            return NotHasAnyArrayOnlyDisabled(bitMapIdx, bufId);
        }
        
        private bool NotHasAnyArrayOnlyDisabled(uint bitMapIdx, byte bufId) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEnd = srcOffset + _maskLen;
            var bufOffset = bufId * _maskLen;
            for (; srcOffset < srcOffsetEnd; srcOffset++, bufOffset++) {
                if ((_bitMapDisabled[srcOffset] & _tempBuffer[bufOffset]) != 0UL) {
                    return false;
                }
            }

            return true;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool NotHasAnyOnlyDisabledIndexed(uint bitMapIdx, uint bufId, ushort count) {
            var end = bufId + count;
            var srcOffset = bitMapIdx * _maskLen;
            for (; bufId < end; bufId++) {
                var rhs = _indexedBuffer[bufId];
                if ((_bitMapDisabled[srcOffset + rhs.Index] & rhs.Mask) != 0UL) {
                    return false;
                }
            }

            return true;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool NotHasAnyWithDisabled(uint bitMapIdx, byte bufId) {
            if (_maskLen < 2) {
                return ((_bitMapDisabled[bitMapIdx] | _bitMap[bitMapIdx]) & _tempBuffer[bufId]) == 0UL;
            }

            return NotHasAnyArrayWithDisabled(bitMapIdx, bufId);
        }
        
        private bool NotHasAnyArrayWithDisabled(uint bitMapIdx, byte bufId) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEnd = srcOffset + _maskLen;
            var bufOffset = bufId * _maskLen;
            for (; srcOffset < srcOffsetEnd; srcOffset++, bufOffset++) {
                if (((_bitMapDisabled[bitMapIdx] | _bitMap[bitMapIdx]) & _tempBuffer[bufOffset]) != 0UL) {
                    return false;
                }
            }

            return true;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool NotHasAnyWithDisabledIndexed(uint bitMapIdx, uint bufId, ushort count) {
            var end = bufId + count;
            var srcOffset = bitMapIdx * _maskLen;
            for (; bufId < end; bufId++) {
                var rhs = _indexedBuffer[bufId];
                if (((_bitMap[srcOffset + rhs.Index] | _bitMapDisabled[srcOffset + rhs.Index]) & rhs.Mask) != 0UL) {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(AggressiveInlining)]
        public void CopyToBuffer(uint bitMapIdx, byte bufId) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEnd = srcOffset + _maskLen;
            var bufOffset = bufId * _maskLen;
            for (; srcOffset < srcOffsetEnd; srcOffset++, bufOffset++) {
                _tempBuffer[bufOffset] = _bitMap[srcOffset];
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void CopyWithDisabledToBuffer(uint bitMapIdx, byte bufId) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEnd = srcOffset + _maskLen;
            var bufOffset = bufId * _maskLen;
            for (; srcOffset < srcOffsetEnd; srcOffset++, bufOffset++) {
                _tempBuffer[bufOffset] = _bitMap[srcOffset] | _bitMapDisabled[srcOffset];
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Copy(uint bitMapIdx1, uint bitMapIdx2) {
            var srcOffset1 = bitMapIdx1 * _maskLen;
            var srcOffset2 = bitMapIdx2 * _maskLen;
            var srcOffsetEnd1 = srcOffset1 + _maskLen;
            for (; srcOffset1 < srcOffsetEnd1; srcOffset1++, srcOffset2++) {
                _bitMap[srcOffset2] = _bitMap[srcOffset1];
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Clear() {
            Array.Clear(_bitMap, 0, _bitMap.Length);
            Array.Clear(_bitMapDisabled, 0, _bitMap.Length);
        }

        [MethodImpl(AggressiveInlining)]
        public (uint index, byte count) AddIndexedBuffer(byte bufId) {
            if (_indexedBufferCount * _maskLen == _indexedBuffer.Length) {
                Array.Resize(ref _indexedBuffer, _indexedBuffer.Length << 1);
            }

            var idx = (uint) (_indexedBufferCount * _maskLen);
            _indexedBufferCount++;
            byte cnt = 0;
            for (var i = 0; i < _maskLen; i++) {
                var val = _tempBuffer[bufId * _maskLen + i];
                if (val != 0) {
                    _indexedBuffer[idx + cnt] = new IndexedMask {
                        Index = (byte) i,
                        Mask = val
                    };
                    cnt++;
                }
            }

            return (idx, cnt);
        }
    }
}