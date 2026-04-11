---
title: Компонент
parent: Возможности
nav_order: 3
---

## Component
Компонент — наделяет сущность данными и свойствами
- Представлен в виде пользовательской структуры с маркер-интерфейсом `IComponent`
- Представлен как struct исключительно по соображениям производительности (SoA-хранение)
- Поддерживает хуки жизненного цикла: `OnAdd`, `OnDelete`, `CopyTo`, `Write`, `Read`
- Может быть включён/отключён без удаления данных

#### Пример:
```csharp
public struct Position : IComponent {
    public Vector3 Value;
}

public struct Velocity : IComponent {
    public float Value;
}

public struct Name : IComponent {
    public string Val;
}
```

___

{: .importantru }
Требуется регистрация в мире между созданием и инициализацией

```csharp
W.Create(WorldConfig.Default());
//...
// Простая регистрация без конфигурации (подходит для большинства случаев)
W.Types()
    .Component<Position>()
    .Component<Velocity>()
    .Component<Name>();

// Регистрация с конфигурацией
W.Types().Component<Scale>(new ComponentTypeConfig<Scale>(
    guid: new Guid("..."),                           // стабильный идентификатор для сериализации (по умолчанию — автоматически из имени типа)
    version: 1,                                      // версия схемы данных для миграции (по умолчанию — 0)
    noDataLifecycle: false,                          // отключить управление данными фреймворком (по умолчанию — false)
    readWriteStrategy: null,                         // стратегия бинарной сериализации (по умолчанию — авто-определение)
    defaultValue: new Scale { Value = Vector3.One }, // значение по умолчанию для инициализации и удаления (по умолчанию — нет)
    trackAdded: true,                                // включить отслеживание добавления (по умолчанию — false), см. Отслеживание изменений
    trackDeleted: true                               // включить отслеживание удаления (по умолчанию — false), см. Отслеживание изменений
));
//...
W.Initialize();
```

{: .noteru }
Параметр `noDataLifecycle` управляет жизненным циклом данных компонента. По умолчанию (`noDataLifecycle: false`) фреймворк пре-инициализирует хранилище значением `defaultValue` и сбрасывает данные к `defaultValue` при удалении — `entity.Add<T>()` возвращает настроенное значение по умолчанию. При `noDataLifecycle: true` инициализация и очистка не выполняются — полезно для высокочастотных unmanaged-типов. Если `OnDelete` определён, хук отвечает за очистку вне зависимости от этого флага.

{: .noteru }
Вместо передачи конфигурации вручную можно объявить статическое поле или свойство типа `ComponentTypeConfig<T>` прямо внутри структуры компонента. `RegisterAll()` автоматически найдёт его по типу (предпочитая имя `Config`):

```csharp
public struct Health : IComponent {
    public float Value;
    public static readonly ComponentTypeConfig<Health> Config = new(
        defaultValue: new Health { Value = 100f }
    );
}
// RegisterAll() автоматически использует Health.Config
```

___

#### Создание сущностей с компонентами:
```csharp
// Создать пустую сущность (без компонентов и тегов)
W.Entity entity = W.NewEntity<Default>();

// Создать сущность с указанием типа и кластера
W.Entity entity = W.NewEntity<Default>(clusterId: 0);

// Создать сущность с компонентами — Set возвращает Entity (перегрузки от 1 до 8 компонентов)
W.Entity entity = W.NewEntity<Default>().Set(new Position { Value = Vector3.One });
W.Entity entity = W.NewEntity<Default>().Set(
    new Position { Value = Vector3.One },
    new Velocity { Value = 1f }
);
```

___

#### Добавление компонентов:
```csharp
// Add без значения: если компонент уже есть → вернёт ref к существующему, хуки НЕ вызываются
// Если нет → инициализирует default, вызовет OnAdd
ref var position = ref entity.Add<Position>();

// С флагом isNew: isNew=true если компонент добавлен впервые
ref var position = ref entity.Add<Position>(out bool isNew);

// Добавление нескольких компонентов за один вызов (перегрузки от 2 до 5)
entity.Add<Position, Velocity>();

// Set с значением: ВСЕГДА перезаписывает данные
// Если компонент уже есть → OnDelete(старый) → замена → OnAdd(новый)
// Если нет → установка значения → OnAdd
ref var position = ref entity.Set(new Position { Value = Vector3.One });

// Установка нескольких компонентов с значениями (перегрузки от 2 до 12)
entity.Set(new Position { Value = Vector3.One }, new Velocity { Value = 1f });
```

