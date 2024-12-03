﻿
// THIS FILE IS AUTOMATICALLY GENERATED
namespace ACRealms;

public static partial class Props
{
    public static partial class Peripheral
    {
        public static class ClassicalInstance
        {
            /// <summary>For the landblocks matching the name of this dungeon set, classical instances will be enabled for players with the ClassicalInstancesActive boolean property, if the ruleset also has UseClassicalInstances set to true.</summary>
            public const RealmPropertyStringStaging DungeonSet = RealmPropertyStringStaging.Peripheral_ClassicalInstance_DungeonSet;
            /// <summary>Enables classical instances for the realm.  Use the dungeon-sets peripheral configuration file to define landblocks for which players will be given private instances.</summary>
            public const RealmPropertyBoolStaging Enabled = RealmPropertyBoolStaging.Peripheral_ClassicalInstance_Enabled;
            /// <summary>If enabled, classical instances will be assigned per account instead of per character.  If a player owns a house in a classical instance, they will lose access to it if this is toggled.</summary>
            public const RealmPropertyBoolStaging ShareWithPlayerAccount = RealmPropertyBoolStaging.Peripheral_ClassicalInstance_ShareWithPlayerAccount;
            /// <summary>If enabled, classical instances will be active regardless of the character's PropertyBool.ClassicalInstancesActive.</summary>
            public const RealmPropertyBoolStaging IgnoreCharacterProp = RealmPropertyBoolStaging.Peripheral_ClassicalInstance_IgnoreCharacterProp;
            /// <summary>If enabled, classical instances will be active regardless of the character's location.  This is not recommended for realms other than true solo-self-found realms, and is considered an advanced feature.</summary>
            public const RealmPropertyBoolStaging EnableForAllLandblocksDangerous = RealmPropertyBoolStaging.Peripheral_ClassicalInstance_EnableForAllLandblocksDangerous;
            /// <summary>If enabled, players can purchase houses in the instance ID assigned to the player.</summary>
            public const RealmPropertyBoolStaging AllowHousingPurchase = RealmPropertyBoolStaging.Peripheral_ClassicalInstance_AllowHousingPurchase;
        }
    }
}