using ACE.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACE.Server.Realms
{
    public class WorldRealm
    {
        public Realm Realm { get; set; }
        public AppliedRuleset Ruleset { get; set; }
        public bool NeedsRefresh { get; internal set; }

        private WorldRealm() { }

        public WorldRealm(Realm realmEntity, AppliedRuleset ruleset)
        {
            Realm = realmEntity;
            Ruleset = ruleset;
        }
    }
}
