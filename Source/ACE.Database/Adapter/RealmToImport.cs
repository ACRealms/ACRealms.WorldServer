using ACE.Database.Models.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACE.Database.Adapter
{
    public class RealmToImport
    {
        public Realm Realm { get; set; }
        public List<RealmRulesetLinks> Links { get; set; }
    }
}
