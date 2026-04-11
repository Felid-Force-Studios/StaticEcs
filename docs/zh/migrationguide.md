---
title: 迁移到 2.0.0
parent: ZH
nav_order: 5
---

# 从 1.2.x 迁移到 2.0.0

2.0.0 版本是框架的完全重构。几乎所有用户代码都需要修改。

___

## 变更概览

- **段式存储模型** — 新的层级：Chunk → Segment（256 实体）→ Block → Entity
- **entityType**（`IEntityType`）— 通过泛型类型参数实现实体逻辑分组以提高缓存局部性
- **IComponent/IEvent 中的钩子** — 通过 default interface methods 实现，取代独立的 Config 类
- **统一的 ISystem** 取代 IInitSystem/IUpdateSystem/IDestroySystem
- **统一的 Query** — `Query.Entities<>()` 和 `Query.For()` 合并为 `Query<>()`
- **关系** — IEntityLinkComponent → ILinkType + Link\<T\>/Links\<T\>
- **Context → Resources** — `Context.Set/Get` → `SetResource`/`GetResource`
- **标签与组件统一** — 标签存储在 `Components<T>` 中并带有 `IsTag` 标志，使用相同的查询过滤器（`All<>`、`None<>`、`Any<>`）
- **属性取代方法** 用于 Entity 和 World 状态
- **查询批量操作重命名**：`DestroyAllEntities()` → `BatchDestroy()`，新增 `BatchUnload()`
- **已删除**：`UnloadCluster()`/`UnloadChunk()` — 改用 `Query().BatchUnload()` 配合集群/块过滤

___

## 0. 查询批量操作重命名

#### `DestroyAllEntities()` → `BatchDestroy()`：
```csharp
// 旧：
W.Query<All<Health>, TagAll<IsDead>>().DestroyAllEntities();

// 新：
W.Query<All<Health, IsDead>>().BatchDestroy();
```

#### 新增：`BatchUnload()` — 批量卸载匹配过滤器的实体：
```csharp
W.Query<All<Position>>().BatchUnload();
```

#### 已删除：`UnloadCluster()` / `UnloadChunk()` — 改用 `BatchUnload()` 配合过滤：
```csharp
// 旧：
W.UnloadCluster(clusterId);
W.UnloadChunk(chunkIdx);

// 新：
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { clusterId };
W.Query().BatchUnload(EntityStatusType.Any, clusters: clusters);

ReadOnlySpan<uint> chunks = stackalloc uint[] { chunkIdx };
W.Query().BatchUnload(EntityStatusType.Any, chunks);
```

___

## 1. World API

详情：[世界](features/world.md)

#### 方法 → 属性：
```csharp
// 旧：                                新：
W.IsInitialized()                  →  W.IsWorldInitialized
W.IsIndependent()                  →  W.IsIndependent
                                      W.Status  // 新（WorldStatus 枚举）
```

#### 创建实体：

详情：[实体](features/entity.md)

```csharp
// 旧：
var entity = W.Entity.New(clusterId);
var entity = W.Entity.New<Position>(new Position());
W.Entity.NewOnes(count, onCreate, clusterId);
bool ok = W.Entity.TryNew(out entity, clusterId);

// 新：
var entity = W.NewEntity<Default>(clusterId: 0);
var entity = W.NewEntity<Default>(new Default(), clusterId: 0);
W.NewEntities<Default>(count: 100, clusterId: 0, onCreate: null);
bool ok = W.TryNewEntity<Default>(out entity, clusterId: 0);
var entity = W.NewEntity<Default>();  // Default 实体类型, clusterId=0
```

#### WorldConfig：
```csharp
// 所有字段现在为 nullable — 未设置的值使用 WorldConfig.Default() 的默认值
// ParallelQueryType 枚举已移除 → 使用 ThreadCount (uint?)
//   0 = 单线程（默认）
//   WorldConfig.MaxThreadCount = 所有可用 CPU 线程
//   N = 指定线程数
// CustomThreadCount 已移除 → 直接使用 ThreadCount

// 工厂方法：
WorldConfig.Default()      // 标准设置
WorldConfig.MaxThreads()   // 所有可用线程
```

