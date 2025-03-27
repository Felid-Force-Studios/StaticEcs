---
title: Component
parent: Main types
nav_order: 3
---

## Component
Component - provides properties to an entity
- Gives the ability to build search queries by component only
- Presented as a custom structure with a marker interface `IComponent`
- Presented as struct solely for performance reasons

#### Example:
```c#
public struct Position : IComponent {
    public Vector3 Value;
}
```
___

{: .important }
Requires registration in the world between creation and initialization

```c#
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.World.RegisterComponentType<Position>();
//...
MyEcs.Initialize();
```
___

#### Creation:
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

___

#### Basic operations:
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

// Remove a component from an entity (in DEBUG mode there will be an error if the component does not exist on the entity,
// cannot be used in a release if there is no guarantee that the component is present) (overload methods from 1-5 components)
entity.Delete<Position>();
entity.Delete<Position, Velocity, Name>();

// Remove a component from an entity if it exists (overload methods from 1-5 components)
bool deleted = entity.Delete<Position>();  // deleted = true if the component has been deleted, false if the component was not there originally
bool deleted = entity.Delete<Position, Velocity, Name>();  // deleted = true if ALL components have been deleted, false if at least 1 component was not there originally

var entity2 = MyEcs.Entity.New<Name>();
// Copy the specified components to another entity (overload methods from 1-5 components)
entity.CopyComponentsTo<Position, Velocity>(entity2);
```

