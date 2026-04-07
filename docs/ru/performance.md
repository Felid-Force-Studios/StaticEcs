---
title: Производительность
parent: RU
nav_order: 3
---

# Производительность

## Архитектурные особенности

StaticEcs спроектирован для максимальной производительности и огромных миров:

- **Сущность никогда не перемещается в памяти** при Add/Remove — операции выполняются побитово за O(1). В архетипных ECS добавление или удаление компонента вызывает перемещение сущности между архетипами с копированием всех данных. В sparse set ECS удаление компонента перемещает последний элемент на место удалённого (swap-back)

- **SoA-хранение** (Structure of Arrays) — компоненты одного типа расположены в памяти последовательно, что обеспечивает оптимальное использование кэша CPU при итерации. Архетипные ECS также используют SoA внутри архетипов, но данные фрагментированы между отдельными массивами разных архетипов, количество которых растёт комбинаторно. В StaticEcs все компоненты одного типа хранятся в едином массиве сегментов — фрагментация возможна при использовании множества entityType и кластеров, но остаётся контролируемой. Sparse set ECS хранят компоненты в плотных массивах, но доступ к нескольким компонентам одной сущности требует индексации через разные массивы с потенциально разным порядком элементов

- **Статические дженерики** — доступ к данным через `Components<T>` — прямое обращение к статическому полю, разрешаемое на этапе компиляции. В других ECS поиск пула компонентов требует хеш-поиска по type ID или доступа через lookup с проверками безопасности

- **Нет проблемы архетипного взрыва** — в архетипных ECS каждая уникальная комбинация компонентов создаёт новый архетип. При 30+ типах компонентов количество архетипов может достигать тысяч, вызывая фрагментацию памяти и деградацию итерации. StaticEcs свободен от этой проблемы — количество типов компонентов не влияет на структуру хранения

- **Нулевые аллокации** на горячем пути — все структуры данных предаллоцированы, запросы возвращают ref struct итераторы. В других ECS создание view/filter может требовать аллокаций при первом вызове или управляется через обёртки с накладными расходами на проверки безопасности

- **Двумерная партиция** (Cluster × EntityType) — встроенная пространственная и логическая группировка на уровне памяти, позволяющая контролировать расположение сущностей без изменения набора компонентов. В других ECS группировка возможна только через фильтры запросов (теги, shared components), без прямого контроля над расположением в памяти

- **Встроенный стриминг** — загрузка/выгрузка кластеров и чанков без перестроения внутренних структур. В архетипных ECS массовое создание или удаление сущностей вызывает перебалансировку чанков. В sparse set ECS массовое удаление фрагментирует плотные массивы

- **Предсказуемая производительность** — время операций Add/Remove/Has не зависит от количества компонентов на сущности и общего числа типов в мире. В архетипных ECS стоимость структурных изменений растёт с количеством компонентов (копируются все данные сущности). В sparse set ECS стоимость Has/Ref постоянна, но итерация по нескольким компонентам требует пересечения множеств

___

## Способы итерации (от быстрого к удобному)

#### 1. ForBlock — указатели на блоки (самый быстрый для unmanaged):
```csharp
readonly struct MoveBlock : W.IQueryBlock.Write<Position>.Read<Velocity> {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(uint count, W.EntityBlock entities,
                       Block<Position> positions, BlockR<Velocity> velocities) {
        for (uint i = 0; i < count; i++) {
            positions[i].Value += velocities[i].Value;
        }
    }
}

W.Query().WriteBlock<Position>().Read<Velocity>().For<MoveBlock>();
```

#### 2. For с функциональной структурой (без аллокаций, с состоянием):
```csharp
struct MoveFunction : W.IQuery.Write<Position>.Read<Velocity> {
    public float DeltaTime;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(W.Entity entity, ref Position pos, in Velocity vel) {
        pos.Value += vel.Value * DeltaTime;
    }
}

W.Query().Write<Position>().Read<Velocity>().For(new MoveFunction { DeltaTime = 0.016f });
```

#### 3. For с делегатом (без аллокаций со static лямбдами):
```csharp
// Без данных
W.Query().For(
    static (ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value;
    }
);

// С пользовательскими данными (без захвата)
W.Query().For(deltaTime,
    static (ref float dt, ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value * dt;
    }
);
```

