---
title: 资源
parent: 功能
nav_order: 11
---

## Resources
资源是 DI 的替代方案 — 一种简单的机制，用于存储和传递用户数据和服务到系统和其他方法中
- 资源是世界级单例：不属于任何特定实体的共享状态
- 适用于配置、时间/增量时间、输入状态、资产缓存、服务引用
- 两种变体：**单例**（每种类型一个）和 **命名的**（每种类型多个，通过字符串键区分）
- 在 `Created` 和 `Initialized` 世界阶段均可使用

___

## 单例资源

单例资源在每个世界中存储给定类型的唯一一个实例。
内部使用静态泛型存储 — 访问复杂度 O(1)，无字典开销。

#### 设置资源：
```csharp
// 用户类和服务
public class GameConfig { public float Gravity; }
public class InputState { public Vector2 MousePos; }

// 在世界中设置资源
// 默认 clearOnDestroy = true — 资源将在 World.Destroy() 时自动清除
W.SetResource(new GameConfig { Gravity = 9.81f });
W.SetResource(new InputState(), clearOnDestroy: false); // 在世界重建后保留

// 对同一类型再次调用 SetResource 时，值会被覆盖而不会报错
W.SetResource(new GameConfig { Gravity = 4.0f }); // 覆盖之前的值
```

{: .importantzh }
`clearOnDestroy` 参数仅在首次注册时生效。替换已有资源时保留原始的 `clearOnDestroy` 设置。

#### 基本操作：
```csharp
// 检查给定类型的资源是否已注册
bool has = W.HasResource<GameConfig>();

// 获取资源值的可变 ref 引用 — 修改直接写入存储
ref var config = ref W.GetResource<GameConfig>();
config.Gravity = 11.0f; // 就地修改，无需调用 setter

// 从世界中移除资源
W.RemoveResource<GameConfig>();

// Resource<T> — 零开销的 readonly 结构体句柄，用于频繁访问（无需初始化）
W.Resource<GameConfig> configHandle;
bool registered = configHandle.IsRegistered;
ref var cfg = ref configHandle.Value;
```

___

## 命名资源

命名资源允许存储同一类型的多个实例，通过字符串键区分。
内部存储在 `Dictionary<string, object>` 中，使用类型安全的 `Box<T>` 包装器。

#### 设置命名资源：
```csharp
// 使用不同的键设置相同类型的命名资源
W.SetResource("player_config", new GameConfig { Gravity = 9.81f });
W.SetResource("moon_config", new GameConfig { Gravity = 1.62f });

// 对已有键再次调用 SetResource 时，值会被覆盖而不会报错
W.SetResource("player_config", new GameConfig { Gravity = 10.0f }); // 覆盖
```

#### 基本操作：
```csharp
// 检查给定键的命名资源是否存在
bool has = W.HasResource<GameConfig>("player_config");

// 获取命名资源值的可变 ref 引用
ref var config = ref W.GetResource<GameConfig>("player_config");
config.Gravity = 5.0f;

// 通过键移除命名资源
W.RemoveResource("player_config");

// NamedResource<T> — 结构体句柄，在首次访问后缓存内部引用
// 创建绑定到键的句柄（不会注册资源）
var moonConfig = new W.NamedResource<GameConfig>("moon_config");
bool registered = moonConfig.IsRegistered;  // 始终执行字典查找，不使用缓存
ref var cfg = ref moonConfig.Value;          // 首次调用从字典解析并缓存；后续调用为 O(1)
// 当资源被移除或世界被销毁时，缓存会自动失效
```

{: .warningzh }
`NamedResource<T>` 是一个可变结构体，在首次访问 `Value` 时缓存内部引用。
**不要**将其存储在 `readonly` 字段中，也不要在首次使用后按值传递 — C# 编译器
会创建防御性副本，丢弃缓存，导致每次访问都执行字典查找。
请存储在非 readonly 字段或局部变量中。

___

## 生命周期

```csharp
W.Create(WorldConfig.Default());

// Create 之后即可设置资源（无需等待 Initialize）
W.SetResource(new GameConfig { Gravity = 9.81f });
W.SetResource("debug_flags", new DebugFlags(), clearOnDestroy: false);

W.Initialize();

// 在 Initialized 阶段资源仍然可用
ref var config = ref W.GetResource<GameConfig>();

// Destroy 时：clearOnDestroy=true 的资源自动清除
// clearOnDestroy=false 的资源保留，在下一个 Create+Initialize 周期后仍可使用
W.Destroy();
```

