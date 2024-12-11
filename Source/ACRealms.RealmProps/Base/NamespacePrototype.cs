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
    public static class Namespaces
    {
        private static readonly Dictionary<string, PropNamespace>? TempBuilder = new();

        public static PropNamespace Root { get; } = Build();

        public static readonly FrozenDictionary<string, PropNamespace> ByFullName = TempBuilder.ToFrozenDictionary();
        

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

    public record PropNamespace
    {
        public required string FullName { get; init; }
        public required string ShortName { get; init; }
        public required bool IsLeaf { get; init; }
        public required bool IsRoot { get; init; }
        internal FrozenDictionary<string, PropNamespace> SubNamespaces { get; }

        public string DisplayName => IsRoot ? "<Root>" : FullName;

        public PropNamespace this[string shortName] => SubNamespaces[shortName];
        public override string ToString() => DisplayName;

        internal PropNamespace(FrozenDictionary<string, PropNamespace> subNamespaces)
        {
            SubNamespaces = subNamespaces;
        }
    }
}
