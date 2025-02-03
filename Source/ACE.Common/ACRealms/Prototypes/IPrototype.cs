using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Prototypes
{
    public interface IPrototype
    {
        string PropName { get; }
        abstract object Fetch(IResolvableContext entity);// => throw new NotImplementedException();
    }

    public interface IPrototype<TVal> : IPrototype where TVal : struct
    {
        TVal? Fetch(IResolvableContext entity);
        virtual object FetchSlow(IResolvableContext entity) => Fetch(entity);
    }

    public interface IPrototype<TEnum, TVal> : IPrototype<TVal>
        where TVal : struct
        where TEnum : Enum
    {
    }

    public interface IPrototype<TEnum, TVal, TEntity, TProtos> : IPrototype<TEnum, TVal>
        where TVal : struct
        where TEnum : Enum
        where TEntity : class, IResolvableContext
        where TProtos : IPrototypes<IPrototype<TEnum, TVal, TEntity, TProtos>>
    {
    }
}
