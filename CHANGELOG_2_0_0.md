# StaticEcs 2.0.0 — Что нового

**StaticEcs 2.0.0** — это не инкрементальное обновление, а полная реструктуризация фреймворка. Переработана модель хранения данных, унифицированы API, добавлены принципиально новые возможности — от системы отслеживания изменений до блочной итерации с прямыми указателями. Если 1.x был быстрым и удобным ECS, то 2.0 — это зрелый фреймворк, готовый к масштабным проектам: сетевые игры, стриминг открытых миров, реактивные UI и симуляции с миллионами сущностей.

Ниже — обзор ключевых изменений, начиная с самых крупных нововведений и заканчивая деталями API.

---

## Крупные нововведения

### Сегментная модель хранения

В 1.x мир делился на чанки (4096 сущностей) и блоки (64 сущности). В 2.0 появился промежуточный уровень — **сегмент** (256 сущностей). Новая иерархия:

```
Chunk (4096) → Segment (256) → Block (64) → Entity
```

Сегменты стали единицей распределения памяти для компонентов. Это позволило реализовать двумерное партиционирование **EntityType × Cluster** — сущности одного типа внутри кластера размещаются в смежных сегментах, что радикально улучшает кэш-локальность при итерации.

---

### Типы сущностей (IEntityType)

Одно из самых значимых архитектурных нововведений. В 1.x все сущности были одинаковы — различались лишь набором компонентов. В 2.0 каждая сущность при создании получает **тип** — логическую категорию, определяющую её назначение и размещение в памяти.

```csharp
public struct Bullet : IEntityType {
    public static readonly byte Id = 1;
}

public struct Enemy : IEntityType {
    public static readonly byte Id = 2;
}

// Создание — тип указывается явно
var bullet = W.NewEntity<Bullet>();
var enemy = W.NewEntity<Enemy>();
```

Типы сущностей обеспечивают:

- **Кэш-локальность** — сущности одного типа хранятся в соседних сегментах памяти. Когда запрос итерирует юнитов, он проходит по плотно упакованным данным без перескакивания через снаряды и эффекты.
- **Фильтрация в запросах** — новые фильтры `EntityIs<T>`, `EntityIsNot<T>`, `EntityIsAny<T>` позволяют ограничить запрос определённым типом:
  ```csharp
  foreach (var entity in W.Query<All<Position>, EntityIs<Bullet>>().Entities()) { ... }
  ```
- **Хуки жизненного цикла** — `OnCreate` и `OnDestroy` определяются прямо в структуре типа:
  ```csharp
  public struct Bullet : IEntityType {
      public static readonly byte Id = 1;

      public void OnCreate<TWorld>(World<TWorld>.Entity entity)
          where TWorld : struct, IWorldType {
          entity.Set(new Velocity { Speed = 100 });
          entity.SetTag<Active>();
      }
  }
  ```
- **Параметризованное создание** — поскольку `IEntityType` это struct, тип может содержать данные, передаваемые при создании:
  ```csharp
  public struct Flora : IEntityType {
      public static readonly byte Id = 4;
      public enum Kind : byte { Grass, Bush, Tree }
      public Kind FloraKind;

      public void OnCreate<TWorld>(World<TWorld>.Entity entity)
          where TWorld : struct, IWorldType {
          entity.Set(new Health { Value = FloraKind == Kind.Tree ? 100 : 10 });
      }
  }

  var tree = W.NewEntity(new Flora { FloraKind = Flora.Kind.Tree });
  ```

Встроенный тип `Default` (Id = 0) регистрируется автоматически и используется как тип по умолчанию.

---

### Система отслеживания изменений (Change Tracking)

Полностью новая подсистема, отсутствовавшая в 1.x. Позволяет отслеживать добавление, удаление и изменение компонентов и тегов без ручного ведения dirty-флагов.

Четыре типа отслеживания:

| Тип | Что отслеживает | Область |
|-----|----------------|---------|
| **Added** | Добавление компонента/тега | Компоненты, теги |
| **Deleted** | Удаление компонента/тега | Компоненты, теги |
| **Changed** | Доступ к данным через `Mut<T>()` | Только компоненты |
| **Created** | Создание сущности | Весь мир |

Трекинг включается при регистрации типа:

```csharp
W.Types().Component<Health>(new ComponentTypeConfig<Health>(
    trackAdded: true,
    trackDeleted: true,
    trackChanged: true
));
```

Трекинг версионируется по тикам мира через кольцевой буфер:

