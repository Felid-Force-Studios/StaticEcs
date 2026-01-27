# StaticEcs 2.0.0 — 新版本介绍

**StaticEcs 2.0.0** — 这不是增量更新，而是对框架的全面重构。重新设计了数据存储模型，统一了API，添加了全新的功能——从变更追踪系统到带直接指针的块迭代。如果说1.x是一个快速方便的ECS，那么2.0就是一个成熟的框架，适用于大规模项目：网络游戏、开放世界流式加载、响应式UI以及百万级实体的仿真。

以下是关键变更的概述，从最重大的新功能开始，到API细节结束。

---

## 重大新功能

### 分段存储模型

在1.x中，世界被划分为块(chunk，4096个实体)和块(block，64个实体)。在2.0中引入了一个中间层级——**段**(segment，256个实体)。新的层级结构：

```
Chunk (4096) → Segment (256) → Block (64) → Entity
```

段成为了组件内存分配的基本单位。这使得实现**EntityType × Cluster**的二维分区成为可能——同一类型的实体在同一集群内被放置在相邻的段中，从而在迭代时极大地改善了缓存局部性。

---

### 实体类型 (IEntityType)

最重要的架构创新之一。在1.x中，所有实体都是相同的——仅通过组件集合来区分。在2.0中，每个实体在创建时都会获得一个**类型**——一个逻辑类别，定义其用途和内存布局。

```csharp
public struct Bullet : IEntityType {
    public static readonly byte Id = 1;
}

public struct Enemy : IEntityType {
    public static readonly byte Id = 2;
}

// 创建——显式指定类型
var bullet = W.NewEntity<Bullet>();
var enemy = W.NewEntity<Enemy>();
```

实体类型提供：

- **缓存局部性** — 相同类型的实体存储在相邻的内存段中。当查询迭代单位时，它遍历密集打包的数据，无需跳过弹丸和特效。
- **查询过滤** — 新的过滤器`EntityIs<T>`、`EntityIsNot<T>`、`EntityIsAny<T>`允许将查询限制为特定类型：
  ```csharp
  foreach (var entity in W.Query<All<Position>, EntityIs<Bullet>>().Entities()) { ... }
  ```
- **生命周期钩子** — `OnCreate`和`OnDestroy`直接在类型结构体中定义：
  ```csharp
  public struct Bullet : IEntityType {
      public static readonly byte Id = 1;

      public void OnCreate<TWorld>(World<TWorld>.Entity entity)
          where TWorld : struct, IWorldType {
          entity.Set(new Velocity { Speed = 100 });
          entity.SetTag<Active>();
      }
  }
  ```
- **参数化创建** — 由于`IEntityType`是struct，类型可以包含创建时传递的数据：
  ```csharp
  public struct Flora : IEntityType {
      public static readonly byte Id = 4;
      public enum Kind : byte { Grass, Bush, Tree }
      public Kind FloraKind;

      public void OnCreate<TWorld>(World<TWorld>.Entity entity)
          where TWorld : struct, IWorldType {
          entity.Set(new Health { Value = FloraKind == Kind.Tree ? 100 : 10 });
      }
  }

  var tree = W.NewEntity(new Flora { FloraKind = Flora.Kind.Tree });
  ```

内置类型`Default`(Id = 0)会自动注册，并作为默认类型使用。

---

### 变更追踪系统 (Change Tracking)

全新的子系统，在1.x中不存在。允许追踪组件和标签的添加、删除和修改，无需手动维护dirty标志。

四种追踪类型：

| 类型 | 追踪内容 | 适用范围 |
|-----|----------------|---------|
| **Added** | 组件/标签的添加 | 组件、标签 |
| **Deleted** | 组件/标签的删除 | 组件、标签 |
| **Changed** | 通过 `Mut<T>()` 访问数据 | 仅组件 |
| **Created** | 实体的创建 | 整个世界 |

追踪在注册类型时启用：

```csharp
W.Types().Component<Health>(new ComponentTypeConfig<Health>(
    trackAdded: true,
    trackDeleted: true,
    trackChanged: true
));
```

追踪通过世界 Tick 环形缓冲区进行版本管理：

