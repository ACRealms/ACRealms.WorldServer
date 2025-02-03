using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Rulesets.Contexts
{
    public interface IRealmPropertyScope
    {
        string Key { get; }

        bool Match<T>(IPredicate<T> predicate) where T : struct;
        public bool Match<TVal>(TVal? val);
        IRealmPropertyScopeOps Ops { get; }
    }

    // Class wrapper around the IRealmPropertyScopeOps ref struct
    // This holds the scope filter criteria for a single entity property on a single ruleset property
    internal class RealmPropertyEntityPropScope<TOps, TVal> : IRealmPropertyScope
        where TOps : IRealmPropertyScopeOps<TVal>
        where TVal : struct
    {
        public string Key { get; } // the name of the property on the entity corresponding to RespondsTo

        public RealmPropertyScopeOps<TVal> OpsTyped { get; }

        public IRealmPropertyScopeOps Ops => OpsTyped;

        public RealmPropertyEntityPropScope(string key, RealmPropertyScopeOps<TVal> ops)
        {
            this.Key = key;
            this.OpsTyped = ops;
        }

        public bool Match(TVal? val) => OpsTyped.PredicatesTyped.All(p => p.Match(val));

        public bool Match<T>(IPredicate<T> predicate) where T : struct
        {
            throw new NotImplementedException();
        }

        public bool Match<TVal1>(TVal1 val)
        {
            TVal? compatVal = (TVal?)(object?)val;
            return OpsTyped.PredicatesTyped.All(p => p.Match(compatVal));
        }
    }

    public interface IPredicate { }
    public interface IPredicate<T> : IPredicate
        where T : struct
    {
        T? Value { get; init; }

        bool Match(T? valueToTest);
    }

    public static class Predicates
    {
        public readonly record struct Equal<T>(T? Value) : IPredicate<T>
            where T : struct, IEquatable<T>
        {
            public bool Match(T? valueToTest) => Value.Equals(valueToTest);
        }
    }

    public interface IRealmPropertyScopeOps
    {
         ImmutableArray<IPredicate> Predicates { get; }
    }
    public interface IRealmPropertyScopeOps<T> : IRealmPropertyScopeOps { }

    public readonly struct RealmPropertyScopeOps<TVal> : IRealmPropertyScopeOps<TVal>
        where TVal : struct
    {
        public ImmutableArray<IPredicate<TVal>> PredicatesTyped { get; }

        public ImmutableArray<IPredicate> Predicates { get; }

        public RealmPropertyScopeOps(ImmutableArray<IPredicate<TVal>> predicates) : this()
        {
            this.PredicatesTyped = predicates;
            Predicates = PredicatesTyped.Cast<IPredicate>().ToImmutableArray(); // TODO: Fix this, only 1 array
        }

        internal static RealmPropertyScopeOps<TVal> Create(ImmutableArray<IPredicate<TVal>> items)
        {
            return new RealmPropertyScopeOps<TVal>(items);
        }
    }
}
