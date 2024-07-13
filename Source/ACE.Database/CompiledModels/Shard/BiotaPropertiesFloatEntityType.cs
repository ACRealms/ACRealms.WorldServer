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
    internal partial class BiotaPropertiesFloatEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "ACE.Database.Models.Shard.BiotaPropertiesFloat",
                typeof(BiotaPropertiesFloat),
                baseEntityType);

            var objectId = runtimeEntityType.AddProperty(
                "ObjectId",
                typeof(ulong),
                propertyInfo: typeof(BiotaPropertiesFloat).GetProperty("ObjectId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesFloat).GetField("<ObjectId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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

            var type = runtimeEntityType.AddProperty(
                "Type",
                typeof(ushort),
                propertyInfo: typeof(BiotaPropertiesFloat).GetProperty("Type", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesFloat).GetField("<Type>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: (ushort)0);
            type.TypeMapping = MySqlUShortTypeMapping.Default.Clone(
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
            type.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            type.AddAnnotation("Relational:ColumnName", "type");

            var value = runtimeEntityType.AddProperty(
                "Value",
                typeof(double),
                propertyInfo: typeof(BiotaPropertiesFloat).GetProperty("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesFloat).GetField("<Value>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            value.TypeMapping = MySqlDoubleTypeMapping.Default.Clone(
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
            value.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            value.AddAnnotation("Relational:ColumnName", "value");

            var key = runtimeEntityType.AddKey(
                new[] { objectId, type });
            runtimeEntityType.SetPrimaryKey(key);
            key.AddAnnotation("Relational:Name", "PRIMARY");

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
                propertyInfo: typeof(BiotaPropertiesFloat).GetProperty("Object", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BiotaPropertiesFloat).GetField("<Object>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var biotaPropertiesFloat = principalEntityType.AddNavigation("BiotaPropertiesFloat",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<BiotaPropertiesFloat>),
                propertyInfo: typeof(Biota).GetProperty("BiotaPropertiesFloat", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Biota).GetField("<BiotaPropertiesFloat>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            runtimeForeignKey.AddAnnotation("Relational:Name", "wcid_float");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "biota_properties_float");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
