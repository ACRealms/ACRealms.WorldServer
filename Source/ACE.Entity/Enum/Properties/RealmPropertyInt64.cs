using System.ComponentModel;
using RealmPropertyInt64Attribute = ACE.Entity.Enum.Properties.RealmPropertyPrimaryMinMaxAttribute<long>;

namespace ACE.Entity.Enum.Properties
{

    #pragma warning disable IDE0001
    [RequiresPrimaryAttribute<RealmPropertyPrimaryAttribute<long>, long>]
    #pragma warning restore IDE0001
    public enum RealmPropertyInt64 : ushort
    {
        [RealmPropertyInt64(defaultValue: 0, minValue: 0, maxValue: 0)]
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
