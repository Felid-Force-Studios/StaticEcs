---
title: Мир
parent: Основные типы
nav_order: 9
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

## World
Точка входа в библиотеку, отвечающая за доступ, создание, инициализацию, работу и уничтожение данных мира
- Представлен в виде статического класса `Ecs<T>` параметризованного `IWorldType`

{: .importantru }
> Так как тип- идентификатор `IWorldType` определяет доступ к конкретному миру  
> Есть три способа работы с библиотекой:

___

#### Первый способ - как есть через полное обращение (очень неудобно):
```c#
public struct WT : IWorldType { }

Ecs<WT>.Create(WorldConfig.Default());
Ecs<WT>.World.EntitiesCount();

var entity = Ecs<WT>.Entity.New<Position>();
```

#### Второй способ - чуть более удобный, использовать статические импорты или статические алиасы (придется писать в каждом файле)
```c#
using static FFS.Libraries.StaticEcs.World<WT>;

public struct WT : IWorldType { }

Create(WorldConfig.Default());
EntitiesCount();

var entity = Entity.New<Position>();
```

#### Трейтий способ - самый удобный, использовать типы-алиасы в корневом неймспейсе (не требуется писать в каждом файле)
Везде в примерах будет использован именно этот способ
```c#
public struct WT : IWorldType { }

public abstract class World : World<WT> { }

World.Create(WorldConfig.Default());
World.EntitiesCount();

var entity = World.Entity.New<Position>();
```

___

#### Основные операции:
```c#
// Определяем ID мира
public struct WT : IWorldType { }

// Регестрируем типы - алиасы
public abstract class World : World<WT> { }

// Создание мира с дефолтной конфигурацие
World.Create(WorldConfig.Default());
// Или кастомной
World.Create(new() {
            BaseEntitiesCount = 256,        // Базовый размер массива сущностей при создания мира
            BaseDeletedEntitiesCount = 256, // Базовый размер массива удаленных сущностей при создания мира
            BaseComponentTypesCount = 64    // Базовый размер всех разновидностей типов компонентов (количество пулов под каждый тип)
            BaseMaskTypesCount = 64,        // Базовый размер всех разновидностей типов масок (количество пулов под каждый тип)
            BaseTagTypesCount = 64,         // Базовый размер всех разновидностей типов тегов (количество пулов под каждый тип)
            BaseComponentPoolCount = 128,   // Базовый размер массива данных компонентов определнного типа (может быть переопределнно для конкретного типа при явной регистрации)
            BaseTagPoolCount = 128,         // Базовый размер массива тегов определнного типа (может быть переопределнно для конкретного типа при явной регистрации)
        });

World.Entity.    // Доступ к сущности для MainWorldType (ID мира)
World.Context.   // Доступ к контексту для MainWorldType (ID мира)
World.Components.// Доступ к компонентам для MainWorldType (ID мира)
World.Tags.      // Доступ к тегам для MainWorldType (ID мира)
World.Masks.     // Доступ к маскам для MainWorldType (ID мира)

// Инициализация мира
World.Initialize();

// Уничтожение и очистка данных мира
World.Destroy();

// При регистрации компонента возможно указать базовой размер массива даных компонентов этого типа
MyWorld.RegisterComponentType<Position>(256);

// аналогично RegisterComponentType, но для тегов
var unitTagId = MyWorld.RegisterTagType<Unit>(256);

// аналогично RegisterComponentType, но для масок
var visibleMaskId = MyWorld.RegisterMaskType<Visible>();

// true если мир инициализирован
bool initialized = MyWorld.IsInitialized();

// количество активных сущностей в мире
int entitiesCount = MyWorld.EntitiesCount();

// текущая емкость массива для сущностей
int entitiesCapacity = MyWorld.EntitiesCapacity();
```