#### 4. Foreach итерация (наиболее гибкий):
```csharp
foreach (var entity in W.Query<All<Position, Velocity>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
    ref readonly var vel = ref entity.Read<Velocity>();
    pos.Value += vel.Value;
}
```

___

## Методы расширения для IL2CPP

При использовании IL2CPP в Unity, стандартные дженерик-методы Entity (`entity.Ref<T>()`, `entity.Has<T>()`) могут быть на 10–25% медленнее из-за особенностей AOT-компиляции. Рекомендуется создавать типизированные методы расширения:

```csharp
public static class ComponentExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref Position RefPosition(this W.Entity entity) {
        return ref W.Components<Position>.Instance.Ref(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasPosition(this W.Entity entity) {
        return W.Components<Position>.Instance.Has(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasTagPlayer(this W.Entity entity) {
        return W.Tags<IsPlayer>.Instance.Has(entity);
    }
}
```

```csharp
// Использование — удобно и быстро
ref var pos = ref entity.RefPosition();
bool has = entity.HasPosition();
bool isPlayer = entity.HasTagPlayer();
```

{: .noteru }
В Mono/CoreCLR разница минимальна благодаря агрессивному инлайнингу JIT. Оптимизация актуальна именно для IL2CPP.

___

## Параллельное выполнение

Для активации многопоточных запросов укажите количество потоков в конфигурации мира:

```csharp
W.Create(new WorldConfig {
    ThreadCount = WorldConfig.MaxThreadCount, // все доступные потоки CPU
    // или
    // ThreadCount = 8, // конкретное количество потоков
});
```

```csharp
// Параллельная итерация
W.Query().ForParallel(
    static (ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value;
    },
    minEntitiesPerThread: 50000  // минимум сущностей на поток
);
```

{: .importantru }
Ограничения параллельной итерации: можно модифицировать/уничтожать только текущую сущность. Нельзя создавать сущности, модифицировать другие сущности. `SendEvent` потокобезопасен (при отсутствии одновременного чтения того же типа).

___

## Тип сущности (entityType)

`entityType` группирует логически схожие сущности в смежных сегментах памяти, что улучшает кэш-локальность:

```csharp
struct UnitType : IEntityType { }
struct BulletType : IEntityType { }
struct EffectType : IEntityType { }

// Юниты расположены рядом в памяти
var unit = W.NewEntity<UnitType>();
unit.Add<Position>(); unit.Add<Health>();

// Снаряды — в своих сегментах
var bullet = W.NewEntity<BulletType>();
bullet.Add<Position>(); bullet.Add<Velocity>();
```

Запросы автоматически итерируют по смежным блокам памяти — чем однороднее данные, тем эффективнее кэш CPU.

___

## Кластерные запросы

Ограничение запросов конкретными кластерами пропускает ненужные чанки:

```csharp
const ushort ACTIVE_ZONE = 1;
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { ACTIVE_ZONE };

// Итерация только по указанным кластерам
W.Query().For(
    static (ref Position pos) => { pos.Value.Y -= 9.8f * 0.016f; },
    clusters: clusters
);
```

___

## Пакетные операции

Пакетные операции работают на уровне битовых масок — одна побитовая операция затрагивает до 64 сущностей за раз. Это на порядки быстрее поэлементной итерации с вызовом на каждую сущность.

#### Доступные операции:

| Метод | Описание                                                        |
|-------|-----------------------------------------------------------------|
| `BatchAdd<T>()` | Добавить компоненты (default-значения, 1–5 типов)               |
| `BatchSet<T>(value)` | Добавить компоненты с значениями (1–5 типов)                    |
| `BatchDelete<T>()` | Удалить компоненты или теги (1–5 типов)                         |
| `BatchEnable<T>()` | Включить компоненты (1–5 типов)                                 |
| `BatchDisable<T>()` | Отключить компоненты (1–5 типов)                                |
| `BatchSet<T>()` | Установить теги (1–5 типов)                                     |
| `BatchToggle<T>()` | Переключить компоненты или теги (1–5 типов)                     |
| `BatchApply<T>(bool)` | Установить или удалить компонент или тег по условию (1–5 типов) |
| `BatchDestroy()` | Уничтожить все подходящие сущности                              |
| `BatchUnload()` | Выгрузить все подходящие сущности                               |
| `EntitiesCount()` | Подсчитать количество подходящих сущностей                      |

