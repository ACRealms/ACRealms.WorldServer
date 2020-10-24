using System;
using System.ComponentModel;
using System.Globalization;

namespace ACE.Entity.Enum.Properties
{
    public enum RealmPropertyInt : ushort
    {
        [RealmPropertyInt(0,0,0)]
        Undef                                    = 0,

        [RealmPropertyInt(0, 0, 0xFFFF)]
        RulesetStampVendorCategory,

        [RealmPropertyInt(0, int.MinValue, int.MaxValue)]
        CreatureStrengthAdded,

        [RealmPropertyInt(0, int.MinValue, int.MaxValue)]
        CreatureEnduranceAdded,

        [RealmPropertyInt(0, int.MinValue, int.MaxValue)]
        CreatureCoordinationAdded,

        [RealmPropertyInt(0, int.MinValue, int.MaxValue)]
        CreatureQuicknessAdded,

        [RealmPropertyInt(0, int.MinValue, int.MaxValue)]
        CreatureFocusAdded,

        [RealmPropertyInt(0, int.MinValue, int.MaxValue)]
        CreatureSelfAdded,

        /*Below not implemented*/
        [RealmPropertyInt(0, -100000, 100000)]
        PlayerDamageMeleeAdded,

        [RealmPropertyInt(0, -100000, 100000)]
        PlayerDamageMagicAdded,

        [RealmPropertyInt(0, -100000, 100000)]
        PlayerDamageMissileAdded,

        [RealmPropertyInt(0, -100000, 100000)]
        HealingKitHealAdded,

        [RealmPropertyInt(0, -100000, 100000)]
        FoodRestoreAmountAdded,
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
