---
title: Component identifiers
parent: Additional features
nav_order: 1
---

### Component identifiers
In case when the project is very large or on the contrary small and the size of compiled code is important:  

Things like - typed Query, and sugar methods of entity handling, can increase the compiled code size due to monomorphization of generic types in structures and methods.

To avoid this, a dynamic identifier mechanism for components, tags, and masks is implemented - which allow you to use them instead of type parameters  

___

#### How it works:
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
foreach (var entity in MyWorld.QueryEntities.For(with)) {
    //..
}

// This mechanism also allows you to use these keys in the game logic
// For example, when the type of the created component changes depending on the conditions
// It is important to remember that these identifiers are dynamic, which means that there is no guarantee that they will be the same from run to run.
// so they can't be serialized or used in a similar way.
```