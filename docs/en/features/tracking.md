---
title: Change Tracking
parent: Features
nav_order: 13
---

## Change Tracking

StaticEcs provides four types of change tracking, all zero-allocation and opt-in:

| Type | What it tracks | Applies to | Config location |
|------|---------------|------------|-----------------|
| **Added** | Component/tag was added | Components, tags | `ComponentTypeConfig` / `TagTypeConfig` |
| **Deleted** | Component/tag was removed | Components, tags | `ComponentTypeConfig` / `TagTypeConfig` |
| **Changed** | Component data accessed via `ref` | Components only | `ComponentTypeConfig` |
| **Created** | New entity was created | Entities (global) | `WorldConfig.TrackCreated` |

- Bitmap-based: one `ulong` per 64 entities per tracked type
- Tracking is versioned per world tick via a ring buffer (default 8 ticks). Changes are written to the current tick's slot and become visible to tracking filters after `W.Tick()` advances the counter — no manual clearing needed
- Zero overhead for types with tracking disabled
- Zero overhead for `Created` when `WorldConfig.TrackCreated = false`

___

## Configuration

All tracking is disabled by default and must be explicitly enabled at registration time.

### Components

`ComponentTypeConfig<T>` supports three tracking flags: `trackAdded`, `trackDeleted`, `trackChanged`:

```csharp
W.Create(WorldConfig.Default());
//...
// Enable all three tracking types
W.Types().Component<Health>(new ComponentTypeConfig<Health>(
    trackAdded: true,
    trackDeleted: true,
    trackChanged: true
));

// Enable only one direction
W.Types().Component<Velocity>(new ComponentTypeConfig<Velocity>(
    trackAdded: true  // track additions only
));

// Full configuration with tracking
W.Types().Component<Position>(new ComponentTypeConfig<Position>(
    guid: new Guid("..."),
    defaultValue: default,
    trackAdded: true,
    trackDeleted: true,
    trackChanged: true
));
//...
W.Initialize();
```

### Tags

`TagTypeConfig<T>` supports `trackAdded` and `trackDeleted`. Tags do **not** support Changed tracking.

```csharp
W.Types().Tag<Unit>(new TagTypeConfig<Unit>(
    trackAdded: true,
    trackDeleted: true
));

// With GUID for serialization
W.Types().Tag<Poisoned>(new TagTypeConfig<Poisoned>(
    guid: new Guid("A1B2C3D4-..."),
    trackAdded: true,
    trackDeleted: true
));
```

### Entity Creation

Entity creation tracking is configured at the **world level** via `WorldConfig`:

```csharp
W.Create(new WorldConfig {
    TrackCreated = true,
    // ...other settings...
});
//...
W.Initialize();
```

{: .note }
`Created` tracks all entity creation regardless of entity type. To filter by type, combine with `EntityIs<T>`: `W.Query<Created, EntityIs<Bullet>>()`.

### Auto-Registration

`trackAdded`, `trackDeleted`, and `trackChanged` can be declared in a static `Config` field inside the struct — `RegisterAll()` will pick them up automatically.

### Compile-Time Disable

The `FFS_ECS_DISABLE_CHANGED_TRACKING` define removes all Changed tracking code paths at compile time, including `AllChanged<T>`, `NoneChanged<T>`, `AnyChanged<T>` filters and the `Mut<T>()` method.

### Tick-Based Tracking

`WorldConfig.TrackingBufferSize` controls the ring buffer size (default 8). Call `W.Tick()` to advance the tick and rotate the buffer.

```csharp
// Default: tick-based tracking with 8 tick history
W.Create(WorldConfig.Default()); // TrackingBufferSize = 8

// Custom buffer size
W.Create(new WorldConfig {
    TrackingBufferSize = 16,   // 16 ticks of history
    // ...other settings...
});
```

#### Choosing buffer size

The buffer must be large enough to hold tracking history for your slowest system that uses tracking filters. If `W.Tick()` is called at 60fps but some systems run at 20fps, those systems skip 2 ticks between executions and need to look back 3 ticks.

