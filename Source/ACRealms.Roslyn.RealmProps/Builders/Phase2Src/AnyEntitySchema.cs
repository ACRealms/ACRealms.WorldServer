using System;
using System.Collections.Generic;
using System.Text;

namespace ACRealms.Roslyn.RealmProps.Builders.Phase2Src
{
    internal static class AnyEntitySchema
    {
        internal static string GenerateEntitySchemaSourceCode(string entityType)
        {
            if (entityType == "WorldObject")
                return BiotaSchema.GenerateEntitySchemaSourceCode();

            string[] weenieProps = ["WeeniePropertyInt", "WeeniePropertyInt64", "WeeniePropertyFloat", "WeeniePropertyString", "WeeniePropertyBool"];

            if (weenieProps.Contains(entityType))
                return WeeniePropSchema.GenerateEntitySchemaSourceCode(entityType);

            throw new NotImplementedException();
        }
    }
}
