﻿using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public partial class RealmPropertiesInt64
    {
        public ushort RealmId { get; set; }
        public ushort Type { get; set; }
        public long Value { get; set; }

        public virtual Realm Realm { get; set; }
    }
}
