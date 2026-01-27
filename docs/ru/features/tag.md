---
title: Тег
parent: Возможности
nav_order: 6
---

## Tag
Тег — аналог компонента, но без данных: служит для маркировки сущности булевым флагом
- Внутренне унифицирован с компонентами — хранится в `Components<T>` с флагом `IsTag`, разделяя единую инфраструктуру хранения
- Хранится исключительно в виде битовой маски — нет массивов данных, минимальные затраты памяти
- Не замедляет поиск по компонентам и позволяет создавать множество тегов
- Нет хуков (`OnAdd`/`OnDelete`) и нет enable/disable — тег либо есть, либо нет
- Идеален для состояний (`IsPlayer`, `IsDead`, `NeedsUpdate`), фильтрации запросов и любых булевых свойств
- Представлен в виде пустой пользовательской структуры с маркер-интерфейсом `ITag`
- Использует те же фильтры запросов, что и компоненты (`All<>`, `None<>`, `Any<>`) — отдельных фильтров для тегов нет

#### Пример:
```csharp
public struct Unit : ITag { }
public struct Player : ITag { }
public struct IsDead : ITag { }
```

___

{: .importantru }
Требуется регистрация в мире между созданием и инициализацией

```csharp
W.Create(WorldConfig.Default());
//...
W.Types()
    .Tag<Unit>()
    .Tag<Player>()
    .Tag<IsDead>();
//...
W.Initialize();
```

{: .noteru }
Для сериализации теги могут иметь стабильный GUID. Его можно передать вручную или объявить статическое поле/свойство `Guid` внутри структуры тега — `RegisterAll()` найдёт его автоматически (предпочитая имя `Guid`):

```csharp
// Ручная регистрация с GUID
W.Types().Tag<Poisoned>(new TagTypeConfig<Poisoned>(guid: new Guid("A1B2C3D4-...")));

// Или объявить статическое поле — RegisterAll() подхватит его
public struct Poisoned : ITag {
    public static readonly Guid Guid = new("A1B2C3D4-...");
}
```

{: .noteru }
Для отслеживания изменений тегов используйте `TagTypeConfig<T>` с параметрами `trackAdded` / `trackDeleted` (подробнее см. [Отслеживание изменений](tracking)):

```csharp
W.Types().Tag<Unit>(new TagTypeConfig<Unit>(
    trackAdded: true,    // включить отслеживание добавления (по умолчанию — false)
    trackDeleted: true   // включить отслеживание удаления (по умолчанию — false)
));

// Полная конфигурация с GUID и отслеживанием
W.Types().Tag<Poisoned>(new TagTypeConfig<Poisoned>(
    guid: new Guid("A1B2C3D4-..."),
    trackAdded: true,
    trackDeleted: true
));
```

___

#### Добавление тегов:
```csharp
// Добавление тега на сущность (перегрузки от 1 до 5 тегов)
// Вернёт true если тег отсутствовал и был добавлен, false если уже был
bool added = entity.Set<Unit>();

// Добавление нескольких тегов за один вызов
entity.Set<Unit, Player>();
entity.Set<Unit, Player, IsDead>();
// Также доступны перегрузки на 4 и 5 тегов
```

___

#### Основные операции:
```csharp
// Получить количество тегов на сущности
int tagsCount = entity.TagsCount();

// Проверить наличие тега (перегрузки от 1 до 3 тегов — проверяет ВСЕ указанные)
bool hasUnit = entity.Has<Unit>();
bool hasBoth = entity.Has<Unit, Player>();
bool hasAll3 = entity.Has<Unit, Player, IsDead>();

// Проверить наличие хотя бы одного из указанных тегов (перегрузки от 2 до 3 тегов)
bool hasAny = entity.HasAny<Unit, Player>();
bool hasAny3 = entity.HasAny<Unit, Player, IsDead>();

// Удалить тег у сущности (перегрузки от 1 до 5 тегов)
// Вернёт true если тег присутствовал и был удалён, false если тега не было
// Безопасно использовать даже если тега нет
bool deleted = entity.Delete<Unit>();
entity.Delete<Unit, Player>();

// Переключить тег: если нет — добавить, если есть — удалить (перегрузки от 1 до 3 тегов)
// Вернёт true если тег был добавлен, false если был удалён
bool state = entity.Toggle<Unit>();
entity.Toggle<Unit, Player>();

// Условная установка или удаление тега по булевому значению (перегрузки от 1 до 3 тегов)
// true — тег устанавливается, false — удаляется
entity.Apply<Unit>(true);
entity.Apply<Unit, Player>(false, true); // Unit удалится, Player установится
```

___

#### Копирование и перемещение:
```csharp
var source = W.Entity.New<Position>();
source.Set<Unit, Player>();

var target = W.Entity.New<Position>();

// Скопировать указанные теги на другую сущность (перегрузки от 1 до 5 тегов)
// Исходная сущность сохраняет свои теги
// Вернёт true (для одного тега) если тег был у источника и скопирован
bool copied = source.CopyTo<Unit>(target);
source.CopyTo<Unit, Player>(target);

// Переместить указанные теги на другую сущность (перегрузки от 1 до 5 тегов)
// Тег добавляется на target и удаляется с source
// Вернёт true (для одного тега) если тег был перемещён
bool moved = source.MoveTo<Unit>(target);
source.MoveTo<Unit, Player>(target);
```

___

#### Фильтры запросов:

Теги используют те же фильтры запросов, что и компоненты: `All<>`, `None<>`, `Any<>` и их варианты. Подробнее см. раздел [Запросы](query.md).

___

#### Отладка:
```csharp
// Собрать все теги сущности в список (для инспектора/отладки)
// Список очищается перед заполнением
var tags = new List<ITag>();
entity.GetAllTags(tags);
```
