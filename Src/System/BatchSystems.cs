using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
using IUS = FFS.Libraries.StaticEcs.IUpdateSystem;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class Ecs<WorldID> where WorldID : struct, IWorldId {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public abstract partial class Systems<SysID> {
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<
                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                       S09, S10, S11, S12, S13, S14, S15, S16,
                                       S17, S18, S19, S20, S21, S22, S23, S24,
                                       S25, S26, S27, S28, S29, S30, S31, S32
            > : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS where S04 : IUS where S05 : IUS where S06 : IUS where S07 : IUS where S08 : IUS
                where S09 : IUS where S10 : IUS where S11 : IUS where S12 : IUS where S13 : IUS where S14 : IUS where S15 : IUS where S16 : IUS
                where S17 : IUS where S18 : IUS where S19 : IUS where S20 : IUS where S21 : IUS where S22 : IUS where S23 : IUS where S24 : IUS
                where S25 : IUS where S26 : IUS where S27 : IUS where S28 : IUS where S29 : IUS where S30 : IUS where S31 : IUS where S32 : IUS
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03; SW<S04> s04; SW<S05> s05; SW<S06> s06; SW<S07> s07; SW<S08> s08;
                SW<S09> s09; SW<S10> s10; SW<S11> s11; SW<S12> s12; SW<S13> s13; SW<S14> s14; SW<S15> s15; SW<S16> s16;
                SW<S17> s17; SW<S18> s18; SW<S19> s19; SW<S20> s20; SW<S21> s21; SW<S22> s22; SW<S23> s23; SW<S24> s24;
                SW<S25> s25; SW<S26> s26; SW<S27> s27; SW<S28> s28; SW<S29> s29; SW<S30> s30; SW<S31> s31; SW<S32> s32;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                                      S09 s09, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14, S15 s15, S16 s16,
                                      S17 s17, S18 s18, S19 s19, S20 s20, S21 s21, S22 s22, S23 s23, S24 s24,
                                      S25 s25, S26 s26, S27 s27, S28 s28, S29 s29, S30 s30, S31 s31, S32 s32) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03); this.s04 = new(s04); this.s05 = new(s05); this.s06 = new(s06); this.s07 = new(s07); this.s08 = new(s08);
                    this.s09 = new(s09); this.s10 = new(s10); this.s11 = new(s11); this.s12 = new(s12); this.s13 = new(s13); this.s14 = new(s14); this.s15 = new(s15); this.s16 = new(s16);
                    this.s17 = new(s17); this.s18 = new(s18); this.s19 = new(s19); this.s20 = new(s20); this.s21 = new(s21); this.s22 = new(s22); this.s23 = new(s23); this.s24 = new(s24); 
                    this.s25 = new(s25); this.s26 = new(s26); this.s27 = new(s27); this.s28 = new(s28); this.s29 = new(s29); this.s30 = new(s30); this.s31 = new(s31); this.s32 = new(s32);
                } 
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init(); s04.Init(); s05.Init(); s06.Init(); s07.Init(); s08.Init();
                    s09.Init(); s10.Init(); s11.Init(); s12.Init(); s13.Init(); s14.Init(); s15.Init(); s16.Init();
                    s17.Init(); s18.Init(); s19.Init(); s20.Init(); s21.Init(); s22.Init(); s23.Init(); s24.Init(); 
                    s25.Init(); s26.Init(); s27.Init(); s28.Init(); s29.Init(); s30.Init(); s31.Init(); s32.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run(); s04.Run(); s05.Run(); s06.Run(); s07.Run(); s08.Run();
                    s09.Run(); s10.Run(); s11.Run(); s12.Run(); s13.Run(); s14.Run(); s15.Run(); s16.Run();
                    s17.Run(); s18.Run(); s19.Run(); s20.Run(); s21.Run(); s22.Run(); s23.Run(); s24.Run();
                    s25.Run(); s26.Run(); s27.Run(); s28.Run(); s29.Run(); s30.Run(); s31.Run(); s32.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy(); s04.Destroy(); s05.Destroy(); s06.Destroy(); s07.Destroy(); s08.Destroy();
                    s09.Destroy(); s10.Destroy(); s11.Destroy(); s12.Destroy(); s13.Destroy(); s14.Destroy(); s15.Destroy(); s16.Destroy();
                    s17.Destroy(); s18.Destroy(); s19.Destroy(); s20.Destroy(); s21.Destroy(); s22.Destroy(); s23.Destroy(); s24.Destroy(); 
                    s25.Destroy(); s26.Destroy(); s27.Destroy(); s28.Destroy(); s29.Destroy(); s30.Destroy(); s31.Destroy(); s32.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info()); res.Add(s04.Info()); res.Add(s05.Info()); res.Add(s06.Info()); res.Add(s07.Info()); res.Add(s08.Info());
                    res.Add(s09.Info()); res.Add(s10.Info()); res.Add(s11.Info()); res.Add(s12.Info()); res.Add(s13.Info()); res.Add(s14.Info()); res.Add(s15.Info()); res.Add(s16.Info());
                    res.Add(s17.Info()); res.Add(s18.Info()); res.Add(s19.Info()); res.Add(s20.Info()); res.Add(s21.Info()); res.Add(s22.Info()); res.Add(s23.Info()); res.Add(s24.Info()); 
                    res.Add(s25.Info()); res.Add(s26.Info()); res.Add(s27.Info()); res.Add(s28.Info()); res.Add(s29.Info()); res.Add(s30.Info()); res.Add(s31.Info()); res.Add(s32.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active), 03 => s04.SetActive(active), 04 => s05.SetActive(active), 05 => s06.SetActive(active), 06 => s07.SetActive(active), 07 => s08.SetActive(active),
                        08 => s09.SetActive(active), 09 => s10.SetActive(active), 10 => s11.SetActive(active), 11 => s12.SetActive(active), 12 => s13.SetActive(active), 13 => s14.SetActive(active), 14 => s15.SetActive(active), 15 => s16.SetActive(active),
                        16 => s17.SetActive(active), 17 => s18.SetActive(active), 18 => s19.SetActive(active), 19 => s20.SetActive(active), 20 => s21.SetActive(active), 21 => s22.SetActive(active), 22 => s23.SetActive(active), 23 => s24.SetActive(active),
                        24 => s25.SetActive(active), 25 => s26.SetActive(active), 26 => s27.SetActive(active), 27 => s28.SetActive(active), 28 => s29.SetActive(active), 29 => s30.SetActive(active), 30 => s31.SetActive(active), 31 => s32.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif

            }
        
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<
                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                       S09, S10, S11, S12, S13, S14, S15, S16,
                                       S17, S18, S19, S20, S21, S22, S23, S24
            > : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS where S04 : IUS where S05 : IUS where S06 : IUS where S07 : IUS where S08 : IUS
                where S09 : IUS where S10 : IUS where S11 : IUS where S12 : IUS where S13 : IUS where S14 : IUS where S15 : IUS where S16 : IUS
                where S17 : IUS where S18 : IUS where S19 : IUS where S20 : IUS where S21 : IUS where S22 : IUS where S23 : IUS where S24 : IUS
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03; SW<S04> s04; SW<S05> s05; SW<S06> s06; SW<S07> s07; SW<S08> s08;
                SW<S09> s09; SW<S10> s10; SW<S11> s11; SW<S12> s12; SW<S13> s13; SW<S14> s14; SW<S15> s15; SW<S16> s16;
                SW<S17> s17; SW<S18> s18; SW<S19> s19; SW<S20> s20; SW<S21> s21; SW<S22> s22; SW<S23> s23; SW<S24> s24;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                                      S09 s09, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14, S15 s15, S16 s16,
                                      S17 s17, S18 s18, S19 s19, S20 s20, S21 s21, S22 s22, S23 s23, S24 s24) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03); this.s04 = new(s04); this.s05 = new(s05); this.s06 = new(s06); this.s07 = new(s07); this.s08 = new(s08);
                    this.s09 = new(s09); this.s10 = new(s10); this.s11 = new(s11); this.s12 = new(s12); this.s13 = new(s13); this.s14 = new(s14); this.s15 = new(s15); this.s16 = new(s16);
                    this.s17 = new(s17); this.s18 = new(s18); this.s19 = new(s19); this.s20 = new(s20); this.s21 = new(s21); this.s22 = new(s22); this.s23 = new(s23); this.s24 = new(s24); 
                } 
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init(); s04.Init(); s05.Init(); s06.Init(); s07.Init(); s08.Init();
                    s09.Init(); s10.Init(); s11.Init(); s12.Init(); s13.Init(); s14.Init(); s15.Init(); s16.Init();
                    s17.Init(); s18.Init(); s19.Init(); s20.Init(); s21.Init(); s22.Init(); s23.Init(); s24.Init(); 
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run(); s04.Run(); s05.Run(); s06.Run(); s07.Run(); s08.Run();
                    s09.Run(); s10.Run(); s11.Run(); s12.Run(); s13.Run(); s14.Run(); s15.Run(); s16.Run();
                    s17.Run(); s18.Run(); s19.Run(); s20.Run(); s21.Run(); s22.Run(); s23.Run(); s24.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy(); s04.Destroy(); s05.Destroy(); s06.Destroy(); s07.Destroy(); s08.Destroy();
                    s09.Destroy(); s10.Destroy(); s11.Destroy(); s12.Destroy(); s13.Destroy(); s14.Destroy(); s15.Destroy(); s16.Destroy();
                    s17.Destroy(); s18.Destroy(); s19.Destroy(); s20.Destroy(); s21.Destroy(); s22.Destroy(); s23.Destroy(); s24.Destroy(); 
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info()); res.Add(s04.Info()); res.Add(s05.Info()); res.Add(s06.Info()); res.Add(s07.Info()); res.Add(s08.Info());
                    res.Add(s09.Info()); res.Add(s10.Info()); res.Add(s11.Info()); res.Add(s12.Info()); res.Add(s13.Info()); res.Add(s14.Info()); res.Add(s15.Info()); res.Add(s16.Info());
                    res.Add(s17.Info()); res.Add(s18.Info()); res.Add(s19.Info()); res.Add(s20.Info()); res.Add(s21.Info()); res.Add(s22.Info()); res.Add(s23.Info()); res.Add(s24.Info()); 
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active), 03 => s04.SetActive(active), 04 => s05.SetActive(active), 05 => s06.SetActive(active), 06 => s07.SetActive(active), 07 => s08.SetActive(active),
                        08 => s09.SetActive(active), 09 => s10.SetActive(active), 10 => s11.SetActive(active), 11 => s12.SetActive(active), 12 => s13.SetActive(active), 13 => s14.SetActive(active), 14 => s15.SetActive(active), 15 => s16.SetActive(active),
                        16 => s17.SetActive(active), 17 => s18.SetActive(active), 18 => s19.SetActive(active), 19 => s20.SetActive(active), 20 => s21.SetActive(active), 21 => s22.SetActive(active), 22 => s23.SetActive(active), 23 => s24.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
        
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<
                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                       S09, S10, S11, S12, S13, S14, S15, S16
            > : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS where S04 : IUS where S05 : IUS where S06 : IUS where S07 : IUS where S08 : IUS
                where S09 : IUS where S10 : IUS where S11 : IUS where S12 : IUS where S13 : IUS where S14 : IUS where S15 : IUS where S16 : IUS
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03; SW<S04> s04; SW<S05> s05; SW<S06> s06; SW<S07> s07; SW<S08> s08;
                SW<S09> s09; SW<S10> s10; SW<S11> s11; SW<S12> s12; SW<S13> s13; SW<S14> s14; SW<S15> s15; SW<S16> s16;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                                      S09 s09, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14, S15 s15, S16 s16) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03); this.s04 = new(s04); this.s05 = new(s05); this.s06 = new(s06); this.s07 = new(s07); this.s08 = new(s08);
                    this.s09 = new(s09); this.s10 = new(s10); this.s11 = new(s11); this.s12 = new(s12); this.s13 = new(s13); this.s14 = new(s14); this.s15 = new(s15); this.s16 = new(s16);
                } 
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init(); s04.Init(); s05.Init(); s06.Init(); s07.Init(); s08.Init();
                    s09.Init(); s10.Init(); s11.Init(); s12.Init(); s13.Init(); s14.Init(); s15.Init(); s16.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run(); s04.Run(); s05.Run(); s06.Run(); s07.Run(); s08.Run();
                    s09.Run(); s10.Run(); s11.Run(); s12.Run(); s13.Run(); s14.Run(); s15.Run(); s16.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy(); s04.Destroy(); s05.Destroy(); s06.Destroy(); s07.Destroy(); s08.Destroy();
                    s09.Destroy(); s10.Destroy(); s11.Destroy(); s12.Destroy(); s13.Destroy(); s14.Destroy(); s15.Destroy(); s16.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info()); res.Add(s04.Info()); res.Add(s05.Info()); res.Add(s06.Info()); res.Add(s07.Info()); res.Add(s08.Info());
                    res.Add(s09.Info()); res.Add(s10.Info()); res.Add(s11.Info()); res.Add(s12.Info()); res.Add(s13.Info()); res.Add(s14.Info()); res.Add(s15.Info()); res.Add(s16.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active), 03 => s04.SetActive(active), 04 => s05.SetActive(active), 05 => s06.SetActive(active), 06 => s07.SetActive(active), 07 => s08.SetActive(active),
                        08 => s09.SetActive(active), 09 => s10.SetActive(active), 10 => s11.SetActive(active), 11 => s12.SetActive(active), 12 => s13.SetActive(active), 13 => s14.SetActive(active), 14 => s15.SetActive(active), 15 => s16.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<
                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                       S09, S10, S11, S12, S13, S14, S15
            > : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS where S04 : IUS where S05 : IUS where S06 : IUS where S07 : IUS where S08 : IUS
                where S09 : IUS where S10 : IUS where S11 : IUS where S12 : IUS where S13 : IUS where S14 : IUS where S15 : IUS
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03; SW<S04> s04; SW<S05> s05; SW<S06> s06; SW<S07> s07; SW<S08> s08;
                SW<S09> s09; SW<S10> s10; SW<S11> s11; SW<S12> s12; SW<S13> s13; SW<S14> s14; SW<S15> s15;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                                      S09 s09, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14, S15 s15) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03); this.s04 = new(s04); this.s05 = new(s05); this.s06 = new(s06); this.s07 = new(s07); this.s08 = new(s08);
                    this.s09 = new(s09); this.s10 = new(s10); this.s11 = new(s11); this.s12 = new(s12); this.s13 = new(s13); this.s14 = new(s14); this.s15 = new(s15);
                } 
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init(); s04.Init(); s05.Init(); s06.Init(); s07.Init(); s08.Init();
                    s09.Init(); s10.Init(); s11.Init(); s12.Init(); s13.Init(); s14.Init(); s15.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run(); s04.Run(); s05.Run(); s06.Run(); s07.Run(); s08.Run();
                    s09.Run(); s10.Run(); s11.Run(); s12.Run(); s13.Run(); s14.Run(); s15.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy(); s04.Destroy(); s05.Destroy(); s06.Destroy(); s07.Destroy(); s08.Destroy();
                    s09.Destroy(); s10.Destroy(); s11.Destroy(); s12.Destroy(); s13.Destroy(); s14.Destroy(); s15.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info()); res.Add(s04.Info()); res.Add(s05.Info()); res.Add(s06.Info()); res.Add(s07.Info()); res.Add(s08.Info());
                    res.Add(s09.Info()); res.Add(s10.Info()); res.Add(s11.Info()); res.Add(s12.Info()); res.Add(s13.Info()); res.Add(s14.Info()); res.Add(s15.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active), 03 => s04.SetActive(active), 04 => s05.SetActive(active), 05 => s06.SetActive(active), 06 => s07.SetActive(active), 07 => s08.SetActive(active),
                        08 => s09.SetActive(active), 09 => s10.SetActive(active), 10 => s11.SetActive(active), 11 => s12.SetActive(active), 12 => s13.SetActive(active), 13 => s14.SetActive(active), 14 => s15.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<
                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                       S09, S10, S11, S12, S13, S14
            > : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS where S04 : IUS where S05 : IUS where S06 : IUS where S07 : IUS where S08 : IUS
                where S09 : IUS where S10 : IUS where S11 : IUS where S12 : IUS where S13 : IUS where S14 : IUS
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03; SW<S04> s04; SW<S05> s05; SW<S06> s06; SW<S07> s07; SW<S08> s08;
                SW<S09> s09; SW<S10> s10; SW<S11> s11; SW<S12> s12; SW<S13> s13; SW<S14> s14;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                                      S09 s09, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03); this.s04 = new(s04); this.s05 = new(s05); this.s06 = new(s06); this.s07 = new(s07); this.s08 = new(s08);
                    this.s09 = new(s09); this.s10 = new(s10); this.s11 = new(s11); this.s12 = new(s12); this.s13 = new(s13); this.s14 = new(s14);
                } 
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init(); s04.Init(); s05.Init(); s06.Init(); s07.Init(); s08.Init();
                    s09.Init(); s10.Init(); s11.Init(); s12.Init(); s13.Init(); s14.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run(); s04.Run(); s05.Run(); s06.Run(); s07.Run(); s08.Run();
                    s09.Run(); s10.Run(); s11.Run(); s12.Run(); s13.Run(); s14.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy(); s04.Destroy(); s05.Destroy(); s06.Destroy(); s07.Destroy(); s08.Destroy();
                    s09.Destroy(); s10.Destroy(); s11.Destroy(); s12.Destroy(); s13.Destroy(); s14.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info()); res.Add(s04.Info()); res.Add(s05.Info()); res.Add(s06.Info()); res.Add(s07.Info()); res.Add(s08.Info());
                    res.Add(s09.Info()); res.Add(s10.Info()); res.Add(s11.Info()); res.Add(s12.Info()); res.Add(s13.Info()); res.Add(s14.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active), 03 => s04.SetActive(active), 04 => s05.SetActive(active), 05 => s06.SetActive(active), 06 => s07.SetActive(active), 07 => s08.SetActive(active),
                        08 => s09.SetActive(active), 09 => s10.SetActive(active), 10 => s11.SetActive(active), 11 => s12.SetActive(active), 12 => s13.SetActive(active), 13 => s14.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<
                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                       S09, S10, S11, S12, S13
            > : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS where S04 : IUS where S05 : IUS where S06 : IUS where S07 : IUS where S08 : IUS
                where S09 : IUS where S10 : IUS where S11 : IUS where S12 : IUS where S13 : IUS
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03; SW<S04> s04; SW<S05> s05; SW<S06> s06; SW<S07> s07; SW<S08> s08;
                SW<S09> s09; SW<S10> s10; SW<S11> s11; SW<S12> s12; SW<S13> s13;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                                      S09 s09, S10 s10, S11 s11, S12 s12, S13 s13) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03); this.s04 = new(s04); this.s05 = new(s05); this.s06 = new(s06); this.s07 = new(s07); this.s08 = new(s08);
                    this.s09 = new(s09); this.s10 = new(s10); this.s11 = new(s11); this.s12 = new(s12); this.s13 = new(s13);
                } 
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init(); s04.Init(); s05.Init(); s06.Init(); s07.Init(); s08.Init();
                    s09.Init(); s10.Init(); s11.Init(); s12.Init(); s13.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run(); s04.Run(); s05.Run(); s06.Run(); s07.Run(); s08.Run();
                    s09.Run(); s10.Run(); s11.Run(); s12.Run(); s13.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy(); s04.Destroy(); s05.Destroy(); s06.Destroy(); s07.Destroy(); s08.Destroy();
                    s09.Destroy(); s10.Destroy(); s11.Destroy(); s12.Destroy(); s13.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info()); res.Add(s04.Info()); res.Add(s05.Info()); res.Add(s06.Info()); res.Add(s07.Info()); res.Add(s08.Info());
                    res.Add(s09.Info()); res.Add(s10.Info()); res.Add(s11.Info()); res.Add(s12.Info()); res.Add(s13.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active), 03 => s04.SetActive(active), 04 => s05.SetActive(active), 05 => s06.SetActive(active), 06 => s07.SetActive(active), 07 => s08.SetActive(active),
                        08 => s09.SetActive(active), 09 => s10.SetActive(active), 10 => s11.SetActive(active), 11 => s12.SetActive(active), 12 => s13.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<
                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                       S09, S10, S11, S12
            > : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS where S04 : IUS where S05 : IUS where S06 : IUS where S07 : IUS where S08 : IUS
                where S09 : IUS where S10 : IUS where S11 : IUS where S12 : IUS
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03; SW<S04> s04; SW<S05> s05; SW<S06> s06; SW<S07> s07; SW<S08> s08;
                SW<S09> s09; SW<S10> s10; SW<S11> s11; SW<S12> s12;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                                      S09 s09, S10 s10, S11 s11, S12 s12) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03); this.s04 = new(s04); this.s05 = new(s05); this.s06 = new(s06); this.s07 = new(s07); this.s08 = new(s08);
                    this.s09 = new(s09); this.s10 = new(s10); this.s11 = new(s11); this.s12 = new(s12);
                } 
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init(); s04.Init(); s05.Init(); s06.Init(); s07.Init(); s08.Init();
                    s09.Init(); s10.Init(); s11.Init(); s12.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run(); s04.Run(); s05.Run(); s06.Run(); s07.Run(); s08.Run();
                    s09.Run(); s10.Run(); s11.Run(); s12.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy(); s04.Destroy(); s05.Destroy(); s06.Destroy(); s07.Destroy(); s08.Destroy();
                    s09.Destroy(); s10.Destroy(); s11.Destroy(); s12.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info()); res.Add(s04.Info()); res.Add(s05.Info()); res.Add(s06.Info()); res.Add(s07.Info()); res.Add(s08.Info());
                    res.Add(s09.Info()); res.Add(s10.Info()); res.Add(s11.Info()); res.Add(s12.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active), 03 => s04.SetActive(active), 04 => s05.SetActive(active), 05 => s06.SetActive(active), 06 => s07.SetActive(active), 07 => s08.SetActive(active),
                        08 => s09.SetActive(active), 09 => s10.SetActive(active), 10 => s11.SetActive(active), 11 => s12.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<
                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                       S09, S10, S11
            > : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS where S04 : IUS where S05 : IUS where S06 : IUS where S07 : IUS where S08 : IUS
                where S09 : IUS where S10 : IUS where S11 : IUS
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03; SW<S04> s04; SW<S05> s05; SW<S06> s06; SW<S07> s07; SW<S08> s08;
                SW<S09> s09; SW<S10> s10; SW<S11> s11;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                                      S09 s09, S10 s10, S11 s11) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03); this.s04 = new(s04); this.s05 = new(s05); this.s06 = new(s06); this.s07 = new(s07); this.s08 = new(s08);
                    this.s09 = new(s09); this.s10 = new(s10); this.s11 = new(s11);
                } 
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init(); s04.Init(); s05.Init(); s06.Init(); s07.Init(); s08.Init();
                    s09.Init(); s10.Init(); s11.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run(); s04.Run(); s05.Run(); s06.Run(); s07.Run(); s08.Run();
                    s09.Run(); s10.Run(); s11.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy(); s04.Destroy(); s05.Destroy(); s06.Destroy(); s07.Destroy(); s08.Destroy();
                    s09.Destroy(); s10.Destroy(); s11.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info()); res.Add(s04.Info()); res.Add(s05.Info()); res.Add(s06.Info()); res.Add(s07.Info()); res.Add(s08.Info());
                    res.Add(s09.Info()); res.Add(s10.Info()); res.Add(s11.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active), 03 => s04.SetActive(active), 04 => s05.SetActive(active), 05 => s06.SetActive(active), 06 => s07.SetActive(active), 07 => s08.SetActive(active),
                        08 => s09.SetActive(active), 09 => s10.SetActive(active), 10 => s11.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<
                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                       S09, S10
            > : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS where S04 : IUS where S05 : IUS where S06 : IUS where S07 : IUS where S08 : IUS
                where S09 : IUS where S10 : IUS
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03; SW<S04> s04; SW<S05> s05; SW<S06> s06; SW<S07> s07; SW<S08> s08;
                SW<S09> s09; SW<S10> s10;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                                      S09 s09, S10 s10) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03); this.s04 = new(s04); this.s05 = new(s05); this.s06 = new(s06); this.s07 = new(s07); this.s08 = new(s08);
                    this.s09 = new(s09); this.s10 = new(s10);
                } 
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init(); s04.Init(); s05.Init(); s06.Init(); s07.Init(); s08.Init();
                    s09.Init(); s10.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run(); s04.Run(); s05.Run(); s06.Run(); s07.Run(); s08.Run();
                    s09.Run(); s10.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy(); s04.Destroy(); s05.Destroy(); s06.Destroy(); s07.Destroy(); s08.Destroy();
                    s09.Destroy(); s10.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info()); res.Add(s04.Info()); res.Add(s05.Info()); res.Add(s06.Info()); res.Add(s07.Info()); res.Add(s08.Info());
                    res.Add(s09.Info()); res.Add(s10.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active), 03 => s04.SetActive(active), 04 => s05.SetActive(active), 05 => s06.SetActive(active), 06 => s07.SetActive(active), 07 => s08.SetActive(active),
                        08 => s09.SetActive(active), 09 => s10.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<
                                       S01, S02, S03, S04, S05, S06, S07, S08,
                                       S09
            > : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS where S04 : IUS where S05 : IUS where S06 : IUS where S07 : IUS where S08 : IUS
                where S09 : IUS
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03; SW<S04> s04; SW<S05> s05; SW<S06> s06; SW<S07> s07; SW<S08> s08;
                SW<S09> s09;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08,
                                      S09 s09) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03); this.s04 = new(s04); this.s05 = new(s05); this.s06 = new(s06); this.s07 = new(s07); this.s08 = new(s08);
                    this.s09 = new(s09);
                } 
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init(); s04.Init(); s05.Init(); s06.Init(); s07.Init(); s08.Init();
                    s09.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run(); s04.Run(); s05.Run(); s06.Run(); s07.Run(); s08.Run();
                    s09.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy(); s04.Destroy(); s05.Destroy(); s06.Destroy(); s07.Destroy(); s08.Destroy();
                    s09.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info()); res.Add(s04.Info()); res.Add(s05.Info()); res.Add(s06.Info()); res.Add(s07.Info()); res.Add(s08.Info());
                    res.Add(s09.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active), 03 => s04.SetActive(active), 04 => s05.SetActive(active), 05 => s06.SetActive(active), 06 => s07.SetActive(active), 07 => s08.SetActive(active),
                        08 => s09.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
        
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<S01, S02, S03, S04, S05, S06, S07, S08> : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS where S04 : IUS where S05 : IUS where S06 : IUS where S07 : IUS where S08 : IUS
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03; SW<S04> s04; SW<S05> s05; SW<S06> s06; SW<S07> s07; SW<S08> s08;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07, S08 s08) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03); this.s04 = new(s04); this.s05 = new(s05); this.s06 = new(s06); this.s07 = new(s07); this.s08 = new(s08);
                } 
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init(); s04.Init(); s05.Init(); s06.Init(); s07.Init(); s08.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run(); s04.Run(); s05.Run(); s06.Run(); s07.Run(); s08.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy(); s04.Destroy(); s05.Destroy(); s06.Destroy(); s07.Destroy(); s08.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info()); res.Add(s04.Info()); res.Add(s05.Info()); res.Add(s06.Info()); res.Add(s07.Info()); res.Add(s08.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active), 03 => s04.SetActive(active), 04 => s05.SetActive(active), 05 => s06.SetActive(active), 06 => s07.SetActive(active), 07 => s08.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<S01, S02, S03, S04, S05, S06, S07> : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS where S04 : IUS where S05 : IUS where S06 : IUS where S07 : IUS 
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03; SW<S04> s04; SW<S05> s05; SW<S06> s06; SW<S07> s07;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06, S07 s07) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03); this.s04 = new(s04); this.s05 = new(s05); this.s06 = new(s06); this.s07 = new(s07);
                } 
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init(); s04.Init(); s05.Init(); s06.Init(); s07.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run(); s04.Run(); s05.Run(); s06.Run(); s07.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy(); s04.Destroy(); s05.Destroy(); s06.Destroy(); s07.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info()); res.Add(s04.Info()); res.Add(s05.Info()); res.Add(s06.Info()); res.Add(s07.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active), 03 => s04.SetActive(active), 04 => s05.SetActive(active), 05 => s06.SetActive(active), 06 => s07.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<S01, S02, S03, S04, S05, S06> : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS where S04 : IUS where S05 : IUS where S06 : IUS
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03; SW<S04> s04; SW<S05> s05; SW<S06> s06;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03, S04 s04, S05 s05, S06 s06) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03); this.s04 = new(s04); this.s05 = new(s05); this.s06 = new(s06);
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init(); s04.Init(); s05.Init(); s06.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run(); s04.Run(); s05.Run(); s06.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy(); s04.Destroy(); s05.Destroy(); s06.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info()); res.Add(s04.Info()); res.Add(s05.Info()); res.Add(s06.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active), 03 => s04.SetActive(active), 04 => s05.SetActive(active), 05 => s06.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<S01, S02, S03, S04, S05> : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS where S04 : IUS where S05 : IUS
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03; SW<S04> s04; SW<S05> s05;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03, S04 s04, S05 s05) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03); this.s04 = new(s04); this.s05 = new(s05);
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init(); s04.Init(); s05.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run(); s04.Run(); s05.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy(); s04.Destroy(); s05.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info()); res.Add(s04.Info()); res.Add(s05.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active), 03 => s04.SetActive(active), 04 => s05.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<S01, S02, S03, S04> : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS where S04 : IUS
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03; SW<S04> s04;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03, S04 s04) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03); this.s04 = new(s04);
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init(); s04.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run(); s04.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy(); s04.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info()); res.Add(s04.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active), 03 => s04.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<S01, S02, S03> : ISystemsBatch
                where S01 : IUS where S02 : IUS where S03 : IUS
            {
                SW<S01> s01; SW<S02> s02; SW<S03> s03;
    
                internal SystemsBatch(S01 s01, S02 s02, S03 s03) {
                    this.s01 = new(s01); this.s02 = new(s02); this.s03 = new(s03);
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init(); s03.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run(); s03.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy(); s03.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info()); res.Add(s03.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active), 02 => s03.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<S01, S02> : ISystemsBatch
                where S01 : IUS where S02 : IUS
            {
                SW<S01> s01; SW<S02> s02;
    
                internal SystemsBatch(S01 s01, S02 s02) {
                    this.s01 = new(s01); this.s02 = new(s02);
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init(); s02.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run(); s02.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy(); s02.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info()); res.Add(s02.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active), 01 => s02.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
            
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            #endif
            internal struct SystemsBatch<S01> : ISystemsBatch
                where S01 : IUS
            {
                SW<S01> s01;
    
                internal SystemsBatch(S01 s01) {
                    this.s01 = new(s01);
                }
    
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Init() {
                    s01.Init();
                }
        
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Update() {
                    s01.Run();
                }
                
                [MethodImpl(AggressiveInlining)]
                void ISystemsBatch.Destroy() {
                    s01.Destroy();
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                void ISystemsBatch.Info(List<(string name, float avgUpdateTime, bool enabled, bool initSystem, bool destroySystem)> res) {
                    res.Clear();
                    res.Add(s01.Info());
                }

                bool ISystemsBatch.SetActive(int sysIdx, bool active) =>
                    sysIdx switch {
                        00 => s01.SetActive(active),
                        var _ => throw new Exception()
                    };
                #endif
            }
        }
    }
}