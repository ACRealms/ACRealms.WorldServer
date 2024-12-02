using ACE.Entity.Enum.RealmProperties;
using System;
using System.ComponentModel;
using RealmPropertyIntAttribute = ACE.Entity.Enum.Properties.RealmPropertyPrimaryMinMaxAttribute<int>;

namespace ACE.Entity.Enum.Properties.STAGING;

[RequiresPrimaryAttribute<RealmPropertyPrimaryMinMaxAttribute<int>, int>]
public enum RealmPropertyInt : ushort
{
    Undef = 0,

    [Description("All creatures will have this value added to their Coordination attribute")]
    [RealmPropertyIntAttribute(0, -10000000, 10000000)]
    Creature_Attributes_CoordinationAdded,

    [Description("All creatures will have this value added to their Endurance attribute")]
    [RealmPropertyIntAttribute(0, -10000000, 10000000)]
    Creature_Attributes_EnduranceAdded,

    [Description("All creatures will have this value added to their Focus attribute")]
    [RealmPropertyIntAttribute(0, -10000000, 10000000)]
    Creature_Attributes_FocusAdded,

    [Description("All creatures will have this value added to their Quickness attribute")]
    [RealmPropertyIntAttribute(0, -10000000, 10000000)]
    Creature_Attributes_QuicknessAdded,

    [Description("All creatures will have this value added to their Self attribute")]
    [RealmPropertyIntAttribute(0, -10000000, 10000000)]
    Creature_Attributes_SelfAdded,

    [Description("All creatures will have this value added to their Strength attribute")]
    [RealmPropertyIntAttribute(0, -10000000, 10000000)]
    Creature_Attributes_StrengthAdded,

    [Description("Landblocks which have been inactive for this many minutes will be unloaded")]
    [RealmPropertyIntAttribute(5, 1, 1440)]
    Core_Landblock_UnloadInterval,
}
