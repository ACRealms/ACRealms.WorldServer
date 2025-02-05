using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.Managers;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ACE.Server.Entity;
using ACE.Database.Adapter;
using ACE.Entity.ACRealms;
using Lifestoned.DataModel.Content;
using System.Diagnostics;
using ACE.Server.WorldObjects;
using ACE.Server.Factories;
using ACE.Entity.Enum.RealmProperties;
using System.Drawing.Text;
using System.Collections.Immutable;
using ACE.Server.Factories.Entity;
using ACE.Server.Factories.Tables;
using ACRealms.RealmProps.Underlying;
using ACRealms.RealmProps;
using ACRealms.Rulesets.Enums;
using ACRealms.Rulesets;
using ACRealms.RealmProps.Contexts;

namespace ACE.Server.Realms
{
    public abstract class Ruleset : RulesetBase
    {
        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal Ruleset(ushort rulesetID, RulesetCompilationContext ctx)
        {
            RulesetID = rulesetID;
            Context = ctx;
        }

        protected Random Random => Context.Randomizer;
        public Ruleset ParentRuleset { get; protected set; }
        public ushort RulesetID { get; }
        internal RulesetCompilationContext Context { get; init; }

        internal abstract IDictionary<RealmPropertyBool, IRealmPropertyGroup<bool>> PropertiesBool { get; } //= new Dictionary<RealmPropertyBool, IRealmPropertyGroup<bool>>();
        internal abstract IDictionary<RealmPropertyFloat, IRealmPropertyGroup<double>> PropertiesFloat { get; } //= new Dictionary<RealmPropertyFloat, IRealmPropertyGroup<double>>();
        internal abstract IDictionary<RealmPropertyInt, IRealmPropertyGroup<int>> PropertiesInt { get; } //= new Dictionary<RealmPropertyInt, IRealmPropertyGroup<int>>();
        internal abstract IDictionary<RealmPropertyInt64, IRealmPropertyGroup<long>> PropertiesInt64 { get; } //= new Dictionary<RealmPropertyInt64, IRealmPropertyGroup<long>>();
        internal abstract IDictionary<RealmPropertyString, IRealmPropertyGroup<string>> PropertiesString { get; } //= new Dictionary<RealmPropertyString, IRealmPropertyGroup<string>>();
        internal abstract IDictionary<string, IRealmPropertyGroup> AllProperties { get; } //= new Dictionary<string, IRealmPropertyGroup>();



        private protected static TemplatedRealmPropertyGroup GetRandomProperty(RulesetTemplate template, RulesetCompilationContext ctx)
        {
            if (template.PropertiesForRandomization?.Count == 0)
                return null;
            var selectedProp = template.PropertiesForRandomization[ctx.Randomizer.Next(template.PropertiesForRandomization.Count - 1)];

            return selectedProp;
        }

        private static void LogTrace(RulesetCompilationContext ctx, Func<string> message)
        {
            if (!ctx.Trace)
                return;
            ctx.LogDirect($"   [C] {message()}");
        }

        protected virtual void LogTrace(IEnumerable<string> messages)
        {
            if (!Context.Trace)
                return;
            foreach(var message in messages)
                Context.LogDirect($"{(this is RulesetTemplate ? "[T]" : "[R]")}[{RealmManager.GetRealm(RulesetID, includeRulesets: true).Realm.Name}] {message}");
        }

        protected virtual void LogTrace(Func<string> message)
        {
            if (!Context.Trace)
                return;
            Context.LogDirect($"{(this is RulesetTemplate ? "[T]" : "[R]")}[{RealmManager.GetRealm(RulesetID, includeRulesets: true).Realm.Name}] {message()}");
        }

        internal static RulesetCompilationContext MakeDefaultContext(IDictionary<ushort, WorldRealmBase> dependencies = null)
        {
            dependencies ??= RealmManager.CompilationContextDependencyHandles;
            return RulesetCompilationContext.CreateContext(
                enableSeedTracking: PropertyManager.GetBool("acr_enable_ruleset_seeds", false).Item,
                gitHash: AssemblyInfo.GitHash,
                dependencies: dependencies
            );
        }
    }

    //Properties should not be changed after initial composition
    internal class RulesetTemplate(ushort rulesetID, RulesetCompilationContext ctx) : Ruleset(rulesetID, ctx)
    {
        public Realm Realm { get; protected set; }
        public new RulesetTemplate ParentRuleset
        {
            get { return (RulesetTemplate)base.ParentRuleset; }
            set { base.ParentRuleset = value; }
        }

        internal IReadOnlyList<TemplatedRealmPropertyGroup> PropertiesForRandomization { get; set; }
        internal IReadOnlyList<RealmLinkJob> Jobs { get; private set; }

        internal readonly Dictionary<RealmPropertyBool, TemplatedRealmPropertyGroup<bool>> _propertiesBool = new Dictionary<RealmPropertyBool, TemplatedRealmPropertyGroup<bool>>();
        internal override IDictionary<RealmPropertyBool, IRealmPropertyGroup<bool>> PropertiesBool => (IDictionary<RealmPropertyBool, IRealmPropertyGroup<bool>>)_propertiesBool.Cast<IRealmPropertyGroup<bool>>();

