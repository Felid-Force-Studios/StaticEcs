---
title: Standard component
parent: Main types
nav_order: 4
---

## StandardComponent
Default Component - standard entity properties, present on every entity created by default
- Optimized storage and direct access to data by entity ID
- Cannot be deleted, only modified
- Not included in queries, as it is present on all entities
- Represented as a custom structure with a `IStandardComponent` marker interface

{: .note }
> Should be used when ALL entities in the world must contain components of some type  
> for example, if the position component must be on every entity without exception, you should use the standard component  
> as the access times are faster, and there will be no additional memory overhead
>
> Can also be used for small components 1-8 bytes in size, if no logic is required based on the presence or absence of a component
>
> For example, the internal version of an entity `entity.Version()` is a standard component

#### Example:
```c#
public struct EntityType : IStandardComponent {
    public int Val;
}
```
___

{: .important }
Requires registration in the world between creation and initialization

```c#
World.Create(WorldConfig.Default());
//...
World.RegisterStandardComponentType<EntityType>();
//...
World.Initialize();
```
___

{: .important } 
If automatic initialization when creating an entity or automatic reset when deleting an entity is required  
handlers must be explicitly registered

#### Example:
```c#
public struct EntityType : IStandardComponent {
    public int Val;
    
    public void Init() {
        Val = -1;
    }
    
    public void Reset() {
        Val = -1;
    }
    
    public void CopyTo(ref EntityType dst) {
        dst.Val = Val;
    }
}

World.Create(WorldConfig.Default());
//...

World.RegisterStandardComponentType<EntityType>(
                onAdd: static (ref EntityType component) => component.Init(), // This function will be called when the entity is created  
                onDelete: static (ref EntityType component) => component.Reset(), // This function will be called when the entity is destroyed  
                onCopy: static (ref EntityType src, ref EntityType dst) => src.CopyTo(ref dst), // When copying standard components, this entity will be called instead of just copying it
            );
//...
World.Initialize();
```
___

#### Basic operations:
```c#
// Get the number of standard components per entity
int standardComponentsCount = entity.StandardComponentsCount();

// Get ref reference to a standard read/write component
ref var entityType = ref entity.RefStandard<EntityType>();
entityType.Val = 123;

var entity2 = World.Entity.New<SomeComponent>();
// Copy the specified standard components to another entity (overload methods from 1-5 components)
entity.CopyStandardComponentsTo<EntityType>(entity2);
```