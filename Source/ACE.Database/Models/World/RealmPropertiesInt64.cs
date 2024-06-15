using ACE.Database.Adapter;
using ACE.Entity.ACRealms;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Enum.RealmProperties;
using ACE.Entity.Models;
using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public sealed partial class RealmPropertiesInt64 : RealmPropertiesBase
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
    }
}
