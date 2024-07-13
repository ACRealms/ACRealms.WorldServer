﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using ACE.Database.Models.Shard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

#pragma warning disable 219, 612, 618
#nullable disable

namespace ACE.Database.CompiledModels.Shard
{
    internal partial class CharacterPropertiesContractRegistryEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "ACE.Database.Models.Shard.CharacterPropertiesContractRegistry",
                typeof(CharacterPropertiesContractRegistry),
                baseEntityType);

            var characterId = runtimeEntityType.AddProperty(
                "CharacterId",
                typeof(ulong),
                propertyInfo: typeof(CharacterPropertiesContractRegistry).GetProperty("CharacterId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CharacterPropertiesContractRegistry).GetField("<CharacterId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: 0ul);
            characterId.TypeMapping = MySqlULongTypeMapping.Default.Clone(
                comparer: new ValueComparer<ulong>(
                    (ulong v1, ulong v2) => v1 == v2,
                    (ulong v) => v.GetHashCode(),
                    (ulong v) => v),
                keyComparer: new ValueComparer<ulong>(
                    (ulong v1, ulong v2) => v1 == v2,
                    (ulong v) => v.GetHashCode(),
                    (ulong v) => v),
                providerValueComparer: new ValueComparer<ulong>(
                    (ulong v1, ulong v2) => v1 == v2,
                    (ulong v) => v.GetHashCode(),
                    (ulong v) => v));
            characterId.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            characterId.AddAnnotation("Relational:ColumnName", "character_Id");

            var contractId = runtimeEntityType.AddProperty(
                "ContractId",
                typeof(uint),
                propertyInfo: typeof(CharacterPropertiesContractRegistry).GetProperty("ContractId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CharacterPropertiesContractRegistry).GetField("<ContractId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: 0u);
            contractId.TypeMapping = MySqlUIntTypeMapping.Default.Clone(
                comparer: new ValueComparer<uint>(
                    (uint v1, uint v2) => v1 == v2,
                    (uint v) => (int)v,
                    (uint v) => v),
                keyComparer: new ValueComparer<uint>(
                    (uint v1, uint v2) => v1 == v2,
                    (uint v) => (int)v,
                    (uint v) => v),
                providerValueComparer: new ValueComparer<uint>(
                    (uint v1, uint v2) => v1 == v2,
                    (uint v) => (int)v,
                    (uint v) => v));
            contractId.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            contractId.AddAnnotation("Relational:ColumnName", "contract_Id");

            var deleteContract = runtimeEntityType.AddProperty(
                "DeleteContract",
                typeof(bool),
                propertyInfo: typeof(CharacterPropertiesContractRegistry).GetProperty("DeleteContract", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CharacterPropertiesContractRegistry).GetField("<DeleteContract>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            deleteContract.TypeMapping = MySqlBoolTypeMapping.Default.Clone(
                comparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                keyComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                providerValueComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "bit(1)"));
            deleteContract.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            deleteContract.AddAnnotation("Relational:ColumnName", "delete_Contract");

            var setAsDisplayContract = runtimeEntityType.AddProperty(
                "SetAsDisplayContract",
                typeof(bool),
                propertyInfo: typeof(CharacterPropertiesContractRegistry).GetProperty("SetAsDisplayContract", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CharacterPropertiesContractRegistry).GetField("<SetAsDisplayContract>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            setAsDisplayContract.TypeMapping = MySqlBoolTypeMapping.Default.Clone(
                comparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                keyComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                providerValueComparer: new ValueComparer<bool>(
                    (bool v1, bool v2) => v1 == v2,
                    (bool v) => v.GetHashCode(),
                    (bool v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "bit(1)"));
            setAsDisplayContract.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            setAsDisplayContract.AddAnnotation("Relational:ColumnName", "set_As_Display_Contract");

            var key = runtimeEntityType.AddKey(
                new[] { characterId, contractId });
            runtimeEntityType.SetPrimaryKey(key);
            key.AddAnnotation("Relational:Name", "PRIMARY");

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("CharacterId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var character = declaringEntityType.AddNavigation("Character",
                runtimeForeignKey,
                onDependent: true,
                typeof(Character),
                propertyInfo: typeof(CharacterPropertiesContractRegistry).GetProperty("Character", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CharacterPropertiesContractRegistry).GetField("<Character>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var characterPropertiesContractRegistry = principalEntityType.AddNavigation("CharacterPropertiesContractRegistry",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<CharacterPropertiesContractRegistry>),
                propertyInfo: typeof(Character).GetProperty("CharacterPropertiesContractRegistry", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Character).GetField("<CharacterPropertiesContractRegistry>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            runtimeForeignKey.AddAnnotation("Relational:Name", "wcid_contract");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "character_properties_contract_registry");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
