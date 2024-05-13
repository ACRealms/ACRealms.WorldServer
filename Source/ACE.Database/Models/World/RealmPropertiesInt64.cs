using ACE.Database.Adapter;
using ACE.Entity.Enum.Properties;
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

        public override AppliedRealmProperty<long> ConvertRealmProperty()
        {
            var @enum = (RealmPropertyInt64)Type;
            var att = RealmConverter.PropertyDefinitionsInt64[@enum];
            var prop = new RealmPropertyOptions<long>(@enum.ToString());
            if (Value.HasValue)
                prop.SeedPropertiesStatic(Value.Value, att.DefaultValue, CompositionType, Locked, Probability);
            else
                prop.SeedPropertiesRandomized(att.DefaultValue, CompositionType, RandomType, RandomLowRange.Value, RandomHighRange.Value, Locked, Probability);
            return new AppliedRealmProperty<long>(Type, prop);
        }
    }
}