        internal readonly Dictionary<RealmPropertyFloat, TemplatedRealmPropertyGroup<double>> _propertiesFloat = new Dictionary<RealmPropertyFloat, TemplatedRealmPropertyGroup<double>>();
        internal override IDictionary<RealmPropertyFloat, IRealmPropertyGroup<double>> PropertiesFloat => (IDictionary<RealmPropertyFloat, IRealmPropertyGroup<double>>)_propertiesFloat;

        internal readonly Dictionary<RealmPropertyInt, TemplatedRealmPropertyGroup<int>> _propertiesInt = new Dictionary<RealmPropertyInt, TemplatedRealmPropertyGroup<int>>();
        internal override IDictionary<RealmPropertyInt, IRealmPropertyGroup<int>> PropertiesInt => (IDictionary<RealmPropertyInt, IRealmPropertyGroup<int>>)_propertiesInt;

        internal readonly Dictionary<RealmPropertyInt64, TemplatedRealmPropertyGroup<long>> _propertiesInt64 = new Dictionary<RealmPropertyInt64, TemplatedRealmPropertyGroup<long>>();
        internal override IDictionary<RealmPropertyInt64, IRealmPropertyGroup<long>> PropertiesInt64 => (IDictionary < RealmPropertyInt64, IRealmPropertyGroup <long>>)_propertiesInt64;

        internal readonly Dictionary<RealmPropertyString, TemplatedRealmPropertyGroup<string>> _propertiesString = new Dictionary<RealmPropertyString, TemplatedRealmPropertyGroup<string>>();
        internal override IDictionary<RealmPropertyString, IRealmPropertyGroup<string>> PropertiesString => (IDictionary<RealmPropertyString, IRealmPropertyGroup<string>>)_propertiesString;

        internal readonly Dictionary<string, TemplatedRealmPropertyGroup> _allProperties = new Dictionary<string, TemplatedRealmPropertyGroup>();
        internal override IDictionary<string, IRealmPropertyGroup> AllProperties => (IDictionary<string, IRealmPropertyGroup>)_allProperties;

        /// <summary>
        /// Recursively rebuilds the template
        /// </summary>
        /// <param name="cloneFrom"></param>
        /// <param name="rulesetID"></param>
        /// <param name="trace"></param>
        public RulesetTemplate(RulesetTemplate cloneFrom, ushort rulesetID, RulesetCompilationContext ctx)
            : this(rulesetID, ctx)
        {
            if (cloneFrom.ParentRuleset != null)
                ParentRuleset = new RulesetTemplate(cloneFrom.ParentRuleset, cloneFrom.ParentRuleset.RulesetID, ctx);
            LogTrace(() => $"New template (parent: {ParentRuleset?.Realm?.Name ?? "<NONE>"})");
            var realm = RealmManager.GetRealm(rulesetID, includeRulesets: true).Realm;
            LoadPropertiesFrom(realm);
            LogTrace(() => $"Completed template initialization");
        }

        public static RulesetTemplate MakeTopLevelRuleset(Realm entity, RulesetCompilationContext ctx)
        {
            var ruleset = new RulesetTemplate(entity.Id, ctx);
            ruleset.LogTrace(() => $"New template (ROOT)");
            ruleset.LoadPropertiesFrom(entity);
            ruleset.LogTrace(() => $"Completed template initialization");
            return ruleset;
        }

        private void LoadPropertiesFrom(Realm entity)
        {
            var trace = Context.Trace;
            // These really should be "TemplateProperties" and not "AppliedProperties"
            Realm = entity;
            foreach (var kv in entity.PropertiesBool)
            {
                var prop = !trace ? kv.Value : new TemplatedRealmPropertyGroup<bool>(Context, kv.Value, null);
                _propertiesBool.Add(kv.Key, prop);
                _allProperties.Add(prop.Options.Name, prop);
            }
            foreach (var kv in entity.PropertiesFloat)
            {
                var prop = !trace ? kv.Value : new TemplatedRealmPropertyGroup<double>(Context, kv.Value, null);
                _propertiesFloat.Add(kv.Key, prop);
                _allProperties.Add(prop.Options.Name, prop);
            }
            foreach (var kv in entity.PropertiesInt)
            {
                var prop = !trace ? kv.Value : new TemplatedRealmPropertyGroup<int>(Context, kv.Value, null);
                _propertiesInt.Add(kv.Key, prop);
                _allProperties.Add(prop.Options.Name, prop);
            }
            foreach (var kv in entity.PropertiesInt64)
            {
                var prop = !trace ? kv.Value : new TemplatedRealmPropertyGroup<long>(Context, kv.Value, null);
                _propertiesInt64.Add(kv.Key, prop);
                _allProperties.Add(prop.Options.Name, prop);
            }
            foreach (var kv in entity.PropertiesString)
            {
                var prop = !trace ? kv.Value : new TemplatedRealmPropertyGroup<string>(Context, kv.Value, null);
                _propertiesString.Add(kv.Key, prop);
                _allProperties.Add(prop.Options.Name, prop);
            }

            Debug.Assert(entity.AllProperties.Count == _allProperties.Count);

            Jobs = entity.Jobs;
            if (trace)
            {
                foreach(var job in Jobs)
                    LogTrace(job.GetLogTraceMessages(Context.Dependencies));
                foreach(var kv in _allProperties)
                    LogTrace(() => $"Loaded uncomposed property {kv.Key} as {kv.Value}");
            }
           
            MakeRandomizationList();
        }

