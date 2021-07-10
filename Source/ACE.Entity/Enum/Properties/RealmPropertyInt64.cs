using System.ComponentModel;

namespace ACE.Entity.Enum.Properties
{
    public enum RealmPropertyInt64 : ushort
    {
        [RealmPropertyInt64(0,0,0)]
        Undef               = 0,
    }

    public static class RealmPropertyInt64Extensions
    {
        public static string GetDescription(this RealmPropertyInt64 prop)
        {
            var description = prop.GetAttributeOfType<DescriptionAttribute>();
            return description?.Description ?? prop.ToString();
        }
    }
}
