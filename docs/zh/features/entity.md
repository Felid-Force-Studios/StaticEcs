---
title: 实体
parent: 功能
nav_order: 1
---

## Entity
实体是用于在世界中标识对象并访问其组件和标签的结构体
- 4 字节结构体（基于槽位索引的 `uint` 包装器）
- 不包含代计数器 — 如需持久引用请使用 [EntityGID](gid.md)
- 所有组件和标签操作均可通过实体自身的方法访问

___

## 实体类型（IEntityType）

实体类型是创建时分配的逻辑类别。它决定了实体的用途（单位、子弹、特效），并控制内存布局 — 同一集群内相同类型的实体存储在相同的段中。

### 定义实体类型

实体类型定义为实现 `IEntityType` 的结构体，并带有稳定的 `byte Id()` 方法：

```csharp
public struct Bullet : IEntityType {
    public byte Id() => 1;
}

public struct Enemy : IEntityType {
    public byte Id() => 2;
}

public struct Effect : IEntityType {
    public byte Id() => 3;
}
```

内置类型 `Default`（Id = 0）在创建世界时自动注册。

### 注册

实体类型在 `Created` 阶段注册。

**手动注册：**
```csharp
W.Types()
    .EntityType<Bullet>()
    .EntityType<Enemy>()
    .EntityType<Effect>();
```

**自动注册：**
```csharp
W.Types().RegisterAll();
```

`RegisterAll()` 会在指定程序集（默认为 `typeof(TWorld).Assembly`；不使用调用栈回溯，在 Unity IL2CPP、Unity WebGL 和 NativeAOT 上都安全）中查找所有实现 `IEntityType` 的类型并自动注册。标识符通过 `Id()` 方法获取。

### 生命周期钩子（OnCreate / OnDestroy）

实体类型可以定义 `OnCreate` 和 `OnDestroy` 钩子。如果方法未在结构体中定义，则不会被调用。无需保留空实现。

```csharp
public struct Bullet : IEntityType {
    public byte Id() => 1;

    public void OnCreate<TWorld>(World<TWorld>.Entity entity) where TWorld : struct, IWorldType {
        entity.Set(new Velocity { Speed = 100 });
        entity.Set<Active>();
    }

    public void OnDestroy<TWorld>(World<TWorld>.Entity entity, HookReason reason) where TWorld : struct, IWorldType {
        // 清理逻辑、发送事件等。
        // 此时所有组件和标签仍然可访问。
    }
}
```

### 带数据的实体类型

由于 `IEntityType` 是结构体，它可以包含字段。`OnCreate` 是实例方法 — 通过 `this` 访问字段。这允许无需额外参数或分配即可参数化创建：

```csharp
public struct Flora : IEntityType {
    public byte Id() => 4;

    public enum Kind : byte { Grass, Bush, Tree }
    public Kind FloraKind;

    public void OnCreate<TWorld>(World<TWorld>.Entity entity) where TWorld : struct, IWorldType {
        entity.Set(new Health { Value = FloraKind == Kind.Tree ? 100 : 10 });
    }
}

// 用法：
var tree = W.NewEntity(new Flora { FloraKind = Flora.Kind.Tree });
var grass = W.NewEntity(new Flora { FloraKind = Flora.Kind.Grass });
```

### 为什么重要

**迭代时的数据局部性。** 组件存储在 SoA 数组中。当相同类型的实体位于同一段中时，它们的组件在内存中连续排列 — CPU 高效利用缓存行。

**减少碎片化。** 无类型化时，不同种类的实体会交错创建。使用类型后，被销毁子弹留下的空洞会被新子弹填充 — 段保持同质性。

**查询过滤。** 实体类型过滤器（`EntityIs<T>`、`EntityIsNot<T>`、`EntityIsAny<T0,T1>`）在段级别工作，每个实体零开销 — `FilterEntities` 是 no-op。这是系统中最廉价的过滤器类型。

