using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using ACRealms;
using ACRealms.RealmProps;
using ACRealms.RealmProps.Underlying;
using ACRealms.Rulesets.Enums;
using static ACRealms.Rulesets.RulesetCompilationContext;

namespace ACRealms.Rulesets
{
    internal class Realm
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public RealmType Type { get; set; }
        public ushort? ParentRealmID { get; set; }
        public ushort? PropertyCountRandomized { get; set; }

        public IDictionary<RealmPropertyBool, TemplatedRealmPropertyGroup<bool>> PropertiesBool { get; set; }
        public IDictionary<RealmPropertyFloat, TemplatedRealmPropertyGroup<double>> PropertiesFloat { get; set; }
        public IDictionary<RealmPropertyInt, TemplatedRealmPropertyGroup<int>> PropertiesInt { get; set; }
        public IDictionary<RealmPropertyInt64, TemplatedRealmPropertyGroup<long>> PropertiesInt64 { get; set; }
        public IDictionary<RealmPropertyString, TemplatedRealmPropertyGroup<string>> PropertiesString { get; set; }
        public IDictionary<string, TemplatedRealmPropertyGroup> AllProperties { get; set; }

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

    internal class RealmLinkJob
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

        public List<string> GetLogTraceMessages(IDictionary<ushort, WorldRealmBase> compilationDependencies)
        {
            var list = new List<string>();
            list.Add($"Job of type {Type}, {Links.Count} links");
            list.AddRange(Links.Select(x => x.GetLogTrace(compilationDependencies[x.RulesetIDToApply].Realm.Name)));
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

    internal interface IAppliedRealmProperty
    {
        public int PropertyKey { get; }
        public RealmPropertyOptions Options { get; }
        public Type ValueType { get; }
    }

    internal interface IAppliedRealmProperty<TVal>
    {
    }


    internal abstract class AppliedRealmProperty(RulesetCompilationContext ctx) : IAppliedRealmProperty
    {
        public int PropertyKey { get; protected set; }
        public RealmPropertyOptions Options { get; init; }
        public Type ValueType => Options.ValueType;
        protected RulesetCompilationContext Context { get; } = ctx;
    }
    internal abstract class AppliedRealmProperty<TVal>(RulesetCompilationContext ctx) : AppliedRealmProperty(ctx), IAppliedRealmProperty<TVal>
        where TVal : IEquatable<TVal>
    {
        public new RealmPropertyOptions<TVal> Options { get => (RealmPropertyOptions<TVal>)base.Options; init => base.Options = value; }


        protected void LogTrace(Func<string> message)
        {
            if (!Context.Trace)
                return;
            Context.LogDirect($"   [P][{Options.Name}] (Def:{Options.RulesetName}) {message()}");
        }


        public override string ToString() => Options.AppliedInfo("<Deferred Load>");
    }

    internal interface ITemplatedRealmProperty : IAppliedRealmProperty
    {

    }

    internal sealed class TemplatedRealmProperty<TVal>(RulesetCompilationContext ctx) : AppliedRealmProperty<TVal>(ctx), ITemplatedRealmProperty, IAppliedRealmProperty<TVal>
        where TVal : IEquatable<TVal>
    {
        /// <summary>
        /// Constructor for realm property from database
        /// </summary>
        /// <param name="propertyKey"></param>
        /// <param name="options"></param>
        /// <param name="traceLog"></param>
        public TemplatedRealmProperty(RulesetCompilationContext ctx, int propertyKey, RealmPropertyOptions<TVal> options)
            : this(ctx)
        {
            PropertyKey = propertyKey;
            Options = options;
        }
    }

    internal sealed class ActiveRealmProperty<TVal>(RulesetCompilationContext ctx) : AppliedRealmProperty<TVal>(ctx)
        where TVal : IEquatable<TVal>
    {
        public required ActiveRealmPropertyGroup<TVal> Group { get; init; }
        public ActiveRealmPropertyGroup<TVal> Parent => Group.Parent;

        private TVal _value;
        private bool rolledOnce = false;
        internal TVal Value
        {
            get
            {
                if (Options.RandomType == RealmPropertyRerollType.always)
                    RollValue();
                return _value;
            }
            private set => _value = value;
        }

        internal bool TryEval(IReadOnlyCollection<(string, IRealmPropContext)> ctx, out TVal value)
        {
            if (ScopeMatch(ctx))
            {
                value = Value;
                return true;
            }
            value = default;
            return false;
        }

        private bool ScopeMatch(IReadOnlyCollection<(string, IRealmPropContext)> ctx)
        {
            return false;
            foreach (var (key, val) in ctx)
            {

            }
        }

        //Clone
        public ActiveRealmProperty(RulesetCompilationContext ctx, AppliedRealmProperty<TVal> prop, ActiveRealmPropertyGroup<TVal> parent = null)
            : this(ctx)
        {
            PropertyKey = prop.PropertyKey;
            Options = prop.Options;
            LogTrace(() => $"Cloning from template. CompositionType: {Options.CompositionType}");

            if (Options.CompositionType != RealmPropertyCompositionType.replace)
            {
                if (parent != null)
                {
                    LogTrace(() => $"Setting parent (explicitly passed): {parent.Options.RulesetName}");
                    Parent = new ActiveRealmProperty<TVal>(ctx, parent, null);
                }
                else if (prop.Parent != null)
                {
                    LogTrace(() => $"Setting parent (from template parent): {prop.Parent.Options.RulesetName}");
                    Parent = new ActiveRealmProperty<TVal>(ctx, prop.Parent, null);
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


        private string GetAppliedParentChain(StringBuilder sb = null)
        {
            bool direct = sb == null;
            sb ??= new StringBuilder();
            if (!direct) sb.AppendFormat("<-{0}", Options.RulesetName);
            if (Parent != null) Parent.GetAppliedParentChain(sb);
            return direct ? sb.ToString() : null;
        }

        public TVal RollValue(bool direct = true)
        {
            TVal composedFromValue;
            if (Parent != null)
            {
                if (direct)
                    LogTrace(() => GetAppliedParentChain());

                composedFromValue = Parent.RollValue(false);
                //composedFromValue = Parent.Value;
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
    }

    internal abstract record RealmPropertyGroupOptions
    {
        internal virtual RealmPropertyPrototypeBase PrototypeBase { get; init; }
    }

    internal record RealmPropertyGroupOptions<TVal> : RealmPropertyGroupOptions
        where TVal : IEquatable<TVal>
    {
        public RealmPropertyPrototype<TVal> Prototype { get; private init; }
    }

    /// <summary>
    /// <para>Represents the immutable property definition from the ruleset config file </para>
    /// <para>The RulesetName is guaranteed to be linked to the ruleset file where defined</para>
    /// </summary>
    internal abstract record RealmPropertyOptions
    {
        public RealmPropertyGroupOptions GroupOptionsBase { get; init; }
        public Type ValueType { get; init; }
        public Type EnumType { get; init; }
        public int EnumValueRaw { get; init; }
        public string Name { get; init; }
        public string RulesetName { get; init; }
        public bool Locked { get; init; }
        public double Probability { get; init; }
        public virtual RealmPropertyRerollType RandomType { get => RealmPropertyRerollType.never; protected init { } }
        public virtual RealmPropertyCompositionType CompositionType { get => RealmPropertyCompositionType.replace; protected init { } }
        public RealmPropertyScopeOptions Scope { get; init; }
        private Lazy<string> TemplateDisplayString { get; init; }
        //protected RealmPropertyOptions(RealmPropertyPrototypeBase prototype, string name, string rulesetName, Type type, Type enumType, int enumValue, RealmPropertyScopeOptions scopeOptions)
        protected RealmPropertyOptions(RealmPropertyGroupOptions groupOptions, string name, string rulesetName, Type type, Type enumType, int enumValue, RealmPropertyScopeOptions scopeOptions)
        {
            GroupOptionsBase = groupOptions;
            ValueType = type;
            EnumType = enumType;
            Name = name;
            RulesetName = rulesetName;
            EnumValueRaw = enumValue;
            TemplateDisplayString = new Lazy<string>(TemplateInfo, System.Threading.LazyThreadSafetyMode.PublicationOnly);
            Scope = scopeOptions;
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

    internal record RealmPropertyOptions<TPrimitive> : RealmPropertyOptions
        where TPrimitive : IEquatable<TPrimitive>
    {
        public RealmPropertyPrototype<TPrimitive> Prototype => GroupOptions.Prototype;
        public TPrimitive HardDefaultValue { get; private init; }
        public TPrimitive DefaultValue { get; private init; }
        public RealmPropertyGroupOptions<TPrimitive> GroupOptions { get; init; }

        //internal RealmPropertyOptions(RealmPropertyPrototype<TPrimitive> prototype, string name, string rulesetName,
        internal RealmPropertyOptions(RealmPropertyGroupOptions groupOptions, string name, string rulesetName,
            TPrimitive hardDefaultValue, TPrimitive defaultValue, bool locked, double? probability, Type enumType, int enumValue,
            RealmPropertyScopeOptions scopeOptions)
            : base(groupOptions, name, rulesetName, typeof(TPrimitive), enumType, enumValue, scopeOptions)
        {
            GroupOptions = (RealmPropertyGroupOptions<TPrimitive>)groupOptions;
            Locked = locked;
            Probability = probability ?? 1.0;
            DefaultValue = defaultValue;
            HardDefaultValue = hardDefaultValue;
        }

        public virtual TPrimitive RollValue(RulesetCompilationContext ctx)
        {
            Log(ctx, () => $"Value: {DefaultValue}");
            return DefaultValue;
        }

        public virtual TPrimitive Compose(TPrimitive parentValue, TPrimitive rolledValue, RulesetCompilationContext ctx)
        {
            Log(ctx, () => $"Compose: Replacing parent {parentValue} with {rolledValue}");
            return DefaultValue;
        }

        protected override string TemplateInfo() => $"Default: {DefaultValue}, Locked: {Locked}, Probability: {Probability}";
        public override string AppliedInfo(string val) => $"Value: {val}";
    }

    internal record MinMaxRangedRealmPropertyOptions<T> : RealmPropertyOptions<T>
        where T : IMinMaxValue<T>, IEquatable<T>, IComparable<T>
    {
        private Func<RulesetCompilationContext, RulesetCompilationContext.IPropertyOperatorsMinMax<T>> Operator { get; init; }
        public T MinValue { get; init; }
        public T MaxValue { get; init; }
        public override RealmPropertyRerollType RandomType { get; protected init; }
        public override RealmPropertyCompositionType CompositionType { get; protected init; }

        internal MinMaxRangedRealmPropertyOptions(RealmPropertyGroupOptions group, string name, string rulesetName, T hardDefaultValue, T defaultValue,
            byte compositionType, bool locked, double? probability, Type enumType, int enumValue,
            RealmPropertyScopeOptions scopeOptions)
            : base(group, name, rulesetName, hardDefaultValue, defaultValue, locked, probability, enumType, enumValue, scopeOptions)
        {
            RandomType = RealmPropertyRerollType.never;
            CompositionType = (RealmPropertyCompositionType)compositionType;
        }

        internal MinMaxRangedRealmPropertyOptions(RealmPropertyGroupOptions group, string name, string rulesetName, T hardDefaultValue,
            byte compositionType, byte randomType, T randomLowRange, T randomHighRange, bool locked,
            double? probability, Type enumType, int enumValue,
            RealmPropertyScopeOptions scopeOptions)
            : base(group, name, rulesetName, hardDefaultValue, hardDefaultValue, locked, probability, enumType, enumValue, scopeOptions)
        {
            RandomType = (RealmPropertyRerollType)randomType;
            if (group.PrototypeBase.TryGetSecondaryValue<RerollRestrictedToAttribute, RealmPropertyRerollType>((att) => att.ConstrainRerollType(RandomType), out RealmPropertyRerollType modifiedRerollType))
                RandomType = modifiedRerollType;

            if (typeof(T) == typeof(double))
                Operator = new Func<RulesetCompilationContext, IPropertyOperatorsMinMax<T>>((ctx) => (IPropertyOperatorsMinMax<T>)ctx.Operators.Float);
            else if (typeof(T) == typeof(long))
                Operator = new Func<RulesetCompilationContext, IPropertyOperatorsMinMax<T>>((ctx) => (IPropertyOperatorsMinMax<T>)ctx.Operators.Int64);
            else if (typeof(T) == typeof(int))
                Operator = new Func<RulesetCompilationContext, IPropertyOperatorsMinMax<T>>((ctx) => (IPropertyOperatorsMinMax<T>)ctx.Operators.Int);
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
            return Operator(ctx).RollValue(MinValue, MaxValue);
        }

        public override T Compose(T parentValue, T rolledValue, RulesetCompilationContext ctx)
        {
            Log(ctx, () => $"Compose {CompositionType}({parentValue}, {rolledValue})");
            switch (CompositionType)
            {
                case RealmPropertyCompositionType.replace:
                    return rolledValue;
                case RealmPropertyCompositionType.add:
                    return Operator(ctx).AddValue(parentValue, rolledValue);
                case RealmPropertyCompositionType.multiply:
                    return Operator(ctx).MultiplyValue(parentValue, rolledValue);
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
}