        public static RulesetTemplate MakeRuleset(RulesetTemplate baseset, Realm subset, RulesetCompilationContext ctx)
        {
            if (baseset.Realm.Type == RealmType.Ruleset && subset.Type == RealmType.Realm)
                throw new Exception("Realms may not inherit from rulesets.");
            var ruleset = new RulesetTemplate(subset.Id, baseset.Context.Trace ? baseset.Context : ctx);
            ruleset.LogTrace(() => $"New (parent: {baseset.Realm.Name})");
            ruleset.ParentRuleset = baseset;
            ruleset.LoadPropertiesFrom(subset);
            return ruleset;
        }

        private void MakeRandomizationList()
        {
            if (Realm.PropertyCountRandomized.HasValue)
            {
                var list = new List<TemplatedRealmPropertyGroup>();
                // TODO: We should be able to make this more efficient eventually, and not use separate AddRange for each. Use AllProperties instead
                // Instead of hard coding on the description, AllProperties should provide (without downcasting) a means
                // to access to the base prototype, specifically the (to be implemented) attribute for allowing randomization
                // An example is that PropertiesString.Description does not allow this, but we were just hardcoding,
                // which would require a downcast to check unless we either do the above, or include more of the base metadata in the new base group classes. 
                list.AddRange(_propertiesBool.Values);
                list.AddRange(_propertiesInt.Values);
                list.AddRange(_propertiesInt64.Values);
                list.AddRange(_propertiesFloat.Values);
                list.AddRange(_propertiesString.Where(x => x.Key != RealmPropertyString.Core_Realm_Description).Select(x => x.Value));

                // This may be a slight performance inefficiency. TODO: Can we easily do this without an extra list?
                PropertiesForRandomization = list.AsReadOnly();
                LogTrace(() => $"Will select {Realm.PropertyCountRandomized.Value} properties at random.");
            }
        }

        public override string ToString()
        {
            return $"Template {Realm.Name}";
        }

        internal AppliedRuleset RecompileWithSeed(int seed, RulesetCompilationContext ctx = null)
        {
            ctx ??= MakeDefaultContext();
            ctx = ctx.WithNewSeed(seed);
            return AppliedRuleset.MakeRerolledRuleset(RebuildTemplateWithContext(ctx), ctx);
        }

        internal RulesetTemplate RebuildTemplateWithContext(RulesetCompilationContext ctx) => new(this, RulesetID, ctx);

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }

    //Properties may be changed freely
    public partial class AppliedRuleset : Ruleset, IAppliedRuleset
    {
        public Landblock Landblock { get; set; }
        internal RulesetTemplate Template { get; }
        internal ACRealms.Rulesets.Realm Realm => Template.Realm;
        public new AppliedRuleset ParentRuleset
        {
            get { return (AppliedRuleset)base.ParentRuleset; }
            set { base.ParentRuleset = value; }
        }
        public LootGenerationFactory LootGenerationFactory { get; private set; }

        private List<ChanceTable<int>> _chanceTableNumCantrips = null;
        public List<ChanceTable<int>> ChanceTableNumCantrips => _chanceTableNumCantrips ?? CantripChance.numCantrips;
        private List<ChanceTable<int>> _chanceTableCantripLevels = null;
        public List<ChanceTable<int>> ChanceTableCantripLevels => _chanceTableCantripLevels ?? CantripChance.cantripLevels;

        internal readonly Dictionary<RealmPropertyBool, ActiveRealmPropertyGroup<bool>> _propertiesBool = new Dictionary<RealmPropertyBool, ActiveRealmPropertyGroup<bool>>();
        internal override IDictionary<RealmPropertyBool, IRealmPropertyGroup<bool>> PropertiesBool => (IDictionary<RealmPropertyBool, IRealmPropertyGroup<bool>>)_propertiesBool;

        internal readonly Dictionary<RealmPropertyFloat, ActiveRealmPropertyGroup<double>> _propertiesFloat = new Dictionary<RealmPropertyFloat, ActiveRealmPropertyGroup<double>>();
        internal override IDictionary<RealmPropertyFloat, IRealmPropertyGroup<double>> PropertiesFloat => (IDictionary<RealmPropertyFloat, IRealmPropertyGroup<double>>)_propertiesFloat;

