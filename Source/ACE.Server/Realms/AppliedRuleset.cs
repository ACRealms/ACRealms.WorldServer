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

namespace ACE.Server.Realms
{
    public abstract class Ruleset(ushort rulesetID, RulesetCompilationContext ctx)
    {
        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected Random Random => Context.Randomizer;
        public Ruleset ParentRuleset { get; protected set; }
        public ushort RulesetID { get; } = rulesetID;
        public RulesetCompilationContext Context { get; init; } = ctx;

        internal IDictionary<RealmPropertyBool, AppliedRealmProperty<bool>> PropertiesBool { get; } = new Dictionary<RealmPropertyBool, AppliedRealmProperty<bool>>();
        internal IDictionary<RealmPropertyFloat, AppliedRealmProperty<double>> PropertiesFloat { get; } = new Dictionary<RealmPropertyFloat, AppliedRealmProperty<double>>();
        internal IDictionary<RealmPropertyInt, AppliedRealmProperty<int>> PropertiesInt { get; } = new Dictionary<RealmPropertyInt, AppliedRealmProperty<int>>();
        internal IDictionary<RealmPropertyInt64, AppliedRealmProperty<long>> PropertiesInt64 { get; } = new Dictionary<RealmPropertyInt64, AppliedRealmProperty<long>>();
        internal IDictionary<RealmPropertyString, AppliedRealmProperty<string>> PropertiesString { get; } = new Dictionary<RealmPropertyString, AppliedRealmProperty<string>>();
        internal IDictionary<string, AppliedRealmProperty> AllProperties { get; } = new Dictionary<string, AppliedRealmProperty>();

        protected void ApplyRulesetDict<K, V>(
            IDictionary<K, AppliedRealmProperty<V>> parent,
            IDictionary<K, AppliedRealmProperty<V>> self,
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
                    AllProperties[propName] = parentprop;
                    continue;
                }

                if (!invertRelationships && parentprop.Options.Locked)
                {
                    LogTrace(ctx, () => $"No property def, using parent '{parentprop.Options.RulesetName}'", pair.Value);

                    //Use parent's property as it is locked
                    self[pair.Key] = parentprop;
                    AllProperties[propName] = parentprop;
                    continue;
                }
                else if (invertRelationships && self[pair.Key].Options.Locked)
                {
                    LogTrace(ctx, () => $"Locked: Discarding parent '{parentprop.Options.RulesetName}'", pair.Value);
                    continue;
                }

                //Replace
                if (!invertRelationships && self[pair.Key].Options.CompositionType == RealmPropertyCompositionType.replace)
                {
                    LogTrace(ctx, () => $"Replace: Discarding parent '{parentprop.Options.RulesetName}'", pair.Value);
                    //No need to do anything here since we are replacing the parent
                    continue;
                }
                else if (invertRelationships && parentprop.Options.CompositionType == RealmPropertyCompositionType.replace)
                {
                    LogTrace(ctx, () => $"Replace: using parent from invertRelationships: '{parentprop.Options.RulesetName}'", pair.Value);
                    self[pair.Key] = parentprop;
                    AllProperties[propName] = parentprop;
                    continue;
                }

                //Apply composition rules
                if (!invertRelationships)
                {
                    LogTrace(ctx, () => $"Cloning new composed property with parent {parentprop.Options.RulesetName}", pair.Value);
                    var newAllocatedProp = new AppliedRealmProperty<V>(ctx, self[pair.Key], parentprop);
                    self[pair.Key] = newAllocatedProp;
                    AllProperties[propName] = newAllocatedProp;
                }
                else
                {
                    LogTrace(ctx, () => $"InvertedRel: cloning new property from '{parentprop.Options.RulesetName}' with parent '{self[pair.Key].Options.RulesetName}'", pair.Value);
                    var newAllocatedProp = new AppliedRealmProperty<V>(ctx, parentprop, self[pair.Key]);
                    self[pair.Key] = newAllocatedProp; //ensure parent chain is kept
                    AllProperties[propName] = newAllocatedProp;
                }

