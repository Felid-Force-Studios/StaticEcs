---
title: Relations
parent: Features
nav_order: 9
---

## Relations
Relations are a mechanism for linking entities to each other through typed link components
- `Link<T>` — link to a single entity (wrapper over `EntityGID`)
- `Links<T>` — link to multiple entities (dynamic collection of `Link<T>`)
- Links are regular components and work through the standard API (`Add`, `Ref`, `Delete`, `Has`)
- Support hooks (`OnAdd`, `OnDelete`, `CopyTo`) for automating logic (e.g., back-references)

___

## Link types

To define a link type, implement one of the interfaces:

```csharp
// ILinkType — type for a single link (Link<T>)
// Implement only the hooks you need
public struct Parent : ILinkType {
    // Called when a link is added
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        // self — entity to which the link was added
        // link — GID of the target entity
    }

    // Called when a link is removed
    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        // ...
    }

    // Called when the entity is copied (Clone/CopyTo)
    public void CopyTo<TW>(World<TW>.Entity self, World<TW>.Entity other, EntityGID link) where TW : struct, IWorldType {
        // ...
    }
}

// ILinksType — type for a multi-link (Links<T>)
// Inherits from ILinkType, same hooks
public struct Children : ILinksType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        // ...
    }
}

// Type without hooks — simply don't implement the methods
public struct FollowTarget : ILinkType { }
```

{: .important }
Do not leave empty hook implementations. If a hook is not needed — don't implement it. Unimplemented hooks are not called and create no overhead.

___

## Link\<T\>

Single link component — wrapper over `EntityGID` (8 bytes).

```csharp
// Properties
EntityGID value = link.Value;    // GID of the target entity (read-only)

// Implicit conversions
W.Link<Parent> link = entity;              // Entity → Link<T>
W.Link<Parent> link = entity.GID;          // EntityGID → Link<T>
W.Link<Parent> link = entity.GIDCompact;   // EntityGIDCompact → Link<T>
EntityGID gid = link;                      // Link<T> → EntityGID

// Creation via constructor
var link = new W.Link<Parent>(targetGID);

// Creation via entity.AsLink
W.Link<Parent> link = entity.AsLink<Parent>();
```

___

## Links\<T\>

Multi-component — dynamic collection of `Link<T>` with automatic memory management.

#### Properties:
```csharp
ref var links = ref entity.Ref<W.Links<Children>>();

ushort len = links.Length;       // Number of items
ushort cap = links.Capacity;     // Current capacity
bool empty = links.IsEmpty;      // Empty
bool notEmpty = links.IsNotEmpty; // Not empty
bool full = links.IsFull;        // Filled to capacity

// Index access
W.Link<Children> first = links[0];
W.Link<Children> last = links[links.Length - 1];

// First and last item
W.Link<Children> f = links.First();
W.Link<Children> l = links.Last();

// Read-only span
ReadOnlySpan<W.Link<Children>> span = links.AsReadOnlySpan;

// Iteration
foreach (var link in links) {
    if (link.Value.TryUnpack<WT>(out var child)) {
        // ...
    }
}
```

#### Adding:
```csharp
// TryAdd — does not add if already exists, returns false
bool added = links.TryAdd(childLink);

// TryAdd multiple (from 2 to 4)
links.TryAdd(child1, child2);
links.TryAdd(child1, child2, child3, child4);

// Add — adds, throws in DEBUG on duplicate
links.Add(childLink);
links.Add(child1, child2);

// Add from array
links.Add(childArray);
links.Add(childArray, srcIdx: 0, len: 3);
```

#### Removing:
```csharp
// By value (returns true if found)
bool removed = links.TryRemove(childLink);

// By value with swap-remove (does not preserve order, faster)
bool removed = links.TryRemoveSwap(childLink);

// By index
links.RemoveAt(0);
links.RemoveAtSwap(0);

// First / last
links.RemoveFirst();
links.RemoveFirstSwap();
links.RemoveLast();

// Remove all (calls OnDelete for each item)
links.Clear();
```

#### Search:
```csharp
bool exists = links.Contains(childLink);
int idx = links.IndexOf(childLink);
```

