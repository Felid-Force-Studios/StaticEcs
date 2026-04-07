---
title: Migration to 2.0.0
parent: EN
nav_order: 5
---

# Migration from 1.2.x to 2.0.0

Version 2.0.0 is a complete framework restructuring. Virtually all user code will require changes.

___

## Overview of changes

- **Segment storage model** — new hierarchy level: Chunk → Segment (256 entities) → Block → Entity
- **entityType** (`IEntityType`) — logical entity grouping for cache locality via generic type parameter
- **Hooks in IComponent/IEvent** — via default interface methods instead of separate Config classes
- **Single ISystem** instead of IInitSystem/IUpdateSystem/IDestroySystem
- **Unified Query** — `Query.Entities<>()` and `Query.For()` merged into `Query<>()`
- **Relations** — IEntityLinkComponent → ILinkType + Link\<T\>/Links\<T\>
- **Context → Resources** — `Context.Set/Get` → `SetResource`/`GetResource`
- **Tags unified with components** — tags stored in `Components<T>` with `IsTag` flag, use same query filters (`All<>`, `None<>`, `Any<>`)
- **Properties instead of methods** for Entity and World state
- **Query batch rename**: `DestroyAllEntities()` → `BatchDestroy()`, new `BatchUnload()`
- **Removed**: `UnloadCluster()`/`UnloadChunk()` — use `Query().BatchUnload()` with cluster/chunk filtering instead

___

## 0. Query Batch Operations Rename

#### `DestroyAllEntities()` → `BatchDestroy()`:
```csharp
// Was:
W.Query<All<Health>, TagAll<IsDead>>().DestroyAllEntities();

// Now:
W.Query<All<Health, IsDead>>().BatchDestroy();
```

#### New: `BatchUnload()` — batch unload entities matching a filter:
```csharp
W.Query<All<Position>>().BatchUnload();
```

#### Removed: `UnloadCluster()` / `UnloadChunk()` — use `BatchUnload()` with filtering:
```csharp
// Was:
W.UnloadCluster(clusterId);
W.UnloadChunk(chunkIdx);

// Now:
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { clusterId };
W.Query().BatchUnload(EntityStatusType.Any, clusters: clusters);

ReadOnlySpan<uint> chunks = stackalloc uint[] { chunkIdx };
W.Query().BatchUnload(EntityStatusType.Any, chunks);
```

___

## 1. World API

Details: [World](features/world.md)

#### Methods → Properties:
```csharp
// Was:                                Now:
W.IsInitialized()                  →  W.IsWorldInitialized
W.IsIndependent()                  →  W.IsIndependent
                                      W.Status  // new (WorldStatus enum)
```

#### Entity creation:

Details: [Entity](features/entity.md)

```csharp
// Was:
var entity = W.Entity.New(clusterId);
var entity = W.Entity.New<Position>(new Position());
W.Entity.NewOnes(count, onCreate, clusterId);
bool ok = W.Entity.TryNew(out entity, clusterId);

// Now:
var entity = W.NewEntity<Default>(clusterId: 0);
var entity = W.NewEntity<Default>(new Default(), clusterId: 0);
W.NewEntities<Default>(count: 100, clusterId: 0, onCreate: null);
bool ok = W.TryNewEntity<Default>(out entity, clusterId: 0);
var entity = W.NewEntity<Default>();  // Default entity type, clusterId=0
```

#### WorldConfig:
```csharp
// All fields are now nullable — unset values fall back to WorldConfig.Default()
// ParallelQueryType enum removed → use ThreadCount (uint?)
//   0 = single-threaded (default)
//   WorldConfig.MaxThreadCount = all available CPU threads
//   N = specific number of threads
// CustomThreadCount removed → use ThreadCount directly

// Factory methods:
WorldConfig.Default()      // standard settings
WorldConfig.MaxThreads()   // all available threads
```

#### Config types (ComponentTypeConfig, TagTypeConfig, EventTypeConfig):
```csharp
// All config fields are now nullable — unset values fall back to defaults
// Guid is auto-computed from type name (no need to specify manually)
// ReadWriteStrategy is auto-detected (UnmanagedPackArrayStrategy for unmanaged types)
// 3-level merge: user config → static Config field → built-in defaults
```

