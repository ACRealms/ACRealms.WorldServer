using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Frozen;
using ACRealms.Rulesets.Enums;
using ACRealms.RealmProps.Underlying;
using System.ComponentModel.DataAnnotations;
using System.Collections.Immutable;

namespace ACRealms.Rulesets.DBOld
{
    internal static class RealmConverter
    {
        public static Rulesets.Realm ConvertToEntityRealm(DBOld.Realm realm, bool instantiateEmptyCollections = false)
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

            var result = new Rulesets.Realm(jobs);

            result.Id = realm.Id;
            result.Name = realm.Name;
            result.Type = (RealmType)realm.Type;
            result.ParentRealmID = realm.ParentRealmId;
            result.PropertyCountRandomized = realm.PropertyCountRandomized;
            result.AllProperties = new Dictionary<string, TemplatedRealmPropertyGroup>();

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

        internal class StagedTemplatePropGroup<TVal>
            where TVal : IEquatable<TVal>
        {
            internal readonly List<TemplatedRealmProperty<TVal>> Props = [];
            internal string Name;
            internal RealmPropertyGroupOptions GroupOptions;
        }

        private static IDictionary<TProp, TemplatedRealmPropertyGroup<TVal>> MakePropertyDict<TProp, TVal>(IEnumerable<RealmPropertiesBase> dbValues, Rulesets.Realm realmEntity)
            where TProp : Enum
            where TVal : IEquatable<TVal>
        {
            var result = new Dictionary<TProp, StagedTemplatePropGroup<TVal>>();
            foreach (var value in dbValues)
            {
                // We can have multiple properties in a ruleset now with the same key.
                // Group each property definition together, differing by scope

                var t = value.Type;
                var e = System.Runtime.CompilerServices.Unsafe.As<int, TProp>(ref t);
                bool isNewGroup = false;
                if (!result.TryGetValue(e, out var group))
                {
                    group = new StagedTemplatePropGroup<TVal>();
                    result[e] = group;
                    isNewGroup = true;
                }

                var scope = value.ConvertScopeOptions();
                if (isNewGroup)
                    group.GroupOptions = value.ConvertGroupOptions();

                var templateProperty = value.ConvertRealmProperty(group.GroupOptions, scope);

                if (isNewGroup)
                    group.Name = templateProperty.Options.Name;

                //realmEntity.AllProperties[templateProperty.Options.Name] = templateProperty;

                var prop = (TemplatedRealmProperty<TVal>)templateProperty;
                group.Props.Add(prop);
            }
            var groupResult = result.ToFrozenDictionary(propsForGroup => propsForGroup.Key, propsForGroup =>
                new TemplatedRealmPropertyGroup<TVal>() {
                    Name = propsForGroup.Value.Name,
                    Properties = [.. propsForGroup.Value.Props]
                });
            foreach(var item in groupResult)
            {
                realmEntity.AllProperties[item.Value.Name] = item.Value;
            }
            return groupResult;
        }
    }
}
