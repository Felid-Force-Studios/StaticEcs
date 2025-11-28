---
title: Запросы
parent: Возможности
nav_order: 12
---

# Query
Запросы - механизм позволяющий осуществлять поиск сущностей и их компонентов в мире

___

## Query Methods
Типы позволяющие описать фильтрации по компонентам\тегам используемые в [QueryEntities](#query-entities) и [Query](#query-components)  
Все типы ниже не требуют явной инициализации, не требуют кеширования, каждый из них занимает 1 байт и может использоваться "на лету"  

### Компоненты:
`All` - фильтрует сущности на наличие всех указанных включенных компонентов (перегрузка от 1 до 8)
```c#
All<Position, Direction, Velocity> all = default;
```

`AllOnlyDisabled` - фильтрует сущности на наличие всех указанных отключенных компонентов (перегрузка от 1 до 8)
```c#
AllOnlyDisabled<Position, Direction, Velocity> all = default;
```

`AllWithDisabled` - фильтрует сущности на наличие всех указанных (включенных и отключенных) компонентов (перегрузка от 1 до 8)
```c#
AllWithDisabled<Position, Direction, Velocity> all = default;
```

`None` - фильтрует сущности на отсутствие всех указанных включенных компонентов (может использоваться только в составе других методов) (перегрузка от 1 до 8)
```c#
None<Position, Name> none = default;
```

`NoneWithDisabled` - фильтрует сущности на отсутствие всех указанных (включенных и отключенных) компонентов (может использоваться только в составе других методов) (перегрузка от 1 до 8)
```c#
NoneWithDisabled<Position, Direction, Velocity> none = default;
```

`Any` - фильтрует сущности на наличие любого из указанных включенных компонентов (может использоваться только в составе других методов) (перегрузка от 1 до 8)
```c#
Any<Position, Direction, Velocity> any = default;
```

`AnyOnlyDisabled` - фильтрует сущности на наличие любого из указанных отключенных компонентов (может использоваться только в составе других методов) (перегрузка от 1 до 8)
```c#
AnyOnlyDisabled<Position, Direction, Velocity> any = default;
```

`AnyWithDisabled` - фильтрует сущности на наличие любого из указанных (включенных и отключенных) компонентов (может использоваться только в составе других методов) (перегрузка от 1 до 8)
```c#
AnyWithDisabled<Position, Direction, Velocity> any = default;
```

### Теги:
`TagAll` - фильтрует сущности на наличие всех указанных тегов (перегрузка от 1 до 8)
```c#
All<Unit, Player> all = default;
```

`TagNone` - фильтрует сущности на отсутствие всех указанных тегов (может использоваться только в составе других методов) (перегрузка от 1 до 8)
```c#
TagNone<Unit, Player> none = default;
```

`TagAny` - фильтрует сущности на наличие любого из указанных тегов (может использоваться только в составе других методов) (перегрузка от 1 до 8)
```c#
TagAny<Unit, Player> any = default;
```

___

## Query Entities
Классический поиск сущностей в мире с указанными компонентами\тегами  
Все способы запросов ниже, не требуют кеширования, аллоцируются на стеке и могут использоваться "на лету"  

```c#
// Итерация по всем сущностям в мире без фильтрации:
foreach (var entity in W.Query.Entities()) {
    Console.WriteLine(entity.PrettyString);
}

// Различные наборы методов фильтрации могут быть применины к методу World.Query.Entities() например:
// Вариант с 1 методом через дженерик
foreach (var entity in W.Query.Entities<All<Position, Velocity, Direction>>()) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Вариант с 1 методом через значение
var all = default(All<Position, Direction, Velocity>);
foreach (var entity in W.Query.Entities(all)) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Вариант с 2 методами через дженерик
foreach (var entity in W.Query.Entities<
             All<Position, Velocity, Name>,
             None<Name>>()) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Вариант с 2 методами (All и None) через дженерик, можно указать до 8 методов фильтраций
foreach (var entity in W.Query.Entities<All<Position, Direction, Velocity>, None<Name>>()) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Вариант с 2 методами  через значение
All<Position, Direction, Velocity> all2 = default;
None<Name> none2 = default;
foreach (var entity in W.Query.Entities(all2, none2)) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}
```

Также все методы фильтрации могут быть сгруппированны в тип With  
который может применяться к методу `World.Query.Entities()` например:  

```c#
// Способ 1 через дженерика
foreach (var entity in W.Query.Entities<With<
             All<Position, Velocity, Direction>,
             None<Name>,
             TagAny<Unit, Player>
         >>()) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Способ 2 через значения
With<
    All<Position, Velocity, Direction>,
    None<Name>,
    TagAny<Unit, Player>
> with = default;
foreach (var entity in W.Query.Entities(with)) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Способ 3 через значения альтернативный
var with2 = With.Create(
    default(All<Position, Velocity, Direction>),
    default(None<Name>),
    default(TagAny<Unit, Player>)
);
foreach (var entity in W.Query.Entities(with2)) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}
```

___

## Query Components
Оптимизированный поиск сущностей и компонентов в мире с помощью делегатов  
Данный способ "под капотом" разворачивает циклы и является более удобным и эффективным способом  
Все способы запросов ниже, не требуют кеширования, аллоцируются на стеке и могут использоваться "на лету"  

- Пример поиска всех активных сущностей в мире:  
```c#
W.Query.For(entity => {
    Console.WriteLine(entity.PrettyString);
});
```

- Пример поиска всех сущностей с указанными компонентами, может быть указано от 1 до 8 типов компонентов:  
```c#
W.Query.For(static (ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});
```


- Можно указать сущность перед компонентами если она требуется:
```c#
W.Query.For(static (W.Entity ent, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});
```


- Для избегания аллокаций делегата возможно передать первым параметром данные любого пользовательского типа:

```c#
W.Query.For(deltaTime, static (ref float dt, W.Entity ent /* Сущность опционально */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value * dt;
});

// Можно использовать кортежи для нескольких параметров
W.Query.For((deltaTime, fixedDeltaTime), static (ref (float dt, float fdt) data, W.Entity entity /* Сущность опционально */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    // ...
});

// Также можно передать ref значение любого пользовательского типа
int count = 0;
W.Query.For(ref count, static (ref int counter, W.Entity ent /* Опционально */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
    counter++;
});
```


- Дополнительно можно указать в каком статусе необходимо искать сущностей или компоненты:

```c#
W.Query.For(
    static (ref Position pos, ref Velocity vel, ref Direction dir) => {
        // ...
    },
    entities: EntityStatusType.Disabled, // (Enabled, Disabled, Any) По умолчанию Enabled 
    components: ComponentStatus.Disabled // (Enabled, Disabled, Any) По умолчанию Enabled
);
```


- Также возможно использовать With() для дополнительной фильтрации сущностей
> Стоит заметить что компоненты которые указаны в делегате расцениваются как фильтр All  
> это значит что With() лишь дополняет фильтрацию и не требует указания используемых в делегате компонентов  

```c#
W.Query.With<TagAny<Unit, Player>>().For((ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});

// или
TagAny<Unit, Player> any = default;
W.Query.With(any).For((ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});

// или можно использовать With
With<
    None<Name>,
    TagAny<Unit, Player>
> with = default;

W.Query.With(with).For((ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});

```


### Parallel
Существует возможность многопоточной обработки:  
Важно! Внутри итерации всегда работает `QueryMode.Strict` это значит что модификация других (не итерируемой) сущностей запрещена (Будет ошибка в DEBUG)
Временно нельзя в многопоточной обработке создавать новые сущности, расширять мультикомпоненты, и отправлять или читать события (Будет улучшено в следующих версиях) (Будет ошибка в DEBUG)  
По умолчанию сервис многопоточной обработки отключен, чтобы его включить необходимо при создании мира указать в конфиге `ParallelQueryType` как `MaxThreadsCount`  
или (`CustomThreadsCount` и указать максимальное количество потоков) - полезно когда хочется задать разное количество для разных миров  
Все способы запросов ниже, не требуют кеширования, аллоцируются на стеке и могут использоваться "на лету"

`minChunkSize` - значение определяет минимальное количество потенциальных сущностей после которого функция будет использовать несколько потоков  
  
Примеры:  

```c#
W.Query.Parallel.For(minChunkSize: 50000, (W.Entity ent /* Опционально */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});

W.Query.Parallel.For(minChunkSize: 50000, deltaTime, (ref float dt, W.Entity ent /* Опционально */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value * dt;
});

With<
    None<Name>,
    TagAny<Unit, Player>
> with = default;
W.Query.Parallel.With(with).For(minChunkSize: 50000, ent => {
    ent.Add<Name>();
});
```

---


### Query Function
`Query` позволяет определять структуры функции вместо делегатов  
Может быть использовано для оптимизации, передачи состояния в структуру или для вынесения логики  

```c#
// Определим структуру-функцию которой можем заменить делегат
// Она должна реализовывать интерфейс World.IQueryFunction с указанием от 1-8 компонентов
readonly struct StructFunction : W.IQueryFunction<Position, Velocity, Direction> {
    public void Run(W.Entity entity, ref Position pos, ref Velocity vel, ref Direction dir) {
        pos.Value += dir.Value * vel.Value;
    }
}

// Вариант 1 с указанием дженерика (default Структура создается автоматически)
W.Query.For<Position, Velocity, Direction, StructFunction>();

// Вариант 1 с передачей через значение
W.Query.For<Position, Velocity, Direction, StructFunction>(new StructFunction());

// Вариант 1 с передачей через ref значение
var func = new StructFunction();
W.Query.For<Position, Velocity, Direction, StructFunction>(ref func);

// Вариант 2 с With через дженерик
W.Query.With<With<
    None<Name>,
    TagAny<Unit, Player>
>>().For<Position, Velocity, Direction, StructFunction>();

// Вариант 2 с With через значение
With<
    None<Name>,
    TagAny<Unit, Player>
> with = default;
W.Query.With(with).For<Position, Velocity, Direction, StructFunction>();

// Также возможно комбинировать систему и IQueryFunction, например:
// это может улучшить восприятия кода и увеличить производительность + это позволяет обращаться к нестатическим членам системы
public struct SomeFunctionSystem : IInitSystem, IUpdateSystem, W.IQueryFunction<Position, Velocity, Direction> {
    private UserService1 _userService1;
    
    With<
        None<Name>,
        TagAny<Unit, Player>
    > with;
    
    public void Init() {
        _userService1 = W.Context<UserService1>.Get();
    }
    
   public void Update() {
       W.Query
            .With(with)
            .For<Position, Velocity, Direction, SomeFunctionSystem>(ref this); // Передаем ссылку на функцию (систему)
   }
    
    // Определяем функцию
    public void Run(W.Entity entity, ref Position pos, ref Velocity vel, ref Direction dir) {
        pos.Value += dir.Value * vel.Value;
        _userService1.CallSomeMethod(entity);
    }
}
```

___

## Search
Существует возможность оптимизированой итерации для поиска кокнретной одной сущности:  
Данная функция позволяет использовать делегаты с ранним выходом при первой совпадении условия поиска.  

Пример:

```c#
if (W.Query.Search(out W.Entity foundEntity, (W.Entity entity, ref Position position, ref Health health) => {
        return position.Val.x > 100 && health.Val < 50;
    })) {
    // foundEntity logic ...
}
```

---

___

## Clusters

{: .importantru }
Для каждого метода фильтрации `Query.For()`, `Query.Parallel.For()`, `Query.Search()`, `Query.Entities()`  
можно указать конкретные кластеры в которых будет произведена итерация:

```c#
ReadOnlySpan<ushort> clusters = stackalloc ushort[3] { 2, 5, 12 };

W.Query.Entities<All<Position>>(clusters: clusters)
    
W.Query.For<Position>((W.Entity entity, ref Position position) => {
    position.Val *= velocity.Val;
}, clusters: clusters);
```

---


## Query Mode
Для каждого метода фильтрации `Query.For()`, `Query.Search()`, `Query.Entities()`
можно указать строгость обращения к не итерируемым сущностям
Доступны параметры: 
- `QueryMode.Default` - По умолчанию если в конфигурации мира `WorldConfig.DefaultQueryModeStrict = true`: будет `Strict` иначе `Flexible` 
- `QueryMode.Strict` - Запрещает модификацию фильтруемых типов компонентов\тегов у других сущностей (Итерация работает немного быстрей чем Flexible)
- `QueryMode.Flexible` - Позволяет модификацию фильтруемых типов компонентов\тегов у других сущностей, и корректно контролирует корректность текущей итерации

Примеры:

```c#
var anotherEntity = ...;

for (var entity : W.Query.Entities<All<Position>>(queryMode: QueryMode.Strict)) {
    anotherEntity.Delete<Position>(); // В DEBUG будет ошибка так как Strict режим и мы пытаемся модифицировать итерируемым типом компонента другую сущность
}

for (var entity : W.Query.Entities<All<Position>>(queryMode: QueryMode.Flexible)) {
    anotherEntity.Delete<Position>(); // Ошибки не будет, и anotherEntity будет корректно исключена из текущей итерации
}

for (var entity : W.Query.Entities<None<Position>>(queryMode: QueryMode.Flexible)) {
    anotherEntity.Add<Position>(); // Ошибки не будет, и anotherEntity будет корректно исключена из текущей итерации
}

// Аналогично для все остальных видов фильтрации, All, Any, None и тд - при Flexible режиме итерация будет стабильна
// Flexible полезен например для иерархий или кешей, когда мы обращаемся и модифицируем сущности из компонентов других сущностей или из сохраненных значений
// в остальных случаях предпочтительно использовать Strict режим по соображениям производительности
```

___


{: .importantru }
Для каждого метода фильтрации `Query.For()`, `Query.Parallel.For()`, `Query.Search()`, `Query.Entities()`  
можно указать фильтрацию по статусу сущности, например:

```c#
W.Query.Entities<All<Position>>(entities: EntityStatusType.Disabled)
    
W.Query.For<Position>((W.Entity entity, ref Position position) => {
    position.Val *= velocity.Val;
}, entities: EntityStatusType.Disabled);
```