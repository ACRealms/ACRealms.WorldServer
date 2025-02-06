using static ACRealms.Roslyn.RealmProps.Builders.SerializationHelpers;

namespace ACRealms.Roslyn.RealmProps.Builders.Phase2Src
{
    internal static class RootGeneratedPropsSchema
    {
        class RecursiveNamespaceDict
        {
            public Dictionary<string, RecursiveNamespaceDict> Children { get; } = [];

            // If we only define a namespace Foo.Bar.Baz, but not Foo or Foo.Bar, then the entries for Foo and Foo.Bar will not have StubData
            public NamespaceStub? StubData { get; set; }
        }

        internal static string GenerateCombinedSchemaSourceCode(ImmutableArray<NamespaceStub> namespaces)
        {
            try
            {
                var recursiveDict = Convert2DArrayToDict(namespaces);
                var rootSchema = GetSubSchemaFor(recursiveDict);

                // We must wrap in a comment as the source generator treats the output as a C# file
                var schema = $$"""
                /*
                {
                  "$schema": "http://json-schema.org/draft-07/schema",
                  {{rootSchema}}
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

        private static RecursiveNamespaceDict Convert2DArrayToDict(IEnumerable<NamespaceStub> namespaces)
        {
            var recursiveDict = new RecursiveNamespaceDict();
            foreach (var ns in namespaces)
            {
                var current = recursiveDict;
                foreach (var key in ns.NestedClassNames)
                {
                    if (!current.Children.ContainsKey(key))
                        current.Children[key] = new();
                    current = current.Children[key];
                }

                // The final substring (separated by a period) of the full namespace string is the one we can decorate with data
                // If there's no stub, we merely use it as a bridge
                // It is still recommended to have a stub (_index.json) for each namespace part, for at least a description
                current.StubData = ns; 
            }
            return recursiveDict;
        }

        private static string GetSubSchemaFor(RecursiveNamespaceDict items, string currentPath = "", string currentKey = "")
        {
            List<string> props = [];
            if (items.StubData?.Description != null)
                AddStringProp(props, "description", items.StubData.Description);

            if (items.Children.Values.Count == 0)
            {
                AddStringProp(props, "$ref", $"{currentPath}.json".Replace("/.json", ".json"));
                return SerializePropsUnwrapped(props);
            }
            else
            {
                List<string> innerProps = [];

                foreach (var kvp in items.Children)
                {
                    var val = GetSubSchemaFor(kvp.Value, $"{currentPath}{kvp.Key}/", kvp.Key);
                    AddUnwrappedObjectProp(innerProps, kvp.Key, val);
                }

                AddStringProp(props, "type", "object");
                AddProp(props, "additionalProperties", "false");
                AddUnwrappedObjectProp(props, "properties", SerializePropsUnwrapped(innerProps));
                return SerializePropsUnwrapped(props);
            }
        }
    }
}
