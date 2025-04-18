﻿using System.Runtime.CompilerServices;
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
        #endif
        internal partial struct ModuleComponents {
            [MethodImpl(AggressiveInlining)]
            internal ushort RegisterMultiComponentType<T, V>(ushort defaultComponentCapacity, uint capacity, OnAddHandler<T> onAdd = null) where T : struct, IMultiComponent<V> where V : struct {
                if (Components<T>.Value.IsRegistered()) {
                    return Components<T>.Value.DynamicId();
                }

                if (!Context<MultiComponents<V>>.Has()) {
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    Context<MultiComponents<V>>.Set(new MultiComponents<V>(defaultComponentCapacity, capacity, MTStatus));
                    #else
                    Context<MultiComponents<V>>.Set(new MultiComponents<V>(defaultComponentCapacity, capacity));
                    #endif
                }

                OnAddHandler<T> initHandler;
                if (onAdd != null) {
                    initHandler = (ref T component) => {
                        component.Access<AutoInit<V>>(default);
                        onAdd(ref component);
                    };
                } else {
                    initHandler = static (ref T component) => component.Access<AutoInit<V>>(default);
                }

                return RegisterComponentType(
                    capacity: capacity,
                    onAdd: initHandler,
                    onPut: static (ref T component) => component.Access<AutoInit<V>>(default),
                    onDelete: static (ref T component) => component.Access<AutoReset<V>>(default),
                    onCopy: static (ref T src, ref T dst) => {
                        var copy = default(CopyToAccess<T, V>);
                        copy.Copy(ref src, ref dst);
                    }
                );
            }
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public struct AutoInit<V> : AccessMulti<V> where V : struct {
            [MethodImpl(AggressiveInlining)]
            public void For(ref Multi<V> multi) => Context<MultiComponents<V>>.Get().Add(ref multi);
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public struct AutoReset<V> : AccessMulti<V> where V : struct {
            [MethodImpl(AggressiveInlining)]
            public void For(ref Multi<V> multi) => Context<MultiComponents<V>>.Get().Delete(ref multi);
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public struct CopyFromAccess<V> : AccessMulti<V> where V : struct {
            internal uint offset;
            internal ushort count;

            [MethodImpl(AggressiveInlining)]
            public void For(ref Multi<V> multi) {
                offset = multi.offset;
                count = multi.count;
            }
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public struct CopyToAccess<T, V> : AccessMulti<V> where V : struct where T : struct, IMultiComponent<V> {
            internal CopyFromAccess<V> copyFrom;

            [MethodImpl(AggressiveInlining)]
            public void For(ref Multi<V> multi) => Context<MultiComponents<V>>.Get().Copy(copyFrom.count, copyFrom.offset, ref multi);

            [MethodImpl(AggressiveInlining)]
            public void Copy(ref T src, ref T dst) {
                src.Access(copyFrom);
                dst.Access(this);
            }
        }
    }
}