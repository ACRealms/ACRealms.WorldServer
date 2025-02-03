using ACRealms.RealmProps.Contexts;
using System;

namespace ACRealms;

/// <summary> The root namespace for all realm properties </summary>
public static partial class Props {}

public static partial class Props
{
    public static partial class Creature
    {
        public static partial class Attributes
        {
            public static int StrengthAdded2(IAppliedRuleset ruleset, ICanonicalContextEntity SpawnedCreature)
            {
                return ruleset.ValueOf(ACRealms.Props.Creature.Attributes.StrengthAdded, ("SpawnedCreature", SpawnedCreature));
            }
        }
    }
}
