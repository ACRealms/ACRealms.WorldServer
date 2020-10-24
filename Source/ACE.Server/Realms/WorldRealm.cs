using ACE.Entity;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.Managers;
using ACE.Server.WorldObjects;
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

        internal uint GetDefaultInstanceID(Player player)
        {
            if (RealmManager.TryParseReservedRealm(Realm.Id, out var r))
            {
                switch(r)
                {
                    default:
                        return ACE.Entity.Position.InstanceIDFromVars(Realm.Id, (ushort)player.Account.AccountId, false);
                }
            }
            return StandardRules.GetDefaultInstanceID();
        }

        internal Position DefaultStartingLocation(Player player)
        {

            if (StandardRules.GetProperty(RealmPropertyBool.IsDuelingRealm))
            {
                //Adventurer's Haven
                //0x01AC0118[29.684622 - 30.072382 0.010000] - 0.027857 0.999612 0.000000 0.000000
                return DuelRealmHelpers.GetDuelingAreaDrop(this);
            }
            else
            {
                //Holtburg
                return new Position(0xA9B40019, 84f, 7.1f, 94.005005f, 0f, 0f, -0.078459f, 0.996917f, GetDefaultInstanceID(player));
            }
        }

        internal bool IsWhitelistedLandblock(ushort landblock)
        {
            if (StandardRules.GetProperty(RealmPropertyBool.IsDuelingRealm))
                return RealmConstants.DuelLandblocks.Contains(landblock);
            return true;
        }
    }
}
