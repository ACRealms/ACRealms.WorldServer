
namespace ACRealms.Props; //ACRealms.RealmProps.Peripheral

public static class ClassicalInstance
{
    /// <summary>For the landblocks matching the name of this dungeon set, classical instances will be enabled for players with the ClassicalInstancesActive boolean property, if the ruleset also has UseClassicalInstances set to true.</summary>
    public const RealmPropertyString DungeonSet = RealmPropertyString.Peripheral_ClassicalInstance_DungeonSet;
    /// <summary>No description</summary>
    public const RealmPropertyBool Enabled = RealmPropertyBool.Peripheral_ClassicalInstance_Enabled;
    /// <summary>No description</summary>
    public const RealmPropertyBool ShareWithPlayerAccount = RealmPropertyBool.Peripheral_ClassicalInstance_ShareWithPlayerAccount;
    /// <summary>No description</summary>
    public const RealmPropertyBool IgnoreCharacterProp = RealmPropertyBool.Peripheral_ClassicalInstance_IgnoreCharacterProp;
    /// <summary>No description</summary>
    public const RealmPropertyBool EnableForAllLandblocksDangerous = RealmPropertyBool.Peripheral_ClassicalInstance_EnableForAllLandblocksDangerous;
    /// <summary>If enabled, players can purchase houses in the instance ID assigned to the player.</summary>
    public const RealmPropertyBool AllowHousingPurchase = RealmPropertyBool.Peripheral_ClassicalInstance_AllowHousingPurchase;
}