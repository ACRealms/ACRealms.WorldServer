using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace ACRealms.RoslynAnalyzer.Usage
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ACR1001_RealmsBinaryWriter_Write_ULong : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ACR1001";

        private static readonly string Title = "RealmsBinaryWriter.Write(ulong) Usage";
        private static readonly string MessageFormat = "RealmsBinaryWriter.Write(ulong) is not permitted to be used. Use WriteNonGuidULong or WriteGuid instead.";
        private static readonly string Description = "This rule is intended to prevent a client crash when sending a 64-bit GUID instead of the 32-bit ClientGUID.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new (DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocationExpr = (InvocationExpressionSyntax)context.Node;
            var memberAccessExpr = invocationExpr.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpr?.Name.ToString().ToLower() != "write")
                return;
            
            if (context.SemanticModel.GetSymbolInfo(invocationExpr, context.CancellationToken).Symbol is not IMethodSymbol methodSymbol)
                return;
            if (methodSymbol.ReceiverType.Name != "RealmsBinaryWriter")
                return;
            if (methodSymbol.ReceiverType.ContainingNamespace.ToString() != "ACE.Server.Network.GameMessages")
                return;
            if (methodSymbol.Name != "Write")
                return;
            if (methodSymbol.Parameters.Length != 1)
                return;
            var arg = methodSymbol.Parameters[0];
            var argType = arg.Type.Name;
            if (argType != "UInt64")
                return;
            if (arg.Type.ContainingNamespace.ToString() != "System")
                return;

            context.ReportDiagnostic(Diagnostic.Create(Rule, invocationExpr.GetLocation(), methodSymbol.Name, argType));
        }
    }
}
