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
        public struct Context : IContext {
            public static Context Value;

            internal Dictionary<Type, Action> contextClearMethods;
            
            [MethodImpl(AggressiveInlining)]
            internal void AddClearMethod<T>() {
                contextClearMethods ??= new Dictionary<Type, Action>();
                contextClearMethods[typeof(T)] = () => {
                    Context<T>._value = default;
                    Context<T>._has = false;
                };
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void RemoveClearMethod<T>() {
                contextClearMethods?.Remove(typeof(T));
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                if (contextClearMethods != null) {
                    foreach (var action in contextClearMethods.Values) {
                        action();
                    }
                    contextClearMethods.Clear();
                }
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool Has<T>() => Context<T>.Has();

            [MethodImpl(AggressiveInlining)]
            public readonly ref T Get<T>() => ref Context<T>._value;

            [MethodImpl(AggressiveInlining)]
            public readonly ref T GetOrCreate<T>() where T : new() {
                if (!Context<T>._has) {
                    Context<T>.Set(new T());
                }
                return ref Context<T>._value;
            }

            [MethodImpl(AggressiveInlining)]
            public readonly void Set<T>(T value, bool clearOnDestroy = true) => Context<T>.Set(value, clearOnDestroy);

            [MethodImpl(AggressiveInlining)]
            public readonly void Replace<T>(T value) => Context<T>.Replace(value);

            [MethodImpl(AggressiveInlining)]
            public readonly void Remove<T>() => Context<T>.Remove();
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
            public static void Set(T value, bool clearOnDestroy = true) {
                if (value == null) {
                    throw new Exception($"{typeof(T).Name} is null, Context<{typeof(WorldType)}>");
                }

                if (_has) {
                    throw new Exception($"{typeof(T).Name} already exist in container Context<{typeof(WorldType)}>");
                }

                _has = true;
                _value = value;
                if (clearOnDestroy) {
                    Context.Value.AddClearMethod<T>();
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void Replace(T value) {
                if (value == null) {
                    throw new Exception($"{typeof(T).Name} is null, Context<{typeof(WorldType)}>");
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
                Context.Value.RemoveClearMethod<T>();
            }
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        public struct NamedContext {
            private static readonly Dictionary<string, object> _values = new();
            private static readonly HashSet<string> _clearKeys = new();

            [MethodImpl(AggressiveInlining)]
            public static void Clear() {
                foreach (var clearKey in _clearKeys) {
                    _values.Remove(clearKey);
                }

                _clearKeys.Clear();
            }

            [MethodImpl(AggressiveInlining)]
            public static bool Has(string key) => _values.ContainsKey(key);

            [MethodImpl(AggressiveInlining)]
            public static T Get<T>(string key) => (T) _values[key];

            [MethodImpl(AggressiveInlining)]
            public static void Set<T>(string key, T value, bool clearOnDestroy = true) {
                if (value == null) {
                    throw new Exception($"{typeof(T).Name} is null, NamedContext<{typeof(WorldType)}>");
                }

                if (_values.ContainsKey(key)) {
                    throw new Exception($"{typeof(T).Name} already exist in container NamedContext<{typeof(WorldType)}>");
                }

                _values[key] = value;
                if (clearOnDestroy) {
                    _clearKeys.Add(key);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void Replace<T>(string key, T value) {
                if (value == null) {
                    throw new Exception($"{typeof(T).Name} is null, NamedContext<{typeof(WorldType)}>");
                }
                
                _values[key] = value;
            }

            [MethodImpl(AggressiveInlining)]
            public static void Remove(string key) {
                _values.Remove(key);
                _clearKeys.Remove(key);
            }
        }
    }

    public interface IContext {
        public bool Has<T>();

        public ref T Get<T>();

        public void Set<T>(T value, bool clearOnDestroy);

        public void Replace<T>(T value);

        public void Remove<T>();
    }
}