---
title: Мир
parent: Возможности
nav_order: 9
---

## WorldType
Тип-тег-идентификатор мира, служит для изоляции статических данных при создании разных миров в одном процессе
- Представлен в виде пользовательской структуры без данных с маркер-интерфейсом `IWorldType`
- Каждый уникальный `IWorldType` получает полностью изолированное статическое хранилище

#### Пример:
```csharp
public struct MainWorldType : IWorldType { }
public struct MiniGameWorldType : IWorldType { }
```

___

## World
Точка входа в библиотеку, отвечающая за доступ, создание, инициализацию, работу и уничтожение данных мира
- Представлен в виде статического класса `World<T>` параметризованного `IWorldType`

{: .importantru }
> Так как тип-идентификатор `IWorldType` определяет доступ к конкретному миру,
> есть три способа работы с библиотекой:

___

#### Первый способ — полное обращение:
```csharp
public struct WT : IWorldType { }

World<WT>.Create(WorldConfig.Default());
World<WT>.CalculateEntitiesCount();

var entity = World<WT>.NewEntity<Default>();
```

#### Второй способ — статические импорты:
```csharp
using static FFS.Libraries.StaticEcs.World<WT>;

public struct WT : IWorldType { }

Create(WorldConfig.Default());
CalculateEntitiesCount();

var entity = NewEntity<Default>();
```

#### Третий способ — тип-алиас в корневом неймспейсе:
Везде в примерах будет использован именно этот способ
```csharp
public struct WT : IWorldType { }

public abstract class W : World<WT> { }

W.Create(WorldConfig.Default());
W.CalculateEntitiesCount();

var entity = W.NewEntity<Default>();
```

___

## Жизненный цикл

```
Create() → Регистрация типов → Initialize() → Работа → Destroy()
```

#### WorldStatus:
- `NotCreated` — мир не создан или уничтожен
- `Created` — структуры выделены, доступна регистрация типов
- `Initialized` — мир полностью готов к работе, доступны операции с сущностями

___

#### Создание мира:
```csharp
// Определяем идентификатор мира
public struct WT : IWorldType { }
public abstract class W : World<WT> { }

// Создание мира с конфигурацией по умолчанию
W.Create(WorldConfig.Default());

// Или с пользовательской конфигурацией (все параметры опциональны — незаданные значения берутся из умолчаний)
W.Create(new WorldConfig {
    // Независимый мир (управляет чанками самостоятельно) или зависимый (требует ручного управления чанками)
    Independent = true,
    // Начальная ёмкость для типов компонентов (по умолчанию — 64)
    BaseComponentTypesCount = 64,
    // Начальная ёмкость для кластеров (минимум 16, по умолчанию — 16)
    BaseClustersCapacity = 16,
    // Количество потоков для параллельных запросов (по умолчанию — 0, однопоточный)
    // 0 — потоки не создаются
    // WorldConfig.MaxThreadCount — все доступные потоки CPU
    // N — указанное количество потоков
    ThreadCount = 4,
    // Количество итераций ожидания потока перед блокировкой (по умолчанию — 256)
    WorkerSpinCount = 256,
    // Включить отслеживание создания сущностей для фильтра Created (по умолчанию — false)
    TrackCreated = true,
});
```

{: .noteru }
`WorldConfig` предоставляет фабричные методы:
- `WorldConfig.Default()` — стандартные настройки (однопоточный, независимый)
- `WorldConfig.MaxThreads()` — все доступные потоки CPU
Все параметры опциональны — незаданные значения берутся из `WorldConfig.Default()`.

___

#### Регистрация типов:
```csharp
W.Create(WorldConfig.Default());

// Регистрация компонентов, тегов и событий — только между Create() и Initialize()
W.Types()
    .EntityType<Bullet>()
    .Component<Position>()
    .Component<Velocity>()
    .Tag<IsPlayer>()
    .Event<OnDamage>();

// Инициализация мира
W.Initialize();
```

{: .importantru }
Регистрация типов (`.Component<T>()`, `.Tag<T>()`, `.EntityType<T>()`) доступна только в состоянии `Created` — после `Create()` и до `Initialize()`. Регистрация событий (`.Event<T>()`) доступна также после инициализации.

___

#### Авторегистрация типов:
Вместо ручной регистрации каждого типа можно использовать автоматическое сканирование сборок.
`RegisterAll()` находит все структуры, реализующие ECS-интерфейсы, и регистрирует их автоматически:

```csharp
W.Create(WorldConfig.Default());

// Авторегистрация всех типов из вызывающей сборки
W.Types().RegisterAll();

// Или указать конкретные сборки
W.Types().RegisterAll(typeof(MyGame).Assembly, typeof(MyPlugin).Assembly);

// Можно комбинировать с ручной регистрацией
W.Types()
    .RegisterAll()
    .Component<SpecialComponent>();

W.Initialize();
```

Обнаруживаемые интерфейсы:

| Интерфейс | Регистрация |
|-----------|-------------|
| `IComponent` | `Types().Component<T>()` |
| `ITag` | `Types().Tag<T>()` |
| `IEvent` | `Types().Event<T>()` |
| `ILinkType` | Оборачивается в `Link<T>` и регистрируется как компонент |
| `ILinksType` | Оборачивается в `Links<T>` и регистрируется как компонент |
| `IMultiComponent` | Оборачивается в `Multi<T>` и регистрируется как компонент |
| `IEntityType` | `Types().EntityType<T>()` |

{: .noteru }
- Если сборки не указаны, сканируется только вызывающая сборка (не все загруженные)
- Сборка самого фреймворка StaticEcs всегда исключается из сканирования
- `RegisterAll()` ищет статическое поле или свойство соответствующего типа конфига внутри каждой структуры и использует его, если найдено. Иначе используется конфигурация по умолчанию. Правила поиска:
  - `IComponent` — ищет `ComponentTypeConfig<T>` (предпочитает имя `Config`)
  - `IEvent` — ищет `EventTypeConfig<T>` (предпочитает имя `Config`)
  - `ITag` — ищет `TagTypeConfig<T>` (предпочитает имя `Config`)
  - `IEntityType` — ищет `byte` (предпочитает имя `Id`)
- Поддерживаются и поля (field), и свойства (property)
- Структура, реализующая несколько интерфейсов (например, `IComponent` и `IMultiComponent`), будет зарегистрирована для каждого

___

#### Инициализация:
```csharp
// Стандартная инициализация (baseEntitiesCapacity — начальная ёмкость для сущностей)
W.Initialize(baseEntitiesCapacity: 4096);

// Инициализация с восстановлением сохранённых идентификаторов (версии EntityGID)
W.InitializeFromGIDStoreSnapshot(snapshot);

// Инициализация с полным восстановлением мира из снимка
W.InitializeFromWorldSnapshot(snapshot);
```

{: .noteru }
`InitializeFromGIDStoreSnapshot` восстанавливает только метаданные идентификаторов сущностей (версии GID). `InitializeFromWorldSnapshot` восстанавливает полное состояние мира, включая все сущности и их данные.

___

#### Уничтожение:
```csharp
// Уничтожить мир и освободить все ресурсы
W.Destroy();
```

___

## Основные операции

```csharp
// Текущий статус мира
WorldStatus status = W.Status;

// true если мир инициализирован
bool initialized = W.IsWorldInitialized;

// true если мир независимый
bool independent = W.IsIndependent;

// Количество сущностей в мире (активные + незагруженные)
uint entitiesCount = W.CalculateEntitiesCount();

// Количество загруженных сущностей
uint loadedCount = W.CalculateLoadedEntitiesCount();

// Текущая ёмкость для сущностей
uint capacity = W.CalculateEntitiesCapacity();

// Уничтожить все сущности в мире (мир остаётся инициализированным)
W.DestroyAllLoadedEntities();
```

___

Подробнее о создании сущностей и операциях с ними — см. [Сущность](entity).

Подробнее о ресурсах мира — см. [Ресурсы](resources).

___

## Кластер

Кластер — это группа чанков сущностей для пространственной сегментации мира. Сущности одного кластера сгруппированы и располагаются в памяти сегментировано.
- Представлен значением `ushort` (0–65535)
- По умолчанию при инициализации мира создаётся кластер с идентификатором 0
- Все сущности по умолчанию создаются в кластере 0
- Кластер можно отключить — сущности из отключённых кластеров не попадают в итерацию

{: .noteru }
Кластеры предназначены для **пространственной группировки**: уровни, зоны карты, игровые комнаты. Для **логической** группировки (юниты, снаряды, эффекты) используйте `entityType`.

___