#### Removed:
- `ParallelQueryType` enum → `WorldConfig.ThreadCount`
- `WorldConfig.CustomThreadCount` → `WorldConfig.ThreadCount`
- `IWorld` interface → `WorldHandle`
- `WorldWrapper<W>` → `WorldHandle`
- `Worlds` static class
- `BoxedEntity<W>` / `IEntity`

___

## 2. Entity API

Details: [Entity](features/entity.md), [Entity global ID](features/gid.md)

#### Methods → Properties:
```csharp
// Was:                 Now:
entity.Gid()        →  entity.GID
entity.GidCompact() →  entity.GIDCompact
entity.IsNotDestroyed()→ entity.IsNotDestroyed  // method → property
                         entity.IsDestroyed     // NEW property
entity.IsDisabled() →  entity.IsDisabled
entity.IsEnabled()  →  entity.IsEnabled
entity.Version()    →  entity.Version
entity.ClusterId()  →  entity.ClusterId
entity.Chunk()      →  entity.ChunkID
entity.IsSelfOwned()→  entity.IsSelfOwned
```

#### New properties:
```csharp
entity.EntityType   // byte — entity type ID (from IEntityType registration)
entity.ID           // raw slot index
```

#### Component presence checks:

Details: [Component](features/component.md), [Tag](features/tag.md)

```csharp
// Was:                                Now:
entity.HasAllOf<C>()              →   entity.Has<C>()
entity.HasAllOf<C1, C2>()         →   entity.Has<C1, C2>()
entity.HasAnyOf<C1, C2>()         →   entity.HasAny<C1, C2>()
entity.HasDisabledAllOf<C>()      →   entity.HasDisabled<C>()
entity.HasEnabledAllOf<C>()       →   entity.HasEnabled<C>()

// Tags:
entity.HasAllOfTags<T>()          →   entity.Has<T>()
entity.HasAnyOfTags<T1, T2>()     →   entity.HasAny<T1, T2>()
```

#### Add — new semantics:

