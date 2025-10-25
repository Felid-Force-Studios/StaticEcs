---
title: Entity global ID
parent: Features
nav_order: 2
---

## EntityGID
Global entity identifier - is a stable entity identifier   
Used for [event sends](events.md), [entity relationships](relations.md), [serializations](serialization.md), network transmissions, etc.    
Assigned automatically or manually when the entity is created.
- Represented as a 8 byte structure

___

#### Creation:
```csharp
// It's possible to get the active entity
EntityGID gid = entity.Gid();

// Or via the constructor
EntityGID gid2 = new EntityGID(id: 0, version: 1, clusterId: 0);
EntityGID gid3 = new EntityGID(rawValue: 16777216UL);
```

___

#### Basic operations:
```csharp
EntityGID gid = entity.Gid();

uint id = gid.Id;                                             // Entity ID
ushort version = gid.Version;                                 // Entity version
ushort clusterId = gid.ClusterId;                             // Entity cluster
uint chunk = gid.Chunk;                                       // Entity chunk
uint rawValue = gid.Raw;                                      // Raw value (id + version + clusterId)

bool actual = gid.IsActual<WT>();                             // Check if the entity is current (the entity may not be loaded)
bool loaded = gid.IsLoaded<WT>();                             // Check if the entity is loaded
bool loadedAndActual = gid.IsLoadedAndActual<WT>();           // Check if the entity is loaded and current
bool status = gid.TryUnpack<WT>(out var unpackedEntity);      // Attempt to get the active entity
var unpacked = gid.Unpack<WT>();                              // Getting the active entity is unsafe (there will be an error in DEBUG if the entity is not loaded or is not current)

W.Entity.New(someGid);                                        // An entity can be created with a custom identifier.

EntityGID gid2 = entity.Gid();
bool equals = gid.Equals(gid2);                               // identity of identifiers
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

## EntityGIDCompact
Similar to EntityGID, but half the size, and can be used in worlds with up to 16,000 entities and up to 4 clusters. (There will be an error in DEBUG if you go out of bounds.)
- Represented as a 4 byte structure

___

#### Creation:
```csharp
// It's possible to get the active entity
EntityGIDCompact gid = entity.GidCompact();

// Or via the constructor
EntityGIDCompact gid2 = new EntityGIDCompact(id: 0, version: 1, clusterId: 0);
EntityGIDCompact gid3 = new EntityGIDCompact(rawValue: 16777216U);
```