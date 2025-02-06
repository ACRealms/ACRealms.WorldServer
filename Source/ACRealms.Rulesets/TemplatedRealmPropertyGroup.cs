using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Rulesets
{
    internal abstract record TemplatedRealmPropertyGroup(RealmPropertyGroupOptions OptionsBase, int propKey)
        : IRealmPropertyGroup
    {
        internal RulesetCompilationContext CompilationContext { get; init; }
        public string Name => OptionsBase.Name;
        public int PropertyKey { get; } = propKey;
    }

    /// <summary>
    /// Represents a collection of all properties associated with a single ruleset, for a single key.
    /// There will be more than one item in this collection if multiple scopes are defined for a ruleset with the same property key
    /// TODO: Try to get more deduplicateddata in here instead of ActiveRealmPropertyGroup, to improve performance
    /// </summary>
    internal record TemplatedRealmPropertyGroup<TVal>(RealmPropertyGroupOptions<TVal> Options, int propKey) : TemplatedRealmPropertyGroup(Options, propKey), IRealmPropertyGroup<TVal>
        where TVal : IEquatable<TVal>
    {
        public ImmutableArray<TemplatedRealmProperty<TVal>> Properties { get; internal init; }
        public RealmPropertyGroupOptions<TVal> Options => (RealmPropertyGroupOptions<TVal>)OptionsBase;

        IRealmPropertyGroup<TVal> IRealmPropertyGroup<TVal>.Parent { get; }
        public TemplatedRealmPropertyGroup<TVal> Parent { get; private init; }

        public TemplatedRealmPropertyGroup(RulesetCompilationContext compilationContext, RealmPropertyGroupOptions<TVal> Options, int propKey)
            : this(Options, propKey)
        {
            CompilationContext = compilationContext;
        }

        public TemplatedRealmPropertyGroup(RulesetCompilationContext compilationContext, TemplatedRealmPropertyGroup<TVal> cloneFrom, TemplatedRealmPropertyGroup<TVal> parent)
            : this(compilationContext, cloneFrom.Options, cloneFrom.PropertyKey)
        {
            Parent = parent;
            Properties = cloneFrom.Properties.Select(p => new TemplatedRealmProperty<TVal>(compilationContext, cloneFrom.PropertyKey, p.Options)).ToImmutableArray();
        }
    }
}
