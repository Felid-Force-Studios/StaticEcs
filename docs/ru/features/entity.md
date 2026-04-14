---
title: Сущность
parent: Возможности
nav_order: 1
---

## Entity
Сущность — структура для идентификации объекта в мире и доступа к его компонентам и тегам
- Структура размером 4 байта (`uint`-обёртка над индексом слота)
- Не содержит счётчика поколений — для устойчивых ссылок используйте [EntityGID](gid.md)
- Все операции с компонентами и тегами доступны через методы на самой сущности

___

## Тип сущности (IEntityType)

Тип сущности — логическая категория, назначаемая при создании. Определяет назначение сущности (юниты, снаряды, эффекты) и управляет размещением в памяти — сущности одного типа внутри кластера хранятся в одних сегментах.

### Определение типов сущностей

Типы сущностей определяются как структуры, реализующие `IEntityType`, с методом `byte Id()`:

```csharp
public struct Bullet : IEntityType {
    public byte Id() => 1;
}

public struct Enemy : IEntityType {
    public byte Id() => 2;
}

public struct Effect : IEntityType {
    public byte Id() => 3;
}
```

Встроенный тип `Default` (Id = 0) регистрируется автоматически при создании мира.

### Регистрация

Типы сущностей регистрируются в фазе `Created`.

**Ручная регистрация:**
```csharp
W.Types()
    .EntityType<Bullet>()
    .EntityType<Enemy>()
    .EntityType<Effect>();
```

**Авторегистрация:**
```csharp
W.Types().RegisterAll();
```

`RegisterAll()` находит все типы, реализующие `IEntityType`, в указанных сборках (по умолчанию — вызывающая сборка) и регистрирует их автоматически. Идентификатор получается из метода `Id()`.

### Хуки жизненного цикла (OnCreate / OnDestroy)

Типы сущностей могут определять хуки `OnCreate` и `OnDestroy`. Если метод не определён в структуре — он не вызывается. Не нужно оставлять пустые реализации.

```csharp
public struct Bullet : IEntityType {
    public byte Id() => 1;

    public void OnCreate<TWorld>(World<TWorld>.Entity entity) where TWorld : struct, IWorldType {
        entity.Set(new Velocity { Speed = 100 });
        entity.Set<Active>();
    }

    public void OnDestroy<TWorld>(World<TWorld>.Entity entity, HookReason reason) where TWorld : struct, IWorldType {
        // Логика очистки, отправка событий и т.д.
        // Все компоненты и теги ещё доступны.
    }
}
```

### Типы сущностей с данными

Поскольку `IEntityType` — struct, он может содержать поля. `OnCreate` — instance-метод с доступом к полям через `this`. Это позволяет параметризировать создание без дополнительных аргументов и аллокаций:

```csharp
public struct Flora : IEntityType {
    public byte Id() => 4;

    public enum Kind : byte { Grass, Bush, Tree }
    public Kind FloraKind;

    public void OnCreate<TWorld>(World<TWorld>.Entity entity) where TWorld : struct, IWorldType {
        entity.Set(new Health { Value = FloraKind == Kind.Tree ? 100 : 10 });
    }
}

// Использование:
var tree = W.NewEntity(new Flora { FloraKind = Flora.Kind.Tree });
var grass = W.NewEntity(new Flora { FloraKind = Flora.Kind.Grass });
```

### Зачем это нужно

**Локальность данных при итерации.** Компоненты хранятся в SoA-массивах. Когда сущности одного типа расположены в одном сегменте, их компоненты лежат рядом в памяти — процессор эффективно использует кэш-линии.

**Уменьшение фрагментации.** Без типизации сущности разных видов создавались бы вперемешку. С типизацией дыры от уничтоженных снарядов заполняются новыми снарядами — сегмент остаётся однородным.

**Фильтрация в запросах.** Фильтры по типу сущности (`EntityIs<T>`, `EntityIsNot<T>`, `EntityIsAny<T0,T1>`) работают на уровне сегментов с нулевой стоимостью per-entity — `FilterEntities` является no-op. Это самый дешёвый тип фильтра в системе.

### entityType и clusterId

