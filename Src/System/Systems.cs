using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    public interface ISystemsType { }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class Ecs<WorldType> where WorldType : struct, IWorldType {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public abstract partial class Systems<SysType> where SysType : struct, ISystemsType {
            internal static ISystemsBatch[] _batchSystems;
            internal static (ISystem system, short order, int index)[] _allSystems;
            internal static int _allSystemsCount;
            internal static int _batchSystemsCount;

            internal static uint _currentSystemIndex;
            internal static uint _updateSystemsCount;

            [MethodImpl(AggressiveInlining)]
            public static void Create(uint baseSize = 64) {
                _batchSystems = new ISystemsBatch[baseSize];
                _allSystems = new (ISystem, short, int)[baseSize];
            }

            [MethodImpl(AggressiveInlining)]
            public static void AddCallOnce<S>(S system, short order = 0) where S : ICallOnceSystem {
                EnsureSize();
                _allSystems[_allSystemsCount] = (system, order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<S01>(S01 s01, short order = 0) where S01 : IUpdateSystem {
                EnsureSize();
                _updateSystemsCount += 1;
                _allSystems[_allSystemsCount] = (new SystemsBatch<S01>(s01), order, _allSystemsCount);
                _allSystemsCount++;
            }

            [MethodImpl(AggressiveInlining)]
            public static void Initialize() {
                Array.Sort(_allSystems, 0, _allSystemsCount, Comparer<(ISystem, short order, int index)>.Create(
                    (a, b) => a.order != b.order 
                        ? a.order.CompareTo(b.order) 
                        : a.index.CompareTo(b.index)
                ));
                
                for (var i = 0; i < _allSystemsCount; i++) {
                    var system = _allSystems[i].system;
                    if (system is IInitSystem initSystem) {
                        initSystem.Init();
                    }
                    
                    if (system is ISystemsBatch batch) {
                        batch.Init();
                        
                        if (_batchSystemsCount == _batchSystems.Length) {
                            Array.Resize(ref _batchSystems, _batchSystemsCount << 1);
                        }

                        _batchSystems[_batchSystemsCount++] = batch;
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void Update() {
                for (var i = 0; i < _batchSystemsCount; i++) {
                    _batchSystems[i].Update();
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void Destroy() {
                for (var i = 0; i < _allSystemsCount; i++) {
                    var system = _allSystems[i].system;
                    if (system is IDestroySystem destroySystem) {
                        destroySystem.Destroy();
                    }
                    if (system is ISystemsBatch batch) {
                        batch.Destroy();
                    }
                }
                
                _batchSystems = default;
                _allSystems = default;
                _allSystemsCount = default;
                _batchSystemsCount = default;
                _currentSystemIndex = default;
                _updateSystemsCount = default;
            }

            [MethodImpl(AggressiveInlining)]
            public static int GetCurrentUpdateSystemIndex() {
                return (int) (_currentSystemIndex % _updateSystemsCount);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static uint GetUpdateSystemsCount() {
                return _updateSystemsCount;
            }

            [MethodImpl(AggressiveInlining)]
            internal static void IncrementSystemIndex() {
                _currentSystemIndex++;
            }
            
            private static void EnsureSize() {
                if (_allSystemsCount == _allSystems.Length) {
                    Array.Resize(ref _allSystems, _allSystemsCount << 1);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02
            >(
                S01 s01, S02 s02,
                short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 2;
                _allSystems[_allSystemsCount] = (new SystemsBatch<S01, S02>(s01, s02), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03
            >(
                S01 s01, S02 s02, S03 s03,
                short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 3;
                _allSystems[_allSystemsCount] = (new SystemsBatch<S01, S02, S03>(s01, s02, s03), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03, S04
            >(
                S01 s01, S02 s02, S03 s03, S04 s04,
                short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem where S04 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 4;
                _allSystems[_allSystemsCount] = (new SystemsBatch<S01, S02, S03, S04>(s01, s02, s03, s04), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03, S04, S05
            >(
                S01 s01, S02 s02, S03 s03, S04 s04, S05 s05,
                short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem where S04 : IUpdateSystem where S05 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 5;
                _allSystems[_allSystemsCount] = (new SystemsBatch<S01, S02, S03, S04, S05>(s01, s02, s03, s04, s05), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03, S04, S05, S06
            >(
                S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06,
                short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem where S04 : IUpdateSystem where S05 : IUpdateSystem where S06 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 6;
                _allSystems[_allSystemsCount] = (new SystemsBatch<S01, S02, S03, S04, S05, S06>(s01, s02, s03, s04, s05, s06), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03, S04, S05, S06, S07
            >(
                S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07,
                short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem where S04 : IUpdateSystem where S05 : IUpdateSystem where S06 : IUpdateSystem where S07 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 7;
                _allSystems[_allSystemsCount] = (new SystemsBatch<S01, S02, S03, S04, S05, S06, S07>(s01, s02, s03, s04, s05, s06, s07), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03, S04, S05, S06, S07, S08
            >(
                S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem where S04 : IUpdateSystem where S05 : IUpdateSystem where S06 : IUpdateSystem where S07 : IUpdateSystem where S08 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 8;
                _allSystems[_allSystemsCount] = (new SystemsBatch<S01, S02, S03, S04, S05, S06, S07, S08>(s01, s02, s03, s04, s05, s06, s07, s08), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03, S04, S05, S06, S07, S08,
                S09
            >(
                S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                S09 s09,
                short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem where S04 : IUpdateSystem where S05 : IUpdateSystem where S06 : IUpdateSystem where S07 : IUpdateSystem where S08 : IUpdateSystem
                where S09 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 9;
                _allSystems[_allSystemsCount] = (new SystemsBatch<
                                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                                       S09
                                                   >(s01, s02, s03, s04, s05, s06, s07, s08,
                                                     s09), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03, S04, S05, S06, S07, S08,
                S09, S10
            >(
                S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                S09 s09, S10 s10,
                short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem where S04 : IUpdateSystem where S05 : IUpdateSystem where S06 : IUpdateSystem where S07 : IUpdateSystem where S08 : IUpdateSystem
                where S09 : IUpdateSystem where S10 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 10;
                _allSystems[_allSystemsCount] = (new SystemsBatch<
                                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                                       S09, S10
                                                   >(s01, s02, s03, s04, s05, s06, s07, s08,
                                                     s09, s10), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03, S04, S05, S06, S07, S08,
                S09, S10, S11
            >(
                S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                S09 s09, S10 s10, S11 s11,
                short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem where S04 : IUpdateSystem where S05 : IUpdateSystem where S06 : IUpdateSystem where S07 : IUpdateSystem where S08 : IUpdateSystem
                where S09 : IUpdateSystem where S10 : IUpdateSystem where S11 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 11;
                _allSystems[_allSystemsCount] = (new SystemsBatch<
                                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                                       S09, S10, S11
                                                   >(s01, s02, s03, s04, s05, s06, s07, s08,
                                                     s09, s10, s11), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03, S04, S05, S06, S07, S08,
                S09, S10, S11, S12
            >(
                S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                S09 s09, S10 s10, S11 s11, S12 s12,
                short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem where S04 : IUpdateSystem where S05 : IUpdateSystem where S06 : IUpdateSystem where S07 : IUpdateSystem where S08 : IUpdateSystem
                where S09 : IUpdateSystem where S10 : IUpdateSystem where S11 : IUpdateSystem where S12 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 12;
                _allSystems[_allSystemsCount] = (new SystemsBatch<
                                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                                       S09, S10, S11, S12
                                                   >(s01, s02, s03, s04, s05, s06, s07, s08,
                                                     s09, s10, s11, s12), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03, S04, S05, S06, S07, S08,
                S09, S10, S11, S12, S13
            >(
                S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                S09 s09, S10 s10, S11 s11, S12 s12, S13 s13,
                short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem where S04 : IUpdateSystem where S05 : IUpdateSystem where S06 : IUpdateSystem where S07 : IUpdateSystem where S08 : IUpdateSystem
                where S09 : IUpdateSystem where S10 : IUpdateSystem where S11 : IUpdateSystem where S12 : IUpdateSystem where S13 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 13;
                _allSystems[_allSystemsCount] = (new SystemsBatch<
                                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                                       S09, S10, S11, S12, S13
                                                   >(s01, s02, s03, s04, s05, s06, s07, s08,
                                                     s09, s10, s11, s12, s13), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03, S04, S05, S06, S07, S08,
                S09, S10, S11, S12, S13, S14
            >(
                S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                S09 s09, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14,
                short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem where S04 : IUpdateSystem where S05 : IUpdateSystem where S06 : IUpdateSystem where S07 : IUpdateSystem where S08 : IUpdateSystem
                where S09 : IUpdateSystem where S10 : IUpdateSystem where S11 : IUpdateSystem where S12 : IUpdateSystem where S13 : IUpdateSystem where S14 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 14;
                _allSystems[_allSystemsCount] = (new SystemsBatch<
                                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                                       S09, S10, S11, S12, S13, S14
                                                   >(s01, s02, s03, s04, s05, s06, s07, s08,
                                                     s09, s10, s11, s12, s13, s14), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03, S04, S05, S06, S07, S08,
                S09, S10, S11, S12, S13, S14, S15
            >(
                S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                S09 s09, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14, S15 s15,
                short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem where S04 : IUpdateSystem where S05 : IUpdateSystem where S06 : IUpdateSystem where S07 : IUpdateSystem where S08 : IUpdateSystem
                where S09 : IUpdateSystem where S10 : IUpdateSystem where S11 : IUpdateSystem where S12 : IUpdateSystem where S13 : IUpdateSystem where S14 : IUpdateSystem where S15 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 15;
                _allSystems[_allSystemsCount] = (new SystemsBatch<
                                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                                       S09, S10, S11, S12, S13, S14, S15
                                                   >(s01, s02, s03, s04, s05, s06, s07, s08,
                                                     s09, s10, s11, s12, s13, s14, s15), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03, S04, S05, S06, S07, S08,
                S09, S10, S11, S12, S13, S14, S15, S16
            >(
                S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                S09 s09, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14, S15 s15, S16 s16,
              short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem where S04 : IUpdateSystem where S05 : IUpdateSystem where S06 : IUpdateSystem where S07 : IUpdateSystem where S08 : IUpdateSystem
                where S09 : IUpdateSystem where S10 : IUpdateSystem where S11 : IUpdateSystem where S12 : IUpdateSystem where S13 : IUpdateSystem where S14 : IUpdateSystem where S15 : IUpdateSystem where S16 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 16;
                _allSystems[_allSystemsCount] = (new SystemsBatch<
                                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                                       S09, S10, S11, S12, S13, S14, S15, S16
                                                   >(s01, s02, s03, s04, s05, s06, s07, s08,
                                                     s09, s10, s11, s12, s13, s14, s15, s16), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03, S04, S05, S06, S07, S08,
                S09, S10, S11, S12, S13, S14, S15, S16,
                S17, S18, S19, S20, S21, S22, S23, S24
            >(
                S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                S09 s09, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14, S15 s15, S16 s16,
                S17 s17, S18 s18, S19 s19, S20 s20, S21 s21, S22 s22, S23 s23, S24 s24,
              short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem where S04 : IUpdateSystem where S05 : IUpdateSystem where S06 : IUpdateSystem where S07 : IUpdateSystem where S08 : IUpdateSystem
                where S09 : IUpdateSystem where S10 : IUpdateSystem where S11 : IUpdateSystem where S12 : IUpdateSystem where S13 : IUpdateSystem where S14 : IUpdateSystem where S15 : IUpdateSystem where S16 : IUpdateSystem
                where S17 : IUpdateSystem where S18 : IUpdateSystem where S19 : IUpdateSystem where S20 : IUpdateSystem where S21 : IUpdateSystem where S22 : IUpdateSystem where S23 : IUpdateSystem where S24 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 24;
                _allSystems[_allSystemsCount] = (new SystemsBatch<
                                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                                       S09, S10, S11, S12, S13, S14, S15, S16,
                                                       S17, S18, S19, S20, S21, S22, S23, S24
                                                   >(s01, s02, s03, s04, s05, s06, s07, s08,
                                                     s09, s10, s11, s12, s13, s14, s15, s16,
                                                     s17, s18, s19, s20, s21, s22, s23, s24), order, _allSystemsCount);
                _allSystemsCount++;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void AddUpdate<
                S01, S02, S03, S04, S05, S06, S07, S08,
                S09, S10, S11, S12, S13, S14, S15, S16,
                S17, S18, S19, S20, S21, S22, S23, S24,
                S25, S26, S27, S28, S29, S30, S31, S32
            >(
                S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                S09 s09, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14, S15 s15, S16 s16,
                S17 s17, S18 s18, S19 s19, S20 s20, S21 s21, S22 s22, S23 s23, S24 s24,
                S25 s25, S26 s26, S27 s27, S28 s28, S29 s29, S30 s30, S31 s31, S32 s32,
              short order = 0) 
                where S01 : IUpdateSystem where S02 : IUpdateSystem where S03 : IUpdateSystem where S04 : IUpdateSystem where S05 : IUpdateSystem where S06 : IUpdateSystem where S07 : IUpdateSystem where S08 : IUpdateSystem
                where S09 : IUpdateSystem where S10 : IUpdateSystem where S11 : IUpdateSystem where S12 : IUpdateSystem where S13 : IUpdateSystem where S14 : IUpdateSystem where S15 : IUpdateSystem where S16 : IUpdateSystem
                where S17 : IUpdateSystem where S18 : IUpdateSystem where S19 : IUpdateSystem where S20 : IUpdateSystem where S21 : IUpdateSystem where S22 : IUpdateSystem where S23 : IUpdateSystem where S24 : IUpdateSystem
                where S25 : IUpdateSystem where S26 : IUpdateSystem where S27 : IUpdateSystem where S28 : IUpdateSystem where S29 : IUpdateSystem where S30 : IUpdateSystem where S31 : IUpdateSystem where S32 : IUpdateSystem
            {
                EnsureSize();
                _updateSystemsCount += 32;
                _allSystems[_allSystemsCount] = (new SystemsBatch<
                                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                                       S09, S10, S11, S12, S13, S14, S15, S16,
                                                       S17, S18, S19, S20, S21, S22, S23, S24,
                                                       S25, S26, S27, S28, S29, S30, S31, S32
                                                   >(s01, s02, s03, s04, s05, s06, s07, s08,
                                                     s09, s10, s11, s12, s13, s14, s15, s16,
                                                     s17, s18, s19, s20, s21, s22, s23, s24,
                                                     s25, s26, s27, s28, s29, s30, s31, s32), order, _allSystemsCount);
                _allSystemsCount++;
            }
        }
    }
}