---
title: Миграция на 2.0.0
parent: RU
nav_order: 5
---

# Миграция с 1.2.x на 2.0.0

Версия 2.0.0 — полная реструктуризация фреймворка. Практически весь пользовательский код потребует изменений.

___

## Обзор изменений

- **Сегментная модель хранения** — новый уровень иерархии Chunk → Segment (256 entity) → Block → Entity
- **entityType** (`IEntityType`) — логическая группировка сущностей для кэш-локальности через дженерик-параметр
- **Хуки в IComponent/IEvent** — через default interface methods вместо отдельных Config-классов
- **Единый ISystem** вместо IInitSystem/IUpdateSystem/IDestroySystem
- **Единый Query** — `Query.Entities<>()` и `Query.For()` объединены в `Query<>()`
- **Связи** — IEntityLinkComponent → ILinkType + Link\<T\>/Links\<T\>
- **Context → Resources** — `Context.Set/Get` → `SetResource`/`GetResource`
- **Теги унифицированы с компонентами** — теги хранятся в `Components<T>` с флагом `IsTag`, используют те же фильтры запросов (`All<>`, `None<>`, `Any<>`)
- **Свойства вместо методов** для состояний Entity и World
- **Переименование batch-операций запроса**: `DestroyAllEntities()` → `BatchDestroy()`, новый `BatchUnload()`
- **Удалено**: `UnloadCluster()`/`UnloadChunk()` — используйте `Query().BatchUnload()` с фильтрацией по кластерам/чанкам

___

## 0. Переименование batch-операций запроса

#### `DestroyAllEntities()` → `BatchDestroy()`:
```csharp
// Было:
W.Query<All<Health>, TagAll<IsDead>>().DestroyAllEntities();

// Стало:
W.Query<All<Health, IsDead>>().BatchDestroy();
```

#### Новое: `BatchUnload()` — массовая выгрузка сущностей по фильтру:
```csharp
W.Query<All<Position>>().BatchUnload();
```

#### Удалено: `UnloadCluster()` / `UnloadChunk()` — используйте `BatchUnload()` с фильтрацией:
```csharp
// Было:
W.UnloadCluster(clusterId);
W.UnloadChunk(chunkIdx);

// Стало:
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { clusterId };
W.Query().BatchUnload(EntityStatusType.Any, clusters: clusters);

ReadOnlySpan<uint> chunks = stackalloc uint[] { chunkIdx };
W.Query().BatchUnload(EntityStatusType.Any, chunks);
```

___

## 1. World API

Подробнее: [Мир](features/world.md)

#### Методы → Свойства:
```csharp
// Было:                              Стало:
W.IsInitialized()                  →  W.IsWorldInitialized
W.IsIndependent()                  →  W.IsIndependent
                                      W.Status  // новое (WorldStatus enum)
```

#### Создание сущностей:

Подробнее: [Сущность](features/entity.md)

```csharp
// Было:
var entity = W.Entity.New(clusterId);
var entity = W.Entity.New<Position>(new Position());
W.Entity.NewOnes(count, onCreate, clusterId);
bool ok = W.Entity.TryNew(out entity, clusterId);

// Стало:
var entity = W.NewEntity<Default>(clusterId: 0);
var entity = W.NewEntity<Default>(new Default(), clusterId: 0);
W.NewEntities<Default>(count: 100, clusterId: 0, onCreate: null);
bool ok = W.TryNewEntity<Default>(out entity, clusterId: 0);
var entity = W.NewEntity<Default>();  // тип сущности Default, clusterId=0
```

#### WorldConfig:
```csharp
// Новые поля:
WorkerSpinCount       // spin-wait итераций перед блокировкой (по умолчанию 256)
BaseClustersCapacity  // начальная ёмкость кластеров (по умолчанию 16)

// Фабричные методы:
WorldConfig.Default()      // стандартные настройки
WorldConfig.MaxThreads()   // все доступные потоки
```

#### Удалено:
- `IWorld` интерфейс → `WorldHandle`
- `WorldWrapper<W>` → `WorldHandle`
- `Worlds` статический класс
- `BoxedEntity<W>` / `IEntity`

___

## 2. Entity API

Подробнее: [Сущность](features/entity.md), [Глобальный идентификатор](features/gid.md)

