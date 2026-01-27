---
title: Component
parent: Features
nav_order: 3
---

## Component
Component gives an entity data and properties
- Represented as a user struct with the `IComponent` marker interface
- Implemented as struct purely for performance reasons (SoA storage)
- Supports lifecycle hooks: `OnAdd`, `OnDelete`, `CopyTo`, `Write`, `Read`
- Can be enabled/disabled without removing data

#### Example:
```csharp
public struct Position : IComponent {
    public Vector3 Value;
}

public struct Velocity : IComponent {
    public float Value;
}

public struct Name : IComponent {
    public string Val;
}
```

___

{: .important }
Requires registration in the world between creation and initialization

```csharp
W.Create(WorldConfig.Default());
//...
// Simple registration without configuration (suitable for most cases)
W.Types()
    .Component<Position>()
    .Component<Velocity>()
    .Component<Name>();

// Registration with configuration
W.Types().Component<Scale>(new ComponentTypeConfig<Scale>(
    guid: new Guid("..."),                           // stable identifier for serialization
    version: 1,                                      // data schema version for migration (default — 0)
    noDataLifecycle: false,                          // disable framework data management (default — false)
    readWriteStrategy: null,                         // binary serialization strategy (default — StructPackArrayStrategy<T>)
    defaultValue: new Scale { Value = Vector3.One }, // default value for init and deletion (default — default(T))
    trackAdded: true,                                // enable addition tracking (default — false), see Change Tracking
    trackDeleted: true                               // enable deletion tracking (default — false), see Change Tracking
));
//...
W.Initialize();
```

{: .note }
The `noDataLifecycle` parameter controls whether the framework manages component data lifecycle. By default (`noDataLifecycle: false`), the framework pre-initializes new storage with `defaultValue` and resets data to `defaultValue` on deletion — so `entity.Add<T>()` returns the configured default. With `noDataLifecycle: true`, no initialization or cleanup is performed — useful for high-frequency unmanaged types. If `OnDelete` is defined, the hook handles cleanup regardless of this flag.

{: .note }
Instead of passing configuration manually, you can declare a static field or property of type `ComponentTypeConfig<T>` directly inside the component struct. `RegisterAll()` will automatically discover it by type (preferring the name `Config`):

```csharp
public struct Health : IComponent {
    public float Value;
    public static readonly ComponentTypeConfig<Health> Config = new(
        defaultValue: new Health { Value = 100f }
    );
}
// RegisterAll() will use Health.Config automatically
```

___

#### Creating entities with components:
```csharp
// Create an empty entity (no components or tags)
W.Entity entity = W.NewEntity<Default>();

// Create an entity with a specific type and cluster
W.Entity entity = W.NewEntity<Default>(clusterId: 0);

// Create an entity with components — Set returns Entity (overloads from 1 to 8 components)
W.Entity entity = W.NewEntity<Default>().Set(new Position { Value = Vector3.One });
W.Entity entity = W.NewEntity<Default>().Set(
    new Position { Value = Vector3.One },
    new Velocity { Value = 1f }
);
```

___

#### Adding components:
```csharp
// Add without value: if component exists → returns ref to existing, hooks are NOT called
// If new → initializes with default, calls OnAdd
ref var position = ref entity.Add<Position>();

// With isNew flag: isNew=true if component was added for the first time
ref var position = ref entity.Add<Position>(out bool isNew);

// Add multiple components in a single call (overloads from 2 to 5)
entity.Add<Position, Velocity>();

// Set with value: ALWAYS overwrites data
// If component exists → OnDelete(old) → replace → OnAdd(new)
// If new → set value → OnAdd
ref var position = ref entity.Set(new Position { Value = Vector3.One });

// Set multiple components with values (overloads from 2 to 12)
entity.Set(new Position { Value = Vector3.One }, new Velocity { Value = 1f });
```

{: .important }
`Add<T>()` without a value and `Set<T>(T value)` with a value have different hook semantics.
Without value: if the component already exists, hooks are **not called**, a ref to current data is returned.
With value: data is **always overwritten** with the full cycle `OnDelete` → replace → `OnAdd`.

___

#### Data access:
```csharp
// Get a mutable ref to the component (read and write)
// Does NOT mark as Changed — use Mut<T>() for tracked access
ref var velocity = ref entity.Ref<Velocity>();
velocity.Value += 10f;

// Get a readonly ref to the component — does NOT mark as Changed
ref readonly var pos = ref entity.Read<Position>();
var x = pos.Value.x; // reading OK, no Changed mark

// Get a tracked mutable ref — marks as Changed if trackChanged is enabled
ref var pos = ref entity.Mut<Position>();
pos.Value += delta; // data modified AND marked as Changed
```

{: .important }
`Ref<T>()` does NOT mark Changed. Use `Mut<T>()` when you need change tracking to work with `AllChanged<T>` filters. In query delegates (`For`), `ref` parameters use `Mut` semantics automatically.

___