**Formula:** `TrackingBufferSize >= tickRate / slowestSystemRate`

| Tick rate | Slowest system | Min buffer |
|-----------|---------------|------------|
| 60 fps | 60 fps (every tick) | 1 |
| 60 fps | 20 fps (every 3rd tick) | 3 |
| 60 fps | 10 fps (every 6th tick) | 6 |
| 60 fps | 1 fps (every 60th tick) | 60 |

If your systems use real-time intervals instead of tick counters, higher-than-expected FPS will increase the number of ticks between executions — add margin accordingly. The default value of 8 covers most games where the slowest tracking system runs at ~20fps or faster.

___

## Tick-Based Tracking

Tick-based tracking solves two common problems:
1. Systems in the middle of a pipeline make changes that systems at the beginning cannot see next frame — if tracking is cleared at the end of the frame
2. Different system groups (Update / FixedUpdate) cannot synchronize tracking — clearing in one group affects the other

### How It Works

- Each system in `W.Systems<T>.Update()` automatically gets a `LastTick` — it sees all changes in the tick range `(LastTick, CurrentTick]` — changes from the current frame become visible next frame
- When a system finishes, its `LastTick` is set to `CurrentTick`
- If a system is skipped (`UpdateIsActive() = false`), its `LastTick` is NOT updated — next time it runs, it sees all accumulated changes
- `W.Tick()` advances the global tick counter — the current write slot becomes readable history, and a new write slot is cleared for the next frame

### Game Loop Integration

{: .important }
Call `W.Tick()` **once per frame after** the most frequently updated system group. Changes made during a frame become visible to tracking filters in the **next** frame. Do not call it after each group — this wastes ring buffer slots. Per-system `LastTick` ensures that infrequent systems automatically see accumulated changes from multiple ticks.

```csharp
// Single system group
while (running) {
    W.Systems<GameLoop>.Update();    // each system sees changes from previous ticks
    W.Tick();                      // current frame's changes become visible next frame
}

// Multiple system groups (e.g., Update + FixedUpdate)
while (running) {
    W.Systems<Update>.Update();

    // FixedUpdate may run multiple times per frame — all within the same tick
    while (fixedTimeAccumulator >= fixedDeltaTime) {
        W.Systems<FixedUpdate>.Update();
        fixedTimeAccumulator -= fixedDeltaTime;
    }

    W.Tick();                      // one tick per frame
}
```

### One-Frame Delay

Tracking changes are written to a dedicated write slot, separate from the readable history. When `W.Tick()` is called, the write slot becomes part of the history. This means every system sees changes made **after its previous execution and before the current frame** — never changes from the current frame itself.

Consider a pipeline of 5 systems where Sys1 and Sys5 modify `Position`, and Sys3 queries `AllChanged<Position>`:

```
Frame 1:
  Sys1  → changes Position  (written to write slot)
  Sys3  → queries tracking   → sees NOTHING (history is empty, first frame)
  Sys5  → changes Position  (written to same write slot)
  Tick()                     → write slot becomes history[tick 1]

Frame 2:
  Sys1  → changes Position  (written to new write slot)
  Sys3  → queries tracking   → sees history[tick 1] = Sys1 + Sys5 from frame 1
  Sys5  → changes Position  (written to same write slot)
  Tick()                     → write slot becomes history[tick 2]

Frame 3:
  Sys3  → queries tracking   → sees history[tick 2] = Sys1 + Sys5 from frame 2
```

Each frame, Sys3 sees **exactly** the changes from the previous frame — both from systems before it (Sys1) and after it (Sys5). No double-processing, no missing data.

### Per-System Tick Tracking

Each system maintains its own `LastTick`. Systems that run every tick see exactly 1 tick of changes. Systems that skip frames see all accumulated changes since their last execution:

