﻿// <auto-generated />
using System;
using System.Reflection;
using ACE.Database.Models.World;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

#pragma warning disable 219, 612, 618
#nullable disable

namespace ACE.Database.CompiledModels.World
{
    internal partial class WeenieEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "ACE.Database.Models.World.Weenie",
                typeof(Weenie),
                baseEntityType);

            var classId = runtimeEntityType.AddProperty(
                "ClassId",
                typeof(uint),
                propertyInfo: typeof(Weenie).GetProperty("ClassId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Weenie).GetField("<ClassId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: 0u);
            classId.TypeMapping = MySqlUIntTypeMapping.Default.Clone(
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
            classId.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
            classId.AddAnnotation("Relational:ColumnName", "class_Id");

            var className = runtimeEntityType.AddProperty(
                "ClassName",
                typeof(string),
                propertyInfo: typeof(Weenie).GetProperty("ClassName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Weenie).GetField("<ClassName>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 100);
            className.TypeMapping = MySqlStringTypeMapping.Default.Clone(
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
                    storeTypeName: "varchar(100)",
                    size: 100));
            className.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            className.AddAnnotation("Relational:ColumnName", "class_Name");

            var lastModified = runtimeEntityType.AddProperty(
                "LastModified",
                typeof(DateTime),
                propertyInfo: typeof(Weenie).GetProperty("LastModified", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Weenie).GetField("<LastModified>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAddOrUpdate,
                beforeSaveBehavior: PropertySaveBehavior.Ignore,
                afterSaveBehavior: PropertySaveBehavior.Ignore,
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            lastModified.TypeMapping = MySqlDateTimeTypeMapping.Default.Clone(
                comparer: new ValueComparer<DateTime>(
                    (DateTime v1, DateTime v2) => v1.Equals(v2),
                    (DateTime v) => v.GetHashCode(),
                    (DateTime v) => v),
                keyComparer: new ValueComparer<DateTime>(
                    (DateTime v1, DateTime v2) => v1.Equals(v2),
                    (DateTime v) => v.GetHashCode(),
                    (DateTime v) => v),
                providerValueComparer: new ValueComparer<DateTime>(
                    (DateTime v1, DateTime v2) => v1.Equals(v2),
                    (DateTime v) => v.GetHashCode(),
                    (DateTime v) => v));
            lastModified.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);
            lastModified.AddAnnotation("Relational:ColumnName", "last_Modified");
            lastModified.AddAnnotation("Relational:ColumnType", "datetime");
            lastModified.AddAnnotation("Relational:DefaultValueSql", "CURRENT_TIMESTAMP");

            var type = runtimeEntityType.AddProperty(
                "Type",
                typeof(int),
                propertyInfo: typeof(Weenie).GetProperty("Type", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Weenie).GetField("<Type>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            type.TypeMapping = MySqlIntTypeMapping.Default.Clone(
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
            type.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            type.AddAnnotation("Relational:ColumnName", "type");

            var key = runtimeEntityType.AddKey(
                new[] { classId });
            runtimeEntityType.SetPrimaryKey(key);
            key.AddAnnotation("Relational:Name", "PRIMARY");

            var className_UNIQUE = runtimeEntityType.AddIndex(
                new[] { className },
                name: "className_UNIQUE",
                unique: true);

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "weenie");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
