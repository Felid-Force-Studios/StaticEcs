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
            internal Dictionary<Type, (Func<object> get, Action<object> set, Action remove)> valuesGetSetRawMethods;
            
            [MethodImpl(AggressiveInlining)]
            internal void Add<T>(bool clearAfterDestroy) {
                var type = typeof(T);
                valuesGetSetRawMethods ??= new Dictionary<Type, (Func<object>, Action<object>, Action)>();
                valuesGetSetRawMethods[type] = (
                    static () => Context<T>.Get(), static val => Context<T>.Replace((T) val), Context<T>.Remove
                );

                if (clearAfterDestroy) {
                    contextClearMethods ??= new Dictionary<Type, Action>();
                    contextClearMethods[type] = () => {
                        Context<T>._value = default;
                        Context<T>._has = false;
                    };
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void RemoveMethods<T>() {
                contextClearMethods?.Remove(typeof(T));
                valuesGetSetRawMethods?.Remove(typeof(T));
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                if (contextClearMethods != null) {
                    foreach (var action in contextClearMethods.Values) {
                        action();
                    }
                    contextClearMethods.Clear();
                    valuesGetSetRawMethods.Clear();
                }
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool Has<T>() => Context<T>.Has();

            [MethodImpl(AggressiveInlining)]
            public readonly ref T Get<T>() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!Context<T>._has) {
                    throw new StaticEcsException($"Context<{typeof(WorldType)}> for {typeof(T)} undefined");
                }
                #endif
                return ref Context<T>._value;
            }

            object IContext.GetRaw(Type type) => valuesGetSetRawMethods[type].get();

            void IContext.ReplaceRaw(Type type, object value) => valuesGetSetRawMethods[type].set(value);

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

            [MethodImpl(AggressiveInlining)]
            public bool HasNamed(string key) =>  NamedContext.Has(key);

            [MethodImpl(AggressiveInlining)]
            public T GetNamed<T>(string key) => NamedContext.Get<T>(key);

            object IContext.GetRawNamed(string key) => NamedContext._values[key];

            void IContext.ReplaceRawNamed(string key, object value) {
                if (!NamedContext._values.ContainsValue(key)) {
                    throw new StaticEcsException($"{key} not exist in container");
                }

                NamedContext._values[key] = value;
            }

            [MethodImpl(AggressiveInlining)]
            public void SetNamed<T>(string key, T value, bool clearOnDestroy) => NamedContext.Set(key, value, clearOnDestroy);

            [MethodImpl(AggressiveInlining)]
            public void ReplaceNamed<T>(string key, T value) => NamedContext.Replace(key, value);

            [MethodImpl(AggressiveInlining)]
            public void RemoveNamed(string key) => NamedContext.Remove(key);

            IReadOnlyDictionary<string, object> IContext.GetAllNamedValues() => NamedContext._values;

            IReadOnlyDictionary<Type, (Func<object>, Action<object>, Action)> IContext.GetAllGetSetRemoveValuesMethods() => valuesGetSetRawMethods;
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
                    throw new StaticEcsException($"{typeof(T).Name} is null, Context<{typeof(WorldType)}>");
                }

                if (_has) {
                    throw new StaticEcsException($"{typeof(T).Name} already exist in container Context<{typeof(WorldType)}>");
                }

                _has = true;
                _value = value;
                Context.Value.Add<T>(clearOnDestroy);
            }

            [MethodImpl(AggressiveInlining)]
            public static void Replace(T value) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (value == null) {
                    throw new StaticEcsException($"{typeof(T).Name} is null, Context<{typeof(WorldType)}>");
                }
                #endif

                if (!_has) {
                    Context.Value.Add<T>(false);
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
                Context.Value.RemoveMethods<T>();
            }
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        public struct NamedContext {
            internal static readonly Dictionary<string, object> _values = new();
            internal static readonly HashSet<string> _clearKeys = new();

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
                    throw new StaticEcsException($"{typeof(T).Name} is null, NamedContext<{typeof(WorldType)}>");
                }

                if (!_values.TryAdd(key, value)) {
                    throw new StaticEcsException($"{typeof(T).Name} already exist in container NamedContext<{typeof(WorldType)}>");
                }

                if (clearOnDestroy) {
                    _clearKeys.Add(key);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void Replace<T>(string key, T value) {
                if (value == null) {
                    throw new StaticEcsException($"{typeof(T).Name} is null, NamedContext<{typeof(WorldType)}>");
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

        internal object GetRaw(Type type);

        internal void ReplaceRaw(Type type, object value);

        public void Set<T>(T value, bool clearOnDestroy);

        public void Replace<T>(T value);

        public void Remove<T>();
        
        public bool HasNamed(string key);
        
        public T GetNamed<T>(string key);

        internal object GetRawNamed(string key);

        internal void ReplaceRawNamed(string key, object value);

        public void SetNamed<T>(string key, T value, bool clearOnDestroy);

        public void ReplaceNamed<T>(string key, T value);

        public void RemoveNamed(string key);
        
        internal IReadOnlyDictionary<string, object> GetAllNamedValues();
        
        internal IReadOnlyDictionary<Type, (Func<object>, Action<object>, Action)> GetAllGetSetRemoveValuesMethods();
    }
}