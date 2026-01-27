---
title: 查询
parent: 功能
nav_order: 12
---

# Query
查询是在世界中搜索实体及其组件的机制
- 所有查询无需缓存，在栈上分配，可以即时使用
- 支持按组件、标签、实体状态和集群过滤
- 两种迭代模式：`Strict`（默认，更快）和 `Flexible`（允许修改其他实体上的过滤类型）

___

## 过滤器

用于描述过滤的类型。每个占 1 字节且无需初始化。

```csharp
// 假设世界中有 5 个实体：
//             Components                 Tags           EntityType
// Entity 1:  Position, Velocity         Unit           Npc
// Entity 2:  Position, Name             Player         Npc
// Entity 3:  Position, Velocity, Name   Unit, Player   Npc
// Entity 4:  Velocity                   —              Bullet
// Entity 5:  Position■, Velocity        Unit           Bullet
//            (■ = disabled)
//
// 以下示例展示每个过滤器匹配哪些实体
```

### 组件:
```csharp
// All — 所有已启用的组件都存在（1 到 8 个类型）
All<Position, Velocity, Direction> all = default;

// AllOnlyDisabled — 所有已禁用的组件都存在
AllOnlyDisabled<Position> disabled = default;

// AllWithDisabled — 所有组件都存在（任何状态）
AllWithDisabled<Position, Velocity> any = default;

// None — 已启用的组件不存在（1 到 8 个类型）
None<Position, Name> none = default;

// NoneWithDisabled — 组件不存在（任何状态）
NoneWithDisabled<Position> noneAll = default;

// Any — 至少一个已启用的组件存在（2 到 8 个类型）
Any<Position, Velocity> any = default;

// AnyOnlyDisabled — 至少一个已禁用的
AnyOnlyDisabled<Position, Velocity> anyDis = default;

// AnyWithDisabled — 至少一个（任何状态）
AnyWithDisabled<Position, Velocity> anyAll = default;

// 以上实体的结果：
// All<Position, Velocity>              → 1, 3
// AllOnlyDisabled<Position>            → 5
// AllWithDisabled<Position, Velocity>  → 1, 3, 5
// None<Name>                           → 1, 4, 5
// NoneWithDisabled<Position>           → 4
// Any<Position, Name>                  → 1, 2, 3
// AnyOnlyDisabled<Position, Velocity>  → 5
// AnyWithDisabled<Position, Name>      → 1, 2, 3, 5
```

### 标签:

标签使用与组件相同的过滤器 — `All<>`、`None<>`、`Any<>` 及其变体。没有单独的标签过滤器类型。

```csharp
// All — 所有指定的标签都存在（1 到 8 个类型）
All<Unit, Player> tagAll = default;

// None — 指定的标签不存在（1 到 8 个类型）
None<Unit, Player> tagNone = default;

// Any — 至少一个指定的标签存在（2 到 8 个类型）
Any<Unit, Player> tagAny = default;

// 以上实体的结果：
// All<Unit, Player>  → 3
// None<Unit>         → 2, 4
// Any<Unit, Player>  → 1, 2, 3, 5
```

### 变更追踪:
```csharp
// AllAdded — 自上次 ClearTracking 以来所有指定组件被添加（1 到 5 个类型）
AllAdded<Position> added = default;
AllAdded<Position, Velocity> addedMulti = default;

// AnyAdded — 至少一个指定组件被添加（2 到 5 个类型）
AnyAdded<Position, Velocity> anyAdded = default;

// NoneAdded — 没有任何指定组件被添加（1 到 5 个类型）
NoneAdded<Position> noneAdded = default;

// AllDeleted — 自上次 ClearTracking 以来所有指定组件被删除（1 到 5 个类型）
AllDeleted<Position> deleted = default;

// AnyDeleted — 至少一个被删除（2 到 5 个类型）
AnyDeleted<Position, Velocity> anyDeleted = default;

// NoneDeleted — 没有任何被删除（1 到 5 个类型）
NoneDeleted<Position> noneDeleted = default;

// AllChanged — 自上次 ClearChangedTracking 以来所有指定组件被更改（1 到 5 个类型）
// 需要 ComponentTypeConfig.TrackChanged = true
AllChanged<Position> changed = default;

// AnyChanged — 至少一个被更改（2 到 5 个类型）
AnyChanged<Position, Velocity> anyChanged = default;

// NoneChanged — 没有任何被更改（1 到 5 个类型）
NoneChanged<Position> noneChanged = default;

// AllAdded / AnyAdded / NoneAdded / AllDeleted / AnyDeleted / NoneDeleted
// 同样适用于标签 — 对组件和标签使用相同的过滤器

// Created — 自上次 ClearCreatedTracking 以来实体被创建
// （需要 WorldConfig.TrackCreated = true，无类型参数）
Created created = default;

// 与其他过滤器组合
foreach (var entity in W.Query<AllAdded<Position>, All<Velocity, Unit>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
    // 处理新添加 Position 的实体
}
```