```csharp
// 缓冲区大小默认为 8 — 可通过 WorldConfig 配置
W.Create(new WorldConfig { TrackingBufferSize = 16 });
```

并通过新的查询过滤器使用：

```csharp
// 在上一帧中被添加了Position的实体
foreach (var entity in W.Query<All<Position>, AllAdded<Position>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
}

// 只处理已更改的位置（用于网络同步）
foreach (var entity in W.Query<All<Position>, AllChanged<Position>>().Entities()) {
    ref readonly var pos = ref entity.Read<Position>();
    SendPositionUpdate(entity, pos);
}

// 在上一帧中创建的实体
foreach (var entity in W.Query<Created, EntityIs<Bullet>, All<Position>>().Entities()) {
    // 初始化视觉效果
}
```

**追踪架构：**
- 位图存储：每 64 个实体一个 `ulong` — 与组件掩码格式相同
- 基于 Tick 的环形缓冲区：`W.Tick()` 推进世界 Tick，`W.Systems<T>.Update()` 中的每个系统自动看到自上次执行以来的变更
- Changed 追踪：`Mut<T>()` 标记 Changed，`Ref<T>()` 不标记（快速访问无追踪）
- 委托查询中 `ref` 标记 Changed，`in` 不标记
- 过滤器和 `HasAdded/HasDeleted/HasChanged` 方法接受可选的 `fromTick` 参数用于自定义 Tick 范围
- `ClearTracking()` 清除所有缓冲区槽位 — 通常不需要，追踪自动管理
- 未启用追踪的类型零开销
- `FFS_ECS_DISABLE_CHANGED_TRACKING` 在编译时移除所有 Changed 追踪代码路径

**游戏循环：**
```csharp
W.Systems<Update>.Update();    // 每个系统看到自 LastTick 以来的变更
W.Tick();                      // 每帧一个 Tick
```

完整的**16个**追踪过滤器：`AllAdded`、`AnyAdded`、`NoneAdded`、`AllDeleted`、`AnyDeleted`、`NoneDeleted`、`AllChanged`、`AnyChanged`、`NoneChanged`、`TagAllAdded`、`TagAnyAdded`、`TagNoneAdded`、`TagAllDeleted`、`TagAnyDeleted`、`TagNoneDeleted`、`Created`。

---

### 块迭代 (ForBlock)

一种全新的、最快的`unmanaged`组件迭代方式。ForBlock不是为每个实体的每个组件获取`ref`，而是提供包装器`Block<T>`和`BlockR<T>`——直接指向数据数组的指针，一次处理最多64个实体。

```csharp
readonly struct MoveBlock : W.IQueryBlock.Write<Position>.Read<Velocity> {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(uint count, EntityBlock entities,
                       Block<Position> positions, BlockR<Velocity> velocities) {
        for (uint i = 0; i < count; i++) {
            positions[i].Value += velocities[i].Value;
        }
    }
}

W.Query().WriteBlock<Position>().Read<Velocity>().For<MoveBlock>();
```

支持并行处理：
```csharp
W.Query().WriteBlock<Position, Velocity>().ForParallel<MoveBlock>(minEntitiesPerThread: 50000);
```

这是最高性能的数据处理方式——最小间接访问、直接指针、最优的SIMD指令利用。

---

### 查询的Fluent Builder API

结构体函数（`IQuery`/`IQueryBlock`）获得了强大的fluent builder API，明确区分可写和只读组件：

```csharp
// 可写 + 只读通过链式调用
W.Query().Write<Position>().Read<Velocity>().For<ApplyVelocity>();

// 全部可写
W.Query().Write<Position, Velocity>().For<MoveFunction>();

// 全部只读
W.Query().Read<Position, Velocity>().For<PrintPositions>();

// 块版本
W.Query().WriteBlock<Position>().Read<Velocity>().For<MoveBlock>();
```

接口类型通过嵌套实现类型级别的访问控制：
- `IQuery.Write<T0>.Read<T1>` — 前者可写（`ref`），后者只读（`in`）
- `IQueryBlock.Write<T0>.Read<T1>` — 前者为`Block<T>`，后者为`BlockR<T>`

