---
title: Отслеживание изменений
parent: Возможности
nav_order: 13
---

## Отслеживание изменений

StaticEcs предоставляет четыре типа отслеживания изменений, все без аллокаций и включаются явно:

| Тип | Что отслеживает | Область | Где включается |
|-----|----------------|---------|----------------|
| **Added** | Добавление компонента/тега | Компоненты, теги | `ComponentTypeConfig` / `TagTypeConfig` |
| **Deleted** | Удаление компонента/тега | Компоненты, теги | `ComponentTypeConfig` / `TagTypeConfig` |
| **Changed** | Доступ к данным компонента через `ref` | Только компоненты | `ComponentTypeConfig` |
| **Created** | Создание сущности | Весь мир | `WorldConfig.TrackCreated` |

- Bitmap-хранение: один `ulong` на 64 сущности для каждого отслеживаемого типа
- Трекинг версионируется по тикам мира через кольцевой буфер (по умолчанию 8 тиков). Каждая система автоматически видит изменения с момента своего последнего запуска
- Нулевые накладные расходы для типов с выключенным трекингом
- Нулевые накладные расходы для `Created` при `WorldConfig.TrackCreated = false`

___

## Конфигурация

Весь трекинг выключен по умолчанию и должен быть явно включён при регистрации типов.

### Компоненты

`ComponentTypeConfig<T>` поддерживает три флага трекинга: `trackAdded`, `trackDeleted`, `trackChanged`:

```csharp
W.Create(WorldConfig.Default());
//...
// Включить все три типа трекинга
W.Types().Component<Health>(new ComponentTypeConfig<Health>(
    trackAdded: true,
    trackDeleted: true,
    trackChanged: true
));

// Включить только одно направление
W.Types().Component<Velocity>(new ComponentTypeConfig<Velocity>(
    trackAdded: true  // отслеживать только добавление
));

// Полная конфигурация с трекингом
W.Types().Component<Position>(new ComponentTypeConfig<Position>(
    guid: new Guid("..."),
    defaultValue: default,
    trackAdded: true,
    trackDeleted: true,
    trackChanged: true
));
//...
W.Initialize();
```

### Теги

`TagTypeConfig<T>` поддерживает `trackAdded` и `trackDeleted`. Теги **не** поддерживают Changed-трекинг.

```csharp
W.Types().Tag<Unit>(new TagTypeConfig<Unit>(
    trackAdded: true,
    trackDeleted: true
));

// С GUID для сериализации
W.Types().Tag<Poisoned>(new TagTypeConfig<Poisoned>(
    guid: new Guid("A1B2C3D4-..."),
    trackAdded: true,
    trackDeleted: true
));
```

### Создание сущностей

Отслеживание создания сущностей настраивается на **уровне мира** через `WorldConfig`:

```csharp
W.Create(new WorldConfig {
    TrackCreated = true,
    // ...другие настройки...
});
//...
W.Initialize();
```

{: .noteru }
`Created` отслеживает создание всех сущностей независимо от типа. Для фильтрации по типу комбинируйте с `EntityIs<T>`: `W.Query<Created, EntityIs<Bullet>>()`.

### Авто-регистрация

Параметры `trackAdded`, `trackDeleted` и `trackChanged` можно объявить в статическом поле `Config` внутри структуры — `RegisterAll()` подхватит их автоматически.

### Отключение на этапе компиляции

Директива `FFS_ECS_DISABLE_CHANGED_TRACKING` удаляет все пути кода Changed-трекинга на этапе компиляции, включая фильтры `AllChanged<T>`, `NoneChanged<T>`, `AnyChanged<T>` и метод `Mut<T>()`.

### Tick-Based трекинг

`WorldConfig.TrackingBufferSize` задаёт глубину кольцевого буфера (по умолчанию 8 тиков). Вызывайте `W.Tick()` для продвижения тика и ротации буфера.

