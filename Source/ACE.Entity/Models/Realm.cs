using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;

namespace ACE.Entity.Models
{
    public class Realm
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public RealmType Type { get; set; }
        public ushort? ParentRealmID { get; set; }
        public ushort? PropertyCountRandomized { get; set; }

        public IDictionary<RealmPropertyBool, AppliedRealmProperty<bool>> PropertiesBool { get; set; }
        public IDictionary<RealmPropertyFloat, AppliedRealmProperty<double>> PropertiesFloat { get; set; }
        public IDictionary<RealmPropertyInt, AppliedRealmProperty<int>> PropertiesInt { get; set; }
        public IDictionary<RealmPropertyInt64, AppliedRealmProperty<long>> PropertiesInt64 { get; set; }
        public IDictionary<RealmPropertyString, AppliedRealmProperty<string>> PropertiesString { get; set; }
        public IDictionary<string, AppliedRealmProperty> AllProperties { get; set; }

        public bool NeedsRefresh { get; set; }

        public IReadOnlyList<RealmLinkJob> Jobs { get; }

        public Realm(List<RealmLinkJob> jobs)
        {
            Jobs = jobs.AsReadOnly();
        }

        public override string ToString()
        {
            return $"{Type} {Name} ({Id})";
        }
    }

    public class RealmLinkJob
    {
        public RealmRulesetLinkType Type { get; }
        public IReadOnlyList<AppliedRealmLink> Links { get; }
        public RealmLinkJob(RealmRulesetLinkType type, ReadOnlyCollection<AppliedRealmLink> links)
        {
            this.Type = type;
            this.Links = links;
        }

        public override string ToString()
        {
            return $"{Links.Count} Links";
        }
    }

    public class AppliedRealmLink
    {
        public double Probability { get; }
        public ushort RulesetIDToApply { get; }

        public AppliedRealmLink(ushort rulesetIDToApply, double probability)
        {
            Probability = probability;
            RulesetIDToApply = rulesetIDToApply;
        }

        public override string ToString()
        {
            return $"{Probability} - Realm {RulesetIDToApply}";
        }
    }

    public abstract class AppliedRealmProperty
    {
        public ushort PropertyKey { get; protected set; }
        public RealmPropertyOptions Options { get; init; }
        public abstract Type ValueType { get; }
    }

    public sealed class AppliedRealmProperty<TVal> : AppliedRealmProperty
        where TVal : IComparable
    {
        public new RealmPropertyOptions<TVal> Options { get => (RealmPropertyOptions<TVal>)base.Options; init => base.Options = value; }
        public AppliedRealmProperty<TVal> Parent { get; }

        private TVal _value;
        public TVal Value
        {
            get
            {
                if (Options.RandomType == RealmPropertyRerollType.always)
                    RollValue();
                return _value;
            }
            private set => _value = value;
        }

        protected AppliedRealmProperty() { }

        //Clone
        public AppliedRealmProperty(AppliedRealmProperty<TVal> prop, AppliedRealmProperty<TVal> parent = null)
            : this(prop.PropertyKey, prop.Options)
        {
            if (Options.CompositionType != RealmPropertyCompositionType.replace)
            {
                if (parent != null)
                    Parent = new AppliedRealmProperty<TVal>(parent);
                else if (prop.Parent != null)
                    Parent = new AppliedRealmProperty<TVal>(prop.Parent);
            }
        }

        public AppliedRealmProperty(ushort propertyKey, RealmPropertyOptions<TVal> options)
        {
            PropertyKey = propertyKey;
            Options = options;
        }

        public void RollValue()
        {
            if (Parent != null)
                Parent.RollValue();
            Value = Options.Compose(Parent != null ? Parent.Value : Options.HardDefaultValue, Options.RollValue());
        }

        public override string ToString()
        {
            switch(Options.RandomType)
            {
                case RealmPropertyRerollType.always:
                    return $"Random ({Options.MinValue} to {Options.MaxValue})";
                case RealmPropertyRerollType.landblock:
                    return $"{Value} (Landblock reroll, random range {Options.MinValue} to {Options.MaxValue}";
                case RealmPropertyRerollType.manual:
                    return $"{Value} (Manual reroll, random range {Options.MinValue} to {Options.MaxValue}";
                case RealmPropertyRerollType.never:
                    return $"{Value}";
                default:
                    return $"{Value}";
            }
        }

        public override Type ValueType => typeof(TVal);
    }

    public abstract class RealmPropertyOptions(string name)
    {
        public object HardDefaultValue { get; protected set; }
        public object DefaultValue { get; protected set; }
        public object MinValue { get; protected set; }
        public object MaxValue { get; protected set; }
        public bool Locked { get; protected set; }
        public double Probability { get; protected set; }
        public RealmPropertyRerollType RandomType { get; protected set; }
        public RealmPropertyCompositionType CompositionType { get; protected set; }
        public string Name { get; init; } = name;
    }

    public sealed class RealmPropertyOptions<T> : RealmPropertyOptions
    {
        static Random Randomizer = new Random();

        public new T HardDefaultValue { get => (T)base.HardDefaultValue; private set => base.HardDefaultValue = value; }
        public new T DefaultValue { get => (T)base.DefaultValue; private set => base.DefaultValue = value; }
        public new T MinValue { get => (T)base.MinValue; private set => base.MinValue = value; }
        public new T MaxValue { get => (T)base.MaxValue; private set => base.MaxValue = value; }

        public RealmPropertyOptions(string name) : base(name) { }

        public void SeedPropertiesStatic(T defaultValue, T hardDefaultValue, byte compositionType, bool locked, double? probability)
        {
            RandomType = RealmPropertyRerollType.never;
            CompositionType = (RealmPropertyCompositionType)compositionType;
            Locked = locked;
            Probability = probability ?? 1.0;
            DefaultValue = defaultValue;
            HardDefaultValue = hardDefaultValue;
        }

        public void SeedPropertiesRandomized(T hardDefaultValue, byte compositionType, byte randomType, T randomLowRange, T randomHighRange, bool locked, double? probability)
        {
            HardDefaultValue = hardDefaultValue;
            Locked = locked;
            Probability = probability ?? 1.0;
            RandomType = (RealmPropertyRerollType)randomType;
            CompositionType = (RealmPropertyCompositionType)compositionType;
            MinValue = randomLowRange;
            MaxValue = randomHighRange;
            ClampMinMax();
        }

        private void ClampMinMax()
        {
            if (typeof(T) == typeof(double))
            {
                if ((double)(object)MinValue > (double)(object)MaxValue)
                    MaxValue = MinValue;
            }
            else if (typeof(T) == typeof(int))
            {
                if ((int)(object)MinValue > (int)(object)MaxValue)
                    MaxValue = MinValue;
            }
            else if (typeof(T) == typeof(long))
            {
                if ((long)(object)MinValue > (long)(object)MaxValue)
                    MaxValue = MinValue;
            }
        }

        public T RollValue()
        {
            if (RandomType == RealmPropertyRerollType.never)
                return DefaultValue;

            if (typeof(T) == typeof(double))
                return (T)(object)(Randomizer.NextDouble() * ((double)(object)MaxValue - (double)(object)MinValue) + (double)(object)MinValue);
            else if (typeof(T) == typeof(int))
                return (T)(object)Randomizer.Next((int)(object)MinValue, (int)(object)MaxValue);
            else if (typeof(T) == typeof(long))
                return (T)(object)NextLong((long)(object)MinValue, (long)(object)MaxValue);
            else
                return DefaultValue;
        }

        private long NextLong(long min, long max)
        {
            if (max <= min)
                throw new ArgumentOutOfRangeException("max", "max must be > min!");

            ulong uRange = (ulong)(max - min);
            ulong ulongRand;
            do
            {
                byte[] buf = new byte[8];
                Randomizer.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange);

            return (long)(ulongRand % uRange) + min;
        }

        internal Tvar Compose<Tvar>(Tvar parentValue, Tvar rolledValue)
        {
            switch (CompositionType)
            {
                case RealmPropertyCompositionType.replace:
                    return rolledValue;
                case RealmPropertyCompositionType.add:
                    return Add(parentValue, rolledValue);
                case RealmPropertyCompositionType.multiply:
                    return Multiply(parentValue, rolledValue);
                default:
                    return rolledValue;
            }
        }

        Tvar Add<Tvar>(Tvar oldvalue, Tvar newvalue)
        {
            if (typeof(Tvar) == typeof(double))
                return (Tvar)(object)((double)(object)oldvalue + (double)(object)newvalue);
            else if (typeof(Tvar) == typeof(int))
                return (Tvar)(object)((int)(object)oldvalue + (int)(object)newvalue);
            else if (typeof(Tvar) == typeof(long))
                return (Tvar)(object)((long)(object)oldvalue + (long)(object)newvalue);
            else
                return newvalue;
        }

        Tvar Multiply<Tvar>(Tvar oldvalue, Tvar newvalue)
        {
            if (typeof(Tvar) == typeof(double))
                return (Tvar)(object)((double)(object)oldvalue * (double)(object)newvalue);
            else if (typeof(Tvar) == typeof(int))
                return (Tvar)(object)((int)(object)oldvalue * (int)(object)newvalue);
            else if (typeof(T) == typeof(long))
                return (Tvar)(object)((long)(object)oldvalue * (long)(object)newvalue);
            else
                return newvalue;
        }

        public override string ToString()
        {
            return $"Default: {DefaultValue}, Min: {MinValue}, Max: {MaxValue}, Locked: {Locked}, Probability: {Probability}";
        }
    }
}
