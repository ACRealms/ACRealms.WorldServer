using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.RealmProps
{
    /// <summary> Contains metadata about namespaced realm properties </summary>
    public static class Namespaces
    {
        private static readonly Dictionary<string, PropNamespace>? TempBuilder = new();

        /// <summary> Metadata for the root namespace </summary>
        public static PropNamespace Root { get; } = Build();

        /// <summary> Flattened dictionary for all namespaces. For example, "Core.Instance" is a key </summary>
        internal static readonly FrozenDictionary<string, PropNamespace> ByFullName = TempBuilder.ToFrozenDictionary();
        

        static Namespaces()
        {
            TempBuilder = null;
        }

        private static PropNamespace Build()
        {
            Type? rootProps = System.Reflection.Assembly.GetExecutingAssembly().GetType("ACRealms.Props");
            if (rootProps is null || !rootProps.IsClass)
                throw new InvalidOperationException("RealmProps failed to load successfully.");

            return MakeFromType(rootProps, ImmutableArray<string>.Empty, true);
        }

        private static PropNamespace MakeFromType(Type type, ImmutableArray<string> nestedNames, bool isRoot)
        {
            var subtypes = type.GetNestedTypes();
            var isLeaf = !subtypes.Any();
            var name = isRoot ? "" : type.Name;
            var names = isRoot ? nestedNames : nestedNames.Add(name);
            var dict = new Dictionary<string, PropNamespace>();
            foreach(var sub in subtypes)
                dict.Add(sub.Name, MakeFromType(sub, names, false));

            var ns = new PropNamespace(dict.ToFrozenDictionary())
            {
                IsLeaf = isLeaf,
                ShortName = name,
                FullName = string.Join(".", names),
                IsRoot = isRoot
            };
            Namespaces.TempBuilder![ns.FullName] = ns;
            return ns;
        }
    }

    /// <summary> Metadata for a namespace, including its sub-namespaces </summary>
    public record PropNamespace
    {
        /// <summary> Gets the fully-qualified name of the namespace </summary>
        public required string FullName { get; init; }

        /// <summary> Gets the simple name of the namespace. </summary>
        public required string ShortName { get; init; }

        /// <summary> Returns true if the namespace is a leaf node (no sub-namespaces) </summary>
        public required bool IsLeaf { get; init; }

        /// <summary> Returns true if the namespace is the root namespace </summary>
        public required bool IsRoot { get; init; }

        internal FrozenDictionary<string, PropNamespace> SubNamespaces { get; }

        /// <summary> Gets the display name of the namespace </summary>
        public string DisplayName => IsRoot ? "<Root>" : FullName;

        /// <summary> Returns the directly-descended namespace with the given ShortName </summary>
        /// <param name="shortName">The ShortName of the namespace to fetch</param>
        /// <returns>The directly-descended namespace with the given ShortName</returns>
        public PropNamespace this[string shortName] => SubNamespaces[shortName];

        /// <inheritdoc/>
        public override string ToString() => DisplayName;

        internal PropNamespace(FrozenDictionary<string, PropNamespace> subNamespaces)
        {
            SubNamespaces = subNamespaces;
        }
    }
}