#### Basic operations:
```csharp
// Get the number of components on an entity
int count = entity.ComponentsCount();

// Check if a component is present (overloads from 1 to 3 — checks ALL specified)
// Checks presence regardless of enabled/disabled state
bool has = entity.Has<Position>();
bool hasBoth = entity.Has<Position, Velocity>();
bool hasAll = entity.Has<Position, Velocity, Name>();

// Check if at least one of the specified components is present (overloads from 2 to 3)
bool hasAny = entity.HasAny<Position, Velocity>();
bool hasAny3 = entity.HasAny<Position, Velocity, Name>();

// Remove a component (overloads from 1 to 5)
// Calls OnDelete if the component was present; returns true if removed, false if not present
bool deleted = entity.Delete<Position>();
entity.Delete<Position, Velocity>();
entity.Delete<Position, Velocity, Name>();
```

___

#### Enable/Disable:
```csharp
// Disable a component — data is preserved, but entity is excluded from standard queries
// Returns ToggleResult: MissingComponent, Unchanged, or Changed
ToggleResult disabled = entity.Disable<Position>();
entity.Disable<Position, Velocity>();
entity.Disable<Position, Velocity, Name>();

// Re-enable a component
// Returns ToggleResult: MissingComponent, Unchanged, or Changed
ToggleResult enabled = entity.Enable<Position>();
entity.Enable<Position, Velocity>();
entity.Enable<Position, Velocity, Name>();

// Check that ALL specified components are enabled (overloads from 1 to 3)
bool posEnabled = entity.HasEnabled<Position>();
bool bothEnabled = entity.HasEnabled<Position, Velocity>();

// Check that at least one is enabled (overloads from 2 to 3)
bool anyEnabled = entity.HasEnabledAny<Position, Velocity>();

// Check that ALL specified components are disabled (overloads from 1 to 3)
bool posDisabled = entity.HasDisabled<Position>();
bool bothDisabled = entity.HasDisabled<Position, Velocity>();

// Check that at least one is disabled (overloads from 2 to 3)
bool anyDisabled = entity.HasDisabledAny<Position, Velocity>();
```

{: .note }
Disabled components are excluded from standard query filters (`All`, `None`, `Any`), but their data remains in memory. Use `WithDisabled`/`OnlyDisabled` filter variants to work with disabled components.

___

#### Copying and moving:
```csharp
var source = W.NewEntity<Default>().Set(new Position(), new Velocity());
var target = W.NewEntity<Default>();

// Copy specified components to another entity (overloads from 1 to 5)
// The source entity keeps its components
// If CopyTo hook is overridden — custom copy logic is used
// If CopyTo hook is NOT overridden — bitwise copy via Add + disabled state is preserved
// Returns true (for single) if the source had the component
bool copied = source.CopyTo<Position>(target);
source.CopyTo<Position, Velocity>(target);

// Move specified components to another entity (overloads from 1 to 5)
// Performs Copy to target, then Delete from source (OnDelete is called on source)
bool moved = source.MoveTo<Position>(target);
source.MoveTo<Position, Velocity>(target);
```

___

#### Query filters:

Component filters are described in the [Queries — Components](query.md#components) section.

___

#### Lifecycle hooks:

The `IComponent` interface provides hooks with empty default implementations — override only the ones you need.

{: .important }
Do not leave empty hook implementations. If a hook is not needed — don't implement it. Unimplemented hooks are not called and create no overhead.

```csharp
public struct Cooldown : IComponent {
    public float Duration;
    public float Elapsed;

    // Called after the component is added or the value is overwritten via Set(value)
    public void OnAdd<TWorld>(World<TWorld>.Entity self) where TWorld : struct, IWorldType {
        Elapsed = 0f; // reset timer on every apply
    }

    // Called before the component is removed (Delete), before overwrite (Set with value),
    // and during entity destruction for each component
    //
    // The `reason` parameter indicates why deletion occurs:
    // HookReason.Default      — explicit removal or entity destruction
    // HookReason.UnloadEntity — entity/chunk unloading
    // HookReason.WorldDestroy — world reset/destruction
    public void OnDelete<TWorld>(World<TWorld>.Entity self, HookReason reason) where TWorld : struct, IWorldType { }

    // Custom copy logic for CopyTo / MoveTo / Clone
    // If NOT overridden — bitwise copy + disabled state preservation
    // If overridden — completely replaces the default copy logic
    public void CopyTo<TWorld>(World<TWorld>.Entity self, World<TWorld>.Entity other, bool disabled)
        where TWorld : struct, IWorldType {
        ref var copy = ref other.Add<Cooldown>();
        copy.Duration = Duration;
        copy.Elapsed = 0f; // clone starts from zero
    }

    // Serialization — write the component to a binary stream
    // Required for EntitiesSnapshot (all types), and for non-unmanaged types in any snapshot
    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType {
        writer.WriteFloat(Duration);
        writer.WriteFloat(Elapsed);
    }

    // Deserialization — read the component from a binary stream
    // The version parameter enables data migration between schema versions
    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled)
        where TWorld : struct, IWorldType {
        Duration = reader.ReadFloat();
        Elapsed = reader.ReadFloat();
    }
}
```

{: .important }
Hook call order for `Set(value)` on an existing component: `OnDelete`(old value) → data replacement → `OnAdd`(new value). For `Delete` or entity destruction, only `OnDelete` is called.

___

#### Debugging:
```csharp
// Collect all components of an entity into a list (for inspector/debugging)
// The list is cleared before populating
var components = new List<IComponent>();
entity.GetAllComponents(components);
```
