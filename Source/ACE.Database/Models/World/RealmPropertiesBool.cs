using ACE.Database.Adapter;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public sealed partial class RealmPropertiesBool : RealmPropertiesBase
    {
        public bool Value { get; set; }

        public override AppliedRealmProperty<bool> ConvertRealmProperty()
        {
            var @enum = (RealmPropertyBool)Type;
            
            var att = RealmConverter.PropertyDefinitionsBool[@enum];
            var prop = new RealmPropertyOptions<bool>(@enum.ToString(), Value, att.DefaultValue, Locked, Probability);
            return new AppliedRealmProperty<bool>(Type, prop, null);
        }
    }
}
