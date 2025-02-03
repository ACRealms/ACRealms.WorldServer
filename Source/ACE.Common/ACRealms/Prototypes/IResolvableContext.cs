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
    }

    public interface IResolvableContext<TProtos, TUnderlying> : IResolvableContext
        where TProtos : IPrototypes
        where TUnderlying : IResolvableContext<TProtos, TUnderlying>
    {
        new TProtos Prototypes => (TProtos)((IResolvableContext)(this)).Prototypes;
        TUnderlying UnderlyingContext { get; }
    }
}
