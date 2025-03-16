using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif


namespace FFS.Libraries.StaticEcs {
    public interface IOnCreateEntityFunction<WorldType> where WorldType : struct, IWorldType {
        public void OnCreate(Ecs<WorldType>.Entity entity);
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal readonly struct AddComponentFunction<WorldType, T1> : IOnCreateEntityFunction<WorldType> 
        where T1 : struct, IComponent 
        where WorldType : struct, IWorldType {

        [MethodImpl(AggressiveInlining)]
        public void OnCreate(Ecs<WorldType>.Entity entity) {
            Ecs<WorldType>.Components<T1>.Value.Add(entity);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal readonly struct AddComponentFunction<WorldType, T1, T2> : IOnCreateEntityFunction<WorldType> 
        where T1 : struct, IComponent 
        where T2 : struct, IComponent
        where WorldType : struct, IWorldType {

        [MethodImpl(AggressiveInlining)]
        public void OnCreate(Ecs<WorldType>.Entity entity) {
            Ecs<WorldType>.Components<T1>.Value.Add(entity);
            Ecs<WorldType>.Components<T2>.Value.Add(entity);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal readonly struct AddComponentFunction<WorldType, T1, T2, T3> : IOnCreateEntityFunction<WorldType> 
        where T1 : struct, IComponent 
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where WorldType : struct, IWorldType {

        [MethodImpl(AggressiveInlining)]
        public void OnCreate(Ecs<WorldType>.Entity entity) {
            Ecs<WorldType>.Components<T1>.Value.Add(entity);
            Ecs<WorldType>.Components<T2>.Value.Add(entity);
            Ecs<WorldType>.Components<T3>.Value.Add(entity);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal readonly struct AddComponentFunction<WorldType, T1, T2, T3, T4> : IOnCreateEntityFunction<WorldType> 
        where T1 : struct, IComponent 
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where WorldType : struct, IWorldType {

        [MethodImpl(AggressiveInlining)]
        public void OnCreate(Ecs<WorldType>.Entity entity) {
            Ecs<WorldType>.Components<T1>.Value.Add(entity);
            Ecs<WorldType>.Components<T2>.Value.Add(entity);
            Ecs<WorldType>.Components<T3>.Value.Add(entity);
            Ecs<WorldType>.Components<T4>.Value.Add(entity);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal readonly struct AddComponentFunction<WorldType, T1, T2, T3, T4, T5> : IOnCreateEntityFunction<WorldType> 
        where T1 : struct, IComponent 
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where WorldType : struct, IWorldType {

        [MethodImpl(AggressiveInlining)]
        public void OnCreate(Ecs<WorldType>.Entity entity) {
            Ecs<WorldType>.Components<T1>.Value.Add(entity);
            Ecs<WorldType>.Components<T2>.Value.Add(entity);
            Ecs<WorldType>.Components<T3>.Value.Add(entity);
            Ecs<WorldType>.Components<T4>.Value.Add(entity);
            Ecs<WorldType>.Components<T5>.Value.Add(entity);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal readonly struct PutComponentFunction<WorldType, T1> : IOnCreateEntityFunction<WorldType> 
        where T1 : struct, IComponent 
        where WorldType : struct, IWorldType {
        private readonly T1 t1;
        
        [MethodImpl(AggressiveInlining)]
        public PutComponentFunction(T1 t1) {
            this.t1 = t1;
        }

        [MethodImpl(AggressiveInlining)]
        public void OnCreate(Ecs<WorldType>.Entity entity) {
            Ecs<WorldType>.Components<T1>.Value.Put(entity, t1);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal readonly struct PutComponentFunction<WorldType, T1, T2> : IOnCreateEntityFunction<WorldType> 
        where T1 : struct, IComponent 
        where T2 : struct, IComponent
        where WorldType : struct, IWorldType {
        private readonly T1 t1;
        private readonly T2 t2;
        
        [MethodImpl(AggressiveInlining)]
        public PutComponentFunction(T1 t1, T2 t2) {
            this.t1 = t1;
            this.t2 = t2;
        }

        [MethodImpl(AggressiveInlining)]
        public void OnCreate(Ecs<WorldType>.Entity entity) {
            Ecs<WorldType>.Components<T1>.Value.Put(entity, t1);
            Ecs<WorldType>.Components<T2>.Value.Put(entity, t2);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal readonly struct PutComponentFunction<WorldType, T1, T2, T3> : IOnCreateEntityFunction<WorldType> 
        where T1 : struct, IComponent 
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where WorldType : struct, IWorldType {
        private readonly T1 t1;
        private readonly T2 t2;
        private readonly T3 t3;
        
        [MethodImpl(AggressiveInlining)]
        public PutComponentFunction(T1 t1, T2 t2, T3 t3) {
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
        }

        [MethodImpl(AggressiveInlining)]
        public void OnCreate(Ecs<WorldType>.Entity entity) {
            Ecs<WorldType>.Components<T1>.Value.Put(entity, t1);
            Ecs<WorldType>.Components<T2>.Value.Put(entity, t2);
            Ecs<WorldType>.Components<T3>.Value.Put(entity, t3);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal readonly struct PutComponentFunction<WorldType, T1, T2, T3, T4> : IOnCreateEntityFunction<WorldType> 
        where T1 : struct, IComponent 
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where WorldType : struct, IWorldType {
        private readonly T1 t1;
        private readonly T2 t2;
        private readonly T3 t3;
        private readonly T4 t4;
        
        [MethodImpl(AggressiveInlining)]
        public PutComponentFunction(T1 t1, T2 t2, T3 t3, T4 t4) {
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
            this.t4 = t4;
        }

        [MethodImpl(AggressiveInlining)]
        public void OnCreate(Ecs<WorldType>.Entity entity) {
            Ecs<WorldType>.Components<T1>.Value.Put(entity, t1);
            Ecs<WorldType>.Components<T2>.Value.Put(entity, t2);
            Ecs<WorldType>.Components<T3>.Value.Put(entity, t3);
            Ecs<WorldType>.Components<T4>.Value.Put(entity, t4);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal readonly struct PutComponentFunction<WorldType, T1, T2, T3, T4, T5> : IOnCreateEntityFunction<WorldType> 
        where T1 : struct, IComponent 
        where T2 : struct, IComponent
        where T3 : struct, IComponent
        where T4 : struct, IComponent
        where T5 : struct, IComponent
        where WorldType : struct, IWorldType {
        private readonly T1 t1;
        private readonly T2 t2;
        private readonly T3 t3;
        private readonly T4 t4;
        private readonly T5 t5;
        
        [MethodImpl(AggressiveInlining)]
        public PutComponentFunction(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) {
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
            this.t4 = t4;
            this.t5 = t5;
        }

        [MethodImpl(AggressiveInlining)]
        public void OnCreate(Ecs<WorldType>.Entity entity) {
            Ecs<WorldType>.Components<T1>.Value.Put(entity, t1);
            Ecs<WorldType>.Components<T2>.Value.Put(entity, t2);
            Ecs<WorldType>.Components<T3>.Value.Put(entity, t3);
            Ecs<WorldType>.Components<T4>.Value.Put(entity, t4);
            Ecs<WorldType>.Components<T5>.Value.Put(entity, t5);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal readonly struct AddComponentOnCreateEntity1<WorldType> : IOnCreateEntityFunction<WorldType> 
        where WorldType : struct, IWorldType {
        private readonly ComponentDynId t1;
        
        [MethodImpl(AggressiveInlining)]
        public AddComponentOnCreateEntity1(ComponentDynId t1) {
            this.t1 = t1;
        }


        [MethodImpl(AggressiveInlining)]
        public void OnCreate(Ecs<WorldType>.Entity entity) {
            Ecs<WorldType>.ModuleComponents.Value.GetPool(t1).Add(entity);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal readonly struct AddComponentOnCreateEntity2<WorldType> : IOnCreateEntityFunction<WorldType> 
        where WorldType : struct, IWorldType {
        private readonly ComponentDynId t1;
        private readonly ComponentDynId t2;
        
        [MethodImpl(AggressiveInlining)]
        public AddComponentOnCreateEntity2(ComponentDynId t1, ComponentDynId t2) {
            this.t1 = t1;
            this.t2 = t2;
        }


        [MethodImpl(AggressiveInlining)]
        public void OnCreate(Ecs<WorldType>.Entity entity) {
            Ecs<WorldType>.ModuleComponents.Value.GetPool(t1).Add(entity);
            Ecs<WorldType>.ModuleComponents.Value.GetPool(t2).Add(entity);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal readonly struct AddComponentOnCreateEntity3<WorldType> : IOnCreateEntityFunction<WorldType> 
        where WorldType : struct, IWorldType {
        private readonly ComponentDynId t1;
        private readonly ComponentDynId t2;
        private readonly ComponentDynId t3;
        
        [MethodImpl(AggressiveInlining)]
        public AddComponentOnCreateEntity3(ComponentDynId t1, ComponentDynId t2, ComponentDynId t3) {
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
        }


        [MethodImpl(AggressiveInlining)]
        public void OnCreate(Ecs<WorldType>.Entity entity) {
            Ecs<WorldType>.ModuleComponents.Value.GetPool(t1).Add(entity);
            Ecs<WorldType>.ModuleComponents.Value.GetPool(t2).Add(entity);
            Ecs<WorldType>.ModuleComponents.Value.GetPool(t3).Add(entity);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal readonly struct AddComponentOnCreateEntity4<WorldType> : IOnCreateEntityFunction<WorldType> 
        where WorldType : struct, IWorldType {
        private readonly ComponentDynId t1;
        private readonly ComponentDynId t2;
        private readonly ComponentDynId t3;
        private readonly ComponentDynId t4;
        
        [MethodImpl(AggressiveInlining)]
        public AddComponentOnCreateEntity4(ComponentDynId t1, ComponentDynId t2, ComponentDynId t3, ComponentDynId t4) {
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
            this.t4 = t4;
        }


        [MethodImpl(AggressiveInlining)]
        public void OnCreate(Ecs<WorldType>.Entity entity) {
            Ecs<WorldType>.ModuleComponents.Value.GetPool(t1).Add(entity);
            Ecs<WorldType>.ModuleComponents.Value.GetPool(t2).Add(entity);
            Ecs<WorldType>.ModuleComponents.Value.GetPool(t3).Add(entity);
            Ecs<WorldType>.ModuleComponents.Value.GetPool(t4).Add(entity);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal readonly struct AddComponentOnCreateEntity5<WorldType> : IOnCreateEntityFunction<WorldType> 
        where WorldType : struct, IWorldType {
        private readonly ComponentDynId t1;
        private readonly ComponentDynId t2;
        private readonly ComponentDynId t3;
        private readonly ComponentDynId t4;
        private readonly ComponentDynId t5;
        
        [MethodImpl(AggressiveInlining)]
        public AddComponentOnCreateEntity5(ComponentDynId t1, ComponentDynId t2, ComponentDynId t3, ComponentDynId t4, ComponentDynId t5) {
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
            this.t4 = t4;
            this.t5 = t5;
        }


        [MethodImpl(AggressiveInlining)]
        public void OnCreate(Ecs<WorldType>.Entity entity) {
            Ecs<WorldType>.ModuleComponents.Value.GetPool(t1).Add(entity);
            Ecs<WorldType>.ModuleComponents.Value.GetPool(t2).Add(entity);
            Ecs<WorldType>.ModuleComponents.Value.GetPool(t3).Add(entity);
            Ecs<WorldType>.ModuleComponents.Value.GetPool(t4).Add(entity);
            Ecs<WorldType>.ModuleComponents.Value.GetPool(t5).Add(entity);
        }
    }
}