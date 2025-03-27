---
title: Packed entity
parent: Main types
nav_order: 2
---

## PackedEntity
Packed entity - stores meta information of the entity, serves for secure transfer of the entity (e.g. in events, components, etc.).
- Represented as an 8 byte structure

{: .note }
> An entity is just an id, a packaged entity is id + version  
> Just by id it is impossible to determine whether this entity that is now in the world under this identifier or not, you can only together with the version, for this purpose packaged version

___

#### Creation:
```csharp
// Creation is only possible through an unpackaged entity
PackedEntity packedEntity = entity.Pack();
```

___

#### Basic operations:
```csharp
PackedEntity packedEntity = entity.Pack();
// Attempt to unpack an entity in the world whose identifier is specified via the type parameter, returns true if the entity is successfully unpacked, in out unpacked entity
if (packedEntity.TryUnpack<WorldType>(out var unpackedEntity)) {
    // ...
}

PackedEntity packedEntity2 = entity.Pack();
bool equals = packedEntity.Equals(packedEntity2);     // Verify the identity of the packaged entities
```