#if !FFS_ECS_DISABLE_MASKS
using System.Runtime.CompilerServices;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        internal partial struct ModuleMasks {
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct MaskCache<M> where M : struct, IComponentMasks {
                internal static MaskCache<M> Cache;
            
                internal uint BufId;
                private ushort Version;
                internal byte Count;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void This(out uint bufId, out ushort count) {
                    if (Version != runtimeVersion) {
                        SetMask();
                    }

                    count = Count;
                    bufId = BufId;
                }
                
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void This(M types, out uint bufId, out ushort count) {
                    if (Version != runtimeVersion) {
                        SetMask(types);
                    }

                    count = Count;
                    bufId = BufId;
                }

                private void SetMask(M types = default) {
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (Status != WorldStatus.Initialized) throw new StaticEcsException($"World<{typeof(WorldType)}>>, World not initialized");
                    #endif
                    var buf = Value.BitMask.BorrowBuf();
                    types.SetBitMask<WorldType>(buf);
                    var buffer = Value.BitMask.AddIndexedBuffer(buf);
                    Value.BitMask.DropBuf();
                    BufId = buffer.index;
                    Count = buffer.count;
                    Version = runtimeVersion;
                }
            }
        }
    }
}
#endif