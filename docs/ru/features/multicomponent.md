---
title: Мульти-компонент
parent: Возможности
nav_order: 5
---

## MultiComponent
Мультикомпоненты — оптимизированные компоненты-списки, позволяющие хранить множество однотипных значений на одной сущности
- Все элементы всех мультикомпонентов одного типа для всех сущностей хранятся в едином хранилище — оптимальное потребление памяти
- Вместимость от 4 до 32768 значений в одном компоненте, автоматическое расширение
- Не требует создания массивов или списков внутри компонента — без аллокаций на куче
- Является реализацией [компонента](component.md), все базовые правила работы аналогичны
- На базе мультикомпонентов реализованы [отношения](relations.md) сущностей (`Links<T>`)

___

## Определение типа

Тип значения мультикомпонента должен реализовать интерфейс `IMultiComponent` и быть `struct`:

```csharp
// Unmanaged тип — сериализация работает автоматически через bulk memory copy
public struct Item : IMultiComponent {
    public int Id;
    public float Weight;
}
```

Не-unmanaged (managed) типы должны реализовать хуки `Write`/`Read` для сериализации:

```csharp
// Managed тип — требует Write/Read хуки для сериализации
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

## Стратегия сериализации

По умолчанию используется `StructPackArrayStrategy<T>` для поэлементной сериализации (через хуки).
Для unmanaged типов можно использовать `UnmanagedPackArrayStrategy<T>` для bulk memory copy (быстрее).

Стратегию можно указать тремя способами:

**1. Явный параметр при регистрации:**
```csharp
W.Types()
    .Multi<Item>(elementStrategy: new UnmanagedPackArrayStrategy<Item>());
```

**2. Статическое поле/свойство на типе (для авторегистрации через `RegisterAll`):**
```csharp
public struct Item : IMultiComponent {
    public int Id;
    public float Weight;

    static readonly IPackArrayStrategy<Item> PackStrategy = new UnmanagedPackArrayStrategy<Item>();
}
```

**3. По умолчанию:** `StructPackArrayStrategy<T>` — использует хуки `Write`/`Read` поэлементно.

___

## Блочная сериализация сегментов

Для снимков чанков/мира/кластеров, когда `TValue` является unmanaged, можно использовать `MultiUnmanagedPackArrayStrategy<TWorld, TValue>` для сериализации целых сегментов хранилища одним блоком памяти вместо поэлементных данных каждой сущности. Это заменяет множество мелких per-entity копирований одной bulk-операцией на сегмент и восстанавливает состояние аллокатора напрямую.

```csharp
W.Types()
    .Multi<Item>(new ComponentTypeConfig<W.Multi<Item>>(
        guid: new Guid("..."),
        readWriteStrategy: new MultiUnmanagedPackArrayStrategy<MyWorld, Item>()
    ));
```

{: .noteru }
Эта стратегия сериализует сырые байты структуры `Multi<T>` плюс сегменты хранилища значений и состояние аллокатора. Entity-level сериализация (`EntitiesSnapshot`) продолжает использовать per-entity хуки `Write`/`Read` — оптимизация применяется только к снимкам чанков/мира/кластеров.

{: .importantru }
`MultiUnmanagedPackArrayStrategy` требует чтобы `Multi<TValue>` удовлетворял ограничению `unmanaged`. Поскольку поля `Multi<T>` — все value-типы, это работает для конкретных типов `TValue`, но **нельзя использовать в generic-коде регистрации** — указывайте явно для каждого конкретного типа.

___

## Регистрация

```csharp
W.Create(WorldConfig.Default());

W.Types()
    .Multi<Item>()                                                         // стратегия по умолчанию (StructPackArrayStrategy)
    .Multi<Item>(elementStrategy: new UnmanagedPackArrayStrategy<Item>())   // явная unmanaged стратегия
    .Multi<NamedItem>();                                                    // managed тип с хуками

W.Initialize();
```

___

## Основные операции

Мультикомпонент работает как обычный компонент:

```csharp
// Добавить (начальная ёмкость — 4 элемента, расширяется автоматически)
ref var items = ref entity.Add<W.Multi<Item>>();

// Получить ссылку
ref var items = ref entity.Ref<W.Multi<Item>>();

// Проверить наличие
bool has = entity.Has<W.Multi<Item>>();

// Удалить (список элементов очищается автоматически)
entity.Delete<W.Multi<Item>>();

