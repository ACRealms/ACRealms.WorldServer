using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Text;

namespace ACRealms.Roslyn.RealmProps
{

    internal static class EntityToContextEntityMapping
    {
        /// <summary>
        /// Key: "Entity" value as defined in the PropDef json
        /// Value: The type of the corresponding argument when passing a context to a Prop eval in the server itself
        /// </summary>
        internal static string GetScopedAttributeType(string entity) => entity switch
        {
            "Weenie" or "WorldObject" => "IWorldObjectContextEntity",
            "WeeniePropertyInt" => "PropertyInt",
            "WeeniePropertyInt64" => "PropertyInt64",
            "WeeniePropertyFloat" => "PropertyFloat",
            "WeeniePropertyBool" => "PropertyBool",
            "WeeniePropertyString" => "PropertyString",
            _ => throw new NotImplementedException($"Not Implemented (EntityToContextEntityMapping): {entity}")
        };

        /// <summary>
        /// To be on the safe side, generate this list just-in-time instead of storing as a static field
        /// It is not fully understood if the Roslyn compiler cache will retain a stale value in some scenarios (such as MS Build Server).
        /// </summary>
        /// <returns></returns>
        internal static ImmutableArray<string> BuildEntityList()
        {
            return
            [
                "WorldObject",
                "WeeniePropertyInt",
                "WeeniePropertyInt64",
                "WeeniePropertyFloat",
                "WeeniePropertyBool",
                "WeeniePropertyString"
            ];
        }

        internal static string GetCanonicalEntitySchemaNameFromAlias(string entity)
        {
            if (entity == "Weenie")
                return "WorldObject";
            return entity;
        }
    }
}
