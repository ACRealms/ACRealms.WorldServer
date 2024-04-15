using System;
using System.Collections.Generic;

#nullable disable

namespace ACE.Database.Models.Shard
{
    public partial class BiotaPropertiesIID
    {
        public ulong ObjectId { get; set; }
        public ushort Type { get; set; }
        public ulong Value { get; set; }

        public virtual Biota Object { get; set; }
    }
}
