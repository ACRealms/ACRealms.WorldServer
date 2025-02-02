using System;
using System.Collections.Generic;
using System.Text;

namespace ACRealms.Roslyn.RealmProps
{
    internal static class EntityToContextEntityMapping
    {
        internal static string GetScopedAttributeType(string entity) => entity switch
        {
            "Creature" => "IWorldObjectContextEntity",
            _ => throw new NotImplementedException($"Not Implemented (EntityToContextEntityMapping): {entity}")
        };
    }
}
