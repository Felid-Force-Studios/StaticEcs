---
title: 世界
parent: 功能
nav_order: 9
---

## WorldType
世界标识类型标签，用于在同一进程中创建不同世界时隔离静态数据
- 以不包含数据的用户自定义结构体和 `IWorldType` 标记接口表示
- 每个唯一的 `IWorldType` 获得完全隔离的静态存储

#### 示例：
```csharp
public struct MainWorldType : IWorldType { }
public struct MiniGameWorldType : IWorldType { }
```

___

## World
库的入口点，负责世界数据的访问、创建、初始化、运行和销毁
- 以参数化 `IWorldType` 的静态类 `World<T>` 表示

{: .importantzh }
> 由于类型标识符 `IWorldType` 定义了对特定世界的访问，
> 有三种使用框架的方式：

___

#### 第一种方式 — 完整限定：
```csharp
public struct WT : IWorldType { }

World<WT>.Create(WorldConfig.Default());
World<WT>.CalculateEntitiesCount();

var entity = World<WT>.NewEntity<Default>();
```

#### 第二种方式 — 静态导入：
```csharp
using static FFS.Libraries.StaticEcs.World<WT>;

public struct WT : IWorldType { }

Create(WorldConfig.Default());
CalculateEntitiesCount();

var entity = NewEntity<Default>();
```

#### 第三种方式 — 在根命名空间中使用类型别名：
所有示例都将使用此方式
```csharp
public struct WT : IWorldType { }

public abstract class W : World<WT> { }

W.Create(WorldConfig.Default());
W.CalculateEntitiesCount();

var entity = W.NewEntity<Default>();
```

___

## 生命周期

```
Create() → 类型注册 → Initialize() → 运行 → Destroy()
```

#### WorldStatus：
- `NotCreated` — 世界未创建或已销毁
- `Created` — 结构已分配，可以注册类型
- `Initialized` — 世界完全就绪，可以进行实体操作

___

#### 创建世界：
```csharp
// 定义世界标识符
public struct WT : IWorldType { }
public abstract class W : World<WT> { }

// 使用默认配置创建
W.Create(WorldConfig.Default());

// 或使用自定义配置（所有参数均为可选 — 未设置的值使用默认值）
W.Create(new WorldConfig {
    // 独立世界（自动管理块）或依赖世界（需要手动管理块）
    Independent = true,
    // 组件类型的初始容量（默认 — 64）
    BaseComponentTypesCount = 64,
    // 集群的初始容量（最小 16，默认 — 16）
    BaseClustersCapacity = 16,
    // 并行查询的线程数（默认 — 0，单线程）
    // 0 — 不创建线程
    // WorldConfig.MaxThreadCount — 使用所有可用 CPU 线程
    // N — 使用指定数量的线程
    ThreadCount = 4,
    // 工作线程阻塞前的自旋次数（默认 — 256）
    WorkerSpinCount = 256,
    // 启用实体创建追踪，用于 Created 查询过滤器（默认 — false）
    TrackCreated = true,
});
```

{: .notezh }
`WorldConfig` 提供工厂方法：
- `WorldConfig.Default()` — 标准设置（单线程，独立）
- `WorldConfig.MaxThreads()` — 使用所有可用 CPU 线程
所有参数均为可选 — 未设置的值使用 `WorldConfig.Default()` 的默认值。

___

#### 类型注册：
```csharp
W.Create(WorldConfig.Default());

// 注册组件、标签和事件 — 仅在 Create() 和 Initialize() 之间
W.Types()
    .EntityType<Bullet>()
    .Component<Position>()
    .Component<Velocity>()
    .Tag<IsPlayer>()
    .Event<OnDamage>();

// 初始化世界
W.Initialize();
```

{: .importantzh }
类型注册（`W.Types().Component<T>()`、`W.Types().Tag<T>()`、`W.Types().EntityType<T>()`）仅在 `Created` 状态下可用 — 在 `Create()` 之后、`Initialize()` 之前。事件注册（`W.Types().Event<T>()`）在初始化之后也可用。

___

#### 类型自动注册：
可以使用自动程序集扫描来替代手动注册每个类型。
`RegisterAll()` 会发现所有实现 ECS 接口的结构体并自动注册：

