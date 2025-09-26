---
title: EN
has_toc: false
parent: Main page
---

![Version](https://img.shields.io/badge/version-1.1.preview-blue.svg?style=for-the-badge)

___

### 🚀 **[Benchmarks](https://gist.github.com/blackbone/6d254a684cf580441bf58690ad9485c3)** 🚀
### ⚙️ **[Unity module](https://github.com/Felid-Force-Studios/StaticEcs-Unity)** ⚙️

### ❗️ **[Guide for migrating from version 1.0.x to 1.1.x](migrationguide.md)** ❗️

# Static ECS - C# Binary Entity component system framework
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
- Compatible with Native AOT

## Table of Contents
* [Contacts](#contacts)
* [Installation](#installation)
* [Concept](#concept)
* [Quick start](#quick-start)
* [Features](features.md)
    * [Entity](features/entity.md)
    * [Entity global ID](features/gid.md)
    * [Component](features/component.md)
    * [Tag](features/tag.md)
    * [MultiComponent](features/multicomponent.md)
    * [Relations](features/relations.md)
    * [World](features/world.md)
    * [Systems](features/systems.md)
    * [Context](features/context.md)
    * [Query](features/query.md)
    * [Events](features/events.md)
    * [Component configurators](features/configs.md)
    * [Serialization](features/serialization.md)
    * [Compiler directives](features/compilerdirectives.md)
* [Performance](performance.md)
* [Unity integration](unityintegrations.md)
* [FAQ](faq.md)
* [License](#license)


# Contacts
* [felid.force.studios@gmail.com](mailto:felid.force.studios@gmail.com)
* [Telegram](https://t.me/felid_force_studios)

# Installation
The library has a dependency on [StaticPack](https://github.com/Felid-Force-Studios/StaticPack) `1.0.3` for binary serialization, StaticPack must also be installed
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
> - Multithreaded processing
> - Low memory usage
> - Based on Bitmap architecture, no archetypes, no sparse-sets
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
        foreach (var entity in W.QueryEntities.For<All<Position, Velocity, Direction>>()) {
            entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
        }
        
        // Or
        W.Query.For((ref Position pos, ref Velocity vel, ref Direction dir) => {
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
            new Position { Value = Vector3.Zero },
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
[MIT license](https://github.com/Felid-Force-Studios/StaticEcs/blob/master/LICENSE.md)