#### 配置类型（ComponentTypeConfig、TagTypeConfig、EventTypeConfig）：
```csharp
// 所有配置字段现在为 nullable — 未设置的值使用默认值
// Guid 从类型名称自动计算（无需手动指定）
// ReadWriteStrategy 自动检测（unmanaged 类型使用 UnmanagedPackArrayStrategy）
// 3 级合并：用户配置 → 静态 Config 字段 → 内置默认值
```

#### 已删除：
- `ParallelQueryType` 枚举 → `WorldConfig.ThreadCount`
- `WorldConfig.CustomThreadCount` → `WorldConfig.ThreadCount`
- `IWorld` 接口 → `WorldHandle`
- `WorldWrapper<W>` → `WorldHandle`
- `Worlds` 静态类
- `BoxedEntity<W>` / `IEntity`

___

## 2. Entity API

详情：[实体](features/entity.md)、[实体全局标识符](features/gid.md)

#### 方法 → 属性：
```csharp
// 旧：                 新：
entity.Gid()        →  entity.GID
entity.GidCompact() →  entity.GIDCompact
entity.IsNotDestroyed()→ entity.IsNotDestroyed  // 方法 → 属性
                         entity.IsDestroyed     // 新增属性
entity.IsDisabled() →  entity.IsDisabled
entity.IsEnabled()  →  entity.IsEnabled
entity.Version()    →  entity.Version
entity.ClusterId()  →  entity.ClusterId
entity.Chunk()      →  entity.ChunkID
entity.IsSelfOwned()→  entity.IsSelfOwned
```

#### 新属性：
```csharp
entity.EntityType   // byte — 实体类型 ID（来自 IEntityType 注册）
entity.ID           // 原始槽位索引
```

#### 组件存在检查：

详情：[组件](features/component.md)、[标签](features/tag.md)

```csharp
// 旧：                                新：
entity.HasAllOf<C>()              →   entity.Has<C>()
entity.HasAllOf<C1, C2>()         →   entity.Has<C1, C2>()
entity.HasAnyOf<C1, C2>()         →   entity.HasAny<C1, C2>()
entity.HasDisabledAllOf<C>()      →   entity.HasDisabled<C>()
entity.HasEnabledAllOf<C>()       →   entity.HasEnabled<C>()

// 标签：
entity.HasAllOfTags<T>()          →   entity.Has<T>()
entity.HasAnyOfTags<T1, T2>()     →   entity.HasAny<T1, T2>()
```

#### Add — 新语义：

详情：[组件 — 添加](features/component.md)

```csharp
// ═══ 旧（v1.2.x）═══
entity.Add<C>();                    // 断言组件不存在
ref var c = ref entity.TryAdd<C>(); // 幂等
entity.Put(new Position(1, 2));     // 无钩子的 upsert

// ═══ 新（v2.0.0）═══
ref var c = ref entity.Add<C>();              // 幂等（原 TryAdd）
ref var c = ref entity.Add<C>(out bool isNew);// 带标志
entity.Set(new Position(1, 2));               // 始终 OnDelete→替换→OnAdd
```

| 旧方法 | 新等价方法 |
|---|---|
| `entity.TryAdd<C>()` | `entity.Add<C>()` |
| `entity.TryAdd<C>(out bool)` | `entity.Add<C>(out bool isNew)` |
| `entity.Put<C>(value)` | `entity.Set<C>(value)`（但现在有钩子） |
| `entity.Add<C>()`（旧，断言） | 无等价方法 |

#### Delete/Disable/Enable — 返回 bool：
```csharp
// 旧：
entity.Delete<C>();               // void，断言
bool ok = entity.TryDelete<C>();  // bool

// 新：
bool deleted = entity.Delete<C>();    // bool（原 TryDelete）
ToggleResult disabled = entity.Disable<C>();  // ToggleResult: MissingComponent, Unchanged, Changed
ToggleResult enabled = entity.Enable<C>();    // ToggleResult: MissingComponent, Unchanged, Changed
```

#### 新方法：
```csharp
entity.Clone(clusterId);                       // 克隆到集群
entity.MoveTo(clusterId);                      // 移动到集群
```

