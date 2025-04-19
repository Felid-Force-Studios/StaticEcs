#if !FFS_ECS_DISABLE_TAGS
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public delegate void EcsTagDeleteMigrationReader<WorldType>(World<WorldType>.Entity entity)
        where WorldType : struct, IWorldType;

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public partial struct Tags<T> where T : struct, ITag {
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            [Il2CppEagerStaticClassConstruction]
            #endif
            public struct Serializer {
                internal static Serializer Value;
                
                internal Guid guid;
                
                [MethodImpl(AggressiveInlining)]
                public void Create(Guid value) {
                    guid = value;
                }

                [MethodImpl(AggressiveInlining)]
                public void Destroy() {
                    guid = default;
                }

                [MethodImpl(AggressiveInlining)]
                internal void WriteAll(ref BinaryPackWriter writer, ref Tags<T> pool) {
                    writer.WriteUint(pool._tagCount);
                    writer.WriteInt(pool._entities.Length);
                    writer.WriteInt(pool._dataIdxByEntityId.Length);
                    writer.WriteArrayUnmanaged(pool._entities, 0, (int) pool._tagCount);
                    writer.WriteArrayUnmanaged(pool._dataIdxByEntityId, 0, (int) Entity.entitiesCount);
                }

                [MethodImpl(AggressiveInlining)]
                internal void ReadAll(ref BinaryPackReader reader, ref Tags<T> pool) {
                    pool._tagCount = reader.ReadUint();

                    var dataCapacity = reader.ReadInt();
                    if (pool._entities == null || dataCapacity > pool._entities.Length) {
                        pool._entities = new uint[dataCapacity];
                    }

                    var entitiesCapacity = reader.ReadInt();
                    if (pool._dataIdxByEntityId == null || entitiesCapacity > pool._dataIdxByEntityId.Length) {
                        pool._dataIdxByEntityId = new uint[entitiesCapacity];
                        for (uint i = 0; i < entitiesCapacity; i++) {
                            pool._dataIdxByEntityId[i] = Const.EmptyComponentMask;
                        }
                    }

                    reader.ReadArrayUnmanaged(ref pool._entities);
                    reader.ReadArrayUnmanaged(ref pool._dataIdxByEntityId);
                }
            }
        }
    }
    
    internal static class TagSerializerUtils {
        
        [MethodImpl(AggressiveInlining)]
        internal static void DeleteAllTagMigration<WorldType>(this ref BinaryPackReader reader, EcsTagDeleteMigrationReader<WorldType> migration) 
            where WorldType : struct, IWorldType {
            reader.SkipNext(sizeof(ushort)); // ID
            var componentsCount = reader.ReadUint();

            var dataCapacity = reader.ReadInt();
            var entitiesCapacity = reader.ReadInt();

            var entities = ArrayPool<uint>.Shared.Rent(dataCapacity);
            var dataIdxByEntityId = ArrayPool<uint>.Shared.Rent(entitiesCapacity);

            reader.ReadArrayUnmanaged(ref entities);
            reader.ReadArrayUnmanaged(ref dataIdxByEntityId);
            reader.SkipArrayHeaders();
            for (var i = 0; i < componentsCount; i++) {
                var entity = entities[i];
                migration(new World<WorldType>.Entity(entity));
            }
            
            ArrayPool<uint>.Shared.Return(entities);
            ArrayPool<uint>.Shared.Return(dataIdxByEntityId);
        }
    }
}
#endif