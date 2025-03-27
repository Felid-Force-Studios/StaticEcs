---
title: Performance
parent: EN
nav_order: 3
---

# Perfomance
Current benchmarks : [BENCHMARKS](../Benchmark.md)

When using il2Cpp in Unity, it's worth noting that direct calls to get components are a little bit faster:  
For example:
```csharp
// Performance in il2Cpp (there is no difference in Mono) can be better in the second option by 10-40%
// The same applies to tags and masks and all other methods HasAllOf<>, Delete<>, etc.
ref var position = ref entity.RefMut<Position>(); // sugar method via the entity
ref var position = ref Ecs.Components<Position>.Value.RefMut(entity); // direct call
```
```csharp
// It is also possible to use extension methods that are practically close in performance to the direct call
// To create them, you can use the live template for rider (read below) or codegenerate (WIP)
public static class PositionExtension {
    [MethodImpl(AggressiveInlining)]
    public static ref Position RefMutPosition(this Ecs.Entity entity) {
        return ref Ecs.Components<Position>.Value.RefMut(entity);
    }
}
ref var position = ref entity.RefMutPosition();
```