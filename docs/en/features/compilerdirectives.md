---
title: Compiler directives
parent: Features
nav_order: 16
---

## Compiler directives

Directives control the library's compilation modes. Defined via `DefineConstants` in `.csproj` or in Unity project settings.

___

### FFS_ECS_ENABLE_DEBUG

Enables debug mode — world state checks, entity validity, component and tag registration correctness, query blocking, and more.

- **Automatically enabled** in the `DEBUG` configuration (when building via `dotnet build` without `-c Release`)
- In Release configuration, all checks are completely removed by the compiler — zero performance impact

```xml
<!-- .csproj — explicitly enable for any configuration -->
<PropertyGroup>
    <DefineConstants>FFS_ECS_ENABLE_DEBUG</DefineConstants>
</PropertyGroup>
```

{: .important }
It is recommended to always test your project in debug mode. Debug checks catch common errors: accessing destroyed entities, unregistered components, data modification during iteration, etc.

#### Examples of debug checks:
- World is created/initialized before use
- Entity is not destroyed and is loaded
- Component/tag is registered before use
- No data modification from a parallel query
- Chunk/cluster is registered before operations

___

### FFS_ECS_DISABLE_DEBUG

Forcefully disables debug mode, even if `DEBUG` or `FFS_ECS_ENABLE_DEBUG` is defined.

```xml
<!-- .csproj — disable debug in Debug configuration -->
<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
    <DefineConstants>FFS_ECS_DISABLE_DEBUG</DefineConstants>
</PropertyGroup>
```

{: .note }
Activation logic: `(DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG`. The `FFS_ECS_DISABLE_DEBUG` directive has the highest priority.

___

### ENABLE_IL2CPP

Activates Unity IL2CPP attributes for AOT compilation optimization:
- `[Il2CppSetOption(Option.NullChecks, false)]` — disables null checks
- `[Il2CppSetOption(Option.ArrayBoundsChecks, false)]` — disables array bounds checks
- `[Il2CppEagerStaticClassConstruction]` — early static class initialization

{: .note }
Defined automatically when building a Unity project for IL2CPP platforms. No manual definition required.

___

### ENABLE_IL2CPP_CHECKS

Enables NullChecks and ArrayBoundsChecks for IL2CPP even when `ENABLE_IL2CPP` is used. By default, these checks are disabled for maximum performance.

```xml
<!-- .csproj — enable IL2CPP checks (for on-device debugging) -->
<PropertyGroup>
    <DefineConstants>ENABLE_IL2CPP_CHECKS</DefineConstants>
</PropertyGroup>
```

___

### FFS_ECS_DISABLE_CHANGED_TRACKING

Disables Changed tracking support at compile time. When defined, `Read` / `ReadBlock` query builder methods are not available. The `AllChanged<T>`, `NoneChanged<T>`, `NoneAdded<T>`, `NoneDeleted<T>` filters and the `Mut<T>()` entity method are also unavailable.

```xml
<!-- .csproj — disable changed tracking globally -->
<PropertyGroup>
    <DefineConstants>FFS_ECS_DISABLE_CHANGED_TRACKING</DefineConstants>
</PropertyGroup>
```

{: .note }
Use this directive if you do not need Changed tracking and want to eliminate any associated overhead. Added/Deleted tracking is not affected by this directive.

___

### Summary table

| Directive | Purpose | Default |
|-----------|---------|---------|
| `FFS_ECS_ENABLE_DEBUG` | Enables debug checks | Enabled in `DEBUG` |
| `FFS_ECS_DISABLE_DEBUG` | Forcefully disables debug | Not defined |
| `ENABLE_IL2CPP` | IL2CPP optimization attributes | Automatic in Unity IL2CPP |
| `ENABLE_IL2CPP_CHECKS` | Enables checks in IL2CPP | Not defined |
| `FFS_ECS_DISABLE_CHANGED_TRACKING` | Disables Changed tracking | Not defined |
