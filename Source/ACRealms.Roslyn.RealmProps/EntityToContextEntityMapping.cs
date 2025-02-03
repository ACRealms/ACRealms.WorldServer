using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Text;

namespace ACRealms.Roslyn.RealmProps
{
    internal static class EntityToContextEntityMapping
    {
        internal static string GetScopedAttributeType(string entity) => entity switch
        {
            "WorldObject" => "IWorldObjectContextEntity",
            _ => throw new NotImplementedException($"Not Implemented (EntityToContextEntityMapping): {entity}")
        };

        internal static ImmutableArray<string> BuildEntityList()
        {
            return
            [
                "WorldObject"
            ];
        }
    }
}
