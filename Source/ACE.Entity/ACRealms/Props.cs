global using RealmPropertyInt = ACE.Entity.Enum.Properties.RealmPropertyInt;
global using RealmPropertyInt64 = ACE.Entity.Enum.Properties.RealmPropertyInt64;
global using RealmPropertyFloat = ACE.Entity.Enum.Properties.RealmPropertyFloat;
global using RealmPropertyBool = ACE.Entity.Enum.Properties.RealmPropertyBool;
global using RealmPropertyString = ACE.Entity.Enum.Properties.RealmPropertyString;
global using RealmPropertyIntStaging = ACE.Entity.Enum.Properties.RealmPropertyIntStaging;
//global using RealmPropertyInt64Staging = ACE.Entity.Enum.Properties.RealmPropertyInt64Staging;
//global using RealmPropertyFloatStaging = ACE.Entity.Enum.Properties.RealmPropertyFloatStaging;
global using RealmPropertyBoolStaging = ACE.Entity.Enum.Properties.RealmPropertyBoolStaging;
global using RealmPropertyStringStaging = ACE.Entity.Enum.Properties.RealmPropertyStringStaging;
namespace ACRealms.Props
{
    public enum RealmPropertyInt2
    {
        None,
        Creature_Attributes_StrengthAddedA,
        Creature_Attributes_EnduranceAddedA,
        Creature_Attributes_CoordinationAddedA
    }
    public static class Creature
    {
        public static class Attributes
        {
            /// <summary>All creatures will have this value added to their Strength attribute</summary>
            public const RealmPropertyInt2 StrengthAddedA = RealmPropertyInt2.Creature_Attributes_StrengthAddedA;
            /// <summary>All creatures will have this value added to their Endurance attribute</summary>
            public const RealmPropertyInt2 EnduranceAddedA = RealmPropertyInt2.Creature_Attributes_EnduranceAddedA;
            /// <summary>All creatures will have this value added to their Coordination attribute</summary>
            public const RealmPropertyInt2 CoordinationAddedA = RealmPropertyInt2.Creature_Attributes_CoordinationAddedA;
            /// <summary>All creatures will have this value added to their Quickness attribute</summary>
          //  public const RealmPropertyInt2 QuicknessAddedA = RealmPropertyInt2.Creature_Attributes_QuicknessAddedA;
            /// <summary>All creatures will have this value added to their Focus attribute</summary>
         //   public const RealmPropertyInt2 FocusAddedA = RealmPropertyInt2.Creature_Attributes_FocusAddedA;
            /// <summary>All creatures will have this value added to their Self attribute</summary>
          //  public const RealmPropertyInt2 SelfAddedA = RealmPropertyInt2.Creature_Attributes_SelfAddedA;
        }
    }
}