{: .notezh }
在基于委托的迭代（`For`）中，`ref` 参数将组件标记为 Changed，而 `in` 参数不标记。使用 `in` 进行只读访问以避免不必要的 Changed 标记。详见 [Changed Tracking](tracking#changed-tracking)。

### 实体类型:
```csharp
// EntityIs — 精确此实体类型（1 个类型参数）
EntityIs<Bullet> entityIs = default;

// EntityIsNot — 排除实体类型（从 1 到 5 个类型）
EntityIsNot<Effect> entityIsNot = default;

// EntityIsAny — 匹配指定实体类型中的任何一个（从 2 到 5 个类型）
EntityIsAny<Bullet, Rocket> entityIsAny = default;

// 以上实体的结果：
// EntityIs<Npc>              → 1, 2, 3
// EntityIsNot<Bullet>        → 1, 2, 3
// EntityIsAny<Npc, Bullet>   → 1, 2, 3, 4, 5
```

### And / Or — 复合过滤器:

`And` 和 `Or` 允许将多个过滤器组合为一个类型。适用场景：
- **将复合过滤器作为一个泛型参数传递** — 存储在字段中、传递给方法、用作类型参数
- **构建基本类型无法表达的过滤器** — 例如，"拥有组件集 A **或** 组件集 B 的实体"

#### And — 所有条件必须匹配（2 到 6 个过滤器）：
```csharp
And<All<Position, Velocity>, None<Name>, Any<Unit, Player>> filter = default;

// 通过工厂方法（类型推断）
var filter = And.By(
    default(All<Position, Velocity>),
    default(None<Name>),
    default(Any<Unit, Player>)
);

// 用例：将复合过滤器传递给辅助方法
void ProcessMovable(And<All<Position, Velocity>, None<Frozen>> filter) {
    foreach (var entity in W.Query(filter).Entities()) {
        entity.Ref<Position>().Value += entity.Read<Velocity>().Value;
    }
}
```

#### Or — 至少一个条件必须匹配（2 到 6 个过滤器）：

`Or` 可以构建基本过滤器类型无法表达的组合式复杂过滤。

```csharp
// 近战战士或远程战士 — 完全不同的组件集，
// 无法用单个 All/Any/None 组合表达
Or<All<MeleeWeapon, Damage>, All<RangedWeapon, Ammo>> fighters = default;

// 当 Position 被添加、删除或修改时重建空间索引
Or<AllAdded<Position>, AllDeleted<Position>, AllChanged<Position>> spatialChanged = default;

// 处理 UI 按钮 (ClickArea + Label) 和世界交互对象 (Collider + Interaction)
Or<All<ClickArea, Label>, All<Collider, Interaction>> clickable = default;

// 通过工厂方法
var filter = Or.By(
    default(All<MeleeWeapon, Damage>),
    default(All<RangedWeapon, Ammo>)
);

// 以上实体的结果：
// Or<All<Position, Velocity>, All<Position, Name>>
// Entity 1: Pos✓ Vel✓         → ✓ (通过第一个)
// Entity 2: Pos✓ Name✓        → ✓ (通过第二个)
// Entity 3: Pos✓ Vel✓ Name✓   → ✓ (通过两个)
// Entity 4: Pos✗              → ✗
// → 结果：1, 2, 3, 5
```

#### 嵌套：
```csharp
// And 和 Or 可以嵌套以实现任意复杂的逻辑
// (A 和 B 和 C) 或 (A 和 B 和 D):
Or<All<A, B, C>, All<A, B, D>> complex = default;

// 所有可见实体中，活着的单位或活跃的效果：
And<All<Visible>, Or<All<Unit, Alive>, All<Effect, Active>>> visibleAlive = default;
```

___

## 实体迭代

```csharp
// 遍历所有实体（无过滤）
foreach (var entity in W.Query().Entities()) {
    Console.WriteLine(entity.PrettyString);
}

// 通过 generic 过滤（1 到 8 个过滤器）
foreach (var entity in W.Query<All<Position, Velocity>>().Entities()) {
    entity.Ref<Position>().Value += entity.Read<Velocity>().Value;
}

// 多个过滤器
foreach (var entity in W.Query<All<Position, Velocity>, None<Name>>().Entities()) {
    entity.Ref<Position>().Value += entity.Read<Velocity>().Value;
}

// 通过过滤器值
var all = default(All<Position, Velocity>);
foreach (var entity in W.Query(all).Entities()) {
    entity.Ref<Position>().Value += entity.Read<Velocity>().Value;
}

// 通过 And/Or — 将过滤器分组为一个类型，用于传递给方法或存储在字段中
var filter = default(And<All<Position, Velocity>, None<Name>>);
foreach (var entity in W.Query(filter).Entities()) {
    entity.Ref<Position>().Value += entity.Read<Velocity>().Value;
}

// Flexible 模式 — 允许修改其他实体上的过滤类型
foreach (var entity in W.Query<All<Position>>().EntitiesFlexible()) {
    // ...
}

// 查找第一个匹配的实体
if (W.Query<All<Position>>().Any(out var found)) {
    // found — 第一个具有 Position 的实体
}

// 获取唯一实体（debug 模式下如果找到多个则报错）
if (W.Query<All<Position>>().One(out var single)) {
    // single — 唯一具有 Position 的实体
}

// 统计匹配的实体数量（完整遍历）
int count = W.Query<All<Position>>().EntitiesCount();
```

___

## 基于委托的迭代 (For)

通过委托优化迭代 — 底层展开循环。

```csharp
// 遍历所有实体
W.Query().For(entity => {
    Console.WriteLine(entity.PrettyString);
});

// 按组件迭代（1 到 6 个类型）
// 委托中的组件自动作为 All 过滤器
W.Query().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});

// 在委托中包含实体
W.Query().For(static (W.Entity entity, ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});

// 使用用户数据（避免委托分配）
W.Query().For(deltaTime, static (ref float dt, ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value * dt;
});

// 使用 ref 数据（用于累积结果）
int count = 0;
W.Query().For(ref count, static (ref int counter, W.Entity entity, ref Position pos) => {
    counter++;
});

// 使用多参数元组
W.Query().For((deltaTime, gravity), static (ref (float dt, float g) data, ref Position pos, ref Velocity vel) => {
    vel.Value += data.g * data.dt;
    pos.Value += vel.Value * data.dt;
});
```

### 只读组件 (Read):

当组件只被读取而不被修改时，在委托中使用 `in` 代替 `ref`。这告诉变更追踪系统不要将组件标记为已更改。

```csharp
// 最后 N 个组件通过 `in` 设为只读
W.Query().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;  // Position — 可写 (ref)，Velocity — 只读 (in)
});

// 所有组件只读
W.Query().For(static (in Position pos, in Velocity vel) => {
    Console.WriteLine(pos.Value + vel.Value);
});

// 带实体
W.Query().For(static (W.Entity entity, ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});

// 带用户数据
W.Query().For(ref result, static (ref float res, in Position pos, in Velocity vel) => {
    res += pos.Value.Length;
});
```

{: .notezh }
Read 变体在启用变更追踪时可用（默认启用）。可通过 `FFS_ECS_DISABLE_CHANGED_TRACKING` 定义禁用。

### 附加过滤:
```csharp
// 委托中的组件作为 All 过滤器，
// 附加过滤器直接在 Query 上指定，无需指定委托中的组件
W.Query<Any<Unit, Player>>().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});

// 多个过滤器
W.Query<None<Name>, Any<Unit, Player>>().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});

// 通过值
var filter = default(Any<Unit, Player>);
W.Query(filter).For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});
```

### 实体和组件状态:
```csharp
W.Query().For(
    static (ref Position pos, ref Velocity vel) => {
        // ...
    },
    entities: EntityStatusType.Disabled,    // Enabled（默认）、Disabled、Any
    components: ComponentStatus.Disabled    // Enabled（默认）、Disabled、Any
);
```

___

## 单实体搜索 (Search)

首次匹配时提前退出的迭代。搜索委托中的所有组件均为只读（`in`）。

```csharp
if (W.Query().Search(out W.Entity found,
    (W.Entity entity, in Position pos, in Health health) => {
        return pos.Value.x > 100 && health.Current < 50;
    })) {
    // found — 第一个满足条件的实体
}
```

___

## 函数结构体 (IQuery / IQueryBlock)

用函数结构体代替委托 — 用于优化、传递状态或提取逻辑。
函数结构体使用 `WorldQuery` 上的 **fluent builder API** — 与委托不同，组件类型不通过 `For` 的泛型参数指定，而通过构建器链指定。

### IQuery — 逐实体回调:

接口层次结构使用嵌套类型控制写入/读取访问（总共 1 到 6 个组件）：
- `IQuery.Write<T0, T1>` — 所有组件可写（`ref`）
- `IQuery.Read<T0, T1>` — 所有组件只读（`in`）
- `IQuery.Write<T0>.Read<T1>` — 前面可写，后面只读

```csharp
// 全部可写 — IQuery.Write
readonly struct MoveFunction : W.IQuery.Write<Position, Velocity> {
    public void Invoke(W.Entity entity, ref Position pos, ref Velocity vel) {
        pos.Value += vel.Value;
    }
}

// Fluent API：Write<...>() 指定可写组件，然后 For<TFunction>() 执行
W.Query().Write<Position, Velocity>().For<MoveFunction>();

// 通过值
W.Query().Write<Position, Velocity>().For(new MoveFunction());

// 通过 ref（迭代后保留状态）
var func = new MoveFunction();
W.Query().Write<Position, Velocity>().For(ref func);

// 混合写入/读取 — IQuery.Write<>.Read<>
readonly struct ApplyVelocity : W.IQuery.Write<Position>.Read<Velocity> {
    public void Invoke(W.Entity entity, ref Position pos, in Velocity vel) {
        pos.Value += vel.Value;
    }
}

// 链：Write<可写>().Read<只读>().For<TFunction>()
W.Query().Write<Position>().Read<Velocity>().For<ApplyVelocity>();

// 全部只读 — IQuery.Read
readonly struct PrintPositions : W.IQuery.Read<Position, Velocity> {
    public void Invoke(W.Entity entity, in Position pos, in Velocity vel) {
        Console.WriteLine(pos.Value + vel.Value);
    }
}

W.Query().Read<Position, Velocity>().For<PrintPositions>();

// 附加过滤
W.Query<None<Name>, Any<Unit, Player>>()
    .Write<Position, Velocity>().For<MoveFunction>();

// 组合系统和 IQuery
public struct MoveSystem : ISystem, W.IQuery.Write<Position>.Read<Velocity> {
    private float _speed;

    public void Update() {
        _speed = W.GetResource<GameConfig>().Speed;
        W.Query<All<Unit>>()
            .Write<Position>().Read<Velocity>().For(ref this);
    }

    public void Invoke(W.Entity entity, ref Position pos, in Velocity vel) {
        pos.Value += vel.Value * _speed;
    }
}
```

### WorldQuery 方法

#### 委托 — 组件类型从 lambda 推导:

| 方法 | 组件 |
|------|------|
| `For(delegate)` | 1–6，每个组件 `ref` 或 `in` |
| `ForParallel(delegate)` | 1–6，每个组件 `ref` 或 `in` |
| `Search(out entity, delegate)` | 1–6，全部 `in` |

#### 函数结构体 — 通过构建器指定组件访问:

| 方法 | 组件 | 访问 |
|------|------|------|
| `Write<1‑6>()` | 1–6 | 全部 `ref` |
| `Write<1‑5>().Read<1‑5>()` | 2–6 合计 | 前面 `ref`，后面 `in` |
| `Read<1‑6>()` | 1–6 | 全部 `in` |

#### 块函数结构体 — 相同模式，仅限 `unmanaged`:

| 方法 | 组件 | 访问 |
|------|------|------|
| `WriteBlock<1‑6>()` | 1–6 | 全部 `Block<T>` |
| `WriteBlock<1‑5>().Read<1‑5>()` | 2–6 合计 | `Block<T>` + `BlockR<T>` |
| `ReadBlock<1‑6>()` | 1–6 | 全部 `BlockR<T>` |

每个构建器提供 `For<F>()` 和 `ForParallel<F>()`。
`Read` / `ReadBlock` 需要变更追踪（默认启用，通过 `FFS_ECS_DISABLE_CHANGED_TRACKING` 禁用）。

___

## 并行处理

{: .warningzh }
并行处理需要在创建世界时启用：在 `WorldConfig` 中设置 `ParallelQueryType.MaxThreadsCount` 或 `ParallelQueryType.CustomThreadsCount` 并指定 `CustomThreadCount`。
在并行迭代中只允许修改和销毁**当前**迭代的实体。禁止：创建实体、修改其他实体、读取事件。发送事件（`SendEvent`）是线程安全的（在没有同时读取同一类型时，详见[事件](events#多线程)）。始终使用 `QueryMode.Strict`。

```csharp
// 委托是第一个参数，minEntitiesPerThread 是命名参数（默认 256）
W.Query().ForParallel(
    static (W.Entity entity, ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value;
    },
    minEntitiesPerThread: 50000
);

// 不带实体 — 仅组件
W.Query().ForParallel(
    static (ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value;
    },
    minEntitiesPerThread: 50000
);

// 使用用户数据
W.Query().ForParallel(deltaTime,
    static (ref float dt, ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value * dt;
    },
    minEntitiesPerThread: 50000
);

// 带过滤
W.Query<None<Name>, Any<Unit, Player>>().ForParallel(
    static (W.Entity entity) => {
        entity.Add<Name>();
    },
    minEntitiesPerThread: 50000
);

// 通过函数结构体
W.Query().Write<Position>().Read<Velocity>().ForParallel<ApplyVelocity>(
    minEntitiesPerThread: 50000
);

// workersLimit — 限制线程数量（0 = 使用所有可用线程）
W.Query().ForParallel(
    static (ref Position pos) => { /* ... */ },
    minEntitiesPerThread: 10000,
    workersLimit: 4
);
```

___

## 块迭代 (ForBlock)

通过函数结构体进行低级别迭代 — 对于 `unmanaged` 组件，提供带有数据数组直接指针的 `Block<T>`（可写）和 `BlockR<T>`（只读）包装器。

接口层次结构与 `IQuery` 相同（总共 1 到 6 个 unmanaged 组件）：
- `IQueryBlock.Write<T0, T1>` — 所有组件可写（`Block<T>`）
- `IQueryBlock.Read<T0, T1>` — 所有组件只读（`BlockR<T>`）
- `IQueryBlock.Write<T0>.Read<T1>` — 前面可写，后面只读

```csharp
// 全部可写 — IQueryBlock.Write
readonly struct MoveBlock : W.IQueryBlock.Write<Position, Velocity> {
    public void Invoke(uint count, EntityBlock entitiesBlock,
                       Block<Position> positions, Block<Velocity> velocities) {
        for (uint i = 0; i < count; i++) {
            positions[i].Value += velocities[i].Value;
        }
    }
}

// Fluent API: WriteBlock<...>().For<TFunction>()
W.Query().WriteBlock<Position, Velocity>().For<MoveBlock>();

// 混合写入/读取 — WriteBlock<>.Read<>
readonly struct ApplyVelocityBlock : W.IQueryBlock.Write<Position>.Read<Velocity> {
    public void Invoke(uint count, EntityBlock entitiesBlock,
                       Block<Position> positions, BlockR<Velocity> velocities) {
        for (uint i = 0; i < count; i++) {
            positions[i].Value += velocities[i].Value;
        }
    }
}

W.Query().WriteBlock<Position>().Read<Velocity>().For<ApplyVelocityBlock>();

// 全部只读 — ReadBlock<>
readonly struct SumPositionsBlock : W.IQueryBlock.Read<Position> {
    public void Invoke(uint count, EntityBlock entitiesBlock, BlockR<Position> positions) {
        for (uint i = 0; i < count; i++) {
            // 只读访问
        }
    }
}

W.Query().ReadBlock<Position>().For<SumPositionsBlock>();

// 通过 ref（保留状态）
var func = new MoveBlock();
W.Query().WriteBlock<Position, Velocity>().For(ref func);

// 并行版本
W.Query().WriteBlock<Position, Velocity>().ForParallel<MoveBlock>(minEntitiesPerThread: 50000);
```

___

## 批量操作

对所有匹配过滤器的实体执行批量操作 — 无需编写循环。
可以比通过 `For` 手动迭代快数十倍：批量操作使用位掩码而非逐个处理实体 — 在最佳情况下，为 64 个实体添加或删除组件/标签只需一次位运算。
支持链式调用 — 多个操作可以在一次遍历中执行。

```csharp
// 为所有实体添加组件（1 到 5 个类型）
W.Query<All<Position>>().BatchSet(new Velocity { Value = 1f });

// 删除所有实体的组件
W.Query<All<Position, Velocity>>().BatchDelete<Velocity>();

// 禁用/启用所有实体的组件
W.Query<All<Position>>().BatchDisable<Position>();
W.Query<AllOnlyDisabled<Position>>().BatchEnable<Position>();

// 标签：设置、删除、切换、按条件应用（1 到 5 个类型）
W.Query<All<Position>>().BatchSet<Unit>();
W.Query<All<Unit>>().BatchDelete<Unit>();
W.Query<All<Position>>().BatchToggle<Unit>();
W.Query<All<Position>>().BatchApply<Unit>(true);

// 链式调用
W.Query<All<Position>>()
    .BatchSet(new Velocity { Value = 1f })
    .BatchSet<Unit>()
    .BatchDisable<Position>();
```

___

## 销毁和卸载实体

```csharp
// 销毁所有匹配过滤器的实体
W.Query<All<Position>>().BatchDestroy();

// 带参数
W.Query<All<Unit>>().BatchDestroy(
    entities: EntityStatusType.Any,
    mode: QueryMode.Flexible
);

// 卸载所有匹配过滤器的实体
// （标记为已卸载，移除组件/标签，但保留实体 ID 和版本）
W.Query<All<Position>>().BatchUnload();

// 带参数
W.Query<All<Unit>>().BatchUnload(
    entities: EntityStatusType.Any,
    mode: QueryMode.Flexible
);
```

___

## 集群

{: .importantzh }
所有查询方法（`Entities`、`For`、`ForParallel`、`Search`、`Batch*`、`BatchDestroy`、`BatchUnload`）都接受 `clusters` 参数：

```csharp
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { 2, 5, 12 };

foreach (var entity in W.Query<All<Position>>().Entities(clusters: clusters)) {
    // 仅遍历集群 2、5、12 中的实体
}

W.Query().For(static (W.Entity entity, ref Position pos) => {
    // ...
}, clusters: clusters);
```

___

## QueryMode

用于 `For`、`Search`、`Entities` 方法：

- **`QueryMode.Strict`**（默认）— 禁止在迭代期间修改**其他**实体上的过滤组件/标签类型。更快。
- **`QueryMode.Flexible`** — 允许修改其他实体上的过滤类型，正确控制当前迭代。

```csharp
var anotherEntity = W.NewEntity<Default>();
anotherEntity.Add<Position>();

// Strict：修改另一个实体的 Position — DEBUG 中报错
foreach (var entity in W.Query<All<Position>>().Entities()) {
    anotherEntity.Delete<Position>(); // DEBUG 中报错
}

// Flexible：允许修改，迭代保持稳定
foreach (var entity in W.Query<All<Position>>().EntitiesFlexible()) {
    anotherEntity.Delete<Position>(); // OK — anotherEntity 被正确排除
}

// 对于 For/Search 通过参数设置
W.Query().For(static (ref Position pos) => {
    // ...
}, queryMode: QueryMode.Flexible);
```

{: .notezh }
`Flexible` 适用于层次结构或缓存，当从一个实体的组件修改其他实体时。其他情况下推荐使用 `Strict` 以获得更好的性能。