ISystem系统现在可以同时实现IQuery：
```csharp
public struct MoveSystem : ISystem, W.IQuery.Write<Position>.Read<Velocity> {
    private float _speed;

    public void Update() {
        _speed = W.GetResource<GameConfig>().Speed;
        W.Query<TagAll<Unit>>().Write<Position>().Read<Velocity>().For(ref this);
    }

    public void Invoke(W.Entity entity, ref Position pos, in Velocity vel) {
        pos.Value += vel.Value * _speed;
    }
}
```

---

### 查询中的Or过滤器

在1.x中，过滤器仅支持And组合：所有条件必须全部匹配。在2.0中添加了`Or<>`，允许构建以前无法实现的查询：

```csharp
// 近战或远程战士——不同的组件集合
Or<All<MeleeWeapon, Damage>, All<RangedWeapon, Ammo>> fighters = default;

// 在位置发生任何变化时重建索引
Or<AllAdded<Position>, AllDeleted<Position>, AllChanged<Position>> spatialChanged = default;

// 嵌套——任意复杂的逻辑
And<All<Visible>, Or<TagAll<Unit, Alive>, TagAll<Effect, Active>>> visibleAlive = default;
```

---

## API统一与简化

### 统一的ISystem替代分散的接口

在1.x中，系统可以实现`IInitSystem`、`IUpdateSystem`、`IDestroySystem`、`ISystemCondition`、`ISystemState`的组合。在2.0中，所有内容合并为一个`ISystem`接口，包含四个可选方法：

```csharp
// 之前 (v1.2.x):
struct MoveSystem : IUpdateSystem {
    public void Update() { }
}
struct InitSystem : IInitSystem, IDestroySystem {
    public void Init() { }
    public void Destroy() { }
}
Systems.AddUpdate(new MoveSystem());
Systems.AddCallOnce(new InitSystem());

// 之后 (v2.0.0):
struct MoveSystem : ISystem {
    public void Update() { }
}
struct InitSystem : ISystem {
    public void Init() { }
    public void Destroy() { }
}
GameSys.Add(new MoveSystem(), order: 0);
GameSys.Add(new InitSystem(), order: -10);
```

未实现的方法通过反射检测，不会被调用——零开销。`UpdateIsActive()`方法替代了`ISystemCondition`：

```csharp
public struct PausableSystem : ISystem {
    public void Update() { /* ... */ }
    public bool UpdateIsActive() => !W.GetResource<GameState>().IsPaused;
}
```

系统注册现在使用fluent API和显式排序：
```csharp
GameSys.Add(new InputSystem(), order: -10)
    .Add(new MoveSystem(), order: 0)
    .Add(new RenderSystem(), order: 10);
```

---

### 统一的Query替代分裂的API

在1.x中，实体和组件的迭代分为`Query.Entities<>()`和`Query.For()`：

```csharp
// 之前：
foreach (var entity in W.Query.Entities<All<Position>>()) { }
W.Query.For((ref Position pos) => { pos.X += 1; });

// 之后：
foreach (var entity in W.Query<All<Position>>().Entities()) { }
W.Query().For(static (ref Position pos) => { pos.X += 1; });
```

统一入口点`W.Query<>()`——更清晰、更简洁、无重复。

---

### 通过默认接口方法在IComponent中定义钩子

在1.x中，每个带钩子的组件需要一个单独的Config类——一对繁琐的结构体：

```csharp
// 之前 (v1.2.x):
struct Position : IComponent { public float X, Y; }

class PositionConfig : IComponentConfig<Position, WT> {
    public OnComponentHandler<Position> OnAdd() => (ref Position c, Entity e) => { };
    public OnComponentHandler<Position> OnDelete() => ...;
    public Guid Id() => ...;
    public BinaryWriter<Position> Writer() => ...;
    public BinaryReader<Position> Reader() => ...;
    // ...还有5+个方法
}
W.RegisterComponentType<Position>(new PositionConfig());
```

在2.0中，钩子直接在组件结构体中声明：

```csharp
// 之后 (v2.0.0):
struct Position : IComponent {
    public float X, Y;

    public void OnAdd<TWorld>(World<TWorld>.Entity self)
        where TWorld : struct, IWorldType { }

    public void OnDelete<TWorld>(World<TWorld>.Entity self, HookReason reason)
        where TWorld : struct, IWorldType { }

    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType {
        writer.WriteFloat(X); writer.WriteFloat(Y);
    }

    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self,
                             byte version, bool disabled)
        where TWorld : struct, IWorldType {
        X = reader.ReadFloat(); Y = reader.ReadFloat();
    }
}

W.Types().Component<Position>(new ComponentTypeConfig<Position>(
    guid: new Guid("...")
));
```