```csharp
// Размер буфера по умолчанию 8 — настраивается через WorldConfig
W.Create(new WorldConfig { TrackingBufferSize = 16 });
```

И используется через новые фильтры запросов:

```csharp
// Сущности, которым добавили Position в предыдущем кадре
foreach (var entity in W.Query<All<Position>, AllAdded<Position>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
}

// Обработать только изменённые позиции (для сетевой синхронизации)
foreach (var entity in W.Query<All<Position>, AllChanged<Position>>().Entities()) {
    ref readonly var pos = ref entity.Read<Position>();
    SendPositionUpdate(entity, pos);
}

// Созданные в предыдущем кадре сущности
foreach (var entity in W.Query<Created, EntityIs<Bullet>, All<Position>>().Entities()) {
    // инициализация визуалов
}
```

**Архитектура отслеживания:**
- Bitmap-хранение: один `ulong` на 64 сущности — тот же формат, что маски компонентов
- Tick-based кольцевой буфер: `W.Tick()` продвигает тик мира, каждая система в `W.Systems<T>.Update()` автоматически видит изменения с момента своего последнего запуска
- Changed-трекинг: `Mut<T>()` помечает Changed, `Ref<T>()` — нет (быстрый доступ без трекинга)
- В делегатных запросах `ref` помечает Changed, `in` — нет
- Фильтры и методы `HasAdded/HasDeleted/HasChanged` принимают опциональный параметр `fromTick` для указания диапазона тиков
- `ClearTracking()` очищает все слоты буфера — обычно не нужен, трекинг управляется автоматически
- Нулевые накладные расходы для типов с выключенным трекингом
- Директива `FFS_ECS_DISABLE_CHANGED_TRACKING` убирает все пути кода Changed на этапе компиляции

**Игровой цикл:**
```csharp
W.Systems<Update>.Update();    // каждая система видит изменения с её LastTick
W.Tick();                      // один тик за кадр
```

Полный набор из **16 фильтров** трекинга: `AllAdded`, `AnyAdded`, `NoneAdded`, `AllDeleted`, `AnyDeleted`, `NoneDeleted`, `AllChanged`, `AnyChanged`, `NoneChanged`, `TagAllAdded`, `TagAnyAdded`, `TagNoneAdded`, `TagAllDeleted`, `TagAnyDeleted`, `TagNoneDeleted`, `Created`.

---

### Блочная итерация (ForBlock)

Новый самый быстрый способ итерации для `unmanaged` компонентов. Вместо получения `ref` на каждый компонент каждой сущности, ForBlock предоставляет обёртки `Block<T>` и `BlockR<T>` — прямые указатели на массивы данных блока до 64 сущностей за раз.

```csharp
readonly struct MoveBlock : W.IQueryBlock.Write<Position>.Read<Velocity> {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(uint count, EntityBlock entities,
                       Block<Position> positions, BlockR<Velocity> velocities) {
        for (uint i = 0; i < count; i++) {
            positions[i].Value += velocities[i].Value;
        }
    }
}

W.Query().WriteBlock<Position>().Read<Velocity>().For<MoveBlock>();
```

Поддерживает параллельную обработку:
```csharp
W.Query().WriteBlock<Position, Velocity>().ForParallel<MoveBlock>(minEntitiesPerThread: 50000);
```

Это самый производительный способ обработки данных — минимальная индирекция, прямые указатели, оптимальное использование SIMD-инструкций.

---

### Fluent Builder API для запросов

Структуры-функции (`IQuery`/`IQueryBlock`) получили мощный fluent builder API с явным разделением записываемых и readonly компонентов:

```csharp
// Записываемые + readonly через цепочку
W.Query().Write<Position>().Read<Velocity>().For<ApplyVelocity>();

// Все записываемые
W.Query().Write<Position, Velocity>().For<MoveFunction>();

// Все readonly
W.Query().Read<Position, Velocity>().For<PrintPositions>();

// Блочные версии
W.Query().WriteBlock<Position>().Read<Velocity>().For<MoveBlock>();
```

Типы интерфейсов вложены для контроля доступа на уровне типов:
- `IQuery.Write<T0>.Read<T1>` — первые записываемые (`ref`), остальные только для чтения (`in`)
- `IQueryBlock.Write<T0>.Read<T1>` — первые как `Block<T>`, остальные как `BlockR<T>`

