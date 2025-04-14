using System;
using System.Collections.Generic;
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
            public short Version() => StandardComponents<EntityVersion>.Value.RefInternal(_entity).Value;

            [MethodImpl(AggressiveInlining)]
            public bool IsActual() => StandardComponents<EntityVersion>.Value.RefInternal(_entity).Value > 0;

            [MethodImpl(AggressiveInlining)]
            public static bool operator ==(ROEntity left, ROEntity right) {
                return left.Equals(right);
            }

            [MethodImpl(AggressiveInlining)]
            public static bool operator !=(ROEntity left, ROEntity right) {
                return !left.Equals(right);
            }

            [MethodImpl(AggressiveInlining)]
            public PackedEntity Pack() => new() { _entity = _entity._id, _version = StandardComponents<EntityVersion>.Value.RefInternal(_entity).Value };

            [MethodImpl(AggressiveInlining)]
            public bool Equals(Entity entity) => _entity == entity;

            [MethodImpl(AggressiveInlining)]
            public override bool Equals(object obj) => throw new Exception("ROEntity` Equals object` not allowed!");

            [MethodImpl(AggressiveInlining)]
            public override int GetHashCode() => (int) _entity._id;

            public override string ToString() => $"ROEntity ID: {_entity._id}";
        }
    }
}