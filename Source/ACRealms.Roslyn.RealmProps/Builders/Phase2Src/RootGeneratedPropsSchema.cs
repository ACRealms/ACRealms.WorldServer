using System;
using System.Collections.Generic;
using System.Text;

namespace ACRealms.Roslyn.RealmProps.Builders.Phase2Src
{
    internal static class RootGeneratedPropsSchema
    {
        internal static string GenerateCombinedSchemaSourceCode(ImmutableArray<ImmutableArray<string>> namespaces)
        {
            try
            {
                Dictionary<string, object> recursiveDict = [];

                foreach (var namespaceParts in namespaces)
                {
                    var current = recursiveDict;
                    foreach (var key in namespaceParts)
                    {
                        if (!current.ContainsKey(key))
                            current[key] = new Dictionary<string, object>();
                        current = (Dictionary<string, object>)current[key];
                    }
                }


                var propertiesSchema = GetSubSchemaFor(recursiveDict);

                Dictionary<string, object> schema = new Dictionary<string, object>
                {
                    { "$schema",  "http://json-schema.org/draft-07/schema" },
                    { "$id", "https://realm.ac/schema/v1/generated/realm-properties-root.json" },
                    { "type", "object" },
                    { "properties", propertiesSchema },
                    { "additionalProperties", false }
                };

                var schemaSerialized = JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });

                return $$"""
                    /*
                    {{schemaSerialized}}
                    */
                    """;
            }
            catch (Exception ex)
            {
                Helpers.ReThrowWrappedException($"RootGeneratedPropsSchema.GenerateCombinedSchemaSourceCode", ex);
                throw;
            }
        }

        private static Dictionary<string, object> GetSubSchemaFor(Dictionary<string, object> items, string currentPath = "", string currentKey = "")
        {
            if (items.Values.Count == 0)
            {
                Dictionary<string, object> dict = [];
                dict.Add("$ref", $"{currentPath}.json".Replace("/.json", ".json"));
                return dict;
            }
            else
            {
                Dictionary<string, object> dictProps = [];
                foreach (var kvp in items)
                    dictProps.Add(kvp.Key, GetSubSchemaFor((Dictionary<string, object>)kvp.Value, $"{currentPath}{kvp.Key}/", kvp.Key));

                if (currentKey == "")
                    return dictProps;

                Dictionary<string, object> dictObj = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", dictProps },
                    { "additionalProperties", false }
                };
                return dictObj;
            }
        }
    }
}
