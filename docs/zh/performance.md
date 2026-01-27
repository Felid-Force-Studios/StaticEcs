---
title: 性能
parent: ZH
nav_order: 3
---

# 性能

## 架构特性

StaticEcs 为最大性能和大规模世界而设计：

- **实体在 Add/Remove 时永远不会在内存中移动** — 操作是位运算 O(1)。在基于原型的 ECS 中，添加或删除组件会导致实体在原型之间移动并复制所有数据。在 sparse set ECS 中，删除组件会将最后一个元素交换到被删除的位置（swap-back）

- **SoA 存储**（Structure of Arrays）— 相同类型的组件在内存中连续排列，确保迭代时最优的 CPU 缓存利用率。基于原型的 ECS 也在原型内部使用 SoA，但数据分散在不同原型的独立数组之间，原型数量呈组合增长。在 StaticEcs 中，相同类型的所有组件存储在统一的段数组中 — 使用大量 entityType 和集群时可能产生碎片化，但仍可控。Sparse set ECS 将组件存储在密集数组中，但访问同一实体的多个组件需要通过不同数组索引，元素顺序可能不同

- **静态泛型** — 通过 `Components<T>` 访问数据是编译时解析的直接静态字段访问。在其他 ECS 中，查找组件池需要按 type ID 进行哈希查找或通过带安全检查的 lookup 访问

- **无原型爆炸问题** — 在基于原型的 ECS 中，每种唯一的组件组合都会创建新的原型。当组件类型超过 30 种时，原型数量可能达到数千个，导致内存碎片化和迭代性能下降。StaticEcs 不存在此问题 — 组件类型数量不影响存储结构

- **热路径零分配** — 所有数据结构预分配，查询返回 ref struct 迭代器。在其他 ECS 中，首次创建 view/filter 可能需要分配，或通过包装器管理，有安全检查开销

- **二维分区**（Cluster × EntityType）— 在内存级别内置空间和逻辑分组，允许在不更改组件集的情况下控制实体放置。在其他 ECS 中，分组仅通过查询过滤器（标签、shared components）实现，无法直接控制内存布局

- **内置流式加载** — 加载/卸载集群和块无需重建内部结构。在基于原型的 ECS 中，大量创建或删除实体会导致块重新平衡。在 sparse set ECS 中，大量删除会使密集数组碎片化

- **可预测的性能** — Add/Remove/Has 操作时间不依赖于实体上的组件数量或世界中的类型总数。在基于原型的 ECS 中，结构变更的成本随组件数量增长（需复制实体的所有数据）。在 sparse set ECS 中，Has/Ref 成本恒定，但多组件迭代需要集合交集

___

## 迭代方式（从最快到最方便）

#### 1. ForBlock — 块指针（unmanaged 最快）：
```csharp
readonly struct MoveBlock : W.IQueryBlock.Write<Position>.Read<Velocity> {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(uint count, W.EntityBlock entities,
                       Block<Position> positions, BlockR<Velocity> velocities) {
        for (uint i = 0; i < count; i++) {
            positions[i].Value += velocities[i].Value;
        }
    }
}

W.Query().WriteBlock<Position>().Read<Velocity>().For<MoveBlock>();
```

#### 2. For 与功能结构体（零分配，有状态）：
```csharp
struct MoveFunction : W.IQuery.Write<Position>.Read<Velocity> {
    public float DeltaTime;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(W.Entity entity, ref Position pos, in Velocity vel) {
        pos.Value += vel.Value * DeltaTime;
    }
}

W.Query().Write<Position>().Read<Velocity>().For(new MoveFunction { DeltaTime = 0.016f });
```

#### 3. For 与委托（使用 static lambda 零分配）：
```csharp
// 无数据
W.Query().For(
    static (ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value;
    }
);

// 带用户数据（无捕获）
W.Query().For(deltaTime,
    static (ref float dt, ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value * dt;
    }
);
```

#### 4. Foreach 迭代（最灵活）：
```csharp
foreach (var entity in W.Query<All<Position, Velocity>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
    ref readonly var vel = ref entity.Read<Velocity>();
    pos.Value += vel.Value;
}
```

___

## IL2CPP 扩展方法

在 Unity 中使用 IL2CPP 时，标准泛型 Entity 方法（`entity.Ref<T>()`、`entity.Has<T>()`）由于 AOT 编译特性可能慢 10–25%。建议创建类型化的扩展方法：

```csharp
public static class ComponentExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref Position RefPosition(this W.Entity entity) {
        return ref W.Components<Position>.Instance.Ref(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasPosition(this W.Entity entity) {
        return W.Components<Position>.Instance.Has(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasTagPlayer(this W.Entity entity) {
        return W.Tags<IsPlayer>.Instance.Has(entity);
    }
}
```

```csharp
// 使用 — 方便且快速
ref var pos = ref entity.RefPosition();
bool has = entity.HasPosition();
bool isPlayer = entity.HasTagPlayer();
```

{: .notezh }
在 Mono/CoreCLR 中由于 JIT 积极内联，差异很小。此优化专门针对 IL2CPP。

___

## 并行执行

要启用多线程查询，在世界配置中指定模式：

```csharp
W.Create(new WorldConfig {
    ParallelQueryType = ParallelQueryType.MaxThreadsCount,
    // 或
    // ParallelQueryType = ParallelQueryType.CustomThreadsCount,
    // CustomThreadCount = 8,
});
```

```csharp
// 并行迭代
W.Query().ForParallel(
    static (ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value;
    },
    minEntitiesPerThread: 50000  // 每个线程的最小实体数
);
```

