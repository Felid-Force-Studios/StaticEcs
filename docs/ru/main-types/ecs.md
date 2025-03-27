---
title: Ecs
parent: Основные типы
nav_order: 8
---

## WorldType
Тип-тег-идентификатор мира, служит для изоляции статических данных при создании разных миров в одном процессе
- Представлен в виде пользовательской структуры без данных с маркер интерфейсом `IWorldType`

#### Пример:
```c#
public struct MainWorldType : IWorldType { }
public struct MiniGameWorldType : IWorldType { }
```
___

## Ecs
Точка входа в библиотеку, отвечающая за доступ, создание, инициализацию, работу и уничтожение данных мира
- Представлен в виде статического класса `Ecs<T>` параметризованного `IWorldType`

{: .importantru }
> Так как тип- идентификатор `IWorldType` определяет доступ к конкретному миру  
> Есть три способа работы с библиотекой:

___

#### Первый способ - как есть через полное обращение (очень неудобно):
```c#
public struct MainWorldType : IWorldType { }

Ecs<MainWorldType>.Create(EcsConfig.Default());
Ecs<MainWorldType>.World.EntitiesCount();

var entity = Ecs<MainWorldType>.Entity.New<Position>();
```

#### Второй способ - чуть более удобный, использовать статические импорты или статические алиасы (придется писать в каждом файле)
```c#
using static FFS.Libraries.StaticEcs.Ecs<MainWorldType>;

public struct MainWorldType : IWorldType { }

Create(EcsConfig.Default());
World.EntitiesCount();

var entity = Entity.New<Position>();
```

#### Трейтий способ - самый удобный, использовать типы-алиасы в корневом неймспейсе (не требуется писать в каждом файле)
Везде в примерах будет использован именно этот способ
```c#
public struct MainWorldType : IWorldType { }

public abstract class MyEcs : Ecs<MainWorldType> { }
public abstract class MyWorld : Ecs<MainWorldType>.World { }

MyEcs.Create(EcsConfig.Default());
MyWorld.EntitiesCount();

var entity = MyEcs.Entity.New<Position>();
```

___

#### Основные операции:
```c#
// Определяем ID мира
public struct MainWorldType : IWorldType { }

// Регестрируем типы - алиасы
public abstract class MyEcs : Ecs<MainWorldType> { }
public abstract class MyWorld : MyEcs.World { }

// Создание мира с дефолтной конфигурацие
MyEcs.Create(EcsConfig.Default());
// Или кастомной
MyEcs.Create(new() {
            BaseEntitiesCount = 256,        // Базовый размер массива сущностей при создания мира
            BaseDeletedEntitiesCount = 256, // Базовый размер массива удаленных сущностей при создания мира
            BaseComponentTypesCount = 64    // Базовый размер всех разновидностей типов компонентов (количество пулов под каждый тип)
            BaseMaskTypesCount = 64,        // Базовый размер всех разновидностей типов масок (количество пулов под каждый тип)
            BaseTagTypesCount = 64,         // Базовый размер всех разновидностей типов тегов (количество пулов под каждый тип)
            BaseComponentPoolCount = 128,   // Базовый размер массива данных компонентов определнного типа (может быть переопределнно для конкретного типа при явной регистрации)
            BaseTagPoolCount = 128,         // Базовый размер массива тегов определнного типа (может быть переопределнно для конкретного типа при явной регистрации)
        });

MyWorld.         // Доступ к миру для MainWorldType (ID мира)
MyEcs.Entity.    // Доступ к сущности для MainWorldType (ID мира)
MyEcs.Context.   // Доступ к контексту для MainWorldType (ID мира)
MyEcs.Components.// Доступ к компонентам для MainWorldType (ID мира)
MyEcs.Tags.      // Доступ к тегам для MainWorldType (ID мира)
MyEcs.Masks.     // Доступ к маскам для MainWorldType (ID мира)

// Инициализация мира
MyEcs.Initialize();

// Уничтожение и очистка данных мира
MyEcs.Destroy();

```