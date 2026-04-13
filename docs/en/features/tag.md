---
title: Tag
parent: Features
nav_order: 6
---

## Tag
Tag is similar to a component, but carries no data — it serves as a boolean flag on an entity
- Internally unified with components — stored in `Components<T>` with the `IsTag` flag, sharing the same storage infrastructure
- Stored purely as a bitmask — no data arrays, minimal memory footprint
- Does not slow down component searches and allows creating many tags
- No hooks (`OnAdd`/`OnDelete`) and no enable/disable — a tag is either present or absent
- Ideal for state markers (`IsPlayer`, `IsDead`, `NeedsUpdate`), query filtering, and any boolean property
- Represented as an empty user struct with the `ITag` marker interface
- Uses the same query filters as components (`All<>`, `None<>`, `Any<>`) — no separate tag filter types

#### Example:
```csharp
public struct Unit : ITag { }
public struct Player : ITag { }
public struct IsDead : ITag { }
```

___

{: .important }
Requires registration in the world between creation and initialization

```csharp
W.Create(WorldConfig.Default());
//...
W.Types()
    .Tag<Unit>()
    .Tag<Player>()
    .Tag<IsDead>();
//...
W.Initialize();
```

{: .note }
Tags automatically get a stable GUID computed from the type name. To provide configuration (GUID override, change tracking), implement the `ITagConfig<T>` interface on the tag struct. Both manual registration and `RegisterAll()` will use it automatically:

```csharp
public struct Poisoned : ITag, ITagConfig<Poisoned> {
    public TagTypeConfig<Poisoned> Config() => new(
        guid: new Guid("A1B2C3D4-..."),
        trackAdded: true,    // enable addition tracking (default — false)
        trackDeleted: true   // enable deletion tracking (default — false)
    );
}
```

___

#### Setting tags:
```csharp
// Add a tag to an entity (overloads from 1 to 5 tags)
// Returns true if the tag was absent and was added, false if already present
bool added = entity.Set<Unit>();

// Add multiple tags in a single call
entity.Set<Unit, Player>();
entity.Set<Unit, Player, IsDead>();
// Overloads for 4 and 5 tags are also available
```

___

#### Basic operations:
```csharp
// Get the number of tags on an entity
int tagsCount = entity.TagsCount();

// Check if a tag is present (overloads from 1 to 3 tags — checks ALL specified)
bool hasUnit = entity.Has<Unit>();
bool hasBoth = entity.Has<Unit, Player>();
bool hasAll3 = entity.Has<Unit, Player, IsDead>();

// Check if at least one of the specified tags is present (overloads from 2 to 3 tags)
bool hasAny = entity.HasAny<Unit, Player>();
bool hasAny3 = entity.HasAny<Unit, Player, IsDead>();

// Remove a tag from an entity (overloads from 1 to 5 tags)
// Returns true if the tag was present and removed, false if it wasn't there
// Safe to use even if the tag doesn't exist
bool deleted = entity.Delete<Unit>();
entity.Delete<Unit, Player>();

// Toggle a tag: adds if absent, removes if present (overloads from 1 to 3 tags)
// Returns true if the tag was added, false if it was removed
bool state = entity.Toggle<Unit>();
entity.Toggle<Unit, Player>();

// Conditionally set or remove a tag based on a boolean value (overloads from 1 to 3 tags)
// true — tag is set, false — tag is removed
entity.Apply<Unit>(true);
entity.Apply<Unit, Player>(false, true); // Unit is removed, Player is set
```

___

#### Copying and moving:
```csharp
var source = W.Entity.New<Position>();
source.Set<Unit, Player>();

var target = W.Entity.New<Position>();

// Copy specified tags to another entity (overloads from 1 to 5 tags)
// The source entity keeps its tags
// Returns true (for single tag) if the source had the tag and it was copied
bool copied = source.CopyTo<Unit>(target);
source.CopyTo<Unit, Player>(target);

// Move specified tags to another entity (overloads from 1 to 5 tags)
// The tag is added on the target and removed from the source
// Returns true (for single tag) if the tag was moved
bool moved = source.MoveTo<Unit>(target);
source.MoveTo<Unit, Player>(target);
```

___

#### Query filters:

Tags use the same query filters as components: `All<>`, `None<>`, `Any<>` and their variants. See the [Queries](query.md) section for details.

___

#### Debugging:
```csharp
// Collect all tags of an entity into a list (for inspector/debugging)
// The list is cleared before populating
var tags = new List<ITag>();
entity.GetAllTags(tags);
```