```csharp
// По умолчанию: трекинг с историей 8 тиков
W.Create(WorldConfig.Default()); // TrackingBufferSize = 8

// Пользовательский размер буфера
W.Create(new WorldConfig {
    TrackingBufferSize = 16,   // 16 тиков истории
    // ...другие настройки...
});
```

#### Выбор размера буфера

Буфер должен вмещать историю трекинга для самой редкой системы, которая использует фильтры трекинга. Если `W.Tick()` вызывается на 60fps, а некоторые системы работают на 20fps, они пропускают 2 тика между запусками и должны заглянуть на 3 тика назад.

**Формула:** `TrackingBufferSize >= tickRate / slowestSystemRate`

| Частота тиков | Самая редкая система | Мин. буфер |
|---------------|---------------------|------------|
| 60 fps | 60 fps (каждый тик) | 1 |
| 60 fps | 20 fps (каждый 3-й тик) | 3 |
| 60 fps | 10 fps (каждый 6-й тик) | 6 |
| 60 fps | 1 fps (каждый 60-й тик) | 60 |

Если системы используют интервалы реального времени вместо счётчика тиков, FPS выше ожидаемого увеличит количество тиков между запусками — берите запас. Значение по умолчанию 8 покрывает большинство игр, где самая редкая система с трекингом работает на ~20fps или быстрее.

___

## Tick-Based трекинг

Tick-based трекинг решает две распространённые проблемы:
1. Системы в середине конвейера вносят изменения, которые системы в начале не видят в следующем кадре — если очистка трекинга происходит в конце кадра
2. Разные группы систем (Update / FixedUpdate) не могут синхронизировать трекинг — очистка в одной группе затрагивает другую

### Как это работает

- Каждая система в `W.Systems<T>.Update()` автоматически получает свой `LastTick` — она видит все изменения в диапазоне тиков `(LastTick, CurrentTick]` — изменения, сделанные в текущем кадре, становятся видимыми в следующем кадре
- Когда система завершается, её `LastTick` устанавливается в `CurrentTick`
- Если система пропущена (`UpdateIsActive() = false`), её `LastTick` НЕ обновляется — при следующем запуске она увидит все накопленные изменения
- `W.Tick()` продвигает глобальный счётчик тиков и ротирует кольцевой буфер — слот записи становится доступной историей, новый слот очищается и становится целью записи для всех операций трекинга

### Интеграция в игровой цикл

```csharp
// Одна группа систем
while (running) {
    W.Systems<GameLoop>.Update();    // каждая система видит изменения с её LastTick
    W.Tick();                      // продвинуть тик, ротировать буфер
}

// Несколько групп систем (например, Update + FixedUpdate)
while (running) {
    W.Systems<Update>.Update();

    // FixedUpdate может выполняться несколько раз за кадр — всё в одном тике
    while (fixedTimeAccumulator >= fixedDeltaTime) {
        W.Systems<FixedUpdate>.Update();
        fixedTimeAccumulator -= fixedDeltaTime;
    }
    
    W.Tick();                      // один тик за кадр
}
```

{: .importantru }
Вызывайте `W.Tick()` **один раз за кадр после** самой быстрой группы систем. Не вызывайте `Tick()` после каждой группы — это расходует слоты впустую. Per-system `LastTick` обеспечивает автоматическое накопление изменений за несколько тиков для редких систем. Изменения, сделанные в течение кадра, становятся видимыми в следующем кадре.

### Задержка в один кадр

Изменения трекинга записываются в отдельный слот записи, отделённый от читаемой истории. При вызове `W.Tick()` слот записи становится частью истории. Поэтому каждая система видит изменения, сделанные **после её предыдущего запуска и до текущего кадра** — но не изменения текущего кадра.

Рассмотрим конвейер из 5 систем, где Sys1 и Sys5 изменяют `Position`, а Sys3 запрашивает `AllChanged<Position>`:

