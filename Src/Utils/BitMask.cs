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
        
        private ulong[] _bitMap;
        private ushort _maskLen;
        
        private ulong[] _tempBuffer;
        private byte _tempBufferFreeCount;
        private byte _tempBufferBorrowed;
        private byte _tempBufferCount;

        [MethodImpl(AggressiveInlining)]
        internal void Create(int worldCapacity, byte tempBuffersCount, ushort maskLen) {
            _maskLen = maskLen;
            _tempBufferCount = tempBuffersCount;
            _bitMap = new ulong[worldCapacity * _maskLen];
            _tempBuffer = new ulong[_tempBufferCount * _maskLen];
            _tempBufferFreeCount = _tempBufferCount;
            _tempBufferBorrowed = 0;
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void ResizeBitMap(int size) {
            Array.Resize(ref _bitMap, size * _maskLen);
        }

        [MethodImpl(AggressiveInlining)]
        internal byte BorrowBuf() {
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
            _maskLen = 0;
            _tempBufferCount = 0;
            _tempBufferBorrowed = 0;
            _tempBufferFreeCount = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Set(int bitMapIdx, ushort bitPos) {
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
        public void Del(int bitMapIdx, ushort bitPos) {
            var div = (ushort) (bitPos >> 6);
            var rem = (ushort) (bitPos & 63);
            var index = bitMapIdx * _maskLen + div;
            _bitMap[index] &= ~(1UL << rem);
        }

        [MethodImpl(AggressiveInlining)]
        public void DelInBuffer(byte bufId, ushort bitPos) {
            var div = (ushort) (bitPos >> 6);
            var rem = (ushort) (bitPos & 63);
            var index = bufId * _maskLen + div;
            _tempBuffer[index] &= ~(1UL << rem);
        }

        [MethodImpl(AggressiveInlining)]
        public bool Has(int bitMapIdx, ushort bitPos) {
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
        public bool IsEmpty(int bitMapIdx) {
            for (int i = bitMapIdx * _maskLen, iMax = (bitMapIdx + 1) * _maskLen; i < iMax; i++) {
                if (_bitMap[i] != 0UL) {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(AggressiveInlining)]
        public bool IsEmptyBuffer(byte bufId) {
            for (int i = bufId * _maskLen, iMax = (bufId + 1) * _maskLen; i < iMax; i++) {
                if (_tempBuffer[i] != 0UL) {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(AggressiveInlining)]
        public ushort Len(int bitMapIdx) {
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
        public int GetMinIndex(int bitMapIdx) {
            var offset = bitMapIdx * _maskLen;
            for (var i = 0; i < _maskLen; i++, offset++) {
                var v = _bitMap[offset];
                if (v != 0UL) {
                    return (i << 6) + BitsLut[((ulong) ((long) v & -(long) v) * 0x37E84A99DAE458F) >> 58];
                }
            }

            return -1;
        }

        [MethodImpl(AggressiveInlining)]
        public int GetMinIndexBuffer(byte BufId) {
            var offset = BufId * _maskLen;
            for (var i = 0; i < _maskLen; i++, offset++) {
                var v = _tempBuffer[offset];
                if (v != 0UL) {
                    return (i << 6) + BitsLut[((ulong) ((long) v & -(long) v) * 0x37E84A99DAE458F) >> 58];
                }
            }

            return -1;
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
        public bool HasAll(int bitMapIdx, byte bufId) {
            if (_maskLen < 2) {
                var rhs = _tempBuffer[bufId];
                return (_bitMap[bitMapIdx] & rhs) == rhs;
            }

            return HasAllArray(bitMapIdx, bufId);
        }
        
        private bool HasAllArray(int bitMapIdx, byte bufId) {
            var srcOffset = bitMapIdx * _maskLen;
            var srcOffsetEnd = srcOffset + _maskLen;
            var bufOffset = bufId * _maskLen;
            for (; srcOffset < srcOffsetEnd; srcOffset++, bufOffset++) {
                var rhs = _tempBuffer[bufOffset];
                if ((_bitMap[srcOffset] & rhs) != rhs) {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(AggressiveInlining)]
        public bool HasAny(int bitMapIdx, byte bufId) {
            if (_maskLen < 2) {
                return (_bitMap[bitMapIdx] & _tempBuffer[bufId]) != 0UL;
            }

            return HasAnyArray(bitMapIdx, bufId);
        }

        private bool HasAnyArray(int bitMapIdx, byte bufId) {
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
        public bool HasAllAndExc(int bitMapIdx, byte bufIdAll, byte bufIdExc) {
            if (_maskLen < 2) {
                var lhs = _bitMap[bitMapIdx];
                var rhs = _tempBuffer[bufIdAll];
                return (lhs & rhs) == rhs && (lhs & _tempBuffer[bufIdExc]) == 0UL;
            }

            return HasAllAndExcArray(bitMapIdx, bufIdAll, bufIdExc);
        }
        
        private bool HasAllAndExcArray(int bitMapIdx, byte bufIdAll, byte bufIdExc) {
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
        public bool NotHasAny(int bitMapIdx, byte bufId) {
            if (_maskLen < 2) {
                return (_bitMap[bitMapIdx] & _tempBuffer[bufId]) == 0UL;
            }

            return NotHasAnyArray(bitMapIdx, bufId);
        }
        
        private bool NotHasAnyArray(int bitMapIdx, byte bufId) {
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
        public void CopyToBuffer(int bitMapIdx, byte bufId) {
            Array.Copy(_bitMap, bitMapIdx * _maskLen, _tempBuffer, bufId * _maskLen, _maskLen);
        }

        [MethodImpl(AggressiveInlining)]
        public void Copy(int bitMapIdx1, int bitMapIdx2) {
            Array.Copy(_bitMap, bitMapIdx1 * _maskLen, _bitMap, bitMapIdx2 * _maskLen, _maskLen);
        }

        [MethodImpl(AggressiveInlining)]
        public void Clear() {
            Array.Clear(_bitMap, 0, _bitMap.Length);
        }
    }
}