﻿using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public partial class RealmPropertiesString
    {
        public ushort RealmId { get; set; }
        public ushort Type { get; set; }
        public string Value { get; set; }

        public virtual Realm Realm { get; set; }
    }
}
