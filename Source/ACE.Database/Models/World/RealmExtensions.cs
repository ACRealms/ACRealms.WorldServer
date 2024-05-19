using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Collections.Generic;
using ACE.Entity;
using ACE.Entity.Enum.Properties;
using Newtonsoft.Json.Linq;

namespace ACE.Database.Models.World
{
    public class RealmPropertyJsonModel
    {
        public string value { get; set; }
        public string low { get; set; }
        public string high { get; set; }
        public bool? locked { get; set; }
        public double? probability { get; set; }
        public RealmPropertyRerollType? reroll { get; set; }
        public RealmPropertyCompositionType compose { get; set; }

        public void ValidateAll()
        {
            if ((low == null && high != null) || (low != null && high == null))
                throw new Exception("Both low and high values must be present if one is present.");
            if (value == null && low == null)
                throw new Exception("Either value or low/high range must be provided.");
            if (value != null && low != null)
                throw new Exception("If providing a value, may not provide a low/high range.");
        }

        public void ValidateValuePresent()
        {
            if (value == null)
                throw new Exception("value must be present");
        }
    }

    public partial class RealmPropertiesBool
    {
        public void SetProperties(RealmPropertyJsonModel model)
        {
            model.ValidateValuePresent();
            this.Value = bool.Parse(model.value);
            this.Locked = model.locked ?? false;
            this.Probability = model.probability;
        }
    }

    public partial class RealmPropertiesFloat
    {
        public void SetProperties(RealmPropertyJsonModel model)
        {
            if (model.value != null)
                this.Value = double.Parse(model.value);
            if (model.low != null)
            {
                this.RandomLowRange = double.Parse(model.low);
                this.RandomHighRange = double.Parse(model.high);
                if (RandomLowRange > RandomHighRange)
                    throw new Exception("high must be > low");
                if (!model.reroll.HasValue)
                    model.reroll = RealmPropertyRerollType.landblock;
            }
            this.Locked = model.locked ?? false;
            this.Probability = model.probability;
            this.RandomType = (byte)(model.reroll ?? RealmPropertyRerollType.never);
            this.CompositionType = (byte)model.compose;
        }
    }

    public partial class RealmPropertiesInt
    {
        public void SetProperties(RealmPropertyJsonModel model)
        {
            if (model.value != null)
                this.Value = int.Parse(model.value);
            if (model.low != null)
            {
                this.RandomLowRange = int.Parse(model.low);
                this.RandomHighRange = int.Parse(model.high);
                if (RandomLowRange > RandomHighRange)
                    throw new Exception("high must be > low");
                if (!model.reroll.HasValue)
                    model.reroll = RealmPropertyRerollType.landblock;
            }
            this.Locked = model.locked ?? false;
            this.Probability = model.probability;
            this.RandomType = (byte)(model.reroll ?? RealmPropertyRerollType.never);
            this.CompositionType = (byte)model.compose;
        }
    }

    public partial class RealmPropertiesInt64
    {
        public void SetProperties(RealmPropertyJsonModel model)
        {
            if (model.value != null)
                this.Value = long.Parse(model.value);
            if (model.low != null)
            {
                this.RandomLowRange = long.Parse(model.low);
                this.RandomHighRange = long.Parse(model.high);
                if (RandomLowRange > RandomHighRange)
                    throw new Exception("high must be > low");
                if (!model.reroll.HasValue)
                    model.reroll = RealmPropertyRerollType.landblock;
            }
            this.Locked = model.locked ?? false;
            this.Probability = model.probability;
            this.RandomType = (byte)(model.reroll ?? RealmPropertyRerollType.never);
            this.CompositionType = (byte)model.compose;
        }
    }

    public partial class RealmPropertiesString
    {
        public void SetProperties(RealmPropertyJsonModel model)
        {
            model.ValidateValuePresent();
            this.Value = model.value;
            this.Locked = model.locked ?? false;
            this.Probability = model.probability;
        }
    }