        internal readonly Dictionary<RealmPropertyInt, ActiveRealmPropertyGroup<int>> _propertiesInt = new Dictionary<RealmPropertyInt, ActiveRealmPropertyGroup<int>>();
        internal override IDictionary<RealmPropertyInt, IRealmPropertyGroup<int>> PropertiesInt => (IDictionary<RealmPropertyInt, IRealmPropertyGroup<int>>)_propertiesInt;

        internal readonly Dictionary<RealmPropertyInt64, ActiveRealmPropertyGroup<long>> _propertiesInt64 = new Dictionary<RealmPropertyInt64, ActiveRealmPropertyGroup<long>>();
        internal override IDictionary<RealmPropertyInt64, IRealmPropertyGroup<long>> PropertiesInt64 => (IDictionary<RealmPropertyInt64, IRealmPropertyGroup<long>>)_propertiesInt64;

        internal readonly Dictionary<RealmPropertyString, ActiveRealmPropertyGroup<string>> _propertiesString = new Dictionary<RealmPropertyString, ActiveRealmPropertyGroup<string>>();
        internal override IDictionary<RealmPropertyString, IRealmPropertyGroup<string>> PropertiesString => (IDictionary<RealmPropertyString, IRealmPropertyGroup<string>>)_propertiesString;

        internal readonly Dictionary<string, ActiveRealmPropertyGroup> _allProperties = new Dictionary<string, ActiveRealmPropertyGroup>();
        internal override IDictionary<string, IRealmPropertyGroup> AllProperties => (IDictionary<string, IRealmPropertyGroup>)_allProperties;

        internal AppliedRuleset(RulesetTemplate template, RulesetCompilationContext ctx) : base(template.RulesetID, ctx)
        {
            Template = template;
        }

        internal string DebugOutputString()
        {
            var sb = new StringBuilder();

            foreach (var item in _propertiesBool)
                sb.AppendLine($"{Enum.GetName(typeof(RealmPropertyBool), item.Key)}: {item.Value}");
            foreach (var item in _propertiesFloat)
                sb.AppendLine($"{Enum.GetName(typeof(RealmPropertyFloat), item.Key)}: {item.Value}");
            foreach (var item in _propertiesInt)
                sb.AppendLine($"{Enum.GetName(typeof(RealmPropertyInt), item.Key)}: {item.Value}");
            foreach (var item in _propertiesInt64)
                sb.AppendLine($"{Enum.GetName(typeof(RealmPropertyInt64), item.Key)}: {item.Value}");
            foreach (var item in _propertiesString.Where(x => x.Key != RealmPropertyString.Core_Realm_Description))
                sb.AppendLine($"{Enum.GetName(typeof(RealmPropertyString), item.Key)}: {item.Value}");

            sb.AppendLine("\n");

            return sb.ToString().Replace("\r", "");
        }

        /// <summary>
        /// Imprints a ruleset template onto a new Active Ruleset, applying all rules as configured
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        internal static AppliedRuleset MakeRerolledRuleset(RulesetTemplate template, RulesetCompilationContext ctx = null)
        {
            ctx ??= Ruleset.MakeDefaultContext();

            var result = new AppliedRuleset(template, ctx);
            result.CopyDicts(template);
            if (template.ParentRuleset != null)
                result.ComposeFrom(template.ParentRuleset);
            foreach (var job in template.Jobs.Where(x => x.Type == RealmRulesetLinkType.apply_after_inherit))
                result.ApplyJob(job);

            // Composition needs to happen first before rerolling,
            // so that random properties that depend on other random
            // properties with a composition rule can be applied in sequence
            result.RerollAllRules();

            result.BuildChanceTablesIfNecessary();

            // Set the loot generation factory here so that it can reference the ruleset
            result.LootGenerationFactory = new LootGenerationFactory(result);

            return result;
        }


        private IEnumerable<TemplatedRealmPropertyGroup> TakeRandomProperties(ushort amount, RulesetTemplate template)
        {
            LogTrace(() => $"Taking {amount} properties at random");
            if (template._propertiesString.TryGetValue(RealmPropertyString.Core_Realm_Description, out var desc))
                amount++;

            var total = template.PropertiesForRandomization.Count;
            if (amount <= total / 2)
            {
                var set = new HashSet<TemplatedRealmPropertyGroup>();
                if (desc != null)
                    set.Add(desc);
                while (set.Count < amount)
                {
                    var selectedProp = GetRandomProperty(template, Context);
                    LogTrace(() => $"Cherry-picking {((IRealmPropertyGroup)selectedProp).OptionsBase.Name} from randomization list");
                    set.Add(selectedProp);
                }
                return set;
            }
            else
            {
                // As an extreme example of why this is needed, imagine if there were 1000 properties, and 999 of them are taken at random.
                // The last few rolls will have as little as 0.1% chance of finding an unused property.
                LogTrace(() => $"{amount} is at least 50% of the randomization properties count of {total}, using subtraction algorithm");
                var all = new List<TemplatedRealmPropertyGroup>(template.PropertiesForRandomization);
                if (amount >= all.Count)
                {
                    LogTrace(() => $"{amount} exceeds the randomization properties count of {all.Count}, skipping set reduction.");
                    return all;
                }
                var set = new HashSet<TemplatedRealmPropertyGroup>(all);
                while (set.Count > amount)
                {
                    var next = GetRandomProperty(template, Context);
                    if (next is TemplatedRealmPropertyGroup<string> s && s.PropertyKey == (int)RealmPropertyString.Core_Realm_Description)
                        continue;
                    LogTrace(() => $"Subtracting {((IRealmPropertyGroup)next).OptionsBase.Name} from randomization selection");
                    set.Remove(next);
                }
                return set;
            }
        }


