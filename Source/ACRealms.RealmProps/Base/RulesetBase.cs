using ACRealms.RealmProps.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms
{
    public abstract class RulesetBase
    {
        
    }

    public interface IAppliedRuleset
    {
        int ValueOf(RealmPropertyInt prop, params IReadOnlyCollection<ValueTuple<string, ICanonicalContextEntity>> ctx);
        long ValueOf(RealmPropertyInt64 prop, params IReadOnlyCollection<ValueTuple<string, ICanonicalContextEntity>> ctx);
        string ValueOf(RealmPropertyString prop, params IReadOnlyCollection<ValueTuple<string, ICanonicalContextEntity>> ctx);
        double ValueOf(RealmPropertyFloat prop, params IReadOnlyCollection<ValueTuple<string, ICanonicalContextEntity>> ctx);
        bool ValueOf(RealmPropertyBool prop, params IReadOnlyCollection<ValueTuple<string, ICanonicalContextEntity>> ctx);
    }
}
