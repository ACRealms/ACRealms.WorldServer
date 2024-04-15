using System;
using System.Collections.Generic;

#nullable disable

namespace ACE.Database.Models.Shard
{
    public partial class BiotaPropertiesAllegiance
    {
        public ulong AllegianceId { get; set; }
        public ulong CharacterId { get; set; }
        public bool Banned { get; set; }
        public bool ApprovedVassal { get; set; }

        public virtual Biota Allegiance { get; set; }
        public virtual Character Character { get; set; }
    }
}
