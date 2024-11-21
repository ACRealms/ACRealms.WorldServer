using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System;
using Newtonsoft.Json.Schema;
using Microsoft.CodeAnalysis.Diagnostics;
using Newtonsoft.Json.Linq;

namespace ACRealms.RealmProps
{
    [Generator(LanguageNames.CSharp)]
    public class NamespacedRealmPropertyGenerator : IIncrementalGenerator
    {
       
        /*
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            if (!Debugger.IsAttached)
            {
               //Debugger.Launch();
            }
            // define the execution pipeline here via a series of transformations:

            // find all additional files that end with .txt
            IncrementalValuesProvider<AdditionalText> textFiles = initContext.AdditionalTextsProvider.Where(static file => file.Path.EndsWith(".jsonc"));

            // read their contents and save their name
            IncrementalValuesProvider<(string name, string content)> namesAndContents = textFiles.Select((text, cancellationToken) => (name: Path.GetFileNameWithoutExtension(text.Path), content: text.GetText(cancellationToken)!.ToString()));

            // generate a class that contains their values as const strings
           /* initContext.RegisterSourceOutput(namesAndContents, (spc, nameAndContent) =>
            {
                spc.AddSource($"ConstStrings.{nameAndContent.name}", $@"
    public static partial class ConstStrings
    {{
        public const string {nameAndContent.name} = ""{nameAndContent.content}"";
    }}");
            });*/
        //}

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Regex for property names: https://regexr.com/8915n

            // TODO: DELETE FILES THAT AREN'T GENERATED

            var realmPropsDirContext = context.AdditionalTextsProvider
                .Where(static file => { var sep = Path.DirectorySeparatorChar; return file.Path.Contains($"{sep}RealmProps{sep}json"); });

            var schema = realmPropsDirContext
                .Where(static file => { var sep = Path.DirectorySeparatorChar; return file.Path.EndsWith($"{sep}RealmProps{sep}json-schema{sep}realm-property-schema.json"); })
                .Select(static (text, cancellationToken) => text.GetText(cancellationToken)!.ToString())
                .Select(static (text, cancellationToken) => JSchema.Parse(text))
                .Collect()
                .WithTrackingName("schema");

            IncrementalValuesProvider<((string Path, JObject RealmPropsObj) ParsedFile, ImmutableArray<JSchema> Schema)> unvalidatedRealmPropObjects = realmPropsDirContext
                .Where(static file => { var sep = Path.DirectorySeparatorChar; return file.Path.Contains($"{sep}RealmProps{sep}json{sep}"); })
                .Where(static file => Path.GetExtension(file.Path).Equals(".jsonc", StringComparison.OrdinalIgnoreCase))
                .Select(static (text, cancellationToken) => ((string Path, string Raw))(text.Path, text.GetText(cancellationToken).ToString()))
                .Select(static (data, cancellationToken) => ((string Path, JObject RealmPropsObj))(data.Path, JObject.Parse(data.Raw)))
                .Combine(schema);

            var validatedRealmPropObjects = unvalidatedRealmPropObjects.Select(static (realmPropsFileContext, cancellationToken) =>
            {
                var schema = realmPropsFileContext.Schema.Single();
                realmPropsFileContext.ParsedFile.RealmPropsObj.Validate(schema);
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
        private static string GenerateSourceCode(string className, JObject RealmPropNamespace)
        {
            return $$"""
                namespace ACRealms.RealmProps.Generated
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
