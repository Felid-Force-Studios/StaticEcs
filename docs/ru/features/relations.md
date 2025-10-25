---
title: Отношения
parent: Возможности
nav_order: 9
---

### Отношения
Отношения - это способ связи одних сущностей с другими, связи позволяют добавлять иерархии между сущностями 
- связи могут быть нескольких видов и могут управляться автоматически
- связи основаны на компонентах или мультикомпонентах, и используют глобальный идентификатор сущности

### Виды связей:

#### Однонаправленная связь к одному (To-One)

#### Пример:
```csharp
//  A Passenger -> B
    
public struct Passenger : IEntityLinkComponent<Passenger> {
    public EntityGID Link;

    ref EntityGID IRefProvider<Passenger, EntityGID>.RefValue(ref Passenger component) => ref component.Link;
}

W.RegisterToOneRelationType<Passenger>();
```

___


#### Однонаправленная связь ко многим (To-Many)

#### Пример:
```csharp
//      Passenger -> B
//   /
//  A-- Passenger -> C
//   \
//      Passenger -> D
        
public struct Passengers : IEntityLinksComponent<Passengers> {
    public ROMulti<EntityGID> Links;

    ref ROMulti<EntityGID> IRefProvider<Passengers, ROMulti<EntityGID>>.RefValue(ref Passengers component) => ref component.Links;
}
                       
W.RegisterToManyRelationType<Passengers>();
```

___


#### Двунаправленная связь один к одному (One-To-One)

#### Пример:
```csharp
//  A <- Parent Child -> B <- Parent Child -> C
    
public struct Parent : IEntityLinkComponent<Parent> {
    public EntityGID Link;

    ref EntityGID IRefProvider<Parent, EntityGID>.RefValue(ref Parent component) => ref component.Link;
}

public struct Child : IEntityLinkComponent<Child> {
    public EntityGID Link;

    ref EntityGID IRefProvider<Child, EntityGID>.RefValue(ref Child component) => ref component.Link;
}

W.RegisterOneToOneRelationType<Parent, Child>();
```

___


#### Двунаправленная связь один к одному (замкнутая пара) (One-To-One)

#### Пример:
```csharp
//    Married   
//  A -------> B
//    Married   
//  A <------- B
    
public struct MarriedTo : IEntityLinkComponent<MarriedTo> {
    public EntityGID Link;

    ref EntityGID IRefProvider<MarriedTo, EntityGID>.RefValue(ref MarriedTo component) => ref component.Link;
}

W.RegisterOneToOneRelationType<MarriedTo, MarriedTo>()
```

___


#### Двунаправленная связь один ко многим (One-To-Many)

#### Пример:
```csharp
                       
//     <- Parent Child -> B 
//   /                     
//  A- <- Parent Child -> С 
//   \
//     <- Parent Child -> D
       
public struct Parent : IEntityLinkComponent<Parent> {
    public EntityGID Link;

    ref EntityGID IRefProvider<Parent, EntityGID>.RefValue(ref Parent component) => ref component.Link;
}

public struct Childs: IEntityLinksComponent<Childs> {
    public ROMulti<EntityGID> Links;

    ref ROMulti<EntityGID> IRefProvider<Childs, ROMulti<EntityGID>>.RefValue(ref Childs component) => ref component.Links;
}
                       
W.RegisterOneToManyRelationType<Parent, Childs>(defaultComponentCapacity: 4);
```

___


#### Двунаправленная связь многие ко многим (Many-To-Many)

#### Пример:
```csharp
                       
//     <- Owners Ownerships -> B 
//   /            
//  A- <- Owners Ownerships -> С 
//   /            
//  D- <- Owners Ownerships -> E 

public struct Ownerships : IEntityLinksComponent<Ownerships> {
    public ROMulti<EntityGID> Links;

    ref ROMulti<EntityGID> IRefProvider<Ownerships, ROMulti<EntityGID>>.RefValue(ref Ownerships component) => ref component.Links;
}

public struct Owners : IEntityLinksComponent<Owners> {
    public ROMulti<EntityGID> Links;

    ref ROMulti<EntityGID> IRefProvider<Owners, ROMulti<EntityGID>>.RefValue(ref Owners component) => ref component.Links;
}
                       
W.RegisterManyToManyRelationType<Ownerships, Owners>(16)
```

