#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
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
    /// Interface for defining link type hooks. Implement methods to receive callbacks when
    /// a <see cref="World{TWorld}.Link{TLinkType}"/> is added, deleted, or copied.
    /// <para>All methods have default empty implementations; override only the hooks you need.</para>
    /// </summary>
    public interface ILinkType {
        /// <summary>
        /// Called when a link of this type is added to <paramref name="self"/>, pointing at <paramref name="link"/>.
        /// </summary>
        /// <param name="self">The entity receiving the link.</param>
        /// <param name="link">The <see cref="EntityGID"/> of the linked entity.</param>
        public void OnAdd<TWorld>(World<TWorld>.Entity self, EntityGID link) where TWorld : struct, IWorldType { }

        /// <summary>
        /// Called when a link of this type is removed from <paramref name="self"/>, previously pointing at <paramref name="link"/>.
        /// </summary>
        /// <param name="self">The entity losing the link.</param>
        /// <param name="link">The <see cref="EntityGID"/> of the previously linked entity.</param>
        public void OnDelete<TWorld>(World<TWorld>.Entity self, EntityGID link, HookReason reason) where TWorld : struct, IWorldType { }

        /// <summary>
        /// Called when a link of this type is copied from <paramref name="self"/> to <paramref name="other"/>.
        /// </summary>
        /// <param name="self">The source entity that owns the link.</param>
        /// <param name="other">The destination entity receiving the copied link.</param>
        /// <param name="link">The <see cref="EntityGID"/> of the linked entity.</param>
        public void CopyTo<TWorld>(World<TWorld>.Entity self, World<TWorld>.Entity other, EntityGID link) where TWorld : struct, IWorldType { }
    }

    /// <summary>
    /// Marker interface extending <see cref="ILinkType"/> that designates a link type for use with
    /// <see cref="World{TWorld}.Links{TLinkType}"/> (multi-link collections).
    /// Types used as <c>TLinkType</c> in <see cref="World{TWorld}.Links{TLinkType}"/> must implement this interface.
    /// </summary>
    public interface ILinksType : ILinkType {}

    internal interface ILinkComponent : IComponent {
        internal EntityGID Value { get; }
        internal void SetValue(EntityGID gid);
    }

    internal interface ILinksComponent : IComponent {
        internal void AddLink(EntityGID gid);
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public abstract partial class World<TWorld> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        /// <summary>
        /// A single-link component that wraps an <see cref="EntityGID"/> as a typed relationship.
        /// Stored as a regular component on an entity, establishing a one-to-one directional link
        /// to another entity. The <typeparamref name="TLinkType"/> parameter defines the relationship
        /// semantics and optional <see cref="ILinkType"/> hooks.
        /// <para>Supports implicit conversions from <see cref="EntityGID"/>, <see cref="EntityGIDCompact"/>,
        /// and <see cref="Entity"/>, as well as implicit conversion back to <see cref="EntityGID"/>.</para>
        /// </summary>
        /// <typeparam name="TLinkType">The link type defining relationship semantics and hooks. Must implement <see cref="ILinkType"/>.</typeparam>
        [Serializable]
        public struct Link<TLinkType> : ILinkComponent, IMultiComponent, IComponentHookOverride, IEquatable<Link<TLinkType>>
            where TLinkType : unmanaged, ILinkType {

            private EntityGID _value;

            /// <summary>
            /// Creates a new link pointing to the specified entity.
            /// </summary>
            /// <param name="value">The <see cref="EntityGID"/> of the entity to link to.</param>
            [MethodImpl(AggressiveInlining)]
            public Link(EntityGID value) {
                _value = value;
            }

            /// <summary>
            /// The <see cref="EntityGID"/> of the linked entity.
            /// </summary>
            public EntityGID Value {
                [MethodImpl(AggressiveInlining)] get => _value;
            }

            [MethodImpl(AggressiveInlining)]
            public void OnAdd<TW>(World<TW>.Entity self) where TW : struct, IWorldType {
                var linkType = default(TLinkType);
                linkType.OnAdd(self, _value);
            }

            [MethodImpl(AggressiveInlining)]
            public void OnDelete<TW>(World<TW>.Entity self, HookReason reason) where TW : struct, IWorldType {
                var linkType = default(TLinkType);
                linkType.OnDelete(self, _value, reason);
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyTo<TW>(World<TW>.Entity self, World<TW>.Entity other, bool disabled) where TW : struct, IWorldType {
                var linkType = default(TLinkType);
                linkType.CopyTo(self, other, _value);
            }

            [MethodImpl(AggressiveInlining)]
            public void Write<TW>(ref BinaryPackWriter writer, World<TW>.Entity self) where TW : struct, IWorldType {
                writer.WriteEntityGID(in _value);
            }

            [MethodImpl(AggressiveInlining)]
            public void Read<TW>(ref BinaryPackReader reader, World<TW>.Entity self, byte version, bool disabled) where TW : struct, IWorldType {
                _value = reader.ReadEntityGID();
            }

            [MethodImpl(AggressiveInlining)]
            void IMultiComponent.Write(ref BinaryPackWriter writer) {
                writer.WriteEntityGID(in _value);
            }

            [MethodImpl(AggressiveInlining)]
            void IMultiComponent.Read(ref BinaryPackReader reader) {
                _value = reader.ReadEntityGID();
            }

            bool IComponentHookOverride.HasOnAdd() => LinkType<TLinkType>.HasOnAdd();
            bool IComponentHookOverride.HasOnDelete() => LinkType<TLinkType>.HasOnDelete();
            bool IComponentHookOverride.HasCopyTo() => LinkType<TLinkType>.HasCopyTo();

            EntityGID ILinkComponent.Value => _value;
            void ILinkComponent.SetValue(EntityGID gid) => _value = gid;

            /// <summary>
            /// Converts a <see cref="Link{TLinkType}"/> to its underlying <see cref="EntityGID"/>.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static implicit operator EntityGID(Link<TLinkType> link) => link._value;

            /// <summary>
            /// Creates a <see cref="Link{TLinkType}"/> from an <see cref="EntityGID"/>.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static implicit operator Link<TLinkType>(EntityGID gid) => new(gid);

            /// <summary>
            /// Creates a <see cref="Link{TLinkType}"/> from an <see cref="EntityGIDCompact"/>.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static implicit operator Link<TLinkType>(EntityGIDCompact gid) => new(gid);

            /// <summary>
            /// Creates a <see cref="Link{TLinkType}"/> from an <see cref="Entity"/>, capturing its current <see cref="EntityGID"/>.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static implicit operator Link<TLinkType>(Entity entity) => new(entity);

            /// <summary>
            /// Returns a string representation including the link type and the linked <see cref="EntityGID"/>.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public override string ToString() => $"Link<{typeof(TLinkType)}> : {_value}";

            /// <summary>
            /// Compares two links for equality by their underlying <see cref="EntityGID"/> values.
            /// </summary>
            /// <param name="other">The other link to compare against.</param>
            /// <returns><c>true</c> if both links point to the same <see cref="EntityGID"/>; otherwise <c>false</c>.</returns>
            [MethodImpl(AggressiveInlining)]
            public bool Equals(Link<TLinkType> other) => _value.Equals(other._value);

            /// <inheritdoc/>
            [MethodImpl(AggressiveInlining)]
            public override bool Equals(object obj) => throw new StaticEcsException($"Link<{typeof(TLinkType)}> `Equals object` not allowed!");

            /// <inheritdoc/>
            [MethodImpl(AggressiveInlining)]
            public override int GetHashCode() => throw new StaticEcsException($"Link<{typeof(TLinkType)}> `GetHashCode object` not allowed!");
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        /// <summary>
        /// A multi-link component that stores a dynamically-sized unique set of <see cref="Link{TLinkType}"/> values per entity.
        /// Backed by <see cref="SegmentAllocator"/> with tiered capacity levels, providing efficient storage for
        /// variable-count entity relationships. Acts as a collection of links where each link points to a distinct entity.
        /// <para>Links are stored contiguously in segment memory and support add, remove, search, and iteration operations.
        /// Duplicate links are prevented by <see cref="TryAdd(Link{TLinkType}, bool)"/> or asserted in debug builds by <see cref="Add(Link{TLinkType})"/>.</para>
        /// </summary>
        /// <typeparam name="TLinkType">The link type defining relationship semantics and hooks. Must implement <see cref="ILinksType"/>.</typeparam>
        [Serializable]
        public struct Links<TLinkType> : ILinksComponent, IComponentInternal, IEquatable<Links<TLinkType>> where TLinkType : unmanaged, ILinksType {
            internal uint Offset;
            internal uint SegmentIdx;
            internal ushort Count;
            internal byte Level;
            internal byte SegmentEntityIdx;

            /// <summary>
            /// The current number of links in this collection.
            /// </summary>
            public readonly ushort Length {
                [MethodImpl(AggressiveInlining)] get => Count;
            }

            /// <summary>
            /// The current capacity (maximum number of links before growth is needed).
            /// Capacity doubles with each allocation level.
            /// </summary>
            public readonly ushort Capacity {
                [MethodImpl(AggressiveInlining)] get => (ushort)(SegmentAllocator.MinSlotCapacity << Level);
            }

            /// <summary>
            /// Returns <c>true</c> if the collection contains no links.
            /// </summary>
            public readonly bool IsEmpty {
                [MethodImpl(AggressiveInlining)] get => Count == 0;
            }

            /// <summary>
            /// Returns <c>true</c> if the collection contains at least one link.
            /// </summary>
            public readonly bool IsNotEmpty {
                [MethodImpl(AggressiveInlining)] get => Count != 0;
            }

            /// <summary>
            /// Returns <c>true</c> if the collection has reached its current capacity and will grow on the next add.
            /// </summary>
            public readonly bool IsFull {
                [MethodImpl(AggressiveInlining)] get => Count == SegmentAllocator.MinSlotCapacity << Level;
            }

            /// <summary>
            /// Gets the link at the specified index by value (not by ref).
            /// </summary>
            /// <param name="idx">Zero-based index into the link collection.</param>
            /// <returns>The <see cref="Link{TLinkType}"/> at the given index.</returns>
            /// <exception cref="StaticEcsException">In debug builds, thrown if <paramref name="idx"/> is out of bounds.</exception>
            public readonly Link<TLinkType> this[int idx] {
                [MethodImpl(AggressiveInlining)]
                get {
                    #if FFS_ECS_DEBUG
                    if (idx < 0 || idx >= Count) {
                        throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.Indexer ] index out of bounds: {idx}, count: {Count}");
                    }
                    #endif
                    return Resources<MultiValueStorage<Link<TLinkType>>>.Value.Segments[SegmentIdx][Offset + idx];
                }
            }

            /// <summary>
            /// Returns a read-only span over the links in this collection.
            /// </summary>
            public readonly ReadOnlySpan<Link<TLinkType>> AsReadOnlySpan {
                [MethodImpl(AggressiveInlining)] get => new(Resources<MultiValueStorage<Link<TLinkType>>>.Value.Segments[SegmentIdx], (int)Offset, Count);
            }

            /// <summary>
            /// Returns the first link in the collection.
            /// </summary>
            /// <returns>The first <see cref="Link{TLinkType}"/>.</returns>
            /// <exception cref="StaticEcsException">In debug builds, thrown if the collection is empty.</exception>
            [MethodImpl(AggressiveInlining)]
            public readonly Link<TLinkType> First() {
                #if FFS_ECS_DEBUG
                if (Count == 0) throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.First ] empty");
                #endif
                return Resources<MultiValueStorage<Link<TLinkType>>>.Value.Segments[SegmentIdx][Offset];
            }

            /// <summary>
            /// Returns the last link in the collection.
            /// </summary>
            /// <returns>The last <see cref="Link{TLinkType}"/>.</returns>
            /// <exception cref="StaticEcsException">In debug builds, thrown if the collection is empty.</exception>
            [MethodImpl(AggressiveInlining)]
            public readonly Link<TLinkType> Last() {
                #if FFS_ECS_DEBUG
                if (Count == 0) throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.Last ] empty");
                #endif
                return Resources<MultiValueStorage<Link<TLinkType>>>.Value.Segments[SegmentIdx][Offset + Count - 1];
            }

            #region ADD
            /// <summary>
            /// Attempts to add a link to this collection. If the link already exists, returns <c>false</c> without modification.
            /// Grows capacity automatically if needed.
            /// </summary>
            /// <param name="val">The link to add.</param>
            /// <param name="withOnAdd">If <c>true</c> and the link type has an OnAdd hook, fires the hook for the newly added link.</param>
            /// <returns><c>true</c> if the link was added; <c>false</c> if it was already present (duplicate).</returns>
            [MethodImpl(AggressiveInlining)]
            public bool TryAdd(Link<TLinkType> val, bool withOnAdd = true) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                #endif
                ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                var values = storage.Segments[SegmentIdx];
                
                for (var i = Offset; i < Offset + Count; i++) {
                    if (values[i].Equals(val)) return false;
                }
                
                if (Count >= SegmentAllocator.MinSlotCapacity << Level) {
                    Grow(ref storage);
                    values = storage.Segments[SegmentIdx];
                }

                values[Offset + Count++] = val;
                if (withOnAdd && storage.HasOnAdd) {
                    val.OnAdd(EntityOwner);
                }
                
                return true;
            }
            
            /// <summary>
            /// Batch try-add of two links. Skips any that already exist. Fires OnAdd for each newly added link if the hook is present.
            /// </summary>
            /// <param name="val1">First link to try adding.</param>
            /// <param name="val2">Second link to try adding.</param>
            [MethodImpl(AggressiveInlining)]
            public void TryAdd(Link<TLinkType> val1, Link<TLinkType> val2) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                #endif
                ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                var values = storage.Segments[SegmentIdx];
                var end = Offset + Count;

                int found = 0;
                for (var i = Offset; i < end; i++) {
                    var stored = values[i];
                    if (stored.Equals(val1)) found |= 1;
                    if (stored.Equals(val2)) found |= 2;
                }
                if (found == 3) return;

                var cap = SegmentAllocator.MinSlotCapacity << Level;
                if (Count + 2 > cap) {
                    EnsureCapacityInternal(ref storage, (ushort)(Count + 2));
                    values = storage.Segments[SegmentIdx];
                }

                if ((found & 1) == 0) values[Offset + Count++] = val1;
                if ((found & 2) == 0) values[Offset + Count++] = val2;
                if (storage.HasOnAdd) {
                    var owner = EntityOwner;
                    if ((found & 1) == 0) val1.OnAdd(owner);
                    if ((found & 2) == 0) val2.OnAdd(owner);
                }
            }

            /// <inheritdoc cref="TryAdd(Link{TLinkType}, Link{TLinkType})"/>
            /// <param name="val1">First link to try adding.</param>
            /// <param name="val2">Second link to try adding.</param>
            /// <param name="val3">Third link to try adding.</param>
            [MethodImpl(AggressiveInlining)]
            public void TryAdd(Link<TLinkType> val1, Link<TLinkType> val2, Link<TLinkType> val3) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                #endif
                ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                var values = storage.Segments[SegmentIdx];
                var end = Offset + Count;

                int found = 0;
                for (var i = Offset; i < end; i++) {
                    var stored = values[i];
                    if (stored.Equals(val1)) found |= 1;
                    if (stored.Equals(val2)) found |= 2;
                    if (stored.Equals(val3)) found |= 4;
                }
                if (found == 7) return;

                var cap = SegmentAllocator.MinSlotCapacity << Level;
                if (Count + 3 > cap) {
                    EnsureCapacityInternal(ref storage, (ushort)(Count + 3));
                    values = storage.Segments[SegmentIdx];
                }

                if ((found & 1) == 0) values[Offset + Count++] = val1;
                if ((found & 2) == 0) values[Offset + Count++] = val2;
                if ((found & 4) == 0) values[Offset + Count++] = val3;
                if (storage.HasOnAdd) {
                    var owner = EntityOwner;
                    if ((found & 1) == 0) val1.OnAdd(owner);
                    if ((found & 2) == 0) val2.OnAdd(owner);
                    if ((found & 4) == 0) val3.OnAdd(owner);
                }
            }

            /// <inheritdoc cref="TryAdd(Link{TLinkType}, Link{TLinkType})"/>
            /// <param name="val1">First link to try adding.</param>
            /// <param name="val2">Second link to try adding.</param>
            /// <param name="val3">Third link to try adding.</param>
            /// <param name="val4">Fourth link to try adding.</param>
            [MethodImpl(AggressiveInlining)]
            public void TryAdd(Link<TLinkType> val1, Link<TLinkType> val2, Link<TLinkType> val3, Link<TLinkType> val4) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                #endif
                ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                var values = storage.Segments[SegmentIdx];
                var end = Offset + Count;

                int found = 0;
                for (var i = Offset; i < end; i++) {
                    var stored = values[i];
                    if (stored.Equals(val1)) found |= 1;
                    if (stored.Equals(val2)) found |= 2;
                    if (stored.Equals(val3)) found |= 4;
                    if (stored.Equals(val4)) found |= 8;
                }
                if (found == 15) return;

                var cap = SegmentAllocator.MinSlotCapacity << Level;
                if (Count + 4 > cap) {
                    EnsureCapacityInternal(ref storage, (ushort)(Count + 4));
                    values = storage.Segments[SegmentIdx];
                }

                if ((found & 1) == 0) values[Offset + Count++] = val1;
                if ((found & 2) == 0) values[Offset + Count++] = val2;
                if ((found & 4) == 0) values[Offset + Count++] = val3;
                if ((found & 8) == 0) values[Offset + Count++] = val4;
                if (storage.HasOnAdd) {
                    var owner = EntityOwner;
                    if ((found & 1) == 0) val1.OnAdd(owner);
                    if ((found & 2) == 0) val2.OnAdd(owner);
                    if ((found & 4) == 0) val3.OnAdd(owner);
                    if ((found & 8) == 0) val4.OnAdd(owner);
                }
            }

            /// <summary>
            /// Adds a link to this collection. The link must not already exist.
            /// Always fires the <see cref="ILinkType.OnAdd{TWorld}"/> hook if present.
            /// </summary>
            /// <param name="val">The link to add.</param>
            /// <exception cref="StaticEcsException">In debug builds, thrown if the link is a duplicate.</exception>
            [MethodImpl(AggressiveInlining)]
            public void Add(Link<TLinkType> val) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                AssertNotContains(val);
                #endif
                ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                if (Count >= SegmentAllocator.MinSlotCapacity << Level) {
                    Grow(ref storage);
                }

                storage.Segments[SegmentIdx][Offset + Count++] = val;
                if (storage.HasOnAdd) {
                    val.OnAdd(EntityOwner);
                }
            }

            /// <inheritdoc cref="Add(Link{TLinkType})"/>
            /// <param name="val1">First link to add.</param>
            /// <param name="val2">Second link to add.</param>
            [MethodImpl(AggressiveInlining)]
            public void Add(Link<TLinkType> val1, Link<TLinkType> val2) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                AssertNotContains(val1);
                AssertNotContains(val2);
                #endif
                ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                var cap = SegmentAllocator.MinSlotCapacity << Level;
                if (Count + 1 >= cap) {
                    EnsureCapacityInternal(ref storage, (ushort)(Count + 2));
                }

                var values = storage.Segments[SegmentIdx];
                values[Offset + Count++] = val1;
                values[Offset + Count++] = val2;
                if (storage.HasOnAdd) {
                    val1.OnAdd(EntityOwner);
                    val2.OnAdd(EntityOwner);
                }
            }

            /// <inheritdoc cref="Add(Link{TLinkType})"/>
            /// <param name="val1">First link to add.</param>
            /// <param name="val2">Second link to add.</param>
            /// <param name="val3">Third link to add.</param>
            [MethodImpl(AggressiveInlining)]
            public void Add(Link<TLinkType> val1, Link<TLinkType> val2, Link<TLinkType> val3) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                AssertNotContains(val1);
                AssertNotContains(val2);
                AssertNotContains(val3);
                #endif
                ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                var cap = SegmentAllocator.MinSlotCapacity << Level;
                if (Count + 2 >= cap) {
                    EnsureCapacityInternal(ref storage, (ushort)(Count + 3));
                }

                var values = storage.Segments[SegmentIdx];
                values[Offset + Count++] = val1;
                values[Offset + Count++] = val2;
                values[Offset + Count++] = val3;
                if (storage.HasOnAdd) {
                    val1.OnAdd(EntityOwner);
                    val2.OnAdd(EntityOwner);
                    val3.OnAdd(EntityOwner);
                }
            }

            /// <inheritdoc cref="Add(Link{TLinkType})"/>
            /// <param name="val1">First link to add.</param>
            /// <param name="val2">Second link to add.</param>
            /// <param name="val3">Third link to add.</param>
            /// <param name="val4">Fourth link to add.</param>
            [MethodImpl(AggressiveInlining)]
            public void Add(Link<TLinkType> val1, Link<TLinkType> val2, Link<TLinkType> val3, Link<TLinkType> val4) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                AssertNotContains(val1);
                AssertNotContains(val2);
                AssertNotContains(val3);
                AssertNotContains(val4);
                #endif
                ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                var cap = SegmentAllocator.MinSlotCapacity << Level;
                if (Count + 3 >= cap) {
                    EnsureCapacityInternal(ref storage, (ushort)(Count + 4));
                }

                var values = storage.Segments[SegmentIdx];
                values[Offset + Count++] = val1;
                values[Offset + Count++] = val2;
                values[Offset + Count++] = val3;
                values[Offset + Count++] = val4;
                if (storage.HasOnAdd) {
                    val1.OnAdd(EntityOwner);
                    val2.OnAdd(EntityOwner);
                    val3.OnAdd(EntityOwner);
                    val4.OnAdd(EntityOwner);
                }
            }

            /// <summary>
            /// Bulk adds all links from the source array. All links must be unique (not already present).
            /// Fires <see cref="ILinkType.OnAdd{TWorld}"/> for each added link if the hook is present.
            /// </summary>
            /// <param name="src">Array of links to add.</param>
            /// <exception cref="StaticEcsException">In debug builds, thrown if <paramref name="src"/> is null or contains duplicates.</exception>
            [MethodImpl(AggressiveInlining)]
            public void Add(Link<TLinkType>[] src) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if (src == null) throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.Add ] src is null");
                #endif
                Add(src, 0, (ushort)src.Length);
            }

            /// <summary>
            /// Bulk adds a range of links from the source array. All links must be unique (not already present).
            /// Fires <see cref="ILinkType.OnAdd{TWorld}"/> for each added link if the hook is present.
            /// </summary>
            /// <param name="src">Source array of links.</param>
            /// <param name="srcIdx">Starting index in <paramref name="src"/>.</param>
            /// <param name="len">Number of links to add from <paramref name="src"/>.</param>
            /// <exception cref="StaticEcsException">In debug builds, thrown if arguments are out of range or contain duplicates.</exception>
            [MethodImpl(AggressiveInlining)]
            public void Add(Link<TLinkType>[] src, int srcIdx, ushort len) {
                if (len > 0) {
                    #if FFS_ECS_DEBUG
                    AssertNotBlockedByIteration();
                    if (src == null) throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.Add ] src is null");
                    if (srcIdx + len > src.Length) throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.Add ] srcIdx + len > src.Length");
                    for (var j = srcIdx; j < srcIdx + len; j++) {
                        AssertNotContains(src[j]);
                    }
                    #endif
                    ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                    var cap = SegmentAllocator.MinSlotCapacity << Level;
                    if (Count + len > cap) {
                        EnsureCapacityInternal(ref storage, (ushort)(Count + len));
                    }

                    Utils.LoopFallbackCopy(src, (uint)srcIdx, storage.Segments[SegmentIdx], Offset + Count, len);
                    Count += len;
                    if (storage.HasOnAdd) {
                        var owner = EntityOwner;
                        for (var i = srcIdx; i < srcIdx + len; i++) {
                            src[i].OnAdd(owner);
                        }
                    }
                }
            }
            #endregion

            void ILinksComponent.AddLink(EntityGID gid) => TryAdd(new Link<TLinkType>(gid));

            #region REMOVE
            /// <summary>
            /// Removes the link at the specified index, shifting subsequent elements left to fill the gap.
            /// Fires <see cref="ILinkType.OnDelete{TWorld}"/> on the removed link if the hook is present.
            /// </summary>
            /// <param name="idx">Zero-based index of the link to remove.</param>
            /// <exception cref="StaticEcsException">In debug builds, thrown if <paramref name="idx"/> is out of bounds.</exception>
            [MethodImpl(AggressiveInlining)]
            public void RemoveAt(int idx) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if (idx < 0 || idx >= Count) throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.RemoveAt ] index out of bounds: {idx}");
                #endif
                ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                var values = storage.Segments[SegmentIdx];
                if (storage.HasOnDelete) {
                    values[Offset + idx].OnDelete(EntityOwner, HookReason.Default);
                }
                Count--;
                if (idx == Count) {
                    values[Offset + idx] = default;
                }
                else {
                    Utils.LoopFallbackCopy(values, Offset + (uint)idx + 1, values, Offset + (uint)idx, (uint)(Count - idx));
                    values[Offset + Count] = default;
                }
            }

            /// <summary>
            /// Removes the link at the specified index by swapping it with the last element (O(1), does not preserve order).
            /// Optionally fires <see cref="ILinkType.OnDelete{TWorld}"/> on the removed link.
            /// </summary>
            /// <param name="idx">Zero-based index of the link to remove.</param>
            /// <param name="withOnDelete">If <c>true</c>, fires the OnDelete hook on the removed link (when the hook is present).</param>
            /// <exception cref="StaticEcsException">In debug builds, thrown if <paramref name="idx"/> is out of bounds.</exception>
            [MethodImpl(AggressiveInlining)]
            public void RemoveAtSwap(int idx, bool withOnDelete = true) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if (idx < 0 || idx >= Count) throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.RemoveAtSwap ] index out of bounds: {idx}");
                #endif
                ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                var values = storage.Segments[SegmentIdx];
                if (withOnDelete && storage.HasOnDelete) {
                    values[Offset + idx].OnDelete(EntityOwner, HookReason.Default);
                }
                values[Offset + idx] = values[Offset + --Count];
                values[Offset + Count] = default;
            }

            /// <summary>
            /// Removes the first link, shifting all subsequent elements left.
            /// Fires <see cref="ILinkType.OnDelete{TWorld}"/> on the removed link if the hook is present.
            /// </summary>
            /// <exception cref="StaticEcsException">In debug builds, thrown if the collection is empty.</exception>
            [MethodImpl(AggressiveInlining)]
            public void RemoveFirst() {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if (Count == 0) throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.RemoveFirst ] empty");
                #endif
                ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                var values = storage.Segments[SegmentIdx];
                if (storage.HasOnDelete) {
                    values[Offset].OnDelete(EntityOwner, HookReason.Default);
                }
                Count--;
                if (Count > 0) {
                    Utils.LoopFallbackCopy(values, Offset + 1, values, Offset, Count);
                    values[Offset + Count] = default;
                }
                else {
                    values[Offset] = default;
                }
            }

            /// <summary>
            /// Removes the first link by swapping it with the last element (O(1), does not preserve order).
            /// Fires <see cref="ILinkType.OnDelete{TWorld}"/> on the removed link if the hook is present.
            /// </summary>
            /// <exception cref="StaticEcsException">In debug builds, thrown if the collection is empty.</exception>
            [MethodImpl(AggressiveInlining)]
            public void RemoveFirstSwap() {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if (Count == 0) throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.RemoveFirstSwap ] empty");
                #endif
                ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                var values = storage.Segments[SegmentIdx];
                if (storage.HasOnDelete) {
                    values[Offset].OnDelete(EntityOwner, HookReason.Default);
                }
                values[Offset] = values[Offset + --Count];
                values[Offset + Count] = default;
            }

            /// <summary>
            /// Removes the last link in the collection.
            /// Fires <see cref="ILinkType.OnDelete{TWorld}"/> on the removed link if the hook is present.
            /// </summary>
            /// <exception cref="StaticEcsException">In debug builds, thrown if the collection is empty.</exception>
            [MethodImpl(AggressiveInlining)]
            public void RemoveLast() {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if (Count == 0) throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.RemoveLast ] empty");
                #endif
                ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                var values = storage.Segments[SegmentIdx];
                if (storage.HasOnDelete) {
                    values[Offset + Count - 1].OnDelete(EntityOwner, HookReason.Default);
                }
                values[Offset + --Count] = default;
            }

            /// <summary>
            /// Finds and removes the specified link, shifting subsequent elements left to preserve order.
            /// Fires <see cref="ILinkType.OnDelete{TWorld}"/> on the removed link if found and the hook is present.
            /// </summary>
            /// <param name="item">The link to find and remove.</param>
            /// <returns><c>true</c> if the link was found and removed; <c>false</c> if not found.</returns>
            [MethodImpl(AggressiveInlining)]
            public bool TryRemove(Link<TLinkType> item) {
                var idx = IndexOf(item);
                if (idx >= 0) {
                    RemoveAt(idx);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Finds and removes the specified link using swap-remove (O(1), does not preserve order).
            /// </summary>
            /// <param name="item">The link to find and remove.</param>
            /// <returns><c>true</c> if the link was found and removed; <c>false</c> if not found.</returns>
            [MethodImpl(AggressiveInlining)]
            public bool TryRemoveSwap(Link<TLinkType> item, bool withOnDelete = true) {
                var idx = IndexOf(item);
                if (idx >= 0) {
                    RemoveAtSwap(idx, withOnDelete);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Removes all links from the collection. Fires <see cref="ILinkType.OnDelete{TWorld}"/> on each link
            /// if the hook is present. Resets count to zero.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void Clear() {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                #endif
                if (Count > 0) {
                    ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                    var values = storage.Segments[SegmentIdx];
                    if (storage.HasOnDelete) {
                        var owner = EntityOwner;
                        for (var i = Offset; i < Offset + Count; i++) {
                            ref var val = ref values[i];
                            val.OnDelete(owner, HookReason.Default);
                            val = default;
                        }
                    }
                    else {
                        Utils.LoopFallbackClear(values, (int)Offset, Count);
                    }
                    Count = 0;
                }
            }

            /// <summary>
            /// Sets the count to zero without firing hooks or clearing underlying storage. Low-level operation
            /// intended for deserialization or manual reconstruction scenarios. Prefer <see cref="Clear"/> for normal use.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ResetCount() {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                #endif
                Count = 0;
            }
            #endregion

            #region UTILITY
            /// <summary>
            /// Returns the zero-based index of the specified link, or -1 if not found.
            /// </summary>
            /// <param name="item">The link to search for.</param>
            /// <returns>The index of the link, or -1 if not present.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly int IndexOf(Link<TLinkType> item) {
                var values = Resources<MultiValueStorage<Link<TLinkType>>>.Value.Segments[SegmentIdx];
                var indexOf = Array.IndexOf(values, item, (int)Offset, Count);
                return indexOf >= 0 ? (int)(indexOf - Offset) : -1;
            }

            /// <summary>
            /// Determines whether the collection contains the specified link.
            /// </summary>
            /// <param name="item">The link to search for.</param>
            /// <returns><c>true</c> if found; otherwise <c>false</c>.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly bool Contains(Link<TLinkType> item) {
                var values = Resources<MultiValueStorage<Link<TLinkType>>>.Value.Segments[SegmentIdx];
                for (var i = Offset; i < Offset + Count; i++) {
                    if (values[i].Equals(item)) return true;
                }

                return false;
            }

            /// <summary>
            /// Determines whether the collection contains the specified link using a custom equality comparer.
            /// </summary>
            /// <typeparam name="TComparer">The comparer type.</typeparam>
            /// <param name="item">The link to search for.</param>
            /// <param name="comparer">The equality comparer to use.</param>
            /// <returns><c>true</c> if found; otherwise <c>false</c>.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly bool Contains<TComparer>(Link<TLinkType> item, TComparer comparer) where TComparer : IEqualityComparer<Link<TLinkType>> {
                var values = Resources<MultiValueStorage<Link<TLinkType>>>.Value.Segments[SegmentIdx];
                for (var i = Offset; i < Offset + Count; i++) {
                    if (comparer.Equals(values[i], item)) return true;
                }

                return false;
            }

            /// <summary>
            /// Ensures the collection has enough capacity to accommodate the specified number of additional links
            /// without further reallocation.
            /// </summary>
            /// <param name="additionalSize">The number of additional links to ensure capacity for.</param>
            /// <exception cref="StaticEcsException">In debug builds, thrown if the resulting capacity would overflow the maximum level.</exception>
            [MethodImpl(AggressiveInlining)]
            public void EnsureSize(int additionalSize) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                if ((long)Count + additionalSize > SegmentAllocator.MinSlotCapacity << (SegmentAllocator.LevelCount - 1)) {
                    throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.EnsureSize ] capacity overflow: {Count} + {additionalSize}");
                }
                #endif
                var cap = SegmentAllocator.MinSlotCapacity << Level;
                if (Count + additionalSize > cap) {
                    ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                    EnsureCapacityInternal(ref storage, (ushort)(Count + additionalSize));
                }
            }

            /// <summary>
            /// Ensures the collection's capacity is at least <paramref name="newCapacity"/>.
            /// Does nothing if the current capacity already meets or exceeds the requested value.
            /// </summary>
            /// <param name="newCapacity">The minimum desired capacity.</param>
            [MethodImpl(AggressiveInlining)]
            public void Resize(ushort newCapacity) {
                #if FFS_ECS_DEBUG
                AssertNotBlockedByIteration();
                #endif
                var cap = SegmentAllocator.MinSlotCapacity << Level;
                if (cap < newCapacity) {
                    ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                    EnsureCapacityInternal(ref storage, newCapacity);
                }
            }

            /// <summary>
            /// Copies all links to the destination array starting at index 0.
            /// </summary>
            /// <param name="dst">The destination array. Must have sufficient length.</param>
            /// <exception cref="StaticEcsException">In debug builds, thrown if <paramref name="dst"/> is null or too small.</exception>
            [MethodImpl(AggressiveInlining)]
            public readonly void CopyTo(Link<TLinkType>[] dst) {
                #if FFS_ECS_DEBUG
                if (dst == null) throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.CopyTo ] dst is null");
                if (Count > dst.Length) throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.CopyTo ] count > dst.Length");
                #endif
                Utils.LoopFallbackCopy(Resources<MultiValueStorage<Link<TLinkType>>>.Value.Segments[SegmentIdx], Offset, dst, 0, Count);
            }

            /// <summary>
            /// Copies a specified number of links to the destination array at the given offset.
            /// </summary>
            /// <param name="dst">The destination array.</param>
            /// <param name="dstIdx">The starting index in <paramref name="dst"/>.</param>
            /// <param name="len">The number of links to copy.</param>
            /// <exception cref="StaticEcsException">In debug builds, thrown if arguments are out of range or <paramref name="dst"/> is null.</exception>
            [MethodImpl(AggressiveInlining)]
            public readonly void CopyTo(Link<TLinkType>[] dst, int dstIdx, int len) {
                #if FFS_ECS_DEBUG
                if (dst == null) throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.CopyTo ] dst is null");
                if (dstIdx + len > dst.Length) throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.CopyTo ] dstIdx + len > dst.Length");
                if (len > Count) throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.CopyTo ] len > count");
                #endif
                Utils.LoopFallbackCopy(Resources<MultiValueStorage<Link<TLinkType>>>.Value.Segments[SegmentIdx], Offset, dst, (uint)dstIdx, (uint)len);
            }

            /// <summary>
            /// Sorts the links in this collection using the default comparer.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly void Sort() {
                Array.Sort(Resources<MultiValueStorage<Link<TLinkType>>>.Value.Segments[SegmentIdx], (int)Offset, Count);
            }

            /// <summary>
            /// Sorts the links in this collection using the specified comparer.
            /// </summary>
            /// <typeparam name="TComparer">The comparer type.</typeparam>
            /// <param name="comparer">The comparer to use for ordering.</param>
            [MethodImpl(AggressiveInlining)]
            public readonly void Sort<TComparer>(TComparer comparer) where TComparer : IComparer<Link<TLinkType>> {
                Array.Sort(Resources<MultiValueStorage<Link<TLinkType>>>.Value.Segments[SegmentIdx], (int)Offset, Count, comparer);
            }

            /// <summary>
            /// Returns a comma-separated string representation of all links in the collection.
            /// </summary>
            public readonly override string ToString() {
                var res = "";
                var values = Resources<MultiValueStorage<Link<TLinkType>>>.Value.Segments[SegmentIdx];
                for (int i = 0; i < Count; i++) {
                    res += values[Offset + i].ToString();
                    if (i + 1 < Count) res += ", ";
                }

                return res;
            }

            /// <summary>
            /// Returns a read-only enumerator over the links in this collection.
            /// </summary>
            /// <returns>A <see cref="ROMultiComponentsIterator{T}"/> for iterating over the links.</returns>
            [MethodImpl(AggressiveInlining)]
            public ROMultiComponentsIterator<Link<TLinkType>> GetEnumerator() => new(
                Resources<MultiValueStorage<Link<TLinkType>>>.Value.Segments[SegmentIdx],
                SegmentIdx,
                Offset,
                Count
            );

            /// <summary>
            /// Compares two <see cref="Links{TLinkType}"/> instances for structural equality (same storage location and count).
            /// </summary>
            /// <param name="other">The other instance to compare against.</param>
            /// <returns><c>true</c> if both reference the same storage region with the same count; otherwise <c>false</c>.</returns>
            [MethodImpl(AggressiveInlining)]
            public bool Equals(Links<TLinkType> other) {
                return Offset == other.Offset && Count == other.Count && SegmentIdx == other.SegmentIdx && Level == other.Level &&
                       SegmentEntityIdx == other.SegmentEntityIdx;
            }

            /// <inheritdoc/>
            [MethodImpl(AggressiveInlining)]
            public override bool Equals(object obj) => throw new StaticEcsException($"Links<{typeof(TLinkType)}> `Equals object` not allowed!");

            /// <inheritdoc/>
            [MethodImpl(AggressiveInlining)]
            public override int GetHashCode() => throw new StaticEcsException($"Links<{typeof(TLinkType)}> `GetHashCode object` not allowed!");

            /// <summary>
            /// Equality operator. Compares two <see cref="Links{TLinkType}"/> for structural equality.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static bool operator ==(Links<TLinkType> left, Links<TLinkType> right) => left.Equals(right);

            /// <summary>
            /// Inequality operator. Compares two <see cref="Links{TLinkType}"/> for structural inequality.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static bool operator !=(Links<TLinkType> left, Links<TLinkType> right) => !left.Equals(right);
            #endregion

            #region ICOMPONENT
            [MethodImpl(AggressiveInlining)]
            public void OnAdd<TW>(World<TW>.Entity self) where TW : struct, IWorldType {
                var entityId = self.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentEntityIdx = (byte)(entityId & Const.ENTITIES_IN_SEGMENT_MASK);

                ref var storage = ref World<TW>.Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                storage.EnsureSegment(segmentIdx);

                SegmentIdx = segmentIdx;
                SegmentEntityIdx = segmentEntityIdx;
                Level = 0;
                Offset = storage.Allocators[segmentIdx].Allocate(Level, ref storage.Segments[segmentIdx]);
                Count = 0;
            }

            [MethodImpl(AggressiveInlining)]
            public void OnDelete<TW>(World<TW>.Entity self, HookReason reason) where TW : struct, IWorldType {
                ref var storage = ref World<TW>.Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                Clear();
                storage.Allocators[SegmentIdx].Free(Offset, Level, out var empty);
                if (empty) {
                    storage.ReleaseSegment(SegmentIdx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyTo<TW>(World<TW>.Entity self, World<TW>.Entity other, bool disabled) where TW : struct, IWorldType {
                ref var storage = ref World<TW>.Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                if (storage.HasOnCopy) {
                    ref var dst = ref World<TW>.Components<Links<TLinkType>>.Instance.Add(other);

                    var neededCapacity = (ushort)Utils.RoundUpToPowerOf2(Math.Max(Count, SegmentAllocator.MinSlotCapacity));
                    var neededLevel = SegmentAllocator.LevelForCapacity(neededCapacity);

                    if (dst.Level < neededLevel) {
                        storage.Allocators[dst.SegmentIdx].Free(dst.Offset, dst.Level, out _);
                        dst.Offset = storage.Allocators[dst.SegmentIdx].Allocate(neededLevel, ref storage.Segments[dst.SegmentIdx]);
                        dst.Level = neededLevel;
                    }

                    if (dst.Count > Count) {
                        Utils.LoopFallbackClear(storage.Segments[dst.SegmentIdx], (int)(dst.Offset + Count), dst.Count - Count);
                    }
                    Utils.LoopFallbackCopy(storage.Segments[SegmentIdx], Offset, storage.Segments[dst.SegmentIdx], dst.Offset, Count);
                    dst.Count = Count;

                    var srcValues = storage.Segments[SegmentIdx];
                    for (var i = Offset; i < Offset + Count; i++) {
                        srcValues[i].CopyTo(self, other, disabled);
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            public void Write<TW>(ref BinaryPackWriter writer, World<TW>.Entity self) where TW : struct, IWorldType {
                writer.WriteUshort(Count);
                if (Count > 0) {
                    writer.WriteArrayUnmanaged(World<TW>.Resources<MultiValueStorage<Link<TLinkType>>>.Value.Segments[SegmentIdx], (int)Offset, Count);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public void Read<TW>(ref BinaryPackReader reader, World<TW>.Entity self, byte version, bool disabled) where TW : struct, IWorldType {
                Count = reader.ReadUshort();
                var entityId = self.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentEntityIdx = (byte)(entityId & Const.ENTITIES_IN_SEGMENT_MASK);

                ref var storage = ref World<TW>.Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                storage.EnsureSegment(segmentIdx);

                SegmentIdx = segmentIdx;
                SegmentEntityIdx = segmentEntityIdx;
                Level = SegmentAllocator.LevelForCapacity(Count);
                Offset = storage.Allocators[segmentIdx].Allocate(Level, ref storage.Segments[segmentIdx]);
                
                
                if (Count > 0) {
                    reader.ReadArrayUnmanaged(ref storage.Segments[SegmentIdx], (int)Offset);
                }
            }
            
            void IComponentInternal.OnInitialize<TW>() {
                ref var storage = ref World<TW>.ResourcesData.Instance.GetOrCreate<MultiValueStorage<Link<TLinkType>>>(out var isNew);
                if (isNew) {
                    storage.Init(Data.Instance.EntitiesSegments.Length, LinkType<TLinkType>.HasOnAdd(), LinkType<TLinkType>.HasOnDelete(), LinkType<TLinkType>.HasCopyTo(), false, false);
                    storage.ElementStrategy = new UnmanagedPackArrayStrategy<Link<TLinkType>>();
                    World<TW>.Data.Instance.RegisterMultiStorageResizer(_ResizeStorage);
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
                if (Resources<MultiValueStorage<Link<TLinkType>>>.Value.IsBlockedByIteration(SegmentIdx, Offset)) {
                    throw new StaticEcsException($"[ Links<{typeof(TLinkType)}> ] Cannot modify while being iterated.");
                }
            }

            [MethodImpl(AggressiveInlining)]
            private readonly void AssertNotContains(Link<TLinkType> val) {
                var values = Resources<MultiValueStorage<Link<TLinkType>>>.Value.Segments[SegmentIdx];
                for (var i = Offset; i < Offset + Count; i++) {
                    if (values[i].Equals(val)) {
                        throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.Add ] duplicate value: {val}");
                    }
                }
            }
            #endif

            [MethodImpl(AggressiveInlining)]
            internal static void _ResizeStorage(uint segmentCapacity) {
                ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                if (storage.Segments != null) {
                    storage.EnsureCapacity((int)segmentCapacity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            private void Grow(ref MultiValueStorage<Link<TLinkType>> storage) {
                #if FFS_ECS_DEBUG
                if (Level >= SegmentAllocator.LevelCount - 1) {
                    throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.Grow ] max level reached: {Level}");
                }
                #endif
                var oldLevel = Level;
                Level++;
                Offset = storage.Allocators[SegmentIdx].Resize(Offset, Count, oldLevel, Level, ref storage.Segments[SegmentIdx]);
            }

            [MethodImpl(AggressiveInlining)]
            private void EnsureCapacityInternal(ref MultiValueStorage<Link<TLinkType>> storage, ushort minCapacity) {
                var newCapacity = Utils.RoundUpToPowerOf2(Math.Max(minCapacity, SegmentAllocator.MinSlotCapacity));
                if (newCapacity <= (uint)(SegmentAllocator.MinSlotCapacity << Level)) return;

                var newLevel = SegmentAllocator.LevelForCapacity(newCapacity);
                #if FFS_ECS_DEBUG
                if (newLevel >= SegmentAllocator.LevelCount) {
                    throw new StaticEcsException($"[ Links<{typeof(TLinkType)}>.EnsureCapacityInternal ] level overflow: {newLevel}");
                }
                #endif
                Offset = storage.Allocators[SegmentIdx].Resize(Offset, Count, Level, newLevel, ref storage.Segments[SegmentIdx]);
                Level = newLevel;
            }
            #endregion
            
            /// <summary>
            /// Implicitly converts a <see cref="Links{TLinkType}"/> to a <see cref="ReadOnlySpan{T}"/> over its links.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static implicit operator ReadOnlySpan<Link<TLinkType>>(Links<TLinkType> multi) {
                return multi.AsReadOnlySpan;
            }

        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct DeepDestroyLinkFunction<TLinkType> where TLinkType : unmanaged, ILinkType {
            internal bool Active;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(EntityGID value) {
                if (!Active) {
                    Active = true;

                    ref var components = ref Components<Link<TLinkType>>.Instance;

                    var gid = value;
                    while (gid.TryUnpack<TWorld>(out var entity)) {
                        var hasLink = components.TryGet(entity, out var link);
                        entity.Destroy();
                        if (!hasLink) break;
                        gid = link;
                    }

                    Active = false;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct DeepDestroyLinksFunction<TLinkType> where TLinkType : unmanaged, ILinksType {
            internal Stack<EntityGID> Ranges;
            internal bool Active;

            [MethodImpl(AggressiveInlining)]
            public void Invoke(EntityGID value) {
                if (!Active) {
                    Active = true;
                    Ranges ??= new Stack<EntityGID>(64);

                    ref var components = ref Components<Links<TLinkType>>.Instance;
                    ref var storage = ref Resources<MultiValueStorage<Link<TLinkType>>>.Value;
                    Ranges.Push(value);

                    while (Ranges.Count > 0) {
                        var gid = Ranges.Pop();
                        if (gid.TryUnpack<TWorld>(out var entity)) {
                            if (components.TryGet(entity, out var links)) {
                                var values = storage.Segments[links.SegmentIdx];
                                for (var i = links.Offset; i < links.Offset + links.Count; i++) {
                                    Ranges.Push(values[i]);
                                }
                            }
                            entity.Destroy();
                        }
                    }

                    Active = false;
                }
            }
        }
    }

    /// <summary>
    /// Status codes returned by link operations (e.g., <see cref="Link.TryAddLink{TW,TLink}"/>, <see cref="Links.TryAddLinkItem{TW,TLink}"/>).
    /// </summary>
    public enum LinkOppStatus : byte {
        /// <summary>The operation completed successfully.</summary>
        Ok = 0,
        /// <summary>The link already exists on the target entity (duplicate).</summary>
        LinkAlreadyExists = 1,
        /// <summary>The specified link was not found on the target entity.</summary>
        LinkNotExists = 2,
        /// <summary>The target entity's <see cref="EntityGID"/> refers to an unloaded entity.</summary>
        LinkNotLoaded = 3,
        /// <summary>The target entity's <see cref="EntityGID"/> is stale (the entity has been destroyed and its version no longer matches).</summary>
        LinkNotActual = 4,
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    /// <summary>
    /// Extension methods for single-link (<see cref="World{TWorld}.Link{TLinkType}"/>) operations on <see cref="EntityGID"/>.
    /// Provides safe add, delete, deep-destroy, and deep-copy operations that validate the target entity's liveness.
    /// </summary>
    public static class Link {
        /// <summary>
        /// Adds or replaces a single link of type <typeparamref name="TLink"/> on the target entity.
        /// If the target already has a link to the same entity, returns <see cref="LinkOppStatus.LinkAlreadyExists"/>.
        /// If the link points to a different entity, the old link is replaced (OnDelete then OnAdd are fired).
        /// </summary>
        /// <typeparam name="TW">The world type.</typeparam>
        /// <typeparam name="TLink">The link type.</typeparam>
        /// <param name="targetGID">The <see cref="EntityGID"/> of the entity to add the link to.</param>
        /// <param name="link">The entity to link to.</param>
        /// <param name="withCyclicHooks">If <c>true</c>, fires OnAdd/OnDelete hooks even when this is a cyclic (re-entrant) call.</param>
        /// <returns>A <see cref="LinkOppStatus"/> indicating the result of the operation.</returns>
        [MethodImpl(AggressiveInlining)]
        public static LinkOppStatus TryAddLink<TW, TLink>(this EntityGID targetGID, World<TW>.Entity link, bool withCyclicHooks = false)
            where TW : struct, IWorldType
            where TLink : unmanaged, ILinkType {
            if (targetGID.TryUnpack<TW>(out var target, out var status)) {
                var linkGID = link.GID;
                ref var components = ref World<TW>.Components<World<TW>.Link<TLink>>.Instance;
                if (!components.TryGet(target, out var stored) || !stored.Value.Equals(linkGID)) {
                    components.Set(target, new World<TW>.Link<TLink>(linkGID), withCyclicHooks);
                    return LinkOppStatus.Ok;
                }
                
                return LinkOppStatus.LinkAlreadyExists;
            }

            return status == GIDStatus.NotLoaded ? LinkOppStatus.LinkNotLoaded : LinkOppStatus.LinkNotActual;
        }

        /// <summary>
        /// Deletes the single link of type <typeparamref name="TLink"/> from the target entity,
        /// but only if it currently points to the specified <paramref name="link"/> entity.
        /// Returns <see cref="LinkOppStatus.LinkNotExists"/> if the link component is absent or points elsewhere.
        /// </summary>
        /// <typeparam name="TW">The world type.</typeparam>
        /// <typeparam name="TLink">The link type.</typeparam>
        /// <param name="targetGID">The <see cref="EntityGID"/> of the entity to remove the link from.</param>
        /// <param name="link">The expected linked entity. Deletion only occurs if the current link matches.</param>
        /// <param name="withCyclicHooks">If <c>true</c>, fires OnDelete hook even when this is a cyclic (re-entrant) call.</param>
        /// <returns>A <see cref="LinkOppStatus"/> indicating the result of the operation.</returns>
        [MethodImpl(AggressiveInlining)]
        public static LinkOppStatus TryDeleteLink<TW, TLink>(this EntityGID targetGID, World<TW>.Entity link, bool withCyclicHooks = false)
            where TW : struct, IWorldType
            where TLink : unmanaged, ILinkType {
            if (targetGID.TryUnpack<TW>(out var target, out var status)) {
                ref var components = ref World<TW>.Components<World<TW>.Link<TLink>>.Instance;
                if (components.TryGet(target, out var stored) && stored.Value.Equals(link.GID)) {
                    components.Delete(target, withOnDelete: withCyclicHooks);
                    return LinkOppStatus.Ok;
                }
                
                return LinkOppStatus.LinkNotExists;
            }

            return status == GIDStatus.NotLoaded 
                ? LinkOppStatus.LinkNotLoaded 
                : LinkOppStatus.LinkNotActual;
        }

        /// <summary>
        /// Recursively destroys an entity chain following single links of type <typeparamref name="TLink"/>.
        /// Starting from <paramref name="targetGID"/>, each entity in the chain is destroyed and its link is followed
        /// until an entity without the link or a dead/unloaded entity is reached.
        /// <para>Re-entrant calls (cycles) are detected and safely ignored.</para>
        /// </summary>
        /// <typeparam name="TW">The world type.</typeparam>
        /// <typeparam name="TLink">The link type to follow.</typeparam>
        /// <param name="targetGID">The <see cref="EntityGID"/> of the first entity in the chain to destroy.</param>
        [MethodImpl(AggressiveInlining)]
        public static void DeepDestroyLink<TW, TLink>(this EntityGID targetGID)
            where TW : struct, IWorldType 
            where TLink : unmanaged, ILinkType {
            World<TW>.ResourcesData.Instance.GetOrCreate<World<TW>.DeepDestroyLinkFunction<TLink>>(out _).Invoke(targetGID);
        }

        /// <summary>
        /// Clones the entity referenced by <paramref name="link"/> (via <see cref="World{TWorld}.Entity.Clone()"/>)
        /// and returns a new <see cref="World{TWorld}.Link{TLinkType}"/> pointing to the clone.
        /// </summary>
        /// <typeparam name="TW">The world type.</typeparam>
        /// <typeparam name="TLink">The link type.</typeparam>
        /// <param name="link">The <see cref="EntityGID"/> of the entity to clone.</param>
        /// <param name="copied">When successful, receives the new link pointing to the cloned entity.</param>
        /// <returns><see cref="LinkOppStatus.Ok"/> on success, or an error status if the entity is not loaded or not actual.</returns>
        [MethodImpl(AggressiveInlining)]
        public static LinkOppStatus TryDeepCopyLink<TW, TLink>(this EntityGID link, out World<TW>.Link<TLink> copied)
            where TW : struct, IWorldType 
            where TLink : unmanaged, ILinkType {
            if (link.TryUnpack<TW>(out var entity, out var status)) {
                copied = new World<TW>.Link<TLink>(entity.Clone().GID);
                return LinkOppStatus.Ok;
            }

            copied = default;
            return status == GIDStatus.NotLoaded 
                ? LinkOppStatus.LinkNotLoaded 
                : LinkOppStatus.LinkNotActual;
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    /// <summary>
    /// Extension methods for multi-link (<see cref="World{TWorld}.Links{TLinkType}"/>) operations on <see cref="EntityGID"/>.
    /// Provides safe add, delete, and deep-destroy operations that validate the target entity's liveness
    /// and automatically manage the <see cref="World{TWorld}.Links{TLinkType}"/> component lifecycle.
    /// </summary>
    public static class Links {
        /// <summary>
        /// Adds a link to the target entity's <see cref="World{TWorld}.Links{TLinkType}"/> collection.
        /// If the target does not yet have the <see cref="World{TWorld}.Links{TLinkType}"/> component, it is created automatically.
        /// Returns <see cref="LinkOppStatus.LinkAlreadyExists"/> if the link is already present in the collection.
        /// </summary>
        /// <typeparam name="TW">The world type.</typeparam>
        /// <typeparam name="TLink">The multi-link type (must implement <see cref="ILinksType"/>).</typeparam>
        /// <param name="targetGID">The <see cref="EntityGID"/> of the entity to add the link to.</param>
        /// <param name="link">The entity to link to.</param>
        /// <param name="withCyclicHooks">If <c>true</c>, fires the OnAdd hook even when this is a cyclic (re-entrant) call.</param>
        /// <returns>A <see cref="LinkOppStatus"/> indicating the result of the operation.</returns>
        [MethodImpl(AggressiveInlining)]
        public static LinkOppStatus TryAddLinkItem<TW, TLink>(this EntityGID targetGID, World<TW>.Entity link, bool withCyclicHooks = false)
            where TW : struct, IWorldType
            where TLink : unmanaged, ILinksType {
            if (targetGID.TryUnpack<TW>(out var target, out var status)) {
                var linkGID = link.GID;
                ref var components = ref World<TW>.Components<World<TW>.Links<TLink>>.Instance;
                ref var links = ref components.Add(target);
                return links.TryAdd(new World<TW>.Link<TLink>(linkGID), withCyclicHooks) 
                    ? LinkOppStatus.Ok 
                    : LinkOppStatus.LinkAlreadyExists;
            }

            return status == GIDStatus.NotLoaded 
                ? LinkOppStatus.LinkNotLoaded 
                : LinkOppStatus.LinkNotActual;
        }
            
        /// <summary>
        /// Removes a specific link from the target entity's <see cref="World{TWorld}.Links{TLinkType}"/> collection.
        /// If the collection becomes empty after removal, the <see cref="World{TWorld}.Links{TLinkType}"/> component
        /// is automatically deleted from the entity.
        /// </summary>
        /// <typeparam name="TW">The world type.</typeparam>
        /// <typeparam name="TLink">The multi-link type (must implement <see cref="ILinksType"/>).</typeparam>
        /// <param name="targetGID">The <see cref="EntityGID"/> of the entity to remove the link from.</param>
        /// <param name="link">The entity whose link should be removed.</param>
        /// <param name="withCyclicHooks">If <c>true</c>, fires the OnDelete hook even when this is a cyclic (re-entrant) call.</param>
        /// <returns>A <see cref="LinkOppStatus"/> indicating the result of the operation.</returns>
        [MethodImpl(AggressiveInlining)]
        public static LinkOppStatus TryDeleteLinkItem<TW, TLink>(this EntityGID targetGID, World<TW>.Entity link, bool withCyclicHooks = false)
            where TW : struct, IWorldType
            where TLink : unmanaged, ILinksType {
            if (targetGID.TryUnpack<TW>(out var target, out var status)) {
                ref var components = ref World<TW>.Components<World<TW>.Links<TLink>>.Instance;
                if (components.Has(target)) {
                    ref var links = ref components.Ref(target);
                    var result = links.TryRemoveSwap(new World<TW>.Link<TLink>(link.GID), withCyclicHooks);
                    if (links.IsEmpty) {
                        components.Delete(target);
                    }
                    
                    return result ? LinkOppStatus.Ok : LinkOppStatus.LinkNotExists;
                }
                
                return LinkOppStatus.LinkNotExists;
            }

            return status == GIDStatus.NotLoaded 
                ? LinkOppStatus.LinkNotLoaded 
                : LinkOppStatus.LinkNotActual;
        }
        
        /// <summary>
        /// Recursively destroys an entity graph following multi-links of type <typeparamref name="TLink"/>.
        /// Starting from <paramref name="targetGID"/>, performs a depth-first traversal: for each entity,
        /// all links in its <see cref="World{TWorld}.Links{TLinkType}"/> collection are pushed onto a stack,
        /// then the entity is destroyed. Continues until the stack is empty.
        /// <para>Re-entrant calls (cycles) are detected and safely ignored.</para>
        /// </summary>
        /// <typeparam name="TW">The world type.</typeparam>
        /// <typeparam name="TLink">The multi-link type to follow (must implement <see cref="ILinksType"/>).</typeparam>
        /// <param name="targetGID">The <see cref="EntityGID"/> of the root entity to begin destroying from.</param>
        [MethodImpl(AggressiveInlining)]
        public static void DeepDestroyLinkItem<TW, TLink>(this EntityGID targetGID)
            where TW : struct, IWorldType 
            where TLink : unmanaged, ILinksType {
            World<TW>.ResourcesData.Instance.GetOrCreate<World<TW>.DeepDestroyLinksFunction<TLink>>(out _).Invoke(targetGID);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    [Il2CppEagerStaticClassConstruction]
    #endif
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    /// <summary>
    /// Bulk segment serialization strategy for <see cref="World{TWorld}.Links{TLinkType}"/> components.
    /// <para>
    /// Writes raw <c>Links&lt;TLinkType&gt;</c> struct bytes via unmanaged bulk copy and serializes
    /// the underlying <see cref="MultiValueStorage{TValue}"/> segments (containing <see cref="World{TWorld}.Link{TLinkType}"/>
    /// values) with their <see cref="SegmentAllocator"/> state. On deserialization, segments and
    /// allocators are restored in bulk — no per-entity data copying.
    /// </para>
    /// <para>
    /// Segment deduplication and entity-level serialization behavior are identical to
    /// <see cref="MultiUnmanagedPackArrayStrategy{TWorld, TValue}"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TWorld">World type for static storage access.</typeparam>
    /// <typeparam name="TLinkType">Link type implementing <see cref="ILinksType"/>. Must be <c>unmanaged</c>.</typeparam>
    /// <example>
    /// <code>
    /// W.Types().Links&lt;MyLinkType&gt;(new ComponentTypeConfig&lt;W.Links&lt;MyLinkType&gt;&gt;(
    ///     guid: new Guid("..."),
    ///     readWriteStrategy: new LinksUnmanagedPackArrayStrategy&lt;MyWorld, MyLinkType&gt;()
    /// ));
    /// </code>
    /// </example>
    public sealed class LinksUnmanagedPackArrayStrategy<TWorld, TLinkType>
        : IPackArrayStrategy<World<TWorld>.Links<TLinkType>>, IPackArrayStrategyResettable
        where TWorld : struct, IWorldType
        where TLinkType : unmanaged, ILinksType {

        private World<TWorld>.Links<TLinkType>[] _lastWriteArray;

        [MethodImpl(AggressiveInlining)]
        public bool IsUnmanaged() => true;

        [MethodImpl(AggressiveInlining)]
        public void Reset() {
            _lastWriteArray = null;
        }

        [MethodImpl(AggressiveInlining)]
        public void Register() {
            new UnmanagedPackArrayStrategy<World<TWorld>.Links<TLinkType>>().Register();
        }

        [MethodImpl(AggressiveInlining)]
        public void WriteArray(ref BinaryPackWriter writer, World<TWorld>.Links<TLinkType>[] value, int idx, int count) {
            writer.WriteArrayUnmanaged(value, idx, count);

            var newSegment = !ReferenceEquals(value, _lastWriteArray);
            _lastWriteArray = value;
            writer.WriteBool(newSegment);

            if (newSegment) {
                var segIdx = value[idx].SegmentIdx;
                ref var storage = ref World<TWorld>.Resources<MultiValueStorage<World<TWorld>.Link<TLinkType>>>.Value;
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
                    storage.ElementStrategy.WriteArray(ref writer, storage.Segments[segIdx], 0, (int)alloc.Used);
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void ReadArray(ref BinaryPackReader reader, ref World<TWorld>.Links<TLinkType>[] result, int idx) {
            reader.ReadArrayUnmanaged(ref result, idx);

            if (reader.ReadBool()) {
                var segIdx = result[idx].SegmentIdx;
                ref var storage = ref World<TWorld>.Resources<MultiValueStorage<World<TWorld>.Link<TLinkType>>>.Value;

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
                    storage.ElementStrategy.ReadArray(
                        ref reader, ref storage.Segments[segIdx], 0);
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void WriteArray(ref BinaryPackWriter writer, World<TWorld>.Links<TLinkType>[] value) {
            writer.WriteArrayUnmanaged(value);
        }

        [MethodImpl(AggressiveInlining)]
        public World<TWorld>.Links<TLinkType>[] ReadArray(ref BinaryPackReader reader) {
            return reader.ReadArrayUnmanaged<World<TWorld>.Links<TLinkType>>();
        }

        [MethodImpl(AggressiveInlining)]
        public void ReadArray(ref BinaryPackReader reader, ref World<TWorld>.Links<TLinkType>[] result) {
            reader.ReadArrayUnmanaged(ref result);
        }

        #if !FFS_PACK_DISABLE_MULTI_ARRAYS && !UNITY_WEBGL
        [MethodImpl(AggressiveInlining)]
        public void WriteArray(ref BinaryPackWriter writer, World<TWorld>.Links<TLinkType>[,] value) {
            writer.WriteArrayUnmanaged(value);
        }

        [MethodImpl(AggressiveInlining)]
        public void WriteArray(ref BinaryPackWriter writer, World<TWorld>.Links<TLinkType>[,,] value) {
            writer.WriteArrayUnmanaged(value);
        }

        [MethodImpl(AggressiveInlining)]
        public World<TWorld>.Links<TLinkType>[,] ReadArray2D(ref BinaryPackReader reader) {
            return reader.ReadArray2DUnmanaged<World<TWorld>.Links<TLinkType>>();
        }

        [MethodImpl(AggressiveInlining)]
        public World<TWorld>.Links<TLinkType>[,,] ReadArray3D(ref BinaryPackReader reader) {
            return reader.ReadArray3DUnmanaged<World<TWorld>.Links<TLinkType>>();
        }
        #endif
    }

    internal static class LinkType<
        #if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
        #endif
        T> where T : unmanaged, ILinkType {
        private static readonly Type[] OnAddParams = {
            typeof(World<>.Entity),
            typeof(EntityGID)
        };
        private static readonly Type[] OnDeleteParams = {
            typeof(World<>.Entity),
            typeof(EntityGID),
            typeof(HookReason)
        };
        private static readonly Type[] CopyToParams = {
            typeof(World<>.Entity),
            typeof(World<>.Entity),
            typeof(EntityGID)
        };

        internal static bool HasOnAdd() {
            return HasMethod(typeof(T), nameof(ILinkType.OnAdd), OnAddParams);
        }
        
        internal static bool HasOnDelete() {
            return HasMethod(typeof(T), nameof(ILinkType.OnDelete), OnDeleteParams);
        }
        
        internal static bool HasCopyTo() {
            return HasMethod(typeof(T), nameof(ILinkType.CopyTo), CopyToParams);
        }
        
        private static bool HasMethod(
            #if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
            #endif
            Type structType, string methodName, Type[] parameterTypes) {
            var methods = structType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var methodInfo in methods) {
                if (methodInfo.Name == methodName && methodInfo.IsGenericMethodDefinition) {
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