    public partial class RealmRulesetLinks
    {
        [NotMapped]
        public string Import_RulesetToApply { get; set; }
    }

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

        public override string ToString()
        {
            return $"{Name} ({Id})";
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
        public static void SetPropertyByName(this Realm realm, string propertyName, JToken value)
        {
            if (Enum.TryParse<RealmPropertyBool>(propertyName, out var boolprop))
                SetProperty(realm, boolprop, ((bool)value));
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

        
        public static void SetPropertyByName_Complex(this Realm realm, string propertyName, RealmPropertyJsonModel pobj)
        {
            pobj.ValidateAll();

            if (Enum.TryParse<RealmPropertyBool>(propertyName, out var boolprop))
                SetProperty_Complex(realm, boolprop, pobj);
            else if (Enum.TryParse<RealmPropertyInt>(propertyName, out var intprop))
                SetProperty_Complex(realm, intprop, pobj);
            else if (Enum.TryParse<RealmPropertyString>(propertyName, out var stringprop))
                SetProperty_Complex(realm, stringprop, pobj);
            else if (Enum.TryParse<RealmPropertyFloat>(propertyName, out var floatprop))
                SetProperty_Complex(realm, floatprop, pobj);
            else if (Enum.TryParse<RealmPropertyInt64>(propertyName, out var longprop))
                SetProperty_Complex(realm, longprop, pobj);
            else
                throw new Exception("Realm property not found: " + propertyName);
        }
        
        
        private static void SetProperty_Complex(Realm realm, RealmPropertyInt64 property, RealmPropertyJsonModel pobj)
        {
            var result = realm.RealmPropertiesInt64.FirstOrDefault(x => x.Type == (ushort)property);

            if (result != null)
                result.SetProperties(pobj);
            else
            {
                var entity = new RealmPropertiesInt64 { RealmId = realm.Id, Type = (ushort)property };
                entity.SetProperties(pobj);
                realm.RealmPropertiesInt64.Add(entity);
            }
        }

        private static void SetProperty_Complex(Realm realm, RealmPropertyBool property, RealmPropertyJsonModel pobj)
        {
            var result = realm.RealmPropertiesBool.FirstOrDefault(x => x.Type == (ushort)property);

            if (result != null)
                result.SetProperties(pobj);
            else
            {
                var entity = new RealmPropertiesBool { RealmId = realm.Id, Type = (ushort)property };
                entity.SetProperties(pobj);
                realm.RealmPropertiesBool.Add(entity);
            }
        }

        private static void SetProperty_Complex(Realm realm, RealmPropertyInt property, RealmPropertyJsonModel pobj)
        {
            var result = realm.RealmPropertiesInt.FirstOrDefault(x => x.Type == (ushort)property);

            if (result != null)
                result.SetProperties(pobj);
            else
            {
                var entity = new RealmPropertiesInt { RealmId = realm.Id, Type = (ushort)property };
                entity.SetProperties(pobj);
                realm.RealmPropertiesInt.Add(entity);
            }
        }

        private static void SetProperty_Complex(Realm realm, RealmPropertyString property, RealmPropertyJsonModel pobj)
        {
            var result = realm.RealmPropertiesString.FirstOrDefault(x => x.Type == (ushort)property);

            if (result != null)
                result.SetProperties(pobj);
            else
            {
                var entity = new RealmPropertiesString { RealmId = realm.Id, Type = (ushort)property };
                entity.SetProperties(pobj);
                realm.RealmPropertiesString.Add(entity);
            }
        }

        private static void SetProperty_Complex(Realm realm, RealmPropertyFloat property, RealmPropertyJsonModel pobj)
        {
            var result = realm.RealmPropertiesFloat.FirstOrDefault(x => x.Type == (ushort)property);

            if (result != null)
                result.SetProperties(pobj);
            else
            {
                var entity = new RealmPropertiesFloat { RealmId = realm.Id, Type = (ushort)property };
                entity.SetProperties(pobj);
                realm.RealmPropertiesFloat.Add(entity);
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
