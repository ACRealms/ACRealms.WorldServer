using System;

namespace ACE.Entity.Enum.Properties
{
    public class RealmPropertyIntAttribute : Attribute
    {
        public int DefaultValue { get; }
        public int MinValue { get; }
        public int MaxValue { get; }
        public RealmPropertyIntAttribute(int defaultValue, int minValue = Int32.MinValue, int maxValue = Int32.MaxValue)
        {
            this.DefaultValue = defaultValue;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }
    }

    public class RealmPropertyInt64Attribute : Attribute
    {
        public long DefaultValue { get; }
        public long MinValue { get; }
        public long MaxValue { get; }
        public RealmPropertyInt64Attribute(long defaultValue, long minValue, long maxValue)
        {
            this.DefaultValue = defaultValue;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }
    }

    public class RealmPropertyFloatAttribute : Attribute
    {
        public double DefaultValue { get; }
        public double MinValue { get; }
        public double MaxValue { get; }
        public RealmPropertyFloatAttribute(double defaultValue, double minValue, double maxValue)
        {
            this.DefaultValue = defaultValue;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }
    }

    public class RealmPropertyStringAttribute : Attribute
    {
        public string DefaultValue { get; }
        public RealmPropertyStringAttribute(string defaultValue)
        {
            this.DefaultValue = defaultValue;
        }
    }

    public class RealmPropertyBoolAttribute : Attribute
    {
        public bool DefaultValue { get; }
        public RealmPropertyBoolAttribute(bool defaultValue)
        {
            this.DefaultValue = defaultValue;
        }
    }
}
