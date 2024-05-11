using System;
using System.Collections.Generic;
using System.Linq;

namespace ACE.Entity.Enum.Properties
{
    public static class RealmPropertyHelper
    {
        public static Dictionary<E, A> MakePropDict<E, A>()
            where E : struct, System.Enum
        {
            return typeof(E).GetEnumNames().Select(n =>
            {
                var value = (E)System.Enum.Parse(typeof(E), n);
                var attributes = typeof(E).GetMember(n)
                    .FirstOrDefault(m => m.DeclaringType == typeof(E))
                    .GetCustomAttributes(typeof(A), false);

                if (attributes.Length == 0)
                    throw new Exception($"Enum {typeof(E).Name}.{n} is missing a {typeof(A)} attribute.");
                if (attributes.Length != 1)
                    throw new Exception($"Enum {typeof(E).Name}.{n} must have no more than 1 {typeof(A)} attributes.");

                var attribute = (A)attributes[0];
                return (value, attribute);
            }).ToDictionary((pair) => pair.value, (pair) => pair.attribute);
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RealmPropertyIntAttribute : Attribute
    {
        public string DefaultFromServerProperty { get; }
        public int DefaultValue { get; }
        public int MinValue { get; }
        public int MaxValue { get; }
        public RealmPropertyIntAttribute(int defaultValue, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            DefaultValue = defaultValue;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public RealmPropertyIntAttribute(string defaultFromServerProperty, int defaultValueFallback, int minValue = int.MinValue, int maxValue = int.MaxValue)
            : this(defaultValueFallback, minValue, maxValue)
        {
            DefaultFromServerProperty = defaultFromServerProperty;
        }
    }

    public class RealmPropertyInt64Attribute : Attribute
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

    public class RealmPropertyFloatAttribute : Attribute
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

    public class RealmPropertyStringAttribute : Attribute
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

    public class RealmPropertyBoolAttribute : Attribute
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
}