组件配置被大大简化——`ComponentTypeConfig<T>`仅包含元数据（guid、version、strategy、defaultValue、tracking标志），所有行为逻辑都在组件本身中。

额外好处——配置可以声明为结构体中的静态字段，`RegisterAll()`会自动发现它：
```csharp
public struct Health : IComponent {
    public float Value;
    public static readonly ComponentTypeConfig<Health> Config = new(
        defaultValue: new Health { Value = 100f }
    );
}
```

**已删除：** `IComponentConfig<T,W>`、`DefaultComponentConfig<T,W>`、`ValueComponentConfig<T,W>`、`OnComponentHandler<T>`、`OnCopyHandler<T>`、钩子`OnPut`。

---

### Context重命名为Resources

全局数据存储机制获得了更直观的名称和扩展的API：

```csharp
// 之前：
W.Context.Set<GameTime>(new GameTime());
ref var time = ref W.Context.Get<GameTime>();

// 之后：
W.SetResource(new GameTime());
ref var time = ref W.GetResource<GameTime>();
bool has = W.HasResource<GameTime>();
W.RemoveResource<GameTime>();
```

新功能——**命名资源**用于存储同一类型的多个实例：
```csharp
W.SetResource("player_config", new GameConfig { Gravity = 9.81f });
W.SetResource("moon_config", new GameConfig { Gravity = 1.62f });
ref var cfg = ref W.GetResource<GameConfig>("moon_config");
```

`NamedResource<T>`——带缓存的结构体句柄：
```csharp
var moonConfig = new W.NamedResource<GameConfig>("moon_config");
ref var cfg = ref moonConfig.Value; // 第一次调用在字典中查找，后续调用——O(1)
```

---

### 关系系统重构 (Relations)

实体之间的关系系统变得更简洁和统一：

```csharp
// 之前 (v1.2.x):
struct Parent : IEntityLinkComponent<Parent> {
    public EntityGID Link;
    ref EntityGID IRefProvider<Parent, EntityGID>.RefValue(ref Parent c) => ref c.Link;
}
W.RegisterToOneRelationType<Parent>(config);
entity.SetLink<Parent>(targetGID);

// 之后 (v2.0.0):
struct ParentLink : ILinkType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType { }
    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType { }
}
W.Types().Link<ParentLink>();       // 一对一关系
W.Types().Links<ParentLink>();      // 一对多关系

entity.Set(new W.Link<ParentLink>(parentEntity));
ref var links = ref entity.Ref<W.Links<ChildLink>>();
```

包装类型`Link<T>`和`Links<T>`是标准组件。关系在查询中像普通组件一样工作，`OnAdd`/`OnDelete`钩子直接定义在`ILinkType`上。

---

### 标签与组件统一

标签和组件现在统一为单一存储系统。标签存储在 `Components<T>` 中并带有 `IsTag` 标志——不再有独立的基础设施。这简化了 API 并减少了约 7,500 行代码。

**变更内容：**
- 过滤器 `TagAll<>`、`TagNone<>`、`TagAny<>` 已删除——对标签使用 `All<>`、`None<>`、`Any<>`
- 实体方法：`SetTag` → `Set`、`HasTag` → `Has`、`DeleteTag` → `Delete`、`ToggleTag` → `Toggle`、`ApplyTag` → `Apply`
- 复制/移动：`CopyTagsTo` → `CopyTo`、`MoveTagsTo` → `MoveTo`、`CopyComponentsTo` → `CopyTo`、`MoveComponentsTo` → `MoveTo`
- 追踪过滤器：`TagAllAdded` → `AllAdded`、`TagAnyAdded` → `AnyAdded` 等——组件和标签使用相同的过滤器
- 实体检查：`HasAddedTag` → `HasAdded`、`HasDeletedTag` → `HasDeleted`
- 清除：`ClearTagTracking` → `ClearTracking`、`ClearAllTagsTracking` → `ClearAllTracking`
- 批量操作：`BatchSetTag` → `BatchSet`、`BatchDeleteTag` → `BatchDelete`、`BatchToggleTag` → `BatchToggle`、`BatchApplyTag` → `BatchApply`
- `TagsHandle` 已删除 → `ComponentsHandle`（带有 `IsTag` 字段）
- `WorldConfig.BaseTagTypesCount` 已删除——标签计入 `BaseComponentTypesCount`