Система ISystem теперь может одновременно реализовывать IQuery:
```csharp
public struct MoveSystem : ISystem, W.IQuery.Write<Position>.Read<Velocity> {
    private float _speed;

    public void Update() {
        _speed = W.GetResource<GameConfig>().Speed;
        W.Query<TagAll<Unit>>().Write<Position>().Read<Velocity>().For(ref this);
    }

    public void Invoke(W.Entity entity, ref Position pos, in Velocity vel) {
        pos.Value += vel.Value * _speed;
    }
}
```

---

### Or-фильтры в запросах

В 1.x фильтры поддерживали только And-комбинации: все условия должны были совпадать. В 2.0 добавлен `Or<>`, позволяющий строить запросы, которые ранее были невозможны:

```csharp
// Бойцы ближнего ИЛИ дальнего боя — разные наборы компонентов
Or<All<MeleeWeapon, Damage>, All<RangedWeapon, Ammo>> fighters = default;

// Перестроить индекс при любом изменении позиции
Or<AllAdded<Position>, AllDeleted<Position>, AllChanged<Position>> spatialChanged = default;

// Вложенность — произвольно сложная логика
And<All<Visible>, Or<TagAll<Unit, Alive>, TagAll<Effect, Active>>> visibleAlive = default;
```

---

## Унификация и упрощение API

### Единый ISystem вместо разрозненных интерфейсов

В 1.x система могла реализовывать комбинации `IInitSystem`, `IUpdateSystem`, `IDestroySystem`, `ISystemCondition`, `ISystemState`. В 2.0 всё объединено в один интерфейс `ISystem` с четырьмя опциональными методами:

```csharp
// Было (v1.2.x):
struct MoveSystem : IUpdateSystem {
    public void Update() { }
}
struct InitSystem : IInitSystem, IDestroySystem {
    public void Init() { }
    public void Destroy() { }
}
Systems.AddUpdate(new MoveSystem());
Systems.AddCallOnce(new InitSystem());

// Стало (v2.0.0):
struct MoveSystem : ISystem {
    public void Update() { }
}
struct InitSystem : ISystem {
    public void Init() { }
    public void Destroy() { }
}
GameSys.Add(new MoveSystem(), order: 0);
GameSys.Add(new InitSystem(), order: -10);
```

Нереализованные методы обнаруживаются через рефлексию и не вызываются — нулевой оверхед. Метод `UpdateIsActive()` заменяет `ISystemCondition`:

```csharp
public struct PausableSystem : ISystem {
    public void Update() { /* ... */ }
    public bool UpdateIsActive() => !W.GetResource<GameState>().IsPaused;
}
```

Регистрация систем теперь с fluent API и явным порядком:
```csharp
GameSys.Add(new InputSystem(), order: -10)
    .Add(new MoveSystem(), order: 0)
    .Add(new RenderSystem(), order: 10);
```

---

### Единый Query вместо раздвоенного API

В 1.x итерация по сущностям и компонентам разделялась на `Query.Entities<>()` и `Query.For()`:

```csharp
// Было:
foreach (var entity in W.Query.Entities<All<Position>>()) { }
W.Query.For((ref Position pos) => { pos.X += 1; });

// Стало:
foreach (var entity in W.Query<All<Position>>().Entities()) { }
W.Query().For(static (ref Position pos) => { pos.X += 1; });
```

Единая точка входа `W.Query<>()` — чище, проще, без дублирования.

---

### Хуки в IComponent через Default Interface Methods

В 1.x каждый компонент с хуками требовал отдельного Config-класса — громоздкую пару структур:

```csharp
// Было (v1.2.x):
struct Position : IComponent { public float X, Y; }

class PositionConfig : IComponentConfig<Position, WT> {
    public OnComponentHandler<Position> OnAdd() => (ref Position c, Entity e) => { };
    public OnComponentHandler<Position> OnDelete() => ...;
    public Guid Id() => ...;
    public BinaryWriter<Position> Writer() => ...;
    public BinaryReader<Position> Reader() => ...;
    // ...ещё 5+ методов
}
W.RegisterComponentType<Position>(new PositionConfig());
```

В 2.0 хуки объявляются прямо в структуре компонента:

