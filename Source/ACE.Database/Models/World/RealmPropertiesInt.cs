using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public partial class RealmPropertiesInt
    {
        public ushort RealmId { get; set; }
        public ushort Type { get; set; }
        public int? Value { get; set; }
        public bool Locked { get; set; }
        public int? RandomLowRange { get; set; }
        public int? RandomHighRange { get; set; }
        public double? Probability { get; set; }
        public byte RandomType { get; set; }
        public byte CompositionType { get; set; }

        public virtual Realm Realm { get; set; }
    }
}
