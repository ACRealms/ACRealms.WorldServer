using System.ComponentModel;

namespace ACE.Entity.Enum.Properties
{
    public enum RealmPropertyFloat : ushort
    {
        [RealmPropertyFloat(0f, 0f, 0f)]
        Undef                          = 0,
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