        private void DictAdd<TKey, TVal>(IDictionary<TKey, ActiveRealmPropertyGroup<TVal>> dict, TKey key, ActiveRealmPropertyGroup<TVal> prop)
            where TKey : Enum
            where TVal : IEquatable<TVal>
        {
            dict.Add(key, prop);
            _allProperties.Add(prop.Options.Name, prop);
        }

        // During init only
        protected private void CopyDicts(RulesetTemplate copyFrom)
        {
            LogTrace(() => "Begin deep clone of properties from template");
            if (copyFrom.Realm.PropertyCountRandomized.HasValue)
            {
                var count = copyFrom.Realm.PropertyCountRandomized.Value;
                var props = TakeRandomProperties(count, copyFrom);
                foreach (var prop in props)
                {
                    if (prop is TemplatedRealmPropertyGroup<bool> a)
                        DictAdd(_propertiesBool, (RealmPropertyBool)a.PropertyKey, new ActiveRealmPropertyGroup<bool>(Context, a, null));
                    else if (prop is TemplatedRealmPropertyGroup<int> b)
                        DictAdd(_propertiesInt, (RealmPropertyInt)b.PropertyKey, new ActiveRealmPropertyGroup<int>(Context, b, null));
                    else if (prop is TemplatedRealmPropertyGroup<double> c)
                        DictAdd(_propertiesFloat, (RealmPropertyFloat)c.PropertyKey, new ActiveRealmPropertyGroup<double>(Context, c, null));
                    else if (prop is TemplatedRealmPropertyGroup<long> d)
                        DictAdd(_propertiesInt64, (RealmPropertyInt64)d.PropertyKey, new ActiveRealmPropertyGroup<long>(Context, d, null));
                    else if (prop is TemplatedRealmPropertyGroup<string> e)
                        DictAdd(_propertiesString, (RealmPropertyString)e.PropertyKey, new ActiveRealmPropertyGroup<string>(Context, e, null));
                }
            }
            else
            {
                foreach (var item in copyFrom._propertiesBool)
                    DictAdd(_propertiesBool, item.Key, new ActiveRealmPropertyGroup<bool>(Context, item.Value, null));
                foreach (var item in copyFrom._propertiesFloat)
                    DictAdd(_propertiesFloat, item.Key, new ActiveRealmPropertyGroup<double>(Context, item.Value, null));
                foreach (var item in copyFrom._propertiesInt)
                    DictAdd(_propertiesInt, item.Key, new ActiveRealmPropertyGroup<int>(Context, item.Value, null));
                foreach (var item in copyFrom._propertiesInt64)
                    DictAdd(_propertiesInt64, item.Key, new ActiveRealmPropertyGroup<long>(Context, item.Value, null));
                foreach (var item in copyFrom._propertiesString)
                    DictAdd(_propertiesString, item.Key, new ActiveRealmPropertyGroup<string>(Context, item.Value, null));
            }
            LogTrace(() => "End deep clone of properties from template");
        }

        private void ApplyJob(RealmLinkJob job)
        {
            if (Context.Trace)
            {
                LogTrace(() => $"ApplyJob start ({job.Type}, {job.Links.Count})");
                foreach (var link in job.Links)
                {
                    LogTrace(() => $"Composition chance of {link.RulesetIDToApply}: {link.Probability.ToString("P2")}");
                }
            }
            foreach (var link in job.Links)
            {
                var roll = Random.NextDouble();
                if (roll < link.Probability)
                {
                    LogTrace(() => $"Rolled {roll}, is < {link.Probability}, applying ruleset, skipping further rolls for this job");
                    var template = ((WorldRealm)Context.Dependencies[link.RulesetIDToApply]).RulesetTemplate;
                    ComposeFrom(template, true);
                    return;
                }
                else
                    LogTrace(() => $"Rolled {roll}, is > {link.Probability}, skipping ruleset");
            }
        }

        private void ComposeFrom(RulesetTemplate parentTemplate, bool invertRules = false)
        {
            if (parentTemplate == null)
                return;
            LogTrace(() => $"BeginCompose (parent: {parentTemplate.Realm.Name}, invertRules: {invertRules})");
            var parent = MakeRerolledRuleset(parentTemplate, Context);
            LogTrace(() => $"ContinueCompose (parent: {parentTemplate.Realm.Name}, invertRules: {invertRules})");

            ApplyRulesetDict(parent._propertiesBool, _propertiesBool, invertRules, Context);
            ApplyRulesetDict(parent._propertiesFloat, _propertiesFloat, invertRules, Context);
            ApplyRulesetDict(parent._propertiesInt, _propertiesInt, invertRules, Context);
            ApplyRulesetDict(parent._propertiesInt64, _propertiesInt64, invertRules, Context);
            ApplyRulesetDict(parent._propertiesString, _propertiesString, invertRules, Context);
            LogTrace(() => $"FinishCompose (parent: {parentTemplate.Realm.Name}, invertRules: {invertRules})");
        }

