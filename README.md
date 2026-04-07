<p align="center">
  <img src="docs/fulllogo.png" alt="Static ECS" width="100%">
  <br><br>
  <a href="./README.md"><img src="https://img.shields.io/badge/EN-English-blue?style=flat-square" alt="English"></a>
  <a href="./README_RU.md"><img src="https://img.shields.io/badge/RU-Русский-blue?style=flat-square" alt="Русский"></a>
  <a href="./README_ZH.md"><img src="https://img.shields.io/badge/ZH-中文-blue?style=flat-square" alt="中文"></a>
  <br><br>
  <img src="https://img.shields.io/badge/version-2.0.2-blue?style=for-the-badge" alt="Version">
  <a href="https://www.nuget.org/packages/FFS.StaticEcs/"><img src="https://img.shields.io/badge/NuGet-FFS.StaticEcs-004880?style=for-the-badge&logo=nuget" alt="NuGet"></a>
  <a href="https://felid-force-studios.github.io/StaticEcs/en/"><img src="https://img.shields.io/badge/Docs-documentation-blueviolet?style=for-the-badge" alt="Documentation"></a>
  <a href="https://gist.github.com/blackbone/6d254a684cf580441bf58690ad9485c3"><img src="https://img.shields.io/badge/Benchmarks-results-green?style=for-the-badge" alt="Benchmarks"></a>
  <a href="https://github.com/Felid-Force-Studios/StaticEcs-Unity"><img src="https://img.shields.io/badge/Unity-module-orange?style=for-the-badge&logo=unity" alt="Unity module"></a>
  <a href="https://github.com/Felid-Force-Studios/StaticEcs-Showcase"><img src="https://img.shields.io/badge/Showcase-examples-yellow?style=for-the-badge" alt="Showcase"></a>
  <br><br>
  <a href="https://felid-force-studios.github.io/StaticEcs/en/migrationguide.html"><img src="https://img.shields.io/badge/Migration_guide-2.0.0-red?style=for-the-badge" alt="Migration guide"></a>
</p>

<p align="center">
  <a href="./CHANGELOG_2_0_0_EN.md"><img src="https://img.shields.io/badge/🚀_What's_New_in_2.0.0-Entity_Types_·_Change_Tracking_·_Burst_·_Block_Iteration_·_Batch_Ops-ff6600?style=for-the-badge&labelColor=222222" alt="What's New in 2.0.0"></a>
</p>

