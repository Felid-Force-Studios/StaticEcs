<p align="center">
  <img src="docs/fulllogo.png" alt="Static ECS" width="100%">
  <br><br>
  <a href="./README.md"><img src="https://img.shields.io/badge/EN-English-blue?style=flat-square" alt="English"></a>
  <a href="./README_RU.md"><img src="https://img.shields.io/badge/RU-Русский-blue?style=flat-square" alt="Русский"></a>
  <a href="./README_ZH.md"><img src="https://img.shields.io/badge/ZH-中文-blue?style=flat-square" alt="中文"></a>
  <br><br>
  <img src="https://img.shields.io/badge/version-2.0.6-blue?style=for-the-badge" alt="Version">
  <a href="https://www.nuget.org/packages/FFS.StaticEcs/"><img src="https://img.shields.io/badge/NuGet-FFS.StaticEcs-004880?style=for-the-badge&logo=nuget" alt="NuGet"></a>
  <a href="https://felid-force-studios.github.io/StaticEcs/ru/"><img src="https://img.shields.io/badge/Docs-документация-blueviolet?style=for-the-badge" alt="Документация"></a>
  <a href="https://gist.github.com/blackbone/6d254a684cf580441bf58690ad9485c3"><img src="https://img.shields.io/badge/Benchmarks-результаты-green?style=for-the-badge" alt="Benchmarks"></a>
  <a href="https://github.com/Felid-Force-Studios/StaticEcs-Unity"><img src="https://img.shields.io/badge/Unity-модуль-orange?style=for-the-badge&logo=unity" alt="Unity модуль"></a>
  <a href="https://github.com/Felid-Force-Studios/StaticEcs-Showcase"><img src="https://img.shields.io/badge/Showcase-примеры-yellow?style=for-the-badge" alt="Showcase"></a>
  <br><br>
  <a href="https://felid-force-studios.github.io/StaticEcs/ru/migrationguide.html"><img src="https://img.shields.io/badge/Гайд_миграции-2.0.0-red?style=for-the-badge" alt="Гайд миграции"></a>
</p>

<p align="center">
  <a href="./CHANGELOG_2_0_0.md"><img src="https://img.shields.io/badge/🚀_Что_нового_в_2.0.0-Типы_сущностей_·_Трекинг_·_Burst_·_Блочная_итерация_·_Batch-ff6600?style=for-the-badge&labelColor=222222" alt="Что нового в 2.0.0"></a>
</p>

