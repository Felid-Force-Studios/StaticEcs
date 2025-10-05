---
title: Entity global ID
parent: Features
nav_order: 2
---

## EntityGID
Global entity identifier - is a stable entity identifier   
Used for [event sends](events.md), [entity relationships](relations.md), [serializations](serialization.md), network transmissions, etc.    
Assigned automatically or manually when the entity is created.
- Represented as a 4 byte structure

___

#### Creation:
```csharp
// It's possible to get the active entity
EntityGID gid = entity.Gid();
```

___

#### Basic operations:
```csharp
EntityGID gid = entity.Gid();

uint id = gid.Id();                                           // Identifier
byte version = gid.Version();                                 // Version
uint rawValue = gid.Raw();                                    // Raw Value (id + version)

bool registered = gid.IsRegistered<WT>();                     // Check if this identifier is registered in the store (the entity may not be loaded)
bool loaded = gid.IsLoaded<WT>();                             // Check if an entity with this identifier is loaded
bool status = gid.TryUnpack<WT>(out var unpackedEntity);      // Trying to get an active entity
var unpacked = gid.Unpack<WT>();                              // It's not safe to get an active entity

W.Entity.New(someGid);                                    // An entity can be created with a custom identifier

EntityGID gid2 = entity.Pack();
bool equals = gid.Equals(gid2);                               // Verify the identity of the identifiers
```

#### Ways of use:
###### Events:
```csharp
public struct DamageEvent : IEvent { 
    public EntityGID Target;
    public float Damage;
}

// In the system:
foreach (var damageEvent in damageEventReceiver) {
    var val = weatherEvent.Value;
    if (val.Target.TryUnpack<WT>(out var entity)) {
        entity.Ref<Health>.Value -= val.Damage;
        //...
    }
}
```

###### Server-client networking:
You can use the GID as an entity relationship identifier on the client and server,  
for example, if the server controls the creation of entities and passes the GID to the client,  
then the client can use this identifier when creating an entity  
This way further interaction or commands from the server can store the GID and the client can easily retrieve the entity via Unpack
```csharp
public struct SomeCreateEntityClientCommand { 
    public EntityGID Id;
    public string prefab;
}
// Server
//.. 
var serverEntityPlayer;
client.SendMessage(new SomeCreateEntityClientCommand(serverEntityPlayer.Gid(), "player"))

// Client:
var someCreateEntityClientCommand = server.ReceiveMessage();
var gidFromServer = someCreateEntityClientCommand.Id;
var entity = ClientWorld.Entity.New(gidFromServer);
```