___


### Рассмотрим конфигурацию и пример шаг за шагом на примере связи `Один ко многим`
- Определение компонентов
> Компоненты могут быть двух видов:  
> `IEntityLinkComponent` - интерфейс компонента для хранения ссылки на одну сущность  
> `IEntityLinksComponent` - интерфейс компонента для хранения ссылок на несколько сущностей  

```csharp
// Определим компонент связи Родителя - `One` типа IEntityLinkComponent
public struct Parent : IEntityLinkComponent<Parent> {
    // Значение связи
    public EntityGID Link;

    ref EntityGID IRefProvider<Parent, EntityGID>.RefValue(ref Parent component) => ref component.Link;

    public override string ToString() => Link.ToString();
}

// Определим компонент связи Детей - `Many` типа IEntityLinksComponent 
public struct Childs: IEntityLinksComponent<Childs> {
    // Значение связей
    public ROMulti<EntityGID> Links;

    // Реализуем технический метод для доступа к значению связи
    ref ROMulti<EntityGID> IRefProvider<Childs, ROMulti<EntityGID>>.RefValue(ref Childs component) => ref component.Links;

    public override string ToString() => Links.ToString();
}
```

___


- Создание мира и сущностей

```csharp

W.Create(WorldConfig.Default());
// ...
// Регистрируем типы компонентов defaultComponentCapacity устанавливает минимальный размер Childs в мультикомпоненте 
W.RegisterOneToManyRelationType<Parent, Childs>(defaultComponentCapacity: 4);
// ...
W.Initialize();

var father = W.Entity.New(new Name("Father"));
var sonAlex = W.Entity.New(new Name("Son Alex"));
var sonJack = W.Entity.New(new Name("Son Jack"));
var sonKevin = W.Entity.New(new Name("Son Kevin"));
```

___


- Регистрация связи (Вариант 1 со стороны родителя)
> Устанавливаем связь где father ссылается на детей {`sonAlex`, `sonJack`, `sonKevin`}  
> При установке компонента дети автоматически получат обратный компонент `Parent` с ссылкой на father  
> Метод `SetLinks` создает или использует существующий компонент (используется для типа `IEntityLinksComponent`)  
> принимает от 1-5 `EntityGID` значений и возвращает ссылку на компонент, в случае если значение уже установлена в `DEBUG` будет ошибка  

```csharp
ref Childs childs = ref father.SetLinks<Childs>(sonAlex, sonJack, sonKevin);
```

> Метод `TrySetLinks` создает или использует существующий компонент (используется для типа `IEntityLinksComponent`)  
> принимает от 1-5 `EntityGID` значений и возвращает ссылку на компонент, в случае если значение уже установлена то оно не будет добавлено 

```csharp
ref Childs childs = ref father.TrySetLinks<Childs>(sonAlex, sonJack, sonKevin);
```

___

- Регистрация связи (Вариант 2 со стороны детей)
> Мы могли бы установить связь со стороны детей  
> При установке компонента родитель автоматически получит обратный компонент Childs с ссылкой на ребенка  
> Метод `SetLink` добавляет компонент и устанавливает значение (используется для типа `IEntityLinkComponent`)   
> в случае если компонент уже присутствует то компонент удаляется и добавляется новый  
> это необходимо для автоматического менеджмента связей

```csharp
 ref Parent sonAlexParent = ref sonAlex.SetLink<Parent>(father);
 ref Parent sonJackParent = ref sonJack.SetLink<Parent>(father);
 ref Parent sonKevinParent = ref sonKevin.SetLink<Parent>(father);
```

Таким образом не имеет значения с какой стороны установлена связь, обратная ссылка будет установлена в любом случае  
Просмотрев все сущности мы можем убедиться в этом

