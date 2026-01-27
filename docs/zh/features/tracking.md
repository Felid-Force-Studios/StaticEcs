---
title: 变更追踪
parent: 功能
nav_order: 13
---

## 变更追踪

StaticEcs 提供四种变更追踪类型，全部零分配且按需启用：

| 类型 | 追踪目标 | 适用范围 | 配置位置 |
|------|---------|---------|---------|
| **Added** | 组件/标签被添加 | 组件、标签 | `ComponentTypeConfig` / `TagTypeConfig` |
| **Deleted** | 组件/标签被删除 | 组件、标签 | `ComponentTypeConfig` / `TagTypeConfig` |
| **Changed** | 组件数据通过 `ref` 被访问 | 仅组件 | `ComponentTypeConfig` |
| **Created** | 新实体被创建 | 全局（实体） | `WorldConfig.TrackCreated` |

- 位图存储：每 64 个实体一个 `ulong`，每个被追踪的类型独立
- 追踪通过环形缓冲区（默认 8 个 Tick）按世界 Tick 进行版本管理。每个系统自动查看自上次执行以来的变更 — 无需手动清除
- 未启用追踪的类型零开销
- `WorldConfig.TrackCreated = false` 时 `Created` 追踪零开销

___

## 配置

所有追踪默认关闭，必须在类型注册时显式启用。

### 组件

`ComponentTypeConfig<T>` 支持三个追踪标志：`trackAdded`、`trackDeleted`、`trackChanged`：

```csharp
W.Create(WorldConfig.Default());
//...
// 启用全部三种追踪
W.Types().Component<Health>(new ComponentTypeConfig<Health>(
    trackAdded: true,
    trackDeleted: true,
    trackChanged: true
));

// 仅启用一个方向
W.Types().Component<Velocity>(new ComponentTypeConfig<Velocity>(
    trackAdded: true  // 仅追踪添加
));

// 带追踪的完整配置
W.Types().Component<Position>(new ComponentTypeConfig<Position>(
    guid: new Guid("..."),
    defaultValue: default,
    trackAdded: true,
    trackDeleted: true,
    trackChanged: true
));
//...
W.Initialize();
```

### 标签

`TagTypeConfig<T>` 支持 `trackAdded` 和 `trackDeleted`。标签**不**支持 Changed 追踪。

```csharp
W.Types().Tag<Unit>(new TagTypeConfig<Unit>(
    trackAdded: true,
    trackDeleted: true
));

// 带序列化 GUID
W.Types().Tag<Poisoned>(new TagTypeConfig<Poisoned>(
    guid: new Guid("A1B2C3D4-..."),
    trackAdded: true,
    trackDeleted: true
));
```

### 实体创建

实体创建追踪在**世界级别**通过 `WorldConfig` 配置：

```csharp
W.Create(new WorldConfig {
    TrackCreated = true,
    // ...其他设置...
});
//...
W.Initialize();
```

{: .notezh }
`Created` 追踪所有实体的创建，不区分实体类型。要按实体类型过滤，请与 `EntityIs<T>` 组合：`W.Query<Created, EntityIs<Bullet>>()`。

### 自动注册

`trackAdded`、`trackDeleted` 和 `trackChanged` 可以在结构体内的静态 `Config` 字段中声明 — `RegisterAll()` 会自动获取。

### 编译时禁用

`FFS_ECS_DISABLE_CHANGED_TRACKING` 编译指令会在编译时移除所有 Changed 追踪代码路径，包括 `AllChanged<T>`、`NoneChanged<T>`、`AnyChanged<T>` 过滤器以及 `Mut<T>()` 方法。

### 基于 Tick 的追踪

`WorldConfig.TrackingBufferSize`（默认 8）控制环形缓冲区保留多少个 Tick 的历史。调用 `W.Tick()` 推进 Tick 并旋转缓冲区。

```csharp
// 默认：保留 8 个 Tick 的历史
W.Create(WorldConfig.Default()); // TrackingBufferSize = 8

// 自定义缓冲区大小
W.Create(new WorldConfig {
    TrackingBufferSize = 16,   // 16 个 Tick 的历史
    // ...其他设置...
});
```

#### 选择缓冲区大小

