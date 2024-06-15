using ACE.Database.Adapter;
using ACE.Entity.ACRealms;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Enum.RealmProperties;
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
            
            var proto = RealmPropertyPrototypes.Bool[@enum];
            var att = proto.PrimaryAttribute;
            var prop = new RealmPropertyOptions<bool>(proto, @enum.ToString(), Realm.Name, att.DefaultValue, Value, Locked, Probability, EnumType, Type);
            return new AppliedRealmProperty<bool>(RulesetCompilationContext.DefaultShared, Type, prop);
        }
    }
}
