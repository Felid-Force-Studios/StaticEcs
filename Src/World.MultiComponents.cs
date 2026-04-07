#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    /// <summary>
    /// Interface for types that can be stored as elements inside <see cref="World{TWorld}.Multi{TValue}"/>.
    /// The implementing type must be a <c>struct</c>. This constraint separates multi-component
    /// value types from regular <see cref="IComponent"/> types, preventing accidental misuse.
    /// <para>
    /// For non-unmanaged types, <see cref="Write"/> and <see cref="Read"/> hooks must be implemented
    /// to enable serialization. Unmanaged types without hooks use bulk memory copy automatically.
    /// </para>
    /// </summary>
    public interface IMultiComponent {
        /// <summary>
        /// Custom serialization hook for writing this element to a binary stream.
        /// Required for non-unmanaged types. Unmanaged types without this hook use bulk memory copy.
        /// </summary>
        void Write(ref BinaryPackWriter writer) {}

        /// <summary>
        /// Custom deserialization hook for reading this element from a binary stream.
        /// Required for non-unmanaged types. Unmanaged types without this hook use bulk memory copy.
        /// </summary>
        void Read(ref BinaryPackReader reader) {}
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public abstract partial class World<TWorld> {

        
        /// <summary>
        /// A dynamically-sized list of <typeparamref name="TValue"/> elements stored as a component on an entity.
        /// Unlike regular components (one value per entity), <c>Multi&lt;TValue&gt;</c> holds a variable-length
        /// collection backed by <see cref="SegmentAllocator"/> with power-of-2 size classes (capacity 4..32768).
        /// <para>
        /// <b>Storage model:</b> The struct itself (12 bytes) is stored in the component array like any other
        /// component. The actual values live in <c>MultiValueStorage&lt;TValue&gt;</c> segments, accessed via
        /// <see cref="Offset"/> and <see cref="SegmentIdx"/>. Allocation and growth are managed automatically.
        /// </para>
        /// <para>
        /// <b>Lifecycle:</b> Storage is allocated in <see cref="IComponent.OnAdd{TW}"/> and freed in
        /// <see cref="IComponent.OnDelete{TW}"/>. Adding/removing the <c>Multi&lt;TValue&gt;</c> component
        /// on an entity controls the lifetime of the underlying value storage.
        /// </para>
        /// <para>
        /// <b>Important:</b> This is a mutable struct. Always access it via <c>ref</c> (e.g., <c>ref var multi = ref entity.Ref&lt;Multi&lt;T&gt;&gt;()</c>)
        /// to ensure modifications are written back to component storage.
        /// </para>
        /// </summary>
        /// <typeparam name="TValue">A struct type implementing <see cref="IMultiComponent"/>.</typeparam>
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        public struct Multi<TValue> : IComponent, IComponentInternal, IEquatable<Multi<TValue>> where TValue : struct, IMultiComponent {
            internal static IPackArrayStrategy<TValue> ElementStrategy;

            internal uint Offset;
            internal uint SegmentIdx;
            internal ushort Count;
            internal byte Level;
            internal byte SegmentEntityIdx;

            /// <summary>
            /// The number of elements currently stored. Always &lt;= <see cref="Capacity"/>.
            /// </summary>
            public readonly ushort Length {
                [MethodImpl(AggressiveInlining)] get => Count;
            }

            /// <summary>
            /// The current allocation capacity (always a power of 2, minimum 4).
            /// Capacity grows automatically when elements are added beyond it.
            /// </summary>
            public readonly ushort Capacity {
                [MethodImpl(AggressiveInlining)] get => (ushort)(SegmentAllocator.MinSlotCapacity << Level);
            }

            /// <summary>
            /// Whether this multi-component contains no elements (<see cref="Length"/> == 0).
            /// </summary>
            public readonly bool IsEmpty {
                [MethodImpl(AggressiveInlining)] get => Count == 0;
            }

            /// <summary>
            /// Whether this multi-component contains at least one element.
            /// </summary>
            public readonly bool IsNotEmpty {
                [MethodImpl(AggressiveInlining)] get => Count != 0;
            }

            /// <summary>
            /// Whether <see cref="Length"/> equals <see cref="Capacity"/>. Adding more elements will trigger a grow.
            /// </summary>
            public readonly bool IsFull {
                [MethodImpl(AggressiveInlining)] get => Count == SegmentAllocator.MinSlotCapacity << Level;
            }

            /// <summary>
            /// Returns a mutable reference to the element at the given index.
            /// </summary>
            /// <param name="idx">Zero-based index. Must be in range [0, <see cref="Length"/>).</param>
            /// <exception cref="StaticEcsException">Thrown in debug builds if <paramref name="idx"/> is out of range.</exception>
            public readonly ref TValue this[int idx] {
                [MethodImpl(AggressiveInlining)]
                get {
                    #if FFS_ECS_DEBUG
                    if (idx < 0 || idx >= Count) {
                        throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.Indexer ] index out of bounds: {idx}, count: {Count}");
                    }
                    #endif
                    return ref Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx][Offset + idx];
                }
            }

            /// <summary>
            /// Returns a <see cref="Span{T}"/> over the currently stored elements.
            /// The span is valid only while the multi-component is not modified (no Add/Remove/Grow).
            /// </summary>
            public readonly Span<TValue> AsSpan {
                [MethodImpl(AggressiveInlining)] get => new(Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx], (int)Offset, Count);
            }
            
            /// <summary>
            /// Returns a <see cref="ReadOnlySpan{T}"/> over the currently stored elements.
            /// </summary>
            public readonly ReadOnlySpan<TValue> AsReadOnlySpan {
                [MethodImpl(AggressiveInlining)] get => new(Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx], (int)Offset, Count);
            }

            /// <summary>
            /// Returns a mutable reference to the first element.
            /// </summary>
            /// <exception cref="StaticEcsException">Thrown in debug builds if the collection is empty.</exception>
            [MethodImpl(AggressiveInlining)]
            public readonly ref TValue First() {
                #if FFS_ECS_DEBUG
                if (Count == 0) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.First ] empty");
                #endif
                return ref Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx][Offset];
            }

            /// <summary>
            /// Returns a mutable reference to the last element.
            /// </summary>
            /// <exception cref="StaticEcsException">Thrown in debug builds if the collection is empty.</exception>
            [MethodImpl(AggressiveInlining)]
            public readonly ref TValue Last() {
                #if FFS_ECS_DEBUG
                if (Count == 0) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.Last ] empty");
                #endif
                return ref Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx][Offset + Count - 1];
            }

            #region ADD
            /// <summary>
            /// Appends one element to the end of the collection. Grows capacity automatically if full.
            /// </summary>
            /// <param name="val">The value to append.</param>
            /// <exception cref="StaticEcsException">Thrown in debug builds if modified during iteration or max capacity exceeded.</exception>
            [MethodImpl(AggressiveInlining)]
            public void Add(TValue val) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                #endif
                ref var storage = ref Resources<MultiValueStorage<TValue>>.Value;
                if (Count >= SegmentAllocator.MinSlotCapacity << Level) {
                    Grow(ref storage);
                }

                storage.Segments[SegmentIdx][Offset + Count++] = val;
            }

            /// <inheritdoc cref="Add(TValue)"/>
            /// <param name="val1">First value to append.</param>
            /// <param name="val2">Second value to append.</param>
            [MethodImpl(AggressiveInlining)]
            public void Add(TValue val1, TValue val2) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                #endif
                ref var storage = ref Resources<MultiValueStorage<TValue>>.Value;
                var cap = SegmentAllocator.MinSlotCapacity << Level;
                if (Count + 1 >= cap) {
                    EnsureCapacityInternal(ref storage, (ushort)(Count + 2));
                }

                var values = storage.Segments[SegmentIdx];
                values[Offset + Count++] = val1;
                values[Offset + Count++] = val2;
            }

            /// <inheritdoc cref="Add(TValue)"/>
            /// <param name="val1">First value to append.</param>
            /// <param name="val2">Second value to append.</param>
            /// <param name="val3">Third value to append.</param>
            [MethodImpl(AggressiveInlining)]
            public void Add(TValue val1, TValue val2, TValue val3) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                #endif
                ref var storage = ref Resources<MultiValueStorage<TValue>>.Value;
                var cap = SegmentAllocator.MinSlotCapacity << Level;
                if (Count + 2 >= cap) {
                    EnsureCapacityInternal(ref storage, (ushort)(Count + 3));
                }

                var values = storage.Segments[SegmentIdx];
                values[Offset + Count++] = val1;
                values[Offset + Count++] = val2;
                values[Offset + Count++] = val3;
            }

            /// <inheritdoc cref="Add(TValue)"/>
            /// <param name="val1">First value to append.</param>
            /// <param name="val2">Second value to append.</param>
            /// <param name="val3">Third value to append.</param>
            /// <param name="val4">Fourth value to append.</param>
            [MethodImpl(AggressiveInlining)]
            public void Add(TValue val1, TValue val2, TValue val3, TValue val4) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                #endif
                ref var storage = ref Resources<MultiValueStorage<TValue>>.Value;
                var cap = SegmentAllocator.MinSlotCapacity << Level;
                if (Count + 3 >= cap) {
                    EnsureCapacityInternal(ref storage, (ushort)(Count + 4));
                }

                var values = storage.Segments[SegmentIdx];
                values[Offset + Count++] = val1;
                values[Offset + Count++] = val2;
                values[Offset + Count++] = val3;
                values[Offset + Count++] = val4;
            }

            /// <summary>
            /// Appends all elements from the source array. Grows capacity automatically if needed.
            /// </summary>
            /// <param name="src">Source array to copy elements from.</param>
            /// <exception cref="StaticEcsException">Thrown in debug builds if <paramref name="src"/> is null or modified during iteration.</exception>
            [MethodImpl(AggressiveInlining)]
            public void Add(TValue[] src) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if (src == null) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.Add ] src is null");
                #endif
                Add(src, 0, (ushort)src.Length);
            }

            /// <summary>
            /// Appends a range of elements from the source array. Grows capacity automatically if needed.
            /// </summary>
            /// <param name="src">Source array to copy elements from.</param>
            /// <param name="srcIdx">Starting index in the source array.</param>
            /// <param name="len">Number of elements to copy.</param>
            /// <exception cref="StaticEcsException">Thrown in debug builds if <paramref name="src"/> is null, range is out of bounds, or modified during iteration.</exception>
            [MethodImpl(AggressiveInlining)]
            public void Add(TValue[] src, int srcIdx, ushort len) {
                if (len > 0) {
                    #if FFS_ECS_DEBUG
                    AssertNotBlockedByIteration();
                    if (src == null) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.Add ] src is null");
                    if (srcIdx + len > src.Length) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.Add ] srcIdx + len > src.Length");
                    #endif
                    ref var storage = ref Resources<MultiValueStorage<TValue>>.Value;
                    var cap = SegmentAllocator.MinSlotCapacity << Level;
                    if (Count + len > cap) {
                        EnsureCapacityInternal(ref storage, (ushort)(Count + len));
                    }

                    Utils.LoopFallbackCopy(src, (uint)srcIdx, storage.Segments[SegmentIdx], Offset + Count, len);
                    Count += len;
                }
            }
            #endregion

            #region REMOVE
            /// <summary>
            /// Removes the element at the specified index, shifting all subsequent elements left by one.
            /// The freed slot at the end is cleared to <c>default</c>.
            /// </summary>
            /// <param name="idx">Zero-based index of the element to remove.</param>
            /// <exception cref="StaticEcsException">Thrown in debug builds if <paramref name="idx"/> is out of range or modified during iteration.</exception>
            [MethodImpl(AggressiveInlining)]
            public void RemoveAt(int idx) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if (idx < 0 || idx >= Count) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.RemoveAt ] index out of bounds: {idx}");
                #endif
                var values = Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx];
                Count--;
                if (idx < Count) {
                    Utils.LoopFallbackCopy(values, Offset + (uint)idx + 1, values, Offset + (uint)idx, (uint)(Count - idx));
                }
                if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>()) {
                    values[Offset + Count] = default;
                }
            }

            /// <summary>
            /// Removes the element at the specified index by swapping it with the last element. O(1) but does not preserve order.
            /// The freed slot at the end is cleared to <c>default</c> only for types containing managed references.
            /// </summary>
            /// <param name="idx">Zero-based index of the element to remove.</param>
            /// <exception cref="StaticEcsException">Thrown in debug builds if <paramref name="idx"/> is out of range or modified during iteration.</exception>
            [MethodImpl(AggressiveInlining)]
            public void RemoveAtSwap(int idx) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if (idx < 0 || idx >= Count) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.RemoveAtSwap ] index out of bounds: {idx}");
                #endif
                var values = Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx];
                values[Offset + idx] = values[Offset + --Count];
                if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>()) {
                    values[Offset + Count] = default;
                }
            }

            /// <summary>
            /// Removes the first element, shifting all subsequent elements left by one.
            /// </summary>
            /// <exception cref="StaticEcsException">Thrown in debug builds if the collection is empty or modified during iteration.</exception>
            [MethodImpl(AggressiveInlining)]
            public void RemoveFirst() {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if (Count == 0) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.RemoveFirst ] empty");
                #endif
                var values = Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx];
                Count--;
                if (Count > 0) {
                    Utils.LoopFallbackCopy(values, Offset + 1, values, Offset, Count);
                }
                if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>()) {
                    values[Offset + Count] = default;
                }
            }

            /// <summary>
            /// Removes the first element by swapping it with the last element. O(1) but does not preserve order.
            /// </summary>
            /// <exception cref="StaticEcsException">Thrown in debug builds if the collection is empty or modified during iteration.</exception>
            [MethodImpl(AggressiveInlining)]
            public void RemoveFirstSwap() {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if (Count == 0) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.RemoveFirstSwap ] empty");
                #endif
                var values = Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx];
                values[Offset] = values[Offset + --Count];
                if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>()) {
                    values[Offset + Count] = default;
                }
            }

            /// <summary>
            /// Removes the last element.
            /// </summary>
            /// <exception cref="StaticEcsException">Thrown in debug builds if the collection is empty or modified during iteration.</exception>
            [MethodImpl(AggressiveInlining)]
            public void RemoveLast() {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if (Count == 0) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.RemoveLast ] empty");
                #endif
                Count--;
                if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>()) {
                    var values = Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx];
                    values[Offset + Count] = default;
                }
            }

            /// <summary>
            /// Searches for the specified item and removes the first occurrence using <see cref="RemoveAt"/>.
            /// </summary>
            /// <param name="item">The value to find and remove.</param>
            /// <returns><c>true</c> if the item was found and removed; <c>false</c> otherwise.</returns>
            [MethodImpl(AggressiveInlining)]
            public bool TryRemove(TValue item) {
                var idx = IndexOf(item);
                if (idx >= 0) {
                    RemoveAt(idx);
                    return true;
                }

                return false;
            }
            
            /// <summary>
            /// Searches for each of the two specified items and removes the first occurrence of each using <see cref="RemoveAt"/>.
            /// Items that are not found are silently skipped.
            /// </summary>
            /// <param name="item1">First value to find and remove.</param>
            /// <param name="item2">Second value to find and remove.</param>
            [MethodImpl(AggressiveInlining)]
            public void TryRemove(TValue item1, TValue item2) {
                var idx = IndexOf(item1);
                if (idx >= 0) {
                    RemoveAt(idx);
                }
                
                idx = IndexOf(item2);
                if (idx >= 0) {
                    RemoveAt(idx);
                }
            }

            /// <summary>
            /// Searches for the specified item and removes the first occurrence using <see cref="RemoveAtSwap"/>.
            /// O(1) removal but does not preserve element order.
            /// </summary>
            /// <param name="item">The value to find and remove.</param>
            /// <returns><c>true</c> if the item was found and removed; <c>false</c> otherwise.</returns>
            [MethodImpl(AggressiveInlining)]
            public bool TryRemoveSwap(TValue item) {
                var idx = IndexOf(item);
                if (idx >= 0) {
                    RemoveAtSwap(idx);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Removes all elements, zeroing the underlying storage slots and resetting <see cref="Length"/> to 0.
            /// Capacity is unchanged.
            /// </summary>
            /// <exception cref="StaticEcsException">Thrown in debug builds if modified during iteration.</exception>
            [MethodImpl(AggressiveInlining)]
            public void Clear() {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                #endif
                if (Count > 0) {
                    if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>()) {
                        var values = Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx];
                        Utils.LoopFallbackClear(values, (int)Offset, Count);
                    }
                    Count = 0;
                }
            }

            /// <summary>
            /// Sets <see cref="Length"/> to 0 without clearing the underlying data. This is a low-level operation;
            /// the old values remain in storage until overwritten. Prefer <see cref="Clear"/> unless you specifically
            /// need to avoid the clearing cost.
            /// </summary>
            /// <exception cref="StaticEcsException">Thrown in debug builds if modified during iteration.</exception>
            [MethodImpl(AggressiveInlining)]
            public void ResetCount() {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                #endif
                Count = 0;
            }
            #endregion

            #region INSERT
            /// <summary>
            /// Inserts a value at the specified position, shifting all subsequent elements right by one.
            /// Grows capacity automatically if full.
            /// </summary>
            /// <param name="idx">Zero-based insertion index. Must be in range [0, <see cref="Length"/>].</param>
            /// <param name="value">The value to insert.</param>
            /// <exception cref="StaticEcsException">Thrown in debug builds if <paramref name="idx"/> is out of range or modified during iteration.</exception>
            [MethodImpl(AggressiveInlining)]
            public void InsertAt(int idx, TValue value) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if (idx < 0 || idx > Count) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.InsertAt ] index out of bounds: {idx}");
                #endif
                ref var storage = ref Resources<MultiValueStorage<TValue>>.Value;
                if (Count >= SegmentAllocator.MinSlotCapacity << Level) {
                    Grow(ref storage);
                }

                var values = storage.Segments[SegmentIdx];
                if (idx < Count) {
                    Utils.LoopFallbackCopyReverse(values, Offset + (uint)idx, values, Offset + (uint)idx + 1, (uint)(Count - idx));
                }

                values[Offset + idx] = value;
                Count++;
            }
            #endregion

            #region UTILITY
            /// <summary>
            /// Returns the zero-based index of the first occurrence of the specified item, or -1 if not found.
            /// Uses <see cref="Array.IndexOf{T}(T[], T, int, int)"/> internally.
            /// </summary>
            /// <param name="item">The value to locate.</param>
            /// <returns>Index of the item within the collection, or -1.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly int IndexOf(TValue item) {
                var values = Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx];
                var indexOf = Array.IndexOf(values, item, (int)Offset, Count);
                return indexOf >= 0 ? (int)(indexOf - Offset) : -1;
            }

            /// <summary>
            /// Determines whether the collection contains the specified item, using <see cref="EqualityComparer{T}.Default"/>.
            /// </summary>
            /// <param name="item">The value to locate.</param>
            /// <returns><c>true</c> if <paramref name="item"/> is found; <c>false</c> otherwise.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly bool Contains(TValue item) {
                var equalityComparer = EqualityComparer<TValue>.Default;
                var values = Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx];
                for (var i = Offset; i < Offset + Count; i++) {
                    if (equalityComparer.Equals(values[i], item)) return true;
                }

                return false;
            }

            /// <summary>
            /// Determines whether the collection contains the specified item, using the provided custom comparer.
            /// </summary>
            /// <typeparam name="TComparer">Type implementing <see cref="IEqualityComparer{T}"/>.</typeparam>
            /// <param name="item">The value to locate.</param>
            /// <param name="comparer">The equality comparer to use.</param>
            /// <returns><c>true</c> if <paramref name="item"/> is found; <c>false</c> otherwise.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly bool Contains<TComparer>(TValue item, TComparer comparer) where TComparer : IEqualityComparer<TValue> {
                var values = Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx];
                for (var i = Offset; i < Offset + Count; i++) {
                    if (comparer.Equals(values[i], item)) return true;
                }

                return false;
            }

            /// <summary>
            /// Ensures that the underlying storage can accommodate <paramref name="additionalSize"/> more elements
            /// beyond the current <see cref="Length"/>, growing capacity if necessary. Does not change <see cref="Length"/>.
            /// </summary>
            /// <param name="additionalSize">Number of additional elements to reserve space for.</param>
            /// <exception cref="StaticEcsException">Thrown in debug builds if the resulting capacity would exceed the maximum or modified during iteration.</exception>
            [MethodImpl(AggressiveInlining)]
            public void EnsureSize(int additionalSize) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if ((long)Count + additionalSize > SegmentAllocator.MinSlotCapacity << (SegmentAllocator.LevelCount - 1)) {
                    throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.EnsureSize ] capacity overflow: {Count} + {additionalSize}");
                }
                #endif
                var cap = SegmentAllocator.MinSlotCapacity << Level;
                if (Count + additionalSize > cap) {
                    ref var storage = ref Resources<MultiValueStorage<TValue>>.Value;
                    EnsureCapacityInternal(ref storage, (ushort)(Count + additionalSize));
                }
            }
            
            /// <summary>
            /// Ensures capacity for <paramref name="count"/> additional elements AND increases <see cref="Length"/>
            /// by that amount. For newly added slots are zero-initialized. 
            /// Use <see cref="EnsureCountUninitialized"/> explicitly if you want to skip initialization for all types.
            /// </summary>
            /// <param name="count">Number of additional elements to reserve and account for in <see cref="Length"/>.</param>
            /// <exception cref="StaticEcsException">Thrown in debug builds if the resulting capacity would exceed the maximum or modified during iteration.</exception>
            [MethodImpl(AggressiveInlining)]
            public void EnsureCount(ushort count) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if (Count + count > SegmentAllocator.MinSlotCapacity << (SegmentAllocator.LevelCount - 1)) {
                    throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.EnsureCount ] capacity overflow: {Count} + {count}");
                }
                #endif
                var cap = SegmentAllocator.MinSlotCapacity << Level;
                ref var storage = ref Resources<MultiValueStorage<TValue>>.Value;
                if (Count + count > cap) {
                    EnsureCapacityInternal(ref storage, (ushort)(Count + count));
                }

                var oldCount = Count;
                Count += count;
                Utils.LoopFallbackClear(storage.Segments[SegmentIdx], (int)(Offset + oldCount), count);
            }

            /// <summary>
            /// Ensures capacity for <paramref name="count"/> additional elements AND increases <see cref="Length"/>
            /// by that amount. The newly added slots are <b>NOT</b> zero-initialized and may contain stale data.
            /// The caller MUST write all new slots before reading them. Useful for bulk-filling via the indexer.
            /// </summary>
            /// <param name="count">Number of additional elements to reserve and account for in <see cref="Length"/>.</param>
            /// <exception cref="StaticEcsException">Thrown in debug builds if the resulting capacity would exceed the maximum or modified during iteration.</exception>
            [MethodImpl(AggressiveInlining)]
            public void EnsureCountUninitialized(ushort count) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if (Count + count > SegmentAllocator.MinSlotCapacity << (SegmentAllocator.LevelCount - 1)) {
                    throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.EnsureCountUninitialized ] capacity overflow: {Count} + {count}");
                }
                #endif
                var cap = SegmentAllocator.MinSlotCapacity << Level;
                if (Count + count > cap) {
                    ref var storage = ref Resources<MultiValueStorage<TValue>>.Value;
                    EnsureCapacityInternal(ref storage, (ushort)(Count + count));
                }

                Count += count;
            }

            /// <summary>
            /// Ensures that the capacity is at least <paramref name="newCapacity"/>. If current capacity is already
            /// sufficient, this is a no-op. Does not change <see cref="Length"/>.
            /// </summary>
            /// <param name="newCapacity">The minimum desired capacity.</param>
            /// <exception cref="StaticEcsException">Thrown in debug builds if modified during iteration.</exception>
            [MethodImpl(AggressiveInlining)]
            public void Resize(ushort newCapacity) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                #endif
                var cap = SegmentAllocator.MinSlotCapacity << Level;
                if (cap < newCapacity) {
                    ref var storage = ref Resources<MultiValueStorage<TValue>>.Value;
                    EnsureCapacityInternal(ref storage, newCapacity);
                }
            }

            /// <summary>
            /// Copies all elements to the destination array, starting at index 0 of <paramref name="dst"/>.
            /// </summary>
            /// <param name="dst">Destination array. Must have length &gt;= <see cref="Length"/>.</param>
            /// <exception cref="StaticEcsException">Thrown in debug builds if <paramref name="dst"/> is null or too small.</exception>
            [MethodImpl(AggressiveInlining)]
            public readonly void CopyTo(TValue[] dst) {
                #if FFS_ECS_DEBUG
                if (dst == null) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.CopyTo ] dst is null");
                if (Count > dst.Length) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.CopyTo ] count > dst.Length");
                #endif
                Utils.LoopFallbackCopy(Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx], Offset, dst, 0, Count);
            }

            /// <summary>
            /// Copies <paramref name="len"/> elements to the destination array starting at <paramref name="dstIdx"/>.
            /// </summary>
            /// <param name="dst">Destination array.</param>
            /// <param name="dstIdx">Starting index in the destination array.</param>
            /// <param name="len">Number of elements to copy. Must be &lt;= <see cref="Length"/>.</param>
            /// <exception cref="StaticEcsException">Thrown in debug builds if arguments are out of range or <paramref name="dst"/> is null.</exception>
            [MethodImpl(AggressiveInlining)]
            public readonly void CopyTo(TValue[] dst, int dstIdx, int len) {
                #if FFS_ECS_DEBUG
                if (dst == null) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.CopyTo ] dst is null");
                if (dstIdx + len > dst.Length) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.CopyTo ] dstIdx + len > dst.Length");
                if (len > Count) throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.CopyTo ] len > count");
                #endif
                Utils.LoopFallbackCopy(Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx], Offset, dst, (uint)dstIdx, (uint)len);
            }

            /// <summary>
            /// Sorts the elements in place using the default comparer. <typeparamref name="TValue"/> must implement
            /// <see cref="IComparable{T}"/> or <see cref="IComparable"/>.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly void Sort() {
                Array.Sort(Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx], (int)Offset, Count);
            }

            /// <summary>
            /// Sorts the elements in place using the specified comparer.
            /// </summary>
            /// <typeparam name="TComparer">Type implementing <see cref="IComparer{T}"/>.</typeparam>
            /// <param name="comparer">The comparer to use for ordering.</param>
            [MethodImpl(AggressiveInlining)]
            public readonly void Sort<TComparer>(TComparer comparer) where TComparer : IComparer<TValue> {
                Array.Sort(Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx], (int)Offset, Count, comparer);
            }

            public readonly override string ToString() {
                var res = "";
                var values = Resources<MultiValueStorage<TValue>>.Value.Segments[SegmentIdx];
                for (int i = 0; i < Count; i++) {
                    res += values[Offset + i].ToString();
                    if (i + 1 < Count) res += ", ";
                }

                return res;
            }

            /// <summary>
            /// Returns a <see cref="MultiComponentsIterator{TValue}"/> that iterates elements in reverse order,
            /// yielding mutable <c>ref TValue</c> references. Supports <c>foreach</c> syntax.
            /// <para>In debug builds, blocks modification of this multi-component during iteration.</para>
            /// </summary>
            /// <returns>A reverse iterator over the elements.</returns>
            [MethodImpl(AggressiveInlining)]
            public MultiComponentsIterator<TValue> GetEnumerator() => new(this);

            /// <summary>
            /// Determines value equality by comparing all internal fields (Offset, Count, SegmentIdx, Level, SegmentEntityIdx).
            /// Two <see cref="Multi{TValue}"/> values are equal if and only if they reference the exact same storage region.
            /// </summary>
            /// <param name="other">The other instance to compare with.</param>
            /// <returns><c>true</c> if all internal fields match; <c>false</c> otherwise.</returns>
            [MethodImpl(AggressiveInlining)]
            public bool Equals(Multi<TValue> other) {
                return Offset == other.Offset && Count == other.Count && SegmentIdx == other.SegmentIdx && Level == other.Level &&
                       SegmentEntityIdx == other.SegmentEntityIdx;
            }

            [MethodImpl(AggressiveInlining)]
            public override bool Equals(object obj) => throw new StaticEcsException($"Multi<{typeof(TValue)}> `Equals object` not allowed!");

            [MethodImpl(AggressiveInlining)]
            public override int GetHashCode() => throw new StaticEcsException($"Multi<{typeof(TValue)}> `GetHashCode object` not allowed!");

            [MethodImpl(AggressiveInlining)]
            public static bool operator ==(Multi<TValue> left, Multi<TValue> right) => left.Equals(right);

            [MethodImpl(AggressiveInlining)]
            public static bool operator !=(Multi<TValue> left, Multi<TValue> right) => !left.Equals(right);
            #endregion

            #region ICOMPONENT
            [MethodImpl(AggressiveInlining)]
            public void OnAdd<TW>(World<TW>.Entity self) where TW : struct, IWorldType {
                var entityId = self.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentEntityIdx = (byte)(entityId & Const.ENTITIES_IN_SEGMENT_MASK);

                ref var storage = ref World<TW>.Resources<MultiValueStorage<TValue>>.Value;
                storage.EnsureSegment(segmentIdx);

                SegmentIdx = segmentIdx;
                SegmentEntityIdx = segmentEntityIdx;
                Level = 0;
                Offset = storage.Allocators[segmentIdx].Allocate(Level, ref storage.Segments[segmentIdx]);
                Count = 0;
            }

            [MethodImpl(AggressiveInlining)]
            public void OnDelete<TW>(World<TW>.Entity self, HookReason reason) where TW : struct, IWorldType {
                ref var storage = ref World<TW>.Resources<MultiValueStorage<TValue>>.Value;
                Clear();
                storage.Allocators[SegmentIdx].Free(Offset, Level, out var empty);
                if (empty) {
                    storage.ReleaseSegment(SegmentIdx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyTo<TW>(World<TW>.Entity self, World<TW>.Entity other, bool disabled) where TW : struct, IWorldType {
                ref var dst = ref World<TW>.Components<Multi<TValue>>.Instance.Add(other);
                ref var storage = ref World<TW>.Resources<MultiValueStorage<TValue>>.Value;

                var neededCapacity = (ushort)Utils.RoundUpToPowerOf2(Math.Max(Count, SegmentAllocator.MinSlotCapacity));
                var neededLevel = SegmentAllocator.LevelForCapacity(neededCapacity);

                if (dst.Level < neededLevel) {
                    storage.Allocators[dst.SegmentIdx].Free(dst.Offset, dst.Level, out var empty);
                    if (empty) {
                        storage.ReleaseSegment(dst.SegmentIdx);
                        storage.EnsureSegment(dst.SegmentIdx);
                    }
                    dst.Offset = storage.Allocators[dst.SegmentIdx].Allocate(neededLevel, ref storage.Segments[dst.SegmentIdx]);
                    dst.Level = neededLevel;
                }

                if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>()) {
                    if (dst.Count > Count) {
                        Utils.LoopFallbackClear(storage.Segments[dst.SegmentIdx], (int)(dst.Offset + Count), dst.Count - Count);
                    }
                }
                Utils.LoopFallbackCopy(storage.Segments[SegmentIdx], Offset, storage.Segments[dst.SegmentIdx], dst.Offset, Count);
                dst.Count = Count;

                if (disabled) {
                    World<TW>.Components<Multi<TValue>>.Instance.Disable(other);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public void Write<TW>(ref BinaryPackWriter writer, World<TW>.Entity self) where TW : struct, IWorldType {
                writer.WriteUshort(Count);
                if (Count > 0) {
                    ref var storage = ref World<TW>.Resources<MultiValueStorage<TValue>>.Value;
                    if (storage.ElementStrategy.IsUnmanaged()) {
                        storage.ElementStrategy.WriteArray(ref writer, storage.Segments[SegmentIdx], (int)Offset, Count);
                    } else {
                        #if FFS_ECS_DEBUG
                        if (!storage.HasWriteHook) {
                            throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.Write ] Non-unmanaged IMultiComponent type must implement Write hook for serialization");
                        }
                        #endif
                        var segment = storage.Segments[SegmentIdx];
                        for (var i = 0; i < Count; i++) {
                            segment[(int)Offset + i].Write(ref writer);
                        }
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            public void Read<TW>(ref BinaryPackReader reader, World<TW>.Entity self, byte version, bool disabled) where TW : struct, IWorldType {
                Count = reader.ReadUshort();
                var entityId = self.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentEntityIdx = (byte)(entityId & Const.ENTITIES_IN_SEGMENT_MASK);

                ref var storage = ref World<TW>.Resources<MultiValueStorage<TValue>>.Value;
                storage.EnsureSegment(segmentIdx);

                SegmentIdx = segmentIdx;
                SegmentEntityIdx = segmentEntityIdx;
                Level = SegmentAllocator.LevelForCapacity(Count);
                Offset = storage.Allocators[segmentIdx].Allocate(Level, ref storage.Segments[segmentIdx]);

                if (Count > 0) {
                    if (storage.ElementStrategy.IsUnmanaged()) {
                        storage.ElementStrategy.ReadArray(ref reader, ref storage.Segments[SegmentIdx], (int)Offset);
                    } else {
                        #if FFS_ECS_DEBUG
                        if (!storage.HasReadHook) {
                            throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.Read ] Non-unmanaged IMultiComponent type must implement Read hook for serialization");
                        }
                        #endif
                        var segment = storage.Segments[SegmentIdx];
                        for (var i = 0; i < Count; i++) {
                            segment[(int)Offset + i].Read(ref reader);
                        }
                    }
                }
            }

            void IComponentInternal.OnInitialize<TW>() {
                var hasWrite = MultiComponentType<TValue>.HasWrite();
                var hasRead = MultiComponentType<TValue>.HasRead();

                ref var storage = ref World<TW>.ResourcesData.Instance.GetOrCreate<MultiValueStorage<TValue>>(out var isNew);
                if (isNew) {
                    storage.Init(Data.Instance.EntitiesSegments.Length, false, false, false, hasWrite, hasRead);
                    storage.ElementStrategy = ElementStrategy ?? new StructPackArrayStrategy<TValue>();
                    World<TW>.Data.Instance.RegisterMultiStorageResizer(_ResizeStorage);
                    World<TW>.Data.Instance.RegisterMultiStorageResetter(_ResetStorage);
                }
            }
            #endregion

            #region INTERNAL
            internal readonly Entity EntityOwner {
                [MethodImpl(AggressiveInlining)] get => new((SegmentIdx << Const.ENTITIES_IN_SEGMENT_SHIFT) + SegmentEntityIdx);
            }

            #if FFS_ECS_DEBUG
            [MethodImpl(AggressiveInlining)]
            private readonly void AssertNotBlockedByIteration() {
                if (Resources<MultiValueStorage<TValue>>.Value.IsBlockedByIteration(SegmentIdx, Offset)) {
                    throw new StaticEcsException($"[ Multi<{typeof(TValue)}> ] Cannot modify while being iterated.");
                }
            }
            #endif

            internal static void _ResizeStorage(uint segmentCapacity) {
                ref var storage = ref Resources<MultiValueStorage<TValue>>.Value;
                if (storage.Segments != null) {
                    storage.EnsureCapacity((int)segmentCapacity);
                }
            }

            private static void _ResetStorage() {
                ref var storage = ref Resources<MultiValueStorage<TValue>>.Value;
                if (storage.Segments == null) return;
                for (var i = 0; i < storage.Segments.Length; i++) {
                    if (storage.Segments[i] != null) {
                        storage.ReleaseSegment((uint)i);
                    }
                }
            }

            private void Grow(ref MultiValueStorage<TValue> storage) {
                #if FFS_ECS_DEBUG
                if (Level >= SegmentAllocator.LevelCount - 1) {
                    throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.Grow ] max level reached: {Level}");
                }
                #endif
                var oldLevel = Level;
                Level++;
                Offset = storage.Allocators[SegmentIdx].Resize(Offset, Count, oldLevel, Level, ref storage.Segments[SegmentIdx]);
            }

            private void EnsureCapacityInternal(ref MultiValueStorage<TValue> storage, ushort minCapacity) {
                var newCapacity = Utils.RoundUpToPowerOf2(Math.Max(minCapacity, SegmentAllocator.MinSlotCapacity));
                if (newCapacity <= (uint)(SegmentAllocator.MinSlotCapacity << Level)) return;

                var newLevel = SegmentAllocator.LevelForCapacity(newCapacity);
                #if FFS_ECS_DEBUG
                if (newLevel >= SegmentAllocator.LevelCount) {
                    throw new StaticEcsException($"[ Multi<{typeof(TValue)}>.EnsureCapacityInternal ] level overflow: {newLevel}");
                }
                #endif
                Offset = storage.Allocators[SegmentIdx].Resize(Offset, Count, Level, newLevel, ref storage.Segments[SegmentIdx]);
                Level = newLevel;
            }
            #endregion
            
            /// <summary>
            /// Implicitly converts to a <see cref="Span{T}"/> over the currently stored elements.
            /// The span is valid only while the multi-component is not modified (no Add/Remove/Grow).
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static implicit operator Span<TValue>(Multi<TValue> multi) {
                return multi.AsSpan;
            }
            
            /// <summary>
            /// Implicitly converts to a <see cref="ReadOnlySpan{T}"/> over the currently stored elements.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static implicit operator ReadOnlySpan<TValue>(Multi<TValue> multi) {
                return multi.AsReadOnlySpan;
            }

        }

        /// <summary>
        /// A read-only view over a <see cref="Multi{TValue}"/>. Provides all query and copy operations
        /// but no mutating methods (Add, Remove, Clear, etc.). All accessors return values by copy rather
        /// than by <c>ref</c>, preventing unintended modification of the underlying storage.
        /// </summary>
        /// <typeparam name="TValue">A struct type implementing <see cref="IMultiComponent"/>.</typeparam>
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        public struct ROMulti<TValue> : IEquatable<ROMulti<TValue>> where TValue : struct, IMultiComponent {
            internal Multi<TValue> Multi;

            internal void Init(Multi<TValue> value) {
                Multi = value;
            }

            /// <inheritdoc cref="Multi{TValue}.Capacity"/>
            public readonly ushort Capacity {
                [MethodImpl(AggressiveInlining)] get => Multi.Capacity;
            }

            /// <inheritdoc cref="Multi{TValue}.Length"/>
            public readonly ushort Length {
                [MethodImpl(AggressiveInlining)] get => Multi.Length;
            }

            /// <inheritdoc cref="Multi{TValue}.IsEmpty"/>
            public readonly bool IsEmpty {
                [MethodImpl(AggressiveInlining)] get => Multi.IsEmpty;
            }

            /// <inheritdoc cref="Multi{TValue}.IsNotEmpty"/>
            public readonly bool IsNotEmpty {
                [MethodImpl(AggressiveInlining)] get => Multi.IsNotEmpty;
            }

            /// <inheritdoc cref="Multi{TValue}.IsFull"/>
            public readonly bool IsFull {
                [MethodImpl(AggressiveInlining)] get => Multi.IsFull;
            }

            /// <summary>
            /// Returns the element at the given index by value (copy), not by reference.
            /// </summary>
            /// <param name="idx">Zero-based index. Must be in range [0, <see cref="Length"/>).</param>
            /// <exception cref="StaticEcsException">Thrown in debug builds if <paramref name="idx"/> is out of range.</exception>
            public readonly TValue this[int idx] {
                [MethodImpl(AggressiveInlining)] get => Multi[idx];
            }

            /// <inheritdoc cref="Multi{TValue}.AsReadOnlySpan"/>
            public readonly ReadOnlySpan<TValue> AsReadOnlySpan {
                [MethodImpl(AggressiveInlining)] get => new(Resources<MultiValueStorage<TValue>>.Value.Segments[Multi.SegmentIdx], (int)Multi.Offset, Multi.Count);
            }

            /// <summary>
            /// Returns the first element by value (copy).
            /// </summary>
            /// <exception cref="StaticEcsException">Thrown in debug builds if the collection is empty.</exception>
            [MethodImpl(AggressiveInlining)]
            public readonly TValue First() {
                return Multi.First();
            }

            /// <summary>
            /// Returns the last element by value (copy).
            /// </summary>
            /// <exception cref="StaticEcsException">Thrown in debug builds if the collection is empty.</exception>
            [MethodImpl(AggressiveInlining)]
            public readonly TValue Last() {
                return Multi.Last();
            }

            /// <inheritdoc cref="Multi{TValue}.IndexOf"/>
            [MethodImpl(AggressiveInlining)]
            public readonly int IndexOf(TValue item) {
                return Multi.IndexOf(item);
            }

            /// <inheritdoc cref="Multi{TValue}.Contains(TValue)"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool Contains(TValue item) {
                return Multi.Contains(item);
            }

            /// <inheritdoc cref="Multi{TValue}.Contains{TComparer}(TValue, TComparer)"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool Contains<TComparer>(TValue item, TComparer comparer) where TComparer : IEqualityComparer<TValue> {
                return Multi.Contains(item, comparer);
            }

            /// <inheritdoc cref="Multi{TValue}.CopyTo(TValue[])"/>
            [MethodImpl(AggressiveInlining)]
            public readonly void CopyTo(TValue[] dst) {
                Multi.CopyTo(dst);
            }

            /// <inheritdoc cref="Multi{TValue}.CopyTo(TValue[], int, int)"/>
            [MethodImpl(AggressiveInlining)]
            public readonly void CopyTo(TValue[] dst, int dstIdx, int len) {
                Multi.CopyTo(dst, dstIdx, len);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly override string ToString() {
                return Multi.ToString();
            }

            /// <summary>
            /// Returns a <see cref="ROMultiComponentsIterator{TValue}"/> that iterates elements in reverse order,
            /// yielding <typeparamref name="TValue"/> by copy. Supports <c>foreach</c> syntax.
            /// <para>In debug builds, blocks modification of the underlying multi-component during iteration.</para>
            /// </summary>
            /// <returns>A read-only reverse iterator over the elements.</returns>
            [MethodImpl(AggressiveInlining)]
            public ROMultiComponentsIterator<TValue> GetEnumerator() => new(
                Resources<MultiValueStorage<TValue>>.Value.Segments[Multi.SegmentIdx],
                Multi.SegmentIdx,
                Multi.Offset,
                Multi.Count
            );

            [MethodImpl(AggressiveInlining)]
            public bool Equals(ROMulti<TValue> other) {
                return Multi.Equals(other.Multi);
            }

            [MethodImpl(AggressiveInlining)]
            public override bool Equals(object obj) => throw new StaticEcsException($"ROMulti<{typeof(TValue)}> `Equals object` not allowed!");

            [MethodImpl(AggressiveInlining)]
            public override int GetHashCode() => throw new StaticEcsException($"ROMulti<{typeof(TValue)}> `GetHashCode object` not allowed!");

            [MethodImpl(AggressiveInlining)]
            public static bool operator ==(ROMulti<TValue> left, ROMulti<TValue> right) => left.Equals(right);

            [MethodImpl(AggressiveInlining)]
            public static bool operator !=(ROMulti<TValue> left, ROMulti<TValue> right) => !left.Equals(right);
            
            /// <summary>
            /// Implicitly converts to a <see cref="ReadOnlySpan{T}"/> over the currently stored elements.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static implicit operator ReadOnlySpan<TValue>(ROMulti<TValue> multi) {
                return multi.AsReadOnlySpan;
            }
        }

        /// <summary>
        /// A mutable reverse iterator over elements of a <see cref="Multi{TValue}"/>. This is a <c>ref struct</c>
        /// that yields elements by mutable <c>ref TValue</c>, iterating from the last element to the first.
        /// <para>
        /// In debug builds, constructing this iterator blocks modification of the underlying multi-component
        /// until the iterator is disposed, preventing invalidation during enumeration.
        /// </para>
        /// </summary>
        /// <typeparam name="TValue">A struct type implementing <see cref="IMultiComponent"/>.</typeparam>
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        public ref struct MultiComponentsIterator<TValue> where TValue : struct, IMultiComponent {
            private readonly TValue[] _segment;
            private readonly int _start;
            private int _index;
            #if FFS_ECS_DEBUG
            private readonly uint _segmentIdx;
            private readonly uint _offset;
            private bool _disposed;
            #endif

            [MethodImpl(AggressiveInlining)]
            internal MultiComponentsIterator(Multi<TValue> multi) {
                _segment = Resources<MultiValueStorage<TValue>>.Value.Segments[multi.SegmentIdx];
                _start = (int)multi.Offset;
                _index = (int)(multi.Offset + multi.Count);
                #if FFS_ECS_DEBUG
                _segmentIdx = multi.SegmentIdx;
                _offset = multi.Offset;
                _disposed = false;
                Resources<MultiValueStorage<TValue>>.Value.BlockIteration(multi.SegmentIdx, multi.Offset);
                #endif
            }

            public ref TValue Current {
                [MethodImpl(AggressiveInlining)] get => ref _segment[_index];
            }

            [MethodImpl(AggressiveInlining)]
            public bool MoveNext() {
                return --_index >= _start;
            }

            [MethodImpl(AggressiveInlining)]
            public void Dispose() {
                #if FFS_ECS_DEBUG
                if (!_disposed) {
                    _disposed = true;
                    Resources<MultiValueStorage<TValue>>.Value.UnblockIteration(_segmentIdx, _offset);
                }
                #endif
            }
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        /// <summary>
        /// A read-only reverse iterator over elements of a <see cref="Multi{TValue}"/>. This is a <c>ref struct</c>
        /// that yields elements by value (<typeparamref name="TValue"/> copy), iterating from the last element to the first.
        /// <para>
        /// In debug builds, constructing this iterator blocks modification of the underlying multi-component
        /// until the iterator is disposed, preventing invalidation during enumeration.
        /// </para>
        /// </summary>
        /// <typeparam name="TValue">A struct type implementing <see cref="IMultiComponent"/>.</typeparam>
        public ref struct ROMultiComponentsIterator<TValue> where TValue : struct, IMultiComponent {
            private readonly TValue[] _segment;
            private readonly int _start;
            private int _index;
            #if FFS_ECS_DEBUG
            private readonly uint _segmentIdx;
            private readonly uint _offset;
            private bool _disposed;
            #endif

            [MethodImpl(AggressiveInlining)]
            internal ROMultiComponentsIterator(TValue[] segment, uint segmentIdx, uint offset, ushort count) {
                _segment = segment;
                _start = (int)offset;
                _index = (int)(offset + count);
                #if FFS_ECS_DEBUG
                _disposed = false;
                _segmentIdx = segmentIdx;
                _offset = offset;
                Resources<MultiValueStorage<TValue>>.Value.BlockIteration(segmentIdx, offset);
                #endif
            }

            public TValue Current {
                [MethodImpl(AggressiveInlining)] get => _segment[_index];
            }

            [MethodImpl(AggressiveInlining)]
            public bool MoveNext() {
                return --_index >= _start;
            }

            [MethodImpl(AggressiveInlining)]
            public void Dispose() {
                #if FFS_ECS_DEBUG
                if (!_disposed) {
                    _disposed = true;
                    Resources<MultiValueStorage<TValue>>.Value.UnblockIteration(_segmentIdx, _offset);
                }
                #endif
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    internal struct MultiValueStorage<TValue> where TValue : struct, IMultiComponent {
        internal TValue[][] Segments;
        internal SegmentAllocator[] Allocators;
        private TValue[][] _segmentsPool;
        private int _segmentsPoolCount;
        internal bool HasOnAdd;
        internal bool HasOnDelete;
        internal bool HasOnCopy;
        internal bool HasWriteHook;
        internal bool HasReadHook;
        internal IPackArrayStrategy<TValue> ElementStrategy;
        #if FFS_ECS_DEBUG
        private ConcurrentDictionary<ulong, int> _blockers;
        #endif

        [MethodImpl(NoInlining)]
        internal void Init(int initialSegmentCapacity, bool hasOnAdd, bool hasOnDelete, bool hasOnCopy, bool hasWriteHook, bool hasReadHook) {
            Segments = new TValue[initialSegmentCapacity][];
            Allocators = new SegmentAllocator[initialSegmentCapacity];
            _segmentsPool = new TValue[initialSegmentCapacity][];
            _segmentsPoolCount = 0;
            HasOnAdd = hasOnAdd;
            HasOnDelete = hasOnDelete;
            HasOnCopy = hasOnCopy;
            HasWriteHook = hasWriteHook;
            HasReadHook = hasReadHook;
            #if FFS_ECS_DEBUG
            _blockers = new ConcurrentDictionary<ulong, int>();
            #endif
        }

        #if FFS_ECS_DEBUG
        [MethodImpl(AggressiveInlining)]
        internal void BlockIteration(uint segmentIdx, uint offset) {
            var key = ((ulong)segmentIdx << 32) | offset;
            _blockers.AddOrUpdate(key, 1, (_, count) => count + 1);
        }

        [MethodImpl(AggressiveInlining)]
        internal void UnblockIteration(uint segmentIdx, uint offset) {
            var key = ((ulong)segmentIdx << 32) | offset;
            _blockers.AddOrUpdate(key, 0, (_, count) => count - 1);
        }

        [MethodImpl(AggressiveInlining)]
        internal bool IsBlockedByIteration(uint segmentIdx, uint offset) {
            var key = ((ulong)segmentIdx << 32) | offset;
            return _blockers.TryGetValue(key, out var count) && count > 0;
        }
        #endif

        [MethodImpl(AggressiveInlining)]
        internal void EnsureCapacity(int segmentCapacity) {
            if (segmentCapacity > Segments.Length) {
                Array.Resize(ref Segments, segmentCapacity);
                Array.Resize(ref Allocators, segmentCapacity);
                Array.Resize(ref _segmentsPool, segmentCapacity);
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal void EnsureSegment(uint segmentIdx) {
            #if FFS_ECS_DEBUG
            if (segmentIdx >= Segments.Length) {
                throw new StaticEcsException($"[ MultiValueStorage<{typeof(TValue)}>.EnsureSegment ] segmentIdx {segmentIdx} >= Segments.Length {Segments.Length}. World resize was not called.");
            }
            #endif

            if (Segments[segmentIdx] == null) {
                CreateSegment(segmentIdx);
            }
        }

        [MethodImpl(NoInlining)]
        private void CreateSegment(uint segmentIdx) {
            Allocators[segmentIdx].Init();
            var poolIdx = Interlocked.Decrement(ref _segmentsPoolCount);
            if (poolIdx >= 0) {
                TValue[] pooled;
                while ((pooled = Volatile.Read(ref _segmentsPool[poolIdx])) == null) {
                    Thread.SpinWait(1);
                }
                _segmentsPool[poolIdx] = null;
                Allocators[segmentIdx].SegmentCapacity = (uint)pooled.Length;
                Segments[segmentIdx] = pooled;
            } else {
                Interlocked.Increment(ref _segmentsPoolCount);
                Segments[segmentIdx] = Array.Empty<TValue>();
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal void ReleaseSegment(uint segmentIdx) {
            var segment = Segments[segmentIdx];
            Segments[segmentIdx] = null;
            Allocators[segmentIdx] = default;
            var poolIdx = Interlocked.Increment(ref _segmentsPoolCount) - 1;
            Volatile.Write(ref _segmentsPool[poolIdx], segment);
        }

    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    internal struct SegmentAllocator {
        internal const ushort MinSlotCapacity = 4;
        internal const int LevelCount = 14;
        internal const int MinLevelShift = 2;
        
        internal uint Used;
        internal uint SegmentCapacity;
        internal uint ActiveAllocations;

        internal uint[][] FreeLists;
        internal uint[] FreeListCounts;

        [MethodImpl(AggressiveInlining)]
        internal void Init() {
            Used = 0;
            SegmentCapacity = 0;
            ActiveAllocations = 0;
            FreeLists = new uint[LevelCount][];
            FreeListCounts = new uint[LevelCount];
            for (uint i = 0; i < LevelCount; i++) {
                FreeLists[i] = new uint[4];
            }
        }

        internal readonly bool IsEmpty {
            [MethodImpl(AggressiveInlining)] get => ActiveAllocations == 0;
        }

        [MethodImpl(AggressiveInlining)]
        internal uint Allocate<T>(byte level, ref T[] segment) {
            var actualCapacity = CapacityForLevel(level);
            ActiveAllocations++;

            if (FreeListCounts[level] > 0) {
                return FreeLists[level][--FreeListCounts[level]];
            }

            var offset = Used;
            Used += actualCapacity;

            if (Used > SegmentCapacity) {
                SegmentCapacity = Math.Max(SegmentCapacity << 1, Used);
                Array.Resize(ref segment, (int)SegmentCapacity);
            }

            return offset;
        }

        [MethodImpl(AggressiveInlining)]
        internal void Free(uint offset, byte level, out bool empty) {
            empty = --ActiveAllocations == 0;
            var count = FreeListCounts[level];
            if (count == (uint)FreeLists[level].Length) {
                Array.Resize(ref FreeLists[level], (int)(count << 1));
            }

            FreeLists[level][count] = offset;
            FreeListCounts[level] = count + 1;
        }

        [MethodImpl(AggressiveInlining)]
        internal uint Resize<T>(uint oldOffset, ushort oldCount, byte oldLevel, byte newLevel,
                                  ref T[] segment) {
            var newOffset = Allocate(newLevel, ref segment);
            if (oldCount > 0) {
                Utils.LoopFallbackCopy(segment, oldOffset, segment, newOffset, oldCount);
            }

            Free(oldOffset, oldLevel, out _);
            return newOffset;
        }

        [MethodImpl(AggressiveInlining)]
        internal static byte LevelForCapacity(uint capacity) {
            var rounded = Math.Max(Utils.RoundUpToPowerOf2((ushort)Math.Min(capacity, ushort.MaxValue)), MinSlotCapacity);
            #if FFS_ECS_DEBUG
            if (rounded > (uint)(MinSlotCapacity << (LevelCount - 1))) {
                throw new StaticEcsException($"[ SegmentAllocator.LevelForCapacity ] capacity {capacity} exceeds max {MinSlotCapacity << (LevelCount - 1)}");
            }
            #endif
            return (byte)((((BitConverter.DoubleToInt64Bits(rounded) >> 52) + 1) & 0xFF) - MinLevelShift);
        }

        [MethodImpl(AggressiveInlining)]
        internal static ushort CapacityForLevel(int level) {
            return (ushort)(1 << (level + MinLevelShift));
        }
    }

    
    /// <summary>
    /// Optional interface for <see cref="IPackArrayStrategy{T}"/> implementations that maintain internal state
    /// across serialization calls. <c>Reset()</c> is called by <c>Components&lt;T&gt;.WriteChunk</c> and
    /// <c>ReadChunk</c> before each per-block iteration loop, ensuring correct behavior when the same
    /// chunk is serialized multiple times.
    /// </summary>
    public interface IPackArrayStrategyResettable {
        void Reset();
    }

    /// <summary>
    /// Bulk segment serialization strategy for <see cref="World{TWorld}.Multi{TValue}"/> components.
    /// <para>
    /// Instead of serializing each entity's <typeparamref name="TValue"/> elements individually,
    /// this strategy writes the raw <c>Multi&lt;TValue&gt;</c> struct bytes via unmanaged bulk copy
    /// and serializes the underlying <see cref="MultiValueStorage{TValue}"/> segments with their
    /// <see cref="SegmentAllocator"/> state as whole memory blocks. On deserialization, segments and
    /// allocators are restored in bulk — no per-entity data copying.
    /// </para>
    /// <para>
    /// <b>Segment deduplication:</b> Within a single <c>WriteChunk</c> call, the strategy tracks
    /// the component segment array reference. When the same array is passed to consecutive
    /// <c>WriteArray</c> calls (multiple blocks within one segment), segment data is written
    /// only once. <see cref="IPackArrayStrategyResettable.Reset"/> clears this tracking before
    /// each chunk operation.
    /// </para>
    /// <para>
    /// <b>Entity-level serialization</b> (<c>EntitiesSnapshot</c>) is not affected — it always
    /// uses per-entity <c>Write</c>/<c>Read</c> hooks on the component.
    /// </para>
    /// </summary>
    /// <typeparam name="TWorld">World type for static storage access.</typeparam>
    /// <typeparam name="TValue">Multi-component element type. Must be <c>unmanaged</c>.</typeparam>
    /// <example>
    /// <code>
    /// W.Types().Multi&lt;Item&gt;(new ComponentTypeConfig&lt;W.Multi&lt;Item&gt;&gt;(
    ///     guid: new Guid("..."),
    ///     readWriteStrategy: new MultiUnmanagedPackArrayStrategy&lt;MyWorld, Item&gt;()
    /// ));
    /// </code>
    /// </example>
    public sealed class MultiUnmanagedPackArrayStrategy<TWorld, TValue> : IPackArrayStrategy<World<TWorld>.Multi<TValue>>, IPackArrayStrategyResettable
        where TWorld : struct, IWorldType
        where TValue : unmanaged, IMultiComponent {

        private World<TWorld>.Multi<TValue>[] _lastWriteArray;

        [MethodImpl(AggressiveInlining)]
        public bool IsUnmanaged() => true;

        [MethodImpl(AggressiveInlining)]
        public void Reset() {
            _lastWriteArray = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void Register() {
            new UnmanagedPackArrayStrategy<World<TWorld>.Multi<TValue>>().Register();
        }

        [MethodImpl(AggressiveInlining)]
        public void WriteArray(ref BinaryPackWriter writer, World<TWorld>.Multi<TValue>[] value, int idx, int count) {
            writer.WriteArrayUnmanaged(value, idx, count);

            var newSegment = !ReferenceEquals(value, _lastWriteArray);
            _lastWriteArray = value;
            writer.WriteBool(newSegment);

            if (newSegment) {
                var segIdx = value[idx].SegmentIdx;
                ref var storage = ref World<TWorld>.Resources<MultiValueStorage<TValue>>.Value;
                ref var alloc = ref storage.Allocators[segIdx];

                writer.WriteUint(alloc.Used);
                writer.WriteUint(alloc.ActiveAllocations);

                for (var level = 0; level < SegmentAllocator.LevelCount; level++) {
                    var cnt = alloc.FreeListCounts[level];
                    writer.WriteUint(cnt);
                    if (cnt > 0) {
                        writer.WriteArrayUnmanaged(alloc.FreeLists[level], 0, (int)cnt);
                    }
                }

                if (alloc.Used > 0) {
                    World<TWorld>.Multi<TValue>.ElementStrategy.WriteArray(ref writer, storage.Segments[segIdx], 0, (int)alloc.Used);
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void ReadArray(ref BinaryPackReader reader, ref World<TWorld>.Multi<TValue>[] result, int idx) {
            reader.ReadArrayUnmanaged(ref result, idx);

            if (reader.ReadBool()) {
                var segIdx = result[idx].SegmentIdx;
                ref var storage = ref World<TWorld>.Resources<MultiValueStorage<TValue>>.Value;

                storage.EnsureSegment(segIdx);
                ref var alloc = ref storage.Allocators[segIdx];

                var used = reader.ReadUint();
                alloc.Used = used;
                alloc.ActiveAllocations = reader.ReadUint();

                for (var level = 0; level < SegmentAllocator.LevelCount; level++) {
                    var cnt = reader.ReadUint();
                    alloc.FreeListCounts[level] = cnt;
                    if (cnt > (uint)alloc.FreeLists[level].Length) {
                        alloc.FreeLists[level] = new uint[cnt];
                    }
                    if (cnt > 0) {
                        reader.ReadArrayUnmanaged(ref alloc.FreeLists[level], 0);
                    }
                }

                if (used > 0) {
                    if (storage.Segments[segIdx].Length < used) {
                        Array.Resize(ref storage.Segments[segIdx], (int)used);
                    }
                    alloc.SegmentCapacity = (uint)storage.Segments[segIdx].Length;
                    World<TWorld>.Multi<TValue>.ElementStrategy.ReadArray(
                        ref reader, ref storage.Segments[segIdx], 0);
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void WriteArray(ref BinaryPackWriter writer, World<TWorld>.Multi<TValue>[] value) {
            writer.WriteArrayUnmanaged(value);
        }

        [MethodImpl(AggressiveInlining)]
        public World<TWorld>.Multi<TValue>[] ReadArray(ref BinaryPackReader reader) {
            return reader.ReadArrayUnmanaged<World<TWorld>.Multi<TValue>>();
        }

        [MethodImpl(AggressiveInlining)]
        public void ReadArray(ref BinaryPackReader reader, ref World<TWorld>.Multi<TValue>[] result) {
            reader.ReadArrayUnmanaged(ref result);
        }

        #if !FFS_PACK_DISABLE_MULTI_ARRAYS && !UNITY_WEBGL
        [MethodImpl(AggressiveInlining)]
        public void WriteArray(ref BinaryPackWriter writer, World<TWorld>.Multi<TValue>[,] value) {
            writer.WriteArrayUnmanaged(value);
        }

        [MethodImpl(AggressiveInlining)]
        public void WriteArray(ref BinaryPackWriter writer, World<TWorld>.Multi<TValue>[,,] value) {
            writer.WriteArrayUnmanaged(value);
        }

        [MethodImpl(AggressiveInlining)]
        public World<TWorld>.Multi<TValue>[,] ReadArray2D(ref BinaryPackReader reader) {
            return reader.ReadArray2DUnmanaged<World<TWorld>.Multi<TValue>>();
        }

        [MethodImpl(AggressiveInlining)]
        public World<TWorld>.Multi<TValue>[,,] ReadArray3D(ref BinaryPackReader reader) {
            return reader.ReadArray3DUnmanaged<World<TWorld>.Multi<TValue>>();
        }
        #endif
    }

    internal static class MultiComponentType<
        #if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
        #endif
        T> where T : struct, IMultiComponent {
        private static readonly Type[] WriteParams = { typeof(BinaryPackWriter).MakeByRefType() };
        private static readonly Type[] ReadParams = { typeof(BinaryPackReader).MakeByRefType() };

        internal static bool HasWrite() {
            return HasMethod(typeof(T), nameof(IMultiComponent.Write), WriteParams);
        }

        internal static bool HasRead() {
            return HasMethod(typeof(T), nameof(IMultiComponent.Read), ReadParams);
        }

        private static bool HasMethod(
            #if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
            #endif
            Type structType, string methodName, Type[] parameterTypes) {
            var methods = structType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var methodInfo in methods) {
                if (methodInfo.Name == methodName && !methodInfo.IsGenericMethodDefinition) {
                    var parameters = methodInfo.GetParameters();
                    if (parameters.Length == parameterTypes.Length) {
                        var match = true;
                        for (var i = 0; i < parameters.Length; i++) {
                            if (parameterTypes[i].Name != parameters[i].ParameterType.Name) {
                                match = false;
                                break;
                            }
                        }
                        if (match) return true;
                    }
                }
            }
            return false;
        }
    }
}