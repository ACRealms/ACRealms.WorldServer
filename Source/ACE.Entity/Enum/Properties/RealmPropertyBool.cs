using System.ComponentModel;

using RealmPropertyBoolAttribute = ACE.Entity.Enum.Properties.RealmPropertyPrimaryAttribute<bool>;

namespace ACE.Entity.Enum.Properties
{

    #pragma warning disable IDE0001
    [RequiresPrimaryAttribute<RealmPropertyPrimaryAttribute<bool>, bool>]
    #pragma warning restore IDE0001
    public enum RealmPropertyBool : ushort
    {
        [RealmPropertyBool(defaultValue: false)]
        Undef                            = 0,

        /// <summary>
        /// If true, any player with a home realm with the property \"CanInteractWithNeutralZone\" may travel to this realm
        /// </summary>
        [Description("If true, any player with a home realm with the property \"CanInteractWithNeutralZone\" may travel to this realm")]
        [RealmPropertyBool(false)]
        IsNeutralZone = 1,

        /// <summary>
        /// Standard "Red server" rules. Players are always Player Killer status unless recently killed by a player killer
        /// </summary>
        [Description("Standard \"Red server\" rules. Players are always Player Killer status unless recently killed by a player killer")]
        [RealmPropertyBool(false)]
        IsPKOnly = 2,

        /// <summary>
        /// If true, this realm may have an expiration timestamp. When this timestamp is reached,
        /// all landblocks will use the parent realm instead. Any players with a homeworld underneath this realm
        /// will be moved to the parent realm.
        /// </summary>
        [Description(@"If true, this realm may have an expiration timestamp. When this timestamp is reached,
all landblocks will use the parent realm instead. Any players with a homeworld underneath this realm
will be moved to the parent realm.")]
        [RealmPropertyBool(false)]
        IsTemporaryRealm = 3,

        [Description("Set this to true to designate this realm as the dueling realm. Allows /rebuff command, all summoned portals lead to ephemeral instances, and no vitae.")]
        [RealmPropertyBool(false)]
        IsDuelingRealm = 4,

        [Description("If true, realm will be listed as available from the realm selector (Blaine)")]
        [RealmPropertyBool(false)]
        CanBeHomeworld = 5,

        [Description("Allows players to use the /hideout command to teleport to a personal instanced hideout")]
        [RealmPropertyBool(false)]
        HideoutEnabled = 6,

        /// <summary>
        /// Players with a homeworld of this realm may enter the neutral zone if true
        /// </summary>
        [Description("Players with a homeworld of this realm may enter the neutral zone if true")]
        [RealmPropertyBool(false)]
        CanInteractWithNeutralZone = 7,

        [Description("Enables a double collision algorithm for projectile spells. In theory, this makes them harder to dodge.")]
        [RealmPropertyBool(true)]
        SpellCastingPKDoubleCollisionCheck = 8,

        [Description("Enables the console commands for recalling to various locations (marketplace, house, etc)")]
        [RealmPropertyBool(true)]
        HasRecalls = 9,

        [Description("Enables classical instances for the realm. Use the dungeon-sets peripheral configuration file to define landblocks for which players will be given private instances")]
        [RealmPropertyBool(false)]
        UseClassicalInstances = 10,

        [Description("If enabled, classical instances will be assigned per account instead of per character. If a player owns a house in a classical instance, they will lose access to it if this is toggled.")]
        [RealmPropertyBool(false)]
        ClassicalInstances_ShareWithPlayerAccount = 11,

        [Description("If enabled, classical instances will be active regardless of the character's PropertyBool.ClassicalInstancesActive")]
        [RealmPropertyBool(false)]
        ClassicalInstances_IgnoreCharacterProp = 12,

        [Description("If enabled, classical instances will be active regardless of the character's location. This is not recommended for realms other than true solo-self-found realms, and is considered an advanced feature.")]
        [RealmPropertyBool(false)]
        ClassicalInstances_EnableForAllLandblocks_Dangerous = 13,

        [Description("If enabled, players can purchase houses in the instance ID assigned to the player.")]
        [RealmPropertyBool(false)]
        ClassicalInstances_AllowHousingPurchase = 14,

        [Description("Disables the primary instance restriction from house purchases. This is separate from the home realm restriction. Consider using ClassicalInstances_AllowHousingPurchase instead.")]
        [RealmPropertyBool(false)]
        Housing_DisablePrimaryInstancePurchaseRestriction = 15,

        [Description("Disables the home realm requirement for house purchases. This configuration should be applied to the realm the house is located in, not the home realm of the player.")]
        [RealmPropertyBool(false)]
        Housing_DisableHomeRealmPurchaseRestriction = 16
    }

    public static class RealmPropertyBoolExtensions
    {
        public static string GetDescription(this RealmPropertyBool prop)
        {
            var description = prop.GetAttributeOfType<DescriptionAttribute>();
            return description?.Description ?? prop.ToString();
        }
    }
}
