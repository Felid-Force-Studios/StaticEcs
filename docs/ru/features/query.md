---
title: Запросы
parent: Возможности
nav_order: 12
---

# Query
Запросы — механизм поиска сущностей и их компонентов в мире
- Все запросы не требуют кеширования, аллоцируются на стеке и могут использоваться «на лету»
- Поддерживают фильтрацию по компонентам, тегам, статусу сущности и кластерам
- Два режима итерации: `Strict` (по умолчанию, быстрее) и `Flexible` (допускает модификацию фильтруемых типов у других сущностей)

___

## Фильтры

Типы для описания фильтрации. Каждый занимает 1 байт и не требует инициализации.

```csharp
// Допустим в мире 5 сущностей:
//            Components                 Tags           EntityType
// Entity 1:  Position, Velocity         Unit           Npc
// Entity 2:  Position, Name             Player         Npc
// Entity 3:  Position, Velocity, Name   Unit, Player   Npc
// Entity 4:  Velocity                   —              Bullet
// Entity 5:  Position■, Velocity        Unit           Bullet
//            (■ = disabled)
//
// Примеры ниже показывают какие сущности пройдут каждый фильтр
```

### Компоненты:
```csharp
// All — наличие ВСЕХ включённых компонентов (от 1 до 8 типов)
All<Position, Velocity, Direction> all = default;

// AllOnlyDisabled — наличие ВСЕХ отключённых компонентов
AllOnlyDisabled<Position> disabled = default;

// AllWithDisabled — наличие ВСЕХ компонентов (любое состояние)
AllWithDisabled<Position, Velocity> any = default;

// None — отсутствие включённых компонентов (от 1 до 8 типов)
None<Position, Name> none = default;

// NoneWithDisabled — отсутствие компонентов (любое состояние)
NoneWithDisabled<Position> noneAll = default;

// Any — наличие хотя бы одного включённого компонента (от 2 до 8 типов)
Any<Position, Velocity> any = default;

// AnyOnlyDisabled — хотя бы один отключённый
AnyOnlyDisabled<Position, Velocity> anyDis = default;

// AnyWithDisabled — хотя бы один (любое состояние)
AnyWithDisabled<Position, Velocity> anyAll = default;

// Результаты для сущностей выше:
// All<Position, Velocity>              → 1, 3
// AllOnlyDisabled<Position>            → 5
// AllWithDisabled<Position, Velocity>  → 1, 3, 5
// None<Name>                           → 1, 4, 5
// NoneWithDisabled<Position>           → 4
// Any<Position, Name>                  → 1, 2, 3
// AnyOnlyDisabled<Position, Velocity>  → 5
// AnyWithDisabled<Position, Name>      → 1, 2, 3, 5
```

### Теги:

Теги используют те же фильтры, что и компоненты — `All<>`, `None<>`, `Any<>` и их варианты. Отдельных типов фильтров для тегов нет.

```csharp
// All — наличие ВСЕХ указанных тегов (от 1 до 8 типов)
All<Unit, Player> tagAll = default;

// None — отсутствие указанных тегов (от 1 до 8 типов)
None<Unit, Player> tagNone = default;

// Any — хотя бы один из указанных тегов (от 2 до 8 типов)
Any<Unit, Player> tagAny = default;

// Результаты для сущностей выше:
// All<Unit, Player>  → 3
// None<Unit>         → 2, 4
// Any<Unit, Player>  → 1, 2, 3, 5
```

