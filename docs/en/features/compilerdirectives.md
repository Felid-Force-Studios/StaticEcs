---
title: Compiler directives
parent: Features
nav_order: 16
---

### Compiler directives
> `FFS_ECS_ENABLE_DEBUG`
> Enables debug mode, by default enabled in `DEBUG`, it is recommended to always test in debug mode

> `FFS_ECS_DISABLE_DEBUG`
> Disables debug mode

> `FFS_ECS_ENABLE_DEBUG_EVENTS`
> Enables technical event functionality, enabled by default in `DEBUG`

> `FFS_ECS_DISABLE_TAGS`
> Completely removes all tag functionality from the compilation

> `FFS_ECS_LIFECYCLE_ENTITY`
> Changes the entity lifecycle management logic to automatic by making the following changes:
> - Entity cannot be created without component - World.Entity.New() method is not available, empty entities are excluded
> - When the last component of type `IComponent` is deleted, the entity is automatically deleted
>   - The tags is not taken into account

> `FFS_ECS_LARGE_WORLDS`
> Increases base component stores from 256 to 4096 items, increases performance in large worlds (larger than ~30-40 thousand entities)

