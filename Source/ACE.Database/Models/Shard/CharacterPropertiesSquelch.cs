using System;
using System.Collections.Generic;

#nullable disable

namespace ACE.Database.Models.Shard
{
    public partial class CharacterPropertiesSquelch
    {
        public ulong CharacterId { get; set; }
        public ulong SquelchCharacterId { get; set; }
        public uint SquelchAccountId { get; set; }
        public uint Type { get; set; }

        public virtual Character Character { get; set; }
    }
}
