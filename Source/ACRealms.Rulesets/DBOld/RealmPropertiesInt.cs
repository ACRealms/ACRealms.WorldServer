using ACRealms.RealmProps;
using ACRealms.RealmProps.Underlying;
using ACRealms.Rulesets.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ACRealms.Rulesets.DBOld
{
    internal sealed partial class RealmPropertiesInt : RealmPropertiesBase
    {
        public int? Value { get; set; }
        public int? RandomLowRange { get; set; }
        public int? RandomHighRange { get; set; }
        public byte RandomType { get; set; }
        public byte CompositionType { get; set; }

        static Type EnumType = typeof(RealmPropertyInt);
        public override TemplatedRealmProperty<int> ConvertRealmProperty<TVal>(RealmPropertyGroupOptions<TVal> group, RealmPropertyScopeOptions scope)
        {
            RealmPropertyOptions<int> prop;
            if (Value.HasValue)
                prop = new RealmPropertyOptions<int>(group, Value.Value, Locked, Probability, EnumType, Type, scope);
            else
            {
                var @default = group.HardDefaultValue;
                prop = new MinMaxRangedRealmPropertyOptions<int>(group, Unsafe.As<TVal, int>(ref @default), CompositionType, RandomType, RandomLowRange.Value, RandomHighRange.Value, Locked, Probability, EnumType, Type, scope);
            }
            return new TemplatedRealmProperty<int>(RulesetCompilationContext.DefaultShared, Type, prop);
        }

        public void SetProperties(RealmPropertyJsonModel model)
        {
            if (model.value != null)
                this.Value = int.Parse(model.value);
            if (model.low != null)
            {
                this.RandomLowRange = int.Parse(model.low);
                this.RandomHighRange = int.Parse(model.high);
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
