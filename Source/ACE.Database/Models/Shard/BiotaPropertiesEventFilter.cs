using System;
using System.Collections.Generic;

#nullable disable

namespace ACE.Database.Models.Shard
{
    public partial class BiotaPropertiesEventFilter
    {
        public ulong ObjectId { get; set; }
        public int Event { get; set; }

        public virtual Biota Object { get; set; }
    }
}
