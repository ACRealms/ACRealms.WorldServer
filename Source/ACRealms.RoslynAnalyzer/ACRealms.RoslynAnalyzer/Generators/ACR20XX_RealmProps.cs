using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace ACRealms.RoslynAnalyzer.Generators
{
    // https://github.com/dotnet/roslyn/blob/ee941ffa3c753fd3f0760d34ca9bca6ba83ea080/docs/features/incremental-generators.cookbook.md#issue-diagnostics
    // Per Microsoft, we can't emit errors while doing incremental generation without ruining performance, so we need a separate diagnostic for that here
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ACR20XX_RealmProps : DiagnosticAnalyzer
    {
        private const string Category = "Generators.RealmProps";

        private static readonly string Title = "RealmProps";

        public enum DescriptorType : byte
        {
            None,
            FailedToParse
        }

        internal record IntermediateDescriptor
        {
            public DiagnosticSeverity Severity { get; init; } = DiagnosticSeverity.Error;
            public required string MessageFormat { get; init; }
            public string Description { get; init; }

            public DiagnosticDescriptor ToDescriptor(DescriptorType type)
            {
                var id = $"ACR20{((byte)type).ToString().PadLeft(2, '0')}";
                return new DiagnosticDescriptor(id, Title, MessageFormat, Category, Severity, true, Description);
            }
        }

        private static readonly FrozenDictionary<DescriptorType, DiagnosticDescriptor> Descriptors = new Dictionary<DescriptorType, IntermediateDescriptor>()
        {
            { DescriptorType.FailedToParse, new IntermediateDescriptor() { MessageFormat = "Failed to parse JSON" } }
        }.ToFrozenDictionary(kvp => kvp.Key, kvp => kvp.Value.ToDescriptor(kvp.Key));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return Descriptors.Values; } }

        public override void Initialize(AnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}
