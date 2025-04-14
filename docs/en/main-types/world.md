---
title: World
parent: Main types
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
- Represented as a static class `Ecs<T>` parameterized by `IWorldType`

{: .important }
> Since the type-identifier `IWorldType` defines access to a specific world   
> There are three ways to work with the framework:

___

#### The first way is as is via full address (very inconvenient):
```c#
public struct WT : IWorldType { }

World<WT>.Create(WorldConfig.Default());
World<WT>.EntitiesCount();

var entity = World<WT>.Entity.New<Position>();
```

#### The second way is a little more convenient, use static imports or static aliases (you'll have to write in each file)
```c#
using static FFS.Libraries.StaticEcs.World<WT>;

public struct WT : IWorldType { }

Create(WorldConfig.Default());
EntitiesCount();

var entity = Entity.New<Position>();
```

#### The third way is the most convenient, use type-aliases in the root namespace (no need to write in every file)
This is the method that will be used everywhere in the examples
```c#
public struct WT : IWorldType { }

public abstract class World : World<WT> { }

World.Create(WorldConfig.Default());
World.EntitiesCount();

var entity = World.Entity.New<Position>();
```

___

#### Basic operations:
```c#
// Defining the world ID
public struct WT : IWorldType { }

// Register types - aliases
public abstract class World : World<WT> { }

// Creating a world with a default configuration
World.Create(WorldConfig.Default());
// Or a custom one
World.Create(new() {
            BaseEntitiesCount = 256,        // Base size of the entity array when creating a world
            BaseDeletedEntitiesCount = 256, // Base size of the deleted entity array when creating a world
            BaseComponentTypesCount = 64    // Base size of all variants of component types (number of pools for each type)
            BaseMaskTypesCount = 64,        // Base size of all variants of mask types (number of pools for each type)
            BaseTagTypesCount = 64,         // Base size of all variants of tags types (number of pools for each type)
            BaseComponentPoolCount = 128,   // Base size of the data array of components of a certain type (can be overridden for a specific type by explicit registration)
            BaseTagPoolCount = 128,         // Base size of the data array of tags of a certain type (can be overridden for a specific type by explicit registration)
        });

World.Entity.    // Entity access for MainWorldType (world ID)
World.Context.   // Access to context for MainWorldType (world ID)
World.Components.// Access to components for MainWorldType (world ID)
World.Tags.      // Access to tags for MainWorldType (world ID)
World.Masks.     // Access to masks for MainWorldType (world ID)

// Initialization of the world
World.Initialize();

// Destroying and deleting the world's data
World.Destroy();

// When registering a component, it is possible to specify the base size of the data array of components of this type
MyWorld.RegisterComponentType<Position>(256);

// similar to RegisterComponentType, but for tags
var unitTagId = MyWorld.RegisterTagType<Unit>(256);

// similar to RegisterComponentType, but for masks
var visibleMaskId = MyWorld.RegisterMaskType<Visible>();

// true if the world is initialized
bool initialized = MyWorld.IsInitialized();

// the number of active entities in the world
int entitiesCount = MyWorld.EntitiesCount();

// current capacity of the entity array
int entitiesCapacity = MyWorld.EntitiesCapacity();
```