```csharp
// Стало (v2.0.0):
struct Position : IComponent {
    public float X, Y;

    public void OnAdd<TWorld>(World<TWorld>.Entity self)
        where TWorld : struct, IWorldType { }

    public void OnDelete<TWorld>(World<TWorld>.Entity self, HookReason reason)
        where TWorld : struct, IWorldType { }

    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType {
        writer.WriteFloat(X); writer.WriteFloat(Y);
    }

    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self,
                             byte version, bool disabled)
        where TWorld : struct, IWorldType {
        X = reader.ReadFloat(); Y = reader.ReadFloat();
    }
}

W.Types().Component<Position>(new ComponentTypeConfig<Position>(
    guid: new Guid("...")
));
```

Конфигурация компонентов значительно упрощена — `ComponentTypeConfig<T>` содержит только метаданные (guid, version, strategy, defaultValue, tracking-флаги), вся поведенческая логика — в самом компоненте.

Бонус — конфиг можно объявить статическим полем прямо в структуре, и `RegisterAll()` подхватит его автоматически:
```csharp
public struct Health : IComponent {
    public float Value;
    public static readonly ComponentTypeConfig<Health> Config = new(
        defaultValue: new Health { Value = 100f }
    );
}
```

**Удалены:** `IComponentConfig<T,W>`, `DefaultComponentConfig<T,W>`, `ValueComponentConfig<T,W>`, `OnComponentHandler<T>`, `OnCopyHandler<T>`, хук `OnPut`.

---

### Context переименован в Resources

Механизм хранения глобальных данных получил более понятное имя и расширенный API:

```csharp
// Было:
W.Context.Set<GameTime>(new GameTime());
ref var time = ref W.Context.Get<GameTime>();

// Стало:
W.SetResource(new GameTime());
ref var time = ref W.GetResource<GameTime>();
bool has = W.HasResource<GameTime>();
W.RemoveResource<GameTime>();
```

Новинка — **именованные ресурсы** для хранения нескольких экземпляров одного типа:
```csharp
W.SetResource("player_config", new GameConfig { Gravity = 9.81f });
W.SetResource("moon_config", new GameConfig { Gravity = 1.62f });
ref var cfg = ref W.GetResource<GameConfig>("moon_config");
```

`NamedResource<T>` — структура-хэндл с кэшированием:
```csharp
var moonConfig = new W.NamedResource<GameConfig>("moon_config");
ref var cfg = ref moonConfig.Value; // первый вызов ищет в словаре, последующие — O(1)
```

---

### Переработка системы связей (Relations)

Система отношений между сущностями стала проще и единообразнее:

```csharp
// Было (v1.2.x):
struct Parent : IEntityLinkComponent<Parent> {
    public EntityGID Link;
    ref EntityGID IRefProvider<Parent, EntityGID>.RefValue(ref Parent c) => ref c.Link;
}
W.RegisterToOneRelationType<Parent>(config);
entity.SetLink<Parent>(targetGID);

// Стало (v2.0.0):
struct ParentLink : ILinkType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType { }
    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType { }
}
W.Types().Link<ParentLink>();       // связь "один"
W.Types().Links<ParentLink>();      // связь "много"

entity.Set(new W.Link<ParentLink>(parentEntity));
ref var links = ref entity.Ref<W.Links<ChildLink>>();
```

Типы-обёртки `Link<T>` и `Links<T>` — стандартные компоненты. Связи работают как обычные компоненты в запросах, с хуками `OnAdd`/`OnDelete` прямо на `ILinkType`.

---

### Унификация тегов и компонентов

Теги и компоненты объединены в единую систему хранения. Теги хранятся в `Components<T>` с флагом `IsTag` — без отдельной инфраструктуры. Это упрощает API и сокращает кодовую базу на ~7500 строк.

**Что изменилось:**
- Фильтры `TagAll<>`, `TagNone<>`, `TagAny<>` удалены — используйте `All<>`, `None<>`, `Any<>` для тегов
- Методы сущностей: `SetTag` → `Set`, `HasTag` → `Has`, `DeleteTag` → `Delete`, `ToggleTag` → `Toggle`, `ApplyTag` → `Apply`
- Копирование/перемещение: `CopyTagsTo` → `CopyTo`, `MoveTagsTo` → `MoveTo`, `CopyComponentsTo` → `CopyTo`, `MoveComponentsTo` → `MoveTo`
- Tracking фильтры: `TagAllAdded` → `AllAdded`, `TagAnyAdded` → `AnyAdded` и т.д. — одни фильтры для компонентов и тегов
- Проверки: `HasAddedTag` → `HasAdded`, `HasDeletedTag` → `HasDeleted`
- Сброс: `ClearTagTracking` → `ClearTracking`, `ClearAllTagsTracking` → `ClearAllTracking`
- Batch: `BatchSetTag` → `BatchSet`, `BatchDeleteTag` → `BatchDelete`, `BatchToggleTag` → `BatchToggle`, `BatchApplyTag` → `BatchApply`
- `TagsHandle` удалён → `ComponentsHandle` (с полем `IsTag`)
- `WorldConfig.BaseTagTypesCount` удалён — теги учитываются в `BaseComponentTypesCount`

