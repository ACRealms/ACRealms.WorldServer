using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Prototypes
{
    public interface IResolvableContext
    {
        IPrototypes Prototypes { get; }
        IResolvableContext UnderlyingContext { get; }
        bool TryFetchValue(IPrototype prototype, out ValueType result);
        bool TryFetchObject(IPrototype prototype, out object result);
    }

    public interface IResolvableContext<TProtos, TUnderlying> : IResolvableContext
        where TProtos : IPrototypes
        where TUnderlying : IResolvableContext<TProtos, TUnderlying>
    {
    }
}