缓冲区必须足够大，以容纳使用追踪过滤器的最慢系统的追踪历史。如果 `W.Tick()` 以 60fps 调用，而某些系统以 20fps 运行，它们在两次执行之间跳过 2 个 Tick，需要回溯 3 个 Tick。

**公式：** `TrackingBufferSize >= tickRate / slowestSystemRate`

| Tick 频率 | 最慢系统 | 最小缓冲区 |
|----------|---------|-----------|
| 60 fps | 60 fps（每个 Tick） | 1 |
| 60 fps | 20 fps（每 3 个 Tick） | 3 |
| 60 fps | 10 fps（每 6 个 Tick） | 6 |
| 60 fps | 1 fps（每 60 个 Tick） | 60 |

如果系统使用实际时间间隔而非 Tick 计数器，高于预期的 FPS 会增加两次执行之间的 Tick 数——请相应留出余量。默认值 8 适用于大多数游戏，其中最慢的使用追踪的系统运行在 ~20fps 或更快。

___

## 基于 Tick 的追踪

基于 Tick 的追踪解决了两个常见问题：
1. 管线中间的系统产生的变更，管线开头的系统在下一帧无法看到——如果在帧末清除追踪
2. 不同的系统组（Update / FixedUpdate）可以自然协作 — 每个系统有独立的 Tick 范围

### 工作原理

- `W.Systems<T>.Update()` 中的每个系统自动获得 `LastTick` — 它能看到 `(LastTick, CurrentTick]` 范围内的所有变更 — 当前帧中的变更在下一帧才可见
- 系统执行完毕后，其 `LastTick` 被设置为 `CurrentTick`
- 如果系统被跳过（`UpdateIsActive() = false`），其 `LastTick` 不会更新 — 下次运行时，它能看到所有累积的变更
- `W.Tick()` 推进全局 Tick 计数器并旋转环形缓冲区 — 写入槽位变为可读的历史记录，新槽位被清除并成为所有追踪操作的写入目标

### 游戏循环集成

{: .importantzh }
每帧在最快的系统组**之后**调用一次 `W.Tick()`。不要在每个组之后都调用 `Tick()` — 这会浪费槽位。Per-system `LastTick` 自动确保低频系统累积多个 Tick 的变更。在帧内所做的变更在下一帧才可见。

```csharp
// 单系统组
while (running) {
    W.Systems<GameLoop>.Update();    // 每个系统看到自 LastTick 以来的变更
    W.Tick();                      // 推进 Tick，旋转环形缓冲区
}

// 多系统组（例如 Update + FixedUpdate）
while (running) {
    W.Systems<Update>.Update();

    // FixedUpdate 每帧可能执行多次——都在同一个 Tick 内
    while (fixedTimeAccumulator >= fixedDeltaTime) {
        W.Systems<FixedUpdate>.Update();
        fixedTimeAccumulator -= fixedDeltaTime;
    }

    W.Tick();                      // 每帧一个 Tick
}
```

### 一帧延迟

追踪变更写入专用的写入槽位，与可读历史分离。调用 `W.Tick()` 时，写入槽位变为历史的一部分。因此每个系统看到的是**上次执行之后、当前帧之前**的变更——不会看到当前帧的变更。

以 5 个系统的管线为例，Sys1 和 Sys5 修改 `Position`，Sys3 查询 `AllChanged<Position>`：

```
帧 1：
  Sys1  → 修改 Position    （写入写入槽位）
  Sys3  → 查询追踪          → 看到空（历史为空，第一帧）
  Sys5  → 修改 Position    （写入同一写入槽位）
  Tick()                    → 写入槽位变为 history[tick 1]

帧 2：
  Sys1  → 修改 Position    （写入新的写入槽位）
  Sys3  → 查询追踪          → 看到 history[tick 1] = 帧 1 的 Sys1 + Sys5
  Sys5  → 修改 Position    （写入同一写入槽位）
  Tick()                    → 写入槽位变为 history[tick 2]

帧 3：
  Sys3  → 查询追踪          → 看到 history[tick 2] = 帧 2 的 Sys1 + Sys5
```

每帧 Sys3 **恰好**看到上一帧的变更——包括它之前的系统（Sys1）和之后的系统（Sys5）。无重复处理，无遗漏。

### 每个系统的 Tick 追踪

每个系统维护自己的 `LastTick`。每个 Tick 都运行的系统恰好看到 1 个 Tick 的变更。跳过若干帧的系统会看到自上次执行以来所有累积的变更：

