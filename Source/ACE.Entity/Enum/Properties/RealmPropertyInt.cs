using System;
using System.ComponentModel;
using System.Globalization;

namespace ACE.Entity.Enum.Properties
{
    public enum RealmPropertyInt : ushort
    {
        Undef                                    = 0,
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