```csharp
W.Create(WorldConfig.Default());

// 从调用程序集自动注册所有类型
W.Types().RegisterAll();

// 或指定特定程序集
W.Types().RegisterAll(typeof(MyGame).Assembly, typeof(MyPlugin).Assembly);

// 可以与手动注册结合使用
W.Types()
    .RegisterAll()
    .Component<SpecialComponent>();

W.Initialize();
```

检测的接口：

| 接口 | 注册方式 |
|------|---------|
| `IComponent` | `Types().Component<T>()` |
| `ITag` | `Types().Tag<T>()` |
| `IEvent` | `Types().Event<T>()` |
| `ILinkType` | 包装为 `Link<T>` 并作为组件注册 |
| `ILinksType` | 包装为 `Links<T>` 并作为组件注册 |
| `IMultiComponent` | 包装为 `Multi<T>` 并作为组件注册 |
| `IEntityType` | `Types().EntityType<T>()` |

{: .notezh }
- 如果未指定程序集，则仅扫描调用程序集（不是所有已加载的程序集）
- StaticEcs 框架程序集本身始终被排除在扫描之外
- `RegisterAll()` 会在每个结构体内搜索对应配置类型的静态字段或属性，如果找到则使用它。否则使用默认配置。查找规则：
  - `IComponent` — 查找 `ComponentTypeConfig<T>`（优先选择名为 `Config` 的成员）
  - `IEvent` — 查找 `EventTypeConfig<T>`（优先选择名为 `Config` 的成员）
  - `ITag` — 查找 `TagTypeConfig<T>`（优先选择名为 `Config` 的成员）
  - `IEntityType` — 查找 `byte`（优先选择名为 `Id` 的成员）
- 同时支持字段（field）和属性（property）
- 实现多个接口的结构体（例如同时实现 `IComponent` 和 `IMultiComponent`）将为每个接口分别注册

___

#### 初始化：
```csharp
// 标准初始化（baseEntitiesCapacity — 实体的初始容量）
W.Initialize(baseEntitiesCapacity: 4096);

// 使用恢复的实体标识符初始化（EntityGID 版本）
W.InitializeFromGIDStoreSnapshot(snapshot);

// 从快照完整恢复世界初始化
W.InitializeFromWorldSnapshot(snapshot);
```

{: .notezh }
`InitializeFromGIDStoreSnapshot` 仅恢复实体标识符元数据（GID 版本）。`InitializeFromWorldSnapshot` 恢复完整的世界状态，包括所有实体及其数据。

___

#### 销毁：
```csharp
// 销毁世界并释放所有资源
W.Destroy();
```

___

## 基本操作

```csharp
// 当前世界状态
WorldStatus status = W.Status;

// 世界是否已初始化
bool initialized = W.IsWorldInitialized;

// 世界是否为独立世界
bool independent = W.IsIndependent;

// 世界中的实体数量（活跃 + 未加载）
uint entitiesCount = W.CalculateEntitiesCount();

// 已加载实体数量
uint loadedCount = W.CalculateLoadedEntitiesCount();

// 当前实体容量
uint capacity = W.CalculateEntitiesCapacity();

// 销毁世界中的所有实体（世界保持初始化状态）
W.DestroyAllLoadedEntities();
```

___

关于创建实体和实体操作的详细信息 — 请参阅[实体](entity)。

关于世界资源的详细信息 — 请参阅[资源](resources)。

___

## 集群

集群是用于世界空间分割的实体块组。同一集群中的实体被分组并在内存中分段存储。
- 以 `ushort` 值表示（0–65535）
- 默认情况下，世界初始化时创建标识符为 0 的集群
- 所有实体默认在集群 0 中创建
- 集群可以被禁用 — 禁用集群中的实体不会出现在迭代中

{: .notezh }
集群用于**空间分组**：关卡、地图区域、游戏房间。对于**逻辑**分组（单位、子弹、特效），请使用 `entityType`。

___