#### 已删除：
- `entity.Box()` / `BoxedEntity<W>` / `IEntity`
- `entity.TryAdd<C>()` → 使用 `entity.Add<C>()`
- `entity.Put<C>(val)` → 使用 `entity.Set<C>(val)`
- `entity.TryDelete<C>()` → 使用 `entity.Delete<C>()`
- 所有 Raw 方法（`RawHasAllOf`、`RawAdd`、`RawGet`、`RawPut` 等）
- `Entity.New(...)`（所有重载）→ `W.NewEntity(...)`
- `W.OnCreateEntity(callback)` → 使用 `IEntityType.OnCreate` 钩子或 `Created` 追踪过滤器

___

## 3. EntityGID API

详见: [全局标识符](features/gid.md)

```csharp
// ═══ 旧版 (v1.2.x) ═══
bool ok = gid.IsActual<WT>();
bool loaded = gid.IsLoaded<WT>();
bool both = gid.IsLoadedAndActual<WT>();

// ═══ 新版 (v2.0.0) ═══
GIDStatus status = gid.Status<WT>();
// GIDStatus.Active     — 实体存在，版本匹配，已加载（原 IsLoadedAndActual）
// GIDStatus.NotActual  — 实体不存在或版本/集群不匹配（原 !IsActual）
// GIDStatus.NotLoaded  — 实体存在，版本匹配，但已卸载（原 IsActual && !IsLoaded）
```

| 旧方法 | 新等价方法 |
|---|---|
| `gid.IsActual<WT>()` | `gid.Status<WT>() != GIDStatus.NotActual` |
| `gid.IsLoaded<WT>()` | `gid.Status<WT>() != GIDStatus.NotLoaded` |
| `gid.IsLoadedAndActual<WT>()` | `gid.Status<WT>() == GIDStatus.Active` |

#### 通过 GID 创建实体:
```csharp
// 旧版:
var entity = W.Entity.New(someGid);

// 新版:
var entity = W.NewEntityByGID<Default>(someGid);
```

___

## 4. 组件

详情：[组件](features/component.md)

#### 钩子 — 通过 default interface methods 在 IComponent 中：
```csharp
// ═══ 旧（v1.2.x）═══
struct Position : IComponent { public float X, Y; }
class PositionConfig : IComponentConfig<Position, WT> {
    public OnComponentHandler<Position> OnAdd() => (ref Position c, Entity e) => { };
    public Guid Id() => ...;
    public BinaryWriter<Position> Writer() => ...;
    public BinaryReader<Position> Reader() => ...;
}
W.RegisterComponentType<Position>(new PositionConfig());

// ═══ 新（v2.0.0）═══
struct Position : IComponent {
    public float X, Y;

    public void OnAdd<TWorld>(World<TWorld>.Entity self)
        where TWorld : struct, IWorldType { }

    public void OnDelete<TWorld>(World<TWorld>.Entity self, HookReason reason)
        where TWorld : struct, IWorldType { }

    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType { writer.WriteFloat(X); writer.WriteFloat(Y); }

    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled)
        where TWorld : struct, IWorldType { X = reader.ReadFloat(); Y = reader.ReadFloat(); }
}

W.Types()
    .Component<Position>(new ComponentTypeConfig<Position>(
        guid: new Guid("..."),
        readWriteStrategy: new UnmanagedPackArrayStrategy<Position>()
    ));
```

#### 已删除：
- `IComponentConfig<T, W>`、`DefaultComponentConfig<T, W>`、`ValueComponentConfig<T, W>`
- `OnComponentHandler<T>`、`OnCopyHandler<T>` 委托
- `OnPut` 钩子（v2 中 `Set(value)` 调用 OnDelete+OnAdd）

#### 池访问：
```csharp
// 旧：                                新：
Components<T>.Value.Ref(entity)   →   Components<T>.Instance.Ref(entity)
Components<T>.Value.Has(entity)   →   Components<T>.Instance.Has(entity)
Components<T>.Value.IsRegistered()→   Components<T>.Instance.IsRegistered  // 属性
```

___

## 5. 标签

详情：[标签](features/tag.md)

标签已与组件内部统一。标签现在存储在 `Components<T>` 中并带有 `IsTag` 标志。

#### 实体方法：
```csharp
// 旧：                                新：
entity.SetTag<T>()                →   entity.Set<T>()
entity.HasTag<T>()                →   entity.Has<T>()
entity.HasAnyTags<T1,T2>()       →   entity.HasAny<T1,T2>()
entity.DeleteTag<T>()             →   entity.Delete<T>()
entity.ToggleTag<T>()             →   entity.Toggle<T>()
entity.ApplyTag<T>(bool)          →   entity.Apply<T>(bool)
entity.CopyTagsTo<T>(target)     →   entity.CopyTo<T>(target)
entity.MoveTagsTo<T>(target)     →   entity.MoveTo<T>(target)
```

