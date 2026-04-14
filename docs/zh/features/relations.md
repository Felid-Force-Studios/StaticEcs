---
title: 关系
parent: 功能
nav_order: 9
---

## 关系
关系是通过类型化链接组件将实体相互连接的机制
- `Link<T>` — 与单个实体的链接（`EntityGID` 的包装器）
- `Links<T>` — 与多个实体的链接（`Link<T>` 的动态集合）
- 链接是普通组件，通过标准 API（`Add`、`Ref`、`Delete`、`Has`）操作
- 支持钩子（`OnAdd`、`OnDelete`、`CopyTo`）用于自动化逻辑（例如反向引用）

___

## 链接类型

要定义链接类型，请实现以下接口之一：

```csharp
// ILinkType — 单链接类型（Link<T>）
// 只实现需要的钩子
public struct Parent : ILinkType {
    // 添加链接时调用
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        // self — 添加了链接的实体
        // link — 目标实体的 GID
    }

    // 删除链接时调用
    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        // ...
    }

    // 复制实体时调用（Clone/CopyTo）
    public void CopyTo<TW>(World<TW>.Entity self, World<TW>.Entity other, EntityGID link) where TW : struct, IWorldType {
        // ...
    }
}

// ILinksType — 多链接类型（Links<T>）
// 继承自 ILinkType，相同的钩子
public struct Children : ILinksType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        // ...
    }
}

// 无钩子类型 — 不实现任何方法即可
public struct FollowTarget : ILinkType { }
```

{: .importantzh }
不要留下空的钩子实现。如果不需要钩子 — 不要实现它。未实现的钩子不会被调用，不会产生开销。

___

## Link\<T\>

单链接组件 — `EntityGID`（8 字节）的包装器。

```csharp
// 属性
EntityGID value = link.Value;    // 目标实体的 GID（只读）

// 隐式转换
W.Link<Parent> link = entity;              // Entity → Link<T>
W.Link<Parent> link = entity.GID;          // EntityGID → Link<T>
W.Link<Parent> link = entity.GIDCompact;   // EntityGIDCompact → Link<T>
EntityGID gid = link;                      // Link<T> → EntityGID

// 通过构造函数创建
var link = new W.Link<Parent>(targetGID);

// 通过 entity.AsLink 创建
W.Link<Parent> link = entity.AsLink<Parent>();
```

___

## Links\<T\>

多组件 — 具有自动内存管理的 `Link<T>` 动态集合。

#### 属性：
```csharp
ref var links = ref entity.Ref<W.Links<Children>>();

ushort len = links.Length;       // 元素数量
ushort cap = links.Capacity;     // 当前容量
bool empty = links.IsEmpty;      // 为空
bool notEmpty = links.IsNotEmpty; // 不为空
bool full = links.IsFull;        // 已填满

// 索引访问
W.Link<Children> first = links[0];
W.Link<Children> last = links[links.Length - 1];

// 第一个和最后一个元素
W.Link<Children> f = links.First();
W.Link<Children> l = links.Last();

// 只读 Span
ReadOnlySpan<W.Link<Children>> span = links.AsReadOnlySpan;

// 迭代
foreach (var link in links) {
    if (link.Value.TryUnpack<WT>(out var child)) {
        // ...
    }
}
```

#### 添加：
```csharp
// TryAdd — 如果已存在则不添加，返回 false
bool added = links.TryAdd(childLink);

// TryAdd 多个（2 到 4 个）
links.TryAdd(child1, child2);
links.TryAdd(child1, child2, child3, child4);

// Add — 添加，在 DEBUG 中遇到重复时抛出错误
links.Add(childLink);
links.Add(child1, child2);

// 从数组添加
links.Add(childArray);
links.Add(childArray, srcIdx: 0, len: 3);
```

#### 删除：
```csharp
// 按值删除（如果找到则返回 true）
bool removed = links.TryRemove(childLink);

// 按值 swap-remove（不保留顺序，更快）
bool removed = links.TryRemoveSwap(childLink);

// 按索引删除
links.RemoveAt(0);
links.RemoveAtSwap(0);

// 第一个 / 最后一个
links.RemoveFirst();
links.RemoveFirstSwap();
links.RemoveLast();

// 删除所有（为每个元素调用 OnDelete）
links.Clear();
```

#### 搜索：
```csharp
bool exists = links.Contains(childLink);
int idx = links.IndexOf(childLink);
```

