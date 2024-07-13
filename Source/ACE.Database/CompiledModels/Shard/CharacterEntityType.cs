﻿// <auto-generated />
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using ACE.Database.Models.Shard;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

#pragma warning disable 219, 612, 618
#nullable disable

namespace ACE.Database.CompiledModels.Shard
{
    internal partial class CharacterEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "ACE.Database.Models.Shard.Character",
                typeof(Character),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(ulong),
                propertyInfo: typeof(Character).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Character).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: 0ul);
            id.TypeMapping = MySqlULongTypeMapping.Default.Clone(
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
            id.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            id.AddAnnotation("Relational:ColumnName", "id");

            var accountId = runtimeEntityType.AddProperty(
                "AccountId",
                typeof(uint),
                propertyInfo: typeof(Character).GetProperty("AccountId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Character).GetField("<AccountId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0u);
            accountId.TypeMapping = MySqlUIntTypeMapping.Default.Clone(
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
            accountId.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            accountId.AddAnnotation("Relational:ColumnName", "account_Id");

            var characterOptions1 = runtimeEntityType.AddProperty(
                "CharacterOptions1",
                typeof(int),
                propertyInfo: typeof(Character).GetProperty("CharacterOptions1", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Character).GetField("<CharacterOptions1>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            characterOptions1.TypeMapping = MySqlIntTypeMapping.Default.Clone(
                comparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                keyComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                providerValueComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v));
            characterOptions1.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            characterOptions1.AddAnnotation("Relational:ColumnName", "character_Options_1");

            var characterOptions2 = runtimeEntityType.AddProperty(
                "CharacterOptions2",
                typeof(int),
                propertyInfo: typeof(Character).GetProperty("CharacterOptions2", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Character).GetField("<CharacterOptions2>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            characterOptions2.TypeMapping = MySqlIntTypeMapping.Default.Clone(
                comparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                keyComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                providerValueComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v));
            characterOptions2.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            characterOptions2.AddAnnotation("Relational:ColumnName", "character_Options_2");

            var defaultHairTexture = runtimeEntityType.AddProperty(
                "DefaultHairTexture",
                typeof(uint),
                propertyInfo: typeof(Character).GetProperty("DefaultHairTexture", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Character).GetField("<DefaultHairTexture>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0u);
            defaultHairTexture.TypeMapping = MySqlUIntTypeMapping.Default.Clone(
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
            defaultHairTexture.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            defaultHairTexture.AddAnnotation("Relational:ColumnName", "default_Hair_Texture");

            var deleteTime = runtimeEntityType.AddProperty(
                "DeleteTime",
                typeof(ulong),
                propertyInfo: typeof(Character).GetProperty("DeleteTime", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Character).GetField("<DeleteTime>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0ul);
            deleteTime.TypeMapping = MySqlULongTypeMapping.Default.Clone(
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
            deleteTime.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            deleteTime.AddAnnotation("Relational:ColumnName", "delete_Time");

            var gameplayOptions = runtimeEntityType.AddProperty(
                "GameplayOptions",
                typeof(byte[]),
                propertyInfo: typeof(Character).GetProperty("GameplayOptions", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Character).GetField("<GameplayOptions>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            gameplayOptions.TypeMapping = MySqlByteArrayTypeMapping.Default.Clone(
                comparer: new ValueComparer<byte[]>(
                    (Byte[] v1, Byte[] v2) => StructuralComparisons.StructuralEqualityComparer.Equals((object)v1, (object)v2),
                    (Byte[] v) => v.GetHashCode(),
                    (Byte[] v) => v),
                keyComparer: new ValueComparer<byte[]>(
                    (Byte[] v1, Byte[] v2) => StructuralComparisons.StructuralEqualityComparer.Equals((object)v1, (object)v2),
                    (Byte[] v) => StructuralComparisons.StructuralEqualityComparer.GetHashCode((object)v),
                    (Byte[] source) => source.ToArray()),
                providerValueComparer: new ValueComparer<byte[]>(
                    (Byte[] v1, Byte[] v2) => StructuralComparisons.StructuralEqualityComparer.Equals((object)v1, (object)v2),
                    (Byte[] v) => StructuralComparisons.StructuralEqualityComparer.GetHashCode((object)v),
                    (Byte[] source) => source.ToArray()),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "blob"));
            gameplayOptions.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            gameplayOptions.AddAnnotation("Relational:ColumnName", "gameplay_Options");
            gameplayOptions.AddAnnotation("Relational:ColumnType", "blob");

            var hairTexture = runtimeEntityType.AddProperty(
                "HairTexture",
                typeof(uint),
                propertyInfo: typeof(Character).GetProperty("HairTexture", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Character).GetField("<HairTexture>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0u);
            hairTexture.TypeMapping = MySqlUIntTypeMapping.Default.Clone(
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
            hairTexture.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            hairTexture.AddAnnotation("Relational:ColumnName", "hair_Texture");

            var isDeleted = runtimeEntityType.AddProperty(
                "IsDeleted",
                typeof(bool),
                propertyInfo: typeof(Character).GetProperty("IsDeleted", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Character).GetField("<IsDeleted>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isDeleted.TypeMapping = MySqlBoolTypeMapping.Default.Clone(
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
            isDeleted.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            isDeleted.AddAnnotation("Relational:ColumnName", "is_Deleted");

            var isPlussed = runtimeEntityType.AddProperty(
                "IsPlussed",
                typeof(bool),
                propertyInfo: typeof(Character).GetProperty("IsPlussed", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Character).GetField("<IsPlussed>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isPlussed.TypeMapping = MySqlBoolTypeMapping.Default.Clone(
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
            isPlussed.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            isPlussed.AddAnnotation("Relational:ColumnName", "is_Plussed");

            var lastLoginTimestamp = runtimeEntityType.AddProperty(
                "LastLoginTimestamp",
                typeof(double),
                propertyInfo: typeof(Character).GetProperty("LastLoginTimestamp", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Character).GetField("<LastLoginTimestamp>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            lastLoginTimestamp.TypeMapping = MySqlDoubleTypeMapping.Default.Clone(
                comparer: new ValueComparer<double>(
                    (double v1, double v2) => v1.Equals(v2),
                    (double v) => v.GetHashCode(),
                    (double v) => v),
                keyComparer: new ValueComparer<double>(
                    (double v1, double v2) => v1.Equals(v2),
                    (double v) => v.GetHashCode(),
                    (double v) => v),
                providerValueComparer: new ValueComparer<double>(
                    (double v1, double v2) => v1.Equals(v2),
                    (double v) => v.GetHashCode(),
                    (double v) => v));
            lastLoginTimestamp.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            lastLoginTimestamp.AddAnnotation("Relational:ColumnName", "last_Login_Timestamp");

            var name = runtimeEntityType.AddProperty(
                "Name",
                typeof(string),
                propertyInfo: typeof(Character).GetProperty("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Character).GetField("<Name>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            name.TypeMapping = MySqlStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "varchar(255)",
                    size: 255));
            name.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            name.AddAnnotation("Relational:ColumnName", "name");

            var spellbookFilters = runtimeEntityType.AddProperty(
                "SpellbookFilters",
                typeof(uint),
                propertyInfo: typeof(Character).GetProperty("SpellbookFilters", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Character).GetField("<SpellbookFilters>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                sentinel: 0u);
            spellbookFilters.TypeMapping = MySqlUIntTypeMapping.Default.Clone(
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
            spellbookFilters.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            spellbookFilters.AddAnnotation("Relational:ColumnName", "spellbook_Filters");
            spellbookFilters.AddAnnotation("Relational:DefaultValueSql", "'16383'");

            var totalLogins = runtimeEntityType.AddProperty(
                "TotalLogins",
                typeof(int),
                propertyInfo: typeof(Character).GetProperty("TotalLogins", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Character).GetField("<TotalLogins>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            totalLogins.TypeMapping = MySqlIntTypeMapping.Default.Clone(
                comparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                keyComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                providerValueComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v));
            totalLogins.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            totalLogins.AddAnnotation("Relational:ColumnName", "total_Logins");

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);
            key.AddAnnotation("Relational:Name", "PRIMARY");

            var character_account_idx = runtimeEntityType.AddIndex(
                new[] { accountId },
                name: "character_account_idx");

            var character_name_idx = runtimeEntityType.AddIndex(
                new[] { name },
                name: "character_name_idx");

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "character");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
