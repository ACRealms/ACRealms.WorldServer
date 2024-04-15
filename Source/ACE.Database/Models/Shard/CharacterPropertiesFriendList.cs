using System;
using System.Collections.Generic;

#nullable disable

namespace ACE.Database.Models.Shard
{
    public partial class CharacterPropertiesFriendList
    {
        public ulong CharacterId { get; set; }
        public ulong FriendId { get; set; }

        public virtual Character Character { get; set; }
    }
}
