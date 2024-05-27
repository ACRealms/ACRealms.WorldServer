using ACE.Database.Adapter;
using ACE.Entity.ACRealms;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACE.Database.Models.World
{
    public sealed partial class RealmPropertiesBool : RealmPropertiesBase
    {
        public bool Value { get; set; }

        static Type EnumType = typeof(RealmPropertyBool);

        public override AppliedRealmProperty<bool> ConvertRealmProperty()
        {
            var @enum = (RealmPropertyBool)Type;
            
            var att = RealmConverter.PropertyDefinitionsBool[@enum];
            var prop = new RealmPropertyOptions<bool>(@enum.ToString(), Realm.Name, att.DefaultValue, Value, Locked, Probability, EnumType, Type);
            return new AppliedRealmProperty<bool>(RulesetCompilationContext.DefaultShared, Type, prop);
        }
    }
}
