---
title: Compiler directives
parent: Additional features
nav_order: 5
---

### Compiler directives
> `FFS_ECS_ENABLE_DEBUG`
> Enables debug mode, by default enabled in `DEBUG`

> `FFS_ECS_ENABLE_DEBUG_EVENTS`
> Enables technical event functionality, enabled by default in `DEBUG`

> `FFS_ECS_DISABLE_TAGS`
> Completely removes all tag functionality from the compilation

> `FFS_ECS_DISABLE_MASKS`
> Completely removes all mask functionality from the compilation

> `FFS_ECS_LIFECYCLE_ENTITY`
> Changes the entity lifecycle management logic to automatic by making the following changes:
> - Entity cannot be created without component - MyEcs.Entity.New() method is not available, empty entities are excluded
> - When the last component of type `IComponent` is deleted, the entity is automatically deleted
>   - The standard component is not taken into account
>   - The tags is not taken into account
>   - The masks is not taken into account