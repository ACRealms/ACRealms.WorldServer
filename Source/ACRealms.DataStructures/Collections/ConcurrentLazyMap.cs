using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ACRealms.DataStructures.Collections
{
    public class ConcurrentLazyAssociationMap<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        protected internal class MutexPool
        {
            private readonly ConcurrentDictionary<TKey, object> _locks;

            public object Get(TKey key) => _locks.GetOrAdd(key, _ => new object());

            internal MutexPool()
            {
                _locks = new ConcurrentDictionary<TKey, object>();
            }

            internal MutexPool(IEqualityComparer<TKey> equalityComparer)
            {
                _locks = new ConcurrentDictionary<TKey, object>(equalityComparer);
            }
        }

        protected MutexPool mutexes = new MutexPool();
        protected readonly DefaultingConcurrentDictionary<TKey, Lazy<IImmutableList<TValue>>> internalDictionary;
        public ConcurrentLazyAssociationMap()
        {
            mutexes = new MutexPool();
            internalDictionary = new DefaultingConcurrentDictionary<TKey, Lazy<IImmutableList<TValue>>>(new Lazy<IImmutableList<TValue>>(() => ImmutableArray.Create<TValue>(), LazyThreadSafetyMode.PublicationOnly));
        }

        public ConcurrentLazyAssociationMap(IEqualityComparer<TKey> equalityComparer)
        {
            mutexes = new MutexPool(equalityComparer);
            internalDictionary = new DefaultingConcurrentDictionary<TKey, Lazy<IImmutableList<TValue>>>(new Lazy<IImmutableList<TValue>>(() => ImmutableArray.Create<TValue>(), LazyThreadSafetyMode.PublicationOnly), equalityComparer);
        }

        public Lazy<IImmutableList<TValue>> this[TKey key]
        {
            get => internalDictionary[key]!;
            set => internalDictionary[key] = value;
        }

        public bool ContainsKey(TKey key) => internalDictionary.ContainsKey(key);

        public Lazy<IImmutableList<TValue>> GetOrInitializeValues(TKey key)
        {
            if (internalDictionary.TryGetValue(key, out var values))
                return values;

            lock (mutexes.Get(key))
            {
                return internalDictionary.GetOrAdd(key, (_) =>
                    new Lazy<IImmutableList<TValue>>(() => ImmutableArray.Create<TValue>(), LazyThreadSafetyMode.PublicationOnly)
                );
            }
        }

        public bool TryGetValue(TKey key, out Lazy<IImmutableList<TValue>> value)
            => internalDictionary.TryGetValue(key, out value!);

        public void Add(TKey key, TValue value)
        {
            lock (mutexes.Get(key))
            {
                var lazyValues = GetOrInitializeValues(key);
                internalDictionary[key] = new Lazy<IImmutableList<TValue>>(() =>
                {
                    var data = lazyValues.Value;
                    if (data.Count >= 15 && data is ImmutableArray<TValue> ary)
                    {
                        // Convert to ImmutableList which is more efficient for adding, less efficient for retrieving
                        // If we already have 15, we'll likely get more
                        // Official docs recommend a threshold of 16 or more items for ImmutableList
                        // https://learn.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutablearray-1?view=net-8.0
                        return ary.ToImmutableList().Add(value);
                    }
                    else
                        return data.Add(value);
                }, LazyThreadSafetyMode.PublicationOnly);
            }
        }

        public void Remove(TKey key, TValue value)
        {
            lock (mutexes.Get(key))
            {
                var lazyValues = GetOrInitializeValues(key);
                internalDictionary[key] = new Lazy<IImmutableList<TValue>>(() =>
                {
                    var data = lazyValues.Value;
                    if (data.Count <= 16 && data is ImmutableList<TValue> list)
                    {
                        // Convert back to ImmutableArray
                        return list.Remove(value).ToImmutableArray();
                    }
                    else
                        return data.Remove(value);
                }, LazyThreadSafetyMode.PublicationOnly);
            }
        }
    }
}
