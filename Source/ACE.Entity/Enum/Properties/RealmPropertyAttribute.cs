using ACE.Entity.Enum.RealmProperties;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace ACE.Entity.Enum.Properties
{
    public static class RealmPropertyHelper
    {
        public static FrozenDictionary<E, TProto> BuildPrototypes<E, TProto, TPrimitive, TAttribute>()
            where E : struct, System.Enum
            where TProto : RealmPropertyPrototype<E, TPrimitive, TAttribute>
            where TPrimitive : IEquatable<TPrimitive>
            where TAttribute : RealmPropertyPrimaryAttribute<TPrimitive>
        {
            Type enumType = typeof(E);
            string currentFieldName = string.Empty;
            Func<string, ushort> getRaw = (n) =>
            {
                currentFieldName = n;
                var value = System.Enum.Parse<E>(n);
                return (ushort)(object)value;
            };

            try
            {
                var primaryAttributeConstraint = enumType.GetCustomAttribute<RequiresPrimaryAttributeAttribute<TPrimitive>>(false);

                var protoMap = enumType.GetEnumNames().Where(e => getRaw(e) != 0).Select(n =>
                {
                    currentFieldName = n;
                    var value = System.Enum.Parse<E>(n);

                    var member = enumType.GetMember(n).Single();
                    var attributes = member.GetCustomAttributes<TAttribute>(false).ToArray();

                    if (attributes.Length == 0)
                        throw new Exception($"Enum {typeof(E).Name}.{n} is missing primary attribute {typeof(TAttribute)}.");

                    var attribute = attributes.Single();
                    if (!primaryAttributeConstraint.RequiredAttributeType.IsAssignableFrom(attribute.GetType()))
                        throw new InvalidOperationException($"A primary attribute of type {attribute.GetType()} was found on enum type {enumType} for field {n}, but does not fit the type specification of {primaryAttributeConstraint.RequiredAttributeType}.");

                    var secondaryAttributesRaw = member.GetCustomAttributes<RealmPropertySecondaryAttributeBase>(false).ToArray();

                    FrozenDictionary<Type, RealmPropertySecondaryAttributeBase> secondaryAttributes = null;
                    if (secondaryAttributesRaw.Length > 0)
                        secondaryAttributes = secondaryAttributesRaw.ToDictionary(att => att.GetType(), att => att).ToFrozenDictionary();

                    var proto = (TProto)Activator.CreateInstance(typeof(TProto), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, [value, attribute, secondaryAttributes, attribute.DefaultValue], System.Globalization.CultureInfo.InvariantCulture, null);

                    return (value, proto);
                }).ToFrozenDictionary((pair) => pair.value, (pair) => pair.proto);

                return protoMap;
            }
            catch (Exception ex)
            {
                var msg = $"{ex}\n{ex.StackTrace}";
                if (ex.InnerException != null) msg += $"Inner exception:\n{ex.InnerException}";
                msg += $"\n\nCould not start AC Realms: Realm property enum class {enumType} has ill-defined attributes, or prototype was otherwise unable to be loaded. Current field name: {currentFieldName}";
                Console.WriteLine(msg);
                Console.Error.WriteLine(msg);
                Thread.Sleep(3000);
                throw;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class RealmPropertyPrimaryAttributeBase : Attribute
    {
        public string DefaultFromServerProperty { get; }

        public RealmPropertyPrimaryAttributeBase(string defaultFromServerProperty)
        {
            DefaultFromServerProperty = defaultFromServerProperty;
        }
    }

    public class RealmPropertyPrimaryAttribute<TPrimitive> : RealmPropertyPrimaryAttributeBase
         where TPrimitive : notnull, IEquatable<TPrimitive>
    {
        public TPrimitive DefaultValue { get; }

        public RealmPropertyPrimaryAttribute(string defaultFromServerProperty, TPrimitive defaultValue)
            : base(defaultFromServerProperty)
        {
            DefaultValue = defaultValue;
        }

        public RealmPropertyPrimaryAttribute(TPrimitive defaultValue) : this(null, defaultValue) { }
    }

    public class RealmPropertyPrimaryMinMaxAttribute<TPrimitive> : RealmPropertyPrimaryAttribute<TPrimitive>
         where TPrimitive : notnull, IComparable<TPrimitive>, IEquatable<TPrimitive>, IMinMaxValue<TPrimitive>, INumber<TPrimitive>, ISignedNumber<TPrimitive>, IAdditionOperators<TPrimitive, TPrimitive, TPrimitive>, IMultiplyOperators<TPrimitive, TPrimitive, TPrimitive>
    {
        public TPrimitive MinValue { get; }
        public TPrimitive MaxValue { get; }

        public RealmPropertyPrimaryMinMaxAttribute(TPrimitive defaultValue)
            : this(null, defaultValue, TPrimitive.MinValue, TPrimitive.MaxValue) { }
        public RealmPropertyPrimaryMinMaxAttribute(string defaultFromServerProperty, TPrimitive defaultValue)
            : this(defaultFromServerProperty, defaultValue, TPrimitive.MinValue, TPrimitive.MaxValue) { }

        public RealmPropertyPrimaryMinMaxAttribute(TPrimitive defaultValue, TPrimitive minValue, TPrimitive maxValue)
            : this(null, defaultValue, minValue, maxValue) { }

        public RealmPropertyPrimaryMinMaxAttribute(string defaultFromServerProperty, TPrimitive defaultValue, TPrimitive minValue, TPrimitive maxValue)
            : base(defaultFromServerProperty, defaultValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }

    /*
    public class RealmPropertyIntAttribute : RealmPropertyPrimaryMinMaxAttribute<int>
    {
        public RealmPropertyIntAttribute(int defaultValue, int minValue, int maxValue) : base(defaultValue, minValue, maxValue) { }
        public RealmPropertyIntAttribute(int defaultValue, int minValue, int maxValue) : base(defaultValue, minValue, maxValue) { }

        public RealmPropertyIntAttribute(string defaultFromServerProperty, int defaultValueFallback, int minValue, int maxValue)
            : base(defaultFromServerProperty, defaultValueFallback, minValue, maxValue) { }
    }

    public class RealmPropertyInt64Attribute : RealmPropertyPrimaryAttributeBase
    {
        public string DefaultFromServerProperty { get; }
        public long DefaultValue { get; }
        public long MinValue { get; }
        public long MaxValue { get; }
        public RealmPropertyInt64Attribute(long defaultValue, long minValue = long.MinValue, long maxValue = long.MaxValue)
        {
            DefaultValue = defaultValue;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public RealmPropertyInt64Attribute(string defaultFromServerProperty, long defaultValueFallback, long minValue = long.MinValue, long maxValue = long.MaxValue)
            : this(defaultValueFallback, minValue, maxValue)
        {
            DefaultFromServerProperty = defaultFromServerProperty;
        }
    }

    public class RealmPropertyFloatAttribute : RealmPropertyPrimaryAttributeBase
    {
        public string DefaultFromServerProperty { get; }
        public double DefaultValue { get; }
        public double MinValue { get; }
        public double MaxValue { get; }
        public RealmPropertyFloatAttribute(double defaultValue, double minValue = double.MinValue, double maxValue = double.MaxValue)
        {
            DefaultValue = defaultValue;
            MinValue = minValue;
            MaxValue = maxValue;
        }
        public RealmPropertyFloatAttribute(string defaultFromServerProperty, double defaultValueFallback, double minValue = double.MinValue, double maxValue = double.MaxValue)
            : this(defaultValueFallback, minValue, maxValue)
        {
            DefaultFromServerProperty = defaultFromServerProperty;
        }
    }

    public class RealmPropertyStringAttribute : RealmPropertyPrimaryAttributeBase
    {
        public string DefaultFromServerProperty { get; }
        public string DefaultValue { get; }
        public RealmPropertyStringAttribute(string defaultValue)
        {
            DefaultValue = defaultValue;
        }
        public RealmPropertyStringAttribute(string defaultFromServerProperty, string defaultValue)
            : this(defaultValue)
        {
            DefaultFromServerProperty = defaultFromServerProperty;
        }
    }

    public class RealmPropertyBoolAttribute : RealmPropertyPrimaryAttributeBase
    {
        public string DefaultFromServerProperty { get; }
        public bool DefaultValue { get; }
        public RealmPropertyBoolAttribute(bool defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public RealmPropertyBoolAttribute(string defaultFromServerProperty, bool defaultValue)
            : this(defaultValue)
        {
            DefaultFromServerProperty = defaultFromServerProperty;
        }
    }
    */

    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public abstract class RequiresPrimaryAttributeAttribute : Attribute
    {
        public Type RequiredAttributeType { get; protected init; }
    }

    public abstract class RequiresPrimaryAttributeAttribute<TPrimitive> : RequiresPrimaryAttributeAttribute
        where TPrimitive : IEquatable<TPrimitive> { }

    public class RequiresPrimaryAttributeAttribute<TAttribute, TPrimitive> : RequiresPrimaryAttributeAttribute<TPrimitive>
        where TAttribute : RealmPropertyPrimaryAttribute<TPrimitive>
        where TPrimitive : IEquatable<TPrimitive>
    {
        public RequiresPrimaryAttributeAttribute()
        {
            RequiredAttributeType = typeof(TAttribute);
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public abstract class RealmPropertySecondaryAttributeBase : Attribute
    {

    }

    public class RerollRestrictedToAttribute : RealmPropertySecondaryAttributeBase
    {
        public RealmPropertyRerollType RerollRestriction { get; }
        public RerollRestrictedToAttribute(RealmPropertyRerollType restrictedTo)
        {
            RerollRestriction = restrictedTo;
        }

        private static FrozenDictionary<RealmPropertyRerollType?, FrozenSet<RealmPropertyRerollType>> RestrictionMap { get; } = new Func<Dictionary<RealmPropertyRerollType?, FrozenSet<RealmPropertyRerollType>>>(() =>
        {
            var all = System.Enum.GetValues<RealmPropertyRerollType>().ToFrozenSet();
            var dict = System.Enum.GetValues<RealmPropertyRerollType>().Select(e =>
            {
                RealmPropertyRerollType[] set = e switch
                {
                    RealmPropertyRerollType.never => [RealmPropertyRerollType.never],
                    RealmPropertyRerollType.manual => [RealmPropertyRerollType.manual, RealmPropertyRerollType.never],

                    // Restricting to "landblock reroll" means the user should still have a choice of constraining the rerolls further
                    RealmPropertyRerollType.landblock => [RealmPropertyRerollType.landblock, RealmPropertyRerollType.manual, RealmPropertyRerollType.never],

                    // Unlike the other types above, if we restrict on the most prolific "always", it means we want to explicitly declare a reroll is forced to happen on all fetches for the property
                    RealmPropertyRerollType.always => [RealmPropertyRerollType.always],
                    _ => throw new NotImplementedException()
                };
                return (key: e, set: set.ToFrozenSet());
            }).ToDictionary(t => (RealmPropertyRerollType?)t.key, t => t.set);
            dict[null] = all;

            return dict;
        })().ToFrozenDictionary();

        public RealmPropertyRerollType ConstrainRerollType(RealmPropertyRerollType source) => IsAllowedRerollType(source, RerollRestriction) ? source : RerollRestriction;
        public static FrozenSet<RealmPropertyRerollType> GetAllowedRerollTypes(RealmPropertyRerollType? restrictionType) => RestrictionMap[restrictionType];
        public bool IsAllowedRerollType(RealmPropertyRerollType? restrictionType, RealmPropertyRerollType rerollType) => RestrictionMap[restrictionType].Contains(rerollType);
    }
}