        private void ApplyRulesetDict<K, V>(
            IDictionary<K, ActiveRealmPropertyGroup<V>> parent,
            IDictionary<K, ActiveRealmPropertyGroup<V>> self,
            bool invertRelationships,
            RulesetCompilationContext ctx)
            where V : IEquatable<V>
            where K : Enum
        {
            //If invertRelationships is true, we are prioritizing the sub dictionary for the purposes of lock, and prioritizing the parent for the purposes of replace.
            //Add and multiply operations can be applied independent of the ordering so they are not affected. 

            //Add all parent items to sub. But if it is already in sub, then calculate using the special composition rules
            foreach (var pair in parent)
            {
                var propName = pair.Value.Options.Name;

                LogTrace(ctx, () => "Begin single property compilation", pair.Value);
                var parentprop = pair.Value;
                if (!self.ContainsKey(pair.Key))
                {
                    LogTrace(ctx, () => $"No property def, using parent '{parentprop.Options.RulesetName}'", pair.Value);
                    //Just add the parent's reference, as we already previously imprinted the parent from its template for this purpose
                    self[pair.Key] = parentprop;
                    _allProperties[propName] = parentprop;
                    continue;
                }

                if (!invertRelationships && parentprop.TrueForAll(o => o.Locked))
                {
                    LogTrace(ctx, () => $"No property def, using parent '{parentprop.Options.RulesetName}'", pair.Value);

                    //Use parent's property as it is locked
                    self[pair.Key] = parentprop;
                    _allProperties[propName] = parentprop;
                    continue;
                }
                else if (invertRelationships && self[pair.Key].TrueForAll(o => o.Locked))
                {
                    LogTrace(ctx, () => $"Locked: Discarding parent '{parentprop.Options.RulesetName}'", pair.Value);
                    continue;
                }

                //Replace
                if (!invertRelationships)
                {
                    var group = self[pair.Key];
                    if (group.TryGetCommonOption(opts => opts.CompositionType, out var result) && result == RealmPropertyCompositionType.replace)
                    {
                        LogTrace(ctx, () => $"Replace: Discarding parent '{parentprop.Options.RulesetName}'", pair.Value);
                        //No need to do anything here since we are replacing the parent
                        continue;
                    }
                }
                else if (invertRelationships)
                {
                    var group = parentprop;
                    if (group.TryGetCommonOption(opts => opts.CompositionType, out var result) && result == RealmPropertyCompositionType.replace)
                    {
                        LogTrace(ctx, () => $"Replace: using parent from invertRelationships: '{parentprop.Options.RulesetName}'", pair.Value);
                        self[pair.Key] = parentprop;
                        _allProperties[propName] = parentprop;
                        continue;
                    }
                }

                //Apply composition rules
                if (!invertRelationships)
                {
                    LogTrace(ctx, () => $"Cloning new composed property with parent {parentprop.Options.RulesetName}", pair.Value);
                    var newAllocatedProp = new ActiveRealmPropertyGroup<V>(ctx, self[pair.Key], parentprop);
                    self[pair.Key] = newAllocatedProp;
                    _allProperties[propName] = newAllocatedProp;
                }
                else
                {
                    LogTrace(ctx, () => $"InvertedRel: cloning new property from '{parentprop.Options.RulesetName}' with parent '{self[pair.Key].Options.RulesetName}'", pair.Value);
                    var newAllocatedProp = new ActiveRealmPropertyGroup<V>(ctx, parentprop, self[pair.Key]);
                    self[pair.Key] = newAllocatedProp; //ensure parent chain is kept
                    _allProperties[propName] = newAllocatedProp;
                }

                LogTrace(ctx, () => "End single property compilation", pair.Value);
            }
        }

        private static void LogTrace(RulesetCompilationContext ctx, Func<string> message, ActiveRealmPropertyGroup property)
        {
            if (!ctx.Trace)
                return;
            ctx.LogDirect($"   [C][{property.OptionsBase.Name}] (Def:{property.OptionsBase.RulesetName}) {message()}");
        }

        private void RerollAllRules()
        {
            LogTrace(() => $"RerollAllRules Begin");
            foreach (var v in _propertiesFloat.Values)
                v.PreEval();
            foreach (var v in _propertiesInt.Values)
                v.PreEval();
            foreach (var v in _propertiesInt64.Values)
                v.PreEval();
            foreach (var v in _propertiesBool.Values)
                v.PreEval();
            foreach (var v in _propertiesString.Values)
                v.PreEval();

            LogTrace(() => $"RerollAllRules End");
        }

