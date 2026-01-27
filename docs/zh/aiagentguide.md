---
title: AI 代理指南
parent: ZH
nav_order: 5
---

# AI 代理指南

如果您使用 AI 编程助手（Claude Code、Cursor、Copilot 等）配合 StaticEcs，您可以为代理提供库的上下文信息。本页包含可直接使用的代码片段。

___

## llms.txt

将您的代理指向库的 AI 可读文档：
- **简洁版**: `https://felid-force-studios.github.io/StaticEcs/llms.txt`
- **完整版**: `https://felid-force-studios.github.io/StaticEcs/llms-full.txt`

___

## CLAUDE.md 代码片段

如果您使用 [Claude Code](https://claude.ai/code)，请将以下代码块复制到您项目的 `CLAUDE.md` 文件中。这将为 Claude 提供正确使用 StaticEcs 所需的上下文。

对于其他代理，请粘贴到相应的指令文件中（`.cursorrules`、`.github/copilot-instructions.md` 等）。

````markdown
## StaticEcs ECS Framework

本项目使用 [StaticEcs](https://github.com/Felid-Force-Studios/StaticEcs) — C# 静态泛型 ECS 框架。命名空间：`FFS.Libraries.StaticEcs`。

### 设置模式
```csharp
public struct WT : IWorldType { }
public abstract class W : World<WT> { }           // 世界访问的类型别名
public struct GameSystems : ISystemsType { }
public abstract class GameSys : W.Systems<GameSystems> { }
```

### 世界生命周期（严格顺序）
1. `W.Create(WorldConfig.Default())` — 创建世界
2. `W.Types().RegisterAll()` 或手动注册 `.Component<T>().Tag<T>().Event<T>()` — 注册所有类型（必须！）
3. `W.Initialize()` — 之后实体操作可用
4. 工作：创建实体、运行系统、迭代查询
5. `W.Destroy()` — 清理

### 关键规则
- 始终在 Create() 和 Initialize() 之间注册组件/标签/事件/链接类型。使用 `W.Types().RegisterAll()` 从程序集自动注册所有类型，或手动注册。未注册的类型会导致运行时错误。
- Entity 是 4 字节 uint 句柄，不是持久引用。永远不要在字段/集合中跨帧存储 Entity。使用 EntityGID 作为持久引用。
- `Add<T>()` 不带值是幂等的（如果存在 → 返回 ref，不调用钩子）。`Set(value)` 总是覆写，触发 OnDelete→OnAdd 钩子。
- `Ref<T>()` 返回组件的 ref 引用。假设组件存在——不确定时用 `Has<T>()` 检查。
- 对于只读组件，使用 `Read<T>()`（返回 `ref readonly`）代替 `Ref<T>()`，在查询委托中使用 `in` 代替 `ref`。
- 查询过滤器类型：`All<>` (要求), `None<>` (排除), `Any<>` (至少一个)。这些过滤器同时适用于组件和标签。组合：`And<Filter1, Filter2>`（全部匹配）或 `Or<Filter1, Filter2>`（任一匹配）。
- 默认查询模式是 Strict——迭代期间不要修改其他实体上的被过滤类型。需要时使用 `EntitiesFlexible()`。
- `ForParallel` 期间只修改当前实体。禁止结构变更。

### 常见模式
```csharp
// 创建带组件的实体
var entity = W.NewEntity<Default>().Set(new Position { Value = v }, new Velocity { Value = 1f });

// 查询迭代 (foreach)
foreach (var e in W.Query<All<Position, Velocity>>().Entities()) {
    ref var pos = ref e.Ref<Position>();
    ref readonly var vel = ref e.Read<Velocity>();
    pos.Value += vel.Value;
}

// 查询迭代（委托——更快，零分配）
W.Query().For(static (ref Position p, in Velocity v) => {
    p.Value += v.Value;
});

// 持久引用
EntityGID gid = entity.GID;
if (gid.TryUnpack<WT>(out var resolved)) { /* resolved 存活 */ }

// 标签
entity.Set<IsPlayer>();
if (entity.Has<IsPlayer>()) { ... }

// 多组件（实体上的同类型值列表）
ref var items = ref entity.Add<W.Multi<Item>>();
items.Add(new Item { Id = 1 });
items.Add(new Item { Id = 2 });
foreach (ref var item in items) { item.Weight *= 2f; }

// 关系（实体链接）
entity.Set(new W.Link<Parent>(parentEntity));           // 单链接
ref var children = ref entity.Add<W.Links<Children>>(); // 多链接
children.TryAdd(childEntity.AsLink<Children>());

// 系统
public struct MoveSystem : ISystem {
    public void Init() { /* 在 Initialize 时调用一次 */ }
    public void Update() {
        W.Query().For(static (ref Position p, in Velocity v) => {
            p.Value += v.Value;
        });
    }
    public void Destroy() { /* 在 Destroy 时调用 */ }
}
GameSys.Create();
GameSys.Add(new MoveSystem(), order: 0);
GameSys.Initialize();
// 在游戏循环中：GameSys.Update();

// 资源
W.SetResource(new GameConfig { ... });
ref var config = ref W.GetResource<GameConfig>();
```

### 完整文档
- 简洁 AI 参考：https://felid-force-studios.github.io/StaticEcs/llms.txt
- 完整文档：https://felid-force-studios.github.io/StaticEcs/zh/features.html
````
