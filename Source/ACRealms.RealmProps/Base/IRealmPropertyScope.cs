using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Rulesets.Contexts
{
    public interface IRealmPropertyScope
    {
        string Key { get; }
    }

    // Class wrapper around the IRealmPropertyScopeOps ref struct
    // This holds the scope filter criteria for a single entity property on a single ruleset property
    internal class RealmPropertyEntityPropScope<T, U> : IRealmPropertyScope
        where T : IRealmPropertyScopeOps<U>
    {
        public string Key { get; } // the name of the property on the entity corresponding to RespondsTo
        internal readonly RealmPropertyScopeOps<U> Ops;

        public RealmPropertyEntityPropScope(string key, RealmPropertyScopeOps<U> ops)
        {
            this.Key = key;
            this.Ops = ops;
        }
    }

    internal interface IPredicate { }
    internal interface IPredicate<T> : IPredicate
    {
        T Value { get; init; }

        bool Match(T valueToTest);
    }

    internal static class Predicates
    {
        internal readonly record struct Equal<T>(T value) : IPredicate<T>
            where T : struct, IEquatable<T>
        {
            public T Value { get; init; } = value;

            public bool Match(T valueToTest) => valueToTest.Equals(valueToTest);
        }
    }

    internal interface IRealmPropertyScopeOps { }
    internal interface IRealmPropertyScopeOps<T> { }
     //   where T : allows ref struct { }

    internal readonly struct RealmPropertyScopeOps<TVal> : IRealmPropertyScopeOps<TVal>
    {
        internal readonly ImmutableArray<IPredicate<TVal>> Predicates;

        public RealmPropertyScopeOps(ImmutableArray<IPredicate<TVal>> predicates) : this()
        {
            this.Predicates = predicates;
        }

        internal static RealmPropertyScopeOps<TVal> Create(ImmutableArray<IPredicate<TVal>> items)
        {
            return new RealmPropertyScopeOps<TVal>(items);
        }
    }
}
