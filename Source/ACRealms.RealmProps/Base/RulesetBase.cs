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
        int ValueOf(RealmPropertyInt prop, params IReadOnlyCollection<(string, IRealmPropContext)> ctx);
        long ValueOf(RealmPropertyInt64 prop, params IReadOnlyCollection<(string, IRealmPropContext)> ctx);
        string ValueOf(RealmPropertyString prop, params IReadOnlyCollection<(string, IRealmPropContext)> ctx);
        double ValueOf(RealmPropertyFloat prop, params IReadOnlyCollection<(string, IRealmPropContext)> ctx);
        bool ValueOf(RealmPropertyBool prop, params IReadOnlyCollection<(string, IRealmPropContext)> ctx);
    }
}
