---
title: Component configurators
parent: Features
nav_order: 14
---

### Component configurators
By default, when adding or deleting a component, the data is filled with the default value, and when copying, the component is completely copied  
To set your own initialization and reset logic for the component you can use configurators  
Configurators are also used to [сериализации](serialization.md)

___

#### Example:
```csharp
// Let's create a component for example Scale
public struct Scale : IComponent {
    public float X, Y, Z;
    
    // The configurator must implement the IComponentConfig interface
    // First, let's look at the full implementation
    public class Config<WorldType> : IComponentConfig<Scale, WorldType> where WorldType : struct, IWorldType {
        
        // replaces the behavior when creating a component via the Add method
        public World<WorldType>.OnComponentHandler<Scale> OnAdd() {
            return (World<WorldType>.Entity entity, ref Scale component) => { };
        }

        // replaces the behavior when creating a component via the Put method
        public World<WorldType>.OnComponentHandler<Scale> OnPut() {
            return (World<WorldType>.Entity entity, ref Scale component) => { };
        }

        // replaces the behavior when deleting a component via the Delete method
        public World<WorldType>.OnComponentHandler<Scale> OnDelete() {
            return (World<WorldType>.Entity entity, ref Scale component) => { };
        }

        // replaces the behavior when copying a component
        public World<WorldType>.OnCopyHandler<Scale> OnCopy() {
            return (World<WorldType>.Entity entity, World<WorldType>.Entity dstEntity, ref Scale src, ref Scale dst) => { };
        }

        // whether the component is copyable and will be copied when entity.Copy() or entity.Move() is used
        public bool IsCopyable() => true;

        // whether it is required to set the component in default when deleting it, default is true, false improves the performance of the Delete method
        public bool IsClearable() => true;

        // component identifier for serialization (see `Serializations` section for details)
        public Guid Id() => new("b121594c-456e-4712-9b64-b75dbb37e611");

        // component version for serialization (more details in `Serialization` section)
        public byte Version() => 0;

        // component writer for serialization (more details in `Serialization` section)
        public BinaryWriter<Scale> Writer() {
            return (ref BinaryPackWriter writer, in Scale value) => { };
        }

        // component reader for serialization (more details in `Serializations` section)
        public BinaryReader<Scale> Reader() {
            return (ref BinaryPackReader reader) => { };
        }

        // component migration when changing for serialization (more details in `Serialization` section)
        public EcsComponentMigrationReader<Scale, WorldType> MigrationReader() {
            return (ref BinaryPackReader reader, World<WorldType>.Entity entity, byte version, bool disabled) => { };
        }

        // read/write strategy of a component when creating a snapshot of the world for serialization (see `Serialization` section for details)
        public IPackArrayStrategy<Scale> ReadWriteStrategy() => new UnmanagedPackArrayStrategy<Scale>();
    }
    
    // There is also a default configurator DefaultComponentConfig that allows you to override only the required methods
    // Example
    public class ConfigCompact<WorldType> : DefaultComponentConfig<Scale, WorldType> where WorldType : struct, IWorldType {
        public override World<WorldType>.OnComponentHandler<Scale> OnAdd() {
            return (World<WorldType>.Entity entity, ref Scale component) => { };
        }
    }
}

// Now it is possible to pass the configuration when registering a component
W.RegisterComponentType<Scale>(new Scale.Config<WT>());
```

{: .important }
> A similar approach is used for:  
> multi-components (IComponentConfig)  
> relational components (IComponentConfig)  
> events (IEventConfig)  
