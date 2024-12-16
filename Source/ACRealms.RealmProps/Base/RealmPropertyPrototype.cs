using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
#nullable enable

using SecondaryDict = System.Collections.Frozen.FrozenDictionary<System.Type, ACRealms.RealmProps.RealmPropertySecondaryAttributeBase>;

namespace ACRealms.RealmProps
{
    internal static class RealmPropertyPrototypes
    {
        public static FrozenDictionary<RealmPropertyBool, RealmPropertyBoolPrototype> Bool { get; } = RealmPropertyHelper.BuildPrototypes<RealmPropertyBool, RealmPropertyBoolPrototype, bool, RealmPropertyPrimaryAttribute<bool>>();
        public static FrozenDictionary<RealmPropertyInt, RealmPropertyIntPrototype> Int { get; } = RealmPropertyHelper.BuildPrototypes<RealmPropertyInt, RealmPropertyIntPrototype, int, RealmPropertyPrimaryMinMaxAttribute<int>>();
        public static FrozenDictionary<RealmPropertyInt64, RealmPropertyInt64Prototype> Int64 { get; } = RealmPropertyHelper.BuildPrototypes<RealmPropertyInt64, RealmPropertyInt64Prototype, long, RealmPropertyPrimaryMinMaxAttribute<long>>();
        public static FrozenDictionary<RealmPropertyFloat, RealmPropertyFloatPrototype> Float { get; } = RealmPropertyHelper.BuildPrototypes<RealmPropertyFloat, RealmPropertyFloatPrototype, double, RealmPropertyPrimaryMinMaxAttribute<double>>();
        public static FrozenDictionary<RealmPropertyString, RealmPropertyStringPrototype> String { get; } = RealmPropertyHelper.BuildPrototypes<RealmPropertyString, RealmPropertyStringPrototype, string, RealmPropertyPrimaryAttribute<string>>();
    }

    internal abstract class RealmPropertyPrototypeBase
    {
        internal int RawIdentifier { get; private init; }
        private SecondaryDict? SecondaryAttributes { get; init; }
        public string SerializedHardDefaultValue { get; private init; }

        protected internal RealmPropertyPrototypeBase(int rawIdentifier, SecondaryDict? secondaryAttributes, string serializedHardDefaultValue)
        {
            RawIdentifier = rawIdentifier;
            SecondaryAttributes = secondaryAttributes;
            SerializedHardDefaultValue = serializedHardDefaultValue;
        }

        public bool TryGetSecondaryValue<TSecondary, TResult>(Func<TSecondary, TResult> selector, out TResult? result)
            where TSecondary : RealmPropertySecondaryAttributeBase
        {
            if (SecondaryAttributes == null)
            {
                result = default;
                return false;
            }
            var t = typeof(TSecondary);
            if (!SecondaryAttributes!.ContainsKey(t))
            {
                result = default;
                return false;
            }

            var att = (TSecondary)SecondaryAttributes[t];
            result = selector(att);
            return true;
        }
    }

    internal abstract class RealmPropertyPrototype<TPrimitive> : RealmPropertyPrototypeBase
    where TPrimitive : IEquatable<TPrimitive>
    {
        public TPrimitive HardDefaultValue { get; init; }
        protected internal RealmPropertyPrototype(int rawIdentifier, SecondaryDict? secondaryAttributes, TPrimitive hardDefaultValue)
            : base(rawIdentifier, secondaryAttributes, hardDefaultValue?.ToString() ?? "<null>")
        {
            HardDefaultValue = hardDefaultValue ?? default(TPrimitive) ?? (TPrimitive)(object)"";
        }
    }

    internal abstract class RealmPropertyPrototype<TEnum, TPrimitive> : RealmPropertyPrototype<TPrimitive>
    where TEnum : System.Enum
    where TPrimitive : IEquatable<TPrimitive>
    {
        public TEnum EnumVal { get; private init; }

        protected internal RealmPropertyPrototype(TEnum enumVal, SecondaryDict? secondaryAttributes, TPrimitive hardDefaultValue)
            : base(System.Runtime.CompilerServices.Unsafe.As<TEnum, int>(ref enumVal), secondaryAttributes, hardDefaultValue)
        {
            EnumVal = enumVal;
        }
    }

    internal abstract class RealmPropertyPrototype<TEnum, TPrimitive, TAttribute> : RealmPropertyPrototype<TEnum, TPrimitive>
        where TEnum : System.Enum
        where TPrimitive : IEquatable<TPrimitive>
        where TAttribute : RealmPropertyPrimaryAttribute<TPrimitive>
    {
        public TAttribute PrimaryAttribute { get; private init; }

        protected internal RealmPropertyPrototype(TEnum enumVal, TAttribute primaryAttribute, SecondaryDict? secondaryAttributes, TPrimitive hardDefaultValue)
            : base(enumVal, secondaryAttributes, hardDefaultValue)
        {
            PrimaryAttribute = primaryAttribute;
        }
    }

    internal sealed class RealmPropertyIntPrototype : RealmPropertyPrototype<RealmPropertyInt, int, RealmPropertyPrimaryMinMaxAttribute<int>>
    {
        internal RealmPropertyIntPrototype(RealmPropertyInt enumVal, RealmPropertyPrimaryMinMaxAttribute<int> primaryAttribute, SecondaryDict? secondaryAttributes, int hardDefaultValue)
            : base(enumVal, primaryAttribute, secondaryAttributes, hardDefaultValue) { }
    }
    internal sealed class RealmPropertyInt64Prototype : RealmPropertyPrototype<RealmPropertyInt64, long, RealmPropertyPrimaryMinMaxAttribute<long>>
    {
        internal RealmPropertyInt64Prototype(RealmPropertyInt64 enumVal, RealmPropertyPrimaryMinMaxAttribute<long> primaryAttribute, SecondaryDict? secondaryAttributes, long hardDefaultValue)
            : base(enumVal, primaryAttribute, secondaryAttributes, hardDefaultValue) { }
    }
    internal sealed class RealmPropertyBoolPrototype : RealmPropertyPrototype<RealmPropertyBool, bool, RealmPropertyPrimaryAttribute<bool>>
    {
        internal RealmPropertyBoolPrototype(RealmPropertyBool enumVal, RealmPropertyPrimaryAttribute<bool> primaryAttribute, SecondaryDict? secondaryAttributes, bool hardDefaultValue)
            : base(enumVal, primaryAttribute, secondaryAttributes, hardDefaultValue) { }
    }
    internal sealed class RealmPropertyFloatPrototype : RealmPropertyPrototype<RealmPropertyFloat, double, RealmPropertyPrimaryMinMaxAttribute<double>>
    {
        internal RealmPropertyFloatPrototype(RealmPropertyFloat enumVal, RealmPropertyPrimaryMinMaxAttribute<double> primaryAttribute, SecondaryDict? secondaryAttributes, double hardDefaultValue)
            : base(enumVal, primaryAttribute, secondaryAttributes, hardDefaultValue) { }
    }
    internal sealed class RealmPropertyStringPrototype : RealmPropertyPrototype<RealmPropertyString, string, RealmPropertyPrimaryAttribute<string>>
    {
        internal RealmPropertyStringPrototype(RealmPropertyString enumVal, RealmPropertyPrimaryAttribute<string> primaryAttribute, SecondaryDict? secondaryAttributes, string hardDefaultValue)
            : base(enumVal, primaryAttribute, secondaryAttributes, hardDefaultValue) { }
    }

}
