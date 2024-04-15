using System;
using System.Collections.Generic;

#nullable disable

namespace ACE.Database.Models.Shard
{
    public partial class CharacterPropertiesShortcutBar
    {
        public ulong CharacterId { get; set; }
        public uint ShortcutBarIndex { get; set; }
        public ulong ShortcutObjectId { get; set; }

        public virtual Character Character { get; set; }
    }
}