# Static ECS - C# Hierarchical Inverted Bitmap ECS framework
- Производительность
- Легковесность
- Отсутствие аллокаций
- Низкое потребление памяти
- Без Unsafe в ядре
- Основан на статике и структурах
- Типобезопасность
- Бесплатные абстракции
- Мощный механизм запросов с поддержкой параллелизма
- Batch-операции над сущностями
- Отслеживание изменений компонентов и тегов
- Группировка сущностей по типам и кластерам
- Система отношений между сущностями
- Сериализация снимков мира
- Система событий
- Минимум болерплейта
- Совместимость с Unity с поддержкой Il2Cpp и [Burst](https://github.com/Felid-Force-Studios/StaticEcs-Unity?tab=readme-ov-file#templates)
- Совместимость с другими C# движками
- Совместимость с Native AOT

## Оглавление
* [Контакты](#контакты)
* [Установка](#установка)
* [Концепция](#концепция)
* [Быстрый старт](#быстрый-старт)
* [Возможности](https://felid-force-studios.github.io/StaticEcs/ru/features.html)
  * [Сущность](https://felid-force-studios.github.io/StaticEcs/ru/features/entity.html)
  * [Глобальный идентификатор сущности](https://felid-force-studios.github.io/StaticEcs/ru/features/gid.html)
  * [Компонент](https://felid-force-studios.github.io/StaticEcs/ru/features/component.html)
  * [Тег](https://felid-force-studios.github.io/StaticEcs/ru/features/tag.html)
  * [Мульти-компонент](https://felid-force-studios.github.io/StaticEcs/ru/features/multicomponent.html)
  * [Отношения](https://felid-force-studios.github.io/StaticEcs/ru/features/relations.html)
  * [Мир](https://felid-force-studios.github.io/StaticEcs/ru/features/world.html)
  * [Системы](https://felid-force-studios.github.io/StaticEcs/ru/features/systems.html)
  * [Ресурсы](https://felid-force-studios.github.io/StaticEcs/ru/features/resources.html)
  * [Запросы](https://felid-force-studios.github.io/StaticEcs/ru/features/query.html)
  * [События](https://felid-force-studios.github.io/StaticEcs/ru/features/events.html)
  * [Отслеживание изменений](https://felid-force-studios.github.io/StaticEcs/ru/features/tracking.html)
  * [Сериализация](https://felid-force-studios.github.io/StaticEcs/ru/features/serialization.html)
  * [Директивы компилятора](https://felid-force-studios.github.io/StaticEcs/ru/features/compilerdirectives.html)
* [Производительность](https://felid-force-studios.github.io/StaticEcs/ru/performance.html)
* [Unity интеграция](https://felid-force-studios.github.io/StaticEcs/ru/unityintegrations.html)
* [AI Agent Integration](#ai-agent-integration)
* [Лицензия](#лицензия)


# Контакты
* [felid.force.studios@gmail.com](mailto:felid.force.studios@gmail.com)
* [Telegram](https://t.me/felid_force_studios)

# Поддержать проект
Если вам нравится Static ECS и он помогает вашему проекту, вы можете поддержать разработку:

<a href="https://www.buymeacoffee.com/felid.force.studios" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" height="60"></a>

# Установка
Библиотека имеет зависимость на [StaticPack](https://github.com/Felid-Force-Studios/StaticPack) версии `1.1.0` для бинарной сериализации, StaticPack должен быть так же установлен
* ### В виде исходников
  Со страницы релизов или как архив из нужной ветки. В ветке `master` стабильная проверенная версия
* ### Установка для Unity
  Через git модуль в Unity PackageManager:
  ```
  https://github.com/Felid-Force-Studios/StaticEcs.git
  https://github.com/Felid-Force-Studios/StaticPack.git
  ```
  Или добавление в манифест `Packages/manifest.json`:
  ```json
  "com.felid-force-studios.static-ecs": "https://github.com/Felid-Force-Studios/StaticEcs.git"
  "com.felid-force-studios.static-pack": "https://github.com/Felid-Force-Studios/StaticPack.git"
  ```
* ### NuGet
  ```
  dotnet add package FFS.StaticEcs
  ```
  Для debug-сборки с проверками:
  ```
  dotnet add package FFS.StaticEcs.Debug
  ```
  Пакеты: [FFS.StaticEcs](https://www.nuget.org/packages/FFS.StaticEcs/) · [FFS.StaticEcs.Debug](https://www.nuget.org/packages/FFS.StaticEcs.Debug/)

# AI Agent Integration
Если вы используете AI-ассистенты (Claude Code, Cursor, Copilot и др.) со StaticEcs:
- **llms.txt**: Укажите агенту на [`https://felid-force-studios.github.io/StaticEcs/llms.txt`](https://felid-force-studios.github.io/StaticEcs/llms.txt) для краткой AI-справки
- **Полный контекст**: [`https://felid-force-studios.github.io/StaticEcs/llms-full.txt`](https://felid-force-studios.github.io/StaticEcs/llms-full.txt) для полной документации
- **Claude Code**: Скопируйте [сниппет для CLAUDE.md](https://felid-force-studios.github.io/StaticEcs/ru/aiagentguide.html) в `CLAUDE.md` вашего проекта
- **Частые ошибки**: См. [руководство по ошибкам](https://felid-force-studios.github.io/StaticEcs/ru/pitfalls.html)


# Концепция
StaticEcs — новая архитектура ECS, основанная на инвертированной иерархической bitmap модели.
В отличие от традиционных фреймворков ECS, которые полагаются на архетипы или разреженные наборы, в этой конструкции используется инвертированная индексная структура, в которой каждый тип компонента владеет масками активных сущностей, а не сущности хранят маски компонентов.
Иерархическая агрегация этих масок обеспечивает логарифмическое индексирование блоков сущностей, что позволяет осуществлять фильтрацию блоков O(1) и эффективную параллельную итерацию с помощью битовых операций.
Этот подход полностью устраняет миграцию архетипов и sparse set индирекцию, предлагая прямой доступ к памяти в стиле SoA с минимальным количеством промахов кэша.
Модель обеспечивает до 64 раз меньшее количество запросов к памяти на блок и линейно масштабируется с количеством активных наборов компонентов, что делает ее идеальной для крупномасштабных симуляций, открытых миров со стримингом, сетевых игр с синхронизацией состояния, реактивного ИИ с тысячами агентов и проектов с частой сменой состава компонентов (баффы, эффекты, статусы).

В архетипных ECS (Unity DOTS, Flecs, Bevy, Arch) каждое добавление или удаление компонента вызывает миграцию сущности — копирование всех данных в новый архетип, а количество комбинаций компонентов ведёт к взрывному росту архетипов.
В sparse-set ECS (EnTT, DefaultEcs) доступ к компонентам требует косвенной адресации через разреженные таблицы с минимум двумя промахами кэша на каждый lookup.
StaticEcs устраняет обе проблемы: сущность занимает фиксированный слот в сегментированных массивах и никогда не перемещается в памяти — Add/Remove это O(1) переключение бита в маске присутствия без копирования данных. Стабильность адреса сущности обеспечивает дешёвые отношения между сущностями через версионированные идентификаторы (EntityGID), включая связи со стримингом, когда часть связанных сущностей находится в выгруженных зонах — идеально для сложных симуляций в открытых мирах. Количество типов компонентов не влияет на структуру хранения, поскольку каждый тип владеет собственной маской независимо от остальных. Двумерное партиционирование EntityType × Cluster дополнительно обеспечивает кэш-локальность: сущности одного типа внутри кластера располагаются в соседних сегментах памяти, а кластеры позволяют загружать и выгружать целые пространственные зоны без затрагивания остальных данных.

Память организована иерархически: чанки (4096 сущностей) → сегменты (256) → блоки (64). Запрос к миру начинается с AND эвристических масок на уровне чанка — одна битовая операция покрывает до 4096 сущностей, отсекая пустые блоки целиком, — а затем уточняется на уровне 64-сущностного блока. Пакетные операции (BatchAdd, BatchRemove, BatchSetTag) обрабатывают до 64 сущностей одной битовой операцией.


> - Основная идея данной реализации в статике, все данные о мире и компонентах находятся в статических generic-классах (`World<TWorld>`), что дает возможность избегать дорогостоящих виртуальных вызовов и аллокаций, иметь удобный API со множеством сахара. JIT-компилятор устраняет мёртвый код для неиспользуемых хуков компонентов
> - Данный фреймворк нацелен на максимальную простоту использования, скорость и комфорт написания кода без жертв в производительности
> - Доступно создание мульти-миров, строгая типизация, обширные бесплатные абстракции
> - Система бинарной сериализации со снапшотами мира, кластеров и отдельных сущностей, с версионированием схемы и поддержкой сжатия
> - Система отношений сущностей с автоматическими двусторонними хуками для иерархий, групп и связей
> - Реактивное отслеживание изменений для сетевой синхронизации, UI и триггеров
> - Мульти-компоненты — переменное количество данных на сущность (инвентарь, баффы) без heap-аллокаций
> - Многопоточная обработка с параллельными запросами и гарантиями безопасности на уровне блоков
> - Низкое потребление памяти, SoA-layout (Structure of Arrays) — компоненты одного типа в непрерывных массивах
> - Основан на Bitmap архитектуре, нет архетипов, нет sparse-set
> - Фреймворк создан для нужд частного проекта и выложен в open-source.

# Быстрый старт
```csharp
using FFS.Libraries.StaticEcs;

// Определяем тип мира
public struct WT : IWorldType { }

// Определяем тип-алиас для удобного доступа
public abstract class W : World<WT> { }

// Определяем тип систем
public struct GameSystems : ISystemsType { }

// Определяем тип-алиас для систем
public abstract class GameSys : W.Systems<GameSystems> { }

// Определяем компоненты
public struct Position : IComponent { public Vector3 Value; }
public struct Direction : IComponent { public Vector3 Value; }
public struct Velocity : IComponent { public float Value; }

// Определяем систему
public struct VelocitySystem : ISystem {
    public void Update() {
        // Итерация через foreach
        foreach (var entity in W.Query<All<Position, Velocity, Direction>>().Entities()) {
            ref var pos = ref entity.Ref<Position>();
            ref readonly var dir = ref entity.Read<Direction>();
            ref readonly var vel = ref entity.Read<Velocity>();
            pos.Value += dir.Value * vel.Value;
        }

        // Или через делегат (быстрее, без аллокаций)
        W.Query().For(
            static (ref Position pos, in Velocity vel, in Direction dir) => {
                pos.Value += dir.Value * vel.Value;
            }
        );
    }
}

public class Program {
    public static void Main() {
        // Создаём мир
        W.Create();

        // Авторегистрация всех компонентов, тегов, событий и т.д. из текущей сборки
        W.Types().RegisterAll();

        // Инициализируем мир
        W.Initialize();

        // Создаём и настраиваем системы
        GameSys.Create();
        GameSys.Add(new VelocitySystem(), order: 0);
        GameSys.Initialize();

        // Создаём сущность с компонентами
        var entity = W.NewEntity<Default>().Set(
            new Position { Value = Vector3.Zero },
            new Direction { Value = Vector3.UnitX },
            new Velocity { Value = 1f }
        );

        // Обновление всех систем — вызывается каждый кадр
        GameSys.Update();
        // Продвижение трекинга изменений (изменения видны в следующем кадре)
        W.Tick();

        // Уничтожение систем
        GameSys.Destroy();
        // Уничтожение мира и очистка всех данных
        W.Destroy();
    }
}
```

# Лицензия
[MIT license](https://github.com/Felid-Force-Studios/StaticEcs/blob/master/LICENSE.md)
