---
title: Системы
parent: Возможности
nav_order: 10
---

## Systems
Системы управляют логикой мира через определённый жизненный цикл
- Вложенный класс `World<TWorld>.Systems<SysType>` — каждый тип `ISystemsType` создаёт изолированную группу систем внутри мира
- Единый интерфейс `ISystem` с четырьмя методами (все опциональны)
- Системы выполняются в порядке, определённом параметром `order`
- Нереализованные методы не вызываются и не создают накладных расходов
- Системы могут быть структурами или классами

___

## ISystemsType

Маркерный интерфейс для изоляции групп систем. Каждый тип получает собственное статическое хранилище:

```csharp
public struct GameSystems : ISystemsType { }
public struct FixedSystems : ISystemsType { }
public struct LateSystems : ISystemsType { }

// Алиасы для удобного доступа
public abstract class GameSys : W.Systems<GameSystems> { }
public abstract class FixedSys : W.Systems<FixedSystems> { }
public abstract class LateSys : W.Systems<LateSystems> { }
```

___

## ISystem

Единый интерфейс для всех систем. Реализуйте только нужные методы — остальные не будут вызываться:

```csharp
public interface ISystem {
    // Вызывается один раз при Systems.Initialize()
    void Init() { }

    // Вызывается каждый кадр при Systems.Update()
    void Update() { }

    // Вызывается перед каждым Update() — false пропускает обновление
    bool UpdateIsActive() => true;

    // Вызывается один раз при Systems.Destroy()
    void Destroy() { }
}
```

{: .importantru }
Не оставляйте пустые реализации методов. Если метод не нужен — не реализуйте его. Нереализованные методы обнаруживаются через рефлексию и не вызываются.

#### Примеры систем:
```csharp
// Система только с Update
public struct MoveSystem : ISystem {
    public void Update() {
        W.Query().For(static (ref Position pos, in Velocity vel) => {
            pos.Value += vel.Value;
        });
    }
}

// Система с инициализацией и уничтожением
public struct AudioSystem : ISystem {
    public void Init() {
        // загрузить аудио-ресурсы
    }

    public void Update() {
        // обработать звуки
    }

    public void Destroy() {
        // освободить ресурсы
    }
}

// Система с условным выполнением
public struct PausableSystem : ISystem {
    public void Update() {
        // игровая логика
    }

    public bool UpdateIsActive() {
        return !W.GetResource<GameState>().IsPaused;
    }
}
```

___

## Жизненный цикл

```
Create() → Add() → Initialize() → Update() цикл → Destroy()
```

```csharp
// 1. Создать группу систем (baseSize — начальная ёмкость массива)
GameSys.Create(baseSize: 64);

// 2. Зарегистрировать системы (order определяет порядок выполнения)
GameSys.Add(new InputSystem(), order: -10)
    .Add(new MoveSystem(), order: 0)
    .Add(new RenderSystem(), order: 10);

// 3. Инициализировать — сортирует по order, вызывает Init() у всех систем
GameSys.Initialize();

// 4. Игровой цикл — вызывает Update() каждый кадр
while (gameIsRunning) {
    GameSys.Update();
}

// 5. Уничтожить — вызывает Destroy() у всех систем, сбрасывает состояние
GameSys.Destroy();
```

___

## Регистрация

Все системы регистрируются одним методом `Add<T>()`:

```csharp
// Базовая регистрация (order по умолчанию = 0)
GameSys.Add(new MoveSystem());

// С указанием порядка (меньше = раньше)
GameSys.Add(new InputSystem(), order: -10)      // выполняется первой
    .Add(new PhysicsSystem(), order: 0)          // затем физика
    .Add(new RenderSystem(), order: 10);         // рендер последним

// Системы с одинаковым order выполняются в порядке регистрации
GameSys.Add(new SystemA(), order: 0)   // первая среди order=0
    .Add(new SystemB(), order: 0);     // вторая среди order=0
```

___

## Условное выполнение

Метод `UpdateIsActive()` позволяет пропускать обновление системы на текущем кадре:

```csharp
public struct GameplaySystem : ISystem {
    public void Update() {
        // логика, выполняемая только когда игра не на паузе
    }

    public bool UpdateIsActive() {
        return !W.GetResource<GameState>().IsPaused;
    }
}

public struct TutorialSystem : ISystem {
    public void Update() {
        // логика обучения
    }

    public bool UpdateIsActive() {
        return W.GetResource<PlayerProgress>().IsFirstPlay;
    }
}
```

___

## Несколько групп систем

Разные `ISystemsType` создают независимые группы с собственным жизненным циклом:

```csharp
public struct GameSystems : ISystemsType { }
public struct FixedSystems : ISystemsType { }
public abstract class GameSys : W.Systems<GameSystems> { }
public abstract class FixedSys : W.Systems<FixedSystems> { }

// Настройка
GameSys.Create();
GameSys.Add(new InputSystem())
    .Add(new RenderSystem());
GameSys.Initialize();

FixedSys.Create();
FixedSys.Add(new PhysicsSystem())
    .Add(new CollisionSystem());
FixedSys.Initialize();

// Игровой цикл
while (gameIsRunning) {
    GameSys.Update();           // каждый кадр

    while (fixedTimeAccumulated) {
        FixedSys.Update();      // с фиксированным шагом
    }
}

GameSys.Destroy();
FixedSys.Destroy();
```

___

## Полный пример

```csharp
// Типы систем
public struct GameSystems : ISystemsType { }

// Системы
public struct InputSystem : ISystem {
    public void Update() {
        // чтение ввода
    }
}

public struct MoveSystem : ISystem {
    public void Update() {
        W.Query().For(static (ref Position pos, in Velocity vel) => {
            pos.Value += vel.Value;
        });
    }
}

public struct DamageSystem : ISystem {
    private EventReceiver<WT, OnDamage> _receiver;

    public void Init() {
        _receiver = W.RegisterEventReceiver<OnDamage>();
    }

    public void Update() {
        foreach (var e in _receiver) {
            if (e.Value.Target.TryUnpack<WT>(out var target)) {
                ref var health = ref target.Ref<Health>();
                health.Current -= e.Value.Amount;
            }
        }
    }

    public void Destroy() {
        W.DeleteEventReceiver(ref _receiver);
    }
}

// Запуск
W.Create(WorldConfig.Default());
// ... регистрация типов ...
W.Initialize();

GameSys.Create();
GameSys.Add(new InputSystem(), order: -10)
    .Add(new MoveSystem(), order: 0)
    .Add(new DamageSystem(), order: 5);
GameSys.Initialize();

while (gameIsRunning) {
    GameSys.Update();
}

GameSys.Destroy();
W.Destroy();
```
