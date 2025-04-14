---
title: Компонент
parent: Основные типы
nav_order: 3
---

## Component
Компонент - наделяет сущность свойствами
- Дает возможность строить запросы поиска только по компонентам
- Представлен в виде пользовательской структуры с маркер интерфейсом `IComponent`
- Представлен в виде struct исключительно по соображениям производительности

#### Пример:
```c#
public struct Position : IComponent {
    public Vector3 Value;
}
```
___

{: .importantru }
Требуется регистрация в мире между созданием и инициализацией

```c#
World.Create(WorldConfig.Default());
//...
World.RegisterComponentType<Position>();
//...
World.Initialize();
```
___

#### Создание:
```c#
// Способ 1 - при создании сущности (аналогично методу Add())
var entity = World.Entity.New<Position>();

// Или через значение  (аналогично методу Put())
// Нужно быть осторожным с AutoInit и AutoReset (смотри дополнительные возможности)
var entity = World.Entity.New(new Position(x: 1, y: 1, z: 2));

// Добавление компонента на сущность и возврат ref значения на компонент (в DEBUG режиме будет ошибка если он уже существует на сущности)
ref var position = ref entity.Add<Position>();
// Добавление компонента на сущность (в DEBUG режиме будет ошибка если он уже существует на сущности) (методы перегрузки от 2-5 компонентов)
entity.Add<Position, Velocity>();

// Добавление компонента на сущность если его еще нет, иначе возврат существующего
ref var position = ref entity.TryAdd<Position>();
ref var position = ref entity.TryAdd<Position>(out bool added); // перегрузка где added = true если компонент новый, false если вернулся существующий
// Добавление компонента на сущность если его еще нет (методы перегрузки от 2-5 компонентов)
entity.TryAdd<Position, Velocity>();
entity.TryAdd<Position, Velocity>(out bool added); // перегрузка где added = true если хотя бы 1 компонент новый, false все компоненты существовали ранее

// Помещение компонента через значение. (методы перегрузки от 1-5 компонентов)
// Если компонент уже существует то все данные будут заменены, если не существует то создан с передаными данными
// Нужно быть осторожным с AutoInit и AutoReset (смотри дополнительные возможности)
entity.Put(new Position(x: 1, y: 1, z: 2));
```

___

#### Основные операции:
```c#
var entity = World.Entity.New(
            new Name { Val = "Player" },
            new Velocity { Val = 1f },
            new Position { Val = Vector3.One }
);
// Получить количество компонентов на сущности
int componentsCount = entity.ComponentsCount();

// Получить ref ссылку на компонент на чтение\запись
ref var velocity = ref entity.Ref<Velocity>();
velocity.Val++;

// Проверить наличие ВСЕХ указанных компонентов (методы перегрузки от 1-3 компонентов)
entity.HasAllOf<Position>();
entity.HasAllOf<Position, Velocity, Name>();

// Проверить наличие хотя бы одного указанного компонента (методы перегрузки от 2-3 компонентов)
entity.HasAnyOf<Position, Name>();
entity.HasAnyOf<Position, Velocity, Name>();

// Удалить компонент у сущности (в DEBUG режиме будет ошибка если компонент не существует на сущности,
// в релизе нельзя использовать если нет гарантии что компонент присутствует) (методы перегрузки от 1-5 компонентов)
entity.Delete<Position>();
entity.Delete<Position, Velocity, Name>();

// Удалить компонент у сущности если он есть (методы перегрузки от 1-5 компонентов)
bool deleted = entity.TryDelete<Position>();  // deleted = true если компонент был удален, false если компонента не было изначально
bool deleted = entity.TryDelete<Position, Velocity, Name>();  // deleted = true если ВСЕ компоненты был удалены, false если хотя бы 1 компонента не было изначально

var entity2 = World.Entity.New<Name>();
// Скопировать указанные компоненты на другую сущность (методы перегрузки от 1-5 компонентов)
entity.CopyComponentsTo<Position, Velocity>(entity2);
```

