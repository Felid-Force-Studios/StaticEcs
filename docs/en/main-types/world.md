---
title: World
parent: Main types
nav_order: 9
---

## World
World, contains meta information of entities, controls and manages creation and deletion of entities
- Represented as a static class `Ecs<IWorldType>.World`

___

#### Creation:
```c#
// It is created only when called
MyEcs.Create(config);

// Initialized only when called
MyEcs.Initialize();
```
___ 

#### Basic operations:
```c#
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