#### 内存管理：
```csharp
links.EnsureSize(10);        // 确保额外 10 个元素的空间
links.Resize(32);            // 更改容量
links.Sort();                // 排序
```

___

## 注册

链接作为普通组件在创建世界时注册：

```csharp
W.Create(WorldConfig.Default());

W.Types()
    .Link<Parent>()
    .Links<Children>();

W.Initialize();
```

___

## 使用链接

链接是普通组件。所有标准方法都可用：

```csharp
var parent = W.NewEntity<Default>();
var child1 = W.NewEntity<Default>();
var child2 = W.NewEntity<Default>();

// 添加单链接
child1.Add(new W.Link<Parent>(parent));
child2.Add(new W.Link<Parent>(parent));

// 获取引用
ref var parentLink = ref child1.Ref<W.Link<Parent>>();
EntityGID parentGID = parentLink.Value;

// 检查存在性
bool hasParent = child1.Has<W.Link<Parent>>();

// 删除链接
child1.Delete<W.Link<Parent>>();

// 添加多链接
ref var children = ref parent.Add<W.Links<Children>>();
children.TryAdd(child1.AsLink<Children>());
children.TryAdd(child2.AsLink<Children>());

// 读取多链接
ref var kids = ref parent.Ref<W.Links<Children>>();
for (int i = 0; i < kids.Length; i++) {
    if (kids[i].Value.TryUnpack<WT>(out var childEntity)) {
        // 操作子实体
    }
}
```

___

## 扩展方法

通过 `EntityGID` 进行安全链接操作 — 自动检查目标实体是否已加载且有效。

### Link（单链接）：
```csharp
// 向目标实体添加 Link<T> 组件
LinkOppStatus status = targetGID.TryAddLink<WT, Parent>(linkEntity);

// 从目标实体删除 Link<T> 组件
LinkOppStatus status = targetGID.TryDeleteLink<WT, Parent>(linkEntity);

// 深度销毁 — 递归销毁链接的实体链
targetGID.DeepDestroyLink<WT, Parent>();

// 深度复制 — 克隆目标实体并返回副本的链接
LinkOppStatus status = sourceGID.TryDeepCopyLink<WT, Parent>(out W.Link<Parent> copied);
```

### Links（多链接）：
```csharp
// 向目标实体的 Links<T> 添加元素
// 如果 Links<T> 组件不存在则自动创建
LinkOppStatus status = targetGID.TryAddLinkItem<WT, Children>(linkEntity);

// 从目标实体的 Links<T> 删除元素
// 如果集合变空则自动删除 Links<T> 组件
LinkOppStatus status = targetGID.TryDeleteLinkItem<WT, Children>(linkEntity);

// 深度销毁 — 递归销毁所有链接的实体
targetGID.DeepDestroyLinkItem<WT, Children>();
```

### LinkOppStatus：
```csharp
// 操作结果
switch (status) {
    case LinkOppStatus.Ok:                // 操作成功完成
    case LinkOppStatus.LinkAlreadyExists: // 链接已存在（TryAdd）
    case LinkOppStatus.LinkNotExists:     // 链接未找到（TryDelete）
    case LinkOppStatus.LinkNotLoaded:     // 目标实体在未加载的区块中
    case LinkOppStatus.LinkNotActual:     // GID 已过期（实体已销毁，槽位被复用）
}
```

___

## 链接示例

### 单向链接（无钩子）

最简单的情况 — 实体引用另一个实体，无反向引用。

```csharp
// 无钩子类型
public struct FollowTarget : ILinkType { }

// 注册
W.Types()
    .Link<FollowTarget>();
```

```csharp
//  A FollowTarget→ B

var unit = W.NewEntity<Default>();
var target = W.NewEntity<Default>();

// 设置追踪目标
unit.Set(new W.Link<FollowTarget>(target));

// 在移动系统中
W.Query().For(static (W.Entity entity, ref W.Link<FollowTarget> follow) => {
    if (follow.Value.TryUnpack<WT>(out var targetEntity)) {
        ref var myPos = ref entity.Ref<Position>();
        ref readonly var targetPos = ref targetEntity.Read<Position>();
        // 向目标移动
    }
});
```

___

### 双向一对一（相同类型）

封闭对 — 两个实体用相同类型相互引用。

