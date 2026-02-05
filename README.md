![Version](https://img.shields.io/badge/version-1.2.14-blue.svg?style=for-the-badge)

### LANGUAGE
[RU](./README_RU.md)
[EN](./README.md)
___
### 🚀 **[Benchmarks](https://gist.github.com/blackbone/6d254a684cf580441bf58690ad9485c3)** 🚀
### ⚙️ **[Unity module](https://github.com/Felid-Force-Studios/StaticEcs-Unity)** ⚙️
### 📖️ **[Documentation](https://felid-force-studios.github.io/StaticEcs/en/)** 📖️
### 🗺️ **[Roadmap](#roadmap)** 🗺️
___

### ❗️ **[Guide for migrating from version 1.0.x to 1.2.x](https://felid-force-studios.github.io/StaticEcs/en/migrationguide.html)** ❗️

# Static ECS - C# Hierarchical Inverted Bitmap ECS framework
- Lightweight
- Performance
- No allocations
- No Unsafe
- Based on statics and structures
- Type-safe
- Free abstractions
- Powerful query engine
- No boilerplate
- Compatibility with Unity with support for Il2Cpp and [Burst](https://github.com/Felid-Force-Studios/StaticEcs-Unity?tab=readme-ov-file#templates)
- Compatibility with other C# engines
- Compatible with Native AOT

## Table of Contents
* [Contacts](#contacts)
* [Installation](#installation)
* [Concept](#concept)
* [Quick start](#quick-start)
* [Features](https://felid-force-studios.github.io/StaticEcs/en/features.html)
  * [Entity](https://felid-force-studios.github.io/StaticEcs/en/features/entity.html)
  * [Entity global ID](https://felid-force-studios.github.io/StaticEcs/en/features/gid.html)
  * [Component](https://felid-force-studios.github.io/StaticEcs/en/features/component.html)
  * [Tag](https://felid-force-studios.github.io/StaticEcs/en/features/tag.html)
  * [MultiComponent](https://felid-force-studios.github.io/StaticEcs/en/features/multicomponent.html)
  * [Relations](https://felid-force-studios.github.io/StaticEcs/en/features/relations.html)
  * [World](https://felid-force-studios.github.io/StaticEcs/en/features/world.html)
  * [Systems](https://felid-force-studios.github.io/StaticEcs/en/features/systems.html)
  * [Context](https://felid-force-studios.github.io/StaticEcs/en/features/context.html)
  * [Query](https://felid-force-studios.github.io/StaticEcs/en/features/query.html)
  * [Events](https://felid-force-studios.github.io/StaticEcs/en/features/events.html)
  * [Component configurators](https://felid-force-studios.github.io/StaticEcs/en/features/configs.html)
  * [Serialization](https://felid-force-studios.github.io/StaticEcs/en/features/serialization.html)
  * [Compiler directives](https://felid-force-studios.github.io/StaticEcs/en/features/compilerdirectives.html)
* [Performance](https://felid-force-studios.github.io/StaticEcs/en/performance.html)
* [Unity integration](https://felid-force-studios.github.io/StaticEcs/en/unityintegrations.html)
* [FAQ](https://felid-force-studios.github.io/StaticEcs/en/faq.html)
* [License](#license)


# Contacts
* [felid.force.studios@gmail.com](mailto:felid.force.studios@gmail.com)
* [Telegram](https://t.me/felid_force_studios)

# Installation
The library has a dependency on [StaticPack](https://github.com/Felid-Force-Studios/StaticPack) `1.0.6` for binary serialization, StaticPack must also be installed
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
StaticEcs - a new ECS architecture based on an inverted hierarchical bitmap model.
Unlike traditional ECS frameworks that rely on archetypes or sparse sets, this design introduces an inverted index structure where each component owns an entity bitmap instead of entities storing component masks.
A hierarchical aggregation of these bitmaps provides logarithmic-space indexing of entity blocks, enabling O(1) block filtering and efficient parallel iteration through bitwise operations.
This approach completely removes archetype migration and sparse-set indirection, offering direct SoA-style memory access across millions of entities with minimal cache misses.
The model achieves up to 64× fewer memory lookups per block and scales linearly with the number of active component sets, making it ideal for large-scale simulations, reactive AI, and open-world environments.


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
        foreach (var entity in W.Query.Entities<All<Position, Velocity, Direction>>()) {
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

# Roadmap 🗺️

It’s been a while since the last update. About **3 months have passed** since the last static update, and since no major bugs have been reported, it’s time to break something! 😄

This roadmap outlines **planned changes** with some significant and radical improvements.

---

### 1. Bitmask Optimization 🧮

The issue with bitmasks (used to store component bits for each entity, primarily for searching all components/tags during copy and delete operations) will be resolved.

**Expected improvements:**

* Memory usage for bitmasks will be reduced by **256x**.
* Component add/remove operations will be **15-20% faster**.
* World serialization will become faster.
* Snapshots will be lighter and **no longer store this bitmask**.

---

### 2. Removing `Put` Method ❌

* The `Put` method on entities will be removed.
* Functionality can be replaced with the extension method `TryAdd`.

---

### 3. Renaming Component Check Methods ✨

* `HasAllOf<>` methods will be renamed to `Has<>` to reduce verbosity and improve readability.

---

### 4. `IComponent` Interface Updates 🛠️

Creating an `IComponent` will now **has empty implementation** of the following methods:

```csharp
OnAdd
OnDelete
CopyTo
Write
Read
```

* Overriding is **optional**, methods may be empty.
* IDEs and snippets will help scaffold these methods.
* This removes callbacks/delegates, allows inlining, and **speeds up add/remove operations**.

---

### 5. Component Registration Changes 🔄

* Multi and relation component registration methods will be removed.
* Automated initialization/deletion must now be explicitly implemented in `OnAdd/OnDelete` using extension helpers.

---

### 6. Optional Component/Tag Auto-Registration ⚡

* Full **optional auto-registration** for components and tags will be introduced.
* Enabled by changes in bitmask storage.
* Will **not affect entity operation speed**.

---

### 7. Entity Creation Changes 🆕

* `W.Entity.New()` will now **require explicit cluster specification** (previously default cluster `0` was used).
* A new method will allow creation in the default cluster, but with a **long and ugly name** to discourage casual use. 😅

---

### 8. Component Change Events 📣

* Ability to receive **events** (not delegates) on component changes:

```csharp
Changed<Position>
Added<Position>
Deleted<Position>
```

* Currently **Work In Progress**.

---

### 9. Iteration Algorithm Improvements ⚡

* Iteration algorithm has been reworked.
* Performance improvements expected.
* **Block SIMD `QueryFunction`** support will be added.

---

### 10. API for Unmanaged Components 🔧

* All API for unmanaged components will be available in **Native/Burst**:

    * `Query`
    * `entity.Add<>(), Ref<>(), Delete<>()`, etc.
* Components containing reference types **will not have this API**.

---

### Release Date 📅

* Planned for **end of February**.
* No major updates are planned after this release.

---

### Feedback & Suggestions 💡

If you have **feature requests, suggestions, or feedback**, please open a GitHub issue! Your input is highly appreciated and will help shape this release. 🙂


# License
[MIT license](https://github.com/Felid-Force-Studios/StaticEcs/blob/master/LICENSE.md)
