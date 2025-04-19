using System;
using FFS.Libraries.StaticPack;

namespace FFS.Libraries.StaticEcs {
    public interface IStandardComponentConfig<T, WorldType>
        where T : struct
        where WorldType : struct, IWorldType {
        public World<WorldType>.OnComponentHandler<T> OnAdd();
        public World<WorldType>.OnComponentHandler<T> OnDelete();
        public World<WorldType>.OnCopyHandler<T> OnCopy();
        public bool IsCopyable();

        public Guid Id();
        public byte Version();
        public BinaryWriter<T> Writer();
        public BinaryReader<T> Reader();
        public EcsStandardComponentMigrationReader<T, WorldType> MigrationReader();
        public IPackArrayStrategy<T> ReadWriteStrategy();
    }

    public class DefaultStandardComponentConfig<T, WorldType> : IStandardComponentConfig<T, WorldType>
        where T : struct 
        where WorldType : struct, IWorldType {

        public static readonly DefaultStandardComponentConfig<T, WorldType> Default = new();
        
        public virtual World<WorldType>.OnComponentHandler<T> OnAdd() => null;
        public virtual World<WorldType>.OnComponentHandler<T> OnDelete() => null;
        public virtual World<WorldType>.OnCopyHandler<T> OnCopy() => null;
        public virtual bool IsCopyable() => true;
        public virtual Guid Id() => Guid.Empty;

        public virtual byte Version() => 0;

        public virtual BinaryWriter<T> Writer() => null;

        public virtual BinaryReader<T> Reader() => null;

        public virtual EcsStandardComponentMigrationReader<T, WorldType> MigrationReader() => null;

        public virtual IPackArrayStrategy<T> ReadWriteStrategy() => new StructPackArrayStrategy<T>();
    }

    public class ValueStandardComponentConfig<T, WorldType> : IStandardComponentConfig<T, WorldType>
        where T : struct
        where WorldType : struct, IWorldType {
        public World<WorldType>.OnComponentHandler<T> OnAddHandler;
        public World<WorldType>.OnComponentHandler<T> OnDeleteHandler;
        public World<WorldType>.OnCopyHandler<T> OnCopyHandler;
        public bool Copyable;
        public Guid IdValue;
        public byte VersionValue;
        public BinaryWriter<T> WriterValue;
        public BinaryReader<T> ReaderValue;
        public EcsStandardComponentMigrationReader<T, WorldType> MigrationReaderValue;
        public IPackArrayStrategy<T> ReadWriteStrategyValue;

        public ValueStandardComponentConfig() { }

        public ValueStandardComponentConfig(
            World<WorldType>.OnComponentHandler<T> onAddHandler = null,
            World<WorldType>.OnComponentHandler<T> onDeleteHandler = null,
            World<WorldType>.OnCopyHandler<T> onCopyHandler = null,
            bool copyable = true,
            Guid idValue = default,
            byte versionValue = default,
            BinaryWriter<T> writerValue = null,
            BinaryReader<T> readerValue = null,
            EcsStandardComponentMigrationReader<T, WorldType> migrationReaderValue = null,
            IPackArrayStrategy<T> readWriteStrategyValue = null
        ) {
            OnAddHandler = onAddHandler;
            OnDeleteHandler = onDeleteHandler;
            OnCopyHandler = onCopyHandler;
            Copyable = copyable;
            IdValue = idValue;
            VersionValue = versionValue;
            WriterValue = writerValue;
            ReaderValue = readerValue;
            MigrationReaderValue = migrationReaderValue;
            ReadWriteStrategyValue = readWriteStrategyValue ?? new StructPackArrayStrategy<T>();
        }

        public ValueStandardComponentConfig(IStandardComponentConfig<T, WorldType> config) {
            CopyFrom(config);
        }

        public void CopyFrom(IStandardComponentConfig<T, WorldType> config) {
            OnAddHandler = config.OnAdd();
            OnDeleteHandler = config.OnDelete();
            OnCopyHandler = config.OnCopy();
            Copyable = config.IsCopyable();
            IdValue = config.Id();
            VersionValue = config.Version();
            WriterValue = config.Writer();
            ReaderValue = config.Reader();
            MigrationReaderValue = config.MigrationReader();
            ReadWriteStrategyValue = config.ReadWriteStrategy();
        }

        public World<WorldType>.OnComponentHandler<T> OnAdd() => OnAddHandler;
        public World<WorldType>.OnComponentHandler<T> OnDelete() => OnDeleteHandler;
        public World<WorldType>.OnCopyHandler<T> OnCopy() => OnCopyHandler;
        public bool IsCopyable() => Copyable;
        public Guid Id() => IdValue;

        public byte Version() => VersionValue;

        public BinaryWriter<T> Writer() => WriterValue;

        public BinaryReader<T> Reader() => ReaderValue;

        public EcsStandardComponentMigrationReader<T, WorldType> MigrationReader() => MigrationReaderValue;

        public IPackArrayStrategy<T> ReadWriteStrategy() => ReadWriteStrategyValue;
    }
}