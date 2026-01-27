---
title: Ресурсы
parent: Возможности
nav_order: 11
---

## Resources
Ресурсы — альтернатива DI, простой механизм хранения и передачи пользовательских данных и сервисов в системы и другие методы
- Ресурсы — это синглтоны уровня мира: общее состояние, не привязанное к конкретной сущности
- Идеальны для конфигурации, времени/дельта-времени, состояния ввода, кэшей ассетов, ссылок на сервисы
- Два варианта: **singleton** (один на тип) и **именованный** (несколько на тип, различаются строковым ключом)
- Доступны как в фазе `Created`, так и в `Initialized`

___

## Singleton-ресурсы

Singleton-ресурс хранит ровно один экземпляр данного типа на мир.
Внутри использует статическое generic-хранилище — доступ O(1) без словарных накладных расходов.

#### Установка ресурса:
```csharp
// Пользовательские классы и сервисы
public class GameConfig { public float Gravity; }
public class InputState { public Vector2 MousePos; }

// Установить ресурс в мире
// По умолчанию clearOnDestroy = true — ресурс будет автоматически очищен при World.Destroy()
W.SetResource(new GameConfig { Gravity = 9.81f });
W.SetResource(new InputState(), clearOnDestroy: false); // переживёт пересоздание мира

// При повторном вызове SetResource для того же типа значение перезаписывается без ошибки
W.SetResource(new GameConfig { Gravity = 4.0f }); // перезаписывает предыдущее значение
```

{: .importantru }
Параметр `clearOnDestroy` учитывается только при первой регистрации. При замене существующего ресурса исходная настройка `clearOnDestroy` сохраняется.

#### Основные операции:
```csharp
// Проверить, зарегистрирован ли ресурс данного типа
bool has = W.HasResource<GameConfig>();

// Получить мутабельную ref-ссылку на значение ресурса — изменения записываются прямо в хранилище
ref var config = ref W.GetResource<GameConfig>();
config.Gravity = 11.0f; // изменено на месте, вызов сеттера не нужен

// Удалить ресурс из мира
W.RemoveResource<GameConfig>();

// Resource<T> — readonly-структура нулевой стоимости, хэндл для частого доступа (инициализация не нужна)
W.Resource<GameConfig> configHandle;
bool registered = configHandle.IsRegistered;
ref var cfg = ref configHandle.Value;
```

___

## Именованные ресурсы

Именованные ресурсы позволяют хранить несколько экземпляров одного типа, различаемых строковыми ключами.
Внутри хранятся в `Dictionary<string, object>` с типобезопасными `Box<T>`-обёртками.

#### Установка именованного ресурса:
```csharp
// Установить именованные ресурсы одного типа под разными ключами
W.SetResource("player_config", new GameConfig { Gravity = 9.81f });
W.SetResource("moon_config", new GameConfig { Gravity = 1.62f });

// При повторном вызове SetResource для существующего ключа значение перезаписывается без ошибки
W.SetResource("player_config", new GameConfig { Gravity = 10.0f }); // перезаписывает
```

#### Основные операции:
```csharp
// Проверить, существует ли именованный ресурс с данным ключом
bool has = W.HasResource<GameConfig>("player_config");

// Получить мутабельную ref-ссылку на значение именованного ресурса
ref var config = ref W.GetResource<GameConfig>("player_config");
config.Gravity = 5.0f;

// Удалить именованный ресурс по ключу
W.RemoveResource("player_config");

// NamedResource<T> — структура-хэндл, кэширующая внутреннюю ссылку после первого обращения
// Создать хэндл, привязанный к ключу (не регистрирует ресурс)
var moonConfig = new W.NamedResource<GameConfig>("moon_config");
bool registered = moonConfig.IsRegistered;  // всегда выполняет поиск в словаре, не кэшируется
ref var cfg = ref moonConfig.Value;          // первый вызов ищет в словаре и кэширует; последующие — O(1)
// Кэш автоматически инвалидируется при удалении ресурса или уничтожении мира
```

{: .warningru }
`NamedResource<T>` — мутабельная структура, кэширующая внутреннюю ссылку при первом обращении к `Value`.
**Не** храните её в `readonly`-поле и не передавайте по значению после первого использования — компилятор C#
создаст защитную копию, сбросит кэш, и каждый доступ будет выполнять поиск в словаре.
Храните в обычном (не readonly) поле или локальной переменной.

___

## Жизненный цикл

```csharp
W.Create(WorldConfig.Default());

// Ресурсы можно устанавливать после Create (не нужно ждать Initialize)
W.SetResource(new GameConfig { Gravity = 9.81f });
W.SetResource("debug_flags", new DebugFlags(), clearOnDestroy: false);

W.Initialize();

// Ресурсы остаются доступными в фазе Initialized
ref var config = ref W.GetResource<GameConfig>();

// При Destroy: ресурсы с clearOnDestroy=true очищаются автоматически
// Ресурсы с clearOnDestroy=false сохраняются и остаются доступными после следующего цикла Create+Initialize
W.Destroy();
```

