using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public partial class RealmPropertiesInt
    {
        public ushort RealmId { get; set; }
        public ushort Type { get; set; }
        public int Value { get; set; }

        public virtual Realm Realm { get; set; }
    }
}
