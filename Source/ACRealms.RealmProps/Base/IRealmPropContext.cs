using ACRealms.Prototypes;
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
    public interface IRealmPropContext { }

    /// <summary>
    /// An object representing either an entity, or a boxed, non-null object or ValueType
    /// </summary>
    public interface IRealmPropContext<T> : IRealmPropContext
    {
        TVal? FetchContextProperty<TVal>(string name) where TVal : struct;
    }
}

namespace ACRealms.RealmProps.Contexts
{
    public interface IContextEntity : IResolvableContext
    {
        sealed bool Match<T>(FrozenDictionary<string, IRealmPropertyScope> propsToMatch)
            where T : struct
        {
            return propsToMatch.Values.Cast<RealmPropertyEntityPropScope<IRealmPropertyScopeOps<T>, T>>().All(predicates =>
            {
                var propVal = FetchContextProperty<T>(predicates.Key);
                return predicates.OpsTyped.PredicatesTyped.All(p => p.Match(propVal));
            });
        }

        T? FetchContextProperty<T>(string key) where T : struct;

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

    public interface ICanonicalContextEntity : IContextEntity
    {
    }
    public interface ICanonicalContextEntity<TEntityIOCInterface, TEntity> : ICanonicalContextEntity, IRealmPropContext<TEntityIOCInterface>
        where TEntityIOCInterface : IContextEntity
        where TEntity : class, ICanonicalContextEntity<TEntityIOCInterface, TEntity>
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

        public TVal? FetchContextProperty<TVal>(string name)
            where TVal : struct
        {
            throw new NotImplementedException();
        }
    }
    internal abstract record Entity : ContextType { }
    internal record Entity<T> : Entity { }
    internal record DynamicEntity : Entity<dynamic> { }
}
