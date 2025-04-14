---
title: Производительность
parent: RU
nav_order: 3
---

# Perfomance
Производительность : [BENCHMARKS](../Benchmark.md)

При использовании il2Cpp в Unity стоит отметить что прямые вызовы для получения компонентов чуть чуть быстрее:  
Например:
```csharp
// производительность в il2Cpp (в Mono нет разницы) может быть лучше во втором варианте на 10-40%
// это же касается тегов и масок и всех остальных методов HasAllOf<>, Delete<> и тд
ref var position = ref entity.Ref<Position>(); // сахарный метод через сущность
ref var position = ref World.Components<Position>.Value.Ref(entity); // прямой вызов
```
```csharp
// так же можно использовать методы расширения которые практически приближены по производительности к прямому вызову
// Для их создания можно воспользоваться шаблоном live template для rider (читать далее) или кодогенерацией (WIP)
public static class PositionExtension {
    [MethodImpl(AggressiveInlining)]
    public static ref Position Position(this World.Entity entity) {
        return ref World.Components<Position>.Value.Ref(entity);
    }
}
ref var position = ref entity.Position();
```