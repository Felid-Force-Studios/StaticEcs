---
title: Идентификаторы компонентов
parent: Дополнительны возможности
nav_order: 1
---

### Идентификаторы компонентов
В случае когда проект очень большой или наоборот маленький и объем компилируемого кода важен:

Такие вещи как = типизированные Query, и сахарные методы работы с сущностью, могут раздувать компилируемый код за счет мономорфизации дженерик типов в структурах и методах.

Чтобы этого избежать, реализован механиз динамических идентификаторов для компонентов, тегов и масок - которые позволяют использовать их вместо параметров типов

___

#### Как это работает:
```csharp
// После вызова Ecs.Create(EcsConfig.Default());
// Можно явно зарегистрировать типы компонентов и получить структуру содержащую идентифкатор типа
ComponentDynId positionId = MyWorld.RegisterComponentType<Position>();
TagDynId unitTagId = MyWorld.RegisterTagType<Unit>();
MaskDynId frozenMaskId = MyWorld.RegisterMaskType<Frozen>();

// Альтернативно можно после инициализации мира в любой момент получить данные идентификаторы след образом:
ComponentDynId positionId = Ecs.Components.DynamicId<Position>();
TagDynId unitTagId = Ecs.Tags.DynamicId<Unit>();
MaskDynId frozenMaskId = Ecs.Masks.DynamicId<Frozen>();

// Данные идентификаторы можно сохранить любым удобным способом и использовать в операциях c сущностью или Query
// Существую перегрузки для большинства методов работы с сущностью для данных идентификаторов
// Пример для сущностей
entity.Add(positionId);
entity.TryAdd(positionId, VelocityId, nameId);
entity.Delete(positionId, VelocityId, nameId);
entity.AddTag(unitTagId);
entity.SetMask(frozenMaskId);

// Пример для Query
// Существуют перегрузки типы Types1-8, Tag1-8, Mask1-8 а также общие реализаци для расширений такие как TypesBox, TypesArray и тд
// Данные перегрузки содержат данные идентификаторов, а не пустые как аналоги с дженериками
// это значит что их желательно не создавать на лету а кешировать для лучшей производительности

var all = new Types3(positionId, VelocityId, nameId).All();
foreach (var entity in MyWorld.QueryEntities.For(all)) {
    //..
}

var with = With.Create(
    new Types3(positionId, VelocityId, nameId).All(),
    new Types1(scaleId).None(),
    new Tag2(playerTagId, unitTagId).Any(),
    new Mask1(frozenMaskId).AllAndNone(new Mask1(flammableMaskId))
);
foreach (var entity in MyWorld.QueryEntities.For(with)) {
    //..
}

// Также это механизм позволяет использовать данные ключи в логике игры
// Например когда тип создаваемого компонента меняется в зависимости от условий
// Важно! помнить что данные идентификаторы динамические - это значит что нет гарантий что от запуска к запуску они будут одинаковые
// значит их нельзя сериализовывать или использовать подобным образом
```