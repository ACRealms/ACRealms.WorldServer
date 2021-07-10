using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public partial class RealmPropertiesBool
    {
        public ushort RealmId { get; set; }
        public ushort Type { get; set; }
        public bool Value { get; set; }
        public bool Locked { get; set; }
        public double? Probability { get; set; }

        public virtual Realm Realm { get; set; }
    }
}
