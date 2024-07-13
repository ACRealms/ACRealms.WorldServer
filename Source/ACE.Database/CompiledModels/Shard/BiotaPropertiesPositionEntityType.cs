﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using ACE.Database.Models.Shard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

#pragma warning disable 219, 612, 618
#nullable disable

namespace ACE.Database.CompiledModels.Shard
{
    internal partial class BiotaPropertiesPositionEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "ACE.Database.Models.Shard.BiotaPropertiesPosition",
                typeof(BiotaPropertiesPosition),
                baseEntityType);

            var objectId = runtimeEntityType.AddProperty(
                "ObjectId",
                typeof(ulong),
                propertyInfo: typeof(BiotaPropertiesPosition).GetProperty("ObjectId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesPosition).GetField("<ObjectId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: 0ul);
            objectId.TypeMapping = MySqlULongTypeMapping.Default.Clone(
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
            objectId.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            objectId.AddAnnotation("Relational:ColumnName", "object_Id");

            var positionType = runtimeEntityType.AddProperty(
                "PositionType",
                typeof(ushort),
                propertyInfo: typeof(BiotaPropertiesPosition).GetProperty("PositionType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesPosition).GetField("<PositionType>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: (ushort)0);
            positionType.TypeMapping = MySqlUShortTypeMapping.Default.Clone(
                comparer: new ValueComparer<ushort>(
                    (ushort v1, ushort v2) => v1 == v2,
                    (ushort v) => (int)v,
                    (ushort v) => v),
                keyComparer: new ValueComparer<ushort>(
                    (ushort v1, ushort v2) => v1 == v2,
                    (ushort v) => (int)v,
                    (ushort v) => v),
                providerValueComparer: new ValueComparer<ushort>(
                    (ushort v1, ushort v2) => v1 == v2,
                    (ushort v) => (int)v,
                    (ushort v) => v));
            positionType.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            positionType.AddAnnotation("Relational:ColumnName", "position_Type");

            var anglesW = runtimeEntityType.AddProperty(
                "AnglesW",
                typeof(float),
                propertyInfo: typeof(BiotaPropertiesPosition).GetProperty("AnglesW", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesPosition).GetField("<AnglesW>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0f);
            anglesW.TypeMapping = MySqlFloatTypeMapping.Default.Clone(
                comparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v),
                keyComparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v),
                providerValueComparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v));
            anglesW.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            anglesW.AddAnnotation("Relational:ColumnName", "angles_W");

            var anglesX = runtimeEntityType.AddProperty(
                "AnglesX",
                typeof(float),
                propertyInfo: typeof(BiotaPropertiesPosition).GetProperty("AnglesX", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesPosition).GetField("<AnglesX>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0f);
            anglesX.TypeMapping = MySqlFloatTypeMapping.Default.Clone(
                comparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v),
                keyComparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v),
                providerValueComparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v));
            anglesX.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            anglesX.AddAnnotation("Relational:ColumnName", "angles_X");

            var anglesY = runtimeEntityType.AddProperty(
                "AnglesY",
                typeof(float),
                propertyInfo: typeof(BiotaPropertiesPosition).GetProperty("AnglesY", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesPosition).GetField("<AnglesY>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0f);
            anglesY.TypeMapping = MySqlFloatTypeMapping.Default.Clone(
                comparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v),
                keyComparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v),
                providerValueComparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v));
            anglesY.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            anglesY.AddAnnotation("Relational:ColumnName", "angles_Y");

            var anglesZ = runtimeEntityType.AddProperty(
                "AnglesZ",
                typeof(float),
                propertyInfo: typeof(BiotaPropertiesPosition).GetProperty("AnglesZ", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesPosition).GetField("<AnglesZ>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0f);
            anglesZ.TypeMapping = MySqlFloatTypeMapping.Default.Clone(
                comparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v),
                keyComparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v),
                providerValueComparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v));
            anglesZ.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            anglesZ.AddAnnotation("Relational:ColumnName", "angles_Z");

            var instance = runtimeEntityType.AddProperty(
                "Instance",
                typeof(uint?),
                propertyInfo: typeof(BiotaPropertiesPosition).GetProperty("Instance", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesPosition).GetField("<Instance>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            instance.TypeMapping = MySqlUIntTypeMapping.Default.Clone(
                comparer: new ValueComparer<uint?>(
                    (Nullable<uint> v1, Nullable<uint> v2) => v1.HasValue && v2.HasValue && (uint)v1 == (uint)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<uint> v) => v.HasValue ? (int)(uint)v : 0,
                    (Nullable<uint> v) => v.HasValue ? (Nullable<uint>)(uint)v : default(Nullable<uint>)),
                keyComparer: new ValueComparer<uint?>(
                    (Nullable<uint> v1, Nullable<uint> v2) => v1.HasValue && v2.HasValue && (uint)v1 == (uint)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<uint> v) => v.HasValue ? (int)(uint)v : 0,
                    (Nullable<uint> v) => v.HasValue ? (Nullable<uint>)(uint)v : default(Nullable<uint>)),
                providerValueComparer: new ValueComparer<uint?>(
                    (Nullable<uint> v1, Nullable<uint> v2) => v1.HasValue && v2.HasValue && (uint)v1 == (uint)v2 || !v1.HasValue && !v2.HasValue,
                    (Nullable<uint> v) => v.HasValue ? (int)(uint)v : 0,
                    (Nullable<uint> v) => v.HasValue ? (Nullable<uint>)(uint)v : default(Nullable<uint>)));
            instance.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            instance.AddAnnotation("Relational:ColumnName", "instance");

            var objCellId = runtimeEntityType.AddProperty(
                "ObjCellId",
                typeof(uint),
                propertyInfo: typeof(BiotaPropertiesPosition).GetProperty("ObjCellId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesPosition).GetField("<ObjCellId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0u);
            objCellId.TypeMapping = MySqlUIntTypeMapping.Default.Clone(
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
            objCellId.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            objCellId.AddAnnotation("Relational:ColumnName", "obj_Cell_Id");

            var originX = runtimeEntityType.AddProperty(
                "OriginX",
                typeof(float),
                propertyInfo: typeof(BiotaPropertiesPosition).GetProperty("OriginX", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesPosition).GetField("<OriginX>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0f);
            originX.TypeMapping = MySqlFloatTypeMapping.Default.Clone(
                comparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v),
                keyComparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v),
                providerValueComparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v));
            originX.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            originX.AddAnnotation("Relational:ColumnName", "origin_X");

            var originY = runtimeEntityType.AddProperty(
                "OriginY",
                typeof(float),
                propertyInfo: typeof(BiotaPropertiesPosition).GetProperty("OriginY", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesPosition).GetField("<OriginY>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0f);
            originY.TypeMapping = MySqlFloatTypeMapping.Default.Clone(
                comparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v),
                keyComparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v),
                providerValueComparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v));
            originY.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            originY.AddAnnotation("Relational:ColumnName", "origin_Y");

            var originZ = runtimeEntityType.AddProperty(
                "OriginZ",
                typeof(float),
                propertyInfo: typeof(BiotaPropertiesPosition).GetProperty("OriginZ", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesPosition).GetField("<OriginZ>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0f);
            originZ.TypeMapping = MySqlFloatTypeMapping.Default.Clone(
                comparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v),
                keyComparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v),
                providerValueComparer: new ValueComparer<float>(
                    (float v1, float v2) => v1.Equals(v2),
                    (float v) => v.GetHashCode(),
                    (float v) => v));
            originZ.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            originZ.AddAnnotation("Relational:ColumnName", "origin_Z");

            var key = runtimeEntityType.AddKey(
                new[] { objectId, positionType });
            runtimeEntityType.SetPrimaryKey(key);
            key.AddAnnotation("Relational:Name", "PRIMARY");

            var type_cell_idx = runtimeEntityType.AddIndex(
                new[] { positionType, objCellId },
                name: "type_cell_idx");

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ObjectId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var @object = declaringEntityType.AddNavigation("Object",
                runtimeForeignKey,
                onDependent: true,
                typeof(Biota),
                propertyInfo: typeof(BiotaPropertiesPosition).GetProperty("Object", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesPosition).GetField("<Object>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var biotaPropertiesPosition = principalEntityType.AddNavigation("BiotaPropertiesPosition",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<BiotaPropertiesPosition>),
                propertyInfo: typeof(Biota).GetProperty("BiotaPropertiesPosition", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Biota).GetField("<BiotaPropertiesPosition>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            runtimeForeignKey.AddAnnotation("Relational:Name", "wcid_position");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "biota_properties_position");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
