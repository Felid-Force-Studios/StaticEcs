#if !FFS_ECS_DISABLE_TAGS
using System;
using System.Runtime.CompilerServices;
using System.Threading;
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
        public abstract partial class ModuleTags {
            private static ulong[] _bitMap;
            internal static BitMask BitMask;
            private static ITagsWrapper[] _pools;
            private static Action[] _resetActions;
            private static Action[] _reInitActions;
            private static ushort _actionsCount;
            private static ushort _poolsCount;

            internal static ushort BitMapLen;

            [MethodImpl(AggressiveInlining)]
            internal static void Create(uint baseComponentsCapacity) {
                _pools = new ITagsWrapper[baseComponentsCapacity];
                _reInitActions ??= new Action[baseComponentsCapacity];
                _resetActions ??= new Action[baseComponentsCapacity];
            }

            [MethodImpl(AggressiveInlining)]
            internal static void Initialize() {
                var count = _actionsCount;
                _actionsCount = 0;
                for (var i = 0; i < count; i++) {
                    _reInitActions[i]();
                }

                BitMapLen = Utils.CalculateMaskLen(_poolsCount);
                _bitMap = new ulong[World.EntitiesCapacity() * BitMapLen];
                BitMask = new BitMask();
                BitMask.Create(_bitMap, 32, BitMapLen);
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].SetBitMask(BitMask);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal static void ReInitialize() {
                var newEntityComponentBitMapLen = Utils.CalculateMaskLen(_poolsCount);

                Array.Resize(ref _bitMap, World.EntitiesCapacity() * newEntityComponentBitMapLen);
                for (var i = World._entityVersionsCount - 1; i > 0; i--) {
                    for (var j = BitMapLen - 1; j >= 0; j--) {
                        var idx = i * BitMapLen + j;
                        ref var bits = ref _bitMap[idx];
                        _bitMap[idx + i] = bits;
                        bits = 0UL;
                    }
                }

                BitMapLen = newEntityComponentBitMapLen;
                BitMask.SetNewBitMap(_bitMap);
                BitMask.ResizeBuffer(BitMapLen);
            }

            internal static void SetBasePoolCapacity<T>(uint capacity) where T : struct, ITag {
                TagInfo<T>.BaseCapacity = capacity;
            }

            internal static void SetBasePoolCapacity(uint capacity) {
                TagInfo.BaseCapacity = capacity;
            }

            [MethodImpl(AggressiveInlining)]
            internal static TagDynId RegisterTag<C>() where C : struct, ITag {
                #if DEBUG
                if (World.Status == WorldStatus.NotCreated) throw new Exception($"World<{typeof(WorldID)}>, Method: RegisterTag, World not created");
                #endif
                if (TagInfo<C>.Has()) {
                    return GetDynamicId<C>();
                }

                TagInfo<C>.Register();
                Tags<C>.Value.Create(TagInfo<C>.Id, BitMask, TagInfo<C>.BaseCapacity ?? TagInfo.BaseCapacity);
                if (_poolsCount == _pools.Length) {
                    Array.Resize(ref _pools, _poolsCount << 1);
                }

                _pools[_poolsCount++] = new TagsWrapper<C>();

                if (_actionsCount == _reInitActions.Length) {
                    Array.Resize(ref _reInitActions, _actionsCount << 1);
                    Array.Resize(ref _resetActions, _actionsCount << 1);
                }

                _reInitActions[TagInfo<C>.Id] = static () => RegisterTag<C>();
                _resetActions[TagInfo<C>.Id] = static () => TagInfo<C>.Reset();
                _actionsCount++;

                if (World.IsInitialized() && _poolsCount % 64 == 0) {
                    ReInitialize();
                }

                return GetDynamicId<C>();
            }

            [MethodImpl(AggressiveInlining)]
            public static ITagsWrapper GetPool(TagDynId id) {
                #if DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: GetTagPool, World not initialized");
                #endif
                return _pools[id.Val];
            }

            [MethodImpl(AggressiveInlining)]
            public static TagsWrapper<T> GetPool<T>() where T : struct, ITag {
                #if DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: GetTagPool, World not initialized");
                #endif
                return default;
            }

            [MethodImpl(AggressiveInlining)]
            public static TagDynId GetDynamicId<T>() where T : struct, ITag {
                #if DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: GetTagDynamicId, World not initialized");
                #endif
                return new TagDynId(Tags<T>.Value.Id());
            }

            [MethodImpl(AggressiveInlining)]
            public static ushort TagsCount(Entity entity) {
                #if DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: TagsCount, World not initialized");
                #endif
                return BitMask.Len(entity._id);
            }

            [MethodImpl(AggressiveInlining)]
            internal static void DestroyEntity(Entity entity) {
                var id = BitMask.GetMinIndex(entity._id);
                while (id >= 0) {
                    _pools[id].Del(entity);
                    id = BitMask.GetMinIndex(entity._id);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static string ToPrettyStringEntity(Entity entity) {
                var str = "Tags:\n";
                var bufId = BitMask.BorrowBuf();
                Array.Copy(_bitMap, entity._id * BitMapLen, BitMask._buffer, bufId * BitMask._len, BitMapLen);
                var id = BitMask.GetMinIndexBuffer(bufId);
                while (id >= 0) {
                    str += _pools[id].ToStringComponent(entity);
                    BitMask.DelInBuffer(bufId, (ushort) id);
                    id = BitMask.GetMinIndexBuffer(bufId);
                }
                BitMask.DropBuf(bufId);
                return str;
            }

            [MethodImpl(AggressiveInlining)]
            internal static void CopyEntity(Entity srcEntity, Entity dstEntity) {
                var bufId = BitMask.BorrowBuf();
                Array.Copy(_bitMap, srcEntity._id * BitMapLen, BitMask._buffer, bufId * BitMask._len, BitMapLen);
                var id = BitMask.GetMinIndexBuffer(bufId);
                while (id >= 0) {
                    _pools[id].Copy(srcEntity, dstEntity);
                    BitMask.Del(bufId, (ushort) id);
                    id = BitMask.GetMinIndex(bufId);
                }

                BitMask.DropBuf(bufId);
            }

            [MethodImpl(AggressiveInlining)]
            internal static void Resize(int cap) {
                for (int i = 0, iMax = _poolsCount; i < iMax; i++) {
                    _pools[i].Resize(cap);
                }
                Array.Resize(ref _bitMap, cap * BitMapLen);
                BitMask.SetNewBitMap(_bitMap);
            }

            [MethodImpl(AggressiveInlining)]
            internal static void SetTagBit(Entity entity, ushort index) {
                BitMask.Set(entity._id, index);
            }

            [MethodImpl(AggressiveInlining)]
            internal static void UnsetTagBit(Entity entity, ushort index) {
                BitMask.Del(entity._id, index);
            }
            
                        
            [MethodImpl(AggressiveInlining)]
            internal static void Clear() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Clear();
                }

                Array.Clear(_bitMap, 0, _bitMap.Length);
            }

            internal static void Destroy() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Destroy();
                    _resetActions[i]();
                }

                BitMask.Destroy();
                BitMask = null;
                _pools = null;
                _poolsCount = 0;
                _bitMap = null;
                BitMapLen = 0;
                TagCounter.Value = -1;
            }

            #if ENABLE_IL2CPP
            [Il2CppEagerStaticClassConstruction]
            #endif
            private static class TagCounter {
                public static int Value = -1;
            }

            #if ENABLE_IL2CPP
            [Il2CppEagerStaticClassConstruction]
            #endif
            private static class TagInfo {
                public static uint BaseCapacity = 128;
            }

            #if ENABLE_IL2CPP
            [Il2CppEagerStaticClassConstruction]
            #endif
            private static class TagInfo<T> where T : struct, ITag {
                public static ushort Id = ushort.MaxValue;
                public static Type Type;
                public static uint? BaseCapacity;

                public static void Register() {
                    Id = (ushort) Interlocked.Increment(ref TagCounter.Value);
                    Type = typeof(T);
                }

                public static void Reset() {
                    Id = ushort.MaxValue;
                }

                public static bool Has() => Id < ushort.MaxValue;
            }
        }
    }
    
    public struct DeleteTagsSystem<WorldId, T> : IUpdateSystem where T : struct, ITag where WorldId : struct, IWorldId {
        [MethodImpl(AggressiveInlining)]
        public void Update() {
            foreach (var entity in Ecs<WorldId>.World.QueryEntities.For<TagAll<T>>()) {
                entity.DeleteTag<T>();
            }
        }
    }
}
#endif