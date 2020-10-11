using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public partial class RealmPropertiesFloat
    {
        public ushort RealmId { get; set; }
        public ushort Type { get; set; }
        public double? Value { get; set; }
        public bool Locked { get; set; }
        public double? RandomLowRange { get; set; }
        public double? RandomHighRange { get; set; }
        public double? Probability { get; set; }
        public byte RandomType { get; set; }
        public byte CompositionType { get; set; }

        public virtual Realm Realm { get; set; }
    }
}
