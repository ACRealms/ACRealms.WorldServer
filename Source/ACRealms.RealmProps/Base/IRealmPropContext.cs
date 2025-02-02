using ACRealms.RealmProps;
using ACRealms.RealmProps.Contexts;
using ACRealms.Rulesets.Contexts;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms
{
    public interface IRealmPropContext
    {

    }

    /// <summary>
    /// An object representing either an entity, or a boxed, non-null object or ValueType
    /// </summary>
    public interface IRealmPropContext<T> : IRealmPropContext
    {
        // This should probably be improved, it may be an inefficient implementation, but leave it for now
        sealed bool Match(FrozenDictionary<string, IRealmPropertyScope> propsToMatch)
        {
            return propsToMatch.Values.Cast<RealmPropertyEntityPropScope<IRealmPropertyScopeOps<T>, T>>().All(predicates =>
            {
                var propVal = FetchContextProperty(predicates.Key);
                return predicates.Ops.Predicates.All(p => p.Match(propVal));
            });
        }

        T FetchContextProperty(string name);

        //static abstract bool RespondsTo(string key);
        //static virtual bool RespondsTo(string key) => false;


    }
}

namespace ACRealms.RealmProps.Contexts
{
    public interface IContextEntity
    {
        /// <summary>
        /// Returns true if the given key can be used to fetch a value during scoped rule evaluation
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static virtual bool RespondsTo(string key) => throw new NotImplementedException();

        /// <summary>
        /// Given the key, returns the type returned by a property.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        static virtual Type TypeOfProperty(string key) => throw new NotImplementedException();
    }

    public interface ICanonicalContextEntity { }
    public interface ICanonicalContextEntity<T, U> : ICanonicalContextEntity
        where T : IContextEntity
        where U : class, ICanonicalContextEntity<T, U>
    {
        /// <summary>
        /// Returns true if the given key can be used to fetch a value during scoped rule evaluation
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static abstract bool RespondsTo(string key);// => throw new NotImplementedException();

        /// <summary>
        /// Given the key, returns the type returned by a property.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        static abstract Type TypeOfProperty(string key);// => throw new NotImplementedException();
    }

    public interface IContextEntity<T> : IRealmPropContext<T>
        where T : class
    {

    }

    public interface IWorldObjectContextEntity : IContextEntity
    {
        
    }
    public interface IWorldObjectContextEntity<T> : IWorldObjectContextEntity, IContextEntity<T>
        where T : class, IWorldObjectContextEntity<T>
    {
    }


    internal abstract record ContextType { }
    internal abstract record Val : ContextType { }
    internal record Val<T> : Val, IContextEntity<Val<T>>
    {
        internal T Value { get; }

        public static bool RespondsTo(string key) => key == "Value";

        public Val<T> FetchContextProperty(string key)
        {
            if (key != "Value")
                throw new InvalidOperationException("Only the 'Value' key is supported for this class");
            return this;
        }
    }
    internal abstract record Entity : ContextType { }
    internal record Entity<T> : Entity { }
    internal record DynamicEntity : Entity<dynamic> { }
    internal static class Entities
    {
        internal record Creature : Entity { }
    }


    /*
    internal abstract record RealmPropContext
    {
        internal RealmPropContext FromAttribute<CtxType>(ScopedWithAttribute<CtxType> attribute)
            where CtxType : RealmProps.Contexts.ContextType
        {
            return attribute.Entity switch
            {
                "Float" => new RealmPropContextWithScalar<float>(),
                "Creature" => new RealmPropContextWithEntity<IWorldObjectContextEntity>(),
                _ => throw new NotImplementedException()
            };
        }
    }

    internal abstract record RealmPropContextWithEntity : RealmPropContext
    {
    }

    internal record RealmPropContextWithEntity<TEntity> : RealmPropContextWithEntity
    {
    }

    internal abstract record RealmPropContextWithScalar : RealmPropContext
    {
    }

    internal record RealmPropContextWithScalar<TVal> : RealmPropContextWithScalar
    {
    }

    internal interface IRealmPropContext<TEntity>
    {
    }

    internal interface IRealmPropScalarContext<TValue> : IRealmPropContext
    {
    }
    */
}
