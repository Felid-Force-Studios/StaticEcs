---
title: Сериализация
parent: Возможности
nav_order: 15
---

### Сериализация
Сериализация - механизм позволяющий делать бинарные снимки мира целиком или конкретных сущностей  
Для бинарной сериализации используется [StaticPack](https://github.com/Felid-Force-Studios/StaticPack)  

### Как это работает:

Определим несколько компонентов:  
> Для поддержки сериализации в [конфигурации компонента](configs.md) 
> необходимо определить как минимум GUID, Writer и Reader.

```csharp
using FFS.Libraries.StaticEcs;
using FFS.Libraries.StaticPack;

// Компонент с ссылочными данными
public struct Name : IComponent {
    public string Value;

    public Name(string value) => Value = value;

    public override string ToString() => Value;

    public class Config<WorldType> : DefaultComponentConfig<Name, WorldType> where WorldType : struct, IWorldType {
        public override Guid Id() => new("531dc870-fdf5-4a8d-a4c6-b4911b1ea1c3");

        public override BinaryWriter<Name> Writer() => (ref BinaryPackWriter writer, in Name value) => writer.WriteString16(value.Value);

        public override BinaryReader<Name> Reader() => (ref BinaryPackReader reader) => new Name(reader.ReadString16());
    }
}

// Компонент с структурами
public struct Position : IComponent {
    public float X, Y, Z;

    public Position(float x, float y, float z) {
        X = x; Y = y; Z = z;
    }

    public override string ToString() => $"X: {X}, Y: {Y}, Z: {Z}";

    public class Config<WorldType> : DefaultComponentConfig<Position, WorldType> where WorldType : struct, IWorldType {
        public override Guid Id() => new("b121594c-456e-4712-9b64-b75dbb37e611");

        public override BinaryWriter<Position> Writer() {
            return (ref BinaryPackWriter w, in Position value) => {
                w.WriteFloat(value.X);
                w.WriteFloat(value.Y);
                w.WriteFloat(value.Z);
            };
        }

        public override BinaryReader<Position> Reader() => (ref BinaryPackReader r) => 
            new Position(r.ReadFloat(), r.ReadFloat(), r.ReadFloat());

        public override IPackArrayStrategy<Position> ReadWriteStrategy() => new UnmanagedPackArrayStrategy<Position>();
    }
}

// Мульти компонент
public struct Items : IMultiComponent<Items, int> {
    public Multi<int> Values;

    public ref Multi<int> RefValue(ref Items component) => ref component.Values;

    public override string ToString() => Values.ToString();

    public class Config<WorldType> : DefaultComponentConfig<Items, WorldType> where WorldType : struct, IWorldType {
        public override Guid Id() => new("c54de753-ff4e-4620-b2ce-6de5c4870db0");

        public override BinaryWriter<Items> Writer() => (ref BinaryPackWriter writer, in Items value) => writer.WriteMulti(value.Values);

        public override BinaryReader<Items> Reader() => (ref BinaryPackReader reader) => new Items {
            Values = reader.ReadMulti<WorldType, int>()
        };
    }
}

// Компонент-отношений
public struct Parent : IEntityLinkComponent<Parent> {
    public EntityGID Link;

    ref EntityGID IRefProvider<Parent, EntityGID>.RefValue(ref Parent component) => ref component.Link;
    
    public override string ToString() => Link.ToString();

    public class Config<WorldType> : DefaultComponentConfig<Parent, WorldType>
        where WorldType : struct, IWorldType {
        public override Guid Id() => new("90a9bb9a-6b86-4041-9a39-2682d5801881");

        public override BinaryWriter<Parent> Writer() => (ref BinaryPackWriter writer, in Parent value) => writer.Write(value.Link);

        public override BinaryReader<Parent> Reader() => (ref BinaryPackReader reader) => new Parent {
            Link = reader.Read<EntityGID>()
        };
    }
}

// Компонент-отношений
public struct Childs: IEntityLinksComponent<Childs> {
    public ROMulti<EntityGID> Links;

    ref ROMulti<EntityGID> IRefProvider<Childs, ROMulti<EntityGID>>.RefValue(ref Childs component) => ref component.Links;
    
    public override string ToString() => Links.ToString();

    public class Config<WorldType> : DefaultComponentConfig<Childs, WorldType>
        where WorldType : struct, IWorldType {
        public override Guid Id() => new("15c875b7-c35f-4e25-a040-e71c8b25103e");

        public override BinaryWriter<Childs> Writer() => (ref BinaryPackWriter writer, in Childs value) => writer.WriteROMulti(value.Links);

        public override BinaryReader<Childs> Reader() => (ref BinaryPackReader reader) => new Childs {
            Links = reader.ReadROMulti<WorldType, EntityGID>()
        };
    }
}

// Какие то теги
public struct Tag1 : ITag {}
public struct Tag2 : ITag {}
public struct Tag3 : ITag {}
```

Определим мир и метод создания:

```csharp
public struct WT : IWorldType { }

public abstract class W : World<WT> { }

public static void InitWorld(GIDStoreSnapshot? snapshot = null) {
    W.Create(WorldConfig.Default());
    W.RegisterComponentType<Name>(new Name.Config<WT>());
    W.RegisterComponentType<Position>(new Position.Config<WT>());
    W.RegisterMultiComponentType<Items, int>(4, new Items.Config<WT>());
    W.RegisterOneToManyRelationType<Parent, Childs>(4, leftConfig: new Parent.Config<WT>(), rightConfig: new Childs.Config<WT>());

    W.RegisterTagType<Tag1>(new("3a6fe6a2-9427-43ae-9b4a-f8582e3a5f90"));
    W.RegisterTagType<Tag2>(new("d25b7a08-cbe6-4c77-bd8e-29ce7f748c30"));
    W.RegisterTagType<Tag3>(new("7f0cbf47-2ac3-4cd0-b5ec-b1f38d08c2aa"));
    
    // Определим отладочный метод
    Utils.EntityGidToString = gid => gid.TryUnpack<WT>(out var e) 
        ? $"{gid.Id()}:{gid.Version()} - {e.Ref<Name>().Value}" 
        : $"GID {gid.Id()} : Version {gid.Version()}";

    if (snapshot.HasValue) {
        W.Initialize(snapshot.Value);
    } else {
        W.Initialize();
    }
}
```

Определим метод создания тестовых сущностей:

```csharp
public static void CreateEntities() {
    var alex = W.Entity.New();
    alex.Add<Name>().Value = "Alex";
    alex.Add<Position>() = new (1.22f, 77.23131f, 54.232f);
    alex.SetTag<Tag1>();
    ref var alexItems = ref alex.Add<Items>().Values;
    alexItems.Add(1);
    alexItems.Add(2);
    alexItems.Add(3);
    alex.Disable();
    
    var jack = W.Entity.New();
    jack.Add<Name>().Value = "Jack";
    jack.Add<Position>() = new (2.57f, 3.23131f, 5.232f);
    jack.SetTag<Tag3>();
    jack.SetLink<Parent>(alex);
    jack.Disable<Position>();
}
```

#### Рассмотрим примеры:

Пример с сохранением и загрузкой всего мира

```csharp
InitWorld();
CreateEntities();
Console.WriteLine("Созданные сущности:");
foreach (var entity in W.AllEntities()) {
    Console.WriteLine(entity.PrettyString);
}
// При сохранении снимка мира, все сущности и события сохраняются
// Сохранение мира в байтовый массив
byte[] snapshot = W.Serializer.CreateWorldSnapshot();
// Или сохранение мира в файл
// W.Serializer.CreateWorldSnapshot("Path/to/save/data/world.bin");
W.Destroy();


InitWorld();
// При загрузке снимка мира, все сущности и события удаляются перед загрузкой, и загружаются из снимка
// Загрузка мира из байтового массива
W.Serializer.LoadWorldSnapshot(snapshot);
// Или загрузка мира из файла
// W.Serializer.LoadWorldSnapshot("Path/to/save/data/world.bin");
Console.WriteLine("Загруженые сущности:");
foreach (var entity in W.AllEntities()) {
    Console.WriteLine(entity.PrettyString);
}
W.Destroy();
```

Пример с сохранением и загрузкой сущностей (как новые)

```csharp
InitWorld();
CreateEntities();
Console.WriteLine("Созданные сущности:");
// Создаем писателя сущностей
// Он записывает необходимую информацию для восстановления сущностей
using var entitiesWriter = W.Serializer.CreateEntitiesSnapshotWriter();
foreach (var entity in W.AllEntities()) {
    // Записываем сущность
    entitiesWriter.Write(entity);
    Console.WriteLine(entity.PrettyString);
}
// Сохранение сущностей в байтовый массив
byte[] snapshot = entitiesWriter.CreateSnapshot();
// Или сохранение сущностей в файл
// entitiesWriter.CreateSnapshot("Path/to/save/data/entities.bin");
W.Destroy();

//   Созданные сущности:                              
//   Entity ID: 1 GID: 1 Version: 1                              
//   Components:         
//    - [0] Name ( Jack )                                        
//    - [1] [Disabled] Position ( X: 2,57, Y: 3,23131, Z: 5,232 )
//    - [3] Parent ( 0:1 - Alex )                                
//   Tags:                                                       
//    - [2] Tag3                                                 
//   
//   Entity ID: 0 GID: 0 Version: 1 ( Disabled )                    
//   Components:       
//    - [0] Name ( Alex )                               
//    - [1] Position ( X: 1,22, Y: 77,23131, Z: 54,232 )
//    - [2] Items ( 1, 2, 3 )                           
//    - [4] Childs ( 1:1 - Jack )                       
//   Tags:                                              
//    - [0] Tag1                                        


InitWorld();
var someEntity1 = W.Entity.New(new Position(1, 2, 3), new Name("someEntity1"));
var someEntity2 = W.Entity.New(new Position(2, 3, 4), new Name("someEntity2"));

// entitiesAsNew указывает нужно ли загрузить сущности как новые и присвоить новый EntityGID (об этом далее)
// если entitiesAsNew = true, это значит что все компоненты-отношения в загруженных сущностях могут иметь неправильные значения
// как этого избежать рассмотрим в следующем примере
// Загрузка сущностей из байтового массива
W.Serializer.LoadEntitiesSnapshot(snapshot, entitiesAsNew: true);
// Или загрузка сущностей из файла
// W.Serializer.LoadEntitiesSnapshot("Path/to/save/data/entities.bin", entitiesAsNew: true);

Console.WriteLine("Загруженые сущности:");
foreach (var entity in W.AllEntities()) {
    Console.WriteLine(entity.PrettyString);
}
W.Destroy();

// Мы можем увидеть что компоненты Parent и Childs в загруженных сущностях указывают на someEntity1 и someEntity2 а не на нужные сущности
// Мы исправим это в следующем примере

//  Загруженые сущности:
//  Entity ID: 3 GID: 3 Version: 1
//  Components:
//   - [0] Name ( Alex )
//   - [1] Position ( X: 1,22, Y: 77,23131, Z: 54,232 )
//   - [2] Items ( 1, 2, 3 )
//   - [4] Childs ( 1:1 - someEntity2 )
//  Tags:
//   - [0] Tag1
//  
//  Entity ID: 2 GID: 2 Version: 1
//  Components:
//   - [1] Name ( Jack )
//   - [1] [Disabled] Position ( X: 2,57, Y: 3,23131, Z: 5,232 )
//   - [3] Parent ( 0:1 - someEntity1 )
//  Tags:
//   - [2] Tag3

//  Созданные сущности:
//  Entity ID: 1 GID: 1 Version: 1
//  Components:
//   - [0] Name ( someEntity2 )
//   - [1] Position ( X: 2, Y: 3, Z: 4 )
//  Tags:
//  
//  Entity ID: 0 GID: 0 Version: 1
//  Components:
//   - [0] Name ( someEntity1 )
//   - [1] Position ( X: 1, Y: 2, Z: 3 )
//  Tags:
```

Пример с сохранением и загрузкой сущностей (с сохранением глобальных идентификаторов)

```csharp
InitWorld();
CreateEntities();
Console.WriteLine("Созданные сущности:");
using var entitiesWriter = W.Serializer.CreateEntitiesSnapshotWriter();
foreach (var entity in W.AllEntities()) {
    entitiesWriter.Write(entity);
    Console.WriteLine(entity.PrettyString);
}
byte[] snapshot = entitiesWriter.CreateSnapshot();
// Сохраняем в отдельный массив\файл хранилище глобальных идентификаторов
// Хранилище глобальных идентификаторов, содержит последовательность и всю информацию о выданных идентификаторах
// что дает возможность не использовать идентификаторы сущностей которые выгруженны в данный момент
byte[] gidSnapshot = W.Serializer.CreateGIDStoreSnapshot();
W.Destroy();

//   Созданные сущности:                              
//   Entity ID: 1 GID: 1 Version: 1                              
//   Components:         
//    - [0] Name ( Jack )                                        
//    - [1] [Disabled] Position ( X: 2,57, Y: 3,23131, Z: 5,232 )
//    - [3] Parent ( 0:1 - Alex )                                
//   Tags:                                                       
//    - [2] Tag3                                                 
//   
//   Entity ID: 0 GID: 0 Version: 1 ( Disabled )                    
//   Components:       
//    - [0] Name ( Alex )                               
//    - [1] Position ( X: 1,22, Y: 77,23131, Z: 54,232 )
//    - [2] Items ( 1, 2, 3 )                           
//    - [4] Childs ( 1:1 - Jack )                       
//   Tags:                                              
//    - [0] Tag1 


// Загружаем хранилище глобальных идентификаторов из файла и инициализируем мир с ним
var gidStoreSnapshot = BinaryPack.ReadFromBytes<GIDStoreSnapshot>(gidSnapshot);
InitWorld(gidStoreSnapshot);
var someEntity1 = W.Entity.New(new Position(1, 2, 3), new Name("someEntity1"));
var someEntity2 = W.Entity.New(new Position(2, 3, 4), new Name("someEntity2"));

// Теперь мы можем указать entitiesAsNew: false потому что идентифиакторы сущностей были восстановлены при инициализации мира
W.Serializer.LoadEntitiesSnapshot(snapshot, entitiesAsNew: false);

Console.WriteLine("Загруженые сущности:");
foreach (var entity in W.AllEntities()) {
    Console.WriteLine(entity.PrettyString);
}
W.Destroy();

// Теперь мы можем увидеть что все связи между сущностями корректны
// Так как при создании someEntity1 и someEntity2 не были использованы идентификаторы загружаемых сущностей
// Такой подход позволяет, загружать и сохранять разные пачки сущностей, например при стриминге мира, или загрузке разных локаций
// и гарантировать что сохраненные идентификаторы в компонентах и событиях не будут перепутаны

//  Загруженые сущности:
//  Entity ID: 3 GID: 0 Version: 1 ( Disabled )
//  Components:
//   - [0] Name ( Alex )
//   - [1] Position ( X: 1,22, Y: 77,23131, Z: 54,232 )
//   - [2] Items ( 1, 2, 3 )
//   - [4] Childs ( 1:1 - Jack )
//  Tags:
//   - [0] Tag1
//  
//  Entity ID: 2 GID: 1 Version: 1
//  Components:
//   - [1] Name ( Jack )
//   - [1] [Disabled] Position ( X: 2,57, Y: 3,23131, Z: 5,232 )
//   - [3] Parent ( 0:1 - Alex )
//  Tags:
//   - [2] Tag3

//  Созданные сущности:
//  Entity ID: 1 GID: 3 Version: 1
//  Components:
//   - [0] Name ( someEntity2 )
//   - [1] Position ( X: 2, Y: 3, Z: 4 )
//  Tags:
//  
//  Entity ID: 0 GID: 2 Version: 1
//  Components:
//   - [0] Name ( someEntity1 )
//   - [1] Position ( X: 1, Y: 2, Z: 3 )
//  Tags:
```

### Вопрос-ответ:

- У меня изменился порядок\набор данных в компоненте, могу ли я загрузить снимок мира\сущностей старой версии?

```csharp
// Чтобы загрузить снимок необходимо написать миграцию компонента старой версии в новую
// Пример:
// Представим что изначально был компонент позиции с X и Y
public struct Position : IComponent {
    public float X, Y;

    public class Config<WorldType> : DefaultComponentConfig<Position, WorldType> where WorldType : struct, IWorldType {
        public override Guid Id() => new("b121594c-456e-4712-9b64-b75dbb37e611");

        public override BinaryWriter<Position> Writer() {
            return (ref BinaryPackWriter w, in Position value) => {
                w.WriteFloat(value.X);
                w.WriteFloat(value.Y);
            };
        }

        public override BinaryReader<Position> Reader() => (ref BinaryPackReader r) => 
            new Position(r.ReadFloat(), r.ReadFloat());
    }
}

// Затем появилась координата Z, и нам требуется поднять версию компонента и написать миграцию
public struct Position : IComponent {
    public float X, Y, Z;

    public class Config<WorldType> : DefaultComponentConfig<Position, WorldType> where WorldType : struct, IWorldType {
        public override Guid Id() => new("b121594c-456e-4712-9b64-b75dbb37e611");

        public override BinaryWriter<Position> Writer() {
            return (ref BinaryPackWriter w, in Position value) => {
                w.WriteFloat(value.X);
                w.WriteFloat(value.Y);
                w.WriteFloat(value.Z); // Актуализируем писатель для Z
            };
        }

        public override BinaryReader<Position> Reader() => (ref BinaryPackReader r) => 
            new Position(r.ReadFloat(), r.ReadFloat(), r.ReadFloat()); // Актуализируем читатель для Z

        // Меняем версию на следующую (по умолчанию версия 0)
        public override byte Version() => 1;

        // Пишем миграцию, где для версии 0 (старой) мы читаем только X и Y а Z устанавливаем в 0
        public override EcsComponentMigrationReader<Position, WorldType> MigrationReader() {
            return (ref BinaryPackReader reader, World<WorldType>.Entity entity, byte version, bool disabled) => {
                if (version == 0) {
                    return new Position(reader.ReadFloat(), reader.ReadFloat(), 0);
                }

                throw new Exception("Unknown version");
            };
        }
    }
}
```

- У меня удалился\добавился компонент, могу ли я загрузить снимок мира\сущностей версии созданный до изменений?

```csharp
// В случае добавления новых компонентов, старый снимок должен корректно загрузиться, восстановление произойдет автоматически
// По умолчанию если компонент был удален то при загрузке он будет пропущен автоматически, ничего дополнительно не требуется

// Если требуется обработать удаление особым образом то требуется зарегистрировать функцию с GUID старого компонента

// Пример для компонентов
W.Serializer.SetComponentDeleteMigrator(
    new("3a6fe6a2-9427-43ae-9b4a-f8582e3a5f90"),
    (ref BinaryPackReader reader, World<WT>.Entity entity, byte version, bool disabled) => {
        // Здесь необходимо корректно прочитать ВСЕ данные и выполнить кастомную логику
    }
);

// Пример для тегов
W.Serializer.SetTagDeleteMigrator(
    new("3a6fe6a2-9427-43ae-9b4a-f8582e3a5f90"),
    (World<WT>.Entity entity) => {
        // Здесь необходимо выполнить кастомную логику
    }
);

// Пример для событий
W.Serializer.SetEventDeleteMigrator(
    new("3a6fe6a2-9427-43ae-9b4a-f8582e3a5f90"),
    (ref BinaryPackReader reader, byte version) => {
        // Здесь необходимо корректно прочитать ВСЕ данные и выполнить кастомную логику
    });
```

- Можно ли исключить компоненты\теги из сериализации?

```csharp
// При использовании сериализации через метод CreateWorldSnapshot сохраняется все состояние мира 
// и нет возможности исключить отдельные компоненты (в DEBUG будет ошибка при вызове что сериализатор не зарегестрирован)

// Но при использовании EntitiesSnapshot такая возможость есть, для этого необходимо не конфигурировать GUID при регистрации компонента\тега\события
// при сохранении сущностей все компоненты\теги\события для которых не определен GUID будут пропущенны при сериализации
// Например чтобы сохранить ВЕСЬ мир включая события и отношения между сущностями, может использоваться следующий код:
using var entitiesWriter = W.Serializer.CreateEntitiesSnapshotWriter();
entitiesWriter.WriteAllEntities();
byte[] snapshot = entitiesWriter.CreateSnapshot();
byte[] gidSnapshot = W.Serializer.CreateGIDStoreSnapshot();
byte[] eventsSnapshot = W.Events.CreateSnapshot();


// Десериализация:
var gidStoreSnapshot = BinaryPack.ReadFromBytes<GIDStoreSnapshot>(gidSnapshot);
InitWorld(gidStoreSnapshot);
W.Serializer.LoadEntitiesSnapshot(snapshot, entitiesAsNew: false);
W.Events.LoadSnapshot(eventsSnapshot);
```

- Можно ли уменьшить размер сериализованных данных\файлов?

```csharp
// Из коробки доступно GZIP сжатие, возможно применять следующим образом
byte[] snapshot = W.Serializer.CreateWorldSnapshot(gzip: true);
W.Serializer.CreateWorldSnapshot("Path/to/save/data/world.bin", gzip: true);

// При загрузке если файл\массив был сжат, также необходимо указать в параметрах
W.Serializer.LoadWorldSnapshot(snapshot, gzip: true);
W.Serializer.LoadWorldSnapshot("Path/to/save/data/world.bin", gzip: true);
```

- Как автоматизировать реагирование на сохранение\загрузку мира\сущностей?

```csharp
// Можно зарегистрировать любое количество колбеков

// Перед созданием снимка
W.Serializer.RegisterPreCreateSnapshotCallback(() => Console.WriteLine("Entities or world `CreateSnapshot` start"));
// После созданием снимка
W.Serializer.RegisterPostCreateSnapshotCallback(() => Console.WriteLine("Entities or world `CreateSnapshot` finish"));

// Перед загрузкой снимка
W.Serializer.RegisterPreLoadSnapshotCallback(() => Console.WriteLine("Entities or world `LoadSnapshot` start"));
// После загрузки снимка
W.Serializer.RegisterPostLoadSnapshotCallback(() => Console.WriteLine("Entities or world `LoadSnapshot` finish"));

// Данные функции будут вызвына при вызове
// W.Serializer.CreateWorldSnapshot(), W.Serializer.LoadWorldSnapshot(snapshot) или entitiesWriter.CreateSnapshot(), W.Serializer.LoadEntitiesSnapshot()

// Как сделать так чтобы функции вызывались при сохранении\загрузке только мира или только сущностей?
// При регистрации колбека вторым параметром можно передать SnapshotActionType, например
W.Serializer.RegisterPreCreateSnapshotCallback(() => Console.WriteLine("Entities `CreateSnapshot` start"), SnapshotActionType.Entities);
W.Serializer.RegisterPostCreateSnapshotCallback(() => Console.WriteLine("World `CreateSnapshot` finish"), SnapshotActionType.World);
```

- Как выполнить post обработку сохраняемых\загружаемых сущностей?

```csharp
// Можно зарегистрировать любое количество колбеков для сущностей

// После созданием снимка
W.Serializer.RegisterPostCreateSnapshotEachEntityCallback(entity => Console.WriteLine($"Saved {entity.PrettyString}"));
// После загрузки снимка
W.Serializer.RegisterPostLoadSnapshotEachEntityCallback(entity => Console.WriteLine($"Loaded {entity.PrettyString}"));

// Данные функции будут вызвына при вызове
// W.Serializer.CreateWorldSnapshot(), W.Serializer.LoadWorldSnapshot(snapshot) или entitiesWriter.CreateSnapshot(), W.Serializer.LoadEntitiesSnapshot()

// Как сделать так чтобы функции вызывались при сохранении\загрузке только мира или только сущностей?
// При регистрации колбека вторым параметром можно передать SnapshotActionType, например
W.Serializer.RegisterPostCreateSnapshotEachEntityCallback(entity => Console.WriteLine($"Saved {entity.PrettyString}"), SnapshotActionType.Entities);
W.Serializer.RegisterPostLoadSnapshotEachEntityCallback(entity => Console.WriteLine($"Loaded {entity.PrettyString}"), SnapshotActionType.World);


// Также при загрузке снимка сущностей можно передать функцию постобработки сущностей в метод
W.Serializer.LoadEntitiesSnapshot(snapshot, entitiesAsNew: true, onLoad: entity => {
    Console.WriteLine($"Loaded {entity.PrettyString}");
});
```

- Как добавить специальные данные в снимок мира\сущностей (например данные из систем или сервисов)?

```csharp
// Можно зарегистрировать любое количество кастомных обработчиков
// Например:
W.Serializer.SetSnapshotHandler(
    new ("57c15483-988a-47e7-919c-51b9a7b957b5"),      // Уникальный гуид типа данных
    version: 0,                                        // Версия
    (ref BinaryPackWriter writer) => {                 // Писатель кастомных данных
        writer.WriteDateTime(DateTime.Now);
        Console.WriteLine("Saved current time");
    },
    (ref BinaryPackReader reader, ushort version) => { // Читатель кастомных данных
        var time = reader.ReadDateTime();
        Console.WriteLine($"Save dateTime is {time}");
    },
    SnapshotActionType.All                            // Тип снимка для которого будет использован данный обработчик
);
```

- Как добавить специальные данные для каждой сущности в снимок мира\сущностей?

```csharp
// Можно зарегистрировать любое количество кастомных обработчиков для сущнсотей
// Например:
W.Serializer.SetSnapshotHandlerEachEntity(
    new ("57c15483-988a-47e7-919c-51b9a7b957b5"),       // Уникальный гуид типа данных
    version: 0,                                         // Версия
    (ref BinaryPackWriter writer, W.Entity entity) => {
        // Write custom entity data
    },
    (ref BinaryPackReader reader, W.Entity entity, ushort version) => {
        // Read custom entity data
    },
    SnapshotActionType.All                             // Тип снимка для которого будет использован данный обработчик
);
```

- Могу ли я сохранить и загрузить данные событий?

```csharp
// Загрзука и сохранение событий выполняется через методы
 W.Events.CreateSnapshot();
 W.Events.LoadSnapshot();
```
