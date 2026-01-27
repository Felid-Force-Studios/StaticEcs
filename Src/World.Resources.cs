#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public abstract partial class World<TWorld> {
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        /// <summary>
        /// Lightweight handle for accessing a singleton resource of type <typeparamref name="T"/>
        /// stored in the world's static <see cref="Resources{T}"/> storage.
        /// <para>
        /// Resources are globally unique per world per type — there can be at most one instance of
        /// <typeparamref name="T"/> registered at any time. Unlike components (which are per-entity),
        /// resources are world-level singletons used for shared state: configuration, asset caches,
        /// time/delta-time values, input state, or any data that doesn't belong to a specific entity.
        /// </para>
        /// <para>
        /// This struct is a zero-cost handle — it contains no data. All access goes through static
        /// generic storage (<c>Resources&lt;T&gt;</c>), so creating or copying this struct is free.
        /// Register the resource via <see cref="SetResource{TResource}(TResource, bool)"/>
        /// before accessing <see cref="Value"/>.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        public readonly struct Resource<T> {
            /// <summary>
            /// Whether a resource of type <typeparamref name="T"/> has been registered in this world.
            /// Returns <c>false</c> before <see cref="SetResource{TResource}(TResource, bool)"/>
            /// or after <see cref="RemoveResource{TResource}"/>.
            /// </summary>
            public bool IsRegistered {
                [MethodImpl(AggressiveInlining)]
                get => Resources<T>.IsRegistered;
            }

            /// <summary>
            /// Returns a direct reference to the stored resource value. Modifications via this ref
            /// are written directly to static storage — no setter call required.
            /// <para>
            /// The resource must be registered before accessing this property.
            /// No bounds checking is performed in release builds for maximum performance.
            /// </para>
            /// </summary>
            public ref T Value {
                [MethodImpl(AggressiveInlining)]
                get => ref Resources<T>.Value;
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        /// <summary>
        /// Handle for accessing a keyed resource of type <typeparamref name="T"/> identified by a string key.
        /// Unlike <see cref="Resource{T}"/> (one per type), keyed resources allow multiple instances
        /// of the same type distinguished by key — e.g., <c>"player_config"</c> and <c>"enemy_config"</c>
        /// can both be <c>NamedResource&lt;GameConfig&gt;</c>.
        /// <para>
        /// Internally, keyed resources are stored in a shared <c>Dictionary&lt;string, object&gt;</c>
        /// with type-erased <c>Box&lt;T&gt;</c> wrappers. This struct caches the resolved box reference
        /// after the first <see cref="Value"/> access, avoiding repeated dictionary lookups. The cache
        /// is automatically invalidated when the underlying resource is removed or replaced (tracked
        /// via a per-box <c>IsValid</c> flag).
        /// </para>
        /// <para>
        /// Use keyed resources when you need multiple instances of the same type (e.g., per-level config,
        /// named service locators) or when the resource identity is determined at runtime (dynamic keys).
        /// For single-instance-per-type resources, prefer <see cref="Resource{T}"/> which uses
        /// static generic storage with zero lookup cost.
        /// </para>
        /// <para>
        /// <b>Warning:</b> This is a mutable struct that caches an internal reference on first
        /// <see cref="Value"/> access. Do NOT store it in a <c>readonly</c> field or pass by value
        /// after first use — the C# compiler will create a defensive copy, discarding the cache and
        /// causing a dictionary lookup on every access. Store it in a non-readonly field or local variable.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The resource value type.</typeparam>
        public struct NamedResource<T> {
            /// <summary>
            /// The string key identifying this resource in the keyed resource dictionary.
            /// </summary>
            public readonly string Key;
            private NamedResources.BoxBase _cache;

            /// <summary>
            /// Creates a keyed resource handle bound to the specified key.
            /// Does not register the resource — call <see cref="SetResource{TResource}(string, TResource, bool)"/> to register a value.
            /// </summary>
            /// <param name="key">The unique string key for this resource.</param>
            [MethodImpl(AggressiveInlining)]
            public NamedResource(string key) {
                Key = key;
                _cache = null;
            }

            /// <summary>
            /// Whether a resource with this key is currently registered.
            /// Unlike <see cref="Value"/>, this always performs a dictionary lookup (not cached).
            /// </summary>
            public bool IsRegistered {
                [MethodImpl(AggressiveInlining)]
                get => NamedResources.Has(Key);
            }

            /// <summary>
            /// Returns a direct reference to the stored resource value. Modifications via this ref
            /// are written directly to the internal box — no setter call required.
            /// <para>
            /// On the first access (or after cache invalidation), resolves the box from the dictionary
            /// and caches the reference. Subsequent accesses return the cached ref in O(1) without
            /// dictionary lookup. The cache is invalidated automatically when the resource is removed
            /// via <see cref="RemoveResource(string)"/> or world destruction (per-box <c>IsValid</c> flag).
            /// </para>
            /// </summary>
            public ref T Value {
                [MethodImpl(AggressiveInlining)]
                get {
                    if (_cache == null || !_cache.IsValid) {
                        if (!NamedResources.Values.TryGetValue(Key, out var boxObj)) {
                            throw new StaticEcsException($"NamedResource<{typeof(T).Name}> with key '{Key}' not found in World<{typeof(TWorld).Name}>");
                        }
                        _cache = (NamedResources.BoxBase)boxObj;
                    }
                    return ref ((NamedResources.Box<T>)_cache).Value;
                }
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        internal readonly struct Resources<T> {
            internal static T Value;
            internal static bool IsRegistered;

            [MethodImpl(AggressiveInlining)]
            internal static bool Has() => IsRegistered;

            [MethodImpl(AggressiveInlining)]
            internal static ref T Get() => ref Value;

            [MethodImpl(AggressiveInlining)]
            internal static void Set(T value, bool clearOnDestroy = true) {
                if (IsRegistered) {
                    Value = value;
                    return;
                }

                IsRegistered = true;
                Value = value;
                ResourcesData.Instance.Add<T>(clearOnDestroy);
            }

            [MethodImpl(AggressiveInlining)]
            internal static void Remove() {
                IsRegistered = false;
                Value = default;
                ResourcesData.Instance.RemoveMethods<T>();
            }
        }
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        internal readonly struct NamedResources {
            internal class BoxBase {
                internal bool IsValid = true;
            }

            internal class Box<T> : BoxBase {
                public T Value;
            }

            internal static readonly Dictionary<string, object> Values = new();
            internal static readonly HashSet<string> KeysToClear = new();

            [MethodImpl(AggressiveInlining)]
            internal static bool Has(string key) => Values.ContainsKey(key);

            [MethodImpl(AggressiveInlining)]
            internal static ref T Get<T>(string key) {
                if (!Values.TryGetValue(key, out var boxObj)) {
                    throw new StaticEcsException($"NamedResources<{typeof(T).Name}> with key '{key}' not found");
                }

                return ref ((Box<T>)boxObj).Value;
            }

            [MethodImpl(AggressiveInlining)]
            internal static void Clear() {
                foreach (var clearKey in KeysToClear) {
                    if (Values.TryGetValue(clearKey, out var boxObj)) {
                        ((BoxBase) boxObj).IsValid = false;
                    }
                    Values.Remove(clearKey);
                }

                KeysToClear.Clear();
            }

            [MethodImpl(AggressiveInlining)]
            internal static void Set<T>(string key, T value, bool clearOnDestroy = true) {
                if (Values.TryGetValue(key, out var existing)) {
                    ((Box<T>)existing).Value = value;
                    return;
                }

                Values[key] = new Box<T> { Value = value };
                if (clearOnDestroy) {
                    KeysToClear.Add(key);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal static void Remove(string key) {
                if (Values.TryGetValue(key, out var boxObj)) {
                    ((BoxBase) boxObj).IsValid = false;
                }
                Values.Remove(key);
                KeysToClear.Remove(key);
            }
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        internal struct ResourcesData {
            internal static ResourcesData Instance;

            internal Dictionary<Type, Action> ContextClearMethods;
            internal Dictionary<Type, (Func<object> get, Action<object, bool> set, Action remove)> ValuesGetSetRawMethods;
            
            [MethodImpl(AggressiveInlining)]
            internal void Add<T>(bool clearAfterDestroy) {
                var type = typeof(T);
                ValuesGetSetRawMethods ??= new Dictionary<Type, (Func<object>, Action<object, bool>, Action)>();
                ValuesGetSetRawMethods[type] = (
                    static () => Resources<T>.Get(), static (val, clear) => Resources<T>.Set((T) val, clear), Resources<T>.Remove
                );

                if (clearAfterDestroy) {
                    ContextClearMethods ??= new Dictionary<Type, Action>();
                    ContextClearMethods[type] = () => {
                        Resources<T>.Value = default;
                        Resources<T>.IsRegistered = false;
                    };
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void RemoveMethods<T>() {
                ContextClearMethods?.Remove(typeof(T));
                ValuesGetSetRawMethods?.Remove(typeof(T));
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                if (ContextClearMethods != null) {
                    foreach (var kvp in ContextClearMethods) {
                        kvp.Value();
                        ValuesGetSetRawMethods?.Remove(kvp.Key);
                    }
                    ContextClearMethods.Clear();
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly ref T GetOrCreate<T>(out bool isNew) where T : new() {
                isNew = !Resources<T>.IsRegistered;
                if (isNew) {
                    Resources<T>.Set(new T());
                }
                return ref Resources<T>.Value;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly Dictionary<Type, (Func<object>, Action<object, bool>, Action)> GetAllGetSetRemoveValuesMethods() => ValuesGetSetRawMethods;

            [MethodImpl(AggressiveInlining)]
            internal readonly bool HasRaw(Type type) => ValuesGetSetRawMethods != null && ValuesGetSetRawMethods.ContainsKey(type);

            [MethodImpl(AggressiveInlining)]
            internal readonly object GetRaw(Type type) => ValuesGetSetRawMethods[type].get();

            [MethodImpl(AggressiveInlining)]
            internal readonly void RemoveRaw(Type type) => ValuesGetSetRawMethods[type].remove();
        }
    }

}