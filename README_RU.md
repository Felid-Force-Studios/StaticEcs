![Version](https://img.shields.io/badge/version-1.0.0-blue.svg?style=for-the-badge)

### ЯЗЫК
[RU](./README_RU.md)
[EN](./README.md)
___
### 🚀 **[Benchmarks](./docs/Benchmark.md)** 🚀
### ⚙️ **[Unity модуль](https://github.com/Felid-Force-Studios/StaticEcs-Unity)** ⚙️
### 📖️ **[Документация](https://felid-force-studios.github.io/StaticEcs/ru/)** 📖️
 
# Static ECS - C# Entity component system framework
- Легковесность
- Производительность
- Отсутсвие аллокаций
- Без Unsafe в ядре
- Основан на статике и структурах
- Типобезопасность
- Бесплатные абстракции
- Мощный механизм запросов
- Минимум болерплейта
- Совместимость с Unity и другими C# движками
### Ограничения и особенности:
> - Не потокобезопасен

## Оглавление
* [Контакты](#контакты)
* [Установка](#установка)
* [Концепция](#концепция)
* [Быстрый старт](#быстрый-старт)
* [Лицензия](#лицензия)


# Контакты
* [Telegram](https://t.me/felid_force_studios)

# Установка
* ### В виде исходников
  Со страницы релизов или как архив из нужной ветки. В ветке `master` стабильная проверенная версия
* ### Установка для Unity
  Как git модуль `https://github.com/Felid-Force-Studios/StaticEcs.git` в Unity PackageManager  
  или добавление в манифест `Packages/manifest.json` `"com.felid-force-studios.static-ecs": "https://github.com/Felid-Force-Studios/StaticEcs.git"`

# Концепция
> - Основная идея данной реализации в статике, все данные о мире и компонентах находятся в статических классах, что дает вохможность избегать дорогостоящих виртуальных вызовов, иметь удобный API со множеством сахара
> - Даннный фреймворк нацелен на максмальную простоту использования, скорость и комфорт написания кода без жертв в производительности
> - Доступно создание мульти-миров, строгая типизация, обширные бесплатные абстракции
> - Система сериализации
> - Система отношений сущностей
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
[MIT license](./LICENSE.md)
