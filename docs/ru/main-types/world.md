---
title: Мир
parent: Основные типы
nav_order: 9
---

## World
Мир, содержит мета информацию сущностей, контролирует и менеджируют создание и удаление сущностей
- Представлен в виде статического класса `Ecs<IWorldType>.World`

___

#### Создание:
```c#
// It is created only when called
MyEcs.Create(config);

// Initialized only when called
MyEcs.Initialize();
```
___ 

#### Основные операции:
```c#
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

// версия сущности
short entityVersion = MyWorld.EntityVersion(entity);

// Удалить сущность и все ее компоненты - аналогично entity.Destroy();
MyWorld.DestroyEntity(entity);

// Копировать все компоненты теги и маски с одной сущности на другую - аналогично entitySrc.CopyTo(entityTarget);
MyWorld.CopyEntityData(entitySrc, entityTarget);

// Клонировать сущность и все компоненты теги и маски - аналогично entity.Clone();
var clone = MyWorld.CloneEntity(entity);

// Получить строку со всей информацией о сущности - аналогично entity.ToPrettyStringEntity();
var str = MyWorld.ToPrettyStringEntity(entity);

```