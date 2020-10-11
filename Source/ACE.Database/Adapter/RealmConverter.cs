using ACE.Database.Models.World;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACE.Database.Adapter
{
    public static class RealmConverter
    {
        public static ACE.Entity.Models.Realm ConvertToEntityRealm(ACE.Database.Models.World.Realm realm, bool instantiateEmptyCollections = false)
        {
            var result = new ACE.Entity.Models.Realm();

            result.Id = realm.Id;
            result.Name = realm.Name;
            result.Type = (RealmType)realm.Type;
            result.ParentRealmID = realm.ParentRealmId;
            result.PropertyCountRandomized = realm.PropertyCountRandomized;

            if (realm.RealmPropertiesBool != null && (instantiateEmptyCollections || realm.RealmPropertiesBool.Count > 0))
            {
                result.PropertiesBool = new Dictionary<RealmPropertyBool, AppliedRealmProperty<bool>> (realm.RealmPropertiesBool.Count);
                foreach (var value in realm.RealmPropertiesBool)
                    result.PropertiesBool[(RealmPropertyBool)value.Type] = ConvertRealmProperty(value);
            }
            if (realm.RealmPropertiesFloat != null && (instantiateEmptyCollections || realm.RealmPropertiesFloat.Count > 0))
            {
                result.PropertiesFloat = new Dictionary<RealmPropertyFloat, AppliedRealmProperty<double>>(realm.RealmPropertiesFloat.Count);
                foreach (var value in realm.RealmPropertiesFloat)
                    result.PropertiesFloat[(RealmPropertyFloat)value.Type] = ConvertRealmProperty(value);
            }
            if (realm.RealmPropertiesInt != null && (instantiateEmptyCollections || realm.RealmPropertiesInt.Count > 0))
            {
                result.PropertiesInt = new Dictionary<RealmPropertyInt, AppliedRealmProperty<int>>(realm.RealmPropertiesInt.Count);
                foreach (var value in realm.RealmPropertiesInt)
                    result.PropertiesInt[(RealmPropertyInt)value.Type] = ConvertRealmProperty(value);
            }
            if (realm.RealmPropertiesInt64 != null && (instantiateEmptyCollections || realm.RealmPropertiesInt64.Count > 0))
            {
                result.PropertiesInt64 = new Dictionary<RealmPropertyInt64, AppliedRealmProperty<long>>(realm.RealmPropertiesInt64.Count);
                foreach (var value in realm.RealmPropertiesInt64)
                    result.PropertiesInt64[(RealmPropertyInt64)value.Type] = ConvertRealmProperty(value);
            }
            if (realm.RealmPropertiesString != null && (instantiateEmptyCollections || realm.RealmPropertiesString.Count > 0))
            {
                result.PropertiesString = new Dictionary<RealmPropertyString, AppliedRealmProperty<string>>(realm.RealmPropertiesString.Count);
                foreach (var value in realm.RealmPropertiesString)
                    result.PropertiesString[(RealmPropertyString)value.Type] = ConvertRealmProperty(value);
            }

            return result;
        }

        private static AppliedRealmProperty<bool> ConvertRealmProperty(RealmPropertiesBool dbobj)
        {
            var prop = new RealmPropertyOptions<bool>();
            prop.SeedPropertiesStatic(dbobj.Value, dbobj.Locked, dbobj.Probability);
            return new AppliedRealmProperty<bool>(dbobj.Type, prop);
        }

        private static AppliedRealmProperty<string> ConvertRealmProperty(RealmPropertiesString dbobj)
        {
            var prop = new RealmPropertyOptions<string>();
            prop.SeedPropertiesStatic(dbobj.Value, dbobj.Locked, dbobj.Probability);
            return new AppliedRealmProperty<string>(dbobj.Type, prop);
        }

        private static AppliedRealmProperty<int> ConvertRealmProperty(RealmPropertiesInt dbobj)
        {
            var prop = new RealmPropertyOptions<int>();
            if (dbobj.Value.HasValue)
                prop.SeedPropertiesStatic(dbobj.Value.Value, dbobj.Locked, dbobj.Probability);
            else
                prop.SeedPropertiesRandomized(dbobj.RandomType, dbobj.RandomLowRange.Value, dbobj.RandomHighRange.Value, dbobj.Locked, dbobj.Probability);
            return new AppliedRealmProperty<int>(dbobj.Type, prop);
        }

        private static AppliedRealmProperty<long> ConvertRealmProperty(RealmPropertiesInt64 dbobj)
        {
            var prop = new RealmPropertyOptions<long>();
            if (dbobj.Value.HasValue)
                prop.SeedPropertiesStatic(dbobj.Value.Value, dbobj.Locked, dbobj.Probability);
            else
                prop.SeedPropertiesRandomized(dbobj.RandomType, dbobj.RandomLowRange.Value, dbobj.RandomHighRange.Value, dbobj.Locked, dbobj.Probability);
            return new AppliedRealmProperty<long>(dbobj.Type, prop);
        }

        private static AppliedRealmProperty<double> ConvertRealmProperty(RealmPropertiesFloat dbobj)
        {
            var prop = new RealmPropertyOptions<double>();
            if (dbobj.Value.HasValue)
                prop.SeedPropertiesStatic(dbobj.Value.Value, dbobj.Locked, dbobj.Probability);
            else
                prop.SeedPropertiesRandomized(dbobj.RandomType, dbobj.RandomLowRange.Value, dbobj.RandomHighRange.Value, dbobj.Locked, dbobj.Probability);
            return new AppliedRealmProperty<double>(dbobj.Type, prop);
        }

        /*public static ACE.Database.Models.World.Realm ConvertFromEntityRealm(ACE.Entity.Models.Realm realm, bool includeDatabaseRecordIds = false)
        {
            var result = new ACE.Database.Models.World.Realm();

            result.Id = realm.Id;
            result.Name = realm.Name;
            result.Type = (ushort)realm.Type;
            result.ParentRealmId = realm.ParentRealmID;

            if (realm.PropertiesBool != null)
            {
                foreach (var kvp in realm.PropertiesBool)
                    result.SetProperty(kvp.Key, kvp.Value);
            }
            if (realm.PropertiesFloat != null)
            {
                foreach (var kvp in realm.PropertiesFloat)
                    result.SetProperty(kvp.Key, kvp.Value);
            }
            if (realm.PropertiesInt != null)
            {
                foreach (var kvp in realm.PropertiesInt)
                    result.SetProperty(kvp.Key, kvp.Value);
            }
            if (realm.PropertiesInt64 != null)
            {
                foreach (var kvp in realm.PropertiesInt64)
                    result.SetProperty(kvp.Key, kvp.Value);
            }
            if (realm.PropertiesString != null)
            {
                foreach (var kvp in realm.PropertiesString)
                    result.SetProperty(kvp.Key, kvp.Value);
            }

            return result;
        }*/
    }
}