```
Кадр 1:
  Sys1  → меняет Position   (записывается в слот записи)
  Sys3  → запрашивает трекинг → видит НИЧЕГО (история пуста, первый кадр)
  Sys5  → меняет Position   (записывается в тот же слот записи)
  Tick()                     → слот записи становится history[тик 1]

Кадр 2:
  Sys1  → меняет Position   (записывается в новый слот записи)
  Sys3  → запрашивает трекинг → видит history[тик 1] = Sys1 + Sys5 из кадра 1
  Sys5  → меняет Position   (записывается в тот же слот записи)
  Tick()                     → слот записи становится history[тик 2]

Кадр 3:
  Sys3  → запрашивает трекинг → видит history[тик 2] = Sys1 + Sys5 из кадра 2
```

Каждый кадр Sys3 видит **ровно** изменения из предыдущего кадра — как от систем до неё (Sys1), так и после неё (Sys5). Без повторной обработки, без пропусков.

### Per-System трекинг тиков

Каждая система хранит свой `LastTick`. Системы, запускающиеся каждый тик, видят изменения ровно за 1 тик. Системы, пропускающие кадры, видят все накопленные изменения с момента последнего запуска:

```csharp
public struct RareSystem : ISystem {
    private int _counter;

    public bool UpdateIsActive() => ++_counter % 5 == 0; // запускается каждые 5 тиков

    public void Update() {
        // Видит ВСЕ изменения за последние 5 тиков (или до TrackingBufferSize)
        foreach (var entity in W.Query<All<Position>, AllAdded<Position>>().Entities()) {
            // обработка добавленных позиций за последние 5 тиков
        }
    }
}
```

### Пользовательский диапазон тиков (FromTick)

Все фильтры трекинга принимают опциональный параметр `fromTick` в конструкторе для переопределения автоматического диапазона:

```csharp
// Автоматический — использует LastTick системы (по умолчанию, конструктор не нужен):
foreach (var entity in W.Query<All<Position>, AllAdded<Position>>().Entities()) { }

// Ручной — видит все изменения начиная с тика 5 до текущего:
var filter = new AllAdded<Position>(fromTick: 5);
foreach (var entity in W.Query<All<Position>>(filter).Entities()) { }
```

- `fromTick = 0` (по умолчанию): автоматический диапазон из `CurrentLastTick` (устанавливается `W.Systems<T>.Update()`)
- `fromTick > 0`: ручная нижняя граница — видит изменения с этого тика до текущего

### Синхронизация между группами

С tick-based трекингом разные группы систем работают вместе естественным образом:

```csharp
W.Systems<Update>.Update();          // системы записывают трекинг в тик N
W.Systems<FixedUpdate>.Update();     // системы видят изменения Update из тика N + старые тики
W.Tick();                          // продвинуть к тику N+1
```

`LastTick` каждой системы независим. Система FixedUpdate, пропускающая кадры, увидит все накопленные изменения из предыдущих тиков с момента своего последнего запуска.

### Переполнение буфера

Если система не запускается дольше, чем `TrackingBufferSize` тиков, самые старые данные трекинга перезаписываются. Система увидит максимум `TrackingBufferSize` тиков истории.

{: .warning }
В debug-режиме (`FFS_ECS_DEBUG`) выбрасывается `StaticEcsException` когда диапазон тиков системы превышает размер буфера. В release-режиме диапазон молча обрезается. Увеличьте `WorldConfig.TrackingBufferSize` если вашим системам нужна более глубокая история.

___

## Фильтры запросов

Все фильтры трекинга используются аналогично стандартным фильтрам компонентов и тегов:

