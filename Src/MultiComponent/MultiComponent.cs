#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
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
            [MethodImpl(AggressiveInlining)] get => multi.Capacity;
        }

        public ushort Count {
            [MethodImpl(AggressiveInlining)] get => multi.count;
        }

        public T this[int idx] {
            [MethodImpl(AggressiveInlining)] get => multi[idx];
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
        internal uint blockIdx;
        internal ushort count;
        internal byte level;
        internal byte dataOffset;

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        internal void Init(MultiComponents<T> data, uint blockIdx, ushort count, byte level, byte dataOffset) {
            this.data = data;
            this.blockIdx = blockIdx;
            this.count = count;
            this.level = level;
            this.dataOffset = dataOffset;
        }

        public ushort Capacity {
            [MethodImpl(AggressiveInlining)] get => (ushort) (1 << level);
        }

        public ushort Count {
            [MethodImpl(AggressiveInlining)] get => count;
        }

        public ref T this[int idx] {
            [MethodImpl(AggressiveInlining)]
            get {
                #if FFS_ECS_DEBUG
                if (idx >= count) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Indexer ] index out of bounds: {idx}");
                #endif
                return ref data.values[blockIdx][dataOffset + idx];
            }
        }

        [MethodImpl(AggressiveInlining)]
        public readonly ref T First() {
            #if FFS_ECS_DEBUG
            if (count == 0) throw new StaticEcsException($"[ Multi<{typeof(T)}>.First ] index out of bounds: {0}");
            #endif
            return ref data.values[blockIdx][dataOffset];
        }

        [MethodImpl(AggressiveInlining)]
        public readonly ref T Last() {
            #if FFS_ECS_DEBUG
            if (count == 0) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Last ] index out of bounds: {0}");
            #endif
            return ref data.values[blockIdx][dataOffset + count - 1];
        }

        [MethodImpl(AggressiveInlining)]
        public void Add(T val) {
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
            #endif
            if (count == 1 << level) {
                data.Resize(ref this, (uint) (1 << level << 1));
            }

            data.values[blockIdx][dataOffset + count++] = val;
        }

        [MethodImpl(AggressiveInlining)]
        public void Add(T val1, T val2) {
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
            #endif
            if (count + 1 >= 1 << level) {
                data.Resize(ref this, (uint) (1 << level << 1));
            }

            var values = data.values[blockIdx];
            values[dataOffset + count++] = val1;
            values[dataOffset + count++] = val2;
        }

        [MethodImpl(AggressiveInlining)]
        public void Add(T val1, T val2, T val3) {
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
            #endif
            if (count + 2 >= 1 << level) {
                data.Resize(ref this, (uint) (1 << level << 1));
            }

            var values = data.values[blockIdx];
            values[dataOffset + count++] = val1;
            values[dataOffset + count++] = val2;
            values[dataOffset + count++] = val3;
        }

        [MethodImpl(AggressiveInlining)]
        public void Add(T val1, T val2, T val3, T val4) {
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
            #endif
            if (count + 3 >= 1 << level) {
                data.Resize(ref this, (uint) (1 << level << 1));
            }

            var values = data.values[blockIdx];
            values[dataOffset + count++] = val1;
            values[dataOffset + count++] = val2;
            values[dataOffset + count++] = val3;
            values[dataOffset + count++] = val4;
        }

        [MethodImpl(AggressiveInlining)]
        public void Add(T[] src) {
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
            if (src == null) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] src is null");
            if (src.Length > short.MaxValue + 1) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] src.Length > 32768");
            #endif
            Add(src, 0, (ushort) src.Length);
        }

        [MethodImpl(AggressiveInlining)]
        public void Add(T[] src, int srcIdx, int len) {
            if (len > 0) {
                #if FFS_ECS_DEBUG
                if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
                if (src == null) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] src is null");
                if (srcIdx >= src.Length) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] srcIdx >= src.Length");
                if (srcIdx + len > src.Length) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] srcIdx + len > src.Length");
                #endif
                if (count + len > 1 << level) {
                    data.Resize(ref this, Utils.CalculateSize((uint) (count + len)));
                }

                Utils.LoopFallbackCopy(src, (uint) srcIdx, data.values[blockIdx], (uint) (dataOffset + count), (uint) len);
                count += (ushort) len;
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Add(ref Multi<T> src) {
            Add(ref src, 0, src.count);
        }

        [MethodImpl(AggressiveInlining)]
        public void Add(ref Multi<T> src, int srcIdx, int len) {
            if (len > 0) {
                #if FFS_ECS_DEBUG
                if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] data is blocked by iterator");
                if (srcIdx >= src.Count) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] srcIdx >= src.Count");
                if (srcIdx + len > src.Count) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Add ] srcIdx + len > src.Count");
                #endif
                if (count + len > 1 << level) {
                    data.Resize(ref this, Utils.CalculateSize((uint) (count + len)));
                }

                Utils.LoopFallbackCopy(src.data.values[src.blockIdx], (uint) (src.dataOffset + srcIdx), data.values[blockIdx], (uint) (dataOffset + count), (uint) len);
                count += (ushort) len;
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void InsertAt(int idx, T value) {
            #if FFS_ECS_DEBUG
            if (idx >= count) throw new StaticEcsException($"[ Multi<{typeof(T)}>.InsertAt ] index out of bounds: {idx}");
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.InsertAt ] data is blocked by iterator");
            #endif
            if (count == 1 << level) {
                data.Resize(ref this, (uint) (1 << level << 1));
            }

            var values = data.values[blockIdx];

            if (idx < count) {
                Utils.LoopFallbackCopyReverse(values, (uint) (dataOffset + idx), values, dataOffset + (uint) (idx + 1), (uint) (count - idx));
            }

            values[dataOffset + idx] = value;
            ++count;
        }

        [MethodImpl(AggressiveInlining)]
        public void RemoveFirst() {
            #if FFS_ECS_DEBUG
            if (count == 0) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteFirst ] index out of bounds: {0}");
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteFirst ] data is blocked by iterator");
            #endif
            count--;
            var values = data.values[blockIdx];
            if (count > 0) {
                Utils.LoopFallbackCopy(values, (uint) (dataOffset + 1), values, dataOffset, count);
                values[dataOffset + count] = default;
            } else {
                values[dataOffset] = default;
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void RemoveFirstSwap() {
            #if FFS_ECS_DEBUG
            if (count == 0) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteFirstSwap ] index out of bounds: {0}");
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteFirstSwap ] data is blocked by iterator");
            #endif
            var values = data.values[blockIdx];

            values[dataOffset] = values[dataOffset + --count];
            values[dataOffset + count] = default;
        }

        [MethodImpl(AggressiveInlining)]
        public void RemoveLast() {
            #if FFS_ECS_DEBUG
            if (count == 0) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteLast ] index out of bounds: {0}");
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteLast ] data is blocked by iterator");
            #endif
            data.values[blockIdx][dataOffset + --count] = default;
        }

        [MethodImpl(AggressiveInlining)]
        public void RemoveLastFast() {
            #if FFS_ECS_DEBUG
            if (count == 0) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteLast ] index out of bounds: {0}");
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteLast ] data is blocked by iterator");
            #endif
            --count;
        }

        [MethodImpl(AggressiveInlining)]
        public void RemoveAt(int idx) {
            #if FFS_ECS_DEBUG
            if (idx >= count) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteAt ] index out of bounds: {idx}");
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteAt ] data is blocked by iterator");
            #endif
            count--;
            var values = data.values[blockIdx];
            if (idx == count) {
                values[dataOffset + idx] = default;
            } else {
                Utils.LoopFallbackCopy(values, (uint) (dataOffset + idx + 1), values, (uint) (dataOffset + idx), (uint) (count - idx));
                values[dataOffset + count] = default;
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
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.DeleteAtSwap ] data is blocked by iterator");
            #endif
            var values = data.values[blockIdx];
            values[dataOffset + idx] = values[dataOffset + --count];
            values[dataOffset + count] = default;
        }

        [MethodImpl(AggressiveInlining)]
        public int IndexOf(T item) {
            var indexOf = Array.IndexOf(data.values[blockIdx], item, dataOffset, count);
            return indexOf >= 0 ? indexOf - dataOffset : -1;
        }

        [MethodImpl(AggressiveInlining)]
        public readonly bool IsEmpty() => count == 0;

        [MethodImpl(AggressiveInlining)]
        public readonly bool IsNotEmpty() => count != 0;

        [MethodImpl(AggressiveInlining)]
        public readonly bool IsFull() => count == 1 << level;

        [MethodImpl(AggressiveInlining)]
        public readonly bool Contains(T item) {
            var equalityComparer = EqualityComparer<T>.Default;
            var values = data.values[blockIdx];
            for (var index = dataOffset; index < dataOffset + count; ++index) {
                if (equalityComparer.Equals(values[index], item)) return true;
            }

            return false;
        }

        [MethodImpl(AggressiveInlining)]
        public readonly bool Contains<C>(T item, C comparer) where C : IEqualityComparer<T> {
            var values = data.values[blockIdx];
            for (var index = dataOffset; index < dataOffset + count; ++index) {
                if (comparer.Equals(values[index], item)) return true;
            }

            return false;
        }

        [MethodImpl(AggressiveInlining)]
        public void Clear() {
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Clear ] data is blocked by iterator");
            #endif
            Utils.LoopFallbackClear(data.values[blockIdx], (int) dataOffset, count);
            count = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void ResetCount() {
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.ResetCount ] data is blocked by iterator");
            #endif
            count = 0;
        }

        [MethodImpl(AggressiveInlining)]
        public void Resize(int newCapacity) {
            #if FFS_ECS_DEBUG
            if (newCapacity > short.MaxValue + 1) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Resize ] newCapacity > 32768");
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Resize ] data is blocked by iterator");
            #endif
            if (1 << level < newCapacity) {
                data.Resize(ref this, Utils.CalculateSize((uint) newCapacity));
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void EnsureSize(int size) {
            #if FFS_ECS_DEBUG
            if (size + count > short.MaxValue + 1) throw new StaticEcsException($"[ Multi<{typeof(T)}>.EnsureSize ] size + count > 32768");
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.EnsureSize ] data is blocked by iterator");
            #endif
            if (count + size > 1 << level) {
                data.Resize(ref this, Utils.CalculateSize((uint) (count + size)));
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void EnsureCount(int size) {
            #if FFS_ECS_DEBUG
            if (size + count > short.MaxValue + 1) throw new StaticEcsException($"[ Multi<{typeof(T)}>.EnsureCount ] size + count > 32768");
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.EnsureCount ] data is blocked by iterator");
            #endif
            if (count + size > 1 << level) {
                data.Resize(ref this, Utils.CalculateSize((uint) (count + size)));
            }

            count += (ushort) size;
        }

        [MethodImpl(AggressiveInlining)]
        public void CopyTo(T[] dst) {
            #if FFS_ECS_DEBUG
            if (dst == null) throw new StaticEcsException($"[ Multi<{typeof(T)}>.CopyTo ] items is null");
            if (count > dst.Length) throw new StaticEcsException($"[ Multi<{typeof(T)}>.CopyTo ] len > count");
            #endif
            Utils.LoopFallbackCopy(data.values[blockIdx], dataOffset, dst, 0, count);
        }

        [MethodImpl(AggressiveInlining)]
        public void CopyTo(T[] dst, int dstIdx, int len) {
            #if FFS_ECS_DEBUG
            if (dst == null) throw new StaticEcsException($"[ Multi<{typeof(T)}>.CopyTo ] items is null");
            if (dstIdx >= dst.Length) throw new StaticEcsException($"[ Multi<{typeof(T)}>.CopyTo ] arrayIndex >= array.Length");
            if (dstIdx + len > dst.Length) throw new StaticEcsException($"[ Multi<{typeof(T)}>.CopyTo ] arrayIndex + len > array.Length");
            if (len > count) throw new StaticEcsException($"[ Multi<{typeof(T)}>.CopyTo ] len > count");
            #endif
            Utils.LoopFallbackCopy(data.values[blockIdx], dataOffset, dst, (uint) dstIdx, (uint) len);
        }

        [MethodImpl(AggressiveInlining)]
        public void Sort<C>(C comparer) where C : IComparer<T> {
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Sort ] data is blocked by iterator");
            #endif
            Array.Sort(data.values[blockIdx], dataOffset, count, comparer);
        }

        [MethodImpl(AggressiveInlining)]
        public void Sort() {
            #if FFS_ECS_DEBUG
            if (data.IsBlocked(ref this)) throw new StaticEcsException($"[ Multi<{typeof(T)}>.Sort ] data is blocked by iterator");
            #endif
            Array.Sort(data.values[blockIdx], dataOffset, count);
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
            return ReferenceEquals(data, other.data) && blockIdx == other.blockIdx && count == other.count && level == other.level && dataOffset == other.dataOffset;
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
        #if FFS_ECS_DEBUG
        internal MultiComponents<C> data;
        internal uint blockIdx;
        #endif
        internal C[] values;
        internal int to;
        internal byte offset;

        [MethodImpl(AggressiveInlining)]
        public MultiComponentsIterator(Multi<C> multi) {
            values = multi.data.values[multi.blockIdx];
            offset = multi.dataOffset;
            to = multi.count;
            #if FFS_ECS_DEBUG
            data = multi.data;
            blockIdx = multi.blockIdx;
            data.Block(new Slot(blockIdx, offset));
            #endif
        }

        public readonly ref C Current {
            [MethodImpl(AggressiveInlining)] get => ref values[offset + to];
        }

        [MethodImpl(AggressiveInlining)]
        public bool MoveNext() => to-- > 0;

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose() {
            data.Unblock(new Slot(blockIdx, offset));
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public ref struct ROMultiComponentsIterator<C> where C : struct {
        #if FFS_ECS_DEBUG
        internal MultiComponents<C> data;
        internal uint blockIdx;
        #endif
        internal C[] values;
        internal int to;
        internal byte offset;

        [MethodImpl(AggressiveInlining)]
        public ROMultiComponentsIterator(ROMulti<C> val) {
            values = val.multi.data.values[val.multi.blockIdx];
            offset = val.multi.dataOffset;
            to = val.multi.count;
            #if FFS_ECS_DEBUG
            data = val.multi.data;
            blockIdx = val.multi.blockIdx;
            data.Block(new Slot(blockIdx, offset));
            #endif
        }

        public readonly C Current {
            [MethodImpl(AggressiveInlining)] get => values[offset + to];
        }

        [MethodImpl(AggressiveInlining)]
        public bool MoveNext() => to-- > 0;

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose() {
            data.Unblock(new Slot(blockIdx, offset));
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public class MultiComponents<T> where T : struct {
        internal const int SLOTS_SHIFT = 8;
        internal const int SLOTS_MASK = 255;

        #if FFS_ECS_DEBUG
        private readonly System.Collections.Concurrent.ConcurrentDictionary<Slot, bool> _blocked = new();
        #endif

        internal T[][] values;
        private readonly Level[] _levels;
        private SpinLock _lock;
        internal int valuesCount;
        private readonly ushort _minSlotCapacity;
        private readonly byte _minLevel;

        [MethodImpl(AggressiveInlining)]
        public MultiComponents(uint minSlotCapacity) {
            _minSlotCapacity = (ushort) Math.Min(Math.Max(Utils.CalculateSize(minSlotCapacity), 4), short.MaxValue + 1);
            _levels = new Level[16];
            byte minLevel = 0;

            TryAddLevel(4, 2, 64);
            TryAddLevel(8, 3, 32);
            TryAddLevel(16, 4, 16);
            TryAddLevel(32, 5, 8);
            TryAddLevel(64, 6, 4);
            TryAddLevel(128, 7, 2);
            TryAddLevel(256, 8, 1);
            TryAddLevel(512, 9, 1);
            TryAddLevel(1024, 10, 1);
            TryAddLevel(2048, 11, 1);
            TryAddLevel(4096, 12, 1);
            TryAddLevel(8192, 13, 1);
            TryAddLevel(16384, 14, 1);
            TryAddLevel(32768, 15, 1);

            values = new T[64][];
            valuesCount = 0;
            _minLevel = minLevel;
            _lock = new SpinLock(false);
            return;

            void TryAddLevel(ushort slotCapacity, byte level, byte baseSlotsCount) {
                if (_minSlotCapacity <= slotCapacity) {
                    _levels[level] = new Level(slotCapacity, baseSlotsCount);
                }

                if (_minSlotCapacity == slotCapacity) minLevel = level;
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal void Add(ref Multi<T> value) {
            AddForLevel(ref value, _minLevel);
        }

        [MethodImpl(AggressiveInlining)]
        internal void Add(ref Multi<T> value, uint slotCapacity) {
            AddForLevel(ref value, SlotCapacityToLevel(Math.Max(Utils.CalculateSize(slotCapacity), _minSlotCapacity)));
        }

        [MethodImpl(AggressiveInlining)]
        private void AddForLevel(ref Multi<T> value, byte levelIdx) {
            ref var level = ref _levels[levelIdx];

            var freeSlotsSeq = level.TryGetNextFree();
            if (freeSlotsSeq < 0) {
                freeSlotsSeq = AddNewDataBlock(ref level);
            }

            var slot = level.FreeSlots[freeSlotsSeq >> SLOTS_SHIFT][freeSlotsSeq & SLOTS_MASK];
            value.Init(this, slot.BlockIdx, 0, levelIdx, slot.Offset);
        }

        [MethodImpl(AggressiveInlining)]
        internal void Delete(ref Multi<T> value) {
            value.Clear();

            _levels[value.level].FreeSlot(new Slot(value.blockIdx, value.dataOffset));
            value = default;
        }

        [MethodImpl(AggressiveInlining)]
        internal void Resize(ref Multi<T> value, uint newCapacity) {
            var levelIdx = SlotCapacityToLevel(newCapacity);
            ref var level = ref _levels[levelIdx];
            var freeSlotsSeq = level.TryGetNextFree();
            if (freeSlotsSeq < 0) {
                freeSlotsSeq = AddNewDataBlock(ref level);
            }

            var newSlot = level.FreeSlots[freeSlotsSeq >> SLOTS_SHIFT][freeSlotsSeq & SLOTS_MASK];
            var count = value.count;

            Utils.LoopFallbackCopy(values[value.blockIdx], value.dataOffset, values[newSlot.BlockIdx], newSlot.Offset, count);
            Delete(ref value);
            value.Init(this, newSlot.BlockIdx, count, levelIdx, newSlot.Offset);
        }

        [MethodImpl(AggressiveInlining)]
        private int AddNewDataBlock(ref Level level) {
            var enter = false;
            level.Lock.Enter(ref enter);
            #if FFS_ECS_DEBUG
            if (!enter) throw new StaticEcsException("Failed to acquire multi components lock");
            #endif
            var result = level.TryGetNextFree();
            while (result < 0) {
                if (valuesCount == values.Length) {
                    Resize();
                }

                var valueIdx = Interlocked.Increment(ref valuesCount) - 1;
                values[valueIdx] = new T[level.InitDataBlock((uint) valueIdx)];
                result = level.TryGetNextFree();
            }

            level.Lock.Exit();
            return result;
        }

        [MethodImpl(NoInlining)]
        private void Resize() {
            var enter = false;
            _lock.Enter(ref enter);
            #if FFS_ECS_DEBUG
            if (!enter) throw new StaticEcsException("Failed to acquire multi components lock");
            #endif
            if (valuesCount == values.Length) {
                Array.Resize(ref values, values.Length << 1);
            }

            _lock.Exit();
        }

        [MethodImpl(AggressiveInlining)]
        private static byte SlotCapacityToLevel(uint x) => (byte) (((BitConverter.DoubleToInt64Bits(x) >> 52) + 1) & 0xFF);

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        internal bool IsBlocked(ref Multi<T> value) {
            return _blocked.ContainsKey(new Slot(value.blockIdx, value.dataOffset));
        }

        [MethodImpl(AggressiveInlining)]
        internal void Block(Slot slot) {
            _blocked[slot] = true;
        }

        [MethodImpl(AggressiveInlining)]
        internal void Unblock(Slot slot) {
            _blocked.TryRemove(slot, out var _);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct Level {
        public Slot[][] FreeSlots;
        public SpinLock Lock;
        public int CurSlots;
        public int FreeSlotsSeq;
        public readonly ushort SlotCapacity;
        public readonly byte BaseSlotsCount;
        public byte ResizeCounter;

        internal const int BASE_SLOTS_CAPACITY = 256;
        internal const int SHIFT = 8;
        internal const int MASK = 255;

        public Level(ushort slotCapacity, byte baseSlotsCount) {
            FreeSlots = new Slot[32][];
            SlotCapacity = slotCapacity;
            FreeSlotsSeq = 0;
            CurSlots = 0;
            BaseSlotsCount = baseSlotsCount;
            ResizeCounter = 1;
            Lock = new SpinLock(false);
        }

        [MethodImpl(AggressiveInlining)]
        public int TryGetNextFree() {
            var current = Volatile.Read(ref FreeSlotsSeq);
            while (true) {
                if (current == 0) return -1;
                var next = current - 1;
                var observed = Interlocked.CompareExchange(ref FreeSlotsSeq, next, current);
                if (observed == current) return next;
                current = observed;
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void FreeSlot(Slot slot) {
            var idx = Interlocked.Increment(ref FreeSlotsSeq) - 1;
            FreeSlots[idx >> SHIFT][idx & MASK] = slot;
        }

        [MethodImpl(AggressiveInlining)]
        public int InitDataBlock(uint blockIdx) {
            var capacity = SlotCapacity * BaseSlotsCount;

            ResizeCounter--;
            if (ResizeCounter == 0) {
                ResizeCounter = (byte) (BASE_SLOTS_CAPACITY / BaseSlotsCount);
                if (CurSlots == FreeSlots.Length) {
                    Array.Resize(ref FreeSlots, FreeSlots.Length << 1);
                }

                FreeSlots[CurSlots++] = new Slot[BASE_SLOTS_CAPACITY];
            }

            int from, to;
            do {
                from = FreeSlotsSeq;
                to = from + BaseSlotsCount;
            } while (Interlocked.CompareExchange(ref FreeSlotsSeq, to, from) != from);

            for (uint offset = 0; offset < capacity; offset += SlotCapacity) {
                FreeSlots[from >> SHIFT][from & MASK] = new Slot(blockIdx, (byte) offset);
                from++;
            }

            return capacity;
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Slot : IEquatable<Slot> {
        private const int OffsetBits = 8;
        private const uint OffsetMask = (1u << OffsetBits) - 1;

        private readonly uint _value;

        public Slot(uint blockIdx, byte offset) {
            _value = (blockIdx << OffsetBits) | (offset & OffsetMask);
        }

        public uint BlockIdx {
            [MethodImpl(AggressiveInlining)] get => _value >> OffsetBits;
        }

        public byte Offset {
            [MethodImpl(AggressiveInlining)] get => (byte) (_value & OffsetMask);
        }

        [MethodImpl(AggressiveInlining)]
        public override string ToString() => $"BlockIdx={BlockIdx}, Offset={Offset}";

        [MethodImpl(AggressiveInlining)]
        public bool Equals(Slot other) => _value == other._value;

        [MethodImpl(AggressiveInlining)]
        public override bool Equals(object obj) => obj is Slot other && Equals(other);

        [MethodImpl(AggressiveInlining)]
        public override int GetHashCode() => (int) _value;
    }
}