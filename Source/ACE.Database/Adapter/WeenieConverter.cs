using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ACE.Database.Models.Shard;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;

namespace ACE.Database.Adapter
{
    public static class WeenieConverter
    {
        public static ACE.Entity.Models.Weenie ConvertToEntityWeenie(ACE.Database.Models.World.Weenie weenie, bool instantiateEmptyCollections = false)
        {
            var result = new ACE.Entity.Models.Weenie();

            result.WeenieClassId = weenie.ClassId;
            result.ClassName = weenie.ClassName;
            result.WeenieType = (WeenieType)weenie.Type;

            if (weenie.WeeniePropertiesBool != null && (instantiateEmptyCollections || weenie.WeeniePropertiesBool.Count > 0))
            {
                result.PropertiesBool = new Dictionary<PropertyBool, bool>(weenie.WeeniePropertiesBool.Count);
                foreach (var value in weenie.WeeniePropertiesBool)
                    result.PropertiesBool[(PropertyBool)value.Type] = value.Value;
            }
            if (weenie.WeeniePropertiesDID != null && (instantiateEmptyCollections || weenie.WeeniePropertiesDID.Count > 0))
            {
                result.PropertiesDID = new Dictionary<PropertyDataId, uint>(weenie.WeeniePropertiesDID.Count);
                foreach (var value in weenie.WeeniePropertiesDID)
                    result.PropertiesDID[(PropertyDataId)value.Type] = value.Value;
            }
            if (weenie.WeeniePropertiesFloat != null && (instantiateEmptyCollections || weenie.WeeniePropertiesFloat.Count > 0))
            {
                result.PropertiesFloat = new Dictionary<PropertyFloat, double>(weenie.WeeniePropertiesFloat.Count);
                foreach (var value in weenie.WeeniePropertiesFloat)
                    result.PropertiesFloat[(PropertyFloat)value.Type] = value.Value;
            }
            if (weenie.WeeniePropertiesIID != null && (instantiateEmptyCollections || weenie.WeeniePropertiesIID.Count > 0))
            {
                result.PropertiesIID = new Dictionary<PropertyInstanceId, ulong>(weenie.WeeniePropertiesIID.Count);
                foreach (var value in weenie.WeeniePropertiesIID)
                    result.PropertiesIID[(PropertyInstanceId)value.Type] = value.Value;
            }
            if (weenie.WeeniePropertiesInt != null && (instantiateEmptyCollections || weenie.WeeniePropertiesInt.Count > 0))
            {
                result.PropertiesInt = new Dictionary<PropertyInt, int>(weenie.WeeniePropertiesInt.Count);
                foreach (var value in weenie.WeeniePropertiesInt)
                    result.PropertiesInt[(PropertyInt)value.Type] = value.Value;
            }
            if (weenie.WeeniePropertiesInt64 != null && (instantiateEmptyCollections || weenie.WeeniePropertiesInt64.Count > 0))
            {
                result.PropertiesInt64 = new Dictionary<PropertyInt64, long>(weenie.WeeniePropertiesInt64.Count);
                foreach (var value in weenie.WeeniePropertiesInt64)
                    result.PropertiesInt64[(PropertyInt64)value.Type] = value.Value;
            }
            if (weenie.WeeniePropertiesString != null && (instantiateEmptyCollections || weenie.WeeniePropertiesString.Count > 0))
            {
                result.PropertiesString = new Dictionary<PropertyString, string>(weenie.WeeniePropertiesString.Count);
                foreach (var value in weenie.WeeniePropertiesString)
                    result.PropertiesString[(PropertyString)value.Type] = value.Value;
            }


            if (weenie.WeeniePropertiesPosition != null && (instantiateEmptyCollections || weenie.WeeniePropertiesPosition.Count > 0))
            {
                result.PropertiesPosition = new Dictionary<PositionType, PropertiesPosition>(weenie.WeeniePropertiesPosition.Count);

                foreach (var record in weenie.WeeniePropertiesPosition)
                {
                    var newEntity = new PropertiesPosition
                    {
                        ObjCellId = record.ObjCellId,
                        PositionX = record.OriginX,
                        PositionY = record.OriginY,
                        PositionZ = record.OriginZ,
                        RotationW = record.AnglesW,
                        RotationX = record.AnglesX,
                        RotationY = record.AnglesY,
                        RotationZ = record.AnglesZ,

                    };

                    result.PropertiesPosition[(PositionType)record.PositionType] = newEntity;
                }
            }


            if (weenie.WeeniePropertiesSpellBook != null && (instantiateEmptyCollections || weenie.WeeniePropertiesSpellBook.Count > 0))
            {
                result.PropertiesSpellBook = new Dictionary<int, float>(weenie.WeeniePropertiesSpellBook.Count);
                foreach (var value in weenie.WeeniePropertiesSpellBook)
                    result.PropertiesSpellBook[value.Spell] = value.Probability;
            }


            if (weenie.WeeniePropertiesAnimPart != null && (instantiateEmptyCollections || weenie.WeeniePropertiesAnimPart.Count > 0))
            {
                result.PropertiesAnimPart = new List<PropertiesAnimPart>(weenie.WeeniePropertiesAnimPart.Count);

                foreach (var record in weenie.WeeniePropertiesAnimPart)
                {
                    var newEntity = new PropertiesAnimPart
                    {
                        Index = record.Index,
                        AnimationId = record.AnimationId,
                    };

                    result.PropertiesAnimPart.Add(newEntity);
                }
            }

            if (weenie.WeeniePropertiesPalette != null && (instantiateEmptyCollections || weenie.WeeniePropertiesPalette.Count > 0))
            {
                result.PropertiesPalette = new Collection<PropertiesPalette>();

                foreach (var record in weenie.WeeniePropertiesPalette)
                {
                    var newEntity = new PropertiesPalette
                    {
                        SubPaletteId = record.SubPaletteId,
                        Offset = record.Offset,
                        Length = record.Length,
                    };

                    result.PropertiesPalette.Add(newEntity);
                }
            }

            if (weenie.WeeniePropertiesTextureMap != null && (instantiateEmptyCollections || weenie.WeeniePropertiesTextureMap.Count > 0))
            {
                result.PropertiesTextureMap = new List<PropertiesTextureMap>(weenie.WeeniePropertiesTextureMap.Count);

                foreach (var record in weenie.WeeniePropertiesTextureMap)
                {
                    var newEntity = new PropertiesTextureMap
                    {
                        PartIndex = record.Index,
                        OldTexture = record.OldId,
                        NewTexture = record.NewId,
                    };

                    result.PropertiesTextureMap.Add(newEntity);
                }
            }


            // Properties for all world objects that typically aren't modified over the original weenie

            if (weenie.WeeniePropertiesCreateList != null && (instantiateEmptyCollections || weenie.WeeniePropertiesCreateList.Count > 0))
            {
                result.PropertiesCreateList = new Collection<PropertiesCreateList>();

                foreach (var record in weenie.WeeniePropertiesCreateList)
                {
                    var newEntity = new PropertiesCreateList
                    {
                        DestinationType = (DestinationType)record.DestinationType,
                        WeenieClassId = record.WeenieClassId,
                        StackSize = record.StackSize,
                        Palette = record.Palette,
                        Shade = record.Shade,
                        TryToBond = record.TryToBond,
                    };

                    result.PropertiesCreateList.Add(newEntity);
                }
            }

            if (weenie.WeeniePropertiesEmote != null && (instantiateEmptyCollections || weenie.WeeniePropertiesEmote.Count > 0))
            {
                result.PropertiesEmote = new Collection<PropertiesEmote>();

                foreach (var record in weenie.WeeniePropertiesEmote)
                {
                    var newEntity = new PropertiesEmote
                    {
                        Category = (EmoteCategory)record.Category,
                        Probability = record.Probability,
                        WeenieClassId = record.WeenieClassId,
                        Style = (MotionStance?)record.Style,
                        Substyle = (MotionCommand?)record.Substyle,
                        Quest = record.Quest,
                        VendorType = (VendorType?)record.VendorType,
                        MinHealth = record.MinHealth,
                        MaxHealth = record.MaxHealth,
                    };

                    foreach (var record2 in record.WeeniePropertiesEmoteAction.OrderBy(r => r.Order))
                    {
                        var newEntity2 = new PropertiesEmoteAction
                        {
                            Type = record2.Type,
                            Delay = record2.Delay,
                            Extent = record2.Extent,
                            Motion = (MotionCommand?)record2.Motion,
                            Message = record2.Message,
                            TestString = record2.TestString,
                            Min = record2.Min,
                            Max = record2.Max,
                            Min64 = record2.Min64,
                            Max64 = record2.Max64,
                            MinDbl = record2.MinDbl,
                            MaxDbl = record2.MaxDbl,
                            Stat = record2.Stat,
                            Display = record2.Display,
                            Amount = record2.Amount,
                            Amount64 = record2.Amount64,
                            HeroXP64 = record2.HeroXP64,
                            Percent = record2.Percent,
                            SpellId = record2.SpellId,
                            WealthRating = record2.WealthRating,
                            TreasureClass = record2.TreasureClass,
                            TreasureType = record2.TreasureType,
                            PScript = (PlayScript?)record2.PScript,
                            Sound = (Sound?)record2.Sound,
                            DestinationType = record2.DestinationType,
                            WeenieClassId = record2.WeenieClassId,
                            StackSize = record2.StackSize,
                            Palette = record2.Palette,
                            Shade = record2.Shade,
                            TryToBond = record2.TryToBond,
                            ObjCellId = record2.ObjCellId,
                            OriginX = record2.OriginX,
                            OriginY = record2.OriginY,
                            OriginZ = record2.OriginZ,
                            AnglesW = record2.AnglesW,
                            AnglesX = record2.AnglesX,
                            AnglesY = record2.AnglesY,
                            AnglesZ = record2.AnglesZ,
                        };

                        newEntity.PropertiesEmoteAction.Add(newEntity2);
                    }

                    result.PropertiesEmote.Add(newEntity);
                }
            }

            if (weenie.WeeniePropertiesEventFilter != null && (instantiateEmptyCollections || weenie.WeeniePropertiesEventFilter.Count > 0))
            {
                result.PropertiesEventFilter = new HashSet<int>();
                foreach (var value in weenie.WeeniePropertiesEventFilter)
                    result.PropertiesEventFilter.Add(value.Event);
            }

            if (weenie.WeeniePropertiesGenerator != null && (instantiateEmptyCollections || weenie.WeeniePropertiesGenerator.Count > 0))
            {
                result.PropertiesGenerator = new List<PropertiesGenerator>(weenie.WeeniePropertiesGenerator.Count);

                foreach (var record in weenie.WeeniePropertiesGenerator) // TODO do we have the correct order?
                {
                    var newEntity = new PropertiesGenerator
                    {
                        Probability = record.Probability,
                        WeenieClassId = record.WeenieClassId,
                        Delay = record.Delay,
                        InitCreate = record.InitCreate,
                        MaxCreate = record.MaxCreate,
                        WhenCreate = (RegenerationType)record.WhenCreate,
                        WhereCreate = (RegenLocationType)record.WhereCreate,
                        StackSize = record.StackSize,
                        PaletteId = record.PaletteId,
                        Shade = record.Shade,
                        ObjCellId = record.ObjCellId,
                        OriginX = record.OriginX,
                        OriginY = record.OriginY,
                        OriginZ = record.OriginZ,
                        AnglesW = record.AnglesW,
                        AnglesX = record.AnglesX,
                        AnglesY = record.AnglesY,
                        AnglesZ = record.AnglesZ,
                    };

                    result.PropertiesGenerator.Add(newEntity);
                }
            }


            // Properties for creatures

            if (weenie.WeeniePropertiesAttribute != null && (instantiateEmptyCollections || weenie.WeeniePropertiesAttribute.Count > 0))
            {
                result.PropertiesAttribute = new Dictionary<PropertyAttribute, PropertiesAttribute>(weenie.WeeniePropertiesAttribute.Count);

                foreach (var record in weenie.WeeniePropertiesAttribute)
                {
                    var newEntity = new PropertiesAttribute
                    {
                        InitLevel = record.InitLevel,
                        LevelFromCP = record.LevelFromCP,
                        CPSpent = record.CPSpent,
                    };

                    result.PropertiesAttribute[(PropertyAttribute)record.Type] = newEntity;
                }
            }

            if (weenie.WeeniePropertiesAttribute2nd != null && (instantiateEmptyCollections || weenie.WeeniePropertiesAttribute2nd.Count > 0))
            {
                result.PropertiesAttribute2nd = new Dictionary<PropertyAttribute2nd, PropertiesAttribute2nd>(weenie.WeeniePropertiesAttribute2nd.Count);

                foreach (var record in weenie.WeeniePropertiesAttribute2nd)
                {
                    var newEntity = new PropertiesAttribute2nd
                    {
                        InitLevel = record.InitLevel,
                        LevelFromCP = record.LevelFromCP,
                        CPSpent = record.CPSpent,
                        CurrentLevel = record.CurrentLevel,
                    };

                    result.PropertiesAttribute2nd[(PropertyAttribute2nd)record.Type] = newEntity;
                }
            }

            if (weenie.WeeniePropertiesBodyPart != null && (instantiateEmptyCollections || weenie.WeeniePropertiesBodyPart.Count > 0))
            {
                result.PropertiesBodyPart = new Dictionary<CombatBodyPart, PropertiesBodyPart>(weenie.WeeniePropertiesBodyPart.Count);

                foreach (var record in weenie.WeeniePropertiesBodyPart)
                {
                    var newEntity = new PropertiesBodyPart
                    {
                        DType = (DamageType)record.DType,
                        DVal = record.DVal,
                        DVar = record.DVar,
                        BaseArmor = record.BaseArmor,
                        ArmorVsSlash = record.ArmorVsSlash,
                        ArmorVsPierce = record.ArmorVsPierce,
                        ArmorVsBludgeon = record.ArmorVsBludgeon,
                        ArmorVsCold = record.ArmorVsCold,
                        ArmorVsFire = record.ArmorVsFire,
                        ArmorVsAcid = record.ArmorVsAcid,
                        ArmorVsElectric = record.ArmorVsElectric,
                        ArmorVsNether = record.ArmorVsNether,
                        BH = record.BH,
                        HLF = record.HLF,
                        MLF = record.MLF,
                        LLF = record.LLF,
                        HRF = record.HRF,
                        MRF = record.MRF,
                        LRF = record.LRF,
                        HLB = record.HLB,
                        MLB = record.MLB,
                        LLB = record.LLB,
                        HRB = record.HRB,
                        MRB = record.MRB,
                        LRB = record.LRB,
                    };

                    result.PropertiesBodyPart[(CombatBodyPart)record.Key] = newEntity;
                }
            }

            if (weenie.WeeniePropertiesSkill != null && (instantiateEmptyCollections || weenie.WeeniePropertiesSkill.Count > 0))
            {
                result.PropertiesSkill = new Dictionary<Skill, PropertiesSkill>(weenie.WeeniePropertiesSkill.Count);

                foreach (var record in weenie.WeeniePropertiesSkill)
                {
                    var newEntity = new PropertiesSkill
                    {
                        LevelFromPP = record.LevelFromPP,
                        SAC = (SkillAdvancementClass)record.SAC,
                        PP = record.PP,
                        InitLevel = record.InitLevel,
                        ResistanceAtLastCheck = record.ResistanceAtLastCheck,
                        LastUsedTime = record.LastUsedTime,
                    };

                    result.PropertiesSkill[(Skill)record.Type] = newEntity;
                }
            }


            // Properties for books

            if (weenie.WeeniePropertiesBook != null)
            {
                result.PropertiesBook = new PropertiesBook
                {
                    MaxNumPages = weenie.WeeniePropertiesBook.MaxNumPages,
                    MaxNumCharsPerPage = weenie.WeeniePropertiesBook.MaxNumCharsPerPage,
                };
            }

            if (weenie.WeeniePropertiesBookPageData != null && (instantiateEmptyCollections || weenie.WeeniePropertiesBookPageData.Count > 0))
            {
                result.PropertiesBookPageData = new List<PropertiesBookPageData>(weenie.WeeniePropertiesBookPageData.Count);

                foreach (var record in weenie.WeeniePropertiesBookPageData.OrderBy(r => r.PageId))
                {
                    var newEntity = new PropertiesBookPageData
                    {
                        AuthorId = record.AuthorId,
                        AuthorName = record.AuthorName,
                        AuthorAccount = record.AuthorAccount,
                        IgnoreAuthor = record.IgnoreAuthor,
                        PageText = record.PageText,
                    };

                    result.PropertiesBookPageData.Add(newEntity);
                }
            }

            return result;
        }
    }
}
