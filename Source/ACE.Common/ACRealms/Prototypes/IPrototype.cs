using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Prototypes
{
    public interface IPrototype
    {
        ValueType UntypedRawKey { get; }
        Type ValueType { get; }
        string PropName { get; }
    }

    public interface IValuePrototype : IPrototype
    {
        sealed bool TryFetchValue(IResolvableContext entity, out ValueType value)
            => entity.UnderlyingContext.TryFetchValue(this, out value);
    }
    public interface IObjectPrototype : IPrototype
    {
        sealed bool TryFetchObject(IResolvableContext entity, out object value)
            => entity.UnderlyingContext.TryFetchObject(this, out value);
    }

    public interface IPrototype<TVal> : IPrototype
    {
    }

    public interface IValuePrototype<TVal> : IValuePrototype where TVal : struct { }
    public interface IObjectPrototype<TVal> : IObjectPrototype where TVal : class { }

    public interface IPrototype<TEnum, TVal> : IPrototype<TVal>
        where TEnum : Enum
    {
    }

    public interface IPrototype<TEnum, TVal, TEntity, TProtos> : IPrototype<TEnum, TVal>
        where TEnum : Enum
        where TEntity : class, IResolvableContext<TProtos, TEntity>
        where TProtos : IPrototypes<IPrototype<TEnum, TVal, TEntity, TProtos>>
    {
    }

    public interface IValuePrototype<TEnum, TVal, TEntity, TProtos>
        : IValuePrototype, IPrototype<TEnum, TVal, TEntity, TProtos>
        where TEnum : Enum
        where TVal : struct
        where TEntity : class, IResolvableContext<TProtos, TEntity>
        where TProtos : IPrototypes<IPrototype<TEnum, TVal, TEntity, TProtos>>
    {
    }

    public interface IObjectPrototype<TEnum, TVal, TEntity, TProtos>
    : IPrototype<TEnum, TVal, TEntity, TProtos>, IObjectPrototype
    where TEnum : Enum
    where TVal : class
    where TEntity : class, IResolvableContext<TProtos, TEntity>
    where TProtos : IPrototypes<IPrototype<TEnum, TVal, TEntity, TProtos>>
    {
    }
}
