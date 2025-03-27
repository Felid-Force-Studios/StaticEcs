---
title: События
parent: Дополнительны возможности
nav_order: 3
---

### События
Событие - cлужит для обмена информацией между системами или пользовательскими сервисами
- Представлено в виде пользовательской структуры с данными

___

#### Пример:
```c#
public struct WeatherChanged : IEvent { 
    public WeatherType WeatherType;
}
```

___

{: .importantru }
Требуется регистрация в мире между созданием и инициализацией

```c#
MyEcs.Create(EcsConfig.Default());
//...
MyEcs.Events.RegisterEventType<WeatherChanged>()
//...
MyEcs.Initialize();
```

___

#### Создание и базовые операции:
```c#
// Система событий будет создана при вызове MyEcs.Create и уничтожена при MyEcs.Destroy
MyEcs.Create(EcsConfig.Default());
MyEcs.Initialize();
//...

// Прежде чем отправлять событие следует зарегестрировать слушателя данного события, иначе событие не будет отправлено
// Слушатель может быть зарегестрирован после выозова Ecs.Create (например в Init методе системы)
var weatherChangedEventReceiver = MyEcs.Events.RegisterEventReceiver<WeatherChanged>();

// Удаление слушателя событий
MyEcs.Events.DeleteEventReceiver(ref weatherChangedEventReceiver);

// Важно! Жизненый цикл события: событие будет удалено в двух случаях:
// 1) когда оно будет прочитано всеми зарегестрированными слушателями
// 2) когда оно будет подавленно при прочтении (при вызове Suppress или SuppressAll метода (информация ниже) )
// Таким образом важно чтобы все зарегестрированные слушатели читали события или событие подавлялось каким либо слушателем, чтобы не было их накопления

// Отправка события
MyEcs.Events.Send(new WeatherChanged { WeatherType = WeatherType.Sunny });

// Отправка дефолтного значения события
MyEcs.Events.Send<WeatherChanged>();

// Получение динамического идентификатора типа события (смотри "Идентификаторы компонентов")
var weatherChangedDynId = MyEcs.Events.DynamicId<WeatherChanged>();
// Отправка дефолтного значения события (Подходит для маркерных событий без данных)
MyEcs.Events.SendDefault(weatherChangedDynId);

// Получение событий
foreach (var weatherEvent in weatherChangedEventReceiver) {
    Console.WriteLine("Weather is " + weatherEvent.Value.WeatherType);
}

foreach (var weatherEvent in weatherChangedEventReceiver) {
    // True если данный слушатель последний из всех слушателей читавших это событий (значит что событие будет удалено после прочтения)
    bool last = weatherEvent.IsLastReading();
    // Возвращает число непрочитавших слушателей не считая данного в данный момент
    int unreadCount = weatherEvent.UnreadCount();
}

foreach (var weatherEvent in weatherChangedEventReceiver) {
    // Подвление события - событие будет удалено и другие слушатели больше не смогут его прочитать
    weatherEvent.Suppress();
}

// Подавление всех событий для данного слушателя
weatherChangedEventReceiver.SuppressAll();

// Пометка о прочтении всех событий для данного слушателя
weatherChangedEventReceiver.MarkAsReadAll();

```