---

## 完整的批量操作

在1.x中，`AddForAll`、`DeleteForAll`、`SetTagForAll`、`DestroyAllEntities`等方法更多是语法糖——内部它们逐个实体处理。在2.0中，这是一个**本质上不同的机制**：批量操作在实体块级别工作，直接操作位掩码。在最佳情况下，一次位运算就能修改64个实体——这比逐个处理快几个数量级，是世界中大规模修改实体的最快方式。

操作集扩展为完整范围：

| 方法 | 描述 |
|-------|----------|
| `BatchAdd<T>()` | 添加组件（默认值，1-5种类型） |
| `BatchSet<T>(value)` | 添加带值的组件（1-5种类型） |
| `BatchDelete<T>()` | 删除组件（1-5种类型） |
| `BatchEnable<T>()` | 启用组件（1-5种类型） |
| `BatchDisable<T>()` | 禁用组件（1-5种类型） |
| `BatchSetTag<T>()` | 设置标签（1-5种类型） |
| `BatchDeleteTag<T>()` | 删除标签（1-5种类型） |
| `BatchToggleTag<T>()` | **新增：**切换标签（1-5种类型） |
| `BatchApplyTag<T>(bool)` | **新增：**条件设置/移除（1-5种类型） |
| `BatchDestroy()` | 销毁实体 |
| `BatchUnload()` | **新增：**卸载实体 |
| `EntitiesCount()` | 计算数量 |

支持链式调用：
```csharp
W.Query<All<Position>>()
    .BatchSet(new Velocity { Value = Vector3.One })
    .BatchSetTag<IsMovable>()
    .BatchDisable<Position>();
```

`UnloadCluster()`/`UnloadChunk()`已删除——由灵活的`BatchUnload()`带过滤替代：
```csharp
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { clusterId };
W.Query().BatchUnload(EntityStatusType.Any, clusters: clusters);
```

---

## 语义和API变更

### Add/Set — 新的明确语义

在1.x中存在三个含义不明确的方法：`Add`（断言）、`TryAdd`（幂等）、`Put`（覆写）。在2.0中保留两个，语义明确划分：

| 方法 | 行为 |
|-------|-----------|
| `Add<T>()` | **幂等**（原`TryAdd`）。如果组件存在——返回ref，不调用钩子。如果不存在——默认初始化 + `OnAdd`。 |
| `Set(value)` | **始终覆写**（原`Put`，但现在带钩子）。如果组件存在——`OnDelete(旧值)` → 替换 → `OnAdd(新值)`。如果不存在——设置 → `OnAdd`。 |

```csharp
entity.Set(new Position { Value = Vector3.Zero }); // 设置
entity.Add<Position>();                             // 不执行任何操作——返回ref
entity.Set(new Position { Value = Vector3.One });   // OnDelete(旧值) → 替换 → OnAdd(新值)
```

### Delete/Disable/Enable — 可预测的返回值

```csharp
// 之前：
entity.Delete<C>();              // void，如果不存在则断言
bool ok = entity.TryDelete<C>(); // bool

// 之后：
bool deleted = entity.Delete<C>();           // bool（原TryDelete）
ToggleResult = entity.Disable<C>();          // MissingComponent, Unchanged, Changed
ToggleResult = entity.Enable<C>();           // MissingComponent, Unchanged, Changed
```

### 方法 → 属性

所有无参数的getter方法变为属性：

```csharp
// Entity:
entity.GID           // 之前：entity.Gid()
entity.IsDestroyed   // 之前：entity.IsDestroyed()
entity.IsDisabled    // 之前：entity.IsDisabled()
entity.Version       // 之前：entity.Version()
entity.ClusterId     // 之前：entity.ClusterId()
entity.ChunkID       // 之前：entity.Chunk()
entity.EntityType    // 新增：byte — 实体类型ID

// World:
W.IsWorldInitialized // 之前：W.IsInitialized()
W.IsIndependent      // 之前：W.IsIndependent()
W.Status             // 新增：WorldStatus枚举
```