#### Memory management:
```csharp
links.EnsureSize(10);        // Ensure space for 10 additional items
links.Resize(32);            // Change capacity
links.Sort();                // Sort
```

___

## Registration

Links are registered as regular components during world creation:

```csharp
W.Create(WorldConfig.Default());

W.Types()
    .Link<Parent>()
    .Links<Children>();

W.Initialize();
```

___

## Working with links

Links are regular components. All standard methods work:

```csharp
var parent = W.NewEntity<Default>();
var child1 = W.NewEntity<Default>();
var child2 = W.NewEntity<Default>();

// Add a single link
child1.Set(new W.Link<Parent>(parent));
child2.Set(new W.Link<Parent>(parent));

// Get reference
ref var parentLink = ref child1.Ref<W.Link<Parent>>();
EntityGID parentGID = parentLink.Value;

// Check presence
bool hasParent = child1.Has<W.Link<Parent>>();

// Delete link
child1.Delete<W.Link<Parent>>();

// Add multi-link
ref var children = ref parent.Add<W.Links<Children>>();
children.TryAdd(child1.AsLink<Children>());
children.TryAdd(child2.AsLink<Children>());

// Read multi-link
ref var kids = ref parent.Ref<W.Links<Children>>();
for (int i = 0; i < kids.Length; i++) {
    if (kids[i].Value.TryUnpack<WT>(out var childEntity)) {
        // work with child entity
    }
}
```

___

## Extension methods

Safe link operations via `EntityGID` — automatically check whether the target entity is loaded and actual.

### Link (single link):
```csharp
// Add Link<T> component to target entity
LinkOppStatus status = targetGID.TryAddLink<WT, Parent>(linkEntity);

// Delete Link<T> component from target entity
LinkOppStatus status = targetGID.TryDeleteLink<WT, Parent>(linkEntity);

// Deep destroy — recursively destroys chain of linked entities
targetGID.DeepDestroyLink<WT, Parent>();

// Deep copy — clones the target entity and returns link to the copy
LinkOppStatus status = sourceGID.TryDeepCopyLink<WT, Parent>(out W.Link<Parent> copied);
```

### Links (multi-link):
```csharp
// Add item to Links<T> on target entity
// Automatically creates Links<T> component if not present
LinkOppStatus status = targetGID.TryAddLinkItem<WT, Children>(linkEntity);

// Remove item from Links<T> on target entity
// Automatically removes Links<T> component if collection becomes empty
LinkOppStatus status = targetGID.TryDeleteLinkItem<WT, Children>(linkEntity);

// Deep destroy — recursively destroys all linked entities
targetGID.DeepDestroyLinkItem<WT, Children>();
```

### LinkOppStatus:
```csharp
// Operation result
switch (status) {
    case LinkOppStatus.Ok:                // Operation completed successfully
    case LinkOppStatus.LinkAlreadyExists: // Link already exists (TryAdd)
    case LinkOppStatus.LinkNotExists:     // Link not found (TryDelete)
    case LinkOppStatus.LinkNotLoaded:     // Target entity in unloaded chunk
    case LinkOppStatus.LinkNotActual:     // GID is stale (entity destroyed, slot reused)
}
```

___

## Link examples

### Unidirectional link (no hooks)

The simplest case — an entity references another without a back-reference.

```csharp
// Type without hooks
public struct FollowTarget : ILinkType { }

// Registration
W.Types().Link<FollowTarget>();
```

```csharp
//  A FollowTarget→ B

var unit = W.NewEntity<Default>();
var target = W.NewEntity<Default>();

// Set pursuit target
unit.Set(new W.Link<FollowTarget>(target));

// In movement system
W.Query().For(static (W.Entity entity, ref W.Link<FollowTarget> follow) => {
    if (follow.Value.TryUnpack<WT>(out var targetEntity)) {
        ref var myPos = ref entity.Ref<Position>();
        ref readonly var targetPos = ref targetEntity.Read<Position>();
        // move towards target
    }
});
```

___

### Bidirectional one-to-one (same type)

A closed pair — both entities reference each other with the same type.

```csharp
//    MarriedTo
//  A ────────→ B
//  A ←──────── B
//    MarriedTo

public struct MarriedTo : ILinkType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        link.TryAddLink<TW, MarriedTo>(self);
    }

    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        link.TryDeleteLink<TW, MarriedTo>(self);
    }
}

W.Types().Link<MarriedTo>();
```

