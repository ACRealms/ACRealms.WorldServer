using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Prototypes
{
    public interface IPrototypes : IEnums
    {
        ICovariantReadOnlyDictionary<string, IPrototype> AllPrototypes { get; }
        static IPrototypes Instance { get; }
        IPrototype GetPrototype(string key);
        IPrototype<TVal> GetPrototype<TVal>(string key) where TVal : struct => (IPrototype<TVal>)GetPrototype(key);
    }

    public interface IPrototypes<in TPrototype> : IPrototypes
        where TPrototype : IPrototype
    {
    }
}