```csharp
public struct RareSystem : ISystem {
    private int _counter;

    public bool UpdateIsActive() => ++_counter % 5 == 0; // runs every 5 ticks

    public void Update() {
        // Sees ALL changes from the last 5 ticks (or up to TrackingBufferSize)
        foreach (var entity in W.Query<All<Position>, AllAdded<Position>>().Entities()) {
            // process newly added positions from the last 5 ticks
        }
    }
}
```

### Custom Tick Range (FromTick)

All tracking filters accept an optional `fromTick` constructor parameter to override the automatic tick range:

```csharp
// Automatic — uses the system's LastTick (default, no constructor needed):
foreach (var entity in W.Query<All<Position>, AllAdded<Position>>().Entities()) { }

// Manual — see all changes from tick 5 to current:
var filter = new AllAdded<Position>(fromTick: 5);
foreach (var entity in W.Query<All<Position>>(filter).Entities()) { }
```

- `fromTick = 0` (default): automatic range from `CurrentLastTick` (set by `W.Systems<T>.Update()`)
- `fromTick > 0`: manual lower bound — see changes from that tick to the current tick

### Cross-Group Synchronization

With tick-based tracking, different system groups within the same frame all write to the same write slot. All changes become visible in the next frame equally:

```csharp
W.Systems<Update>.Update();          // systems write tracking data into tick N's write slot
W.Systems<FixedUpdate>.Update();     // also writes to tick N's write slot
W.Tick();                          // advance to tick N+1; tick N becomes readable history
```

Each system's `LastTick` is independent. A FixedUpdate system that skips frames will see accumulated changes from all previous ticks since its last run.

### Buffer Overflow

If a system does not run for more ticks than `TrackingBufferSize`, the oldest tracking data is overwritten. The system will see at most `TrackingBufferSize` ticks of history.

{: .warning }
In debug mode (`FFS_ECS_DEBUG`), a `StaticEcsException` is thrown when a system's tick range exceeds the buffer size. In release mode, the range is silently clamped. Increase `WorldConfig.TrackingBufferSize` if your systems need deeper history.

___

## Query Filters

All tracking filters are used in the same way as standard component/tag filters:

| Category | Filter | Type Params | Description |
|----------|--------|-------------|-------------|
| **Component Added** | `AllAdded<T0..T4>` | 1–5 | ALL listed components were added |
| | `NoneAdded<T0..T4>` | 1–5 | Excludes entities where ANY was added |
| | `AnyAdded<T0..T4>` | 2–5 | AT LEAST ONE was added |
| **Component Deleted** | `AllDeleted<T0..T4>` | 1–5 | ALL listed components were deleted |
| | `NoneDeleted<T0..T4>` | 1–5 | Excludes entities where ANY was deleted |
| | `AnyDeleted<T0..T4>` | 2–5 | AT LEAST ONE was deleted |
| **Component Changed** | `AllChanged<T0..T4>` | 1–5 | ALL listed components were accessed via `ref` |
| | `NoneChanged<T0..T4>` | 1–5 | Excludes entities where ANY was changed |
| | `AnyChanged<T0..T4>` | 2–5 | AT LEAST ONE was accessed via `ref` |
| **Entity** | `Created` | — | Entity was created (requires `WorldConfig.TrackCreated`) |

{: .note }
`AllAdded`, `NoneAdded`, `AnyAdded`, `AllDeleted`, `NoneDeleted`, `AnyDeleted` filters work with both components and tags. No separate tag tracking filter types exist.

### Examples

