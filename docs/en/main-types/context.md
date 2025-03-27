---
title: Context
parent: Main types
nav_order: 11
---

## Context
Context is an alternative to DI, a simple mechanism for storing and transferring user data and services to systems and other methods
- Represented as a static class `Ecs<IWorldType>.Context<T>`

___
#### Basic operations:
```csharp
// User classes and services
public class UserService1 { }
public class UserService2 { }

// Adding necessary objects to the context, it is not necessary to add objects to the context before initialization, new data can also be added in the process of systems operation
// It is important to remember that if the context is used in Init systems, the data should be passed there before Ecs.Initialize() or before the call in the call chain of a particular Init system. 
// Important! The context can store strictly 1 object of 1 type - an error will occur if the Set method is set repeatedly of the same type.
MyEcs.Context<UserService1>.Set(new UserService1(), clearOnDestroy: true);
MyEcs.Context<UserService2>.Set(new UserService2());

// If Replace is called, the specified type will be set or replaced without error
MyEcs.Context<UserService2>.Replace(new UserService2());

// Check if there is a value of this type in the context
bool has = MyEcs.Context<UserService2>.Has();

// Remove the value from the context
MyEcs.Context<UserService2>.Remove();

// Important! context will be cleared when MyEcs.Destroy() is called; if clearOnDestroy true was specified when set to
```