                LogTrace(ctx, () => "End single property compilation", pair.Value);
            }
        }

        private static void LogTrace(RulesetCompilationContext ctx, Func<string> message)
        {
            if (!ctx.Trace)
                return;
            ctx.LogDirect($"   [C] {message()}");
        }
            
        private static void LogTrace(RulesetCompilationContext ctx, Func<string> message, AppliedRealmProperty property)
        {
            if (!ctx.Trace)
                return;
            ctx.LogDirect($"   [C][{property.Options.Name}] (Def:{property.Options.RulesetName}) {message()}");
        }


        private static AppliedRealmProperty GetRandomProperty(RulesetTemplate template, RulesetCompilationContext ctx)
        {
            if (template.PropertiesForRandomization?.Count == 0)
                return null;
            var selectedProp = template.PropertiesForRandomization[ctx.Randomizer.Next(template.PropertiesForRandomization.Count - 1)];

            return selectedProp;
        }

        private void DictAdd<TKey, TVal>(IDictionary<TKey, AppliedRealmProperty<TVal>> dict, TKey key, AppliedRealmProperty<TVal> prop)
            where TKey : Enum
            where TVal : IEquatable<TVal>
        {
            dict.Add(key, prop);
            AllProperties.Add(prop.Options.Name, prop);
        }

        protected void CopyDicts(RulesetTemplate copyFrom)
        {
            LogTrace(() => "Begin deep clone of properties from template");
            if (copyFrom.Realm.PropertyCountRandomized.HasValue)
            {
                var count = copyFrom.Realm.PropertyCountRandomized.Value;
                var props = TakeRandomProperties(count, copyFrom);
                foreach (var prop in props)
                {
                    if (prop is AppliedRealmProperty<bool> a)
                        DictAdd(PropertiesBool, (RealmPropertyBool)a.PropertyKey, new AppliedRealmProperty<bool>(Context, a, null));
                    else if (prop is AppliedRealmProperty<int> b)
                        DictAdd(PropertiesInt, (RealmPropertyInt)b.PropertyKey, new AppliedRealmProperty<int>(Context, b, null));
                    else if (prop is AppliedRealmProperty<double> c)
                        DictAdd(PropertiesFloat, (RealmPropertyFloat)c.PropertyKey, new AppliedRealmProperty<double>(Context, c, null));
                    else if (prop is AppliedRealmProperty<long> d)
                        DictAdd(PropertiesInt64, (RealmPropertyInt64)d.PropertyKey, new AppliedRealmProperty<long>(Context, d, null));
                    else if (prop is AppliedRealmProperty<string> e)
                        DictAdd(PropertiesString, (RealmPropertyString)e.PropertyKey, new AppliedRealmProperty<string>(Context, e, null));
                }
            }
            else
            {
                foreach (var item in copyFrom.PropertiesBool)
                    DictAdd(PropertiesBool, item.Key, new AppliedRealmProperty<bool>(Context, item.Value, null));
                foreach (var item in copyFrom.PropertiesFloat)
                    DictAdd(PropertiesFloat, item.Key, new AppliedRealmProperty<double>(Context, item.Value, null));
                foreach (var item in copyFrom.PropertiesInt)
                    DictAdd(PropertiesInt, item.Key, new AppliedRealmProperty<int>(Context, item.Value, null));
                foreach (var item in copyFrom.PropertiesInt64)
                    DictAdd(PropertiesInt64, item.Key, new AppliedRealmProperty<long>(Context, item.Value, null));
                foreach (var item in copyFrom.PropertiesString)
                    DictAdd(PropertiesString, item.Key, new AppliedRealmProperty<string>(Context, item.Value, null));
            }
            LogTrace(() => "End deep clone of properties from template");
        }

        private IEnumerable<AppliedRealmProperty> TakeRandomProperties(ushort amount, RulesetTemplate template)
        {
            LogTrace(() => $"Taking {amount} properties at random");
            if (PropertiesString.TryGetValue(RealmPropertyString.Description, out var desc))
                amount++;

            var total = template.PropertiesForRandomization.Count;
            if (amount <= total / 2)
            {
                var set = new HashSet<AppliedRealmProperty>();
                if (desc != null)
                    set.Add(desc);
                while (set.Count < amount)
                {
                    var selectedProp = GetRandomProperty(template, Context);
                    LogTrace(Context, () => $"Cherry-picking {selectedProp.Options.Name} from randomization list");
                    set.Add(selectedProp);
                }
                return set;
            }
            else
            {
                // As an extreme example of why this is needed, imagine if there were 1000 properties, and 999 of them are taken at random.
                // The last few rolls will have as little as 0.1% chance of finding an unused property.
                LogTrace(() => $"{amount} is at least 50% of the randomization properties count of {total}, using subtraction algorithm");
                var all = new List<AppliedRealmProperty>(template.PropertiesForRandomization);
                if (amount >= all.Count)
                {
                    LogTrace(() => $"{amount} exceeds the randomization properties count of {all.Count}, skipping set reduction.");
                    return all;
                }
                var set = new HashSet<AppliedRealmProperty>(all);
                while (set.Count > amount)
                {
                    var next = GetRandomProperty(template, Context);
                    if (next is AppliedRealmProperty<string> s && s.PropertyKey == (ushort)RealmPropertyString.Description)
                        continue;
                    LogTrace(Context, () => $"Subtracting {next.Options.Name} from randomization selection");
                    set.Remove(next);
                }
                return set;
            }
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

        public static RulesetCompilationContext MakeDefaultContext() =>
            RulesetCompilationContext.CreateContext(
                enableSeedTracking: PropertyManager.GetBool("acr_enable_ruleset_seeds", false).Item,
                gitHash: AssemblyInfo.GitHash
            );
    }

    //Properties should not be changed after initial composition
    public class RulesetTemplate(ushort rulesetID, RulesetCompilationContext ctx) : Ruleset(rulesetID, ctx)
    {
        public Realm Realm { get; protected set; }
        public new RulesetTemplate ParentRuleset
        {
            get { return (RulesetTemplate)base.ParentRuleset; }
            set { base.ParentRuleset = value; }
        }

        internal IReadOnlyList<AppliedRealmProperty> PropertiesForRandomization { get; set; }
        internal IReadOnlyList<RealmLinkJob> Jobs { get; private set; }

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
                var prop = !trace ? kv.Value : new AppliedRealmProperty<bool>(Context, kv.Value, null);
                PropertiesBool.Add(kv.Key, prop);
                AllProperties.Add(prop.Options.Name, prop);
            }
            foreach (var kv in entity.PropertiesFloat)
            {
                var prop = !trace ? kv.Value : new AppliedRealmProperty<double>(Context, kv.Value, null);
                PropertiesFloat.Add(kv.Key, prop);
                AllProperties.Add(prop.Options.Name, prop);
            }
            foreach (var kv in entity.PropertiesInt)
            {
                var prop = !trace ? kv.Value : new AppliedRealmProperty<int>(Context, kv.Value, null);
                PropertiesInt.Add(kv.Key, prop);
                AllProperties.Add(prop.Options.Name, prop);
            }
            foreach (var kv in entity.PropertiesInt64)
            {
                var prop = !trace ? kv.Value : new AppliedRealmProperty<long>(Context, kv.Value, null);
                PropertiesInt64.Add(kv.Key, prop);
                AllProperties.Add(prop.Options.Name, prop);
            }
            foreach (var kv in entity.PropertiesString)
            {
                var prop = !trace ? kv.Value : new AppliedRealmProperty<string>(Context, kv.Value, null);
                PropertiesString.Add(kv.Key, prop);
                AllProperties.Add(prop.Options.Name, prop);
            }

            Debug.Assert(entity.AllProperties.Count == AllProperties.Count);

            Jobs = entity.Jobs;
            if (trace)
            {
                var rulesetNames = RealmManager.RealmsAndRulesets.ToDictionary(x => x.Realm.Id, x => x.Realm.Name);
                foreach(var job in Jobs)
                    LogTrace(job.GetLogTraceMessages(rulesetNames));
                foreach(var kv in AllProperties)
                    LogTrace(() => $"Loaded uncomposed property {kv.Key} as {kv.Value}");
            }
           
            MakeRandomizationList();
        }

        public static RulesetTemplate MakeRuleset(RulesetTemplate baseset, Realm subset, RulesetCompilationContext ctx)
        {
            if (baseset.Realm.Type == ACE.Entity.Enum.RealmType.Ruleset && subset.Type == ACE.Entity.Enum.RealmType.Realm)
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
                var list = new List<AppliedRealmProperty>();
                list.AddRange(PropertiesBool.Values);
                list.AddRange(PropertiesInt.Values);
                list.AddRange(PropertiesInt64.Values);
                list.AddRange(PropertiesFloat.Values);
                list.AddRange(PropertiesString.Where(x => x.Key != RealmPropertyString.Description).Select(x => x.Value));
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
    }

    //Properties may be changed freely
    public partial class AppliedRuleset(RulesetTemplate template, RulesetCompilationContext ctx) : Ruleset(template.RulesetID, ctx)
    {
        public Landblock Landblock { get; set; }
        public RulesetTemplate Template { get; } = template;
        public Realm Realm => Template.Realm;
        public new AppliedRuleset ParentRuleset
        {
            get { return (AppliedRuleset)base.ParentRuleset; }
            set { base.ParentRuleset = value; }
        }
        internal string DebugOutputString()
        {
            var sb = new StringBuilder();

            foreach (var item in PropertiesBool)
                sb.AppendLine($"{Enum.GetName(typeof(RealmPropertyBool), item.Key)}: {item.Value}");
            foreach (var item in PropertiesFloat)
                sb.AppendLine($"{Enum.GetName(typeof(RealmPropertyFloat), item.Key)}: {item.Value}");
            foreach (var item in PropertiesInt)
                sb.AppendLine($"{Enum.GetName(typeof(RealmPropertyInt), item.Key)}: {item.Value}");
            foreach (var item in PropertiesInt64)
                sb.AppendLine($"{Enum.GetName(typeof(RealmPropertyInt64), item.Key)}: {item.Value}");
            foreach (var item in PropertiesString.Where(x => x.Key != RealmPropertyString.Description))
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
            return result;
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
                    var template = RealmManager.GetRealm(link.RulesetIDToApply, includeRulesets: true).RulesetTemplate;
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

            ApplyRulesetDict(parent.PropertiesBool, PropertiesBool, invertRules, Context);
            ApplyRulesetDict(parent.PropertiesFloat, PropertiesFloat, invertRules, Context);
            ApplyRulesetDict(parent.PropertiesInt, PropertiesInt, invertRules, Context);
            ApplyRulesetDict(parent.PropertiesInt64, PropertiesInt64, invertRules, Context);
            ApplyRulesetDict(parent.PropertiesString, PropertiesString, invertRules, Context);
            LogTrace(() => $"FinishCompose (parent: {parentTemplate.Realm.Name}, invertRules: {invertRules})");
        }

        private void RerollAllRules()
        {
            LogTrace(() => $"RerollAllRules Begin");
            foreach (var v in PropertiesFloat.Values)
                v.RollValue();
            foreach (var v in PropertiesInt.Values)
                v.RollValue();
            foreach (var v in PropertiesInt64.Values)
                v.RollValue();
            foreach (var v in PropertiesBool.Values)
                v.RollValue();
            foreach (var v in PropertiesString.Values)
                v.RollValue();
            LogTrace(() => $"RerollAllRules End");
        }

        public bool GetProperty(RealmPropertyBool property)
        {
            var att = RealmConverter.PropertyDefinitionsBool[property];
            if (PropertiesBool.TryGetValue(property, out var value))
                return value.Value;
            if (att.DefaultFromServerProperty != null)
                return PropertyManager.GetBool(att.DefaultFromServerProperty, att.DefaultValue).Item;
            return att.DefaultValue;
        }

        public double GetProperty(RealmPropertyFloat property)
        {
            var att = RealmConverter.PropertyDefinitionsFloat[property];
            if (PropertiesFloat.TryGetValue(property, out var result))
            {
                var val = result.Value;
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

        public int GetProperty(RealmPropertyInt property)
        {
            var att = RealmConverter.PropertyDefinitionsInt[property];
            if (PropertiesInt.TryGetValue(property, out var result))
            {
                var val = result.Value;
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

        public long GetProperty(RealmPropertyInt64 property)
        {
            var att = RealmConverter.PropertyDefinitionsInt64[property];
            if (PropertiesInt64.TryGetValue(property, out var result))
            {
                var val = result.Value;
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

        public string GetProperty(RealmPropertyString property)
        {
            var att = RealmConverter.PropertyDefinitionsString[property];
            if (PropertiesString.TryGetValue(property, out var result))
                return result.Value;
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
                _ => GetFullInstanceID(player.GetDefaultShortInstanceID())
            };
        }

        public uint GetFullInstanceID(ushort shortInstanceID)
        {
            return ACE.Entity.Position.InstanceIDFromVars(Template.Realm.Id, shortInstanceID, Template.Realm.Type == ACE.Entity.Enum.RealmType.Ruleset);
        }

        public uint GetDefaultInstanceID(LocalPosition position) => GetFullInstanceID(0);
        
        public override string ToString()
        {
            return $"Applied Ruleset {Realm.Name}";
        }
    }
}
