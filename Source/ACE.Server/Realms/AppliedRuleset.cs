using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.Managers;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ACE.Server.Entity;

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

        protected static void ApplyRulesetDict<K, V>(IDictionary<K, AppliedRealmProperty<V>> parent, IDictionary<K, AppliedRealmProperty<V>> sub)
        {
            //Add all parent items to sub. But if it is already in sub, then calculate using the special composition rules
            foreach(var prop in parent)
            {
                if (prop.Value.Options.Locked)
                {
                    //Use parent's property as it is locked
                    sub[prop.Key] = prop.Value;
                    continue;
                }
                if (!sub.ContainsKey(prop.Key))
                {
                    //Just add the parent's reference, as we already previously imprinted the parent from its template for this purpose
                    sub[prop.Key] = parent[prop.Key];
                    continue;
                }

                //Apply composition rules
                //if (prop.Value.Options.CompositionType)
                //return new AppliedRealmProperty<V>(sub[prop.Key], prop.Value);
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
        public HashSet<ushort> InheritedRealmIDs { get; protected set; } = new HashSet<ushort>();
        public Realm Realm { get; protected set; }
        public new RulesetTemplate ParentRuleset
        {
            get { return (RulesetTemplate)base.ParentRuleset; }
            set { base.ParentRuleset = value; }
        }

        internal IReadOnlyList<AppliedRealmProperty> PropertiesForRandomization { get; set; }

        public static RulesetTemplate MakeTopLevelRuleset(Realm entity)
        {
            var ruleset = new RulesetTemplate();
            ruleset.Realm = entity;
            ruleset.InheritedRealmIDs.Add(entity.Id);

            ruleset.PropertiesBool = new Dictionary<RealmPropertyBool, AppliedRealmProperty<bool>>(entity.PropertiesBool);
            ruleset.PropertiesFloat = new Dictionary<RealmPropertyFloat, AppliedRealmProperty<double>>(entity.PropertiesFloat);
            ruleset.PropertiesInt = new Dictionary<RealmPropertyInt, AppliedRealmProperty<int>>(entity.PropertiesInt);
            ruleset.PropertiesInt64 = new Dictionary<RealmPropertyInt64, AppliedRealmProperty<long>>(entity.PropertiesInt64);
            ruleset.PropertiesString = new Dictionary<RealmPropertyString, AppliedRealmProperty<string>>(entity.PropertiesString);

            ruleset.MakeRandomizationList();
            return ruleset;
        }

        public static RulesetTemplate MakeRuleset(RulesetTemplate baseset, Realm subset)
        {
            if (baseset.InheritedRealmIDs.Contains(subset.Id))
                throw new Exception("Circular inheritance chain detected for realm id " + subset.Id.ToString());
            if (baseset.Realm.Type == ACE.Entity.Enum.RealmType.Ruleset && subset.Type == ACE.Entity.Enum.RealmType.Realm)
                throw new Exception("Realms may not inherit from rulesets at this time.");
            var ruleset = new RulesetTemplate();
            ruleset.ParentRuleset = baseset;
            ruleset.Realm = subset;
            ruleset.InheritedRealmIDs.Add(subset.Id);

            ruleset.PropertiesBool = new Dictionary<RealmPropertyBool, AppliedRealmProperty<bool>>(subset.PropertiesBool);
            ruleset.PropertiesFloat = new Dictionary<RealmPropertyFloat, AppliedRealmProperty<double>>(subset.PropertiesFloat);
            ruleset.PropertiesInt = new Dictionary<RealmPropertyInt, AppliedRealmProperty<int>>(subset.PropertiesInt);
            ruleset.PropertiesInt64 = new Dictionary<RealmPropertyInt64, AppliedRealmProperty<long>>(subset.PropertiesInt64);
            ruleset.PropertiesString = new Dictionary<RealmPropertyString, AppliedRealmProperty<string>>(subset.PropertiesString);

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
    }

    //Properties may be changed freely
    public class AppliedRuleset : Ruleset
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

            sb.AppendLine("AC Realmulator Zone Info:");

            foreach (var item in PropertiesBool)
                sb.AppendLine($"{Enum.GetName(typeof(RealmPropertyBool), item.Key)}: {item.Value}");
            foreach (var item in PropertiesFloat)
                sb.AppendLine($"{Enum.GetName(typeof(RealmPropertyFloat), item.Key)}: {item.Value}");
            foreach (var item in PropertiesInt)
                sb.AppendLine($"{Enum.GetName(typeof(RealmPropertyInt), item.Key)}: {item.Value}");
            foreach (var item in PropertiesInt64)
                sb.AppendLine($"{Enum.GetName(typeof(RealmPropertyInt64), item.Key)}: {item.Value}");
            foreach (var item in PropertiesString)
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
            // Composition needs to happen first before rerolling,
            // so that random properties that depend on other random
            // properties with a composition rule can be applied in sequence

            result.RerollAllRules();
            return result;
        }

        private void ComposeFrom(RulesetTemplate parentTemplate)
        {
            if (parentTemplate == null)
                return;
            var parent = MakeRerolledRuleset(parentTemplate);
            ParentRuleset = parent;

            ApplyRulesetDict(parent.PropertiesBool, PropertiesBool);
            ApplyRulesetDict(parent.PropertiesFloat, PropertiesFloat);
            ApplyRulesetDict(parent.PropertiesInt, PropertiesInt);
            ApplyRulesetDict(parent.PropertiesInt64, PropertiesInt64);
            ApplyRulesetDict(parent.PropertiesString, PropertiesString);
        }

        private void RerollAllRules()
        {
            foreach (var v in PropertiesFloat.Values.Where(x =>
                x.Options.RandomType == RealmPropertyRerollType.landblock ||
                x.Options.RandomType == RealmPropertyRerollType.manual))
                    v.RollValue();
            foreach (var v in PropertiesInt.Values.Where(x =>
                x.Options.RandomType == RealmPropertyRerollType.landblock ||
                x.Options.RandomType == RealmPropertyRerollType.manual))
                    v.RollValue();
            foreach (var v in PropertiesInt64.Values.Where(x =>
                x.Options.RandomType == RealmPropertyRerollType.landblock ||
                x.Options.RandomType == RealmPropertyRerollType.manual))
                    v.RollValue();
        }



        public bool GetProperty(RealmPropertyBool property)
        {
            if (PropertiesBool.TryGetValue(property, out var value))
                return value.Value;
            return RealmManager.PropertyDefinitionsBool[property].DefaultValue;
        }
        public double GetProperty(RealmPropertyFloat property)
        {
            var att = RealmManager.PropertyDefinitionsFloat[property];
            if (PropertiesFloat.TryGetValue(property, out var result))
            {
                var val = result.Value;
                val = Math.Max(val, att.MinValue);
                val = Math.Min(val, att.MaxValue);
                return val;
            }
            return att.DefaultValue;
        }

        public int GetProperty(RealmPropertyInt property)
        {
            var att = RealmManager.PropertyDefinitionsInt[property];
            if (PropertiesInt.TryGetValue(property, out var result))
            {
                var val = result.Value;
                val = Math.Max(val, att.MinValue);
                val = Math.Min(val, att.MaxValue);
                return val;
            }
            return att.DefaultValue;
        }

        public long GetProperty(RealmPropertyInt64 property)
        {
            var att = RealmManager.PropertyDefinitionsInt64[property];
            if (PropertiesInt64.TryGetValue(property, out var result))
            {
                var val = result.Value;
                val = Math.Max(val, att.MinValue);
                val = Math.Min(val, att.MaxValue);
                return val;
            }
            return att.DefaultValue;
        }

        public string GetProperty(RealmPropertyString property)
        {
            if (PropertiesString.TryGetValue(property, out var result))
                return result.Value;
            return RealmManager.PropertyDefinitionsString[property].DefaultValue;
        }

        public uint GetDefaultInstanceID()
        {
            return ACE.Entity.Position.InstanceIDFromVars(this.Template.Realm.Id, 0, this.Template.Realm.Type == ACE.Entity.Enum.RealmType.Ruleset);
        }
    }
}