### Отслеживание изменений:
```csharp
// AllAdded — ВСЕ указанные компоненты были добавлены с последнего ClearTracking (от 1 до 5 типов)
AllAdded<Position> added = default;
AllAdded<Position, Velocity> addedMulti = default;

// AnyAdded — ХОТЯ БЫ ОДИН из указанных компонентов был добавлен (от 2 до 5 типов)
AnyAdded<Position, Velocity> anyAdded = default;

// NoneAdded — НИ ОДИН из указанных компонентов не был добавлен (от 1 до 5 типов)
NoneAdded<Position> noneAdded = default;

// AllDeleted — ВСЕ указанные компоненты были удалены с последнего ClearTracking (от 1 до 5 типов)
AllDeleted<Position> deleted = default;

// AnyDeleted — ХОТЯ БЫ ОДИН был удалён (от 2 до 5 типов)
AnyDeleted<Position, Velocity> anyDeleted = default;

// NoneDeleted — НИ ОДИН не был удалён (от 1 до 5 типов)
NoneDeleted<Position> noneDeleted = default;

// AllChanged — ВСЕ указанные компоненты были изменены с последнего ClearChangedTracking (от 1 до 5 типов)
// Требует ComponentTypeConfig.TrackChanged = true
AllChanged<Position> changed = default;

// AnyChanged — ХОТЯ БЫ ОДИН был изменён (от 2 до 5 типов)
AnyChanged<Position, Velocity> anyChanged = default;

// NoneChanged — НИ ОДИН не был изменён (от 1 до 5 типов)
NoneChanged<Position> noneChanged = default;

// AllAdded / AnyAdded / NoneAdded / AllDeleted / AnyDeleted / NoneDeleted
// также работают с тегами — используйте одни и те же фильтры для компонентов и тегов

// Created — сущность была создана с момента последнего ClearCreatedTracking
// (требует WorldConfig.TrackCreated = true, без параметров типа)
Created created = default;

// Комбинация с другими фильтрами
foreach (var entity in W.Query<AllAdded<Position>, All<Velocity, Unit>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
    // обработка новых сущностей с Position
}
```

