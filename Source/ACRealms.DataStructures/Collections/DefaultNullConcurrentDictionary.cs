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
    public class DefaultNullConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue?>,
        ICollection<KeyValuePair<TKey, TValue?>>,
        IEnumerable<KeyValuePair<TKey, TValue?>>,
        IEnumerable,
        IDictionary<TKey, TValue?>,
        IReadOnlyCollection<KeyValuePair<TKey, TValue?>>,
        IReadOnlyDictionary<TKey, TValue?>,
        ICollection,
        IDictionary
        where TKey : notnull
        where TValue : class
    {

        public DefaultNullConcurrentDictionary() { }
        public DefaultNullConcurrentDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer) { }

        public new bool TryAdd(TKey key, TValue? value)
        {
            Put(key, value);
            return true;
        }

        public TValue? Put(TKey key, TValue? value)
        {
            if (value == null)
            {
                TryRemove(key, out _);
                return null;
            }

            return AddOrUpdate(key, value, (k, _) => value);
        }

        public new TValue? this[TKey key]
        {
            get => base.TryGetValue(key, out TValue? value) ? value : null;
            set => Put(key, value);
        }

        public new bool TryGetValue(TKey key, out TValue? value)
        {
            if (base.TryGetValue(key, out value!))
                return true;
            value = null;
            return false;
        }
    }
}
