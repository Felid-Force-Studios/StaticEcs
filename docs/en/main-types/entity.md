---
title: Entity
parent: Main types
nav_order: 1
---

## Entity
Entity - serves to identify an object in the game world and access attached components
- Represented as a 4 byte structure

___

{: .important }
> By default an entity can be created and exist without components, also when the last component is deleted it is not deleted  
> If you want to override this, you must use the compiler directive `FFS_ECS_LIFECYCLE_ENTITY`  
> More info: [Compiler directives](../additional-features/compilerdirectives.md)

___

#### Creation:
```csharp
// Creating a single entity

// Method 1 - creating an “empty” entity
var entity = MyEcs.Entity.New();

// Method 2 - with component type (overload methods from 1-5 components)
var entity = MyEcs.Entity.New<Position>();
var entity = MyEcs.Entity.New<Position, Velocity, Name>();

// Method 3 - with component value (overload methods from 1-5 components)
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
___

#### Basic operations:
```csharp
var entity = MyEcs.Entity.New(
            new Name { Val = "SomeName" },
            new Velocity { Val = 1f },
            new Position { Val = Vector3.One }
);

entity.Disable();                        // Disable entity (a disabled entity is not found by default in queries (see Query))
entity.Enable();                         // Enable entity

bool enabled = entity.IsEnabled();       // Check if the entity is enabled in the world
bool disabled = entity.IsDisabled();     // Check if the entity is disabled in the world

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