﻿// <auto-generated />
using System;
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
    internal partial class WeeniePropertiesBookEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "ACE.Database.Models.World.WeeniePropertiesBook",
                typeof(WeeniePropertiesBook),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(uint),
                propertyInfo: typeof(WeeniePropertiesBook).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesBook).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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

            var maxNumCharsPerPage = runtimeEntityType.AddProperty(
                "MaxNumCharsPerPage",
                typeof(int),
                propertyInfo: typeof(WeeniePropertiesBook).GetProperty("MaxNumCharsPerPage", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesBook).GetField("<MaxNumCharsPerPage>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                sentinel: 0);
            maxNumCharsPerPage.TypeMapping = MySqlIntTypeMapping.Default.Clone(
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
            maxNumCharsPerPage.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            maxNumCharsPerPage.AddAnnotation("Relational:ColumnName", "max_Num_Chars_Per_Page");
            maxNumCharsPerPage.AddAnnotation("Relational:DefaultValueSql", "'1000'");

            var maxNumPages = runtimeEntityType.AddProperty(
                "MaxNumPages",
                typeof(int),
                propertyInfo: typeof(WeeniePropertiesBook).GetProperty("MaxNumPages", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesBook).GetField("<MaxNumPages>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                sentinel: 0);
            maxNumPages.TypeMapping = MySqlIntTypeMapping.Default.Clone(
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
            maxNumPages.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            maxNumPages.AddAnnotation("Relational:ColumnName", "max_Num_Pages");
            maxNumPages.AddAnnotation("Relational:DefaultValueSql", "'1'");

            var objectId = runtimeEntityType.AddProperty(
                "ObjectId",
                typeof(uint),
                propertyInfo: typeof(WeeniePropertiesBook).GetProperty("ObjectId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesBook).GetField("<ObjectId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0u);
            objectId.TypeMapping = MySqlUIntTypeMapping.Default.Clone(
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
            objectId.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            objectId.AddAnnotation("Relational:ColumnName", "object_Id");

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var wcid_bookdata_uidx = runtimeEntityType.AddIndex(
                new[] { objectId },
                name: "wcid_bookdata_uidx",
                unique: true);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ObjectId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ClassId") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                unique: true,
                required: true);

            var @object = declaringEntityType.AddNavigation("Object",
                runtimeForeignKey,
                onDependent: true,
                typeof(Weenie),
                propertyInfo: typeof(WeeniePropertiesBook).GetProperty("Object", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesBook).GetField("<Object>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var weeniePropertiesBook = principalEntityType.AddNavigation("WeeniePropertiesBook",
                runtimeForeignKey,
                onDependent: false,
                typeof(WeeniePropertiesBook),
                propertyInfo: typeof(Weenie).GetProperty("WeeniePropertiesBook", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Weenie).GetField("<WeeniePropertiesBook>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            runtimeForeignKey.AddAnnotation("Relational:Name", "wcid_bookdata");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "weenie_properties_book");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
