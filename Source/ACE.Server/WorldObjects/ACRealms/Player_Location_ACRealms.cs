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
    }

    partial class Player
    {
        /// <summary>
        /// Returns true if the player is located in the player's home realm
        /// </summary>
        public bool IsInHomeRealm => CurrentLandblock.WorldRealmID == HomeRealm;

        /// <summary>
        /// Returns true if the player is in the primary instance for the current realm
        /// </summary>
        public bool IsInPrimaryInstance => CurrentLandblock.IsPrimaryForWorldRealm;
        internal ushort AssignedClassicalShortInstanceID => RealmRuleset.GetClassicalShortInstanceID(this, true);
        public bool IsInOwnedClassicalInstance
        {
            get
            {
                if (!RealmRuleset.ClassicalInstancesActivated(this, Location.AsLocalPosition()))
                    return false;
                return CurrentLandblock.ShortInstanceID == AssignedClassicalShortInstanceID;
            }
        }

        public bool IsInEphemeralRealm => CurrentLandblock.IsEphemeral;
    }
}
