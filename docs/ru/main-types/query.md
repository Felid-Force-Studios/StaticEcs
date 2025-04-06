---
title: Запросы
parent: Основные типы
nav_order: 12
---

## Query
Запросы - механизм позволяющий осуществлять поиск сущностей и их компонентов в мире

___

#### Рассмотрим базовые возможности поиска сущностей в мире:
```c#
// Существует множество доступных вариантов запросов
// World.QueryEntities.For()\With() возвращает итератор сущностей подходящих под условие
// Для применение условий фильтрации компонентов доступны следующие типы:

// All - фильтрует сущности на наличие всех указанных компонентов (перегрузка от 1 до 8)
AllTypes<Types<Position, Direction, Velocity>> _all = default;
// или так
All<Position, Direction, Velocity> _all2 = default;

// None - фильтрует сущности на отсутсвие всех указанных компонентов (может использоваться только в составе других методов) (перегрузка от 1 до 8)
NoneTypes<Types<Name>> _none = default;
// или так
None<Name> _none2 = default;

// Any - фильтрует сущности на наличие любого из указанных компонентов (может использоваться только в составе других методов) (перегрузка от 1 до 8)
AnyTypes<Types<Position, Direction, Velocity>> _any = default;
// или так
Any<Position, Direction, Velocity> _any2 = default;

// Аналоги для тегов
// TagAll - фильтрует сущности на наличие всех указанных тегов (перегрузка от 1 до 8)
TagAllTypes<Tag<Unit, Player>> _all = default;
// или так
TagAll<Unit, Player> _all2 = default;

// None - фильтрует сущности на отсутсвие всех указанных тегов (может использоваться только в составе других методов) (перегрузка от 1 до 8)
TagNoneTypes<Tag<Unit>> _none = default;
// или так
TagNone<Unit> _none2 = default;

// Any - фильтрует сущности на наличие любого из указанных тегов (может использоваться только в составе других методов) (перегрузка от 1 до 8)
TagAnyTypes<Tag<Unit, Player>> _any = default;
// или так
TagAny<Unit, Player> _any2 = default;

// Аналоги для масок
// MaskAll - фильтрует сущности на наличие всех указанных масок (может использоваться только в составе других методов) (перегрузка от 1 до 8)
MaskAllTypes<Mask<Flammable, Frozen, Visible>> _all = default;
// или так
MaskAll<Flammable, Frozen, Visible> _all2 = default;

// None - фильтрует сущности на отсутсвие всех указанных масок (может использоваться только в составе других методов) (перегрузка от 1 до 8)
MaskNoneTypes<Mask<Frozen>> _none = default;
// или так
MaskNone<Frozen> _none2 = default;

// Any - фильтрует сущности на наличие любой из указанных масок (может использоваться только в составе других методов) (перегрузка от 1 до 8)
MaskAnyTypes<Mask<Flammable, Frozen, Visible>> _any = default;
// или так
MaskAny<Flammable, Frozen, Visible> _any2 = default;

// Все типы выше не требуют явной инициализации, не требуют кеширования, каждый из них занимает не больше 1-2 байт и может использоваться "на лету"


// Различные наборы методов фильтрации могут быть применины к методу World.QueryEntities.For() например:
// Вариант с 1 методом через дженерик
foreach (var entity in MyWorld.QueryEntities.For<All<Position, Direction, Velocity>>()) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Вариант с 1 методом через значение
var all = default(All<Position, Direction, Velocity>);
foreach (var entity in MyWorld.QueryEntities.For(all)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Вариант с 2 методами  через дженерик
foreach (var entity in MyWorld.QueryEntities.For<
             All<Position, Velocity, Name>,
             None<Name>>()) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Вариант с 2 методами  через значение
All<Position, Direction, Velocity> all2 = default;
None<Name> none2 = default;
foreach (var entity in MyWorld.QueryEntities.For(all2, none2)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Альтернативный вариант с 2 методами  через значение
var all3 = Types<Position, Direction, Velocity>.All();
var none3 = Types<Name>.None();
foreach (var entity in MyWorld.QueryEntities.For(all3, none3)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}


// Также все методы фильтрации могут быть сгруппированны в тип With
// который может применяться к методу World.QueryEntities.For() например:

// Method 1 via generic
foreach (var entity in MyWorld.QueryEntities.For<With<
             All<Position, Velocity, Name>,
             None<Name>,
             Any<Position, Direction, Velocity>
         >>()) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Способ 2 через значения
With<
    All<Position, Velocity, Name>,
    None<Name>,
    Any<Position, Direction, Velocity>
> with = default;
foreach (var entity in MyWorld.QueryEntities.For(with)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Способ 3 через значения альтернативный
var with2 = With.Create(
    default(All<Position, Velocity, Name>),
    default(None<Name>),
    default(Any<Position, Direction, Velocity>)
);
foreach (var entity in MyWorld.QueryEntities.For(with2)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Способ 4 через значения альтернативный
var with3 = With.Create(
    Types<Position, Velocity, Name>.All(),
    Types<Name>.None(),
    Types<Position, Direction, Velocity>.Any()
);
foreach (var entity in MyWorld.QueryEntities.For(with3)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}
```
  