### 简化存在性检查

```csharp
// 组件：
entity.Has<C>()          // 之前：entity.HasAllOf<C>()
entity.Has<C1, C2>()     // 之前：entity.HasAllOf<C1, C2>()
entity.HasAny<C1, C2>()  // 之前：entity.HasAnyOf<C1, C2>()
entity.HasDisabled<C>()  // 之前：entity.HasDisabledAllOf<C>()

// 标签：
entity.HasTag<T>()          // 之前：entity.HasAllOfTags<T>()
entity.HasAnyTags<T1, T2>() // 之前：entity.HasAnyOfTags<T1, T2>()
```

### 访问池

```csharp
Components<T>.Instance  // 之前：Components<T>.Value
Tags<T>.Instance        // 之前：Tags<T>.Value
```

---

## 类型注册

### 统一注册器 W.Types()

在1.x中，每种类型都通过自己的方法注册：
```csharp
W.RegisterComponentType<Position>(new PositionConfig());
W.RegisterToOneRelationType<Parent>(config);
W.RegisterMultiComponentType<Items, int>(4);
```

在2.0中，所有注册通过fluent `W.Types()`完成：
```csharp
W.Types()
    .Component<Position>()
    .Component<Health>(new ComponentTypeConfig<Health>(defaultValue: new Health { Value = 100 }))
    .Tag<Unit>()
    .Tag<Poisoned>(new TagTypeConfig<Poisoned>(trackAdded: true))
    .Event<DamageEvent>(new EventTypeConfig<DamageEvent>(guid: new Guid("...")))
    .Link<ParentLink>()
    .Links<ChildrenLinks>()
    .Multi<Item>()
    .EntityType<Bullet>(Bullet.Id)
    .EntityType<Enemy>(Enemy.Id);
```

`RegisterAll()`仍然可用于自动发现程序集中的所有类型。

---

## 创建实体

创建API从`Entity.New()`移至世界级别，并获得了类型化：

```csharp
// 之前：
var entity = W.Entity.New();
var entity = W.Entity.New<Position>(new Position());
W.Entity.NewOnes(count, onCreate);
bool ok = W.Entity.TryNew(out entity);

// 之后：
var entity = W.NewEntity<Default>();
var entity = W.NewEntity<Bullet>(clusterId: 5);
W.NewEntities<Default>(count: 100, onCreate: null);
bool ok = W.TryNewEntity<Default>(out entity);
var entity = W.NewEntity<Default>().Set(
    new Position { Value = Vector3.One },
    new Velocity { Value = 1f }
);
```

---

## 更新的性能与文档

### 新的性能页面

性能文档已全面重写。现在包括：

1. **架构分析** — 与基于原型的ECS（Unity DOTS、Flecs、Bevy）和稀疏集ECS（EnTT、DefaultEcs）在各方面的详细比较
2. **迭代方式层级** — 从最快到最方便：
   - `ForBlock` — 块指针（unmanaged最快）
   - `For`搭配IQuery结构体（无分配，带状态）
   - `For`搭配委托（使用static lambda无分配）
   - `foreach`（最灵活）
3. **代码精简/trimming** — 减小程序集大小的建议
4. **二维分区** — EntityType × Cluster缓存局部性说明

### 新的"常见错误"页面 (pitfalls.md)

全新的文档章节，系统化地整理了典型错误：

- **生命周期错误** — 忘记注册类型、在Initialize之前执行操作
- **Entity错误** — Destroy之后使用、跨帧存储
- **组件错误** — Add vs Set语义、空钩子、HasOnDelete vs DataLifecycle
- **查询错误** — 违反Strict模式、不必要的Flexible
- **注册错误** — MultiComponent没有包装器、缺少序列化
- **资源错误** — 在readonly字段中缓存NamedResource

### AI代理指南 (aiagentguide.md)

全新章节——为CLAUDE.md和其他AI助手提供的代码片段，包括：
- 世界和系统设置模式
- 严格的生命周期顺序
- Entity使用的关键规则
- 典型代码模式
- llms.txt链接

