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
public struct EnitiyType : IStandardComponent {
    public int Val;
}
```
___

{: .important }
Requires registration in the world between creation and initialization

```c#
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.World.RegisterStandardComponentType<EnitiyType>();
//...
MyEcs.Initialize();
```
___

{: .important } 
If automatic initialization when creating an entity or automatic reset when deleting an entity is required  
handlers must be explicitly registered

#### Example:
```c#
public struct EnitiyType : IStandardComponent {
    public int Val;
    
    public void Init() {
        Val = -1;
    }
    
    public void Reset() {
        Val = -1;
    }
    
    public void CopyTo(ref EnitiyType dst) {
        dst.Val = Val;
    }
}

MyEcs.Create(EcsConfig.Default());
//...

MyEcs.World.RegisterStandardComponentType<EnitiyType>(
                autoInit: static (ref EnitiyType component) => component.Init(), // This function will be called when the entity is created  
                autoReset: static (ref EnitiyType component) => component.Reset(), // This function will be called when the entity is destroyed  
                autoCopy: static (ref EnitiyType src, ref EnitiyType dst) => src.CopyTo(ref dst), // When copying standard components, this entity will be called instead of just copying it
            );
//...
MyEcs.Initialize();
```
___

#### Basic operations:
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