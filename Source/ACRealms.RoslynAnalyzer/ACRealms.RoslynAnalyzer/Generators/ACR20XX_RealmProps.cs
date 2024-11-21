using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
# nullable enable

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
            { DescriptorType.Deserialization, new IntermediateDescriptor() { MessageFormat = "(Likely ACRealms bug, please report) - Failed to deserialize valid JSON" } }
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

            /*
            var schemaText = realmPropSchema!.GetText(context.CancellationToken);
            Schema schema;
            if (schemaText == null)
            {
                Report(DescriptorType.FailedToParse, null, [pathSuffix]);
                return;
            }
            try
            {
                schema = JSchema.Parse(schemaText.ToString());
            }
            catch (Exception)
            {
                Report(DescriptorType.FailedToParse, null, [pathSuffix]);
                return;
            }

            var realmPropFiles = additionalFiles.Where(f => f.Path.Contains($"ACRealms{sep}RealmProps{sep}json{sep}") && f.Path.EndsWith(".jsonc")).ToList();
            foreach(var file in realmPropFiles)
            {
                var text = file.GetText(context.CancellationToken);
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
                    continue;
                }
                var raw = text.ToString();
                JObject realmPropsObj;
                try
                {
                    realmPropsObj = JObject.Parse(raw);
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
                    continue;
                }
                try
                {
                    realmPropsObj.Validate(schema);
                }
                catch (JSchemaValidationException ex)
                {
                    Report(DescriptorType.JSONValidation,
                        Location.Create(
                            file.Path,
                            TextSpan.FromBounds(ex.LineNumber, ex.LineNumber),
                            new LinePositionSpan(
                                new LinePosition(ex.LineNumber, ex.LinePosition),
                                new LinePosition(ex.LineNumber, ex.LinePosition)
                            )
                        ),[ex.Message]);
                    continue;
                }
            }*/
        }
    }
}
