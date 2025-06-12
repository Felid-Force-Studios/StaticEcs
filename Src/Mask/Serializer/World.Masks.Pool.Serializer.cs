#if !FFS_ECS_DISABLE_MASKS
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public delegate void EcsMaskDeleteMigrationReader<WorldType>(World<WorldType>.Entity entity)
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
        public partial struct Masks<T> where T : struct, IMask {
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
                internal void WriteAll(ref BinaryPackWriter writer, ref Masks<T> pool) {
                    writer.WriteUint(pool.count);
                }

                [MethodImpl(AggressiveInlining)]
                internal void ReadAll(ref BinaryPackReader reader, ref Masks<T> pool) {
                    pool.count = reader.ReadUint();
                }
            }
        }
    }
}
#endif