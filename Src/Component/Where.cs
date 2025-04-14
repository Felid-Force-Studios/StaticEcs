using System;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace FFS.Libraries.StaticEcs {
    public struct SomeComponent : IComponent, IWhereValue<int>, IWhereValue<string>, IWhereValue<bool> {
        public int Id;
        public string Name;
        public bool Created;

        int IWhereValue<int>.Val() => Id;
        public static WhereInt<SomeComponent> WhereId() => default;

        string IWhereValue<string>.Val() => Name;
        public static WhereString<SomeComponent> WhereName() => default;

        bool IWhereValue<bool>.Val() => Created;
        public static WhereBool<SomeComponent> WhereCreated() => default;

        public override string ToString() {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Created)}: {Created}";
        }
    }

    public static class Test {
        struct WT : IWorldType { }

        abstract class MyWorld : World<WT> { }
        
        public static void Main() {
            MyWorld.Create(WorldConfig.Default());
            MyWorld.RegisterComponentType<SomeComponent>();
            MyWorld.Initialize();

            MyWorld.Entity.New(new SomeComponent {
                Created = true,
                Id = 1,
                Name = "Alex"
            });

            MyWorld.Entity.New(new SomeComponent {
                Created = true,
                Id = 2,
                Name = "Din"
            });

            MyWorld.Entity.New(new SomeComponent {
                Created = true,
                Id = 3,
                Name = "Jonh"
            });
            
            foreach (var entity in MyWorld.QueryEntities.For(
                         Where<SomeComponent>.By(
                             SomeComponent.WhereId().Between(0, 10),
                             SomeComponent.WhereName().NotEquals("Jonh"),
                             new WhereAction<SomeComponent>(static (ref SomeComponent value) => value.Created)
                         ))) {
                Console.WriteLine(entity.ToPrettyString());
            }
        }
    }
    
    public interface IWhereValue<T> {
        public T Val();
    }

    public interface IWhere<T> {
        public bool Match(ref T value);
    }

    public static class Where<T> where T : struct, IComponent {

        public static Where<T, W1> By<W1>(W1 where1) where W1 : struct, IWhere<T> {
            return new Where<T, W1>(where1);
        }
        
        public static Where<T, W1, W2> By<W1, W2>(W1 where1, W2 where2) where W1 : struct, IWhere<T> where W2 : struct, IWhere<T> {
            return new Where<T, W1, W2>(where1, where2);
        }
        
        public static Where<T, W1, W2, W3> By<W1, W2, W3>(W1 where1, W2 where2, W3 where3) where W1 : struct, IWhere<T> where W2 : struct, IWhere<T> where W3 : struct, IWhere<T> {
            return new Where<T, W1, W2, W3>(where1, where2, where3);
        }
    }

    public struct Where<C1, W1> : IPrimaryQueryMethod, ISealedQueryMethod
        where C1 : struct, IComponent
        where W1 : struct, IWhere<C1> {
        private uint[] m1;
        private C1[] d1;
        private W1 _where1;

        public Where(W1 where1) {
            _where1 = where1;
            m1 = default;
            d1 = default;
        }

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref C1 value) {
            return _where1.Match(ref value);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            World<WorldType>.Components<C1>.Value.AddBlocker(1);
            #endif
            m1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            d1 = World<WorldType>.Components<C1>.Value.Data();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            var i1 = m1[entityId];
            return (i1 & Const.EmptyAndDisabledComponentMask) == 0 && Match(ref d1[i1 & Const.DisabledComponentMaskInv]);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.AddBlocker(-1);
        }
        #endif
    }

    public struct Where<C1, W1, W2> : IPrimaryQueryMethod, ISealedQueryMethod
        where C1 : struct, IComponent
        where W1 : struct, IWhere<C1>
        where W2 : struct, IWhere<C1> {
        private uint[] m1;
        private C1[] d1;
        private W1 _where1;
        private W2 _where2;

        public Where(W1 where1, W2 where2) {
            _where1 = where1;
            _where2 = where2;
            m1 = default;
            d1 = default;
        }

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref C1 value) {
            return _where1.Match(ref value) && _where2.Match(ref value);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            World<WorldType>.Components<C1>.Value.AddBlocker(1);
            #endif
            m1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            d1 = World<WorldType>.Components<C1>.Value.Data();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            var i1 = m1[entityId];
            return (i1 & Const.EmptyAndDisabledComponentMask) == 0 && Match(ref d1[i1 & Const.DisabledComponentMaskInv]);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.AddBlocker(-1);
        }
        #endif
    }

    public struct Where<C1, W1, W2, W3> : IPrimaryQueryMethod, ISealedQueryMethod
        where C1 : struct, IComponent
        where W1 : struct, IWhere<C1>
        where W2 : struct, IWhere<C1>
        where W3 : struct, IWhere<C1> {
        private uint[] m1;
        private C1[] d1;
        private W1 _where1;
        private W2 _where2;
        private W3 _where3;

        public Where(W1 where1, W2 where2, W3 where3) {
            _where1 = where1;
            _where2 = where2;
            _where3 = where3;
            m1 = default;
            d1 = default;
        }

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref C1 value) {
            return _where1.Match(ref value) && _where2.Match(ref value) && _where3.Match(ref value);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            World<WorldType>.Components<C1>.Value.AddBlocker(1);
            #endif
            m1 = World<WorldType>.Components<C1>.Value.GetDataIdxByEntityId();
            d1 = World<WorldType>.Components<C1>.Value.Data();
        }

        [MethodImpl(AggressiveInlining)]
        public bool CheckEntity(uint entityId) {
            var i1 = m1[entityId];
            return (i1 & Const.EmptyAndDisabledComponentMask) == 0 && Match(ref d1[i1 & Const.DisabledComponentMaskInv]);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.AddBlocker(-1);
        }
        #endif
    }


    #region WHERE_INT
    public struct WhereInt<T>  where T : struct, IComponent, IWhereValue<int> {
        public WhereIntEquals<T> Equals(int value) => new(value);
        public WhereIntNotEquals<T> NotEquals(int value) => new(value);
        public WhereIntGreaterThan<T> GreaterThan(int value) => new(value);
        public WhereIntGreaterOrEquals<T> GreaterThanOrEquals(int value) => new(value);
        public WhereIntLessThan<T> LessThan(int value) => new(value);
        public WhereLessOrEquals<T> LessThanOrEquals(int value) => new(value);
        public WhereBetween<T> Between(int fromIncluded, int toExcluded) => new(fromIncluded, toExcluded);
    }

    public delegate bool WherePredicate<T>(ref T value) where T : struct, IComponent;
    
    public readonly struct WhereAction<T> : IWhere<T> where T : struct, IComponent {
        private readonly WherePredicate<T> _condition;

        [MethodImpl(AggressiveInlining)]
        public WhereAction(WherePredicate<T> condition) => _condition = condition;

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) => _condition(ref value);
    }
    
    public readonly struct WhereIntEquals<T> : IWhere<T> where T : struct, IComponent, IWhereValue<int> {
        private readonly int _value;

        [MethodImpl(AggressiveInlining)]
        public WhereIntEquals(int value) => _value = value;

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) => value.Val() == _value;
    }
    
    public readonly struct WhereIntNotEquals<T> : IWhere<T> where T : struct, IComponent, IWhereValue<int> {
        private readonly int _value;

        [MethodImpl(AggressiveInlining)]
        public WhereIntNotEquals(int value) => _value = value;

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) => value.Val() != _value;
    }
    
    public readonly struct WhereIntGreaterThan<T> : IWhere<T> where T : struct, IComponent, IWhereValue<int> {
        private readonly int _value;

        [MethodImpl(AggressiveInlining)]
        public WhereIntGreaterThan(int value) => _value = value;

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) => value.Val() > _value;
    }
    
    public readonly struct WhereIntGreaterOrEquals<T> : IWhere<T> where T : struct, IComponent, IWhereValue<int> {
        private readonly int _value;

        [MethodImpl(AggressiveInlining)]
        public WhereIntGreaterOrEquals(int value) => _value = value;

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) => value.Val() >= _value;
    }
    
    public readonly struct WhereIntLessThan<T> : IWhere<T> where T : struct, IComponent, IWhereValue<int> {
        private readonly int _value;

        [MethodImpl(AggressiveInlining)]
        public WhereIntLessThan(int value) => _value = value;

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) => value.Val() < _value;
    }
    
    public readonly struct WhereLessOrEquals<T> : IWhere<T> where T : struct, IComponent, IWhereValue<int> {
        private readonly int _value;

        [MethodImpl(AggressiveInlining)]
        public WhereLessOrEquals(int value) => _value = value;

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) => value.Val() <= _value;
    }
    
    public readonly struct WhereBetween<T> : IWhere<T> where T : struct, IComponent, IWhereValue<int> {
        private readonly int _fromIncluded;
        private readonly int _toExcluded;
        
        [MethodImpl(AggressiveInlining)]
        public WhereBetween(int fromIncluded, int toExcluded) {
            _fromIncluded = fromIncluded;
            _toExcluded = toExcluded;
        }

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) {
            var val = value.Val();
            return val >= _fromIncluded && val < _toExcluded;
        }
    }
    #endregion

    #region WHERE_BOOL
    public struct WhereBool<T> where T : struct, IComponent, IWhereValue<bool> {
        public WhereBoolEquals<T> Is(bool value) => new(value);
    }
    
    public readonly struct WhereBoolEquals<T> : IWhere<T> where T : struct, IComponent, IWhereValue<bool> {
        private readonly bool _value;

        [MethodImpl(AggressiveInlining)]
        public WhereBoolEquals(bool value) => _value = value;

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) => value.Val() == _value;
    }
    #endregion

    #region WHERE_OBJECT
    public struct WhereObject<T, O> where T : struct, IComponent, IWhereValue<O> where O : class {
        public WhereObjectIsNull<T, O> IsNull() => new();
        public WhereObjectIsNotNull<T, O> IsNotNull() => new();
    }
    
    public readonly struct WhereObjectIsNull<T, O> : IWhere<T> where T : struct, IWhereValue<O> where O : class {
        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) => value.Val() == null;
    }
    
    public readonly struct WhereObjectIsNotNull<T, O> : IWhere<T> where T : struct, IWhereValue<O> where O : class {
        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) => value.Val() != null;
    }
    #endregion WHERE_OBJECT

    #region WHERE_STRING
    public struct WhereString<T> where T : struct, IWhereValue<string> {
        public WhereStringEquals<T> Equals(string value) => new(value);
        public WhereStringNotEquals<T> NotEquals(string value) => new(value);
        public WhereStringStartsWith<T> Contains(string value) => new(value);
        public WhereStringStartsWith<T> StartsWith(string value) => new(value);
        public WhereStringEndsWith<T> EndsWith(string value) => new(value);
        public WhereObjectIsNull<T, string> IsNull() => new();
        public WhereObjectIsNotNull<T, string> IsNotNull() => new();
    }
    
    public readonly struct WhereStringEquals<T> : IWhere<T> where T : struct, IWhereValue<string> {
        private readonly string _value;

        [MethodImpl(AggressiveInlining)]
        public WhereStringEquals(string value) => _value = value;

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) => _value == value.Val();
    }
    
    public readonly struct WhereStringNotEquals<T> : IWhere<T> where T : struct, IWhereValue<string> {
        private readonly string _value;

        [MethodImpl(AggressiveInlining)]
        public WhereStringNotEquals(string value) => _value = value;

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) => _value != value.Val();
    }
    
    public readonly struct WhereStringContains<T> : IWhere<T> where T : struct, IWhereValue<string> {
        private readonly string _value;

        [MethodImpl(AggressiveInlining)]
        public WhereStringContains(string value) => _value = value;

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) => value.Val()?.Contains(_value) ?? false;
    }
    
    public readonly struct WhereStringStartsWith<T> : IWhere<T> where T : struct, IWhereValue<string> {
        private readonly string _value;

        [MethodImpl(AggressiveInlining)]
        public WhereStringStartsWith(string value) => _value = value;

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) => value.Val()?.StartsWith(_value) ?? false;
    }
    
    public readonly struct WhereStringEndsWith<T> : IWhere<T> where T : struct, IWhereValue<string> {
        private readonly string _value;

        [MethodImpl(AggressiveInlining)]
        public WhereStringEndsWith(string value) => _value = value;

        [MethodImpl(AggressiveInlining)]
        public bool Match(ref T value) => value.Val()?.EndsWith(_value) ?? false;
    }
    #endregion
    
}