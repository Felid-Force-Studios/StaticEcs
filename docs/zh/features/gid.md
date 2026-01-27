---
title: 实体全局标识符
parent: 功能
nav_order: 2
---

## EntityGID
实体全局标识符 — 对实体的稳定引用，可安全用于存储、序列化和网络传输
- 用于[事件](events.md)、[实体关系](relations.md)、[序列化](serialization.md)、网络交互
- 包含 Id、Version 和 ClusterId — 通过版本检查检测过期引用
- 在创建实体时自动分配，或通过 `NewEntityByGID` 手动指定
- 8 字节结构体（`StructLayout.Explicit`，字段通过 `Raw` 重叠）

___

#### 获取:
```csharp
// 实体上的属性
EntityGID gid = entity.GID;

// 隐式转换 Entity → EntityGID
EntityGID gid = entity;

// 通过构造函数
EntityGID gid = new EntityGID(id: 0, version: 1, clusterId: 0);
EntityGID gid = new EntityGID(rawValue: 16777216UL);
```

___

#### 属性:
```csharp
EntityGID gid = entity.GID;

uint id = gid.Id;               // 内部实体槽位索引
ushort version = gid.Version;   // 代计数器（槽位复用时递增）
ushort clusterId = gid.ClusterId; // 集群标识符
uint chunk = gid.Chunk;         // 区块索引（计算得出）
ulong raw = gid.Raw;            // 原始 8 字节表示（所有字段打包）
```

___

#### 验证和解包:
```csharp
EntityGID gid = entity.GID;

// 检查 GID 状态：Active、NotActual 或 NotLoaded
GIDStatus status = gid.Status<WT>();

// 安全解包 — 如果实体已加载且是最新的则返回 true
if (gid.TryUnpack<WT>(out var entity)) {
    ref var pos = ref entity.Ref<Position>();
}

// 带失败诊断
if (!gid.TryUnpack<WT>(out var entity, out GIDStatus status)) {
    // status == GIDStatus.NotActual → 实体不存在或版本/集群不匹配（过期引用）
    // status == GIDStatus.NotLoaded → 实体存在且版本匹配，但当前已卸载
}

// 不安全解包 — 在 DEBUG 中如果未加载或过期将抛出错误
var entity = gid.Unpack<WT>();
```

___

#### 使用指定 GID 创建实体:
```csharp
// 在 GID 指定的确切槽位创建实体
// 用于反序列化和网络同步
var entity = W.NewEntityByGID<Default>(gid);

// 非泛型变体（实体类型在运行时作为 byte 已知）
byte entityTypeId = EntityTypeInfo<Default>.Id;
var entity = W.NewEntityByGID(entityTypeId, gid);
```

___

#### 失效:
```csharp
// 递增版本而不销毁实体
// 所有之前获取的 GID 将变为过期（Status 返回 GIDStatus.NotActual）
entity.UpVersion();
```

___

#### 比较:
```csharp
EntityGID a = entity1.GID;
EntityGID b = entity2.GID;

bool eq = a == b;           // 按 Raw 比较（8 字节）
bool eq = a.Equals(b);      // 相同

// 与 EntityGIDCompact 的跨类型比较
EntityGIDCompact compact = entity1.GIDCompact;
bool eq = a == compact;     // 按 Id、Version、ClusterId 比较
bool eq = a.Equals(compact);

// 显式窄化转换为 EntityGIDCompact
// 在 DEBUG 中如果 Chunk >= 4 或 ClusterId >= 4 将抛出错误
EntityGIDCompact compact = (EntityGIDCompact)gid;
```

___

## EntityGIDCompact
EntityGID 的紧凑版本 — 4 字节而非 8 字节，用于内存受限的场景
- 位打包：`[31..16]` Version，`[15..14]` ClusterId（2 位），`[13..12]` Chunk（2 位），`[11..0]` 区块内实体索引
- 限制：最多 4 个区块（约 16,384 个实体），最多 4 个集群
- 在 DEBUG 中超出限制时抛出错误

#### 获取:
```csharp
EntityGIDCompact gid = entity.GIDCompact;

// 显式转换 Entity → EntityGIDCompact
EntityGIDCompact gid = (EntityGIDCompact)entity;

// 通过构造函数
EntityGIDCompact gid = new EntityGIDCompact(id: 0, version: 1, clusterId: 0);
EntityGIDCompact gid = new EntityGIDCompact(raw: 16777216U);
```

___

#### 验证和解包:
```csharp
// API 与 EntityGID 相同
GIDStatus status = gid.Status<WT>();

if (gid.TryUnpack<WT>(out var entity)) {
    // ...
}

var entity = gid.Unpack<WT>();

// 隐式宽化转换为 EntityGID（始终安全）
EntityGID full = gid;
```

___

## 使用示例

#### 事件:
```csharp
public struct OnDamage : IEvent {
    public EntityGID Target;
    public float Amount;
}

// 在系统中：
foreach (var e in damageReceiver) {
    ref var data = ref e.Value;
    if (data.Target.TryUnpack<WT>(out var target)) {
        ref var health = ref target.Ref<Health>();
        health.Current -= data.Amount;
    }
}
```

#### 服务器-客户端网络交互:
GID 可以用作客户端和服务器之间的实体绑定标识符。
服务器创建实体，将 GID 发送给客户端，客户端使用相同的 GID 创建实体 —
后续带有 GID 的命令使客户端可以通过 `TryUnpack` 轻松找到对应的实体。

```csharp
public struct CreateEntityCommand {
    public EntityGID Id;
    public string Prefab;
}

// 服务器：
var serverEntity = W.NewEntity<Default>();
client.Send(new CreateEntityCommand { Id = serverEntity.GID, Prefab = "player" });

// 客户端：
var cmd = server.Receive<CreateEntityCommand>();
var clientEntity = ClientW.NewEntityByGID<Default>(cmd.Id);
```
