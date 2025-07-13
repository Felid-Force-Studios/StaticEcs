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

// All - filters entities for the presence of all specified enabled components (overload from 1 to 8)
AllTypes<Types<Position, Direction, Velocity>> _all = default;
// or
All<Position, Direction, Velocity> _all2 = default;

// AllOnlyDisabled - filters entities for the presence of all specified disabled components (overload from 1 to 8)
AllOnlyDisabled<Position, Direction, Velocity>
// AllWithDisabled - filters entities for the presence of all specified (enabled and disabled) components (overload from 1 to 8)
AllWithDisabled<Position, Direction, Velocity>

// None - filters entities for the absence of all specified enabled components (can be used only as part of other methods) (overloading from 1 to 8)
NoneTypes<Types<Name>> _none = default;
// or
None<Name> _none2 = default;

// NoneWithDisabled - filters entities for the absence of all specified (enabled and disabled) components (can only be used as part of other methods) (overloading from 1 to 8).
NoneWithDisabled<Position, Direction, Velocity>

// Any - filters entities for the presence of any of the specified enabled components (can only be used as part of other methods) (overloading from 1 to 8)
AnyTypes<Types<Position, Direction, Velocity>> _any = default;
// or
Any<Position, Direction, Velocity> _any2 = default;

// AnyOnlyDisabled - filters entities for the presence of any of the specified disabled components (can only be used as part of other methods) (overload from 1 to 8)
AnyOnlyDisabled<Position, Direction, Velocity>
    