{: .importantru }
`Add<T>()` без значения и `Set<T>(T value)` с значением имеют разную семантику хуков.
Без значения: если компонент уже существует, хуки **не вызываются**, возвращается ref к текущим данным.
С значением: данные **всегда перезаписываются** с полным циклом `OnDelete` → замена → `OnAdd`.

___

#### Доступ к данным:
```csharp
// Получить мутабельную ref-ссылку на компонент (чтение и запись)
// НЕ помечает как Changed — используйте Mut<T>() для отслеживаемого доступа
ref var velocity = ref entity.Ref<Velocity>();
velocity.Value += 10f;

// Получить readonly ref-ссылку на компонент — НЕ помечает как Changed
ref readonly var pos = ref entity.Read<Position>();
var x = pos.Value.x; // чтение OK, без пометки Changed

// Получить отслеживаемую мутабельную ref-ссылку — помечает как Changed если trackChanged включён
ref var pos = ref entity.Mut<Position>();
pos.Value += delta; // данные изменены И помечены как Changed
```

{: .importantru }
`Ref<T>()` НЕ помечает Changed. Используйте `Mut<T>()` когда нужен трекинг изменений для фильтров `AllChanged<T>`. В делегатах запросов (`For`) параметры `ref` автоматически используют семантику `Mut`.

___

#### Основные операции:
```csharp
// Получить количество компонентов на сущности
int count = entity.ComponentsCount();

// Проверить наличие компонента (перегрузки от 1 до 3 — проверяет ВСЕ указанные)
// Проверяет наличие независимо от состояния enabled/disabled
bool has = entity.Has<Position>();
bool hasBoth = entity.Has<Position, Velocity>();
bool hasAll = entity.Has<Position, Velocity, Name>();

// Проверить наличие хотя бы одного из указанных компонентов (перегрузки от 2 до 3)
bool hasAny = entity.HasAny<Position, Velocity>();
bool hasAny3 = entity.HasAny<Position, Velocity, Name>();

// Удалить компонент (перегрузки от 1 до 5)
// Вызывает OnDelete если компонент был; вернёт true если удалён, false если не было
bool deleted = entity.Delete<Position>();
entity.Delete<Position, Velocity>();
entity.Delete<Position, Velocity, Name>();
```

___

#### Enable/Disable:
```csharp
// Отключить компонент — данные сохраняются, но сущность исключается из стандартных запросов
// Вернёт ToggleResult: MissingComponent, Unchanged или Changed
ToggleResult disabled = entity.Disable<Position>();
entity.Disable<Position, Velocity>();
entity.Disable<Position, Velocity, Name>();

// Включить компонент обратно
// Вернёт ToggleResult: MissingComponent, Unchanged или Changed
ToggleResult enabled = entity.Enable<Position>();
entity.Enable<Position, Velocity>();
entity.Enable<Position, Velocity, Name>();

// Проверить что ВСЕ указанные компоненты включены (перегрузки от 1 до 3)
bool posEnabled = entity.HasEnabled<Position>();
bool bothEnabled = entity.HasEnabled<Position, Velocity>();

// Проверить что хотя бы один включён (перегрузки от 2 до 3)
bool anyEnabled = entity.HasEnabledAny<Position, Velocity>();

// Проверить что ВСЕ указанные компоненты отключены (перегрузки от 1 до 3)
bool posDisabled = entity.HasDisabled<Position>();
bool bothDisabled = entity.HasDisabled<Position, Velocity>();

// Проверить что хотя бы один отключён (перегрузки от 2 до 3)
bool anyDisabled = entity.HasDisabledAny<Position, Velocity>();
```

