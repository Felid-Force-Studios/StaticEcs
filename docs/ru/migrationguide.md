---
title: Обновление версий
parent: RU
nav_order: 5
---

#### Обновление версии с 1.0.x на 1.1.x

В версии 1.1.0 следующие ломающие изменения:
 - Переименован метод запросов из `QueryComponents` в `Query`
 - Удалены Компоненты-маски `IMask`, так как они больше не имеют смысла и требуется при поднятии версии заменить их все на Компоненты-теги `ITag`
 - Удалены Стандартные компоненты `IStandardComponent`, теперь они не имеют смысла и требуется при поднятии версии заменить их все на обычные Компоненты `IComponent`
   - Если требуется поведение стандартных компонентов (Чтобы компонент присутствовал на всех без исключения сущностях) можно использовать `OnEntityCreate` [раздел сущности](features/entity.md)
 - Тип `RoEntity` удален, многопоточные запросы теперь возвращают `Entity`, а не `RoEntity`
 - Тип `WithAdds` удален, необходимо использовать стандартный `With`
 - Метод `TryDeleteTag` удален, теперь операции `SetTag` и `DeleteTag` безопасные и возвращают `bool`, подробнее в [разделе тегов](features/tag.md)
 - Entity теперь не реализует интерфейс `IEntity`, если нужен интерфейс используйте `entity.Box()`, подробнее в [раздел сущности](features/entity.md)
 - `Query` при передаче пользовательского значения параметром без `ref` все равно ожидает модификатор `ref` внутри функции (W.Query.For(Time.deltaTime, (ref float dt, ent) => ...))
- Удалены `WithLink`, `WithLinksAll`, `WithLinksAny` методы запросов к отношениям

#### Обновление версии с 1.1.x на 1.2.x

В версии 1.2.0 следующие ломающие изменения:
 - Переименован метод запросов из `QueryEntities.For()` -> `Query.Entities()`
 - Переименован методы сущности для работы с сырыми типами:
   - `entity.HasAllOf(Type componentType)` -> `entity.RawHasAllOf(Type componentType)`
   - `entity.Add(Type componentType)` -> `entity.RawAdd(Type componentType)`
   - `entity.TryAdd(Type componentType)` -> `entity.RawTryAdd(Type componentType)`
   - `entity.TryAdd(Type componentType, out bool added)` -> `entity.RawTryAdd(Type componentType, out bool added)`
   - `entity.GetRaw(Type componentType)` -> `entity.RawGet(Type componentType)`
   - `entity.PutRaw(IComponent component)` -> `entity.RawPut(IComponent component)`
   - `entity.TryDelete(Type componentType)` -> `entity.RawTryDelete(Type componentType)`
   - `entity.Delete(Type componentType)` -> `entity.RawDelete(Type componentType)`
   - `entity.CopyComponentsTo(Type componentType, Entity target)` -> `entity.RawCopyComponentsTo(Type componentType, Entity target)`
   - `entity.MoveComponentsTo(Type componentType, Entity target)` -> `entity.RawMoveComponentsTo(Type componentType, Entity target)`
   - `entity.HasAllOfTags(Type tagType)` -> `entity.RawHasAllOfTags(Type tagType)`
   - `entity.SetTag(Type tagType)` -> `entity.RawSetTag(Type tagType)`
   - `entity.DeleteTag(Type tagType)` -> `entity.RawDeleteTag(Type tagType)`
   - `entity.MoveTagsTo(Type tagType, Entity target)` -> `entity.RawMoveTagsTo(Type tagType, Entity target)`
 - Переименован методы интерфейса `IQueryFunction`:
   - `Run(...)` -> `Invoke(...)`
 - `EntityGID`:
   - Изменился размер 4 байт -> 8 байт
   - Метод `Id()` заменен на поле `Id`
   - Метод `Version()` заменен на поле `Version`
   - Метод `Raw()` заменен на поле `Raw`
   - Метод `IsRegistered()` заменен на метод `IsActual()`
   - Версия сущности: изменился тип `byte` -> `ushort`
 - Удален тип `GIDStore`
 - Удален метод `World.Clear()`
 - Удалена настройка конфигурации мира `WorldConfig.baseEntitiesCapacity` -> данный параметр принимает метод `World.Initialize(baseEntitiesCapacity)`
 - `Entity`:
   - `default(W.Entity)` никогда не может быть актуальной сущностью
   - Метод `entity.IsActual()` переименован в `entity.IsNotDestroyed()`
