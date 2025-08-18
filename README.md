![Version](https://img.shields.io/badge/version-1.0.25-blue.svg?style=for-the-badge)

### LANGUAGE
[RU](./README_RU.md)
[EN](./README.md)
___
### 🚀 **[Benchmarks](./docs/Benchmark.md)** 🚀
### ⚙️ **[Unity module](https://github.com/Felid-Force-Studios/StaticEcs-Unity)** ⚙️
### 📖️ **[Documentation](https://felid-force-studios.github.io/StaticEcs/en/)** 📖️

# Static ECS - C# Entity component system framework
- Lightweight
- Performance
- No allocations
- No Unsafe
- Based on statics and structures
- Type-safe
- Free abstractions
- Powerful query engine
- No boilerplate
- Compatible with Unity and other C# engines

#### Limitations and Features:
> - Not thread safe
> - There may be minor API changes

## Table of Contents
* [Contacts](#contacts)
* [Installation](#installation)
* [Concept](#concept)
* [Quick start](#quick-start)
* [License](#license)


# Contacts
* [felid.force.studios@gmail.com](mailto:felid.force.studios@gmail.com)
* [Telegram](https://t.me/felid_force_studios)

# Installation
  The library has a dependency on [StaticPack](https://github.com/Felid-Force-Studios/StaticPack) for binary serialization, StaticPack must also be installed
* ### As source code
  From the release page or as an archive from the branch. In the `master` branch there is a stable tested version
* ### Installation for Unity
  - How to git module in Unity PackageManager     
    `https://github.com/Felid-Force-Studios/StaticEcs.git`  
    `https://github.com/Felid-Force-Studios/StaticPack.git`
  - Or adding to the manifest `Packages/manifest.json`  
    `"com.felid-force-studios.static-ecs": "https://github.com/Felid-Force-Studios/StaticEcs.git"`  
    `"com.felid-force-studios.static-pack": "https://github.com/Felid-Force-Studios/StaticPack.git"`

# Concept
> - The main idea of this implementation is static, all data about the world and components are in static classes, which makes it possible to avoid expensive virtual calls and have a convenient API
> - This framework is focused on maximum ease of use, speed and comfort of code writing without loss of performance
> - Multi-world creation, strict typing, ~zero-cost abstractions
> - Serialization system
> - System of entity relations
> - Based on a sparse-set architecture, the core is inspired by a series of libraries from Leopotam
> - The framework was created for the needs of a private project and put out in open-source.

# Quick start
```csharp
using FFS.Libraries.StaticEcs;

// Define the world type
public struct WT : IWorldType { }

public abstract class W : World<WT> { }

// Define the systems type
public struct SystemsType : ISystemsType { }

// Define type-alias for easy access to systems
public abstract class Systems : W.Systems<SystemsType> { }

// Define components
public struct Position : IComponent { public Vector3 Value; }
public struct Direction : IComponent { public Vector3 Value; }
public struct Velocity : IComponent { public float Value; }

// Define systems
public readonly struct VelocitySystem : IUpdateSystem {
    public void Update() {
        foreach (var entity in W.QueryEntities.For<All<Position, Velocity>>()) {
            entity.Ref<Position>().Value *= entity.Ref<Velocity>().Value;
        }
        
        // Or
        W.QueryComponents.For((ref Position pos, ref Velocity vel, ref Direction dir) => {
            pos.Value += dir.Value * vel.Value;
        });
    }
}

public class Program {
    public static void Main() {
        // Creating world data
        W.Create(WorldConfig.Default());
        
        // Registering components
        W.RegisterComponentType<Position>();
        W.RegisterComponentType<Direction>();
        W.RegisterComponentType<Velocity>();
        
        // Initializing the world
        W.Initialize();
        
        // Creating systems
        Systems.Create();
        Systems.AddUpdate(new VelocitySystem());

        // Initializing systems
        Systems.Initialize();

        // Creating entity
        var entity = W.Entity.New(
            new Velocity { Value = 1f },
            new Position { Value = Vector3.Zero }
            new Direction { Value = Vector3.UnitX }
        );
        // Update all systems - called in every frame
        Systems.Update();
        // Destroying systems
        Systems.Destroy();
        // Destroying the world and deleting all data
        W.Destroy();
    }
}
```

# License
[MIT license](./LICENSE.md)
