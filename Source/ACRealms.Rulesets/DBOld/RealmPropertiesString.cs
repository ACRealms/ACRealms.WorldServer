using ACE.Entity.ACRealms;
using ACE.Entity.Models;
using ACRealms.RealmProps;
using ACRealms.RealmProps.Underlying;
using System;
using System.Collections.Generic;

namespace ACRealms.Rulesets.DBOld
{
    public sealed partial class RealmPropertiesString : RealmPropertiesBase
    {
        public string Value { get; set; }

        static Type EnumType = typeof(RealmPropertyString);
        public override AppliedRealmProperty<string> ConvertRealmProperty()
        {
            var @enum = (RealmPropertyString)Type;
            var proto = RealmPropertyPrototypes.String[@enum];
            var att = proto.PrimaryAttribute;
            var prop = new RealmPropertyOptions<string>(proto, @enum.ToString(), Realm.Name, att.DefaultValue, Value, Locked, Probability, EnumType, Type);
            return new AppliedRealmProperty<string>(RulesetCompilationContext.DefaultShared, Type, prop);
        }

        public void SetProperties(RealmPropertyJsonModel model)
        {
            model.ValidateValuePresent();
            this.Value = model.value;
            this.Locked = model.locked ?? false;
            this.Probability = model.probability;
        }
    }
}
