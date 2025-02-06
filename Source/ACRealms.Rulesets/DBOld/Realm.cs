using ACRealms.RealmProps.Underlying;
using ACRealms.Rulesets.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ACRealms.Rulesets.DBOld
{
    // This was previously a EntityFramework entity class, but is no longer part of the world db
    internal partial class Realm
    {
        // This is set by ACE.Server currently, so it needs public setter
        public ushort? ParentRealmId { get; set; }
        public ushort Id { get; private set; }
        public RealmType Type { get; internal set; }
        public string Name { get; internal set; }
        public string ParentRealmName { get; internal set; }
        public Dictionary<ushort, Realm> Descendants = [];

        internal ushort? PropertyCountRandomized { get; set; }

        internal virtual IList<RealmPropertiesBool> RealmPropertiesBool { get; } = [];
        internal virtual IList<RealmPropertiesFloat> RealmPropertiesFloat { get; } = [];
        internal virtual IList<RealmPropertiesInt> RealmPropertiesInt { get; } = [];
        internal virtual IList<RealmPropertiesInt64> RealmPropertiesInt64 { get; } = [];
        internal virtual IList<RealmPropertiesString> RealmPropertiesString { get; } = [];
        internal virtual IList<RealmRulesetLinks> RealmRulesetLinksLinkedRealm { get; } = [];
        internal virtual IList<RealmRulesetLinks> RealmRulesetLinksRealm { get; } = [];

        public override string ToString() => $"{Name} ({Id})";

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

        public void SetPropertyByName_Complex(string propertyName, RealmPropertyJsonModel pobj)
        {
            pobj.ValidateAll();

            if (Enum.TryParse<RealmPropertyBool>(propertyName, out var boolprop))
                SetProperty_Complex(boolprop, pobj, RealmPropertiesBool);
            else if (Enum.TryParse<RealmPropertyInt>(propertyName, out var intprop))
                SetProperty_Complex(intprop, pobj, RealmPropertiesInt);
            else if (Enum.TryParse<RealmPropertyString>(propertyName, out var stringprop))
                SetProperty_Complex(stringprop, pobj, RealmPropertiesString);
            else if (Enum.TryParse<RealmPropertyFloat>(propertyName, out var floatprop))
                SetProperty_Complex(floatprop, pobj, RealmPropertiesFloat);
            else if (Enum.TryParse<RealmPropertyInt64>(propertyName, out var longprop))
                SetProperty_Complex(longprop, pobj, RealmPropertiesInt64);
            else
                throw new InvalidDataException("Realm property not found: " + propertyName);
        }

        private void SetProperty_Complex<TEnum, TPropEntity>(TEnum property, RealmPropertyJsonModel pobj, IList<TPropEntity> props)
            where TEnum : Enum
            where TPropEntity : RealmPropertiesBase, new()
        {
            var entity = new TPropEntity { RealmId = Id, Type = Unsafe.As<TEnum, int>(ref property), Realm = this, RawScope = pobj.scope ?? [] };
            entity.SetProperties(pobj);
            props.Add(entity);
        }

        internal void SetPropertyByName(string propertyName, JToken value)
        {
            if (Enum.TryParse<RealmPropertyBool>(propertyName, out var boolprop))
                SetProperty<RealmPropertyBool, RealmPropertiesBool, bool, bool> (boolprop, ((bool)value), RealmPropertiesBool);
            else if (Enum.TryParse<RealmPropertyInt>(propertyName, out var intprop))
                SetProperty<RealmPropertyInt, RealmPropertiesInt, int, int?>(intprop, (int)value, RealmPropertiesInt);
            else if (Enum.TryParse<RealmPropertyString>(propertyName, out var stringprop))
                SetProperty<RealmPropertyString, RealmPropertiesString, string, string>(stringprop, (string)value, RealmPropertiesString);
            else if (Enum.TryParse<RealmPropertyFloat>(propertyName, out var floatprop))
                SetProperty<RealmPropertyFloat, RealmPropertiesFloat, double, double?>(floatprop, (double)value, RealmPropertiesFloat);
            else if (Enum.TryParse<RealmPropertyInt64>(propertyName, out var longprop))
                SetProperty<RealmPropertyInt64, RealmPropertiesInt64, long, long?>(longprop, (long)value, RealmPropertiesInt64);
            else
                throw new Exception("Realm property not found: " + propertyName);
        }

        internal void SetProperty<TEnum, TPropEntity, TPrim, TVal>(TEnum property, TVal value, IList<TPropEntity> props)
            where TEnum : Enum
            where TPropEntity : RealmPropertiesBaseWithBoxableValue<TEnum, TPrim, TVal>, new()
            where TPrim : IComparable<TPrim>, IEquatable<TPrim>, IParsable<TPrim>
        {
            var entity = new TPropEntity { RealmId = Id, Type = Unsafe.As<TEnum, int>(ref property), Value = value, Realm = this, RawScope = [] };
            props.Add(entity);
        }
    }
}
