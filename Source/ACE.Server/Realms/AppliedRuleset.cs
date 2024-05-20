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

namespace ACE.Server.Realms
{
    public static class RandomHelpers
    {
        static Random rnd = new Random();
        public static Dictionary<int, T> ToIndexedDictionary<T>(this IEnumerable<T> lst)
        {
            return lst.ToIndexedDictionary(t => t);
        }

        public static Dictionary<int, T> ToIndexedDictionary<S, T>(this IEnumerable<S> lst, Func<S, T> valueSelector)
        {
            int index = -1;
            return lst.ToDictionary(t => ++index, valueSelector);
        }

        public static int GetRandomIndex<T>(this ICollection<T> source)
        {
            return rnd.Next(source.Count);
        }

        public static T GetRandom<T>(this IList<T> source)
        {
            return source[source.GetRandomIndex()];
        }
    }

    public abstract class Ruleset(ushort rulesetID, bool trace = false, List<string> traceLog = null)
    {
        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected static Random random = new Random();
        public Ruleset ParentRuleset { get; protected set; }
        public ushort RulesetID { get; } = rulesetID;
        protected bool Trace { get; init; } = traceLog != null || trace;
        public virtual List<string> TraceLog { get; } = traceLog ?? (trace ? new List<string>() : null);

        protected IDictionary<RealmPropertyBool, AppliedRealmProperty<bool>> PropertiesBool { get; } = new Dictionary<RealmPropertyBool, AppliedRealmProperty<bool>>();
        protected IDictionary<RealmPropertyFloat, AppliedRealmProperty<double>> PropertiesFloat { get; } = new Dictionary<RealmPropertyFloat, AppliedRealmProperty<double>>();
        protected IDictionary<RealmPropertyInt, AppliedRealmProperty<int>> PropertiesInt { get; } = new Dictionary<RealmPropertyInt, AppliedRealmProperty<int>>();
        protected IDictionary<RealmPropertyInt64, AppliedRealmProperty<long>> PropertiesInt64 { get; } = new Dictionary<RealmPropertyInt64, AppliedRealmProperty<long>>();
        protected IDictionary<RealmPropertyString, AppliedRealmProperty<string>> PropertiesString { get; } = new Dictionary<RealmPropertyString, AppliedRealmProperty<string>>();
        protected IDictionary<string, AppliedRealmProperty> AllProperties { get; } = new Dictionary<string, AppliedRealmProperty>();

        protected static void ApplyRulesetDict<K, V>(
            IDictionary<K, AppliedRealmProperty<V>> parent,
            IDictionary<K, AppliedRealmProperty<V>> sub,
            bool invertRelationships,
            Realm parentRealm,
            List<string> traceLog = null)
            where V : IEquatable<V>
            where K : Enum
        {
            //If invertRelationships is true, we are prioritizing the sub dictionary for the purposes of lock, and prioritizing the parent for the purposes of replace.
            //Add and multiply operations can be applied independent of the ordering so they are not affected. 

            //Add all parent items to sub. But if it is already in sub, then calculate using the special composition rules
            foreach (var pair in parent)
            {
                LogTrace(traceLog, () => "Begin single property compilation", pair.Value);
                var parentprop = pair.Value;
                if (!sub.ContainsKey(pair.Key))
                {
                    LogTrace(traceLog, () => $"No property def, using parent '{parentprop.Options.RulesetName}'", pair.Value);
                    //Just add the parent's reference, as we already previously imprinted the parent from its template for this purpose
                    sub[pair.Key] = parentprop;
                    continue;
                }

                if (!invertRelationships && parentprop.Options.Locked)
                {
                    LogTrace(traceLog, () => $"No property def, using parent '{parentprop.Options.RulesetName}'", pair.Value);

                    //Use parent's property as it is locked
                    sub[pair.Key] = parentprop;
                    continue;
                }
                else if (invertRelationships && sub[pair.Key].Options.Locked)
                {
                    LogTrace(traceLog, () => $"Locked: Discarding parent '{parentprop.Options.RulesetName}'", pair.Value);
                    continue;
                }

                //Replace
                if (!invertRelationships && sub[pair.Key].Options.CompositionType == RealmPropertyCompositionType.replace)
                {
                    LogTrace(traceLog, () => $"Replace: Discarding parent '{parentprop.Options.RulesetName}'", pair.Value);
                    //No need to do anything here since we are replacing the parent
                    continue;
                }
                else if (invertRelationships && parentprop.Options.CompositionType == RealmPropertyCompositionType.replace)
                {
                    LogTrace(traceLog, () => $"Replace: using parent from invertRelationships: '{parentprop.Options.RulesetName}'", pair.Value);
                    sub[pair.Key] = parentprop;
                    continue;
                }

                //Apply composition rules
                if (!invertRelationships)
                {
                    sub[pair.Key] = new AppliedRealmProperty<V>(sub[pair.Key], parentprop, traceLog);
                }
                else
                {
                    LogTrace(traceLog, () => $"InvertedRel: using '{parentprop.Options.RulesetName}' with parent '{sub[pair.Key].Options.RulesetName}'", pair.Value);
                    sub[pair.Key] = new AppliedRealmProperty<V>(parentprop, sub[pair.Key], traceLog); //ensure parent chain is kept
                }

                LogTrace(traceLog, () => "End single property compilation", pair.Value);
            }
        }