        private void BuildChanceTablesIfNecessary()
        {
            if (_propertiesFloat.ContainsKey(RealmPropertyFloat.Loot_DropRates_CantripDropRate) || Props.Loot.DropRates.CantripDropRate(this) != PropertyManager.GetDouble("cantrip_drop_rate").Item)
            {
                LogTrace(() => $"CantripDropRate property is present or differs from server property, building custom NumCantrips ChanceTables");
                _chanceTableNumCantrips = CantripChance.ApplyNumCantripsMod(Props.Loot.DropRates.CantripDropRate(this), false);
            }

            var keys = new (string server_prop, RealmPropertyFloat realm_prop)[] {
                ("minor_cantrip_drop_rate", RealmPropertyFloat.Loot_DropRates_MinorCantripDropRate),
                ("major_cantrip_drop_rate", RealmPropertyFloat.Loot_DropRates_MajorCantripDropRate),
                ("epic_cantrip_drop_rate", RealmPropertyFloat.Loot_DropRates_EpicCantripDropRate),
                ("legendary_cantrip_drop_rate", RealmPropertyFloat.Loot_DropRates_LegendaryCantripDropRate) };
            var appliedLevels = keys.Select(keypair => (
                name: keypair.server_prop,
                server_prop: PropertyManager.GetDouble(keypair.server_prop).Item,
                realm_prop: _propertiesFloat.ContainsKey(keypair.realm_prop) ? (double?)GetProperty(keypair.realm_prop) : null,
                realm_prop_with_fallback: GetProperty(keypair.realm_prop)
            )).ToArray();
            var mapping = appliedLevels.ToDictionary(x => x.name, x => x.realm_prop_with_fallback);
            var needsLevelsTable = appliedLevels.Any(levels => levels.realm_prop.HasValue || levels.realm_prop_with_fallback != levels.server_prop);
            if (needsLevelsTable)
            {
                LogTrace(() => $"Major, Minor, Epic, or Legendary CantripDropRate property is present or differs from server property, building custom CantripLevels ChanceTables");
                _chanceTableCantripLevels = CantripChance.ApplyCantripLevelsMod(
                    mapping["minor_cantrip_drop_rate"],
                    mapping["major_cantrip_drop_rate"],
                    mapping["epic_cantrip_drop_rate"],
                    mapping["legendary_cantrip_drop_rate"],
                    false);
            }
        }

        public bool GetProperty(RealmPropertyBool prop) => ValueOf(prop);
        public bool ValueOf(RealmPropertyBool prop, params IReadOnlyCollection<ValueTuple<string, ICanonicalContextEntity>> ctx)
        {
            var att = RealmPropertyPrototypes.Bool[prop].PrimaryAttribute;
            if (_propertiesBool.TryGetValue(prop, out var value))
                return value.Eval(ctx);
            if (att.DefaultFromServerProperty != null)
                return PropertyManager.GetBool(att.DefaultFromServerProperty, att.DefaultValue).Item;
            return att.DefaultValue;
        }

        public double GetProperty(RealmPropertyFloat prop) => ValueOf(prop);
        public double ValueOf(RealmPropertyFloat prop, params IReadOnlyCollection<ValueTuple<string, ICanonicalContextEntity>> ctx)
        {
            var att = RealmPropertyPrototypes.Float[prop].PrimaryAttribute;
            if (_propertiesFloat.TryGetValue(prop, out var value))
            {
                var val = value.Eval(ctx);
                val = Math.Max(val, att.MinValue);
                val = Math.Min(val, att.MaxValue);
                return val;
            }
            var retval = att.DefaultValue;
            if (att.DefaultFromServerProperty != null)
                retval = PropertyManager.GetDouble(att.DefaultFromServerProperty, att.DefaultValue).Item;
            retval = Math.Max(retval, att.MinValue);
            retval = Math.Min(retval, att.MaxValue);
            return retval;
        }

        public int GetProperty(RealmPropertyInt prop) => ValueOf(prop);
        public int ValueOf(RealmPropertyInt prop, params IReadOnlyCollection<ValueTuple<string, ICanonicalContextEntity>> ctx)
        {
            var proto = RealmPropertyPrototypes.Int[prop];
            // Evaluate ctx vs scopes

            var att = proto.PrimaryAttribute;
            if (_propertiesInt.TryGetValue(prop, out var value))
            {
                var val = value.Eval(ctx);
                val = Math.Max(val, att.MinValue);
                val = Math.Min(val, att.MaxValue);
                return val;
            }
            var retval = att.DefaultValue;
            if (att.DefaultFromServerProperty != null)
            {
                var longval = PropertyManager.GetLong(att.DefaultFromServerProperty, att.DefaultValue).Item;
                longval = Math.Max(longval, att.MinValue);
                longval = Math.Min(longval, att.MaxValue);
                retval = (int)longval;
            }
            else
            {
                retval = Math.Max(retval, att.MinValue);
                retval = Math.Min(retval, att.MaxValue);
            }
            return retval;
        }

