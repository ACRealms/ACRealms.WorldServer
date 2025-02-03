using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Prototypes
{
    /// <summary>
    /// An interface that represents a mapping between primitive types and enum types
    /// representing dictionary keys that can be used to fetch that primitive, from some arbitrary dictionary
    /// </summary>
    public interface IEnums
    {
        FrozenDictionary<Type, Type> IEnumsTypeMapping { get; }
    }
}
