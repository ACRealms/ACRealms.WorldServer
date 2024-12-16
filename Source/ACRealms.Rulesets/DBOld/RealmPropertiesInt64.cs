using ACE.Entity.ACRealms;
using ACE.Entity.Models;
using ACRealms.RealmProps;
using ACRealms.RealmProps.Underlying;
using ACRealms.Rulesets.Enums;
using System;
using System.Collections.Generic;

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
        public override AppliedRealmProperty<long> ConvertRealmProperty()
        {
            var @enum = (RealmPropertyInt64)Type;
            var proto = RealmPropertyPrototypes.Int64[@enum];
            var att = proto.PrimaryAttribute;
            RealmPropertyOptions<long> prop;
            if (Value.HasValue)
                prop = new RealmPropertyOptions<long>(proto, @enum.ToString(), Realm.Name, att.DefaultValue, Value.Value, Locked, Probability, EnumType, Type);
            else
                prop = new MinMaxRangedRealmPropertyOptions<long>(proto, @enum.ToString(), Realm.Name, att.DefaultValue, CompositionType, RandomType, RandomLowRange.Value, RandomHighRange.Value, Locked, Probability, EnumType, Type);
            return new AppliedRealmProperty<long>(RulesetCompilationContext.DefaultShared, Type, prop);
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