// При клонировании и копировании — все элементы копируются автоматически
var clone = entity.Clone();
entity.CopyTo<W.Multi<Item>>(targetEntity);
```

___

## Свойства

```csharp
ref var items = ref entity.Ref<W.Multi<Item>>();

ushort len = items.Length;       // Количество элементов
ushort cap = items.Capacity;     // Текущая ёмкость
bool empty = items.IsEmpty;      // Пусто
bool notEmpty = items.IsNotEmpty; // Не пусто
bool full = items.IsFull;        // Заполнено до ёмкости

// Доступ по индексу (возвращает ref)
ref var first = ref items[0];
ref var last = ref items[items.Length - 1];

// Первый и последний элемент
ref var f = ref items.First();
ref var l = ref items.Last();

// Span для прямого доступа к памяти
Span<Item> span = items.AsSpan;
ReadOnlySpan<Item> roSpan = items.AsReadOnlySpan;

// Неявное преобразование в Span
Span<Item> span = items;
ReadOnlySpan<Item> roSpan = items;
```

___

## Добавление

```csharp
// Один элемент
items.Add(new Item { Id = 1, Weight = 0.5f });

// Несколько (от 2 до 4)
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

// Из массива
Item[] array = { new Item { Id = 5 }, new Item { Id = 6 } };
items.Add(array);

// Из среза массива
items.Add(array, srcIdx: 0, len: 1);

// Вставка в указанный индекс (остальные элементы сдвигаются)
items.InsertAt(idx: 1, new Item { Id = 10 });
```

#### Управление ёмкостью:
```csharp
// Гарантировать место для N дополнительных элементов
items.EnsureSize(10);

// Увеличить Length на N (с предварительным расширением если нужно)
items.EnsureCount(5);

// Увеличить Length на N без инициализации данных (низкоуровневая операция)
items.EnsureCountUninitialized(5);

// Установить минимальную ёмкость
items.Resize(32);
```

___

## Удаление

```csharp
// По индексу (с сохранением порядка — сдвигает элементы)
items.RemoveAt(idx: 1);

// По индексу (swap-remove — заменяет последним, быстрее, порядок не сохраняется)
items.RemoveAtSwap(idx: 1);

// Первый элемент
items.RemoveFirst();       // с сохранением порядка
items.RemoveFirstSwap();   // swap-remove

// Последний элемент
items.RemoveLast();

// По значению (возвращает true если найден)
bool removed = items.TryRemove(new Item { Id = 1 });

// По значению со swap-remove
bool removed = items.TryRemoveSwap(new Item { Id = 1 });

// Два элемента по значению
items.TryRemove(new Item { Id = 1 }, new Item { Id = 2 });

// Очистить все элементы
items.Clear();

// Сбросить счётчик без очистки данных (низкоуровневая операция)
items.ResetCount();
```

___

## Поиск

```csharp
// Индекс элемента (-1 если не найден)
int idx = items.IndexOf(new Item { Id = 1 });

// Проверить наличие
bool exists = items.Contains(new Item { Id = 1 });

// С пользовательским компаратором
bool exists = items.Contains(new Item { Id = 1 }, comparer);
```

___

## Итерация

```csharp
// foreach — мутабельный доступ по ссылке
foreach (ref var item in items) {
    item.Weight *= 2f;
}

// for — доступ по индексу
for (int i = 0; i < items.Length; i++) {
    ref var item = ref items[i];
    item.Weight *= 2f;
}

// Через Span
foreach (ref var item in items.AsSpan) {
    item.Weight *= 2f;
}
```

___

## Копирование и сортировка

```csharp
// Копировать в массив
var array = new Item[items.Length];
items.CopyTo(array);

// Копировать срез
items.CopyTo(array, dstIdx: 0, len: 5);

// Сортировка
items.Sort();

// С пользовательским компаратором
items.Sort(comparer);
```

___

## Запросы

Мультикомпоненты используются в запросах как обычные компоненты:

```csharp
// Все сущности с инвентарём
W.Query().For(static (W.Entity entity, ref W.Multi<Item> items) => {
    for (int i = 0; i < items.Length; i++) {
        ref var item = ref items[i];
        // ...
    }
});

// С фильтрацией
foreach (var entity in W.Query<All<W.Multi<Item>>>().Entities()) {
    ref var items = ref entity.Ref<W.Multi<Item>>();
    // ...
}
```
