#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

#if FFS_ECS_DEBUG
using System.Diagnostics;
using System.Collections.Generic;
#endif
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    public interface ISystem { }
    public interface ICallOnceSystem : ISystem { }

    public interface IInitSystem : ICallOnceSystem {
        public void Init();
    }

    public interface IUpdateSystem : ISystem {
        public void Update();
    }

    public interface IDestroySystem : ICallOnceSystem {
        public void Destroy();
    }
    
    internal interface ISystemsBatch : ISystem {
        internal void Update();
        internal void Init();
        internal void Destroy();
        
        #if FFS_ECS_DEBUG
        internal void Info(List<SystemInfo> res);

        internal bool SetActive(int sysIdx, bool active);
        #endif
    }
    
    public interface ISystemState {
        public bool IsActive();
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TrueState : ISystemState {

        [MethodImpl(AggressiveInlining)]
        public bool IsActive() => true;
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> where WorldType : struct, IWorldType {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public abstract partial class Systems<SysType> {
            
            [MethodImpl(AggressiveInlining)]
            public static void InitSys<TSystem>(ref TSystem sys) where TSystem : IUpdateSystem {
                if (sys is IInitSystem system) {
                    #if FFS_ECS_DEBUG
                    if (FileLogger != null && FileLogger.Enabled) {
                        FileLogger.Write(OperationType.SystemCallInit, TypeData<TSystem>.Name);
                    }
                    #endif

                    system.Init();
                    sys = (TSystem) system;
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void DestroySys<TSystem>(ref TSystem sys) where TSystem : IUpdateSystem {
                if (sys is IDestroySystem system) {
                    #if FFS_ECS_DEBUG
                    if (FileLogger != null && FileLogger.Enabled) {
                        FileLogger.Write(OperationType.SystemCallDestroy, TypeData<TSystem>.Name);
                    }
                    #endif
                    system.Destroy();
                    sys = (TSystem) system;
                }
            }
            
            #if FFS_ECS_DEBUG
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SW<TSystem>
                where TSystem : IUpdateSystem {
                internal TSystem System;
                internal bool Active;
                internal float Time;
                internal Stopwatch stopwatch;
                internal bool initSystem;
                internal bool destroySystem;
            
                public SW(TSystem system) {
                    System = system;
                    Active = true;
                    Time = 0;
                    stopwatch = new Stopwatch();
                    initSystem = System is IInitSystem;
                    destroySystem = System is IDestroySystem;
                }

                [MethodImpl(AggressiveInlining)]
                public void Update() {
                    if (!Active) {
                        return;
                    }
                    if (FileLogger != null && FileLogger.Enabled) {
                        FileLogger.Write(OperationType.SystemCallUpdate, TypeData<TSystem>.Name);
                    }
                    stopwatch.Restart();
                    System.Update();
                    stopwatch.Stop();
                    Time = ((float)stopwatch.ElapsedTicks / Stopwatch.Frequency * 1000 + Time) * 0.5f;
                }
                
                [MethodImpl(AggressiveInlining)]
                public void Init() {
                    InitSys(ref System);
                }
                
                [MethodImpl(AggressiveInlining)]
                public void Destroy() {
                    DestroySys(ref System);
                }
                
                internal SystemInfo Info() {
                    return new SystemInfo {
                        SystemType = typeof(TSystem),
                        Enabled = Active,
                        AvgUpdateTime = Time,
                        InitSystem = initSystem,
                        DestroySystem = destroySystem
                    };
                }
                
                internal bool SetActive(bool val) {
                    Active = val;
                    return true;
                }
            }
            #endif
        }
    }
    
    #if FFS_ECS_DEBUG
    internal struct SystemInfo {
        public System.Type SystemType;
        public float AvgUpdateTime;
        public bool Enabled;
        public bool InitSystem;
        public bool DestroySystem;
    }
    #endif
}