---
title: Serialization
parent: Features
nav_order: 15
---

### Serialization
Serialization is a mechanism for taking binary snapshots of the whole world or of specific entities  
Binary serialization uses [StaticPack](https://github.com/Felid-Force-Studios/StaticPack)

### How it works:

Let's define a few components:
> To support serialization in [component configuration](configs.md)
> GUID, Writer, and Reader must be defined as a minimum.

```csharp
using FFS.Libraries.StaticEcs;
using FFS.Libraries.StaticPack;

// Сomponent with reference data
public struct Name : IComponent {
    public string Value;

    public Name(string value) => Value = value;

    public override string ToString() => Value;

    public class Config<WorldType> : DefaultComponentConfig<Name, WorldType> where WorldType : struct, IWorldType {
        public override Guid Id() => new("531dc870-fdf5-4a8d-a4c6-b4911b1ea1c3");

        public override BinaryWriter<Name> Writer() => (ref BinaryPackWriter writer, in Name value) => writer.WriteString16(value.Value);

        public override BinaryReader<Name> Reader() => (ref BinaryPackReader reader) => new Name(reader.ReadString16());
    }
}

// Component with structures
public struct Position : IComponent {
    public float X, Y, Z;

    public Position(float x, float y, float z) {
        X = x; Y = y; Z = z;
    }

    public override string ToString() => $"X: {X}, Y: {Y}, Z: {Z}";

    public class Config<WorldType> : DefaultComponentConfig<Position, WorldType> where WorldType : struct, IWorldType {
        public override Guid Id() => new("b121594c-456e-4712-9b64-b75dbb37e611");

        public override BinaryWriter<Position> Writer() {
            return (ref BinaryPackWriter w, in Position value) => {
                w.WriteFloat(value.X);
                w.WriteFloat(value.Y);
                w.WriteFloat(value.Z);
            };
        }

        public override BinaryReader<Position> Reader() => (ref BinaryPackReader r) => 
            new Position(r.ReadFloat(), r.ReadFloat(), r.ReadFloat());

        public override IPackArrayStrategy<Position> ReadWriteStrategy() => new UnmanagedPackArrayStrategy<Position>();
    }
}

// Multi component
public struct Items : IMultiComponent<Items, int> {
    public Multi<int> Values;

    public ref Multi<int> RefValue(ref Items component) => ref component.Values;

    public override string ToString() => Values.ToString();

    public class Config<WorldType> : DefaultComponentConfig<Items, WorldType> where WorldType : struct, IWorldType {
        public override Guid Id() => new("c54de753-ff4e-4620-b2ce-6de5c4870db0");

        public override BinaryWriter<Items> Writer() => (ref BinaryPackWriter writer, in Items value) => writer.WriteMulti(value.Values);

        public override BinaryReader<Items> Reader() => (ref BinaryPackReader reader) => new Items {
            Values = reader.ReadMulti<WorldType, int>()
        };
    }
}

// Component-relationship
public struct Parent : IEntityLinkComponent<Parent> {
    public EntityGID Link;

    ref EntityGID IRefProvider<Parent, EntityGID>.RefValue(ref Parent component) => ref component.Link;
    
    public override string ToString() => Link.ToString();

    public class Config<WorldType> : DefaultComponentConfig<Parent, WorldType>
        where WorldType : struct, IWorldType {
        public override Guid Id() => new("90a9bb9a-6b86-4041-9a39-2682d5801881");

        public override BinaryWriter<Parent> Writer() => (ref BinaryPackWriter writer, in Parent value) => writer.Write(value.Link);

        public override BinaryReader<Parent> Reader() => (ref BinaryPackReader reader) => new Parent {
            Link = reader.Read<EntityGID>()
        };
    }
}

// Component-relationship
public struct Childs: IEntityLinksComponent<Childs> {
    public ROMulti<EntityGID> Links;

    ref ROMulti<EntityGID> IRefProvider<Childs, ROMulti<EntityGID>>.RefValue(ref Childs component) => ref component.Links;
    
    public override string ToString() => Links.ToString();

    public class Config<WorldType> : DefaultComponentConfig<Childs, WorldType>
        where WorldType : struct, IWorldType {
        public override Guid Id() => new("15c875b7-c35f-4e25-a040-e71c8b25103e");

        public override BinaryWriter<Childs> Writer() => (ref BinaryPackWriter writer, in Childs value) => writer.WriteROMulti(value.Links);

        public override BinaryReader<Childs> Reader() => (ref BinaryPackReader reader) => new Childs {
            Links = reader.ReadROMulti<WorldType, EntityGID>()
        };
    }
}

// Some kind of tags
public struct Tag1 : ITag {}
public struct Tag2 : ITag {}
public struct Tag3 : ITag {}
```

Let's define the world and the method of creation:

```csharp
public struct WT : IWorldType { }

public abstract class W : World<WT> { }

public static void InitWorld(GIDStoreSnapshot? snapshot = null) {
    W.Create(WorldConfig.Default());
    W.RegisterComponentType<Name>(new Name.Config<WT>());
    W.RegisterComponentType<Position>(new Position.Config<WT>());
    W.RegisterMultiComponentType<Items, int>(4, new Items.Config<WT>());
    W.RegisterOneToManyRelationType<Parent, Childs>(4, leftConfig: new Parent.Config<WT>(), rightConfig: new Childs.Config<WT>());

    W.RegisterTagType<Tag1>(new("3a6fe6a2-9427-43ae-9b4a-f8582e3a5f90"));
    W.RegisterTagType<Tag2>(new("d25b7a08-cbe6-4c77-bd8e-29ce7f748c30"));
    W.RegisterTagType<Tag3>(new("7f0cbf47-2ac3-4cd0-b5ec-b1f38d08c2aa"));
    
    // Let's define the debugging method
    Utils.EntityGidToString = gid => gid.TryUnpack<WT>(out var e) 
        ? $"{gid.Id()}:{gid.Version()} - {e.Ref<Name>().Value}" 
        : $"GID {gid.Id()} : Version {gid.Version()}";

    if (snapshot.HasValue) {
        W.Initialize(snapshot.Value);
    } else {
        W.Initialize();
    }
}
```

Let's define a method for creating test entities:

```csharp
public static void CreateEntities() {
    var alex = W.Entity.New();
    alex.Add<Name>().Value = "Alex";
    alex.Add<Position>() = new (1.22f, 77.23131f, 54.232f);
    alex.SetTag<Tag1>();
    ref var alexItems = ref alex.Add<Items>().Values;
    alexItems.Add(1);
    alexItems.Add(2);
    alexItems.Add(3);
    alex.Disable();
    
    var jack = W.Entity.New();
    jack.Add<Name>().Value = "Jack";
    jack.Add<Position>() = new (2.57f, 3.23131f, 5.232f);
    jack.SetTag<Tag3>();
    jack.SetLink<Parent>(alex);
    jack.Disable<Position>();
}
```

#### Let's look at some examples:

Example with saving and loading the full world

```csharp
InitWorld();
CreateEntities();
Console.WriteLine("Created entities:");
foreach (var entity in W.AllEntities()) {
    Console.WriteLine(entity.PrettyString);
}
// When saving a snapshot of the world, all entities and events are saved
// Saving the world to a byte array
byte[] snapshot = W.Serializer.CreateWorldSnapshot();
// Or saving the world to a file
// W.Serializer.CreateWorldSnapshot("Path/to/save/data/world.bin");
W.Destroy();


InitWorld();
// When loading a world snapshot, all entities and events are removed before loading, and loaded from the snapshot
// Loading a world from a byte array
W.Serializer.LoadWorldSnapshot(snapshot);
// Or download the world from a file
// W.Serializer.LoadWorldSnapshot("Path/to/save/data/world.bin");
Console.WriteLine("Loaded entities:");
foreach (var entity in W.AllEntities()) {
    Console.WriteLine(entity.PrettyString);
}
W.Destroy();
```

Example with saving and loading entities (as new ones)

```csharp
InitWorld();
CreateEntities();
Console.WriteLine("Created entities:");
// Creating an entity writer
// It writes down the necessary information for entity recovery
using var entitiesWriter = W.Serializer.CreateEntitiesSnapshotWriter();
foreach (var entity in W.AllEntities()) {
    // Write the entity
    entitiesWriter.Write(entity);
    Console.WriteLine(entity.PrettyString);
}
// Saving entities to a byte array
byte[] snapshot = entitiesWriter.CreateSnapshot();
// Or saving entities to a file
// entitiesWriter.CreateSnapshot("Path/to/save/data/entities.bin");
W.Destroy();

//   Created entities:                              
//   Entity ID: 1 GID: 1 Version: 1                              
//   Components:         
//    - [0] Name ( Jack )                                        
//    - [1] [Disabled] Position ( X: 2,57, Y: 3,23131, Z: 5,232 )
//    - [3] Parent ( 0:1 - Alex )                                
//   Tags:                                                       
//    - [2] Tag3                                                 
//   
//   Entity ID: 0 GID: 0 Version: 1 ( Disabled )                    
//   Components:       
//    - [0] Name ( Alex )                               
//    - [1] Position ( X: 1,22, Y: 77,23131, Z: 54,232 )
//    - [2] Items ( 1, 2, 3 )                           
//    - [4] Childs ( 1:1 - Jack )                       
//   Tags:                                              
//    - [0] Tag1   


InitWorld();
var someEntity1 = W.Entity.New(new Position(1, 2, 3), new Name("someEntity1"));
var someEntity2 = W.Entity.New(new Position(2, 3, 4), new Name("someEntity2"));

// entitiesAsNew indicates whether to load entities as new and assign a new EntityGID (more on this later)
// If entitiesAsNew = true, it means that all the component-relationships in the loaded entities may have incorrect values.
// Let's see how to avoid this in the following example
// Loading entities from a byte array
W.Serializer.LoadEntitiesSnapshot(snapshot, entitiesAsNew: true);
// Or loading entities from a file
// W.Serializer.LoadEntitiesSnapshot("Path/to/save/data/entities.bin", entitiesAsNew: true);

Console.WriteLine("Loaded entities:");
foreach (var entity in W.AllEntities()) {
    Console.WriteLine(entity.PrettyString);
}
W.Destroy();

// We can see that the Parent and Childs components in the loaded entities point to someEntity1 and someEntity2 instead of the desired entities
// We will fix this in the next example

//  Loaded entities:
//  Entity ID: 3 GID: 3 Version: 1
//  Components:
//   - [0] Name ( Alex )
//   - [1] Position ( X: 1,22, Y: 77,23131, Z: 54,232 )
//   - [2] Items ( 1, 2, 3 )
//   - [4] Childs ( 1:1 - someEntity2 )
//  Tags:
//   - [0] Tag1
//  
//  Entity ID: 2 GID: 2 Version: 1
//  Components:
//   - [1] Name ( Jack )
//   - [1] [Disabled] Position ( X: 2,57, Y: 3,23131, Z: 5,232 )
//   - [3] Parent ( 0:1 - someEntity1 )
//  Tags:
//   - [2] Tag3

//  Created entities:                              
//  Entity ID: 1 GID: 1 Version: 1
//  Components:
//   - [0] Name ( someEntity2 )
//   - [1] Position ( X: 2, Y: 3, Z: 4 )
//  Tags:
//  
//  Entity ID: 0 GID: 0 Version: 1
//  Components:
//   - [0] Name ( someEntity1 )
//   - [1] Position ( X: 1, Y: 2, Z: 3 )
//  Tags:
```

Example with saving and loading entities (with global identifiers preserved)

```csharp
InitWorld();
CreateEntities();
Console.WriteLine("Created entities:");
using var entitiesWriter = W.Serializer.CreateEntitiesSnapshotWriter();
foreach (var entity in W.AllEntities()) {
    entitiesWriter.Write(entity);
    Console.WriteLine(entity.PrettyString);
}
byte[] snapshot = entitiesWriter.CreateSnapshot();
// Save the global identifiers repository into a separate array\file
// Global identifiers repository, contains sequence and all information about issued identifiers
// which makes it possible not to use identifiers of entities that are currently unloaded.
byte[] gidSnapshot = W.Serializer.CreateGIDStoreSnapshot();
W.Destroy();

//   Created entities:                              
//   Entity ID: 1 GID: 1 Version: 1                              
//   Components:         
//    - [0] Name ( Jack )                                        
//    - [1] [Disabled] Position ( X: 2,57, Y: 3,23131, Z: 5,232 )
//    - [3] Parent ( 0:1 - Alex )                                
//   Tags:                                                       
//    - [2] Tag3                                                 
//   
//   Entity ID: 0 GID: 0 Version: 1 ( Disabled )                    
//   Components:       
//    - [0] Name ( Alex )                               
//    - [1] Position ( X: 1,22, Y: 77,23131, Z: 54,232 )
//    - [2] Items ( 1, 2, 3 )                           
//    - [4] Childs ( 1:1 - Jack )                       
//   Tags:                                              
//    - [0] Tag1


// Load the global identifier store from a file and initialize the world with it
var gidStoreSnapshot = BinaryPack.ReadFromBytes<GIDStoreSnapshot>(gidSnapshot);
InitWorld(gidStoreSnapshot);
var someEntity1 = W.Entity.New(new Position(1, 2, 3), new Name("someEntity1"));
var someEntity2 = W.Entity.New(new Position(2, 3, 4), new Name("someEntity2"));

// We can now specify entitiesAsNew: false because the entity identifiers were restored when the world was initialized
W.Serializer.LoadEntitiesSnapshot(snapshot, entitiesAsNew: false);

Console.WriteLine("Loaded entities:");
foreach (var entity in W.AllEntities()) {
    Console.WriteLine(entity.PrettyString);
}
W.Destroy();

// Now we can see that all links between entities are correct
// Since when creating someEntity1 and someEntity2, the identifiers of the entities to be loaded were not used
// This approach allows loading and saving different bundles of entities, for example, when streaming the world or loading different locations.
// and ensure that the saved identifiers in components and events are not mixed up.

//  Loaded entities:
//  Entity ID: 3 GID: 0 Version: 1 ( Disabled )
//  Components:
//   - [0] Name ( Alex )
//   - [1] Position ( X: 1,22, Y: 77,23131, Z: 54,232 )
//   - [2] Items ( 1, 2, 3 )
//   - [4] Childs ( 1:1 - Jack )
//  Tags:
//   - [0] Tag1
//  
//  Entity ID: 2 GID: 1 Version: 1
//  Components:
//   - [1] Name ( Jack )
//   - [1] [Disabled] Position ( X: 2,57, Y: 3,23131, Z: 5,232 )
//   - [3] Parent ( 0:1 - Alex )
//  Tags:
//   - [2] Tag3

//  Created entities:                              
//  Entity ID: 1 GID: 3 Version: 1
//  Components:
//   - [0] Name ( someEntity2 )
//   - [1] Position ( X: 2, Y: 3, Z: 4 )
//  Tags:
//  
//  Entity ID: 0 GID: 2 Version: 1
//  Components:
//   - [0] Name ( someEntity1 )
//   - [1] Position ( X: 1, Y: 2, Z: 3 )
//  Tags:
```

### Q&A:

- I have changed the order\type of data in a component, can I download a snapshot of the old version of the world?

```csharp
// To load a snapshot you need to write a migration of an old version component to a new one
// Example:
// Let's imagine that originally there was a position component with X and Y
public struct Position : IComponent {
    public float X, Y;

    public class Config<WorldType> : DefaultComponentConfig<Position, WorldType> where WorldType : struct, IWorldType {
        public override Guid Id() => new("b121594c-456e-4712-9b64-b75dbb37e611");

        public override BinaryWriter<Position> Writer() {
            return (ref BinaryPackWriter w, in Position value) => {
                w.WriteFloat(value.X);
                w.WriteFloat(value.Y);
            };
        }

        public override BinaryReader<Position> Reader() => (ref BinaryPackReader r) => 
            new Position(r.ReadFloat(), r.ReadFloat());
    }
}

// Then the Z coordinate appeared and we need to bring up the component version and write a migration
public struct Position : IComponent {
    public float X, Y, Z;

    public class Config<WorldType> : DefaultComponentConfig<Position, WorldType> where WorldType : struct, IWorldType {
        public override Guid Id() => new("b121594c-456e-4712-9b64-b75dbb37e611");

        public override BinaryWriter<Position> Writer() {
            return (ref BinaryPackWriter w, in Position value) => {
                w.WriteFloat(value.X);
                w.WriteFloat(value.Y);
                w.WriteFloat(value.Z); // Actualizing the writer for Z
            };
        }

        public override BinaryReader<Position> Reader() => (ref BinaryPackReader r) => 
            new Position(r.ReadFloat(), r.ReadFloat(), r.ReadFloat()); // Actualizing the reader for Z

        // Change the version to the following (default version is 0)
        public override byte Version() => 1;

        // Write a migration where for version 0 (old) we read only X and Y and set Z to 0
        public override EcsComponentMigrationReader<Position, WorldType> MigrationReader() {
            return (ref BinaryPackReader reader, World<WorldType>.Entity entity, byte version, bool disabled) => {
                if (version == 0) {
                    return new Position(reader.ReadFloat(), reader.ReadFloat(), 0);
                }

                throw new Exception("Unknown version");
            };
        }
    }
}
```

- I have deleted/added a component, can I upload a snapshot of the version world before the change?

```csharp
// If new components are added, the old snapshot should load correctly, restoration will happen automatically
// By default, if a component has been deleted, it will be skipped automatically when loading, nothing additional is required

// If you need to handle deletion in a special way, you need to register a function with the GUID of the old component

// Example for components
W.Serializer.SetComponentDeleteMigrator(
    new("3a6fe6a2-9427-43ae-9b4a-f8582e3a5f90"),
    (ref BinaryPackReader reader, World<WT>.Entity entity, byte version, bool disabled) => {
        // Here you need to read ALL the data correctly and execute the custom logic
    }
);

// Example for tags
W.Serializer.SetTagDeleteMigrator(
    new("3a6fe6a2-9427-43ae-9b4a-f8582e3a5f90"),
    (World<WT>.Entity entity) => {
        // Здесь необходимо выполнить кастомную логику
    }
);

// Example for events
W.Serializer.SetEventDeleteMigrator(
    new("3a6fe6a2-9427-43ae-9b4a-f8582e3a5f90"),
    (ref BinaryPackReader reader, byte version) => {
        // Here you need to read ALL the data correctly and execute the custom logic
    });
```


- Can components/masks/tags be excluded from serialization?

```csharp
// When using serialization via the CreateWorldSnapshot method, the full state of the world is saved
// and there is no possibility to exclude individual components (DEBUG will have an error when calling that the serializer is not registered).

// But when using EntitiesSnapshot there is such a possibility, for this purpose it is necessary not to configure GUID when registering a component/tag/event.
// When saving entities, all components/tags/events for which GUID is not defined will be skipped during serialization.
// For example, to preserve the WHOLE world including events and relationships between entities, the following code can be used:
using var entitiesWriter = W.Serializer.CreateEntitiesSnapshotWriter();
entitiesWriter.WriteAllEntities();
byte[] snapshot = entitiesWriter.CreateSnapshot();
byte[] gidSnapshot = W.Serializer.CreateGIDStoreSnapshot();
byte[] eventsSnapshot = W.Events.CreateSnapshot();


// Deserialization:
var gidStoreSnapshot = BinaryPack.ReadFromBytes<GIDStoreSnapshot>(gidSnapshot);
InitWorld(gidStoreSnapshot);
W.Serializer.LoadEntitiesSnapshot(snapshot, entitiesAsNew: false);
W.Events.LoadSnapshot(eventsSnapshot);
```

- Is it possible to reduce the size of serialized data/files?

```csharp
// GZIP compression is available out of the box and can be applied as follows
byte[] snapshot = W.Serializer.CreateWorldSnapshot(gzip: true);
W.Serializer.CreateWorldSnapshot("Path/to/save/data/world.bin", gzip: true);

// If the array or file was compressed, it is also necessary to specify in parameters
W.Serializer.LoadWorldSnapshot(snapshot, gzip: true);
W.Serializer.LoadWorldSnapshot("Path/to/save/data/world.bin", gzip: true);
```

- How to automate the response to save/load the world\entities?

```csharp
// You can register any number of callbacks

// Before creating a snapshot
W.Serializer.RegisterPreCreateSnapshotCallback(() => Console.WriteLine("Entities or world `CreateSnapshot` start"));
// After creating a snapshot
W.Serializer.RegisterPostCreateSnapshotCallback(() => Console.WriteLine("Entities or world `CreateSnapshot` finish"));

// Before loading a snapshot
W.Serializer.RegisterPreLoadSnapshotCallback(() => Console.WriteLine("Entities or world `LoadSnapshot` start"));
// After loading a snapshot
W.Serializer.RegisterPostLoadSnapshotCallback(() => Console.WriteLine("Entities or world `LoadSnapshot` finish"));

// These functions will be called when use
// W.Serializer.CreateWorldSnapshot(), W.Serializer.LoadWorldSnapshot(snapshot) или entitiesWriter.CreateSnapshot(), W.Serializer.LoadEntitiesSnapshot()

// How can I make functions be called when saving/loading only the world or only entities?
// When registering a callbacks, you can pass SnapshotActionType as the second parameter, e.g.
W.Serializer.RegisterPreCreateSnapshotCallback(() => Console.WriteLine("Entities `CreateSnapshot` start"), SnapshotActionType.Entities);
W.Serializer.RegisterPostCreateSnapshotCallback(() => Console.WriteLine("World `CreateSnapshot` finish"), SnapshotActionType.World);
```

- How to perform post processing of saved/loaded entities?

```csharp
// You can register any number of callbacks for entities

// After creating a snapshot
W.Serializer.RegisterPostCreateSnapshotEachEntityCallback(entity => Console.WriteLine($"Saved {entity.PrettyString}"));
// After loading a snapshot
W.Serializer.RegisterPostLoadSnapshotEachEntityCallback(entity => Console.WriteLine($"Loaded {entity.PrettyString}"));

// These functions will be called when use
// W.Serializer.CreateWorldSnapshot(), W.Serializer.LoadWorldSnapshot(snapshot) или entitiesWriter.CreateSnapshot(), W.Serializer.LoadEntitiesSnapshot()

// How can I make functions be called when saving/loading only the world or only entities?
// When registering a callbacks, you can pass SnapshotActionType as the second parameter, e.g.
W.Serializer.RegisterPostCreateSnapshotEachEntityCallback(entity => Console.WriteLine($"Saved {entity.PrettyString}"), SnapshotActionType.Entities);
W.Serializer.RegisterPostLoadSnapshotEachEntityCallback(entity => Console.WriteLine($"Loaded {entity.PrettyString}"), SnapshotActionType.World);


// You can also pass the entity post-processing function to a method when loading an entity snapshot
W.Serializer.LoadEntitiesSnapshot(snapshot, entitiesAsNew: true, onLoad: entity => {
    Console.WriteLine($"Loaded {entity.PrettyString}");
});
```

- How do I add special data to a snapshot of the world\entities (e.g. data from systems or services)?

```csharp
// Any number of custom handlers can be registered
// For example:
W.Serializer.SetSnapshotHandler(
    new ("57c15483-988a-47e7-919c-51b9a7b957b5"),      // Unique data type guid
    version: 0,                                        // Version
    (ref BinaryPackWriter writer) => {                 // Custom Data Writer
        writer.WriteDateTime(DateTime.Now);
        Console.WriteLine("Saved current time");
    },
    (ref BinaryPackReader reader, ushort version) => { // Custom Data Reader
        var time = reader.ReadDateTime();
        Console.WriteLine($"Save dateTime is {time}");
    },
    SnapshotActionType.All                            // Type of snapshot for which this handler will be used
);
```

- How do I add special data for each entity in the entity world snapshot?

```csharp
// You can register any number of custom handlers for entities
// For example:
W.Serializer.SetSnapshotHandlerEachEntity(
    new ("57c15483-988a-47e7-919c-51b9a7b957b5"),       // Unique data type guid
    version: 0,                                         // Version
    (ref BinaryPackWriter writer, W.Entity entity) => {
        // Write custom entity data
    },
    (ref BinaryPackReader reader, W.Entity entity, ushort version) => {
        // Read custom entity data
    },
    SnapshotActionType.All                             // Type of snapshot for which this handler will be used
);
```

- Can I save and download event data?

```csharp
// Loading and saving of events is performed via methods
 W.Events.CreateSnapshot();
 W.Events.LoadSnapshot();
```