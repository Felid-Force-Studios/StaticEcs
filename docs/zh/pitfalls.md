---
title: 常见陷阱
parent: ZH
nav_order: 4
---

# 常见陷阱

使用 StaticEcs 时的常见错误列表。对开发者和 AI 编程助手都有用。

___

## 生命周期错误

### 忘记注册类型
所有组件、标签、事件、链接和多组件类型都必须在 `W.Create()` 和 `W.Initialize()` 之间注册。使用未注册的类型会导致运行时错误。
```csharp
// 错误：组件未注册
W.Create(WorldConfig.Default());
W.Initialize();
var e = W.NewEntity<Position>(0); // RuntimeError!

// 正确 — 手动注册
W.Create(WorldConfig.Default());
W.Types().Component<Position>();
W.Initialize();
var e = W.NewEntity<Position>(0); // OK

// 正确 — 从程序集自动注册所有类型
W.Create(WorldConfig.Default());
W.Types().RegisterAll();
W.Initialize();
```

### 多程序集项目 / Unity IL2CPP / WebGL / NativeAOT 下的 `RegisterAll()`

无参的 `W.Types().RegisterAll()` **只扫描一个程序集** —— 声明你的 `IWorldType` 标记的那个（`typeof(TWorld).Assembly`）。该方法**不**使用调用栈回溯，**也不**枚举所有已加载的程序集。这意味着：

- **在所有运行时上都安全**，包括 Unity IL2CPP、Unity WebGL 和 NativeAOT —— 这些平台上 `Assembly.GetCallingAssembly` 返回不可靠的结果。
- **会遗漏位于其他程序集的 ECS 类型。** 一个常见错误是：`TWorld` 标记结构体放在 "core"/"shared" 程序集中，组件放在游戏程序集中 —— 无参调用将无法注册任何组件。

```csharp
// 错误 —— MyWorld 位于 Game.Core.dll，组件位于 Game.Gameplay.dll。
// 仅扫描 Game.Core.dll，因此不会注册任何组件。
W.Types().RegisterAll();

// 正确 —— 列出所有包含 ECS 类型的程序集。
W.Types().RegisterAll(
    typeof(MyWorld).Assembly,
    typeof(Position).Assembly,
    typeof(AiPlugin).Assembly
);
```

如果拿不准，请将 `TWorld` 标记放在与组件相同的程序集中，并使用无参形式。

### 在 Initialize 之前操作实体
`NewEntity`、查询和所有实体操作只有在 `W.Initialize()` 之后才能使用。在 `Created` 阶段（Create 和 Initialize 之间）调用将会失败。

### 重复调用 Create
在没有先调用 `W.Destroy()` 的情况下调用 `W.Create()` 是错误的。世界必须在重新创建之前销毁。

___

## Entity 句柄错误

### 在 Destroy 之后使用 Entity
`Entity` 是一个 4 字节的 uint 槽位句柄，没有代数计数器。调用 `Destroy()` 后，槽位立即可供重用。旧句柄现在悄悄指向一个完全不同的实体。
```csharp
var entity = W.NewEntity<Position>(0);
entity.Destroy();
// entity 现在无效——任何使用都是未定义行为
entity.Ref<Position>(); // 危险：可能访问另一个实体的数据
```

### 跨帧存储 Entity
由于 Entity 没有代数计数器，它无法检测过期。永远不要将 `Entity` 存储在字段、列表或其他持久结构中。请使用 `EntityGID`。
```csharp
// 错误
class MySystem { Entity targetEntity; } // 目标销毁后会过期

// 正确
class MySystem { EntityGID targetGid; } // 安全——版本检查能检测过期
// 用法：
if (targetGid.TryUnpack<WT>(out var entity)) {
    // entity 有效且存活
}
```

### 用 Entity 比较身份
`Entity` 的相等性仅基于 IdWithOffset (uint)。在不同时间创建的两个实体如果使用同一个槽位，它们的 Entity 值相同。请使用 `EntityGID` 进行身份比较。

___

## 组件错误

### Add 与 Set 语义
`Add<T>()` 不带值是**幂等的**——如果组件已存在，返回现有数据的 ref，不调用任何钩子。这不是覆写。

`Set(value)` **总是覆写**——对旧值调用 OnDelete，覆写数据，对新值调用 OnAdd。

```csharp
entity.Set(new Position { Value = Vector3.Zero }); // 设置位置
entity.Add<Position>(); // 什么都不做——返回现有 {0,0,0} 的 ref
entity.Set(new Position { Value = Vector3.One }); // 覆写：OnDelete(old) → set → OnAdd(new)
```

### 实现空钩子方法
`ComponentTypeInfo<T>` 在启动时使用反射检测已实现的钩子。如果任何钩子有非空方法体，该组件类型的所有实例都会启用钩子分发。不要实现不需要的钩子。
```csharp
// 错误：空钩子体仍然导致分发开销
public struct Foo : IComponent {
    public void OnAdd<TW>(World<TW>.Entity self) where TW : struct, IWorldType { }
}

// 正确：不需要的钩子就不要实现
public struct Foo : IComponent { }
```

### HasOnDelete vs DataLifecycle
OnDelete 钩子和 DataLifecycle（重置为 `DefaultValue`）是互斥的清理路径。如果组件有 OnDelete 钩子，钩子负责清理——数据不会被重置。DataLifecycle 重置仅适用于没有 OnDelete 的组件。当配置中设置 `noDataLifecycle: true` 时，框架不执行任何初始化或清理。

___

## 查询错误

### 违反 Strict 模式
在默认的 Strict 查询模式下，禁止在迭代期间修改其他实体上的被过滤组件/标签类型。
```csharp
// 在 Strict 模式下错误：
foreach (var e in W.Query<All<Position>>().Entities()) {
    otherEntity.Delete<Position>(); // 修改了另一个实体的被过滤类型！
}

// 正确：使用 Flexible 模式
foreach (var e in W.Query<All<Position>>().EntitiesFlexible()) {
    otherEntity.Delete<Position>(); // 在 Flexible 模式下 OK
}
```

### 并行迭代限制
在 `ForParallel` 期间，只能修改当前实体的数据。不要创建/销毁实体，不要修改其他实体。

### 不必要的 Flexible 模式
Flexible 模式在每个实体上重新检查位掩码，比 Strict 慢很多。只有在确实需要修改其他实体的被过滤类型时才使用 Flexible。

___

## 注册错误

### MultiComponent 没有 Multi 包装
`IMultiComponent` 类型必须通过 `W.Types().Multi<Item>()` 注册，而不是作为普通组件。

### 缺少序列化设置
序列化需要：
1. FFS.StaticPack 依赖
2. 所有类型自动获得 GUID。通过 `new ComponentTypeConfig<T>(guid: ...)` 覆盖以确保重命名类型时的稳定性
3. 非 unmanaged 组件需要 `Write`/`Read` 钩子实现
4. 序列化策略自动检测（unmanaged 类型使用 `UnmanagedPackArrayStrategy<T>`，其他使用 `StructPackArrayStrategy<T>`）

___

## 资源错误

### NamedResource 缓存问题
`NamedResource<T>` 在首次访问时缓存内部 box 引用。如果存储为 `readonly` 或在首次使用后按值传递，缓存副本将变得过期。
```csharp
// 错误
readonly NamedResource<Config> config = new("main"); // readonly 破坏缓存

// 正确
NamedResource<Config> config = new("main"); // 可变——缓存正常工作
```
