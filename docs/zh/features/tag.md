---
title: 标签
parent: 功能
nav_order: 6
---

## Tag
标签类似于组件，但不包含任何数据 — 用作实体上的布尔标记
- 内部与组件统一 — 存储在 `Components<T>` 中并带有 `IsTag` 标志，共享相同的存储基础设施
- 纯粹以位掩码形式存储 — 没有数据数组，内存开销极小
- 不会减慢组件查询速度，可以创建大量标签
- 没有钩子（`OnAdd`/`OnDelete`），没有 enable/disable — 标签要么存在，要么不存在
- 非常适合状态标记（`IsPlayer`、`IsDead`、`NeedsUpdate`）、查询过滤以及任何布尔属性
- 以空的用户自定义结构体表示，带有 `ITag` 标记接口
- 使用与组件相同的查询过滤器（`All<>`、`None<>`、`Any<>`）— 没有单独的标签过滤器类型

#### 示例:
```csharp
public struct Unit : ITag { }
public struct Player : ITag { }
public struct IsDead : ITag { }
```

___

{: .importantzh }
需要在世界创建和初始化之间进行注册

```csharp
W.Create(WorldConfig.Default());
//...
W.Types()
    .Tag<Unit>()
    .Tag<Player>()
    .Tag<IsDead>();
//...
W.Initialize();
```

{: .notezh }
标签自动获得由类型名称计算的稳定 GUID。要覆盖（例如，为了在重命名类型时保持稳定），可以手动传递，也可以在标签结构体内声明一个静态 `Guid` 字段/属性 — `RegisterAll()` 会自动发现它（优先选择名为 `Guid` 的成员）：

```csharp
// 手动注册并覆盖 GUID
W.Types().Tag<Poisoned>(new TagTypeConfig<Poisoned>(guid: new Guid("A1B2C3D4-...")));

// 或声明静态字段 — RegisterAll() 会自动获取
public struct Poisoned : ITag {
    public static readonly Guid Guid = new("A1B2C3D4-...");
}
```

{: .notezh }
要启用标签的变更追踪，请使用带 `trackAdded` / `trackDeleted` 参数的 `TagTypeConfig<T>`（详见[变更追踪](tracking)）：

```csharp
W.Types().Tag<Unit>(new TagTypeConfig<Unit>(
    trackAdded: true,    // 启用添加追踪（默认 — false）
    trackDeleted: true   // 启用删除追踪（默认 — false）
));

// 带 GUID 和追踪的完整配置
W.Types().Tag<Poisoned>(new TagTypeConfig<Poisoned>(
    guid: new Guid("A1B2C3D4-..."),
    trackAdded: true,
    trackDeleted: true
));
```

___

#### 设置标签:
```csharp
// 为实体添加标签（1 到 5 个标签的重载）
// 如果标签不存在则添加并返回 true，如果已存在则返回 false
bool added = entity.Set<Unit>();

// 一次添加多个标签
entity.Set<Unit, Player>();
entity.Set<Unit, Player, IsDead>();
// 还有 4 个和 5 个标签的重载
```

___

#### 基本操作:
```csharp
// 获取实体上的标签数量
int tagsCount = entity.TagsCount();

// 检查标签是否存在（1 到 3 个标签的重载 — 检查是否全部存在）
bool hasUnit = entity.Has<Unit>();
bool hasBoth = entity.Has<Unit, Player>();
bool hasAll3 = entity.Has<Unit, Player, IsDead>();

// 检查是否至少存在一个指定的标签（2 到 3 个标签的重载）
bool hasAny = entity.HasAny<Unit, Player>();
bool hasAny3 = entity.HasAny<Unit, Player, IsDead>();

// 从实体中删除标签（1 到 5 个标签的重载）
// 如果标签存在则删除并返回 true，如果不存在则返回 false
// 即使标签不存在也可以安全使用
bool deleted = entity.Delete<Unit>();
entity.Delete<Unit, Player>();

// 切换标签：不存在则添加，存在则删除（1 到 3 个标签的重载）
// 返回 true 表示标签已添加，false 表示已删除
bool state = entity.Toggle<Unit>();
entity.Toggle<Unit, Player>();

// 根据布尔值条件设置或删除标签（1 到 3 个标签的重载）
// true — 设置标签，false — 删除标签
entity.Apply<Unit>(true);
entity.Apply<Unit, Player>(false, true); // Unit 被删除，Player 被设置
```

___

#### 复制和移动:
```csharp
var source = W.Entity.New<Position>();
source.Set<Unit, Player>();

var target = W.Entity.New<Position>();

// 将指定标签复制到另一个实体（1 到 5 个标签的重载）
// 源实体保留其标签
// 对于单个标签，如果源实体拥有该标签并已复制则返回 true
bool copied = source.CopyTo<Unit>(target);
source.CopyTo<Unit, Player>(target);

// 将指定标签移动到另一个实体（1 到 5 个标签的重载）
// 标签被添加到目标实体并从源实体删除
// 对于单个标签，如果标签已移动则返回 true
bool moved = source.MoveTo<Unit>(target);
source.MoveTo<Unit, Player>(target);
```

___

#### 查询过滤器:

标签使用与组件相同的查询过滤器：`All<>`、`None<>`、`Any<>` 及其变体。详情请参阅[查询](query.md)章节。

___

#### 调试:
```csharp
// 将实体的所有标签收集到列表中（用于检查器/调试）
// 列表会在填充前被清除
var tags = new List<ITag>();
entity.GetAllTags(tags);
```
