---
title: Глобальный идентификатор сущности
parent: Возможности
nav_order: 2
---

## EntityGID
Глобальный идентификатор сущности — стабильная ссылка на сущность, безопасная для хранения, сериализации и передачи по сети
- Используется для [событий](events.md), [связей между сущностями](relations.md), [сериализации](serialization.md), сетевого взаимодействия
- Содержит Id, Version и ClusterId — позволяет обнаружить устаревшие ссылки через проверку версии
- Назначается автоматически при создании сущности или вручную через `NewEntityByGID`
- Структура размером 8 байт (`StructLayout.Explicit`, поля перекрываются через `Raw`)

___

#### Получение:
```csharp
// Свойство на сущности
EntityGID gid = entity.GID;

// Неявное преобразование Entity → EntityGID
EntityGID gid = entity;

// Через конструктор
EntityGID gid = new EntityGID(id: 0, version: 1, clusterId: 0);
EntityGID gid = new EntityGID(rawValue: 16777216UL);
```

___

#### Свойства:
```csharp
EntityGID gid = entity.GID;

uint id = gid.Id;               // Внутренний индекс слота сущности
ushort version = gid.Version;   // Счётчик поколений (инкрементируется при переиспользовании слота)
ushort clusterId = gid.ClusterId; // Идентификатор кластера
uint chunk = gid.Chunk;         // Индекс чанка (вычисляемый)
ulong raw = gid.Raw;            // Сырое 8-байтное представление (все поля упакованы)
```

___

#### Проверка и распаковка:
```csharp
EntityGID gid = entity.GID;

// Проверить статус GID: Active, NotActual или NotLoaded
GIDStatus status = gid.Status<WT>();

// Безопасная распаковка — вернёт true если сущность загружена и актуальна
if (gid.TryUnpack<WT>(out var entity)) {
    ref var pos = ref entity.Ref<Position>();
}

// С диагностикой причины неудачи
if (!gid.TryUnpack<WT>(out var entity, out GIDStatus status)) {
    // status == GIDStatus.NotActual → сущность не существует или версия/кластер не совпадают (устаревшая ссылка)
    // status == GIDStatus.NotLoaded → сущность существует и версия совпадает, но она выгружена
}

// Небезопасная распаковка — в DEBUG будет ошибка если не загружена или устарела
var entity = gid.Unpack<WT>();
```

___

#### Создание сущности с заданным GID:
```csharp
// Создать сущность в конкретном слоте, определённом GID
// Используется при десериализации и сетевой синхронизации
var entity = W.NewEntityByGID<Default>(gid);

// Не-дженерик вариант (тип сущности известен в runtime как byte)
byte entityTypeId = EntityTypeInfo<Default>.Id;
var entity = W.NewEntityByGID(entityTypeId, gid);
```

___

#### Инвалидация:
```csharp
// Инкрементировать версию без уничтожения сущности
// Все ранее полученные GID станут неактуальными (Status вернёт GIDStatus.NotActual)
entity.UpVersion();
```

___

#### Сравнение:
```csharp
EntityGID a = entity1.GID;
EntityGID b = entity2.GID;

bool eq = a == b;           // Сравнение по Raw (8 байт)
bool eq = a.Equals(b);      // То же самое

// Кросс-тип сравнение с EntityGIDCompact
EntityGIDCompact compact = entity1.GIDCompact;
bool eq = a == compact;     // Сравнение по Id, Version, ClusterId
bool eq = a.Equals(compact);

// Явное сужающее преобразование в EntityGIDCompact
// В DEBUG будет ошибка если Chunk >= 4 или ClusterId >= 4
EntityGIDCompact compact = (EntityGIDCompact)gid;
```

___

## EntityGIDCompact
Компактная версия EntityGID — 4 байта вместо 8, для сценариев с ограничениями по памяти
- Битовая упаковка: `[31..16]` Version, `[15..14]` ClusterId (2 бита), `[13..12]` Chunk (2 бита), `[11..0]` индекс в чанке
- Лимиты: макс 4 чанка (~16 384 сущностей), макс 4 кластера
- В DEBUG будет ошибка при выходе за границы

#### Получение:
```csharp
EntityGIDCompact gid = entity.GIDCompact;

// Явное преобразование Entity → EntityGIDCompact
EntityGIDCompact gid = (EntityGIDCompact)entity;

// Через конструктор
EntityGIDCompact gid = new EntityGIDCompact(id: 0, version: 1, clusterId: 0);
EntityGIDCompact gid = new EntityGIDCompact(raw: 16777216U);
```

___

#### Проверка и распаковка:
```csharp
// API аналогичен EntityGID
GIDStatus status = gid.Status<WT>();

if (gid.TryUnpack<WT>(out var entity)) {
    // ...
}

var entity = gid.Unpack<WT>();

// Неявное расширяющее преобразование в EntityGID (всегда безопасно)
EntityGID full = gid;
```

___

## Примеры использования

#### События:
```csharp
public struct OnDamage : IEvent {
    public EntityGID Target;
    public float Amount;
}

// В системе:
foreach (var e in damageReceiver) {
    ref var data = ref e.Value;
    if (data.Target.TryUnpack<WT>(out var target)) {
        ref var health = ref target.Ref<Health>();
        health.Current -= data.Amount;
    }
}
```

#### Сетевое взаимодействие сервер-клиент:
GID можно использовать как идентификатор связи сущности между клиентом и сервером.
Сервер создаёт сущность, передаёт GID клиенту, клиент создаёт сущность с тем же GID —
дальнейшие команды с GID позволяют клиенту легко найти нужную сущность через `TryUnpack`.

```csharp
public struct CreateEntityCommand {
    public EntityGID Id;
    public string Prefab;
}

// Сервер:
var serverEntity = W.NewEntity<Default>();
client.Send(new CreateEntityCommand { Id = serverEntity.GID, Prefab = "player" });

// Клиент:
var cmd = server.Receive<CreateEntityCommand>();
var clientEntity = ClientW.NewEntityByGID<Default>(cmd.Id);
```