{: .importantzh }
并行迭代限制：只能修改/销毁当前实体。不能创建实体，不能修改其他实体。`SendEvent` 是线程安全的（在没有同时读取同一类型时）。

___

## 实体类型（entityType）

`entityType` 将逻辑相似的实体分组到相邻的内存段中，提高缓存局部性：

```csharp
struct UnitType : IEntityType { }
struct BulletType : IEntityType { }
struct EffectType : IEntityType { }

// 单位在内存中相邻
var unit = W.NewEntity<UnitType>();
unit.Add<Position>(); unit.Add<Health>();

// 子弹 — 在自己的段中
var bullet = W.NewEntity<BulletType>();
bullet.Add<Position>(); bullet.Add<Velocity>();
```

查询自动遍历连续的内存块 — 数据越同质，CPU 缓存效率越高。

___

## 集群范围查询

将查询限制到特定集群可以跳过无关的块：

```csharp
const ushort ACTIVE_ZONE = 1;
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { ACTIVE_ZONE };

// 仅遍历指定集群
W.Query().For(
    static (ref Position pos) => { pos.Value.Y -= 9.8f * 0.016f; },
    clusters: clusters
);
```

___

## 批量操作

批量操作在位掩码级别工作 — 单次位运算一次影响多达 64 个实体。这比逐实体迭代快几个数量级。

#### 可用操作：

| 方法 | 描述 |
|------|------|
| `BatchAdd<T>()` | 添加组件（默认值，1–5 个类型） |
| `BatchSet<T>(value)` | 添加带值的组件（1–5 个类型） |
| `BatchDelete<T>()` | 删除组件（1–5 个类型） |
| `BatchEnable<T>()` | 启用组件（1–5 个类型） |
| `BatchDisable<T>()` | 禁用组件（1–5 个类型） |
| `BatchSet<T>()` | 设置标签（1–5 个类型） |
| `BatchDelete<T>()` | 删除标签（1–5 个类型） |
| `BatchToggle<T>()` | 切换标签（1–5 个类型） |
| `BatchApply<T>(bool)` | 按条件设置或取消标签（1–5 个类型） |
| `BatchDestroy()` | 销毁所有匹配实体 |
| `BatchUnload()` | 卸载所有匹配实体 |
| `EntitiesCount()` | 计算匹配实体数量 |

#### 示例：
```csharp
// 链式操作 — 添加组件、设置标签、禁用组件
W.Query<All<Position>>()
    .BatchSet(new Velocity { Value = Vector3.One })
    .BatchSet<IsMovable>()
    .BatchDisable<Position>();

// 销毁所有带 IsDead 标签的实体
W.Query<All<Health, IsDead>>().BatchDestroy();

// 计算实体数量
int count = W.Query<All<Position, Velocity>>().EntitiesCount();

// 按集群和实体状态过滤
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { 1, 2 };
W.Query<All<Position>>().BatchDelete<Velocity>(
    entities: EntityStatusType.Any,
    clusters: clusters
);

// 切换标签 — 有标签的会被移除；没有的会被设置
W.Query<All<Position>>().BatchToggle<IsVisible>();
```

{: .notezh }
所有批量操作支持按 `EntityStatusType`（Enabled/Disabled/Any）和 `clusters` 过滤。方法返回 `WorldQuery` 以支持链式调用。

___

## QueryMode

默认使用 `QueryMode.Strict` — 最快的模式。仅在迭代期间需要修改**其他**实体的过滤组件/标签时使用 `QueryMode.Flexible`：

```csharp
// Strict（默认）— 完整块的快速路径
W.Query().For(
    static (ref Position pos) => { /* ... */ }
);

// Flexible — 每次迭代重新检查位掩码
W.Query().For(
    static (W.Entity entity, ref Position pos) => {
        // 可以修改其他实体上的 Position
    },
    queryMode: QueryMode.Flexible
);
```

___

## 代码裁剪（减小构建大小）

StaticEcs 大量使用泛型重载：Query0–Query6 × 委托变体 × Read 变体 × Parallel — 这会产生大量泛型特化，其中大部分在任何给定项目中都不会使用。要从构建中移除未使用的代码并显著减小其大小，请使用托管代码裁剪。

#### Unity:
将 **Player Settings → Other Settings → Managed Stripping Level** 设置为 **Medium** 或 **High**。这会移除库生成的未引用泛型实例化。

#### .NET（发布裁剪）：
```xml
<PropertyGroup>
    <PublishTrimmed>true</PublishTrimmed>
    <TrimMode>link</TrimMode>
</PropertyGroup>
```

{: .importantzh }
启用裁剪后请彻底测试您的构建 — 激进的裁剪可能会移除仅通过反射访问的代码。如果您使用 `RegisterAll` 进行自动发现，请确保相关类型被保留（例如，在 Unity 中通过 `[Preserve]` 特性或在 .NET 中通过 TrimmerRootAssembly）。

___

## 建议

| 实践 | 原因 |
|------|------|
| 关键循环使用 `ForBlock` | 直接指针，最小开销 |
| `For` 中使用 `static` lambda | 零分配，JIT 内联 |
| 只读组件使用 `in` | 正确的变更追踪语义 |
| 按 `entityType` 分组实体 | 缓存局部性 |
| 将查询限制到集群 | 跳过无关块 |
| 默认 `QueryMode.Strict` | 比 Flexible 快 10–40% |
| 批量操作进行大量修改 | 每 64 个实体一次操作 |
| Unity 中使用 Medium/High 裁剪 | 移除未使用的泛型重载 |
| 序列化使用 `UnmanagedPackArrayStrategy<T>` | 块内存复制 |
| IL2CPP 类型化扩展方法 | 比泛型 Entity 包装快 10–25% |
