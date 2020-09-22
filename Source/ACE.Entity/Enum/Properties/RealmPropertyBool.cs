using System.ComponentModel;

namespace ACE.Entity.Enum.Properties
{
    public enum RealmPropertyBool : ushort
    {
        Undef                            = 0,
        IsNeutralZone                    = 1,

        /// <summary>
        /// Standard "Red server" rules. Players are always Player Killer status unless recently killed by a player killer
        /// </summary>
        IsPKOnly                         = 2,

        /// <summary>
        /// If true, this realm may have an expiration timestamp. When this timestamp is reached,
        /// all landblocks will use the parent realm instead. Any players with a homeworld underneath this realm
        /// will be moved to the parent realm.
        /// </summary>
        IsTemporaryRealm                 = 3,
        CanBeHomeworld                   = 4,

        /// <summary>
        /// Players with a homeworld of this realm may enter the neutral zone if true
        /// </summary>
        CanInteractWithNeutralZone       = 5
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
