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
        #endif
        public readonly partial struct ROEntity {
            internal readonly Entity _entity;
            
            internal ROEntity(uint entityId) {
                _entity = new Entity(entityId);
            }

            [MethodImpl(AggressiveInlining)]
            public static ROEntity FromIdx(uint idx) => new(idx);

            [MethodImpl(AggressiveInlining)]
            public EntityGID Gid() => GIDStore.Value.Get(_entity);

            [MethodImpl(AggressiveInlining)]
            public bool IsActual() => GIDStore.Value.Has(_entity);

            [MethodImpl(AggressiveInlining)]
            public static bool operator ==(ROEntity left, ROEntity right) => left.Equals(right);

            [MethodImpl(AggressiveInlining)]
            public static bool operator !=(ROEntity left, ROEntity right) => !left.Equals(right);

            [MethodImpl(AggressiveInlining)]
            public bool Equals(Entity entity) => _entity == entity;

            [MethodImpl(AggressiveInlining)]
            public override bool Equals(object obj) => throw new StaticEcsException("ROEntity` Equals object` not allowed!");

            [MethodImpl(AggressiveInlining)]
            public override int GetHashCode() => (int) _entity._id;

            [MethodImpl(AggressiveInlining)]
            public override string ToString() => $"ROEntity ID: {_entity._id}";
        }
    }
}