---

## 事件 (Events)

事件系统从`W.Events`级别移至世界级别，并在IEvent中获得了钩子：

```csharp
// 之前：
W.Events.Send(new DamageEvent { Amount = 10 });
var receiver = W.Events.RegisterEventReceiver<DamageEvent>();
W.Events.DeleteEventReceiver(ref receiver);

// 之后：
W.SendEvent(new DamageEvent { Amount = 10 });
var receiver = W.RegisterEventReceiver<DamageEvent>();
W.DeleteEventReceiver(ref receiver);
```

`IEventConfig<T,W>`已删除——通过`EventTypeConfig<T>`配置，Write/Read钩子直接在IEvent中：
```csharp
struct DamageEvent : IEvent {
    public int Amount;
    public void Write(ref BinaryPackWriter writer) { writer.WriteInt(Amount); }
    public void Read(ref BinaryPackReader reader, byte version) { Amount = reader.ReadInt(); }
}
```

---

## 多组件

接口被简化——`IMultiComponent<T, V>`变为标记接口`IMultiComponent`：

```csharp
// 之前：
struct Items : IMultiComponent<Items, int> {
    public Multi<int> Values;
    public ref Multi<int> RefValue(ref Items c) => ref c.Values;
}
W.RegisterMultiComponentType<Items, int>(4);

// 之后：
struct Items : IMultiComponent {
    public Multi<int> Values;
}
W.Types().Multi<Item>();
```

API变更：`Count` → `Length`，方法`IsEmpty()`/`IsFull()` → 属性。

---

## 序列化

Write/Read钩子从Config类移至IComponent/IEvent中。这是主要的架构变更——组件的所有行为逻辑现在都在一个地方。

```csharp
// 之前——单独的Config类：
class PositionConfig : DefaultComponentConfig<Position, WT> {
    public override BinaryWriter<Position> Writer() => ...;
    public override BinaryReader<Position> Reader() => ...;
}

// 之后——结构体中的钩子：
struct Position : IComponent {
    public float X, Y;
    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType { writer.WriteFloat(X); writer.WriteFloat(Y); }
    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self,
                             byte version, bool disabled)
        where TWorld : struct, IWorldType { X = reader.ReadFloat(); Y = reader.ReadFloat(); }
}
```

事件快照已迁移：
```csharp
// 之前：
W.Events.CreateSnapshot();
W.Events.LoadSnapshot(snapshot);

// 之后：
W.Serializer.CreateEventsSnapshot();
W.Serializer.LoadEventsSnapshot(snapshot);
```

---

## 组件只读访问

在1.x中，查询中的所有组件都是`ref`。在2.0中引入了明确的区分：

```csharp
// 委托——ref（写入）vs in（读取）
W.Query().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;  // Position可写，Velocity只读
});

// 查询外部
ref var pos = ref entity.Ref<Position>();           // 可变，不标记Changed（快速路径）
ref var tracked = ref entity.Mut<Position>();       // 可变，标记Changed
ref readonly var vel = ref entity.Read<Velocity>(); // 只读，不标记Changed
```

这与变更追踪系统集成——`Ref`/`Read`不会触发Changed的误报，`Mut`显式将组件标记为已更改。

---

## WorldConfig — 新参数

```csharp
new WorldConfig {
    // 已有：
    BaseComponentTypesCount = 64,
    BaseTagTypesCount = 64,
    ParallelQueryType = ParallelQueryType.Disabled,
    Independent = true,

    // 2.0新增：
    WorkerSpinCount = 256,        // 线程阻塞前的自旋等待迭代次数
    BaseClustersCapacity = 16,    // 集群数组的初始容量
    TrackCreated = false,         // 全局实体创建追踪
}

// 工厂方法：
WorldConfig.Default()     // 标准设置
WorldConfig.MaxThreads()  // 所有可用线程
```

---

## 新编译器指令

`FFS_ECS_DISABLE_CHANGED_TRACKING` — 在编译阶段移除所有Changed追踪的代码路径，包括过滤器`AllChanged`、`NoneChanged`、`AnyChanged`以及`Mut<T>()`方法。

---

## 已删除的API

