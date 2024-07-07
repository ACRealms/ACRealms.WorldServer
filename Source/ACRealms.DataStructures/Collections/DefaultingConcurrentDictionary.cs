using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.DataStructures.Collections
{
    /// <summary>
    /// A ConcurrentDictionary where fetching a value with a missing key does not throw an exception
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DefaultingConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>,
        ICollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>,
        IEnumerable,
        IDictionary<TKey, TValue>,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
        IReadOnlyDictionary<TKey, TValue>,
        ICollection,
        IDictionary
        where TKey : notnull
        where TValue : notnull, new()
    {
        readonly Func<TValue> defaultValueFactory;

        private DefaultingConcurrentDictionary() { defaultValueFactory = () => new TValue(); }

        public DefaultingConcurrentDictionary(TValue defaultValue)
        {
            defaultValueFactory = () => defaultValue;
        }

        public DefaultingConcurrentDictionary(Func<TValue> defaultValueFactory)
        {
            this.defaultValueFactory = defaultValueFactory;
        }

        public DefaultingConcurrentDictionary(TValue defaultValue, IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
            defaultValueFactory = () => defaultValue;
        }

        public DefaultingConcurrentDictionary(Func<TValue> defaultValueFactory, IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
            this.defaultValueFactory = defaultValueFactory;
        }

        public new bool TryAdd(TKey key, TValue value)
        {
            Put(key, value);
            return true;
        }

        public TValue Put(TKey key, TValue value)
        {
            if (value == null)
            {
                TryRemove(key, out _);
                return defaultValueFactory();
            }

            return AddOrUpdate(key, value, (k, _) => value);
        }

        public new TValue this[TKey key]
        {
            get => base.TryGetValue(key, out TValue? value) ? value! : defaultValueFactory();
            set => Put(key, value);
        }

        public new bool TryGetValue(TKey key, out TValue value)
        {
            if (base.TryGetValue(key, out TValue? foundValue))
            {
                value = foundValue!;
                return true;
            }
            value = defaultValueFactory();
            return false;
        }
    }
}
