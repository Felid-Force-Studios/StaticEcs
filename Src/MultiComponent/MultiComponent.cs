using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public interface IMultiComponent<T> : IComponent where T : struct {
        
        public void Access<A>(A access) where A : struct, AccessMulti<T>;
    }
    
    public interface AccessMulti<T> where T : struct {
        public void For(ref Multi<T> values);
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Multi<T> : IMultiComponent<T> where T : struct {
        internal MultiComponents<T> data;
        internal uint offset;
        internal ushort count;
        internal ushort capacity;
        
        internal void Init(MultiComponents<T> newData, uint newOffset, ushort newCount, ushort newCapacity) {
            data = newData;
            offset = newOffset;
            count = newCount;
            capacity = newCapacity;
        }

        public ushort Capacity {
            [MethodImpl(AggressiveInlining)]
            get => capacity;
        }

        public ushort Count {
            [MethodImpl(AggressiveInlining)]
            get => count;
        }

        public ref T this[int idx] {
            [MethodImpl(AggressiveInlining)]
            get {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (idx >= count) throw new Exception($"[ Multi<{typeof(T)}>.Indexer ] index out of bounds: {idx}");
                #endif
                return ref data.values[offset + idx];
            }
        }

        [MethodImpl(AggressiveInlining)]
        public readonly ref T First() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (count == 0) throw new Exception($"[ Multi<{typeof(T)}>.First ] index out of bounds: {0}");
            #endif
            return ref data.values[offset];
        }
        
        [MethodImpl(AggressiveInlining)]
        public readonly ref T Last() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (count == 0) throw new Exception($"[ Multi<{typeof(T)}>.Last ] index out of bounds: {0}");
            #endif
            return ref data.values[offset + count - 1];
        }

        [MethodImpl(AggressiveInlining)]
        public void Add(T val) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
            #endif
            if (count == capacity) {
                data.Resize(ref this, (uint) (capacity << 1));
            }
            data.values[offset + count++] = val;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Add(T val1, T val2) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
            #endif
            if (count + 1 >= capacity) {
                data.Resize(ref this, (uint) (capacity << 1));
            }
            data.values[offset + count++] = val1;
            data.values[offset + count++] = val2;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Add(T val1, T val2, T val3) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
            #endif
            if (count + 2 >= capacity) {
                data.Resize(ref this, (uint) (capacity << 1));
            }
            data.values[offset + count++] = val1;
            data.values[offset + count++] = val2;
            data.values[offset + count++] = val3;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Add(T val1, T val2, T val3, T val4) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
            #endif
            if (count + 3 >= capacity) {
                data.Resize(ref this, (uint) (capacity << 1));
            }
            data.values[offset + count++] = val1;
            data.values[offset + count++] = val2;
            data.values[offset + count++] = val3;
            data.values[offset + count++] = val4;
        }

        [MethodImpl(AggressiveInlining)]
        public void Add(T[] src) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (src == null) throw new Exception($"[ Multi<{typeof(T)}>.Add ] src is null");
            if (src.Length > short.MaxValue + 1) throw new Exception($"[ Multi<{typeof(T)}>.Add ] src.Length > 32768");
            #endif
            Add(src, 0, (ushort) src.Length);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Add(T[] src, int srcIdx, int len) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (src == null) throw new Exception($"[ Multi<{typeof(T)}>.Add ] src is null");
            if (srcIdx >= src.Length) throw new Exception($"[ Multi<{typeof(T)}>.Add ] srcIdx >= src.Length");
            if (srcIdx + len > src.Length) throw new Exception($"[ Multi<{typeof(T)}>.Add ] srcIdx + len > src.Length");
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
            #endif
            if (count + len >= capacity) {
                data.Resize(ref this, Utils.CalculateSize((uint) (count + len)));
            }
            
            Utils.LoopFallbackCopy(src, (uint) srcIdx, data.values, offset + count, (uint) len);
            count += (ushort)len;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Add(ref Multi<T> src) {
            Add(ref src, 0, src.count);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Add(ref Multi<T> src, int srcIdx, int len) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (srcIdx >= src.Count) throw new Exception($"[ Multi<{typeof(T)}>.Add ] srcIdx >= src.Count");
            if (srcIdx + len > src.Count) throw new Exception($"[ Multi<{typeof(T)}>.Add ] srcIdx + len > src.Count");
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
            #endif
            if (count + len >= capacity) {
                data.Resize(ref this, Utils.CalculateSize((uint) (count + len)));
            }
            
            Utils.LoopFallbackCopy(src.data.values, (uint) (src.offset + srcIdx), data.values, offset + count, (uint) len);
            count += (ushort)len;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void InsertAt(int idx, T value) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (idx >= count) throw new Exception($"[ Multi<{typeof(T)}>.InsertAt ] index out of bounds: {idx}");
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.InsertAt ] data is blocked by iterator");
            #endif
            if (count == capacity) {
                data.Resize(ref this, (uint) (capacity << 1));
            }

            if (idx < count) {
                Utils.LoopFallbackCopyReverse(data.values, (uint) (offset + idx), data.values, offset + (uint) (idx + 1), (uint) (count - idx));
            }

            data.values[offset + idx] = value;
            ++count;
        }

        [MethodImpl(AggressiveInlining)]
        public void RemoveFirst() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (count == 0) throw new Exception($"[ Multi<{typeof(T)}>.DeleteFirst ] index out of bounds: {0}");
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.DeleteFirst ] data is blocked by iterator");
            #endif
            count--;
            if (count > 0) {
                Utils.LoopFallbackCopy(data.values, offset + 1, data.values, offset, count);
                data.values[offset + count] = default;
            } else {
                data.values[offset] = default;
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void RemoveFirstSwap() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (count == 0) throw new Exception($"[ Multi<{typeof(T)}>.DeleteFirstSwap ] index out of bounds: {0}");
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.DeleteFirstSwap ] data is blocked by iterator");
            #endif
            data.values[offset] = data.values[offset + --count];
            data.values[offset + count] = default;
        }

        [MethodImpl(AggressiveInlining)]
        public void RemoveLast() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (count == 0) throw new Exception($"[ Multi<{typeof(T)}>.DeleteLast ] index out of bounds: {0}");
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.DeleteLast ] data is blocked by iterator");
            #endif
            data.values[offset + --count] = default;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void RemoveLastFast() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (count == 0) throw new Exception($"[ Multi<{typeof(T)}>.DeleteLast ] index out of bounds: {0}");
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.DeleteLast ] data is blocked by iterator");
            #endif
            --count;
        }

        [MethodImpl(AggressiveInlining)]
        public void RemoveAt(int idx) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (idx >= count) throw new Exception($"[ Multi<{typeof(T)}>.DeleteAt ] index out of bounds: {idx}");
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.DeleteAt ] data is blocked by iterator");
            #endif
            count--;
            if (idx == count) {
                data.values[offset + idx] = default;
            } else {
                Utils.LoopFallbackCopy(data.values, (uint) (offset + idx + 1), data.values, (uint) (offset + idx), (uint) (count - idx));
                data.values[offset + count] = default;
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void RemoveAtSwap(int idx) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (idx >= count) throw new Exception($"[ Multi<{typeof(T)}>.DeleteAtSwap ] index out of bounds: {idx}");
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.DeleteAtSwap ] data is blocked by iterator");
            #endif
            data.values[offset + idx] = data.values[offset + --count];
            data.values[offset + count] = default;
        }

        [MethodImpl(AggressiveInlining)]
        public int IndexOf(T item) {
            var indexOf = Array.IndexOf(data.values, item, (int)offset, count);
            return (int) (indexOf > 0 ? indexOf - offset : -1);
        }

        [MethodImpl(AggressiveInlining)]
        public readonly bool IsEmpty() => count == 0;

        [MethodImpl(AggressiveInlining)]
        public readonly bool IsNotEmpty() => count != 0;

        [MethodImpl(AggressiveInlining)]
        public readonly bool IsFull() => count == capacity;

        [MethodImpl(AggressiveInlining)]
        public readonly bool Contains(T item) {
            var equalityComparer = EqualityComparer<T>.Default;
            for (var index = offset; index < offset + count; ++index) {
                if (equalityComparer.Equals(data.values[index], item)) return true;
            }

            return false;
        }

        [MethodImpl(AggressiveInlining)]
        public readonly bool Contains<C>(T item, C comparer) where C : IEqualityComparer<T> {
            for (var index = offset; index < offset + count; ++index) {
                if (comparer.Equals(data.values[index], item)) return true;
            }

            return false;
        }

        [MethodImpl(AggressiveInlining)]
        public void Clear() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.Clear ] data is blocked by iterator");
            #endif
            Utils.LoopFallbackClear(data.values, (int) offset, count);
            count = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void ResetCount() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.ResetCount ] data is blocked by iterator");
            #endif
            count = 0;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Resize(int newCapacity) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (newCapacity > short.MaxValue + 1) throw new Exception($"[ Multi<{typeof(T)}>.Resize ] newCapacity > 32768");
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.Resize ] data is blocked by iterator");
            #endif
            if (capacity < newCapacity) {
                data.Resize(ref this, Utils.CalculateSize((uint) newCapacity));
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void EnsureSize(int size) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (size + count > short.MaxValue + 1) throw new Exception($"[ Multi<{typeof(T)}>.EnsureSize ] size + count > 32768");
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.EnsureSize ] data is blocked by iterator");
            #endif
            if (count + size >= capacity) {
                data.Resize(ref this, Utils.CalculateSize((uint) (count + size)));
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void EnsureCount(int size) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (size + count > short.MaxValue + 1) throw new Exception($"[ Multi<{typeof(T)}>.EnsureCount ] size + count > 32768");
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.EnsureCount ] data is blocked by iterator");
            #endif
            if (count + size >= capacity) {
                data.Resize(ref this, Utils.CalculateSize((uint) (count + size)));
            }

            count += (ushort)size;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void CopyTo(T[] dst) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (dst == null) throw new Exception($"[ Multi<{typeof(T)}>.CopyTo ] items is null");
            if (count > dst.Length) throw new Exception($"[ Multi<{typeof(T)}>.CopyTo ] len > count");
            #endif
            Utils.LoopFallbackCopy(data.values, offset, dst, 0, count);
        }

        [MethodImpl(AggressiveInlining)]
        public void CopyTo(T[] dst, int dstIdx, int len) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (dst == null) throw new Exception($"[ Multi<{typeof(T)}>.CopyTo ] items is null");
            if (dstIdx >= dst.Length) throw new Exception($"[ Multi<{typeof(T)}>.CopyTo ] arrayIndex >= array.Length");
            if (dstIdx + len > dst.Length) throw new Exception($"[ Multi<{typeof(T)}>.CopyTo ] arrayIndex + len > array.Length");
            if (len > count) throw new Exception($"[ Multi<{typeof(T)}>.CopyTo ] len > count");
            #endif
            Utils.LoopFallbackCopy(data.values, offset, dst, (uint) dstIdx, (uint) len);
        }

        [MethodImpl(AggressiveInlining)]
        public void Sort<C>(C comparer) where C : IComparer<T> {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.Sort ] data is blocked by iterator");
            #endif
            Array.Sort(data.values, (int)offset, count, comparer);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Sort() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (data.IsBlocked(offset)) throw new Exception($"[ Multi<{typeof(T)}>.Sort ] data is blocked by iterator");
            #endif
            Array.Sort(data.values, (int)offset, count);
        }

        [MethodImpl(AggressiveInlining)]
        public MultiComponentsIterator<T> GetEnumerator() => new(this);

        [MethodImpl(AggressiveInlining)]
        public void Access<A>(A access) where A : struct, AccessMulti<T> => access.For(ref this);
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public ref struct MultiComponentsIterator<C> where C : struct {
        internal MultiComponents<C> data;
        internal uint offset;
        internal short from;
        internal ushort to;

        [MethodImpl(AggressiveInlining)]
        public MultiComponentsIterator(Multi<C> multi) {
            data = multi.data;
            offset = multi.offset;
            from = -1;
            to = multi.count;
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            data.Block(offset);
            #endif
        }

        public readonly ref C Current {
            [MethodImpl(AggressiveInlining)]
            get => ref data.values[offset + from];
        }

        [MethodImpl(AggressiveInlining)]
        public bool MoveNext() {
            if (from < to - 1) {
                from++;
                return true;
            }
            return false;
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose() {
            data.Unblock(offset);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public class MultiComponents<T> where T : struct {
        private const int LevelOffset = 16;
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        private readonly HashSet<uint> _blocked = new();
        private MultiThreadStatus _mtStatus;
        #endif
        
        internal T[] values;
        private readonly Level[] _levels;
        private uint _valuesCount;
        private readonly ushort _minCapacity;
        private readonly ushort _minLevel;


        [MethodImpl(AggressiveInlining)]
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        internal MultiComponents(ushort minCapacity, uint baseEntitiesCount, MultiThreadStatus MTStatus) {
            _mtStatus = MTStatus;
        #else
        internal MultiComponents(ushort minCapacity, uint baseEntitiesCount) {
        #endif
            _minCapacity = (ushort) Utils.CalculateSize((uint) Math.Max((int) minCapacity, 4));
            _minLevel = Log2(_minCapacity);
            _levels = new Level[LevelOffset - _minLevel];
            for (var i = 0; i < _levels.Length; i++) {
                _levels[i] = new Level(new uint[Utils.CalculateSize((uint) (baseEntitiesCount / (2 * (i + 1))))], 0);
            }

            values = new T[Utils.CalculateSize(_minCapacity * baseEntitiesCount)];
            _valuesCount = 0;
        }

        [MethodImpl(AggressiveInlining)]
        internal void Add(ref Multi<T> value) {
            var offset = _valuesCount;
            ref var level = ref _levels[0];
            if (level.Count > 0) {
                offset = level.Chunks[--level.Count];
            } else {
                if (_valuesCount + _minCapacity >= values.Length) {
                    Array.Resize(ref values, values.Length << 1);
                }
                _valuesCount += _minCapacity;
            }

            value.Init(this, offset, 0, _minCapacity);
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void Add(ref Multi<T> value, ushort capacity) {
            capacity = (ushort) Math.Max((int) capacity, _minCapacity);
            if (!FindFree(capacity, out var offset)) {
                if (_valuesCount + capacity >= values.Length) {
                    Array.Resize(ref values, (int) Utils.CalculateSize((uint) (values.Length + capacity)));
                }
                offset = _valuesCount;
                _valuesCount += capacity;
            }
            
            value.Init(this, offset, 0, _minCapacity);
        }

        [MethodImpl(AggressiveInlining)]
        internal void Delete(ref Multi<T> value) {
            var levelIdx = Log2(value.capacity) - _minLevel;
            ref var level = ref _levels[levelIdx];
            if (level.Count == level.Chunks.Length) {
                Array.Resize(ref level.Chunks, (int) (level.Count << 1));
            }
            
            level.Chunks[level.Count++] = value.offset;
            Utils.LoopFallbackClear(values, (int) value.offset, value.count);
            value = default;
        }

        [MethodImpl(AggressiveInlining)]
        internal void Copy(ref Multi<T> src, ref Multi<T> dst) {
            if (dst.capacity < src.count) {
                Delete(ref dst);
                Add(ref dst, src.count);
            }

            Utils.LoopFallbackCopy(values, src.offset, values, dst.offset, src.count);
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void Copy(ushort srcCount, uint srcOffset, ref Multi<T> dst) {
            if (dst.capacity < srcCount) {
                Delete(ref dst);
                Add(ref dst, srcCount);
            }

            Utils.LoopFallbackCopy(values, srcOffset, values, dst.offset, srcCount);
        }

        [MethodImpl(AggressiveInlining)]
        internal void Resize(ref Multi<T> value, uint newCapacity) {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (_mtStatus.Active) throw new Exception($"MultiComponents<{typeof(T)}>, Method: Resize, this operation is not supported in multithreaded mode");
            #endif
            if (!FindFree(newCapacity, out var offset)) {
                if (_valuesCount + newCapacity >= values.Length) {
                    Array.Resize(ref values, (int) Utils.CalculateSize((uint) (values.Length + newCapacity)));
                }
                offset = _valuesCount;
                _valuesCount += newCapacity;
            }
            Utils.LoopFallbackCopy(values, value.offset, values, offset, value.count);
            var count = value.count;
            Delete(ref value);
            value.Init(this, offset, count, (ushort) newCapacity);
        }
        
        [MethodImpl(AggressiveInlining)]
        internal bool FindFree(uint capacity, out uint offset) {
            var levelIdx = Log2(capacity) - _minLevel;
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (levelIdx >= _levels.Length) throw new Exception($"[ Multi<{typeof(T)}> ] max capacity is {short.MaxValue + 1}");
            #endif
            ref var level = ref _levels[levelIdx];
            if (level.Count > 0) {
                offset = level.Chunks[--level.Count];
                return true;
            }

            offset = 0;
            return false;
        }
        
        [MethodImpl(AggressiveInlining)]
        private static byte Log2(uint x) => (byte) ((byte) ((BitConverter.DoubleToInt64Bits(x) >> 52) + 1) & 0xFF);
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        internal bool IsBlocked(uint offset) {
            return _blocked.Contains(offset);
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void Block(uint offset) {
            _blocked.Add(offset);
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void Unblock(uint offset) {
            _blocked.Remove(offset);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct Level {
        public uint[] Chunks;
        public uint Count;

        public Level(uint[] chunks, uint count) {
            Chunks = chunks;
            Count = count;
        }
    }
}