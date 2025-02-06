using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using ACRealms;
using ACRealms.Prototypes;
using ACRealms.RealmProps;
using ACRealms.RealmProps.Contexts;
using ACRealms.RealmProps.Underlying;
using ACRealms.Rulesets.Contexts;
using ACRealms.Rulesets.Enums;
using static ACRealms.Rulesets.RulesetCompilationContext;

namespace ACRealms.Rulesets
{
    /// <summary>
    /// Synthesized "Dat" for a realm. Not portable between installations whatsoever, even with the same AC Realms version.
    /// The Roslyn Compiler extension (producing ACRealms.RealmProps.dll) allows for synthesis of our canonical underlying types for the properties themselves.
    /// This causes the property's primary key (the enum value as a raw System.Int32) to be automatically assigned
    /// We still have realms.jsonc (lockfile) to safely auto assign realms
    /// The net result is that we have completely removed the database from the pipeline, and all persistence for anything having to do with ruleset options.
    /// As our only persistence is the lockfile, we rely on the ruleset source as well as the RealmProps compilation to produce realm metadata at runtime.
    /// This may produce different results between runs depending on the lockfile, 
    ///  or possibly depending on mods, if any mods were to need to change the internals of ACRealms for some reason.
    ///  If you're forking or developing a mod, I recommend those core modifications in production only as an absolute last resort,
    ///  and only after you get in touch to help me understand why this is needed so I can attempt to address it (it may save you a lot of headache if it wasn't necessary).
    /// These hypothetical differences, if any, are believed to be limited to the realm Id.
    /// Nonetheless, the message I want to convey is that we should think of this class as representing a Dat file for a realm, but with the proviso that the
    /// data it contians is only valid for the exact running version of the app, including lockfile and mods. Therefore, we'll likely never have serialization as an option,
    /// nor should anyone try to do so. It provides little known benefit, and isn't safely usable without architecture knowledge.
    /// We do have a compilation DebugLog which can emit everything needed to recreate a test, but it may need fixing before the next major release.
    /// The one possible reason I can see to serialize this is to allow for easier troubleshooting and integration with Debug Log, or unit tests.
    /// </summary>
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
        : IAppliedRealmProperty
        where TVal : IEquatable<TVal>
    {
        new RealmPropertyOptions<TVal> Options { get; }
    }


    internal abstract class AppliedRealmProperty(RulesetCompilationContext ctx) : IAppliedRealmProperty
    {
        public int PropertyKey { get; internal init; }
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
            Context.LogDirect($"   [P][{Options.GroupOptionsBase.Name}] (Def:{Options.GroupOptions.RulesetName}) {message()}");
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

        private bool rolledOnce = false;
        private TVal Value { get; set; }

        private TVal DoEval(IReadOnlyCollection<(string, RealmProps.Contexts.ICanonicalContextEntity)> worldCtx, bool direct = true, bool scopelessPreEval = false)
        {
            if (rolledOnce && Options.RandomType != RealmPropertyRerollType.always)
                return Value;

            TVal composedFromValue;
            if (Parent != null)
            {
                if (direct)
                    LogTrace(() => Group.GetAppliedParentChain());

                composedFromValue = Parent.Eval(worldCtx, direct: false, scopelessPreEval: scopelessPreEval);

                // 1-20-2025 - FlaggAC
                // This might have been bugged before in the previous versions (2.1.x or earlier)
                // TODO: Check for bug in previous realms: If ruleset parent randomizes,
                // then child ruleset composes but only randomizes in a small narrow range,
                // with reroll always on child and reroll landblock on parent.
                // then is the parent still rerolled always? it should be rerolled only for landblock.
                // composedFromValue = Parent.RollValue(false);
                // composedFromValue = Parent.Value;
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
            return val;
        }

        internal bool TryEval(IReadOnlyCollection<(string, RealmProps.Contexts.ICanonicalContextEntity)> worldCtx, out TVal value, bool scopelessPreEval = false)
        {
            if (scopelessPreEval || ScopeMatch(worldCtx))
            {
                value = DoEval(worldCtx);
                return true;
            }
            value = default;
            return false;
        }

        private bool ScopeMatch(IReadOnlyCollection<(string, RealmProps.Contexts.ICanonicalContextEntity)> ctx)
        {
            var scopeDefs = this.Options.Scope.Scopes;
            return ctx.All(thisCtx => 
            {
                (string contextParamKey, ICanonicalContextEntity contextEntityBase) = thisCtx;

                
                if (!scopeDefs.TryGetValue(contextParamKey, out var scopeDefBase))
                    return true; // Scope wasn't narrowed on this entity
                foreach(var (scopePropKey, scopeValUntyped) in scopeDefBase)
                {
                    // TODO: Handle possibility of prototypes being different depending on key (i.e. biota vs player)
                    var entityProtos = contextEntityBase.Prototypes;
                    var proto = entityProtos.AllPrototypes[scopePropKey];
                    if (proto is IValuePrototype vProto)
                    {
                        if (vProto.TryFetchValue(contextEntityBase, out ValueType val))
                        {
                            if (!scopeValUntyped.MatchValue(val))
                                return false;
                        }
                        else if (!scopeValUntyped.MatchNull())
                            return false;
                    }
                    else if (proto is IObjectPrototype refProto)
                    {
                        if (refProto.TryFetchObject(contextEntityBase, out object val))
                        {
                            if (!scopeValUntyped.MatchObject(val))
                                return false;
                        }
                        else if (!scopeValUntyped.MatchNull())
                            return false;
                    }
                    else
                        throw new InvalidCastException("Prototype was neither IObjectPrototype or IValuePrototype");
                }
                return true;
            });
        }

        public override string ToString()
        {
            string val;
            if (!rolledOnce)
                val = "<Deferred Load>";
            else
                val = Value.ToString();

            return Options.AppliedInfo(val);
        }
    }

