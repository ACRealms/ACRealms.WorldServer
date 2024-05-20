using ACE.Database.Adapter;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public sealed partial class RealmPropertiesString : RealmPropertiesBase
    {
        public string Value { get; set; }

        public override AppliedRealmProperty<string> ConvertRealmProperty()
        {
            var @enum = (RealmPropertyString)Type;
            var att = RealmConverter.PropertyDefinitionsString[@enum];
            var prop = new RealmPropertyOptions<string>(@enum.ToString(), Realm.Name, Value, att.DefaultValue, Locked, Probability);
            return new AppliedRealmProperty<string>(Type, prop, null);
        }
    }
}