#### 池访问：
```csharp
// 旧：                                新：
Tags<T>.Value                     →   Components<T>.Instance (IsTag = true)
```

#### 查询过滤器：
```csharp
// 旧：                                新：
TagAll<T>                         →   All<T>
TagNone<T>                        →   None<T>
TagAny<T1,T2>                    →   Any<T1,T2>

#### 已删除：
- `TagsHandle` → 使用 `ComponentsHandle`（带有 `IsTag` 字段）
- `WorldConfig.BaseTagTypesCount` → 标签计入 `BaseComponentTypesCount`
- `DeleteTagsSystem<W, T>` → 使用 `Query().BatchDelete<T>()`

___

## 6. 查询

详情：[查询](features/query.md)

#### 统一入口：
```csharp
// ═══ 旧（v1.2.x）═══
foreach (var entity in W.Query.Entities<All<Position>>()) { }

W.Query.For((ref Position pos) => {
    pos.X += 1;
});

// ═══ 新（v2.0.0）═══
foreach (var entity in W.Query<All<Position>>().Entities()) { }

W.Query().For(
    static (ref Position pos) => { pos.X += 1; }
);
```

#### 实体搜索：
```csharp
// 旧：
W.Query.Entities<All<P>>().First(out entity);

// 新：
W.Query<All<P>>().Any(out entity);
```

#### 批量操作：
```csharp
// 旧（在 QueryEntitiesIterator 上）：
W.Query.Entities<All<P>>().AddForAll<Health>();
W.Query.Entities<All<P>>().DeleteForAll<Health>();
W.Query.Entities<All<P>>().SetTagForAll<Active>();

// 新（在 WorldQuery 上，支持链式调用）：
W.Query<All<P>>().BatchAdd<Health>().BatchSet<Active>();
W.Query<All<P>>().BatchDelete<Health>();
W.Query<All<P>>().BatchDisable<Health>();   // 新
W.Query<All<P>>().BatchEnable<Health>();    // 新
W.Query<All<P>>().BatchDestroy();
W.Query<All<P>>().BatchUnload();    // 新
```

#### 过滤器包装 With → And:
```csharp
// 旧版:
W.Query.Entities<With<All<Pos>, None<Name>>>();

// 新版:
W.Query<And<All<Pos>, None<Name>>>();
// 同时新增 Or<> 用于过滤器析取（新功能）
```

#### 并行迭代:
```csharp
// 旧版:
W.Query.Parallel.For(minChunkSize: 50000, (W.Entity ent, ref Position pos) => { });

// 新版:
W.Query().ForParallel(
    static (W.Entity ent, ref Position pos) => { },
    minEntitiesPerThread: 50000
);
```

#### QueryMode：

详情：[性能 — QueryMode](performance.md#querymode)

```csharp
// 旧：创建迭代器时的运行时参数
W.Query.Entities<All<P>>(queryMode: QueryMode.Flexible);

// 新：默认 Strict，Flexible 使用单独的方法
foreach (var entity in W.Query<All<P>>().EntitiesFlexible()) { }

W.Query().For(
    static (ref Position pos) => { },
    queryMode: QueryMode.Flexible
);
```

___

## 7. 关系

详情：[关系](features/relations.md)

```csharp
// ═══ 旧（v1.2.x）═══
struct Parent : IEntityLinkComponent<Parent> {
    public EntityGID Link;
    ref EntityGID IRefProvider<Parent, EntityGID>.RefValue(ref Parent c) => ref c.Link;
}
W.RegisterToOneRelationType<Parent>(config);

// ═══ 新（v2.0.0）═══
struct ParentLink : ILinkType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType { }
    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType { }
}
W.Types()
    .Link<ParentLink>()    // 单链接
    .Links<ParentLink>();  // 多链接