```csharp
public struct RareSystem : ISystem {
    private int _counter;

    public bool UpdateIsActive() => ++_counter % 5 == 0; // 每 5 个 Tick 运行一次

    public void Update() {
        // 看到过去 5 个 Tick 的所有变更（最多 TrackingBufferSize 个）
        foreach (var entity in W.Query<All<Position>, AllAdded<Position>>().Entities()) {
            // 处理过去 5 个 Tick 中新添加的 Position
        }
    }
}
```

### 自定义 Tick 范围（FromTick）

所有追踪过滤器接受可选的 `fromTick` 构造参数来覆盖自动 Tick 范围：

```csharp
// 自动 — 使用系统的 LastTick（默认，无需构造参数）：
foreach (var entity in W.Query<All<Position>, AllAdded<Position>>().Entities()) { }

// 手动 — 查看从 Tick 5 到当前的所有变更：
var filter = new AllAdded<Position>(fromTick: 5);
foreach (var entity in W.Query<All<Position>>(filter).Entities()) { }
```

- `fromTick = 0`（默认）：自动范围，从 `CurrentLastTick`（由 `W.Systems<T>.Update()` 设置）开始
- `fromTick > 0`：手动下界 — 查看从该 Tick 到当前 Tick 的变更

### 跨组同步

使用基于 Tick 的追踪，不同系统组在同一帧内自然协作：

```csharp
W.Systems<Update>.Update();          // 系统将追踪数据写入 tick N
W.Systems<FixedUpdate>.Update();     // 同样写入 tick N 的写入槽位
W.Tick();                          // 推进到 tick N+1；tick N 变为可读历史
```

每个系统的 `LastTick` 是独立的。跳过若干帧的 FixedUpdate 系统会看到自上次运行以来所有前帧的累积变更。

### 缓冲区溢出

如果系统未运行的 Tick 数超过 `TrackingBufferSize`，最旧的追踪数据会被覆盖。系统最多能看到 `TrackingBufferSize` 个 Tick 的历史。

{: .warningzh }
在调试模式（`FFS_ECS_DEBUG`）下，当系统的 Tick 范围超过缓冲区大小时，会抛出 `StaticEcsException`。在发布模式下，范围会被静默截断。如果系统需要更深的历史记录，请增加 `WorldConfig.TrackingBufferSize`。

___

## 查询过滤器

所有追踪过滤器的使用方式与标准组件/标签过滤器一致：

| 类别 | 过滤器 | 类型参数 | 描述 |
|------|--------|---------|------|
| **组件 Added** | `AllAdded<T0..T4>` | 1–5 | **所有**指定组件都被添加 |
| | `NoneAdded<T0..T4>` | 1–5 | 排除任何指定组件被添加的实体 |
| | `AnyAdded<T0..T4>` | 2–5 | **至少一个**指定组件被添加 |
| **组件 Deleted** | `AllDeleted<T0..T4>` | 1–5 | **所有**指定组件都被删除 |
| | `NoneDeleted<T0..T4>` | 1–5 | 排除任何指定组件被删除的实体 |
| | `AnyDeleted<T0..T4>` | 2–5 | **至少一个**指定组件被删除 |
| **组件 Changed** | `AllChanged<T0..T4>` | 1–5 | **所有**指定组件都通过 `ref` 被访问 |
| | `NoneChanged<T0..T4>` | 1–5 | 排除任何指定组件被更改的实体 |
| | `AnyChanged<T0..T4>` | 2–5 | **至少一个**指定组件通过 `ref` 被访问 |
| **实体** | `Created` | — | 实体被创建（需要 `WorldConfig.TrackCreated`） |

{: .notezh }
`AllAdded`、`NoneAdded`、`AnyAdded`、`AllDeleted`、`NoneDeleted`、`AnyDeleted` 过滤器同时适用于组件和标签。没有单独的标签追踪过滤器类型。

### 示例