```csharp
//    MarriedTo
//  A ────────→ B
//  A ←──────── B
//    MarriedTo

public struct MarriedTo : ILinkType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        link.TryAddLink<TW, MarriedTo>(self);
    }

    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        link.TryDeleteLink<TW, MarriedTo>(self);
    }
}

W.Types()
    .Link<MarriedTo>();
```

```csharp
var alice = W.NewEntity<Default>();
var bob = W.NewEntity<Default>();

// 从一侧设置 — 反向引用自动创建
alice.Set(new W.Link<MarriedTo>(bob));
// 现在：alice 有 Link<MarriedTo> → bob
//       bob 有 Link<MarriedTo> → alice

// 删除也是双向的
alice.Delete<W.Link<MarriedTo>>();
// 现在：两个组件都已删除
```

___

### 双向一对一（不同类型）

两个实体用不同链接类型连接。

```csharp
//  A ←Rider── Mount──→ B

public struct Mount : ILinkType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        link.TryAddLink<TW, Rider>(self);
    }

    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        link.TryDeleteLink<TW, Rider>(self);
    }
}

public struct Rider : ILinkType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        link.TryAddLink<TW, Mount>(self);
    }

    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        link.TryDeleteLink<TW, Mount>(self);
    }
}

W.Types()
    .Link<Mount>()
    .Link<Rider>();
```

```csharp
var player = W.NewEntity<Default>();
var horse = W.NewEntity<Default>();

player.Set(new W.Link<Mount>(horse));
// player 有 Link<Mount> → horse
// horse 有 Link<Rider> → player
```

___

### 双向一对多（Parent ↔ Children）

父实体和子实体 — 经典层级关系。

```csharp
//      ←Parent  Children→ child1
//     /
//  parent ←Parent  Children→ child2
//     \
//      ←Parent  Children→ child3

public struct Parent : ILinkType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        link.TryAddLinkItem<TW, Children>(self);
    }

    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        link.TryDeleteLinkItem<TW, Children>(self);
    }
}

public struct Children : ILinksType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        link.TryAddLink<TW, Parent>(self);
    }

    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        link.TryDeleteLink<TW, Parent>(self);
    }
}

W.Types()
    .Link<Parent>()
    .Links<Children>();
```

```csharp
var father = W.NewEntity<Default>();
var son = W.NewEntity<Default>();
var daughter = W.NewEntity<Default>();

// 从子实体侧设置
son.Set(new W.Link<Parent>(father));
daughter.Set(new W.Link<Parent>(father));
// father 自动获得 Links<Children> → [son, daughter]

// 或从父实体侧添加
ref var kids = ref father.Ref<W.Links<Children>>();
var newChild = W.NewEntity<Default>();
kids.TryAdd(newChild.AsLink<Children>());
// newChild 自动获得 Link<Parent> → father
```

{: .notezh }
扩展方法 `TryAddLink`/`TryDeleteLink`/`TryAddLinkItem`/`TryDeleteLinkItem` 中的 `withCyclicHooks: false`（默认值）是一种优化：从钩子中调用时，无需调用对方的钩子，因为它已经在执行中。

___

### 单向一对多链接（To-Many）

实体引用多个其他实体，无反向引用。

```csharp
//      Targets→ B
//     /
//  A── Targets→ C
//     \
//      Targets→ D

public struct Targets : ILinksType { }

W.Types()
    .Links<Targets>();
```

```csharp
var turret = W.NewEntity<Default>();
var enemy1 = W.NewEntity<Default>();
var enemy2 = W.NewEntity<Default>();

ref var targets = ref turret.Add<W.Links<Targets>>();
targets.TryAdd(enemy1.AsLink<Targets>());
targets.TryAdd(enemy2.AsLink<Targets>());
```

___

### 双向多对多（Many-To-Many）

双方都存储对彼此的引用集合。

```csharp
//      ←Owners  Memberships→ groupA
//     /
//  user1 ←Owners  Memberships→ groupB
//
//  user2 ←Owners  Memberships→ groupA

public struct Memberships : ILinksType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        link.TryAddLinkItem<TW, Owners>(self);
    }

    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        link.TryDeleteLinkItem<TW, Owners>(self);
    }
}

public struct Owners : ILinksType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        link.TryAddLinkItem<TW, Memberships>(self);
    }

    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        link.TryDeleteLinkItem<TW, Memberships>(self);
    }
}

W.Types()
    .Links<Memberships>()
    .Links<Owners>();
```

