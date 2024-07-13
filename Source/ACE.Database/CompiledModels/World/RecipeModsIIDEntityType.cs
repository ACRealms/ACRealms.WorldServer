﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using ACE.Database.Models.World;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

#pragma warning disable 219, 612, 618
#nullable disable

namespace ACE.Database.CompiledModels.World
{
    internal partial class RecipeModsIIDEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "ACE.Database.Models.World.RecipeModsIID",
                typeof(RecipeModsIID),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(uint),
                propertyInfo: typeof(RecipeModsIID).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(RecipeModsIID).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: 0u);
            id.TypeMapping = MySqlUIntTypeMapping.Default.Clone(
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
            id.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
            id.AddAnnotation("Relational:ColumnName", "id");

            var @enum = runtimeEntityType.AddProperty(
                "Enum",
                typeof(int),
                propertyInfo: typeof(RecipeModsIID).GetProperty("Enum", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(RecipeModsIID).GetField("<Enum>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            @enum.TypeMapping = MySqlIntTypeMapping.Default.Clone(
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
            @enum.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            @enum.AddAnnotation("Relational:ColumnName", "enum");

            var index = runtimeEntityType.AddProperty(
                "Index",
                typeof(sbyte),
                propertyInfo: typeof(RecipeModsIID).GetProperty("Index", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(RecipeModsIID).GetField("<Index>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: (sbyte)0);
            index.TypeMapping = MySqlSByteTypeMapping.Default.Clone(
                comparer: new ValueComparer<sbyte>(
                    (sbyte v1, sbyte v2) => v1 == v2,
                    (sbyte v) => (int)v,
                    (sbyte v) => v),
                keyComparer: new ValueComparer<sbyte>(
                    (sbyte v1, sbyte v2) => v1 == v2,
                    (sbyte v) => (int)v,
                    (sbyte v) => v),
                providerValueComparer: new ValueComparer<sbyte>(
                    (sbyte v1, sbyte v2) => v1 == v2,
                    (sbyte v) => (int)v,
                    (sbyte v) => v));
            index.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            index.AddAnnotation("Relational:ColumnName", "index");

            var recipeModId = runtimeEntityType.AddProperty(
                "RecipeModId",
                typeof(uint),
                propertyInfo: typeof(RecipeModsIID).GetProperty("RecipeModId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(RecipeModsIID).GetField("<RecipeModId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0u);
            recipeModId.TypeMapping = MySqlUIntTypeMapping.Default.Clone(
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
            recipeModId.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            recipeModId.AddAnnotation("Relational:ColumnName", "recipe_Mod_Id");

            var source = runtimeEntityType.AddProperty(
                "Source",
                typeof(int),
                propertyInfo: typeof(RecipeModsIID).GetProperty("Source", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(RecipeModsIID).GetField("<Source>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            source.TypeMapping = MySqlIntTypeMapping.Default.Clone(
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
            source.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            source.AddAnnotation("Relational:ColumnName", "source");

            var stat = runtimeEntityType.AddProperty(
                "Stat",
                typeof(int),
                propertyInfo: typeof(RecipeModsIID).GetProperty("Stat", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(RecipeModsIID).GetField("<Stat>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            stat.TypeMapping = MySqlIntTypeMapping.Default.Clone(
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
            stat.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            stat.AddAnnotation("Relational:ColumnName", "stat");

            var value = runtimeEntityType.AddProperty(
                "Value",
                typeof(uint),
                propertyInfo: typeof(RecipeModsIID).GetProperty("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(RecipeModsIID).GetField("<Value>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0u);
            value.TypeMapping = MySqlUIntTypeMapping.Default.Clone(
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
            value.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            value.AddAnnotation("Relational:ColumnName", "value");

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var recipeId_mod_iid = runtimeEntityType.AddIndex(
                new[] { recipeModId },
                name: "recipeId_mod_iid");

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("RecipeModId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var recipeMod = declaringEntityType.AddNavigation("RecipeMod",
                runtimeForeignKey,
                onDependent: true,
                typeof(RecipeMod),
                propertyInfo: typeof(RecipeModsIID).GetProperty("RecipeMod", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(RecipeModsIID).GetField("<RecipeMod>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var recipeModsIID = principalEntityType.AddNavigation("RecipeModsIID",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<RecipeModsIID>),
                propertyInfo: typeof(RecipeMod).GetProperty("RecipeModsIID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(RecipeMod).GetField("<RecipeModsIID>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            runtimeForeignKey.AddAnnotation("Relational:Name", "recipeId_mod_iid");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "recipe_mods_i_i_d");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