| Категория | Фильтр | Параметры | Описание |
|-----------|--------|-----------|----------|
| **Компоненты Added** | `AllAdded<T0..T4>` | 1–5 | ВСЕ указанные компоненты были добавлены |
| | `NoneAdded<T0..T4>` | 1–5 | Исключает сущности, у которых ХОТЬ ОДИН был добавлен |
| | `AnyAdded<T0..T4>` | 2–5 | ХОТЯ БЫ ОДИН был добавлен |
| **Компоненты Deleted** | `AllDeleted<T0..T4>` | 1–5 | ВСЕ указанные компоненты были удалены |
| | `NoneDeleted<T0..T4>` | 1–5 | Исключает сущности, у которых ХОТЬ ОДИН был удалён |
| | `AnyDeleted<T0..T4>` | 2–5 | ХОТЯ БЫ ОДИН был удалён |
| **Компоненты Changed** | `AllChanged<T0..T4>` | 1–5 | ВСЕ указанные компоненты были получены через `ref` |
| | `NoneChanged<T0..T4>` | 1–5 | Исключает сущности, у которых ХОТЬ ОДИН был изменён |
| | `AnyChanged<T0..T4>` | 2–5 | ХОТЯ БЫ ОДИН был получен через `ref` |
| **Сущности** | `Created` | — | Сущность была создана (требует `WorldConfig.TrackCreated`) |

{: .noteru }
Фильтры `AllAdded`, `NoneAdded`, `AnyAdded`, `AllDeleted`, `NoneDeleted`, `AnyDeleted` работают и с компонентами, и с тегами. Отдельных типов фильтров для тегов нет.

### Примеры

```csharp
// Сущности, которым добавлен Position и он сейчас есть
foreach (var entity in W.Query<All<Position>, AllAdded<Position>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
}

// Сущности, которым добавлены И Position, И Velocity
foreach (var entity in W.Query<AllAdded<Position, Velocity>>().Entities()) { }

// Хотя бы один из Position или Velocity был добавлен
foreach (var entity in W.Query<AnyAdded<Position, Velocity>>().Entities()) { }

// Реакция на установку тега
foreach (var entity in W.Query<AllAdded<IsDead>>().Entities()) { }

// Хотя бы один из указанных тегов был установлен
foreach (var entity in W.Query<AnyAdded<Poisoned, Stunned>>().Entities()) { }

// Обработать сущности с изменённым Position (через ref)
foreach (var entity in W.Query<All<Position>, AllChanged<Position>>().Entities()) {
    ref readonly var pos = ref entity.Read<Position>();
}

// Только реально изменённые, исключая новые
foreach (var entity in W.Query<All<Position>, AllChanged<Position>, NoneAdded<Position>>().Entities()) {
    ref readonly var pos = ref entity.Read<Position>();
}

// Обработать недавно созданные сущности с Position
foreach (var entity in W.Query<Created, All<Position>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
}

// Группировка фильтров через And
var filter = default(And<AllAdded<Position, Unit>, AllDeleted<Velocity>>);
foreach (var entity in W.Query(filter).Entities()) { }
```

___

## Семантика

### Added / Deleted

{: .importantru }
**`AllAdded<T>` означает только факт добавления — НЕ гарантирует что компонент сейчас присутствует!** Если компонент был добавлен, а затем удалён в том же кадре — он по-прежнему отмечен как Added, но компонента уже нет. Аналогично, `AllDeleted<T>` означает факт удаления — но компонент мог быть добавлен снова.

**Рекомендуемые комбинации:**
```csharp
// "Добавлен И сейчас присутствует" — РЕКОМЕНДУЕМЫЙ паттерн
foreach (var entity in W.Query<All<Position>, AllAdded<Position>>().Entities()) {
    ref var pos = ref entity.Ref<Position>(); // безопасно — All<Position> гарантирует наличие
}

// "Удалён И сейчас отсутствует"
foreach (var entity in W.Query<None<Position>, AllDeleted<Position>>().Entities()) {
    // сущность жива, Position удалён — можно очистить ресурсы
}

// Только AllAdded<Position> — без гарантии наличия!
foreach (var entity in W.Query<AllAdded<Position>>().Entities()) {
    // ОСТОРОЖНО: компонент мог быть уже удалён!
    if (entity.Has<Position>()) {
        ref var pos = ref entity.Ref<Position>();
    }
}
```