#### Методы → Свойства:
```csharp
// Было:               Стало:
entity.Gid()        →  entity.GID
entity.GidCompact() →  entity.GIDCompact
entity.IsNotDestroyed()→ entity.IsNotDestroyed  // метод → свойство
                         entity.IsDestroyed     // НОВОЕ свойство
entity.IsDisabled() →  entity.IsDisabled
entity.IsEnabled()  →  entity.IsEnabled
entity.Version()    →  entity.Version
entity.ClusterId()  →  entity.ClusterId
entity.Chunk()      →  entity.ChunkID
entity.IsSelfOwned()→  entity.IsSelfOwned
```

#### Новые свойства:
```csharp
entity.EntityType   // byte — ID типа сущности (из регистрации IEntityType)
entity.ID           // raw slot index
```

#### Проверка наличия компонентов:

Подробнее: [Компонент](features/component.md), [Тег](features/tag.md)

```csharp
// Было:                              Стало:
entity.HasAllOf<C>()              →   entity.Has<C>()
entity.HasAllOf<C1, C2>()         →   entity.Has<C1, C2>()
entity.HasAnyOf<C1, C2>()         →   entity.HasAny<C1, C2>()
entity.HasDisabledAllOf<C>()      →   entity.HasDisabled<C>()
entity.HasEnabledAllOf<C>()       →   entity.HasEnabled<C>()

// Теги:
entity.HasAllOfTags<T>()          →   entity.Has<T>()
entity.HasAnyOfTags<T1, T2>()     →   entity.HasAny<T1, T2>()
```

#### Add — новая семантика:

