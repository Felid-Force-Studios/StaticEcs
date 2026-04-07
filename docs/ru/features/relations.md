---
title: Отношения
parent: Возможности
nav_order: 9
---

## Отношения
Отношения — механизм связи сущностей друг с другом через типизированные компоненты-ссылки
- `Link<T>` — связь с одной сущностью (обёртка над `EntityGID`)
- `Links<T>` — связь с несколькими сущностями (динамическая коллекция `Link<T>`)
- Связи являются обычными компонентами и работают через стандартный API (`Add`, `Ref`, `Delete`, `Has`)
- Поддерживают хуки (`OnAdd`, `OnDelete`, `CopyTo`) для автоматизации логики (например, обратных ссылок)

___

## Типы связей

Для определения типа связи реализуйте один из интерфейсов:

```csharp
// ILinkType — тип для одиночной связи (Link<T>)
// Реализуйте только те хуки, которые нужны
public struct Parent : ILinkType {
    // Вызывается при добавлении связи
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        // self — сущность, к которой добавлена связь
        // link — GID целевой сущности
    }

    // Вызывается при удалении связи
    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        // ...
    }

    // Вызывается при копировании сущности (Clone/CopyTo)
    public void CopyTo<TW>(World<TW>.Entity self, World<TW>.Entity other, EntityGID link) where TW : struct, IWorldType {
        // ...
    }
}

// ILinksType — тип для множественной связи (Links<T>)
// Наследуется от ILinkType, те же хуки
public struct Children : ILinksType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType {
        // ...
    }
}

// Тип без хуков — просто не реализуйте методы
public struct FollowTarget : ILinkType { }
```

{: .importantru }
Не оставляйте пустые реализации хуков. Если хук не нужен — не реализуйте его. Нереализованные хуки не вызываются и не создают накладных расходов.

___

## Link\<T\>

Компонент одиночной связи — обёртка над `EntityGID` (8 байт).

```csharp
// Свойства
EntityGID value = link.Value;    // GID целевой сущности (только чтение)

// Неявные преобразования
W.Link<Parent> link = entity;              // Entity → Link<T>
W.Link<Parent> link = entity.GID;          // EntityGID → Link<T>
W.Link<Parent> link = entity.GIDCompact;   // EntityGIDCompact → Link<T>
EntityGID gid = link;                      // Link<T> → EntityGID

// Создание через конструктор
var link = new W.Link<Parent>(targetGID);

// Создание через entity.AsLink
W.Link<Parent> link = entity.AsLink<Parent>();
```

___

## Links\<T\>

Мультикомпонент — динамическая коллекция `Link<T>` с автоматическим управлением памятью.

#### Свойства:
```csharp
ref var links = ref entity.Ref<W.Links<Children>>();

ushort len = links.Length;       // Количество элементов
ushort cap = links.Capacity;     // Текущая ёмкость
bool empty = links.IsEmpty;      // Пусто
bool notEmpty = links.IsNotEmpty; // Не пусто
bool full = links.IsFull;        // Заполнено до ёмкости

// Доступ по индексу
W.Link<Children> first = links[0];
W.Link<Children> last = links[links.Length - 1];

// Первый и последний элемент
W.Link<Children> f = links.First();
W.Link<Children> l = links.Last();

// Span для чтения
ReadOnlySpan<W.Link<Children>> span = links.AsReadOnlySpan;

// Итерация
foreach (var link in links) {
    if (link.Value.TryUnpack<WT>(out var child)) {
        // ...
    }
}
```

#### Добавление:
```csharp
// TryAdd — не добавляет если уже существует, возвращает false
bool added = links.TryAdd(childLink);

// TryAdd нескольких (от 2 до 4)
links.TryAdd(child1, child2);
links.TryAdd(child1, child2, child3, child4);

// Add — добавляет, в DEBUG бросает ошибку при дубликате
links.Add(childLink);
links.Add(child1, child2);

// Add из массива
links.Add(childArray);
links.Add(childArray, srcIdx: 0, len: 3);
```

