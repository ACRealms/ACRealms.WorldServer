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

    public abstract class Ruleset
    {
        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected static Random random = new Random();
        public Ruleset ParentRuleset { get; protected set; }

        protected IDictionary<RealmPropertyBool, AppliedRealmProperty<bool>> PropertiesBool { get; set; }
        protected IDictionary<RealmPropertyFloat, AppliedRealmProperty<double>> PropertiesFloat { get; set; }
        protected IDictionary<RealmPropertyInt, AppliedRealmProperty<int>> PropertiesInt { get; set; }
        protected IDictionary<RealmPropertyInt64, AppliedRealmProperty<long>> PropertiesInt64 { get; set; }
        protected IDictionary<RealmPropertyString, AppliedRealmProperty<string>> PropertiesString { get; set; }

        protected static void ApplyRulesetDict<K, V>(IDictionary<K, AppliedRealmProperty<V>> parent, IDictionary<K, AppliedRealmProperty<V>> sub, bool invertRelationships = false)
        {
            //If invertRelationships is true, we are prioritizing the sub dictionary for the purposes of lock, and prioritizing the parent for the purposes of replace.
            //Add and multiply operations can be applied independent of the ordering so they are not affected. 

            //Add all parent items to sub. But if it is already in sub, then calculate using the special composition rules
            foreach (var pair in parent)
            {
                var parentprop = pair.Value;
                if (!sub.ContainsKey(pair.Key))
                {
                    //Just add the parent's reference, as we already previously imprinted the parent from its template for this purpose
                    sub[pair.Key] = parentprop;
                    continue;
                }

                if (!invertRelationships && parentprop.Options.Locked)
                {
                    //Use parent's property as it is locked
                    sub[pair.Key] = parentprop;
                    continue;
                }
                else if (invertRelationships && sub[pair.Key].Options.Locked)
                    continue;

                //Replace
                if (!invertRelationships && sub[pair.Key].Options.CompositionType == RealmPropertyCompositionType.replace)
                {
                    //No need to do anything here since we are replacing the parent
                    continue;
                }
                else if (invertRelationships && parentprop.Options.CompositionType == RealmPropertyCompositionType.replace)
                {
                    sub[pair.Key] = parentprop;
                    continue;
                }

                //Apply composition rules
                if (!invertRelationships)
                    sub[pair.Key] = new AppliedRealmProperty<V>(sub[pair.Key], parentprop);
                else
                    sub[pair.Key] = new AppliedRealmProperty<V>(parentprop, sub[pair.Key]); //ensure parent chain is kept
            }
        }
        private static AppliedRealmProperty GetRandomProperty(RulesetTemplate template)
        {
            if (template.PropertiesForRandomization?.Count == 0)
                return null;
            return template.PropertiesForRandomization[random.Next(template.PropertiesForRandomization.Count - 1)];
        }

        protected void CopyDicts(RulesetTemplate copyFrom)
        {
            if (copyFrom.Realm.PropertyCountRandomized.HasValue)
            {
                var count = copyFrom.Realm.PropertyCountRandomized.Value;
                var props = TakeRandomProperties(count, copyFrom);
                foreach (var prop in props)
                {
                    if (prop is AppliedRealmProperty<bool> a)
                        PropertiesBool.Add((RealmPropertyBool)a.PropertyKey, new AppliedRealmProperty<bool>(a));
                    else if (prop is AppliedRealmProperty<int> b)
                        PropertiesInt.Add((RealmPropertyInt)b.PropertyKey, new AppliedRealmProperty<int>(b));
                    else if (prop is AppliedRealmProperty<double> c)
                        PropertiesFloat.Add((RealmPropertyFloat)c.PropertyKey, new AppliedRealmProperty<double>(c));
                    else if (prop is AppliedRealmProperty<long> d)
                        PropertiesInt64.Add((RealmPropertyInt64)d.PropertyKey, new AppliedRealmProperty<long>(d));
                    else if (prop is AppliedRealmProperty<string> e)
                        PropertiesString.Add((RealmPropertyString)e.PropertyKey, new AppliedRealmProperty<string>(e));
                }
            }
            else
            {
                foreach (var item in copyFrom.PropertiesBool)
                    PropertiesBool.Add(item.Key, new AppliedRealmProperty<bool>(item.Value));
                foreach (var item in copyFrom.PropertiesFloat)
                    PropertiesFloat.Add(item.Key, new AppliedRealmProperty<double>(item.Value));
                foreach (var item in copyFrom.PropertiesInt)
                    PropertiesInt.Add(item.Key, new AppliedRealmProperty<int>(item.Value));
                foreach (var item in copyFrom.PropertiesInt64)
                    PropertiesInt64.Add(item.Key, new AppliedRealmProperty<long>(item.Value));
                foreach (var item in copyFrom.PropertiesString)
                    PropertiesString.Add(item.Key, new AppliedRealmProperty<string>(item.Value));
            }
        }

        private IEnumerable<AppliedRealmProperty> TakeRandomProperties(ushort amount, RulesetTemplate template)
        {
            if (PropertiesString.TryGetValue(RealmPropertyString.Description, out var desc))
                amount++;

            var total = PropertiesBool.Count +
                PropertiesInt.Count +
                PropertiesInt64.Count +
                PropertiesFloat.Count +
                PropertiesString.Count;
            if (amount <= total / 2)
            {
                var set = new HashSet<AppliedRealmProperty>();
                if (desc != null)
                    set.Add(desc);
                while(set.Count < amount)
                    set.Add(GetRandomProperty(template));
                return set;
            }
            else
            {
                var all = new List<AppliedRealmProperty>(template.PropertiesForRandomization);
                if (amount >= all.Count)
                    return all;
                var set = new HashSet<AppliedRealmProperty>(all);
                while (set.Count > amount)
                {
                    var next = GetRandomProperty(template);
                    if (next is AppliedRealmProperty<string> s && s.PropertyKey == (ushort)RealmPropertyString.Description)
                        continue;
                    set.Remove(next);
                }
                return set;
            }
        }

        protected void InitializePropertyDictionaries()
        {
            PropertiesBool = new Dictionary<RealmPropertyBool, AppliedRealmProperty<bool>>();
            PropertiesFloat = new Dictionary<RealmPropertyFloat, AppliedRealmProperty<double>>();
            PropertiesInt = new Dictionary<RealmPropertyInt, AppliedRealmProperty<int>>();
            PropertiesInt64 = new Dictionary<RealmPropertyInt64, AppliedRealmProperty<long>>();
            PropertiesString = new Dictionary<RealmPropertyString, AppliedRealmProperty<string>>();
        }
    }

    //Properties should not be changed after initial composition
    public class RulesetTemplate : Ruleset
    {
        public Realm Realm { get; protected set; }
        public new RulesetTemplate ParentRuleset
        {
            get { return (RulesetTemplate)base.ParentRuleset; }
            set { base.ParentRuleset = value; }
        }

        internal IReadOnlyList<AppliedRealmProperty> PropertiesForRandomization { get; set; }
        internal IReadOnlyList<RealmLinkJob> Jobs { get; private set; }
        public static RulesetTemplate MakeTopLevelRuleset(Realm entity)
        {
            var ruleset = new RulesetTemplate();
            ruleset.Realm = entity;

            ruleset.PropertiesBool = new Dictionary<RealmPropertyBool, AppliedRealmProperty<bool>>(entity.PropertiesBool);
            ruleset.PropertiesFloat = new Dictionary<RealmPropertyFloat, AppliedRealmProperty<double>>(entity.PropertiesFloat);
            ruleset.PropertiesInt = new Dictionary<RealmPropertyInt, AppliedRealmProperty<int>>(entity.PropertiesInt);
            ruleset.PropertiesInt64 = new Dictionary<RealmPropertyInt64, AppliedRealmProperty<long>>(entity.PropertiesInt64);
            ruleset.PropertiesString = new Dictionary<RealmPropertyString, AppliedRealmProperty<string>>(entity.PropertiesString);
            ruleset.Jobs = entity.Jobs;

            ruleset.MakeRandomizationList();
            return ruleset;
        }

        public static RulesetTemplate MakeRuleset(RulesetTemplate baseset, Realm subset)
        {
            if (baseset.Realm.Type == ACE.Entity.Enum.RealmType.Ruleset && subset.Type == ACE.Entity.Enum.RealmType.Realm)
                throw new Exception("Realms may not inherit from rulesets at this time.");
            var ruleset = new RulesetTemplate();
            ruleset.ParentRuleset = baseset;
            ruleset.Realm = subset;

            ruleset.PropertiesBool = new Dictionary<RealmPropertyBool, AppliedRealmProperty<bool>>(subset.PropertiesBool);
            ruleset.PropertiesFloat = new Dictionary<RealmPropertyFloat, AppliedRealmProperty<double>>(subset.PropertiesFloat);
            ruleset.PropertiesInt = new Dictionary<RealmPropertyInt, AppliedRealmProperty<int>>(subset.PropertiesInt);
            ruleset.PropertiesInt64 = new Dictionary<RealmPropertyInt64, AppliedRealmProperty<long>>(subset.PropertiesInt64);
            ruleset.PropertiesString = new Dictionary<RealmPropertyString, AppliedRealmProperty<string>>(subset.PropertiesString);
            ruleset.Jobs = subset.Jobs;

            ruleset.MakeRandomizationList();
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
            }
        }

        public override string ToString()
        {
            return $"Template {Realm.Name}";
        }
    }

    //Properties may be changed freely
    public partial class AppliedRuleset : Ruleset
    {
        public Landblock Landblock { get; set; }

        public RulesetTemplate Template { get; }
        public Realm Realm => Template.Realm;
        public new AppliedRuleset ParentRuleset
        {
            get { return (AppliedRuleset)base.ParentRuleset; }
            set { base.ParentRuleset = value; }
        }

        private AppliedRuleset() { }

        //Deep copy
        private AppliedRuleset(RulesetTemplate template)
        {
            Template = template;
            InitializePropertyDictionaries();
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
        internal static AppliedRuleset MakeRerolledRuleset(RulesetTemplate template)
        {
            var result = new AppliedRuleset(template);
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
            foreach(var link in job.Links)
            {
                if (random.NextDouble() < link.Probability)
                {
                    var template = RealmManager.GetRealm(link.RulesetIDToApply).RulesetTemplate;
                    ComposeFrom(template, true);
                    return;
                }
            }
        }

        private void ComposeFrom(RulesetTemplate parentTemplate, bool invertRules = false)
        {
            if (parentTemplate == null)
                return;
            var parent = MakeRerolledRuleset(parentTemplate);

            ApplyRulesetDict(parent.PropertiesBool, PropertiesBool, invertRules);
            ApplyRulesetDict(parent.PropertiesFloat, PropertiesFloat, invertRules);
            ApplyRulesetDict(parent.PropertiesInt, PropertiesInt, invertRules);
            ApplyRulesetDict(parent.PropertiesInt64, PropertiesInt64, invertRules);
            ApplyRulesetDict(parent.PropertiesString, PropertiesString, invertRules);
        }

        private void RerollAllRules()
        {
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

        public uint GetDefaultInstanceID()
        {
            return ACE.Entity.Position.InstanceIDFromVars(this.Template.Realm.Id, 0, this.Template.Realm.Type == ACE.Entity.Enum.RealmType.Ruleset);
        }

        public override string ToString()
        {
            return $"Applied Rulest {Realm.Name}";
        }
    }
}
