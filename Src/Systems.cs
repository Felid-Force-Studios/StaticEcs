#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
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
    }

    internal enum SystemsStatus : byte {
        NotCreated,
        Created,
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
        #if FFS_ECS_DEBUG
        public float AvgUpdateTime;
        public bool DebugDisabled;
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
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
            [MethodImpl(AggressiveInlining)]
            public static void Create(uint baseSize = 64) {
                #if FFS_ECS_DEBUG
                if (Status != SystemsStatus.NotCreated) {
                    throw new StaticEcsException($"Systems<{typeof(TSystemsType)}>, Method: Create, systems pipeline already created.");
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
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
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
                AllSystems[AllSystemsCount] = new SystemData {
                    Order = order,
                    System = system,
                    Index = AllSystemsCount,
                    HasDestroy = SystemType<TSystem>.HasDestroy(),
                    HasInit = SystemType<TSystem>.HasInit(),
                    HasUpdate = SystemType<TSystem>.HasUpdate(),
                    HasUpdateIsActive = SystemType<TSystem>.HasUpdateIsActive()
                };
                AllSystemsCount++;
                return default;
            }
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
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
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
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
        #endif
        T> where T : ISystem {
        internal static bool HasUpdate() {
            return HasMethod(typeof(T), nameof(ISystem.Update), Array.Empty<Type>());
        }

        internal static bool HasInit() {
            return HasMethod(typeof(T), nameof(ISystem.Init), Array.Empty<Type>());
        }

        internal static bool HasDestroy() {
            return HasMethod(typeof(T), nameof(ISystem.Destroy), Array.Empty<Type>());
        }

        internal static bool HasUpdateIsActive() {
            return HasMethod(typeof(T), nameof(ISystem.UpdateIsActive), Array.Empty<Type>());
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
}