```csharp
// Entities where Position was added and is currently present
foreach (var entity in W.Query<All<Position>, AllAdded<Position>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
}

// Entities where both Position AND Velocity were added
foreach (var entity in W.Query<AllAdded<Position, Velocity>>().Entities()) { }

// Entities where at least one of Position or Velocity was added
foreach (var entity in W.Query<AnyAdded<Position, Velocity>>().Entities()) { }

// React to tag being set
foreach (var entity in W.Query<AllAdded<IsDead>>().Entities()) { }

// At least one of the listed tags was added
foreach (var entity in W.Query<AnyAdded<Poisoned, Stunned>>().Entities()) { }

// Process entities whose Position was modified (via ref)
foreach (var entity in W.Query<All<Position>, AllChanged<Position>>().Entities()) {
    ref readonly var pos = ref entity.Read<Position>();
}

// Only truly changed, excluding newly added
foreach (var entity in W.Query<All<Position>, AllChanged<Position>, NoneAdded<Position>>().Entities()) {
    ref readonly var pos = ref entity.Read<Position>();
}

// Process recently created entities that have Position
foreach (var entity in W.Query<Created, All<Position>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
}

// Group filters via And
var filter = default(And<AllAdded<Position, Unit>, AllDeleted<Velocity>>);
foreach (var entity in W.Query(filter).Entities()) { }
```

___

## Semantics

### Added / Deleted

{: .important }
**`AllAdded<T>` means the component was added — it does NOT guarantee the component is currently present.** If a component was added and then deleted in the same frame, it is still marked Added but the component no longer exists. Similarly, `AllDeleted<T>` means the component was deleted — but it may have been added back.

**Recommended filter combinations:**
```csharp
// "Added AND currently present" — RECOMMENDED
foreach (var entity in W.Query<All<Position>, AllAdded<Position>>().Entities()) {
    ref var pos = ref entity.Ref<Position>(); // safe — All<Position> guarantees presence
}

// "Deleted AND currently absent"
foreach (var entity in W.Query<None<Position>, AllDeleted<Position>>().Entities()) {
    // entity is alive, Position was deleted — can clean up related resources
}

// AllAdded<Position> only — no guarantee of presence!
foreach (var entity in W.Query<AllAdded<Position>>().Entities()) {
    // CAUTION: the component may have already been deleted!
    if (entity.Has<Position>()) {
        ref var pos = ref entity.Ref<Position>();
    }
}
```

### Changed (Pessimistic Model)

Changed tracking uses a **dirty-on-access** model: any `ref` access marks the component as Changed, regardless of whether the data was actually modified. This is by design — checking actual value changes at the field level would be too expensive for a high-performance ECS.

#### Data Access Methods

| Method | Returns | Marks Changed | Marks Added | Notes |
|--------|---------|:---:|:---:|-------|
| `Ref<T>()` | `ref T` | — | — | Fast mutable access, no tracking |
| `Mut<T>()` | `ref T` | Yes | — | Tracked mutable access |
| `Read<T>()` | `ref readonly T` | — | — | Read-only access |
| `Add<T>()` (new) | `ref T` | Yes | Yes | Component is new |
| `Add<T>()` (exists) | `ref T` | — | — | Returns ref to existing, no hooks |
| `Set(value)` (new) | void | Yes | Yes | Component is new |
| `Set(value)` (exists) | void | Yes | — | Overwrites existing |

{: .important }
**`Ref<T>()` does NOT mark Changed.** Use `Mut<T>()` when you need change tracking. `Ref<T>()` is the fastest way to access component data — zero overhead, no tracking branch. Use `Read<T>()` for read-only access. In query delegate iteration (`For`, `ForBlock`), `ref` parameters automatically use tracked access (`Mut` semantics), `in` parameters use read-only access (`Read` semantics).

#### Query Auto-Tracking

Query iteration automatically marks Changed based on access semantics:

**For delegates** — `ref` marks Changed, `in` does not:
```csharp
// Position is marked as Changed (ref), Velocity is NOT (in)
W.Query<All<Position, Velocity>>().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});
```

**IQuery structs** — `Write<T>` marks Changed, `Read<T>` does not:
```csharp
public struct MoveSystem : IQuery.Write<Position>.Read<Velocity> {
    public void Invoke(Entity entity, ref Position pos, in Velocity vel) {
        pos.Value += vel.Value;
    }
}
```

