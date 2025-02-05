using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Rulesets.Contexts
{
    internal interface IRealmPropertyScope
    {
        public bool MatchObject(object val);
        public bool MatchValue(ValueType val);
        public bool MatchNull();
    }

    // Wrapper around the IRealmPropertyScopeOps ref struct
    // This holds the scope filter criteria for a single entity property on a single ruleset property
    internal abstract record RealmPropertyEntityPropScope<TOps, TVal> : IRealmPropertyScope
        where TOps : IRealmPropertyScopeOps<TVal>
    {
        public string Key { get; } // the name of the property on the entity corresponding to RespondsTo
        public RealmPropertyScopeOps<TVal> Ops { get; }

        public RealmPropertyEntityPropScope(string key, RealmPropertyScopeOps<TVal> ops)
        {
            this.Key = key;
            this.Ops = ops;
        }

        public abstract bool MatchObject(object val);
        public abstract bool MatchValue(ValueType val);
        public bool MatchNull() => Ops.MatchNull();
    }

    internal record RealmPropertyEntityObjectPropScope<TOps, TVal> : RealmPropertyEntityPropScope<TOps, TVal>
    where TOps : IRealmPropertyScopeOps<TVal>
    where TVal : class
    {
        public RealmPropertyEntityObjectPropScope(string key, RealmPropertyScopeOps<TVal> ops)
            : base(key, ops) { }

        public override bool MatchObject(object val) => Ops.Match((TVal)val);

        [DoesNotReturn]
        public override bool MatchValue(ValueType val)
            => throw new InvalidOperationException("Must use MatchObject instead on this type");
    }

    internal record RealmPropertyEntityValuePropScope<TOps, TVal> : RealmPropertyEntityPropScope<TOps, TVal>
        where TOps : IRealmPropertyScopeOps<TVal>
        where TVal : struct
    {
        public RealmPropertyEntityValuePropScope(string key, RealmPropertyScopeOps<TVal> ops)
            : base(key, ops) { }

        [DoesNotReturn]
        public override bool MatchObject(object val)
            => throw new InvalidOperationException("Must use MatchValue instead on this type");

        public override bool MatchValue(ValueType val) => Ops.Match((TVal)val);
    }

    public interface IPredicate { }
    public interface IPredicate<T> : IPredicate
    {
        T Value { get; init; }

        bool Match(T valueToTest);

        bool MatchNull();
    }

    public static class Predicates
    {
        public readonly record struct Equal<T>(T Value) : IPredicate<T>
            where T : IEquatable<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool Match(T valueToTest) => Value.Equals(valueToTest);


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool MatchNull() => Value.Equals(null);
        }

        public readonly record struct NotEqual<T>(T Value) : IPredicate<T>
            where T : IEquatable<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool Match(T valueToTest) => !Value.Equals(valueToTest);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool MatchNull() => !Value.Equals(null);
        }

        public readonly record struct GreaterThan<T>(T Value) : IPredicate<T>
            where T : IComparable<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool Match(T valueToTest) => valueToTest != null && valueToTest.CompareTo(Value) > 0;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool MatchNull() => false;
        }

        public readonly record struct GreaterThanOrEqual<T>(T Value) : IPredicate<T>
            where T : IComparable<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool Match(T valueToTest) =>
                valueToTest != null && valueToTest.CompareTo(Value) >= 0;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool MatchNull() => false;
        }

        public readonly record struct LessThan<T>(T Value) : IPredicate<T>
            where T : IComparable<T>
        {

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool Match(T valueToTest) => valueToTest != null && valueToTest.CompareTo(Value) < 0;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool MatchNull() => false;
        }

        public readonly record struct LessThanOrEqual<T>(T Value) : IPredicate<T>
            where T : IComparable<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool Match(T valueToTest) => valueToTest != null && valueToTest.CompareTo(Value) <= 0;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool MatchNull() => false;
        }
    }

    public interface IRealmPropertyScopeOps { }
    public interface IRealmPropertyScopeOps<T> : IRealmPropertyScopeOps { }

    internal readonly record struct RealmPropertyScopeOps<TVal> : IRealmPropertyScopeOps<TVal>
    {
        readonly ImmutableArray<IPredicate<TVal>> Predicates;

        internal RealmPropertyScopeOps(ImmutableArray<IPredicate<TVal>> predicates) : this()
        {
            this.Predicates = predicates;
        }

        internal static RealmPropertyScopeOps<TVal> Create(ImmutableArray<IPredicate<TVal>> items)
        {
            return new RealmPropertyScopeOps<TVal>(items);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Match(TVal val) => Predicates.All(p => p.Match(val));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool MatchNull() => Predicates.All(p => p.MatchNull());
    }
}
