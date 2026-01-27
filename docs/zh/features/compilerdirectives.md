---
title: 编译器指令
parent: 功能
nav_order: 16
---

## 编译器指令

指令控制库的编译模式。通过 `.csproj` 中的 `DefineConstants` 或 Unity 项目设置定义。

___

### FFS_ECS_ENABLE_DEBUG

启用调试模式 — 世界状态检查、实体有效性、组件和标签注册正确性、查询阻塞等。

- 在 `DEBUG` 配置中**自动启用**（通过 `dotnet build` 不带 `-c Release` 构建时）
- 在 Release 配置中，所有检查被编译器完全移除 — 零性能影响

```xml
<!-- .csproj — 在任何配置中显式启用 -->
<PropertyGroup>
    <DefineConstants>FFS_ECS_ENABLE_DEBUG</DefineConstants>
</PropertyGroup>
```

{: .importantzh }
建议始终在调试模式下测试项目。调试检查可以捕获常见错误：访问已销毁的实体、未注册的组件、迭代期间的数据修改等。

#### 调试检查示例：
- 使用前世界已创建/初始化
- 实体未被销毁且已加载
- 组件/标签在使用前已注册
- 并行查询中没有数据修改
- 块/集群在操作前已注册

___

### FFS_ECS_DISABLE_DEBUG

强制禁用调试模式，即使定义了 `DEBUG` 或 `FFS_ECS_ENABLE_DEBUG`。

```xml
<!-- .csproj — 在 Debug 配置中禁用调试 -->
<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
    <DefineConstants>FFS_ECS_DISABLE_DEBUG</DefineConstants>
</PropertyGroup>
```

{: .notezh }
激活逻辑：`(DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG`。`FFS_ECS_DISABLE_DEBUG` 指令具有最高优先级。

___

### ENABLE_IL2CPP

激活 Unity IL2CPP 属性以优化 AOT 编译：
- `[Il2CppSetOption(Option.NullChecks, false)]` — 禁用空引用检查
- `[Il2CppSetOption(Option.ArrayBoundsChecks, false)]` — 禁用数组越界检查
- `[Il2CppEagerStaticClassConstruction]` — 静态类提前初始化

{: .notezh }
在为 IL2CPP 平台构建 Unity 项目时自动定义。无需手动定义。

___

### ENABLE_IL2CPP_CHECKS

即使使用 `ENABLE_IL2CPP` 也启用 NullChecks 和 ArrayBoundsChecks。默认情况下，这些检查被禁用以获得最大性能。

```xml
<!-- .csproj — 启用 IL2CPP 检查（用于设备上调试）-->
<PropertyGroup>
    <DefineConstants>ENABLE_IL2CPP_CHECKS</DefineConstants>
</PropertyGroup>
```

___

### FFS_ECS_DISABLE_CHANGED_TRACKING

在编译时禁用 Changed 追踪支持。定义后，查询构建器的 `Read` / `ReadBlock` 方法不可用。`AllChanged<T>`、`NoneChanged<T>`、`NoneAdded<T>`、`NoneDeleted<T>` 过滤器以及实体方法 `Mut<T>()` 也不可用。

```xml
<!-- .csproj — 全局禁用 Changed 追踪 -->
<PropertyGroup>
    <DefineConstants>FFS_ECS_DISABLE_CHANGED_TRACKING</DefineConstants>
</PropertyGroup>
```

{: .notezh }
如果不需要 Changed 追踪并希望消除任何相关开销，请使用此指令。Added/Deleted 追踪不受此指令影响。

___

### 汇总表

| 指令 | 用途 | 默认值 |
|------|------|--------|
| `FFS_ECS_ENABLE_DEBUG` | 启用调试检查 | 在 `DEBUG` 中启用 |
| `FFS_ECS_DISABLE_DEBUG` | 强制禁用调试 | 未定义 |
| `ENABLE_IL2CPP` | IL2CPP 优化属性 | Unity IL2CPP 中自动启用 |
| `ENABLE_IL2CPP_CHECKS` | 启用 IL2CPP 中的检查 | 未定义 |
| `FFS_ECS_DISABLE_CHANGED_TRACKING` | 禁用 Changed 追踪 | 未定义 |
