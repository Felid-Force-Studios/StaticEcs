---
title: Version update
parent: EN
nav_order: 5
---

#### Update versions from 1.0.x to 1.1.x

In version 1.1.0, the following breaking changes:
 - Renamed query method from `QueryComponents` to `Query`
 - Removed `IMask` components as they are no longer meaningful and it is required to replace them all with `ITag` Components when raising the version
 - Removed `IStandardComponent` components, they are now meaningless and it is required to replace them all with regular `IComponent` components when raising the version
   - If the behavior of standard components is required (To have the component present on all entities) you can use `OnEntityCreate` [entity section](features/entity.md)
 - The `RoEntity` type has been removed, parallel queries now return `Entity` instead of `RoEntity`
 - The `WithAdds` type has been removed, the standard `With` must be used
 - The `TryDeleteTag` method has been removed, the `SetTag` and `DeleteTag` operations are now safe and return `bool`. For more details in [tag section](features/tag.md)
 - Entity no longer implements the `IEntity` interface. If you need the interface, use `entity.Box()`. For more details, see [entity section](features/entity.md)
 - `Query` when passing a custom value with a parameter without `ref` still expects the `ref` modifier inside the function (W.Query.For(Time.deltaTime, (ref float dt, ent) => ...))
- Removed `WithLink`, `WithLinksAll`, `WithLinksAny` query methods for relations

#### Update versions from 1.1.x to 1.2.x

In version 1.1.0, the following breaking changes:
- Renamed query method `QueryEntities.For()` -> `Query.Entities()`
- Renamed entity methods for raw types:
    - `entity.HasAllOf(Type componentType)` -> `entity.RawHasAllOf(Type componentType)`
    - `entity.Add(Type componentType)` -> `entity.RawAdd(Type componentType)`
    - `entity.TryAdd(Type componentType)` -> `entity.RawTryAdd(Type componentType)`
    - `entity.TryAdd(Type componentType, out bool added)` -> `entity.RawTryAdd(Type componentType, out bool added)`
    - `entity.GetRaw(Type componentType)` -> `entity.RawGet(Type componentType)`
    - `entity.PutRaw(IComponent component)` -> `entity.RawPut(IComponent component)`
    - `entity.TryDelete(Type componentType)` -> `entity.RawTryDelete(Type componentType)`
    - `entity.Delete(Type componentType)` -> `entity.RawDelete(Type componentType)`
    - `entity.CopyComponentsTo(Type componentType, Entity target)` -> `entity.RawCopyComponentsTo(Type componentType, Entity target)`
    - `entity.MoveComponentsTo(Type componentType, Entity target)` -> `entity.RawMoveComponentsTo(Type componentType, Entity target)`
    - `entity.HasAllOfTags(Type tagType)` -> `entity.RawHasAllOfTags(Type tagType)`
    - `entity.SetTag(Type tagType)` -> `entity.RawSetTag(Type tagType)`
    - `entity.DeleteTag(Type tagType)` -> `entity.RawDeleteTag(Type tagType)`
    - `entity.MoveTagsTo(Type tagType, Entity target)` -> `entity.RawMoveTagsTo(Type tagType, Entity target)`
- Renamed interface methods `IQueryFunction`:
    - `Run(...)` -> `Invoke(...)`
- `EntityGID`:
    - The size has changed from 4 bytes to 8 bytes.
    - Method `Id()` replaced by the field `Id`
    - Method `Version()` replaced by the field `Version`
    - Method `Raw()` replaced by the field `Raw`
    - Method `IsRegistered()` replaced by the method `IsActual()`
    - Entity version: type changed `byte` -> `ushort`
- Removed type `GIDStore`
- Method removed `World.Clear()`
- World configuration setting removed `WorldConfig.baseEntitiesCapacity` -> this parameter accepts the method `World.Initialize(baseEntitiesCapacity)`
- `Entity`:
    - `default(W.Entity)` can never be a actual entity
    - Method `entity.IsActual()` replaced by the method `entity.IsNotDestroyed()`