---

## Полноценные Batch-операции

В 1.x методы `AddForAll`, `DeleteForAll`, `SetTagForAll`, `DestroyAllEntities` существовали скорее как синтаксический сахар — внутри они работали per entity, обрабатывая каждую сущность по отдельности. В 2.0 это **принципиально другой механизм**: batch-операции работают на уровне блоков сущностей, напрямую с битовыми масками. В лучшем случае одна побитовая операция изменяет сразу 64 сущности — это на порядки быстрее поштучной обработки и самый быстрый способ массово модифицировать сущности в мире.

Набор операций вырос до полного спектра:

| Метод | Описание |
|-------|----------|
| `BatchAdd<T>()` | Добавить компоненты (default, 1-5 типов) |
| `BatchSet<T>(value)` | Добавить компоненты с значениями (1-5 типов) |
| `BatchDelete<T>()` | Удалить компоненты (1-5 типов) |
| `BatchEnable<T>()` | Включить компоненты (1-5 типов) |
| `BatchDisable<T>()` | Отключить компоненты (1-5 типов) |
| `BatchSetTag<T>()` | Установить теги (1-5 типов) |
| `BatchDeleteTag<T>()` | Удалить теги (1-5 типов) |
| `BatchToggleTag<T>()` | **Новое:** Переключить теги (1-5 типов) |
| `BatchApplyTag<T>(bool)` | **Новое:** Условная установка/снятие (1-5 типов) |
| `BatchDestroy()` | Уничтожить сущности |
| `BatchUnload()` | **Новое:** Выгрузить сущности |
| `EntitiesCount()` | Подсчитать количество |

Поддержка цепочек:
```csharp
W.Query<All<Position>>()
    .BatchSet(new Velocity { Value = Vector3.One })
    .BatchSetTag<IsMovable>()
    .BatchDisable<Position>();
```

`UnloadCluster()`/`UnloadChunk()` удалены — заменены гибким `BatchUnload()` с фильтрацией:
```csharp
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { clusterId };
W.Query().BatchUnload(EntityStatusType.Any, clusters: clusters);
```

---

## Изменения семантики и API

### Add/Set — новая чёткая семантика

В 1.x существовало три метода с неочевидными различиями: `Add` (assert), `TryAdd` (идемпотентный), `Put` (перезапись). В 2.0 остались два с чётко разделённой семантикой:

| Метод | Поведение |
|-------|-----------|
| `Add<T>()` | **Идемпотентный** (бывший `TryAdd`). Если компонент есть — возвращает ref, хуки НЕ вызываются. Если нет — default-инициализация + `OnAdd`. |
| `Set(value)` | **Всегда перезаписывает** (бывший `Put`, но теперь с хуками). Если компонент есть — `OnDelete(старый)` → замена → `OnAdd(новый)`. Если нет — установка → `OnAdd`. |

```csharp
entity.Set(new Position { Value = Vector3.Zero }); // устанавливает
entity.Add<Position>();                             // ничего не делает — возвращает ref
entity.Set(new Position { Value = Vector3.One });   // OnDelete(old) → замена → OnAdd(new)
```

### Delete/Disable/Enable — предсказуемые возвраты

```csharp
// Было:
entity.Delete<C>();              // void, assert если нет
bool ok = entity.TryDelete<C>(); // bool

// Стало:
bool deleted = entity.Delete<C>();           // bool (бывший TryDelete)
ToggleResult = entity.Disable<C>();          // MissingComponent, Unchanged, Changed
ToggleResult = entity.Enable<C>();           // MissingComponent, Unchanged, Changed
```

### Методы → Свойства

Все методы-геттеры без параметров стали свойствами:

```csharp
// Entity:
entity.GID           // было: entity.Gid()
entity.IsDestroyed   // было: entity.IsDestroyed()
entity.IsDisabled    // было: entity.IsDisabled()
entity.Version       // было: entity.Version()
entity.ClusterId     // было: entity.ClusterId()
entity.ChunkID       // было: entity.Chunk()
entity.EntityType    // НОВОЕ: byte — ID типа сущности

// World:
W.IsWorldInitialized // было: W.IsInitialized()
W.IsIndependent      // было: W.IsIndependent()
W.Status             // НОВОЕ: WorldStatus enum
```

