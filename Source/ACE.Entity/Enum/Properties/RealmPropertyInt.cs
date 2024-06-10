using ACE.Entity.Enum.RealmProperties;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace ACE.Entity.Enum.Properties
{
    public enum RealmPropertyInt : ushort
    {
        [RealmPropertyInt(defaultValue: 0, minValue: 0, maxValue: 0)]
        Undef                                    = 0,

        [Obsolete("This will be changed to a string")]
        [Description("Vendor weenies with a matching PropertyInt.RulesetStampVendorType will include this ruleset as a stamp for sale, to allow players to craft these rulesets for ephemeral instances")]
        [RealmPropertyInt(0, 0, 0xFFFF)]
        RulesetStampVendorCategory = 1,


        [Description("All creatures will have this value added to their strength attribute")]
        [RealmPropertyInt(0, -10000000, 10000000)]
        CreatureStrengthAdded = 2,

        [Description("All creatures will have this value added to their endurance attribute")]
        [RealmPropertyInt(0, -10000000, 10000000)]
        CreatureEnduranceAdded = 3,

        [Description("All creatures will have this value added to their coordination attribute")]
        [RealmPropertyInt(0, -10000000, 10000000)]
        CreatureCoordinationAdded = 4,

        [Description("All creatures will have this value added to their quickness attribute")]
        [RealmPropertyInt(0, -10000000, 10000000)]
        CreatureQuicknessAdded = 5,

        [Description("All creatures will have this value added to their focus attribute")]
        [RealmPropertyInt(0, -10000000, 10000000)]
        CreatureFocusAdded = 6,

        [Description("All creatures will have this value added to their self attribute")]
        [RealmPropertyInt(0, -10000000, 10000000)]
        CreatureSelfAdded = 7,

        /*Below not implemented*/
        [Description("NOT IMPLEMENTED")]
        [RealmPropertyInt(0, -100000, 100000)]
        PlayerDamageMeleeAdded = 8,

        [Description("NOT IMPLEMENTED")]
        [RealmPropertyInt(0, -100000, 100000)]
        PlayerDamageMagicAdded = 9,

        [Description("NOT IMPLEMENTED")]
        [RealmPropertyInt(0, -100000, 100000)]
        PlayerDamageMissileAdded = 10,

        [Description("NOT IMPLEMENTED")]
        [RealmPropertyInt(0, -100000, 100000)]
        HealingKitHealAdded = 11,

        [Description("NOT IMPLEMENTED")]
        [RealmPropertyInt(0, -100000, 100000)]
        FoodRestoreAmountAdded = 12,

        [Description("When recalling, determines which mode to use to select the realm. Defaults to 1 for PlayerInstanceSelectMode.HomeRealm")]
        [RealmPropertyInt((int)PlayerInstanceSelectMode.HomeRealm, 1, (ushort)PlayerInstanceSelectMode.reserved - 1)]
        RecallInstanceSelectMode = 13,

        [Description("When using a portal, determines which mode to use to select the realm. Defaults to 1 for PlayerInstanceSelectMode.HomeRealm")]
        [RealmPropertyInt((int)PlayerInstanceSelectMode.HomeRealm, 1, (ushort)PlayerInstanceSelectMode.reserved - 1)]
        PortalInstanceSelectMode = 14,

        /// <summary>
        /// Landblocks which have been inactive for this many minutes will be unloaded
        /// </summary>
        [Description("Landblocks which have been inactive for this many minutes will be unloaded")]
        [RealmPropertyInt(5, 1, 1440)]
        LandblockUnloadInterval = 15,
    }

    public static class RealmPropertyIntExtensions
    {
        public static string GetDescription(this RealmPropertyInt prop)
        {
            var description = prop.GetAttributeOfType<DescriptionAttribute>();
            return description?.Description ?? prop.ToString();
        }
    }
}
