using System;
using FFS.Libraries.StaticPack;

namespace FFS.Libraries.StaticEcs {
    public interface IComponentConfig<T, WorldType>
        where T : struct
        where WorldType : struct, IWorldType {
        public World<WorldType>.OnComponentHandler<T> OnAdd();
        public World<WorldType>.OnComponentHandler<T> OnPut();
        public World<WorldType>.OnComponentHandler<T> OnDelete();
        public World<WorldType>.OnCopyHandler<T> OnCopy();
        public bool IsCopyable();
        public bool IsClearable();
        public Guid Id();
        public byte Version();
        public BinaryWriter<T> Writer();
        public BinaryReader<T> Reader();
        public EcsComponentMigrationReader<T, WorldType> MigrationReader();
        public IPackArrayStrategy<T> ReadWriteStrategy();
    }

    public class DefaultComponentConfig<T, WorldType> : IComponentConfig<T, WorldType>
        where T : struct 
        where WorldType : struct, IWorldType {

        public static readonly DefaultComponentConfig<T, WorldType> Default = new();
        
        public virtual World<WorldType>.OnComponentHandler<T> OnAdd() => null;
        public virtual World<WorldType>.OnComponentHandler<T> OnPut() => null;
        public virtual World<WorldType>.OnComponentHandler<T> OnDelete() => null;
        public virtual World<WorldType>.OnCopyHandler<T> OnCopy() => null;
        public virtual bool IsCopyable() => true;
        public virtual bool IsClearable() => true;
        public virtual Guid Id() => Guid.Empty;
        public virtual byte Version() => 0;
        public virtual BinaryWriter<T> Writer() => null;
        public virtual BinaryReader<T> Reader() => null;
        public virtual EcsComponentMigrationReader<T, WorldType> MigrationReader() => null;
        public virtual IPackArrayStrategy<T> ReadWriteStrategy() => new StructPackArrayStrategy<T>();
    }

    public class ValueComponentConfig<T, WorldType> : IComponentConfig<T, WorldType>
        where T : struct
        where WorldType : struct, IWorldType {
        public World<WorldType>.OnComponentHandler<T> OnAddHandler;
        public World<WorldType>.OnComponentHandler<T> OnPutHandler;
        public World<WorldType>.OnComponentHandler<T> OnDeleteHandler;
        public World<WorldType>.OnCopyHandler<T> OnCopyHandler;
        public bool Copyable;
        public bool Clearable;
        public Guid IdValue;
        public byte VersionValue;
        public BinaryWriter<T> WriterValue;
        public BinaryReader<T> ReaderValue;
        public EcsComponentMigrationReader<T, WorldType> MigrationReaderValue;
        public IPackArrayStrategy<T> ReadWriteStrategyValue;

        public ValueComponentConfig() { }

        public ValueComponentConfig(
            World<WorldType>.OnComponentHandler<T> onAdd = null,
            World<WorldType>.OnComponentHandler<T> onPut = null,
            World<WorldType>.OnComponentHandler<T> onDelete = null,
            World<WorldType>.OnCopyHandler<T> onCopy = null,
            bool copyable = true,
            bool clearable = true,
            Guid idValue = default,
            byte version = default,
            BinaryWriter<T> writer = null,
            BinaryReader<T> reader = null,
            EcsComponentMigrationReader<T, WorldType> migrationReaderValue = null,
            IPackArrayStrategy<T> readWriteStrategyValue = null
        ) {
            OnAddHandler = onAdd;
            OnPutHandler = onPut;
            OnDeleteHandler = onDelete;
            OnCopyHandler = onCopy;
            Copyable = copyable;
            Clearable = clearable;
            IdValue = idValue;
            VersionValue = version;
            WriterValue = writer;
            ReaderValue = reader;
            MigrationReaderValue = migrationReaderValue;
            ReadWriteStrategyValue = readWriteStrategyValue ?? new StructPackArrayStrategy<T>();
        }

        public ValueComponentConfig(IComponentConfig<T, WorldType> config) {
            CopyFrom(config);
        }

        public void CopyFrom(IComponentConfig<T, WorldType> config) {
            OnAddHandler = config.OnAdd();
            OnPutHandler = config.OnAdd();
            OnDeleteHandler = config.OnDelete();
            OnCopyHandler = config.OnCopy();
            Copyable = config.IsCopyable();
            Clearable = config.IsClearable();
            IdValue = config.Id();
            VersionValue = config.Version();
            WriterValue = config.Writer();
            ReaderValue = config.Reader();
            MigrationReaderValue = config.MigrationReader();
            ReadWriteStrategyValue = config.ReadWriteStrategy();
        }

        public World<WorldType>.OnComponentHandler<T> OnAdd() => OnAddHandler;
        public World<WorldType>.OnComponentHandler<T> OnPut() => OnPutHandler;
        public World<WorldType>.OnComponentHandler<T> OnDelete() => OnDeleteHandler;
        public World<WorldType>.OnCopyHandler<T> OnCopy() => OnCopyHandler;
        public bool IsCopyable() => Copyable;
        public bool IsClearable() => Clearable;
        public Guid Id() => IdValue;
        public byte Version() => VersionValue;
        public BinaryWriter<T> Writer() => WriterValue;
        public BinaryReader<T> Reader() => ReaderValue;
        public EcsComponentMigrationReader<T, WorldType> MigrationReader() => MigrationReaderValue;
        public IPackArrayStrategy<T> ReadWriteStrategy() => ReadWriteStrategyValue;
    }
}