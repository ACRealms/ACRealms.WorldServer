using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
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

        private IDictionary<RealmPropertyBool, bool> PropertiesBool { get; set; }
        private IDictionary<RealmPropertyFloat, double> PropertiesFloat { get; set; }
        private IDictionary<RealmPropertyInt, int> PropertiesInt { get; set; }
        private IDictionary<RealmPropertyInt64, long> PropertiesInt64 { get; set; }
        private IDictionary<RealmPropertyString, string> PropertiesString { get; set; }

        public static AppliedRuleset MakeTopLevelRuleset(Realm entity)
        {
            var ruleset = new AppliedRuleset();
            ruleset.Realm = entity;
            ruleset.InheritedRealmIDs.Add(entity.Id);

            ruleset.PropertiesBool = new Dictionary<RealmPropertyBool, bool>(entity.PropertiesBool);
            ruleset.PropertiesFloat = new Dictionary<RealmPropertyFloat, double>(entity.PropertiesFloat);
            ruleset.PropertiesInt = new Dictionary<RealmPropertyInt, int>(entity.PropertiesInt);
            ruleset.PropertiesInt64 = new Dictionary<RealmPropertyInt64, long>(entity.PropertiesInt64);
            ruleset.PropertiesString = new Dictionary<RealmPropertyString, string>(entity.PropertiesString);

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

            ruleset.PropertiesBool = new Dictionary<RealmPropertyBool, bool>(subset.PropertiesBool);
            ruleset.PropertiesFloat = new Dictionary<RealmPropertyFloat, double>(subset.PropertiesFloat);
            ruleset.PropertiesInt = new Dictionary<RealmPropertyInt, int>(subset.PropertiesInt);
            ruleset.PropertiesInt64 = new Dictionary<RealmPropertyInt64, long>(subset.PropertiesInt64);
            ruleset.PropertiesString = new Dictionary<RealmPropertyString, string>(subset.PropertiesString);

            foreach (var prop in subset.PropertiesBool)
                ruleset.PropertiesBool[prop.Key] = prop.Value;
            foreach (var prop in subset.PropertiesFloat)
                ruleset.PropertiesFloat[prop.Key] = prop.Value;
            foreach (var prop in subset.PropertiesInt)
                ruleset.PropertiesInt[prop.Key] = prop.Value;
            foreach (var prop in subset.PropertiesInt64)
                ruleset.PropertiesInt64[prop.Key] = prop.Value;
            foreach (var prop in subset.PropertiesString)
                ruleset.PropertiesString[prop.Key] = prop.Value;

            return ruleset;
        }

        private AppliedRuleset() { }

        private void InitializePropertyDictionaries()
        {
            PropertiesBool = new Dictionary<RealmPropertyBool, bool>();
            PropertiesFloat = new Dictionary<RealmPropertyFloat, double>();
            PropertiesInt = new Dictionary<RealmPropertyInt, int>();
            PropertiesInt64 = new Dictionary<RealmPropertyInt64, long>();
            PropertiesString = new Dictionary<RealmPropertyString, string>();
        }

        public bool? GetProperty(RealmPropertyBool property)
        {
            if (PropertiesBool.TryGetValue(property, out var result))
                return result;
            return null;
        }
        public double? GetProperty(RealmPropertyFloat property)
        {
            if (PropertiesFloat.TryGetValue(property, out var result))
                return result;
            return null;
        }

        public int? GetProperty(RealmPropertyInt property)
        {
            if (PropertiesInt.TryGetValue(property, out var result))
                return result;
            return null;
        }

        public long? GetProperty(RealmPropertyInt64 property)
        {
            if (PropertiesInt64.TryGetValue(property, out var result))
                return result;
            return null;
        }

        public string GetProperty(RealmPropertyString property)
        {
            if (PropertiesString.TryGetValue(property, out var result))
                return result;
            return null;
        }
    }
}
