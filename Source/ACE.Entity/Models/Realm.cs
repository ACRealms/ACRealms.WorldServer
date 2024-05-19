using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
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

        public List<string> GetLogTraceMessages(Dictionary<ushort, string> rulesetNames)
        {
            var list = new List<string>();
            list.Add($"Job of type {Type}, {Links.Count} links");
            list.AddRange(Links.Select(x => x.GetLogTrace(rulesetNames[x.RulesetIDToApply])));
            return list;
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

        internal string GetLogTrace(string realmName) => $"<- ({Probability.ToString("P2")}) [{realmName}]";
    }

    public abstract class AppliedRealmProperty(List<string> traceLog = null)
    {
        public ushort PropertyKey { get; protected set; }
        public RealmPropertyOptions Options { get; init; }
        public abstract Type ValueType { get; }
        public List<string> TraceLog { get; } = traceLog;
    }

    public sealed class AppliedRealmProperty<TVal>(List<string> traceLog) : AppliedRealmProperty(traceLog)
        where TVal : IEquatable<TVal>
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

            return Options.AppliedInfo(val);
        }

        public override Type ValueType => typeof(TVal);
    }

    public abstract record RealmPropertyOptions
    {
        public string RulesetName { get; init; }
        public string Name { get; init; }
        public bool Locked { get; init; }
        public double Probability { get; init; }
        public virtual RealmPropertyRerollType RandomType { get => RealmPropertyRerollType.never; protected init { } }
        public virtual RealmPropertyCompositionType CompositionType { get => RealmPropertyCompositionType.replace; protected init { } }

        private Lazy<string> TemplateDisplayString { get; init; } 
        protected RealmPropertyOptions(string name, string rulesetName)
        {
            Name = name;
            RulesetName = rulesetName;
            TemplateDisplayString = new Lazy<string>(TemplateInfo, System.Threading.LazyThreadSafetyMode.PublicationOnly);
        }

        protected void Log(List<string> traceLog, Func<string> message)
        {
            if (traceLog == null)
                return;
            traceLog.Add($"   [T][{Name}] {message()}");
        }

        public sealed override string ToString() => TemplateDisplayString.Value;
        protected abstract string TemplateInfo();
        public abstract string AppliedInfo(string val);
    }

    public record RealmPropertyOptions<T> : RealmPropertyOptions
        where T : IEquatable<T>
    {
        public T HardDefaultValue { get; init; }
        public T DefaultValue { get; init; }

        public RealmPropertyOptions(string name, string rulesetName, T hardDefaultValue, T defaultValue, bool locked, double? probability) : base(name, rulesetName)
        {
            Locked = locked;
            Probability = probability ?? 1.0;
            DefaultValue = defaultValue;
            HardDefaultValue = hardDefaultValue;
        }

        public virtual T RollValue(List<string> traceLog)
        {
            Log(traceLog, () => $"Value: {DefaultValue}");
            return DefaultValue;
        }

        public virtual T Compose(T parentValue, T rolledValue, List<string> traceLog)
        {
            Log(traceLog, () => $"Compose: Replacing parent {parentValue} with {rolledValue}");
            return DefaultValue;
        }

        protected override string TemplateInfo() => $"Default: {DefaultValue}, Locked: {Locked}, Probability: {Probability}";
        public override string AppliedInfo(string val) => $"Value: {val}";
    }

    public record RollableRealmPropertyOptions<T> : RealmPropertyOptions<T>
        where T : IMinMaxValue<T>, IEquatable<T>, IComparable<T>
    {
        IPropertyOperators<T> Operators { get; init; }
        public T MinValue { get; init; }
        public T MaxValue { get; init; }
        public override RealmPropertyRerollType RandomType { get; protected init; }
        public override RealmPropertyCompositionType CompositionType { get; protected init; }

        public RollableRealmPropertyOptions(string name, string rulesetName, T hardDefaultValue, T defaultValue, byte compositionType, bool locked, double? probability)
            : base(name, rulesetName, hardDefaultValue, defaultValue, locked, probability)
        {
            RandomType = RealmPropertyRerollType.never;
            CompositionType = (RealmPropertyCompositionType)compositionType;
        }

        public RollableRealmPropertyOptions(string name, string rulesetName, T hardDefaultValue, byte compositionType, byte randomType, T randomLowRange, T randomHighRange, bool locked, double? probability)
            : base(name, rulesetName, hardDefaultValue, hardDefaultValue, locked, probability)
        {
            RandomType = (RealmPropertyRerollType)randomType;
            if (typeof(T) == typeof(double))
                Operators = (IPropertyOperators<T>)PropertyOperatorsDouble.Instance;
            else if (typeof(T) == typeof(long))
                Operators = (IPropertyOperators<T>)PropertyOperatorsInt64.Instance;
            else if (typeof(T) == typeof(int))
                Operators = (IPropertyOperators<T>)PropertyOperatorsInt.Instance;
            else
                throw new NotImplementedException();

            CompositionType = (RealmPropertyCompositionType)compositionType;
            MinValue = randomLowRange;
            MaxValue = randomHighRange;
            if (MinValue.CompareTo(MaxValue) > 0)
                MaxValue = MinValue;
        }

        public override T RollValue(List<string> traceLog)
        {
            if (RandomType == RealmPropertyRerollType.never)
            {
                Log(traceLog, () => $"No randomization, returning DefaultValue {DefaultValue}");
                return DefaultValue;
            }

            Log(traceLog, () => $"Rolling between {MinValue} and {MaxValue}");
            return Operators.RollValue(MinValue, MaxValue);
        }

        public override T Compose(T parentValue, T rolledValue, List<string> traceLog)
        {
            Log(traceLog, () => $"Compose {CompositionType}({parentValue}, {rolledValue})");
            switch (CompositionType)
            {
                case RealmPropertyCompositionType.replace:
                    return rolledValue;
                case RealmPropertyCompositionType.add:
                    return Operators.AddValue(parentValue, rolledValue);
                case RealmPropertyCompositionType.multiply:
                    return Operators.MultiplyValue(parentValue, rolledValue);
                default:
                    throw new NotImplementedException();
            }
        }

        public override string AppliedInfo(string val)
        {
            switch(RandomType)
            {
                case RealmPropertyRerollType.always:
                    return $"Random ({MinValue} to {MaxValue})";
                case RealmPropertyRerollType.landblock:
                    return $"{val} (Landblock reroll, random range {MinValue} to {MaxValue}";
                case RealmPropertyRerollType.manual:
                    return $"{val} (Manual reroll, random range {MinValue} to {MaxValue}";
                default:
                    return base.AppliedInfo(val);
            }
        }

        protected override string TemplateInfo() => $"Min: {MinValue}, Max: {MaxValue}, Locked: {Locked}, Probability: {Probability}";
    }


    public interface IPropertyOperators<T>
        where T : IMinMaxValue<T>, IEquatable<T>, IComparable<T>
    {
        public abstract T RollValue(T min, T max);
        public abstract T AddValue(T val1, T val2);
        public abstract T MultiplyValue(T val1, T val2);
    }

    public abstract class PropertyOperatorsBase
    {
        protected static Random Randomizer { get; } = new Random();
    }

    public class PropertyOperatorsDouble : PropertyOperatorsBase, IPropertyOperators<double>
    {
        public static PropertyOperatorsDouble Instance { get; } = new PropertyOperatorsDouble();
        public double RollValue(double min, double max) => Randomizer.NextDouble() * (max - min) + min;
        public double AddValue(double val1, double val2) => val1 + val2;
        public double MultiplyValue(double val1, double val2) => val1 * val2;
    }

    public class PropertyOperatorsInt : PropertyOperatorsBase, IPropertyOperators<int>
    {
        public static PropertyOperatorsInt Instance { get; } = new PropertyOperatorsInt();
        public int RollValue(int min, int max) => Randomizer.Next(min, max);
        public int AddValue(int val1, int val2) => val1 + val2;
        public int MultiplyValue(int val1, int val2) => val1 * val2;
    }

    public class PropertyOperatorsInt64 : PropertyOperatorsBase, IPropertyOperators<long>
    {
        public static PropertyOperatorsInt64 Instance { get; } = new PropertyOperatorsInt64();
        public long RollValue(long min, long max) => Randomizer.NextInt64(min, max);
        public long AddValue(long val1, long val2) => val1 + val2;
        public long MultiplyValue(long val1, long val2) => val1 * val2;
    }
}