```csharp
// 添加了 Position 且当前存在的实体
foreach (var entity in W.Query<All<Position>, AllAdded<Position>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
}

// Position 和 Velocity 都被添加的实体
foreach (var entity in W.Query<AllAdded<Position, Velocity>>().Entities()) { }

// 至少一个（Position 或 Velocity）被添加
foreach (var entity in W.Query<AnyAdded<Position, Velocity>>().Entities()) { }

// 响应标签设置
foreach (var entity in W.Query<AllAdded<IsDead>>().Entities()) { }

// 至少一个指定标签被设置
foreach (var entity in W.Query<AnyAdded<Poisoned, Stunned>>().Entities()) { }

// 处理 Position 被修改（通过 ref 访问）的实体
foreach (var entity in W.Query<All<Position>, AllChanged<Position>>().Entities()) {
    ref readonly var pos = ref entity.Read<Position>();
}

// 仅处理真正更改的，排除新添加的
foreach (var entity in W.Query<All<Position>, AllChanged<Position>, NoneAdded<Position>>().Entities()) {
    ref readonly var pos = ref entity.Read<Position>();
}

// 处理最近创建的带 Position 的实体
foreach (var entity in W.Query<Created, All<Position>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
}

// 通过 And 组合过滤器
var filter = default(And<AllAdded<Position, Unit>, AllDeleted<Velocity>>);
foreach (var entity in W.Query(filter).Entities()) { }
```

___

## 语义

### Added / Deleted

{: .importantzh }
**`AllAdded<T>` 仅表示组件被添加过 — 并不保证组件当前存在！** 如果组件在同一帧内被添加后又被删除，它仍然被标记为 Added，但组件已经不存在了。同样，`AllDeleted<T>` 表示组件被删除过 — 但组件可能已经被重新添加。

**推荐的过滤器组合：**
```csharp
// "已添加且当前存在" — 推荐模式
foreach (var entity in W.Query<All<Position>, AllAdded<Position>>().Entities()) {
    ref var pos = ref entity.Ref<Position>(); // 安全 — All<Position> 保证存在
}

// "已删除且当前不存在"
foreach (var entity in W.Query<None<Position>, AllDeleted<Position>>().Entities()) {
    // 实体存活，Position 已被删除 — 可以清理相关资源
}

// 仅 AllAdded<Position> — 不保证存在！
foreach (var entity in W.Query<AllAdded<Position>>().Entities()) {
    // 注意：组件可能已被删除！
    if (entity.Has<Position>()) {
        ref var pos = ref entity.Ref<Position>();
    }
}
```

### Changed（悲观模型）

Changed 追踪采用 **dirty-on-access** 模型：只要获取了组件的 `ref` 引用就会标记为 Changed，无论数据是否实际被修改。这是有意为之 — 在字段级别检查实际变化对于高性能 ECS 来说开销过大。

#### 数据访问方法

| 方法 | 返回类型 | 标记 Changed | 标记 Added | 说明 |
|------|---------|:---:|:---:|------|
| `Ref<T>()` | `ref T` | — | — | 快速可变访问，无追踪 |
| `Mut<T>()` | `ref T` | 是 | — | 带追踪的可变访问 |
| `Read<T>()` | `ref readonly T` | — | — | 只读访问 |
| `Add<T>()` (新组件) | `ref T` | 是 | 是 | 组件是新的 |
| `Add<T>()` (已存在) | `ref T` | — | — | 返回已有组件的引用，无钩子 |
| `Set(value)` (新组件) | void | 是 | 是 | 组件是新的 |
| `Set(value)` (已存在) | void | 是 | — | 覆盖已有组件 |

{: .importantzh }
**`Ref<T>()` 不标记 Changed。** 需要变更追踪时请使用 `Mut<T>()`。`Ref<T>()` 是访问组件数据最快的方式 — 零开销，无分支。`Read<T>()` 用于只读访问。在查询委托迭代（`For`、`ForBlock`）中，`ref` 参数自动使用追踪访问（`Mut` 语义），`in` 参数使用只读访问（`Read` 语义）。

#### 查询中的自动追踪

查询迭代根据访问语义自动标记 Changed：

**For 委托** — `ref` 标记 Changed，`in` 不标记：
```csharp
// Position 被标记为 Changed (ref)，Velocity 不标记 (in)
W.Query<All<Position, Velocity>>().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});
```

**IQuery 结构体** — `Write<T>` 标记 Changed，`Read<T>` 不标记：
```csharp
public struct MoveSystem : IQuery.Write<Position>.Read<Velocity> {
    public void Invoke(Entity entity, ref Position pos, in Velocity vel) {
        pos.Value += vel.Value;
    }
}
```