### Changed (пессимистичная модель)

Changed-трекинг использует модель **dirty-on-access**: любое получение `ref`-ссылки помечает компонент как Changed, независимо от того, были ли данные реально изменены. Это сделано намеренно — проверка реальных изменений на уровне полей была бы слишком дорогой для высокопроизводительного ECS.

#### Методы доступа к данным

| Метод | Возвращает | Changed | Added | Примечание |
|-------|-----------|:---:|:---:|-----------|
| `Ref<T>()` | `ref T` | — | — | Быстрый мутабельный доступ, без трекинга |
| `Mut<T>()` | `ref T` | Да | — | Мутабельный доступ с трекингом |
| `Read<T>()` | `ref readonly T` | — | — | Только чтение |
| `Add<T>()` (новый) | `ref T` | Да | Да | Компонент новый |
| `Add<T>()` (существующий) | `ref T` | — | — | Возвращает ссылку на существующий, без хуков |
| `Set(value)` (новый) | void | Да | Да | Компонент новый |
| `Set(value)` (существующий) | void | Да | — | Перезаписывает существующий |

{: .importantru }
**`Ref<T>()` НЕ помечает Changed.** Используйте `Mut<T>()` когда нужен трекинг изменений. `Ref<T>()` — самый быстрый способ доступа к данным компонента — нулевые накладные расходы, без ветвлений. `Read<T>()` — для доступа только на чтение. В итерации запросов через делегаты (`For`, `ForBlock`) параметры `ref` автоматически используют отслеживаемый доступ (семантика `Mut`), параметры `in` — только чтение (семантика `Read`).

#### Авто-трекинг в запросах

Итерация запросов автоматически помечает Changed в зависимости от семантики доступа:

**Делегаты For** — `ref` помечает Changed, `in` — нет:
```csharp
// Position помечается как Changed (ref), Velocity — нет (in)
W.Query<All<Position, Velocity>>().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});
```

**Структуры IQuery** — `Write<T>` помечает Changed, `Read<T>` — нет:
```csharp
public struct MoveSystem : IQuery.Write<Position>.Read<Velocity> {
    public void Invoke(Entity entity, ref Position pos, in Velocity vel) {
        pos.Value += vel.Value;
    }
}
```

**ForBlock** — `Block<T>` (мутабельный) помечает Changed, `BlockR<T>` (readonly) — нет:
```csharp
public struct MoveBlockSystem : IQueryBlock.Write<Position>.Read<Velocity> {
    public void Invoke(uint count, EntityBlock entities, Block<Position> pos, BlockR<Velocity> vel) {
        // обработка блока
    }
}
```

Параллельные запросы следуют тем же правилам.

#### Взаимодействие Changed + Added

{: .importantru }
При добавлении компонента через `Add<T>()` или `Set(value)` он помечается ОДНОВРЕМЕННО как Added И Changed. Чтобы обрабатывать только реально изменённые сущности (без новых), комбинируйте `AllChanged<T>` с `NoneAdded<T>`:

```csharp
foreach (var entity in W.Query<All<Position>, AllChanged<Position>, NoneAdded<Position>>().Entities()) {
    // только изменённые, не новые
}
```

### Created

`Created` отслеживает факт создания сущностей глобально. Не несёт информации о типе — для фильтрации по типу комбинируйте с `EntityIs<T>`:

```csharp
foreach (var entity in W.Query<Created, EntityIs<Bullet>, All<Position>>().Entities()) {
    // только что созданные пули с Position
}
```

___

## Граничные случаи

{: .importantru }
Состояния Added и Deleted **независимы** и **не отменяют друг друга**. Они фиксируют все операции, произошедшие в течение текущего тика. Changed также независим от обоих.