```csharp
var alice = W.NewEntity<Default>();
var bob = W.NewEntity<Default>();

// Set from one side — the back-reference is created automatically
alice.Set(new W.Link<MarriedTo>(bob));
// Now: alice has Link<MarriedTo> → bob
//      bob has Link<MarriedTo> → alice

// Deletion is also bidirectional
alice.Delete<W.Link<MarriedTo>>();
// Now: both components are removed
```

___

### Bidirectional one-to-one (different types)

Two entities linked with different link types.

```csharp
//  A ←Rider── Mount──→ B

public struct Mount : ILinkType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        link.TryAddLink<TW, Rider>(self);
    }

    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        link.TryDeleteLink<TW, Rider>(self);
    }
}

public struct Rider : ILinkType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        link.TryAddLink<TW, Mount>(self);
    }

    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        link.TryDeleteLink<TW, Mount>(self);
    }
}

W.Types()
    .Link<Mount>()
    .Link<Rider>();
```

```csharp
var player = W.NewEntity<Default>();
var horse = W.NewEntity<Default>();

player.Set(new W.Link<Mount>(horse));
// player has Link<Mount> → horse
// horse has Link<Rider> → player
```

___

### Bidirectional one-to-many (Parent ↔ Children)

Parent and children — classic hierarchy.

```csharp
//      ←Parent  Children→ child1
//     /
//  parent ←Parent  Children→ child2
//     \
//      ←Parent  Children→ child3

public struct Parent : ILinkType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        link.TryAddLinkItem<TW, Children>(self);
    }

    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        link.TryDeleteLinkItem<TW, Children>(self);
    }
}

public struct Children : ILinksType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        link.TryAddLink<TW, Parent>(self);
    }

    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        link.TryDeleteLink<TW, Parent>(self);
    }
}

W.Types()
    .Link<Parent>()
    .Links<Children>();
```

```csharp
var father = W.NewEntity<Default>();
var son = W.NewEntity<Default>();
var daughter = W.NewEntity<Default>();

// Set from child side
son.Set(new W.Link<Parent>(father));
daughter.Set(new W.Link<Parent>(father));
// father automatically gets Links<Children> → [son, daughter]

// Or add from parent side
ref var kids = ref father.Ref<W.Links<Children>>();
var newChild = W.NewEntity<Default>();
kids.TryAdd(newChild.AsLink<Children>());
// newChild automatically gets Link<Parent> → father
```

{: .note }
`withCyclicHooks: false` (the default) in extension methods `TryAddLink`/`TryDeleteLink`/`TryAddLinkItem`/`TryDeleteLinkItem` is an optimization: when called from a hook, there is no need to call the hook on the opposite side since it is already executing.

___

### Unidirectional to-many link

An entity references multiple others without back-references.

```csharp
//      Targets→ B
//     /
//  A── Targets→ C
//     \
//      Targets→ D

public struct Targets : ILinksType { }

W.Types().Links<Targets>();
```

```csharp
var turret = W.NewEntity<Default>();
var enemy1 = W.NewEntity<Default>();
var enemy2 = W.NewEntity<Default>();

ref var targets = ref turret.Add<W.Links<Targets>>();
targets.TryAdd(enemy1.AsLink<Targets>());
targets.TryAdd(enemy2.AsLink<Targets>());
```

___

### Bidirectional many-to-many

Both sides store collections of references to each other.

```csharp
//      ←Owners  Memberships→ groupA
//     /
//  user1 ←Owners  Memberships→ groupB
//
//  user2 ←Owners  Memberships→ groupA

public struct Memberships : ILinksType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        link.TryAddLinkItem<TW, Owners>(self);
    }

    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        link.TryDeleteLinkItem<TW, Owners>(self);
    }
}

public struct Owners : ILinksType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        link.TryAddLinkItem<TW, Memberships>(self);
    }

    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        link.TryDeleteLinkItem<TW, Memberships>(self);
    }
}

W.Types()
    .Links<Memberships>()
    .Links<Owners>();
```

