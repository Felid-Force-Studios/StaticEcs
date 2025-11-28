#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
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

        [MethodImpl(AggressiveInlining)]
        public static void RegisterMultiComponentType<T, V>(ushort defaultComponentCapacity, IComponentConfig<T, WorldType> config = null)
            where T : struct, IMultiComponent<T, V> where V : struct {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif
            config ??= DefaultComponentConfig<T, WorldType>.Default;
            ModuleComponents.Value.RegisterMultiComponentType<T, V>(defaultComponentCapacity, config);
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        internal partial struct ModuleComponents {
            [MethodImpl(AggressiveInlining)]
            internal void RegisterMultiComponentType<T, V>(
                ushort defaultComponentCapacity,
                IComponentConfig<T, WorldType> config
            ) where T : struct, IMultiComponent<T, V> where V : struct {
                #if FFS_ECS_DEBUG
                AssertNotRegisteredComponent<T>(WorldTypeName);
                #endif

                RegisterMultiComponentsData<V>(defaultComponentCapacity);

                var actualConfig = new ValueComponentConfig<T, WorldType>(config);

                var onAdd = config.OnAdd();
                if (onAdd != null) {
                    actualConfig.OnAddHandler = (Entity e, ref T component) => {
                        OnAddMultiComponent<T, V>(e, ref component);
                        onAdd(e, ref component);
                    };
                } else {
                    actualConfig.OnAddHandler = OnAddMultiComponent<T, V>;
                }

                var onPut = config.OnPut();
                if (onPut != null) {
                    actualConfig.OnPutHandler = (Entity e, ref T component) => {
                        OnAddMultiComponent<T, V>(e, ref component);
                        onPut(e, ref component);
                    };
                } else {
                    actualConfig.OnPutHandler = OnAddMultiComponent<T, V>;
                }

                var onDelete = config.OnDelete();
                if (onDelete != null) {
                    actualConfig.OnDeleteHandler = (Entity e, ref T component) => {
                        onDelete(e, ref component);
                        Context<MultiComponents<V>>.Get().Delete(ref component.RefValue(ref component));
                    };
                } else {
                    actualConfig.OnDeleteHandler = static (Entity _, ref T component) => Context<MultiComponents<V>>.Get().Delete(ref component.RefValue(ref component));
                }

                var onCopy = config.OnCopy();
                if (onCopy != null) {
                    actualConfig.OnCopyHandler = (Entity srcEntity, Entity dstEntity, ref T src, ref T dst) => {
                        onCopy(srcEntity, dstEntity, ref src, ref dst);
                        CopyMultiComponent<T, V>(srcEntity, dstEntity, ref src, ref dst);
                    };
                } else {
                    actualConfig.OnCopyHandler = CopyMultiComponent<T, V>;
                }

                RegisterComponentType(
                    config : actualConfig
                );
            }
            
            private static void RegisterMultiComponentsData<T>(ushort defaultComponentCapacity) where T : struct {
                if (!Context<MultiComponents<T>>.Has()) {
                    #if FFS_ECS_DEBUG
                    Context<MultiComponents<T>>.Set(new MultiComponents<T>(defaultComponentCapacity, MTStatus));
                    #else
                    Context<MultiComponents<T>>.Set(new MultiComponents<T>(defaultComponentCapacity));
                    #endif
                }
            }
            
            private static void OnAddMultiComponent<T, V>(Entity e, ref T component) where T : struct, IMultiComponent<T, V> where V : struct {
                Context<MultiComponents<V>>.Get().Add(ref component.RefValue(ref component));
            }

            [MethodImpl(AggressiveInlining)]
            internal static void CopyMultiComponent<T, V>(Entity srcEntity, Entity dstEntity, ref T src, ref T dst) where T : struct, IMultiComponent<T, V> where V : struct {
                ref var srsItems = ref src.RefValue(ref src);
                var dstItemsTemp = dst.RefValue(ref dst);
                dst = src;
                ref var dstItems = ref dst.RefValue(ref dst);
                dstItems = dstItemsTemp;
                dstItems.Clear();
                dstItems.Add(ref srsItems);
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public static class MultiComponentExtensions {
        
        [MethodImpl(AggressiveInlining)]
        public static Multi<T> ReadMulti<WorldType, T>(this ref BinaryPackReader reader) where T : struct where WorldType : struct, IWorldType {
            var value = new Multi<T>();
            var count = reader.ReadUshort();
            if (count > 0) {
                World<WorldType>.Context<MultiComponents<T>>.Get().Add(ref value, count);
                for (var i = 0; i < count; i++) {
                    value.Add(reader.Read<T>());
                }
            } else {
                World<WorldType>.Context<MultiComponents<T>>.Get().Add(ref value);
            }

            return value;
        }
        
        [MethodImpl(AggressiveInlining)]
        public static void WriteMulti<T>(this ref BinaryPackWriter writer, in Multi<T> value) where T : struct {
            var count = value.count;
            writer.WriteUshort(count);
            if (count > 0) {
                var values = value.data.values;
                var offset = value.offset;
                for (var i = 0; i < count; i++) {
                    writer.Write(in values[i + offset]);
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public static ROMulti<T> ReadROMulti<WorldType, T>(this ref BinaryPackReader reader) where T : struct where WorldType : struct, IWorldType {
            return new ROMulti<T> {
                multi = reader.ReadMulti<WorldType, T>(),
            };
        }
        
        [MethodImpl(AggressiveInlining)]
        public static void WriteROMulti<T>(this ref BinaryPackWriter writer, in ROMulti<T> value) where T : struct {
            writer.WriteMulti(in value.multi);
        }
    }
}