```csharp
foreach (var entity in W.Query.Entities()) {
    Console.WriteLine(entity.PrettyString);
}
//  Entity ID: 3                   
//  Components:                    
//   - [0] Name ( Son Kevin )      
//   - [1] Parent ( Father )       
//  
//  Entity ID: 2                   
//  Components:                    
//   - [0] Name ( Son Jack )       
//   - [1] Parent ( Father )       
//                                 
//  Entity ID: 1                   
//  Components:                    
//   - [0] Name ( Son Alex )       
//   - [1] Parent ( Father )       
//                                 
//  Entity ID: 0                   
//  Components:
//   - [0] Name ( Father )
//   - [2] Childs ( Son Alex, Son Jack, Son Kevin )
```

___


- Удаление связи (Вариант 1 со стороны детей )
> Метод `TryDeleteLink` удаляет связь (и компонент) если она есть (используется для типа `IEntityLinkComponent`)

```csharp
sonAlex.TryDeleteLink<Parent>();
sonJack.TryDeleteLink<Parent>();
sonKevin.TryDeleteLink<Parent>();
```

> По умолчанию при удалении связи будет удалена обратная ссылка  
> Это значит что у `father` будет удалена ссылка на всех детей  
> Чтобы переопределить это поведение, необходимо указать стратегию удаления при регистрации компонентов  
> `leftDeleteStrategy` - стратегия удаления для компонента `Parent`

```csharp
W.RegisterOneToManyRelationType<Parent, Childs>(defaultComponentCapacity: 4, leftDeleteStrategy: Default);
```

Доступны следующие виды стратегий:  
    - `Default`               : Ничего не делает при удалении  
    - `DestroyLinkedEntity`   : Уничтожает прилинкованную сущность  
    - `DeleteAnotherLink`     : Удаляет связь у прилинкованной сущности (поведение по умолчанию)  


- Удаление связи (Вариант 2 со стороны родителя )
> Метод `TryDeleteLinks` удаляет связь если она есть (используется для типа `IEntityLinksComponent`)  
> принимает от 0-5 `EntityGID` значение, если значение не передано удаляет все связи  
> если связей не осталось то компонент тоже будет удален

```csharp
father.TryDeleteLinks<Childs>();
```

> По умолчанию при удалении связи будет удалена обратная ссылка
> Это значит что у всех детей будет удалена ссылка на родителя
> Чтобы переопределить это поведение, необходимо указать стратегию удаления при регистрации компонентов
> `rightDeleteStrategy` - стратегия удаления для компонента `Childs`
```csharp
W.RegisterOneToManyRelationType<Parent, Childs>(defaultComponentCapacity: 4, rightDeleteStrategy: Default);
```

Доступны следующие виды стратегий:  
    - `Default`               : Ничего не делает при удалении  
    - `DestroyLinkedEntity`   : Уничтожает прилинкованную сущность  
    - `DeleteAnotherLink`     : Удаляет связь у прилинкованной сущности (поведение по умолчанию)

___


- Дополнительно
> При регистрации можно переопределить DEBUG валидацию циклических связей, по умолчанию она включена  
> Для этого необходимо указать `disableRelationsCheckDebug` = `true` в методе регистрации компонентов  
>
> При регистрации можно переопределить поведение при копировании указав CopyStrategy `FFS.Libraries.StaticEcs.CopyStrategy`

> Компоненты отношений являются обычными компонентами и доступны все стандартные методы работы с некоторыми особенностями

```csharp
entity.Ref<Parent>();
entity.HasAllOf<Parent>();
//..
```

{: .importantru }
Стоит предостеречь от изменения значений связей вручную (не через специальные методы такие как `SetLink`, `SetLinks`, `TrySetLinks`, `TryDeleteLink`, `TryDeleteLinks`)  
например не стоит делать так: `entity.Ref<Parent>().Link = someGid;`  
потому что это не позволяет автоматически управлять обратными ссылками и другими действиями и может привести к сломанной игровой логике  
при этом ничего не мешает хранить дополнительные данные в компонентах помимо самой связи

___


- Методы фильтрации
> Компоненты отношений можно использовать в запросах как и любые другие компоненты

```csharp
W.Query.Entities<All<Parent, Childs>>()
// ..
```