#### 基本操作：
```csharp
// 注册集群（可在 Create() 或 Initialize() 之后调用）
const ushort LEVEL_1_CLUSTER = 1;
const ushort LEVEL_2_CLUSTER = 2;
W.RegisterCluster(LEVEL_1_CLUSTER);
W.RegisterCluster(LEVEL_2_CLUSTER);

// 检查集群是否已注册
bool registered = W.ClusterIsRegistered(LEVEL_1_CLUSTER);

// 启用或禁用集群 — 禁用集群中的实体不参与迭代
W.SetActiveCluster(LEVEL_2_CLUSTER, false);

// 检查集群是否已启用
bool active = W.ClusterIsActive(LEVEL_2_CLUSTER);

// 销毁集群中的所有实体
W.DestroyAllEntitiesInCluster(LEVEL_1_CLUSTER);

// 释放集群 — 所有实体被删除，块和标识符被释放
W.FreeCluster(LEVEL_2_CLUSTER);

// 安全释放 — 如果集群未注册则返回 false
bool freed = W.TryFreeCluster(LEVEL_2_CLUSTER);
```

___

#### 集群快照和卸载：
```csharp
// 创建集群快照（存储所有实体数据）
// 提供写入磁盘、压缩等重载
byte[] snapshot = W.Serializer.CreateClusterSnapshot(LEVEL_1_CLUSTER);

// 从内存中卸载集群
// 组件和标签数据被移除，实体被标记为未加载
// 仅保留标识符信息，实体不会出现在查询中
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { LEVEL_1_CLUSTER };
W.Query().BatchUnload(EntityStatusType.Any, clusters: clusters);

// 从快照加载集群
W.Serializer.LoadClusterSnapshot(snapshot);
```

___

#### 集群中的块：
```csharp
// 获取集群中的所有块（包括空块）
ReadOnlySpan<uint> chunks = W.GetClusterChunks(LEVEL_1_CLUSTER);

// 获取至少有一个已加载实体的块
ReadOnlySpan<uint> loadedChunks = W.GetClusterLoadedChunks(LEVEL_1_CLUSTER);
```

___

#### 在集群中创建实体：
```csharp
// 创建实体时指定集群（默认 — 集群 0）
struct UnitType : IEntityType { }
var entity = W.NewEntity<UnitType>(clusterId: LEVEL_1_CLUSTER);

// 所有重载都支持 clusterId 参数
W.NewEntity<UnitType>(
    new UnitType(),  // 实体类型实例（可以携带 OnCreate 的配置数据）
    clusterId: LEVEL_1_CLUSTER
);

// 获取实体的集群
ushort entityClusterId = entity.ClusterId;

// 从 EntityGID 获取集群
ushort gidClusterId = entity.GID.ClusterId;
```

___

## 块

块是包含 4096 个实体的单元。整个世界由块组成。每个块属于一个集群。

- **独立世界**（`Independent = true`）— 自动管理块，按需创建新块
- **依赖世界**（`Independent = false`）— 没有可用于通过 `NewEntity()` 创建实体的块，必须显式指定可用块

___

#### 基本操作：
```csharp
// 查找不属于任何集群的空闲块
// 独立世界：如果没有空闲块 — 创建新块
// 依赖世界：如果没有空闲块 — 错误
EntitiesChunkInfo chunkInfo = W.FindNextSelfFreeChunk();
uint chunkIdx = chunkInfo.ChunkIdx;
// chunkInfo.EntitiesFrom — 块中第一个实体标识符
// chunkInfo.EntitiesCapacity — 块大小（始终为 4096）

// 安全变体（没有空闲块时返回 false）
bool found = W.TryFindNextSelfFreeChunk(out EntitiesChunkInfo info);

// 在集群中注册块
W.RegisterChunk(chunkIdx, clusterId: LEVEL_1_CLUSTER);

// 注册块并指定所有权类型
W.RegisterChunk(chunkIdx, owner: ChunkOwnerType.Self, clusterId: LEVEL_1_CLUSTER);

// 安全注册（如果块已注册则返回 false）
bool registered = W.TryRegisterChunk(chunkIdx, clusterId: LEVEL_1_CLUSTER);

// 检查块是否已注册
bool isRegistered = W.ChunkIsRegistered(chunkIdx);

// 获取块所属的集群
ushort clusterId = W.GetChunkClusterId(chunkIdx);

// 将块移动到另一个集群
W.ChangeChunkCluster(chunkIdx, LEVEL_2_CLUSTER);

// 检查块中是否有实体
bool hasEntities = W.HasEntitiesInChunk(chunkIdx);           // 活跃 + 未加载
bool hasLoaded = W.HasLoadedEntitiesInChunk(chunkIdx);       // 仅已加载

// 销毁块中的所有实体
W.DestroyAllEntitiesInChunk(chunkIdx);

// 释放块 — 所有实体被删除，标识符被释放
W.FreeChunk(chunkIdx);
```