| 已删除 | 替代方案 |
|---------|--------|
| `IWorld`接口 | `WorldHandle` |
| `WorldWrapper<W>` | `WorldHandle` |
| `Worlds`静态类 | — |
| `BoxedEntity<W>` / `IEntity` / `entity.Box()` | — |
| `entity.TryAdd<C>()` | `entity.Add<C>()` |
| `entity.Put<C>(val)` | `entity.Set<C>(val)` |
| `entity.TryDelete<C>()` | `entity.Delete<C>()` |
| 所有`Raw`方法的Entity | — |
| `Entity.New(...)` | `W.NewEntity<TEntityType>(...)` |
| `IInitSystem` / `IUpdateSystem` / `IDestroySystem` | `ISystem` |
| `ISystemCondition` / `ISystemState` | `ISystem.UpdateIsActive()` |
| `Systems.AddUpdate()` / `AddCallOnce()` | `Sys.Add(system, order)` |
| `IComponentConfig<T,W>` | `ComponentTypeConfig<T>` + IComponent中的钩子 |
| `IEventConfig<T,W>` | `EventTypeConfig<T>` + IEvent中的钩子 |
| `IEntityLinkComponent<T>` | `ILinkType` + `Link<T>` |
| `IEntityLinksComponent<T>` | `ILinksType` + `Links<T>` |
| `IMultiComponent<T,V>` | `IMultiComponent`（标记接口） |
| `DeleteTagsSystem<W, T>` | `Query.BatchDeleteTag<T>()` |
| `OnComponentHandler<T>` / `OnCopyHandler<T>` | IComponent中的钩子 |
| `W.UnloadCluster()` / `W.UnloadChunk()` | `Query().BatchUnload()` |
| `W.Events.XXX` | `W.XXX` |
| `W.Context.Set/Get/Has` | `W.SetResource/GetResource/HasResource` |

---

## 快速重命名对照表

| 之前 (v1.2.x) | 之后 (v2.0.0) |
|---|---|
| `W.Entity.New(...)` | `W.NewEntity<TEntityType>(...)` |
| `W.Entity.NewOnes(...)` | `W.NewEntities<TEntityType>(count)` |
| `W.IsInitialized()` | `W.IsWorldInitialized` |
| `entity.Gid()` | `entity.GID` |
| `entity.HasAllOf<C>()` | `entity.Has<C>()` |
| `entity.HasAnyOf<C1,C2>()` | `entity.HasAny<C1,C2>()` |
| `entity.HasAllOfTags<T>()` | `entity.HasTag<T>()` |
| `Components<T>.Value` | `Components<T>.Instance` |
| `Tags<T>.Value` | `Tags<T>.Instance` |
| `W.Query.Entities<F>()` | `W.Query<F>()` |
| `W.Query.For(...)` | `W.Query().For(...)` |
| `AddForAll<C>()` | `BatchAdd<C>()` |
| `DeleteForAll<C>()` | `BatchDelete<C>()` |
| `SetTagForAll<T>()` | `BatchSetTag<T>()` |
| `DestroyAllEntities()` | `BatchDestroy()` |
| `Multi<T>.Count` | `Multi<T>.Length` |

---

## 总结

StaticEcs 2.0 是从快速但相对分散的API到统一的、根本性扩展的框架的转变。主要成就：

- **变更追踪** — 16个过滤器、位图存储、禁用时零开销。网络同步、响应式UI、触发器——无需手动dirty标志。
- **实体类型** — 逻辑和物理分组。开箱即用的缓存局部性、生命周期钩子、参数化创建。
- **块迭代** — 直接指向数据数组的指针，实现unmanaged代码的最大性能。
- **统一的ISystem** — 一个接口替代五个，自动检测已实现的方法。
- **IComponent中的钩子** — 组件行为集中在一处，无需Config类。
- **Or过滤器** — 以前无法实现的查询。
- **扩展的批量操作** — 完整范围，支持链式调用。
- **清晰、统一的API** — 属性替代方法、简短命名、fluent注册。

迁移将需要修改几乎所有用户代码，但每一处改动都是朝着更简洁、更快速、更具表达力的API迈进的一步。详细的迁移指南及对照表请参阅[migrationguide](docs/zh/migrationguide.md)。
