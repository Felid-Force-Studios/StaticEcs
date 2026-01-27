---
title: Директивы компилятора
parent: Возможности
nav_order: 16
---

## Директивы компилятора

Директивы управляют режимами компиляции библиотеки. Определяются через `DefineConstants` в `.csproj` или в настройках проекта Unity.

___

### FFS_ECS_ENABLE_DEBUG

Включает режим отладки — проверки состояния мира, валидность сущностей, корректность регистрации компонентов и тегов, блокировки запросов и многое другое.

- **Автоматически включен** в конфигурации `DEBUG` (при сборке через `dotnet build` без `-c Release`)
- В Release-конфигурации все проверки полностью удаляются компилятором — нулевое влияние на производительность

```xml
<!-- .csproj — явное включение для любой конфигурации -->
<PropertyGroup>
    <DefineConstants>FFS_ECS_ENABLE_DEBUG</DefineConstants>
</PropertyGroup>
```

{: .importantru }
Рекомендуется всегда тестировать проект в режиме отладки. Отладочные проверки обнаруживают распространённые ошибки: обращение к уничтоженным сущностям, незарегистрированные компоненты, модификация данных во время итерации и т.д.

#### Примеры проверок в режиме отладки:
- Мир создан/инициализирован перед использованием
- Сущность не уничтожена и загружена
- Компонент/тег зарегистрирован перед использованием
- Отсутствует модификация данных из параллельного запроса
- Чанк/кластер зарегистрирован перед операциями

___

### FFS_ECS_DISABLE_DEBUG

Принудительно отключает режим отладки, даже если определён `DEBUG` или `FFS_ECS_ENABLE_DEBUG`.

```xml
<!-- .csproj — отключить отладку в Debug-конфигурации -->
<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
    <DefineConstants>FFS_ECS_DISABLE_DEBUG</DefineConstants>
</PropertyGroup>
```

{: .noteru }
Логика активации: `(DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG`. Директива `FFS_ECS_DISABLE_DEBUG` имеет наивысший приоритет.

___

### ENABLE_IL2CPP

Активирует атрибуты Unity IL2CPP для оптимизации AOT-компиляции:
- `[Il2CppSetOption(Option.NullChecks, false)]` — отключение проверок на null
- `[Il2CppSetOption(Option.ArrayBoundsChecks, false)]` — отключение проверок границ массивов
- `[Il2CppEagerStaticClassConstruction]` — ранняя инициализация статических классов

{: .noteru }
Определяется автоматически при сборке Unity проекта для IL2CPP платформ. Вручную определять не требуется.

___

### ENABLE_IL2CPP_CHECKS

Включает проверки NullChecks и ArrayBoundsChecks для IL2CPP даже при использовании `ENABLE_IL2CPP`. По умолчанию эти проверки отключены для максимальной производительности.

```xml
<!-- .csproj — включить IL2CPP проверки (для отладки на устройстве) -->
<PropertyGroup>
    <DefineConstants>ENABLE_IL2CPP_CHECKS</DefineConstants>
</PropertyGroup>
```

___

### FFS_ECS_DISABLE_CHANGED_TRACKING

Отключает поддержку Changed-отслеживания на этапе компиляции. При определении методы `Read` / `ReadBlock` билдера запросов недоступны. Фильтры `AllChanged<T>`, `NoneChanged<T>`, `NoneAdded<T>`, `NoneDeleted<T>`, а также метод сущности `Mut<T>()` также недоступны.

```xml
<!-- .csproj — глобально отключить Changed-отслеживание -->
<PropertyGroup>
    <DefineConstants>FFS_ECS_DISABLE_CHANGED_TRACKING</DefineConstants>
</PropertyGroup>
```

{: .noteru }
Используйте эту директиву если Changed-отслеживание не нужно и вы хотите убрать любые связанные накладные расходы. Отслеживание Added/Deleted не затрагивается этой директивой.

___

### Сводная таблица

| Директива | Назначение | По умолчанию |
|-----------|-----------|--------------|
| `FFS_ECS_ENABLE_DEBUG` | Включает отладочные проверки | Включена в `DEBUG` |
| `FFS_ECS_DISABLE_DEBUG` | Принудительно отключает отладку | Не определена |
| `ENABLE_IL2CPP` | Атрибуты IL2CPP оптимизации | Автоматически в Unity IL2CPP |
| `ENABLE_IL2CPP_CHECKS` | Включает проверки в IL2CPP | Не определена |
| `FFS_ECS_DISABLE_CHANGED_TRACKING` | Отключает Changed-отслеживание | Не определена |