        private static void LogTrace(List<string> traceLog, Func<string> message)
        {
            if (traceLog == null)
                return;
            traceLog.Add($"   [C] {message()}");
        }
            
        private static void LogTrace(List<string> traceLog, Func<string> message, AppliedRealmProperty property)
        {
            if (traceLog == null)
                return;
            traceLog.Add($"   [C][{property.Options.Name}] (Def:{property.Options.RulesetName}) {message()}");
        }


        private static AppliedRealmProperty GetRandomProperty(RulesetTemplate template)
        {
            if (template.PropertiesForRandomization?.Count == 0)
                return null;
            var selectedProp = template.PropertiesForRandomization[random.Next(template.PropertiesForRandomization.Count - 1)];

            return template.PropertiesForRandomization[random.Next(template.PropertiesForRandomization.Count - 1)];
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
                        DictAdd(PropertiesBool, (RealmPropertyBool)a.PropertyKey, new AppliedRealmProperty<bool>(a, null, TraceLog));
                    else if (prop is AppliedRealmProperty<int> b)
                        DictAdd(PropertiesInt, (RealmPropertyInt)b.PropertyKey, new AppliedRealmProperty<int>(b, null, TraceLog));
                    else if (prop is AppliedRealmProperty<double> c)
                        DictAdd(PropertiesFloat, (RealmPropertyFloat)c.PropertyKey, new AppliedRealmProperty<double>(c, null, TraceLog));
                    else if (prop is AppliedRealmProperty<long> d)
                        DictAdd(PropertiesInt64, (RealmPropertyInt64)d.PropertyKey, new AppliedRealmProperty<long>(d, null, TraceLog));
                    else if (prop is AppliedRealmProperty<string> e)
                        DictAdd(PropertiesString, (RealmPropertyString)e.PropertyKey, new AppliedRealmProperty<string>(e, null, TraceLog));
                }
            }
            else
            {
                foreach (var item in copyFrom.PropertiesBool)
                    DictAdd(PropertiesBool, item.Key, new AppliedRealmProperty<bool>(item.Value, null, TraceLog));
                foreach (var item in copyFrom.PropertiesFloat)
                    DictAdd(PropertiesFloat, item.Key, new AppliedRealmProperty<double>(item.Value, null, TraceLog));
                foreach (var item in copyFrom.PropertiesInt)
                    DictAdd(PropertiesInt, item.Key, new AppliedRealmProperty<int>(item.Value, null, TraceLog));
                foreach (var item in copyFrom.PropertiesInt64)
                    DictAdd(PropertiesInt64, item.Key, new AppliedRealmProperty<long>(item.Value, null, TraceLog));
                foreach (var item in copyFrom.PropertiesString)
                    DictAdd(PropertiesString, item.Key, new AppliedRealmProperty<string>(item.Value, null, TraceLog));
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
                    var selectedProp = GetRandomProperty(template);
                    LogTrace(TraceLog, () => $"Cherry-picking {selectedProp.Options.Name} from randomization list");
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
                    var next = GetRandomProperty(template);
                    if (next is AppliedRealmProperty<string> s && s.PropertyKey == (ushort)RealmPropertyString.Description)
                        continue;
                    LogTrace(TraceLog, () => $"Subtracting {next.Options.Name} from randomization selection");
                    set.Remove(next);
                }
                return set;
            }
        }

        protected virtual void LogTrace(IEnumerable<string> messages)
        {
            if (!Trace)
                return;
            foreach(var message in messages)
                TraceLog.Add($"{(this is RulesetTemplate ? "[T]" : "[R]")}[{RealmManager.GetRealm(RulesetID).Realm.Name}] {message}");
        }

        protected virtual void LogTrace(Func<string> message)
        {
            if (!Trace)
                return;
            TraceLog.Add($"{(this is RulesetTemplate ? "[T]" : "[R]")}[{RealmManager.GetRealm(RulesetID).Realm.Name}] {message()}");
        }
    }

    //Properties should not be changed after initial composition
    public class RulesetTemplate(ushort rulesetID, bool trace = false, List<string> traceLog = null) : Ruleset(rulesetID, trace, traceLog)
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
        public RulesetTemplate(RulesetTemplate cloneFrom, ushort rulesetID, bool trace, List<string> traceLog)
            : this(rulesetID, trace, traceLog)
        {
            if (cloneFrom.ParentRuleset != null)
                ParentRuleset = new RulesetTemplate(cloneFrom.ParentRuleset, cloneFrom.ParentRuleset.RulesetID, trace, traceLog);
            LogTrace(() => $"New template (parent: {ParentRuleset.Realm.Name})");
            var realm = RealmManager.GetRealm(rulesetID).Realm;
            LoadPropertiesFrom(realm);
            LogTrace(() => $"Completed template initialization");
        }

        public static RulesetTemplate MakeTopLevelRuleset(Realm entity, bool trace)
        {
            
            var ruleset = new RulesetTemplate(entity.Id, trace);
            ruleset.LogTrace(() => $"New template (ROOT)");
            ruleset.LoadPropertiesFrom(entity);
            ruleset.LogTrace(() => $"Completed template initialization");
            return ruleset;
        }

        private void LoadPropertiesFrom(Realm entity)
        {
            var trace = Trace;
            // These really should be "TemplateProperties" and not "AppliedProperties"
            Realm = entity;
            foreach (var kv in entity.PropertiesBool)
            {
                PropertiesBool.Add(kv.Key, !trace ? kv.Value : new AppliedRealmProperty<bool>(kv.Value, null, TraceLog));
                // This is arguably slower and consumes more memory so doing the check for trace mode here
                if (trace)
                    AllProperties.Add(kv.Key.ToString(), PropertiesBool[kv.Key]);
            }
            foreach (var kv in entity.PropertiesFloat)
            {
                PropertiesFloat.Add(kv.Key, !trace ? kv.Value : new AppliedRealmProperty<double>(kv.Value, null, TraceLog));
                if (trace)
                    AllProperties.Add(kv.Key.ToString(), PropertiesFloat[kv.Key]);
            }
            foreach (var kv in entity.PropertiesInt)
            {
                PropertiesInt.Add(kv.Key, !trace ? kv.Value : new AppliedRealmProperty<int>(kv.Value, null, TraceLog));
                if (trace)
                    AllProperties.Add(kv.Key.ToString(), PropertiesInt[kv.Key]);
            }
            foreach (var kv in entity.PropertiesInt64)
            {
                PropertiesInt64.Add(kv.Key, !trace ? kv.Value : new AppliedRealmProperty<long>(kv.Value, null, TraceLog));
                if (trace)
                    AllProperties.Add(kv.Key.ToString(), PropertiesInt64[kv.Key]);
            }
            foreach (var kv in entity.PropertiesString)
            {
                PropertiesString.Add(kv.Key, !trace ? kv.Value : new AppliedRealmProperty<string>(kv.Value, null, TraceLog));
                if (trace)
                    AllProperties.Add(kv.Key.ToString(), PropertiesString[kv.Key]);
            }
            if (!trace)
            {
                foreach (var kv in entity.AllProperties)
                    AllProperties.Add(kv.Key, kv.Value);
            }

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

        public static RulesetTemplate MakeRuleset(RulesetTemplate baseset, Realm subset, bool trace)
        {
            if (baseset.Realm.Type == ACE.Entity.Enum.RealmType.Ruleset && subset.Type == ACE.Entity.Enum.RealmType.Realm)
                throw new Exception("Realms may not inherit from rulesets.");
            var ruleset = new RulesetTemplate(subset.Id, baseset.Trace || trace, baseset.TraceLog);
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

        internal RulesetTemplate RebuildTemplateWithTrace()
        {
            return new RulesetTemplate(this, RulesetID, true, TraceLog);
        }
    }

    //Properties may be changed freely
    public partial class AppliedRuleset(RulesetTemplate template, bool trace = false, List<string> traceLog = null) : Ruleset(template.RulesetID, trace, traceLog)
    {
        public Landblock Landblock { get; set; }
        public override List<string> TraceLog => Trace ? (Template.TraceLog ?? base.TraceLog) : null;
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
        internal static AppliedRuleset MakeRerolledRuleset(RulesetTemplate template, bool trace = false, List<string> traceLog = null)
        {
            var result = new AppliedRuleset(template, trace, traceLog);
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
            if (Trace)
            {
                LogTrace(() => $"ApplyJob start ({job.Type}, {job.Links.Count})");
                foreach (var link in job.Links)
                {
                    LogTrace(() => $"Composition chance of {link.RulesetIDToApply}: {link.Probability.ToString("P2")}");
                }
            }
            foreach (var link in job.Links)
            {
                var roll = random.NextDouble();
                if (roll < link.Probability)
                {
                    LogTrace(() => $"Rolled {roll}, is < {link.Probability}, applying ruleset, skipping further rolls for this job");
                    var template = RealmManager.GetRealm(link.RulesetIDToApply).RulesetTemplate;
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
            var parent = MakeRerolledRuleset(parentTemplate, traceLog: TraceLog);
            LogTrace(() => $"ContinueCompose (parent: {parentTemplate.Realm.Name}, invertRules: {invertRules})");

            ApplyRulesetDict(parent.PropertiesBool, PropertiesBool, invertRules, parentTemplate.Realm, TraceLog);
            ApplyRulesetDict(parent.PropertiesFloat, PropertiesFloat, invertRules, parentTemplate.Realm, TraceLog);
            ApplyRulesetDict(parent.PropertiesInt, PropertiesInt, invertRules, parentTemplate.Realm, TraceLog);
            ApplyRulesetDict(parent.PropertiesInt64, PropertiesInt64, invertRules, parentTemplate.Realm, TraceLog);
            ApplyRulesetDict(parent.PropertiesString, PropertiesString, invertRules, parentTemplate.Realm, TraceLog);
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

        public uint GetFullInstanceID(ushort shortInstanceID)
        {
            return ACE.Entity.Position.InstanceIDFromVars(Template.Realm.Id, shortInstanceID, Template.Realm.Type == ACE.Entity.Enum.RealmType.Ruleset);
        }

        public uint GetDefaultInstanceID() => GetFullInstanceID(0);
        
        public override string ToString()
        {
            return $"Applied Ruleset {Realm.Name}";
        }
    }
}