        public long GetProperty(RealmPropertyInt64 prop) => ValueOf(prop);
        public long ValueOf(RealmPropertyInt64 prop, params IReadOnlyCollection<ValueTuple<string, ICanonicalContextEntity>> ctx)
        {
            var att = RealmPropertyPrototypes.Int64[prop].PrimaryAttribute;
            if (_propertiesInt64.TryGetValue(prop, out var value))
            {
                var val = value.Eval(ctx);
                val = Math.Max(val, att.MinValue);
                val = Math.Min(val, att.MaxValue);
                return val;
            }
            var retval = att.DefaultValue;
            if (att.DefaultFromServerProperty != null)
                retval = PropertyManager.GetLong(att.DefaultFromServerProperty, att.DefaultValue).Item;
            retval = Math.Max(retval, att.MinValue);
            retval = Math.Min(retval, att.MaxValue);
            return retval;
        }

        public string GetProperty(RealmPropertyString prop) => ValueOf(prop);
        public string ValueOf(RealmPropertyString prop, params IReadOnlyCollection<ValueTuple<string, ICanonicalContextEntity>> ctx)
        {
            var att = RealmPropertyPrototypes.String[prop].PrimaryAttribute;
            if (_propertiesString.TryGetValue(prop, out var value))
                return value.Eval(ctx);
            if (att.DefaultFromServerProperty != null)
                return PropertyManager.GetString(att.DefaultFromServerProperty, att.DefaultValue).Item;
            return att.DefaultValue;
        }

        public uint GetDefaultInstanceID(IPlayer player, LocalPosition position)
        {
            RealmManager.TryParseReservedRealm(Realm.Id, out var r);
            return r switch
            {
                // RealmSelector and Hideout uses separate instance ID for each account
                ReservedRealm.RealmSelector or ReservedRealm.hideout or ReservedRealm.NULL => ACE.Entity.Position.InstanceIDFromVars(Realm.Id, (ushort)player.Account.AccountId, false),
                _ => GetFullInstanceID(GetDefaultShortInstanceID(player, position))
            };
        }

        internal bool ClassicalInstancesActivated(IPlayer player, LocalPosition position)
        {
            if (!Props.Peripheral.ClassicalInstance.Enabled(this))
                return false;

            if (player.GetProperty(PropertyBool.ClassicalInstancesActive) != true)
            {
                if (!Props.Peripheral.ClassicalInstance.IgnoreCharacterProp(this))
                    return false;
            }

            if (Props.Peripheral.ClassicalInstance.EnableForAllLandblocksDangerous(this))
                return true;

            var set = Props.Peripheral.ClassicalInstance.DungeonSet(this);
            return RealmManager.Peripherals.DungeonSets.IncludedInSet(position, set);
        }

        private ushort GetDefaultShortInstanceID(IPlayer player, LocalPosition position)
        {
            if (player.GetProperty(PropertyBool.AttemptUniqueInstanceID) == true) // TODO: Merge this with Classical Instances
                return ShortInstanceIDForClassicalNonEphemeralInstances_PerCharacter(player);

            if (ClassicalInstancesActivated(player, position))
                return GetClassicalShortInstanceID(player);
            else
                return 0;
        }

        internal ushort GetClassicalShortInstanceID(IPlayer player, bool isFellowLeader = false)
        {
            if (!isFellowLeader && player is Player p && p.Fellowship != null)
            {
                if (p.Fellowship.FellowshipLeaderGuid != player.Guid.Full)
                {
                    var members = p.Fellowship.GetFellowshipMembers();
                    if (members.TryGetValue(p.Fellowship.FellowshipLeaderGuid, out var leader))
                    {
                        return GetClassicalShortInstanceID(leader, isFellowLeader: true);
                    }
                    else
                    {
                        log.Warn($"GetClassicalShortInstanceID: Unable to get fellowship leader for {player.Name} (previously known leader guid {p.Fellowship.FellowshipLeaderGuid}). Using player's own ClassicalShortInstanceID");
                    }
                }
            }

            if (Props.Peripheral.ClassicalInstance.ShareWithPlayerAccount(this))
                return ShortInstanceIDForClassicalNonEphemeralInstances_PerAccount(player);
            else
                return ShortInstanceIDForClassicalNonEphemeralInstances_PerCharacter(player);
        }

        private static ushort ShortInstanceIDForClassicalNonEphemeralInstances_PerCharacter(IPlayer player) => (ushort)((player.Guid.Full % 0xFFFE) + 1);
        public static ushort ShortInstanceIDForClassicalNonEphemeralInstances_PerAccount(IPlayer player) => (ushort)((player.Account.AccountId % 0xFFFE) + 1);

        public uint GetFullInstanceID(ushort shortInstanceID)
        {
            return ACE.Entity.Position.InstanceIDFromVars(Template.Realm.Id, shortInstanceID, Template.Realm.Type == RealmType.Ruleset);
        }

        public uint GetDefaultInstanceID(LocalPosition position) => GetFullInstanceID(0);
        
        public override string ToString()
        {
            return $"Applied Ruleset {Realm.Name}";
        }
    }
}
