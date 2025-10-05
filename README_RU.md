![Version](https://img.shields.io/badge/version-1.1.preview-blue.svg?style=for-the-badge)

### ЯЗЫК
[RU](./README_RU.md)
[EN](./README.md)
___
### 🚀 **[Benchmarks](https://gist.github.com/blackbone/6d254a684cf580441bf58690ad9485c3)** 🚀
### ⚙️ **[Unity module](https://github.com/Felid-Force-Studios/StaticEcs-Unity)** ⚙️
### 📖️ **[Документация](https://felid-force-studios.github.io/StaticEcs/ru/)** 📖️

___

### ❗️ **[Гайд миграции с версии 1.0.x на 1.1.x](https://felid-force-studios.github.io/StaticEcs/ru/migrationguide.html)** ❗️

# Static ECS - C# Binary Entity component system framework
- Легковесность
- Производительность
- Отсутствие аллокаций
- Без Unsafe
- Основан на статике и структурах
- Типобезопасность
- Бесплатные абстракции
- Мощный механизм запросов
- Минимум болерплейта
- Совместимость с Unity и другими C# движками
- Совместимость с Native AOT

## Оглавление
* [Контакты](#контакты)
* [Установка](#установка)
* [Концепция](#концепция)
* [Быстрый старт](#быстрый-старт)
* [Возможности](https://felid-force-studios.github.io/StaticEcs/ru/maintypes.html)
  * [Сущность](https://felid-force-studios.github.io/StaticEcs/ru/features/entity.html)
  * [Глобальный идентификатор сущности](https://felid-force-studios.github.io/StaticEcs/ru/features/gid.html)
  * [Компонент](https://felid-force-studios.github.io/StaticEcs/ru/features/component.html)
  * [Тег](https://felid-force-studios.github.io/StaticEcs/ru/features/tag.html)
  * [Мульти-компонент](https://felid-force-studios.github.io/StaticEcs/ru/features/multicomponent.html)
  * [Отношения](https://felid-force-studios.github.io/StaticEcs/ru/features/relations.html)
  * [Мир](https://felid-force-studios.github.io/StaticEcs/ru/features/world.html)
  * [Системы](https://felid-force-studios.github.io/StaticEcs/ru/features/systems.html)
  * [Контекст](https://felid-force-studios.github.io/StaticEcs/ru/features/context.html)
  * [Запросы](https://felid-force-studios.github.io/StaticEcs/ru/features/query.html)
  * [События](https://felid-force-studios.github.io/StaticEcs/ru/features/events.html)
  * [Конфигураторы компонентов](https://felid-force-studios.github.io/StaticEcs/ru/features/configs.html)
  * [Сериализация](https://felid-force-studios.github.io/StaticEcs/ru/features/serialization.html)
  * [Директивы компилятора](https://felid-force-studios.github.io/StaticEcs/ru/features/compilerdirectives.html)
* [Производительность](https://felid-force-studios.github.io/StaticEcs/ru/performance.html)
* [Unity интеграция](https://felid-force-studios.github.io/StaticEcs/ru/unityintegrations.html)
* [Лицензия](#лицензия)


# Контакты
* [felid.force.studios@gmail.com](mailto:felid.force.studios@gmail.com)
* [Telegram](https://t.me/felid_force_studios)

# Установка
Библиотека имеет зависимость на [StaticPack](https://github.com/Felid-Force-Studios/StaticPack) версии `1.0.3` для бинарной сериализации, StaticPack должен быть так же установлен
* ### В виде исходников
  Со страницы релизов или как архив из нужной ветки. В ветке `master` стабильная проверенная версия
* ### Установка для Unity
  - Как git модуль в Unity PackageManager     
    `https://github.com/Felid-Force-Studios/StaticEcs.git`  
    `https://github.com/Felid-Force-Studios/StaticPack.git`
  - Или добавление в манифест `Packages/manifest.json`  
    `"com.felid-force-studios.static-ecs": "https://github.com/Felid-Force-Studios/StaticEcs.git"`  
    `"com.felid-force-studios.static-pack": "https://github.com/Felid-Force-Studios/StaticPack.git"`

# Концепция
> - Основная идея данной реализации в статике, все данные о мире и компонентах находятся в статических классах, что дает возможность избегать дорогостоящих виртуальных вызовов, иметь удобный API со множеством сахара
> - Данный фреймворк нацелен на максимальную простоту использования, скорость и комфорт написания кода без жертв в производительности
> - Доступно создание мульти-миров, строгая типизация, обширные бесплатные абстракции
> - Система сериализации
> - Система отношений сущностей
> - Многопоточная обработка
> - Низкое потребление памяти
> - Основан на Bitmap архитектуре, нет архетипов, нет sparse-set
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
public struct Direction : IComponent { public Vector3 Value; }
public struct Velocity : IComponent { public float Value; }

// Определяем системы
public readonly struct VelocitySystem : IUpdateSystem {
    public void Update() {
        foreach (var entity in W.QueryEntities.For<All<Position, Velocity, Direction>>()) {
            entity.Ref<Position>().Value += entity.Ref<Direction>().Value * entity.Ref<Velocity>().Value;
        }
        
        // Или
        W.Query.For((ref Position pos, ref Velocity vel, ref Direction dir) => {
            pos.Value += dir.Value * vel.Value;
        });
    }
}

public class Program {
    public static void Main() {
        // Создаем данные мира
        W.Create(WorldConfig.Default());
        
        // Регистрируем компоненты
        W.RegisterComponentType<Position>();
        W.RegisterComponentType<Direction>();
        W.RegisterComponentType<Velocity>();
        
        // Инициализируем мир
        W.Initialize();
        
        // Создаем системы
        Systems.Create();
        Systems.AddUpdate(new VelocitySystem());

        // Инициализируем системы
        Systems.Initialize();

        // Создание сущности
        var entity = W.Entity.New(
            new Velocity { Value = 1f },
            new Position { Value = Vector3.Zero },
            new Direction { Value = Vector3.UnitX }
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
