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
        public readonly struct Context : IContext {
            public static Context Value = default;

            [MethodImpl(AggressiveInlining)]
            public bool Has<T>() => Context<T>.Has();

            [MethodImpl(AggressiveInlining)]
            public ref T Get<T>() => ref Context<T>._value;

            [MethodImpl(AggressiveInlining)]
            public void Set<T>(T value) => Context<T>.Set(value);

            [MethodImpl(AggressiveInlining)]
            public void Replace<T>(T value) => Context<T>.Replace(value);

            [MethodImpl(AggressiveInlining)]
            public void Remove<T>() => Context<T>.Remove();
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        public struct Context<T> {
            internal static T _value;
            internal static bool _has;

            [MethodImpl(AggressiveInlining)]
            public static void Set(T value) {
                if (value == null) {
                    throw new Exception($"{typeof(T).Name} is null, Context<{typeof(WorldID)}>");
                }

                if (_has) {
                    throw new Exception($"{typeof(T).Name} already exist in container Context<{typeof(WorldID)}>");
                }

                _has = true;
                _value = value;
            }

            [MethodImpl(AggressiveInlining)]
            public static void Replace(T value) {
                if (value == null) {
                    throw new Exception($"{typeof(T).Name} is null, Context<{typeof(WorldID)}>");
                }

                _has = true;
                _value = value;
            }

            [MethodImpl(AggressiveInlining)]
            public static bool Has() {
                return _has;
            }

            [MethodImpl(AggressiveInlining)]
            public static ref T Get() {
                return ref _value;
            }

            [MethodImpl(AggressiveInlining)]
            public static void Remove() {
                _has = false;
                _value = default;
            }
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        public struct NamedContext {
            private static Dictionary<string, object> _values;

            public static void Create() {
                _values = new Dictionary<string, object>();
            }

            [MethodImpl(AggressiveInlining)]
            public static bool Has(string key) => _values.ContainsKey(key);

            [MethodImpl(AggressiveInlining)]
            public static T Get<T>(string key) => (T) _values[key];

            [MethodImpl(AggressiveInlining)]
            public static void Set<T>(string key, T value) {
                if (value == null) {
                    throw new Exception($"{typeof(T).Name} is null, NamedContext<{typeof(WorldID)}>");
                }

                if (_values.ContainsKey(key)) {
                    throw new Exception($"{typeof(T).Name} already exist in container NamedContext<{typeof(WorldID)}>");
                }

                _values[key] = value;
            }

            [MethodImpl(AggressiveInlining)]
            public static void Replace<T>(string key, T value) {
                if (value == null) {
                    throw new Exception($"{typeof(T).Name} is null, NamedContext<{typeof(WorldID)}>");
                }
                
                _values[key] = value;
            }

            [MethodImpl(AggressiveInlining)]
            public static void Remove(string key) => _values.Remove(key);
        }
    }

    public interface IContext {
        public bool Has<T>();

        public ref T Get<T>();

        public void Set<T>(T value);

        public void Replace<T>(T value);

        public void Remove<T>();
    }
}