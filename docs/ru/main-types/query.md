---
title: Запросы
parent: Основные типы
nav_order: 12
---

# Query
Запросы - механизм позволяющий осуществлять поиск сущностей и их компонентов в мире

___

## Query Methods
Типы позволяющие описать фильтрации по компонентам\тегам\маскам используемые в [QueryEntities](#query-entities) и [QueryComponents](#query-components)  
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

### Маски:
`MaskAll` - фильтрует сущности на наличие всех указанных масок (может использоваться только в составе других методов) (перегрузка от 1 до 8)
```c#
MaskAll<Flammable, Frozen, Visible> all = default;
```

`MaskNone` - фильтрует сущности на отсутствие всех указанных масок (может использоваться только в составе других методов) (перегрузка от 1 до 8)
```c#
MaskNone<Flammable, Frozen, Visible> none = default;
```

`MaskAny` - фильтрует сущности на наличие любой из указанных масок (может использоваться только в составе других методов) (перегрузка от 1 до 8)
```c#
MaskAny<Flammable, Frozen, Visible> any = default;
```

___

## Query Entities
Классический поиск сущностей в мире с указанными компонентами\тегами\масками  
Все способы запросов ниже, не требуют кеширования, аллоцируются на стеке и могут использоваться "на лету"  

```c#
// Различные наборы методов фильтрации могут быть применины к методу World.QueryEntities.For() например:
// Вариант с 1 методом через дженерик
foreach (var entity in W.QueryEntities.For<All<Position, Velocity, Direction>>()) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Вариант с 1 методом через значение
var all = default(All<Position, Direction, Velocity>);
foreach (var entity in W.QueryEntities.For(all)) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Вариант с 2 методами через дженерик
foreach (var entity in W.QueryEntities.For<
             All<Position, Velocity, Name>,
             None<Name>>()) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Вариант с 2 методами (All и None) через дженерик, можно указать до 8 методов фильтраций
foreach (var entity in W.QueryEntities.For<All<Position, Direction, Velocity>, None<Name>>()) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Вариант с 2 методами  через значение
All<Position, Direction, Velocity> all2 = default;
None<Name> none2 = default;
foreach (var entity in W.QueryEntities.For(all2, none2)) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}
```

Также все методы фильтрации могут быть сгруппированны в тип With  
который может применяться к методу `World.QueryEntities.For()` например:  

```c#
// Способ 1 через дженерика
foreach (var entity in W.QueryEntities.For<With<
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
foreach (var entity in W.QueryEntities.For(with)) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Способ 3 через значения альтернативный
var with2 = With.Create(
    default(All<Position, Velocity, Direction>),
    default(None<Name>),
    default(TagAny<Unit, Player>)
);
foreach (var entity in W.QueryEntities.For(with2)) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}
```

___

## Query Components
Оптимизированный поиск сущностей и компонентов в мире с помощью делегатов  
Данный способ "под капотом" разворачивает циклы и является более удобным и эффективным способом  
Все способы запросов ниже, не требуют кеширования, аллоцируются на стеке и могут использоваться "на лету"  
> `QueryEntities` все еще полезен когда нужен "ранний" выход из цикла или не нужны данные компонентов  


- Пример поиска всех сущностей с указанными компонентами, может быть указано от 1 до 8 типов компонентов:  
```c#
W.QueryComponents.For(static (ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});
```


- Можно указать сущность перед компонентами если она требуется:
```c#
W.QueryComponents.For(static (W.Entity ent, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});
```


- Для избегания аллокаций делегата возможно передать первым параметром данные любого пользовательского типа:

```c#
W.QueryComponents.For(deltaTime, static (float dt, W.Entity ent /* Опционально */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value * dt;
});

// Можно использовать кортежи для нескольких параметров
W.QueryComponents.For((deltaTime, fixedDeltaTime), static ((float dt, float fdt) data, W.Entity entity /* Опционально */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    // ...
});

// Также можно передать ref значение структуры любого пользовательского типа
int count = 0;
W.QueryComponents.For(ref count, static (ref int counter, W.Entity ent /* Опционально */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
    counter++;
});
```


- Дополнительно можно указать в каком статусе необходимо искать сущностей или компоненты:

```c#
W.QueryComponents.For(
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
W.QueryComponents.With<TagAny<Unit, Player>>().For((ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});

// или
TagAny<Unit, Player> any = default;
W.QueryComponents.With(any).For((ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});

// или можно использовать WithAdds\With (WithAdds аналогичен With но разрешающий указания только вторичных методов фильтрации (таких как None, Any))
WithAdds<
    None<Name>,
    TagAny<Unit, Player>
> with = default;

W.QueryComponents.With(with).For((ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});

```


### Parallel
Существует возможность многопоточной обработки:  
Важно! Возвращается специальный тип сущности который запрещает все операции такие как (`Add`, `Put` ...), разрешены только `Ref`, `Has` и тд  
Нельзя в многопоточной обработке создавать, удалять сущности или компоненты, только читать и изменять существующие  
По умолчанию сервис многопоточной обработки отключен, чтобы его включить необходимо при создании мира указать в конфиге `ParallelQueryType` как `MaxThreadsCount`  
или (`CustomThreadsCount` и указать максимальное количество потоков) - полезно когда хочется задать разное количество для разных миров  
Все способы запросов ниже, не требуют кеширования, аллоцируются на стеке и могут использоваться "на лету"

`minChunkSize` - значение определяет минимальное количество потенциальных сущностей после которого функция будет использовать несколько потоков  
  
Примеры:  

```c#
W.QueryComponents.Parallel.For(minChunkSize: 50000, (W.ROEntity ent /* Опционально */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});

W.QueryComponents.Parallel.For(minChunkSize: 50000, deltaTime, (float dt, W.ROEntity ent /* Опционально */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value * dt;
});

WithAdds<
    None<Name>,
    TagAny<Unit, Player>
> with = default;
W.QueryComponents.Parallel.With(with).For(minChunkSize: 50000, (ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});
```

---


### Query Function
`QueryComponents` позволяет определять структуры функции вместо делегатов  
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
W.QueryComponents.For<Position, Velocity, Direction, StructFunction>();

// Вариант 1 с передачей через значение
W.QueryComponents.For<Position, Velocity, Direction, StructFunction>(new StructFunction());

// Вариант 1 с передачей через ref значение
var func = new StructFunction();
W.QueryComponents.For<Position, Velocity, Direction, StructFunction>(ref func);

// Вариант 2 с With через дженерик
W.QueryComponents.With<WithAdds<
    None<Name>,
    TagAny<Unit, Player>
>>().For<Position, Velocity, Direction, StructFunction>();

// Вариант 2 с With через значение
WithAdds<
    None<Name>,
    TagAny<Unit, Player>
> with = default;
W.QueryComponents.With(with).For<Position, Velocity, Direction, StructFunction>();

// Также возможно комбинировать систему и IQueryFunction, например:
// это может улучшить восприятия кода и увеличить производительность + это позволяет обращаться к нестатическим членам системы
public struct SomeFunctionSystem : IInitSystem, IUpdateSystem, W.IQueryFunction<Position, Velocity, Direction> {
    private UserService1 _userService1;
    
    WithAdds<
        None<Name>,
        TagAny<Unit, Player>
    > with;
    
    public void Init() {
        _userService1 = World.Context<UserService1>.Get();
    }
    
   public void Update() {
       W.QueryComponents
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

---


{: .importantru }
Для каждого метода фильтрации `QueryComponents.For()`, `QueryEntities.For()`  
можно указать фильтрацию по статусу сущности, например:

```c#
W.QueryEntities.For<All<Position>>(entities: EntityStatusType.Disabled)
    
World.QueryComponents.For<Position>((World.Entity entity, ref Position position) => {
    position.Val *= velocity.Val;
}, entities: EntityStatusType.Disabled);
```