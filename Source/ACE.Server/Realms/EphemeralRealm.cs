using ACE.Entity.Models;
using ACE.Server.Managers;
using ACE.Server.WorldObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACE.Server.Realms
{
    /// <summary>
    /// Represents a temporary realm created on a landblock by landblock basis (such as an 'instance' in the traditional sense)
    /// </summary>
    public class EphemeralRealm
    {
        public Player Owner { get; set; }
        public List<Player> AllowedPlayers { get; } = new List<Player>();
        public bool OpenToFellowship { get; set; } = true;
        public DateTime ExpiresAt = DateTime.UtcNow.AddDays(1);
        public AppliedRuleset Ruleset { get; set; }

        private EphemeralRealm() { }
        private EphemeralRealm(Player owner, AppliedRuleset ruleset)
        {
            this.Owner = owner;
            this.Ruleset = ruleset;
        }

        public static EphemeralRealm Initialize(Player owner, Realm rulesetTemplate)
        {
            var baseRealm = RealmManager.GetBaseRealm(owner);
            return Initialize(owner, baseRealm, rulesetTemplate);
        }

        private static EphemeralRealm Initialize(Player owner, WorldRealm baseRealm, Realm rulesetTemplate)
        {
            var ruleset = AppliedRuleset.ApplyRuleset(baseRealm.Ruleset, rulesetTemplate);
            return new EphemeralRealm(owner, ruleset);
        }
    }
}
