---
title: Конфигураторы компонентов
parent: Дополнительныe возможности
nav_order: 2
---

### Конфигураторы компонентов
По умолчанию при добавлении или удалении компонента данные заполняются дефолтным значение, а при копировании компонент полностью копируется  
Чтобы установить свою логику инициализации и сброса компонента можно воспользоваться конфигураторами  
Также конфигураторы используются для [сериализации](serialization.md)

___

#### Пример:
```csharp
// Создадим для примера компонент Scale
public struct Scale : IComponent {
    public float X, Y, Z;
    
    // Конфигуратор должен реализовывать интерфейс IComponentConfig
    // Для начала рассмотрим полную имплементацию
    public class Config<WorldType> : IComponentConfig<Scale, WorldType> where WorldType : struct, IWorldType {
        
        // заменяет поведение при создании компонента через метод Add
        public World<WorldType>.OnComponentHandler<Scale> OnAdd() {
            return (World<WorldType>.Entity entity, ref Scale component) => { };
        }

        // заменяет поведение при создании компонента через метод Add с передачей значения
        public World<WorldType>.OnComponentHandler<Scale> OnAddWithValue() {
            return (World<WorldType>.Entity entity, ref Scale component) => { };
        }

        // заменяет поведение при удалении компонента через метод Delete
        public World<WorldType>.OnComponentHandler<Scale> OnDelete() {
            return (World<WorldType>.Entity entity, ref Scale component) => { };
        }

        // заменяет поведение при копировании компонента
        public World<WorldType>.OnCopyHandler<Scale> OnCopy() {
            return (World<WorldType>.Entity entity, World<WorldType>.Entity dstEntity, ref Scale src, ref Scale dst) => { };
        }

        // является ли компонент копируемым, и будет ли скопирован при entity.Copy() или entity.Move()
        public bool IsCopyable() => true;

        // идентификатор компонента для сериализации (подробнее в разделе `Сериализации`)
        public Guid Id() => new("b121594c-456e-4712-9b64-b75dbb37e611");

        // версия компонента для сериализации (подробнее в разделе `Сериализации`)
        public byte Version() => 0;

        // писатель компонента для сериализации (подробнее в разделе `Сериализации`)
        public BinaryWriter<Scale> Writer() {
            return (ref BinaryPackWriter writer, in Scale value) => { };
        }

        // читатель компонента для сериализации (подробнее в разделе `Сериализации`)
        public BinaryReader<Scale> Reader() {
            return (ref BinaryPackReader reader) => { };
        }

        // миграция компонента при изменении для сериализации (подробнее в разделе `Сериализации`)
        public EcsComponentMigrationReader<Scale, WorldType> MigrationReader() {
            return (ref BinaryPackReader reader, World<WorldType>.Entity entity, byte version, bool disabled) => { };
        }

        // стратегия чтения\записи компонента при создании снимка мира для сериализации (подробнее в разделе `Сериализации`)
        public IPackArrayStrategy<Scale> ReadWriteStrategy() => new UnmanagedPackArrayStrategy<Scale>();
    }
    
    // Существует также дефолтный конфигуратор DefaultComponentConfig, позволяющий переопределить только нужные методы
    // Пример
    public class ConfigCompact<WorldType> : DefaultComponentConfig<Scale, WorldType> where WorldType : struct, IWorldType {
        public override World<WorldType>.OnComponentHandler<Scale> OnAdd() {
            return (World<WorldType>.Entity entity, ref Scale component) => { };
        }
    }
}

// Теперь при регистрации компонента возможно передать конфигурацию
W.RegisterComponentType<Scale>(new Scale.Config<WT>());
```

{: .importantru }
> Аналогичный подход используется для:  
> стандартных компонентов (IStandardComponentConfig)  
> мультикомпонентов (IComponentConfig)  
> компонентов-отношений (IComponentConfig)  
> событий (IEventConfig)  
