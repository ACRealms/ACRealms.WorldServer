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

        public override TemplatedRealmProperty<bool> ConvertRealmProperty<TVal>(RealmPropertyGroupOptions<TVal> group, RealmPropertyScopeOptions scope)
        {
            var prop = new RealmPropertyOptions<bool>(group, Value, Locked, Probability, EnumType, Type, scope);
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