```csharp
var user1 = W.NewEntity<Default>();
var user2 = W.NewEntity<Default>();
var groupA = W.NewEntity<Default>();
var groupB = W.NewEntity<Default>();

// 将 user1 添加到两个组
ref var memberships = ref user1.Add<W.Links<Memberships>>();
memberships.TryAdd(groupA.AsLink<Memberships>());
memberships.TryAdd(groupB.AsLink<Memberships>());
// groupA 和 groupB 自动获得 Links<Owners> → [user1]

// 将 user2 添加到 groupA
ref var memberships2 = ref user2.Add<W.Links<Memberships>>();
memberships2.TryAdd(groupA.AsLink<Memberships>());
// groupA 现在有 Links<Owners> → [user1, user2]
```

___

## Links 批量段序列化

对于使用 unmanaged 链接类型的 chunk/world/cluster 快照，`LinksUnmanagedPackArrayStrategy` 会自动应用——无需手动配置。

要为链接注册提供自定义配置，请在链接类型上实现 `ILinksConfig<T>`：

```csharp
public struct MyLinkType : ILinksType, ILinksConfig<MyLinkType> {
    public ComponentTypeConfig<W.Links<MyLinkType>> Config<TWorld>() where TWorld : struct, IWorldType => new(
        guid: new Guid("...")
    );
}
```

工作方式与 `MultiUnmanagedPackArrayStrategy` 相同——详情参见[多组件批量段序列化](multicomponent.md#批量段序列化)。

___

## 多线程

{: .warningzh }
在 `ForParallel` 中只允许修改**当前**迭代的实体。修改**其他**实体状态的链接钩子（例如向父实体添加反向引用）在并行迭代期间会在 DEBUG 中引发错误。

要在并行查询中使用链接，请使用**事件** — `SendEvent` 是线程安全的（在没有同时读取同一类型时，详见[事件](events#多线程)），可以从任何线程调用。在并行迭代完成后，在主线程上处理事件逻辑。

#### 示例：通过事件延迟删除链接

```csharp
// 1. 定义事件
public struct DeleteLinkEvent<TLink> : IEvent where TLink : unmanaged, ILinkType {
    public EntityGID Target;    // 需要删除链接的实体
    public EntityGID Link;      // 用于验证的链接值
}

// 2. 注册事件并创建接收器
W.Types()
    .Event<DeleteLinkEvent<Parent>>();
var deleteLinkReceiver = W.RegisterEventReceiver<DeleteLinkEvent<Parent>>();

// 将接收器存储在世界资源中以便从系统中访问
W.SetResource(deleteLinkReceiver);
```

```csharp
// 3. 定义链接类型，不在钩子中修改其他实体
public struct Parent : ILinkType {
    // 在 OnDelete 中，不直接修改父实体 — 而是发送事件
    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        World<TW>.SendEvent(new DeleteLinkEvent<Parent> {
            Target = link,
            Link = self.GID
        });
    }
}
```

```csharp
// 4. 并行迭代 — 安全的，钩子发送事件而非直接修改
W.Query().ForParallel(
    static (W.Entity entity, ref W.Link<Parent> parent) => {
        if (someCondition) {
            entity.Delete<W.Link<Parent>>();
            // OnDelete 将发送 DeleteLinkEvent 而非修改父实体
        }
    },
    minEntitiesPerThread: 1000
);

// 5. 在主线程上处理所有事件
ref var receiver = ref W.GetResource<EventReceiver<WT, DeleteLinkEvent<Parent>>>();
receiver.ReadAll(static (W.Event<DeleteLinkEvent<Parent>> e) => {
    // 现在可以安全地修改其他实体
    ref var data = ref e.Value;
    data.Target.TryDeleteLinkItem<WT, Children>(data.Link.Unpack<WT>());
});
```

___

## 查询

链接组件在查询中像任何其他组件一样使用：

```csharp
// 所有有父实体的实体
foreach (var entity in W.Query<All<W.Link<Parent>>>().Entities()) {
    ref var parentLink = ref entity.Ref<W.Link<Parent>>();
    // ...
}

// 所有有子实体但没有父实体的实体（根实体）
W.Query<All<W.Links<Children>>, None<W.Link<Parent>>>()
    .For(static (W.Entity entity, ref W.Links<Children> kids) => {
        // 根实体
    });

// 通过委托
W.Query().For(static (ref W.Link<Parent> parent) => {
    if (parent.Value.TryUnpack<WT>(out var parentEntity)) {
        // ...
    }
});
```
