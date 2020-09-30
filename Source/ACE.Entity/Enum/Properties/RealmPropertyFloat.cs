using System.ComponentModel;

namespace ACE.Entity.Enum.Properties
{
    public enum RealmPropertyFloat : ushort
    {
        [RealmPropertyFloat(0f, 0f, 0f)]
        Undef                          = 0,

        [RealmPropertyFloat(1f, 0.1f, 5f)]
        SpellCasting_MoveToState_UpdatePosition_Threshold = 1,

        [RealmPropertyFloat(5f, 1f, 360f)]
        Spellcasting_Max_Angle = 2
    }

    public static class RealmPropertyFloatExtensions
    {
        public static string GetDescription(this RealmPropertyFloat prop)
        {
            var description = prop.GetAttributeOfType<DescriptionAttribute>();
            return description?.Description ?? prop.ToString();
        }
    }
}
