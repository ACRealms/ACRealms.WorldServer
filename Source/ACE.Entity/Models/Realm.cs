using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using ACE.Entity.ACRealms;
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

    public abstract class AppliedRealmProperty(RulesetCompilationContext ctx)
    {
        public ushort PropertyKey { get; protected set; }
        public RealmPropertyOptions Options { get; init; }
        public abstract Type ValueType { get; }
        protected RulesetCompilationContext Context { get; } = ctx;
    }

    public sealed class AppliedRealmProperty<TVal>(RulesetCompilationContext ctx) : AppliedRealmProperty(ctx)
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
            if (!Context.Trace)
                return;
            Context.LogDirect($"   [P][{Options.Name}] (Def:{Options.RulesetName}) {message()}");
        }

        //Clone
        public AppliedRealmProperty(RulesetCompilationContext ctx, AppliedRealmProperty<TVal> prop, AppliedRealmProperty<TVal> parent = null)
            : this(ctx, prop.PropertyKey, prop.Options)
        {
            LogTrace(() => $"Cloning from template. CompositionType: {Options.CompositionType}");

            if (Options.CompositionType != RealmPropertyCompositionType.replace)
            {
                if (parent != null)
                {
                    LogTrace(() => $"Setting parent (explicitly passed): {parent.Options.RulesetName}");
                    Parent = new AppliedRealmProperty<TVal>(ctx, parent, null);
                }
                else if (prop.Parent != null)
                {
                    LogTrace(() => $"Setting parent (from template parent): {prop.Parent.Options.RulesetName}");
                    Parent = new AppliedRealmProperty<TVal>(ctx, prop.Parent, null);
                }
            }
            else
            {
                if (parent != null)
                    LogTrace(() => $"Discarding parent: {parent.Options.RulesetName}");
                else if (prop.Parent != null)
                    LogTrace(() => $"Discarding parent (from template parent): {prop.Parent.Options.RulesetName}");
            }
        }

        /// <summary>
        /// Constructor for realm property from database
        /// </summary>
        /// <param name="propertyKey"></param>
        /// <param name="options"></param>
        /// <param name="traceLog"></param>
        public AppliedRealmProperty(RulesetCompilationContext ctx, ushort propertyKey, RealmPropertyOptions<TVal> options)
            : this(ctx)
        {
            PropertyKey = propertyKey;
            Options = options;
        }

        private string GetAppliedParentChain(StringBuilder sb = null)
        {
            bool direct = sb == null;
            sb ??= new StringBuilder();
            if (!direct) sb.AppendFormat("<-{0}", Options.RulesetName);
            if (Parent != null) Parent.GetAppliedParentChain(sb);
            return direct ? sb.ToString() : null;
        }

        public void RollValue(bool direct = true)
        {
            TVal composedFromValue;
            if (Parent != null)
            {
                if (direct)
                    LogTrace(() => GetAppliedParentChain());

                Parent.RollValue(false);
                composedFromValue = Parent.Value;
            }
            else
            {
                if (direct)
                    LogTrace(() => "<-[DEFAULT]");
                composedFromValue = Options.HardDefaultValue;
            }

            var val = Options.Compose(composedFromValue, Options.RollValue(Context), Context);
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

    /// <summary>
    /// <para>Represents the immutable property definition from the ruleset config file </para>
    /// <para>The RulesetName is guaranteed to be linked to the ruleset file where defined</para>
    /// </summary>
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

        protected void Log(RulesetCompilationContext ctx, Func<string> message)
        {
            if (!ctx.Trace)
                return;
            ctx.LogDirect($"   [T][{Name}] (Def: {RulesetName}) {message()}");
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

        public virtual T RollValue(RulesetCompilationContext ctx)
        {
            Log(ctx, () => $"Value: {DefaultValue}");
            return DefaultValue;
        }

        public virtual T Compose(T parentValue, T rolledValue, RulesetCompilationContext ctx)
        {
            Log(ctx, () => $"Compose: Replacing parent {parentValue} with {rolledValue}");
            return DefaultValue;
        }

        protected override string TemplateInfo() => $"Default: {DefaultValue}, Locked: {Locked}, Probability: {Probability}";
        public override string AppliedInfo(string val) => $"Value: {val}";
    }

    public record MinMaxRangedRealmPropertyOptions<T> : RealmPropertyOptions<T>
        where T : IMinMaxValue<T>, IEquatable<T>, IComparable<T>
    {
        IPropertyOperatorsMinMax<T> Operators { get; init; }
        public T MinValue { get; init; }
        public T MaxValue { get; init; }
        public override RealmPropertyRerollType RandomType { get; protected init; }
        public override RealmPropertyCompositionType CompositionType { get; protected init; }

        public MinMaxRangedRealmPropertyOptions(string name, string rulesetName, T hardDefaultValue, T defaultValue, byte compositionType, bool locked, double? probability)
            : base(name, rulesetName, hardDefaultValue, defaultValue, locked, probability)
        {
            RandomType = RealmPropertyRerollType.never;
            CompositionType = (RealmPropertyCompositionType)compositionType;
        }

        public MinMaxRangedRealmPropertyOptions(string name, string rulesetName, T hardDefaultValue, byte compositionType, byte randomType, T randomLowRange, T randomHighRange, bool locked, double? probability)
            : base(name, rulesetName, hardDefaultValue, hardDefaultValue, locked, probability)
        {
            RandomType = (RealmPropertyRerollType)randomType;
            if (typeof(T) == typeof(double))
                Operators = (IPropertyOperatorsMinMax<T>)PropertyOperatorsDouble.Instance;
            else if (typeof(T) == typeof(long))
                Operators = (IPropertyOperatorsMinMax<T>)PropertyOperatorsInt64.Instance;
            else if (typeof(T) == typeof(int))
                Operators = (IPropertyOperatorsMinMax<T>)PropertyOperatorsInt.Instance;
            else
                throw new NotImplementedException();

            CompositionType = (RealmPropertyCompositionType)compositionType;
            MinValue = randomLowRange;
            MaxValue = randomHighRange;
            if (MinValue.CompareTo(MaxValue) > 0)
                MaxValue = MinValue;
        }

        public override T RollValue(RulesetCompilationContext ctx)
        {
            if (RandomType == RealmPropertyRerollType.never)
            {
                Log(ctx, () => $"No randomization, returning DefaultValue {DefaultValue}");
                return DefaultValue;
            }

            Log(ctx, () => $"Rolling between {MinValue} and {MaxValue}");
            return Operators.RollValue(MinValue, MaxValue);
        }

        public override T Compose(T parentValue, T rolledValue, RulesetCompilationContext ctx)
        {
            Log(ctx, () => $"Compose {CompositionType}({parentValue}, {rolledValue})");
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

    public interface IPropertyOperators { }
    public interface IPropertyOperators<T> : IPropertyOperators { }

    public interface IPropertyOperatorsRollable<T> : IPropertyOperators<T>
        where T : IEquatable<T>
    {
        public abstract T RollValue(T min, T max);
    }

    public interface IPropertyOperatorsMinMax<T> : IPropertyOperatorsRollable<T>
        where T : IMinMaxValue<T>, IEquatable<T>, IComparable<T>
    {
        public abstract T AddValue(T val1, T val2);
        public abstract T MultiplyValue(T val1, T val2);
    }

    public abstract class PropertyOperatorsBase : IPropertyOperators
    {
        protected static Random Randomizer { get; } = new Random();
    }

    public class PropertyOperatorsDouble : PropertyOperatorsBase, IPropertyOperatorsMinMax<double>
    {
        public static PropertyOperatorsDouble Instance { get; } = new PropertyOperatorsDouble();
        public double RollValue(double min, double max) => Randomizer.NextDouble() * (max - min) + min;
        public double AddValue(double val1, double val2) => val1 + val2;
        public double MultiplyValue(double val1, double val2) => val1 * val2;
    }

    public class PropertyOperatorsInt : PropertyOperatorsBase, IPropertyOperatorsMinMax<int>
    {
        public static PropertyOperatorsInt Instance { get; } = new PropertyOperatorsInt();
        public int RollValue(int min, int max) => Randomizer.Next(min, max);
        public int AddValue(int val1, int val2) => val1 + val2;
        public int MultiplyValue(int val1, int val2) => val1 * val2;
    }

    public class PropertyOperatorsInt64 : PropertyOperatorsBase, IPropertyOperatorsMinMax<long>
    {
        public static PropertyOperatorsInt64 Instance { get; } = new PropertyOperatorsInt64();
        public long RollValue(long min, long max) => Randomizer.NextInt64(min, max);
        public long AddValue(long val1, long val2) => val1 + val2;
        public long MultiplyValue(long val1, long val2) => val1 * val2;
    }
}