### Добавление → Удаление
```csharp
entity.Set(new Position { X = 10 });   // Added = 1
entity.Delete<Position>();              // Deleted = 1, Added остаётся

// Результат: компонента нет, но Added=1 и Deleted=1
// Query<AllAdded<Position>>                    -> находит
// Query<AllDeleted<Position>>                  -> находит
// Query<All<Position>, AllAdded<Position>>     -> НЕ находит (компонента нет)
// Query<None<Position>, AllDeleted<Position>>  -> находит
```

### Удаление → Добавление
```csharp
entity.Delete<Weapon>();                // Deleted = 1
entity.Set(new Weapon { Damage = 50 }); // Added = 1, Deleted остаётся

// Результат: компонент есть, Added=1 и Deleted=1
// Query<All<Weapon>, AllAdded<Weapon>>   -> находит
// Query<All<Weapon>, AllDeleted<Weapon>> -> находит
```

### Добавление → Удаление → Добавление
```csharp
entity.Set(new Health { Value = 100 }); // Added = 1
entity.Delete<Health>();                // Deleted = 1
entity.Set(new Health { Value = 50 });  // Added уже отмечен

// Результат: компонент есть (Value = 50), Added=1 и Deleted=1
// Эквивалентно «Удаление → Добавление» с точки зрения трекинга
```

### Множественные добавления (идемпотентность)
```csharp
// Add без значения — не перезаписывает существующий компонент
entity.Add<Position>();                 // Added = 1 (новый компонент)
entity.Add<Position>();                 // Added уже отмечен, без изменений
// Added отмечается только при первом добавлении (когда компонент новый)

// Set с значением — ВСЕГДА перезаписывает
entity.Set(new Position { X = 10 });    // Added = 1 (новый)
entity.Set(new Position { X = 20 });    // перезапись, Added не отмечается повторно
                                         // (компонент уже существовал)
```

### Mut без модификации
```csharp
ref var pos = ref entity.Mut<Position>(); // ПОМЕЧЕН как Changed, даже если запись не последует!
// Changed-трекинг пессимистичный — отслеживает доступ, а не реальные мутации
// Используйте entity.Ref<Position>() если трекинг не нужен — нулевые накладные расходы
```

### Множественные вызовы Mut
```csharp
entity.Mut<Position>(); // помечен
entity.Mut<Position>(); // уже помечен, без дополнительных расходов
// Changed-бит идемпотентен
```

### Итерация запроса помечает все итерируемые сущности
```csharp
// ВСЕ сущности, подходящие под запрос, получают Changed для ref-компонентов,
// даже если делегат реально не модифицирует данные
W.Query<All<Position>>().For(static (ref Position pos) => {
    var x = pos.X; // помечен Changed из-за `ref`, хотя мы только читаем
});

// Используйте `in` чтобы избежать этого:
W.Query<All<Position>>().For(static (in Position pos) => {
    var x = pos.X; // НЕ помечен как Changed
});
```

### Changed и Deleted независимы
Changed и Deleted — независимые биты. Если компонент был получен через `ref`, а затем удалён в том же кадре, оба бита будут установлены.

___

## Destroy и десериализация

### Destroy

`entity.Destroy()` удаляет все компоненты/теги — они отмечаются как Deleted. Но сущность мертва, поэтому маска alive отфильтрует её из ВСЕХ запросов. Следовательно, `AllDeleted<T>` **не** найдёт уничтоженные сущности.

```csharp
var entity = W.Entity.New<Position, Velocity>();
entity.Destroy();
// Query<AllDeleted<Position>> -> НЕ находит (сущность мертва)

// Если нужно реагировать на уничтожение — удаляйте компоненты явно перед Destroy:
entity.Delete<Position>();  // Deleted = 1, сущность жива
// ... обработка AllDeleted<Position> ...
entity.Destroy();
```

### После десериализации

```csharp
// ReadChunk записывает маски напрямую — трекинг НЕ срабатывает
// ReadEntity проходит через Add — компоненты отмечаются как Added
// Рекомендуется вызвать ClearTracking() после загрузки:
W.Serializer.ReadChunk(ref reader);
W.ClearTracking(); // сбросить весь трекинг — очищает все слоты кольцевого буфера
```

