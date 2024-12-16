using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Rulesets.Loader
{
    internal class RealmToImport
    {
        public required DBOld.Realm Realm { get; set; }
        public required List<DBOld.RealmRulesetLinks> Links { get; set; }
    }
}
