---
title: Системы
parent: Основные типы
nav_order: 10
---

## SystemsType
Тип-тег-идентификатор систем, служит для изоляции статических данных при создании групп систем в одном процессе
- Представлен в виде пользовательской структуры без данных с маркер интерфейсом `ISystemsType`

#### Пример:
```c#
public struct BaseSystemsType : ISystemsType { }
public struct FixedSystemsType : ISystemsType { }
public struct LateSystemsType : ISystemsType { }
```

___

## Systems
Системы, контролирует и менеджируют создание и запуск систем
- Представлен в виде статического класса `Systems<ISystemsType>`


```c#
// Системы существуют 3 видов и могут использоваться во всех комбинациях вместе или отдельно

// Система IInitSystem - метод Init() запускается один раз при инициализации MySystems.Initialize()
// Пример:
public struct SomeInitSystem : IInitSystem {
    public void Init() { }
}

// Система IUpdateSystem  - метод Update() запускается каждый раз при вызове MySystems.Update();
// Пример:
public struct SomeUpdateSystem : IUpdateSystem {
    public void Update() { }
}

// Система IDestroySystem - метод Destroy() запускается один раз при вызове MySystems.Destroy();
// Пример:
public struct SomeDestroySystem : IDestroySystem {
    public void Destroy() { }
}

 // Комбинированная система
 public struct SomeInitDestroySystem : IInitSystem, IDestroySystem {
     public void Init() { }
     public void Destroy() { }
 }

 // Комбинированная система
 public struct SomeComboSystem : IInitSystem, IUpdateSystem, IDestroySystem {
     public void Init() { }
     public void Update() { }
     public void Destroy() { }
 }
```

___

#### Создание и операции:
```c#
// Определяем идентификатор систем
public struct MySystemsType : ISystemsType { }

// Определяем тип-алиас для удобного доступа к системам
public abstract class MySystems : MyEcs.Systems<MySystemsType> { }

// Здесь будет созданны структуры для систем
MySystems.Create();

// Добавление системы НЕ реализующей IUpdateSystem, то есть Init и\или Destroy системы
MySystems.AddCallOnce(new SomeInitSystem());
MySystems.AddCallOnce(new SomeDestroySystem>());
MySystems.AddCallOnce(new SomeInitDestroySystem>());

// Добавление системы реализующей IUpdateSystem, с наличием любых имплементаций таких как Init или Destroy
MySystems.AddUpdate(new SomeComboSystem());

// Важно! Системы запускаются в порядке перeданным вторым аргументом (по умолчанию order 0)
MySystems.AddUpdate(new SomeComboSystem(), order: 3);

// это значит что сначала будут запущены все Init системы в том порядке в котором добавлены
// затем в игровом цикле будут выполняться по порядку все Update системы
// затем по порядку вызовутся все системы типа Destroy при уничтожении мира

// Важно! Системы могут быть структурами или классами
// (использование структур может существенно увеличить производительность для небольших систем)

// Есть возможность подключать системы батчами что может существенно увеличить производительность
// Добавление батча систем, каждая система может реализовывать любые типы систем но обязана иметь реализацию IUpdateSystem
MySystems.AddUpdate(
    new SomeUpdateSystem1(),
    new SomeComboSystem1(),
    new SomeComboSystem2(),
    new SomeComboSystem3(),
    new SomeComboSystem4(),
    new SomeComboSystem5(),
    new SomeComboSystem()
);

// Здесь будут вызваны все Init системы
MySystems.Initialize();

// Здесь будут вызваны все Update системы
MySystems.Update();

// Здесь будут вызваны все Destroy системы
MySystems.Destroy();
```