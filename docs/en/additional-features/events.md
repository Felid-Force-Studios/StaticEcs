---
title: Events
parent: Additional features
nav_order: 3
---

### Events
Event - used to exchange information between systems or user services
- Presented as a custom structure with data

___

#### Example:
```c#
public struct WeatherChanged : IEvent { 
    public WeatherType WeatherType;
}
```

___

{: .important }
Requires registration in the world between creation and initialization

```c#
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.Events.RegisterEventType<WeatherChanged>()
//...
MyEcs.Initialize();
```

___

#### Creation and basic operations:
```c#
// The event system will be created when MyEcs.Create is called and destroyed when MyEcs.Destroy is called
MyEcs.Create(EcsConfig.Default());
MyEcs.Initialize();
//...

// Before sending an event, the receiver of the event must be registered, otherwise the event will not be sent.
// Receiver can be registered after calling Ecs.Create (e.g. in the Init method of the system).
var weatherChangedEventReceiver = MyEcs.Events.RegisterEventReceiver<WeatherChanged>();

// Deleting an event receiver
MyEcs.Events.DeleteEventReceiver(ref weatherChangedEventReceiver);

// Important! The lifecycle of an event: the event will be deleted in two cases:
// 1) when it's been read by all registered receivers.
// 2) when it will be suppressed on reading (by calling Suppress or SuppressAll method (information below) ) )
// So it is important that all registered listeners read the events or the event is suppressed by any listener so that there is no accumulation of them

// Sending an event
MyEcs.Events.Send(new WeatherChanged { WeatherType = WeatherType.Sunny });

// Sending default event value
MyEcs.Events.Send<WeatherChanged>();

// Get a dynamic identifier of event type (see “Component Identifiers”)
var weatherChangedDynId = MyEcs.Events.DynamicId<WeatherChanged>();
// Send default event value (Suitable for marker events without data)
MyEcs.Events.SendDefault(weatherChangedDynId);

// Receiving events
foreach (var weatherEvent in weatherChangedEventReceiver) {
    Console.WriteLine("Weather is " + weatherEvent.Value.WeatherType);
}


foreach (var weatherEvent in weatherChangedEventReceiver) {
    // True if this listener is the last listener to read this event (means that the event will be deleted after reading).
    bool last = weatherEvent.IsLastReading();
    // Returns the number of unread listeners other than the given listener at the moment
    int unreadCount = weatherEvent.UnreadCount();
}

foreach (var weatherEvent in weatherChangedEventReceiver) {
    // Event suppression - the event will be deleted and other receivers will no longer be able to read it
    weatherEvent.Suppress();
}

// Suppress all events for a given receiver
weatherChangedEventReceiver.SuppressAll();

// Marks the reading of all events for this receiver
weatherChangedEventReceiver.MarkAsReadAll();

```