Эти два параметра дополняют друг друга:

- **`entityType`** — **логическая** группировка: определяет *что* это за сущность (юнит, снаряд, эффект). Влияет на размещение в памяти — сущности одного типа хранятся вместе для оптимальной итерации.
- **`clusterId`** — **пространственная** группировка: определяет *где* находится сущность (уровень, зона карты, комната). Позволяет ограничивать запросы конкретными областями мира и управлять стримингом.

Сегментация работает на пересечении этих параметров: внутри каждого кластера для каждого типа выделяются отдельные сегменты.

___

## Создание

```csharp
// Создание сущности с типом по умолчанию (Id = 0)
W.Entity entity = W.NewEntity<Default>();

// С конкретным типом — OnCreate хук вызывается автоматически
W.Entity entity = W.NewEntity<Bullet>();
W.Entity entity = W.NewEntity<Enemy>(clusterId: LEVEL_1_CLUSTER);

// С данными в структуре типа сущности
W.Entity entity = W.NewEntity(new Flora { FloraKind = Flora.Kind.Tree });

// С компонентами — Set возвращает Entity, можно использовать как цепочку
W.Entity entity = W.NewEntity<Bullet>().Set(new Position { Value = Vector3.One });
W.Entity entity = W.NewEntity<Bullet>().Set(
    new Position { Value = Vector3.One },
    new Velocity { Value = 10f },
    new Damage { Value = 5 }
);

// Создание в конкретном чанке
W.Entity entity = W.NewEntityInChunk<Bullet>(chunkIdx: chunkIdx);

// Создание по GID (для десериализации и сетевой синхронизации)
W.Entity entity = W.NewEntityByGID<Default>(gid);

// Не-дженерик перегрузки (тип сущности известен только в runtime, например при десериализации)
byte entityTypeId = EntityTypeInfo<Bullet>.Id;
W.Entity entity = W.NewEntity(entityTypeId, clusterId: LEVEL_1_CLUSTER);
W.Entity entity = W.NewEntityInChunk(entityTypeId, chunkIdx: chunkIdx);
W.Entity entity = W.NewEntityByGID(entityTypeId, gid);
```

#### Создание в зависимом мире (Try):

{: .noteru }
Зависимый мир (`Independent = false`) делит пространство слотов с другими мирами. Если выделенные ему слоты исчерпаны, создание сущности невозможно.

```csharp
// Вернёт false, если в зависимом мире закончились выделенные слоты
if (W.TryNewEntity<Bullet>(out var entity, clusterId: LEVEL_1_CLUSTER)) {
    entity.Set(new Position { Value = Vector3.Zero });
}
```

___

## Массовое создание

```csharp
uint count = 1000;

// Без компонентов
W.NewEntities<Default>(count);

// С компонентами по типу (от 1 до 5 типов)
W.NewEntities<Default, Position>(count);
W.NewEntities<Default, Position, Velocity>(count);

// С компонентами по значению (от 1 до 8 компонентов)
W.NewEntities<Default>(count, new Position { Value = Vector3.Zero });
W.NewEntities<Default>(count,
    new Position { Value = Vector3.Zero },
    new Velocity { Value = 1f }
);

// С делегатом инициализации каждой сущности
W.NewEntities<Default, Position>(count, onCreate: static entity => {
    entity.Set<Unit>();
});

// С кластером
W.NewEntities<Default, Position>(count, clusterId: LEVEL_1_CLUSTER);

// Полная перегрузка: значения + кластер + делегат
W.NewEntities<Default>(count,
    new Position { Value = Vector3.Zero },
    clusterId: LEVEL_1_CLUSTER,
    onCreate: static entity => {
        entity.Set<Unit>();
    }
);
```

___

## Свойства

