using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

    public abstract class AppliedRealmProperty(List<string> traceLog = null)
    {
        public ushort PropertyKey { get; protected set; }
        public RealmPropertyOptions Options { get; init; }
        public abstract Type ValueType { get; }
        public List<string> TraceLog { get; } = traceLog;
    }

    public sealed class AppliedRealmProperty<TVal>(List<string> traceLog) : AppliedRealmProperty(traceLog)
        where TVal : IComparable
    {
        public new RealmPropertyOptions<TVal> Options { get => (RealmPropertyOptions<TVal>)base.Options; init => base.Options = value; }
        public AppliedRealmProperty<TVal> Parent { get; }

        private TVal _value;
        private bool rolledOnce = false;
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

        private void LogTrace(Func<string> message)
        {
            if (TraceLog == null)
                return;
            TraceLog.Add($"   [P][{Options.Name}] {message()}");
        }

        //Clone
        public AppliedRealmProperty(AppliedRealmProperty<TVal> prop, AppliedRealmProperty<TVal> parent = null, List<string> traceLog = null)
            : this(prop.PropertyKey, prop.Options, traceLog)
        {
            LogTrace(() => $"Cloning from template. CompositionType: {Options.CompositionType}");

            if (Options.CompositionType != RealmPropertyCompositionType.replace)
            {
                if (parent != null)
                {
                    LogTrace(() => $"Setting parent (explicitly passed): {parent.Options.Name}");
                    Parent = new AppliedRealmProperty<TVal>(parent, null, traceLog);
                }
                else if (prop.Parent != null)
                {
                    LogTrace(() => $"Setting parent (from template parent): {prop.Parent.Options.Name}");
                    Parent = new AppliedRealmProperty<TVal>(prop.Parent, null, traceLog);
                }
            }
            else
            {
                if (parent != null)
                    LogTrace(() => $"Discarding parent: {prop.Parent.Options.Name}");
                else if (prop.Parent != null)
                    LogTrace(() => $"Discarding parent (from template parent): {prop.Parent.Options.Name}");
            }
        }

        /// <summary>
        /// Constructor for realm property from database
        /// </summary>
        /// <param name="propertyKey"></param>
        /// <param name="options"></param>
        /// <param name="traceLog"></param>
        public AppliedRealmProperty(ushort propertyKey, RealmPropertyOptions<TVal> options, List<string> traceLog)
            : this(traceLog)
        {
            PropertyKey = propertyKey;
            Options = options;
        }

        public void RollValue()
        {
            TVal composedFromValue;
            if (Parent != null)
            {
                LogTrace(() => $"Rerolling parent property before composition");
                Parent.RollValue();
                composedFromValue = Parent.Value;
                LogTrace(() => $"Composing from parent property value: {composedFromValue}");
            }
            else
            {
                LogTrace(() => $"Composing from hard default value: {Options.HardDefaultValue}");
                composedFromValue = Options.HardDefaultValue;
            }

            var val = Options.Compose(composedFromValue, Options.RollValue(TraceLog), TraceLog);
            LogTrace(() => $"Rolled value {val}");
            Value = val;
            if (!rolledOnce)
                rolledOnce = true;
        }

        public override string ToString()
        {
            string val;
            if (!rolledOnce)
                val = "<Deferred Load>";
            else
                val = _value.ToString();

            switch(Options.RandomType)
            {
                case RealmPropertyRerollType.always:
                    return $"Random ({Options.MinValue} to {Options.MaxValue})";
                case RealmPropertyRerollType.landblock:
                    return $"{val} (Landblock reroll, random range {Options.MinValue} to {Options.MaxValue}";
                case RealmPropertyRerollType.manual:
                    return $"{val} (Manual reroll, random range {Options.MinValue} to {Options.MaxValue}";
                case RealmPropertyRerollType.never:
                    return val;
                default:
                    return val;
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

        public T RollValue(List<string> traceLog)
        {
            if (RandomType == RealmPropertyRerollType.never)
            {
                Log(traceLog, () => $"No randomization, returning DefaultValue {DefaultValue}");
                return DefaultValue;
            }

            if (typeof(T) == typeof(double))
            {
                Log(traceLog, () => $"Rolling between {MinValue} and {MaxValue}");
                return (T)(object)(Randomizer.NextDouble() * ((double)(object)MaxValue - (double)(object)MinValue) + (double)(object)MinValue);
            }
            else if (typeof(T) == typeof(int))
            {
                Log(traceLog, () => $"Rolling between {MinValue} and {MaxValue}");
                return (T)(object)Randomizer.Next((int)(object)MinValue, (int)(object)MaxValue);
            }
            else if (typeof(T) == typeof(long))
            {
                Log(traceLog, () => $"Rolling between {MinValue} and {MaxValue}");
                return (T)(object)NextLong((long)(object)MinValue, (long)(object)MaxValue);
            }
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

        internal Tvar Compose<Tvar>(Tvar parentValue, Tvar rolledValue, List<string> traceLog)
        {
            Log(traceLog, () => $"Compose {CompositionType}({parentValue}, {rolledValue})");
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

        private void Log(List<string> traceLog, Func<string> message)
        {
            if (traceLog == null)
                return;
            traceLog.Add($"   [T][{Name}] {message()}");
        }

        public override string ToString()
        {
            return $"Default: {DefaultValue}, Min: {MinValue}, Max: {MaxValue}, Locked: {Locked}, Probability: {Probability}";
        }
    }
}
