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
            public static int StrengthAdded2(IAppliedRuleset ruleset, IRealmPropContext SpawnedCreature)
            {
                return 0;
               // return ruleset.ValueOf(RealmPropertyInt.Creature_Attributes_StrengthAdded, ("SpawnedCreature", SpawnedCreature));
            }
        }
    }
}
