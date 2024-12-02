
namespace ACRealms.Props; //ACRealms.RealmProps.Peripheral

public static class ClassicalInstance
{
    /// <summary>For the landblocks matching the name of this dungeon set, classical instances will be enabled for players with the ClassicalInstancesActive boolean property, if the ruleset also has UseClassicalInstances set to true.</summary>
    public const RealmPropertyStringStaging DungeonSet = RealmPropertyStringStaging.Peripheral_ClassicalInstance_DungeonSet;
    /// <summary>No description</summary>
    public const RealmPropertyBoolStaging Enabled = RealmPropertyBoolStaging.Peripheral_ClassicalInstance_Enabled;
    /// <summary>No description</summary>
    public const RealmPropertyBoolStaging ShareWithPlayerAccount = RealmPropertyBoolStaging.Peripheral_ClassicalInstance_ShareWithPlayerAccount;
    /// <summary>No description</summary>
    public const RealmPropertyBoolStaging IgnoreCharacterProp = RealmPropertyBoolStaging.Peripheral_ClassicalInstance_IgnoreCharacterProp;
    /// <summary>No description</summary>
    public const RealmPropertyBoolStaging EnableForAllLandblocksDangerous = RealmPropertyBoolStaging.Peripheral_ClassicalInstance_EnableForAllLandblocksDangerous;
    /// <summary>If enabled, players can purchase houses in the instance ID assigned to the player.</summary>
    public const RealmPropertyBoolStaging AllowHousingPurchase = RealmPropertyBoolStaging.Peripheral_ClassicalInstance_AllowHousingPurchase;
}