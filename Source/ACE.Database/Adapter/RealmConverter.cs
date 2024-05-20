using ACE.Database.Models.World;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ACE.Database.Adapter
{
    public static class RealmConverter
    {
        public static Dictionary<RealmPropertyBool, RealmPropertyBoolAttribute> PropertyDefinitionsBool;
        public static Dictionary<RealmPropertyInt, RealmPropertyIntAttribute> PropertyDefinitionsInt;
        public static Dictionary<RealmPropertyInt64, RealmPropertyInt64Attribute> PropertyDefinitionsInt64;
        public static Dictionary<RealmPropertyFloat, RealmPropertyFloatAttribute> PropertyDefinitionsFloat;
        public static Dictionary<RealmPropertyString, RealmPropertyStringAttribute> PropertyDefinitionsString;

        public static void Initialize()
        {
            PropertyDefinitionsBool = RealmPropertyHelper.MakePropDict<RealmPropertyBool, RealmPropertyBoolAttribute>();
            PropertyDefinitionsInt = RealmPropertyHelper.MakePropDict<RealmPropertyInt, RealmPropertyIntAttribute>();
            PropertyDefinitionsInt64 = RealmPropertyHelper.MakePropDict<RealmPropertyInt64, RealmPropertyInt64Attribute>();
            PropertyDefinitionsString = RealmPropertyHelper.MakePropDict<RealmPropertyString, RealmPropertyStringAttribute>();
            PropertyDefinitionsFloat = RealmPropertyHelper.MakePropDict<RealmPropertyFloat, RealmPropertyFloatAttribute>();
        }

        public static ACE.Entity.Models.Realm ConvertToEntityRealm(ACE.Database.Models.World.Realm realm, bool instantiateEmptyCollections = false)
        {
            var jobs = new List<RealmLinkJob>();
            foreach (var jobtype_group in realm.RealmRulesetLinksRealm.GroupBy(x => x.LinkType))
            {
                foreach (var group in jobtype_group.GroupBy(x => x.ProbabilityGroup))
                {
                    if (group.Key == null)
                    {
                        foreach (var unconditionallink in group)
                        {
                            jobs.Add(new RealmLinkJob((RealmRulesetLinkType)unconditionallink.LinkType, new List<AppliedRealmLink>() {
                            new AppliedRealmLink(unconditionallink.LinkedRealmId, 1.0)}.AsReadOnly()));
                        }
                    }
                    else
                    {
                        var linksToAdd = new List<AppliedRealmLink>();
                        foreach (var link in group)
                            linksToAdd.Add(new AppliedRealmLink(link.LinkedRealmId, link.Probability.Value));
                        jobs.Add(new RealmLinkJob((RealmRulesetLinkType)jobtype_group.Key, linksToAdd.AsReadOnly()));
                    }
                }
            }

            var result = new ACE.Entity.Models.Realm(jobs);

            result.Id = realm.Id;
            result.Name = realm.Name;
            result.Type = (RealmType)realm.Type;
            result.ParentRealmID = realm.ParentRealmId;
            result.PropertyCountRandomized = realm.PropertyCountRandomized;
            result.AllProperties = new Dictionary<string, AppliedRealmProperty>();

            if (realm.RealmPropertiesBool != null && (instantiateEmptyCollections || realm.RealmPropertiesBool.Count > 0))
                result.PropertiesBool = MakePropertyDict<RealmPropertyBool, bool>(realm.RealmPropertiesBool, result);

            if (realm.RealmPropertiesFloat != null && (instantiateEmptyCollections || realm.RealmPropertiesFloat.Count > 0))
                result.PropertiesFloat = MakePropertyDict<RealmPropertyFloat, double> (realm.RealmPropertiesFloat, result);

            if (realm.RealmPropertiesInt != null && (instantiateEmptyCollections || realm.RealmPropertiesInt.Count > 0))
                result.PropertiesInt = MakePropertyDict<RealmPropertyInt, int>(realm.RealmPropertiesInt, result);

            if (realm.RealmPropertiesInt64 != null && (instantiateEmptyCollections || realm.RealmPropertiesInt64.Count > 0))
                result.PropertiesInt64 = MakePropertyDict<RealmPropertyInt64, long>(realm.RealmPropertiesInt64, result);

            if (realm.RealmPropertiesString != null && (instantiateEmptyCollections || realm.RealmPropertiesString.Count > 0))
                result.PropertiesString = MakePropertyDict<RealmPropertyString, string>(realm.RealmPropertiesString, result);

            return result;
        }

        private static IDictionary<TProp, AppliedRealmProperty<TVal>> MakePropertyDict<TProp, TVal>(IEnumerable<RealmPropertiesBase> dbValues, ACE.Entity.Models.Realm realmEntity)
            where TProp : Enum
            where TVal : IEquatable<TVal>
        {
            var result = new Dictionary<TProp, AppliedRealmProperty<TVal>>(dbValues.Count());
            foreach (var value in dbValues)
            {
                var appliedProperty = value.ConvertRealmProperty();
                result[(TProp)(object)value.Type] = (AppliedRealmProperty<TVal>)appliedProperty;
                realmEntity.AllProperties[appliedProperty.Options.Name] = appliedProperty;
            }
            return result;
        }
    }
}
