using ACE.Database.Adapter;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public sealed partial class RealmPropertiesFloat : RealmPropertiesBase
    {
        public double? Value { get; set; }
        public double? RandomLowRange { get; set; }
        public double? RandomHighRange { get; set; }
        public byte RandomType { get; set; }
        public byte CompositionType { get; set; }

        public override AppliedRealmProperty<double> ConvertRealmProperty()
        {
            var @enum = (RealmPropertyFloat)Type;
            var att = RealmConverter.PropertyDefinitionsFloat[@enum];
            var prop = new RealmPropertyOptions<double>(@enum.ToString());
            if (Value.HasValue)
                prop.SeedPropertiesStatic(Value.Value, att.DefaultValue, CompositionType, Locked, Probability);
            else
                prop.SeedPropertiesRandomized(att.DefaultValue, CompositionType, RandomType, RandomLowRange.Value, RandomHighRange.Value, Locked, Probability);
            return new AppliedRealmProperty<double>(Type, prop, null);
        }
    }
}