Details: [Component — Adding](features/component.md#adding-components)

```csharp
// ═══ Was (v1.2.x) ═══
entity.Add<C>();                    // ASSERT component doesn't exist
ref var c = ref entity.TryAdd<C>(); // idempotent
entity.Put(new Position(1, 2));     // upsert without hooks

// ═══ Now (v2.0.0) ═══
ref var c = ref entity.Add<C>();              // idempotent (former TryAdd)
ref var c = ref entity.Add<C>(out bool isNew);// with flag
entity.Set(new Position(1, 2));               // ALWAYS OnDelete→replace→OnAdd
```

| Old method | New equivalent |
|---|---|
| `entity.TryAdd<C>()` | `entity.Add<C>()` |
| `entity.TryAdd<C>(out bool)` | `entity.Add<C>(out bool isNew)` |
| `entity.Put<C>(value)` | `entity.Set<C>(value)` (but now with hooks) |
| `entity.Add<C>()` (old, assert) | No equivalent |

#### Delete/Disable/Enable — return bool:
```csharp
// Was:
entity.Delete<C>();               // void, assert
bool ok = entity.TryDelete<C>();  // bool

// Now:
bool deleted = entity.Delete<C>();              // bool (former TryDelete)
ToggleResult disabled = entity.Disable<C>();   // ToggleResult: MissingComponent, Unchanged, Changed
ToggleResult enabled = entity.Enable<C>();     // ToggleResult: MissingComponent, Unchanged, Changed
```

#### New methods:
```csharp
entity.Clone(clusterId);                       // clone to cluster
entity.MoveTo(clusterId);                      // move to cluster
```

#### Removed:
- `entity.Box()` / `BoxedEntity<W>` / `IEntity`
- `entity.TryAdd<C>()` → use `entity.Add<C>()`
- `entity.Put<C>(val)` → use `entity.Set<C>(val)`
- `entity.TryDelete<C>()` → use `entity.Delete<C>()`
- `entity.TryCopyComponentsTo<C>(target)` → `entity.CopyTo<C>(target)` (returns bool)
- `entity.TryMoveComponentsTo<C>(target)` → `entity.MoveTo<C>(target)` (returns bool)
- All Raw methods (`RawHasAllOf`, `RawAdd`, `RawGet`, `RawPut`, etc.)
- `Entity.New(...)` (all overloads) → `W.NewEntity(...)`
- `W.OnCreateEntity(callback)` → use `IEntityType.OnCreate` hook or `Created` tracking filter

___

## 3. EntityGID API

See: [Global Identifier](features/gid.md)

```csharp
// ═══ Was (v1.2.x) ═══
bool ok = gid.IsActual<WT>();
bool loaded = gid.IsLoaded<WT>();
bool both = gid.IsLoadedAndActual<WT>();

// ═══ Now (v2.0.0) ═══
GIDStatus status = gid.Status<WT>();
// GIDStatus.Active     — entity exists, version matches, loaded (was IsLoadedAndActual)
// GIDStatus.NotActual  — entity doesn't exist or version/cluster mismatch (was !IsActual)
// GIDStatus.NotLoaded  — entity exists, version matches, but unloaded (was IsActual && !IsLoaded)
```

| Old method | New equivalent |
|---|---|
| `gid.IsActual<WT>()` | `gid.Status<WT>() != GIDStatus.NotActual` |
| `gid.IsLoaded<WT>()` | `gid.Status<WT>() != GIDStatus.NotLoaded` |
| `gid.IsLoadedAndActual<WT>()` | `gid.Status<WT>() == GIDStatus.Active` |

#### Entity creation by GID:
```csharp
// Was:
var entity = W.Entity.New(someGid);

// Now:
var entity = W.NewEntityByGID<Default>(someGid);
```

___

## 4. Components

Details: [Component](features/component.md)

#### Hooks — in IComponent via default interface methods:
```csharp
// ═══ Was (v1.2.x) ═══
struct Position : IComponent { public float X, Y; }

class PositionConfig : IComponentConfig<Position, WT> {
    public OnComponentHandler<Position> OnAdd() => (ref Position c, Entity e) => { };
    public Guid Id() => ...;
    public BinaryWriter<Position> Writer() => ...;
    public BinaryReader<Position> Reader() => ...;
}
W.RegisterComponentType<Position>(new PositionConfig());

// ═══ Now (v2.0.0) ═══
struct Position : IComponent {
    public float X, Y;

    public void OnAdd<TWorld>(World<TWorld>.Entity self)
        where TWorld : struct, IWorldType { }

    public void OnDelete<TWorld>(World<TWorld>.Entity self, HookReason reason)
        where TWorld : struct, IWorldType { }

    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType { writer.WriteFloat(X); writer.WriteFloat(Y); }

    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled)
        where TWorld : struct, IWorldType { X = reader.ReadFloat(); Y = reader.ReadFloat(); }
}

W.Types().Component<Position>(new ComponentTypeConfig<Position>(
    guid: new Guid("..."),
    version: 0,
    readWriteStrategy: new UnmanagedPackArrayStrategy<Position>()
));
```

#### Removed:
- `IComponentConfig<T, W>`, `DefaultComponentConfig<T, W>`, `ValueComponentConfig<T, W>`
- `OnComponentHandler<T>`, `OnCopyHandler<T>` delegates
- `OnPut` hook (in v2 `Add(value)` calls OnDelete+OnAdd)

#### Pool access:
```csharp
// Was:                                Now:
Components<T>.Value.Ref(entity)   →   Components<T>.Instance.Ref(entity)
Components<T>.Value.Has(entity)   →   Components<T>.Instance.Has(entity)
Components<T>.Value.IsRegistered()→   Components<T>.Instance.IsRegistered  // property
Components<T>.Value.DynamicId()   →   Components<T>.Instance.DynamicId    // property
```

___

## 5. Tags

Details: [Tag](features/tag.md)

Tags are now unified with components internally. Tags are stored in `Components<T>` with an `IsTag` flag.

#### Entity methods:
```csharp
// Was:                                Now:
entity.SetTag<T>()                →   entity.Set<T>()
entity.HasTag<T>()                →   entity.Has<T>()
entity.HasAnyTags<T1,T2>()       →   entity.HasAny<T1,T2>()
entity.DeleteTag<T>()             →   entity.Delete<T>()
entity.ToggleTag<T>()             →   entity.Toggle<T>()
entity.ApplyTag<T>(bool)          →   entity.Apply<T>(bool)
entity.CopyTagsTo<T>(target)     →   entity.CopyTo<T>(target)
entity.MoveTagsTo<T>(target)     →   entity.MoveTo<T>(target)
```

#### Pool access:
```csharp
// Was:                                Now:
Tags<T>.Value                     →   Components<T>.Instance (IsTag = true)
```

#### Query filters:
```csharp
// Was:                                Now:
TagAll<T>                         →   All<T>
TagNone<T>                        →   None<T>
TagAny<T1,T2>                    →   Any<T1,T2>

#### Removed:
- `TagsHandle` → use `ComponentsHandle` (with `IsTag` field)
- `WorldConfig.BaseTagTypesCount` → tags count towards `BaseComponentTypesCount`
- `DeleteTagsSystem<W, T>` → use `Query().BatchDelete<T>()`

___

## 6. Queries

Details: [Query](features/query.md)

#### Unified entry point:
```csharp
// ═══ Was (v1.2.x) ═══
foreach (var entity in W.Query.Entities<All<Position>>()) { }

W.Query.For((ref Position pos) => {
    pos.X += 1;
});

// ═══ Now (v2.0.0) ═══
foreach (var entity in W.Query<All<Position>>().Entities()) { }

W.Query().For(
    static (ref Position pos) => { pos.X += 1; }
);
```

#### Entity search:
```csharp
// Was:
W.Query.Entities<All<P>>().First(out entity);

// Now:
W.Query<All<P>>().Any(out entity);
```

#### Batch operations:
```csharp
// Was (on QueryEntitiesIterator):
W.Query.Entities<All<P>>().AddForAll<Health>();
W.Query.Entities<All<P>>().DeleteForAll<Health>();
W.Query.Entities<All<P>>().SetTagForAll<Active>();

// Now (on WorldQuery, with chaining):
W.Query<All<P>>().BatchAdd<Health>().BatchSet<Active>();
W.Query<All<P>>().BatchDelete<Health>();
W.Query<All<P>>().BatchDisable<Health>();   // new
W.Query<All<P>>().BatchEnable<Health>();    // new
W.Query<All<P>>().BatchDestroy();
W.Query<All<P>>().BatchUnload();    // new
```

#### Filter wrapper With → And:
```csharp
// Was:
W.Query.Entities<With<All<Pos>, None<Name>>>();

// Now:
W.Query<And<All<Pos>, None<Name>>>();
// Also available: Or<> for filter disjunction (new)
```

#### Parallel iteration:
```csharp
// Was:
W.Query.Parallel.For(minChunkSize: 50000, (W.Entity ent, ref Position pos) => { });

// Now:
W.Query().ForParallel(
    static (W.Entity ent, ref Position pos) => { },
    minEntitiesPerThread: 50000
);
```

#### QueryMode:

Details: [Performance — QueryMode](performance.md#querymode)

```csharp
// Was: runtime parameter when creating iterator
W.Query.Entities<All<P>>(queryMode: QueryMode.Flexible);

// Now: default is Strict, for Flexible — separate methods
foreach (var entity in W.Query<All<P>>().EntitiesFlexible()) { }

W.Query().For(
    static (ref Position pos) => { },
    queryMode: QueryMode.Flexible
);
```

___

## 7. Relations

Details: [Relations](features/relations.md)

```csharp
// ═══ Was (v1.2.x) ═══
struct Parent : IEntityLinkComponent<Parent> {
    public EntityGID Link;
    ref EntityGID IRefProvider<Parent, EntityGID>.RefValue(ref Parent c) => ref c.Link;
}
W.RegisterToOneRelationType<Parent>(config);

// ═══ Now (v2.0.0) ═══
struct ParentLink : ILinkType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType { }
    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType { }
}
W.Types()
    .Link<ParentLink>()    // single link
    .Links<ParentLink>();  // multiple links

