---
title: События
parent: Возможности
nav_order: 13
---

## Events
Событие — механизм обмена информацией между системами или пользовательскими сервисами
- Представлено в виде пользовательской структуры с данными и маркер-интерфейсом `IEvent`
- Модель «отправитель → множество получателей» с автоматическим управлением жизненным циклом
- Каждый получатель имеет независимый курсор чтения
- Событие автоматически удаляется, когда все получатели его прочитали или оно подавлено

#### Пример:
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

{: .importantru }
Регистрация типа события доступна как в фазе `Created`, так и в `Initialized`

```csharp
W.Create(WorldConfig.Default());
//...
// Простая регистрация
W.Types()
    .Event<WeatherChanged>()
    .Event<OnDamage>();

// Регистрация с конфигурацией
W.Types().Event<WeatherChanged>(new EventTypeConfig<WeatherChanged>(
    guid: new Guid("..."),      // стабильный идентификатор для сериализации (по умолчанию — автоматически из имени типа)
    version: 1,                  // версия схемы данных для миграции (по умолчанию — 0)
    readWriteStrategy: null      // стратегия бинарной сериализации (по умолчанию — авто-определение)
));
//...
W.Initialize();
```

{: .noteru }
Вместо передачи конфигурации вручную можно объявить статическое поле или свойство типа `EventTypeConfig<T>` внутри структуры события. `RegisterAll()` автоматически найдёт его по типу (предпочитая имя `Config`):

```csharp
public struct WeatherChanged : IEvent {
    public WeatherType WeatherType;
    public static readonly EventTypeConfig<WeatherChanged> Config = new(
        guid: new Guid("..."),
        version: 1
    );
}
// RegisterAll() автоматически использует WeatherChanged.Config
```

___

#### Отправка событий:
```csharp
// Отправить событие с данными
// Вернёт true если событие добавлено в буфер, false если нет зарегистрированных слушателей
bool sent = W.SendEvent(new WeatherChanged { WeatherType = WeatherType.Sunny });

// Отправить событие с default-значением
bool sent = W.SendEvent<OnDamage>();
```

{: .warningru }
Если нет зарегистрированных слушателей, `SendEvent` вернёт `false` и событие **не будет сохранено**. Регистрируйте слушателей до отправки событий.

___

#### Получение событий:
```csharp
// Создать слушателя — каждый слушатель имеет независимый курсор чтения
var weatherReceiver = W.RegisterEventReceiver<WeatherChanged>();

// Отправить события
W.SendEvent(new WeatherChanged { WeatherType = WeatherType.Sunny });
W.SendEvent(new WeatherChanged { WeatherType = WeatherType.Rainy });

// Чтение событий через foreach
// После итерации события помечаются как прочитанные для данного слушателя
foreach (var e in weatherReceiver) {
    ref var data = ref e.Value; // ref-доступ к данным события
    Console.WriteLine(data.WeatherType);
}

// Дополнительная информация о событии при итерации
foreach (var e in weatherReceiver) {
    // true если данный слушатель последний, кто читает это событие
    // (событие будет удалено после прочтения)
    bool last = e.IsLastReading();

    // Количество слушателей, ещё не прочитавших это событие (без текущего)
    int remaining = e.UnreadCount();

    // Подавить событие — немедленно удаляет его для всех оставшихся слушателей
    e.Suppress();
}
```

___

#### Управление слушателями:
```csharp
// Чтение всех событий через делегат
weatherReceiver.ReadAll(static (Event<WeatherChanged> e) => {
    Console.WriteLine(e.Value.WeatherType);
});

// Подавить все непрочитанные события для данного слушателя
// События удаляются и другие слушатели больше не смогут их прочитать
weatherReceiver.SuppressAll();

// Пометить все события как прочитанные без обработки
// События не удаляются — другие слушатели всё ещё могут их прочитать
weatherReceiver.MarkAsReadAll();

// Удалить слушателя
W.DeleteEventReceiver(ref weatherReceiver);
```

___

#### Многопоточность:

{: .warningru }
Отправка событий (`SendEvent`) потокобезопасна при соблюдении следующих условий:
- Несколько потоков могут одновременно вызывать `SendEvent` для **одного и того же** типа событий
- **Одновременное чтение и отправка** одного типа событий из разных потоков **запрещены** — отправка потокобезопасна только при отсутствии одновременного чтения того же типа
- Чтение событий одного типа (`foreach`, `ReadAll`) должно выполняться в **одном потоке**
- Разные типы событий можно читать из **разных потоков одновременно**, так как каждый тип хранится независимо
- Один и тот же тип событий можно читать из разных потоков **в разное время** (не одновременно)

Операции со слушателями (`foreach`, `ReadAll`, `MarkAsReadAll`, `SuppressAll`, создание и удаление слушателей) **не поддерживаются** в многопоточном режиме и должны выполняться только в основном потоке.

___

#### Жизненный цикл события:

{: .importantru }
Событие удаляется автоматически в двух случаях:
1. Все зарегистрированные слушатели прочитали событие
2. Событие было подавлено (`Suppress` или `SuppressAll`)

Важно, чтобы все зарегистрированные слушатели читали свои события (или вызывали `MarkAsReadAll`/`SuppressAll`), иначе события будут накапливаться в памяти.

```csharp
// Пример жизненного цикла с двумя слушателями
var receiverA = W.RegisterEventReceiver<WeatherChanged>();
var receiverB = W.RegisterEventReceiver<WeatherChanged>();

W.SendEvent(new WeatherChanged { WeatherType = WeatherType.Sunny });
// Событие имеет UnreadCount = 2

foreach (var e in receiverA) {
    // receiverA прочитал, UnreadCount = 1
}

foreach (var e in receiverB) {
    // receiverB прочитал, UnreadCount = 0 → событие автоматически удаляется
}
```