// 使用：
entity.Set(new W.Link<ParentLink>(parentEntity));
ref var links = ref entity.Ref<W.Links<ChildLink>>();
```

| 旧类型 | 新等价类型 |
|---|---|
| `IEntityLinkComponent<T>` | `ILinkType` + `Link<T>` |
| `IEntityLinksComponent<T>` | `ILinksType` + `Links<T>` |
| `RegisterToOneRelationType` | `W.Types().Link<T>()` |
| `RegisterToManyRelationType` | `W.Types().Links<T>()` |
| `RegisterOneToManyRelationType` | 分别：`W.Types().Link<T>().Links<T>()` |

___

## 8. 事件

详情：[事件](features/events.md)

```csharp
// ═══ 旧（v1.2.x）═══
struct MyEvent : IEvent { public int Data; }
class MyEventConfig : IEventConfig<MyEvent, WT> { ... }
W.Events.RegisterEventType<MyEvent>(new MyEventConfig());
W.Events.Send(new MyEvent { Data = 1 });
var receiver = W.Events.RegisterEventReceiver<MyEvent>();
W.Events.DeleteEventReceiver(ref receiver);

// ═══ 新（v2.0.0）═══
struct MyEvent : IEvent {
    public int Data;
    public void Write(ref BinaryPackWriter writer) { writer.WriteInt(Data); }
    public void Read(ref BinaryPackReader reader, byte version) { Data = reader.ReadInt(); }
}
W.Types()
    .Event<MyEvent>(new EventTypeConfig<MyEvent>(guid: new Guid("...")));
W.SendEvent(new MyEvent { Data = 1 });
var receiver = W.RegisterEventReceiver<MyEvent>();
W.DeleteEventReceiver(ref receiver);
```

- `IEventConfig<T, W>` 已删除 → `EventTypeConfig<T>` + `IEvent` 钩子
- `W.Events.XXX` → `W.XXX`（方法移至 World）
- `IsClearable()` → `NoDataLifecycle`（逻辑反转，扩展语义 — 控制初始化和清理）

___

## 9. 系统

详情：[系统](features/systems.md)

```csharp
// ═══ 旧（v1.2.x）═══
struct MoveSystem : IUpdateSystem {
    public void Update() { }
}
Systems.AddUpdate(new MoveSystem());
Systems.AddCallOnce(new InitSystem());

// ═══ 新（v2.0.0）═══
struct MoveSystem : ISystem {
    public void Init() { }          // 取代 IInitSystem
    public void Update() { }        // 取代 IUpdateSystem
    public bool UpdateIsActive() => true; // 取代 ISystemCondition
    public void Destroy() { }       // 取代 IDestroySystem
}
GameSys.Add(new MoveSystem(), order: 0);
```

| 旧类型 | 新等价类型 |
|---|---|
| `IInitSystem` | `ISystem.Init()` |
| `IUpdateSystem` | `ISystem.Update()` |
| `IDestroySystem` | `ISystem.Destroy()` |
| `ISystemCondition` | `ISystem.UpdateIsActive()` |
| `Systems.AddUpdate(sys)` | `Sys.Add(sys, order)` |
| `Systems.AddCallOnce(sys)` | `Sys.Add(sys, order)` + `Init()` |

___

## 10. 多组件

详情：[多组件](features/multicomponent.md)

```csharp
// ═══ 旧（v1.2.x）═══
struct Items : IMultiComponent<Items, int> {
    public Multi<int> Values;
    public ref Multi<int> RefValue(ref Items c) => ref c.Values;
}
W.RegisterMultiComponentType<Items, int>(4, config);

// ═══ 新（v2.0.0）═══
struct Items : IMultiComponent {  // 标记接口，无 RefValue
    public Multi<int> Values;
}
W.Types()
    .Multi<Items>();  // 多组件注册
```

- `IMultiComponent<T, V>` → `IMultiComponent`（标记）
- `RefValue()` 已删除
- `RegisterMultiComponentType` → `W.Types().Multi<T>()`（不是 `Component<T>`!）
- `Count` → `Length`
- `IsEmpty()`/`IsNotEmpty()`/`IsFull()` → 属性

___

## 11. 序列化

详情：[序列化](features/serialization.md)

```csharp
// ═══ 旧（v1.2.x）═══
class PositionConfig : DefaultComponentConfig<Position, WT> {
    public override BinaryWriter<Position> Writer() => ...;
    public override BinaryReader<Position> Reader() => ...;
}
W.Events.CreateSnapshot();
W.Events.LoadSnapshot(snapshot);

