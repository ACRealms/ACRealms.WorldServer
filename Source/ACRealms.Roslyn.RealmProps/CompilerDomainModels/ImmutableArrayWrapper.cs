using System;
using System.Collections.Generic;
using System.Text;

namespace ACRealms.Roslyn.RealmProps.CompilerDomainModels
{
    // This class may not be necessary, but decided to leave it in here
    internal sealed class ImmutableArrayWrapper<T> : IEquatable<ImmutableArrayWrapper<T>>
    {
        public ImmutableArray<T> Array { get; }

        public ImmutableArrayWrapper(ImmutableArray<T> array)
        {
            Array = array;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ImmutableArrayWrapper<T>);
        }

        public bool Equals(ImmutableArrayWrapper<T>? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (Array.Length != other.Array.Length)
                return false;

            for (int i = 0; i < Array.Length; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(Array[i], other.Array[i]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            foreach (T? item in Array)
            {
                hash.Add(item);
            }

            return hash.ToHashCode();
        }
    }
}