### entityType 和 clusterId

这两个参数相互补充：

- **`entityType`** — **逻辑**分组：定义实体*是什么*（单位、子弹、特效）。影响内存布局 — 相同类型的实体存储在一起以实现最佳迭代性能。
- **`clusterId`** — **空间**分组：定义实体*在哪里*（关卡、地图区域、房间）。允许将查询限制在世界的特定区域，并管理流式加载。

分段在这两个参数的交叉点上工作：在每个集群内，为每种类型分配单独的段。

___

## 创建

```csharp
// 使用默认类型创建实体（Id = 0）
W.Entity entity = W.NewEntity<Default>();

// 使用特定类型 — OnCreate 钩子自动调用
W.Entity entity = W.NewEntity<Bullet>();
W.Entity entity = W.NewEntity<Enemy>(clusterId: LEVEL_1_CLUSTER);

// 在实体类型结构体中携带数据
W.Entity entity = W.NewEntity(new Flora { FloraKind = Flora.Kind.Tree });

// 使用组件 — Set 返回 Entity，支持链式调用
W.Entity entity = W.NewEntity<Bullet>().Set(new Position { Value = Vector3.One });
W.Entity entity = W.NewEntity<Bullet>().Set(
    new Position { Value = Vector3.One },
    new Velocity { Value = 10f },
    new Damage { Value = 5 }
);

// 在指定区块中创建
W.Entity entity = W.NewEntityInChunk<Bullet>(chunkIdx: chunkIdx);

// 通过 GID 创建（用于反序列化和网络同步）
W.Entity entity = W.NewEntityByGID<Default>(gid);

// 非泛型重载（实体类型仅在运行时已知，例如反序列化时）
byte entityTypeId = EntityTypeInfo<Bullet>.Id;
W.Entity entity = W.NewEntity(entityTypeId, clusterId: LEVEL_1_CLUSTER);
W.Entity entity = W.NewEntityInChunk(entityTypeId, chunkIdx: chunkIdx);
W.Entity entity = W.NewEntityByGID(entityTypeId, gid);
```

#### 在依赖世界中创建（Try）：

{: .notezh }
依赖世界（`Independent = false`）与其他世界共享槽位空间。如果分配给它的槽位已耗尽，则无法创建实体。

```csharp
// 如果依赖世界中分配的槽位已耗尽则返回 false
if (W.TryNewEntity<Bullet>(out var entity, clusterId: LEVEL_1_CLUSTER)) {
    entity.Set(new Position { Value = Vector3.Zero });
}
```

___

## 批量创建

```csharp
uint count = 1000;

// 不带组件
W.NewEntities<Default>(count);

// 按类型指定组件（1 到 5 个类型）
W.NewEntities<Default, Position>(count);
W.NewEntities<Default, Position, Velocity>(count);

// 按值指定组件（1 到 8 个组件）
W.NewEntities<Default>(count, new Position { Value = Vector3.Zero });
W.NewEntities<Default>(count,
    new Position { Value = Vector3.Zero },
    new Velocity { Value = 1f }
);

// 带每个实体的初始化委托
W.NewEntities<Default, Position>(count, onCreate: static entity => {
    entity.Set<Unit>();
});

// 指定集群
W.NewEntities<Default, Position>(count, clusterId: LEVEL_1_CLUSTER);

// 完整重载：值 + 集群 + 委托
W.NewEntities<Default>(count,
    new Position { Value = Vector3.Zero },
    clusterId: LEVEL_1_CLUSTER,
    onCreate: static entity => {
        entity.Set<Unit>();
    }
);
```

___

## 属性