### Упрощение проверок наличия

```csharp
// Компоненты:
entity.Has<C>()          // было: entity.HasAllOf<C>()
entity.Has<C1, C2>()     // было: entity.HasAllOf<C1, C2>()
entity.HasAny<C1, C2>()  // было: entity.HasAnyOf<C1, C2>()
entity.HasDisabled<C>()  // было: entity.HasDisabledAllOf<C>()

// Теги:
entity.HasTag<T>()          // было: entity.HasAllOfTags<T>()
entity.HasAnyTags<T1, T2>() // было: entity.HasAnyOfTags<T1, T2>()
```

### Доступ к пулам

```csharp
Components<T>.Instance  // было: Components<T>.Value
Tags<T>.Instance        // было: Tags<T>.Value
```

---

## Регистрация типов

### Единый регистратор W.Types()

В 1.x каждый тип регистрировался своим методом:
```csharp
W.RegisterComponentType<Position>(new PositionConfig());
W.RegisterToOneRelationType<Parent>(config);
W.RegisterMultiComponentType<Items, int>(4);
```

В 2.0 всё через fluent `W.Types()`:
```csharp
W.Types()
    .Component<Position>()
    .Component<Health>(new ComponentTypeConfig<Health>(defaultValue: new Health { Value = 100 }))
    .Tag<Unit>()
    .Tag<Poisoned>(new TagTypeConfig<Poisoned>(trackAdded: true))
    .Event<DamageEvent>(new EventTypeConfig<DamageEvent>(guid: new Guid("...")))
    .Link<ParentLink>()
    .Links<ChildrenLinks>()
    .Multi<Item>()
    .EntityType<Bullet>(Bullet.Id)
    .EntityType<Enemy>(Enemy.Id);
```

`RegisterAll()` по-прежнему доступен для автоматического обнаружения всех типов в сборке.

---

## Создание сущностей

API создания перенесён из `Entity.New()` на уровень мира и получил типизацию:

```csharp
// Было:
var entity = W.Entity.New();
var entity = W.Entity.New<Position>(new Position());
W.Entity.NewOnes(count, onCreate);
bool ok = W.Entity.TryNew(out entity);

// Стало:
var entity = W.NewEntity<Default>();
var entity = W.NewEntity<Bullet>(clusterId: 5);
W.NewEntities<Default>(count: 100, onCreate: null);
bool ok = W.TryNewEntity<Default>(out entity);
var entity = W.NewEntity<Default>().Set(
    new Position { Value = Vector3.One },
    new Velocity { Value = 1f }
);
```

---

## Обновлённая производительность и документация

### Новая страница производительности

Документация производительности полностью переписана. Теперь она включает:

1. **Архитектурный разбор** — детальное сравнение с архетипными ECS (Unity DOTS, Flecs, Bevy) и sparse-set ECS (EnTT, DefaultEcs) по каждому аспекту
2. **Иерархия способов итерации** — от самого быстрого к самому удобному:
   - `ForBlock` — указатели на блоки (самый быстрый для unmanaged)
   - `For` с IQuery-структурой (без аллокаций, с состоянием)
   - `For` с делегатом (без аллокаций со static лямбдами)
   - `foreach` (наиболее гибкий)
3. **Стриппинг/trimming** — рекомендации по уменьшению размера сборки
4. **Двумерная партиция** — объяснение EntityType × Cluster для кэш-локальности

### Новая страница "Частые ошибки" (pitfalls.md)

Полностью новый раздел документации с систематизацией типичных ошибок:

- **Ошибки жизненного цикла** — забытая регистрация типов, операции до Initialize
- **Ошибки Entity** — использование после Destroy, хранение между кадрами
- **Ошибки компонентов** — семантика Add vs Set, пустые хуки, HasOnDelete vs DataLifecycle
- **Ошибки запросов** — нарушение Strict режима, ненужный Flexible
- **Ошибки регистрации** — MultiComponent без обёртки, отсутствие сериализации
- **Ошибки ресурсов** — кэширование NamedResource в readonly-полях

### Руководство для AI-агентов (aiagentguide.md)

