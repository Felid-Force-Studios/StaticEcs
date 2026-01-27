---
title: 序列化
parent: 功能
nav_order: 15
---

## 序列化
序列化是创建整个世界或单个实体、集群、块的二进制快照的机制。
二进制序列化使用 [StaticPack](https://github.com/Felid-Force-Studios/StaticPack)。

___

## 配置组件

要支持组件序列化：
1. 注册时指定 `Guid`（稳定的类型标识符）
2. 在组件上实现 `Write` 和 `Read` 钩子

{: .importantzh }
`Write` 和 `Read` 钩子对于 `EntitiesSnapshot` 序列化是**必需的**（适用于所有组件类型，包括 unmanaged）。对于世界/集群/块快照，non-unmanaged 类型也始终使用这些钩子。

#### Unmanaged 组件：
```csharp
public struct Position : IComponent {
    public float X, Y, Z;

    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType {
        writer.WriteFloat(X);
        writer.WriteFloat(Y);
        writer.WriteFloat(Z);
    }

    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled)
        where TWorld : struct, IWorldType {
        X = reader.ReadFloat();
        Y = reader.ReadFloat();
        Z = reader.ReadFloat();
    }
}

W.Types()
    .Component<Position>(new ComponentTypeConfig<Position>(
        guid: new Guid("b121594c-456e-4712-9b64-b75dbb37e611"),
        readWriteStrategy: new UnmanagedPackArrayStrategy<Position>()
    ));
```

#### Non-unmanaged 组件（包含引用字段）：
```csharp
public struct Name : IComponent {
    public string Value;

    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType {
        writer.WriteString16(Value);
    }

    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled)
        where TWorld : struct, IWorldType {
        Value = reader.ReadString16();
    }
}

W.Types()
    .Component<Name>(new ComponentTypeConfig<Name>(
        guid: new Guid("531dc870-fdf5-4a8d-a4c6-b4911b1ea1c3")
    ));
```

#### Unmanaged 类型的块内存复制：

对于世界/集群/块快照，unmanaged 组件可以作为内存块序列化，而不是逐个调用 `Write`/`Read`。要启用此功能，需要**显式指定** `UnmanagedPackArrayStrategy<T>`：

```csharp
W.Types()
    .Component<Position>(new ComponentTypeConfig<Position>(
        guid: new Guid("b121594c-456e-4712-9b64-b75dbb37e611"),
        readWriteStrategy: new UnmanagedPackArrayStrategy<Position>()  // 块内存复制
    ));
```

{: .notezh }
`UnmanagedPackArrayStrategy<T>` 执行直接内存复制 — 比逐个组件序列化快得多。仅适用于 unmanaged 类型。版本不匹配时（数据迁移），系统自动回退到 `Read` 钩子。默认策略为 `StructPackArrayStrategy<T>`。

#### Multi 和 Links 的批量段序列化：

多组件和 Links 将值存储在共享段存储中。默认情况下，每个实体的值单独序列化。要对底层存储进行批量段序列化（需要 unmanaged 值类型）：

```csharp
// 带批量段策略的多组件
W.Types().Multi<Item>(new ComponentTypeConfig<W.Multi<Item>>(
    guid: new Guid("..."),
    readWriteStrategy: new MultiUnmanagedPackArrayStrategy<MyWorld, Item>()
));

// 带批量段策略的 Links
W.Types().Links<MyLinkType>(new ComponentTypeConfig<W.Links<MyLinkType>>(
    guid: new Guid("..."),
    readWriteStrategy: new LinksUnmanagedPackArrayStrategy<MyWorld, MyLinkType>()
));
```

#### 完整配置：
```csharp
W.Types()
    .Component<Position>(new ComponentTypeConfig<Position>(
        guid: new Guid("b121594c-456e-4712-9b64-b75dbb37e611"),
        version: 1,                  // 用于迁移的数据模式版本（默认 — 0）
        noDataLifecycle: true,       // 禁用框架数据管理（默认 — false）
        readWriteStrategy: new UnmanagedPackArrayStrategy<Position>() // 序列化策略（默认 — StructPackArrayStrategy<T>）
    ));
```

___

## 配置标签

标签通过 `TagTypeConfig<T>` 配置：

```csharp
W.Types()
    .Tag<IsPlayer>(new TagTypeConfig<IsPlayer>(guid: new Guid("3a6fe6a2-9427-43ae-9b4a-f8582e3a5f90")))
    .Tag<IsDead>(new TagTypeConfig<IsDead>(guid: new Guid("d25b7a08-cbe6-4c77-bd8e-29ce7f748c30")));
```

#### 完整配置:
```csharp
W.Types().Tag<Poisoned>(new TagTypeConfig<Poisoned>(
    guid: new Guid("A1B2C3D4-..."), // 序列化的稳定标识符（默认 — default）
    trackAdded: true,                // 启用添加追踪（默认 — false）
    trackDeleted: true               // 启用删除追踪（默认 — false）
));
```

{: .notezh }
自动注册时，`RegisterAll()` 会获取标签结构体内的静态 `Guid` 字段。自动注册不会设置 `trackAdded` / `trackDeleted` 参数 — 要启用追踪请使用 `TagTypeConfig<T>` 手动注册。

___

## 配置事件

事件使用 `EventTypeConfig<T>` — 类似于组件：

```csharp
public struct OnDamage : IEvent {
    public float Amount;

    public void Write(ref BinaryPackWriter writer) {
        writer.WriteFloat(Amount);
    }

    public void Read(ref BinaryPackReader reader, byte version) {
        Amount = reader.ReadFloat();
    }
}

W.Types()
    .Event<OnDamage>(new EventTypeConfig<OnDamage>(
        guid: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890")
    ));
```

___

## 世界快照（World Snapshot）

保存完整的世界状态：所有实体、组件、标签和事件。

#### 在初始化时保存和加载：
```csharp
// 保存世界
byte[] worldSnapshot = W.Serializer.CreateWorldSnapshot();
W.Destroy();

// 在初始化时加载世界 — 最简单的方式
CreateWorld(); // Create + 类型注册
W.InitializeFromWorldSnapshot(worldSnapshot);

// 所有实体和事件已恢复
foreach (var entity in W.Query().Entities()) {
    Console.WriteLine(entity.PrettyString);
}
```

#### 在初始化后保存和加载：
```csharp
byte[] worldSnapshot = W.Serializer.CreateWorldSnapshot();
W.Destroy();

CreateWorld();
W.Initialize();
// 加载前所有现有实体和事件将被删除
W.Serializer.LoadWorldSnapshot(worldSnapshot);
```

#### 附加参数：
```csharp
// 保存到文件
W.Serializer.CreateWorldSnapshot("path/to/world.bin");

// 使用 GZIP 压缩
byte[] compressed = W.Serializer.CreateWorldSnapshot(gzip: true);

// 按集群过滤
W.Serializer.CreateWorldSnapshot(clusters: new ushort[] { 0, 1 });

// 块写入策略
W.Serializer.CreateWorldSnapshot(strategy: ChunkWritingStrategy.SelfOwner);

// 不包含事件
W.Serializer.CreateWorldSnapshot(writeEvents: false);

// 不包含自定义数据
W.Serializer.CreateWorldSnapshot(withCustomSnapshotData: false);

// 从文件加载
W.Serializer.LoadWorldSnapshot("path/to/world.bin");

// 加载压缩数据
W.Serializer.LoadWorldSnapshot(compressed, gzip: true);
```

{: .importantzh }
世界快照中的所有组件和标签**必须**注册 `Guid`。在 DEBUG 模式下，尝试序列化未注册 `Guid` 的类型将报错。

___

## 实体快照（Entities Snapshot）

允许以精细控制保存和加载单个实体。

#### 保存实体：
```csharp
// 创建实体写入器
using var writer = W.Serializer.CreateEntitiesSnapshotWriter();

// 写入特定实体
foreach (var entity in W.Query().Entities()) {
    writer.Write(entity);
}

// 或一次写入所有实体
// writer.WriteAllEntities();

// 创建快照
byte[] snapshot = writer.CreateSnapshot();

// 或保存到文件
// writer.CreateSnapshot("path/to/entities.bin");
```

#### 写入并同时卸载：
```csharp
using var writer = W.Serializer.CreateEntitiesSnapshotWriter();

// 写入并卸载 — 在流式加载时节省内存
foreach (var entity in W.Query().Entities()) {
    writer.WriteAndUnload(entity);
}

// 或一次处理所有实体
// writer.WriteAndUnloadAllEntities();

byte[] snapshot = writer.CreateSnapshot();
```

___

#### 加载实体（entitiesAsNew）：

`entitiesAsNew` 参数决定实体的加载方式：

- **`entitiesAsNew: false`**（默认）— 实体恢复到**相同槽位**（相同 EntityGID）。如果槽位已被占用 — DEBUG 模式下报错。
- **`entitiesAsNew: true`** — 实体加载到**新槽位**，获得新的 EntityGID。实体间的链接（Link、Links）可能指向错误的实体。

```csharp
// 加载到原始槽位
W.Serializer.LoadEntitiesSnapshot(snapshot, entitiesAsNew: false);

// 作为新实体加载
W.Serializer.LoadEntitiesSnapshot(snapshot, entitiesAsNew: true);

// 为每个加载的实体添加回调
W.Serializer.LoadEntitiesSnapshot(snapshot, entitiesAsNew: true, onLoad: entity => {
    Console.WriteLine($"已加载: {entity.PrettyString}");
});
```

___

#### 保持实体间链接（GID Store）：

要正确使用 `entitiesAsNew: false` 加载实体，需要保存全局标识符存储：

```csharp
// 1. 保存实体和 GID Store
using var writer = W.Serializer.CreateEntitiesSnapshotWriter();
writer.WriteAllEntities();
byte[] entitiesSnapshot = writer.CreateSnapshot();
byte[] gidSnapshot = W.Serializer.CreateGIDStoreSnapshot();
W.Destroy();

// 2. 使用 GID Store 恢复世界
CreateWorld();
W.InitializeFromGIDStoreSnapshot(gidSnapshot);

// 新实体不会占用已保存实体的槽位
var newEntity = W.NewEntity<Default>();
newEntity.Set(new Position { X = 1 });

// 3. 将实体加载到原始槽位 — 所有链接正确
W.Serializer.LoadEntitiesSnapshot(entitiesSnapshot, entitiesAsNew: false);
```

{: .notezh }
GID Store 包含所有已发放标识符的信息。这保证了新实体不会占用已卸载实体的槽位，所有链接（Link、Links、数据中的 EntityGID）保持正确。

___

## GID Store

```csharp
// 保存 GID Store
byte[] gidSnapshot = W.Serializer.CreateGIDStoreSnapshot();

// 使用 GZIP 压缩
byte[] gidCompressed = W.Serializer.CreateGIDStoreSnapshot(gzip: true);

// 保存到文件
W.Serializer.CreateGIDStoreSnapshot("path/to/gid.bin");

// 使用块写入策略
W.Serializer.CreateGIDStoreSnapshot(strategy: ChunkWritingStrategy.SelfOwner);

// 按集群过滤
W.Serializer.CreateGIDStoreSnapshot(clusters: new ushort[] { 0, 1 });

// 从 GID Store 初始化世界
CreateWorld();
W.InitializeFromGIDStoreSnapshot(gidSnapshot);

// 在已初始化的世界中恢复 GID Store
// 所有实体将被删除，状态将重置
W.Serializer.RestoreFromGIDStoreSnapshot(gidSnapshot);
```

___

## 集群和块快照

#### 集群：
```csharp
// 保存集群
byte[] clusterSnapshot = W.Serializer.CreateClusterSnapshot(clusterId: 1);

// 包含实体数据以便作为新实体加载
byte[] clusterWithEntities = W.Serializer.CreateClusterSnapshot(
    clusterId: 1,
    withEntitiesData: true  // 加载时使用 entitiesAsNew 所需
);

// 从内存卸载集群
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { 1 };
W.Query().BatchUnload(EntityStatusType.Any, clusters: clusters);

// 从快照加载集群
W.Serializer.LoadClusterSnapshot(clusterSnapshot);

// 作为新实体加载到不同集群
W.Serializer.LoadClusterSnapshot(clusterWithEntities,
    new EntitiesAsNewParams(entitiesAsNew: true, clusterId: 2)
);
```

#### 块：
```csharp
// 保存块
byte[] chunkSnapshot = W.Serializer.CreateChunkSnapshot(chunkIdx: 0);

// 从内存卸载块
ReadOnlySpan<uint> unloadChunks = stackalloc uint[] { 0 };
W.Query().BatchUnload(EntityStatusType.Any, unloadChunks);

// 从快照加载块
W.Serializer.LoadChunkSnapshot(chunkSnapshot);
```

{: .importantzh }
默认情况下，集群和块快照**不存储**实体标识符数据（仅存储组件数据）。如果需要作为新实体加载（`entitiesAsNew: true`），创建快照时请指定 `withEntitiesData: true`。

___

#### 综合流式加载示例：
```csharp
void PrintCounts(string label) {
    Console.WriteLine($"{label} — 总计: {W.CalculateEntitiesCount()} | 已加载: {W.CalculateLoadedEntitiesCount()}");
}

// 保存单个实体
using var writer = W.Serializer.CreateEntitiesSnapshotWriter();
foreach (var entity in W.Query().Entities()) {
    writer.WriteAndUnload(entity);
}
byte[] entitiesSnapshot = writer.CreateSnapshot();
PrintCounts("卸载实体后"); // 总计: 2 | 已加载: 0

// 创建集群并填充
const ushort ZONE_CLUSTER = 1;
W.RegisterCluster(ZONE_CLUSTER);
struct ZoneEntityType : IEntityType { }
W.NewEntities<ZoneEntityType>(count: 2000, clusterId: ZONE_CLUSTER);
PrintCounts("创建集群后"); // 总计: 2002 | 已加载: 2000

// 保存并卸载集群
byte[] clusterSnapshot = W.Serializer.CreateClusterSnapshot(ZONE_CLUSTER);
ReadOnlySpan<ushort> zoneClusters = stackalloc ushort[] { ZONE_CLUSTER };
W.Query().BatchUnload(EntityStatusType.Any, clusters: zoneClusters);
PrintCounts("卸载集群后"); // 总计: 2002 | 已加载: 0

// 创建块并填充
var chunkIdx = W.FindNextSelfFreeChunk().ChunkIdx;
W.RegisterChunk(chunkIdx, clusterId: 0);
for (int i = 0; i < 100; i++) {
    W.NewEntityInChunk<ZoneEntityType>(chunkIdx: chunkIdx);
}
PrintCounts("创建块后"); // 总计: 2102 | 已加载: 100

// 保存并卸载块
byte[] chunkSnapshot = W.Serializer.CreateChunkSnapshot(chunkIdx);
ReadOnlySpan<uint> unloadChunks = stackalloc uint[] { chunkIdx };
W.Query().BatchUnload(EntityStatusType.Any, unloadChunks);
PrintCounts("卸载块后"); // 总计: 2102 | 已加载: 0

// 保存 GID Store 并重建世界
byte[] gidSnapshot = W.Serializer.CreateGIDStoreSnapshot();
W.Destroy();

CreateWorld();
W.InitializeFromGIDStoreSnapshot(gidSnapshot);

// 以任意顺序加载
W.Serializer.LoadClusterSnapshot(clusterSnapshot);
PrintCounts("加载集群后"); // 总计: 2102 | 已加载: 2000

W.Serializer.LoadEntitiesSnapshot(entitiesSnapshot);
PrintCounts("加载实体后"); // 总计: 2102 | 已加载: 2002

W.Serializer.LoadChunkSnapshot(chunkSnapshot);
PrintCounts("加载块后"); // 总计: 2102 | 已加载: 2102
```

___

## 数据迁移

#### 组件版本控制：

`Read` 钩子中的 `version` 参数支持在模式版本之间迁移数据：

```csharp
public struct Position : IComponent {
    public float X, Y, Z;

    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType {
        writer.WriteFloat(X);
        writer.WriteFloat(Y);
        writer.WriteFloat(Z);
    }

    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled)
        where TWorld : struct, IWorldType {
        X = reader.ReadFloat();
        Y = reader.ReadFloat();
        // 版本 0 没有 Z — 使用默认值
        Z = version >= 1 ? reader.ReadFloat() : 0f;
    }
}

// 使用新版本注册
W.Types()
    .Component<Position>(new ComponentTypeConfig<Position>(
        guid: new Guid("b121594c-456e-4712-9b64-b75dbb37e611"),
        version: 1,  // 之前是版本 0，现在是 1
        readWriteStrategy: new UnmanagedPackArrayStrategy<Position>()
    ));
```

___

#### 已删除类型的迁移：

如果组件、标签或事件已从代码中删除，数据默认会自动跳过。对于自定义处理：

```csharp
// 已删除组件的迁移
W.Serializer.SetComponentDeleteMigrator(
    new Guid("已删除组件的guid"),
    (ref BinaryPackReader reader, W.Entity entity, byte version, bool disabled) => {
        // 读取所有数据并执行自定义逻辑
    }
);

// 已删除标签的迁移
W.Serializer.SetMigrator(
    new Guid("已删除标签的guid"),
    (W.Entity entity) => {
        // 自定义逻辑
    }
);

// 已删除事件的迁移
W.Serializer.SetEventDeleteMigrator(
    new Guid("已删除事件的guid"),
    (ref BinaryPackReader reader, byte version) => {
        // 读取所有数据并执行自定义逻辑
    }
);
```

{: .notezh }
添加新类型时，旧快照可以正确加载 — 新组件只是在加载的实体上不存在。

___

## 回调

#### 全局回调：
```csharp
// 对所有快照类型调用（World、Cluster、Chunk、Entities）

// 创建快照前
W.Serializer.RegisterPreCreateSnapshotCallback(param => {
    Console.WriteLine($"正在创建快照类型: {param.Type}");
});

// 创建快照后
W.Serializer.RegisterPostCreateSnapshotCallback(param => {
    Console.WriteLine($"快照已创建: {param.Type}");
});

// 加载快照前
W.Serializer.RegisterPreLoadSnapshotCallback(param => {
    Console.WriteLine($"正在加载快照: {param.Type}, AsNew: {param.EntitiesAsNew}");
});

// 加载快照后
W.Serializer.RegisterPostLoadSnapshotCallback(param => {
    Console.WriteLine($"快照已加载: {param.Type}");
});
```

#### 按快照类型过滤：
```csharp
W.Serializer.RegisterPreCreateSnapshotCallback(param => {
    if (param.Type == SnapshotType.World) {
        Console.WriteLine("正在保存世界");
    }
});
```

#### 每个实体的回调：
```csharp
// 保存每个实体后
W.Serializer.RegisterPostCreateSnapshotEachEntityCallback((entity, param) => {
    Console.WriteLine($"已保存: {entity.PrettyString}");
});

// 加载每个实体后
W.Serializer.RegisterPostLoadSnapshotEachEntityCallback((entity, param) => {
    Console.WriteLine($"已加载: {entity.PrettyString}");
});
```

___

## 快照中的自定义数据

#### 全局自定义数据：
```csharp
// 向快照添加任意数据（例如系统或服务数据）
W.Serializer.SetSnapshotHandler(
    new Guid("57c15483-988a-47e7-919c-51b9a7b957b5"), // 唯一的数据类型 guid
    version: 0,
    writer: (ref BinaryPackWriter writer, SnapshotWriteParams param) => {
        writer.WriteDateTime(DateTime.Now);
    },
    reader: (ref BinaryPackReader reader, ushort version, SnapshotReadParams param) => {
        var savedTime = reader.ReadDateTime();
        Console.WriteLine($"保存时间: {savedTime}");
    }
);
```

#### 每个实体的自定义数据：
```csharp
W.Serializer.SetSnapshotHandlerEachEntity(
    new Guid("68d26594-1a9b-48f8-b2de-71c0a8b068c6"),
    version: 0,
    writer: (ref BinaryPackWriter writer, W.Entity entity, SnapshotWriteParams param) => {
        // 为实体写入附加数据
    },
    reader: (ref BinaryPackReader reader, W.Entity entity, ushort version, SnapshotReadParams param) => {
        // 读取实体的附加数据
    }
);
```

___

## 事件序列化

```csharp
// 保存事件
byte[] eventsSnapshot = W.Serializer.CreateEventsSnapshot();

// 使用 GZIP 压缩
byte[] eventsCompressed = W.Serializer.CreateEventsSnapshot(gzip: true);

// 保存到文件
W.Serializer.CreateEventsSnapshot("path/to/events.bin");

// 加载事件
W.Serializer.LoadEventsSnapshot(eventsSnapshot);

// 从文件加载
W.Serializer.LoadEventsSnapshot("path/to/events.bin");
```

{: .notezh }
使用 `CreateWorldSnapshot` 时，事件会自动保存（除非指定 `writeEvents: false`）。使用 `EntitiesSnapshot` 时需要单独序列化事件。

___

## 排除序列化

```csharp
// 没有 Guid 的组件、标签和事件在 EntitiesSnapshot 序列化时会被跳过
W.Types()
    .Component<DebugInfo>()     // 无 guid — 不会被序列化
    .Tag<EditorOnly>();         // 无 guid — 不会被序列化

// 世界快照（CreateWorldSnapshot）要求所有类型都有 Guid — 否则 DEBUG 模式下报错
// 实体快照（EntitiesSnapshot）会简单跳过没有 Guid 的类型

// 示例：保存所有实体，跳过调试数据
using var writer = W.Serializer.CreateEntitiesSnapshotWriter();
writer.WriteAllEntities();
byte[] snapshot = writer.CreateSnapshot();
byte[] gidSnapshot = W.Serializer.CreateGIDStoreSnapshot();
byte[] eventsSnapshot = W.Serializer.CreateEventsSnapshot();
```

___

## 压缩（GZIP）

所有快照创建和加载方法都支持 GZIP 压缩：

```csharp
// 世界
byte[] snapshot = W.Serializer.CreateWorldSnapshot(gzip: true);
W.Serializer.LoadWorldSnapshot(snapshot, gzip: true);

// 集群
byte[] cluster = W.Serializer.CreateClusterSnapshot(1, gzip: true);
W.Serializer.LoadClusterSnapshot(cluster, gzip: true);

// 块
byte[] chunk = W.Serializer.CreateChunkSnapshot(0, gzip: true);
W.Serializer.LoadChunkSnapshot(chunk, gzip: true);

// GID Store
byte[] gid = W.Serializer.CreateGIDStoreSnapshot(gzip: true);

// 事件
byte[] events = W.Serializer.CreateEventsSnapshot(gzip: true);
W.Serializer.LoadEventsSnapshot(events, gzip: true);

// 文件
W.Serializer.CreateWorldSnapshot("world.bin", gzip: true);
W.Serializer.LoadWorldSnapshot("world.bin", gzip: true);
```
