using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Rulesets
{
    internal abstract class TemplatedRealmPropertyGroup
    {
        internal required string Name { get; init; }
    }

    /// <summary>
    /// Represents a collection of all properties associated with a single ruleset, for a single key.
    /// There will be more than one item in this collection if multiple scopes are defined for a ruleset with the same property key
    /// TODO: Try to get more deduplicateddata in here instead of ActiveRealmPropertyGroup, to improve performance
    /// </summary>
    internal class TemplatedRealmPropertyGroup<TVal> : TemplatedRealmPropertyGroup
        where TVal : IEquatable<TVal>
    {
        internal required ImmutableList<TemplatedRealmProperty<TVal>> Properties { get; init; }
    }
}
