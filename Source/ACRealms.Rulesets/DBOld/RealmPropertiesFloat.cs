using ACRealms.RealmProps;
using ACRealms.RealmProps.Underlying;
using ACRealms.Rulesets.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace ACRealms.Rulesets.DBOld
{
    internal sealed partial class RealmPropertiesFloat : RealmPropertiesBase
    {
        public double? Value { get; set; }
        public double? RandomLowRange { get; set; }
        public double? RandomHighRange { get; set; }
        public byte RandomType { get; set; }
        public byte CompositionType { get; set; }

        static Type EnumType = typeof(RealmPropertyFloat);
        public override TemplatedRealmProperty<double> ConvertRealmProperty<TVal>(RealmPropertyGroupOptions<TVal> group, RealmPropertyScopeOptions scope)
        {
            RealmPropertyOptions<double> prop;
            if (Value.HasValue)
                prop = new RealmPropertyOptions<double>(group, Value.Value, Locked, Probability, EnumType, Type, scope);
            else
            {
                var @default = group.HardDefaultValue;
                prop = new MinMaxRangedRealmPropertyOptions<double>(group, Unsafe.As<TVal, double>(ref @default), CompositionType, RandomType, RandomLowRange.Value, RandomHighRange.Value, Locked, Probability, EnumType, Type, scope);
            }
            return new TemplatedRealmProperty<double>(RulesetCompilationContext.DefaultShared, Type, prop);
        }

        public void SetProperties(RealmPropertyJsonModel model)
        {
            if (model.value != null)
                this.Value = double.Parse(model.value);
            if (model.low != null)
            {
                this.RandomLowRange = double.Parse(model.low);
                this.RandomHighRange = double.Parse(model.high);
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
