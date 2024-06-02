using ACE.Database.Models.Shard;
using ACE.Entity.Enum.Properties;
using ACE.Server.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.WorldObjects
{
    public static class IPlayerACRealmsLocationExtensions
    {
        public static ushort GetDefaultShortInstanceID(this IPlayer player)
        {
            if (player.GetProperty(PropertyBool.AttemptUniqueInstanceID) == true)
                return (ushort)((player.Guid.Full % 0xFFFE) + 1);
            return 0;
        }
    }

    partial class Player
    {
        public ushort DefaultShortInstanceID => IPlayerACRealmsLocationExtensions.GetDefaultShortInstanceID(this);
    }
}
