#if !FFS_ECS_DISABLE_MASKS
using System;
using System.Collections.Generic;
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
            
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<IMaskDebugEventListener> debugEventListeners;
            #endif
            
            private BitMask _bitMask;
            internal ushort id;
            internal uint count;
            private bool _registered;

            internal void Create(ushort componentId, BitMask bitMask) {
                _bitMask = bitMask;
                id = componentId;
                _registered = true;
            }
                       
            [MethodImpl(AggressiveInlining)]
            public bool IsRegistered() {
                return _registered;
            }

            [MethodImpl(AggressiveInlining)]
            public uint Count() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}> Method: Count, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: Count, Mask type not registered");
                #endif
                return count;
            }

            [MethodImpl(AggressiveInlining)]
            public void Set(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}> Method: Set, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: Set, Mask type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: Set, cannot access Entity ID - {id} from deleted entity");
                #endif
                _bitMask.Set(entity._id, id);
                count++;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnMaskSet<T>(entity);
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public bool Has(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}> Method: Has, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: Has, Mask type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: Has, cannot access Entity ID - {id} from deleted entity");
                #endif
                return _bitMask.Has(entity._id, id);
            }

            [MethodImpl(AggressiveInlining)]
            public void Delete(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}> Method: Delete, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: Delete, Mask type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: Delete, cannot access Entity ID - {id} from deleted entity");
                if (!Has(entity)) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: Delete, cannot access Entity ID - {id} mask not added");
                #endif
                _bitMask.Del(entity._id, id);
                count--;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnMaskDelete<T>(entity);
                }
                #endif
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryDelete(Entity entity) {
                if (Has(entity)) {
                    Delete(entity);
                    return true;
                }

                return false;
            }

            [MethodImpl(AggressiveInlining)]
            public string ToStringComponent(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}> Method: ToStringComponent, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: ToStringComponent, Mask type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: ToStringComponent, cannot access Entity ID - {id} from deleted entity");
                #endif
                return $" - [{id}] {typeof(T).Name}\n";
            }
            

            [MethodImpl(AggressiveInlining)]
            internal ushort DynamicId() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (World.Status < WorldStatus.Created) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}> Method: DynamicId, World not created");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Masks<{typeof(T)}>, Method: DynamicId, Mask type not registered");
                #endif
                return id;
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                count = 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                _registered = false;
                count = 0;
                id = 0;
                _bitMask = null;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                debugEventListeners = null;
                #endif
            }
        }
    }
}
#endif