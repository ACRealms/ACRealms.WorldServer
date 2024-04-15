using System;
using System.Collections.Generic;

#nullable disable

namespace ACE.Database.Models.Shard
{
    public partial class HousePermission
    {
        public ulong HouseId { get; set; }
        public ulong PlayerGuid { get; set; }
        public bool Storage { get; set; }

        public virtual Biota House { get; set; }
    }
}
