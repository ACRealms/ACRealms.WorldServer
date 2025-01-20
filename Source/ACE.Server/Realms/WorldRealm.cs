using ACE.Entity;
using ACE.Entity.ACRealms;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Enum.RealmProperties;
using ACE.Entity.Models;
using ACE.Server.Managers;
using ACE.Server.WorldObjects;
using System;
using System.Collections.Generic;
using System.Text;
using ACRealms;
using ACRealms.RealmProps.Enums;

namespace ACE.Server.Realms
{
    internal sealed class WorldRealm(ACRealms.Rulesets.Realm realm, RulesetTemplate rulesetTemplate)
        : ACRealms.Rulesets.WorldRealmBase(realm, (RulesetBase)rulesetTemplate)
    {
        public RulesetTemplate RulesetTemplate { get; } = rulesetTemplate;
        public AppliedRuleset StandardRules { get; } = AppliedRuleset.MakeRerolledRuleset(rulesetTemplate, rulesetTemplate.Context);

        // This isn't really used yet
        public bool NeedsRefresh { get; internal set; }

        internal InstancedPosition DefaultStartingLocation(Player player)
        {
           // Props.Creature.Attributes.CoordinationMultiplier.
            if (StandardRules.GetProperty(Props.Pvp.World.IsDuelingRealm))
            {
                //Adventurer's Haven
                //0x01AC0118[29.684622 - 30.072382 0.010000] - 0.027857 0.999612 0.000000 0.000000
                return DuelRealmHelpers.GetDuelingAreaDrop(player);
            }
            else
            {
                //Holtburg
                return new LocalPosition(0xA9B40019, 84f, 7.1f, 94.005005f, 0f, 0f, -0.078459f, 0.996917f).AsInstancedPosition(player, PlayerInstanceSelectMode.HomeRealm);
            }
        }

        internal bool IsWhitelistedLandblock(ushort landblock)
        {
            if (StandardRules.GetProperty(Props.Pvp.World.IsDuelingRealm))
                return RealmConstants.DuelLandblocks.Contains(landblock);
            return true;
        }
    }
}