#### Основные операции:
```csharp
// Регистрация кластеров (можно вызывать после Create() или после Initialize())
const ushort LEVEL_1_CLUSTER = 1;
const ushort LEVEL_2_CLUSTER = 2;
W.RegisterCluster(LEVEL_1_CLUSTER);
W.RegisterCluster(LEVEL_2_CLUSTER);

// Проверить зарегистрирован ли кластер
bool registered = W.ClusterIsRegistered(LEVEL_1_CLUSTER);

// Включить или отключить кластер — сущности из отключённых кластеров не попадают в итерацию
W.SetActiveCluster(LEVEL_2_CLUSTER, false);

// Проверить включён ли кластер
bool active = W.ClusterIsActive(LEVEL_2_CLUSTER);

// Уничтожить все сущности в кластере
W.DestroyAllEntitiesInCluster(LEVEL_1_CLUSTER);

// Освободить кластер — все сущности удаляются, чанки и идентификатор освобождаются
W.FreeCluster(LEVEL_2_CLUSTER);

// Безопасное освобождение — вернёт false если кластер не зарегистрирован
bool freed = W.TryFreeCluster(LEVEL_2_CLUSTER);
```

___

#### Снимки и выгрузка кластеров:
```csharp
// Создать снимок кластера (хранит все данные сущностей)
// Существуют перегрузки для записи на диск, сжатия и т.д.
byte[] snapshot = W.Serializer.CreateClusterSnapshot(LEVEL_1_CLUSTER);

// Выгрузить кластер из памяти
// Данные компонентов и тегов удаляются, сущности помечаются как незагруженные
// Сохраняется только информация об идентификаторах, сущности не попадают в запросы
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { LEVEL_1_CLUSTER };
W.Query().BatchUnload(EntityStatusType.Any, clusters: clusters);

// Загрузить кластер из снимка
W.Serializer.LoadClusterSnapshot(snapshot);
```

___

#### Чанки кластера:
```csharp
// Получить все чанки в кластере (включая пустые)
ReadOnlySpan<uint> chunks = W.GetClusterChunks(LEVEL_1_CLUSTER);

// Получить чанки, в которых есть хотя бы одна загруженная сущность
ReadOnlySpan<uint> loadedChunks = W.GetClusterLoadedChunks(LEVEL_1_CLUSTER);
```

___

#### Создание сущностей в кластере:
```csharp
// При создании сущности можно указать кластер (по умолчанию — кластер 0)
struct UnitType : IEntityType { }
var entity = W.NewEntity<UnitType>(clusterId: LEVEL_1_CLUSTER);

// Для всех перегрузок доступен параметр clusterId
W.NewEntity<UnitType>(
    new UnitType(),  // экземпляр типа сущности (может содержать данные для OnCreate)
    clusterId: LEVEL_1_CLUSTER
);

// Получить кластер сущности
ushort entityClusterId = entity.ClusterId;

// Получить кластер из EntityGID
ushort gidClusterId = entity.GID.ClusterId;
```

___

## Чанк

Чанк — это блок на 4096 сущностей. Весь мир состоит из чанков. Каждый чанк принадлежит какому-либо кластеру.

- **Независимый мир** (`Independent = true`) — управляет чанками автоматически, создаёт новые при необходимости
- **Зависимый мир** (`Independent = false`) — не имеет чанков для создания сущностей через `NewEntity()`, необходимо явно указать какие чанки доступны

___

#### Основные операции:
```csharp
// Найти свободный чанк, не принадлежащий никакому кластеру
// Независимый мир: если нет свободного — создаст новый
// Зависимый мир: если нет свободного — ошибка
EntitiesChunkInfo chunkInfo = W.FindNextSelfFreeChunk();
uint chunkIdx = chunkInfo.ChunkIdx;
// chunkInfo.EntitiesFrom — первый идентификатор сущности в чанке
// chunkInfo.EntitiesCapacity — размер чанка (всегда 4096)

// Безопасный вариант (вернёт false если нет свободных чанков)
bool found = W.TryFindNextSelfFreeChunk(out EntitiesChunkInfo info);

// Зарегистрировать чанк в кластере
W.RegisterChunk(chunkIdx, clusterId: LEVEL_1_CLUSTER);

// Зарегистрировать чанк с указанием типа владения
W.RegisterChunk(chunkIdx, owner: ChunkOwnerType.Self, clusterId: LEVEL_1_CLUSTER);

// Безопасная регистрация (вернёт false если чанк уже зарегистрирован)
bool registered = W.TryRegisterChunk(chunkIdx, clusterId: LEVEL_1_CLUSTER);

// Проверить зарегистрирован ли чанк
bool isRegistered = W.ChunkIsRegistered(chunkIdx);

// Получить кластер чанка
ushort clusterId = W.GetChunkClusterId(chunkIdx);

// Переместить чанк в другой кластер
W.ChangeChunkCluster(chunkIdx, LEVEL_2_CLUSTER);

// Проверить наличие сущностей в чанке
bool hasEntities = W.HasEntitiesInChunk(chunkIdx);           // активные + незагруженные
bool hasLoaded = W.HasLoadedEntitiesInChunk(chunkIdx);       // только загруженные

// Уничтожить все сущности в чанке
W.DestroyAllEntitiesInChunk(chunkIdx);

// Освободить чанк — все сущности удаляются, идентификатор освобождается
W.FreeChunk(chunkIdx);
```