```csharp
W.Entity entity = W.NewEntity<Bullet>();

uint id = entity.ID;                         // 内部槽位索引
EntityGID gid = entity.GID;                  // 全局标识符（8 字节）
EntityGIDCompact gidC = entity.GIDCompact;   // 紧凑标识符（4 字节）
ushort version = entity.Version;             // 槽位代计数器
ushort clusterId = entity.ClusterId;         // 集群标识符
byte entityType = entity.EntityType;         // 实体类型（0–255）
uint chunkId = entity.ChunkID;              // 区块索引

bool alive = entity.IsNotDestroyed;          // 未销毁
bool destroyed = entity.IsDestroyed;         // 已销毁
bool enabled = entity.IsEnabled;             // 已启用（参与查询）
bool disabled = entity.IsDisabled;           // 已禁用
bool selfOwned = entity.IsSelfOwned;         // 段属于此世界

// 实体类型检查
bool isBullet = entity.Is<Bullet>();                    // 精确类型匹配
bool isProjectile = entity.IsAny<Bullet, Rocket>();     // 任一类型
bool isNotEffect = entity.IsNot<Effect>();              // 非此类型
bool isNotVfx = entity.IsNot<Effect, Particle>();       // 非这些类型中的任何一个

string info = entity.PrettyString;           // 调试字符串
```

___

## 生命周期

```csharp
// 禁用实体 — 从标准查询中排除，但保留所有数据
entity.Disable();

// 重新启用
entity.Enable();

// 销毁 — 删除所有组件（触发 OnDelete 钩子）、标签，释放槽位
// 返回 bool：如果实体存活并被销毁返回 true，如果已被销毁返回 false
entity.Destroy();

// 从内存中卸载 — 实体变为不可见，但其 ID 被保留
// 用于流式加载（临时卸载，之后通过序列化重新加载）
entity.Unload();

// 递增版本而不销毁 — 所有之前获取的 GID 将变为无效
entity.UpVersion();
```

___

## 克隆和转移

```csharp
// 克隆实体 — 创建具有所有组件和标签副本的新实体
W.Entity clone = entity.Clone();

// 克隆到其他集群
W.Entity clone = entity.Clone(clusterId: OTHER_CLUSTER);

// 将所有组件和标签复制到现有实体
// 如果目标实体已有相同组件 — 它们将被覆盖
entity.CopyTo(targetEntity);

// 将所有数据转移到现有实体并销毁源实体
entity.MoveTo(targetEntity);

// 转移到其他集群 — 创建新实体，复制数据，销毁源实体
W.Entity moved = entity.MoveTo(clusterId: OTHER_CLUSTER);
```

___

## 组件

组件 API 请参阅[组件](component.md)章节。

___

## 标签

标签 API 请参阅[标签](tag.md)章节。

___

## 多组件

多组件 API 请参阅[多组件](multicomponent.md)章节。

___

## 关系

实体关系 API 请参阅[关系](relations.md)章节。

___

## 调试

```csharp
// 包含完整信息的调试字符串
string info = entity.PrettyString;

// 组件数量（已启用 + 已禁用）
int compCount = entity.ComponentsCount();

// 标签数量
int tagCount = entity.TagsCount();

// 获取所有组件（列表在填充前会被清空）
var components = new List<IComponent>();
entity.GetAllComponents(components);

// 获取所有标签（列表在填充前会被清空）
var tags = new List<ITag>();
entity.GetAllTags(tags);
```

___

## 按实体类型的查询过滤器

实体类型过滤器请参阅[查询 — 实体类型](query.md#实体类型)章节。

___

## 运算符和转换

```csharp
W.Entity a = W.NewEntity<Default>();
W.Entity b = W.NewEntity<Default>();

// 按槽位索引比较（不检查版本）
bool eq = a == b;
bool neq = a != b;

// 隐式转换 Entity → EntityGID（8 字节）
EntityGID gid = entity;

// 显式转换 Entity → EntityGIDCompact（4 字节）
// 在 DEBUG 中如果 Chunk >= 4 或 ClusterId >= 4 将抛出错误
EntityGIDCompact compact = (EntityGIDCompact)entity;

// 转换为类型化链接（用于关系系统）
Link<ChildOf> link = entity.AsLink<ChildOf>();
```
