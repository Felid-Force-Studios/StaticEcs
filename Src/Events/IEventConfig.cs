﻿using System;
using FFS.Libraries.StaticPack;

namespace FFS.Libraries.StaticEcs {
    public interface IEventConfig<T, WorldType>
        where T : struct, IEvent
        where WorldType : struct, IWorldType {
        public Guid Id();
        public byte Version();
        public BinaryWriter<T> Writer();
        public BinaryReader<T> Reader();
        public EcsEventMigrationReader<T, WorldType> MigrationReader();
        public IPackArrayStrategy<T> ReadWriteStrategy();
    }

    public class DefaultEventConfig<T, WorldType> : IEventConfig<T, WorldType>
        where T : struct, IEvent
        where WorldType : struct, IWorldType {

        public static readonly DefaultEventConfig<T, WorldType> Default = new();
        public virtual Guid Id() => Guid.Empty;

        public virtual byte Version() => 0;

        public virtual BinaryWriter<T> Writer() => null;

        public virtual BinaryReader<T> Reader() => null;

        public virtual EcsEventMigrationReader<T, WorldType> MigrationReader() => null;

        public virtual IPackArrayStrategy<T> ReadWriteStrategy() => new StructPackArrayStrategy<T>();
    }

    public class ValueEventConfig<T, WorldType> : IEventConfig<T, WorldType>
        where T : struct, IEvent
        where WorldType : struct, IWorldType {
        public Guid IdValue;
        public byte VersionValue;
        public BinaryWriter<T> WriterValue;
        public BinaryReader<T> ReaderValue;
        public EcsEventMigrationReader<T, WorldType> MigrationReaderValue;
        public IPackArrayStrategy<T> ReadWriteStrategyValue;

        public ValueEventConfig() { }

        public ValueEventConfig(
            Guid idValue = default,
            byte version = default,
            BinaryWriter<T> writer = null,
            BinaryReader<T> reader = null,
            EcsEventMigrationReader<T, WorldType> migrationReaderValue = null,
            IPackArrayStrategy<T> readWriteStrategyValue = null
        ) {
            IdValue = idValue;
            VersionValue = version;
            WriterValue = writer;
            ReaderValue = reader;
            MigrationReaderValue = migrationReaderValue;
            ReadWriteStrategyValue = readWriteStrategyValue ?? new StructPackArrayStrategy<T>();
        }

        public ValueEventConfig(IEventConfig<T, WorldType> config) {
            CopyFrom(config);
        }

        public void CopyFrom(IEventConfig<T, WorldType> config) {
            IdValue = config.Id();
            VersionValue = config.Version();
            WriterValue = config.Writer();
            ReaderValue = config.Reader();
            MigrationReaderValue = config.MigrationReader();
            ReadWriteStrategyValue = config.ReadWriteStrategy();
        }
        public Guid Id() => IdValue;

        public byte Version() => VersionValue;

        public BinaryWriter<T> Writer() => WriterValue;

        public BinaryReader<T> Reader() => ReaderValue;

        public EcsEventMigrationReader<T, WorldType> MigrationReader() => MigrationReaderValue;

        public IPackArrayStrategy<T> ReadWriteStrategy() => ReadWriteStrategyValue;
    }
}