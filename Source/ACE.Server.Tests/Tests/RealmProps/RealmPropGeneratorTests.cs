using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Xunit;
using ACRealms.RealmProps;
using Gen = ACRealms.RealmProps.NamespacedRealmPropertyGenerator;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Testing;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using ACRealms.Tests.Helpers;
using Xunit.Sdk;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Threading;

#nullable enable
// The main purpose of this is to allow debugging of the ACRealms.RealmProps code generator

namespace ACRealms.Tests.Tests.RealmPropGenerator
{
    public interface IGeneratorTestBase
    {
        /// <summary>
        /// Allows you to specify additional global options that will appear in the context.AnalyzerConfigOptions.GlobalOptions object.
        /// </summary>
        public List<(string, string)> GlobalOptions { get; }
    }

    public class RealmPropVerifier : IVerifier
    {
        public RealmPropVerifier() : this(ImmutableStack<string>.Empty) { }

        protected RealmPropVerifier(ImmutableStack<string> context) =>
            Context = context ?? throw new ArgumentNullException(nameof(context));

        protected ImmutableStack<string> Context { get; }

        protected virtual string CreateMessage(string message)
        {
            foreach (var frame in Context)
                message = "Context: " + frame + Environment.NewLine + message;

            return message ?? string.Empty;
        }

        void IVerifier.Empty<T>(string collectionName, IEnumerable<T> collection)
        {
            var tracker = CollectionTracker<T>.Wrap(collection);
            using var enumerator = tracker.GetEnumerator();

            if (enumerator.MoveNext())
                throw EmptyException.ForNonEmptyCollection(collectionName);
        }

        void IVerifier.Equal<T>(T expected, T actual, string message)
        {
            if (EqualityComparer<T>.Default.Equals(expected, actual))
                return;

            if (message is null && Context.IsEmpty)
                throw EqualException.ForMismatchedValues(expected, actual);
            else
                throw EqualException.ForMismatchedValuesWithError(expected, actual, new Exception(CreateMessage(message)));
        }

        [DoesNotReturn]
        void IVerifier.Fail(string? message) => Assert.Fail(message is null ? "<no message provided>" : CreateMessage(message));

        void IVerifier.False([DoesNotReturnIf(true)] bool assert, string message)
        {
            if (message is null && Context.IsEmpty)
                Assert.False(assert);
            else
                Assert.False(assert, CreateMessage(message));
        }

        void IVerifier.LanguageIsSupported(string language) =>
            Assert.False(language != LanguageNames.CSharp, CreateMessage($"Unsupported Language: '{language}'"));

        void IVerifier.NotEmpty<T>(string collectionName, IEnumerable<T> collection)
        {
            if (collectionName == "TestState.Sources")
                return;

            using var enumerator = collection.GetEnumerator();

            if (!enumerator.MoveNext())
                throw NotEmptyException.ForNonEmptyCollection();
        }


        IVerifier IVerifier.PushContext(string context)
        {
            Assert.IsAssignableFrom<RealmPropVerifier>(this);
            return new RealmPropVerifier(Context.Push(context));
        }

        void IVerifier.SequenceEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, IEqualityComparer<T>? equalityComparer, string message)
        {
            var comparer = new SequenceEqualEnumerableEqualityComparer<T>(equalityComparer);
            var areEqual = comparer.Equals(expected, actual);

            if (!areEqual)
                throw EqualException.ForMismatchedValuesWithError(expected, actual, new Exception(CreateMessage(message)));
        }

        void IVerifier.True([DoesNotReturnIf(false)] bool assert, string? message)
        {
            if (message is null && Context.IsEmpty)
                Assert.True(assert);
            else
                Assert.True(assert, CreateMessage(message));
        }
    }

    public class RealmPropGeneratorTest : SourceGeneratorTest<RealmPropVerifier>, IGeneratorTestBase
    {
        public List<(string, string)> GlobalOptions { get; } = [];
        public override string Language => "C#";
        protected override string DefaultFileExt => "jsonc";

        protected override CompilationOptions CreateCompilationOptions()
        {
            return new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary) { };
        }

        protected override ParseOptions CreateParseOptions() => new CSharpParseOptions(LanguageVersion.CSharp12, kind: SourceCodeKind.Regular);
        protected override IEnumerable<Type> GetSourceGenerators() => [typeof(Gen)];
    }

    [Collection("RealmProps")]
    public class RealmPropGeneratorTests
    {
        static string SchemaFile { get; } = File.ReadAllText($"{Paths.SolutionPath}/ACE.Entity/ACRealms/RealmProps/json-schema/realm-property-schema.json");

        [Fact]
        public async Task CanGenerateRealmProps()
        {
            await RunTest<RealmPropGeneratorTest>(
                """
                    {
                        "$schema_version": 1,
                        "namespace": "Creature.Attribute",
                        "groups": [
                        {
                            "type": "integer",
                            "key_prefix": "Creature", "key_suffix": "Added",
                            "min_value": -10000000, "max_value": 10000000,
                            "description_format": "All creatures will have this value added to their {short_key} attribute",
                            "properties": [ "Strength", "Endurance", "Coordination", "Quickness", "Focus", "Self" ]
                        }
                        ]
                    }
                """,
                """
namespace ACRealms.RealmProps.Generated
{
    public partial class Foo1
    {
        public static int GetNum2()
        {
            return 1;
        }
    }
}
"""
).ConfigureAwait(false);
        }

        private static async Task RunTest<TestType>(string testSource, string generatedSource, DiagnosticResult? expectedDiagnostic = null)
           where TestType : SourceGeneratorTest<RealmPropVerifier>, IGeneratorTestBase, new()
        {
            var test = new TestType
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            };
            if (expectedDiagnostic is not null)
            {
                test.ExpectedDiagnostics.Add(expectedDiagnostic.Value);
            }
            test.TestState.AdditionalFiles.Add(("ACRealms\\RealmProps\\json\\peripheral\\classical-instance.jsonc", SourceText.From(testSource)));
            test.TestState.AdditionalFiles.Add(("ACRealms\\RealmProps\\json-schema\\realm-property-schema.json", SourceText.From(SchemaFile)));
            test.TestState.GeneratedSources.Add((typeof(Gen), "classical-instance.g.cs", generatedSource));
            await test.RunAsync();
        }
    }
    sealed class SequenceEqualEnumerableEqualityComparer<T> : IEqualityComparer<IEnumerable<T>?>
    {
        readonly IEqualityComparer<T> itemEqualityComparer;

        public SequenceEqualEnumerableEqualityComparer(IEqualityComparer<T>? itemEqualityComparer) =>
            this.itemEqualityComparer = itemEqualityComparer ?? EqualityComparer<T>.Default;

        public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x is null || y is null)
                return false;

            return x.SequenceEqual(y, itemEqualityComparer);
        }

        public int GetHashCode(IEnumerable<T>? obj)
        {
            if (obj is null)
                return 0;

            // From System.Tuple
            //
            // The suppression is required due to an invalid contract in IEqualityComparer<T>
            // https://github.com/dotnet/runtime/issues/30998
            return obj
                .Select(item => itemEqualityComparer.GetHashCode(item!))
                .Aggregate(
                    0,
                    (aggHash, nextHash) => ((aggHash << 5) + aggHash) ^ nextHash
                );
        }
    }
}