# Static ECS - C# Hierarchical Inverted Bitmap ECS framework
- Performance
- Lightweight
- No allocations
- Low memory footprint
- No Unsafe in core
- Based on statics and structures
- Type-safe
- Free abstractions
- Powerful query engine with parallelism support
- Batch entity operations
- Component and tag change tracking
- Entity grouping by types and clusters
- Entity relations system
- World snapshot serialization
- Event system
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
  * [Resources](https://felid-force-studios.github.io/StaticEcs/en/features/resources.html)
  * [Query](https://felid-force-studios.github.io/StaticEcs/en/features/query.html)
  * [Events](https://felid-force-studios.github.io/StaticEcs/en/features/events.html)
  * [Change Tracking](https://felid-force-studios.github.io/StaticEcs/en/features/tracking.html)
  * [Serialization](https://felid-force-studios.github.io/StaticEcs/en/features/serialization.html)
  * [Compiler directives](https://felid-force-studios.github.io/StaticEcs/en/features/compilerdirectives.html)
* [Performance](https://felid-force-studios.github.io/StaticEcs/en/performance.html)
* [Unity integration](https://felid-force-studios.github.io/StaticEcs/en/unityintegrations.html)
* [AI Agent Integration](#ai-agent-integration)
* [License](#license)


# Contacts
* [felid.force.studios@gmail.com](mailto:felid.force.studios@gmail.com)
* [Telegram](https://t.me/felid_force_studios)

# Support the project
If you like Static ECS and it helps your project, you can support its development:

<a href="https://www.buymeacoffee.com/felid.force.studios" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" height="60"></a>

# Installation
The library has a dependency on [StaticPack](https://github.com/Felid-Force-Studios/StaticPack) `1.1.0` for binary serialization, StaticPack must also be installed
* ### As source code
  From the release page or as an archive from the branch. In the `master` branch there is a stable tested version
* ### Installation for Unity
  Via git module in Unity PackageManager:
  ```
  https://github.com/Felid-Force-Studios/StaticEcs.git
  https://github.com/Felid-Force-Studios/StaticPack.git
  ```
  Or adding to the manifest `Packages/manifest.json`:
  ```json
  "com.felid-force-studios.static-ecs": "https://github.com/Felid-Force-Studios/StaticEcs.git"
  "com.felid-force-studios.static-pack": "https://github.com/Felid-Force-Studios/StaticPack.git"
  ```
* ### NuGet
  ```
  dotnet add package FFS.StaticEcs
  ```
  For debug build with assertions:
  ```
  dotnet add package FFS.StaticEcs.Debug
  ```
  Packages: [FFS.StaticEcs](https://www.nuget.org/packages/FFS.StaticEcs/) · [FFS.StaticEcs.Debug](https://www.nuget.org/packages/FFS.StaticEcs.Debug/)

# AI Agent Integration
If you use AI coding assistants (Claude Code, Cursor, Copilot, etc.) with StaticEcs:
- **llms.txt**: Point your agent at [`https://felid-force-studios.github.io/StaticEcs/llms.txt`](https://felid-force-studios.github.io/StaticEcs/llms.txt) for a concise AI-readable reference
- **Full context**: [`https://felid-force-studios.github.io/StaticEcs/llms-full.txt`](https://felid-force-studios.github.io/StaticEcs/llms-full.txt) for comprehensive documentation
- **Claude Code**: Copy the [consumer CLAUDE.md snippet](https://felid-force-studios.github.io/StaticEcs/en/aiagentguide.html) into your project's `CLAUDE.md`
- **Common pitfalls**: See the [pitfalls guide](https://felid-force-studios.github.io/StaticEcs/en/pitfalls.html)


# Concept
StaticEcs — a new ECS architecture based on an inverted hierarchical bitmap model.
Unlike traditional ECS frameworks that rely on archetypes or sparse sets, this design introduces an inverted index structure where each component type owns entity bitmaps instead of entities storing component masks.
A hierarchical aggregation of these bitmaps provides logarithmic-space indexing of entity blocks, enabling O(1) block filtering and efficient parallel iteration through bitwise operations.
This approach completely removes archetype migration and sparse-set indirection, offering direct SoA-style memory access across millions of entities with minimal cache misses.
The model achieves up to 64× fewer memory lookups per block and scales linearly with the number of active component sets, making it ideal for large-scale simulations, open worlds with streaming, networked games with state synchronization, reactive AI with thousands of agents, and projects with frequent component composition changes (buffs, effects, statuses).

In archetype-based ECS (Unity DOTS, Flecs, Bevy, Arch), every component addition or removal triggers entity migration — copying all data to a new archetype, and the number of component combinations leads to archetype explosion.
In sparse-set ECS (EnTT, DefaultEcs), component access requires indirect addressing through sparse tables with at least two cache misses per lookup.
StaticEcs eliminates both problems: each entity occupies a fixed slot in segmented arrays and never moves in memory — Add/Remove is an O(1) bit flip in the presence mask with no data copying. Memory-stable entity addresses enable cheap entity relations through versioned identifiers (EntityGID), including links with streaming where some related entities reside in unloaded zones — ideal for complex simulations in open worlds. The number of component types has no impact on storage structure, since each type owns its own mask independently of the others. Two-dimensional EntityType × Cluster partitioning further ensures cache locality: entities of the same type within a cluster occupy adjacent memory segments, while clusters allow loading and unloading entire spatial zones without touching the rest of the data.

Memory is organized hierarchically: chunks (4,096 entities) → segments (256) → blocks (64). A world query starts by ANDing heuristic masks at the chunk level — a single bitwise operation covers up to 4,096 entities, skipping empty blocks entirely — then refines at the 64-entity block level. Batch operations (BatchAdd, BatchRemove, BatchSetTag) process up to 64 entities with a single bitwise operation.


> - The core idea of this implementation is static: all world and component data resides in static generic classes (`World<TWorld>`), enabling avoidance of costly virtual calls and allocations, with a convenient API and plenty of syntactic sugar. The JIT compiler eliminates dead code for unused component hooks
> - This framework is focused on maximum ease of use, speed and comfort of code writing without loss of performance
> - Multi-world creation, strict typing, ~zero-cost abstractions
> - Binary serialization system with world, cluster, and per-entity snapshots, schema versioning and compression support
> - Entity relations system with automatic bidirectional hooks for hierarchies, groups, and links
> - Reactive change tracking for network synchronization, UI, and triggers
> - Multi-components — variable-length per-entity data (inventory, buffs) without heap allocations
> - Multithreaded processing with parallel queries and block-level safety guarantees
> - Low memory usage, SoA layout (Structure of Arrays) — components of the same type in contiguous arrays
> - Built on Bitmap architecture, no archetypes, no sparse-sets
> - The framework was created for the needs of a private project and put out in open-source.

# Quick start
```csharp
using FFS.Libraries.StaticEcs;

// Define the world type
public struct WT : IWorldType { }

// Define type-alias for convenient access
public abstract class W : World<WT> { }

// Define the systems type
public struct GameSystems : ISystemsType { }

// Define type-alias for systems
public abstract class GameSys : W.Systems<GameSystems> { }

// Define components
public struct Position : IComponent { public Vector3 Value; }
public struct Direction : IComponent { public Vector3 Value; }
public struct Velocity : IComponent { public float Value; }

// Define a system
public struct VelocitySystem : ISystem {
    public void Update() {
        // Iteration via foreach
        foreach (var entity in W.Query<All<Position, Velocity, Direction>>().Entities()) {
            ref var pos = ref entity.Ref<Position>();
            ref readonly var dir = ref entity.Read<Direction>();
            ref readonly var vel = ref entity.Read<Velocity>();
            pos.Value += dir.Value * vel.Value;
        }

        // Or via delegate (faster, zero-allocation)
        W.Query<All<Position, Velocity, Direction>>().For(
            static (ref Position pos, in Velocity vel, in Direction dir) => {
                pos.Value += dir.Value * vel.Value;
            }
        );
    }
}

public class Program {
    public static void Main() {
        // Create the world
        W.Create(WorldConfig.Default());

        // Auto-register all components, tags, events, etc. from the calling assembly
        W.Types().RegisterAll();

        // Initialize the world
        W.Initialize();

        // Create and configure systems
        GameSys.Create();
        GameSys.Add(new VelocitySystem(), order: 0);
        GameSys.Initialize();

        // Create an entity with components
        var entity = W.NewEntity<Default>().Set(
            new Position { Value = Vector3.Zero },
            new Direction { Value = Vector3.UnitX },
            new Velocity { Value = 1f }
        );

        // Update all systems — called every frame
        GameSys.Update();
        // Advance change tracking (changes become visible next frame)
        W.Tick();

        // Destroy systems
        GameSys.Destroy();
        // Destroy the world and clean up all data
        W.Destroy();
    }
}
```

# License
[MIT license](https://github.com/Felid-Force-Studios/StaticEcs/blob/master/LICENSE.md)
