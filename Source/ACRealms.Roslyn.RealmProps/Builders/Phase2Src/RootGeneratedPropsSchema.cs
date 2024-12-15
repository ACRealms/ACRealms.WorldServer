using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ACRealms.Roslyn.RealmProps.Builders.Phase2Src
{
    internal static class RootGeneratedPropsSchema
    {
        internal static string GenerateCombinedSchemaSourceCode(ImmutableArray<ImmutableArray<string>> namespaces)
        {
            try
            {
                var recursiveDict = Convert2DArrayToDict(namespaces);
                var propertiesSchema = GetSubSchemaFor(recursiveDict);

                // We must wrap in a comment as the source generator treats the output as a C# file
                var schema = $$"""
                /*
                {
                  "$schema": "http://json-schema.org/draft-07/schema",
                  "type": "object",
                  "properties": {
                  {{propertiesSchema}}
                  },
                  "additionalProperties": false
                }
                */
                """;

                return schema;
            }
            catch (Exception ex)
            {
                Helpers.ReThrowWrappedException($"RootGeneratedPropsSchema.GenerateCombinedSchemaSourceCode", ex);
                throw;
            }
        }

        private static Dictionary<string, object> Convert2DArrayToDict(ImmutableArray<ImmutableArray<string>> namespaces)
        {
            var recursiveDict = new Dictionary<string, object>();
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
            return recursiveDict;
        }

        private static string GetSubSchemaFor(Dictionary<string, object> items, string currentPath = "", string currentKey = "")
        {
            if (items.Values.Count == 0)
            { 
                var path = $"{currentPath}.json".Replace("/.json", ".json");

                return $$"""
                    { "$ref": "{{path}}" }
                    """;
            }
            else
            {
                StringBuilder innerProps = new StringBuilder();

                string sep = ",";
                int iter = 1;
                foreach (var kvp in items)
                {
                    if (iter == items.Count)
                        sep = "";
                    var val = GetSubSchemaFor((Dictionary<string, object>)kvp.Value, $"{currentPath}{kvp.Key}/", kvp.Key);
                    innerProps.Append($$"""
                      "{{kvp.Key}}": {{val}}{{sep}}

                    """);
                    iter++;
                }

                if (currentKey == "")
                    return innerProps.ToString();

                return $$"""
                    {
                      "type": "object",
                      "properties": {
                        {{innerProps}}
                      },
                      "additionalProperties": false
                    }
                    """;
            }
        }
    }
}
