---
title: Глобальный идентификатор сущности
parent: Основные типы
nav_order: 2
---

## EntityGID
Глобальный идентификатор сущности - является стабильным идентификатором сущности   
Используется для [отправки событий](../additional-features/events.md), [связей между сущностями](../additional-features/relations.md), [сериализации](../additional-features/serialization.md), передачи по сети, и тд.  
Назначается автоматически или вручную при создании сущности.  
- Представлена в виде структуры размером 4 байт

___

#### Создание:
```csharp
// Возможно получить у активной сущности
EntityGID gid = entity.Gid();
```

___

#### Основные операции:
```csharp
EntityGID gid = entity.Gid();

uint id = gid.Id();                                           // Идентификатор
byte version = gid.Version();                                 // Версия
bool registered = gid.IsRegistered<WT>();                     // Проверить зарегистрирован ли данный идентификатор в хранилище (сущность может быть не загружена)
bool loaded = gid.IsLoaded<WT>();                             // Проверить загружена ли сущность с таким идентификатором
bool status = gid.TryUnpack<WT>(out var unpackedEntity);      // Попытаться получить активную сущность
var unpacked = gid.Unpack<WT>();                              // Получить активную сущность небезопасно

World.Entity.New(someGid);                                    // Сущность может быть создана с кастомным идентификатором

EntityGID gid2 = entity.Pack();
bool equals = gid.Equals(gid2);                               // Проверить идентичность идентификаторов
```

#### Способы применения:
###### События:
```csharp
public struct DamageEvent : IEvent { 
    public EntityGID Target;
    public float Damage;
}

// В системе:
foreach (var damageEvent in damageEventReceiver) {
    var val = weatherEvent.Value;
    if (val.Target.TryUnpack<WT>(out var entity)) {
        entity.Ref<Health>.Value -= val.Damage;
        //...
    }
}
```

###### Сетевое взаимодействие сервер-клиент:
Можно использовать GID как идентификатор связи сущности на клиенте и сервере,  
например если сервер контролирует создание сущностей и передает на клиент GID,  
то клиент может использовать данный идентификатор при создании сущности  
Таким образом дальнейшее взаимодействие или команды с сервера могут сожержать GID, и клиент сможет легко получить сущность через Unpack
```csharp
public struct SomeCreateEntityClientCommand { 
    public EntityGID Id;
    public string prefab;
}
// Cервер
//.. 
var serverEntityPlayer;
client.SendMessage(new SomeCreateEntityClientCommand(serverEntityPlayer.Gid(), "player"))

// Клиент:
var someCreateEntityClientCommand = server.ReceiveMessage();
var gidFromServer = someCreateEntityClientCommand.Id;
var entity = ClientWorld.Entity.New(gidFromServer);
```