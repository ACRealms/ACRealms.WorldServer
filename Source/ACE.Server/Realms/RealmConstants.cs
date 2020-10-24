using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Server.Managers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace ACE.Server.Realms
{
    //Todo: Support this in JSON
    public static class RealmConstants
    {
        public static readonly ImmutableHashSet<ushort> DuelLandblocks;
        public static readonly ImmutableList<AugmentationType> DuelAugmentations;
        public static readonly Position DuelStagingAreaDrop = new Position(0x01AC0118, 29.684622f, -30.072382f, 0.005000f, 0.000000f, 0.000000f, 0.035476156f, -0.9993705f, 0);
        static RealmConstants()
        {
            DuelLandblocks = new List<ushort>()
            {
                0x01AC,
                0x039D
            }.ToImmutableHashSet();

            DuelAugmentations = new List<AugmentationType>() {
                AugmentationType.AllStats,
                AugmentationType.BonusSalvage,
                AugmentationType.BonusSalvage,
                AugmentationType.BonusSalvage,
                AugmentationType.BonusSalvage,
                AugmentationType.BurdenLimit,
                AugmentationType.BurdenLimit,
                AugmentationType.BurdenLimit,
                AugmentationType.BurdenLimit,
                AugmentationType.BurdenLimit,
                AugmentationType.CritChance,
                AugmentationType.CritDamage,
                AugmentationType.CritProtect,
                AugmentationType.Damage,
                AugmentationType.DamageResist,
                AugmentationType.DeathItemLoss,
                AugmentationType.DeathItemLoss,
                AugmentationType.DeathItemLoss,
                AugmentationType.DeathSpellLoss,
                AugmentationType.FociCreature,
                AugmentationType.FociItem,
                AugmentationType.FociLife,
                AugmentationType.FociWar,
                AugmentationType.Magic,
                AugmentationType.Melee,
                AugmentationType.Missile,
                AugmentationType.PackSlot,
                AugmentationType.RegenBonus,
                AugmentationType.SpellDuration,
                AugmentationType.SpellDuration,
                AugmentationType.SpellDuration,
                AugmentationType.SpellDuration,
                AugmentationType.SpellDuration
            }.ToImmutableList();
        }
    }
}
