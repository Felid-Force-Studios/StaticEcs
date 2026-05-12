#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    /// <summary>
    /// Marker interface for systems group identity. Each unique <c>ISystemsType</c> implementation
    /// creates an isolated <see cref="World{TWorld}.Systems{SysType}"/> instance with its own static storage,
    /// following the same pattern as <c>World&lt;TWorld&gt;</c>.
    /// <para>
    /// This allows multiple independent system pipelines to coexist (e.g., separate groups
    /// for game logic, rendering, physics, or network sync), each with their own lifecycle.
    /// </para>
    /// </summary>
    public interface ISystemsType { }

    /// <summary>
    /// Interface for ECS systems. Systems contain game logic that operates on entities.
    /// All methods have default empty implementations — override only what you need.
    /// <para>
    /// Method detection uses reflection at registration time (similar to <see cref="IComponent"/> hooks):
    /// only overridden methods are called at runtime, so unimplemented methods have zero cost.
    /// </para>
    /// </summary>
    public interface ISystem {
        /// <summary>
        /// Called once during <see cref="World{TWorld}.Systems{SysType}.Initialize"/>, after all systems are added.
        /// Systems are initialized in order (by <c>order</c> parameter, then by registration order).
        /// Use for one-time setup: caching queries, loading resources, subscribing to events.
        /// </summary>
        public void Init() { }

        /// <summary>
        /// Called every frame by <see cref="World{TWorld}.Systems{SysType}.Update"/>, but only if
        /// <see cref="UpdateIsActive"/> returns <c>true</c>. This is the main per-frame logic entry point.
        /// Only invoked if the system actually overrides this method.
        /// </summary>
        public void Update() { }

        /// <summary>
        /// Called before each <see cref="Update"/> invocation. Return <c>false</c> to skip
        /// this system's Update for the current frame. Default returns <c>true</c> (always active).
        /// Use for conditional execution (e.g., pause state, feature flags, cooldown timers).
        /// </summary>
        public bool UpdateIsActive() => true;

        /// <summary>
        /// Called once during <see cref="World{TWorld}.Systems{SysType}.Destroy"/>, in initialization order
        /// (sorted by order, then registration order).
        /// Use for cleanup: releasing resources, unsubscribing from events, saving state.
        /// </summary>
        public void Destroy() { }

        /// <summary>
        /// Stable identifier used to match this system instance against a <c>WorldSnapshot</c> entry
        /// at load time. Returning a non-empty <see cref="System.Guid"/> opts the system into automatic
        /// serialisation. Default returns <c>null</c> — the system is excluded from snapshots.
        /// </summary>
        public Guid? Guid() => null;

        /// <summary>
        /// Schema version stored alongside serialised system state. When the saved version differs
        /// from the current one, <see cref="Read"/> is called for migration instead of bulk byte-copy.
        /// </summary>
        public byte Version() => 0;

        /// <summary>
        /// Custom serialisation hook. Required only when <see cref="Guid"/> is set and the system type
        /// is not unmanaged (e.g. it is a class or contains references). For unmanaged struct systems
        /// the framework copies raw memory and skips this hook unless the version changes.
        /// </summary>
        public void Write(ref BinaryPackWriter writer) {}

        /// <summary>
        /// Custom deserialisation hook. Required only when <see cref="Guid"/> is set and the system type
        /// is not unmanaged. Also invoked on version mismatch for unmanaged types.
        /// </summary>
        public void Read(ref BinaryPackReader reader, byte version) {}
    }

    /// <summary>
    /// Lifecycle state of a <see cref="World{TWorld}.Systems{TSystemsType}"/> pipeline.
    /// </summary>
    public enum SystemsStatus : byte {
        /// <summary>Pipeline not created yet — call <c>Create</c> first.</summary>
        NotCreated,
        /// <summary>Pipeline created, awaiting <c>Add</c> calls and <c>Initialize</c>.</summary>
        Created,
        /// <summary>Pipeline initialized, <c>Update</c> can be called.</summary>
        Initialized
    }

    internal struct SystemData {
        public ISystem System;
        public int Index;
        public short Order;
        public bool HasUpdate;
        public bool HasInit;
        public bool HasDestroy;
        public bool HasUpdateIsActive;
        public Guid? Guid;
        public byte Version;
        #if FFS_ECS_DEBUG
        public float AvgUpdateTime;
        public bool DebugDisabled;
        #endif
    }

    public abstract partial class World<TWorld> where TWorld : struct, IWorldType {

        /// <summary>
        /// Static systems pipeline keyed by <typeparamref name="TSystemsType"/>.
        /// Manages a collection of <see cref="ISystem"/> instances with ordered initialization,
        /// per-frame updates, and cleanup. Each unique <typeparamref name="TSystemsType"/> type gets its own
        /// isolated static storage, allowing multiple independent system groups.
        /// <para>
        /// Lifecycle: <see cref="Create"/> → <see cref="Add{TSystem}"/> (register systems) → <see cref="Initialize"/>
        /// (sorts by order, calls Init) → <see cref="Update"/> (called each frame) → <see cref="Destroy"/> (cleanup).
        /// </para>
        /// <para>
        /// In debug mode, tracks per-system average update time via <c>DebugUpdateSystemsInfo</c> for profiling.
        /// </para>
        /// </summary>
        /// <typeparam name="TSystemsType">Systems group identity type. Must implement <see cref="ISystemsType"/>.</typeparam>
        public abstract class Systems<TSystemsType> where TSystemsType : struct, ISystemsType {
            
            /// <summary>
            /// Type-erased handle for accessing this systems pipeline's resources without
            /// knowing <typeparamref name="TSystemsType"/> at compile time. See <see cref="StaticEcs.SystemsHandle"/>.
            /// <para>
            /// Returned by reference; the handle is initialized in <see cref="Create"/> and
            /// reset to <c>default</c> in <see cref="Destroy"/>.
            /// </para>
            /// </summary>
            public static SystemsHandle Handle;
            
            #if FFS_ECS_DEBUG
            internal static int[] UpdateSystemsAllIndex;
            internal static Stopwatch Stopwatch;
            #endif
            internal static ISystem[] UpdateSystems;
            internal static ulong[] UpdateSystemLastTicks;
            internal static SystemData[] AllSystems;
            internal static int AllSystemsCount;
            internal static uint UpdateSystemsCount;
            internal static SystemsStatus Status;

            private static readonly IComparer<SystemData> SystemDataComparer =
                Comparer<SystemData>.Create((a, b) => a.Order != b.Order
                    ? a.Order.CompareTo(b.Order)
                    : a.Index.CompareTo(b.Index));

            /// <summary>
            /// Whether <see cref="Initialize"/> has been called. Systems cannot be updated before initialization.
            /// </summary>
            public static bool IsInitialized => Status == SystemsStatus.Initialized;

            /// <summary>
            /// Allocates internal arrays for the systems pipeline. Must be called before <see cref="Add{TSystem}"/>.
            /// </summary>
            /// <param name="baseSize">Initial capacity for the systems arrays. Will auto-resize if exceeded.
            /// Set higher if you know you will register many systems to avoid resizing.</param>
            internal static Guid Guid;

            [MethodImpl(AggressiveInlining)]
            public static void Create(uint baseSize = 64, Guid? snapshotGuid = null) {
                #if FFS_ECS_DEBUG
                if (Status != SystemsStatus.NotCreated) {
                    throw new StaticEcsException($"Systems<{typeof(TSystemsType)}>, Method: Create, systems already created.");
                }
                #endif
                baseSize = Math.Max(baseSize, 4);
                UpdateSystems = new ISystem[baseSize];
                UpdateSystemLastTicks = new ulong[baseSize];
                AllSystems = new SystemData[baseSize];
                AllSystemsCount = default;
                UpdateSystemsCount = default;
                #if FFS_ECS_DEBUG
                UpdateSystemsAllIndex = new int[baseSize];
                Stopwatch = new Stopwatch();
                #endif
                Status = SystemsStatus.Created;

                ResourcesData<TSystemsType>.Create();
                Guid = snapshotGuid ?? typeof(TSystemsType).GuidFromAQN();
                Handle = SystemsHandle.Create<TWorld, TSystemsType>(Guid);
                Data.Instance.RegisteredSystems.Add(Handle);
            }

            /// <summary>
            /// Sorts all registered systems by their <c>order</c> (ascending), then by registration order
            /// for systems with the same order value. After sorting, calls <see cref="ISystem.Init"/>
            /// on each system that overrides it, and builds the update list (only systems that override
            /// <see cref="ISystem.Update"/> are included in the per-frame update loop).
            /// <para>
            /// Must be called after all <see cref="Add{TSystem}"/> calls and before the first <see cref="Update"/>.
            /// </para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static void Initialize() {
                #if FFS_ECS_DEBUG
                if (Status != SystemsStatus.Created) {
                    throw new StaticEcsException($"Systems<{typeof(TSystemsType)}>, Method: Initialize, systems pipeline must be in Created state (current: {Status}).");
                }
                #endif

                Array.Sort(AllSystems, 0, AllSystemsCount, SystemDataComparer);

                UpdateSystemsCount = 0;
                for (var i = 0; i < AllSystemsCount; i++) {
                    var systemData = AllSystems[i];
                    if (systemData.HasInit) {
                        systemData.System.Init();
                    }

                    if (systemData.HasUpdate) {
                        if (UpdateSystemsCount == UpdateSystems.Length) {
                            Array.Resize(ref UpdateSystems, (int)(UpdateSystemsCount << 1));
                            Array.Resize(ref UpdateSystemLastTicks, (int)(UpdateSystemsCount << 1));
                            #if FFS_ECS_DEBUG
                            Array.Resize(ref UpdateSystemsAllIndex, (int)(UpdateSystemsCount << 1));
                            #endif
                        }
                        UpdateSystems[UpdateSystemsCount] = systemData.System;
                        UpdateSystemLastTicks[UpdateSystemsCount] = Data.Instance.CurrentTick;
                        #if FFS_ECS_DEBUG
                        UpdateSystemsAllIndex[UpdateSystemsCount] = i;
                        #endif
                        UpdateSystemsCount++;
                    }
                }

                Status = SystemsStatus.Initialized;
            }

            /// <summary>
            /// Executes the per-frame update loop. Iterates over all systems that override <see cref="ISystem.Update"/>,
            /// calling <see cref="ISystem.UpdateIsActive"/> first — if it returns <c>false</c>, the system is skipped.
            /// <para>
            /// Call this once per frame from your game loop. Systems run sequentially in the order
            /// determined during <see cref="Initialize"/> (sorted by order, then registration order).
            /// In debug mode, measures each system's update time for profiling.
            /// </para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static void Update() {
                #if FFS_ECS_DEBUG
                if (Status != SystemsStatus.Initialized) {
                    throw new StaticEcsException($"Systems<{typeof(TSystemsType)}>, Method: Update, systems pipeline must be initialized first.");
                }
                #endif
                ref var currentLastTick = ref Data.Instance.CurrentLastTick;
                for (var i = 0; i < UpdateSystemsCount; i++) {
                    var system = UpdateSystems[i];
                    #if FFS_ECS_DEBUG
                    if (AllSystems[UpdateSystemsAllIndex[i]].DebugDisabled) continue;
                    #endif
                    if (system.UpdateIsActive()) {
                        currentLastTick = UpdateSystemLastTicks[i];
                        #if FFS_ECS_DEBUG
                        Stopwatch.Restart();
                        #endif
                        system.Update();
                        #if FFS_ECS_DEBUG
                        Stopwatch.Stop();
                        ref var time = ref AllSystems[UpdateSystemsAllIndex[i]].AvgUpdateTime;
                        var elapsed = (float)Stopwatch.ElapsedTicks / Stopwatch.Frequency * 1000;
                        time = time == 0f ? elapsed : (elapsed + time) * 0.5f;
                        #endif
                        UpdateSystemLastTicks[i] = Data.Instance.CurrentTick;
                        currentLastTick = 0;
                    }
                }
            }

            /// <summary>
            /// Calls <see cref="ISystem.Destroy"/> on every registered system that overrides it
            /// (in initialization order: sorted by order, then registration order),
            /// then releases all internal state. After this call, the systems pipeline is fully reset
            /// and can be re-created with <see cref="Create"/>.
            /// <para>
            /// Can be called in both <see cref="SystemsStatus.Created"/> and <see cref="SystemsStatus.Initialized"/> states.
            /// In <see cref="SystemsStatus.Created"/> state, no <see cref="ISystem.Destroy"/> calls are made
            /// (systems were never initialized).
            /// </para>
            /// </summary>
            public static void Destroy() {
                #if FFS_ECS_DEBUG
                if (Status == SystemsStatus.NotCreated) {
                    throw new StaticEcsException($"Systems<{typeof(TSystemsType)}>, Method: Destroy, systems pipeline is not created.");
                }
                #endif

                if (Status == SystemsStatus.Initialized) {
                    for (var i = 0; i < AllSystemsCount; i++) {
                        if (AllSystems[i].HasDestroy) {
                            AllSystems[i].System.Destroy();
                        }
                    }
                }

                ResourcesData<TSystemsType>.Instance.Clear();
                NamedResources<TSystemsType>.Clear();
                Handle = default;

                #if FFS_ECS_DEBUG
                UpdateSystemsAllIndex = default;
                Stopwatch = default;
                #endif

                UpdateSystems = default;
                UpdateSystemLastTicks = default;
                AllSystems = default;
                AllSystemsCount = default;
                UpdateSystemsCount = default;
                Status = SystemsStatus.NotCreated;

                for (var i = 0; i < Data.Instance.RegisteredSystems.Count; i++) {
                    if (Data.Instance.RegisteredSystems[i].Guid == Guid) {
                        Data.Instance.RegisteredSystems.RemoveAt(i);
                        break;
                    }
                }
                Guid = default;
            }

            internal static void WriteSnapshot(ref BinaryPackWriter writer) {
                ResourcesData<TSystemsType>.Instance.WriteSnapshot(ref writer);

                var countPos = writer.MakePoint(sizeof(ushort));
                ushort serializableCount = 0;
                for (var i = 0; i < AllSystemsCount; i++) {
                    ref var data = ref AllSystems[i];
                    if (data.Guid == null) continue;
                    writer.WriteGuid(data.Guid.Value);
                    writer.WriteByte(data.Version);
                    var sizePos = writer.MakePoint(sizeof(uint));
                    data.System.Write(ref writer);
                    writer.WriteUintAt(sizePos, writer.Position - (sizePos + sizeof(uint)));
                    serializableCount++;
                }
                writer.WriteUshortAt(countPos, serializableCount);
            }

            internal static void ReadSnapshot(ref BinaryPackReader reader) {
                ResourcesData<TSystemsType>.Instance.ReadSnapshot(ref reader);

                var systemCount = reader.ReadUshort();
                for (var i = 0; i < systemCount; i++) {
                    var guid = reader.ReadGuid();
                    var version = reader.ReadByte();
                    var size = reader.ReadUint();

                    var found = -1;
                    for (var j = 0; j < AllSystemsCount; j++) {
                        if (AllSystems[j].Guid == guid) {
                            found = j;
                            break;
                        }
                    }

                    if (found >= 0) {
                        var endPos = reader.Position + size;
                        AllSystems[found].System.Read(ref reader, version);
                        reader.Position = endPos;
                    } else {
                        reader.SkipNext(size);
                    }
                }
            }

            /// <summary>
            /// Registers a system instance for this pipeline. Must be called after <see cref="Create"/>
            /// and before <see cref="Initialize"/>.
            /// <para>
            /// The <paramref name="order"/> parameter controls execution priority during <see cref="Initialize"/>
            /// and <see cref="Update"/>: lower values run first. Systems with the same order run in
            /// registration order. Use negative values for early systems (e.g., input), positive for late
            /// systems (e.g., rendering).
            /// </para>
            /// </summary>
            /// <typeparam name="TSystem">Concrete system type implementing <see cref="ISystem"/>.</typeparam>
            /// <param name="system">The system instance to register.</param>
            /// <param name="order">Execution priority. Lower values execute first. Default is 0.</param>
            [MethodImpl(AggressiveInlining)]
            public static SystemsRegistrar<TSystemsType> Add<
                #if NET5_0_OR_GREATER
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
                #endif
                TSystem>(TSystem system, short order = 0) where TSystem : ISystem {
                #if FFS_ECS_DEBUG
                if (Status != SystemsStatus.Created) {
                    throw new StaticEcsException($"Systems<{typeof(TSystemsType)}>, Method: Add, systems pipeline must be in Created state (current: {Status}).");
                }
                #endif
                if (AllSystemsCount == AllSystems.Length) {
                    Array.Resize(ref AllSystems, AllSystemsCount << 1);
                }

                ISystem boxedSystem = system;
                var data = new SystemData {
                    Order = order,
                    System = boxedSystem,
                    Index = AllSystemsCount,
                    HasDestroy = SystemType<TSystem>.HasDestroy(),
                    HasInit = SystemType<TSystem>.HasInit(),
                    HasUpdate = SystemType<TSystem>.HasUpdate(),
                    HasUpdateIsActive = SystemType<TSystem>.HasUpdateIsActive()
                };

                var guid = boxedSystem.Guid();
                if (guid != null && guid.Value != Guid.Empty) {
                    #if FFS_ECS_DEBUG
                    if (!SystemType<TSystem>.HasWrite() || !SystemType<TSystem>.HasRead()) {
                        throw new StaticEcsException(
                            $"Systems<{typeof(TSystemsType)}>: system `{typeof(TSystem)}` declares Guid but does not implement both Write and Read. " +
                            $"Override both methods to enable snapshot serialization.");
                    }
                    for (var k = 0; k < AllSystemsCount; k++) {
                        if (AllSystems[k].Guid == guid) {
                            throw new StaticEcsException($"Systems<{typeof(TSystemsType)}>: duplicate system Guid {guid.Value} (types `{AllSystems[k].System.GetType()}` and `{typeof(TSystem)}`)");
                        }
                    }
                    #endif
                    data.Guid = guid.Value;
                    data.Version = boxedSystem.Version();
                }

                AllSystems[AllSystemsCount] = data;
                AllSystemsCount++;
                return default;
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
            #endif
            /// <summary>
            /// Lightweight handle for accessing a singleton resource of type <typeparamref name="T"/>
            /// scoped to the systems pipeline <see cref="Systems{TSystemsType}"/>.
            /// <para>
            /// Functionally identical to <see cref="World{TWorld}.Resource{T}"/>, but the underlying
            /// storage is keyed by <typeparamref name="TSystemsType"/> rather than the world.
            /// Such resources are automatically cleared when the systems pipeline is destroyed
            /// via <see cref="Destroy"/>, decoupled from the world's own resource lifecycle.
            /// </para>
            /// </summary>
            /// <typeparam name="T">The resource type.</typeparam>
            public readonly struct Resource<T> where T : IResource {
                /// <summary>
                /// Whether a resource of type <typeparamref name="T"/> has been registered for this systems pipeline.
                /// </summary>
                public bool IsRegistered {
                    [MethodImpl(AggressiveInlining)]
                    get => Resources<TSystemsType, T>.IsRegistered;
                }

                /// <summary>
                /// Returns a direct reference to the stored resource value.
                /// </summary>
                public ref T Value {
                    [MethodImpl(AggressiveInlining)]
                    get => ref Resources<TSystemsType, T>.Value;
                }

                /// <summary>
                /// Sets (or replaces) the resource value. Equivalent to
                /// <see cref="Systems{TSystemsType}.SetResource{TResource}(TResource, bool)"/>.
                /// </summary>
                /// <param name="clearOnDestroy">
                /// If <c>true</c> (default), the resource is cleared on
                /// <see cref="Systems{TSystemsType}.Destroy"/>. Only applied on first registration.
                /// </param>
                [MethodImpl(AggressiveInlining)]
                public void Set(T value, bool clearOnDestroy = true) => Resources<TSystemsType, T>.Set(value, clearOnDestroy);

                /// <summary>
                /// Removes the resource of type <typeparamref name="T"/> from this systems pipeline.
                /// </summary>
                [MethodImpl(AggressiveInlining)]
                public void Remove() => Resources<TSystemsType, T>.Remove();
            }

            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
            #endif
            /// <summary>
            /// Handle for accessing a keyed resource scoped to <see cref="Systems{TSystemsType}"/>.
            /// Functionally identical to <see cref="World{TWorld}.NamedResource{T}"/>, but the
            /// underlying dictionary is per-systems-pipeline and is cleared on <see cref="Destroy"/>.
            /// <para>
            /// <b>Warning:</b> mutable struct that caches an internal box reference on first
            /// <see cref="Value"/> access. Do not store in a <c>readonly</c> field.
            /// </para>
            /// </summary>
            /// <typeparam name="T">The resource value type.</typeparam>
            public struct NamedResource<T> where T : IResource {
                /// <summary>The string key identifying this resource.</summary>
                public readonly string Key;
                private NamedResources<TSystemsType>.BoxBase _cache;

                /// <summary>Creates a keyed resource handle bound to the specified key.</summary>
                [MethodImpl(AggressiveInlining)]
                public NamedResource(string key) {
                    Key = key;
                    _cache = null;
                }

                /// <summary>Whether a resource with this key is currently registered.</summary>
                public bool IsRegistered {
                    [MethodImpl(AggressiveInlining)]
                    get => NamedResources<TSystemsType>.Has(Key);
                }

                /// <summary>Returns a direct reference to the stored resource value.</summary>
                public ref T Value {
                    [MethodImpl(AggressiveInlining)]
                    get {
                        if (_cache == null || !_cache.IsValid) {
                            if (!NamedResources<TSystemsType>.Values.TryGetValue(Key, out var boxObj)) {
                                throw new StaticEcsException($"NamedResource<{typeof(T).Name}> with key '{Key}' not found in Systems<{typeof(TSystemsType).Name}> of World<{typeof(TWorld).Name}>");
                            }
                            _cache = (NamedResources<TSystemsType>.BoxBase)boxObj;
                        }
                        return ref ((NamedResources<TSystemsType>.Box<T>)_cache).Value;
                    }
                }

                /// <summary>
                /// Sets (or replaces) the value for this resource's <see cref="Key"/>.
                /// </summary>
                /// <param name="clearOnDestroy">
                /// If <c>true</c> (default), the resource is cleared on
                /// <see cref="Systems{TSystemsType}.Destroy"/>.
                /// </param>
                [MethodImpl(AggressiveInlining)]
                public void Set(T value, bool clearOnDestroy = true) => NamedResources<TSystemsType>.Set(Key, value, clearOnDestroy);

                /// <summary>
                /// Removes the resource bound to <see cref="Key"/> from this systems pipeline.
                /// </summary>
                [MethodImpl(AggressiveInlining)]
                public void Remove() {
                    NamedResources<TSystemsType>.Remove(Key);
                    _cache = null;
                }
            }

            /// <summary>
            /// Checks whether a singleton resource of the given type is registered for this systems pipeline.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static bool HasResource<TResource>() where TResource : IResource {
                return Resources<TSystemsType, TResource>.Has();
            }

            /// <summary>
            /// Checks whether a keyed resource exists for this systems pipeline.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static bool HasResource<TResource>(string key) where TResource : IResource {
                return NamedResources<TSystemsType>.Has(key);
            }

            /// <summary>
            /// Returns a reference to the singleton resource of the given type for this systems pipeline.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static ref TResource GetResource<TResource>() where TResource : IResource {
                return ref Resources<TSystemsType, TResource>.Value;
            }

            /// <summary>
            /// Returns a reference to a keyed resource for this systems pipeline.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static ref TResource GetResource<TResource>(string key) where TResource : IResource {
                return ref NamedResources<TSystemsType>.Get<TResource>(key);
            }

            /// <summary>
            /// Sets (or replaces) the singleton resource of the given type for this systems pipeline.
            /// </summary>
            /// <param name="value">The resource value to store.</param>
            /// <param name="clearOnDestroy">
            /// If <c>true</c> (default), the resource is automatically cleared when the systems pipeline
            /// is destroyed via <see cref="Destroy"/>. Only applied on first registration.
            /// </param>
            [MethodImpl(AggressiveInlining)]
            public static void SetResource<TResource>(TResource value, bool clearOnDestroy = true) where TResource : IResource {
                Resources<TSystemsType, TResource>.Set(value, clearOnDestroy);
            }

            /// <summary>
            /// Sets (or replaces) a keyed resource for this systems pipeline.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static void SetResource<TResource>(string key, TResource value, bool clearOnDestroy = true) where TResource : IResource {
                NamedResources<TSystemsType>.Set(key, value, clearOnDestroy);
            }

            /// <summary>
            /// Removes the singleton resource of the given type from this systems pipeline.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static void RemoveResource<TResource>() where TResource : IResource {
                Resources<TSystemsType, TResource>.Remove();
            }

            /// <summary>
            /// Removes a keyed resource from this systems pipeline.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static void RemoveResource(string key) {
                NamedResources<TSystemsType>.Remove(key);
            }

            #region INTERNAL_HANDLE_PROXIES
            [MethodImpl(AggressiveInlining)]
            internal static SystemsStatus _Status() => Status;

            [MethodImpl(AggressiveInlining)]
            internal static bool _HasResource(Type type) => ResourcesData<TSystemsType>.Instance.HasRaw(type);

            [MethodImpl(AggressiveInlining)]
            internal static bool _HasResourceByKey(string key) => NamedResources<TSystemsType>.Has(key);

            [MethodImpl(AggressiveInlining)]
            internal static IResource _GetResource(Type type) => ResourcesData<TSystemsType>.Instance.GetRaw(type);

            [MethodImpl(AggressiveInlining)]
            internal static IResource _GetResourceByKey(string key) => NamedResources<TSystemsType>.GetRaw(key);

            [MethodImpl(AggressiveInlining)]
            internal static void _RemoveResource(Type type) => ResourcesData<TSystemsType>.Instance.RemoveRaw(type);

            [MethodImpl(AggressiveInlining)]
            internal static void _RemoveResourceByKey(string key) => NamedResources<TSystemsType>.Remove(key);

            [MethodImpl(AggressiveInlining)]
            internal static void _SetResource(Type type, IResource value, bool clearOnDestroy) => ResourcesData<TSystemsType>.Instance.SetRaw(type, value, clearOnDestroy);

            [MethodImpl(AggressiveInlining)]
            internal static void _SetResourceByKey(string key, IResource value, bool clearOnDestroy) => NamedResources<TSystemsType>.SetRaw(key, value);

            [MethodImpl(AggressiveInlining)]
            internal static IReadOnlyCollection<string> _GetAllResourcesKeys() => NamedResources<TSystemsType>.Values.Keys;

            [MethodImpl(AggressiveInlining)]
            internal static IReadOnlyCollection<Type> _GetAllResourcesTypes() => ResourcesData<TSystemsType>.Instance.GetAllGetSetRemoveValuesMethods().Keys;

            [MethodImpl(AggressiveInlining)]
            internal static Span<SystemData> _GetAllSystems() => new(AllSystems, 0, AllSystemsCount);
            #endregion
        }

        /// <summary>
        /// Fluent builder for registering systems in a <see cref="Systems{TSystemsType}"/> pipeline.
        /// Obtained via <see cref="Systems{TSystemsType}.Add{TSystem}"/>. Each method returns <c>this</c> for chaining.
        /// <para>
        /// Example: <c>Systems&lt;S&gt;.Add(new InputSystem(), -10).Add(new PhysicsSystem()).Add(new RenderSystem(), 100);</c>
        /// </para>
        /// </summary>
        /// <typeparam name="TSystemsType">Systems group identity type.</typeparam>
        public readonly struct SystemsRegistrar<TSystemsType> where TSystemsType : struct, ISystemsType {

            /// <inheritdoc cref="Systems{TSystemsType}.Add{TSystem}"/>
            [MethodImpl(AggressiveInlining)]
            public SystemsRegistrar<TSystemsType> Add<
                #if NET5_0_OR_GREATER
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
                #endif
                TSystem>(TSystem system, short order = 0) where TSystem : ISystem {
                Systems<TSystemsType>.Add(system, order);
                return this;
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    [Il2CppEagerStaticClassConstruction]
    #endif
    internal static class SystemType<
        #if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
        #endif
        T> where T : ISystem {
        private static readonly Type[] WriteParams = { typeof(BinaryPackWriter).MakeByRefType() };
        private static readonly Type[] ReadParams = { typeof(BinaryPackReader).MakeByRefType(), typeof(byte) };

        internal static bool HasUpdate() {
            return HasMethodInHierarchy(typeof(T), nameof(ISystem.Update), Array.Empty<Type>());
        }

        internal static bool HasInit() {
            return HasMethodInHierarchy(typeof(T), nameof(ISystem.Init), Array.Empty<Type>());
        }

        internal static bool HasDestroy() {
            return HasMethodInHierarchy(typeof(T), nameof(ISystem.Destroy), Array.Empty<Type>());
        }

        internal static bool HasUpdateIsActive() {
            return HasMethodInHierarchy(typeof(T), nameof(ISystem.UpdateIsActive), Array.Empty<Type>());
        }

        internal static bool HasWrite() {
            return HasMethodInHierarchy(typeof(T), nameof(ISystem.Write), WriteParams);
        }

        internal static bool HasRead() {
            return HasMethodInHierarchy(typeof(T), nameof(ISystem.Read), ReadParams);
        }
        
        private static bool HasMethodInHierarchy(
#if NET5_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
            Type type,
            string methodName,
            Type[] parameterTypes
        ) {
            while (type != null && type != typeof(object)) {
                if (HasMethod(type, methodName, parameterTypes))
                    return true;

                type = type.BaseType;
            }

            return false;
        }

        private static bool HasMethod(
            #if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
            #endif
            Type structType, string methodName, Type[] parameterTypes) {
            return structType.GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly,
                binder: null,
                types: parameterTypes,
                modifiers: null
            ) != null;
        }
    }

    /// <summary>
    /// Type-erased handle for accessing the resources of a specific
    /// <see cref="World{TWorld}.Systems{TSystemsType}"/> pipeline without knowing
    /// <c>TWorld</c> or <c>TSystemsType</c> at compile time. Mirrors the resources
    /// portion of <see cref="WorldHandle"/> but is scoped to one systems group.
    /// <para>
    /// <b>Primary use cases:</b> editor/inspector tools, debug visualizers, and
    /// serialization frameworks that need to read or modify systems-pipeline
    /// resources generically. All operations go through <c>delegate*</c> function
    /// pointers captured at handle creation, so overhead is minimal.
    /// </para>
    /// <para>
    /// Obtain a <see cref="SystemsHandle"/> via <c>World&lt;TWorld&gt;.Systems&lt;TSystemsType&gt;.Handle</c>.
    /// The handle is initialized when the systems pipeline is created and reset on destroy.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct SystemsHandle {
        private readonly unsafe delegate*<SystemsStatus> _status;
        private readonly unsafe delegate*<Type, bool> _hasResource;
        private readonly unsafe delegate*<Type, IResource, bool, void> _setResource;
        private readonly unsafe delegate*<Type, IResource> _getResource;
        private readonly unsafe delegate*<Type, void> _removeResource;
        private readonly unsafe delegate*<string, bool> _hasResourceByKey;
        private readonly unsafe delegate*<string, IResource> _getResourceByKey;
        private readonly unsafe delegate*<string, IResource, bool, void> _setResourceByKey;
        private readonly unsafe delegate*<string, void> _removeResourceByKey;
        private readonly unsafe delegate*<IReadOnlyCollection<string>> _getAllResourcesKeys;
        private readonly unsafe delegate*<IReadOnlyCollection<Type>> _getAllResourcesTypes;
        private readonly unsafe delegate*<ref BinaryPackWriter, void> _writeSnapshot;
        private readonly unsafe delegate*<ref BinaryPackReader, void> _readSnapshot;
        private readonly unsafe delegate*<Span<SystemData>> _getAllSystems;

        /// <summary>The <see cref="Type"/> of the <c>TWorld</c> struct that owns this systems pipeline.</summary>
        public readonly Type WorldType;

        /// <summary>The <see cref="Type"/> of the <c>TSystemsType</c> struct identifying this pipeline.</summary>
        public readonly Type SystemsType;

        /// <summary>
        /// Stable identifier for this systems pipeline used to match snapshot entries on load.
        /// Defaults to a deterministic GUID derived from <see cref="SystemsType"/>; can be overridden
        /// at <c>Create</c> time.
        /// </summary>
        public readonly Guid Guid;

        internal static unsafe SystemsHandle Create<TWorld, TSystemsType>(Guid guid)
            where TWorld : struct, IWorldType
            where TSystemsType : struct, ISystemsType {
            return new SystemsHandle(
                typeof(TWorld),
                typeof(TSystemsType),
                guid,
                &World<TWorld>.Systems<TSystemsType>._Status,
                &World<TWorld>.Systems<TSystemsType>._HasResource,
                &World<TWorld>.Systems<TSystemsType>._SetResource,
                &World<TWorld>.Systems<TSystemsType>._GetResource,
                &World<TWorld>.Systems<TSystemsType>._RemoveResource,
                &World<TWorld>.Systems<TSystemsType>._HasResourceByKey,
                &World<TWorld>.Systems<TSystemsType>._GetResourceByKey,
                &World<TWorld>.Systems<TSystemsType>._SetResourceByKey,
                &World<TWorld>.Systems<TSystemsType>._RemoveResourceByKey,
                &World<TWorld>.Systems<TSystemsType>._GetAllResourcesKeys,
                &World<TWorld>.Systems<TSystemsType>._GetAllResourcesTypes,
                &World<TWorld>.Systems<TSystemsType>.WriteSnapshot,
                &World<TWorld>.Systems<TSystemsType>.ReadSnapshot,
                &World<TWorld>.Systems<TSystemsType>._GetAllSystems
            );
        }

        internal unsafe SystemsHandle(
            Type worldType,
            Type systemsType,
            Guid guid,
            delegate*<SystemsStatus> status,
            delegate*<Type, bool> hasResource,
            delegate*<Type, IResource, bool, void> setResource,
            delegate*<Type, IResource> getResource,
            delegate*<Type, void> removeResource,
            delegate*<string, bool> hasResourceByKey,
            delegate*<string, IResource> getResourceByKey,
            delegate*<string, IResource, bool, void> setResourceByKey,
            delegate*<string, void> removeResourceByKey,
            delegate*<IReadOnlyCollection<string>> getAllResourcesKeys,
            delegate*<IReadOnlyCollection<Type>> getAllResourcesTypes,
            delegate*<ref BinaryPackWriter, void> writeSnapshot,
            delegate*<ref BinaryPackReader, void> readSnapshot,
            delegate*<Span<SystemData>> getAllSystems) {
            WorldType = worldType;
            SystemsType = systemsType;
            Guid = guid;
            _status = status;
            _hasResource = hasResource;
            _setResource = setResource;
            _getResource = getResource;
            _removeResource = removeResource;
            _hasResourceByKey = hasResourceByKey;
            _getResourceByKey = getResourceByKey;
            _setResourceByKey = setResourceByKey;
            _removeResourceByKey = removeResourceByKey;
            _getAllResourcesKeys = getAllResourcesKeys;
            _getAllResourcesTypes = getAllResourcesTypes;
            _writeSnapshot = writeSnapshot;
            _readSnapshot = readSnapshot;
            _getAllSystems = getAllSystems;
        }

        [MethodImpl(AggressiveInlining)]
        internal void WriteSnapshot(ref BinaryPackWriter writer) {
            unsafe { _writeSnapshot(ref writer); }
        }

        [MethodImpl(AggressiveInlining)]
        internal void ReadSnapshot(ref BinaryPackReader reader) {
            unsafe { _readSnapshot(ref reader); }
        }

        [MethodImpl(AggressiveInlining)]
        internal Span<SystemData> GetAllSystems() {
            unsafe { return _getAllSystems(); }
        }

        /// <summary>Whether this handle was initialized for an existing systems pipeline.</summary>
        public unsafe bool IsValid {
            [MethodImpl(AggressiveInlining)]
            get => _status != null;
        }

        /// <summary>Current lifecycle state of the systems pipeline.</summary>
        [MethodImpl(AggressiveInlining)]
        public SystemsStatus Status() {
            unsafe { return _status(); }
        }

        #region RESOURCES
        /// <summary>
        /// Checks whether a singleton resource of the given runtime <see cref="Type"/> exists in this systems pipeline.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public bool HasResource(Type type) {
            unsafe { return _hasResource(type); }
        }

        /// <summary>
        /// Checks whether a keyed resource with the given string key exists in this systems pipeline.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public bool HasResource(string key) {
            unsafe { return _hasResourceByKey(key); }
        }

        /// <summary>
        /// Returns the singleton resource of the given runtime type as <see cref="IResource"/>.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public IResource GetResource(Type type) {
            unsafe { return _getResource(type); }
        }

        /// <summary>
        /// Returns a keyed resource as <see cref="IResource"/>.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public IResource GetResource(string key) {
            unsafe { return _getResourceByKey(key); }
        }

        /// <summary>
        /// Removes the singleton resource of the given runtime type from this systems pipeline.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public void RemoveResource(Type type) {
            unsafe { _removeResource(type); }
        }

        /// <summary>
        /// Removes a keyed resource from this systems pipeline.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public void RemoveResource(string key) {
            unsafe { _removeResourceByKey(key); }
        }

        /// <summary>
        /// Sets (or replaces) a singleton resource. The resource type must already be registered
        /// for this systems pipeline via the typed <c>SetResource&lt;T&gt;</c> API.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public void SetResource(Type type, IResource value, bool clearOnDestroy) {
            unsafe { _setResource(type, value, clearOnDestroy); }
        }

        /// <summary>
        /// Sets (or replaces) a keyed resource. The key must already be registered
        /// for this systems pipeline via the typed <c>SetResource&lt;T&gt;(key, ...)</c> API.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public void SetResource(string key, IResource value, bool clearOnDestroy) {
            unsafe { _setResourceByKey(key, value, clearOnDestroy); }
        }

        /// <summary>Returns all string keys of currently stored keyed resources.</summary>
        [MethodImpl(AggressiveInlining)]
        public IReadOnlyCollection<string> GetAllResourcesKeys() {
            unsafe { return _getAllResourcesKeys(); }
        }

        /// <summary>Returns the <see cref="Type"/> objects of all currently stored singleton resources.</summary>
        [MethodImpl(AggressiveInlining)]
        public IReadOnlyCollection<Type> GetAllResourcesTypes() {
            unsafe { return _getAllResourcesTypes(); }
        }
        #endregion
    }

}