**ForBlock** — `Block<T>` (mutable) marks Changed, `BlockR<T>` (read-only) does not:
```csharp
public struct MoveBlockSystem : IQueryBlock.Write<Position>.Read<Velocity> {
    public void Invoke(uint count, EntityBlock entities, Block<Position> pos, BlockR<Velocity> vel) {
        // process block
    }
}
```

Parallel queries follow the same rules.

#### Changed + Added Interaction

{: .important }
When a component is added via `Add<T>()` or `Set(value)`, it is marked as BOTH Added AND Changed. To filter only genuinely modified entities — excluding newly added ones — combine `AllChanged<T>` with `NoneAdded<T>`:

```csharp
foreach (var entity in W.Query<All<Position>, AllChanged<Position>, NoneAdded<Position>>().Entities()) {
    // truly changed, not just created
}
```

### Created

`Created` tracks the fact of entity creation globally. It does not carry any type information — to filter by entity type, combine with `EntityIs<T>`:

```csharp
foreach (var entity in W.Query<Created, EntityIs<Bullet>, All<Position>>().Entities()) {
    // newly created bullets with Position
}
```

___

## Edge Cases

{: .important }
Added and Deleted states are **independent** and **do not cancel each other out**. They record all operations that occurred within the current tick. Changed is also independent from both.

### Add then Delete
```csharp
entity.Set(new Position { X = 10 });   // Added = 1
entity.Delete<Position>();              // Deleted = 1, Added remains

// Result: entity does NOT have Position, but is marked as both Added and Deleted
// Query<AllAdded<Position>>                    -> finds the entity
// Query<AllDeleted<Position>>                  -> finds the entity
// Query<All<Position>, AllAdded<Position>>     -> does NOT find (component absent)
// Query<None<Position>, AllDeleted<Position>>  -> finds (deleted and absent)
```

### Delete then Add
```csharp
entity.Delete<Weapon>();                // Deleted = 1
entity.Set(new Weapon { Damage = 50 }); // Added = 1, Deleted remains

// Result: entity DOES have Weapon, marked as both Added and Deleted
// Query<All<Weapon>, AllAdded<Weapon>>   -> finds (added and present)
// Query<All<Weapon>, AllDeleted<Weapon>> -> finds (deleted and present again)
```

### Add, Delete, Add
```csharp
entity.Set(new Health { Value = 100 }); // Added = 1
entity.Delete<Health>();                // Deleted = 1
entity.Set(new Health { Value = 50 });  // Added already marked

// Result: entity DOES have Health (Value = 50), marked as both Added and Deleted
// Equivalent to "Delete then Add" from the tracking perspective
```

### Multiple Additions (Idempotency)
```csharp
// Add without value — does not overwrite existing component
entity.Add<Position>();                 // Added = 1 (new component)
entity.Add<Position>();                 // Added already marked, no change
// Added is only marked on the first addition (when the component is new)

// Set with value — ALWAYS overwrites
entity.Set(new Position { X = 10 });    // Added = 1 (new)
entity.Set(new Position { X = 20 });    // overwrite, Added not marked again
                                         // (component already existed)
```

### Mut Without Modification
```csharp
ref var pos = ref entity.Mut<Position>(); // MARKED as Changed even if no write follows!
// Changed tracking is pessimistic — it tracks access, not actual mutations
// Use entity.Ref<Position>() if you don't need tracking — it has zero overhead
```

### Multiple Mut Calls
```csharp
entity.Mut<Position>(); // marked
entity.Mut<Position>(); // already marked, no additional cost
// Changed bit is idempotent
```

### Query Iteration Marks All Iterated Entities
```csharp
// ALL entities matching the query get Changed mark for ref components,
// even if the delegate doesn't actually modify the data
W.Query<All<Position>>().For(static (ref Position pos) => {
    var x = pos.X; // marked Changed because of `ref`, even though we only read
});

// Use `in` to avoid this:
W.Query<All<Position>>().For(static (in Position pos) => {
    var x = pos.X; // NOT marked as Changed
});
```