Полностью новый раздел — сниппет для CLAUDE.md и других AI-ассистентов с:
- Паттернами настройки мира и систем
- Строгим порядком жизненного цикла
- Критическими правилами работы с Entity
- Типовыми паттернами кода
- Ссылками на llms.txt

---

## События (Events)

Система событий перенесена с уровня `W.Events` на уровень мира и получила хуки в IEvent:

```csharp
// Было:
W.Events.Send(new DamageEvent { Amount = 10 });
var receiver = W.Events.RegisterEventReceiver<DamageEvent>();
W.Events.DeleteEventReceiver(ref receiver);

// Стало:
W.SendEvent(new DamageEvent { Amount = 10 });
var receiver = W.RegisterEventReceiver<DamageEvent>();
W.DeleteEventReceiver(ref receiver);
```

`IEventConfig<T,W>` удалён — конфигурация через `EventTypeConfig<T>`, хуки Write/Read прямо в IEvent:
```csharp
struct DamageEvent : IEvent {
    public int Amount;
    public void Write(ref BinaryPackWriter writer) { writer.WriteInt(Amount); }
    public void Read(ref BinaryPackReader reader, byte version) { Amount = reader.ReadInt(); }
}
```

---

## Мульти-компоненты

Интерфейс упрощён — `IMultiComponent<T, V>` стал маркерным `IMultiComponent`:

```csharp
// Было:
struct Items : IMultiComponent<Items, int> {
    public Multi<int> Values;
    public ref Multi<int> RefValue(ref Items c) => ref c.Values;
}
W.RegisterMultiComponentType<Items, int>(4);

// Стало:
struct Items : IMultiComponent {
    public Multi<int> Values;
}
W.Types().Multi<Item>();
```

Изменения API: `Count` → `Length`, методы `IsEmpty()`/`IsFull()` → свойства.

---

## Сериализация

Хуки Write/Read перенесены из Config-классов прямо в IComponent/IEvent. Это главное архитектурное изменение — вся поведенческая логика компонента теперь в одном месте.

```csharp
// Было — отдельный Config-класс:
class PositionConfig : DefaultComponentConfig<Position, WT> {
    public override BinaryWriter<Position> Writer() => ...;
    public override BinaryReader<Position> Reader() => ...;
}

// Стало — хуки в структуре:
struct Position : IComponent {
    public float X, Y;
    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType { writer.WriteFloat(X); writer.WriteFloat(Y); }
    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self,
                             byte version, bool disabled)
        where TWorld : struct, IWorldType { X = reader.ReadFloat(); Y = reader.ReadFloat(); }
}
```

Снапшоты событий перенесены:
```csharp
// Было:
W.Events.CreateSnapshot();
W.Events.LoadSnapshot(snapshot);

// Стало:
W.Serializer.CreateEventsSnapshot();
W.Serializer.LoadEventsSnapshot(snapshot);
```

---

## Readonly доступ к компонентам

В 1.x все компоненты в запросах были `ref`. В 2.0 появилось чёткое разделение:

```csharp
// Делегаты — ref (запись) vs in (чтение)
W.Query().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;  // Position записываемый, Velocity только чтение
});

// Вне запросов
ref var pos = ref entity.Ref<Position>();           // мутабельный, НЕ помечает Changed (быстрый путь)
ref var tracked = ref entity.Mut<Position>();       // мутабельный, помечает Changed
ref readonly var vel = ref entity.Read<Velocity>(); // readonly, НЕ помечает Changed
```

Это интегрировано с системой отслеживания изменений — `Ref`/`Read` не вызывают ложных срабатываний Changed, `Mut` явно помечает компонент как изменённый.

---

## WorldConfig — новые параметры

```csharp
new WorldConfig {
    // Существующие:
    BaseComponentTypesCount = 64,
    BaseTagTypesCount = 64,
    ParallelQueryType = ParallelQueryType.Disabled,
    Independent = true,

    // Новые в 2.0:
    WorkerSpinCount = 256,        // spin-wait итераций перед блокировкой потока
    BaseClustersCapacity = 16,    // начальная ёмкость массива кластеров
    TrackCreated = false,         // глобальное отслеживание создания сущностей
}

// Фабричные методы:
WorldConfig.Default()     // стандартные настройки
WorldConfig.MaxThreads()  // все доступные потоки
```

---

## Новая директива компилятора

`FFS_ECS_DISABLE_CHANGED_TRACKING` — убирает на этапе компиляции все пути кода Changed-трекинга, включая фильтры `AllChanged`, `NoneChanged`, `AnyChanged` и метод `Mut<T>()`.

