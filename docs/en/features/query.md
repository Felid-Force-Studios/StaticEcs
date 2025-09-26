---
title: Query
parent: Features
nav_order: 12
---

# Query
Queries are a mechanism to search for entities and their components in the world

___

## Query Methods
Types allowing to describe filtering by components/tags used in [QueryEntities](#query-entities) and [Query](#query-components)  
All of the types below do not require explicit initialization, do not require caching, each takes 1 byte and can be used on the fly

### Components:
`All` - filters entities for the presence of all specified included components (overload from 1 to 8)
```c#
All<Position, Direction, Velocity> all = default;
```

`AllOnlyDisabled` - filters entities for the presence of all specified disabled components (overload from 1 to 8)
```c#
AllOnlyDisabled<Position, Direction, Velocity> all = default;
```

`AllWithDisabled` - filters entities for the presence of all specified (enabled and disabled) components (overload from 1 to 8)
```c#
AllWithDisabled<Position, Direction, Velocity> all = default;
```

`None` - filters entities for the absence of all specified included components (can only be used as part of other methods) (overloading from 1 to 8)
```c#
None<Position, Name> none = default;
```

`NoneWithDisabled` - filters entities for the absence of all specified (enabled and disabled) components (can only be used as part of other methods) (overload from 1 to 8)
```c#
NoneWithDisabled<Position, Direction, Velocity> none = default;
```

`Any` - filters entities for the presence of any of the specified included components (can only be used as part of other methods) (overloading from 1 to 8)
```c#
Any<Position, Direction, Velocity> any = default;
```

`AnyOnlyDisabled` - filters entities for the presence of any of the specified disabled components (can only be used as part of other methods) (overload from 1 to 8)
```c#
AnyOnlyDisabled<Position, Direction, Velocity> any = default;
```

`AnyWithDisabled` - filters entities for the presence of any of the specified (enabled and disabled) components (can only be used as part of other methods) (overload from 1 to 8)
```c#
AnyWithDisabled<Position, Direction, Velocity> any = default;
```

### Tags:
`TagAll` - filters entities for the presence of all specified tags (overload from 1 to 8)
```c#
All<Unit, Player> all = default;
```

`TagNone` - filters entities for the absence of all specified tags (can be used only as part of other methods) (overloading from 1 to 8)
```c#
TagNone<Unit, Player> none = default;
```

`TagAny` - filters entities for the presence of any of the specified tags (can only be used as part of other methods) (overloading from 1 to 8)
```c#
TagAny<Unit, Player> any = default;
```
___

## Query Entities
Classic query for entities in the world with specified components/tags  
All query methods below are cache-free, allocated on the stack and can be used on-the-fly.

```c#
// Iterate over all active entities in the world without filtering:
foreach (var entity in W.QueryEntities.For()) {
    Console.WriteLine(entity.PrettyString);
}

// Different sets of filtering methods can be applied to the World.QueryEntities.For() method for example:
// Option with 1 method via generic
foreach (var entity in W.QueryEntities.For<All<Position, Velocity, Direction>>()) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Option with 1 method via value
var all = default(All<Position, Direction, Velocity>);
foreach (var entity in W.QueryEntities.For(all)) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Option with 2 methods via common
foreach (var entity in W.QueryEntities.For<
             All<Position, Velocity, Name>,
             None<Name>>()) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Option with 2 methods (All and None) through the common, it is possible to specify up to 8 filtering methods
foreach (var entity in W.QueryEntities.For<All<Position, Direction, Velocity>, None<Name>>()) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Option with 2 methods via value
All<Position, Direction, Velocity> all2 = default;
None<Name> none2 = default;
foreach (var entity in W.QueryEntities.For(all2, none2)) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}
```

Also, all filtering methods can be grouped into a With type  
which can be applied to the `World.QueryEntities.For()` method, for example:

```c#
// Option 1 via generic
foreach (var entity in W.QueryEntities.For<With<
             All<Position, Velocity, Direction>,
             None<Name>,
             TagAny<Unit, Player>
         >>()) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Option 2 via value
With<
    All<Position, Velocity, Direction>,
    None<Name>,
    TagAny<Unit, Player>
> with = default;
foreach (var entity in W.QueryEntities.For(with)) {
    entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
}

// Option 3 through values alternative
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
Optimized search for entities and components in the world using delegates  
This "under the hood" method deploys loops and is a more convenient and efficient way of doing things  
All query methods below, do not require caching, are allocated on the stack and can be used on-the-fly  

- An example of searching for all active entities in the world:
```c#
W.Query.For(entity => {
    Console.WriteLine(entity.PrettyString);
});
```


- Example search for all entities with specified components, 1 to 8 component types can be specified:
```c#
W.Query.For(static (ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});
```


- It is possible to specify an entity before the components if it is required:
```c#
W.Query.For(static (W.Entity ent, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});
```



- To avoid delegate allocations, it is possible to pass any custom data type as the first param:

```c#

W.Query.For(deltaTime, static (ref float dt, W.Entity ent /* Optional */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value * dt;
});

// It is possible to use tuples for multiple parameters
W.Query.For((deltaTime, fixedDeltaTime), static (ref (float dt, float fdt) data, W.Entity entity /* Optional */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    // ...
});

// It is also possible to pass ref the value of any custom type
int count = 0;
W.Query.For(ref count, static (ref int counter, W.Entity ent /* Optional */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
    counter++;
});
```


- Additionally, it is possible to specify in which status you want to search for entities or components:

```c#
W.Query.For(
    static (ref Position pos, ref Velocity vel, ref Direction dir) => {
        // ...
    },
    entities: EntityStatusType.Disabled, // (Enabled, Disabled, Any) Default Enabled
    components: ComponentStatus.Disabled // (Enabled, Disabled, Any) Default Enabled
);
```


- It is also possible to use With() for additional filtering of entities  
> It should be noted that the components that are specified in the delegate are considered as All filter  
> This means that With() only completes the filtering and does not require specifying the components used in the delegate.

```c#
W.Query.With<TagAny<Unit, Player>>().For((ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});

// or
TagAny<Unit, Player> any = default;
W.Query.With(any).For((ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});

// or it is possible to use With
With<
    None<Name>,
    TagAny<Unit, Player>
> with = default;

W.Query.With(with).For((ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});
```


### Parallel
There is a possibility of multithreaded processing:  
Important! Within an iteration always works `QueryMode.Strict` which means that modification of other (non-iterated) entities is forbidden (There will be an error in DEBUG).
It is temporarily not possible in multithreaded processing to create new entities, extend multicomponents, and send or read events (Will be improved in future versions) (There will be a bug in DEBUG)  
Multithreading service is disabled by default, to enable it you should specify `ParallelQueryType` as `MaxThreadsCount` in the config when creating a world.  
or (`CustomThreadsCount` and specify the maximum number of threads) - useful when you want to specify different numbers for different worlds.  
All the query methods below do not require caching, are allocated on the stack and can be used on the fly  

`minChunkSize` - the value defines the minimum number of potential entities after which the function will use several threads.  

Examples:  

```c#
W.Query.Parallel.For(minChunkSize: 50000, (W.Entity ent /* Optional */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});

W.Query.Parallel.For(minChunkSize: 50000, deltaTime, (ref float dt, W.Entity ent /* Optional */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value * dt;
});

With<
    None<Name>,
    TagAny<Unit, Player>
> with = default;
W.Query.Parallel.With(with).For(minChunkSize: 50000, (ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});
```

---

### Query Function
`Query` allows you to define function structures instead of delegates  
Can be used for optimization, passing state to a structure, or for passing logic.  

```c#
// Let's define a structure-function for which we can replace the delegate
// It should implement the World.IQueryFunction interface with the specification of 1-8 components
readonly struct StructFunction : W.IQueryFunction<Position, Velocity, Direction> {
    public void Run(W.Entity entity, ref Position pos, ref Velocity vel, ref Direction dir) {
        pos.Value += dir.Value * vel.Value;
    }
}

// Option 1 with indication of generic (default The structure is created automatically)
W.Query.For<Position, Velocity, Direction, StructFunction>();

// Option 1 with value transfer
W.Query.For<Position, Velocity, Direction, StructFunction>(new StructFunction());

// Option 1 with ref value transfer
var func = new StructFunction();
W.Query.For<Position, Velocity, Direction, StructFunction>(ref func);

// Option 2 with With via generic
W.Query.With<With<
    None<Name>,
    TagAny<Unit, Player>
>>().For<Position, Velocity, Direction, StructFunction>();

// Option 2 with `With` via value
With<
    None<Name>,
    TagAny<Unit, Player>
> with = default;
W.Query.With(with).For<Position, Velocity, Direction, StructFunction>();

// It is also possible to combine system and IQueryFunction for example:
// it can improve code perception and increase performance + it allows you to access non-static members of the system
public struct SomeFunctionSystem : IInitSystem, IUpdateSystem, W.IQueryFunction<Position, Velocity, Direction> {
    private UserService1 _userService1;
    
    With<
        None<Name>,
        TagAny<Unit, Player>
    > with;
    
    public void Init() {
        _userService1 = World.Context<UserService1>.Get();
    }
    
   public void Update() {
       W.Query
            .With(with)
            .For<Position, Velocity, Direction, SomeFunctionSystem>(ref this); // Pass a reference to a function (system)
   }
    
    // Define the function
    public void Run(W.Entity entity, ref Position pos, ref Velocity vel, ref Direction dir) {
        pos.Value += dir.Value * vel.Value;
        _userService1.CallSomeMethod(entity);
    }
}
```

---


## Query Mode
For each filtering method `Query.For()`, `Query.Search()`, `QueryEntities.For()`
it is possible to specify the strictness of treatment of non-iterated entities
Options available:
- `QueryMode.Default` - By default if in the world configuration `WorldConfig.DefaultQueryModeStrict = true`: will be `Strict` otherwise `Flexible`.
- `QueryMode.Strict` - Forbid modification of filtered component/tag types in other entities (Iteration works slightly faster than Flexible)
- `QueryMode.Flexible` - Allows modification of filtered types of components/tags from other entities, and correctly controls the correctness of the current iteration

Examples:

```c#
var anotherEntity = ...;

for (var entity : W.QueryEntities.For<All<Position>>(queryMode: QueryMode.Strict)) {
    anotherEntity.Delete<Position>(); // There will be an error in DEBUG because Strict mode and we are trying to modify another entity with the iterated component type
}

for (var entity : W.QueryEntities.For<All<Position>>(queryMode: QueryMode.Flexible)) {
    anotherEntity.Delete<Position>(); // There will be no error and anotherEntity will be correctly excluded from the current iteration
}

for (var entity : W.QueryEntities.For<None<Position>>(queryMode: QueryMode.Flexible)) {
    anotherEntity.Add<Position>(); // There will be no error and anotherEntity will be correctly excluded from the current iteration
}

// Similarly for all other types of filtering, All, Any, None, etc - in Flexible mode iteration will be stable.
// Flexible mode is useful for example for hierarchies or caches, when we access and modify entities from components of other entities or from stored values.
// in other cases Strict mode is preferable for performance reasons
```

___


{: .important }
For each filtering method  `Query.For()`, `Query.Parallel.For()`, `Query.Search()`, `QueryEntities.For()`  
you can specify filtering by entity status, for example:

```c#
W.QueryEntities.For<All<Position>>(entities: EntityStatusType.Disabled)
    
World.Query.For<Position>((World.Entity entity, ref Position position) => {
    position.Val *= velocity.Val;
}, entities: EntityStatusType.Disabled);
```