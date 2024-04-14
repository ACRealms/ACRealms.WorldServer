using System.ComponentModel;

namespace ACE.Entity.Enum.Properties
{
    public enum RealmPropertyFloat : ushort
    {
        [RealmPropertyFloat(0f, 0f, 0f)]
        Undef                          = 0,

        [RealmPropertyFloat(1f, 0.1f, 5f)]
        SpellCasting_MoveToState_UpdatePosition_Threshold = 1,

        [RealmPropertyFloat(5f, 1f, 360f)]
        Spellcasting_Max_Angle = 2,

        [RealmPropertyFloat(6f, 1f, 1000f)]
        SpellCastingPvPWindupMaxMove = 3,

        [RealmPropertyFloat(1f, 0.001f, 100f)]
        CreatureSpawnHPMultiplier = 4,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureSpawnRateMultiplier = 5,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureDamageMultiplier = 6,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureStrengthMultiplier = 7,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureEnduranceMultiplier = 8,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureCoordinationMultiplier = 9,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureQuicknessMultiplier = 10,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureFocusMultiplier = 11,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureSelfMultiplier = 12,

        /*Below is not implemented*/
        [RealmPropertyFloat(1f, 0.01f, 100f)]
        PlayerDamageGlobalMultiplier = 13,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        PlayerDamageMeleeMultiplier = 14,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        PlayerDamageMissileMultiplier = 15,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        PlayerDamageMagicMultiplier = 16,

        [RealmPropertyFloat(1f, 0f, 1f)]
        PlayerChanceToEvadeMeleeCap = 17,

        [RealmPropertyFloat(1f, 0f, 1f)]
        PlayerChanceToDodgeMissileCap = 18,

        [RealmPropertyFloat(1f, 0f, 1f)]
        PlayerChanceToResistMagicCap = 19,

        [RealmPropertyFloat(1f, 0f, 1f)]
        CreatureChanceToEvadeMeleeCap = 20,

        [RealmPropertyFloat(1f, 0f, 1f)]
        CreatureChanceToDodgeMissileCap = 21,

        [RealmPropertyFloat(1f, 0f, 1f)]
        CreatureChanceToResistMagicCap = 22,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureDamageGlobalMultiplier = 23,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureDamageMeleeMultiplier = 24,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        CreatureDamageMagicMultiplier = 25,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        PlayerCriticalDamageMultiplier = 26,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        HealingKitHealMultiplier = 27,

        [RealmPropertyFloat(1f, 0.01f, 100f)]
        FoodRestoreAmountMultiplier = 28,

        [RealmPropertyFloat(1f, 0.001f, 1000f)]
        ExperienceMultiplierAll = 29,

        [RealmPropertyFloat(1f, 0.001f, 1000f)]
        ExperienceMultiplierQuest = 30,

        [RealmPropertyFloat(1f, 0.001f, 1000f)]
        ExperienceMultiplierKills = 31,

        [RealmPropertyFloat(33f, -10f, 100f)]
        TinkeringBaseImbueChanceCap = 32,
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
