using ACE.Entity.ACRealms;
using ACE.Entity.Models;
using ACRealms.RealmProps;
using ACRealms.RealmProps.Underlying;
using ACRealms.Rulesets.Enums;
using System;
using System.Collections.Generic;

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
        public override AppliedRealmProperty<double> ConvertRealmProperty()
        {
            var @enum = (RealmPropertyFloat)Type;
            var proto = RealmPropertyPrototypes.Float[@enum];
            var att = proto.PrimaryAttribute;
            RealmPropertyOptions<double> prop;
            if (Value.HasValue)
                prop = new RealmPropertyOptions<double>(proto, @enum.ToString(), Realm.Name, att.DefaultValue, Value.Value, Locked, Probability, EnumType, Type);
            else
                prop = new MinMaxRangedRealmPropertyOptions<double>(proto, @enum.ToString(), Realm.Name, att.DefaultValue, CompositionType, RandomType, RandomLowRange.Value, RandomHighRange.Value, Locked, Probability, EnumType, Type);
            return new AppliedRealmProperty<double>(RulesetCompilationContext.DefaultShared, Type, prop);
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
