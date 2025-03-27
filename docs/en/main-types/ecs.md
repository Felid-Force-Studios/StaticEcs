---
title: Ecs
parent: Main types
nav_order: 8
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

## Ecs
Library entry point responsible for accessing, creating, initializing, operating, and destroying world data
- Represented as a static class `Ecs<T>` parameterized by `IWorldType`

{: .important }
> Since the type-identifier `IWorldType` defines access to a specific world   
> There are three ways to work with the framework:

___

#### The first way is as is via full address (very inconvenient):
```c#
public struct MainWorldType : IWorldType { }

Ecs<MainWorldType>.Create(EcsConfig.Default());
Ecs<MainWorldType>.World.EntitiesCount();

var entity = Ecs<MainWorldType>.Entity.New<Position>();
```

#### The second way is a little more convenient, use static imports or static aliases (you'll have to write in each file)
```c#
using static FFS.Libraries.StaticEcs.Ecs<MainWorldType>;

public struct MainWorldType : IWorldType { }

Create(EcsConfig.Default());
World.EntitiesCount();

var entity = Entity.New<Position>();
```

#### The third way is the most convenient, use type-aliases in the root namespace (no need to write in every file)  
This is the method that will be used everywhere in the examples
```c#
public struct MainWorldType : IWorldType { }

public abstract class MyEcs : Ecs<MainWorldType> { }
public abstract class MyWorld : Ecs<MainWorldType>.World { }

MyEcs.Create(EcsConfig.Default());
MyWorld.EntitiesCount();

var entity = MyEcs.Entity.New<Position>();
```

___

#### Basic operations:
```c#
// Defining the world ID
public struct MainWorldType : IWorldType { }

// Register types - aliases
public abstract class MyEcs : Ecs<MainWorldType> { }
public abstract class MyWorld : Ecs<MainWorldType>.World { }

// Creating a world with a default configuration
MyEcs.Create(EcsConfig.Default());
// Or a custom one
MyEcs.Create(new() {
            BaseEntitiesCount = 256,        // Base size of the entity array when creating a world
            BaseDeletedEntitiesCount = 256, // Base size of the deleted entity array when creating a world
            BaseComponentTypesCount = 64    // Base size of all variants of component types (number of pools for each type)
            BaseMaskTypesCount = 64,        // Base size of all variants of mask types (number of pools for each type)
            BaseTagTypesCount = 64,         // Base size of all variants of tags types (number of pools for each type)
            BaseComponentPoolCount = 128,   // Base size of the data array of components of a certain type (can be overridden for a specific type by explicit registration)
            BaseTagPoolCount = 128,         // Base size of the data array of tags of a certain type (can be overridden for a specific type by explicit registration)
        });

MyWorld.         // World access for MainWorldType (world ID)
MyEcs.Entity.    // Entity access for MainWorldType (world ID)
MyEcs.Context.   // Access to context for MainWorldType (world ID)
MyEcs.Components.// Access to components for MainWorldType (world ID)
MyEcs.Tags.      // Access to tags for MainWorldType (world ID)
MyEcs.Masks.     // Access to masks for MainWorldType (world ID)

// Initialization of the world
MyEcs.Initialize();

// Destroying and deleting the world's data
MyEcs.Destroy();

```