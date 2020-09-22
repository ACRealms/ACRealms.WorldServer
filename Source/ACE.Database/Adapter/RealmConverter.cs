using ACE.Database.Models.World;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
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

            if (realm.RealmPropertiesBool != null && (instantiateEmptyCollections || realm.RealmPropertiesBool.Count > 0))
            {
                result.PropertiesBool = new Dictionary<RealmPropertyBool, bool>(realm.RealmPropertiesBool.Count);
                foreach (var value in realm.RealmPropertiesBool)
                    result.PropertiesBool[(RealmPropertyBool)value.Type] = value.Value;
            }
            if (realm.RealmPropertiesFloat != null && (instantiateEmptyCollections || realm.RealmPropertiesFloat.Count > 0))
            {
                result.PropertiesFloat = new Dictionary<RealmPropertyFloat, double>(realm.RealmPropertiesFloat.Count);
                foreach (var value in realm.RealmPropertiesFloat)
                    result.PropertiesFloat[(RealmPropertyFloat)value.Type] = value.Value;
            }
            if (realm.RealmPropertiesInt != null && (instantiateEmptyCollections || realm.RealmPropertiesInt.Count > 0))
            {
                result.PropertiesInt = new Dictionary<RealmPropertyInt, int>(realm.RealmPropertiesInt.Count);
                foreach (var value in realm.RealmPropertiesInt)
                    result.PropertiesInt[(RealmPropertyInt)value.Type] = value.Value;
            }
            if (realm.RealmPropertiesInt64 != null && (instantiateEmptyCollections || realm.RealmPropertiesInt64.Count > 0))
            {
                result.PropertiesInt64 = new Dictionary<RealmPropertyInt64, long>(realm.RealmPropertiesInt64.Count);
                foreach (var value in realm.RealmPropertiesInt64)
                    result.PropertiesInt64[(RealmPropertyInt64)value.Type] = value.Value;
            }
            if (realm.RealmPropertiesString != null && (instantiateEmptyCollections || realm.RealmPropertiesString.Count > 0))
            {
                result.PropertiesString = new Dictionary<RealmPropertyString, string>(realm.RealmPropertiesString.Count);
                foreach (var value in realm.RealmPropertiesString)
                    result.PropertiesString[(RealmPropertyString)value.Type] = value.Value;
            }

            return result;
        }

        public static ACE.Database.Models.World.Realm ConvertFromEntityRealm(ACE.Entity.Models.Realm realm, bool includeDatabaseRecordIds = false)
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
        }
    }
}
