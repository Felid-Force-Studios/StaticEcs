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
    public abstract partial class World<WorldType> {
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
            private int[] TempIndexes;

            private IMasksWrapper[] _pools;
            private Dictionary<Type, int> _poolIdxByType;
            private ushort _poolsCount;

            [MethodImpl(AggressiveInlining)]
            internal void Create(uint baseComponentsCapacity) {
                _pools = new IMasksWrapper[baseComponentsCapacity];
                _poolIdxByType = new Dictionary<Type, int>();
                BitMask = new BitMask();
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners ??= new List<IMaskDebugEventListener>();
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal void Initialize() {
                BitMask.Create(EntitiesCapacity(), 32, Utils.CalculateMaskLen(_poolsCount), false);
                TempIndexes = new int[_poolsCount];
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort RegisterMaskType<T>() where T : struct, IMask {
                if (Masks<T>.Value.IsRegistered()) {
                    return Masks<T>.Value.DynamicId();
                }

                if (_poolsCount == _pools.Length) {
                    Array.Resize(ref _pools, _poolsCount << 1);
                }

                _pools[_poolsCount] = new MasksWrapper<T>();
                _poolIdxByType[typeof(T)] = _poolsCount;

                Masks<T>.Value.Create(_poolsCount, BitMask);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                Masks<T>.Value.debugEventListeners = _debugEventListeners;
                #endif
                _poolsCount++;

                return Masks<T>.Value.DynamicId();
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort MasksCount(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.ModuleMasks, Method: MasksCount, World not initialized");
                #endif
                return BitMask.Len(entity._id);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal List<IRawPool> GetAllRawsPools() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetAllRawsPools, World not initialized");
                #endif
                var pools = new List<IRawPool>();
                for (int i = 0; i < _poolsCount; i++) {
                    pools.Add(_pools[i]);
                }
                return pools;
            }

            [MethodImpl(AggressiveInlining)]
            internal IMasksWrapper GetPool(ushort id) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetPool, World not initialized");
                if (id >= _poolsCount) throw new Exception($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetPool, Mask type for dyn id {id} not registered");
                #endif
                return _pools[id];
            }

            [MethodImpl(AggressiveInlining)]
            internal IMasksWrapper GetPool(Type maskType) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetPool, World not initialized");
                if (!_poolIdxByType.ContainsKey(maskType)) throw new Exception($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetPool, Mask type {maskType} not registered");
                #endif
                return _pools[_poolIdxByType[maskType]];
            }

            [MethodImpl(AggressiveInlining)]
            internal MasksWrapper<T> GetPool<T>() where T : struct, IMask {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetPool, World not initialized");
                if (!Masks<T>.Value.IsRegistered()) throw new Exception($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetPool<{typeof(T)}>, Mask type not registered");
                #endif
                return default;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(ushort id, out IMasksWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetPool, World not initialized");
                #endif
                if (id >= _poolsCount) {
                    pool = default;
                    return false;
                }
                
                pool = _pools[id];
                return true;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(Type maskType, out IMasksWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetPool, World not initialized");
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
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetPool, World not initialized");
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
            internal string ToPrettyStringEntity(Entity entity) {
                var str = "Masks:\n";
                var bufId = BitMask.BorrowBuf();
                BitMask.CopyToBuffer(entity._id, bufId);
                while (BitMask.GetMinIndexBuffer(bufId, out var id)) {
                    str += _pools[id].ToStringComponent(entity);
                    BitMask.DelInBuffer(bufId, (ushort) id);
                }
                BitMask.DropBuf();
                return str;
            }

            [MethodImpl(AggressiveInlining)]
            internal void GetAllMasks(Entity entity, List<IMask> result) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.ModuleMasks, Method: GetAllMasks, World not initialized");
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
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners = default;
                #endif
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct MaskCache<M> where M : struct, IComponentMasks {
                internal static MaskCache<M> Cache;
            
                internal uint BufId;
                private ushort Version;
                internal byte Count;

                [MethodImpl(AggressiveInlining)]
                public void This(out uint bufId, out ushort count) {
                    if (Version != runtimeVersion) {
                        SetMask();
                    }

                    count = Count;
                    bufId = BufId;
                }
                
                [MethodImpl(AggressiveInlining)]
                public void This(M types, out uint bufId, out ushort count) {
                    if (Version != runtimeVersion) {
                        SetMask(types);
                    }

                    count = Count;
                    bufId = BufId;
                }

                private void SetMask(M types = default) {
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (Status != WorldStatus.Initialized) throw new Exception($"World<{typeof(WorldType)}>>, World not initialized");
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

        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
        public interface IMaskDebugEventListener {
            void OnMaskSet<T>(Entity entity) where T : struct, IMask;
            void OnMaskDelete<T>(Entity entity) where T : struct, IMask;
        }
        #endif
    }
}
#endif