// ═══ 新（v2.0.0）═══
// Write/Read 钩子直接在 IComponent/IEvent 上
struct Position : IComponent {
    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType { }
    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled)
        where TWorld : struct, IWorldType { }
}
W.Serializer.CreateEventsSnapshot();
W.Serializer.LoadEventsSnapshot(snapshot);
```

___

## 12. Context → Resources

详情：[资源](features/resources.md)

```csharp
// ═══ 旧（v1.2.x）═══
W.Context.Set<GameTime>(new GameTime());
ref var time = ref W.Context.Get<GameTime>();
bool has = W.Context.Has<GameTime>();

// ═══ 新（v2.0.0）═══
W.SetResource(new GameTime());  // Set 自动覆盖（替代 Replace）
ref var time = ref W.GetResource<GameTime>();
bool has = W.HasResource<GameTime>();
W.RemoveResource<GameTime>();

// 命名资源（新）：
W.SetResource("key", new GameConfig());
ref var cfg = ref W.GetResource<GameConfig>("key");
```

___

## 重命名表

| 旧（v1.2.x） | 新（v2.0.0） |
|---|---|
| `W.Entity.New(...)` | `W.NewEntity<TEntityType>(...)` / `W.NewEntity<Default>()` |
| `W.Entity.NewOnes(...)` | `W.NewEntities<TEntityType>(count, ...)` |
| `W.IsInitialized()` | `W.IsWorldInitialized` |
| `W.IsIndependent()` | `W.IsIndependent` |
| `entity.Gid()` | `entity.GID` |
| `entity.HasAllOf<C>()` | `entity.Has<C>()` |
| `entity.HasAnyOf<C1,C2>()` | `entity.HasAny<C1,C2>()` |
| `entity.HasAllOfTags<T>()` | `entity.Has<T>()` |
| `entity.TryAdd<C>()` | `entity.Add<C>()` |
| `entity.Put<C>(val)` | `entity.Set<C>(val)` |
| `entity.TryDelete<C>()` | `entity.Delete<C>()`（bool） |
| `Components<T>.Value` | `Components<T>.Instance` |
| `Tags<T>.Value` | `Components<T>.Instance` |
| `W.Query.Entities<F>()` | `W.Query<F>()` |
| `W.Query.For(...)` | `W.Query<F>().For(...)` |
| `AddForAll<C>()` | `BatchAdd<C>()` |
| `DeleteForAll<C>()` | `BatchDelete<C>()` |
| `SetTagForAll<T>()` | `BatchSet<T>()` |
| `IInitSystem` / `IUpdateSystem` | `ISystem` |
| `Systems.AddUpdate(sys)` | `Sys.Add(sys, order)` |
| `IComponentConfig<T,W>` | `ComponentTypeConfig<T>` + IComponent 钩子 |
| `IEventConfig<T,W>` | `EventTypeConfig<T>` + IEvent 钩子 |
| `IEntityLinkComponent<T>` | `ILinkType` + `Link<T>` |
| `IEntityLinksComponent<T>` | `ILinksType` + `Links<T>` |
| `RegisterToOneRelationType` | `W.Types().Link<T>()` |
| `IMultiComponent<T,V>` | `IMultiComponent`（标记） |
| `RegisterMultiComponentType` | `W.Types().Multi<T>()` |
| `W.Context.Set/Get/Has` | `W.SetResource/GetResource/HasResource` |
| `W.Context<T>.Replace(val)` | `W.SetResource(val)`（自动覆盖）|
| `W.Events.Send(...)` | `W.SendEvent(...)` |
| `W.Events.RegisterEventReceiver` | `W.RegisterEventReceiver` |
| `W.Events.CreateSnapshot()` | `W.Serializer.CreateEventsSnapshot()` |
| `gid.IsActual<WT>()` | `gid.Status<WT>() != GIDStatus.NotActual` |
| `gid.IsLoadedAndActual<WT>()` | `gid.Status<WT>() == GIDStatus.Active` |
| `W.Entity.New(gid)` | `W.NewEntityByGID<TEntityType>(gid)` |
| `W.OnCreateEntity(callback)` | `IEntityType.OnCreate` / `Created` 追踪 |
| `With<F1, F2>` | `And<F1, F2>` |
| `Query.Parallel.For(minChunkSize:)` | `Query().ForParallel(minEntitiesPerThread:)` |