**ForBlock** — `Block<T>`（可变）标记 Changed，`BlockR<T>`（只读）不标记：
```csharp
public struct MoveBlockSystem : IQueryBlock.Write<Position>.Read<Velocity> {
    public void Invoke(uint count, EntityBlock entities, Block<Position> pos, BlockR<Velocity> vel) {
        // 处理块
    }
}
```

并行查询遵循相同规则。

#### Changed 与 Added 的交互

{: .importantzh }
通过 `Add<T>()` 或 `Set(value)` 添加组件时，它会**同时**被标记为 Added 和 Changed。要仅处理真正被修改（而非新添加）的组件，使用 `AllChanged<T>` 配合 `NoneAdded<T>`：

```csharp
foreach (var entity in W.Query<All<Position>, AllChanged<Position>, NoneAdded<Position>>().Entities()) {
    // 仅真正更改的，而非新添加的
}
```

### Created

`Created` 全局追踪实体创建。不携带类型信息 — 要按实体类型过滤，与 `EntityIs<T>` 组合：

```csharp
foreach (var entity in W.Query<Created, EntityIs<Bullet>, All<Position>>().Entities()) {
    // 刚创建的带 Position 的子弹
}
```

___

## 边界情况

{: .importantzh }
Added 和 Deleted 状态是**独立的**，**不会互相抵消**。它们记录当前 Tick 内发生的所有操作。Changed 也独立于两者。

### 添加 → 删除
```csharp
entity.Set(new Position { X = 10 });   // Added = 1
entity.Delete<Position>();              // Deleted = 1，Added 保留

// 结果：实体没有 Position，但被标记为 Added 和 Deleted
// Query<AllAdded<Position>>                    -> 找到
// Query<AllDeleted<Position>>                  -> 找到
// Query<All<Position>, AllAdded<Position>>     -> 找不到（组件不存在）
// Query<None<Position>, AllDeleted<Position>>  -> 找到
```

### 删除 → 添加
```csharp
entity.Delete<Weapon>();                // Deleted = 1
entity.Set(new Weapon { Damage = 50 }); // Added = 1，Deleted 保留

// 结果：实体拥有 Weapon，被标记为 Added 和 Deleted
// Query<All<Weapon>, AllAdded<Weapon>>   -> 找到
// Query<All<Weapon>, AllDeleted<Weapon>> -> 找到
```

### 添加 → 删除 → 添加
```csharp
entity.Set(new Health { Value = 100 }); // Added = 1
entity.Delete<Health>();                // Deleted = 1
entity.Set(new Health { Value = 50 });  // Added 已标记

// 结果：实体拥有 Health（Value = 50），被标记为 Added 和 Deleted
// 从追踪角度看，等同于"删除 → 添加"
```

### 多次添加（幂等性）
```csharp
// 不带值的 Add 不会覆盖已存在的组件
entity.Add<Position>();                 // Added = 1（新组件）
entity.Add<Position>();                 // Added 已标记，无变化
// Added 仅在第一次添加时标记（当组件是新的时候）

// 带值的 Set 总是覆盖
entity.Set(new Position { X = 10 });    // Added = 1（新组件）
entity.Set(new Position { X = 20 });    // 覆盖，Added 不再标记
                                         //（组件已存在）
```

### Mut 未修改数据
```csharp
ref var pos = ref entity.Mut<Position>(); // 即使未写入也被标记为 Changed！
// Changed 追踪是悲观的 — 追踪访问而非实际修改
// 如果不需要追踪，使用 entity.Ref<Position>() — 零开销
```

### 多次 Mut 调用
```csharp
entity.Mut<Position>(); // 标记
entity.Mut<Position>(); // 已标记，无额外开销
// Changed 位是幂等的
```

### 查询迭代标记所有被迭代的实体
```csharp
// 所有匹配查询的实体都会为 ref 组件获得 Changed 标记，
// 即使委托实际未修改数据
W.Query<All<Position>>().For(static (ref Position pos) => {
    var x = pos.X; // 因 `ref` 而被标记为 Changed，尽管只是读取
});

// 使用 `in` 来避免：
W.Query<All<Position>>().For(static (in Position pos) => {
    var x = pos.X; // 不会被标记为 Changed
});
```

### Changed 和 Deleted 相互独立
Changed 和 Deleted 是独立的位。如果组件在同一帧内通过 `ref` 被访问然后被删除，两个位都会被设置。

