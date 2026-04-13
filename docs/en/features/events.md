---
title: Events
parent: Features
nav_order: 13
---

## Events
Event is a mechanism for exchanging information between systems or user services
- Represented as a user struct with data and the `IEvent` marker interface
- "Sender → multiple receivers" model with automatic lifecycle management
- Each receiver has an independent read cursor
- An event is automatically deleted when all receivers have read it or it is suppressed

#### Example:
```csharp
public struct WeatherChanged : IEvent {
    public WeatherType WeatherType;
}

public struct OnDamage : IEvent {
    public float Amount;
    public EntityGID Target;
}
```

___

{: .important }
Event type registration is available in both `Created` and `Initialized` phases

```csharp
W.Create(WorldConfig.Default());
//...
// Simple registration
W.Types()
    .Event<WeatherChanged>()
    .Event<OnDamage>();

// Configuration is provided by implementing IEventConfig<T> on the event struct
// (see example below)
//...
W.Initialize();
```

{: .note }
To provide configuration, implement the `IEventConfig<T>` interface on the event struct. Both manual registration and `RegisterAll()` will use it automatically:

```csharp
public struct WeatherChanged : IEvent, IEventConfig<WeatherChanged> {
    public WeatherType WeatherType;
    public EventTypeConfig<WeatherChanged> Config() => new(
        guid: new Guid("..."),   // stable identifier for serialization (default — auto-computed from type name)
        version: 1               // data schema version for migration (default — 0)
    );
}
```

___

#### Sending events:
```csharp
// Send an event with data
// Returns true if the event was added to the buffer, false if no registered receivers
bool sent = W.SendEvent(new WeatherChanged { WeatherType = WeatherType.Sunny });

// Send an event with default value
bool sent = W.SendEvent<OnDamage>();
```

{: .warning }
If there are no registered receivers, `SendEvent` returns `false` and the event is **not stored**. Register receivers before sending events.

___

#### Receiving events:
```csharp
// Create a receiver — each receiver has an independent read cursor
var weatherReceiver = W.RegisterEventReceiver<WeatherChanged>();

// Send events
W.SendEvent(new WeatherChanged { WeatherType = WeatherType.Sunny });
W.SendEvent(new WeatherChanged { WeatherType = WeatherType.Rainy });

// Read events via foreach
// After iteration, events are marked as read for this receiver
foreach (var e in weatherReceiver) {
    ref var data = ref e.Value; // ref access to event data
    Console.WriteLine(data.WeatherType);
}

// Additional event information during iteration
foreach (var e in weatherReceiver) {
    // true if this receiver is the last one to read this event
    // (the event will be deleted after reading)
    bool last = e.IsLastReading();

    // Number of receivers that haven't read this event yet (excluding current)
    int remaining = e.UnreadCount();

    // Suppress the event — immediately deletes it for all remaining receivers
    e.Suppress();
}
```

___

#### Receiver management:
```csharp
// Read all events via delegate
weatherReceiver.ReadAll(static (Event<WeatherChanged> e) => {
    Console.WriteLine(e.Value.WeatherType);
});

// Suppress all unread events for this receiver
// Events are deleted and other receivers can no longer read them
weatherReceiver.SuppressAll();

// Mark all events as read without processing
// Events are not deleted — other receivers can still read them
weatherReceiver.MarkAsReadAll();

// Delete the receiver
W.DeleteEventReceiver(ref weatherReceiver);
```

___

#### Multithreading:

{: .warning }
Sending events (`SendEvent`) is thread-safe under the following conditions:
- Multiple threads can simultaneously call `SendEvent` for the **same** event type
- **Simultaneous reading and sending** of the same event type from different threads is **forbidden** — sending is thread-safe only when there is no concurrent reading of the same type
- Reading events of one type (`foreach`, `ReadAll`) must be done in a **single thread**
- Different event types can be read from **different threads simultaneously**, as each type is stored independently
- The same event type can be read from different threads **at different times** (not concurrently)

Receiver operations (`foreach`, `ReadAll`, `MarkAsReadAll`, `SuppressAll`, creating and deleting receivers) are **not supported** in multithreaded mode and must only be performed on the main thread.

___

#### Event lifecycle:

{: .important }
An event is automatically deleted in two cases:
1. All registered receivers have read the event
2. The event was suppressed (`Suppress` or `SuppressAll`)

It is important that all registered receivers read their events (or call `MarkAsReadAll`/`SuppressAll`), otherwise events will accumulate in memory.

```csharp
// Lifecycle example with two receivers
var receiverA = W.RegisterEventReceiver<WeatherChanged>();
var receiverB = W.RegisterEventReceiver<WeatherChanged>();

W.SendEvent(new WeatherChanged { WeatherType = WeatherType.Sunny });
// Event has UnreadCount = 2

foreach (var e in receiverA) {
    // receiverA read it, UnreadCount = 1
}

foreach (var e in receiverB) {
    // receiverB read it, UnreadCount = 0 → event is automatically deleted
}
```
