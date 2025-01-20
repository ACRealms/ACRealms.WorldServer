using ACRealms.Rulesets.Contexts;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Rulesets
{
    internal class RealmPropertyScopeOptions
    {
        readonly FrozenDictionary<string, IRealmPropertyScope> Scopes;
    }
}
