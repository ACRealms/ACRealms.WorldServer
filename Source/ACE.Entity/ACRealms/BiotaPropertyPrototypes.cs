using ACE.Entity.Enum.Properties;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Entity.ACRealms
{
    public class BiotaPropertyPrimaryAttribute<TPrimitive> : global::ACRealms.Prototypes.PropertyPrimaryAttributeBase { }

    public static class BiotaPropertyPrototypes
    {
        private static readonly ImmutableArray<(Type, Type)> EnumTypes = [
            (typeof(PropertyInt), typeof(int?)),
            (typeof(PropertyInt64), typeof(long?)),
            (typeof(PropertyFloat), typeof(double?)),
            (typeof(PropertyBool), typeof(bool?)),
            (typeof(PropertyString), typeof(string))];
        private static readonly IReadOnlyDictionary<string, BiotaPropertyPrototype> AllBiotaPrototypes = BuildPrototypes();

        public static BiotaPropertyPrototype GetPrototype(string key) => AllBiotaPrototypes[key];

        // If true, server will fail to startup if properties with the same key exist in more than one type
        // Should address this eventually but adding this bypass flag for now
        private static readonly bool PropsMustBeUnique = false;

        private static FrozenDictionary<string, BiotaPropertyPrototype> BuildPrototypes()
        {
            var dict = new Dictionary<string, BiotaPropertyPrototype>();
            foreach(var (enumType, valType) in EnumTypes)
            {
                var prototypeType = typeof(BiotaPropertyPrototype<,>).MakeGenericType(enumType, valType);
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
            return dict.ToFrozenDictionary();
        }
    }

    public abstract record BiotaPropertyPrototype(Type EnumType, Type ValueType, string PropName, ushort PropRawKey)
    {
    }

    public record BiotaPropertyPrototype<TEnum, TVal>(string PropName, ushort PropRawKey, TEnum PropKey) : BiotaPropertyPrototype(typeof(TEnum), typeof(TVal), PropName, PropRawKey)
        where TEnum : System.Enum
    {
    }
}
