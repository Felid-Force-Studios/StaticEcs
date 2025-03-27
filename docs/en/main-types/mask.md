---
title: Mask
parent: Main types
nav_order: 7
---

## Mask
Mask - similar to a tag, but uses only 1 bit of memory
- NOT Gives the option to build queries by masks only, can only be used as an additional search criterion
- Represented as a user structure without data with a marker interface `IMask`

#### Example:
```c#
public struct Visible : IMask { }
```
___

{: .important }
Requires registration in the world between creation and initialization

```c#
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.World.RegisterMaskType<Visible>();
//...
MyEcs.Initialize();
```
___

#### Creation:
```c#
// Adding a mask to an entity (overload methods from 1-5 masks)
entity.SetMask<Flammable, Frozen, Visible>();
```
___

#### Basic operations:
```c#
// Get the count of masks on an entity
int masksCount = entity.MasksCount();

// Check for the presence of ALL specified masks (overload methods from 1-3 masks)
entity.HasAllOfMasks<Flammable>();
entity.HasAllOfMasks<Flammable, Frozen, Visible>();

// Check for the presence of at least one specified mask (overload methods from 2-3 masks)
entity.HasAnyOfMasks<Flammable, Frozen, Visible>();

// Remove mask from entity (In DEBUG mode there will be an error if the entity is not present, 
// in release you cannot use if there is no guarantee that the mask is present) (overload methods from 1-5 masks)
entity.DeleteMask<Frozen>();

// Remove a mask from an entity if it exists (overload methods from 1-5 masks)
var deleted = entity.TryDeleteMask<Frozen>();
```