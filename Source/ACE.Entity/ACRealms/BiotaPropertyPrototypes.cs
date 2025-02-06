using ACE.Entity.Enum.Properties;
using ACRealms.Prototypes;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ACRealms;
using ACE.Entity.Models;

namespace ACE.Entity.ACRealms
{
    public class BiotaPropertyPrimaryAttribute<TPrimitive> : global::ACRealms.Prototypes.PropertyPrimaryAttributeBase { }

    public class BiotaPropertyPrototypes : IPrototypes<IPrototype>
    {
        private static ImmutableArray<(Type, Type)> EnumTypes { get; } = [
            (typeof(PropertyInt), typeof(int)),
            (typeof(PropertyInt64), typeof(long)),
            (typeof(PropertyFloat), typeof(double)),
            (typeof(PropertyBool), typeof(bool)),
            (typeof(PropertyString), typeof(string))
        ];
        public static IPrototypes Instance { get; } = new BiotaPropertyPrototypes();
        public FrozenDictionary<Type, Type> IEnumsTypeMapping { get; private init; } = EnumTypes.ToFrozenDictionary(x => x.Item1, x => x.Item2);

        public ICovariantReadOnlyDictionary<string, IPrototype> AllPrototypes { get; } = BuildPrototypes();

        public IPrototype GetPrototype(string key) => AllPrototypes[key];

        // If true, server will fail to startup if properties with the same key exist in more than one type
        // Should address this eventually but adding this bypass flag for now
        private static readonly bool PropsMustBeUnique = false;

        private static ICovariantReadOnlyDictionary<string, IPrototype> BuildPrototypes()
        {
            var dict = new Dictionary<string, BiotaPropertyPrototype>();
            foreach (var (enumType, valType) in EnumTypes)
            {
                Type prototypeType;
                if (valType.IsClass)
                    prototypeType = typeof(BiotaPropertyObjectPrototype<,>).MakeGenericType(enumType, valType);
                else
                    prototypeType = typeof(BiotaPropertyValuePrototype<,>).MakeGenericType(enumType, valType);

                foreach (var propName in System.Enum.GetNames(enumType))
                {
                    var propKey = System.Enum.Parse(enumType, propName);
                    var propRawKey = (ushort)propKey;

                    if (propRawKey != 0)
                    {
                        var proto = (BiotaPropertyPrototype)Activator.CreateInstance(
                            prototypeType,
                            BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance,
                            binder: null,
                            args: [propName, propRawKey, propKey],
                            System.Globalization.CultureInfo.InvariantCulture,
                            activationAttributes: null
                        );
                        if (PropsMustBeUnique)
                            dict.Add(propName, proto);
                        else
                            dict.TryAdd(propName, proto);
                    }
                }
            }
            return (ICovariantReadOnlyDictionary<string, IPrototype>)dict.ToCovariantFrozenDictionary();
        }
    }

    public abstract record BiotaPropertyPrototype(Type EnumType, Type ValueType, string PropName, ushort PropRawKey)
        : IPrototype
    {
        public ValueType UntypedRawKey => PropRawKey;
    }

    public abstract record BiotaPropertyPrototype<TVal>(Type EnumType, string PropName, ushort PropRawKey)
        : BiotaPropertyPrototype(EnumType, typeof(TVal), PropName, PropRawKey) { }

    public abstract record BiotaPropertyPrototype<TEnum, TVal>(string PropName, ushort PropRawKey, TEnum PropKey)
        : BiotaPropertyPrototype<TVal>(typeof(TEnum), PropName, PropRawKey), IPrototype<TEnum, TVal, Biota, BiotaPropertyPrototypes>
        where TEnum : System.Enum { }

    public record BiotaPropertyObjectPrototype<TEnum, TVal>(string PropName, ushort PropRawKey, TEnum PropKey)
       : BiotaPropertyPrototype<TVal>(typeof(TEnum), PropName, PropRawKey),
       IObjectPrototype<TEnum, TVal, Biota, BiotaPropertyPrototypes>
       where TEnum : System.Enum
       where TVal : class
    {
    }

    // We want to avoid boxing of ValueTypes, so we have separate prototype for struct vs class values
    public record BiotaPropertyValuePrototype<TEnum, TVal>(string PropName, ushort PropRawKey, TEnum PropKey)
        : BiotaPropertyPrototype<TVal>(typeof(TEnum), PropName, PropRawKey),
        IValuePrototype<TEnum, TVal, Biota, BiotaPropertyPrototypes>
        where TEnum : System.Enum
        where TVal : struct
    {
    }
}
