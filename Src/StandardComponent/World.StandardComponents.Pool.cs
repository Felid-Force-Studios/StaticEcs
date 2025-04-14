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
    public abstract partial class World<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        public struct StandardComponents<T> where T : struct, IStandardComponent {
            public static StandardComponents<T> Value;
            
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<IStandardComponentsDebugEventListener> debugEventListeners;
            #endif
            
            private OnAddHandler<T> _onAddHandler;
            private OnDeleteHandler<T> _onDeleteHandler;
            private OnCopyHandler<T> _onCopyHandler;
            
            private T[] _data;
            internal ushort id;
            private bool _registered;

            #region PUBLIC
            [MethodImpl(AggressiveInlining)]
            public bool IsRegistered() {
                return _registered;
            }

            [MethodImpl(AggressiveInlining)]
            public ref T Ref(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Ref, World not initialized");
                if (!_registered) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, cannot access Entity ID - {id} from deleted entity");
                #endif
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                ref var val = ref _data[entity._id];
                if (debugEventListeners != null) {
                    foreach (var listener in debugEventListeners) {
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
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.StandardComponents<{typeof(T)}> Method: Copy, World not initialized");
                if (!_registered) throw new Exception($"World<{typeof(WorldType)}>.StandardComponents<{typeof(T)}>, Method: Copy, Component type not registered");
                if (!src.IsActual()) throw new Exception($"World<{typeof(WorldType)}>.StandardComponents<{typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
                if (!dst.IsActual()) throw new Exception($"World<{typeof(WorldType)}>.StandardComponents<{typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
                if (MultiThreadActive) throw new Exception($"World<{typeof(WorldType)}>.StandardComponents<{typeof(T)}>, Method: Copy, this operation is not supported in multithreaded mode");
                #endif

                if (_onCopyHandler == null) {
                    Ref(dst) = Ref(src);
                } else {
                    _onCopyHandler(ref Ref(src), ref Ref(dst));
                }
            }
            #endregion

            #region INTERNAL
            
            [MethodImpl(AggressiveInlining)]
            internal ref T RefInternal(Entity entity) {
                return ref _data[entity._id];
            }
            
            [MethodImpl(AggressiveInlining)]
            internal ref T RefInternal(uint entityId) {
                return ref _data[entityId];
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void AutoInit(Entity entity) {
                _onAddHandler(ref _data[entity._id]);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void AutoReset(Entity entity) {
                _onDeleteHandler(ref _data[entity._id]);
            }
            
            internal void Create(ushort componentId) {
                id = componentId;
                _data = new T[EntitiesCapacity()];
                _registered = true;
            }

            [MethodImpl(AggressiveInlining)]
            internal StandardComponentDynId DynamicId() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (Status < WorldStatus.Created) throw new Exception($"World<{typeof(WorldType)}>.StandardComponents<{typeof(T)}>, Method: DynamicId, World not created");
                if (!_registered) throw new Exception($"World<{typeof(WorldType)}>.StandardComponents<{typeof(T)}>, Method: DynamicId, Component type not registered");
                #endif
                return new StandardComponentDynId(id);
            }
                        
            [MethodImpl(AggressiveInlining)]
            internal T[] Data() {
                return _data;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Resize(uint cap) {
                Array.Resize(ref _data, (int) cap);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal string ToStringComponent(Entity entity) {
                return $" - [{id}] {typeof(T).Name} ( {_data[entity._id]} )\n";
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool SetAutoInit(OnAddHandler<T> handler) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (handler == null) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoInit, handler is null");
                if (!_registered) throw new Exception($"World<{typeof(WorldType)}>.StandardComponents<{typeof(T)}>, Method: SetAutoInit, Component type not registered");
                if (Status != WorldStatus.Created) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoInit, world status not `Created`");
                #endif
                var added = _onAddHandler == null;
                _onAddHandler = handler;
                return added;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool SetAutoReset(OnDeleteHandler<T> handler) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (handler == null) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoReset, handler is null");
                if (!_registered) throw new Exception($"World<{typeof(WorldType)}>.StandardComponents<{typeof(T)}>, Method: SetAutoReset, Component type not registered");
                if (Status != WorldStatus.Created) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoReset, world status not `Created`");
                #endif
                var added = _onDeleteHandler == null;
                _onDeleteHandler = handler;
                return added;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool SetAutoCopy(OnCopyHandler<T> handler) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (handler == null) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoCopy, handler is null");
                if (!_registered) throw new Exception($"World<{typeof(WorldType)}>.StandardComponents<{typeof(T)}>, Method: SetAutoCopy, Component type not registered");
                if (Status != WorldStatus.Created) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoCopy, world status not `Created`");
                #endif
                var added = _onCopyHandler == null;
                _onCopyHandler = handler;
                return added;
            }

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                _onDeleteHandler = null;
                _onCopyHandler = null;
                _onAddHandler = null;
                _data = null;
                id = 0;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                debugEventListeners = null;
                #endif
                _registered = false;
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                if (_onDeleteHandler != null) {
                    for (var i = 0; i < EntitiesCount(); i++) {
                        _onDeleteHandler(ref _data[i]);
                    }
                    return;
                } 
                Array.Clear(_data, 0, _data.Length);
            }
            #endregion
        }
    }
}