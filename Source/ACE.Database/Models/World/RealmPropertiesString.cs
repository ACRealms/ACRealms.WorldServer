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
            var prop = new RealmPropertyOptions<string>(@enum.ToString());
            var att = RealmConverter.PropertyDefinitionsString[@enum];
            prop.SeedPropertiesStatic(Value, att.DefaultValue, (byte)RealmPropertyCompositionType.replace, Locked, Probability);
            return new AppliedRealmProperty<string>(Type, prop);
        }
    }
}
