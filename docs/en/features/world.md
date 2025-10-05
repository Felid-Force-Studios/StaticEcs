---
title: World
parent: Features
nav_order: 9
---

## WorldType
World identifier type-tag, used to isolate static data when creating different worlds in the same process
- Represented as a user structure without data with a marker interface `IWorldType`

#### Example:
```c#
public struct MainWorldType : IWorldType { }
public struct MiniGameWorldType : IWorldType { }
```
___


## World
Library entry point responsible for accessing, creating, initializing, operating, and destroying world data
- Represented as a static class `World<T>` parameterized by `IWorldType`

{: .important }
> Since the type-identifier `IWorldType` defines access to a specific world   
> There are three ways to work with the framework:

___

#### The first way is as is via full address (very inconvenient):
```c#
public struct WT : IWorldType { }

World<WT>.Create(WorldConfig.Default());
World<WT>.CalculateEntitiesCount();

var entity = World<WT>.Entity.New<Position>();
```

#### The second way is a little more convenient, use static imports or static aliases (you'll have to write in each file)
```c#
using static FFS.Libraries.StaticEcs.World<WT>;

public struct WT : IWorldType { }

Create(WorldConfig.Default());
CalculateEntitiesCount();

var entity = Entity.New<Position>();
```

#### The third way is the most convenient, use type-aliases in the root namespace (no need to write in every file)
This is the method that will be used everywhere in the examples
```c#
public struct WT : IWorldType { }

public abstract class W : World<WT> { }

W.Create(WorldConfig.Default());
W.CalculateEntitiesCount();

var entity = W.Entity.New<Position>();
```

___

#### Basic operations:
```c#
// Defining the world ID
public struct WT : IWorldType { }

// Register types - aliases
public abstract class World : World<WT> { }

// Creating a world with a default configuration
W.Create(WorldConfig.Default());
// Or a custom one
W.Create(new() {
            // Base capacity for entities when creating a world
            baseEntitiesCapacity = 4096,                        
            // Base size of all component types (number of component types)
            BaseComponentTypesCount = 64                        
            // Base size of all varieties of tag types (number of tag types)
            BaseTagTypesCount = 64,                             
            // Operation mode of multithreaded processing 
            // (Disabled - no threads are created, MaxThreadsCount - maximum available number of threads is created, CustomThreadsCount - specified number of threads)
            ParallelQueryType = ParallelQueryType.Disabled,
            // Number of threads at ParallelQueryType.CustomThreadsCount
            CustomThreadCount = 4,
            // Strict Query default mode of operation, optionally in the Queries section
            DefaultQueryModeStrict = true
        });

W.Entity.    // Entity access for MainWorldType (world ID)
W.Context.   // Access to context for MainWorldType (world ID)
W.Components.// Access to components for MainWorldType (world ID)
W.Tags.      // Access to tags for MainWorldType (world ID)

// Initialization of the world
W.Initialize();

// Destroying and deleting the world's data
W.Destroy();

// true if the world is initialized
bool initialized = W.IsInitialized();

// the number of active entities in the world
int entitiesCount = W.CalculateEntitiesCount();

// current capacity for entities
int entitiesCapacity = W.CalculateEntitiesCapacity();
```