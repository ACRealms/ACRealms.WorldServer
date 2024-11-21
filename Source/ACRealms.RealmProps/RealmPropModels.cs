using System;
using System.Collections.Generic;
using System.Text;

namespace ACRealms.RealmProps
{
    // https://github.com/corvus-dotnet/corvus.jsonschema?tab=readme-ov-file#getting-started
    // dotnet tool install --global Corvus.Json.JsonSchema.TypeGeneratorTool
    // generatejsonschematypes ACE.Entity\ACRealms\RealmProps\json-schema\realm-property-schema.json
    internal abstract record RealmPropModel { }

    internal record RealmPropNamespace : RealmPropModel
    {
    }
}
