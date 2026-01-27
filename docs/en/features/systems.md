---
title: Systems
parent: Features
nav_order: 10
---

## Systems
Systems manage world logic through a defined lifecycle
- Nested class `World<TWorld>.Systems<SysType>` — each `ISystemsType` type creates an isolated system group within a world
- Single `ISystem` interface with four methods (all optional)
- Systems execute in order defined by the `order` parameter
- Unimplemented methods are not called and create no overhead
- Systems can be structs or classes

___

## ISystemsType

Marker interface for isolating system groups. Each type gets its own static storage:

```csharp
public struct GameSystems : ISystemsType { }
public struct FixedSystems : ISystemsType { }
public struct LateSystems : ISystemsType { }

// Aliases for convenient access
public abstract class GameSys : W.Systems<GameSystems> { }
public abstract class FixedSys : W.Systems<FixedSystems> { }
public abstract class LateSys : W.Systems<LateSystems> { }
```

___

## ISystem

Single interface for all systems. Implement only the methods you need — the rest will not be called:

```csharp
public interface ISystem {
    // Called once during Systems.Initialize()
    void Init() { }

    // Called every frame during Systems.Update()
    void Update() { }

    // Called before each Update() — false skips the update
    bool UpdateIsActive() => true;

    // Called once during Systems.Destroy()
    void Destroy() { }
}
```

{: .important }
Do not leave empty method implementations. If a method is not needed — don't implement it. Unimplemented methods are detected via reflection and are not called.

#### System examples:
```csharp
// Update-only system
public struct MoveSystem : ISystem {
    public void Update() {
        W.Query().For(static (ref Position pos, in Velocity vel) => {
            pos.Value += vel.Value;
        });
    }
}

// System with init and destroy
public struct AudioSystem : ISystem {
    public void Init() {
        // load audio resources
    }

    public void Update() {
        // process sounds
    }

    public void Destroy() {
        // release resources
    }
}

// System with conditional execution
public struct PausableSystem : ISystem {
    public void Update() {
        // game logic
    }

    public bool UpdateIsActive() {
        return !W.GetResource<GameState>().IsPaused;
    }
}
```

___

## Lifecycle

```
Create() → Add() → Initialize() → Update() loop → Destroy()
```

```csharp
// 1. Create system group (baseSize — initial array capacity)
GameSys.Create(baseSize: 64);

// 2. Register systems (order determines execution order)
GameSys.Add(new InputSystem(), order: -10)
    .Add(new MoveSystem(), order: 0)
    .Add(new RenderSystem(), order: 10);

// 3. Initialize — sorts by order, calls Init() on all systems
GameSys.Initialize();

// 4. Game loop — calls Update() every frame
while (gameIsRunning) {
    GameSys.Update();
}

// 5. Destroy — calls Destroy() on all systems, resets state
GameSys.Destroy();
```

___

## Registration

All systems are registered with a single `Add<T>()` method:

```csharp
// Basic registration (order defaults to 0)
GameSys.Add(new MoveSystem());

// With order (lower = earlier)
GameSys.Add(new InputSystem(), order: -10)    // executes first
    .Add(new PhysicsSystem(), order: 0)       // then physics
    .Add(new RenderSystem(), order: 10);      // render last

// Systems with the same order execute in registration order
GameSys.Add(new SystemA(), order: 0)  // first among order=0
    .Add(new SystemB(), order: 0);    // second among order=0
```

___

## Conditional execution

The `UpdateIsActive()` method allows skipping a system's update on the current frame:

```csharp
public struct GameplaySystem : ISystem {
    public void Update() {
        // logic that only runs when the game is not paused
    }

    public bool UpdateIsActive() {
        return !W.GetResource<GameState>().IsPaused;
    }
}

public struct TutorialSystem : ISystem {
    public void Update() {
        // tutorial logic
    }

    public bool UpdateIsActive() {
        return W.GetResource<PlayerProgress>().IsFirstPlay;
    }
}
```

___

## Multiple system groups

Different `ISystemsType` types create independent groups with their own lifecycle:

```csharp
public struct GameSystems : ISystemsType { }
public struct FixedSystems : ISystemsType { }
public abstract class GameSys : W.Systems<GameSystems> { }
public abstract class FixedSys : W.Systems<FixedSystems> { }

// Setup
GameSys.Create();
GameSys.Add(new InputSystem())
    .Add(new RenderSystem());
GameSys.Initialize();

FixedSys.Create();
FixedSys.Add(new PhysicsSystem())
    .Add(new CollisionSystem());
FixedSys.Initialize();

// Game loop
while (gameIsRunning) {
    GameSys.Update();           // every frame

    while (fixedTimeAccumulated) {
        FixedSys.Update();      // fixed timestep
    }
}

GameSys.Destroy();
FixedSys.Destroy();
```

___

## Full example

```csharp
// System types
public struct GameSystems : ISystemsType { }

// Systems
public struct InputSystem : ISystem {
    public void Update() {
        // read input
    }
}

public struct MoveSystem : ISystem {
    public void Update() {
        W.Query().For(static (ref Position pos, in Velocity vel) => {
            pos.Value += vel.Value;
        });
    }
}

public struct DamageSystem : ISystem {
    private EventReceiver<WT, OnDamage> _receiver;

    public void Init() {
        _receiver = W.RegisterEventReceiver<OnDamage>();
    }

    public void Update() {
        foreach (var e in _receiver) {
            if (e.Value.Target.TryUnpack<WT>(out var target)) {
                ref var health = ref target.Ref<Health>();
                health.Current -= e.Value.Amount;
            }
        }
    }

    public void Destroy() {
        W.DeleteEventReceiver(ref _receiver);
    }
}

// Startup
W.Create(WorldConfig.Default());
// ... register types ...
W.Initialize();

GameSys.Create();
GameSys.Add(new InputSystem(), order: -10)
    .Add(new MoveSystem(), order: 0)
    .Add(new DamageSystem(), order: 5);
GameSys.Initialize();

while (gameIsRunning) {
    GameSys.Update();
}

GameSys.Destroy();
W.Destroy();
```
