---
title: Entity global ID
parent: Features
nav_order: 2
---

## EntityGID
Global entity identifier — a stable reference to an entity, safe for storage, serialization, and network transmission
- Used for [events](events.md), [entity relationships](relations.md), [serialization](serialization.md), networking
- Contains Id, Version, and ClusterId — enables stale reference detection via version checking
- Assigned automatically on entity creation or manually via `NewEntityByGID`
- 8-byte struct (`StructLayout.Explicit`, fields overlap via `Raw`)

___

#### Obtaining:
```csharp
// Property on entity
EntityGID gid = entity.GID;

// Implicit conversion Entity → EntityGID
EntityGID gid = entity;

// Via constructor
EntityGID gid = new EntityGID(id: 0, version: 1, clusterId: 0);
EntityGID gid = new EntityGID(rawValue: 16777216UL);
```

___

#### Properties:
```csharp
EntityGID gid = entity.GID;

uint id = gid.Id;               // Internal entity slot index
ushort version = gid.Version;   // Generation counter (incremented on slot reuse)
ushort clusterId = gid.ClusterId; // Cluster identifier
uint chunk = gid.Chunk;         // Chunk index (computed)
ulong raw = gid.Raw;            // Raw 8-byte representation (all fields packed)
```

___

#### Validation and unpacking:
```csharp
EntityGID gid = entity.GID;

// Check GID status: Active, NotActual, or NotLoaded
GIDStatus status = gid.Status<WT>();

// Safe unpacking — returns true if entity is loaded and actual
if (gid.TryUnpack<WT>(out var entity)) {
    ref var pos = ref entity.Ref<Position>();
}

// With failure diagnostics
if (!gid.TryUnpack<WT>(out var entity, out GIDStatus status)) {
    // status == GIDStatus.NotActual → entity does not exist or version/cluster doesn't match (stale reference)
    // status == GIDStatus.NotLoaded → entity exists and version matches, but is currently unloaded
}

// Unsafe unpacking — throws in DEBUG if not loaded or stale
var entity = gid.Unpack<WT>();
```

___

#### Creating an entity with a specific GID:
```csharp
// Create entity at the exact slot specified by GID
// Used during deserialization and network synchronization
var entity = W.NewEntityByGID<Default>(gid);

// Non-generic variant (entity type known at runtime as byte)
byte entityTypeId = EntityTypeInfo<Default>.Id;
var entity = W.NewEntityByGID(entityTypeId, gid);
```

___

#### Invalidation:
```csharp
// Increment version without destroying the entity
// All previously obtained GIDs become stale (Status returns GIDStatus.NotActual)
entity.UpVersion();
```

___

#### Comparison:
```csharp
EntityGID a = entity1.GID;
EntityGID b = entity2.GID;

bool eq = a == b;           // Comparison by Raw (8 bytes)
bool eq = a.Equals(b);      // Same

// Cross-type comparison with EntityGIDCompact
EntityGIDCompact compact = entity1.GIDCompact;
bool eq = a == compact;     // Comparison by Id, Version, ClusterId
bool eq = a.Equals(compact);

// Explicit narrowing conversion to EntityGIDCompact
// Throws in DEBUG if Chunk >= 4 or ClusterId >= 4
EntityGIDCompact compact = (EntityGIDCompact)gid;
```

___

## EntityGIDCompact
Compact version of EntityGID — 4 bytes instead of 8, for memory-constrained scenarios
- Bit packing: `[31..16]` Version, `[15..14]` ClusterId (2 bits), `[13..12]` Chunk (2 bits), `[11..0]` entity index within chunk
- Limits: max 4 chunks (~16,384 entities), max 4 clusters
- Throws in DEBUG when exceeding limits

#### Obtaining:
```csharp
EntityGIDCompact gid = entity.GIDCompact;

// Explicit conversion Entity → EntityGIDCompact
EntityGIDCompact gid = (EntityGIDCompact)entity;

// Via constructor
EntityGIDCompact gid = new EntityGIDCompact(id: 0, version: 1, clusterId: 0);
EntityGIDCompact gid = new EntityGIDCompact(raw: 16777216U);
```

___

#### Validation and unpacking:
```csharp
// API is identical to EntityGID
GIDStatus status = gid.Status<WT>();

if (gid.TryUnpack<WT>(out var entity)) {
    // ...
}

var entity = gid.Unpack<WT>();

// Implicit widening conversion to EntityGID (always safe)
EntityGID full = gid;
```

___

## Usage examples

#### Events:
```csharp
public struct OnDamage : IEvent {
    public EntityGID Target;
    public float Amount;
}

// In a system:
foreach (var e in damageReceiver) {
    ref var data = ref e.Value;
    if (data.Target.TryUnpack<WT>(out var target)) {
        ref var health = ref target.Ref<Health>();
        health.Current -= data.Amount;
    }
}
```

#### Server-client networking:
GID can be used as an entity binding identifier between client and server.
The server creates an entity, sends the GID to the client, the client creates an entity with the same GID —
further commands with GID allow the client to easily find the needed entity via `TryUnpack`.

```csharp
public struct CreateEntityCommand {
    public EntityGID Id;
    public string Prefab;
}

// Server:
var serverEntity = W.NewEntity<Default>();
client.Send(new CreateEntityCommand { Id = serverEntity.GID, Prefab = "player" });

// Client:
var cmd = server.Receive<CreateEntityCommand>();
var clientEntity = ClientW.NewEntityByGID<Default>(cmd.Id);
```