{: .noteru }
В делегатной итерации (`For`) параметры `ref` помечают компонент как Changed, а параметры `in` — нет. Используйте `in` для read-only доступа, чтобы избежать лишних Changed-пометок. Подробнее см. [Changed Tracking](tracking#changed-tracking).

### Типы сущностей:
```csharp
// EntityIs — точно этот тип сущности (1 параметр)
EntityIs<Bullet> entityIs = default;

// EntityIsNot — исключить типы сущностей (от 1 до 5 типов)
EntityIsNot<Effect> entityIsNot = default;

// EntityIsAny — любой из указанных типов сущностей (от 2 до 5 типов)
EntityIsAny<Bullet, Rocket> entityIsAny = default;

// Результаты для сущностей выше:
// EntityIs<Npc>              → 1, 2, 3
// EntityIsNot<Bullet>        → 1, 2, 3
// EntityIsAny<Npc, Bullet>   → 1, 2, 3, 4, 5
```

### And / Or — составные фильтры:

`And` и `Or` позволяют группировать несколько фильтров в один тип. Это полезно для:
- **Передачи сложного фильтра одним дженерик-параметром** — хранить в поле, передавать в метод, использовать как аргумент типа
- **Построения фильтров, которые невозможно выразить базовыми типами** — например, «сущности с набором компонентов A **или** набором компонентов B»

#### And — все условия должны совпасть (от 2 до 6 фильтров):
```csharp
And<All<Position, Velocity>, None<Name>, Any<Unit, Player>> filter = default;

// Через фабричный метод (вывод типов)
var filter = And.By(
    default(All<Position, Velocity>),
    default(None<Name>),
    default(Any<Unit, Player>)
);

// Пример: передача составного фильтра в вспомогательный метод
void ProcessMovable(And<All<Position, Velocity>, None<Frozen>> filter) {
    foreach (var entity in W.Query(filter).Entities()) {
        entity.Ref<Position>().Value += entity.Read<Velocity>().Value;
    }
}
```

#### Or — хотя бы одно условие должно совпасть (от 2 до 6 фильтров):

`Or` позволяет строить комбинационно сложные фильтры, которые невозможно выразить базовыми типами.

```csharp
// Бойцы ближнего боя ИЛИ дальнего боя — совершенно разные наборы компонентов,
// невозможно выразить одной комбинацией All/Any/None
Or<All<MeleeWeapon, Damage>, All<RangedWeapon, Ammo>> fighters = default;

// Перестроить пространственный индекс при добавлении, удалении или изменении Position
Or<AllAdded<Position>, AllDeleted<Position>, AllChanged<Position>> spatialChanged = default;

// Обработать UI-кнопки (ClickArea + Label) и мировые интерактивные объекты (Collider + Interaction)
Or<All<ClickArea, Label>, All<Collider, Interaction>> clickable = default;

// Через фабричный метод
var filter = Or.By(
    default(All<MeleeWeapon, Damage>),
    default(All<RangedWeapon, Ammo>)
);

// Результаты для сущностей выше:
// Or<All<Position, Velocity>, All<Position, Name>>
// Entity 1: Pos✓ Vel✓         → ✓ (проходит первый)
// Entity 2: Pos✓ Name✓        → ✓ (проходит второй)
// Entity 3: Pos✓ Vel✓ Name✓   → ✓ (проходит оба)
// Entity 4: Pos✗              → ✗
// → Результат: 1, 2, 3, 5
```

#### Вложенность:
```csharp
// And и Or можно вкладывать для произвольно сложной логики
// (A и B и C) или (A и B и D):
Or<All<A, B, C>, All<A, B, D>> complex = default;

// Все видимые сущности, которые либо живые юниты, либо активные эффекты:
And<All<Visible>, Or<All<Unit, Alive>, All<Effect, Active>>> visibleAlive = default;
```

___

## Итерация по сущностям

```csharp
// Итерация по всем сущностям без фильтрации
foreach (var entity in W.Query().Entities()) {
    Console.WriteLine(entity.PrettyString);
}

// С фильтром через generic (от 1 до 8 фильтров)
foreach (var entity in W.Query<All<Position, Velocity>>().Entities()) {
    entity.Ref<Position>().Value += entity.Read<Velocity>().Value;
}

// С несколькими фильтрами
foreach (var entity in W.Query<All<Position, Velocity>, None<Name>>().Entities()) {
    entity.Ref<Position>().Value += entity.Read<Velocity>().Value;
}

// Через значение фильтра
var all = default(All<Position, Velocity>);
foreach (var entity in W.Query(all).Entities()) {
    entity.Ref<Position>().Value += entity.Read<Velocity>().Value;
}

// Через And/Or — группировка фильтров в один тип для передачи в метод или хранения в поле
var filter = default(And<All<Position, Velocity>, None<Name>>);
foreach (var entity in W.Query(filter).Entities()) {
    entity.Ref<Position>().Value += entity.Read<Velocity>().Value;
}

// Flexible режим — позволяет модифицировать фильтруемые типы у других сущностей
foreach (var entity in W.Query<All<Position>>().EntitiesFlexible()) {
    // ...
}

// Найти первую подходящую сущность
if (W.Query<All<Position>>().Any(out var found)) {
    // found — первая сущность с Position
}

// Получить единственную сущность (ошибка в debug если найдено больше одной)
if (W.Query<All<Position>>().One(out var single)) {
    // single — единственная сущность с Position
}

// Подсчёт количества сущностей (полный обход)
int count = W.Query<All<Position>>().EntitiesCount();
```

___

## Делегатный поиск (For)

Оптимизированная итерация через делегаты — «под капотом» разворачивает циклы.

```csharp
// По всем сущностям
W.Query().For(entity => {
    Console.WriteLine(entity.PrettyString);
});

// По компонентам (от 1 до 6 типов)
// Компоненты в делегате автоматически выступают как фильтр All
W.Query().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});

// С сущностью в делегате
W.Query().For(static (W.Entity entity, ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});

// С пользовательскими данными (для избежания аллокаций делегата)
W.Query().For(deltaTime, static (ref float dt, ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value * dt;
});

// С ref данными (для аккумуляции результата)
int count = 0;
W.Query().For(ref count, static (ref int counter, W.Entity entity, ref Position pos) => {
    counter++;
});

// С кортежем нескольких параметров
W.Query().For((deltaTime, gravity), static (ref (float dt, float g) data, ref Position pos, ref Velocity vel) => {
    vel.Value += data.g * data.dt;
    pos.Value += vel.Value * data.dt;
});
```

### Readonly-компоненты (Read):

Когда компонент только читается и не модифицируется, используйте `in` вместо `ref` в делегатах. Это указывает системе отслеживания изменений не помечать компонент как изменённый.

```csharp
// Последние N компонентов как readonly через `in`
W.Query().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;  // Position — записываемый (ref), Velocity — только чтение (in)
});

// Все компоненты readonly
W.Query().For(static (in Position pos, in Velocity vel) => {
    Console.WriteLine(pos.Value + vel.Value);
});

// С сущностью
W.Query().For(static (W.Entity entity, ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});

// С пользовательскими данными
W.Query().For(ref result, static (ref float res, in Position pos, in Velocity vel) => {
    res += pos.Value.Length;
});
```

{: .noteru }
Read-варианты доступны при включённом отслеживании изменений (по умолчанию). Можно отключить через дефайн `FFS_ECS_DISABLE_CHANGED_TRACKING`.

### С дополнительной фильтрацией:
```csharp
// Компоненты в делегате расцениваются как фильтр All,
// дополнительные фильтры задаются прямо в Query и не требуют указания компонентов из делегата
W.Query<Any<Unit, Player>>().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});

// С несколькими фильтрами
W.Query<None<Name>, Any<Unit, Player>>().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});

// Через значение
var filter = default(Any<Unit, Player>);
W.Query(filter).For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});
```

### Статус сущности и компонентов:
```csharp
W.Query().For(
    static (ref Position pos, ref Velocity vel) => {
        // ...
    },
    entities: EntityStatusType.Disabled,    // Enabled (по умолчанию), Disabled, Any
    components: ComponentStatus.Disabled    // Enabled (по умолчанию), Disabled, Any
);
```

___

## Поиск одной сущности (Search)

Итерация с ранним выходом при первом совпадении условия. Все компоненты в делегатах поиска — readonly (`in`).

```csharp
if (W.Query().Search(out W.Entity found,
    (W.Entity entity, in Position pos, in Health health) => {
        return pos.Value.x > 100 && health.Current < 50;
    })) {
    // found — первая сущность удовлетворяющая условию
}
```

___

## Структуры-функции (IQuery / IQueryBlock)

Структуры-функции вместо делегатов — для оптимизации, передачи состояния или вынесения логики.
Структуры-функции используют **fluent builder API** на `WorldQuery` — в отличие от делегатов, типы компонентов указываются не через дженерик-параметры `For`, а через цепочку билдера.

### IQuery — поэлементный вызов:

Иерархия интерфейсов использует вложенные типы для контроля доступа на запись/чтение (от 1 до 6 компонентов суммарно):
- `IQuery.Write<T0, T1>` — все компоненты записываемые (`ref`)
- `IQuery.Read<T0, T1>` — все компоненты только для чтения (`in`)
- `IQuery.Write<T0>.Read<T1>` — первые записываемые, остальные только для чтения

```csharp
// Все записываемые — IQuery.Write
readonly struct MoveFunction : W.IQuery.Write<Position, Velocity> {
    public void Invoke(W.Entity entity, ref Position pos, ref Velocity vel) {
        pos.Value += vel.Value;
    }
}

// Fluent API: Write<...>() указывает записываемые компоненты, затем For<TFunction>() выполняет
W.Query().Write<Position, Velocity>().For<MoveFunction>();

// Через значение
W.Query().Write<Position, Velocity>().For(new MoveFunction());

// Через ref (для сохранения состояния после итерации)
var func = new MoveFunction();
W.Query().Write<Position, Velocity>().For(ref func);

// Смешанный запись/чтение — IQuery.Write<>.Read<>
readonly struct ApplyVelocity : W.IQuery.Write<Position>.Read<Velocity> {
    public void Invoke(W.Entity entity, ref Position pos, in Velocity vel) {
        pos.Value += vel.Value;
    }
}

// Цепочка: Write<записываемые>().Read<только чтение>().For<TFunction>()
W.Query().Write<Position>().Read<Velocity>().For<ApplyVelocity>();

// Все readonly — IQuery.Read
readonly struct PrintPositions : W.IQuery.Read<Position, Velocity> {
    public void Invoke(W.Entity entity, in Position pos, in Velocity vel) {
        Console.WriteLine(pos.Value + vel.Value);
    }
}

W.Query().Read<Position, Velocity>().For<PrintPositions>();

// С дополнительной фильтрацией
W.Query<None<Name>, Any<Unit, Player>>()
    .Write<Position, Velocity>().For<MoveFunction>();

// Комбинация системы и IQuery
public struct MoveSystem : ISystem, W.IQuery.Write<Position>.Read<Velocity> {
    private float _speed;

    public void Update() {
        _speed = W.GetResource<GameConfig>().Speed;
        W.Query<All<Unit>>()
            .Write<Position>().Read<Velocity>().For(ref this);
    }

    public void Invoke(W.Entity entity, ref Position pos, in Velocity vel) {
        pos.Value += vel.Value * _speed;
    }
}
```

### Методы WorldQuery

#### Делегаты — типы компонентов выводятся из лямбды:

| Метод | Компоненты |
|-------|------------|
| `For(delegate)` | 1–6, `ref` или `in` на компонент |
| `ForParallel(delegate)` | 1–6, `ref` или `in` на компонент |
| `Search(out entity, delegate)` | 1–6, все `in` |

#### Структуры-функции — доступ к компонентам через билдер:

| Метод | Компоненты | Доступ |
|-------|------------|--------|
| `Write<1‑6>()` | 1–6 | все `ref` |
| `Write<1‑5>().Read<1‑5>()` | 2–6 суммарно | первые `ref`, остальные `in` |
| `Read<1‑6>()` | 1–6 | все `in` |

#### Блочные структуры-функции — аналогично, только `unmanaged`:

| Метод | Компоненты | Доступ |
|-------|------------|--------|
| `WriteBlock<1‑6>()` | 1–6 | все `Block<T>` |
| `WriteBlock<1‑5>().Read<1‑5>()` | 2–6 суммарно | `Block<T>` + `BlockR<T>` |
| `ReadBlock<1‑6>()` | 1–6 | все `BlockR<T>` |

Каждый билдер предоставляет `For<F>()` и `ForParallel<F>()`.
`Read` / `ReadBlock` требуют отслеживания изменений (включено по умолчанию, отключается через `FFS_ECS_DISABLE_CHANGED_TRACKING`).

___

## Параллельная обработка

{: .warningru }
Параллельная обработка требует включения при создании мира: `ParallelQueryType.MaxThreadsCount` или `ParallelQueryType.CustomThreadsCount` с указанием `CustomThreadCount` в `WorldConfig`.
Внутри параллельной итерации разрешена модификация и уничтожение только **текущей** итерируемой сущности. Запрещено: создание сущностей, модификация других сущностей, чтение событий. Отправка событий (`SendEvent`) потокобезопасна (при отсутствии одновременного чтения того же типа, подробнее см. [События](events#многопоточность)). Всегда используется `QueryMode.Strict`.

```csharp
// Делегат — первый параметр, minEntitiesPerThread — именованный (по умолчанию 256)
W.Query().ForParallel(
    static (W.Entity entity, ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value;
    },
    minEntitiesPerThread: 50000
);

// Без сущности — только компоненты
W.Query().ForParallel(
    static (ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value;
    },
    minEntitiesPerThread: 50000
);

// С пользовательскими данными
W.Query().ForParallel(deltaTime,
    static (ref float dt, ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value * dt;
    },
    minEntitiesPerThread: 50000
);

// С фильтрацией
W.Query<None<Name>, Any<Unit, Player>>().ForParallel(
    static (W.Entity entity) => {
        entity.Add<Name>();
    },
    minEntitiesPerThread: 50000
);

// Через структуру-функцию
W.Query().Write<Position>().Read<Velocity>().ForParallel<ApplyVelocity>(
    minEntitiesPerThread: 50000
);

// workersLimit — ограничение числа потоков (0 = все доступные)
W.Query().ForParallel(
    static (ref Position pos) => { /* ... */ },
    minEntitiesPerThread: 10000,
    workersLimit: 4
);
```

___

## Блочная итерация (ForBlock)

Низкоуровневая итерация через структуры-функции — для `unmanaged` компонентов предоставляет обёртки `Block<T>` (записываемые) и `BlockR<T>` (только для чтения) с прямыми указателями на массивы данных.

Иерархия интерфейсов аналогична `IQuery` (от 1 до 6 unmanaged компонентов суммарно):
- `IQueryBlock.Write<T0, T1>` — все компоненты записываемые (`Block<T>`)
- `IQueryBlock.Read<T0, T1>` — все компоненты только для чтения (`BlockR<T>`)
- `IQueryBlock.Write<T0>.Read<T1>` — первые записываемые, остальные только для чтения

```csharp
// Все записываемые — IQueryBlock.Write
readonly struct MoveBlock : W.IQueryBlock.Write<Position, Velocity> {
    public void Invoke(uint count, EntityBlock entitiesBlock,
                       Block<Position> positions, Block<Velocity> velocities) {
        for (uint i = 0; i < count; i++) {
            positions[i].Value += velocities[i].Value;
        }
    }
}

// Fluent API: WriteBlock<...>().For<TFunction>()
W.Query().WriteBlock<Position, Velocity>().For<MoveBlock>();

// Смешанный запись/чтение — WriteBlock<>.Read<>
readonly struct ApplyVelocityBlock : W.IQueryBlock.Write<Position>.Read<Velocity> {
    public void Invoke(uint count, EntityBlock entitiesBlock,
                       Block<Position> positions, BlockR<Velocity> velocities) {
        for (uint i = 0; i < count; i++) {
            positions[i].Value += velocities[i].Value;
        }
    }
}

W.Query().WriteBlock<Position>().Read<Velocity>().For<ApplyVelocityBlock>();

// Все readonly — ReadBlock<>
readonly struct SumPositionsBlock : W.IQueryBlock.Read<Position> {
    public void Invoke(uint count, EntityBlock entitiesBlock, BlockR<Position> positions) {
        for (uint i = 0; i < count; i++) {
            // доступ только на чтение
        }
    }
}

W.Query().ReadBlock<Position>().For<SumPositionsBlock>();

// Через ref (для сохранения состояния)
var func = new MoveBlock();
W.Query().WriteBlock<Position, Velocity>().For(ref func);

// Параллельная версия
W.Query().WriteBlock<Position, Velocity>().ForParallel<MoveBlock>(minEntitiesPerThread: 50000);
```

___

## Пакетные операции

Массовые операции над всеми сущностями, подходящими под фильтр — без написания цикла.
Могут быть в десятки раз быстрее ручной итерации через `For`: вместо поштучной обработки каждой сущности, пакетные операции работают с битовыми масками — в лучшем случае добавление или удаление компонента/тега для 64 сущностей выполняется одной битовой операцией.
Поддерживают цепочки вызовов — несколько операций можно выполнить за один проход.

```csharp
// Добавить компонент всем сущностям (от 1 до 5 типов)
W.Query<All<Position>>().BatchSet(new Velocity { Value = 1f });

// Удалить компонент у всех
W.Query<All<Position, Velocity>>().BatchDelete<Velocity>();

// Отключить/включить компонент у всех
W.Query<All<Position>>().BatchDisable<Position>();
W.Query<AllOnlyDisabled<Position>>().BatchEnable<Position>();

// Теги: установить, удалить, переключить, применить по условию (от 1 до 5 типов)
W.Query<All<Position>>().BatchSet<Unit>();
W.Query<All<Unit>>().BatchDelete<Unit>();
W.Query<All<Position>>().BatchToggle<Unit>();
W.Query<All<Position>>().BatchApply<Unit>(true);

// Цепочки
W.Query<All<Position>>()
    .BatchSet(new Velocity { Value = 1f })
    .BatchSet<Unit>()
    .BatchDisable<Position>();
```

___

## Удаление и выгрузка сущностей

```csharp
// Уничтожить все сущности подходящие под фильтр
W.Query<All<Position>>().BatchDestroy();

// С параметрами
W.Query<All<Unit>>().BatchDestroy(
    entities: EntityStatusType.Any,
    mode: QueryMode.Flexible
);

// Выгрузить все сущности подходящие под фильтр
// (помечает как выгруженные, удаляет компоненты/теги, но сохраняет ID и версии сущностей)
W.Query<All<Position>>().BatchUnload();

// С параметрами
W.Query<All<Unit>>().BatchUnload(
    entities: EntityStatusType.Any,
    mode: QueryMode.Flexible
);
```

___

## Кластеры

{: .importantru }
Для каждого метода (`Entities`, `For`, `ForParallel`, `Search`, `Batch*`, `BatchDestroy`, `BatchUnload`) можно указать конкретные кластеры:

```csharp
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { 2, 5, 12 };

foreach (var entity in W.Query<All<Position>>().Entities(clusters: clusters)) {
    // итерация только по сущностям из кластеров 2, 5, 12
}

W.Query().For(static (W.Entity entity, ref Position pos) => {
    // ...
}, clusters: clusters);
```

___

## QueryMode

Для методов `For`, `Search`, `Entities`:

- **`QueryMode.Strict`** (по умолчанию) — запрещает модификацию фильтруемых типов компонентов/тегов у **других** сущностей во время итерации. Работает быстрее.
- **`QueryMode.Flexible`** — позволяет модификацию фильтруемых типов у других сущностей, корректно контролирует текущую итерацию.

```csharp
var anotherEntity = W.NewEntity<Default>();
anotherEntity.Add<Position>();

// Strict: модификация Position у другой сущности — ошибка в DEBUG
foreach (var entity in W.Query<All<Position>>().Entities()) {
    anotherEntity.Delete<Position>(); // ОШИБКА в DEBUG
}

// Flexible: модификация допустима, итерация стабильна
foreach (var entity in W.Query<All<Position>>().EntitiesFlexible()) {
    anotherEntity.Delete<Position>(); // OK — anotherEntity корректно исключена
}

// Для For/Search через параметр
W.Query().For(static (ref Position pos) => {
    // ...
}, queryMode: QueryMode.Flexible);
```

{: .noteru }
`Flexible` полезен для иерархий или кэшей, когда из компонентов одних сущностей модифицируются другие. В остальных случаях предпочтителен `Strict` по соображениям производительности.
