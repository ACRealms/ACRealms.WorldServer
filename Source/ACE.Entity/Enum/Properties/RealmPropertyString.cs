using System.ComponentModel;

namespace ACE.Entity.Enum.Properties
{
    public enum RealmPropertyString : ushort
    {
        [RealmPropertyString(defaultValue: "")]
        Undef                           = 0,

        [Description("A description of the ruleset.")]
        [RealmPropertyString("No Description")]
        Description                     = 1
    }

    public static class RealmPropertyStringExtensions
    {
        public static string GetDescription(this RealmPropertyString prop)
        {
            var description = prop.GetAttributeOfType<DescriptionAttribute>();
            return description?.Description ?? prop.ToString();
        }
    }
}
