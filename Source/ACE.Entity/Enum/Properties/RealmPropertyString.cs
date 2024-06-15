using System.ComponentModel;

namespace ACE.Entity.Enum.Properties
{
    public enum RealmPropertyString : ushort
    {
        [RealmPropertyString(defaultValue: "")]
        Undef                           = 0,

        [Description("A description of the ruleset.")]
        [RealmPropertyString("No Description")]
        Description                     = 1,

        [Description(@"For the landblocks matching the name of this dungeon set, 
classical instances will be enabled for players with the ClassicalInstancesActive boolean property,
if the ruleset also has UseClassicalInstances set to true")]
        [RealmPropertyString("default")]
        ClassicalInstanceDungeonSet     = 2
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
