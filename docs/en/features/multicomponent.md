---
title: Multi-component
parent: Features
nav_order: 5
---

## MultiComponent
Multi-components are optimized list-components that allow storing multiple values of the same type on a single entity
- All elements of all multi-components of one type for all entities are stored in a unified storage — optimal memory usage
- Capacity from 4 to 32768 values per component, automatic expansion
- No need to create arrays or lists inside a component — zero heap allocations
- Implements [component](component.md), all base rules apply
- Entity [relations](relations.md) (`Links<T>`) are built on top of multi-components

___

## Type definition

The multi-component value type must implement the interface `IMultiComponent` and be a `struct`:

```csharp
// Unmanaged type — serialization works automatically via bulk memory copy
public struct Item : IMultiComponent {
    public int Id;
    public float Weight;
}
```

Non-unmanaged (managed) types must implement `Write`/`Read` hooks for serialization:

```csharp
// Managed type — requires Write/Read hooks for serialization
public struct NamedItem : IMultiComponent {
    public string Name;
    public int Count;

    public void Write(ref BinaryPackWriter writer) {
        writer.Write(in Name);
        writer.Write(in Count);
    }

    public void Read(ref BinaryPackReader reader) {
        Name = reader.Read<string>();
        Count = reader.ReadInt();
    }
}
```

___

## Serialization strategy

By default, `StructPackArrayStrategy<T>` is used for element serialization (per-element via hooks).
For unmanaged types, you can use `UnmanagedPackArrayStrategy<T>` for bulk memory copy (faster).

The strategy can be specified in three ways:

**1. Explicit parameter at registration:**
```csharp
W.Types()
    .Multi<Item>(elementStrategy: new UnmanagedPackArrayStrategy<Item>());
```

**2. Static field/property on the type (for auto-registration via `RegisterAll`):**
```csharp
public struct Item : IMultiComponent {
    public int Id;
    public float Weight;

    static readonly IPackArrayStrategy<Item> PackStrategy = new UnmanagedPackArrayStrategy<Item>();
}
```

**3. Default:** `StructPackArrayStrategy<T>` — uses `Write`/`Read` hooks per element.

___

## Bulk segment serialization

For chunk/world/cluster snapshots, when `TValue` is unmanaged, you can use `MultiUnmanagedPackArrayStrategy<TWorld, TValue>` to serialize entire storage segments as memory blocks instead of per-entity element data. This replaces many small per-entity copies with one bulk operation per segment and restores the allocator state directly.

```csharp
W.Types()
    .Multi<Item>(new ComponentTypeConfig<W.Multi<Item>>(
        guid: new Guid("..."),
        readWriteStrategy: new MultiUnmanagedPackArrayStrategy<MyWorld, Item>()
    ));
```

{: .note }
This strategy serializes the raw `Multi<T>` struct bytes plus the underlying value storage segments and allocator state. Entity-level serialization (`EntitiesSnapshot`) continues using per-entity `Write`/`Read` hooks — this optimization only applies to chunk/world/cluster snapshots.

{: .important }
`MultiUnmanagedPackArrayStrategy` requires `Multi<TValue>` to satisfy the `unmanaged` constraint. Since `Multi<T>` fields are all value types, this works for concrete `TValue` types but **cannot be used in generic registration code** — specify it explicitly for each concrete type.

___

## Registration

```csharp
W.Create(WorldConfig.Default());

W.Types()
    .Multi<Item>()                                                         // default strategy (StructPackArrayStrategy)
    .Multi<Item>(elementStrategy: new UnmanagedPackArrayStrategy<Item>())   // explicit unmanaged strategy
    .Multi<NamedItem>();                                                    // managed type with hooks

W.Initialize();
```

___

## Basic operations

Multi-components work like regular components:

```csharp
// Add (initial capacity — 4 elements, expands automatically)
ref var items = ref entity.Add<W.Multi<Item>>();

// Get reference
ref var items = ref entity.Ref<W.Multi<Item>>();

// Check presence
bool has = entity.Has<W.Multi<Item>>();

// Delete (element list is cleared automatically)
entity.Delete<W.Multi<Item>>();

// On clone and copy — all elements are copied automatically
var clone = entity.Clone();
entity.CopyTo<W.Multi<Item>>(targetEntity);
```

