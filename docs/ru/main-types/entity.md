---
title: Сущность
parent: Основные типы
nav_order: 1
---

## Entity
Сущность - служит для идентификации объекта в игровом мире и доступа к прикрепленным компонентам
- Представлена в виде структуры размером 4 байт

___

{: .importantru }
> По умолчанию сущность может быть создана и существовать без компонентов, также при удалении последнего компонента не удаляется  
> Если требуется переопределить это поведение необходимо указать директиву компилятора `FFS_ECS_LIFECYCLE_ENTITY`  
> Дополнительная информация: [Директивы компилятора](../additional-features/compilerdirectives.md)

___

#### Создание:
```csharp
// Создание одной сущности

// Способ 1 - создание "пустой" сущности
var entity = MyEcs.Entity.New();

// Способ 2 - с указанием типа компонента (методы перегрузки от 1-5 компонентов)
var entity = MyEcs.Entity.New<Position>();
var entity = MyEcs.Entity.New<Position, Velocity, Name>();

// Способ 3 - с указанием значения компонента (методы перегрузки от 1-5 компонентов)
var entity = MyEcs.Entity.New(new Position(x: 1, y: 1, z: 2));
var entity = MyEcs.Entity.New(
            new Name { Val = "SomeName" },
            new Velocity { Val = 1f },
            new Position { Val = Vector3.One }
);

// Создание множества сущностей
// Способ 1 - с указанием типа компонента (методы перегрузки от 1-5 компонентов)
int count = 100;
MyEcs.Entity.NewOnes<Position>(count);

// Способ 2 - с указанием типа компонента (методы перегрузки от 1-5 компонентов) + делегата инициализации каждой сущности
int count = 100;
MyEcs.Entity.NewOnes<Position>(count, static entity => {
    // some init logic for each entity
});

// Способ 3 - с указанием значения компонента (методы перегрузки от 1-5 компонентов)
int count = 100;
MyEcs.Entity.NewOnes(count, new Position(x: 1, y: 1, z: 2));

// Способ 4 - с указанием значения компонента (методы перегрузки от 1-5 компонентов) + делегата инициализации каждой сущности
int count = 100;
MyEcs.Entity.NewOnes(count, new Position(x: 1, y: 1, z: 2), static entity => {
    // some init logic for each entity
});
```
___

#### Основные операции:
```csharp
var entity = MyEcs.Entity.New(
            new Name { Val = "SomeName" },
            new Velocity { Val = 1f },
            new Position { Val = Vector3.One }
);

entity.Disable();                        // Отключить сущность (отключенная сущность по умолчанию не находится при запросах (смотри Query))
entity.Enable();                         // Включить сущность

bool enabled = entity.IsEnabled();       // Проверить включена ли сущность в мире
bool disabled = entity.IsDisabled();     // Проверить выключена ли сущность в мире

bool actual = entity.IsActual();         // Проверить не удалена ли сущность в мире
short version = entity.Version();        // Получить версию сущности
var clone = entity.Clone();              // Клонировать сущность и все компоненты, теги, маски
entity.Destroy();                        // Удалить сущность и все компоненты, теги, маски

var entity2 = MyEcs.Entity.New<Name>();
clone.CopyTo(entity2);                   // Копировать все компоненты, теги, маски в указанную сущность

var entity3 = MyEcs.Entity.New<Name>();
entity2.MoveTo(entity3);                 // Перенести все компоненты в указанную сущность и удалить текущую

PackedEntity packed = entity3.Pack();    // Упаковать сущность с мета информацией о версии для передачи

var str = entity3.ToPrettyString();      // Получить строку со всей информацией о сущности
```