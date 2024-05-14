using ACE.Database.Adapter;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public sealed partial class RealmPropertiesInt : RealmPropertiesBase
    {
        public int? Value { get; set; }
        public int? RandomLowRange { get; set; }
        public int? RandomHighRange { get; set; }
        public byte RandomType { get; set; }
        public byte CompositionType { get; set; }

        public override AppliedRealmProperty<int> ConvertRealmProperty()
        {
            var @enum = (RealmPropertyInt)Type;
            var att = RealmConverter.PropertyDefinitionsInt[@enum];
            var prop = new RealmPropertyOptions<int>(@enum.ToString());
            if (Value.HasValue)
                prop.SeedPropertiesStatic(Value.Value, att.DefaultValue, CompositionType, Locked, Probability);
            else
                prop.SeedPropertiesRandomized(att.DefaultValue, CompositionType, RandomType, RandomLowRange.Value, RandomHighRange.Value, Locked, Probability);
            return new AppliedRealmProperty<int>(Type, prop, null);
        }
    }
}
