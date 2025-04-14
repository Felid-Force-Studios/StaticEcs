using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if DEBUG || FFS_ECS_ENABLE_DEBUG
using System.Diagnostics;
#endif
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
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        internal void Info(List<SystemInfo> res);

        internal bool SetActive(int sysIdx, bool active);
        #endif
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
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SW<TSystem>
                where TSystem : IUpdateSystem {
                internal TSystem System;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                internal bool Active;
                internal float Time;
                internal Stopwatch stopwatch;
                internal bool initSystem;
                internal bool destroySystem;
                #endif
            
                public SW(TSystem system) {
                    System = system;
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    Active = true;
                    Time = 0;
                    stopwatch = new Stopwatch();
                    initSystem = System is IInitSystem;
                    destroySystem = System is IDestroySystem;
                    #endif
                }

                [MethodImpl(AggressiveInlining)]
                public void Init() {
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (!Active) {
                        return;
                    }
                    #endif

                    if (System is IInitSystem system) {
                        #if DEBUG || FFS_ECS_ENABLE_DEBUG
                        if (FileLogger != null && FileLogger.Enabled) {
                            FileLogger.Write(OperationType.SystemCallInit, TypeData<TSystem>.Name);
                        }
                        #endif

                        system.Init();
                        System = (TSystem) system;
                    }
                }

                [MethodImpl(AggressiveInlining)]
                public void Run() {
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (!Active) {
                        IncrementSystemIndex();
                        return;
                    }
                    if (FileLogger != null && FileLogger.Enabled) {
                        FileLogger.Write(OperationType.SystemCallUpdate, TypeData<TSystem>.Name);
                    }
                    stopwatch.Restart();
                    #endif
                    System.Update();
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    stopwatch.Stop();
                    Time = ((float)stopwatch.ElapsedTicks / Stopwatch.Frequency * 1000 + Time) * 0.5f;
                    #endif
                    IncrementSystemIndex();
                }

                [MethodImpl(AggressiveInlining)]
                public void Destroy() {
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    if (!Active) {
                        return;
                    }
                    #endif
                    if (System is IDestroySystem system) {
                        #if DEBUG || FFS_ECS_ENABLE_DEBUG
                        if (FileLogger != null && FileLogger.Enabled) {
                            FileLogger.Write(OperationType.SystemCallDestroy, TypeData<TSystem>.Name);
                        }
                        #endif
                        system.Destroy();
                        System = (TSystem) system;
                    }
                }
                
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
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
                #endif
            }
        }
    }
    
    #if DEBUG || FFS_ECS_ENABLE_DEBUG
    internal struct SystemInfo {
        public System.Type SystemType;
        public float AvgUpdateTime;
        public bool Enabled;
        public bool InitSystem;
        public bool DestroySystem;
    }
    #endif
}