    internal abstract record RealmPropertyGroupOptions()
    {
        internal string Name { get; private init; }
        public string RulesetName { get; init; }
        internal RealmPropertyPrototypeBase PrototypeBase { get; init; }

        protected RealmPropertyGroupOptions(RealmPropertyPrototypeBase prototypeBase, string rulesetName, string propKey)
            : this()
        {
            PrototypeBase = prototypeBase;
            RulesetName = rulesetName;
            Name = propKey;
        }
    }

    internal record RealmPropertyGroupOptions<TVal> : RealmPropertyGroupOptions
        where TVal : IEquatable<TVal>
    {
        
        internal RealmPropertyPrototype<TVal> Prototype { get; private init; }
        internal TVal HardDefaultValue { get; private init; }

        internal RealmPropertyGroupOptions(RealmPropertyPrototype<TVal> prototype, string rulesetName, string propKey, TVal hardDefaultValue)
            : base(prototype, rulesetName, propKey)
        {
            HardDefaultValue = hardDefaultValue;
            Prototype = prototype;
        }
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
        public bool Locked { get; init; }
        public double Probability { get; init; }
        public virtual RealmPropertyRerollType RandomType { get => RealmPropertyRerollType.never; protected init { } }
        public virtual RealmPropertyCompositionType CompositionType { get => RealmPropertyCompositionType.replace; protected init { } }
        public RealmPropertyScopeOptions Scope { get; init; }
        private Lazy<string> TemplateDisplayString { get; init; }
        //protected RealmPropertyOptions(RealmPropertyPrototypeBase prototype, string name, string rulesetName, Type type, Type enumType, int enumValue, RealmPropertyScopeOptions scopeOptions)
        protected RealmPropertyOptions(RealmPropertyGroupOptions groupOptions, Type type, Type enumType, int enumValue, RealmPropertyScopeOptions scopeOptions)
        {
            GroupOptionsBase = groupOptions;
            ValueType = type;
            EnumType = enumType;
            EnumValueRaw = enumValue;
            TemplateDisplayString = new Lazy<string>(TemplateInfo, System.Threading.LazyThreadSafetyMode.PublicationOnly);
            Scope = scopeOptions;
        }

        protected void Log(RulesetCompilationContext ctx, Func<string> message)
        {
            if (!ctx.Trace)
                return;
            ctx.LogDirect($"   [T][{GroupOptionsBase.Name}] (Def: {GroupOptionsBase.RulesetName}) {message()}");
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
        internal RealmPropertyOptions(RealmPropertyGroupOptions groupOptions, 
            TPrimitive defaultValue, bool locked, double? probability, Type enumType, int enumValue,
            RealmPropertyScopeOptions scopeOptions)
            : base(groupOptions, typeof(TPrimitive), enumType, enumValue, scopeOptions)
        {
            GroupOptions = (RealmPropertyGroupOptions<TPrimitive>)groupOptions;
            Locked = locked;
            Probability = probability ?? 1.0;
            DefaultValue = defaultValue;
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

        internal MinMaxRangedRealmPropertyOptions(RealmPropertyGroupOptions group, T defaultValue,
            byte compositionType, bool locked, double? probability, Type enumType, int enumValue,
            RealmPropertyScopeOptions scopeOptions)
            : base(group, defaultValue, locked, probability, enumType, enumValue, scopeOptions)
        {
            RandomType = RealmPropertyRerollType.never;
            CompositionType = (RealmPropertyCompositionType)compositionType;
        }

        internal MinMaxRangedRealmPropertyOptions(RealmPropertyGroupOptions group, T defaultValue,
            byte compositionType, byte randomType, T randomLowRange, T randomHighRange, bool locked,
            double? probability, Type enumType, int enumValue,
            RealmPropertyScopeOptions scopeOptions)
            : base(group, defaultValue, locked, probability, enumType, enumValue, scopeOptions)
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
