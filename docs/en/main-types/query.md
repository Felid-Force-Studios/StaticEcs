---
title: Query
parent: Main types
nav_order: 12
---

# Query
Queries are a mechanism to search for entities and their components in the world

___

## Query Methods
Types allowing to describe filtering by components/tags/masks used in [QueryEntities](#query-entities) and [QueryComponents](#query-components)  
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

### Masks:
`MaskAll` - filters entities for the presence of all specified masks (can only be used as part of other methods) (overloading from 1 to 8)
```c#
MaskAll<Flammable, Frozen, Visible> all = default;
```

`MaskNone` - filters entities for the absence of all specified masks (can only be used as part of other methods) (overloading from 1 to 8)
```c#
MaskNone<Flammable, Frozen, Visible> none = default;
```

`MaskAny` - filters entities for the presence of any of the specified masks (can only be used as part of other methods) (overloading from 1 to 8)
```c#
MaskAny<Flammable, Frozen, Visible> any = default;
```

___

## Query Entities
Classic search for entities in the world with specified components/tags/masks  
All query methods below are cache-free, allocated on the stack and can be used on-the-fly.

```c#
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
> `QueryEntities` is still useful when want an “early” exit from a loop or don't need the component data  


- Example search for all entities with specified components, 1 to 8 component types can be specified:
```c#
W.QueryComponents.For(static (ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});
```


- It is possible to specify an entity before the components if it is required:
```c#
W.QueryComponents.For(static (W.Entity ent, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});
```


- To avoid delegate allocations, it is possible to pass any custom data type as the first parameter:  
```c#
W.QueryComponents.For(deltaTime, static (float dt, W.Entity ent /* Optional */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value * dt;
});

// It is possible to use tuples for multiple parameters
W.QueryComponents.For((deltaTime, fixedDeltaTime), static ((float dt, float fdt) data, W.Entity entity /* Optional */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    // ...
});

// It is also possible to pass ref the value of a structure of any custom type
int count = 0;
W.QueryComponents.For(ref count, static (ref int counter, W.Entity ent /* Optional */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
    counter++;
});
```


- Additionally, it is possible to specify in which status you want to search for entities or components:
```c#
W.QueryComponents.For(
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
W.QueryComponents.With<TagAny<Unit, Player>>().For((ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});

// or
TagAny<Unit, Player> any = default;
W.QueryComponents.With(any).For((ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});

// or it is possible to use WithAdds\With (WithAdds is similar to With but allowing only secondary filtering methods (such as None, Any)).
WithAdds<
    None<Name>,
    TagAny<Unit, Player>
> with = default;

W.QueryComponents.With(with).For((ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});
```


### Parallel
There is a possibility of multithreaded processing:  
Important! A special entity type is returned which prohibits all operations such as (`Add`, `Put` ...), only `Ref`, `Has` etc. are allowed  
You cannot create, delete entities or components in multithreaded processing, only read and modify existing ones.  
Multithreading service is disabled by default, to enable it you should specify `ParallelQueryType` as `MaxThreadsCount` in the config when creating a world.  
or (`CustomThreadsCount` and specify the maximum number of threads) - useful when you want to specify different numbers for different worlds.  
All the query methods below do not require caching, are allocated on the stack and can be used on the fly  

`minChunkSize` - the value defines the minimum number of potential entities after which the function will use several threads.  

Examples:  

```c#
W.QueryComponents.Parallel.For(minChunkSize: 50000, (W.ROEntity ent /* Optional */, ref Position pos, ref Velocity vel, ref Direction dir) => {
    pos.Value += dir.Value * vel.Value;
});

W.QueryComponents.Parallel.For(minChunkSize: 50000, deltaTime, (float dt, W.ROEntity ent /* Optional */, ref Position pos, ref Velocity vel, ref Direction dir) => {
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
`QueryComponents` allows you to define function structures instead of delegates  
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
W.QueryComponents.For<Position, Velocity, Direction, StructFunction>();

// Option 1 with value transfer
W.QueryComponents.For<Position, Velocity, Direction, StructFunction>(new StructFunction());

// Option 1 with ref value transfer
var func = new StructFunction();
W.QueryComponents.For<Position, Velocity, Direction, StructFunction>(ref func);

// Option 2 with With via generic
W.QueryComponents.With<WithAdds<
    None<Name>,
    TagAny<Unit, Player>
>>().For<Position, Velocity, Direction, StructFunction>();

// Option 2 with `With` via value
WithAdds<
    None<Name>,
    TagAny<Unit, Player>
> with = default;
W.QueryComponents.With(with).For<Position, Velocity, Direction, StructFunction>();

// It is also possible to combine system and IQueryFunction for example:
// it can improve code perception and increase performance + it allows you to access non-static members of the system
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


{: .important }
For each filtering method `QueryComponents.For()`, `QueryEntities.For()`
you can specify filtering by entity status, for example:

```c#
W.QueryEntities.For<All<Position>>(entities: EntityStatusType.Disabled)
    
World.QueryComponents.For<Position>((World.Entity entity, ref Position position) => {
    position.Val *= velocity.Val;
}, entities: EntityStatusType.Disabled);
```