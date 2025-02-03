using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Entity.Enum.Properties
{
    public static class PropertyIntExtensions
    {
        public static string GetDescription(this PropertyInt prop)
        {
            var description = prop.GetAttributeOfType<DescriptionAttribute>();
            return description?.Description ?? prop.ToString();
        }

        public static string GetValueEnumName(this PropertyInt property, int value)
        {
            switch (property)
            {
                case PropertyInt.ActivationResponse:
                    return System.Enum.GetName(typeof(ActivationResponse), value);
                case PropertyInt.AetheriaBitfield:
                    return System.Enum.GetName(typeof(AetheriaBitfield), value);
                case PropertyInt.AttackHeight:
                    return System.Enum.GetName(typeof(AttackHeight), value);
                case PropertyInt.AttackType:
                    return System.Enum.GetName(typeof(AttackType), value);
                case PropertyInt.Attuned:
                    return System.Enum.GetName(typeof(AttunedStatus), value);
                case PropertyInt.AmmoType:
                    return System.Enum.GetName(typeof(AmmoType), value);
                case PropertyInt.Bonded:
                    return System.Enum.GetName(typeof(BondedStatus), value);
                case PropertyInt.ChannelsActive:
                case PropertyInt.ChannelsAllowed:
                    return System.Enum.GetName(typeof(Channel), value);
                case PropertyInt.CombatMode:
                    return System.Enum.GetName(typeof(CombatMode), value);
                case PropertyInt.DefaultCombatStyle:
                case PropertyInt.AiAllowedCombatStyle:
                    return System.Enum.GetName(typeof(CombatStyle), value);
                case PropertyInt.CombatUse:
                    return System.Enum.GetName(typeof(CombatUse), value);
                case PropertyInt.ClothingPriority:
                    return System.Enum.GetName(typeof(CoverageMask), value);
                case PropertyInt.CreatureType:
                case PropertyInt.SlayerCreatureType:
                case PropertyInt.FoeType:
                case PropertyInt.FriendType:
                    return System.Enum.GetName(typeof(CreatureType), value);
                case PropertyInt.DamageType:
                case PropertyInt.ResistanceModifierType:
                    return System.Enum.GetName(typeof(DamageType), value);
                case PropertyInt.CurrentWieldedLocation:
                case PropertyInt.ValidLocations:
                    return System.Enum.GetName(typeof(EquipMask), value);
                case PropertyInt.EquipmentSetId:
                    return System.Enum.GetName(typeof(EquipmentSet), value);
                case PropertyInt.Gender:
                    return System.Enum.GetName(typeof(Gender), value);
                case PropertyInt.GeneratorDestructionType:
                case PropertyInt.GeneratorEndDestructionType:
                    return System.Enum.GetName(typeof(GeneratorDestruct), value);
                case PropertyInt.GeneratorTimeType:
                    return System.Enum.GetName(typeof(GeneratorTimeType), value);
                case PropertyInt.GeneratorType:
                    return System.Enum.GetName(typeof(GeneratorType), value);
                case PropertyInt.HeritageGroup:
                case PropertyInt.HeritageSpecificArmor:
                    return System.Enum.GetName(typeof(HeritageGroup), value);
                case PropertyInt.HookType:
                    return System.Enum.GetName(typeof(HookType), value);
                case PropertyInt.HouseType:
                    return System.Enum.GetName(typeof(HouseType), value);
                case PropertyInt.ImbuedEffect:
                case PropertyInt.ImbuedEffect2:
                case PropertyInt.ImbuedEffect3:
                case PropertyInt.ImbuedEffect4:
                case PropertyInt.ImbuedEffect5:
                    return System.Enum.GetName(typeof(ImbuedEffectType), value);
                case PropertyInt.HookItemType:
                case PropertyInt.ItemType:
                case PropertyInt.MerchandiseItemTypes:
                case PropertyInt.TargetType:
                    return System.Enum.GetName(typeof(ItemType), value);
                case PropertyInt.ItemXpStyle:
                    return System.Enum.GetName(typeof(ItemXpStyle), value);
                case PropertyInt.MaterialType:
                    return System.Enum.GetName(typeof(MaterialType), value);
                case PropertyInt.PaletteTemplate:
                    return System.Enum.GetName(typeof(PaletteTemplate), value);
                case PropertyInt.PhysicsState:
                    return System.Enum.GetName(typeof(PhysicsState), value);
                case PropertyInt.HookPlacement:
                case PropertyInt.Placement:
                case PropertyInt.PCAPRecordedPlacement:
                    return System.Enum.GetName(typeof(Placement), value);
                case PropertyInt.PortalBitmask:
                    return System.Enum.GetName(typeof(PortalBitmask), value);
                case PropertyInt.PlayerKillerStatus:
                    return System.Enum.GetName(typeof(PlayerKillerStatus), value);
                case PropertyInt.BoosterEnum:
                    return System.Enum.GetName(typeof(PropertyAttribute2nd), value);
                case PropertyInt.ShowableOnRadar:
                    return System.Enum.GetName(typeof(RadarBehavior), value);
                case PropertyInt.RadarBlipColor:
                    return System.Enum.GetName(typeof(RadarColor), value);
                case PropertyInt.WeaponSkill:
                case PropertyInt.WieldSkillType:
                case PropertyInt.WieldSkillType2:
                case PropertyInt.WieldSkillType3:
                case PropertyInt.WieldSkillType4:
                case PropertyInt.AppraisalItemSkill:
                    return System.Enum.GetName(typeof(Skill), value);
                case PropertyInt.AccountRequirements:
                    return System.Enum.GetName(typeof(SubscriptionStatus), value);
                case PropertyInt.SummoningMastery:
                    return System.Enum.GetName(typeof(SummoningMastery), value);
                case PropertyInt.UiEffects:
                    return System.Enum.GetName(typeof(UiEffects), value);
                case PropertyInt.ItemUseable:
                    return System.Enum.GetName(typeof(Usable), value);
                case PropertyInt.WeaponType:
                    return System.Enum.GetName(typeof(WeaponType), value);
                case PropertyInt.WieldRequirements:
                case PropertyInt.WieldRequirements2:
                case PropertyInt.WieldRequirements3:
                case PropertyInt.WieldRequirements4:
                    return System.Enum.GetName(typeof(WieldRequirement), value);

                case PropertyInt.GeneratorStartTime:
                case PropertyInt.GeneratorEndTime:
                    return DateTimeOffset.FromUnixTimeSeconds(value).DateTime.ToString(System.Globalization.CultureInfo.InvariantCulture);

                case PropertyInt.ArmorType:
                    return System.Enum.GetName(typeof(ArmorType), value);
                case PropertyInt.ParentLocation:
                    return System.Enum.GetName(typeof(ParentLocation), value);
                case PropertyInt.PlacementPosition:
                    return System.Enum.GetName(typeof(Placement), value);
                case PropertyInt.HouseStatus:
                    return System.Enum.GetName(typeof(HouseStatus), value);

                case PropertyInt.UseCreatesContractId:
                    return System.Enum.GetName(typeof(ContractId), value);

                case PropertyInt.Faction1Bits:
                case PropertyInt.Faction2Bits:
                case PropertyInt.Faction3Bits:
                case PropertyInt.Hatred1Bits:
                case PropertyInt.Hatred2Bits:
                case PropertyInt.Hatred3Bits:
                    return System.Enum.GetName(typeof(FactionBits), value);

                case PropertyInt.UseRequiresSkill:
                case PropertyInt.UseRequiresSkillSpec:
                case PropertyInt.SkillToBeAltered:
                    return System.Enum.GetName(typeof(Skill), value);

                case PropertyInt.HookGroup:
                    return System.Enum.GetName(typeof(HookGroupType), value);

                    //case PropertyInt.TypeOfAlteration:
                    //    return System.Enum.GetName(typeof(SkillAlterationType), value);
            }

            return null;
        }
    }
}
