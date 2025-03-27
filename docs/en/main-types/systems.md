---
title: System
parent: Main types
nav_order: 10
---

## SystemsType
Type-tag system identifier, used to isolate static data when creating groups of systems in the same process
- Represented as a user structure without data with a marker interface `ISystemsType`

#### Example:
```c#
public struct BaseSystemsType : ISystemsType { }
public struct FixedSystemsType : ISystemsType { }
public struct LateSystemsType : ISystemsType { }
```

___

## Systems
Systems, controls and manages the creation and run of systems
- Represented as a static class `Systems<ISystemsType>`


```c#
// The systems are of 3 types and can be used in all combinations together or separately

// IInitSystem - Init() method is run once when MySystems.Initialize() is called
// Example:
public struct SomeInitSystem : IInitSystem {
    public void Init() { }
}

// IUpdateSystem - the Update() method runs every time MySystems.Update() is called
// Example:
public struct SomeUpdateSystem : IUpdateSystem {
    public void Update() { }
}

// IDestroySystem - the Destroy() method runs once when MySystems.Destroy() is called;
// Example:
public struct SomeDestroySystem : IDestroySystem {
    public void Destroy() { }
}

 // Combined system
 public struct SomeInitDestroySystem : IInitSystem, IDestroySystem {
     public void Init() { }
     public void Destroy() { }
 }

 // Combined system
 public struct SomeComboSystem : IInitSystem, IUpdateSystem, IDestroySystem {
     public void Init() { }
     public void Update() { }
     public void Destroy() { }
 }
```

___

#### Creation and operations:
```c#
// Define system identifier
public struct MySystemsType : ISystemsType { }

// Define type-alias for easy access to systems
public abstract class MySystems : MyEcs.Systems<MySystemsType> { }

// The structures for the systems will be created here
MySystems.Create();

// Adding a system NOT implementing IUpdateSystem, i.e. Init and / or Destroy system
MySystems.AddCallOnce(new SomeInitSystem());
MySystems.AddCallOnce(new SomeDestroySystem>());
MySystems.AddCallOnce(new SomeInitDestroySystem>());

// Adding a system implementing IUpdateSystem, with any implementations such as Init or Destroy.
MySystems.AddUpdate(new SomeComboSystem());

// Important! The systems are started in the order passed by the second argument (default order 0)
MySystems.AddUpdate(new SomeComboSystem(), order: 3);

// this means that first all Init systems will be started in the order in which they were added.
// then in the game loop all Update systems will be executed in order.
// then all Destroy type systems will be called in order when the world is destroyed.

// Important! Systems can be structures or classes
// (using structures can significantly increase perfomance for small systems)

// It is possible to connect systems in batches, which can significantly increase performance
// Adding a batches of systems, each system can implement any types of systems but must have IUpdateSystem implementation.
MySystems.AddUpdate(
    new SomeUpdateSystem1(),
    new SomeComboSystem1(),
    new SomeComboSystem2(),
    new SomeComboSystem3(),
    new SomeComboSystem4(),
    new SomeComboSystem5(),
    new SomeComboSystem()
);

// All Init systems will be called here
MySystems.Initialize();

// All Update systems will be called here
MySystems.Update();

// All Destroy systems will be called here
MySystems.Destroy();
```