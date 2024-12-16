using System;
using System.Collections.Generic;
using System.Text;

namespace ACRealms.Roslyn.RealmProps.Builders
{
    internal static class SerializationHelpers
    {
        internal static string SerializePropsUnwrapped(List<string> props)
            => string.Join("""
                ,

                """, props);
        internal static void AddProp(List<string> props, string key, string valLiteral) => props.Add($"\"{key}\": {valLiteral}");
        internal static void AddStringProp(List<string> props, string key, string stringVal) => AddProp(props, key, $"\"{stringVal}\"");
        internal static void AddUnwrappedObjectProp(List<string> props, string key, string serializedObjectWithoutWrapper) => AddProp(props, key, $$"""
                {
                  {{serializedObjectWithoutWrapper}}
                }
                """);
        internal static string RefSnippet(string target) =>
            $$"""
            "$ref": "{{target}}"
            """;
        internal static string RefLiteral(string target) => $"{{ {RefSnippet(target)} }}";
    }
}
