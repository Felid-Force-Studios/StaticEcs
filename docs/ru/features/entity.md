---
title: Сущность
parent: Возможности
nav_order: 1
---

## Entity
Сущность - служит для идентификации объекта в игровом мире и доступа к прикрепленным компонентам
- Представлена в виде структуры размером 4 байт

___

{: .importantru }
> По умолчанию сущность может быть создана и существовать без компонентов, также при удалении последнего компонента не удаляется  
> Если требуется переопределить это поведение необходимо указать директиву компилятора `FFS_ECS_LIFECYCLE_ENTITY`  
> Дополнительная информация: [Директивы компилятора](compilerdirectives.md)

___

#### Создание:
```csharp
// Создание одной сущности

// Способ 1 - создание "пустой" сущности
W.Entity entity = W.Entity.New();

// Способ 2 - с указанием типа компонента (методы перегрузки от 1-5 компонентов)
W.Entity entity = W.Entity.New<Position>();
W.Entity entity = W.Entity.New<Position, Velocity, Name>();

// Способ 3 - с указанием значения компонента (методы перегрузки от 1-8 компонентов)
W.Entity entity = W.Entity.New(new Position(x: 1, y: 1, z: 2));
W.Entity entity = W.Entity.New(
            new Name { Val = "SomeName" },
            new Velocity { Val = 1f },
            new Position { Val = Vector3.One }
);

// Создание множества сущностей
// Способ 1 - с указанием типа компонента (методы перегрузки от 1-5 компонентов)
uint count = 100;
W.Entity.NewOnes<Position>(count);

// Способ 2 - с указанием типа компонента (методы перегрузки от 1-5 компонентов) + делегата инициализации каждой сущности
uint count = 100;
W.Entity.NewOnes<Position>(count, static entity => {
    // some init logic for each entity
});

// Способ 3 - с указанием значения компонента (методы перегрузки от 1-5 компонентов)
uint count = 100;
W.Entity.NewOnes(count, new Position(x: 1, y: 1, z: 2));

// Способ 4 - с указанием значения компонента (методы перегрузки от 1-5 компонентов) + делегата инициализации каждой сущности
uint count = 100;
W.Entity.NewOnes(count, new Position(x: 1, y: 1, z: 2), static entity => {
    // some init logic for each entity
});
```
___

#### Основные операции:
```csharp
W.Entity entity = W.Entity.New(
            new Name { Val = "SomeName" },
            new Velocity { Val = 1f },
            new Position { Val = Vector3.One }
);

entity.Disable();                              // Отключить сущность (отключенная сущность по умолчанию не находится при запросах (смотри Query))
entity.Enable();                               // Включить сущность
      
bool enabled = entity.IsEnabled();             // Проверить включена ли сущность в мире
bool disabled = entity.IsDisabled();           // Проверить выключена ли сущность в мире
      
bool actual = entity.IsActual();               // Проверить не удалена ли сущность в мире
short version = entity.Version();              // Получить версию сущности
W.Entity clone = entity.Clone();               // Клонировать сущность и все компоненты, теги
entity.Destroy();                              // Удалить сущность и все компоненты, теги
      
W.Entity entity2 = W.Entity.New<Name>();      
clone.CopyTo(entity2);                         // Копировать все компоненты, теги в указанную сущность
      
W.Entity entity3 = W.Entity.New<Name>();      
entity2.MoveTo(entity3);                       // Перенести все компоненты, теги в указанную сущность и удалить текущую
      
EntityGID gid = entity3.Gid();                 // Получить глобальный идентификатор сущности
      
var str = entity3.ToPrettyString;              // Получить строку со всей информацией о сущности

BoxedEntity<WorldType> boxed = entity3.Box();  // Получить класс-сущность реализующую IEntity
```
___

#### Дополнительно:

При инициализации мира можно передать функцию, которая будет выполняться при создании любой новой сущности

Пример:

```csharp
W.Create(WorldConfig.Default());
//...
W.OnCreateEntity(entity => entity.Add<Position, Rotation, Scale>());
//...
W.Initialize();
```