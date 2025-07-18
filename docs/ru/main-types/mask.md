---
title: Маска
parent: Основные типы
nav_order: 7
---

## Mask
Маска - аналогична тегу, но занимает лишь 1 бит памяти
- НЕ Дает возможность строить запросы только по маскам, может использоваться только как дополнительный критерий поиска
- Представлен в виде пользовательской структуры без данных с маркер интерфейсом `IMask`

#### Пример:
```c#
public struct Visible : IMask { }
```
___

{: .importantru }
Требуется регистрация в мире между созданием и инициализацией

```c#
World.Create(WorldConfig.Default());
//...
World.RegisterMaskType<Visible>();
//...
World.Initialize();
```
___

#### Создание:
```c#
// Добавление маски на сущность (методы перегрузки от 1-5 масок)
entity.SetMask<Flammable, Frozen, Visible>();
```
___

#### Основные операции:
```c#
// Получить количество масок на сущности
int masksCount = entity.MasksCount();

// Проверить наличие ВСЕХ масок (методы перегрузки от 1-3 масок)
entity.HasAllOfMasks<Flammable>();
entity.HasAllOfMasks<Flammable, Frozen, Visible>();

// Проверить наличие хотя бы одной маски (методы перегрузки от 2-3 масок)
entity.HasAnyOfMasks<Flammable, Frozen, Visible>();

// Удалить маску у сущности (В DEBUG режиме будет ошибка если сущности нет, 
// в релизе нельзя использовать если нет гарантии что маска присутствует) (методы перегрузки от 1-5 масок)
entity.DeleteMask<Frozen>();

// Удалить маску у сущности если она существует (методы перегрузки от 1-5 масок)
var deleted = entity.TryDeleteMask<Frozen>();

// Если маски нет на сущности то она добавляется, если есть то удаляется (методы перегрузки от 1-3 масок)
entity.ToggleMask<Frozen>();
entity.ToggleMask<Flammable, Frozen, Visible>();

// В зависимости от переданого значения или устанавливается маска (true) или удаляется (false) (методы перегрузки от 1-3 масок)
entity.ApplyMask<Frozen>(true);
entity.ApplyMask<Flammable, Frozen, Visible>(false, true, true);
```