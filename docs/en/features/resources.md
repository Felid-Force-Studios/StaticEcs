---
title: Resources
parent: Features
nav_order: 11
---

## Resources
Resources are an alternative to DI — a simple mechanism for storing and passing user data and services to systems and other methods
- Resources are world-level singletons: shared state that doesn't belong to any specific entity
- Ideal for configuration, time/delta-time, input state, asset caches, service references
- Two variants: **singleton** (one per type) and **named** (multiple per type, distinguished by string key)
- Available in both `Created` and `Initialized` world phases

___

## Singleton Resources

A singleton resource stores exactly one instance of a given type per world.
Internally uses static generic storage — access is O(1) with zero dictionary overhead.

#### Setting a resource:
```csharp
// User classes and services
public class GameConfig { public float Gravity; }
public class InputState { public Vector2 MousePos; }

// Set a resource in the world
// By default clearOnDestroy = true — the resource will be automatically cleared on World.Destroy()
W.SetResource(new GameConfig { Gravity = 9.81f });
W.SetResource(new InputState(), clearOnDestroy: false); // persists across world re-creation

// If SetResource is called again for the same type, the value is replaced without error
W.SetResource(new GameConfig { Gravity = 4.0f }); // overwrites the previous value
```

{: .important }
The `clearOnDestroy` parameter is only applied on the first registration. Replacing an existing resource preserves the original `clearOnDestroy` setting.

#### Basic operations:
```csharp
// Check if a resource of the given type is registered
bool has = W.HasResource<GameConfig>();

// Get a mutable ref to the resource value — modifications are written directly to storage
ref var config = ref W.GetResource<GameConfig>();
config.Gravity = 11.0f; // modified in-place, no setter call needed

// Remove the resource from the world
W.RemoveResource<GameConfig>();

// Resource<T> — zero-cost readonly struct handle for frequent access (no initialization needed)
W.Resource<GameConfig> configHandle;
bool registered = configHandle.IsRegistered;
ref var cfg = ref configHandle.Value;
```

___

## Named Resources

Named resources allow multiple instances of the same type, distinguished by string keys.
Internally stored in a `Dictionary<string, object>` with type-safe `Box<T>` wrappers.

#### Setting a named resource:
```csharp
// Set named resources of the same type under different keys
W.SetResource("player_config", new GameConfig { Gravity = 9.81f });
W.SetResource("moon_config", new GameConfig { Gravity = 1.62f });

// If SetResource is called again for an existing key, the value is replaced without error
W.SetResource("player_config", new GameConfig { Gravity = 10.0f }); // overwrites
```

#### Basic operations:
```csharp
// Check if a named resource with the given key exists
bool has = W.HasResource<GameConfig>("player_config");

// Get a mutable ref to the named resource value
ref var config = ref W.GetResource<GameConfig>("player_config");
config.Gravity = 5.0f;

// Remove a named resource by key
W.RemoveResource("player_config");

// NamedResource<T> — struct handle that caches the internal reference after the first access
// Create a handle bound to a key (does not register the resource)
var moonConfig = new W.NamedResource<GameConfig>("moon_config");
bool registered = moonConfig.IsRegistered;  // always performs dictionary lookup, not cached
ref var cfg = ref moonConfig.Value;          // first call resolves from dictionary and caches; subsequent calls are O(1)
// The cache is automatically invalidated when the resource is removed or the world is destroyed
```

{: .warning }
`NamedResource<T>` is a mutable struct that caches an internal reference on first `Value` access.
Do **not** store it in a `readonly` field or pass by value after first use — the C# compiler
will create a defensive copy, discarding the cache and causing a dictionary lookup on every access.
Store it in a non-readonly field or local variable.

___

## Lifecycle

```csharp
W.Create(WorldConfig.Default());

// Resources can be set after Create (no need to wait for Initialize)
W.SetResource(new GameConfig { Gravity = 9.81f });
W.SetResource("debug_flags", new DebugFlags(), clearOnDestroy: false);

W.Initialize();

// Resources remain available during the Initialized phase
ref var config = ref W.GetResource<GameConfig>();

// On Destroy: resources with clearOnDestroy=true are cleared automatically
// Resources with clearOnDestroy=false persist and remain available after the next Create+Initialize cycle
W.Destroy();
```