Подробнее: [Компонент — Добавление](features/component.md#добавление-компонентов)

```csharp
// ═══ Было (v1.2.x) ═══
entity.Add<C>();                    // ASSERT что компонента нет
ref var c = ref entity.TryAdd<C>(); // идемпотентный
entity.Put(new Position(1, 2));     // upsert без хуков

// ═══ Стало (v2.0.0) ═══
ref var c = ref entity.Add<C>();              // идемпотентный (бывший TryAdd)
ref var c = ref entity.Add<C>(out bool isNew);// с флагом
entity.Set(new Position(1, 2));               // ВСЕГДА OnDelete→замена→OnAdd
```

| Старый метод | Новый эквивалент |
|---|---|
| `entity.TryAdd<C>()` | `entity.Add<C>()` |
| `entity.TryAdd<C>(out bool)` | `entity.Add<C>(out bool isNew)` |
| `entity.Put<C>(value)` | `entity.Set<C>(value)` (но теперь с хуками) |
| `entity.Add<C>()` (старый, assert) | Нет аналога |

#### Delete/Disable/Enable — возвращают bool:
```csharp
// Было:
entity.Delete<C>();               // void, assert
bool ok = entity.TryDelete<C>();  // bool

// Стало:
bool deleted = entity.Delete<C>();    // bool (бывший TryDelete)
ToggleResult disabled = entity.Disable<C>();  // ToggleResult: MissingComponent, Unchanged, Changed
ToggleResult enabled = entity.Enable<C>();    // ToggleResult: MissingComponent, Unchanged, Changed
```

#### Новые методы:
```csharp
entity.Clone(clusterId);                       // клон в кластер
entity.MoveTo(clusterId);                      // перемещение в кластер
```

#### Удалено:
- `entity.Box()` / `BoxedEntity<W>` / `IEntity`
- `entity.TryAdd<C>()` → используйте `entity.Add<C>()`
- `entity.Put<C>(val)` → используйте `entity.Set<C>(val)`
- `entity.TryDelete<C>()` → используйте `entity.Delete<C>()`
- `entity.TryCopyComponentsTo<C>(target)` → `entity.CopyTo<C>(target)` (возвращает bool)
- `entity.TryMoveComponentsTo<C>(target)` → `entity.MoveTo<C>(target)` (возвращает bool)
- Все Raw-методы (`RawHasAllOf`, `RawAdd`, `RawGet`, `RawPut`, etc.)
- `Entity.New(...)` (все перегрузки) → `W.NewEntity(...)`
- `W.OnCreateEntity(callback)` → используйте `IEntityType.OnCreate` хук или `Created` трекинг-фильтр

___

## 3. EntityGID API

Подробнее: [Глобальный идентификатор](features/gid.md)

```csharp
// ═══ Было (v1.2.x) ═══
bool ok = gid.IsActual<WT>();
bool loaded = gid.IsLoaded<WT>();
bool both = gid.IsLoadedAndActual<WT>();

// ═══ Стало (v2.0.0) ═══
GIDStatus status = gid.Status<WT>();
// GIDStatus.Active     — сущность существует, версия совпадает, загружена (бывший IsLoadedAndActual)
// GIDStatus.NotActual  — сущность не существует или версия/кластер не совпадает (бывший !IsActual)
// GIDStatus.NotLoaded  — сущность существует, версия совпадает, но выгружена (бывший IsActual && !IsLoaded)
```

| Старый метод | Новый эквивалент |
|---|---|
| `gid.IsActual<WT>()` | `gid.Status<WT>() != GIDStatus.NotActual` |
| `gid.IsLoaded<WT>()` | `gid.Status<WT>() != GIDStatus.NotLoaded` |
| `gid.IsLoadedAndActual<WT>()` | `gid.Status<WT>() == GIDStatus.Active` |

#### Создание сущности по GID:
```csharp
// Было:
var entity = W.Entity.New(someGid);

// Стало:
var entity = W.NewEntityByGID<Default>(someGid);
```

___

## 4. Компоненты

Подробнее: [Компонент](features/component.md)

#### Хуки — в IComponent через default interface methods:
```csharp
// ═══ Было (v1.2.x) ═══
struct Position : IComponent { public float X, Y; }

class PositionConfig : IComponentConfig<Position, WT> {
    public OnComponentHandler<Position> OnAdd() => (ref Position c, Entity e) => { };
    public OnComponentHandler<Position> OnDelete() => ...;
    public Guid Id() => ...;
    public BinaryWriter<Position> Writer() => ...;
    public BinaryReader<Position> Reader() => ...;
}
W.RegisterComponentType<Position>(new PositionConfig());

// ═══ Стало (v2.0.0) ═══
struct Position : IComponent {
    public float X, Y;

    public void OnAdd<TWorld>(World<TWorld>.Entity self)
        where TWorld : struct, IWorldType { }

    public void OnDelete<TWorld>(World<TWorld>.Entity self, HookReason reason)
        where TWorld : struct, IWorldType { }

    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType { writer.WriteFloat(X); writer.WriteFloat(Y); }

    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled)
        where TWorld : struct, IWorldType { X = reader.ReadFloat(); Y = reader.ReadFloat(); }
}

W.Types().Component<Position>(new ComponentTypeConfig<Position>(
    guid: new Guid("..."),
    version: 0,
    readWriteStrategy: new UnmanagedPackArrayStrategy<Position>()
));
```

#### Удалено:
- `IComponentConfig<T, W>`, `DefaultComponentConfig<T, W>`, `ValueComponentConfig<T, W>`
- `OnComponentHandler<T>`, `OnCopyHandler<T>` делегаты
- Хук `OnPut` (в v2 `Set(value)` вызывает OnDelete+OnAdd)

#### Доступ к пулу:
```csharp
// Было:                              Стало:
Components<T>.Value.Ref(entity)   →   Components<T>.Instance.Ref(entity)
Components<T>.Value.Has(entity)   →   Components<T>.Instance.Has(entity)
Components<T>.Value.IsRegistered()→   Components<T>.Instance.IsRegistered  // свойство
Components<T>.Value.DynamicId()   →   Components<T>.Instance.DynamicId    // свойство
```

___

## 5. Теги

Подробнее: [Тег](features/tag.md)

Теги унифицированы с компонентами. Теги теперь хранятся в `Components<T>` с флагом `IsTag`.

#### Методы сущности:
```csharp
// Было:                              Стало:
entity.SetTag<T>()                →   entity.Set<T>()
entity.HasTag<T>()                →   entity.Has<T>()
entity.HasAnyTags<T1,T2>()        →   entity.HasAny<T1,T2>()
entity.DeleteTag<T>()             →   entity.Delete<T>()
entity.ToggleTag<T>()             →   entity.Toggle<T>()
entity.ApplyTag<T>(bool)          →   entity.Apply<T>(bool)
entity.CopyTagsTo<T>(target)      →   entity.CopyTo<T>(target)
entity.MoveTagsTo<T>(target)      →   entity.MoveTo<T>(target)
```

#### Доступ к пулу:
```csharp
// Было:                              Стало:
Tags<T>.Value                     →   Components<T>.Instance (IsTag = true)
```

#### Фильтры запросов:
```csharp
// Было:                              Стало:
TagAll<T>                         →   All<T>
TagNone<T>                        →   None<T>
TagAny<T1,T2>                     →   Any<T1,T2>
```

#### Удалено:
- `WorldConfig.BaseTagTypesCount` → теги учитываются в `BaseComponentTypesCount`
- `DeleteTagsSystem<W, T>` → используйте `Query().BatchDelete<T>()`

___

## 6. Запросы (Query)

Подробнее: [Запросы](features/query.md)

#### Единая точка входа:
```csharp
// ═══ Было (v1.2.x) ═══
foreach (var entity in W.Query.Entities<All<Position>>()) { }

W.Query.For((ref Position pos) => {
    pos.X += 1;
});

// ═══ Стало (v2.0.0) ═══
foreach (var entity in W.Query<All<Position>>().Entities()) { }

W.Query().For(
    static (ref Position pos) => { pos.X += 1; }
);
```

#### Поиск сущности:
```csharp
// Было:
W.Query.Entities<All<P>>().First(out entity);

// Стало:
W.Query<All<P>>().Any(out entity);
```

#### Batch-операции:
```csharp
// Было (на QueryEntitiesIterator):
W.Query.Entities<All<P>>().AddForAll<Health>();
W.Query.Entities<All<P>>().DeleteForAll<Health>();
W.Query.Entities<All<P>>().SetTagForAll<Active>();

// Стало (на WorldQuery, с chaining):
W.Query<All<P>>().BatchAdd<Health>().BatchSet<Active>();
W.Query<All<P>>().BatchDelete<Health>();
W.Query<All<P>>().BatchDisable<Health>();   // новое
W.Query<All<P>>().BatchEnable<Health>();    // новое
W.Query<All<P>>().BatchDestroy();
W.Query<All<P>>().BatchUnload();    // новое
```

#### Фильтр-обёртка With → And:
```csharp
// Было:
W.Query.Entities<With<All<Pos>, None<Name>>>();

// Стало:
W.Query<And<All<Pos>, None<Name>>>();
// Также доступен Or<> для дизъюнкции фильтров (новое)
```

#### Параллельная итерация:
```csharp
// Было:
W.Query.Parallel.For(minChunkSize: 50000, (W.Entity ent, ref Position pos) => { });

// Стало:
W.Query().ForParallel(
    static (W.Entity ent, ref Position pos) => { },
    minEntitiesPerThread: 50000
);
```

#### QueryMode:

Подробнее: [Производительность — QueryMode](performance.md#querymode)

```csharp
// Было: runtime параметр при создании итератора
W.Query.Entities<All<P>>(queryMode: QueryMode.Flexible);

// Стало: по умолчанию Strict, для Flexible — отдельные методы
foreach (var entity in W.Query<All<P>>().EntitiesFlexible()) { }

W.Query().For(
    static (ref Position pos) => { },
    queryMode: QueryMode.Flexible
);
```

___

## 7. Связи (Relations)

Подробнее: [Отношения](features/relations.md)

```csharp
// ═══ Было (v1.2.x) ═══
struct Parent : IEntityLinkComponent<Parent> {
    public EntityGID Link;
    ref EntityGID IRefProvider<Parent, EntityGID>.RefValue(ref Parent c) => ref c.Link;
}
W.RegisterToOneRelationType<Parent>(config);

// ═══ Стало (v2.0.0) ═══
struct ParentLink : ILinkType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType { }
    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType { }
}
W.Types()
    .Link<ParentLink>()       // связь "один"
    .Links<ParentLink>();     // связь "много"

// Использование:
entity.Set(new W.Link<ParentLink>(parentEntity));
ref var links = ref entity.Ref<W.Links<ChildLink>>();
```

| Старый тип | Новый эквивалент |
|---|---|
| `IEntityLinkComponent<T>` | `ILinkType` + `Link<T>` |
| `IEntityLinksComponent<T>` | `ILinksType` + `Links<T>` |
| `RegisterToOneRelationType` | `W.Types().Link<T>()` |
| `RegisterToManyRelationType` | `W.Types().Links<T>()` |
| `RegisterOneToManyRelationType` | Раздельно: `Link<T>` + `Links<T>` |

___

## 8. События (Events)

Подробнее: [События](features/events.md)

```csharp
// ═══ Было (v1.2.x) ═══
struct MyEvent : IEvent { public int Data; }
class MyEventConfig : IEventConfig<MyEvent, WT> { ... }
W.Events.RegisterEventType<MyEvent>(new MyEventConfig());
W.Events.Send(new MyEvent { Data = 1 });
var receiver = W.Events.RegisterEventReceiver<MyEvent>();
W.Events.DeleteEventReceiver(ref receiver);

// ═══ Стало (v2.0.0) ═══
struct MyEvent : IEvent {
    public int Data;
    public void Write(ref BinaryPackWriter writer) { writer.WriteInt(Data); }
    public void Read(ref BinaryPackReader reader, byte version) { Data = reader.ReadInt(); }
}
W.Types().Event<MyEvent>(new EventTypeConfig<MyEvent>(guid: new Guid("...")));
W.SendEvent(new MyEvent { Data = 1 });
var receiver = W.RegisterEventReceiver<MyEvent>();
W.DeleteEventReceiver(ref receiver);
```

- `IEventConfig<T, W>` удалён → `EventTypeConfig<T>` + хуки в `IEvent`
- `W.Events.XXX` → `W.XXX` (методы перенесены на World)
- `IsClearable()` → `NoDataLifecycle` (инвертированная логика, расширенная семантика — управляет инициализацией и очисткой)

___

## 9. Системы (Systems)

Подробнее: [Системы](features/systems.md)

```csharp
// ═══ Было (v1.2.x) ═══
struct MoveSystem : IUpdateSystem {
    public void Update() { }
}
Systems.AddUpdate(new MoveSystem());
Systems.AddCallOnce(new InitSystem());

// ═══ Стало (v2.0.0) ═══
struct MoveSystem : ISystem {
    public void Init() { }          // вместо IInitSystem
    public void Update() { }        // вместо IUpdateSystem
    public bool UpdateIsActive() => true; // вместо ISystemCondition
    public void Destroy() { }       // вместо IDestroySystem
}
GameSys.Add(new MoveSystem(), order: 0);
```

| Старый тип | Новый эквивалент |
|---|---|
| `IInitSystem` | `ISystem.Init()` |
| `IUpdateSystem` | `ISystem.Update()` |
| `IDestroySystem` | `ISystem.Destroy()` |
| `ISystemCondition` | `ISystem.UpdateIsActive()` |
| `Systems.AddUpdate(system)` | `Sys.Add(system, order)` |
| `Systems.AddCallOnce(system)` | `Sys.Add(system, order)` + `Init()` |

___

## 10. Мульти-компоненты

Подробнее: [Мульти-компонент](features/multicomponent.md)

```csharp
// ═══ Было (v1.2.x) ═══
struct Items : IMultiComponent<Items, int> {
    public Multi<int> Values;
    public ref Multi<int> RefValue(ref Items c) => ref c.Values;
}
W.RegisterMultiComponentType<Items, int>(4, config);

// ═══ Стало (v2.0.0) ═══
struct Items : IMultiComponent {  // маркерный интерфейс, без RefValue
    public Multi<int> Values;
}
W.Types().Multi<Items>();  // регистрация мульти-компонента
```

- `IMultiComponent<T, V>` → `IMultiComponent` (маркер)
- `RefValue()` удалён
- `RegisterMultiComponentType` → `W.Types().Multi<T>()` (не `Component<T>`!)
- `Count` → `Length`
- `IsEmpty()`/`IsNotEmpty()`/`IsFull()` → свойства

___

## 11. Сериализация

Подробнее: [Сериализация](features/serialization.md)

```csharp
// ═══ Было (v1.2.x) ═══
// Через Config-классы с BinaryWriter<T>/BinaryReader<T> делегатами
class PositionConfig : DefaultComponentConfig<Position, WT> {
    public override BinaryWriter<Position> Writer() => ...;
    public override BinaryReader<Position> Reader() => ...;
}

// События:
W.Events.CreateSnapshot();
W.Events.LoadSnapshot(snapshot);

// ═══ Стало (v2.0.0) ═══
// Хуки Write/Read прямо на IComponent/IEvent
struct Position : IComponent {
    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType { }
    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled)
        where TWorld : struct, IWorldType { }
}

// События:
W.Serializer.CreateEventsSnapshot();
W.Serializer.LoadEventsSnapshot(snapshot);
```

___

## 12. Context → Resources

Подробнее: [Ресурсы](features/resources.md)

```csharp
// ═══ Было (v1.2.x) ═══
W.Context.Set<GameTime>(new GameTime());
ref var time = ref W.Context.Get<GameTime>();
bool has = W.Context.Has<GameTime>();

// ═══ Стало (v2.0.0) ═══
W.SetResource(new GameTime());                // Set перезаписывает без ошибки (заменяет Replace)
ref var time = ref W.GetResource<GameTime>();
bool has = W.HasResource<GameTime>();
W.RemoveResource<GameTime>();

// Именованные ресурсы (новое):
W.SetResource("key", new GameConfig());
ref var cfg = ref W.GetResource<GameConfig>("key");
```

___

## Краткая таблица переименований

| Было (v1.2.x) | Стало (v2.0.0) |
|---|---|
| `W.Entity.New(...)` | `W.NewEntity<TEntityType>(...)` / `W.NewEntity<Default>()` |
| `W.Entity.NewOnes(...)` | `W.NewEntities<TEntityType>(count, ...)` |
| `W.IsInitialized()` | `W.IsWorldInitialized` |
| `W.IsIndependent()` | `W.IsIndependent` |
| `entity.Gid()` | `entity.GID` |
| `entity.HasAllOf<C>()` | `entity.Has<C>()` |
| `entity.HasAnyOf<C1,C2>()` | `entity.HasAny<C1,C2>()` |
| `entity.HasAllOfTags<T>()` | `entity.Has<T>()` |
| `entity.TryAdd<C>()` | `entity.Add<C>()` |
| `entity.Put<C>(val)` | `entity.Set<C>(val)` |
| `entity.TryDelete<C>()` | `entity.Delete<C>()` (bool) |
| `Components<T>.Value` | `Components<T>.Instance` |
| `Tags<T>.Value` | `Components<T>.Instance` |
| `W.Query.Entities<F>()` | `W.Query<F>()` |
| `W.Query.For(...)` | `W.Query<F>().For(...)` |
| `AddForAll<C>()` | `BatchAdd<C>()` |
| `DeleteForAll<C>()` | `BatchDelete<C>()` |
| `SetTagForAll<T>()` | `BatchSet<T>()` |
| `IInitSystem` / `IUpdateSystem` | `ISystem` |
| `Systems.AddUpdate(sys)` | `Sys.Add(sys, order)` |
| `IComponentConfig<T,W>` | `ComponentTypeConfig<T>` + хуки в IComponent |
| `IEventConfig<T,W>` | `EventTypeConfig<T>` + хуки в IEvent |
| `IEntityLinkComponent<T>` | `ILinkType` + `Link<T>` |
| `IEntityLinksComponent<T>` | `ILinksType` + `Links<T>` |
| `RegisterToOneRelationType` | `W.Types().Link<T>()` |
| `IMultiComponent<T,V>` | `IMultiComponent` (маркер) |
| `RegisterMultiComponentType` | `W.Types().Multi<T>()` |
| `W.Context.Set/Get/Has` | `W.SetResource/GetResource/HasResource` |
| `W.Context<T>.Replace(val)` | `W.SetResource(val)` (перезаписывает автоматически) |
| `W.Events.Send(...)` | `W.SendEvent(...)` |
| `W.Events.RegisterEventReceiver` | `W.RegisterEventReceiver` |
| `W.Events.CreateSnapshot()` | `W.Serializer.CreateEventsSnapshot()` |
| `gid.IsActual<WT>()` | `gid.Status<WT>() != GIDStatus.NotActual` |
| `gid.IsLoadedAndActual<WT>()` | `gid.Status<WT>() == GIDStatus.Active` |
| `W.Entity.New(gid)` | `W.NewEntityByGID<TEntityType>(gid)` |
| `W.OnCreateEntity(callback)` | `IEntityType.OnCreate` / `Created` трекинг |
| `With<F1, F2>` | `And<F1, F2>` |
| `Query.Parallel.For(minChunkSize:)` | `Query().ForParallel(minEntitiesPerThread:)` |
