using ACE.Entity.ACRealms;
using ACE.Entity.Models;
using ACRealms.RealmProps;
using ACRealms.RealmProps.Underlying;
using ACRealms.Rulesets.Enums;
using System;
using System.Collections.Generic;

namespace ACRealms.Rulesets.DBOld
{
    public sealed partial class RealmPropertiesInt : RealmPropertiesBase
    {
        public int? Value { get; set; }
        public int? RandomLowRange { get; set; }
        public int? RandomHighRange { get; set; }
        public byte RandomType { get; set; }
        public byte CompositionType { get; set; }

        static Type EnumType = typeof(RealmPropertyInt);
        public override AppliedRealmProperty<int> ConvertRealmProperty()
        {
            var @enum = (RealmPropertyInt)Type;
            var proto = RealmPropertyPrototypes.Int[@enum];
            var att = proto.PrimaryAttribute;
            RealmPropertyOptions<int> prop;
            if (Value.HasValue)
                prop = new RealmPropertyOptions<int>(proto, @enum.ToString(), Realm.Name, att.DefaultValue, Value.Value, Locked, Probability, EnumType, Type);
            else
                prop = new MinMaxRangedRealmPropertyOptions<int>(proto, @enum.ToString(), Realm.Name, att.DefaultValue, CompositionType, RandomType, RandomLowRange.Value, RandomHighRange.Value, Locked, Probability, EnumType, Type);
            return new AppliedRealmProperty<int>(RulesetCompilationContext.DefaultShared, Type, prop);
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
