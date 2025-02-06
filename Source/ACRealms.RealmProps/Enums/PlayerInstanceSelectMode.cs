using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.RealmProps.Enums
{

    /// <summary>
    /// Determines the conversion function from a relative position to an instance position, contextually for a player
    /// </summary>
    public enum PlayerInstanceSelectMode : int
    {
        /// <summary> Undefined </summary>
        Undefined = 0,

        /// <summary> The current instance the player was located on </summary>
        Same,

        /// <summary>
        /// The current instance the player was located on, but only if teleporting within the same landblock.
        /// For example, teleport traps that move a player to another location in the same dungeon will need to use this mode
        /// </summary>
        SameIfSameLandblock,

        /// <summary> Based on the player's home realm </summary>
        HomeRealm,

        /// <summary> The player's personal realm (Support for this is minimal right now) </summary>
        PersonalRealm,

        /// <summary> The current realm's default instance ID </summary>
        RealmDefaultInstanceID,




        /// <summary> Reserved (do not use) </summary>
        reserved         // Do not add entries below this item
    }
}
