using ACRealms.RealmProps.Contexts;
using ACRealms.Rulesets.Enums;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace ACRealms.RealmProps
{
    internal static class RealmPropertyHelper
    {
        internal static FrozenDictionary<E, TProto> BuildPrototypes<E, TProto, TPrimitive, TAttribute>()
            where E : struct, System.Enum
            where TProto : RealmPropertyPrototype<E, TPrimitive, TAttribute>
            where TPrimitive : IEquatable<TPrimitive>
            where TAttribute : RealmPropertyPrimaryAttribute<TPrimitive>
        {
            Type enumType = typeof(E);
            if (!typeof(int).IsAssignableFrom(Enum.GetUnderlyingType(enumType)))
                throw new ArgumentException($"The type {typeof(E).FullName} was expected to be assignable to a System.Int32, but was not.");

            string currentFieldName = string.Empty;
            Func<string, int> getRaw = (n) =>
            {
                currentFieldName = n;
                var value = System.Enum.Parse<E>(n);
                System.Runtime.CompilerServices.Unsafe.As<E, int>(ref value);
                return (int)(object)value;
            };

            try
            {
                var primaryAttributeConstraint = enumType.GetCustomAttribute<Prototypes.RequiresPrimaryAttributeAttribute<TPrimitive>>(false)!;

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

                    FrozenDictionary<Type, RealmPropertySecondaryAttributeBase>? secondaryAttributes = null;
                    if (secondaryAttributesRaw.Length > 0)
                        secondaryAttributes = secondaryAttributesRaw.ToDictionary(att => att.GetType(), att => att).ToFrozenDictionary();

                    var scopedWithAttributes = member.GetCustomAttributes<ScopedWithAttribute>(true);

                    // We use reflection here to force explicit constructor usage, though there's possibly a better way to do this.
                    var proto = (TProto?)Activator.CreateInstance(
                        typeof(TProto),
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
                        binder: null,
                        args: [currentFieldName, value, attribute, secondaryAttributes, attribute.DefaultValue, scopedWithAttributes],
                        System.Globalization.CultureInfo.InvariantCulture,
                        activationAttributes: null
                    );

                    return (value, proto);
                }).ToFrozenDictionary((pair) => pair.value, (pair) => pair.proto);

                return protoMap!;
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



    internal interface IRealmPropertyPrimaryAttribute
    {
        string? DefaultFromServerProperty { get; }
    }

    internal class RealmPropertyPrimaryAttribute<TPrimitive> : Prototypes.PropertyPrimaryAttribute<TPrimitive>, IRealmPropertyPrimaryAttribute
         where TPrimitive : notnull, IEquatable<TPrimitive>
    {
        public string? DefaultFromServerProperty { get; }
        public TPrimitive DefaultValue { get; }

        public RealmPropertyPrimaryAttribute(string? defaultFromServerProperty, TPrimitive defaultValue)
        {
            DefaultFromServerProperty = defaultFromServerProperty;
            DefaultValue = defaultValue;
        }

        public RealmPropertyPrimaryAttribute(TPrimitive defaultValue) : this(null, defaultValue) { }
    }

    internal class RealmPropertyPrimaryMinMaxAttribute<TPrimitive> : RealmPropertyPrimaryAttribute<TPrimitive>
         where TPrimitive : notnull, IComparable<TPrimitive>, IEquatable<TPrimitive>, IMinMaxValue<TPrimitive>, INumber<TPrimitive>, IAdditionOperators<TPrimitive, TPrimitive, TPrimitive>, IMultiplyOperators<TPrimitive, TPrimitive, TPrimitive>
    {
        public TPrimitive MinValue { get; }
        public TPrimitive MaxValue { get; }

        public RealmPropertyPrimaryMinMaxAttribute(TPrimitive defaultValue)
            : this(null, defaultValue, TPrimitive.MinValue, TPrimitive.MaxValue) { }
        public RealmPropertyPrimaryMinMaxAttribute(string? defaultFromServerProperty, TPrimitive defaultValue)
            : this(defaultFromServerProperty, defaultValue, TPrimitive.MinValue, TPrimitive.MaxValue) { }

        public RealmPropertyPrimaryMinMaxAttribute(TPrimitive defaultValue, TPrimitive minValue, TPrimitive maxValue)
            : this(null, defaultValue, minValue, maxValue) { }

        public RealmPropertyPrimaryMinMaxAttribute(string? defaultFromServerProperty, TPrimitive defaultValue, TPrimitive minValue, TPrimitive maxValue)
            : base(defaultFromServerProperty, defaultValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }

    internal class RealmPropertyEnumAttribute<TEnum>
        : RealmPropertyPrimaryMinMaxAttribute<int>
        // Must be int for the following reasons:
        // 1. Fast conversion with Unsafe.As requires both types to be the same underlying size in bytes (32 bits for the default enum type)
        // 2. Compatibility with the underlying RealmPropertyInt requires a range that doesn't exceed RealmPropertyInt's range (which is that of a signed int32)
         where TEnum : struct, System.Enum

    {
        private static readonly Dictionary<Type, (int min, int max)> EnumMinMaxCache = [];

        public RealmPropertyEnumAttribute(TEnum defaultValue) : base(UnsafeCast(defaultValue))
        {
        }

        private static int UnsafeCast(TEnum enumValue)
        {
            return System.Runtime.CompilerServices.Unsafe.As<TEnum, int>(ref enumValue);
        }

        public RealmPropertyEnumAttribute(string defaultFromServerProperty, TEnum defaultValue)
            : base(defaultFromServerProperty, UnsafeCast(defaultValue))
        {
        }

        public RealmPropertyEnumAttribute(TEnum defaultValue, int minValue, int maxValue)
            : base(UnsafeCast(defaultValue), minValue, maxValue)
        {
        }

        public RealmPropertyEnumAttribute(string defaultFromServerProperty, TEnum defaultValue, int minValue, int maxValue)
            : base(defaultFromServerProperty, UnsafeCast(defaultValue), minValue, maxValue)
        {
        }
    }





    [AttributeUsage(AttributeTargets.Field)]
    internal abstract class RealmPropertySecondaryAttributeBase : Attribute
    {

    }

    internal class RerollRestrictedToAttribute : RealmPropertySecondaryAttributeBase
    {
        public RealmPropertyRerollType RerollRestriction { get; }
        public RerollRestrictedToAttribute(RealmPropertyRerollType restrictedTo)
        {
            RerollRestriction = restrictedTo;
        }

        private static readonly FrozenSet<RealmPropertyRerollType> All = System.Enum.GetValues<RealmPropertyRerollType>().ToFrozenSet();

        private static FrozenDictionary<RealmPropertyRerollType, FrozenSet<RealmPropertyRerollType>> RestrictionMap { get; } = new Func<Dictionary<RealmPropertyRerollType, FrozenSet<RealmPropertyRerollType>>>(() =>
        {
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
            }).ToDictionary(t => t.key, t => t.set);

            return dict;
        })().ToFrozenDictionary();

        public RealmPropertyRerollType ConstrainRerollType(RealmPropertyRerollType source) => IsAllowedRerollType(source, RerollRestriction) ? source : RerollRestriction;
        public static FrozenSet<RealmPropertyRerollType> GetAllowedRerollTypes(RealmPropertyRerollType? restrictionType) => restrictionType.HasValue ? RestrictionMap[restrictionType.Value] : All;
        public bool IsAllowedRerollType(RealmPropertyRerollType? restrictionType, RealmPropertyRerollType rerollType) => GetAllowedRerollTypes(restrictionType).Contains(rerollType);
    }

    [AttributeUsage(AttributeTargets.Field)]
    internal abstract class ScopedWithAttribute : Attribute, IScopedWithAttribute
    {
        public string Name { get; }
        public bool Required { get; }
        public string Entity { get; }
        public string Description { get; }

        public ScopedWithAttribute(string name, bool required, string entity, string description)
        {
            Name = name;
            Required = required;
            Entity = entity;
            Description = description;
        }
    }

    internal interface IScopedWithAttribute
    {
        string Name { get; }
        bool Required { get; }
        string Entity { get; }
        string Description { get; }
    }
    internal interface IScopedWithAttribute<out T> : IScopedWithAttribute
        where T : IContextEntity
    {
        Lazy<Type> EntityType { get; }

        //static virtual bool RespondsTo(string key) => T.RespondsTo(key);


        /// <summary>
        /// Warning, this method is somewhat slow, using reflection, and should only be called during ruleset warming (RealmConverter)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool RespondsTo(string key);
        Type TypeOfProperty(string key);
        //virtual Type TypeOfProperty(string key) => T.TypeOfProperty(key);
    }

    internal sealed class ScopedWithAttribute<T> : ScopedWithAttribute, IScopedWithAttribute<T> where T : IContextEntity
    {
        public Lazy<Type> EntityType { get; } = new Lazy<Type>(() => typeof(T), true);
        internal ScopedWithAttribute(string name, bool required, string entity, string description)
            : base(name, required, entity, description)
        {
        }

        public bool RespondsTo(string key)
        {
            try
            {
                return T.RespondsTo(key);
            }
            catch (NotImplementedException)
            {
                if (EntityType == null)
                    throw;

                var entityType = EntityType.Value;
                if (!RealmPropertyPrototypes.ContextMappings.TryGetValue(entityType, out var secondType))
                    throw;

                var method = secondType.GetMethod("RespondsTo", BindingFlags.Static | BindingFlags.Public, [typeof(string)]);
                return (bool)method!.Invoke(null, [key])!;
            }
        }

        public Type TypeOfProperty(string key)
        {
            try
            {
                return T.TypeOfProperty(key);
            }
            catch (NotImplementedException)
            {
                if (EntityType == null)
                    throw;

                var entityType = EntityType.Value;
                if (!RealmPropertyPrototypes.ContextMappings.TryGetValue(entityType, out var secondType))
                    throw;

                var method = secondType.GetMethod("TypeOfProperty", BindingFlags.Static | BindingFlags.Public, [typeof(string)]);
                return (Type)method!.Invoke(null, [key])!;
            }
        }
    }
}