___

## Destroy 与反序列化

### Destroy 行为

`entity.Destroy()` 删除所有组件/标签 — 它们被标记为 Deleted。但实体已死亡，alive 掩码会将其从所有查询中过滤掉。因此 `AllDeleted<T>` **不会**找到已销毁的实体。

```csharp
var entity = W.Entity.New<Position, Velocity>();
entity.Destroy();
// Query<AllDeleted<Position>> -> 找不到（实体已死亡）

// 如果需要响应销毁 — 在 Destroy 之前显式删除组件：
entity.Delete<Position>();  // Deleted = 1，实体仍然存活
// ... 处理 AllDeleted<Position> ...
entity.Destroy();
```

### 反序列化

```csharp
// ReadChunk 直接写入掩码 — 追踪不会被触发
// ReadEntity 通过 Add 执行 — 组件被标记为 Added
// 建议在加载后调用 ClearTracking()：
W.Serializer.ReadChunk(ref reader);
W.ClearTracking(); // 重置所有追踪，清除所有环形缓冲区槽位
```

___

## 清除追踪

{: .importantzh }
通常**不需要**手动清除 — 追踪由 `W.Tick()` 和 `W.Systems<T>.Update()` 自动管理。`ClearTracking()` 方法作为"核选项"存在，会清除所有环形缓冲区槽位。主要用于反序列化后或需要完全重置追踪状态的场景。

```csharp
// === 全部清除 ===
W.ClearTracking();                         // 所有掩码（Added + Deleted + Changed + Created）

// === 按类别清除 ===
W.ClearAllTracking();                      // 所有组件和标签（Added + Deleted + Changed）
W.ClearCreatedTracking();                  // 实体创建

// === 按追踪种类清除（所有类型） ===
W.ClearAllAddedTracking();                 // 所有组件和标签的 Added
W.ClearAllDeletedTracking();               // 所有组件和标签的 Deleted
W.ClearAllChangedTracking();               // 所有组件的 Changed

// === 按具体类型清除（组件和标签） ===
W.ClearTracking<Position>();               // Position 的 Added + Deleted + Changed
W.ClearAddedTracking<Position>();          // 仅 Added
W.ClearDeletedTracking<Position>();        // 仅 Deleted
W.ClearChangedTracking<Position>();        // 仅 Changed

// 标签使用相同方法
W.ClearTracking<Unit>();                   // Unit 的 Added + Deleted
W.ClearAddedTracking<Unit>();              // 仅 Added
W.ClearDeletedTracking<Unit>();            // 仅 Deleted
```

{: .notezh }
正常游戏循环中无需调用上述任何清除方法。`W.Systems.Update()` -> `W.Tick()` -> 重复 — 追踪自动管理。

___

## 检查状态

除了查询过滤器之外，还可以直接检查单个实体的追踪状态：

```csharp
// 组件 — ALL 语义（所有指定类型都必须匹配）
bool wasAdded = entity.HasAdded<Position>();
bool bothAdded = entity.HasAdded<Position, Velocity>();       // Position 和 Velocity 都已添加
bool wasDeleted = entity.HasDeleted<Health>();
bool wasChanged = entity.HasChanged<Position>();
bool bothChanged = entity.HasChanged<Position, Velocity>();   // Position 和 Velocity 都已更改

// 组件 — ANY 语义（至少一个必须匹配）
bool anyAdded = entity.HasAnyAdded<Position, Velocity>();     // Position 或 Velocity 已添加
bool anyDeleted = entity.HasAnyDeleted<Position, Velocity>(); // Position 或 Velocity 已删除
bool anyChanged = entity.HasAnyChanged<Position, Velocity>(); // Position 或 Velocity 已更改

// 标签 — 使用相同方法（ALL 语义）
bool tagAdded = entity.HasAdded<Unit>();
bool tagDeleted = entity.HasDeleted<Poisoned>();
bool bothTagsAdded = entity.HasAdded<Unit, Player>();          // Unit 和 Player 都已添加

// 标签 — ANY 语义
bool anyTagAdded = entity.HasAnyAdded<Unit, Player>();         // Unit 或 Player 已添加
bool anyTagDeleted = entity.HasAnyDeleted<Unit, Player>();     // Unit 或 Player 已删除

// 组合使用
if (entity.HasAdded<Position>() && entity.Has<Position>()) {
    ref var pos = ref entity.Ref<Position>();
    // 组件已添加且当前存在
}

// 所有方法都接受可选的 fromTick 参数，用于指定自定义 Tick 范围：
bool addedSinceTick5 = entity.HasAdded<Position>(fromTick: 5);
bool changedRecently = entity.HasChanged<Position>(fromTick: W.CurrentTick);
```

