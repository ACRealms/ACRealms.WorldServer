using ACRealms.RealmProps;
using ACRealms.RealmProps.Underlying;
using ACRealms.Rulesets.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ACRealms.Rulesets.DBOld
{
    internal sealed partial class RealmPropertiesInt64 : RealmPropertiesBase
    {
        public long? Value { get; set; }
        public long? RandomLowRange { get; set; }
        public long? RandomHighRange { get; set; }
        public byte RandomType { get; set; }
        public byte CompositionType { get; set; }

        static Type EnumType = typeof(RealmPropertyInt64);
        public override TemplatedRealmProperty<long> ConvertRealmProperty<TVal>(RealmPropertyGroupOptions<TVal> group, RealmPropertyScopeOptions scope)
        {
            RealmPropertyOptions<long> prop;
            if (Value.HasValue)
                prop = new RealmPropertyOptions<long>(group, Value.Value, Locked, Probability, EnumType, Type, scope);
            else
            {
                var @default = group.HardDefaultValue;
                prop = new MinMaxRangedRealmPropertyOptions<long>(group, Unsafe.As<TVal, long>(ref @default), CompositionType, RandomType, RandomLowRange.Value, RandomHighRange.Value, Locked, Probability, EnumType, Type, scope);
            }
            return new TemplatedRealmProperty<long>(RulesetCompilationContext.DefaultShared, Type, prop);
        }

        public void SetProperties(RealmPropertyJsonModel model)
        {
            if (model.value != null)
                this.Value = long.Parse(model.value);
            if (model.low != null)
            {
                this.RandomLowRange = long.Parse(model.low);
                this.RandomHighRange = long.Parse(model.high);
                if (RandomLowRange > RandomHighRange)
                    throw new Exception("high must be > low");
                if (!model.reroll.HasValue)
                    model.reroll = RealmPropertyRerollType.landblock;
            }
            this.Locked = model.locked ?? false;
            this.Probability = model.probability;
            this.RandomType = (byte)(model.reroll ?? RealmPropertyRerollType.never);
            this.CompositionType = (byte)model.compose;
        }
    }
}
