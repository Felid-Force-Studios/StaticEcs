---
title: RU
has_toc: false
parent: Main page
---

![Version](https://img.shields.io/badge/version-0.9.82-blue.svg?style=for-the-badge)  

___

### 🚀 **[Benchmarks](../Benchmark.md)** 🚀
### ⚙️ **[Unity module](https://github.com/Felid-Force-Studios/StaticEcs-Unity)** ⚙️

# Static ECS - C# Entity component system framework
- Легковесность
- Производительность
- Отсутсвие аллокаций
- Отсутствие зависимостей
- Без Unsafe в ядре
- Основан на статике и структурах
- Типобезопасность
- Бесплатные абстракции
- Мощный механизм запросов
- Минимум болерплейта
- Совместимость с Unity и другими C# движками

{: .noteru-title }
> Ограничения и особенности:
> - Не потокобезопасен
> - Могут быть незначительные изменения API

## Оглавление
* [Контакты](#контакты)
* [Установка](#установка)
* [Концепция](#концепция)
* [Быстрый старт](#быстрый-старт)
* [Основные типы](maintypes.md)
  * [Сущность](main-types/entity.md)
  * [Упакованная сущность](main-types/packedentity.md)
  * [Компонент](main-types/component.md)
  * [Стандартный компонент](main-types/standardcomponent.md)
  * [Мульти-компонент](main-types/multicomponent.md)
  * [Тег](main-types/tag.md)
  * [Маска](main-types/mask.md)
  * [Мир](main-types/world.md)
  * [Системы](main-types/systems.md)
  * [Контекст](main-types/context.md)
  * [Запросы](main-types/query.md)
* [Дополнительны возможности](additionalfeatures.md)
  * [Авто обработчики](additional-features/autohandlers.md)
  * [События](additional-features/events.md)
  * [Отношения](additional-features/relations.md)
  * [Директивы компилятора](additional-features/compilerdirectives.md)
* [Производительность](performance.md)
* [Шаблоны](livetemplates.md)
* [Unity интеграция](unityintegrations.md)
* [Вопрос-ответ](faq.md)
* [Лицензия](#лицензия)


# Контакты
* [Telegram](https://t.me/felid_force_studios)

# Установка
* ### В виде исходников
  Со страцины релизов или как архив из нужной ветки. В ветке `master` стабильная проверенная версия
* ### Установка для Unity
  Как git модуль `https://github.com/Felid-Force-Studios/StaticEcs.git` в Unity PackageManager  
  или добавление в манифест `Packages/manifest.json` `"com.felid-force-studios.static-ecs": "https://github.com/Felid-Force-Studios/StaticEcs.git"`

# Концепция
> - Основная идея данной реализации в статике, все данные о мире и компонентах находятся в статических классах, что дает вохможность избегать дорогостоящих виртуальных вызовов, иметь удобный API со множеством сахара
> - Даннный фреймворк нацелен на максмальную простоту использования, скорость и комфорт написания кода без жертв в производительности
> - Доступно создание мульти-миров, строгая типизация, обширные бесплатные абстракции
> - Основан на sparse-set архитектуре, ядро вдохновленно серией библиотек от Leopotam
> - Фреймворк создан для нужд частного проекта и выложен в open-source.

# Быстрый старт
```csharp
using FFS.Libraries.StaticEcs;

// Определяем тип мира
public struct WT : IWorldType { }

// Определяем типы-алиасы для удобного доступа к типам библиотеки
public abstract class W : World<WT> { }

// Определяем тип систем
public struct SystemsType : ISystemsType { }

// Определяем тип-алиас для удобного доступа к системам
public abstract class Systems : W.Systems<SystemsType> { }

// Определяем компоненты
public struct Position : IComponent { public Vector3 Value; }
public struct Velocity : IComponent { public float Value; }

// Определяем системы
public readonly struct VelocitySystem : IUpdateSystem {
    public void Update() {
        foreach (var entity in W.QueryEntities.For<All<Position, Velocity>>()) {
            entity.Ref<Position>().Value *= entity.Ref<Velocity>().Value;
        }
    }
}

public class Program {
    public static void Main() {
        // Создаем данные мира
        W.Create(WorldConfig.Default());
        
        // Регестрируем компоненты
        W.RegisterComponentType<Position>();
        W.RegisterComponentType<Velocity>();
        
        // Инициализацируем мир
        W.Initialize();
        
        // Создаем системы
        Systems.Create();
        Systems.AddUpdate(new VelocitySystem());

        // Инициализацируем системы
        Systems.Initialize();

        // Создание сущности
        var entity = W.Entity.New(
            new Velocity { Value = 1f },
            new Position { Value = Vector3.One }
        );
        // Обновление всех систем - вызывается в каждом кадре
        Systems.Update();
        // Уничтожение систем
        Systems.Destroy();
        // Уничтожение мира и очистка всех данных
        W.Destroy();
    }
}
```

# Лицензия
[MIT license](https://github.com/Felid-Force-Studios/StaticEcs/blob/master/LICENSE.md)

