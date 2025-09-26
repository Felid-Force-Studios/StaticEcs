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
