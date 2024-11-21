using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Xml.Serialization;
using Corvus.Json.Validator;
using System.Text.Json.Nodes;
using JsonObject = Corvus.Json.JsonObject;

namespace ACRealms.RealmProps
{
    // To debug: Test Explorer -> ACRealms.Tests.Tests.RealmPropGenerator -> Debug CanGenerateRealmProps test
    // Important: Significant changes to this generator's process or algorithm should be reflected in ACRealms.RoslynAnalyzer.ACR20XX_RealmProps where applicable
    //            This allows for more accurate errors to be displayed during compilation
    //
    // The C# model classes in ACRealms.RealmProps.RealmPropModels are generated through a manual script invocation as needed
    [Generator(LanguageNames.CSharp)]
    public class NamespacedRealmPropertyGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Regex for property names: https://regexr.com/8915n

            // TODO: DELETE FILES THAT AREN'T GENERATED

            var realmPropsDirContext = context.AdditionalTextsProvider
                .Where(static file => { var sep = Path.DirectorySeparatorChar; return file.Path.Contains($"{sep}RealmProps{sep}json"); });

            var schema = realmPropsDirContext
                .Where(static file => { var sep = Path.DirectorySeparatorChar; return file.Path.EndsWith($"{sep}RealmProps{sep}json-schema{sep}realm-property-schema.json"); })
                .Select(static (text, cancellationToken) => text.GetText(cancellationToken)!.ToString())
                .Select(static (text, cancellationToken) => JsonSchema.FromText(text))
                .Collect()
                .WithTrackingName("schema");

            IncrementalValuesProvider<((string Path, JsonObject RealmPropsObj) ParsedFile, ImmutableArray<JsonSchema> Schema)> unvalidatedRealmPropObjects = realmPropsDirContext
                .Where(static file => { var sep = Path.DirectorySeparatorChar; return file.Path.Contains($"{sep}RealmProps{sep}json{sep}"); })
                .Where(static file => Path.GetExtension(file.Path).Equals(".jsonc", StringComparison.OrdinalIgnoreCase))
                .Select(static (text, cancellationToken) => ((string Path, string Raw))(text.Path, text.GetText(cancellationToken).ToString()))
                .Select(static (data, cancellationToken) => ((string Path, JsonObject RealmPropsObj))(data.Path, JsonObject.Parse(data.Raw)))
                .Combine(schema);

            var validatedRealmPropObjects = unvalidatedRealmPropObjects.Select(static (realmPropsFileContext, cancellationToken) =>
            {
                var schema = realmPropsFileContext.Schema.Single();
                var realmPropsJObject = realmPropsFileContext.ParsedFile.RealmPropsObj;

                var validation = schema.Validate(realmPropsJObject.AsJsonElement);
                if (!validation.IsValid)
                    throw new InvalidOperationException("Invalid JSON!");

                return realmPropsFileContext.ParsedFile;
            });

            context.RegisterSourceOutput(validatedRealmPropObjects, (spc, data) =>
            {
                var fileName = Path.GetFileNameWithoutExtension(data.Path);
                var jsonObject = data.RealmPropsObj;
                var sourceCode = GenerateSourceCode(fileName.Replace("-", "_"), jsonObject);
                spc.AddSource($"{fileName}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
            });
        }

        private static string RemoveJsonComments(string jsonc)
        {
            var sb = new StringBuilder();
            using (var reader = new StringReader(jsonc))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Remove single-line comments
                    var index = line.IndexOf("//");
                    if (index >= 0)
                        line = line.Substring(0, index);

                    // Remove block comments (simple implementation)
                    // You can enhance this to handle multi-line block comments
                    sb.AppendLine(line);
                }

                return sb.ToString();
            }
        }
        private static string GenerateSourceCode(string className, JsonObject realmPropNamespace)
        {
            return $$"""
                
                """;
        }


        /*
        private static string GetPropertyType(JsonNode node)
        {
            if (node is JsonValue value)
            {
                if (value.TryGetValue(out int _))
                    return "int";
                if (value.TryGetValue(out long _))
                    return "long";
                if (value.TryGetValue(out float _))
                    return "float";
                if (value.TryGetValue(out double _))
                    return "double";
                if (value.TryGetValue(out bool _))
                    return "bool";
                if (value.TryGetValue(out string _))
                    return "string";
                if (value.TryGetValue(out DateTime _))
                    return "DateTime";
                // Add other types as necessary
            }
            else if (node is JsonArray)
            {
                return "object[]"; // Simplified for this example
            }
            else if (node is JsonObject)
            {
                return "object"; // For nested objects, consider generating nested classes
            }

            return "object";
        }*/
    }
}
