#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    public interface IRefProvider<T, V> {
        protected internal ref V RefValue(ref T component);
    }

    public interface IMultiComponent<T, V> : IComponent, IRefProvider<T, Multi<V>> where T : struct where V : struct { }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct ROMulti<T> : IMultiComponent<ROMulti<T>, T>, IEquatable<ROMulti<T>> where T : struct {
        internal Multi<T> multi;
        
        internal void Init(Multi<T> value) {
            multi = value;
        }

        public ushort Capacity {
            [MethodImpl(AggressiveInlining)]
            get => multi.capacity;
        }

        public ushort Count {
            [MethodImpl(AggressiveInlining)]
            get => multi.count;
        }

        public T this[int idx] {
            [MethodImpl(AggressiveInlining)]
            get => multi[idx];
        }

        [MethodImpl(AggressiveInlining)]
        public readonly T First() {
            return multi.First();
        }
        
        [MethodImpl(AggressiveInlining)]
        public readonly T Last() {
            return multi.Last();
        }

        [MethodImpl(AggressiveInlining)]
        public int IndexOf(T item) {
            return multi.IndexOf(item);
        }

        [MethodImpl(AggressiveInlining)]
        public readonly bool IsEmpty() => multi.IsEmpty();

        [MethodImpl(AggressiveInlining)]
        public readonly bool IsNotEmpty() => multi.IsNotEmpty();

        [MethodImpl(AggressiveInlining)]
        public readonly bool IsFull() => multi.IsFull();

        [MethodImpl(AggressiveInlining)]
        public readonly bool Contains(T item) {
            return multi.Contains(item);
        }

        [MethodImpl(AggressiveInlining)]
        public readonly bool Contains<C>(T item, C comparer) where C : IEqualityComparer<T> {
            return multi.Contains(item, comparer);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void CopyTo(T[] dst) {
            multi.CopyTo(dst);
        }

        [MethodImpl(AggressiveInlining)]
        public void CopyTo(T[] dst, int dstIdx, int len) {
            multi.CopyTo(dst, dstIdx, len);
        }

        [MethodImpl(AggressiveInlining)]
        ref Multi<T> IRefProvider<ROMulti<T>, Multi<T>>.RefValue(ref ROMulti<T> component) => ref component.multi;

        public override string ToString() {
            var res = "";
            for (int i = 0; i < Count; i++) {
                res += this[i].ToString();
                if (i + 1 < Count) {
                    res += ", ";
                }
            }

            return res;
        }
        
        [MethodImpl(AggressiveInlining)]
        public ROMultiComponentsIterator<T> GetEnumerator() => new(this);

        [MethodImpl(AggressiveInlining)]
        public bool Equals(ROMulti<T> other) {
            return multi.Equals(other.multi);
        }

        [MethodImpl(AggressiveInlining)]
        public override bool Equals(object obj) => throw new StaticEcsException($"ROMulti<{typeof(T)}> `Equals object` not allowed!");

        [MethodImpl(AggressiveInlining)]
        public override int GetHashCode() => throw new StaticEcsException($"ROMulti<{typeof(T)}> `GetHashCode object` not allowed!");

        [MethodImpl(AggressiveInlining)]
        public static bool operator ==(ROMulti<T> left, ROMulti<T> right) => left.Equals(right);

        [MethodImpl(AggressiveInlining)]
        public static bool operator !=(ROMulti<T> left, ROMulti<T> right) => !left.Equals(right);
    }
    

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct Multi<T> : IMultiComponent<Multi<T>, T>, IEquatable<Multi<T>> where T : struct {
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
                #if FFS_ECS_DEBUG
                if (idx >= count) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Indexer ] index out of bounds: {idx}");
                #endif
                return ref data.values[offset + idx];
            }
        }

        [MethodImpl(AggressiveInlining)]
        public readonly ref T First() {
            #if FFS_ECS_DEBUG
            if (count == 0) throw new StaticEcsException($"[ Multi<{typeof(T)}>.First ] index out of bounds: {0}");
            #endif
            return ref data.values[offset];
        }
        
        [MethodImpl(AggressiveInlining)]
        public readonly ref T Last() {
            #if FFS_ECS_DEBUG
            if (count == 0) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Last ] index out of bounds: {0}");
            #endif
            return ref data.values[offset + count - 1];
        }

        [MethodImpl(AggressiveInlining)]
        public void Add(T val) {
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
            #endif
            if (count == capacity) {
                data.Resize(ref this, (uint) (capacity << 1));
            }
            data.values[offset + count++] = val;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Add(T val1, T val2) {
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
            #endif
            if (count + 1 >= capacity) {
                data.Resize(ref this, (uint) (capacity << 1));
            }
            data.values[offset + count++] = val1;
            data.values[offset + count++] = val2;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Add(T val1, T val2, T val3) {
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
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
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
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
            #if FFS_ECS_DEBUG
            if (src == null) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] src is null");
            if (src.Length > short.MaxValue + 1) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] src.Length > 32768");
            #endif
            Add(src, 0, (ushort) src.Length);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Add(T[] src, int srcIdx, int len) {
            #if FFS_ECS_DEBUG
            if (src == null) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] src is null");
            if (srcIdx >= src.Length) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] srcIdx >= src.Length");
            if (srcIdx + len > src.Length) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] srcIdx + len > src.Length");
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
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
            #if FFS_ECS_DEBUG
            if (srcIdx >= src.Count) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] srcIdx >= src.Count");
            if (srcIdx + len > src.Count) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] srcIdx + len > src.Count");
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
            #endif
            if (count + len >= capacity) {
                data.Resize(ref this, Utils.CalculateSize((uint) (count + len)));
            }
            
            Utils.LoopFallbackCopy(src.data.values, (uint) (src.offset + srcIdx), data.values, offset + count, (uint) len);
            count += (ushort)len;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void InsertAt(int idx, T value) {
            #if FFS_ECS_DEBUG
            if (idx >= count) throw new StaticEcsException($"[ Multi<{typeof(T)}>.InsertAt ] index out of bounds: {idx}");
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.InsertAt ] data is blocked by iterator");
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
            #if FFS_ECS_DEBUG
            if (count == 0) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteFirst ] index out of bounds: {0}");
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteFirst ] data is blocked by iterator");
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
            #if FFS_ECS_DEBUG
            if (count == 0) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteFirstSwap ] index out of bounds: {0}");
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteFirstSwap ] data is blocked by iterator");
            #endif
            data.values[offset] = data.values[offset + --count];
            data.values[offset + count] = default;
        }

        [MethodImpl(AggressiveInlining)]
        public void RemoveLast() {
            #if FFS_ECS_DEBUG
            if (count == 0) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteLast ] index out of bounds: {0}");
            if (data.IsBlocked(offset, count - 1)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteLast ] data is blocked by iterator");
            #endif
            data.values[offset + --count] = default;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void RemoveLastFast() {
            #if FFS_ECS_DEBUG
            if (count == 0) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteLast ] index out of bounds: {0}");
            if (data.IsBlocked(offset, count - 1)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteLast ] data is blocked by iterator");
            #endif
            --count;
        }

        [MethodImpl(AggressiveInlining)]
        public void RemoveAt(int idx) {
            #if FFS_ECS_DEBUG
            if (idx >= count) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteAt ] index out of bounds: {idx}");
            if (data.IsBlocked(offset, idx)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteAt ] data is blocked by iterator");
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
        public bool TryRemoveSwap(T item) {
            var idx = IndexOf(item);
            if (idx >= 0) {
                RemoveAtSwap(idx);
                return true;
            }

            return false;
        }
        
        [MethodImpl(AggressiveInlining)]
        public bool TryRemove(T item) {
            var idx = IndexOf(item);
            if (idx >= 0) {
                RemoveAt(idx);
                return true;
            }

            return false;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryRemove(T item1, T item2) {
            var idx = IndexOf(item1);
            if (idx >= 0) {
                RemoveAt(idx);
            }
            idx = IndexOf(item2);
            if (idx >= 0) {
                RemoveAt(idx);
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryRemove(T item1, T item2, T item3) {
            var idx = IndexOf(item1);
            if (idx >= 0) {
                RemoveAt(idx);
            }
            idx = IndexOf(item2);
            if (idx >= 0) {
                RemoveAt(idx);
            }
            idx = IndexOf(item3);
            if (idx >= 0) {
                RemoveAt(idx);
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryRemove(T item1, T item2, T item3, T item4) {
            var idx = IndexOf(item1);
            if (idx >= 0) {
                RemoveAt(idx);
            }
            idx = IndexOf(item2);
            if (idx >= 0) {
                RemoveAt(idx);
            }
            idx = IndexOf(item3);
            if (idx >= 0) {
                RemoveAt(idx);
            }
            idx = IndexOf(item4);
            if (idx >= 0) {
                RemoveAt(idx);
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void RemoveAtSwap(int idx) {
            #if FFS_ECS_DEBUG
            if (idx >= count) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteAtSwap ] index out of bounds: {idx}");
            if (data.IsBlocked(offset, idx)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteAtSwap ] data is blocked by iterator");
            #endif
            data.values[offset + idx] = data.values[offset + --count];
            data.values[offset + count] = default;
        }

        [MethodImpl(AggressiveInlining)]
        public int IndexOf(T item) {
            var indexOf = Array.IndexOf(data.values, item, (int)offset, count);
            return (int) (indexOf >= 0 ? indexOf - offset : -1);
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
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Clear ] data is blocked by iterator");
            #endif
            Utils.LoopFallbackClear(data.values, (int) offset, count);
            count = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void ResetCount() {
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.ResetCount ] data is blocked by iterator");
            #endif
            count = 0;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Resize(int newCapacity) {
            #if FFS_ECS_DEBUG
            if (newCapacity > short.MaxValue + 1) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Resize ] newCapacity > 32768");
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Resize ] data is blocked by iterator");
            #endif
            if (capacity < newCapacity) {
                data.Resize(ref this, Utils.CalculateSize((uint) newCapacity));
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void EnsureSize(int size) {
            #if FFS_ECS_DEBUG
            if (size + count > short.MaxValue + 1) throw new StaticEcsException($"[ Multi<{typeof(T)}>.EnsureSize ] size + count > 32768");
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.EnsureSize ] data is blocked by iterator");
            #endif
            if (count + size >= capacity) {
                data.Resize(ref this, Utils.CalculateSize((uint) (count + size)));
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void EnsureCount(int size) {
            #if FFS_ECS_DEBUG
            if (size + count > short.MaxValue + 1) throw new StaticEcsException($"[ Multi<{typeof(T)}>.EnsureCount ] size + count > 32768");
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.EnsureCount ] data is blocked by iterator");
            #endif
            if (count + size >= capacity) {
                data.Resize(ref this, Utils.CalculateSize((uint) (count + size)));
            }

            count += (ushort)size;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void CopyTo(T[] dst) {
            #if FFS_ECS_DEBUG
            if (dst == null) throw new StaticEcsException($"[ Multi<{typeof(T)}>.CopyTo ] items is null");
            if (count > dst.Length) throw new StaticEcsException($"[ Multi<{typeof(T)}>.CopyTo ] len > count");
            #endif
            Utils.LoopFallbackCopy(data.values, offset, dst, 0, count);
        }

        [MethodImpl(AggressiveInlining)]
        public void CopyTo(T[] dst, int dstIdx, int len) {
            #if FFS_ECS_DEBUG
            if (dst == null) throw new StaticEcsException($"[ Multi<{typeof(T)}>.CopyTo ] items is null");
            if (dstIdx >= dst.Length) throw new StaticEcsException($"[ Multi<{typeof(T)}>.CopyTo ] arrayIndex >= array.Length");
            if (dstIdx + len > dst.Length) throw new StaticEcsException($"[ Multi<{typeof(T)}>.CopyTo ] arrayIndex + len > array.Length");
            if (len > count) throw new StaticEcsException($"[ Multi<{typeof(T)}>.CopyTo ] len > count");
            #endif
            Utils.LoopFallbackCopy(data.values, offset, dst, (uint) dstIdx, (uint) len);
        }

        [MethodImpl(AggressiveInlining)]
        public void Sort<C>(C comparer) where C : IComparer<T> {
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Sort ] data is blocked by iterator");
            #endif
            Array.Sort(data.values, (int)offset, count, comparer);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Sort() {
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(offset, 0)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Sort ] data is blocked by iterator");
            #endif
            Array.Sort(data.values, (int)offset, count);
        }

        [MethodImpl(AggressiveInlining)]
        ref Multi<T> IRefProvider<Multi<T>, Multi<T>>.RefValue(ref Multi<T> component) => ref component;

        public override string ToString() {
            var res = "";
            for (int i = 0; i < Count; i++) {
                res += this[i].ToString();
                if (i + 1 < Count) {
                    res += ", ";
                }
            }

            return res;
        }
        
        [MethodImpl(AggressiveInlining)]
        public MultiComponentsIterator<T> GetEnumerator() => new(this);

        [MethodImpl(AggressiveInlining)]
        public bool Equals(Multi<T> other) {
            return ReferenceEquals(data, other.data) && offset == other.offset && count == other.count && capacity == other.capacity;
        }

        [MethodImpl(AggressiveInlining)]
        public override bool Equals(object obj) => throw new StaticEcsException($"Multi<{typeof(T)}> `Equals object` not allowed!");

        [MethodImpl(AggressiveInlining)]
        public override int GetHashCode() => throw new StaticEcsException($"Multi<{typeof(T)}> `GetHashCode object` not allowed!");

        [MethodImpl(AggressiveInlining)]
        public static bool operator ==(Multi<T> left, Multi<T> right) => left.Equals(right);

        [MethodImpl(AggressiveInlining)]
        public static bool operator !=(Multi<T> left, Multi<T> right) => !left.Equals(right);
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public ref struct MultiComponentsIterator<C> where C : struct {
        internal MultiComponents<C> data;
        internal uint offset;
        internal ushort to;

        [MethodImpl(AggressiveInlining)]
        public MultiComponentsIterator(Multi<C> multi) {
            data = multi.data;
            offset = multi.offset;
            to = multi.count;
            #if FFS_ECS_DEBUG
            data.Block(offset, to);
            #endif
        }

        public readonly ref C Current {
            [MethodImpl(AggressiveInlining)]
            get => ref data.values[offset + to];
        }

        [MethodImpl(AggressiveInlining)]
        public bool MoveNext() {
            if (to > 0) {
                to--;
                #if FFS_ECS_DEBUG
                data.Block(offset, to);
                #endif
                return true;
            }
            return false;
        }

        #if FFS_ECS_DEBUG
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
    public ref struct ROMultiComponentsIterator<C> where C : struct {
        internal MultiComponents<C> data;
        internal uint offset;
        internal ushort to;

        [MethodImpl(AggressiveInlining)]
        public ROMultiComponentsIterator(ROMulti<C> val) {
            data = val.multi.data;
            offset = val.multi.offset;
            to = val.multi.count;
            #if FFS_ECS_DEBUG
            data.Block(offset, to);
            #endif
        }

        public readonly C Current {
            [MethodImpl(AggressiveInlining)]
            get => data.values[offset + to];
        }

        [MethodImpl(AggressiveInlining)]
        public bool MoveNext() {
            if (to > 0) {
                to--;
                #if FFS_ECS_DEBUG
                data.Block(offset, to);
                #endif
                return true;
            }
            return false;
        }

        #if FFS_ECS_DEBUG
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
        
        #if FFS_ECS_DEBUG
        private readonly Dictionary<uint, int> _blocked = new();
        private MultiThreadStatus _mtStatus;
        #endif
        
        internal T[] values;
        private readonly Level[] _levels;
        private uint _valuesCount;
        private readonly ushort _minCapacity;
        private readonly ushort _minLevel;


        [MethodImpl(AggressiveInlining)]
        #if FFS_ECS_DEBUG
        internal MultiComponents(ushort minCapacity, MultiThreadStatus MTStatus) {
            _mtStatus = MTStatus;
        #else
        internal MultiComponents(ushort minCapacity) {
        #endif
            _minCapacity = (ushort) Utils.CalculateSize((uint) Math.Max((int) minCapacity, 4));
            _minLevel = Log2(_minCapacity);
            _levels = new Level[LevelOffset - _minLevel];
            for (var i = 0; i < _levels.Length; i++) {
                _levels[i] = new Level(new uint[Utils.CalculateSize((uint) (Const.ENTITIES_IN_CHUNK / (2 * (i + 1))))], 0);
            }

            values = new T[Utils.CalculateSize((uint) (_minCapacity * Const.ENTITIES_IN_CHUNK))];
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
            #if FFS_ECS_DEBUG
            if (_mtStatus.Active()) throw new StaticEcsException($"MultiComponents<{typeof(T)}>, Method: Resize, this operation is not supported in multithreaded mode");
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
            #if FFS_ECS_DEBUG
            if (levelIdx >= _levels.Length) throw new StaticEcsException($"[ Multi<{typeof(T)}> ] max capacity is {short.MaxValue + 1}");
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
        
        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        internal bool IsBlocked(uint offset, int val) {
            return _blocked.TryGetValue(offset, out var v) && v < val;
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void Block(uint offset, int val) {
            _blocked[offset] = val;
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