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

namespace ACRealms.RealmProps.Contexts
{
    public interface IContextEntity : IResolvableContext
    {
        /// <summary>
        /// Returns true if the given key can be used to fetch a value during scoped rule evaluation
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static virtual bool RespondsTo(string key) => false;

        /// <summary>
        /// Given the key, returns the type returned by a property.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static virtual Type? TypeOfProperty(string key) => null;
    }

    public interface ICanonicalContextEntity : IContextEntity { }
    public interface ICanonicalContextEntity<TEntityIOCInterface, TEntity>
        : ICanonicalContextEntity, IResolvableContext
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

    public interface IWorldObjectContextEntity : IContextEntity, ICanonicalContextEntity { }

    internal abstract record ContextType { }
    internal abstract record Val : ContextType { }
    internal record Val<T> : Val, IContextEntity
    {
        public IPrototypes Prototypes => throw new NotImplementedException();

        public IResolvableContext UnderlyingContext => throw new NotImplementedException();

        internal T Value { get; }

        public static bool RespondsTo(string key) => key == "Value";

        public Val<T> FetchContextProperty(string key)
        {
            if (key != "Value")
                throw new InvalidOperationException("Only the 'Value' key is supported for this class");
            return this;
        }

        public TVal? FetchContextProperty<TVal>(string name)
        {
            throw new NotImplementedException();

        }

        public bool TryFetchObject(IPrototype prototype, out object result)
        {
            throw new NotImplementedException();
        }

        public bool TryFetchValue(IPrototype prototype, out ValueType result)
        {
            throw new NotImplementedException();
        }
    }
}
