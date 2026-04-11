---
title: 事件
parent: 功能
nav_order: 13
---

## Events
事件是系统之间或用户服务之间交换信息的机制
- 以带有 `IEvent` 标记接口的用户自定义数据结构体表示
- "发送者 → 多个接收者"模型，具有自动生命周期管理
- 每个接收者拥有独立的读取游标
- 当所有接收者都读取了事件或事件被抑制时，事件会自动删除

#### 示例:
```csharp
public struct WeatherChanged : IEvent {
    public WeatherType WeatherType;
}

public struct OnDamage : IEvent {
    public float Amount;
    public EntityGID Target;
}
```

___

{: .importantzh }
事件类型注册在 `Created` 和 `Initialized` 阶段均可用

```csharp
W.Create(WorldConfig.Default());
//...
// 简单注册
W.Types()
    .Event<WeatherChanged>()
    .Event<OnDamage>();

// 带配置的注册
W.Types()
    .Event<WeatherChanged>(new EventTypeConfig<WeatherChanged>(
        guid: new Guid("..."),      // 序列化的稳定标识符（默认 — 从类型名称自动计算）
        version: 1,                  // 数据模式版本，用于迁移（默认 — 0）
        readWriteStrategy: null      // 二进制序列化策略（默认 — 自动检测）
    ));
//...
W.Initialize();
```

{: .notezh }
无需手动传递配置，您可以在事件结构体内声明一个 `EventTypeConfig<T>` 类型的静态字段或属性。`RegisterAll()` 会按类型自动发现它（优先选择名为 `Config` 的成员）：

```csharp
public struct WeatherChanged : IEvent {
    public WeatherType WeatherType;
    public static readonly EventTypeConfig<WeatherChanged> Config = new(
        guid: new Guid("..."),
        version: 1
    );
}
// RegisterAll() 会自动使用 WeatherChanged.Config
```

___

#### 发送事件:
```csharp
// 发送带数据的事件
// 如果事件已添加到缓冲区则返回 true，如果没有注册的监听者则返回 false
bool sent = W.SendEvent(new WeatherChanged { WeatherType = WeatherType.Sunny });

// 发送默认值的事件
bool sent = W.SendEvent<OnDamage>();
```

{: .warningzh }
如果没有注册的监听者，`SendEvent` 返回 `false` 且事件**不会被存储**。请在发送事件前注册监听者。

___

#### 接收事件:
```csharp
// 创建监听者 — 每个监听者拥有独立的读取游标
var weatherReceiver = W.RegisterEventReceiver<WeatherChanged>();

// 发送事件
W.SendEvent(new WeatherChanged { WeatherType = WeatherType.Sunny });
W.SendEvent(new WeatherChanged { WeatherType = WeatherType.Rainy });

// 通过 foreach 读取事件
// 迭代后，事件对此监听者标记为已读
foreach (var e in weatherReceiver) {
    ref var data = ref e.Value; // ref 访问事件数据
    Console.WriteLine(data.WeatherType);
}

// 迭代时的附加事件信息
foreach (var e in weatherReceiver) {
    // 如果此监听者是最后一个读取此事件的，则为 true
    //（事件将在读取后被删除）
    bool last = e.IsLastReading();

    // 尚未读取此事件的监听者数量（不包括当前监听者）
    int remaining = e.UnreadCount();

    // 抑制事件 — 立即为所有剩余监听者删除该事件
    e.Suppress();
}
```

___

#### 监听者管理:
```csharp
// 通过委托读取所有事件
weatherReceiver.ReadAll(static (Event<WeatherChanged> e) => {
    Console.WriteLine(e.Value.WeatherType);
});

// 抑制此监听者的所有未读事件
// 事件被删除，其他监听者将无法再读取它们
weatherReceiver.SuppressAll();

// 将所有事件标记为已读但不处理
// 事件不会被删除 — 其他监听者仍然可以读取它们
weatherReceiver.MarkAsReadAll();

// 删除监听者
W.DeleteEventReceiver(ref weatherReceiver);
```

___

#### 多线程:

{: .warningzh }
发送事件（`SendEvent`）在以下条件下是线程安全的：
- 多个线程可以同时为**同一**事件类型调用 `SendEvent`
- **同时读取和发送**同一事件类型是**禁止的** — 只有在没有同时读取同一类型时，发送才是线程安全的
- 读取同一类型的事件（`foreach`、`ReadAll`）必须在**单个线程**中进行
- 不同的事件类型可以从**不同线程同时**读取，因为每种类型独立存储
- 同一事件类型可以在不同线程中**在不同时间**读取（非同时）

监听者操作（`foreach`、`ReadAll`、`MarkAsReadAll`、`SuppressAll`、创建和删除监听者）**不支持**多线程模式，只能在主线程中执行。

___

#### 事件生命周期:

{: .importantzh }
事件在以下两种情况下自动删除：
1. 所有注册的监听者都已读取该事件
2. 事件被抑制（`Suppress` 或 `SuppressAll`）

所有注册的监听者都必须读取其事件（或调用 `MarkAsReadAll`/`SuppressAll`），否则事件会在内存中累积。

```csharp
// 两个监听者的生命周期示例
var receiverA = W.RegisterEventReceiver<WeatherChanged>();
var receiverB = W.RegisterEventReceiver<WeatherChanged>();

W.SendEvent(new WeatherChanged { WeatherType = WeatherType.Sunny });
// 事件的 UnreadCount = 2

foreach (var e in receiverA) {
    // receiverA 已读取，UnreadCount = 1
}

foreach (var e in receiverB) {
    // receiverB 已读取，UnreadCount = 0 → 事件自动删除
}
```
