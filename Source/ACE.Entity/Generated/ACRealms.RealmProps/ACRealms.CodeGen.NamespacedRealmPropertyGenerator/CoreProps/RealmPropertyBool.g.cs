using ACE.Entity.Enum.RealmProperties;
using System;
using System.ComponentModel;
using RealmPropertyBoolAttribute = ACE.Entity.Enum.Properties.RealmPropertyPrimaryAttribute<bool>;

namespace ACE.Entity.Enum.Properties.STAGING;

[RequiresPrimaryAttribute<RealmPropertyPrimaryAttribute<bool>, bool>]
public enum RealmPropertyBool : ushort
{
    Undef = 0,

    [Description("If enabled, players can purchase houses in the instance ID assigned to the player.")]
    [RealmPropertyBoolAttribute(false)]
    Peripheral_ClassicalInstance_AllowHousingPurchase,

    [Description("No description")]
    [RealmPropertyBoolAttribute(false)]
    Peripheral_ClassicalInstance_Enabled,

    [Description("No description")]
    [RealmPropertyBoolAttribute(false)]
    Peripheral_ClassicalInstance_EnableForAllLandblocksDangerous,

    [Description("No description")]
    [RealmPropertyBoolAttribute(false)]
    Peripheral_ClassicalInstance_IgnoreCharacterProp,

    [Description("No description")]
    [RealmPropertyBoolAttribute(false)]
    Peripheral_ClassicalInstance_ShareWithPlayerAccount,
}
