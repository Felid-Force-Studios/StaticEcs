#if !FFS_ECS_DISABLE_MASKS
using System;
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
    public abstract partial class Ecs<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        public struct Masks<T> where T : struct, IMask {
            public static Masks<T> Value;
            private BitMask _bitMask;
            internal ushort id;
            internal int count;

            internal void Create(ushort componentId, BitMask bitMask) {
                _bitMask = bitMask;
                id = componentId;
                ModuleMasks.MaskInfo<T>.Register();
            }

            [MethodImpl(AggressiveInlining)]
            public int Count() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}> Method: Count, World not initialized");
                if (!ModuleMasks.MaskInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: Count, Mask type not registered");
                #endif
                return count;
            }

            [MethodImpl(AggressiveInlining)]
            public void Set(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}> Method: Set, World not initialized");
                if (!ModuleMasks.MaskInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: Set, Mask type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: Set, cannot access Entity ID - {id} from deleted entity");
                #endif
                _bitMask.Set(entity._id, id);
                count++;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (ModuleMasks.Value._debugEventListeners != null) {
                    foreach (var listener in ModuleMasks.Value._debugEventListeners) {
                        listener.OnMaskSet<T>(entity);
                    }
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public bool Has(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}> Method: Has, World not initialized");
                if (!ModuleMasks.MaskInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: Has, Mask type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: Has, cannot access Entity ID - {id} from deleted entity");
                #endif
                return _bitMask.Has(entity._id, id);
            }

            [MethodImpl(AggressiveInlining)]
            public void Delete(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}> Method: Delete, World not initialized");
                if (!ModuleMasks.MaskInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: Delete, Mask type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: Delete, cannot access Entity ID - {id} from deleted entity");
                #endif
                _bitMask.Del(entity._id, id);
                count--;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (ModuleMasks.Value._debugEventListeners != null) {
                    foreach (var listener in ModuleMasks.Value._debugEventListeners) {
                        listener.OnMaskDelete<T>(entity);
                    }
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public string ToStringComponent(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}> Method: ToStringComponent, World not initialized");
                if (!ModuleMasks.MaskInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: ToStringComponent, Mask type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: ToStringComponent, cannot access Entity ID - {id} from deleted entity");
                #endif
                return $" - [{id}] {typeof(T).Name}\n";
            }
            

            [MethodImpl(AggressiveInlining)]
            internal MaskDynId DynamicId() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (World.Status < WorldStatus.Created) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}> Method: DynamicId, World not created");
                if (!ModuleMasks.MaskInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: DynamicId, Mask type not registered");
                #endif
                return new MaskDynId(id);
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                count = 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                ModuleMasks.MaskInfo<T>.Reset();
            }
        }
    }
}
#endif