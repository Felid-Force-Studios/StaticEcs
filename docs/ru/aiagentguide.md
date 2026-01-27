---
title: Руководство для AI-агентов
parent: RU
nav_order: 5
---

# Руководство для AI-агентов

Если вы используете AI-ассистенты (Claude Code, Cursor, Copilot и др.) со StaticEcs, вы можете предоставить агенту контекст о библиотеке. Эта страница содержит готовые сниппеты.

___

## llms.txt

Укажите вашему агенту на AI-документацию библиотеки:
- **Краткая**: `https://felid-force-studios.github.io/StaticEcs/llms.txt`
- **Полная**: `https://felid-force-studios.github.io/StaticEcs/llms-full.txt`

___

## Сниппет для CLAUDE.md

Если вы используете [Claude Code](https://claude.ai/code), скопируйте следующий блок в `CLAUDE.md` вашего проекта. Это даст Claude необходимый контекст для правильного использования StaticEcs.

Для других агентов вставьте это в соответствующие файлы инструкций (`.cursorrules`, `.github/copilot-instructions.md` и т.д.).

````markdown
## StaticEcs ECS Framework

Этот проект использует [StaticEcs](https://github.com/Felid-Force-Studios/StaticEcs) — статический generic ECS-фреймворк для C#. Namespace: `FFS.Libraries.StaticEcs`.

### Паттерн настройки
```csharp
public struct WT : IWorldType { }
public abstract class W : World<WT> { }           // type alias для доступа к миру
public struct GameSystems : ISystemsType { }
public abstract class GameSys : W.Systems<GameSystems> { }
```

### Жизненный цикл мира (строгий порядок)
1. `W.Create(WorldConfig.Default())` — создание мира
2. `W.Types().RegisterAll()` или ручная регистрация `.Component<T>().Tag<T>().Event<T>()` — регистрация ВСЕХ типов (обязательно!)
3. `W.Initialize()` — после этого доступны операции с сущностями
4. Работа: создание сущностей, запуск систем, итерация запросов
5. `W.Destroy()` — очистка

### Критические правила
- ВСЕГДА регистрируйте типы компонентов/тегов/событий/связей между Create() и Initialize(). Используйте `W.Types().RegisterAll()` для авторегистрации всех типов из сборки, или регистрируйте вручную. Незарегистрированные типы вызывают ошибки.
- Entity — 4-байтовый uint-хендл, НЕ постоянная ссылка. НИКОГДА не храните Entity в полях/коллекциях между кадрами. Используйте EntityGID.
- `Add<T>()` без значения идемпотентен (если существует → возвращает ref, без хуков). `Set(value)` ВСЕГДА перезаписывает с циклом OnDelete→OnAdd.
- `Ref<T>()` возвращает ref на компонент. Предполагает наличие компонента — проверяйте `Has<T>()` если не уверены.
- Для компонентов, которые только читаются, используйте `Read<T>()` (возвращает `ref readonly`) вместо `Ref<T>()`, а в делегатах запросов — `in` вместо `ref`.
- Типы фильтров запросов: `All<>` (требовать), `None<>` (исключить), `Any<>` (хотя бы один). Эти фильтры работают и с компонентами, и с тегами. Комбинировать: `And<Filter1, Filter2>` (все должны совпасть) или `Or<Filter1, Filter2>` (хотя бы один).
- Стандартный режим запроса — Strict. НЕ модифицируйте фильтруемые типы у ДРУГИХ сущностей при итерации. Используйте `EntitiesFlexible()` при необходимости.
- При `ForParallel` модифицируйте только текущую сущность. Без структурных изменений.

### Типичные паттерны
```csharp
// Создание сущности с компонентами
var entity = W.NewEntity<Default>().Set(new Position { Value = v }, new Velocity { Value = 1f });

// Итерация запроса (foreach)
foreach (var e in W.Query<All<Position, Velocity>>().Entities()) {
    ref var pos = ref e.Ref<Position>();
    ref readonly var vel = ref e.Read<Velocity>();
    pos.Value += vel.Value;
}

// Итерация запроса (делегат — быстрее, без аллокаций)
W.Query().For(static (ref Position p, in Velocity v) => {
    p.Value += v.Value;
});

// Постоянная ссылка
EntityGID gid = entity.GID;
if (gid.TryUnpack<WT>(out var resolved)) { /* resolved жив */ }

// Теги
entity.Set<IsPlayer>();
if (entity.Has<IsPlayer>()) { ... }

// Мультикомпоненты (список однотипных значений на сущности)
ref var items = ref entity.Add<W.Multi<Item>>();
items.Add(new Item { Id = 1 });
items.Add(new Item { Id = 2 });
foreach (ref var item in items) { item.Weight *= 2f; }

// Отношения (связи между сущностями)
entity.Set(new W.Link<Parent>(parentEntity));           // одиночная связь
ref var children = ref entity.Add<W.Links<Children>>(); // множественная связь
children.TryAdd(childEntity.AsLink<Children>());

// Системы
public struct MoveSystem : ISystem {
    public void Init() { /* вызывается один раз при Initialize */ }
    public void Update() {
        W.Query().For(static (ref Position p, in Velocity v) => {
            p.Value += v.Value;
        });
    }
    public void Destroy() { /* вызывается при Destroy */ }
}
GameSys.Create();
GameSys.Add(new MoveSystem(), order: 0);
GameSys.Initialize();
// В игровом цикле: GameSys.Update();

// Ресурсы
W.SetResource(new GameConfig { ... });
ref var config = ref W.GetResource<GameConfig>();
```

### Полная документация
- Краткая AI-справка: https://felid-force-studios.github.io/StaticEcs/llms.txt
- Полная документация: https://felid-force-studios.github.io/StaticEcs/ru/features.html
````