___

#### Снимки и выгрузка чанков:
```csharp
// Создать снимок чанка
byte[] snapshot = W.Serializer.CreateChunkSnapshot(chunkIdx);

// Выгрузить чанк из памяти (данные удаляются, сущности помечаются как незагруженные)
ReadOnlySpan<uint> chunks = stackalloc uint[] { chunkIdx };
W.Query().BatchUnload(EntityStatusType.Any, chunks);

// Загрузить чанк из снимка
W.Serializer.LoadChunkSnapshot(snapshot);
```

___

#### Создание сущностей в конкретном чанке:
```csharp
// Создать сущность в указанном чанке
struct UnitType : IEntityType { }
var entity = W.NewEntityInChunk<UnitType>(chunkIdx: chunkIdx);

// Безопасный вариант (вернёт false если чанк заполнен)
bool created = W.TryNewEntityInChunk<UnitType>(out var entity, chunkIdx: chunkIdx);

// Не-дженерик вариант (тип сущности известен в runtime как byte)
byte entityTypeId = EntityTypeInfo<UnitType>.Id;
var entity = W.NewEntityInChunk(entityTypeId, chunkIdx: chunkIdx);
```

___

## Владение чанками (ChunkOwnerType)

Тип владения определяет, как мир использует чанк для создания сущностей:

- **`ChunkOwnerType.Self`** — чанк управляется данным миром. Сущности, создаваемые через `NewEntity()`, размещаются в этих чанках
  - Независимый мир по умолчанию имеет все чанки с `Self` владением
- **`ChunkOwnerType.Other`** — чанк не управляется данным миром. `NewEntity()` никогда не будет размещать сущности в этих чанках
  - Зависимый мир по умолчанию имеет все чанки с `Other` владением

```csharp
// Получить тип владения чанка
ChunkOwnerType owner = W.GetChunkOwner(chunkIdx);

// Изменить тип владения
// Self → Other: чанк становится недоступен для NewEntity()
// Other → Self: чанк становится доступен для NewEntity()
W.ChangeChunkOwner(chunkIdx, ChunkOwnerType.Other);
```

{: .importantru }
Создание сущностей через `NewEntityByGID<TEntityType>(gid)` доступно только для чанков с владением `Other`.
Создание сущностей через `NewEntityInChunk<TEntityType>(chunkIdx)` доступно только для чанков с владением `Self`.

___

#### Клиент-серверный пример:

```csharp
// === Серверная сторона (Independent мир) ===
// Находим свободный чанк и регистрируем с владением Other
// Сервер не будет создавать свои сущности в этом диапазоне идентификаторов
EntitiesChunkInfo chunkInfo = WServer.FindNextSelfFreeChunk();
WServer.RegisterChunk(chunkInfo.ChunkIdx, ChunkOwnerType.Other);
// Отправляем идентификатор чанка клиенту

// === Клиентская сторона (Dependent мир) ===
// Получаем идентификатор чанка от сервера
// Регистрируем с владением Self — теперь доступно 4096 слотов для сущностей
WClient.RegisterChunk(chunkIdxFromServer, ChunkOwnerType.Self);

// Клиент может создавать сущности через NewEntity()
// Например, для UI или VFX
var vfx = WClient.NewEntity<VfxType>();

// Аналогично работает для P2P:
// один Independent хост + N Dependent клиентов
```

___

## Примеры применения кластеров и чанков

#### Кластеры:
- **Уровни и зоны карты** — разные кластеры для разных частей игрового мира. При движении игрока можно загружать и выгружать кластеры, экономя память
- **Игровые уровни** — загрузка/выгрузка кластеров при смене уровня
- **Игровые сессии** — идентификатор кластера определяет сессию. В сочетании с параллельной итерацией возможна эмуляция мультимиров в рамках одного мира

#### Чанки:
- **Стриминг мира** — загрузка и выгрузка чанков в процессе игры
- **Пользовательское управление идентификаторами** — контроль над распределением EntityGID
- **Арена-память** — быстрое выделение и очистка большого количества временных сущностей

#### Владение чанками:
- **Клиент-серверное взаимодействие** — сервер выделяет диапазоны идентификаторов клиентам
- **P2P сетевые форматы** — один Independent хост и N Dependent клиентов