___

## Сброс отслеживания

{: .important }
Ручная очистка обычно **не нужна** — трекинг управляется автоматически через `W.Tick()` и `W.Systems<T>.Update()`. Методы `ClearTracking()` остаются доступными как «ядерная кнопка», очищающая ВСЕ слоты кольцевого буфера.

```csharp
// === Полный сброс ===
W.ClearTracking();                         // ВСЕ маски (Added + Deleted + Changed + Created)

// === По категориям ===
W.ClearAllTracking();                      // все компоненты и теги (Added + Deleted + Changed)
W.ClearCreatedTracking();                  // Created

// === По виду трекинга (все типы) ===
W.ClearAllAddedTracking();                 // Added для всех компонентов и тегов
W.ClearAllDeletedTracking();               // Deleted для всех компонентов и тегов
W.ClearAllChangedTracking();               // Changed для всех компонентов

// === Для конкретного типа (компоненты и теги) ===
W.ClearTracking<Position>();               // Added + Deleted + Changed для Position
W.ClearAddedTracking<Position>();          // только Added
W.ClearDeletedTracking<Position>();        // только Deleted
W.ClearChangedTracking<Position>();        // только Changed

// Для тегов — те же методы
W.ClearTracking<Unit>();                   // Added + Deleted для Unit
W.ClearAddedTracking<Unit>();              // только Added
W.ClearDeletedTracking<Unit>();            // только Deleted
```

{: .note }
`W.Systems.Update()` → `W.Tick()` → повторить. Ручная очистка не нужна. `ClearTracking()` — только для особых случаев (десериализация, полный сброс).

___

## Проверка состояния

Помимо фильтров запросов, можно проверять состояние трекинга отдельных сущностей:

```csharp
// Компоненты — ALL-семантика (все указанные должны совпадать)
bool wasAdded = entity.HasAdded<Position>();
bool bothAdded = entity.HasAdded<Position, Velocity>();       // Position И Velocity добавлены
bool wasDeleted = entity.HasDeleted<Health>();
bool wasChanged = entity.HasChanged<Position>();
bool bothChanged = entity.HasChanged<Position, Velocity>();   // Position И Velocity изменены

// Компоненты — ANY-семантика (хотя бы один должен совпадать)
bool anyAdded = entity.HasAnyAdded<Position, Velocity>();     // Position ИЛИ Velocity добавлен
bool anyDeleted = entity.HasAnyDeleted<Position, Velocity>(); // Position ИЛИ Velocity удалён
bool anyChanged = entity.HasAnyChanged<Position, Velocity>(); // Position ИЛИ Velocity изменён

// Теги — те же методы (ALL-семантика)
bool tagAdded = entity.HasAdded<Unit>();
bool tagDeleted = entity.HasDeleted<Poisoned>();
bool bothTagsAdded = entity.HasAdded<Unit, Player>();          // Unit И Player добавлены

// Теги — ANY-семантика
bool anyTagAdded = entity.HasAnyAdded<Unit, Player>();         // Unit ИЛИ Player добавлен
bool anyTagDeleted = entity.HasAnyDeleted<Unit, Player>();     // Unit ИЛИ Player удалён

// Комбинирование с проверкой наличия
if (entity.HasAdded<Position>() && entity.Has<Position>()) {
    ref var pos = ref entity.Ref<Position>();
    // компонент добавлен и сейчас присутствует
}

// Все методы принимают опциональный параметр fromTick для указания диапазона тиков:
bool addedSinceTick5 = entity.HasAdded<Position>(fromTick: 5);
bool changedRecently = entity.HasChanged<Position>(fromTick: W.CurrentTick);
```

___

## Производительность