#### Посмотрим на дополнительные способы поиска сущностей в мире:
```c#
// World.QueryComponents.For()\With() возвращает итератор сущностей подходящих под условие cразу с компонентами 

// Вариант 1 с указанием делегата и сразу получением нужных компонентов, может быть указано от 1 до 8 типов компонентов
MyWorld.QueryComponents.For<Position, Velocity, Name>((Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// можно убрать дженерики, так как они выводятся из типа переданной функции
MyWorld.QueryComponents.For((Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// можно добавить ограничение static для делегата для того чтобы гарантировать что данный делегат не будет аллоцироваться каждый раз
// в совокупности с Ecs.Context дает возможность удобного и производительного кода без создания замыканий в делегате
MyWorld.QueryComponents.For(static (Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// Также можно использовать WithAdds аналогичный With из прошлого примера но разрешающий указания только вторичных методов фильтрации (такиех как None, Any) для дополнительной фильтрации сущностей
// Стоит заметить что компоненты которые указаны в делегате расцениваются как фильтр All
// то есть WithAdds лишь дополнят фильтрации и не требует указания используемых компонентов

WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
> with = default;

MyWorld.QueryComponents.With(with).For(static (Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// или так
MyWorld.QueryComponents.With<WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
>>().For(static (Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});
```
  
#### Посмотрим на особые возможности поиска сущностей в мире:
```c#
// Запросы с передачей структуры-функции 
// может использоваться для оптимизации или передачи состояния в стракт или для вынесения логики

// Определим структуру-функцию которой можем заменить делегат
// Она должна реализовывать интерфейс IQueryFunction с указанием от 1-8 компонентов
readonly struct StructFunction : Ecs.IQueryFunction<Position, Velocity, Name> {
    public void Run(Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) {
        position.Val *= velocity.Val;
    }
}

// Вариант 1 с передачей через дженерик
MyWorld.QueryComponents.For<Position, Velocity, Name, StructFunction>();

// Вариант 1 с передачей через значение
MyWorld.QueryComponents.For<Position, Velocity, Name, StructFunction>(new StructFunction());

// Вариант 2 с With через дженерик
MyWorld.QueryComponents.With<WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
>>().For<Position, Velocity, Name, StructFunction>();

// Вариант 2 с With через значение
WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
> with = default;
MyWorld.QueryComponents.With(with).For<Position, Velocity, Name, StructFunction>();

// Также возможно комбинировать систему и IQueryFunction, например:
// это может улучшить восприятия кода и увеличить производительность + это позволяет обращаться в нестатическим членам системы
public struct SomeFunctionSystem : IInitSystem, IUpdateSystem, Ecs.IQueryFunction<Position, Velocity, Name> {
    private UserService1 _userService1;
    
    WithAdds<
        None<Types<Direction>>,
        Any<Types<Position, Direction, Velocity>>
    > with;
    
    public void Init() {
        _userService1 = Ecs.Context<UserService1>.Get();
    }
    
   public void Update() {
       MyWorld.QueryComponents
            .With(with)
            .For<Position, Velocity, Name, SomeFunctionSystem>(ref this);
   }
    
    public void Run(Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) {
        position.Val *= velocity.Val;
        _userService1.CallSomeMethod(name.Val);
    }
}
```