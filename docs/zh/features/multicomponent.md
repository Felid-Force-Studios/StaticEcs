---
title: 多组件
parent: 功能
nav_order: 5
---

## MultiComponent
多组件是优化的列表组件，允许在单个实体上存储多个相同类型的值
- 所有实体的所有同类型多组件的所有元素存储在统一存储中 — 最优内存使用
- 每个组件容量从 4 到 32768 个值，自动扩展
- 无需在组件内创建数组或列表 — 零堆分配
- 实现了[组件](component.md)，所有基本规则适用
- 实体[关系](relations.md)（`Links<T>`）基于多组件构建

___

## 类型定义

多组件值类型必须实现接口 `IMultiComponent` 并且为 `struct`：

```csharp
// Unmanaged 类型 — 通过批量内存复制自动序列化
public struct Item : IMultiComponent {
    public int Id;
    public float Weight;
}
```

非 unmanaged（managed）类型必须实现 `Write`/`Read` 钩子以支持序列化：

```csharp
// Managed 类型 — 需要 Write/Read 钩子用于序列化
public struct NamedItem : IMultiComponent {
    public string Name;
    public int Count;

    public void Write(ref BinaryPackWriter writer) {
        writer.Write(in Name);
        writer.Write(in Count);
    }

    public void Read(ref BinaryPackReader reader) {
        Name = reader.Read<string>();
        Count = reader.ReadInt();
    }
}
```

___

## 序列化策略

元素序列化策略自动选择：
- 对于 **unmanaged** 类型 — `UnmanagedPackArrayStrategy<T>`（批量内存复制，更快）
- 对于 **managed** 类型 — `StructPackArrayStrategy<T>`（逐元素通过 `Write`/`Read` 钩子）

要覆盖策略或提供自定义配置，请实现 `IMultiComponentConfig<T>`：

```csharp
public struct Item : IMultiComponent, IMultiComponentConfig<Item> {
    public int Id;
    public float Weight;

    public ComponentTypeConfig<W.Multi<Item>> Config<TWorld>() where TWorld : struct, IWorldType => default;
    public IPackArrayStrategy<Item> ElementPackStrategy() => new UnmanagedPackArrayStrategy<Item>();
}
```

___

## 批量段序列化

对于 chunk/world/cluster 快照，当 `TValue` 为 unmanaged 类型时，可以使用 `MultiUnmanagedPackArrayStrategy<TWorld, TValue>` 将整个存储段作为内存块序列化，而不是逐实体序列化元素数据。这将许多小的逐实体拷贝替换为每段一次批量操作，并直接恢复分配器状态。

对于 unmanaged 类型，`MultiUnmanagedPackArrayStrategy` 会自动应用。要提供自定义配置：

```csharp
public struct Item : IMultiComponent, IMultiComponentConfig<Item> {
    public int Id;
    public float Weight;

    public ComponentTypeConfig<W.Multi<Item>> Config<TWorld>() where TWorld : struct, IWorldType => new(
        guid: new Guid("...")
    );
    public IPackArrayStrategy<Item> ElementPackStrategy() => null; // null = 自动检测
}
```

{: .note }
此策略序列化 `Multi<T>` 结构体的原始字节加上底层值存储段和分配器状态。实体级序列化（`EntitiesSnapshot`）继续使用逐实体 `Write`/`Read` 钩子——此优化仅适用于 chunk/world/cluster 快照。

{: .important }
`MultiUnmanagedPackArrayStrategy` 要求 `Multi<TValue>` 满足 `unmanaged` 约束。由于 `Multi<T>` 的字段均为值类型，这适用于具体的 `TValue` 类型，但**不能在泛型注册代码中使用**——请为每个具体类型显式指定。

___

## 注册

```csharp
W.Create(WorldConfig.Default());

W.Types()
    .Multi<Item>()         // 自动检测策略（unmanaged 类型使用 UnmanagedPackArrayStrategy）
    .Multi<NamedItem>();   // managed 类型 — 使用 StructPackArrayStrategy 及 Write/Read 钩子

W.Initialize();
```

___

## 基本操作

多组件像普通组件一样工作：

```csharp
// 添加（初始容量 — 4 个元素，自动扩展）
ref var items = ref entity.Add<W.Multi<Item>>();

// 获取引用
ref var items = ref entity.Ref<W.Multi<Item>>();

// 检查存在性
bool has = entity.Has<W.Multi<Item>>();

// 删除（元素列表自动清除）
entity.Delete<W.Multi<Item>>();

// 克隆和复制时 — 所有元素自动复制
var clone = entity.Clone();
entity.CopyTo<W.Multi<Item>>(targetEntity);
```