#### Удаление:
```csharp
// По значению (возвращает true если найден)
bool removed = links.TryRemove(childLink);

// По значению со swap-remove (не сохраняет порядок, быстрее)
bool removed = links.TryRemoveSwap(childLink);

// По индексу
links.RemoveAt(0);
links.RemoveAtSwap(0);

// Первый / последний
links.RemoveFirst();
links.RemoveFirstSwap();
links.RemoveLast();

// Удалить все (вызывает OnDelete для каждого элемента)
links.Clear();
```

#### Поиск:
```csharp
bool exists = links.Contains(childLink);
int idx = links.IndexOf(childLink);
```

#### Управление памятью:
```csharp
links.EnsureSize(10);        // Гарантировать место для 10 дополнительных элементов
links.Resize(32);            // Изменить ёмкость
links.Sort();                // Сортировка
```

___

## Регистрация

Связи регистрируются как обычные компоненты на этапе создания мира:

```csharp
W.Create(WorldConfig.Default());

W.Types()
    .Link<Parent>()
    .Links<Children>();

W.Initialize();
```

___

## Работа со связями

Связи — обычные компоненты. Все стандартные методы работают:

```csharp
var parent = W.NewEntity<Default>();
var child1 = W.NewEntity<Default>();
var child2 = W.NewEntity<Default>();

// Добавить одиночную связь
child1.Set(new W.Link<Parent>(parent));
child2.Set(new W.Link<Parent>(parent));

// Получить ссылку
ref var parentLink = ref child1.Ref<W.Link<Parent>>();
EntityGID parentGID = parentLink.Value;

// Проверить наличие
bool hasParent = child1.Has<W.Link<Parent>>();

// Удалить связь
child1.Delete<W.Link<Parent>>();

// Добавить множественную связь
ref var children = ref parent.Add<W.Links<Children>>();
children.TryAdd(child1.AsLink<Children>());
children.TryAdd(child2.AsLink<Children>());

// Прочитать множественную связь
ref var kids = ref parent.Ref<W.Links<Children>>();
for (int i = 0; i < kids.Length; i++) {
    if (kids[i].Value.TryUnpack<WT>(out var childEntity)) {
        // работа с дочерней сущностью
    }
}
```

___

## Extension-методы

Безопасные операции со связями через `EntityGID` — автоматически проверяют загруженность и актуальность целевой сущности.

### Link (одиночная связь):
```csharp
// Добавить связь Link<T> к целевой сущности
LinkOppStatus status = targetGID.TryAddLink<WT, Parent>(linkEntity);

// Удалить связь Link<T> у целевой сущности
LinkOppStatus status = targetGID.TryDeleteLink<WT, Parent>(linkEntity);

// Глубокое уничтожение — рекурсивно уничтожает цепочку связанных сущностей
targetGID.DeepDestroyLink<WT, Parent>();

// Глубокое копирование — клонирует целевую сущность и возвращает ссылку на копию
LinkOppStatus status = sourceGID.TryDeepCopyLink<WT, Parent>(out W.Link<Parent> copied);
```

### Links (множественная связь):
```csharp
// Добавить элемент в Links<T> целевой сущности
// Автоматически создаёт компонент Links<T> если его ещё нет
LinkOppStatus status = targetGID.TryAddLinkItem<WT, Children>(linkEntity);

// Удалить элемент из Links<T> целевой сущности
// Автоматически удаляет компонент Links<T> если коллекция стала пустой
LinkOppStatus status = targetGID.TryDeleteLinkItem<WT, Children>(linkEntity);

// Глубокое уничтожение — рекурсивно уничтожает все связанные сущности
targetGID.DeepDestroyLinkItem<WT, Children>();
```

