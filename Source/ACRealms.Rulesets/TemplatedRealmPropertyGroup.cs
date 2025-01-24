using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Rulesets
{
    internal abstract record TemplatedRealmPropertyGroup(string name, int propKey)
        : IRealmPropertyGroup
    {
        internal RulesetCompilationContext CompilationContext { get; init; }
        public string Name { get; init; } = name;
        public int PropertyKey { get; } = propKey;

        public RealmPropertyGroupOptions Options { get; }
    }

    /// <summary>
    /// Represents a collection of all properties associated with a single ruleset, for a single key.
    /// There will be more than one item in this collection if multiple scopes are defined for a ruleset with the same property key
    /// TODO: Try to get more deduplicateddata in here instead of ActiveRealmPropertyGroup, to improve performance
    /// </summary>
    internal record TemplatedRealmPropertyGroup<TVal> : TemplatedRealmPropertyGroup, IRealmPropertyGroup<TVal>
        where TVal : IEquatable<TVal>
    {
        IEnumerable<IAppliedRealmProperty<TVal>> IRealmPropertyGroup<TVal>.Properties { get; }
        public ImmutableList<TemplatedRealmProperty<TVal>> Properties { get; internal init; }
        public new RealmPropertyGroupOptions<TVal> Options { get; private init; }
        IRealmPropertyGroup<TVal> IRealmPropertyGroup<TVal>.Parent { get; }
        public TemplatedRealmPropertyGroup<TVal> Parent { get; private init; }

        public TemplatedRealmPropertyGroup(RulesetCompilationContext compilationContext, string name, int propKey, RealmPropertyGroupOptions<TVal> opts)
            : base(name, propKey)
        {
            CompilationContext = compilationContext;
            Options = opts;
        }

        public TemplatedRealmPropertyGroup(RulesetCompilationContext compilationContext, TemplatedRealmPropertyGroup<TVal> cloneFrom, TemplatedRealmPropertyGroup<TVal> parent)
            : this(compilationContext, cloneFrom.Name, cloneFrom.PropertyKey, cloneFrom.Options)
        {
        }

        internal TemplatedRealmPropertyGroup(string name, int propKey)
            : base(name, propKey)
        {

        }
    }
}
