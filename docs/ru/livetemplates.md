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
    public static ref $COMPONENT$ Mut$COMPONENT$(this $ECS$.Entity entity) {
        return ref $ECS$.Components<$COMPONENT$>.Value.RefMut(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static ref readonly $COMPONENT$ $COMPONENT$(this $ECS$.Entity entity) {
        return ref $ECS$.Components<$COMPONENT$>.Value.Ref(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static ref $COMPONENT$ Add$COMPONENT$(this $ECS$.Entity entity) {
        return ref $ECS$.Components<$COMPONENT$>.Value.Add(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Add$COMPONENT$(this $ECS$.Entity entity, $COMPONENT$ value) {
        $ECS$.Components<$COMPONENT$>.Value.Add(entity) = value;
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static ref $COMPONENT$ TryAdd$COMPONENT$(this $ECS$.Entity entity) {
        return ref $ECS$.Components<$COMPONENT$>.Value.TryAdd(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void TryAdd$COMPONENT$(this $ECS$.Entity entity, $COMPONENT$ value) {
        $ECS$.Components<$COMPONENT$>.Value.TryAdd(entity) = value;
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Put$COMPONENT$(this $ECS$.Entity entity, $COMPONENT$ value) {
        $ECS$.Components<$COMPONENT$>.Value.Put(entity, value);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool Has$COMPONENT$(this $ECS$.Entity entity) {
        return $ECS$.Components<$COMPONENT$>.Value.Has(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Delete$COMPONENT$(this $ECS$.Entity entity) {
        $ECS$.Components<$COMPONENT$>.Value.Delete(entity);
    }
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool TryDelete$COMPONENT$(this $ECS$.Entity entity) {
        return $ECS$.Components<$COMPONENT$>.Value.TryDelete(entity);
    }
}
```


Standard component live template
```csharp
public static class $COMPONENT$Extension {
    
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static ref readonly $COMPONENT$ $COMPONENT$(this $ECS$.Entity entity) {
        return ref $ECS$.StandardComponents<$COMPONENT$>.Value.Ref(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static ref $COMPONENT$ Mut$COMPONENT$(this $ECS$.Entity entity) {
        return ref $ECS$.StandardComponents<$COMPONENT$>.Value.RefMut(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Set$COMPONENT$(this $ECS$.Entity entity, $COMPONENT$ value) {
        $ECS$.StandardComponents<$COMPONENT$>.Value.RefMut(entity) = value;
    }
}
```

Tag live template
```csharp
public static class $TAG$Extension {
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Set$TAG$(this $Ecs$.Entity entity) {
        $Ecs$.Tags<$TAG$>.Value.Set(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool Has$TAG$(this $Ecs$.Entity entity) {
        return $Ecs$.Tags<$TAG$>.Value.Has(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Delete$TAG$(this $Ecs$.Entity entity) {
        $Ecs$.Tags<$TAG$>.Value.Delete(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool TryDelete$TAG$(this $Ecs$.Entity entity) {
        return $Ecs$.Tags<$TAG$>.Value.TryDelete(entity);
    }
}
```

Mask live template
```csharp
public static class $MASK$Extension {
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Set$MASK$(this $Ecs$.Entity entity) {
        $Ecs$.Masks<$MASK$>.Value.Set(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool Has$MASK$(this $Ecs$.Entity entity) {
        return $Ecs$.Masks<$MASK$>.Value.Has(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Delete$MASK$(this $Ecs$.Entity entity) {
        $Ecs$.Masks<$MASK$>.Value.Delete(entity);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool TryDelete$MASK$(this $Ecs$.Entity entity) {
        return $Ecs$.Masks<$MASK$>.Value.TryDelete(entity);
    }
}
```