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
    internal partial class WeeniePropertiesFloatEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "ACE.Database.Models.World.WeeniePropertiesFloat",
                typeof(WeeniePropertiesFloat),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(uint),
                propertyInfo: typeof(WeeniePropertiesFloat).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesFloat).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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

            var objectId = runtimeEntityType.AddProperty(
                "ObjectId",
                typeof(uint),
                propertyInfo: typeof(WeeniePropertiesFloat).GetProperty("ObjectId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesFloat).GetField("<ObjectId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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

            var type = runtimeEntityType.AddProperty(
                "Type",
                typeof(ushort),
                propertyInfo: typeof(WeeniePropertiesFloat).GetProperty("Type", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesFloat).GetField("<Type>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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
                propertyInfo: typeof(WeeniePropertiesFloat).GetProperty("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesFloat).GetField("<Value>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
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
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var wcid_float_type_uidx = runtimeEntityType.AddIndex(
                new[] { objectId, type },
                name: "wcid_float_type_uidx",
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
                propertyInfo: typeof(WeeniePropertiesFloat).GetProperty("Object", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WeeniePropertiesFloat).GetField("<Object>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var weeniePropertiesFloat = principalEntityType.AddNavigation("WeeniePropertiesFloat",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<WeeniePropertiesFloat>),
                propertyInfo: typeof(Weenie).GetProperty("WeeniePropertiesFloat", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Weenie).GetField("<WeeniePropertiesFloat>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            runtimeForeignKey.AddAnnotation("Relational:Name", "wcid_float");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "weenie_properties_float");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