### Changed and Deleted Are Independent
Changed and Deleted are independent bits. If a component was accessed via `ref` and then deleted in the same frame, both Changed and Deleted bits are set.

___

## Destroy and Deserialization

### Destroy Behavior

`entity.Destroy()` removes all components/tags — they are marked as Deleted. But the entity is dead, so the alive mask filters it out of ALL queries. Therefore `AllDeleted<T>` will **not** find destroyed entities.

```csharp
var entity = W.Entity.New<Position, Velocity>();
entity.Destroy();
// Query<AllDeleted<Position>> -> does NOT find (entity is dead)

// If you need to react to destruction — delete components explicitly before Destroy:
entity.Delete<Position>();  // Deleted tracking bit = 1, entity is alive
// ... process AllDeleted<Position> ...
entity.Destroy();
```

### After Deserialization

```csharp
// ReadChunk writes masks directly — tracking is NOT triggered
// ReadEntity goes through Add — components are marked as Added
// Recommended to call ClearTracking() after loading:
W.Serializer.ReadChunk(ref reader);
W.ClearTracking(); // reset all tracking (in tick-based mode: clears all ring buffer slots)
```

___

## Clearing Tracking

{: .important }
`ClearTracking()` methods clear ALL ring buffer slots. Normally not needed — tracking is managed automatically by `W.Tick()` and `W.Systems<T>.Update()`. Use as a "nuclear option" to reset all tracking state.

```csharp
// === Full reset ===
W.ClearTracking();                         // Everything (Added + Deleted + Changed + Created)

// === By category ===
W.ClearAllTracking();                      // All components and tags (Added + Deleted + Changed)
W.ClearCreatedTracking();                  // Entity creation

// === By tracking kind (all types) ===
W.ClearAllAddedTracking();                 // Added for all components and tags
W.ClearAllDeletedTracking();               // Deleted for all components and tags
W.ClearAllChangedTracking();               // Changed for all components

// === Per-type (components and tags) ===
W.ClearTracking<Position>();               // Added + Deleted + Changed for Position
W.ClearAddedTracking<Position>();          // Added only
W.ClearDeletedTracking<Position>();        // Deleted only
W.ClearChangedTracking<Position>();        // Changed only

// Works the same for tags
W.ClearTracking<Unit>();                   // Added + Deleted for Unit
W.ClearAddedTracking<Unit>();              // Added only
W.ClearDeletedTracking<Unit>();            // Deleted only
```

{: .note }
Standard pattern: `W.Systems.Update()` → `W.Tick()` → repeat. No manual clearing needed.

___

## Checking Entity State

In addition to query filters, you can check tracking state on individual entities:

```csharp
// Components — ALL semantics (all specified must match)
bool wasAdded = entity.HasAdded<Position>();
bool bothAdded = entity.HasAdded<Position, Velocity>();       // Position AND Velocity added
bool wasDeleted = entity.HasDeleted<Health>();
bool wasChanged = entity.HasChanged<Position>();
bool bothChanged = entity.HasChanged<Position, Velocity>();   // Position AND Velocity changed

// Components — ANY semantics (at least one must match)
bool anyAdded = entity.HasAnyAdded<Position, Velocity>();     // Position OR Velocity added
bool anyDeleted = entity.HasAnyDeleted<Position, Velocity>(); // Position OR Velocity deleted
bool anyChanged = entity.HasAnyChanged<Position, Velocity>(); // Position OR Velocity changed

// Tags — same methods work for tags (ALL semantics)
bool tagAdded = entity.HasAdded<Unit>();
bool tagDeleted = entity.HasDeleted<Poisoned>();
bool bothTagsAdded = entity.HasAdded<Unit, Player>();          // Unit AND Player added

// Tags — ANY semantics
bool anyTagAdded = entity.HasAnyAdded<Unit, Player>();         // Unit OR Player added
bool anyTagDeleted = entity.HasAnyDeleted<Unit, Player>();     // Unit OR Player deleted

// Combine with presence check
if (entity.HasAdded<Position>() && entity.Has<Position>()) {
    ref var pos = ref entity.Ref<Position>();
    // component was added and is currently present
}

// All methods accept an optional fromTick parameter for custom tick range:
bool addedSinceTick5 = entity.HasAdded<Position>(fromTick: 5);
bool changedRecently = entity.HasChanged<Position>(fromTick: W.CurrentTick);
```

