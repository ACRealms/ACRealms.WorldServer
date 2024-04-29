using System.ComponentModel;

namespace ACE.Entity.Enum.Properties
{
    public enum RealmPropertyBool : ushort
    {
        [RealmPropertyBool(defaultValue: false)]
        Undef                            = 0,

        [RealmPropertyBool(false)]
        IsNeutralZone = 1,

        /// <summary>
        /// Standard "Red server" rules. Players are always Player Killer status unless recently killed by a player killer
        /// </summary>
        [RealmPropertyBool(false)]
        IsPKOnly = 2,

        /// <summary>
        /// If true, this realm may have an expiration timestamp. When this timestamp is reached,
        /// all landblocks will use the parent realm instead. Any players with a homeworld underneath this realm
        /// will be moved to the parent realm.
        /// </summary>
        [RealmPropertyBool(false)]
        IsTemporaryRealm = 3,

        [RealmPropertyBool(false)]
        IsDuelingRealm = 4,

        [RealmPropertyBool(false)]
        CanBeHomeworld = 5,

        [RealmPropertyBool(false)]
        HideoutEnabled = 6,
        /// <summary>
        /// Players with a homeworld of this realm may enter the neutral zone if true
        /// </summary>
        [RealmPropertyBool(false)]
        CanInteractWithNeutralZone = 7,

        [RealmPropertyBool(true)]
        SpellCastingPKDoubleCollisionCheck = 8,

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