// Usage:
entity.Set(new W.Link<ParentLink>(parentEntity));
ref var links = ref entity.Ref<W.Links<ChildLink>>();
```

| Old type | New equivalent |
|---|---|
| `IEntityLinkComponent<T>` | `ILinkType` + `Link<T>` |
| `IEntityLinksComponent<T>` | `ILinksType` + `Links<T>` |
| `RegisterToOneRelationType` | `Types().Link<T>()` |
| `RegisterToManyRelationType` | `Types().Links<T>()` |
| `RegisterOneToManyRelationType` | Separately: `Types().Link<T>().Links<T>()` |

___

## 8. Events

Details: [Events](features/events.md)

```csharp
// ═══ Was (v1.2.x) ═══
struct MyEvent : IEvent { public int Data; }
class MyEventConfig : IEventConfig<MyEvent, WT> { ... }
W.Events.RegisterEventType<MyEvent>(new MyEventConfig());
W.Events.Send(new MyEvent { Data = 1 });
var receiver = W.Events.RegisterEventReceiver<MyEvent>();
W.Events.DeleteEventReceiver(ref receiver);

// ═══ Now (v2.0.0) ═══
struct MyEvent : IEvent {
    public int Data;
    public void Write(ref BinaryPackWriter writer) { writer.WriteInt(Data); }
    public void Read(ref BinaryPackReader reader, byte version) { Data = reader.ReadInt(); }
}
W.Types().Event<MyEvent>(new EventTypeConfig<MyEvent>(guid: new Guid("...")));
W.SendEvent(new MyEvent { Data = 1 });
var receiver = W.RegisterEventReceiver<MyEvent>();
W.DeleteEventReceiver(ref receiver);
```

- `IEventConfig<T, W>` removed → `EventTypeConfig<T>` + hooks in `IEvent`
- `W.Events.XXX` → `W.XXX` (methods moved to World)
- `IsClearable()` → `NoDataLifecycle` (inverted logic, expanded semantics — controls both initialization and cleanup)

___

## 9. Systems

Details: [Systems](features/systems.md)

```csharp
// ═══ Was (v1.2.x) ═══
struct MoveSystem : IUpdateSystem {
    public void Update() { }
}
Systems.AddUpdate(new MoveSystem());
Systems.AddCallOnce(new InitSystem());

