using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.Managers;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ACE.Server.Realms
{
    public class AppliedRuleset
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Realm Realm { get; private set; }
        public HashSet<ushort> InheritedRealmIDs { get; private set; } = new HashSet<ushort>();

        private IDictionary<RealmPropertyBool, AppliedRealmProperty<bool>> PropertiesBool { get; set; }
        private IDictionary<RealmPropertyFloat, AppliedRealmProperty<double>> PropertiesFloat { get; set; }
        private IDictionary<RealmPropertyInt, AppliedRealmProperty<int>> PropertiesInt { get; set; }
        private IDictionary<RealmPropertyInt64, AppliedRealmProperty<long>> PropertiesInt64 { get; set; }
        private IDictionary<RealmPropertyString, AppliedRealmProperty<string>> PropertiesString { get; set; }

        public static AppliedRuleset MakeTopLevelRuleset(Realm entity)
        {
            var ruleset = new AppliedRuleset();
            ruleset.Realm = entity;
            ruleset.InheritedRealmIDs.Add(entity.Id);

            ruleset.PropertiesBool = new Dictionary<RealmPropertyBool, AppliedRealmProperty<bool>>(entity.PropertiesBool);
            ruleset.PropertiesFloat = new Dictionary<RealmPropertyFloat, AppliedRealmProperty<double>>(entity.PropertiesFloat);
            ruleset.PropertiesInt = new Dictionary<RealmPropertyInt, AppliedRealmProperty<int>>(entity.PropertiesInt);
            ruleset.PropertiesInt64 = new Dictionary<RealmPropertyInt64, AppliedRealmProperty<long>>(entity.PropertiesInt64);
            ruleset.PropertiesString = new Dictionary<RealmPropertyString, AppliedRealmProperty<string>>(entity.PropertiesString);

            return ruleset;
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

        public static AppliedRuleset ApplyRuleset(AppliedRuleset baseset, Realm subset)
        {
            if (baseset.InheritedRealmIDs.Contains(subset.Id))
                throw new Exception("Circular inheritance chain detected for realm id " + subset.Id.ToString());
            if (baseset.Realm.Type == ACE.Entity.Enum.RealmType.Ruleset && subset.Type == ACE.Entity.Enum.RealmType.Realm)
                throw new Exception("Realms may not inherit from rulesets at this time.");
            var ruleset = new AppliedRuleset();
            ruleset.Realm = subset;
            ruleset.InheritedRealmIDs.Add(subset.Id);

            ruleset.PropertiesBool = new Dictionary<RealmPropertyBool, AppliedRealmProperty<bool>>(baseset.PropertiesBool);
            ruleset.PropertiesFloat = new Dictionary<RealmPropertyFloat, AppliedRealmProperty<double>>(baseset.PropertiesFloat);
            ruleset.PropertiesInt = new Dictionary<RealmPropertyInt, AppliedRealmProperty<int>>(baseset.PropertiesInt);
            ruleset.PropertiesInt64 = new Dictionary<RealmPropertyInt64, AppliedRealmProperty<long>>(baseset.PropertiesInt64);
            ruleset.PropertiesString = new Dictionary<RealmPropertyString, AppliedRealmProperty<string>>(baseset.PropertiesString);

            ApplyRulesetDict(ruleset.PropertiesBool, subset.PropertiesBool);
            ApplyRulesetDict(ruleset.PropertiesFloat, subset.PropertiesFloat);
            ApplyRulesetDict(ruleset.PropertiesInt, subset.PropertiesInt);
            ApplyRulesetDict(ruleset.PropertiesInt64, subset.PropertiesInt64);
            ApplyRulesetDict(ruleset.PropertiesString, subset.PropertiesString);

            return ruleset;
        }

        internal static AppliedRuleset MakeRerolledRuleset(AppliedRuleset baseset)
        {
            var ruleset = new AppliedRuleset(baseset);
            ruleset.RerollAllRules();
            return ruleset;
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

        private static void ApplyRulesetDict<K, V>(IDictionary<K, AppliedRealmProperty<V>> dest, IDictionary<K, AppliedRealmProperty<V>> sub)
        {
            foreach(var prop in sub)
            {
                if (dest.ContainsKey(prop.Key))
                {
                    if (dest[prop.Key].Options.Locked)
                        continue;
                }
                dest[prop.Key] = new AppliedRealmProperty<V>(prop.Value);
                if (prop.Value.Options.RandomType == RealmPropertyRerollType.landblock)
                    prop.Value.RollValue();
            }
        }

        private AppliedRuleset() { }

        //Deep copy
        private AppliedRuleset(AppliedRuleset baseset)
        {
            Realm = baseset.Realm;
            InheritedRealmIDs = baseset.InheritedRealmIDs;

            InitializePropertyDictionaries();

            foreach(var item in baseset.PropertiesBool)
                PropertiesBool.Add(item.Key, new AppliedRealmProperty<bool>(item.Value));
            foreach(var item in baseset.PropertiesFloat)
                PropertiesFloat.Add(item.Key, new AppliedRealmProperty<double>(item.Value));
            foreach (var item in baseset.PropertiesInt)
                PropertiesInt.Add(item.Key, new AppliedRealmProperty<int>(item.Value));
            foreach (var item in baseset.PropertiesInt64)
                PropertiesInt64.Add(item.Key, new AppliedRealmProperty<long>(item.Value));
            foreach (var item in baseset.PropertiesString)
                PropertiesString.Add(item.Key, new AppliedRealmProperty<string>(item.Value));
        }

        private void InitializePropertyDictionaries()
        {
            PropertiesBool = new Dictionary<RealmPropertyBool, AppliedRealmProperty<bool>>();
            PropertiesFloat = new Dictionary<RealmPropertyFloat, AppliedRealmProperty<double>>();
            PropertiesInt = new Dictionary<RealmPropertyInt, AppliedRealmProperty<int>>();
            PropertiesInt64 = new Dictionary<RealmPropertyInt64, AppliedRealmProperty<long>>();
            PropertiesString = new Dictionary<RealmPropertyString, AppliedRealmProperty<string>>();
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
            return ACE.Entity.Position.InstanceIDFromVars(this.Realm.Id, 0, this.Realm.Type == ACE.Entity.Enum.RealmType.Ruleset);
        }
    }
}