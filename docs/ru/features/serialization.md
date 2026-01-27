---
title: Сериализация
parent: Возможности
nav_order: 15
---

## Сериализация
Сериализация — механизм создания бинарных снимков мира целиком или отдельных сущностей, кластеров, чанков.
Для бинарной сериализации используется [StaticPack](https://github.com/Felid-Force-Studios/StaticPack).

___

## Настройка компонентов

Для поддержки сериализации компонентов необходимо:
1. Указать `Guid` при регистрации (стабильный идентификатор типа)
2. Реализовать хуки `Write` и `Read` на компоненте

{: .importantru }
Хуки `Write` и `Read` **обязательны** для сериализации через `EntitiesSnapshot` (для всех типов компонентов, включая unmanaged). Для снимков мира/кластера/чанка non-unmanaged типы также всегда используют эти хуки.

#### Unmanaged компонент:
```csharp
public struct Position : IComponent {
    public float X, Y, Z;

    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType {
        writer.WriteFloat(X);
        writer.WriteFloat(Y);
        writer.WriteFloat(Z);
    }

    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled)
        where TWorld : struct, IWorldType {
        X = reader.ReadFloat();
        Y = reader.ReadFloat();
        Z = reader.ReadFloat();
    }
}

W.Types().Component<Position>(new ComponentTypeConfig<Position>(
    guid: new Guid("b121594c-456e-4712-9b64-b75dbb37e611"),
    readWriteStrategy: new UnmanagedPackArrayStrategy<Position>()
));
```

#### Non-unmanaged компонент (содержит ссылочные поля):
```csharp
public struct Name : IComponent {
    public string Value;

    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType {
        writer.WriteString16(Value);
    }

    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled)
        where TWorld : struct, IWorldType {
        Value = reader.ReadString16();
    }
}

W.Types().Component<Name>(new ComponentTypeConfig<Name>(
    guid: new Guid("531dc870-fdf5-4a8d-a4c6-b4911b1ea1c3")
));
```

#### Блочное копирование для unmanaged типов:

Для снимков мира/кластера/чанка unmanaged компоненты могут быть сериализованы блоком памяти вместо поэлементных вызовов `Write`/`Read`. Для этого нужно **явно указать** `UnmanagedPackArrayStrategy<T>`:

```csharp
W.Types().Component<Position>(new ComponentTypeConfig<Position>(
    guid: new Guid("b121594c-456e-4712-9b64-b75dbb37e611"),
    readWriteStrategy: new UnmanagedPackArrayStrategy<Position>()  // блочное копирование
));
```

{: .noteru }
`UnmanagedPackArrayStrategy<T>` выполняет прямое копирование памяти — значительно быстрее поэлементной сериализации. Работает только для unmanaged типов. При несовпадении версий (миграция данных) система автоматически переключается на хуки `Read`. Стратегия по умолчанию — `StructPackArrayStrategy<T>`.

#### Блочная сериализация сегментов для Multi и Links:

Мульти-компоненты и Links хранят значения в общем сегментном хранилище. По умолчанию значения каждой сущности сериализуются индивидуально. Для блочной сериализации сегментов хранилища (требуются unmanaged типы значений):

```csharp
// Мульти-компонент с блочной стратегией
W.Types().Multi<Item>(new ComponentTypeConfig<W.Multi<Item>>(
    guid: new Guid("..."),
    readWriteStrategy: new MultiUnmanagedPackArrayStrategy<MyWorld, Item>()
));

// Links с блочной стратегией
W.Types().Links<MyLinkType>(new ComponentTypeConfig<W.Links<MyLinkType>>(
    guid: new Guid("..."),
    readWriteStrategy: new LinksUnmanagedPackArrayStrategy<MyWorld, MyLinkType>()
));
```

#### Полная конфигурация:
```csharp
W.Types().Component<Position>(new ComponentTypeConfig<Position>(
    guid: new Guid("b121594c-456e-4712-9b64-b75dbb37e611"),
    version: 1,                  // версия схемы данных для миграции (по умолчанию — 0)
    noDataLifecycle: true,       // отключить управление данными фреймворком (по умолчанию — false)
    readWriteStrategy: new UnmanagedPackArrayStrategy<Position>() // стратегия сериализации (по умолчанию — StructPackArrayStrategy<T>)
));
```

___

## Настройка тегов

Теги настраиваются через `TagTypeConfig<T>`:

```csharp
W.Types()
    .Tag<IsPlayer>(new TagTypeConfig<IsPlayer>(guid: new Guid("3a6fe6a2-9427-43ae-9b4a-f8582e3a5f90")))
    .Tag<IsDead>(new TagTypeConfig<IsDead>(guid: new Guid("d25b7a08-cbe6-4c77-bd8e-29ce7f748c30")));
```

#### Полная конфигурация:
```csharp
W.Types().Tag<Poisoned>(new TagTypeConfig<Poisoned>(
    guid: new Guid("A1B2C3D4-..."), // стабильный идентификатор для сериализации (по умолчанию — default)
    trackAdded: true,                // включить отслеживание добавления (по умолчанию — false)
    trackDeleted: true               // включить отслеживание удаления (по умолчанию — false)
));
```

{: .noteru }
Для автоматической регистрации `RegisterAll()` подхватывает статическое поле `Guid` внутри структуры тега. Параметры `trackAdded` / `trackDeleted` при автоматической регистрации не устанавливаются — для отслеживания используйте ручную регистрацию с `TagTypeConfig<T>`.

___

## Настройка событий

Для событий используется `EventTypeConfig<T>` — аналогично компонентам:

```csharp
public struct OnDamage : IEvent {
    public float Amount;

    public void Write(ref BinaryPackWriter writer) {
        writer.WriteFloat(Amount);
    }

    public void Read(ref BinaryPackReader reader, byte version) {
        Amount = reader.ReadFloat();
    }
}

W.Types().Event<OnDamage>(new EventTypeConfig<OnDamage>(
    guid: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890")
));
```

___

## Снимок мира (World Snapshot)

Сохраняет полное состояние мира: все сущности, компоненты, теги и события.

#### Сохранение и загрузка при инициализации:
```csharp
// Сохранение мира
byte[] worldSnapshot = W.Serializer.CreateWorldSnapshot();
W.Destroy();

// Загрузка мира при инициализации — самый простой способ
CreateWorld(); // Create + регистрация типов
W.InitializeFromWorldSnapshot(worldSnapshot);

// Все сущности и события восстановлены
foreach (var entity in W.Query().Entities()) {
    Console.WriteLine(entity.PrettyString);
}
```

#### Сохранение и загрузка после инициализации:
```csharp
byte[] worldSnapshot = W.Serializer.CreateWorldSnapshot();
W.Destroy();

CreateWorld();
W.Initialize();
// Все существующие сущности и события удаляются перед загрузкой
W.Serializer.LoadWorldSnapshot(worldSnapshot);
```

#### Дополнительные параметры:
```csharp
// Сохранение в файл
W.Serializer.CreateWorldSnapshot("path/to/world.bin");

// С GZIP сжатием
byte[] compressed = W.Serializer.CreateWorldSnapshot(gzip: true);

// Фильтрация по кластерам
W.Serializer.CreateWorldSnapshot(clusters: new ushort[] { 0, 1 });

// Стратегия записи чанков
W.Serializer.CreateWorldSnapshot(strategy: ChunkWritingStrategy.SelfOwner);

// Без событий
W.Serializer.CreateWorldSnapshot(writeEvents: false);

// Без кастомных данных
W.Serializer.CreateWorldSnapshot(withCustomSnapshotData: false);

// Загрузка из файла
W.Serializer.LoadWorldSnapshot("path/to/world.bin");

// Загрузка сжатых данных
W.Serializer.LoadWorldSnapshot(compressed, gzip: true);
```

{: .importantru }
Все компоненты и теги, участвующие в снимке мира, **должны** иметь зарегистрированный `Guid`. В DEBUG-режиме будет ошибка при попытке сериализации без `Guid`.

___

## Снимок сущностей (Entities Snapshot)

Позволяет сохранять и загружать отдельные сущности с гранулярным контролем.

#### Сохранение сущностей:
```csharp
// Создаём писатель сущностей
using var writer = W.Serializer.CreateEntitiesSnapshotWriter();

// Записываем конкретные сущности
foreach (var entity in W.Query().Entities()) {
    writer.Write(entity);
}

// Или записываем все сущности сразу
// writer.WriteAllEntities();

// Создаём снимок
byte[] snapshot = writer.CreateSnapshot();

// Или сохраняем в файл
// writer.CreateSnapshot("path/to/entities.bin");
```

#### Запись с одновременной выгрузкой:
```csharp
using var writer = W.Serializer.CreateEntitiesSnapshotWriter();

// Записать и выгрузить — экономит память при стриминге
foreach (var entity in W.Query().Entities()) {
    writer.WriteAndUnload(entity);
}

// Или все сущности сразу
// writer.WriteAndUnloadAllEntities();

byte[] snapshot = writer.CreateSnapshot();
```

___

#### Загрузка сущностей (entitiesAsNew):

Параметр `entitiesAsNew` определяет, как загружаются сущности:

- **`entitiesAsNew: false`** (по умолчанию) — сущности восстанавливаются в **те же слоты** (тот же EntityGID). Если слот уже занят — ошибка в DEBUG.
- **`entitiesAsNew: true`** — сущности загружаются в **новые слоты** с новыми EntityGID. Связи между сущностями (Link, Links) могут указывать на неверные сущности.

```csharp
// Загрузка в оригинальные слоты
W.Serializer.LoadEntitiesSnapshot(snapshot, entitiesAsNew: false);

// Загрузка как новые сущности
W.Serializer.LoadEntitiesSnapshot(snapshot, entitiesAsNew: true);

// С колбеком для каждой загруженной сущности
W.Serializer.LoadEntitiesSnapshot(snapshot, entitiesAsNew: true, onLoad: entity => {
    Console.WriteLine($"Loaded: {entity.PrettyString}");
});
```

___

#### Сохранение связей между сущностями (GID Store):

Чтобы корректно загружать сущности с `entitiesAsNew: false`, нужно сохранить хранилище глобальных идентификаторов:

```csharp
// 1. Сохраняем сущности и GID Store
using var writer = W.Serializer.CreateEntitiesSnapshotWriter();
writer.WriteAllEntities();
byte[] entitiesSnapshot = writer.CreateSnapshot();
byte[] gidSnapshot = W.Serializer.CreateGIDStoreSnapshot();
W.Destroy();

// 2. Восстанавливаем мир с GID Store
CreateWorld();
W.InitializeFromGIDStoreSnapshot(gidSnapshot);

// Новые сущности не займут слоты сохранённых
var newEntity = W.NewEntity<Default>();
newEntity.Set(new Position { X = 1 });

// 3. Загружаем сущности в оригинальные слоты — все связи корректны
W.Serializer.LoadEntitiesSnapshot(entitiesSnapshot, entitiesAsNew: false);
```

{: .noteru }
GID Store хранит информацию обо всех выданных идентификаторах. Это гарантирует, что новые сущности не займут слоты выгруженных сущностей, и все связи (Link, Links, EntityGID в данных) останутся корректными.

___

## Хранилище GID (GID Store)

```csharp
// Сохранить GID Store
byte[] gidSnapshot = W.Serializer.CreateGIDStoreSnapshot();

// С GZIP сжатием
byte[] gidCompressed = W.Serializer.CreateGIDStoreSnapshot(gzip: true);

// В файл
W.Serializer.CreateGIDStoreSnapshot("path/to/gid.bin");

// Со стратегией записи чанков
W.Serializer.CreateGIDStoreSnapshot(strategy: ChunkWritingStrategy.SelfOwner);

// Фильтрация по кластерам
W.Serializer.CreateGIDStoreSnapshot(clusters: new ushort[] { 0, 1 });

// Инициализация мира из GID Store
CreateWorld();
W.InitializeFromGIDStoreSnapshot(gidSnapshot);

// Восстановление GID Store в уже инициализированном мире
// Все сущности удаляются, состояние сбрасывается
W.Serializer.RestoreFromGIDStoreSnapshot(gidSnapshot);
```

___

## Снимки кластеров и чанков

#### Кластер:
```csharp
// Сохранить кластер
byte[] clusterSnapshot = W.Serializer.CreateClusterSnapshot(clusterId: 1);

// С данными для загрузки как новые сущности
byte[] clusterWithEntities = W.Serializer.CreateClusterSnapshot(
    clusterId: 1,
    withEntitiesData: true  // необходимо для entitiesAsNew при загрузке
);

// Выгрузить кластер из памяти
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { 1 };
W.Query().BatchUnload(EntityStatusType.Any, clusters: clusters);

// Загрузить кластер из снимка
W.Serializer.LoadClusterSnapshot(clusterSnapshot);

// Загрузить как новые сущности в другой кластер
W.Serializer.LoadClusterSnapshot(clusterWithEntities,
    new EntitiesAsNewParams(entitiesAsNew: true, clusterId: 2)
);
```

#### Чанк:
```csharp
// Сохранить чанк
byte[] chunkSnapshot = W.Serializer.CreateChunkSnapshot(chunkIdx: 0);

// Выгрузить чанк из памяти
ReadOnlySpan<uint> unloadChunks = stackalloc uint[] { 0 };
W.Query().BatchUnload(EntityStatusType.Any, unloadChunks);

// Загрузить чанк из снимка
W.Serializer.LoadChunkSnapshot(chunkSnapshot);
```

{: .importantru }
По умолчанию снимки кластеров и чанков **не хранят** данные идентификаторов сущностей (только данные компонентов). Если нужно загружать их как новые сущности (`entitiesAsNew: true`), при создании снимка укажите `withEntitiesData: true`.

___

#### Комплексный пример стриминга:
```csharp
void PrintCounts(string label) {
    Console.WriteLine($"{label} — Всего: {W.CalculateEntitiesCount()} | Загружено: {W.CalculateLoadedEntitiesCount()}");
}

// Сохраняем отдельные сущности
using var writer = W.Serializer.CreateEntitiesSnapshotWriter();
foreach (var entity in W.Query().Entities()) {
    writer.WriteAndUnload(entity);
}
byte[] entitiesSnapshot = writer.CreateSnapshot();
PrintCounts("После выгрузки сущностей"); // Всего: 2 | Загружено: 0

// Создаём кластер и наполняем его
const ushort ZONE_CLUSTER = 1;
W.RegisterCluster(ZONE_CLUSTER);
struct ZoneEntityType : IEntityType { }
W.NewEntities<ZoneEntityType>(count: 2000, clusterId: ZONE_CLUSTER);
PrintCounts("После создания кластера"); // Всего: 2002 | Загружено: 2000

// Сохраняем и выгружаем кластер
byte[] clusterSnapshot = W.Serializer.CreateClusterSnapshot(ZONE_CLUSTER);
ReadOnlySpan<ushort> zoneClusters = stackalloc ushort[] { ZONE_CLUSTER };
W.Query().BatchUnload(EntityStatusType.Any, clusters: zoneClusters);
PrintCounts("После выгрузки кластера"); // Всего: 2002 | Загружено: 0

// Создаём чанк и наполняем его
var chunkIdx = W.FindNextSelfFreeChunk().ChunkIdx;
W.RegisterChunk(chunkIdx, clusterId: 0);
for (int i = 0; i < 100; i++) {
    W.NewEntityInChunk<ZoneEntityType>(chunkIdx: chunkIdx);
}
PrintCounts("После создания чанка"); // Всего: 2102 | Загружено: 100

// Сохраняем и выгружаем чанк
byte[] chunkSnapshot = W.Serializer.CreateChunkSnapshot(chunkIdx);
ReadOnlySpan<uint> unloadChunks = stackalloc uint[] { chunkIdx };
W.Query().BatchUnload(EntityStatusType.Any, unloadChunks);
PrintCounts("После выгрузки чанка"); // Всего: 2102 | Загружено: 0

// Сохраняем GID Store и пересоздаём мир
byte[] gidSnapshot = W.Serializer.CreateGIDStoreSnapshot();
W.Destroy();

CreateWorld();
W.InitializeFromGIDStoreSnapshot(gidSnapshot);

// Загружаем в любом порядке
W.Serializer.LoadClusterSnapshot(clusterSnapshot);
PrintCounts("После загрузки кластера"); // Всего: 2102 | Загружено: 2000

W.Serializer.LoadEntitiesSnapshot(entitiesSnapshot);
PrintCounts("После загрузки сущностей"); // Всего: 2102 | Загружено: 2002

W.Serializer.LoadChunkSnapshot(chunkSnapshot);
PrintCounts("После загрузки чанка"); // Всего: 2102 | Загружено: 2102
```

___

## Миграция данных

#### Версионирование компонентов:

Параметр `version` в хуке `Read` позволяет мигрировать данные между версиями схемы:

```csharp
public struct Position : IComponent {
    public float X, Y, Z;

    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType {
        writer.WriteFloat(X);
        writer.WriteFloat(Y);
        writer.WriteFloat(Z);
    }

    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled)
        where TWorld : struct, IWorldType {
        X = reader.ReadFloat();
        Y = reader.ReadFloat();
        // В версии 0 не было Z — задаём значение по умолчанию
        Z = version >= 1 ? reader.ReadFloat() : 0f;
    }
}

// Регистрация с новой версией
W.Types().Component<Position>(new ComponentTypeConfig<Position>(
    guid: new Guid("b121594c-456e-4712-9b64-b75dbb37e611"),
    version: 1,  // была версия 0, теперь 1
    readWriteStrategy: new UnmanagedPackArrayStrategy<Position>()
));
```

___

#### Миграция удалённых типов:

Если компонент, тег или событие было удалено из кода, по умолчанию данные пропускаются автоматически. Для кастомной обработки:

```csharp
// Миграция удалённого компонента
W.Serializer.SetComponentDeleteMigrator(
    new Guid("guid-удалённого-компонента"),
    (ref BinaryPackReader reader, W.Entity entity, byte version, bool disabled) => {
        // Прочитать ВСЕ данные и выполнить кастомную логику
    }
);

// Миграция удалённого тега
W.Serializer.SetMigrator(
    new Guid("guid-удалённого-тега"),
    (W.Entity entity) => {
        // Кастомная логика
    }
);

// Миграция удалённого события
W.Serializer.SetEventDeleteMigrator(
    new Guid("guid-удалённого-события"),
    (ref BinaryPackReader reader, byte version) => {
        // Прочитать ВСЕ данные и выполнить кастомную логику
    }
);
```

{: .noteru }
При добавлении новых типов старые снимки загружаются корректно — новые компоненты просто отсутствуют на загруженных сущностях.

___

## Колбеки

#### Глобальные колбеки:
```csharp
// Вызываются при всех типах снимков (World, Cluster, Chunk, Entities)

// Перед созданием снимка
W.Serializer.RegisterPreCreateSnapshotCallback(param => {
    Console.WriteLine($"Создание снимка типа: {param.Type}");
});

// После создания снимка
W.Serializer.RegisterPostCreateSnapshotCallback(param => {
    Console.WriteLine($"Снимок создан: {param.Type}");
});

// Перед загрузкой снимка
W.Serializer.RegisterPreLoadSnapshotCallback(param => {
    Console.WriteLine($"Загрузка снимка: {param.Type}, AsNew: {param.EntitiesAsNew}");
});

// После загрузки снимка
W.Serializer.RegisterPostLoadSnapshotCallback(param => {
    Console.WriteLine($"Снимок загружен: {param.Type}");
});
```

#### Фильтрация по типу снимка:
```csharp
W.Serializer.RegisterPreCreateSnapshotCallback(param => {
    if (param.Type == SnapshotType.World) {
        Console.WriteLine("Сохранение мира");
    }
});
```

#### Колбеки для каждой сущности:
```csharp
// После сохранения каждой сущности
W.Serializer.RegisterPostCreateSnapshotEachEntityCallback((entity, param) => {
    Console.WriteLine($"Сохранена: {entity.PrettyString}");
});

// После загрузки каждой сущности
W.Serializer.RegisterPostLoadSnapshotEachEntityCallback((entity, param) => {
    Console.WriteLine($"Загружена: {entity.PrettyString}");
});
```

___

## Кастомные данные в снимках

#### Глобальные кастомные данные:
```csharp
// Добавить произвольные данные в снимок (например, данные систем или сервисов)
W.Serializer.SetSnapshotHandler(
    new Guid("57c15483-988a-47e7-919c-51b9a7b957b5"), // уникальный guid типа данных
    version: 0,
    writer: (ref BinaryPackWriter writer, SnapshotWriteParams param) => {
        writer.WriteDateTime(DateTime.Now);
    },
    reader: (ref BinaryPackReader reader, ushort version, SnapshotReadParams param) => {
        var savedTime = reader.ReadDateTime();
        Console.WriteLine($"Время сохранения: {savedTime}");
    }
);
```

#### Кастомные данные для каждой сущности:
```csharp
W.Serializer.SetSnapshotHandlerEachEntity(
    new Guid("68d26594-1a9b-48f8-b2de-71c0a8b068c6"),
    version: 0,
    writer: (ref BinaryPackWriter writer, W.Entity entity, SnapshotWriteParams param) => {
        // Записать дополнительные данные для сущности
    },
    reader: (ref BinaryPackReader reader, W.Entity entity, ushort version, SnapshotReadParams param) => {
        // Прочитать дополнительные данные для сущности
    }
);
```

___

## Сериализация событий

```csharp
// Сохранить события
byte[] eventsSnapshot = W.Serializer.CreateEventsSnapshot();

// С GZIP сжатием
byte[] eventsCompressed = W.Serializer.CreateEventsSnapshot(gzip: true);

// В файл
W.Serializer.CreateEventsSnapshot("path/to/events.bin");

// Загрузить события
W.Serializer.LoadEventsSnapshot(eventsSnapshot);

// Из файла
W.Serializer.LoadEventsSnapshot("path/to/events.bin");
```

{: .noteru }
При использовании `CreateWorldSnapshot` события сохраняются автоматически (если не указано `writeEvents: false`). Отдельная сериализация событий нужна при использовании `EntitiesSnapshot`.

___

## Исключение из сериализации

```csharp
// Компоненты, теги и события без Guid пропускаются при сериализации через EntitiesSnapshot
W.Types()
    .Component<DebugInfo>()               // без guid — не сериализуется
    .Tag<EditorOnly>();                   // без guid — не сериализуется

// Для сохранения мира (CreateWorldSnapshot) требуются все Guid — иначе ошибка в DEBUG
// Для сохранения сущностей (EntitiesSnapshot) типы без Guid просто пропускаются

// Пример: сохранение всех сущностей с пропуском отладочных данных
using var writer = W.Serializer.CreateEntitiesSnapshotWriter();
writer.WriteAllEntities();
byte[] snapshot = writer.CreateSnapshot();
byte[] gidSnapshot = W.Serializer.CreateGIDStoreSnapshot();
byte[] eventsSnapshot = W.Serializer.CreateEventsSnapshot();
```

___

## Сжатие (GZIP)

Все методы создания и загрузки снимков поддерживают GZIP сжатие:

```csharp
// Мир
byte[] snapshot = W.Serializer.CreateWorldSnapshot(gzip: true);
W.Serializer.LoadWorldSnapshot(snapshot, gzip: true);

// Кластер
byte[] cluster = W.Serializer.CreateClusterSnapshot(1, gzip: true);
W.Serializer.LoadClusterSnapshot(cluster, gzip: true);

// Чанк
byte[] chunk = W.Serializer.CreateChunkSnapshot(0, gzip: true);
W.Serializer.LoadChunkSnapshot(chunk, gzip: true);

// GID Store
byte[] gid = W.Serializer.CreateGIDStoreSnapshot(gzip: true);

// События
byte[] events = W.Serializer.CreateEventsSnapshot(gzip: true);
W.Serializer.LoadEventsSnapshot(events, gzip: true);

// Файлы
W.Serializer.CreateWorldSnapshot("world.bin", gzip: true);
W.Serializer.LoadWorldSnapshot("world.bin", gzip: true);
```
