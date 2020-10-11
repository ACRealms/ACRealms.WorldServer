using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public partial class RealmPropertiesInt64
    {
        public ushort RealmId { get; set; }
        public ushort Type { get; set; }
        public long? Value { get; set; }
        public bool Locked { get; set; }
        public double? Probability { get; set; }
        public long? RandomLowRange { get; set; }
        public long? RandomHighRange { get; set; }
        public byte RandomType { get; set; }
        public byte CompositionType { get; set; }

        public virtual Realm Realm { get; set; }
    }
}