- Маски отслеживания хранятся как `ulong` на блок из 64 сущностей — тот же формат, что маски компонентов/тегов
- Компоненты: до 3 дополнительных `ulong` на блок (Added, Deleted, Changed) для каждого отслеживаемого типа
- Теги: до 2 `ulong` на блок (Added, Deleted)
- `Created`: 1 `ulong` на блок глобально, плюс эвристики на чанк для быстрого пропуска
- Фильтры `AllAdded<T>` / `AllDeleted<T>` / `AllChanged<T>` — та же стоимость, что `All<T>` / `None<T>`: одна побитовая операция на блок
- Changed-трекинг в запросах: одна батчевая OR-операция на блок
- `ClearTracking()` использует эвристики чанков для пропуска пустых регионов — O(занятые чанки), а не O(весь мир)
- `Ref<T>()` имеет нулевые накладные расходы — без runtime-ветвлений, идентичен коду до добавления трекинга
- Нулевые накладные расходы для типов с выключенным трекингом
- Нулевые накладные расходы для `Created` при `WorldConfig.TrackCreated = false`
- `FFS_ECS_DISABLE_CHANGED_TRACKING` убирает все пути кода Changed-трекинга на этапе компиляции
- **Tick-based запись:** нулевые накладные расходы (swap указателей)
- **Tick-based чтение:** O(ticksToCheck) операций OR, ограничено `TrackingBufferSize`. Работает иерархическая фильтрация: сначала на уровне чанков (4096 сущностей), затем на уровне блоков (64 сущности) — проверяются только чанки/блоки с реальными данными трекинга
- **Продвижение тика:** пренебрежимая стоимость за кадр
- **Память:** эвристические массивы × `TrackingBufferSize`; данные сегментов аллоцируются лениво

___

## Сценарии использования

**Сетевая синхронизация (дельта-обновления):**
```csharp
foreach (var entity in W.Query<All<Position>, AllChanged<Position>>().Entities()) {
    ref readonly var pos = ref entity.Read<Position>();
    SendPositionUpdate(entity, pos);
}
```

**Физика:**
```csharp
foreach (var entity in W.Query<All<Transform, PhysicsBody>, AllChanged<Transform>>().Entities()) {
    ref readonly var transform = ref entity.Read<Transform>();
    ref var body = ref entity.Ref<PhysicsBody>();
    SyncPhysicsBody(ref body, transform);
}
```

**Реактивная инициализация:**
```csharp
foreach (var entity in W.Query<All<Position, Unit>, AllAdded<Position>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
    // создание визуального представления для новой сущности
}
```

**Инициализация сущностей:**
```csharp
foreach (var entity in W.Query<Created, All<Position, Unit>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
    // создание визуалов, физического тела и т.д.
}
```

**UI-обновления:**
```csharp
// Создать полоску здоровья для новых сущностей
foreach (var entity in W.Query<All<Health, Player>, AllAdded<Health>>().Entities()) {
    ref var health = ref entity.Ref<Health>();
    // создать UI-элемент
}

// Обновить полоску здоровья только при изменении данных
foreach (var entity in W.Query<All<Health, Player>, AllChanged<Health>>().Entities()) {
    ref readonly var health = ref entity.Read<Health>();
    // обновить отображение
}
```

**Несколько групп систем (tick-based):**
```csharp
void GameLoop() {
    W.Systems<Update>.Update();          // каждая система видит изменения из предыдущих кадров
    W.Systems<FixedUpdate>.Update();     // аналогично — per-system LastTick определяет диапазон
    W.Tick();                          // зафиксировать трекинг текущего кадра в историю
}
```

**Условные системы (tick-based):**
```csharp
public struct PeriodicSync : ISystem {
    private int _frame;
    public bool UpdateIsActive() => ++_frame % 10 == 0;

    public void Update() {
        // Автоматически видит ВСЕ изменения за последние 10 тиков
        foreach (var entity in W.Query<All<Position>, AllChanged<Position>>().Entities()) {
            SyncToNetwork(entity);
        }
    }
}
```