### LinkOppStatus:
```csharp
// Результат операции
switch (status) {
    case LinkOppStatus.Ok:                // Операция выполнена успешно
    case LinkOppStatus.LinkAlreadyExists: // Связь уже существует (TryAdd)
    case LinkOppStatus.LinkNotExists:     // Связь не найдена (TryDelete)
    case LinkOppStatus.LinkNotLoaded:     // Целевая сущность в выгруженном чанке
    case LinkOppStatus.LinkNotActual:     // GID устарел (сущность уничтожена, слот переиспользован)
}
```

___

## Примеры связей

### Однонаправленная связь (без хуков)

Простейший случай — сущность ссылается на другую без обратной связи.

```csharp
// Тип без хуков
public struct FollowTarget : ILinkType { }

// Регистрация
W.Types().Link<FollowTarget>();
```

```csharp
//  A FollowTarget→ B

var unit = W.NewEntity<Default>();
var target = W.NewEntity<Default>();

// Установить цель для преследования
unit.Set(new W.Link<FollowTarget>(target));

// В системе движения
W.Query().For(static (W.Entity entity, ref W.Link<FollowTarget> follow) => {
    if (follow.Value.TryUnpack<WT>(out var targetEntity)) {
        ref var myPos = ref entity.Ref<Position>();
        ref readonly var targetPos = ref targetEntity.Read<Position>();
        // двигаться к цели
    }
});
```

___

### Двунаправленная связь один к одному (One-To-One)

Замкнутая пара — обе сущности ссылаются друг на друга одним типом.

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

W.Types().Link<MarriedTo>();
```

```csharp
var alice = W.NewEntity<Default>();
var bob = W.NewEntity<Default>();

// Достаточно установить с одной стороны — обратная создастся автоматически
alice.Set(new W.Link<MarriedTo>(bob));
// Теперь: alice имеет Link<MarriedTo> → bob
//         bob имеет Link<MarriedTo> → alice

// Удаление тоже двустороннее
alice.Delete<W.Link<MarriedTo>>();
// Теперь: оба компонента удалены
```

___

### Двунаправленная связь один к одному (разные типы)

Две сущности связаны разными типами связей.

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
// player имеет Link<Mount> → horse
// horse имеет Link<Rider> → player
```

___

### Двунаправленная связь один ко многим (One-To-Many)

Родитель и дети — классическая иерархия.

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

// Установить связь с любой стороны
son.Set(new W.Link<Parent>(father));
daughter.Set(new W.Link<Parent>(father));
// father автоматически получит Links<Children> → [son, daughter]

// Или добавить со стороны родителя
ref var kids = ref father.Ref<W.Links<Children>>();
var newChild = W.NewEntity<Default>();
kids.TryAdd(newChild.AsLink<Children>());
// newChild автоматически получит Link<Parent> → father
```

{: .noteru }
`withCyclicHooks: false` (значение по умолчанию) в extension-методах `TryAddLink`/`TryDeleteLink`/`TryAddLinkItem`/`TryDeleteLinkItem` — это оптимизация: при вызове из хука не нужно вызывать хук у противоположной стороны, так как он уже выполняется.

___

### Однонаправленная связь ко многим (To-Many)

Сущность ссылается на несколько других без обратной связи.

```csharp
//      Targets→ B
//     /
//  A── Targets→ C
//     \
//      Targets→ D

public struct Targets : ILinksType { }

W.Types().Links<Targets>();
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

### Двунаправленная связь многие ко многим (Many-To-Many)

Обе стороны хранят коллекции ссылок друг на друга.

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

// Добавить user1 в обе группы
ref var memberships = ref user1.Add<W.Links<Memberships>>();
memberships.TryAdd(groupA.AsLink<Memberships>());
memberships.TryAdd(groupB.AsLink<Memberships>());
// groupA и groupB автоматически получат Links<Owners> → [user1]

