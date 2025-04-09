![Version](https://img.shields.io/badge/version-0.9.73-blue.svg?style=for-the-badge)

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
- No dependencies
- No Unsafe in core
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
* [Telegram](https://t.me/felid_force_studios)

# Installation
* ### As source code
  From the release page or as an archive from the branch. In the `master` branch there is a stable tested version
* ### Installation for Unity
  git module `https://github.com/Felid-Force-Studios/StaticEcs.git` in Unity PackageManager or adding it to `Packages/manifest.json`

# Concept
> - The main idea of this implementation is static, all data about the world and components are in static classes, which makes it possible to avoid expensive virtual calls and have a convenient API
> - This framework is focused on maximum ease of use, speed and comfort of code writing without loss of performance
> - Multi-world creation, strict typing, ~zero-cost abstractions
> - Based on a sparse-set architecture, the core is inspired by a series of libraries from Leopotam
> - The framework was created for the needs of a private project and put out in open-source.

# Quick start
```csharp
using FFS.Libraries.StaticEcs;
// Define components
public struct Position : IComponent { public Vector3 Val; }
public struct Velocity : IComponent { public float Val; }

// Define the world type
public struct MyWorldType : IWorldType { }

// Define type-aliases for easy access to library types
public abstract class MyEcs : Ecs<MyWorldType> { }
public abstract class MyWorld : MyEcs.World { }

// Define the systems type
public struct MySystemsType : ISystemsType { }

// Define type-alias for easy access to systems
public abstract class MySystems : MyEcs.Systems<MySystemsType> { }

// Define systems
public readonly struct VelocitySystem : IUpdateSystem {
    public void Update() {
        foreach (var entity in MyWorld.QueryEntities.For<All<Position, Velocity>>()) {
            entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
        }
    }
}

public class Program {
    public static void Main() {
        // Creating world data
        MyEcs.Create(EcsConfig.Default());
        
        // Registering components
        MyWorld.RegisterComponentType<Position>();
        MyWorld.RegisterComponentType<Velocity>();
        
        // Initializing the world
        MyEcs.Initialize();
        
        // Creating systems
        MySystems.Create();
        MySystems.AddUpdate(new VelocitySystem());

        // Initializing systems
        MySystems.Initialize();

        // Creating entity
        var entity = MyEcs.Entity.New(
            new Velocity { Val = 1f },
            new Position { Val = Vector3.One }
        );
        // Update all systems - called in every frame
        MySystems.Update();
        // Destroying systems
        MySystems.Destroy();
        // Destroying the world and deleting all data
        MyEcs.Destroy();
    }
}
```

# License
[MIT license](./LICENSE.md)
