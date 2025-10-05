---
title: Entity
parent: Features
nav_order: 1
---

## Entity
Entity - serves to identify an object in the game world and access attached components
- Represented as a 4 byte structure

___

{: .important }
> By default an entity can be created and exist without components, also when the last component is deleted it is not deleted  
> If you want to override this, you must use the compiler directive `FFS_ECS_LIFECYCLE_ENTITY`  
> More info: [Compiler directives](compilerdirectives.md)

___

#### Creation:
```csharp
// Creating a single entity

// Method 1 - creating an “empty” entity
W.Entity entity = W.Entity.New();

// Method 2 - with component type (overload methods from 1-5 components)
W.Entity entity = W.Entity.New<Position>();
W.Entity entity = W.Entity.New<Position, Velocity, Name>();

// Method 3 - with component value (overload methods from 1-8 components)
W.Entity entity = W.Entity.New(new Position(x: 1, y: 1, z: 2));
W.Entity entity = W.Entity.New(
            new Name { Val = "SomeName" },
            new Velocity { Val = 1f },
            new Position { Val = Vector3.One }
);

// Creating multiple entities
// Method 1 - with component type (overload methods from 1-5 components)
uint count = 100;
W.Entity.NewOnes<Position>(count);

// Method 2 - specifying component type (overload methods from 1-5 components) + delegate initialization of each entity
uint count = 100;
W.Entity.NewOnes<Position>(count, static entity => {
    // some init logic for each entity
});

// Method 3 - with component value (overload methods from 1-5 components)
uint count = 100;
W.Entity.NewOnes(count, new Position(x: 1, y: 1, z: 2));

// Method 4 - with component value (overload methods from 1-5 components) + initialization delegate of each entity
uint count = 100;
W.Entity.NewOnes(count, new Position(x: 1, y: 1, z: 2), static entity => {
    // some init logic for each entity
});

```
___

#### Basic operations:
```csharp
W.Entity entity = W.Entity.New(
            new Name { Val = "SomeName" },
            new Velocity { Val = 1f },
            new Position { Val = Vector3.One }
);

entity.Disable();                              // Disable entity (a disabled entity is not found by default in queries (see Query))
entity.Enable();                               // Enable entity

bool enabled = entity.IsEnabled();             // Check if the entity is enabled in the world
bool disabled = entity.IsDisabled();           // Check if the entity is disabled in the world

bool actual = entity.IsActual();               // Check if an entity has been deleted in the world
short version = entity.Version();              // Get entity version
W.Entity clone = entity.Clone();               // Clone the entity and all components, tags
entity.Destroy();                              // Delete the entity and all components, tags

W.Entity entity2 = W.Entity.New<Name>();
clone.CopyTo(entity2);                         // Copy all components, tags to the specified entity

W.Entity entity3 = W.Entity.New<Name>();
entity2.MoveTo(entity3);                       // Move all components to the specified entity and delete the current entity

EntityGID gid = entity3.Gid();                 // Get global entity identifier

var str = entity3.ToPrettyString();            // Get a string with all information about the entity

BoxedEntity<WorldType> boxed = entity3.Box();  // Get an entity class that implements IEntity

```
___

#### Additionally:

When initializing a world, you can pass a function that will be executed when any new entity is created

Example:

```csharp
W.Create(WorldConfig.Default());
//...
W.OnCreateEntity(entity => entity.Add<Position, Rotation, Scale>());
//...
W.Initialize();
```