___

## Properties

```csharp
ref var items = ref entity.Ref<W.Multi<Item>>();

ushort len = items.Length;       // Number of elements
ushort cap = items.Capacity;     // Current capacity
bool empty = items.IsEmpty;      // Empty
bool notEmpty = items.IsNotEmpty; // Not empty
bool full = items.IsFull;        // Filled to capacity

// Index access (returns ref)
ref var first = ref items[0];
ref var last = ref items[items.Length - 1];

// First and last element
ref var f = ref items.First();
ref var l = ref items.Last();

// Span for direct memory access
Span<Item> span = items.AsSpan;
ReadOnlySpan<Item> roSpan = items.AsReadOnlySpan;

// Implicit conversion to Span
Span<Item> span = items;
ReadOnlySpan<Item> roSpan = items;
```

___

## Adding

```csharp
// Single element
items.Add(new Item { Id = 1, Weight = 0.5f });

// Multiple (from 2 to 4)
items.Add(
    new Item { Id = 1, Weight = 0.5f },
    new Item { Id = 2, Weight = 1.0f }
);

items.Add(
    new Item { Id = 1, Weight = 0.5f },
    new Item { Id = 2, Weight = 1.0f },
    new Item { Id = 3, Weight = 1.5f },
    new Item { Id = 4, Weight = 2.0f }
);

// From array
Item[] array = { new Item { Id = 5 }, new Item { Id = 6 } };
items.Add(array);

// From array slice
items.Add(array, srcIdx: 0, len: 1);

// Insert at index (remaining elements are shifted)
items.InsertAt(idx: 1, new Item { Id = 10 });
```

#### Capacity management:
```csharp
// Ensure space for N additional elements
items.EnsureSize(10);

// Increase Length by N (with pre-expansion if needed)
items.EnsureCount(5);

// Increase Length by N without initializing data (low-level operation)
items.EnsureCountUninitialized(5);

// Set minimum capacity
items.Resize(32);
```

___

## Removing

```csharp
// By index (order-preserving — shifts elements)
items.RemoveAt(idx: 1);

// By index (swap-remove — replaces with last, faster, order not preserved)
items.RemoveAtSwap(idx: 1);

// First element
items.RemoveFirst();       // order-preserving
items.RemoveFirstSwap();   // swap-remove

// Last element
items.RemoveLast();

// By value (returns true if found)
bool removed = items.TryRemove(new Item { Id = 1 });

// By value with swap-remove
bool removed = items.TryRemoveSwap(new Item { Id = 1 });

// Two elements by value
items.TryRemove(new Item { Id = 1 }, new Item { Id = 2 });

// Clear all elements
items.Clear();

// Reset count without clearing data (low-level operation)
items.ResetCount();
```

___

## Search

```csharp
// Element index (-1 if not found)
int idx = items.IndexOf(new Item { Id = 1 });

// Check presence
bool exists = items.Contains(new Item { Id = 1 });

// With custom comparer
bool exists = items.Contains(new Item { Id = 1 }, comparer);
```

___

## Iteration

```csharp
// foreach — mutable access by reference
foreach (ref var item in items) {
    item.Weight *= 2f;
}

// for — access by index
for (int i = 0; i < items.Length; i++) {
    ref var item = ref items[i];
    item.Weight *= 2f;
}

// Via Span
foreach (ref var item in items.AsSpan) {
    item.Weight *= 2f;
}
```

___

## Copying and sorting

```csharp
// Copy to array
var array = new Item[items.Length];
items.CopyTo(array);

// Copy slice
items.CopyTo(array, dstIdx: 0, len: 5);

// Sort
items.Sort();

// With custom comparer
items.Sort(comparer);
```

___

## Queries

Multi-components are used in queries like regular components:

```csharp
// All entities with inventory
W.Query().For(static (W.Entity entity, ref W.Multi<Item> items) => {
    for (int i = 0; i < items.Length; i++) {
        ref var item = ref items[i];
        // ...
    }
});

// With filtering
foreach (var entity in W.Query<All<W.Multi<Item>>>().Entities()) {
    ref var items = ref entity.Ref<W.Multi<Item>>();
    // ...
}
```