---

## Удалённые API

| Удалено | Замена |
|---------|--------|
| `IWorld` интерфейс | `WorldHandle` |
| `WorldWrapper<W>` | `WorldHandle` |
| `Worlds` статический класс | — |
| `BoxedEntity<W>` / `IEntity` / `entity.Box()` | — |
| `entity.TryAdd<C>()` | `entity.Add<C>()` |
| `entity.Put<C>(val)` | `entity.Set<C>(val)` |
| `entity.TryDelete<C>()` | `entity.Delete<C>()` |
| Все `Raw`-методы Entity | — |
| `Entity.New(...)` | `W.NewEntity<TEntityType>(...)` |
| `IInitSystem` / `IUpdateSystem` / `IDestroySystem` | `ISystem` |
| `ISystemCondition` / `ISystemState` | `ISystem.UpdateIsActive()` |
| `Systems.AddUpdate()` / `AddCallOnce()` | `Sys.Add(system, order)` |
| `IComponentConfig<T,W>` | `ComponentTypeConfig<T>` + хуки в IComponent |
| `IEventConfig<T,W>` | `EventTypeConfig<T>` + хуки в IEvent |
| `IEntityLinkComponent<T>` | `ILinkType` + `Link<T>` |
| `IEntityLinksComponent<T>` | `ILinksType` + `Links<T>` |
| `IMultiComponent<T,V>` | `IMultiComponent` (маркер) |
| `DeleteTagsSystem<W, T>` | `Query.BatchDeleteTag<T>()` |
| `OnComponentHandler<T>` / `OnCopyHandler<T>` | Хуки в IComponent |
| `W.UnloadCluster()` / `W.UnloadChunk()` | `Query().BatchUnload()` |
| `W.Events.XXX` | `W.XXX` |
| `W.Context.Set/Get/Has` | `W.SetResource/GetResource/HasResource` |

---

## Краткая таблица переименований

| Было (v1.2.x) | Стало (v2.0.0) |
|---|---|
| `W.Entity.New(...)` | `W.NewEntity<TEntityType>(...)` |
| `W.Entity.NewOnes(...)` | `W.NewEntities<TEntityType>(count)` |
| `W.IsInitialized()` | `W.IsWorldInitialized` |
| `entity.Gid()` | `entity.GID` |
| `entity.HasAllOf<C>()` | `entity.Has<C>()` |
| `entity.HasAnyOf<C1,C2>()` | `entity.HasAny<C1,C2>()` |
| `entity.HasAllOfTags<T>()` | `entity.HasTag<T>()` |
| `Components<T>.Value` | `Components<T>.Instance` |
| `Tags<T>.Value` | `Tags<T>.Instance` |
| `W.Query.Entities<F>()` | `W.Query<F>()` |
| `W.Query.For(...)` | `W.Query().For(...)` |
| `AddForAll<C>()` | `BatchAdd<C>()` |
| `DeleteForAll<C>()` | `BatchDelete<C>()` |
| `SetTagForAll<T>()` | `BatchSetTag<T>()` |
| `DestroyAllEntities()` | `BatchDestroy()` |
| `Multi<T>.Count` | `Multi<T>.Length` |

---

## Итог

StaticEcs 2.0 — это переход от быстрого, но несколько разрозненного API к целостному, принципиально расширенному фреймворку. Ключевые достижения:

- **Отслеживание изменений** — 16 фильтров, bitmap-хранение, нулевой оверхед при отключении. Сетевая синхронизация, реактивный UI, триггеры — без ручных dirty-флагов.
- **Типы сущностей** — логическая и физическая группировка. Кэш-локальность из коробки, хуки жизненного цикла, параметризованное создание.
- **Блочная итерация** — прямые указатели на массивы данных для максимальной производительности unmanaged-кода.
- **Единый ISystem** — один интерфейс вместо пяти, с автоматическим обнаружением реализованных методов.
- **Хуки в IComponent** — поведение компонента в одном месте, без Config-классов.
- **Or-фильтры** — запросы, которые раньше были невозможны.
- **Расширенные batch-операции** — полный спектр с цепочками.
- **Чистый, единообразный API** — свойства вместо методов, короткие имена, fluent-регистрация.

Миграция потребует изменений практически во всём пользовательском коде, но каждое изменение — это шаг к более простому, быстрому и выразительному API. Подробный гайд миграции с таблицами соответствий доступен в [migrationguide](docs/ru/migrationguide.md).
