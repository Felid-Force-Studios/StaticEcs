#if !FFS_ECS_DISABLE_MASKS
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public interface IMask { }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        [MethodImpl(AggressiveInlining)]
        public static void RegisterMaskType<M>(Guid guid = default) where M : struct, IMask {
            if (Status != WorldStatus.Created) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: RegisterMaskType<{typeof(M)}>, World not created");
            }

            ModuleMasks.Value.RegisterMaskType<M>(guid);
        }

        [MethodImpl(AggressiveInlining)]
        public static IMasksWrapper GetMasksPool(Type maskType) {
            return ModuleMasks.Value.GetPool(maskType);
        }

        [MethodImpl(AggressiveInlining)]
        public static MasksWrapper<T> GetMasksPool<T>() where T : struct, IMask {
            return ModuleMasks.Value.GetPool<T>();
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetMasksPool(Type maskType, out IMasksWrapper pool) {
            return ModuleMasks.Value.TryGetPool(maskType, out pool);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetMasksPool<T>(out MasksWrapper<T> pool) where T : struct, IMask {
            return ModuleMasks.Value.TryGetPool(out pool);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
        [MethodImpl(AggressiveInlining)]
        public static void AddMaskDebugEventListener(IMaskDebugEventListener listener) {
            ModuleMasks.Value._debugEventListeners ??= new List<IMaskDebugEventListener>();
            ModuleMasks.Value._debugEventListeners.Add(listener);
        }

        [MethodImpl(AggressiveInlining)]
        public static void RemoveMaskDebugEventListener(IMaskDebugEventListener listener) {
            ModuleMasks.Value._debugEventListeners?.Remove(listener);
        }
        #endif
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        internal partial struct ModuleMasks {
            public static ModuleMasks Value;
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<IMaskDebugEventListener> _debugEventListeners;
            #endif

            internal BitMask BitMask;
            private int[] TempIndexes;

            private IMasksWrapper[] _pools;
            private Dictionary<Type, IMasksWrapper> _poolIdxByType;
            private ushort _poolsCount;

            [MethodImpl(AggressiveInlining)]
            internal void Create(uint baseComponentsCapacity) {
                _pools = new IMasksWrapper[baseComponentsCapacity];
                _poolIdxByType = new Dictionary<Type, IMasksWrapper>();
                BitMask = new BitMask();
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners ??= new List<IMaskDebugEventListener>();
                #endif
                Serializer.Value.Create();
            }

            [MethodImpl(AggressiveInlining)]
            internal void Initialize() {
                BitMask.Create(EntitiesCapacity(), 32, Utils.CalculateMaskLen(_poolsCount), false);
                TempIndexes = new int[_poolsCount];
            }

            [MethodImpl(AggressiveInlining)]
            internal void RegisterMaskType<T>(Guid guid) where T : struct, IMask {
                if (Masks<T>.Value.IsRegistered()) throw new StaticEcsException($"Masks {typeof(T)} already registered");

                if (_poolsCount == _pools.Length) {
                    Array.Resize(ref _pools, _poolsCount << 1);
                }

                _pools[_poolsCount] = new MasksWrapper<T>();
                _poolIdxByType[typeof(T)] = new MasksWrapper<T>();

                Masks<T>.Value.Create(guid, _poolsCount, BitMask);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                Masks<T>.Value.debugEventListeners = _debugEventListeners;
                #endif
                _poolsCount++;
                
                Serializer.Value.RegisterMaskType<T>(guid);
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort MasksCount(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleMasks, Method: MasksCount, World not initialized");
                #endif
                return BitMask.Len(entity._id);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal List<IRawPool> GetAllRawsPools() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetAllRawsPools, World not initialized");
                #endif
                var pools = new List<IRawPool>();
                for (int i = 0; i < _poolsCount; i++) {
                    pools.Add(_pools[i]);
                }
                return pools;
            }

            [MethodImpl(AggressiveInlining)]
            internal IMasksWrapper GetPool(Type maskType) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetPool, World not initialized");
                if (!_poolIdxByType.ContainsKey(maskType)) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetPool, Mask type {maskType} not registered");
                #endif
                return _poolIdxByType[maskType];
            }

            [MethodImpl(AggressiveInlining)]
            internal MasksWrapper<T> GetPool<T>() where T : struct, IMask {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetPool, World not initialized");
                if (!Masks<T>.Value.IsRegistered()) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetPool<{typeof(T)}>, Mask type not registered");
                #endif
                return default;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(Type maskType, out IMasksWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetPool, World not initialized");
                #endif
                return _poolIdxByType.TryGetValue(maskType, out pool);
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool<T>(out MasksWrapper<T> pool) where T : struct, IMask {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetPool, World not initialized");
                #endif
                pool = default;
                return Masks<T>.Value.IsRegistered();
            }

            [MethodImpl(AggressiveInlining)]
            internal void Resize(uint cap) {
                BitMask.ResizeBitMap(cap);
            }

            [MethodImpl(AggressiveInlining)]
            internal void DestroyEntity(Entity entity) {
                BitMask.GetAndDelAllIndexes(entity._id, TempIndexes, out var count);
                for (var i = 0; i < count; i++) {
                    _pools[TempIndexes[i]].DeleteWithoutMask(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void ToPrettyStringEntity(StringBuilder builder, Entity entity) {
                builder.AppendLine("Masks:");
                var bufId = BitMask.BorrowBuf();
                BitMask.CopyToBuffer(entity._id, bufId);
                while (BitMask.GetMinIndexBuffer(bufId, out var id)) {
                    _pools[id].ToStringComponent(builder, entity);
                    BitMask.DelInBuffer(bufId, (ushort) id);
                }
                BitMask.DropBuf();
            }

            [MethodImpl(AggressiveInlining)]
            internal void GetAllMasks(Entity entity, List<IMask> result) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetAllMasks, World not initialized");
                #endif
                var bufId = BitMask.BorrowBuf();
                BitMask.CopyToBuffer(entity._id, bufId);
                while (BitMask.GetMinIndexBuffer(bufId, out var id)) {
                    result.Add(_pools[id].GetRaw());
                    BitMask.DelInBuffer(bufId, (ushort) id);
                }
                BitMask.DropBuf();
            }

            [MethodImpl(AggressiveInlining)]
            internal void CopyEntity(Entity srcEntity, Entity dstEntity) {
                BitMask.Copy(srcEntity._id, dstEntity._id);
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Clear();
                }

                BitMask.Clear();
            }

            internal void Destroy() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Destroy();
                }

                BitMask.Destroy();
                BitMask = default;
                TempIndexes = default;
                _pools = default;
                _poolIdxByType = default;
                _poolsCount = default;
                Serializer.Value.Destroy();
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners = default;
                #endif
            }
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
        public interface IMaskDebugEventListener {
            void OnMaskSet<T>(Entity entity) where T : struct, IMask;
            void OnMaskDelete<T>(Entity entity) where T : struct, IMask;
        }
        #endif
    }
}
#endif