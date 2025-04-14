---
title: Стандартный компонент
parent: Основные типы
nav_order: 4
---

## StandardComponent
Стандартный компонент - стандартные свойства сущности, присутствует на каждой создаваемой сущности по умолчанию
- Оптимизированное хранилище и прямой доступ к данным по идентификатору сущности
- Не может быть удален, только изменен
- Не учавствует в запросах, так как присутствует на всех сущностях
- Представлен в виде пользовательской структуры с маркер интерфейсом `IStandardComponent`

{: .noteru }
> Следует использовать в случаях когда ВСЕ сущности в мире должны содержать компоненты какого либо типа  
> например если компонент позиции должен быть на каждой сущности без исключения, стоит использовать стандартный компонент  
> так как скорость доступа выше, и не будет дополнительных затрат по памяти
>
> Также может быть использован для небольших компонентов размером 1-8 байт, если не требуется строить логику на базе наличия или отсутсвия компонента
>
> Например внутреняя версия сущности `entity.Version()` является стандартным компонентом

#### Пример:
```c#
public struct EntityType : IStandardComponent {
    public int Val;
}
```
___

{: .importantru }
Требуется регистрация в мире между созданием и инициализацией

```c#
World.Create(WorldConfig.Default());
//...
World.RegisterStandardComponentType<EntityType>();
//...
World.Initialize();
```
___

{: .importantru }
Если требуется автоматическая инициализация при создании сущности или автоматический сброс при удалении сущности  
необходимо явно зарегестрировать обработчики

#### Example:
```c#
public struct EntityType : IStandardComponent {
    public int Val;
    
    public void Init() {
        Val = -1;
    }
    
    public void Reset() {
        Val = -1;
    }
    
    public void CopyTo(ref EntityType dst) {
        dst.Val = Val;
    }
}

World.Create(WorldConfig.Default());
//...

World.RegisterStandardComponentType<EntityType>(
                onAdd: static (ref EntityType component) => component.Init(), // При создании сущности будет вызвана данная функция 
                onDelete: static (ref EntityType component) => component.Reset(), // При уничтожении сущности будет вызвана данная функция  
                onCopy: static (ref EntityType src, ref EntityType dst) => src.CopyTo(ref dst), // При копировании стандартных компонентов будет вызвана данная сущности вместо простого копирования
            );
//...
World.Initialize();
```
___

#### Основные операции:
```c#
 // Получить количество стандартных компонентов на сущности
int standardComponentsCount = entity.StandardComponentsCount();

// Получить ref ссылку на стандартный компонент на чтение\запись
ref var entityType = ref entity.RefStandard<EntityType>();
entityType.Val = 123;

var entity2 = World.Entity.New<SomeComponent>();
// Скопировать указанные стандартные компоненты на другую сущность (методы перегрузки от 1-5 компонентов)
entity.CopyStandardComponentsTo<EntityType>(entity2);
```