___

#### 块快照和卸载：
```csharp
// 创建块快照
byte[] snapshot = W.Serializer.CreateChunkSnapshot(chunkIdx);

// 从内存中卸载块（数据被移除，实体被标记为未加载）
ReadOnlySpan<uint> chunks = stackalloc uint[] { chunkIdx };
W.Query().BatchUnload(EntityStatusType.Any, chunks);

// 从快照加载块
W.Serializer.LoadChunkSnapshot(snapshot);
```

___

#### 在指定块中创建实体：
```csharp
// 在指定块中创建实体
struct UnitType : IEntityType { }
var entity = W.NewEntityInChunk<UnitType>(chunkIdx: chunkIdx);

// 安全变体（块已满时返回 false）
bool created = W.TryNewEntityInChunk<UnitType>(out var entity, chunkIdx: chunkIdx);

// 非泛型变体（实体类型在运行时作为 byte 已知）
byte entityTypeId = EntityTypeInfo<UnitType>.Id;
var entity = W.NewEntityInChunk(entityTypeId, chunkIdx: chunkIdx);
```

___

## 块所有权（ChunkOwnerType）

所有权类型决定了世界如何使用块来创建实体：

- **`ChunkOwnerType.Self`** — 块由当前世界管理。通过 `NewEntity()` 创建的实体放置在这些块中
  - 独立世界默认所有块为 `Self` 所有权
- **`ChunkOwnerType.Other`** — 块不由当前世界管理。`NewEntity()` 永远不会在这些块中放置实体
  - 依赖世界默认所有块为 `Other` 所有权

```csharp
// 获取块的所有权类型
ChunkOwnerType owner = W.GetChunkOwner(chunkIdx);

// 更改所有权
// Self → Other：块不再可用于 NewEntity()
// Other → Self：块变为可用于 NewEntity()
W.ChangeChunkOwner(chunkIdx, ChunkOwnerType.Other);
```

{: .importantzh }
通过 `NewEntityByGID<TEntityType>(gid)` 创建实体仅适用于 `Other` 所有权的块。
通过 `NewEntityInChunk<TEntityType>(chunkIdx)` 创建实体仅适用于 `Self` 所有权的块。

___

#### 客户端-服务器示例：

```csharp
// === 服务器端（Independent 世界）===
// 找到空闲块并以 Other 所有权注册
// 服务器不会在此标识符范围内创建自己的实体
EntitiesChunkInfo chunkInfo = WServer.FindNextSelfFreeChunk();
WServer.RegisterChunk(chunkInfo.ChunkIdx, ChunkOwnerType.Other);
// 将块标识符发送给客户端

// === 客户端（Dependent 世界）===
// 从服务器接收块标识符
// 以 Self 所有权注册 — 现在有 4096 个实体槽位可用
WClient.RegisterChunk(chunkIdxFromServer, ChunkOwnerType.Self);

// 客户端可以通过 NewEntity() 创建实体
// 例如用于 UI 或 VFX
var vfx = WClient.NewEntity<VfxType>();

// 同样适用于 P2P：
// 一个 Independent 主机 + N 个 Dependent 客户端
```

___

## 集群和块的使用示例

#### 集群：
- **关卡和地图区域** — 为游戏世界的不同部分使用不同的集群。随着玩家移动，可以加载和卸载集群以节省内存
- **游戏关卡** — 切换关卡时加载/卸载集群
- **游戏会话** — 集群标识符定义会话。结合并行迭代，可以在单个世界内实现多世界模拟

#### 块：
- **世界流式加载** — 游戏过程中加载和卸载块
- **自定义标识符管理** — 控制 EntityGID 的分配
- **竞技场内存** — 快速分配和清理大量临时实体

#### 块所有权：
- **客户端-服务器交互** — 服务器为客户端分配标识符范围
- **P2P 网络格式** — 一个 Independent 主机和 N 个 Dependent 客户端
