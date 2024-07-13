﻿// <auto-generated />
using System;
using System.Reflection;
using ACE.Database.Models.Auth;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

#pragma warning disable 219, 612, 618
#nullable disable

namespace ACE.Database.CompiledModels.Auth
{
    internal partial class AccesslevelEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "ACE.Database.Models.Auth.Accesslevel",
                typeof(Accesslevel),
                baseEntityType);

            var level = runtimeEntityType.AddProperty(
                "Level",
                typeof(uint),
                propertyInfo: typeof(Accesslevel).GetProperty("Level", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Accesslevel).GetField("<Level>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: 0u);
            level.TypeMapping = MySqlUIntTypeMapping.Default.Clone(
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
            level.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            level.AddAnnotation("Relational:ColumnName", "level");

            var name = runtimeEntityType.AddProperty(
                "Name",
                typeof(string),
                propertyInfo: typeof(Accesslevel).GetProperty("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Accesslevel).GetField("<Name>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 45);
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
                    storeTypeName: "varchar(45)",
                    size: 45));
            name.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            name.AddAnnotation("Relational:ColumnName", "name");

            var prefix = runtimeEntityType.AddProperty(
                "Prefix",
                typeof(string),
                propertyInfo: typeof(Accesslevel).GetProperty("Prefix", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Accesslevel).GetField("<Prefix>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                valueGenerated: ValueGenerated.OnAdd,
                maxLength: 45);
            prefix.TypeMapping = MySqlStringTypeMapping.Default.Clone(
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
                    storeTypeName: "varchar(45)",
                    size: 45));
            prefix.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            prefix.AddAnnotation("Relational:ColumnName", "prefix");
            prefix.AddAnnotation("Relational:DefaultValueSql", "''");

            var key = runtimeEntityType.AddKey(
                new[] { level });
            runtimeEntityType.SetPrimaryKey(key);
            key.AddAnnotation("Relational:Name", "PRIMARY");

            var level0 = runtimeEntityType.AddIndex(
                new[] { level },
                name: "level",
                unique: true);

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "accesslevel");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
