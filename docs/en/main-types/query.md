---
title: Query
parent: Main types
nav_order: 12
---

## Query
Queries - a mechanism that allows you to search for entities and their components in the world

___

#### Let's look at the basic capabilities of searching for entities in the world:
```c#
// There are many available query options
// World.QueryEntities.For()\With() returns an iterator of entities matching the condition
// The following types are available for applying component filtering conditions:

// All - filters entities for the presence of all specified components (overload from 1 to 8)
AllTypes<Types<Position, Direction, Velocity>> _all = default;
// or
All<Position, Direction, Velocity> _all2 = default;

// None - filters entities for the absence of all specified components (can be used only as part of other methods) (overloading from 1 to 8)
NoneTypes<Types<Name>> _none = default;
// or
None<Name> _none2 = default;

// Any - filters entities for the presence of any of the specified components (can only be used as part of other methods) (overloading from 1 to 8)
AnyTypes<Types<Position, Direction, Velocity>> _any = default;
// or
Any<Position, Direction, Velocity> _any2 = default;

// Analogs for tags
// TagAll - filters entities for the presence of all specified tags (overload from 1 to 8)
TagAllTypes<Tag<Unit, Player>> _all = default;
// or
TagAll<Unit, Player> _all2 = default;

// None - filters entities for the absence of all specified tags (can only be used as part of other methods) (overloading from 1 to 8).
TagNoneTypes<Tag<Unit>> _none = default;
// or
TagNone<Unit> _none2 = default;

// Any - filters entities for the presence of any of the specified tags (can only be used as part of other methods) (overloading from 1 to 8)
TagAnyTypes<Tag<Unit, Player>> _any = default;
// or
TagAny<Unit, Player> _any2 = default;

// Mask analogs
// MaskAll - filters entities for the presence of all specified tags (can only be used as part of other methods) (overloading from 1 to 8)
MaskAllTypes<Mask<Flammable, Frozen, Visible>> _all = default;
// or
MaskAll<Flammable, Frozen, Visible> _all2 = default;

// None - filters entities for the absence of all specified tags (can only be used as part of other methods) (overloading from 1 to 8).
MaskNoneTypes<Mask<Frozen>> _none = default;
// or
MaskNone<Frozen> _none2 = default;

// Any - filters entities for the presence of any of the specified tags (can only be used as part of other methods) (overloading from 1 to 8)
MaskAnyTypes<Mask<Flammable, Frozen, Visible>> _any = default;
// or
MaskAny<Flammable, Frozen, Visible> _any2 = default;

// All types above do not require explicit initialization, do not require caching, each of them takes no more than 1-2 bytes and can be used on the fly


// Different sets of filtering methods can be applied to the World.QueryEntities.For() method for example:
// Option 1 method through generic
foreach (var entity in MyWorld.QueryEntities.For<All<Position, Direction, Velocity>>()) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Variant with 1 method via value
var all = default(All<Position, Direction, Velocity>);
foreach (var entity in MyWorld.QueryEntities.For(all)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Option with 2 methods via generic
foreach (var entity in MyWorld.QueryEntities.For<
             All<Position, Velocity, Name>,
             None<Name>>()) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Variant with 2 methods via value
All<Position, Direction, Velocity> all2 = default;
None<Name> none2 = default;
foreach (var entity in MyWorld.QueryEntities.For(all2, none2)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Alternative with 2 methods via value
var all3 = Types<Position, Direction, Velocity>.All();
var none3 = Types<Name>.None();
foreach (var entity in MyWorld.QueryEntities.For(all3, none3)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}


// Also, all filtering methods can be grouped into a With type
// which can be applied to the World.QueryEntities.For() method, for example:

// Method 1 via generic
foreach (var entity in MyWorld.QueryEntities.For<With<
             All<Position, Velocity, Name>,
             None<Name>,
             Any<Position, Direction, Velocity>
         >>()) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Method 2 through values
With<
    All<Position, Velocity, Name>,
    None<Name>,
    Any<Position, Direction, Velocity>
> with = default;
foreach (var entity in MyWorld.QueryEntities.For(with)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Method 3 through values alternative
var with2 = With.Create(
    default(All<Position, Velocity, Name>),
    default(None<Name>),
    default(Any<Position, Direction, Velocity>)
);
foreach (var entity in MyWorld.QueryEntities.For(with2)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Method 4 through values alternative
var with3 = With.Create(
    Types<Position, Velocity, Name>.All(),
    Types<Name>.None(),
    Types<Position, Direction, Velocity>.Any()
);
foreach (var entity in MyWorld.QueryEntities.For(with3)) {
    entity.RefMut<Position>().Val *= entity.Ref<Velocity>().Val;
}
```
  
#### Look at additional ways to search for entities in the world:
```c#
// World.QueryComponents.For()\With() returns an iterator of entities matching the condition immediately with components 

// Option 1 with specifying a delegate and getting the required components at once, from 1 to 8 component types can be specified
MyWorld.QueryComponents.For<Position, Velocity, Name>((Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// you can remove generics, since they are derived from the passed function type
MyWorld.QueryComponents.For((Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// you can add a static constraint to the delegate to ensure that this delegate will not be allocated every time.
// in combination with Ecs.Context allows for convenient and productive code without creating closures in the delegate
MyWorld.QueryComponents.For(static (Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// You can also use WithAdds similar to With from the previous example, but allowing only secondary filtering methods (such as None, Any) for additional filtering of entities
// It should be noted that the components that are specified in the delegate are treated as All filter.
// i.e. WithAdds is just an addition to filtering and does not require specifying the components used.

WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
> with = default;

MyWorld.QueryComponents.With(with).For(static (Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// or
MyWorld.QueryComponents.With<WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
>>().For(static (Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});
```
  
#### Look at the special possibilities for finding entities in the world:
```c#
// Queries with structure-function passing 
// can be used to optimize or pass a state to a stratct or to pass logic.

// Let's define a structure-function that we can replace the delegate with.
// It must implement the IQueryFunction interface, specifying from 1-8 components.
readonly struct StructFunction : Ecs.IQueryFunction<Position, Velocity, Name> {
    public void Run(Ecs.Entity entity, ref Position position, ref Velocity velocity, ref Name name) {
        position.Val *= velocity.Val;
    }
}

// Option 1 with generic transmission
MyWorld.QueryComponents.For<Position, Velocity, Name, StructFunction>();

// Variant 1 with value transfer
MyWorld.QueryComponents.For<Position, Velocity, Name, StructFunction>(new StructFunction());

// Option 2 with With through generic
MyWorld.QueryComponents.With<WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
>>().For<Position, Velocity, Name, StructFunction>();

// Variant 2 with With through value
WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
> with = default;
MyWorld.QueryComponents.With(with).For<Position, Velocity, Name, StructFunction>();

// It is also possible to combine system and IQueryFunction, for example:
// this can improve code readability and increase perfomance + it allows accessing non-static members of the system
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