using System.ComponentModel;

namespace ACE.Entity.Enum.Properties
{
    public enum RealmPropertyFloat : ushort
    {
        [RealmPropertyFloat(0f, 0f, 0f)]
        Undef                          = 0,

        [RealmPropertyFloat(1f, 0.1f, 5f)]
        SpellCasting_MoveToState_UpdatePosition_Threshold,

        [RealmPropertyFloat(5f, 1f, 360f)]
        Spellcasting_Max_Angle,

        [RealmPropertyFloat(6f, 1f, 1000f)]
        SpellCastingPvPWindupMaxMove,

        [RealmPropertyFloat(1f, 0.001f, 100f)]
        CreatureSpawnHPMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureSpawnRateMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureDamageMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureStrengthMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureEnduranceMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureCoordinationMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureQuicknessMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureFocusMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureSelfMultiplier,

        /*Below is not implemented*/
        [RealmPropertyFloat(1f, 0.01f, 100f)]
        PlayerDamageGlobalMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        PlayerDamageMeleeMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        PlayerDamageMissileMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        PlayerDamageMagicMultiplier,

        [RealmPropertyFloat(1f, 0f, 1f)]
        PlayerChanceToEvadeMeleeCap,

        [RealmPropertyFloat(1f, 0f, 1f)]
        PlayerChanceToDodgeMissileCap,

        [RealmPropertyFloat(1f, 0f, 1f)]
        PlayerChanceToResistMagicCap,

        [RealmPropertyFloat(1f, 0f, 1f)]
        CreatureChanceToEvadeMeleeCap,

        [RealmPropertyFloat(1f, 0f, 1f)]
        CreatureChanceToDodgeMissileCap,

        [RealmPropertyFloat(1f, 0f, 1f)]
        CreatureChanceToResistMagicCap,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureDamageGlobalMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureDamageMeleeMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureDamageMagicMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        PlayerCriticalDamageMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        HealingKitHealMultiplier,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        FoodRestoreAmountMultiplier,

        [RealmPropertyFloat(1f, 0.001f, 1000f)]
        ExperienceMultiplierAll,

        [RealmPropertyFloat(1f, 0.001f, 1000f)]
        ExperienceMultiplierQuest,

        [RealmPropertyFloat(1f, 0.001f, 1000f)]
        ExperienceMultiplierKills,

        [RealmPropertyFloat(33f, -10f, 100f)]
        TinkeringBaseImbueChanceCap,
    }

    public static class RealmPropertyFloatExtensions
    {
        public static string GetDescription(this RealmPropertyFloat prop)
        {
            var description = prop.GetAttributeOfType<DescriptionAttribute>();
            return description?.Description ?? prop.ToString();
        }
    }
}
