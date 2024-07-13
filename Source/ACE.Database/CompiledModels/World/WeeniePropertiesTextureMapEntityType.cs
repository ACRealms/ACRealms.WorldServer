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
    internal partial class WeeniePropertiesTextureMapEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "ACE.Database.Models.World.WeeniePropertiesTextureMap",
                typeof(WeeniePropertiesTextureMap),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(uint),
                propertyInfo: typeof(WeeniePropertiesTextureMap).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesTextureMap).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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

            var index = runtimeEntityType.AddProperty(
                "Index",
                typeof(byte),
                propertyInfo: typeof(WeeniePropertiesTextureMap).GetProperty("Index", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesTextureMap).GetField("<Index>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: (byte)0);
            index.TypeMapping = MySqlByteTypeMapping.Default.Clone(
                comparer: new ValueComparer<byte>(
                    (byte v1, byte v2) => v1 == v2,
                    (byte v) => (int)v,
                    (byte v) => v),
                keyComparer: new ValueComparer<byte>(
                    (byte v1, byte v2) => v1 == v2,
                    (byte v) => (int)v,
                    (byte v) => v),
                providerValueComparer: new ValueComparer<byte>(
                    (byte v1, byte v2) => v1 == v2,
                    (byte v) => (int)v,
                    (byte v) => v));
            index.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            index.AddAnnotation("Relational:ColumnName", "index");

            var newId = runtimeEntityType.AddProperty(
                "NewId",
                typeof(uint),
                propertyInfo: typeof(WeeniePropertiesTextureMap).GetProperty("NewId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesTextureMap).GetField("<NewId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0u);
            newId.TypeMapping = MySqlUIntTypeMapping.Default.Clone(
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
            newId.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            newId.AddAnnotation("Relational:ColumnName", "new_Id");

            var objectId = runtimeEntityType.AddProperty(
                "ObjectId",
                typeof(uint),
                propertyInfo: typeof(WeeniePropertiesTextureMap).GetProperty("ObjectId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesTextureMap).GetField("<ObjectId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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

            var oldId = runtimeEntityType.AddProperty(
                "OldId",
                typeof(uint),
                propertyInfo: typeof(WeeniePropertiesTextureMap).GetProperty("OldId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesTextureMap).GetField("<OldId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0u);
            oldId.TypeMapping = MySqlUIntTypeMapping.Default.Clone(
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
            oldId.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            oldId.AddAnnotation("Relational:ColumnName", "old_Id");

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var object_Id_index_oldId_uidx = runtimeEntityType.AddIndex(
                new[] { objectId, index, oldId },
                name: "object_Id_index_oldId_uidx",
                unique: true);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ObjectId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ClassId") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var @object = declaringEntityType.AddNavigation("Object",
                runtimeForeignKey,
                onDependent: true,
                typeof(Weenie),
                propertyInfo: typeof(WeeniePropertiesTextureMap).GetProperty("Object", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesTextureMap).GetField("<Object>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var weeniePropertiesTextureMap = principalEntityType.AddNavigation("WeeniePropertiesTextureMap",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<WeeniePropertiesTextureMap>),
                propertyInfo: typeof(Weenie).GetProperty("WeeniePropertiesTextureMap", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Weenie).GetField("<WeeniePropertiesTextureMap>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            runtimeForeignKey.AddAnnotation("Relational:Name", "wcid_texturemap");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "weenie_properties_texture_map");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