___

## Performance

- Tracking masks use the same `ulong`-per-block format as component/tag presence masks
- Components: up to 3 bands per tracked type (Added, Deleted, Changed), each one `ulong` per 64 entities
- Tags: up to 2 bands per tracked type (Added, Deleted)
- `Created`: 1 `ulong` per block globally, plus heuristic chunks for fast skip
- `AllAdded<T>` / `AllDeleted<T>` / `AllChanged<T>` filters have the same cost as `All<T>` / `None<T>`: one bitmask operation per block
- Changed tracking in queries: one batch OR per block — same cost as a single bitmask operation
- `ClearTracking()` uses heuristic chunks to skip empty regions — O(occupied chunks), not O(entire world)
- `Ref<T>()` has zero tracking overhead — no runtime branch, identical to pre-tracking code
- Zero overhead for types that do not have tracking enabled
- Zero overhead for `Created` when `WorldConfig.TrackCreated = false`
- Compile-time elimination via `FFS_ECS_DISABLE_CHANGED_TRACKING` removes all Changed tracking code paths
- **Tick-based write:** zero overhead (pointer swap)
- **Tick-based read:** O(ticksToCheck) OR operations, bounded by `TrackingBufferSize`. Hierarchical filtering applies: first at chunk level (4096 entities), then at block level (64 entities) — only chunks/blocks with actual tracking data are checked
- **Tick advance:** negligible per-frame cost
- **Memory:** heuristic arrays × `TrackingBufferSize`; segment data is lazily allocated

___

## Use Cases

**Network synchronization (delta updates):**
```csharp
foreach (var entity in W.Query<All<Position>, AllChanged<Position>>().Entities()) {
    ref readonly var pos = ref entity.Read<Position>();
    SendPositionUpdate(entity, pos);
}
```

**Physics sync:**
```csharp
foreach (var entity in W.Query<All<Transform, PhysicsBody>, AllChanged<Transform>>().Entities()) {
    ref readonly var transform = ref entity.Read<Transform>();
    ref var body = ref entity.Ref<PhysicsBody>();
    SyncPhysicsBody(ref body, transform);
}
```

**Reactive initialization:**
```csharp
foreach (var entity in W.Query<All<Position, Unit>, AllAdded<Position>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
    // create visual representation for new entity
}
```

**Entity initialization:**
```csharp
foreach (var entity in W.Query<Created, All<Position, Unit>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
    // set up visuals, physics body, etc.
}
```

**UI updates:**
```csharp
// Create health bar for new entities
foreach (var entity in W.Query<All<Health, Player>, AllAdded<Health>>().Entities()) {
    ref var health = ref entity.Ref<Health>();
    // create health bar UI element
}

// Update health bar only when data changes
foreach (var entity in W.Query<All<Health, Player>, AllChanged<Health>>().Entities()) {
    ref readonly var health = ref entity.Read<Health>();
    // update display
}
```

**Multiple system groups (tick-based):**
```csharp
void GameLoop() {
    W.Systems<Update>.Update();          // each system sees changes from previous frames
    W.Systems<FixedUpdate>.Update();     // same — per-system LastTick determines the range
    W.Tick();                          // commit current frame's tracking to history
}
```

**Conditional systems (tick-based):**
```csharp
public struct PeriodicSync : ISystem {
    private int _frame;
    public bool UpdateIsActive() => ++_frame % 10 == 0;

    public void Update() {
        // Automatically sees ALL changes from the last 10 ticks
        foreach (var entity in W.Query<All<Position>, AllChanged<Position>>().Entities()) {
            SyncToNetwork(entity);
        }
    }
}
```