___

## 性能

- 追踪掩码与组件/标签存在掩码使用相同的 `ulong` 每块格式
- 组件：每个被追踪的类型最多 3 个带（Added、Deleted、Changed），每个带每 64 个实体一个 `ulong`
- 标签：每个被追踪的类型最多 2 个带（Added、Deleted）
- `Created`：全局每块 1 个 `ulong`，加上启发式 Chunk 用于快速跳过
- `AllAdded<T>` / `AllDeleted<T>` / `AllChanged<T>` 过滤器与 `All<T>` / `None<T>` 成本相同：每块一次位掩码操作
- 查询中的 Changed 追踪：每块一次批量 OR 操作 — 与一次位掩码操作成本相同
- `ClearTracking()` 使用启发式 Chunk 跳过空区域 — O(已占用的块)，而非 O(整个世界)
- `Ref<T>()` 零追踪开销 — 无运行时分支，与追踪功能添加前的代码完全相同
- 未启用追踪的类型零开销
- `WorldConfig.TrackCreated = false` 时 `Created` 追踪零开销
- `FFS_ECS_DISABLE_CHANGED_TRACKING` 编译指令在编译时移除所有 Changed 追踪代码路径
- **基于 Tick 的写入：** 零开销（指针交换）
- **基于 Tick 的读取：** O(ticksToCheck) 次 OR 操作，受 `TrackingBufferSize` 限制。分层过滤：先在 Chunk 级别（4096 个实体），再在 Block 级别（64 个实体）——仅检查有实际追踪数据的 Chunk/Block
- **Tick 推进：** 每帧开销可忽略
- **内存：** 启发式数组 × `TrackingBufferSize`；段数据延迟分配

___

## 使用场景

**网络同步（增量更新）：**
```csharp
foreach (var entity in W.Query<All<Position>, AllChanged<Position>>().Entities()) {
    ref readonly var pos = ref entity.Read<Position>();
    SendPositionUpdate(entity, pos);
}
```

**物理同步：**
```csharp
foreach (var entity in W.Query<All<Transform, PhysicsBody>, AllChanged<Transform>>().Entities()) {
    ref readonly var transform = ref entity.Read<Transform>();
    ref var body = ref entity.Ref<PhysicsBody>();
    SyncPhysicsBody(ref body, transform);
}
```

**响应式初始化：**
```csharp
foreach (var entity in W.Query<All<Position, Unit>, AllAdded<Position>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
    // 为新实体创建视觉表示
}
```

**实体初始化：**
```csharp
foreach (var entity in W.Query<Created, All<Position, Unit>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
    // 创建视觉效果、物理体等
}
```

**UI 更新：**
```csharp
// 为新实体创建血条
foreach (var entity in W.Query<All<Health, Player>, AllAdded<Health>>().Entities()) {
    ref var health = ref entity.Ref<Health>();
    // 创建血条 UI 元素
}

// 仅在数据变化时更新血条
foreach (var entity in W.Query<All<Health, Player>, AllChanged<Health>>().Entities()) {
    ref readonly var health = ref entity.Read<Health>();
    // 更新显示
}
```

**多系统组（基于 Tick 模式）：**
```csharp
void GameLoop() {
    W.Systems<Update>.Update();      // 每个系统看到之前帧的变更
    W.Systems<FixedUpdate>.Update(); // 同理——per-system LastTick 决定范围
    W.Tick();                      // 将当前帧的追踪提交到历史
}
```

**条件系统（基于 Tick 模式）：**
```csharp
public struct PeriodicSync : ISystem {
    private int _frame;
    public bool UpdateIsActive() => ++_frame % 10 == 0;

    public void Update() {
        // 自动看到过去 10 个 Tick 的所有变更
        foreach (var entity in W.Query<All<Position>, AllChanged<Position>>().Entities()) {
            SyncToNetwork(entity);
        }
    }
}
```