{: .noteru }
Отключённые компоненты не попадают в стандартные фильтры запросов (`All`, `None`, `Any`), но данные сохраняются в памяти. Используйте `WithDisabled`/`OnlyDisabled` варианты фильтров для работы с отключёнными компонентами.

___

#### Копирование и перемещение:
```csharp
var source = W.NewEntity<Default>().Set(new Position(), new Velocity());
var target = W.NewEntity<Default>();

// Скопировать указанные компоненты на другую сущность (перегрузки от 1 до 5)
// Исходная сущность сохраняет свои компоненты
// Если хук CopyTo переопределён — вызывается пользовательская логика копирования
// Если хук CopyTo НЕ переопределён — выполняется побитовое копирование через Add + сохранение disabled-состояния
// Вернёт true (для одного) если компонент был у источника
bool copied = source.CopyTo<Position>(target);
source.CopyTo<Position, Velocity>(target);

// Переместить указанные компоненты на другую сущность (перегрузки от 1 до 5)
// Выполняет Copy на target, затем Delete с source (OnDelete вызывается на source)
bool moved = source.MoveTo<Position>(target);
source.MoveTo<Position, Velocity>(target);
```

___

#### Фильтры запросов:

Фильтры по компонентам описаны в разделе [Запросы — Компоненты](query.md#компоненты).

___

#### Хуки жизненного цикла:

Интерфейс `IComponent` предоставляет хуки с пустыми реализациями по умолчанию — переопределяйте только те, которые нужны.

{: .importantru }
Не оставляйте пустые реализации хуков. Если хук не нужен — не реализуйте его. Нереализованные хуки не вызываются и не создают накладных расходов.

```csharp
public struct Cooldown : IComponent {
    public float Duration;
    public float Elapsed;

    // Вызывается после добавления компонента или перезаписи значения через Set(value)
    public void OnAdd<TWorld>(World<TWorld>.Entity self) where TWorld : struct, IWorldType {
        Elapsed = 0f; // сбросить таймер при каждом применении
    }

    // Вызывается перед удалением компонента (Delete), перед перезаписью (Set с значением),
    // а также при уничтожении сущности для каждого её компонента
    //
    // Параметр `reason` указывает причину удаления:
    // HookReason.Default      — явное удаление или уничтожение сущности
    // HookReason.UnloadEntity — выгрузка сущности/чанка
    // HookReason.WorldDestroy — сброс/уничтожение мира
    public void OnDelete<TWorld>(World<TWorld>.Entity self, HookReason reason) where TWorld : struct, IWorldType { }

    // Пользовательская логика копирования для CopyTo / MoveTo / Clone
    // Если НЕ переопределён — побитовое копирование + сохранение disabled-состояния
    // Если переопределён — полностью заменяет стандартную логику копирования
    public void CopyTo<TWorld>(World<TWorld>.Entity self, World<TWorld>.Entity other, bool disabled)
        where TWorld : struct, IWorldType {
        ref var copy = ref other.Add<Cooldown>();
        copy.Duration = Duration;
        copy.Elapsed = 0f; // клон начинает с нуля
    }

    // Сериализация — запись компонента в бинарный поток
    // Обязательно для EntitiesSnapshot (все типы), и для non-unmanaged типов в любых снимках
    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType {
        writer.WriteFloat(Duration);
        writer.WriteFloat(Elapsed);
    }

    // Десериализация — чтение компонента из бинарного потока
    // Параметр version позволяет мигрировать данные между версиями схемы
    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled)
        where TWorld : struct, IWorldType {
        Duration = reader.ReadFloat();
        Elapsed = reader.ReadFloat();
    }
}
```

{: .importantru }
Порядок вызова хуков при `Set(value)` для существующего компонента: `OnDelete`(старое значение) → замена данных → `OnAdd`(новое значение). При `Delete` или уничтожении сущности вызывается только `OnDelete`.

___

#### Отладка:
```csharp
// Собрать все компоненты сущности в список (для инспектора/отладки)
// Список очищается перед заполнением
var components = new List<IComponent>();
entity.GetAllComponents(components);
```