#### Примеры:
```csharp
// Цепочка операций — добавить компонент, установить тег, отключить компонент
W.Query<All<Position>>()
    .BatchSet(new Velocity { Value = Vector3.One })
    .BatchSet<IsMovable>()
    .BatchDisable<Position>();

// Уничтожить все сущности с тегом IsDead
W.Query<All<Health, IsDead>>().BatchDestroy();

// Подсчёт сущностей
int count = W.Query<All<Position, Velocity>>().EntitiesCount();

// Фильтрация по кластерам и статусу сущности
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { 1, 2 };
W.Query<All<Position>>().BatchDelete<Velocity>(
    entities: EntityStatusType.Any,
    clusters: clusters
);

// Переключить тег — у кого был, будет снят; у кого не было, будет установлен
W.Query<All<Position>>().BatchToggle<IsVisible>();
```

{: .noteru }
Все пакетные операции поддерживают фильтрацию по `EntityStatusType` (Enabled/Disabled/Any) и `clusters`. Методы возвращают `WorldQuery` для построения цепочек.

___

## QueryMode

По умолчанию используется `QueryMode.Strict` — самый быстрый режим. Используйте `QueryMode.Flexible` только если во время итерации нужно модифицировать фильтруемые компоненты/теги на **других** сущностях:

```csharp
// Strict (по умолчанию) — быстрый путь для полных блоков
W.Query().For(
    static (ref Position pos) => { /* ... */ }
);

// Flexible — перепроверяет битовые маски на каждой итерации
W.Query().For(
    static (W.Entity entity, ref Position pos) => {
        // Можно модифицировать Position на других сущностях
    },
    queryMode: QueryMode.Flexible
);
```

___

## Стриппинг (уменьшение размера сборки)

StaticEcs активно использует дженерик-перегрузки: Query0–Query6 × варианты делегатов × Read-варианты × Parallel — это порождает огромное количество дженерик-специализаций, большинство из которых не используется в конкретном проекте. Для удаления неиспользуемого кода из сборки и значительного уменьшения её размера используйте стриппинг управляемого кода.

#### Unity:
Установите **Player Settings → Other Settings → Managed Stripping Level** в **Medium** или **High**. Это удалит неиспользуемые дженерик-инстанциации, генерируемые библиотекой.

#### .NET (publish trimming):
```xml
<PropertyGroup>
    <PublishTrimmed>true</PublishTrimmed>
    <TrimMode>link</TrimMode>
</PropertyGroup>
```

{: .importantru }
После включения стриппинга тщательно протестируйте сборку — агрессивный стриппинг может удалить код, доступ к которому происходит только через рефлексию. Если вы используете `RegisterAll` для автоматического обнаружения типов, убедитесь что нужные типы сохранены (например, через атрибут `[Preserve]` в Unity или TrimmerRootAssembly в .NET).

___

## Рекомендации

| Практика | Причина |
|----------|---------|
| Используйте `ForBlock` для критичных циклов | Прямые указатели, минимальный оверхед |
| Используйте `static` лямбды в `For` | Без аллокаций, JIT-инлайнинг |
| Используйте `in` для read-only компонентов | Корректная семантика отслеживания изменений |
| Группируйте сущности по `entityType` | Кэш-локальность |
| Ограничивайте запросы кластерами | Пропуск ненужных чанков |
| `QueryMode.Strict` по умолчанию | На 10–40% быстрее Flexible |
| Пакетные операции для массовых изменений | Одна операция на 64 сущности |
| Medium/High стриппинг в Unity | Удаление неиспользуемых дженерик-перегрузок |
| `UnmanagedPackArrayStrategy<T>` для сериализации | Блочное копирование памяти |
| Типизированные extension-методы для IL2CPP | На 10–25% быстрее дженерик-обёрток Entity |
