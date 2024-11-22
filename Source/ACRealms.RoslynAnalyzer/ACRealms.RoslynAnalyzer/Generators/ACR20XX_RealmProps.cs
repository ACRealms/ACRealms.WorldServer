using ACRealms.RealmProps.IntermediateModels;
using Corvus.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using JsonObject = Corvus.Json.JsonObject;


// This is not a generator, it is a generator diagnostic
// The actual generator is in the project ACRealms.RealmsProps
namespace ACRealms.RoslynAnalyzer.Generators
{
    // https://github.com/dotnet/roslyn/blob/ee941ffa3c753fd3f0760d34ca9bca6ba83ea080/docs/features/incremental-generators.cookbook.md#issue-diagnostics
    // Per Microsoft, we can't emit errors while doing incremental generation without ruining performance, so we need a separate diagnostic for that here
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ACR20XX_RealmProps : DiagnosticAnalyzer
    {
       // static string SchemaFile { get; } = File.ReadAllText($"{SolutionPath}/ACE.Entity/ACRealms/RealmProps/json-schema/realm-property-schema.json");

        private const string Category = "Generators";

        private static readonly string Title = "RealmProps";

        internal record IntermediateDescriptor
        {
            public DiagnosticSeverity Severity { get; init; } = DiagnosticSeverity.Error;
            public required string MessageFormat { get; init; }
            public string? Description { get; init; }

            public DiagnosticDescriptor ToDescriptor(DescriptorType type)
            {
                var id = $"ACR20{((byte)type).ToString().PadLeft(2, '0')}";
                return new DiagnosticDescriptor(id, Title, MessageFormat, Category, Severity, true, Description);
            }
        }

        public enum DescriptorType : byte
        {
            None,
            MissingSchema,
            FailedToParse,
            JSONValidation,
            Deserialization
        }

        private static readonly FrozenDictionary<DescriptorType, DiagnosticDescriptor> Descriptors = new Dictionary<DescriptorType, IntermediateDescriptor>()
        {
            { DescriptorType.MissingSchema, new IntermediateDescriptor() { MessageFormat = "Missing JSON Schema file {0}" } },
            { DescriptorType.FailedToParse, new IntermediateDescriptor() { MessageFormat = "Failed to parse JSON for file {0}" } },
            { DescriptorType.JSONValidation, new IntermediateDescriptor() { MessageFormat = "Validation Error: {0}" } },
            { DescriptorType.Deserialization, new IntermediateDescriptor() { MessageFormat = "(Likely ACRealms bug, please report) - Failed to deserialize valid JSON: {0}" } }
        }.ToFrozenDictionary(kvp => kvp.Key, kvp => kvp.Value.ToDescriptor(kvp.Key));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return Descriptors.Values; } }

        private static Diagnostic Diag(DescriptorType type, Location? location = null, object?[]? messageArgs = null)
        {
            return Diagnostic.Create(Descriptors[type], location, messageArgs);
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(SchemaValidationActions);
        }

        private void SchemaValidationActions(CompilationStartAnalysisContext context)
        {
            void Report(DescriptorType type, Location? location = null, object?[]? messageArgs = null)
            {
                context.RegisterCompilationEndAction(c => c.ReportDiagnostic(Diagnostic.Create(Descriptors[type], location, messageArgs)));
            }
            if (context.Compilation.AssemblyName != "ACE.Entity")
                return;

            ImmutableArray<AdditionalText> additionalFiles = context.Options.AdditionalFiles;
            var sep = Path.DirectorySeparatorChar;
            var pathSuffix = $"ACRealms{sep}RealmProps{sep}json-schema{sep}realm-property-schema.json";
            AdditionalText? realmPropSchema = additionalFiles.FirstOrDefault(file => file.Path.EndsWith(pathSuffix));
            if (realmPropSchema == null)
            {
                Report(DescriptorType.MissingSchema, null, [pathSuffix]);
                return;
            }

            var schemaText = realmPropSchema!.GetText(context.CancellationToken);
            if (schemaText == null)
            {
                Report(DescriptorType.FailedToParse, null, [pathSuffix]);
                return;
            }

            JsonObject schema;
            try
            {
                schema = JsonObject.Parse(schemaText.ToString());
            }
            catch (Exception)
            {
                Report(DescriptorType.FailedToParse, null, [pathSuffix]);
                return;
            }

            context.RegisterAdditionalFileAction(c => FileValidationActions(c, realmPropSchema, schema));
        }


        private void FileValidationActions(AdditionalFileAnalysisContext c, AdditionalText realmPropSchema, JsonObject schema)
        {
            void Report(DescriptorType type, Location? location = null, object?[]? messageArgs = null)
            {
                c.ReportDiagnostic(Diagnostic.Create(Descriptors[type], location, messageArgs));
            }

            var sep = Path.DirectorySeparatorChar;
            if (!c.AdditionalFile.Path.Contains($"ACRealms{sep}RealmProps{sep}json{sep}") || !c.AdditionalFile.Path.EndsWith(".jsonc"))
                return;

            var file = c.AdditionalFile;
            var text = file.GetText(c.CancellationToken);
            if (text == null)
            {
                Report(DescriptorType.FailedToParse, Location.Create(
                        file.Path,
                        TextSpan.FromBounds(1, 1),
                        new LinePositionSpan(
                            new LinePosition(1, 1),
                            new LinePosition(1, 1)
                        )
                    ), [file.Path]);
                return;
            }

            var raw = text.ToString();
            JsonObject realmPropsObj;
            try
            {
                realmPropsObj = JsonObject.Parse(raw);
            }
            catch (Exception)
            {
                Report(DescriptorType.FailedToParse, Location.Create(
                        file.Path,
                        TextSpan.FromBounds(1, 1),
                        new LinePositionSpan(
                            new LinePosition(1, 1),
                            new LinePosition(1, 1)
                        )
                    ), [file.Path]);
                return;
            }

            try
            {
                try
                {
                    var ctx = new ValidationContext().UsingResults().PushSchemaLocation(realmPropSchema.Path);
                    var realmPropsObjParsed = RealmPropertySchema.FromJson(realmPropsObj.AsJsonElement);
                    var validation = realmPropsObjParsed.Validate(ctx, ValidationLevel.Detailed);

                    if (!validation.IsValid)
                    {
                        foreach (var badResult in validation.Results)
                        {
                            //var loc = badResult.Location.Value;
                            var lineNum = 1;
                            var linePos = 1;
                            Report(DescriptorType.JSONValidation,
                            Location.Create(
                                file.Path,
                                TextSpan.FromBounds(lineNum, lineNum),
                                new LinePositionSpan(
                                    new LinePosition(lineNum, linePos),
                                    new LinePosition(lineNum, linePos)
                                )
                            ), [badResult.Message]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex.InnerException?.InnerException ?? ex.InnerException ?? ex;
                }
            }
            catch (Exception ex)
            {
                var lineNum = 1;
                var linePos = 1;
                Report(DescriptorType.Deserialization,
                Location.Create(file.Path, TextSpan.FromBounds(lineNum, lineNum),
                    new LinePositionSpan(new LinePosition(lineNum, linePos), new LinePosition(lineNum, linePos))), [ex.Message]);
            }
        }
    }
}