// ═══ Now (v2.0.0) ═══
struct MoveSystem : ISystem {
    public void Init() { }          // replaces IInitSystem
    public void Update() { }        // replaces IUpdateSystem
    public bool UpdateIsActive() => true; // replaces ISystemCondition
    public void Destroy() { }       // replaces IDestroySystem
}
GameSys.Add(new MoveSystem(), order: 0);
```

| Old type | New equivalent |
|---|---|
| `IInitSystem` | `ISystem.Init()` |
| `IUpdateSystem` | `ISystem.Update()` |
| `IDestroySystem` | `ISystem.Destroy()` |
| `ISystemCondition` | `ISystem.UpdateIsActive()` |
| `Systems.AddUpdate(sys)` | `Sys.Add(sys, order)` |
| `Systems.AddCallOnce(sys)` | `Sys.Add(sys, order)` + `Init()` |

___

## 10. Multi-components

Details: [Multi-component](features/multicomponent.md)

```csharp
// ═══ Was (v1.2.x) ═══
struct Items : IMultiComponent<Items, int> {
    public Multi<int> Values;
    public ref Multi<int> RefValue(ref Items c) => ref c.Values;
}
W.RegisterMultiComponentType<Items, int>(4, config);

// ═══ Now (v2.0.0) ═══
struct Items : IMultiComponent {  // marker interface, no RefValue
    public Multi<int> Values;
}
W.Types().Multi<Items>();  // multi-component registration
```

- `IMultiComponent<T, V>` → `IMultiComponent` (marker)
- `RefValue()` removed
- `RegisterMultiComponentType` → `W.Types().Multi<T>()` (not `Component<T>`!)
- `Count` → `Length`
- `IsEmpty()`/`IsNotEmpty()`/`IsFull()` → properties

___

## 11. Serialization

Details: [Serialization](features/serialization.md)

```csharp
// ═══ Was (v1.2.x) ═══
class PositionConfig : DefaultComponentConfig<Position, WT> {
    public override BinaryWriter<Position> Writer() => ...;
    public override BinaryReader<Position> Reader() => ...;
}
W.Events.CreateSnapshot();
W.Events.LoadSnapshot(snapshot);

