using System;
using System.Collections.Generic;
using System.Text;

namespace ACRealms.Rulesets.Enums // Belongs in this namespace but must be used in this project
{
    /// <summary> Determines the frequency of the realm property randomization </summary>
    public enum RealmPropertyRerollType : byte
    {
        /// <summary> Never reroll (use the default fallback value) </summary>
        never = 1,

        /// <summary> Reroll each time the property is accessed by game logic </summary>
        always = 2,

        /// <summary> Reroll once during landblock load </summary>
        landblock = 3,

        /// <summary> Not used </summary>
        manual = 4,
    }
}
