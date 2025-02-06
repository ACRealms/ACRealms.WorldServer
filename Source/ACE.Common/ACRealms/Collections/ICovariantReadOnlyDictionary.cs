using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms
{
    public interface ICovariantReadOnlyDictionary<out TValue>
    {
        Type KeyType { get; }
    }

    public interface ICovariantReadOnlyDictionary<TKey, out TValue> : ICovariantReadOnlyDictionary<TValue>
    {
        TValue this[TKey key] { get; }
    }

    public static class DictionaryExtensions
    {
        public static CovariantReadOnlyDictionary<TKey, TSource> ToCovariantFrozenDictionary<TSource, TKey>(
            this IEnumerable<TSource> data, Func<TSource, TKey> keySelector)
            => new(data.ToFrozenDictionary(keySelector));
        public static CovariantReadOnlyDictionary<TKey, TValue> ToCovariantFrozenDictionary<TSource, TKey, TValue>(
            this IEnumerable<TSource> data, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector)
            => new(data.ToFrozenDictionary(keySelector, elementSelector));
        public static CovariantReadOnlyDictionary<TKey, TValue> ToCovariantFrozenDictionary<TKey, TValue>(this IDictionary<TKey, TValue> data)
            => data.ToCovariantFrozenDictionary(x => x.Key, x => x.Value);

        public static CovariantReadOnlyDictionary<TKey, TSource> ToCovariantReadOnlyDictionary<TSource, TKey>(
            this IEnumerable<TSource> data, Func<TSource, TKey> keySelector)
            => new(data.ToDictionary(keySelector).AsReadOnly());
        public static CovariantReadOnlyDictionary<TKey, TValue> ToCovariantReadOnlyDictionary<TSource, TKey, TValue>(
            this IEnumerable<TSource> data, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector)
            => new(data.ToDictionary(keySelector, elementSelector).AsReadOnly());
        public static CovariantReadOnlyDictionary<TKey, TValue> ToCovariantReadOnlyDictionary<TKey, TValue>(this IDictionary<TKey, TValue> data)
            => data.ToCovariantReadOnlyDictionary(x => x.Key, x => x.Value);
        public static CovariantReadOnlyDictionary<TKey, TValue> ToCovariantReadOnlyDictionary<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> data)
            => new(data);
    }

    public class CovariantReadOnlyDictionary<TKey, TValue> : ICovariantReadOnlyDictionary<TKey, TValue> 
    {
        protected IReadOnlyDictionary<TKey, TValue> _dictionary;
        public Type KeyType => typeof(TKey);

        public CovariantReadOnlyDictionary(IReadOnlyDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null) dictionary = FrozenDictionary<TKey, TValue>.Empty;
            _dictionary = dictionary;
        }

        public bool ContainsKey(TKey key) { return _dictionary.ContainsKey(key); }

        public IEnumerable<TKey> Keys { get { return _dictionary.Keys; } }

        public bool TryGetValue(TKey key, out TValue value)
            => _dictionary.TryGetValue(key, out value);
      
        public IEnumerable<TValue> Values { get { return _dictionary.Values.Cast<TValue>(); } }

        public TValue this[TKey key] { get { return _dictionary[key]; } }

        public int Count { get { return _dictionary.Count; } }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary
                        .Select(x => new KeyValuePair<TKey, TValue>(x.Key, x.Value))
                        .GetEnumerator();
        }

        /*
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }*/
    }


    public interface IDictionaryConvertibleKey<TUnderlyingKey>
    {
        bool TryFetchWithUnderlying(TUnderlyingKey key, out object result);
    }
    public interface IDictionaryConvertibleKeyStructValue<TUnderlyingKey>
    {
        bool TryFetchValueWithUnderlying(TUnderlyingKey key, out ValueType result);
    }

    public interface IDictionaryConvertibleKeyObjectValue<TUnderlyingKey>
    {
        bool TryFetchObjectWithUnderlying(TUnderlyingKey key, out object result);
    }

    public interface IDictionaryConvertibleKey<TRealKey, TUnderlyingKey, out TValue> : IDictionaryConvertibleKey<TUnderlyingKey>
    {
        TValue TryTypedFetchWithUnderlying(TUnderlyingKey key, out bool success);
    }

    public class CovariantReadOnlyDictionaryPolyKey<TRealKey, TUnderlyingKey, TValue>
        : CovariantReadOnlyDictionary<TRealKey, TValue>, IDictionaryConvertibleKey<TRealKey, TUnderlyingKey, TValue>
        where TRealKey : Enum
    {
        public CovariantReadOnlyDictionaryPolyKey(IReadOnlyDictionary<TRealKey, TValue> dictionary) : base(dictionary) { }

        static CovariantReadOnlyDictionaryPolyKey()
        {
            if (Enum.GetUnderlyingType(typeof(TRealKey)) != typeof(TUnderlyingKey))
                throw new InvalidOperationException("TRealKey's underlying type must be equal to TConvertibleKey");
        }

        public TValue TryTypedFetchWithUnderlying(TUnderlyingKey key, out bool result)
        { 
            var fetchResult = TryGetValue(Unsafe.As<TUnderlyingKey, TRealKey>(ref key), out var tResult);
            result = fetchResult;
            return tResult;
        }

        bool IDictionaryConvertibleKey<TUnderlyingKey>.TryFetchWithUnderlying(TUnderlyingKey key, out object result)
        { 
            result = TryTypedFetchWithUnderlying(key, out bool fetchResult);
            return fetchResult;
        }
    }

    public class CovariantReadOnlyDictionaryPolyKeyStructValue<TRealKey, TUnderlyingKey, TValue>
        : CovariantReadOnlyDictionaryPolyKey<TRealKey, TUnderlyingKey, TValue>, IDictionaryConvertibleKeyStructValue<TUnderlyingKey>
        where TRealKey : Enum
        where TValue : struct
    {
        public CovariantReadOnlyDictionaryPolyKeyStructValue(IReadOnlyDictionary<TRealKey, TValue> dictionary) : base(dictionary) { }

        bool IDictionaryConvertibleKeyStructValue<TUnderlyingKey>.TryFetchValueWithUnderlying(TUnderlyingKey key, out ValueType result)
        {
            result = TryTypedFetchWithUnderlying(key, out bool fetchResult);
            return fetchResult;
        }
    }

    public class CovariantReadOnlyDictionaryPolyKeyObjectValue<TRealKey, TUnderlyingKey, TValue>
    : CovariantReadOnlyDictionaryPolyKey<TRealKey, TUnderlyingKey, TValue>, IDictionaryConvertibleKeyObjectValue<TUnderlyingKey>
        where TRealKey : Enum
        where TValue : class
    {
        public CovariantReadOnlyDictionaryPolyKeyObjectValue(IReadOnlyDictionary<TRealKey, TValue> dictionary) : base(dictionary) { }

        bool IDictionaryConvertibleKeyObjectValue<TUnderlyingKey>.TryFetchObjectWithUnderlying(TUnderlyingKey key, out object result)
        {
            result = TryTypedFetchWithUnderlying(key, out bool fetchResult);
            return fetchResult;
        }
    }
}
