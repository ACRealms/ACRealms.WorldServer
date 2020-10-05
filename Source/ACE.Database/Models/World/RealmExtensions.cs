using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Collections.Generic;
using ACE.Entity;
using ACE.Entity.Enum.Properties;

namespace ACE.Database.Models.World
{
    public partial class Realm
    {
        [NotMapped]
        public string ParentRealmName { get; set; }

        [NotMapped]
        public Dictionary<ushort, Realm> Descendents = new Dictionary<ushort, Realm>();

        public void SetId(ushort value)
        {
            this.Id = value;
            foreach (var item in RealmPropertiesBool)
                item.RealmId = value;
            foreach (var item in RealmPropertiesInt)
                item.RealmId = value;
            foreach (var item in RealmPropertiesInt64)
                item.RealmId = value;
            foreach (var item in RealmPropertiesString)
                item.RealmId = value;
            foreach (var item in RealmPropertiesFloat)
                item.RealmId = value;
        }
    }

    public static class RealmExtensions
    {
        // =====================================
        // Get
        // Bool, DID, Float, Int, Int64, String
        // =====================================

        public static bool? GetProperty(this Realm realm, RealmPropertyBool property)
        {
            return realm.RealmPropertiesBool.FirstOrDefault(x => x.Type == (uint)property)?.Value;
        }

        public static double? GetProperty(this Realm realm, RealmPropertyFloat property)
        {
            return realm.RealmPropertiesFloat.FirstOrDefault(x => x.Type == (ushort)property)?.Value;
        }

        public static int? GetProperty(this Realm realm, RealmPropertyInt property)
        {
            return realm.RealmPropertiesInt.FirstOrDefault(x => x.Type == (uint)property)?.Value;
        }

        public static long? GetProperty(this Realm realm, RealmPropertyInt64 property)
        {
            return realm.RealmPropertiesInt64.FirstOrDefault(x => x.Type == (uint)property)?.Value;
        }

        public static string GetProperty(this Realm realm, RealmPropertyString property)
        {
            return realm.RealmPropertiesString.FirstOrDefault(x => x.Type == (uint)property)?.Value;
        }


        // =====================================
        // Set
        // Bool, DID, Float, Int, Int64, String
        // =====================================

        //Slower than SetProperty as it has to use reflection
        public static void SetPropertyByName(this Realm realm, string propertyName, dynamic value)
        {
            if (Enum.TryParse<RealmPropertyBool>(propertyName, out var boolprop))
                SetProperty(realm, boolprop, (bool)value);
            else if (Enum.TryParse<RealmPropertyInt>(propertyName, out var intprop))
                SetProperty(realm, intprop, (int)value);
            else if (Enum.TryParse<RealmPropertyString>(propertyName, out var stringprop))
                SetProperty(realm, stringprop, (string)value);
            else if (Enum.TryParse<RealmPropertyFloat>(propertyName, out var floatprop))
                SetProperty(realm, floatprop, (double)value);
            else if (Enum.TryParse<RealmPropertyInt64>(propertyName, out var longprop))
                SetProperty(realm, longprop, (long)value);
            else
                throw new Exception("Realm property not found: " + propertyName);
        }

        public static void SetProperty(this Realm realm, RealmPropertyBool property, bool value)
        {
            var result = realm.RealmPropertiesBool.FirstOrDefault(x => x.Type == (uint)property);

            if (result != null)
                result.Value = value;
            else
            {
                var entity = new RealmPropertiesBool { RealmId = realm.Id, Type = (ushort)property, Value = value, Realm = realm };

                realm.RealmPropertiesBool.Add(entity);
            }
        }

        public static void SetProperty(this Realm realm, RealmPropertyFloat property, double value)
        {
            var result = realm.RealmPropertiesFloat.FirstOrDefault(x => x.Type == (ushort)property);

            if (result != null)
                result.Value = value;
            else
            {
                var entity = new RealmPropertiesFloat { RealmId = realm.Id, Type = (ushort)property, Value = value, Realm = realm };

                realm.RealmPropertiesFloat.Add(entity);
            }
        }

        public static void SetProperty(this Realm realm, RealmPropertyInt property, int value)
        {
            var result = realm.RealmPropertiesInt.FirstOrDefault(x => x.Type == (uint)property);

            if (result != null)
                result.Value = value;
            else
            {
                var entity = new RealmPropertiesInt { RealmId = realm.Id, Type = (ushort)property, Value = value, Realm = realm };

                realm.RealmPropertiesInt.Add(entity);
            }
        }

        public static void SetProperty(this Realm realm, RealmPropertyInt64 property, long value)
        {
            var result = realm.RealmPropertiesInt64.FirstOrDefault(x => x.Type == (uint)property);

            if (result != null)
                result.Value = value;
            else
            {
                var entity = new RealmPropertiesInt64 { RealmId = realm.Id, Type = (ushort)property, Value = value, Realm = realm };

                realm.RealmPropertiesInt64.Add(entity);
            }
        }

        public static void SetProperty(this Realm realm, RealmPropertyString property, string value)
        {
            var result = realm.RealmPropertiesString.FirstOrDefault(x => x.Type == (uint)property);

            if (result != null)
                result.Value = value;
            else
            {
                var entity = new RealmPropertiesString { RealmId = realm.Id, Type = (ushort)property, Value = value, Realm = realm };

                realm.RealmPropertiesString.Add(entity);
            }
        }

        // =====================================
        // Remove
        // Bool, DID, Float, Int, Int64, String
        // =====================================

        public static bool TryRemoveProperty(this Realm realm, RealmPropertyBool property, out RealmPropertiesBool entity)
        {
            entity = realm.RealmPropertiesBool.FirstOrDefault(x => x.Type == (uint)property);

            if (entity != null)
            {
                realm.RealmPropertiesBool.Remove(entity);
                entity.Realm = null;
                return true;
            }

            return false;
        }

        public static bool TryRemoveProperty(this Realm realm, RealmPropertyFloat property, out RealmPropertiesFloat entity)
        {
            entity = realm.RealmPropertiesFloat.FirstOrDefault(x => x.Type == (ushort)property);

            if (entity != null)
            {
                realm.RealmPropertiesFloat.Remove(entity);
                entity.Realm = null;
                return true;
            }

            return false;
        }

        public static bool TryRemoveProperty(this Realm realm, RealmPropertyInt property, out RealmPropertiesInt entity)
        {
            entity = realm.RealmPropertiesInt.FirstOrDefault(x => x.Type == (uint)property);

            if (entity != null)
            {
                realm.RealmPropertiesInt.Remove(entity);
                entity.Realm = null;
                return true;
            }

            return false;
        }

        public static bool TryRemoveProperty(this Realm realm, RealmPropertyInt64 property, out RealmPropertiesInt64 entity)
        {
            entity = realm.RealmPropertiesInt64.FirstOrDefault(x => x.Type == (uint)property);

            if (entity != null)
            {
                realm.RealmPropertiesInt64.Remove(entity);
                entity.Realm = null;
                return true;
            }

            return false;
        }

        public static bool TryRemoveProperty(this Realm realm, RealmPropertyString property, out RealmPropertiesString entity)
        {
            entity = realm.RealmPropertiesString.FirstOrDefault(x => x.Type == (uint)property);

            if (entity != null)
            {
                realm.RealmPropertiesString.Remove(entity);
                entity.Realm = null;
                return true;
            }

            return false;
        }
    }
}
