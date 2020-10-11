using ACE.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACE.Server.Realms
{
    public class WorldRealm
    {
        public Realm Realm { get; set; }
        public RulesetTemplate RulesetTemplate { get; set; }
        public AppliedRuleset StandardRules { get; set; }
        public bool NeedsRefresh { get; internal set; }

        private WorldRealm() { }

        public WorldRealm(Realm realmEntity, RulesetTemplate ruleset)
        {
            Realm = realmEntity;
            RulesetTemplate = ruleset;
            StandardRules = AppliedRuleset.MakeRerolledRuleset(RulesetTemplate);
        }
    }
}