// AnyWithDisabled - filters entities for the presence of any of the specified (enabled and disabled) components (can only be used as part of other methods) (overload from 1 to 8)
AnyWithDisabled<Position, Direction, Velocity>

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
foreach (var entity in World.QueryEntities.For<All<Position, Direction, Velocity>>()) {
    entity.Ref<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Variant with 1 method via value
var all = default(All<Position, Direction, Velocity>);
foreach (var entity in World.QueryEntities.For(all)) {
    entity.Ref<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Option with 2 methods via generic
foreach (var entity in World.QueryEntities.For<
             All<Position, Velocity, Name>,
             None<Name>>()) {
    entity.Ref<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Variant with 2 methods via value
All<Position, Direction, Velocity> all2 = default;
None<Name> none2 = default;
foreach (var entity in World.QueryEntities.For(all2, none2)) {
    entity.Ref<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Alternative with 2 methods via value
var all3 = Types<Position, Direction, Velocity>.All();
var none3 = Types<Name>.None();
foreach (var entity in World.QueryEntities.For(all3, none3)) {
    entity.Ref<Position>().Val *= entity.Ref<Velocity>().Val;
}


// Also, all filtering methods can be grouped into a With type
// which can be applied to the World.QueryEntities.For() method, for example:

// Method 1 via generic
foreach (var entity in World.QueryEntities.For<With<
             All<Position, Velocity, Name>,
             None<Name>,
             Any<Position, Direction, Velocity>
         >>()) {
    entity.Ref<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Method 2 through values
With<
    All<Position, Velocity, Name>,
    None<Name>,
    Any<Position, Direction, Velocity>
> with = default;
foreach (var entity in World.QueryEntities.For(with)) {
    entity.Ref<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Method 3 through values alternative
var with2 = With.Create(
    default(All<Position, Velocity, Name>),
    default(None<Name>),
    default(Any<Position, Direction, Velocity>)
);
foreach (var entity in World.QueryEntities.For(with2)) {
    entity.Ref<Position>().Val *= entity.Ref<Velocity>().Val;
}

// Method 4 through values alternative
var with3 = With.Create(
    Types<Position, Velocity, Name>.All(),
    Types<Name>.None(),
    Types<Position, Direction, Velocity>.Any()
);
foreach (var entity in World.QueryEntities.For(with3)) {
    entity.Ref<Position>().Val *= entity.Ref<Velocity>().Val;
}
```
  
#### Look at additional ways to search for entities in the world:
```c#
// World.QueryComponents.For()\With() returns an iterator of entities matching the condition immediately with components 

// Option 1 with specifying a delegate and getting the required components at once, from 1 to 8 component types can be specified
World.QueryComponents.For<Position, Velocity, Name>((World.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// you can remove generics, since they are derived from the passed function type
World.QueryComponents.For((World.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// you can add a static constraint to the delegate to ensure that this delegate will not be allocated every time.
// in combination with World.Context allows for convenient and productive code without creating closures in the delegate
World.QueryComponents.For(static (World.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// You can also use WithAdds similar to With from the previous example, but allowing only secondary filtering methods (such as None, Any) for additional filtering of entities
// It should be noted that the components that are specified in the delegate are treated as All filter.
// i.e. WithAdds is just an addition to filtering and does not require specifying the components used.

WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
> with = default;

World.QueryComponents.With(with).For(static (World.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

// or
World.QueryComponents.With<WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
>>().For(static (World.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});


// Also similarly, there are variants for searching by disabled or together with disabled components:
// It is important that the filter is applied only to components specified in the function, not to With components 
// If you need to set a filter for disabled components in With, use the constructs AllOnlyDisabled, AllWithDisabled, etc.

World.QueryComponents.With(with).ForOnlyDiabled(static (World.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});

World.QueryComponents.With(with).ForWithDiabled(static (World.Entity entity, ref Position position, ref Velocity velocity, ref Name name) => {
    position.Val *= velocity.Val;
});


// Multithreaded processing is also possible:
// Important! A special entity type is returned which blocks all operations except Ref, Has, etc.
// Cannot create, delete entities or components in multithreaded processing, only read and modify existing ones
// By default multithreading service is disabled, to enable it you need to specify ParallelQueryType as MaxThreadsCount in the config when creating a world 
// or (CustomThreadsCount and specify the maximum number of threads) - useful when you want to specify different numbers for different worlds


World.QueryComponents.Parallel.For(static (World.ROEntity entity, ref Position position, ref Velocity velocity) => {
    position.Val *= velocity.Val;
});

World.QueryComponents.Parallel.With(with).For(static (World.ROEntity entity, ref Position position, ref Velocity velocity) => {
    position.Val *= velocity.Val;
});

World.QueryComponents.Parallel.ForOnlyDiabled(static (World.ROEntity entity, ref Position position, ref Velocity velocity) => {
    position.Val *= velocity.Val;
});

World.QueryComponents.Parallel.ForWithDiabled(static (World.ROEntity entity, ref Position position, ref Velocity velocity) => {
    position.Val *= velocity.Val;
});
```
  
#### Look at the special possibilities for finding entities in the world:
```c#
// Queries with structure-function passing 
// can be used to optimize or pass a state to a stratct or to pass logic.

// Let's define a structure-function that we can replace the delegate with.
// It must implement the IQueryFunction interface, specifying from 1-8 components.
readonly struct StructFunction : World.IQueryFunction<Position, Velocity, Name> {
    public void Run(World.Entity entity, ref Position position, ref Velocity velocity, ref Name name) {
        position.Val *= velocity.Val;
    }
}

// Option 1 with generic transmission
World.QueryComponents.For<Position, Velocity, Name, StructFunction>();

// Variant 1 with value transfer
World.QueryComponents.For<Position, Velocity, Name, StructFunction>(new StructFunction());

// Option 2 with With through generic
World.QueryComponents.With<WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
>>().For<Position, Velocity, Name, StructFunction>();

// Variant 2 with With through value
WithAdds<
    None<Direction>,
    Any<Position, Direction, Velocity>
> with = default;
World.QueryComponents.With(with).For<Position, Velocity, Name, StructFunction>();

// It is also possible to combine system and IQueryFunction, for example:
// this can improve code readability and increase perfomance + it allows accessing non-static members of the system
public struct SomeFunctionSystem : IInitSystem, IUpdateSystem, World.IQueryFunction<Position, Velocity, Name> {
    private UserService1 _userService1;
    
    WithAdds<
        None<Types<Direction>>,
        Any<Types<Position, Direction, Velocity>>
    > with;
    
    public void Init() {
        _userService1 = World.Context<UserService1>.Get();
    }
    
   public void Update() {
       World.QueryComponents
            .With(with)
            .For<Position, Velocity, Name, SomeFunctionSystem>(ref this);
   }
    
    public void Run(World.Entity entity, ref Position position, ref Velocity velocity, ref Name name) {
        position.Val *= velocity.Val;
        _userService1.CallSomeMethod(name.Val);
    }
}
```

{: .important }
For each filtering method `QueryComponents.For()`, `QueryEntities.For()`
you can specify filtering by entity status, for example:

```c#
W.QueryEntities.For<All<Position>>(entities: EntityStatusType.Disabled)
    
World.QueryComponents.For<Position>((World.Entity entity, ref Position position) => {
    position.Val *= velocity.Val;
}, entities: EntityStatusType.Disabled);
```