using ACRealms.RealmProps;
using ACRealms.RealmProps.Underlying;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACRealms.Rulesets.DBOld
{
    internal sealed partial class RealmPropertiesBool : RealmPropertiesBase
    {
        public bool Value { get; set; }

        static Type EnumType = typeof(RealmPropertyBool);

        public override TemplatedRealmProperty<bool> ConvertRealmProperty(RealmPropertyGroupOptions group, RealmPropertyScopeOptions scope)
        {
            var @enum = (RealmPropertyBool)Type;
            
            var proto = RealmPropertyPrototypes.Bool[@enum];
            var att = proto.PrimaryAttribute;
            var prop = new RealmPropertyOptions<bool>(group, @enum.ToString(), Realm.Name, att.DefaultValue, Value, Locked, Probability, EnumType, Type, scope);
            return new TemplatedRealmProperty<bool>(RulesetCompilationContext.DefaultShared, Type, prop);
        }
        public void SetProperties(RealmPropertyJsonModel model)
        {
            model.ValidateValuePresent();
            this.Value = bool.Parse(model.value);
            this.Locked = model.locked ?? false;
            this.Probability = model.probability;
        }
    }
}
