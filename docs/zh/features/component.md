---
title: 组件
parent: 功能
nav_order: 3
---

## Component
组件为实体提供数据和属性
- 以带有 `IComponent` 标记接口的用户自定义结构体表示
- 使用 struct 纯粹出于性能考虑（SoA 存储）
- 支持生命周期钩子：`OnAdd`、`OnDelete`、`CopyTo`、`Write`、`Read`
- 可以在不删除数据的情况下启用/禁用

#### 示例:
```csharp
public struct Position : IComponent {
    public Vector3 Value;
}

public struct Velocity : IComponent {
    public float Value;
}

public struct Name : IComponent {
    public string Val;
}
```

___

{: .importantzh }
需要在世界创建和初始化之间进行注册

```csharp
W.Create(WorldConfig.Default());
//...
// 不带配置的简单注册（适用于大多数情况）
W.Types()
    .Component<Position>()
    .Component<Velocity>()
    .Component<Name>();

// 带配置的注册
W.Types()
    .Component<Scale>(new ComponentTypeConfig<Scale>(
        guid: new Guid("..."),                           // 序列化的稳定标识符（默认 — 从类型名称自动计算）
        version: 1,                                      // 数据模式版本，用于迁移（默认 — 0）
        noDataLifecycle: false,                          // 禁用框架数据管理（默认 — false）
        readWriteStrategy: null,                         // 二进制序列化策略（默认 — 自动检测）
        defaultValue: new Scale { Value = Vector3.One }, // 初始化和删除时的默认值（默认 — 无）
        trackAdded: true,                                // 启用添加追踪（默认 — false），参见变更追踪
        trackDeleted: true                               // 启用删除追踪（默认 — false），参见变更追踪
    ));
//...
W.Initialize();
```

{: .notezh }
`noDataLifecycle` 参数控制框架是否管理组件数据生命周期。默认情况下（`noDataLifecycle: false`），框架会用 `defaultValue` 预初始化新存储，并在删除时将数据重置为 `defaultValue` — 因此 `entity.Add<T>()` 返回配置的默认值。设置 `noDataLifecycle: true` 时不执行初始化或清理 — 对于高频 unmanaged 类型很有用。如果定义了 `OnDelete`，无论此标志如何，钩子都负责清理。

{: .notezh }
无需手动传递配置，您可以直接在组件结构体内声明一个 `ComponentTypeConfig<T>` 类型的静态字段或属性。`RegisterAll()` 会按类型自动发现它（优先选择名为 `Config` 的成员）：

```csharp
public struct Health : IComponent {
    public float Value;
    public static readonly ComponentTypeConfig<Health> Config = new(
        defaultValue: new Health { Value = 100f }
    );
}
// RegisterAll() 会自动使用 Health.Config
```

___

#### 创建带组件的实体:
```csharp
// 创建空实体（无组件和标签）
W.Entity entity = W.NewEntity<Default>();

// 创建指定类型和集群的实体
W.Entity entity = W.NewEntity<Default>(clusterId: 0);

// 创建带组件的实体 — Set 返回 Entity（1 到 8 个组件的重载）
W.Entity entity = W.NewEntity<Default>().Set(new Position { Value = Vector3.One });
W.Entity entity = W.NewEntity<Default>().Set(
    new Position { Value = Vector3.One },
    new Velocity { Value = 1f }
);
```

___

#### 添加组件:
```csharp
// 不带值的 Add：如果组件已存在 → 返回现有数据的 ref，钩子不会被调用
// 如果不存在 → 用默认值初始化，调用 OnAdd
ref var position = ref entity.Add<Position>();

// 带 isNew 标志：isNew=true 表示组件是首次添加的
ref var position = ref entity.Add<Position>(out bool isNew);

// 一次添加多个组件（2 到 5 个的重载）
entity.Add<Position, Velocity>();

// 带值的 Set：始终覆盖数据
// 如果组件已存在 → OnDelete(旧值) → 替换 → OnAdd(新值)
// 如果不存在 → 设置值 → OnAdd
ref var position = ref entity.Set(new Position { Value = Vector3.One });

// 带值设置多个组件（2 到 12 个的重载）
entity.Set(new Position { Value = Vector3.One }, new Velocity { Value = 1f });
```

{: .importantzh }
不带值的 `Add<T>()` 和带值的 `Set<T>(T value)` 具有不同的钩子语义。
不带值：如果组件已存在，钩子**不会被调用**，返回当前数据的 ref。
带值：数据**始终被覆盖**，执行完整的 `OnDelete` → 替换 → `OnAdd` 周期。

___

#### 数据访问:
```csharp
// 获取组件的可变 ref 引用（读写）
// 不标记为 Changed — 使用 Mut<T>() 进行追踪访问
ref var velocity = ref entity.Ref<Velocity>();
velocity.Value += 10f;

// 获取组件的只读 ref 引用 — 不标记为 Changed
ref readonly var pos = ref entity.Read<Position>();
var x = pos.Value.x; // 读取 OK，无 Changed 标记

// 获取带追踪的可变 ref 引用 — 当 trackChanged 启用时标记为 Changed
ref var pos = ref entity.Mut<Position>();
pos.Value += delta; // 数据已修改且标记为 Changed
```

{: .importantzh }
`Ref<T>()` 不标记 Changed。需要变更追踪以配合 `AllChanged<T>` 过滤器时，请使用 `Mut<T>()`。在查询委托（`For`）中，`ref` 参数自动使用 `Mut` 语义。

___

