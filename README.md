![Version](https://img.shields.io/badge/version-0.9.2.1-blue.svg?style=for-the-badge)

### LANGUAGE
[RU](./README_RU.md)
[EN](./README.md)
___
### [BENCHMARKS](./Benchmark.md)
### [Unity module](https://github.com/Felid-Force-Studios/StaticEcs-Unity)

# Static ECS - C# Entity component system framework
- Lightweight
- Performance
- No allocations
- No dependencies
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
* [Quick start](#quick-start)
* [Concept](#concept)
* [Main types](#main-types)
    * [Entity](#entity)
    * [PackedEntity](#packedentity)
    * [Component](#component)
    * [StandardComponent](#standardComponent)
    * [Tag](#tag)
    * [Mask](#mask)
    * [WorldType](#WorldType)
    * [Ecs](#ecs)
    * [World](#world)
    * [Systems](#systems)
    * [Context](#context)
    * [Query](#query)
* [Additional features](#additional-features)
    * [Component identifiers](#component-identifiers)
    * [Auto handlers](#auto-handlers)
    * [Events](#events)
    * [Relations](#relation)
* [Performance](#perfomance)
* [Engine integration](#engine-integration)
    * [Unity](#unity)
* [FAQ](#faq)
* [License](#license)


# Contacts
* [Telegram](https://t.me/felid_force_studios)
# Installation
* ### As source code
  From the release page or as an archive from the branch. In the `master` branch there is a stable tested version
* ### Installation for Unity
  git module `https://github.com/Felid-Force-Studios/StaticEcs.git` in Unity PackageManager or adding it to `Packages/manifest.json`


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

# Concept
> - The main idea of this implementation is static, all data about the world and components are in static classes, which makes it possible to avoid expensive virtual calls and have a convenient API
> - This framework is focused on maximum ease of use, speed and comfort of code writing without loss of performance
> - Multi-world creation, strict typing, ~zero-cost abstractions
> - Reduced monomorphization of generic types and methods is available to reduce code sources through the component identifier mechanism (additional features section)
> - Based on a sparse-set architecture, the core is inspired by a series of libraries from Leopotam
> - The framework was created for the needs of a private project and put out in open-source.


# Main types:

### Entity
Entity - serves to identify an object in the game world and access attached components
 - Represented as a 4 byte structure

<details><summary><u><b>Usage 👇</b></u></summary>

- Creation:
```csharp
// Creation is only possible with the initial components specified
// Creating a single entity
// Method 1 - with component type (overload methods from 1-5 components)
var entity = MyEcs.Entity.New<Position>();
var entity = MyEcs.Entity.New<Position, Velocity, Name>();

// Method 2 - with component value (overload methods from 1-5 components)
var entity = MyEcs.Entity.New(new Position(x: 1, y: 1, z: 2));
var entity = MyEcs.Entity.New(
            new Name { Val = "SomeName" },
            new Velocity { Val = 1f },
            new Position { Val = Vector3.One }
);

// Creating multiple entities
// Method 1 - with component type (overload methods from 1-5 components)
int count = 100;
MyEcs.Entity.NewOnes<Position>(count);

// Method 2 - specifying component type (overload methods from 1-5 components) + delegate initialization of each entity
int count = 100;
Ecs.Entity.NewOnes<Position>(count, static entity => {
    // some init logic for each entity
});

// Method 3 - with component value (overload methods from 1-5 components)
int count = 100;
MyEcs.Entity.NewOnes(count, new Position(x: 1, y: 1, z: 2));

// Method 4 - with component value (overload methods from 1-5 components) + initialization delegate of each entity
int count = 100;
MyEcs.Entity.NewOnes(count, new Position(x: 1, y: 1, z: 2), static entity => {
    // some init logic for each entity
});
```

- Basic operations:
```csharp
var entity = MyEcs.Entity.New(
            new Name { Val = "SomeName" },
            new Velocity { Val = 1f },
            new Position { Val = Vector3.One }
);

bool actual = entity.IsActual();         // Check if an entity has been deleted in the world
short version = entity.Version();        // Get entity version
var clone = entity.Clone();              // Clone the entity and all components, tags, masks
entity.Destroy();                        // Delete the entity and all components, tags, masks

var entity2 = MyEcs.Entity.New<Name>();
clone.CopyTo(entity2);                   // Copy all components, tags, masks to the specified entity

var entity3 = MyEcs.Entity.New<Name>();
entity2.MoveTo(entity3);                 // Move all components to the specified entity and delete the current entity

PackedEntity packed = entity3.Pack();  // Pack an entity with meta information about the version to be transmitted

var str = entity3.ToPrettyString();      // Get a string with all information about the entity
```
</details>

### PackedEntity
Packed entity - stores meta information of the entity, serves for secure transfer of the entity (e.g. in events, components, etc.).
> an entity is just an id, a packaged entity is id + version  
> just by id it is impossible to determine whether this entity that is now in the world under this identifier or not, you can only together with the version, for this purpose packaged version
 - Represented as an 8 byte structure

<details><summary><u><b>Usage 👇</b></u></summary>

- Creation:
```csharp
// Creation is only possible through an unpackaged entity
PackedEntity packedEntity = entity.Pack();
```
- Basic operations:
```csharp
PackedEntity packedEntity = entity.Pack();
// Attempt to unpack an entity in the world whose identifier is specified via the type parameter, returns true if the entity is successfully unpacked, in out unpacked entity
if (packedEntity.TryUnpack<WorldType>(out var unpackedEntity)) {
    // ...
}

PackedEntity packedEntity2 = entity.Pack();
bool equals = packedEntity.Equals(packedEntity2);     // Verify the identity of the packaged entities
```
</details>

### Component
Component - provides properties to an entity  
 - An entity cannot exist without components, because an entity without data is just an identifier
 - When the last component is deleted, the entity is deleted
 - Gives the ability to build search queries by component only
 - Presented as a custom structure with a marker interface `IComponent`  
 - Presented as struct solely for performance reasons
   Example:
```c#
public struct Position : IComponent {
    public Vector3 Value;
}
```

**IMPORTANT** ❗️  
Requires registration in the world between creation and initialization

Example:
```c#
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.World.RegisterComponentType<Position>();
//...
MyEcs.Initialize();
```

<details><summary><u><b>Usage 👇</b></u></summary>

- Creation:
```c#
// Method 1 - when creating an entity (similar to the Add() method)
var entity = MyEcs.Entity.New<Position>();

// Or via a value (similar to the Put() method)
// Be careful with AutoInit and AutoReset (see additional features)
var entity = MyEcs.Entity.New(new Position(x: 1, y: 1, z: 2));

// Adding a component to an entity and returning a ref value to the component (in DEBUG mode there will be an error if it already exists on the entity).
ref var position = ref entity.Add<Position>();
// Adding a component to an entity (in DEBUG mode there will be an error if it already exists on the entity) (overload methods from 2-5 components)
entity.Add<Position, Velocity>();

// Add a component to an entity if it does not already exist, otherwise return an existing component
ref var position = ref entity.TryAdd<Position>();
ref var position = ref entity.TryAdd<Position>(out bool added); // overload where added = true if the component is new, false if an existing component is returned
// Adding a component to an entity if it does not already exist (overload methods from 2-5 components)
entity.TryAdd<Position, Velocity>();
entity.TryAdd<Position, Velocity>(out bool added); // overloading where added = true if at least 1 component is new, false all components existed previously

// Putting a component through a value. (overload methods from 1-5 components)
// If the component already exists, all data will be replaced, if it does not exist, it will be created with the passed data.
// Be careful with AutoInit and AutoReset (see additional features).
entity.Put(new Position(x: 1, y: 1, z: 2));
```
- Basic operations:
```c#
var entity = MyEcs.Entity.New(
            new Name { Val = "Player" },
            new Velocity { Val = 1f },
            new Position { Val = Vector3.One }
);
// Get the count of components on an entity
int componentsCount = entity.ComponentsCount();

// Get ref reference to read/write component
ref var velocity = ref entity.RefMut<Velocity>();
velocity.Val++;

// Get ref reference to a read-only component
ref readonly var readOnlyVelocity = ref entity.Ref<Velocity>();
//readOnlyVelocity.Value++;  -   ERROR

// Check for the presence of ALL specified components (overload methods from 1-3 components)
entity.HasAllOf<Position>();
entity.HasAllOf<Position, Velocity, Name>();

// Check for the presence of at least one specified component (overload methods from 2-3 components)
entity.HasAnyOf<Position, Name>();
entity.HasAnyOf<Position, Velocity, Name>();

// Remove a component from an entity (overload methods from 1-5 components)
bool deleted = entity.Delete<Position>();  // deleted = true if the component has been deleted, false if the component was not there originally
bool deleted = entity.Delete<Position, Velocity, Name>();  // deleted = true if ALL components have been deleted, false if at least 1 component was not there originally

var entity2 = MyEcs.Entity.New<Name>();
// Copy the specified components to another entity (overload methods from 1-5 components)
entity.CopyComponentsTo<Position, Velocity>(entity2);
```
</details>


### StandardComponent
Default Component - standard entity properties, present on every entity created by default
> Should be used when ALL entities in the world must contain components of some type  
> for example, if the position component must be on every entity without exception, you should use the standard component  
> as the access times are faster, and there will be no additional memory overhead
>
> Can also be used for small components 1-8 bytes in size, if no logic is required based on the presence or absence of a component
>
> For example, the internal version of an entity `entity.Version()` is a standard component
- Optimized storage and direct access to data by entity ID
- Cannot be deleted, only modified
- When the last `IComponent` is deleted, the Standard Component is ignored and the entity is deleted
- Not included in queries, as it is present on all entities
- Represented as a custom structure with a `IStandardComponent` marker interface

Example:
```c#
public struct EnitiyType : IStandardComponent {
    public int Val;
}
```

**IMPORTANT** ❗️  
Requires registration in the world between creation and initialization

Example:
```c#
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.World.RegisterStandardComponentType<EnitiyType>();
//...
MyEcs.Initialize();
```

**IMPORTANT** ❗️  
If automatic initialization when creating an entity or automatic reset when deleting an entity is required  
handlers must be explicitly registered

Example:
```c#
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.World.RegisterStandardComponentType<EnitiyType>();

// This function will be called when the entity is created  
MyEcs.StandardComponents<EnitiyType>.SetAutoInit((ref EnitiyType component) => component.Val = 1);

// This function will be called when the entity is destroyed  
MyEcs.StandardComponents<EnitiyType>.SetAutoReset((ref EnitiyType component) => component.Val = -1);

// When copying standard components, this entity will be called instead of just copying it
MyEcs.StandardComponents<EnitiyType>.SetAutoCopy((ref EnitiyType src, ref EnitiyType dst) => dst.Val = src.Val);
//...
MyEcs.Initialize();
```

<details><summary><u><b>Usage 👇</b></u></summary>

- Basic operations:
```c#
// Get the number of standard components per entity
int standardComponentsCount = entity.StandardComponentsCount();

// Get ref reference to a standard read/write component
ref var enitiyType = ref entity.RefMutStandard<EnitiyType>();
enitiyType.Val = 123;

// Get ref reference to a standard read-only component
ref readonly var readOnlyEnitiyType = ref entity.RefStandard<EnitiyType>();
//readOnlyEnitiyType.Val = 123;  -   ERROR

var entity2 = MyEcs.Entity.New<SomeComponent>();
// Copy the specified standard components to another entity (overload methods from 1-5 components)
entity.CopyStandardComponentsTo<EnitiyType>(entity2);
```
</details>

### Tag
Tag - similar to a component, but does not contain any data, serves to label an entity  
 - Optimized storage, doesn't store massive amounts of data, doesn't slow down component searches, allows you to create multiple tags
 - When the last component is deleted, the Tags are ignored and the entity is deleted
 - Gives the option to build search queries based on tags only
 - Represented as a user structure without data with a marker interface `ITag`

Example:
```c#
public struct Unit : ITag { }
```

**IMPORTANT** ❗️  
Requires registration in the world between creation and initialization

Example:
```c#
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.World.RegisterTagType<Unit>();
//...
MyEcs.Initialize();
```

<details><summary><u><b>Usage 👇</b></u></summary>

- Creation:
```c#
// Adding a tag to an entity (in DEBUG mode there will be an error if it already exists on the entity) (overload methods from 1-5 tags)
entity.SetTag<Unit, Player>();
```
- Basic operations:
```c#
// Get the count of tags on an entity
int tagsCount = entity.TagsCount();

// Check for the presence of ALL specified tags (overload methods from 1-3 tags)
entity.HasAllOfTags<Unit>();
entity.HasAllOfTags<Unit, Player>();

// Check for the presence of at least one specified tag (overload methods from 2-3 tags)
entity.HasAnyOfTags<Unit, Player>();

// Remove a tag from an entity (overload methods from 1-5 tags)
bool deleted = entity.DeleteTag<Unit>();  // deleted = true if the tag has been deleted, false if the tag was not there originally
bool deleted = entity.DeleteTag<Unit, Player>();  // deleted = true if ALL tags have been deleted, false if at least 1 tag was not originally there.
```
</details>

### Mask
Mask - similar to a tag, but uses only 1 bit of memory
- When the last component is deleted, Masks as well as Tags are ignored and the entity is deleted
- NOT Gives the option to build queries by masks only, can only be used as an additional search criterion
- Represented as a user structure without data with a marker interface `IMask`

Example:
```c#
public struct Visible : IMask { }
```

**IMPORTANT** ❗️  
Requires registration in the world between creation and initialization

Example:
```c#
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.World.RegisterMaskType<Visible>();
//...
MyEcs.Initialize();
```

<details><summary><u><b>Usage 👇</b></u></summary>

- Creation:
```c#
// Adding a mask to an entity (overload methods from 1-5 masks)
entity.SetMask<Flammable, Frozen, Visible>();
```
- Basic operations:
```c#
// Get the count of masks on an entity
int masksCount = entity.MasksCount();

// Check for the presence of ALL specified masks (overload methods from 1-3 masks)
entity.HasAllOfMasks<Flammable>();
entity.HasAllOfMasks<Flammable, Frozen, Visible>();

// Check for the presence of at least one specified mask (overload methods from 2-3 masks)
entity.HasAnyOfMasks<Flammable, Frozen, Visible>();

// Remove a mask from an entity (overload methods from 1-5 masks)
entity.DeleteMask<Frozen>();
```
</details>

### WorldType
World identifier type-tag, used to isolate static data when creating different worlds in the same process
- Represented as a user structure without data with a marker interface `IWorldType` 

Example:
```c#
public struct MainWorldType : IWorldType { }
public struct MiniGameWorldType : IWorldType { }
```

### Ecs
Library entry point responsible for accessing, creating, initializing, operating, and destroying world data
- Represented as a static class `Ecs<T>` parameterized by `IWorldType`
> IMPORTANT: Since the type-identifier `IWorldType` defines access to a specific world   
> There are three ways to work with the framework:

The first way is as is via full address (very inconvenient):
```c#
public struct MainWorldType : IWorldType { }

Ecs<MainWorldType>.Create(EcsConfig.Default());
Ecs<MainWorldType>.World.EntitiesCount();

var entity = Ecs<MainWorldType>.Entity.New<Position>();
```

The second way is a little more convenient, use static imports or static aliases (you'll have to write in each file)
```c#
using static FFS.Libraries.StaticEcs.Ecs<MainWorldType>;

public struct MainWorldType : IWorldType { }

Create(EcsConfig.Default());
World.EntitiesCount();

var entity = Entity.New<Position>();
```

The third way is the most convenient, use type-aliases in the root namespace (no need to write in every file)  
This is the method that will be used everywhere in the examples
```c#
public struct MainWorldType : IWorldType { }

public abstract class MyEcs : Ecs<MainWorldType> { }
public abstract class MyWorld : Ecs<MainWorldType>.World { }

MyEcs.Create(EcsConfig.Default());
MyWorld.EntitiesCount();

var entity = MyEcs.Entity.New<Position>();
```

<details><summary><u><b>Usage 👇</b></u></summary>

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
</details>

### World
World, contains meta information of entities, controls and manages creation and deletion of entities
- Represented as a static class `Ecs<IWorldType>.World`

<details><summary><u><b>Usage 👇</b></u></summary>

- Creation:
```c#
// It is created only when called
MyEcs.Create(config);

// Initialized only when called
MyEcs.Initialize();
```
- Basic operations:
```c#
// When registering a component, it is possible to specify the base size of the data array of components of this type
// The dynamic identifier of the component type is also returned (section Additional Features)
var positionComponentId = MyWorld.RegisterComponentType<Position>(256);

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

// entity version
short entityVersion = MyWorld.EntityVersion(entity);

// Delete an entity and all its components - similar to entity.Destroy();
MyWorld.DestroyEntity(entity);

// Copy all component tags and masks from one entity to another - similar to entitySrc.CopyTo(entityTarget);
MyWorld.CopyEntityData(entitySrc, entityTarget);

// Clone the entity and all component tags and masks - similar to entity.Clone();
var clone = MyWorld.CloneEntity(entity);

// Get a string with all the information about the entity - similar to entity.ToPrettyStringEntity();
var str = MyWorld.ToPrettyStringEntity(entity);

```
</details>

### SystemsType
Type-tag system identifier, used to isolate static data when creating groups of systems in the same process
- Represented as a user structure without data with a marker interface `ISystemsType`

Пример:
```c#
public struct BaseSystemsType : ISystemsType { }
public struct FixedSystemsType : ISystemsType { }
public struct LateSystemsType : ISystemsType { }
```

### Systems
Systems, controls and manages the creation and run of systems
- Represented as a static class `Systems<ISystemsType>`

<details><summary><u><b>Usage 👇</b></u></summary>

```c#
// The systems are of 3 types and can be used in all combinations together or separately

// IInitSystem - Init() method is run once when MySystems.Initialize() is called
// Example:
public struct SomeInitSystem : IInitSystem {
    public void Init() { }
}

// IUpdateSystem - the Update() method runs every time MySystems.Update() is called
// Example:
public struct SomeUpdateSystem : IUpdateSystem {
    public void Update() { }
}

// IDestroySystem - the Destroy() method runs once when MySystems.Destroy() is called;
// Example:
public struct SomeDestroySystem : IDestroySystem {
    public void Destroy() { }
}

 // Combined system
 public struct SomeInitDestroySystem : IInitSystem, IDestroySystem {
     public void Init() { }
     public void Destroy() { }
 }

 // Combined system
 public struct SomeComboSystem : IInitSystem, IUpdateSystem, IDestroySystem {
     public void Init() { }
     public void Update() { }
     public void Destroy() { }
 }
```

- Создание и операции:
```c#
// Define system identifier
public struct MySystemsType : ISystemsType { }

// Define type-alias for easy access to systems
public abstract class MySystems : MyEcs.Systems<MySystemsType> { }

// The structures for the systems will be created here
MySystems.Create();

// Adding a system NOT implementing IUpdateSystem, i.e. Init and / or Destroy system
MySystems.AddCallOnce(new SomeInitSystem());
MySystems.AddCallOnce(new SomeDestroySystem>());
MySystems.AddCallOnce(new SomeInitDestroySystem>());

// Adding a system implementing IUpdateSystem, with any implementations such as Init or Destroy.
MySystems.AddUpdate(new SomeComboSystem());

// Important! The systems are started in the order passed by the second argument (default order 0)
MySystems.AddUpdate(new SomeComboSystem(), order: 3);

// this means that first all Init systems will be started in the order in which they were added.
// then in the game loop all Update systems will be executed in order.
// then all Destroy type systems will be called in order when the world is destroyed.

// Important! Systems can be structures or classes
// (using structures can significantly increase perfomance for small systems)

// It is possible to connect systems in batches, which can significantly increase performance
// Adding a batches of systems, each system can implement any types of systems but must have IUpdateSystem implementation.
MySystems.AddUpdate(
    new SomeUpdateSystem1(),
    new SomeComboSystem1(),
    new SomeComboSystem2(),
    new SomeComboSystem3(),
    new SomeComboSystem4(),
    new SomeComboSystem5(),
    new SomeComboSystem()
);

// All Init systems will be called here
MySystems.Initialize();

// All Update systems will be called here
MySystems.Update();

// All Destroy systems will be called here
MySystems.Destroy();
```
</details>

### Context
Context is an alternative to DI, a simple mechanism for storing and transferring user data and services to systems and other methods
- Represented as a static class `Ecs<IWorldType>.Context<T>`

<details><summary><u><b>Usage 👇</b></u></summary>

- Basic operations:
```c#
// User classes and services
public class UserService1 { }
public class UserService2 { }

// Adding necessary objects to the context, it is not necessary to add objects to the context before initialization, new data can also be added in the process of systems operation
// It is important to remember that if the context is used in Init systems, the data should be passed there before Ecs.Initialize() or before the call in the call chain of a particular Init system. 
// Important! The context can store strictly 1 object of 1 type - an error will occur if the Set method is set repeatedly of the same type.
MyEcs.Context<UserService1>.Set(new UserService1());
MyEcs.Context<UserService2>.Set(new UserService2());

// If Replace is called, the specified type will be set or replaced without error
MyEcs.Context<UserService2>.Replace(new UserService2());

// Check if there is a value of this type in the context
bool has = MyEcs.Context<UserService2>.Has();

// Remove the value from the context
MyEcs.Context<UserService2>.Remove();

// Important! The user himself takes care of clearing the context if it is no longer needed or when the world is already deleted, e.g. in the Destroy method in systems
```
</details>

### Query
Queries - a mechanism that allows you to search for entities and their components in the world

<details><summary><u><b>Usage 👇</b></u></summary>

Let's look at the basic capabilities of searching for entities in the world:
```c#
// There are many available query options
// World.QueryEntities.For()\With() returns an iterator of entities matching the condition
// The following types are available for applying component filtering conditions:

// All - filters entities for the presence of all specified components (overload from 1 to 8)
AllTypes<Types<Position, Direction, Velocity>> _all = default;
// or
All<Position, Direction, Velocity> _all2 = default;

// AllAndNone - filters entities for the presence of all specified components in the first group and the absence of all components in the second group (overload from 1 to 8).
AllAndNoneTypes<Types<Position, Direction, Velocity>, Types<Name>> _allAndNone = default;

// None - filters entities for the absence of all specified components (can be used only as part of other methods) (overloading from 1 to 8)
NoneTypes<Types<Name>> _none = default;
// or
None<Name> _none2 = default;

// Any - filters entities for the presence of any of the specified components (can only be used as part of other methods) (overloading from 1 to 8)
AnyTypes<Types<Position, Direction, Velocity>> _any = default;
// or
Any<Position, Direction, Velocity> _any2 = default;

// Analogs for tags
// TagAll - filters entities for the presence of all specified tags (overload from 1 to 8)
TagAllTypes<Tag<Unit, Player>> _all = default;
// or
TagAll<Unit, Player> _all2 = default;

// AllAndNone - filters entities for the presence of all specified tags in the first group and the absence of all tags in the second group (overloading from 1 to 8).
TagAllAndNoneTypes<Tag<Unit>, Tag<Player>> _allAndNone = default;

// None - filters entities for the absence of all specified tags (can only be used as part of other methods) (overloading from 1 to 8).
TagNoneTypes<Tag<Unit>> _none = default;
// or
TagNone<Unit> _none2 = default;

// Any - filters entities for the presence of any of the specified tags (can only be used as part of other methods) (overloading from 1 to 8)
TagAnyTypes<Tag<Unit, Player>> _any = default;
// or
TagAny<Unit, Player> _any2 = default;

// Mask analogs
// MaskAll - filters entities for the presence of all specified tags (can only be used as part of other methods) (overloading from 1 to 8)
MaskAllTypes<Mask<Flammable, Frozen, Visible>> _all = default;
// or
MaskAll<Flammable, Frozen, Visible> _all2 = default;

// AllAndNone - filters entities for the presence of all specified tags in the first group and the absence of all tags in the second group (can only be used as part of other methods).
MaskAllAndNoneTypes<Mask<Flammable, Frozen>, Mask<Visible>> _allAndNone = default;

// None - filters entities for the absence of all specified tags (can only be used as part of other methods) (overloading from 1 to 8).
MaskNoneTypes<Mask<Frozen>> _none = default;
// or
MaskNone<Frozen> _none2 = default;

// Any - filters entities for the presence of any of the specified tags (can only be used as part of other methods) (overloading from 1 to 8)
MaskAnyTypes<Mask<Flammable, Frozen, Visible>> _any = default;
// or
MaskAny<Flammable, Frozen, Visible> _any2 = default;

// All types above do not require explicit initialization, do not require caching, each of them takes no more than 1-2 bytes and can be used on the fly


// Different sets of filtering methods can be applied to the World.QueryEntities.For() method for example:
// Option 1 method through generic
foreach (var entity in MyWorld.QueryEntities.For<All<Position, Direction, Velocity>>()) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Variant with 1 method via value
var all = default(All<Position, Direction, Velocity>);
foreach (var entity in MyWorld.QueryEntities.For(all)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Option with 3 methods via generic
foreach (var entity in MyWorld.QueryEntities.For<
             All<Position, Velocity, Name>,
             AllAndNoneTypes<Types<Position, Direction, Velocity>, Types<Name>>,
             None<Name>>()) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Variant with 3 methods via value
All<Position, Direction, Velocity> all2 = default;
AllAndNoneTypes<Types<Position, Direction, Velocity>, Types<Name>> allAndNone2 = default;
None<Name> none2 = default;
foreach (var entity in MyWorld.QueryEntities.For(all2, allAndNone2, none2)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Alternative with 3 methods via value
var all3 = Types<Position, Direction, Velocity>.All();
var allAndNone3 = Types<Position, Direction, Velocity>.AllAndNone(default(Types<Name>));
var none3 = Types<Name>.None();
foreach (var entity in MyWorld.QueryEntities.For(all3, allAndNone3, none3)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}


// Also, all filtering methods can be grouped into a With type
// which can be applied to the World.QueryEntities.With() method, for example:

// Method 1 via generic
foreach (var entity in MyWorld.QueryEntities.With<With<
             All<Position, Velocity, Name>,
             AllAndNoneTypes<Types<Position, Direction, Velocity>, Types<Name>>,
             None<Name>,
             Any<Position, Direction, Velocity>
         >>()) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Method 2 through values
With<
    All<Position, Velocity, Name>,
    AllAndNoneTypes<Types<Position, Direction, Velocity>, Types<Name>>,
    None<Name>,
    Any<Position, Direction, Velocity>
> with = default;
foreach (var entity in MyWorld.QueryEntities.With(with)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Method 3 through values alternative
var with2 = With.Create(
    default(All<Position, Velocity, Name>),
    default(AllAndNoneTypes<Types<Position, Direction, Velocity>, Types<Name>>),
    default(None<Name>),
    default(Any<Position, Direction, Velocity>)
);
foreach (var entity in MyWorld.QueryEntities.With(with2)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Method 4 through values alternative
var with3 = With.Create(
    Types<Position, Velocity, Name>.All(),
    Types<Position, Direction, Velocity>.AllAndNone(default(Types<Name>)),
    Types<Name>.None(),
    Types<Position, Direction, Velocity>.Any()
);
foreach (var entity in MyWorld.QueryEntities.With(with3)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}
```

Look at additional ways to search for entities in the world:
```c#
// World.QueryComponents.For()\With() returns an iterator of entities matching the condition immediately with components 


// Option 1 when you want to go through all components of the same type
// (very fast, can be used for very simple operations for which no entity or other components are needed)
foreach (ref var position in MyWorld.QueryComponents.For<Position>()) {
    position.Val += Vector3.UnitX;
}

// Option 2 with specifying a delegate and getting the required components at once, from 1 to 8 component types can be specified
MyWorld.QueryComponents.For<Position, Velocity, Name>((Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// you can remove generics, since they are derived from the passed function type
MyWorld.QueryComponents.For((Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// you can add a static constraint to the delegate to ensure that this delegate will not be allocated every time.
// in combination with Ecs.Context allows for convenient and productive code without creating closures in the delegate
MyWorld.QueryComponents.For(static (Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// You can also use WithAdds similar to With from the previous example, but allowing only secondary filtering methods (such as None, Any) for additional filtering of entities
// It should be noted that the components that are specified in the delegate are treated as All filter.
// i.e. WithAdds is just an addition to filtering and does not require specifying the components used.

WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
> with = default;

MyWorld.QueryComponents.With(with).For(static (Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// or
MyWorld.QueryComponents.With<WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
>>().For(static (Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});
```

Look at the special possibilities for finding entities in the world:
```c#
// Queries with structure-function passing 
// can be used to optimize or pass a state to a stratct or to pass logic.

// Let's define a structure-function that we can replace the delegate with.
// It must implement the IQueryFunction interface, specifying from 1-8 components.
readonly struct StructFunction : Ecs.IQueryFunction<Position, Velocity, Name> {
    public void Run(Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) {
        position.Val *= velocity.Val;
    }
}

// Option 1 with generic transmission
MyWorld.QueryComponents.For<Position, Velocity, Name, StructFunction>();

// Variant 1 with value transfer
MyWorld.QueryComponents.For<Position, Velocity, Name, StructFunction>(new StructFunction());

// Option 2 with With through generic
MyWorld.QueryComponents.With<WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
>>().For<Position, Velocity, Name, StructFunction>();

// Variant 2 with With through value
WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
> with = default;
MyWorld.QueryComponents.With(with).For<Position, Velocity, Name, StructFunction>();

// It is also possible to combine system and IQueryFunction, for example:
// this can improve code readability and increase perfomance + it allows accessing non-static members of the system
public struct SomeFunctionSystem : IInitSystem, IUpdateSystem, Ecs.IQueryFunction<Position, Velocity, Name> {
    private UserService1 _userService1;
    
    WithAdds<
        None<Types<Direction>>,
        Any<Types<Position, Direction, Velocity>>
    > with;
    
    public void Init() {
        _userService1 = Ecs.Context<UserService1>.Get();
    }
    
   public void Update() {
       MyWorld.QueryComponents
            .With(with)
            .For<Position, Velocity, Name, SomeFunctionSystem>(ref this);
   }
    
    public void Run(Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) {
        position.Val *= velocity.Val;
        _userService1.CallSomeMethod(name.Val);
    }
}
```

</details>

# Additional features
### Component identifiers
In case when the project is very large or on the contrary small and the size of compiled code is important:  
Things like - typed Query, and sugar methods of entity handling, can increase the compiled code size due to monomorphization of generic types in structures and methods.
To avoid this, a dynamic identifier mechanism for components, tags, and masks is implemented - which allow you to use them instead of type parameters  
How it works:
```csharp
// After calling Ecs.Create(EcsConfig.Default());
// You can explicitly register component types and get a structure containing the type identifier
ComponentDynId positionId = MyWorld.RegisterComponentType<Position>();
TagDynId unitTagId = MyWorld.RegisterTagType<Unit>();
MaskDynId frozenMaskId = MyWorld.RegisterMaskType<Frozen>();

// Alternatively, it is possible to get these identifiers at any time after initializing the world in the following way:
ComponentDynId positionId = Ecs.Components.DynamicId<Position>();
TagDynId unitTagId = Ecs.Tags.DynamicId<Unit>();
MaskDynId frozenMaskId = Ecs.Masks.DynamicId<Frozen>();

// These identifiers can be saved in any convenient way and used in entity or Query operations
// There are overloads for most entity methods for these identifiers.
// Example for entities
entity.Add(positionId);
entity.TryAdd(positionId, VelocityId, nameId);
entity.Delete(positionId, VelocityId, nameId);
entity.AddTag(unitTagId);
entity.SetMask(frozenMaskId);

// Example for Query
// There are Types1-8, Tag1-8, Mask1-8 overloads as well as generic implementations for extensions such as TypesBox, TypesArray, etc.
// These overloads contain identifier data, not empty ones like generic counterparts.
// this means that they should not be created on the fly, but cached for better performance.

var all = new Types3(positionId, VelocityId, nameId).All();
foreach (var entity in MyWorld.QueryEntities.For(all)) {
    //..
}

var with = With.Create(
    new Types3(positionId, VelocityId, nameId).All(),
    new Types1(scaleId).None(),
    new Tag2(playerTagId, unitTagId).Any(),
    new Mask1(frozenMaskId).AllAndNone(new Mask1(flammableMaskId))
);
foreach (var entity in MyWorld.QueryEntities.With(with)) {
    //..
}

// This mechanism also allows you to use these keys in the game logic
// For example, when the type of the created component changes depending on the conditions
// It is important to remember that these identifiers are dynamic, which means that there is no guarantee that they will be the same from run to run.
// so they can't be serialized or used in a similar way.
```
### Auto handlers
By default, when adding or deleting a component, the data is filled with the default value, and when copying, the component is completely copied  
To set your own logic of default initialization and resetting of the component you can use handlers

**AutoInit** - replaces the behavior when creating a component via the Add method
```csharp
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.Components<Position>.SetAutoInit(static (ref Position position) => position.Val = Vector3.One);
//...
MyEcs.Initialize();
```

**AutoReset** - replaces the behavior when deleting a component via the Delete method
```csharp
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.Components<Position>.SetAutoInit(static (ref Position position) => position.Val = Vector3.One);
//...
MyEcs.Initialize();
```

**AutoCopy** - replaces the behavior when copying a component
```csharp
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.Components<Position>.SetAutoCopy(static (ref Position src, ref Position dst) => dst.Val = src.Val);
//...
MyEcs.Initialize();
```

> **Important!** Keep in mind that creating an entity with a value or adding a component via the Put method  
> completely replace the data in the component, bypassing the auto handlers installed

### Events
Event - used to exchange information between systems or user services
- Presented as a custom structure with data

Example:
```c#
public struct WeatherChanged : IEvent { 
    public WeatherType WeatherType;
}
```

**IMPORTANT** ❗️  
Requires registration in the world between creation and initialization

Example:
```c#
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.Events.RegisterEventType<WeatherChanged>()
//...
MyEcs.Initialize();
```

<details><summary><u><b>Usage 👇</b></u></summary>

- Creation and basic operations:
```c#
// The event system will be created when MyEcs.Create is called and destroyed when MyEcs.Destroy is called
MyEcs.Create(EcsConfig.Default());
MyEcs.Initialize();
//...

// Before sending an event, the receiver of the event must be registered, otherwise the event will not be sent.
// Receiver can be registered after calling Ecs.Create (e.g. in the Init method of the system).
var weatherChangedEventReceiver = MyEcs.Events.RegisterEventReceiver<WeatherChanged>();

// Deleting an event receiver
MyEcs.Events.DeleteEventReceiver(ref weatherChangedEventReceiver);

// Important! The lifecycle of an event: the event will be deleted in two cases:
// 1) when it's been read by all registered receivers.
// 2) when it will be suppressed on reading (by calling Suppress or SuppressAll method (information below) ) )
// So it is important that all registered listeners read the events or the event is suppressed by any listener so that there is no accumulation of them

// Sending an event
MyEcs.Events.Send(new WeatherChanged { WeatherType = WeatherType.Sunny });

// Sending default event value
MyEcs.Events.Send<WeatherChanged>();

// Get a dynamic identifier of event type (see “Component Identifiers”)
var weatherChangedDynId = MyEcs.Events.DynamicId<WeatherChanged>();
// Send default event value (Suitable for marker events without data)
MyEcs.Events.SendDefault(weatherChangedDynId);

// Receiving events
foreach (var weatherEvent in weatherChangedEventReceiver) {
    Console.WriteLine("Weather is " + weatherEvent.Value.WeatherType);
}

foreach (var weatherEvent in weatherChangedEventReceiver) {
    // Event suppression - the event will be deleted and other receivers will no longer be able to read it
    weatherEvent.Suppress();
}

// Suppress all events for a given receiver
weatherChangedEventReceiver.SuppressAll();

// Marks the reading of all events for this receiver
weatherChangedEventReceiver.MarkAsReadAll();

```
</details>

### Relations
**WIP**

# Perfomance
Current benchmarks : [BENCHMARKS](./Benchmark.md)

When using il2Cpp in Unity, it's worth noting that direct calls to get components are a little bit faster:  
For example:
```csharp
// Performance in il2Cpp (there is no difference in Mono) can be better in the second option by 10-40%
// The same applies to tags and masks and all other methods HasAllOf<>, Delete<>, etc.
ref var position = ref entity.RefMut<Position>(); // sugar method via the entity
ref var position = ref Ecs.Components<Position>.Value.RefMut(entity); // direct call
```
```csharp
// It is also possible to use extension methods that are practically close in performance to the direct call
// To create them, you can use the live template for rider (read below) or codegenerate (WIP)
public static class PositionExtension {
    [MethodImpl(AggressiveInlining)]
    public static ref Position RefMutPosition(this Ecs.Entity entity) {
        return ref Ecs.Components<Position>.Value.RefMut(entity);
    }
}
ref var position = ref entity.RefMutPosition();
```
Component live template
```csharp
public static class $COMPONENT$Extension {
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static ref $COMPONENT$ Mut$COMPONENT$(this $ECS$.Entity entity) {
        return ref $ECS$.Components<$COMPONENT$>.Value.RefMut(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static ref readonly $COMPONENT$ $COMPONENT$(this $ECS$.Entity entity) {
        return ref $ECS$.Components<$COMPONENT$>.Value.Ref(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static ref $COMPONENT$ Add$COMPONENT$(this $ECS$.Entity entity) {
        return ref $ECS$.Components<$COMPONENT$>.Value.Add(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Add$COMPONENT$(this $ECS$.Entity entity, $COMPONENT$ $COMPONENT$) {
        $ECS$.Components<$COMPONENT$>.Value.Add(entity) = $COMPONENT$;
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static ref $COMPONENT$ TryAdd$COMPONENT$(this $ECS$.Entity entity) {
        return ref $ECS$.Components<$COMPONENT$>.Value.TryAdd(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void TryAdd$COMPONENT$(this $ECS$.Entity entity, $COMPONENT$ $COMPONENT$) {
        $ECS$.Components<$COMPONENT$>.Value.TryAdd(entity) = $COMPONENT$;
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Put$COMPONENT$(this $ECS$.Entity entity, $COMPONENT$ $COMPONENT$) {
        $ECS$.Components<$COMPONENT$>.Value.Put(entity, $COMPONENT$);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool Has$COMPONENT$(this $ECS$.Entity entity) {
        return $ECS$.Components<$COMPONENT$>.Value.Has(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool Delete$COMPONENT$(this $ECS$.Entity entity) {
        return $ECS$.Components<$COMPONENT$>.Value.Del(entity);
    }
}
```

Tag live template
```csharp
public static class $TAG$Extension {
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Set$TAG$(this $Ecs$.Entity entity) {
        $Ecs$.Tags<$TAG$>.Value.Set(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool Has$TAG$(this $Ecs$.Entity entity) {
        return $Ecs$.Tags<$TAG$>.Value.Has(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool Delete$TAG$(this $Ecs$.Entity entity) {
        return $Ecs$.Tags<$TAG$>.Value.Del(entity);
    }
}
```

Mask live template
```csharp
public static class $MASK$Extension {
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Set$MASK$(this $Ecs$.Entity entity) {
        $Ecs$.Masks<$MASK$>.Value.Set(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool Has$MASK$(this $Ecs$.Entity entity) {
        return $Ecs$.Masks<$MASK$>.Value.Has(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Delete$MASK$(this $Ecs$.Entity entity) {
        $Ecs$.Masks<$MASK$>.Value.Del(entity);
    }
}
```

# Engine integration
### Unity
Example:
```csharp
using System;
using UnityEngine;
using FFS.Libraries.StaticEcs;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public struct MyWorldType : IWorldType { }
public struct MySystemsType : ISystemsType { }

public abstract class MyEcs : Ecs<MyWorldType> { }
public abstract class MyWorld : MyEcs.World { }
public abstract class MySystems : MyEcs.Systems<MySystemsType> { }

public struct Position : IComponent {
    public Transform Value;
}

public struct Direction : IComponent {
    public Vector3 Value;
}

public struct Velocity : IComponent {
    public float Value;
}

[Serializable]
public struct SceneData {
    public GameObject EntityPrefab;
}

public struct CreateRandomEntities : IInitSystem {
    public void Init() {
        for (var i = 0; i < 100; i++) {
            var gameObject = Object.Instantiate(MyEcs.Context<SceneData>.Get().EntityPrefab);
            gameObject.transform.position = new Vector3(Random.Range(0, 50), 0, Random.Range(0, 50));
            MyEcs.Entity.New(
                new Position { Value = gameObject.transform },
                new Direction { Value = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)) },
                new Velocity { Value = 2f });
        }
    }
}

public struct UpdatePositions : IUpdateSystem {
    public void Update() {
        MyWorld.QueryComponents.For((MyEcs.Entity entity, ref Position position, ref Velocity velocity, ref Direction direction) => {
            position.Value.position += direction.Value * (Time.deltaTime * velocity.Value);
        });
    }
}

public class Main : MonoBehaviour {
    public SceneData sceneData;
    
    void Start() {
        MyEcs.Create(EcsConfig.Default());
                
        MyWorld.RegisterComponentType<Position>();
        MyWorld.RegisterComponentType<Direction>();
        MyWorld.RegisterComponentType<Velocity>();
        
        MyEcs.Initialize();
        
        MyEcs.Context<SceneData>.Set(sceneData);
        
        MySystems.Create();
        
        MySystems.AddCallOnce(new CreateRandomEntities());
        MySystems.AddUpdate(new UpdatePositions());
        
        MySystems.Initialize();
    }

    void Update() {
        MySystems.Update();
    }

    private void OnDestroy() {
        MySystems.Destroy();
        MyEcs.Destroy();
    }
}
```

# Faq
**WIP**

# License
[MIT license](./LICENSE.md)
