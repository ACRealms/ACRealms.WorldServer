using ACRealms.RealmProps;
using ACRealms.RealmProps.Underlying;
using System;
using System.Collections.Generic;

namespace ACRealms.Rulesets.DBOld
{
    internal sealed partial class RealmPropertiesString : RealmPropertiesBase
    {
        public string Value { get; set; }

        static Type EnumType = typeof(RealmPropertyString);
        public override TemplatedRealmProperty<string> ConvertRealmProperty<TVal>(RealmPropertyGroupOptions<TVal> group, RealmPropertyScopeOptions scope)
        {
            var prop = new RealmPropertyOptions<string>(group, Value, Locked, Probability, EnumType, Type, scope);
            return new TemplatedRealmProperty<string>(RulesetCompilationContext.DefaultShared, Type, prop);
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
