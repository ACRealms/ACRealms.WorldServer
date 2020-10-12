using System;
using System.ComponentModel;
using System.Globalization;

namespace ACE.Entity.Enum.Properties
{
    public enum RealmPropertyInt : ushort
    {
        [RealmPropertyInt(0,0,0)]
        Undef                                    = 0,

        [RealmPropertyInt(0, int.MinValue, int.MaxValue)]
        CreatureStrengthAdded = 1,

        [RealmPropertyInt(0, int.MinValue, int.MaxValue)]
        CreatureEnduranceAdded = 2,

        [RealmPropertyInt(0, int.MinValue, int.MaxValue)]
        CreatureCoordinationAdded = 3,

        [RealmPropertyInt(0, int.MinValue, int.MaxValue)]
        CreatureQuicknessAdded = 4,

        [RealmPropertyInt(0, int.MinValue, int.MaxValue)]
        CreatureFocusAdded = 5,

        [RealmPropertyInt(0, int.MinValue, int.MaxValue)]
        CreatureSelfAdded = 6,
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
