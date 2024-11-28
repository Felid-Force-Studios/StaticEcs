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
    public abstract partial class Ecs<WorldID> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        internal struct ModuleMasks {
            public static ModuleMasks Value;
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<IMaskDebugEventListener> _debugEventListeners;
            #endif

            internal BitMask BitMask;

            private IMasksWrapper[] _pools;
            private Dictionary<Type, int> _poolIdxByType;
            private ushort _poolsCount;

            [MethodImpl(AggressiveInlining)]
            internal void Create(uint baseComponentsCapacity) {
                _pools = new IMasksWrapper[baseComponentsCapacity];
                _poolIdxByType = new Dictionary<Type, int>();
                BitMask = new BitMask();
            }

            [MethodImpl(AggressiveInlining)]
            internal void Initialize() {
                BitMask.Create(World.EntitiesCapacity(), 32, Utils.CalculateMaskLen(_poolsCount));
            }

            [MethodImpl(AggressiveInlining)]
            internal MaskDynId RegisterMask<T>() where T : struct, IMask {
                if (MaskInfo<T>.IsRegistered()) {
                    return Masks<T>.Value.DynamicId();
                }

                if (_poolsCount == _pools.Length) {
                    Array.Resize(ref _pools, _poolsCount << 1);
                }

                _pools[_poolsCount] = new MasksWrapper<T>();
                _poolIdxByType[typeof(T)] = _poolsCount;

                Masks<T>.Value.Create(_poolsCount, BitMask);
                _poolsCount++;

                return Masks<T>.Value.DynamicId();
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort MasksCount(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldID)}>.ModuleMasks, Method: MasksCount, World not initialized");
                #endif
                return BitMask.Len(entity._id);
            }

            [MethodImpl(AggressiveInlining)]
            internal IMasksWrapper GetPool(MaskDynId id) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldID)}>.ModuleMasks, Method: GetPool, World not initialized");
                if (id.Value >= _poolsCount) throw new Exception($"Ecs<{typeof(WorldID)}>.ModuleMasks, Method: GetPool, Mask type for dyn id {id.Value} not registered");
                #endif
                return _pools[id.Value];
            }

            [MethodImpl(AggressiveInlining)]
            internal IMasksWrapper GetPool(Type maskType) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldID)}>.ModuleMasks, Method: GetPool, World not initialized");
                if (!_poolIdxByType.ContainsKey(maskType)) throw new Exception($"Ecs<{typeof(WorldID)}>.ModuleMasks, Method: GetPool, Mask type {maskType} not registered");
                #endif
                return _pools[_poolIdxByType[maskType]];
            }

            [MethodImpl(AggressiveInlining)]
            internal MasksWrapper<T> GetPool<T>() where T : struct, IMask {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldID)}>.ModuleMasks, Method: GetPool, World not initialized");
                if (!MaskInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldID)}>.ModuleMasks, Method: GetPool<{typeof(T)}>, Mask type not registered");
                #endif
                return default;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(MaskDynId id, out IMasksWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldID)}>.ModuleMasks, Method: GetPool, World not initialized");
                #endif
                if (id.Value >= _poolsCount) {
                    pool = default;
                    return false;
                }
                
                pool = _pools[id.Value];
                return true;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(Type maskType, out IMasksWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldID)}>.ModuleMasks, Method: GetPool, World not initialized");
                #endif
                if (!_poolIdxByType.TryGetValue(maskType, out var idx)) {
                    pool = default;
                    return false;
                }
                
                pool = _pools[idx];
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool<T>(out MasksWrapper<T> pool) where T : struct, IMask {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldID)}>.ModuleMasks, Method: GetPool, World not initialized");
                #endif
                pool = default;
                return MaskInfo<T>.IsRegistered();
            }

            [MethodImpl(AggressiveInlining)]
            internal void Resize(int cap) {
                BitMask.ResizeBitMap(cap);
            }

            [MethodImpl(AggressiveInlining)]
            internal void DestroyEntity(Entity entity) {
                var id = BitMask.GetMinIndex(entity._id);
                while (id >= 0) {
                    _pools[id].Del(entity);
                    id = BitMask.GetMinIndex(entity._id);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal string ToPrettyStringEntity(Entity entity) {
                var str = "Masks:\n";
                var bufId = BitMask.BorrowBuf();
                BitMask.CopyToBuffer(entity._id, bufId);
                var id = BitMask.GetMinIndexBuffer(bufId);
                while (id >= 0) {
                    str += _pools[id].ToStringComponent(entity);
                    BitMask.DelInBuffer(bufId, (ushort) id);
                    id = BitMask.GetMinIndexBuffer(bufId);
                }

                BitMask.DropBuf();
                return str;
            }

            [MethodImpl(AggressiveInlining)]
            internal void GetAllMasks(Entity entity, List<IMask> result) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldID)}>.ModuleMasks, Method: GetAllMasks, World not initialized");
                #endif
                var bufId = BitMask.BorrowBuf();
                BitMask.CopyToBuffer(entity._id, bufId);
                var id = BitMask.GetMinIndexBuffer(bufId);
                while (id >= 0) {
                    result.Add(_pools[id].GetRaw());
                    BitMask.DelInBuffer(bufId, (ushort) id);
                    id = BitMask.GetMinIndexBuffer(bufId);
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
                _pools = default;
                _poolIdxByType = default;
                _poolsCount = default;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners = default;
                #endif
            }

            #if ENABLE_IL2CPP
            [Il2CppEagerStaticClassConstruction]
            #endif
            public struct MaskInfo<T> where T : struct, IMask {
                internal static bool Registered;

                [MethodImpl(AggressiveInlining)]
                internal static void Register() => Registered = true;

                [MethodImpl(AggressiveInlining)]
                internal static void Reset() => Registered = false;

                [MethodImpl(AggressiveInlining)]
                public static bool IsRegistered() => Registered;
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