___

## 属性

```csharp
ref var items = ref entity.Ref<W.Multi<Item>>();

ushort len = items.Length;       // 元素数量
ushort cap = items.Capacity;     // 当前容量
bool empty = items.IsEmpty;      // 为空
bool notEmpty = items.IsNotEmpty; // 不为空
bool full = items.IsFull;        // 已填满

// 索引访问（返回 ref）
ref var first = ref items[0];
ref var last = ref items[items.Length - 1];

// 第一个和最后一个元素
ref var f = ref items.First();
ref var l = ref items.Last();

// Span 直接内存访问
Span<Item> span = items.AsSpan;
ReadOnlySpan<Item> roSpan = items.AsReadOnlySpan;

// 隐式转换为 Span
Span<Item> span = items;
ReadOnlySpan<Item> roSpan = items;
```

___

## 添加

```csharp
// 单个元素
items.Add(new Item { Id = 1, Weight = 0.5f });

// 多个（2 到 4 个）
items.Add(
    new Item { Id = 1, Weight = 0.5f },
    new Item { Id = 2, Weight = 1.0f }
);

items.Add(
    new Item { Id = 1, Weight = 0.5f },
    new Item { Id = 2, Weight = 1.0f },
    new Item { Id = 3, Weight = 1.5f },
    new Item { Id = 4, Weight = 2.0f }
);

// 从数组
Item[] array = { new Item { Id = 5 }, new Item { Id = 6 } };
items.Add(array);

// 从数组切片
items.Add(array, srcIdx: 0, len: 1);

// 在指定索引处插入（其余元素移位）
items.InsertAt(idx: 1, new Item { Id = 10 });
```

#### 容量管理：
```csharp
// 确保额外 N 个元素的空间
items.EnsureSize(10);

// 增加 Length N 个（必要时预扩展）
items.EnsureCount(5);

// 增加 Length N 个元素，不初始化数据（低级操作）
items.EnsureCountUninitialized(5);

// 设置最小容量
items.Resize(32);
```

___

## 删除

```csharp
// 按索引（保留顺序 — 移位元素）
items.RemoveAt(idx: 1);

// 按索引（swap-remove — 用最后一个替换，更快，不保留顺序）
items.RemoveAtSwap(idx: 1);

// 第一个元素
items.RemoveFirst();       // 保留顺序
items.RemoveFirstSwap();   // swap-remove

// 最后一个元素
items.RemoveLast();

// 按值（如果找到则返回 true）
bool removed = items.TryRemove(new Item { Id = 1 });

// 按值 swap-remove
bool removed = items.TryRemoveSwap(new Item { Id = 1 });

// 两个元素按值
items.TryRemove(new Item { Id = 1 }, new Item { Id = 2 });

// 清除所有元素
items.Clear();

// 重置计数而不清除数据（底层操作）
items.ResetCount();
```

___

## 搜索

```csharp
// 元素索引（未找到返回 -1）
int idx = items.IndexOf(new Item { Id = 1 });

// 检查存在性
bool exists = items.Contains(new Item { Id = 1 });

// 使用自定义比较器
bool exists = items.Contains(new Item { Id = 1 }, comparer);
```

___

## 迭代

```csharp
// foreach — 按引用的可变访问
foreach (ref var item in items) {
    item.Weight *= 2f;
}

// for — 按索引访问
for (int i = 0; i < items.Length; i++) {
    ref var item = ref items[i];
    item.Weight *= 2f;
}

// 通过 Span
foreach (ref var item in items.AsSpan) {
    item.Weight *= 2f;
}
```

___

## 复制和排序

```csharp
// 复制到数组
var array = new Item[items.Length];
items.CopyTo(array);

// 复制切片
items.CopyTo(array, dstIdx: 0, len: 5);

// 排序
items.Sort();

// 使用自定义比较器
items.Sort(comparer);
```

___

## 查询

多组件在查询中像普通组件一样使用：

```csharp
// 所有有物品栏的实体
W.Query().For(static (W.Entity entity, ref W.Multi<Item> items) => {
    for (int i = 0; i < items.Length; i++) {
        ref var item = ref items[i];
        // ...
    }
});

// 带过滤
foreach (var entity in W.Query<All<W.Multi<Item>>>().Entities()) {
    ref var items = ref entity.Ref<W.Multi<Item>>();
    // ...
}
```
