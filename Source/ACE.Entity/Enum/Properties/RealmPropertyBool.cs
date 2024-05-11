using System.ComponentModel;

namespace ACE.Entity.Enum.Properties
{
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
        HasRecalls = 9
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
