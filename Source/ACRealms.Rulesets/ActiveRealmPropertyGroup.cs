using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Rulesets
{
    internal interface IRealmPropertyGroup<TVal>
        where TVal : IEquatable<TVal>
    {
        RealmPropertyGroupOptions Options { get; init; }
        IEnumerable<IAppliedRealmProperty<TVal>> Properties { get; init;  }
        IRealmPropertyGroup<TVal> Parent { get; init; }
    }

    internal abstract record ActiveRealmPropertyGroup
    {
    }

    /// <summary>
    /// Represents a collection of all properties associated with a single ruleset, for a single key.
    /// There will be more than one item in this collection if multiple scopes are defined for a ruleset with the same property key
    /// </summary>
    internal record ActiveRealmPropertyGroup<TVal> : ActiveRealmPropertyGroup, IRealmPropertyGroup<TVal>
        where TVal : IEquatable<TVal>
    {
        IEnumerable<IAppliedRealmProperty<TVal>> IRealmPropertyGroup<TVal>.Properties { get; init; }
        IEnumerable<ActiveRealmProperty<TVal>> Properties { get; init; }
        IRealmPropertyGroup<TVal> IRealmPropertyGroup<TVal>.Parent { get; init; }
        public ActiveRealmPropertyGroup<TVal> Parent { get; }
        RealmPropertyGroupOptions IRealmPropertyGroup<TVal>.Options { get; init; }
        public RealmPropertyGroupOptions<TVal> Options { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worldCtx"></param>
        /// <param name="val"></param>
        /// <returns>True if the property was successfully eval</returns>
        public bool TryEval(IReadOnlyCollection<(string, IRealmPropContext)> worldCtx, out TVal val)
        {
            foreach(var prop in Properties)
            {
                if (prop.TryEval(worldCtx, out val))
                    return true;
            }

            if (Parent != null)
            {
                return Parent.TryEval(worldCtx, out val);
            }
            else
            {
                val = Options.Prototype.HardDefaultValue;
                return false;
            }
        }
    }
}
