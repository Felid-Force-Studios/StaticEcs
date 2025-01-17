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
        public struct StandardComponents<T> where T : struct, IStandardComponent {
            public static StandardComponents<T> Value;
            private static AutoInitHandler<T> AutoInitHandler;
            private static AutoResetHandler<T> AutoResetHandler;
            private static AutoCopyHandler<T> AutoCopyHandler;
            
            private T[] _data;
            internal ushort id;

            #region PUBLIC
            [MethodImpl(AggressiveInlining)]
            public ref T RefMut(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: RefMut, World not initialized");
                if (!ModuleStandardComponents.StandardComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: RefMut, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: RefMut, cannot access Entity ID - {id} from deleted entity");
                #endif
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                ref var val = ref _data[entity._id];
                if (ModuleStandardComponents.Value._debugEventListeners != null) {
                    foreach (var listener in ModuleStandardComponents.Value._debugEventListeners) {
                        listener.OnComponentRefMut(entity, ref val);
                    }
                }
                return ref val;
                #else
                return ref _data[entity._id];
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public ref readonly T Ref(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Ref, World not initialized");
                if (!ModuleStandardComponents.StandardComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, cannot access Entity ID - {id} from deleted entity");
                #endif
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                ref var val = ref _data[entity._id];
                if (ModuleStandardComponents.Value._debugEventListeners != null) {
                    foreach (var listener in ModuleStandardComponents.Value._debugEventListeners) {
                        listener.OnComponentRef(entity, ref val);
                    }
                }
                return ref val;
                #else
                return ref _data[entity._id];
                #endif
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Copy(Entity src, Entity dst) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Copy, World not initialized");
                if (!ModuleStandardComponents.StandardComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, Component type not registered");
                if (!src.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
                if (!dst.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
                #endif

                if (AutoCopyHandler == null) {
                    RefMut(dst) = RefMut(src);
                } else {
                    AutoCopyHandler(ref RefMut(src), ref RefMut(dst));
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void SetAutoInit(AutoInitHandler<T> handler) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (handler == null) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoInit, handler is null");
                if (!ModuleStandardComponents.StandardComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: SetAutoInit, Component type not registered");
                if (World.Status != WorldStatus.Created) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoInit, world status not `Created`");
                #endif
                if (AutoInitHandler == null) {
                    ModuleStandardComponents.Value.AddAutoInitPool(Value.id);
                }
                AutoInitHandler = handler;
            }

            [MethodImpl(AggressiveInlining)]
            public static void SetAutoReset(AutoResetHandler<T> handler) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (handler == null) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoReset, handler is null");
                if (!ModuleStandardComponents.StandardComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: SetAutoReset, Component type not registered");
                if (World.Status != WorldStatus.Created) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoReset, world status not `Created`");
                #endif
                if (AutoResetHandler == null) {
                    ModuleStandardComponents.Value.AddAutoResetPool(Value.id);
                }
                AutoResetHandler = handler;
            }

            [MethodImpl(AggressiveInlining)]
            public static void SetAutoCopy(AutoCopyHandler<T> handler) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (handler == null) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoCopy, handler is null");
                if (!ModuleStandardComponents.StandardComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: SetAutoCopy, Component type not registered");
                if (World.Status != WorldStatus.Created) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoCopy, world status not `Created`");
                #endif
                AutoCopyHandler = handler;
            }
            #endregion

            #region INTERNAL
            
            [MethodImpl(AggressiveInlining)]
            internal ref T RefMutInternal(Entity entity) {
                return ref _data[entity._id];
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void AutoInit(Entity entity) {
                AutoInitHandler(ref _data[entity._id]);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void AutoReset(Entity entity) {
                AutoResetHandler(ref _data[entity._id]);
            }
            
            internal void Create(ushort componentId) {
                id = componentId;
                _data = new T[World.EntitiesCapacity()];
                ModuleStandardComponents.StandardComponentInfo<T>.Register();
            }

            [MethodImpl(AggressiveInlining)]
            internal StandardComponentDynId DynamicId() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (World.Status < WorldStatus.Created) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: DynamicId, World not created");
                if (!ModuleStandardComponents.StandardComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: DynamicId, Component type not registered");
                #endif
                return new StandardComponentDynId(id);
            }
                        
            [MethodImpl(AggressiveInlining)]
            internal T[] Data() {
                return _data;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Resize(int cap) {
                Array.Resize(ref _data, cap);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal string ToStringComponent(Entity entity) {
                return $" - [{id}] {typeof(T).Name} ( {_data[entity._id]} )\n";
            }

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                AutoResetHandler = null;
                AutoCopyHandler = null;
                AutoInitHandler = null;
                _data = null;
                id = 0;
                ModuleStandardComponents.StandardComponentInfo<T>.Reset();
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                if (AutoResetHandler != null) {
                    for (var i = 0; i < World.EntitiesCount(); i++) {
                        AutoResetHandler(ref _data[i]);
                    }
                    return;
                } 
                Array.Clear(_data, 0, _data.Length);
            }
            #endregion
        }
    }
}