```csharp
var user1 = W.NewEntity<Default>();
var user2 = W.NewEntity<Default>();
var groupA = W.NewEntity<Default>();
var groupB = W.NewEntity<Default>();

// Add user1 to both groups
ref var memberships = ref user1.Add<W.Links<Memberships>>();
memberships.TryAdd(groupA.AsLink<Memberships>());
memberships.TryAdd(groupB.AsLink<Memberships>());
// groupA and groupB automatically get Links<Owners> → [user1]

// Add user2 to groupA
ref var memberships2 = ref user2.Add<W.Links<Memberships>>();
memberships2.TryAdd(groupA.AsLink<Memberships>());
// groupA now has Links<Owners> → [user1, user2]
```

___

## Bulk segment serialization for Links

For chunk/world/cluster snapshots with unmanaged link types, `LinksUnmanagedPackArrayStrategy` is applied automatically — no manual configuration needed.

To provide custom configuration for links registration, implement `ILinksConfig<T>` on the link type:

```csharp
public struct MyLinkType : ILinksType, ILinksConfig<MyLinkType> {
    public ComponentTypeConfig<W.Links<MyLinkType>> Config<TWorld>() where TWorld : struct, IWorldType => new(
        guid: new Guid("...")
    );
}
```

This works identically to `MultiUnmanagedPackArrayStrategy` — see [multi-component bulk serialization](multicomponent.md#bulk-segment-serialization) for details.

___

## Multithreading

{: .warning }
In `ForParallel`, only the **current** iterated entity may be modified. Link hooks that change the state of **other** entities (e.g., adding a back-reference to a parent) will cause an error in DEBUG during parallel iteration.

To work with links in parallel queries, use **events** — `SendEvent` is thread-safe (when there is no concurrent reading of the same type, see [Events](events#multithreading) for details) and can be called from any thread. Process event logic on the main thread after the parallel iteration completes.

#### Example: deferred link deletion via events

```csharp
// 1. Define the event
public struct DeleteLinkEvent<TLink> : IEvent where TLink : unmanaged, ILinkType {
    public EntityGID Target;    // entity from which to remove the link
    public EntityGID Link;      // link value for verification
}

// 2. Register the event and create a receiver
W.Types().Event<DeleteLinkEvent<Parent>>();
var deleteLinkReceiver = W.RegisterEventReceiver<DeleteLinkEvent<Parent>>();

// Store the receiver in world resources for access from systems
W.SetResource(deleteLinkReceiver);
```

```csharp
// 3. Define link type WITHOUT hooks that modify other entities
public struct Parent : ILinkType {
    // In OnDelete, instead of directly modifying the parent — send an event
    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        World<TW>.SendEvent(new DeleteLinkEvent<Parent> {
            Target = link,
            Link = self.GID
        });
    }
}
```

```csharp
// 4. Parallel iteration — safe, hook sends event instead of direct modification
W.Query().ForParallel(
    static (W.Entity entity, ref W.Link<Parent> parent) => {
        if (someCondition) {
            entity.Delete<W.Link<Parent>>();
            // OnDelete will send DeleteLinkEvent instead of modifying the parent
        }
    },
    minEntitiesPerThread: 1000
);

// 5. On the main thread, process all events
ref var receiver = ref W.GetResource<EventReceiver<WT, DeleteLinkEvent<Parent>>>();
receiver.ReadAll(static (W.Event<DeleteLinkEvent<Parent>> e) => {
    // Now it's safe to modify other entities
    ref var data = ref e.Value;
    data.Target.TryDeleteLinkItem<WT, Children>(data.Link.Unpack<WT>());
});
```

___

## Queries

Link components are used in queries like any other components:

```csharp
// All entities with a parent
foreach (var entity in W.Query<All<W.Link<Parent>>>().Entities()) {
    ref var parentLink = ref entity.Ref<W.Link<Parent>>();
    // ...
}

// All entities with children but no parent (root entities)
W.Query<All<W.Links<Children>>, None<W.Link<Parent>>>()
    .For(static (W.Entity entity, ref W.Links<Children> kids) => {
        // root entities
    });

// Via delegate
W.Query().For(static (ref W.Link<Parent> parent) => {
    if (parent.Value.TryUnpack<WT>(out var parentEntity)) {
        // ...
    }
});
```
