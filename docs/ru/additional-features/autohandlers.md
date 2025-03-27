---
title: Авто обработчики
parent: Дополнительны возможности
nav_order: 2
---

### Авто обработчики
По умолчанию при добавлении или удалении компонента данные заполняются дефолтным значение, а при копировании компонент полностью копируется  
Чтобы установить свою логику дефолтной инициализации и сброса компонента можно воспользоваться обработчиками

___

#### Пример:
```csharp
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.World.RegisterComponentType<Position>(
                autoInit: static (ref Position position) => position.Val = Vector3.One, // заменяет поведение при создании компонента через метод Add
                autoReset: static (ref Position position) => position.Val = Vector3.One, // заменяет поведение при удалении компонента через метод Delete
                autoCopy: static (ref Position src, ref Position dst) => dst.Val = src.Val, // заменяет поведение при копировании компонента
            );
//...
MyEcs.Initialize();
```

{: .importantru }
> Стоит учитывать что создание сущности с установкой значения или добавления компонента через метод Put  
> полностью заменяют данные в компоненте, в обход установленных авто обработчиков