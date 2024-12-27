﻿#if !FFS_ECS_DISABLE_MASKS
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class Ecs<WorldID> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public struct Masks<T> where T : struct, IMask {
            public static Masks<T> Value;
            private BitMask _bitMask;
            internal ushort id;
            internal int count;

            static Masks() {
                ModuleMasks.RegisterMask<T>();
            }

            internal void Create(ushort componentId, BitMask bitMask) {
                _bitMask = bitMask;
                id = componentId;
            }

            [MethodImpl(AggressiveInlining)]
            public MaskDynId DynamicId() => new(id);

            [MethodImpl(AggressiveInlining)]
            public int Count() => count;

            [MethodImpl(AggressiveInlining)]
            public void Set(Entity entity) {
                _bitMask.Set(entity._id, id);
                count++;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (ModuleMasks._debugEventListeners != null) {
                    foreach (var listener in ModuleMasks._debugEventListeners) {
                        listener.OnMaskSet<T>(entity);
                    }
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public bool Has(Entity entity) {
                return _bitMask.Has(entity._id, id);
            }

            [MethodImpl(AggressiveInlining)]
            public void Delete(Entity entity) {
                _bitMask.Del(entity._id, id);
                count--;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (ModuleMasks._debugEventListeners != null) {
                    foreach (var listener in ModuleMasks._debugEventListeners) {
                        listener.OnMaskDelete<T>(entity);
                    }
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public string ToStringComponent(Entity entity) {
                return $" - [{id}] {typeof(T).Name}\n";
            }

            [MethodImpl(AggressiveInlining)]
            public void Clear() {
                count = 0;
            }
        }
    }
}
#endif