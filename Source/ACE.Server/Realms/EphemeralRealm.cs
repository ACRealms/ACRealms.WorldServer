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

        public RulesetTemplate RulesetTemplate { get; set; }

        private EphemeralRealm() { }
        private EphemeralRealm(Player owner, RulesetTemplate template)
        {
            this.Owner = owner;
            this.RulesetTemplate = template;
        }

        public static EphemeralRealm Initialize(Player owner, Realm realm)
        {
            var baseRealm = RealmManager.GetBaseRealm(owner);
            return Initialize(owner, baseRealm, realm);
        }

        private static EphemeralRealm Initialize(Player owner, WorldRealm baseRealm, Realm appliedRealm)
        {
            var template = RealmManager.GetEphemeralRealmRulesetTemplate(baseRealm, appliedRealm);
            if (template != null) //cached
                return new EphemeralRealm(owner, template);

            template = RulesetTemplate.MakeRuleset(baseRealm.RulesetTemplate, appliedRealm);
            template = RealmManager.SyncRulesetForEphemeralRealm(baseRealm, appliedRealm, template);
            return new EphemeralRealm(owner, template);
        }
    }
}
