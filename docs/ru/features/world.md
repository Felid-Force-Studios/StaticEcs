---
title: Мир
parent: Возможности
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
- Представлен в виде статического класса `World<T>` параметризованного `IWorldType`

{: .importantru }
> Так как тип-идентификатор `IWorldType` определяет доступ к конкретному миру  
> Есть три способа работы с библиотекой:

___

#### Первый способ - как есть через полное обращение (очень неудобно):
```c#
public struct WT : IWorldType { }

World<WT>.Create(WorldConfig.Default());
World<WT>.CalculateEntitiesCount();

var entity = World<WT>.Entity.New<Position>();
```

#### Второй способ - чуть более удобный, использовать статические импорты или статические алиасы (придется писать в каждом файле)
```c#
using static FFS.Libraries.StaticEcs.World<WT>;

public struct WT : IWorldType { }

Create(WorldConfig.Default());
CalculateEntitiesCount();

var entity = Entity.New<Position>();
```

#### Трейтий способ - самый удобный, использовать типы-алиасы в корневом неймспейсе (не требуется писать в каждом файле)
Везде в примерах будет использован именно этот способ
```c#
public struct WT : IWorldType { }

public abstract class World : World<WT> { }

World.Create(WorldConfig.Default());
World.CalculateEntitiesCount();

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
            // Базовая емкость для сущностей при создания мира
            baseEntitiesCapacity = 4096,                        
            // Базовый размер всех разновидностей типов компонентов (количество типов компонент)
            BaseComponentTypesCount = 64                        
            // Базовый размер всех разновидностей типов тегов (количество типов тегов)
            BaseTagTypesCount = 64,                             
            // Режим работы многопоточной обработки 
            // (Disabled - потоки не создаются, MaxThreadsCount - создается максимально доступное количество потоков, CustomThreadsCount - указанное количество потоков)
            ParallelQueryType = ParallelQueryType.Disabled,
            // Количество потоков при ParallelQueryType.CustomThreadsCount
            CustomThreadCount = 4,
            // Строгий режим работы Query по умолчанию, дополнительно в разделе "Запросы"
            DefaultQueryModeStrict = true
        });

World.Entity.    // Доступ к сущности для MainWorldType (ID мира)
World.Context.   // Доступ к контексту для MainWorldType (ID мира)
World.Components.// Доступ к компонентам для MainWorldType (ID мира)
World.Tags.      // Доступ к тегам для MainWorldType (ID мира)

// Инициализация мира
World.Initialize();

// Уничтожение и очистка данных мира
World.Destroy();

// true если мир инициализирован
bool initialized = World.IsInitialized();

// количество активных сущностей в мире
int entitiesCount = World.CalculateEntitiesCount();

// текущая емкость для сущностей
int entitiesCapacity = World.CalculateEntitiesCapacity();
```
