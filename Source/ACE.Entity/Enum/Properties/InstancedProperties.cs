using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACE.Entity.Enum.Properties
{
    /// <summary>
    /// These are location properties that are instanced.
    /// </summary>
    public class InstancedAttribute : Attribute
    {
    }

    public static class InstancedProperties
    {
        /// <summary>
        /// Method to return a list of enums by attribute type - in this case [Ephemeral] using generics to enhance code reuse.
        /// </summary>
        /// <typeparam name="T">Enum to list by [Ephemeral]</typeparam>
        /// <typeparam name="TResult">Type of the results</typeparam>
        private static HashSet<T> GetValues<T>()
        {
            var list = typeof(T).GetFields().Select(x => new
            {
                att = x.GetCustomAttributes(false).OfType<InstancedAttribute>().FirstOrDefault(),
                member = x
            }).Where(x => x.att != null && x.member.Name != "value__").Select(x => (T)x.member.GetValue(null)).ToList();

            return new HashSet<T>(list);
        }

        /// <summary>
        /// returns a list of values for PositionType that are [Instanced]
        /// </summary>
        public static HashSet<PositionType> PositionTypes = GetValues<PositionType>();
    }
}