// ═══ Now (v2.0.0) ═══
// Write/Read hooks directly on IComponent/IEvent
struct Position : IComponent {
    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType { }
    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled)
        where TWorld : struct, IWorldType { }
}
W.Serializer.CreateEventsSnapshot();
W.Serializer.LoadEventsSnapshot(snapshot);
```

___

## 12. Context → Resources

Details: [Resources](features/resources.md)

```csharp
// ═══ Was (v1.2.x) ═══
W.Context.Set<GameTime>(new GameTime());
ref var time = ref W.Context.Get<GameTime>();
bool has = W.Context.Has<GameTime>();

// ═══ Now (v2.0.0) ═══
W.SetResource(new GameTime());  // Set overwrites without error (replaces Replace)
ref var time = ref W.GetResource<GameTime>();
bool has = W.HasResource<GameTime>();
W.RemoveResource<GameTime>();

// Named resources (new):
W.SetResource("key", new GameConfig());
ref var cfg = ref W.GetResource<GameConfig>("key");
```

___

## Rename table

| Was (v1.2.x) | Now (v2.0.0) |
|---|---|
| `W.Entity.New(...)` | `W.NewEntity<TEntityType>(...)` / `W.NewEntity<Default>()` |
| `W.Entity.NewOnes(...)` | `W.NewEntities<TEntityType>(count, ...)` |
| `W.IsInitialized()` | `W.IsWorldInitialized` |
| `W.IsIndependent()` | `W.IsIndependent` |
| `entity.Gid()` | `entity.GID` |
| `entity.HasAllOf<C>()` | `entity.Has<C>()` |
| `entity.HasAnyOf<C1,C2>()` | `entity.HasAny<C1,C2>()` |
| `entity.HasAllOfTags<T>()` | `entity.Has<T>()` |
| `entity.TryAdd<C>()` | `entity.Add<C>()` |
| `entity.Put<C>(val)` | `entity.Set<C>(val)` |
| `entity.TryDelete<C>()` | `entity.Delete<C>()` (bool) |
| `Components<T>.Value` | `Components<T>.Instance` |
| `Tags<T>.Value` | `Components<T>.Instance` |
| `W.Query.Entities<F>()` | `W.Query<F>()` |
| `W.Query.For(...)` | `W.Query<F>().For(...)` |
| `AddForAll<C>()` | `BatchAdd<C>()` |
| `DeleteForAll<C>()` | `BatchDelete<C>()` |
| `SetTagForAll<T>()` | `BatchSet<T>()` |
| `IInitSystem` / `IUpdateSystem` | `ISystem` |
| `Systems.AddUpdate(sys)` | `Sys.Add(sys, order)` |
| `IComponentConfig<T,W>` | `ComponentTypeConfig<T>` + hooks in IComponent |
| `IEventConfig<T,W>` | `EventTypeConfig<T>` + hooks in IEvent |
| `IEntityLinkComponent<T>` | `ILinkType` + `Link<T>` |
| `IEntityLinksComponent<T>` | `ILinksType` + `Links<T>` |
| `RegisterToOneRelationType` | `Types().Link<T>()` |
| `IMultiComponent<T,V>` | `IMultiComponent` (marker) |
| `RegisterMultiComponentType` | `W.Types().Multi<T>()` |
| `W.Context.Set/Get/Has` | `W.SetResource/GetResource/HasResource` |
| `W.Context<T>.Replace(val)` | `W.SetResource(val)` (overwrites automatically) |
| `W.Events.Send(...)` | `W.SendEvent(...)` |
| `W.Events.RegisterEventReceiver` | `W.RegisterEventReceiver` |
| `W.Events.CreateSnapshot()` | `W.Serializer.CreateEventsSnapshot()` |
| `gid.IsActual<WT>()` | `gid.Status<WT>() != GIDStatus.NotActual` |
| `gid.IsLoadedAndActual<WT>()` | `gid.Status<WT>() == GIDStatus.Active` |
| `W.Entity.New(gid)` | `W.NewEntityByGID<TEntityType>(gid)` |
| `W.OnCreateEntity(callback)` | `IEntityType.OnCreate` / `Created` tracking |
| `With<F1, F2>` | `And<F1, F2>` |
| `Query.Parallel.For(minChunkSize:)` | `Query().ForParallel(minEntitiesPerThread:)` |
