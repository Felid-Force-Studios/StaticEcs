---
title: Шаблоны
parent: RU
nav_order: 4
---

# Live templates

Component live template
```csharp
public static class $COMPONENT$Extension {
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static ref $COMPONENT$ $COMPONENT$(this $WORLD$.Entity entity) {
        return ref $WORLD$.Components<$COMPONENT$>.Value.Ref(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static ref $COMPONENT$ Add$COMPONENT$(this $WORLD$.Entity entity) {
        return ref $WORLD$.Components<$COMPONENT$>.Value.Add(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Add$COMPONENT$(this $WORLD$.Entity entity, $COMPONENT$ value) {
        $WORLD$.Components<$COMPONENT$>.Value.Add(entity) = value;
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static ref $COMPONENT$ TryAdd$COMPONENT$(this $WORLD$.Entity entity) {
        return ref $WORLD$.Components<$COMPONENT$>.Value.TryAdd(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void TryAdd$COMPONENT$(this $WORLD$.Entity entity, $COMPONENT$ value) {
        $WORLD$.Components<$COMPONENT$>.Value.TryAdd(entity) = value;
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Put$COMPONENT$(this $WORLD$.Entity entity, $COMPONENT$ value) {
        $WORLD$.Components<$COMPONENT$>.Value.Put(entity, value);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool Has$COMPONENT$(this $WORLD$.Entity entity) {
        return $WORLD$.Components<$COMPONENT$>.Value.Has(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Delete$COMPONENT$(this $WORLD$.Entity entity) {
        $WORLD$.Components<$COMPONENT$>.Value.Delete(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool TryDelete$COMPONENT$(this $WORLD$.Entity entity) {
        return $WORLD$.Components<$COMPONENT$>.Value.TryDelete(entity);
    }
}
```


Standard component live template
```csharp
public static class $COMPONENT$Extension {
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static ref $COMPONENT$ $COMPONENT$(this $WORLD$.Entity entity) {
        return ref $WORLD$.StandardComponents<$COMPONENT$>.Value.Ref(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Set$COMPONENT$(this $WORLD$.Entity entity, $COMPONENT$ value) {
        $WORLD$.StandardComponents<$COMPONENT$>.Value.Ref(entity) = value;
    }
}
```

Tag live template
```csharp
public static class $TAG$Extension {
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Set$TAG$(this $WORLD$.Entity entity) {
        $WORLD$.Tags<$TAG$>.Value.Set(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool Has$TAG$(this $WORLD$.Entity entity) {
        return $WORLD$.Tags<$TAG$>.Value.Has(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Delete$TAG$(this $WORLD$.Entity entity) {
        $WORLD$.Tags<$TAG$>.Value.Delete(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool TryDelete$TAG$(this $WORLD$.Entity entity) {
        return $WORLD$.Tags<$TAG$>.Value.TryDelete(entity);
    }
}
```

Mask live template
```csharp
public static class $MASK$Extension {
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Set$MASK$(this $WORLD$.Entity entity) {
        $WORLD$.Masks<$MASK$>.Value.Set(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool Has$MASK$(this $WORLD$.Entity entity) {
        return $WORLD$.Masks<$MASK$>.Value.Has(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Delete$MASK$(this $WORLD$.Entity entity) {
        $WORLD$.Masks<$MASK$>.Value.Delete(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool TryDelete$MASK$(this $WORLD$.Entity entity) {
        return $WORLD$.Masks<$MASK$>.Value.TryDelete(entity);
    }
}
```