#### 基本操作:
```csharp
// 获取实体上的组件数量
int count = entity.ComponentsCount();

// 检查组件是否存在（1 到 3 个的重载 — 检查是否全部存在）
// 无论 enabled/disabled 状态都会检查
bool has = entity.Has<Position>();
bool hasBoth = entity.Has<Position, Velocity>();
bool hasAll = entity.Has<Position, Velocity, Name>();

// 检查是否至少存在一个指定的组件（2 到 3 个的重载）
bool hasAny = entity.HasAny<Position, Velocity>();
bool hasAny3 = entity.HasAny<Position, Velocity, Name>();

// 删除组件（1 到 5 个的重载）
// 如果组件存在则调用 OnDelete；返回 true 表示已删除，false 表示不存在
bool deleted = entity.Delete<Position>();
entity.Delete<Position, Velocity>();
entity.Delete<Position, Velocity, Name>();
```

___

#### 启用/禁用:
```csharp
// 禁用组件 — 数据保留，但实体从标准查询中排除
// 返回 ToggleResult 枚举：MissingComponent、Unchanged、Changed
ToggleResult disabled = entity.Disable<Position>();
entity.Disable<Position, Velocity>();
entity.Disable<Position, Velocity, Name>();

// 重新启用组件
// 返回 ToggleResult 枚举：MissingComponent、Unchanged、Changed
ToggleResult enabled = entity.Enable<Position>();
entity.Enable<Position, Velocity>();
entity.Enable<Position, Velocity, Name>();

// 检查所有指定的组件是否已启用（1 到 3 个的重载）
bool posEnabled = entity.HasEnabled<Position>();
bool bothEnabled = entity.HasEnabled<Position, Velocity>();

// 检查是否至少有一个已启用（2 到 3 个的重载）
bool anyEnabled = entity.HasEnabledAny<Position, Velocity>();

// 检查所有指定的组件是否已禁用（1 到 3 个的重载）
bool posDisabled = entity.HasDisabled<Position>();
bool bothDisabled = entity.HasDisabled<Position, Velocity>();

// 检查是否至少有一个已禁用（2 到 3 个的重载）
bool anyDisabled = entity.HasDisabledAny<Position, Velocity>();
```

{: .notezh }
禁用的组件不会出现在标准查询过滤器（`All`、`None`、`Any`）中，但数据保留在内存中。使用 `WithDisabled`/`OnlyDisabled` 过滤器变体来处理禁用的组件。

___

#### 复制和移动:
```csharp
var source = W.NewEntity<Default>().Set(new Position(), new Velocity());
var target = W.NewEntity<Default>();

// 将指定组件复制到另一个实体（1 到 5 个的重载）
// 源实体保留其组件
// 如果 CopyTo 钩子已重写 — 使用自定义复制逻辑
// 如果 CopyTo 钩子未重写 — 通过 Add 进行按位复制 + 保留 disabled 状态
// 对于单个组件，如果源实体拥有该组件则返回 true
bool copied = source.CopyTo<Position>(target);
source.CopyTo<Position, Velocity>(target);

// 将指定组件移动到另一个实体（1 到 5 个的重载）
// 先 Copy 到目标实体，再从源实体 Delete（在源实体上触发 OnDelete）
bool moved = source.MoveTo<Position>(target);
source.MoveTo<Position, Velocity>(target);
```

___

#### 查询过滤器:

组件过滤器请参阅[查询 — 组件](query.md#组件)章节。

___

#### 生命周期钩子:

`IComponent` 接口提供了带有空默认实现的钩子 — 只需重写你需要的。

{: .importantzh }
不要留下空的钩子实现。如果不需要钩子 — 不要实现它。未实现的钩子不会被调用，不会产生开销。

```csharp
public struct Cooldown : IComponent {
    public float Duration;
    public float Elapsed;

    // 在组件添加后或通过 Set(value) 覆盖值后调用
    public void OnAdd<TWorld>(World<TWorld>.Entity self) where TWorld : struct, IWorldType {
        Elapsed = 0f; // 每次应用时重置计时器
    }

    // 在组件被删除（Delete）前、被覆盖（带值的 Set）前调用，
    // 以及在实体销毁时为每个组件调用
    //
    // 参数 `reason` 指示删除原因：
    // HookReason.Default      — 显式删除或实体销毁
    // HookReason.UnloadEntity — 实体/区块卸载
    // HookReason.WorldDestroy — 世界重置/销毁
    public void OnDelete<TWorld>(World<TWorld>.Entity self, HookReason reason) where TWorld : struct, IWorldType { }

    // CopyTo / MoveTo / Clone 的自定义复制逻辑
    // 如果未重写 — 按位复制 + 保留 disabled 状态
    // 如果已重写 — 完全替代默认复制逻辑
    public void CopyTo<TWorld>(World<TWorld>.Entity self, World<TWorld>.Entity other, bool disabled)
        where TWorld : struct, IWorldType {
        ref var copy = ref other.Add<Cooldown>();
        copy.Duration = Duration;
        copy.Elapsed = 0f; // 克隆从零开始
    }

    // 序列化 — 将组件写入二进制流
    // EntitiesSnapshot 必须实现（所有类型），non-unmanaged 类型在任何快照中也必须实现
    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType {
        writer.WriteFloat(Duration);
        writer.WriteFloat(Elapsed);
    }

    // 反序列化 — 从二进制流读取组件
    // version 参数支持在不同数据版本之间进行迁移
    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled)
        where TWorld : struct, IWorldType {
        Duration = reader.ReadFloat();
        Elapsed = reader.ReadFloat();
    }
}
```

{: .importantzh }
`Set(value)` 对已存在组件的钩子调用顺序：`OnDelete`(旧值) → 数据替换 → `OnAdd`(新值)。对于 `Delete` 或实体销毁，只调用 `OnDelete`。

___

#### 调试:
```csharp
// 将实体的所有组件收集到列表中（用于检查器/调试）
// 列表会在填充前被清除
var components = new List<IComponent>();
entity.GetAllComponents(components);
```
