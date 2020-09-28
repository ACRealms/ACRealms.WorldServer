using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.Managers;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACE.Server.Realms
{
    public class AppliedRuleset
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Realm Realm { get; private set; }
        public HashSet<ushort> InheritedRealmIDs { get; } = new HashSet<ushort>();

        private IDictionary<RealmPropertyBool, (bool value, bool locked)> PropertiesBool { get; set; }
        private IDictionary<RealmPropertyFloat, (double value, bool locked)> PropertiesFloat { get; set; }
        private IDictionary<RealmPropertyInt, (int value, bool locked)> PropertiesInt { get; set; }
        private IDictionary<RealmPropertyInt64, (long value, bool locked)> PropertiesInt64 { get; set; }
        private IDictionary<RealmPropertyString, (string value, bool locked)> PropertiesString { get; set; }

        public static AppliedRuleset MakeTopLevelRuleset(Realm entity)
        {
            var ruleset = new AppliedRuleset();
            ruleset.Realm = entity;
            ruleset.InheritedRealmIDs.Add(entity.Id);

            ruleset.PropertiesBool = new Dictionary<RealmPropertyBool, (bool, bool)>(entity.PropertiesBool);
            ruleset.PropertiesFloat = new Dictionary<RealmPropertyFloat, (double, bool)>(entity.PropertiesFloat);
            ruleset.PropertiesInt = new Dictionary<RealmPropertyInt, (int, bool)>(entity.PropertiesInt);
            ruleset.PropertiesInt64 = new Dictionary<RealmPropertyInt64, (long, bool)>(entity.PropertiesInt64);
            ruleset.PropertiesString = new Dictionary<RealmPropertyString, (string, bool)>(entity.PropertiesString);

            return ruleset;
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

            ruleset.PropertiesBool = new Dictionary<RealmPropertyBool, (bool, bool)>(baseset.PropertiesBool);
            ruleset.PropertiesFloat = new Dictionary<RealmPropertyFloat, (double, bool)>(baseset.PropertiesFloat);
            ruleset.PropertiesInt = new Dictionary<RealmPropertyInt, (int, bool)>(baseset.PropertiesInt);
            ruleset.PropertiesInt64 = new Dictionary<RealmPropertyInt64, (long, bool)>(baseset.PropertiesInt64);
            ruleset.PropertiesString = new Dictionary<RealmPropertyString, (string, bool)>(baseset.PropertiesString);

            ApplyRulesetDict(ruleset.PropertiesBool, subset.PropertiesBool);
            ApplyRulesetDict(ruleset.PropertiesFloat, subset.PropertiesFloat);
            ApplyRulesetDict(ruleset.PropertiesInt, subset.PropertiesInt);
            ApplyRulesetDict(ruleset.PropertiesInt64, subset.PropertiesInt64);
            ApplyRulesetDict(ruleset.PropertiesString, subset.PropertiesString);

            return ruleset;
        }

        private static void ApplyRulesetDict<K, V>(IDictionary<K, (V value, bool locked)> dest, IDictionary<K, (V value, bool locked)> sub)
        {
            foreach(var prop in sub)
            {
                if (dest.ContainsKey(prop.Key))
                {
                    if (dest[prop.Key].locked)
                        continue;
                }
                dest[prop.Key] = prop.Value;
            }
        }

        private AppliedRuleset() { }

        private void InitializePropertyDictionaries()
        {
            PropertiesBool = new Dictionary<RealmPropertyBool, (bool, bool)>();
            PropertiesFloat = new Dictionary<RealmPropertyFloat, (double, bool)>();
            PropertiesInt = new Dictionary<RealmPropertyInt, (int, bool)>();
            PropertiesInt64 = new Dictionary<RealmPropertyInt64, (long, bool)>();
            PropertiesString = new Dictionary<RealmPropertyString, (string, bool)>();
        }

        public bool GetProperty(RealmPropertyBool property)
        {
            if (PropertiesBool.TryGetValue(property, out var value))
                return value.value;
            return RealmManager.PropertyDefinitionsBool[property].DefaultValue;
        }
        public double GetProperty(RealmPropertyFloat property)
        {
            var att = RealmManager.PropertyDefinitionsFloat[property];
            if (PropertiesFloat.TryGetValue(property, out var result))
            {
                var val = result.value;
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
                var val = result.value;
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
                var val = result.value;
                val = Math.Max(val, att.MinValue);
                val = Math.Min(val, att.MaxValue);
                return val;
            }
            return att.DefaultValue;
        }

        public string GetProperty(RealmPropertyString property)
        {
            if (PropertiesString.TryGetValue(property, out var result))
                return result.value;
            return RealmManager.PropertyDefinitionsString[property].DefaultValue;
        }

        public uint GetDefaultInstanceID()
        {
            return ACE.Entity.Position.InstanceIDFromVars(this.Realm.Id, 0, this.Realm.Type == ACE.Entity.Enum.RealmType.Ruleset);
        }
    }
}
