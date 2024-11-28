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
    public readonly struct TypesBox : IComponentTypes {
        public readonly IComponentTypes Types;

        [MethodImpl(AggressiveInlining)]
        public TypesBox(IComponentTypes types) {
            Types = types;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Types.SetData<WorldID>(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            Types.SetBitMask<WorldID>(bufId);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            Types.Dispose<WorldID>();
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types1 : IComponentTypes {
        public readonly ComponentDynId C1;

        [MethodImpl(AggressiveInlining)]
        public Types1(ComponentDynId c1) {
            C1 = c1;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif

            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId, C1.Value);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types2 : IComponentTypes {
        public readonly ComponentDynId C1;
        public readonly ComponentDynId C2;

        [MethodImpl(AggressiveInlining)]
        public Types2(ComponentDynId c1, ComponentDynId c2) {
            C1 = c1;
            C2 = c2;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C2).SetDataIfCountLess(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif
            
            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId, C1.Value, C2.Value);
        }


        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C2).AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types3 : IComponentTypes {
        public readonly ComponentDynId C1;
        public readonly ComponentDynId C2;
        public readonly ComponentDynId C3;

        [MethodImpl(AggressiveInlining)]
        public Types3(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3) {
            C1 = c1;
            C2 = c2;
            C3 = c3;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C2).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C3).SetDataIfCountLess(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif
            
            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId, C1.Value, C2.Value, C3.Value);
        }


        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C2).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C3).AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types4 : IComponentTypes {
        public readonly ComponentDynId C1;
        public readonly ComponentDynId C2;
        public readonly ComponentDynId C3;
        public readonly ComponentDynId C4;

        [MethodImpl(AggressiveInlining)]
        public Types4(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4) {
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C2).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C3).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C4).SetDataIfCountLess(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif

            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId, C1.Value, C2.Value, C3.Value, C4.Value);
        }


        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C2).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C3).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C4).AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types5 : IComponentTypes {
        public readonly ComponentDynId C1;
        public readonly ComponentDynId C2;
        public readonly ComponentDynId C3;
        public readonly ComponentDynId C4;
        public readonly ComponentDynId C5;

        [MethodImpl(AggressiveInlining)]
        public Types5(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4, ComponentDynId c5) {
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
            C5 = c5;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C2).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C3).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C4).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C5).SetDataIfCountLess(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif
            
            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId, C1.Value, C2.Value, C3.Value, C4.Value, C5.Value);
        }


        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C2).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C3).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C4).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C5).AddBlocker(val);
        }
        #endif
    }


    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types6 : IComponentTypes {
        public readonly ComponentDynId C1;
        public readonly ComponentDynId C2;
        public readonly ComponentDynId C3;
        public readonly ComponentDynId C4;
        public readonly ComponentDynId C5;
        public readonly ComponentDynId C6;

        [MethodImpl(AggressiveInlining)]
        public Types6(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4, ComponentDynId c5, ComponentDynId c6) {
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
            C5 = c5;
            C6 = c6;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C2).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C3).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C4).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C5).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C6).SetDataIfCountLess(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif
            
            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId, C1.Value, C2.Value, C3.Value, C4.Value, C5.Value, C6.Value);
        }


        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C2).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C3).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C4).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C5).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C6).AddBlocker(val);
        }
        #endif
    }


    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types7 : IComponentTypes {
        public readonly ComponentDynId C1;
        public readonly ComponentDynId C2;
        public readonly ComponentDynId C3;
        public readonly ComponentDynId C4;
        public readonly ComponentDynId C5;
        public readonly ComponentDynId C6;
        public readonly ComponentDynId C7;

        [MethodImpl(AggressiveInlining)]
        public Types7(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4, ComponentDynId c5, ComponentDynId c6, ComponentDynId c7) {
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
            C5 = c5;
            C6 = c6;
            C7 = c7;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C2).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C3).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C4).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C5).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C6).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C7).SetDataIfCountLess(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif
            
            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId, C1.Value, C2.Value, C3.Value, C4.Value, C5.Value, C6.Value, C7.Value);
        }


        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C2).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C3).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C4).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C5).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C6).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C7).AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types8 : IComponentTypes {
        public readonly ComponentDynId C1;
        public readonly ComponentDynId C2;
        public readonly ComponentDynId C3;
        public readonly ComponentDynId C4;
        public readonly ComponentDynId C5;
        public readonly ComponentDynId C6;
        public readonly ComponentDynId C7;
        public readonly ComponentDynId C8;

        [MethodImpl(AggressiveInlining)]
        public Types8(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4, ComponentDynId c5, ComponentDynId c6, ComponentDynId c7, ComponentDynId c8) {
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
            C5 = c5;
            C6 = c6;
            C7 = c7;
            C8 = c8;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C2).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C3).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C4).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C5).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C6).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C7).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C8).SetDataIfCountLess(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif

            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId, C1.Value, C2.Value, C3.Value, C4.Value, C5.Value, C6.Value, C7.Value, C8.Value);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C1).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C2).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C3).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C4).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C5).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C6).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C7).AddBlocker(val);
            Ecs<WorldID>.ModuleComponents.Value.GetPool(C8).AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct TypesArray : IComponentTypes {
        public readonly ComponentDynId[] Types;

        [MethodImpl(AggressiveInlining)]
        public TypesArray(params ComponentDynId[] types) {
            Types = types;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            foreach (var type in Types) {
                Ecs<WorldID>.ModuleComponents.Value.GetPool(type).SetDataIfCountLess(ref minCount, ref entities);
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            foreach (var type in Types) {
                Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId, type.Value);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                Ecs<WorldID>.ModuleComponents.Value.GetPool(type).AddBlocker(1);
                #endif
            }
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            foreach (var type in Types) {
                Ecs<WorldID>.ModuleComponents.Value.GetPool(type).AddBlocker(-1);
            }
        }
        #endif
    }
}