```csharp
W.Entity entity = W.NewEntity<Bullet>();

uint id = entity.ID;                         // Внутренний индекс слота
EntityGID gid = entity.GID;                  // Глобальный идентификатор (8 байт)
EntityGIDCompact gidC = entity.GIDCompact;   // Компактный идентификатор (4 байта)
ushort version = entity.Version;             // Счётчик поколений слота
ushort clusterId = entity.ClusterId;         // Идентификатор кластера
byte entityType = entity.EntityType;         // Тип сущности (0–255)
uint chunkId = entity.ChunkID;              // Индекс чанка

bool alive = entity.IsNotDestroyed;          // Не уничтожена
bool destroyed = entity.IsDestroyed;         // Уничтожена
bool enabled = entity.IsEnabled;             // Включена (участвует в запросах)
bool disabled = entity.IsDisabled;           // Отключена
bool selfOwned = entity.IsSelfOwned;         // Сегмент принадлежит этому миру

// Проверки типа сущности
bool isBullet = entity.Is<Bullet>();                    // Точное совпадение типа
bool isProjectile = entity.IsAny<Bullet, Rocket>();     // Любой из типов
bool isNotEffect = entity.IsNot<Effect>();              // Не этот тип
bool isNotVfx = entity.IsNot<Effect, Particle>();       // Ни один из указанных

string info = entity.PrettyString;           // Отладочная строка
```

___

## Жизненный цикл

```csharp
// Отключить сущность — исключается из стандартных запросов, но сохраняет все данные
entity.Disable();

// Включить обратно
entity.Enable();

// Уничтожить — удаляет все компоненты (с вызовом OnDelete), теги, освобождает слот
// Возвращает bool: true если сущность была жива и уничтожена, false если уже уничтожена
entity.Destroy();

// Выгрузить из памяти — сущность становится невидимой, но её ID сохраняется
// Используется для стриминга (временная выгрузка с последующей загрузкой через сериализацию)
entity.Unload();

// Инкрементировать версию без уничтожения — все ранее полученные GID станут невалидными
entity.UpVersion();
```

___

## Клонирование и перенос

```csharp
// Клонировать сущность — создаёт новую сущность с копией всех компонентов и тегов
W.Entity clone = entity.Clone();

// Клонировать в другой кластер
W.Entity clone = entity.Clone(clusterId: OTHER_CLUSTER);

// Копировать все компоненты и теги в существующую сущность
// Если у целевой сущности уже есть совпадающие компоненты — они перезаписываются
entity.CopyTo(targetEntity);

// Перенести все данные в существующую сущность и уничтожить исходную
entity.MoveTo(targetEntity);

// Перенести в другой кластер — создаёт новую сущность, копирует данные, уничтожает исходную
W.Entity moved = entity.MoveTo(clusterId: OTHER_CLUSTER);
```

___

## Компоненты

API работы с компонентами описан в разделе [Компоненты](component.md).

___

## Теги

API работы с тегами описан в разделе [Теги](tag.md).

___

## Мульти-компоненты

API мульти-компонентов описан в разделе [Мульти-компоненты](multicomponent.md).

___

## Отношения

API отношений между сущностями описан в разделе [Отношения](relations.md).

___

## Отладка

```csharp
// Отладочная строка с полной информацией
string info = entity.PrettyString;

// Количество компонентов (включённых + отключённых)
int compCount = entity.ComponentsCount();

// Количество тегов
int tagCount = entity.TagsCount();

// Получить все компоненты (список очищается перед заполнением)
var components = new List<IComponent>();
entity.GetAllComponents(components);

// Получить все теги (список очищается перед заполнением)
var tags = new List<ITag>();
entity.GetAllTags(tags);
```

___

## Фильтры запросов по типу сущности

Фильтры по типу сущности описаны в разделе [Запросы — Типы сущностей](query.md#типы-сущностей).

___

## Операторы и преобразования

```csharp
W.Entity a = W.NewEntity<Default>();
W.Entity b = W.NewEntity<Default>();

// Сравнение по индексу слота (без проверки версии)
bool eq = a == b;
bool neq = a != b;

// Неявное преобразование Entity → EntityGID (8 байт)
EntityGID gid = entity;

// Явное преобразование Entity → EntityGIDCompact (4 байта)
// В DEBUG бросит ошибку если Chunk >= 4 или ClusterId >= 4
EntityGIDCompact compact = (EntityGIDCompact)entity;

// Преобразование в типизированную связь (для системы отношений)
Link<ChildOf> link = entity.AsLink<ChildOf>();
```