// Добавить user2 в groupA
ref var memberships2 = ref user2.Add<W.Links<Memberships>>();
memberships2.TryAdd(groupA.AsLink<Memberships>());
// groupA теперь имеет Links<Owners> → [user1, user2]
```

___

## Блочная сериализация сегментов для Links

Для снимков чанков/мира/кластеров с unmanaged типами связей используйте `LinksUnmanagedPackArrayStrategy<TWorld, TLinkType>` для блочной сериализации сегментов хранилища связей:

```csharp
W.Types()
    .Links<MyLinkType>(new ComponentTypeConfig<W.Links<MyLinkType>>(
        guid: new Guid("..."),
        readWriteStrategy: new LinksUnmanagedPackArrayStrategy<MyWorld, MyLinkType>()
    ));
```

Работает идентично `MultiUnmanagedPackArrayStrategy` — подробности см. в [блочная сериализация мульти-компонентов](multicomponent.md#блочная-сериализация-сегментов).

___

## Многопоточность

{: .warningru }
В `ForParallel` разрешено модифицировать только **текущую** итерируемую сущность. Хуки связей, которые изменяют состояние **других** сущностей (например, добавляют обратную ссылку родителю), вызовут ошибку в DEBUG при параллельной итерации.

Для работы со связями в параллельных запросах используйте **события** — `SendEvent` потокобезопасен (при отсутствии одновременного чтения того же типа, подробнее см. [События](events#многопоточность)) и может вызываться из любого потока. Логику обработки событий выполняйте в основном потоке после завершения параллельной итерации.

#### Пример: отложенное удаление связи через события

```csharp
// 1. Определяем событие
public struct DeleteLinkEvent<TLink> : IEvent where TLink : unmanaged, ILinkType {
    public EntityGID Target;    // сущность, у которой нужно удалить связь
    public EntityGID Link;      // значение связи для проверки
}

// 2. Регистрируем событие и создаём ресивер
W.Types().Event<DeleteLinkEvent<Parent>>();
var deleteLinkReceiver = W.RegisterEventReceiver<DeleteLinkEvent<Parent>>();

// Сохраняем ресивер в ресурсах мира для доступа из системы
W.SetResource(deleteLinkReceiver);
```

```csharp
// 3. Определяем тип связи БЕЗ хуков изменяющих другие сущности
public struct Parent : ILinkType {
    // В OnDelete вместо прямого изменения родителя — отправляем событие
    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType {
        World<TW>.SendEvent(new DeleteLinkEvent<Parent> {
            Target = link,
            Link = self.GID
        });
    }
}
```

```csharp
// 4. Параллельная итерация — безопасна, хук отправляет событие вместо прямой модификации
W.Query().ForParallel(
    static (W.Entity entity, ref W.Link<Parent> parent) => {
        if (someCondition) {
            entity.Delete<W.Link<Parent>>();
            // OnDelete отправит DeleteLinkEvent вместо модификации родителя
        }
    },
    minEntitiesPerThread: 1000
);

// 5. В основном потоке обрабатываем все события
ref var receiver = ref W.GetResource<EventReceiver<WT, DeleteLinkEvent<Parent>>>();
receiver.ReadAll(static (W.Event<DeleteLinkEvent<Parent>> e) => {
    // Теперь безопасно модифицировать другие сущности
    ref var data = ref e.Value;
    data.Target.TryDeleteLinkItem<WT, Children>(data.Link.Unpack<WT>());
});
```

___

## Запросы

Компоненты связей используются в запросах как любые другие компоненты:

```csharp
// Все сущности с родителем
foreach (var entity in W.Query<All<W.Link<Parent>>>().Entities()) {
    ref var parentLink = ref entity.Ref<W.Link<Parent>>();
    // ...
}

// Все сущности с детьми, но без родителя (корневые)
W.Query<All<W.Links<Children>>, None<W.Link<Parent>>>()
    .For(static (W.Entity entity, ref W.Links<Children> kids) => {
        // корневые сущности
    });

// Через делегат
W.Query().For(static (ref W.Link<Parent> parent) => {
    if (parent.Value.TryUnpack<WT>(out var parentEntity)) {
        // ...
    }
});
```
