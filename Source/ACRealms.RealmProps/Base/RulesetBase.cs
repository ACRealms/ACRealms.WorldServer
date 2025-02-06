using ACRealms.RealmProps.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms
{
    /// <summary>
    /// Base ruleset class for visibility by the core library
    /// </summary>
    public abstract class RulesetBase
    {
        
    }

    /// <summary>
    /// Base interface for AppliedRuleset, for visiblity from the core library
    /// </summary>
    public interface IAppliedRuleset
    {
        /// <summary>
        /// Underlying fetch of realm property. Not intended for direct use
        /// </summary>
        int ValueOf(RealmPropertyInt prop, params IReadOnlyCollection<ValueTuple<string, ICanonicalContextEntity>> ctx);

        /// <summary>
        /// Underlying fetch of realm property. Not intended for direct use
        /// </summary>
        long ValueOf(RealmPropertyInt64 prop, params IReadOnlyCollection<ValueTuple<string, ICanonicalContextEntity>> ctx);

        /// <summary>
        /// Underlying fetch of realm property. Not intended for direct use
        /// </summary>
        string ValueOf(RealmPropertyString prop, params IReadOnlyCollection<ValueTuple<string, ICanonicalContextEntity>> ctx);

        /// <summary>
        /// Underlying fetch of realm property. Not intended for direct use
        /// </summary>
        double ValueOf(RealmPropertyFloat prop, params IReadOnlyCollection<ValueTuple<string, ICanonicalContextEntity>> ctx);

        /// <summary>
        /// Underlying fetch of realm property. Not intended for direct use
        /// </summary>
        bool ValueOf(RealmPropertyBool prop, params IReadOnlyCollection<ValueTuple<string, ICanonicalContextEntity>> ctx);
    }
}
