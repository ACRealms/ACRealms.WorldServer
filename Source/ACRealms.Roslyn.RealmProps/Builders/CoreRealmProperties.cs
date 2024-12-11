using System;
using System.Collections.Generic;
using System.Text;

namespace ACRealms.Roslyn.RealmProps.Builders
{
    // Builds the classes RealmPropertyInt, RealmPropertyBool, etc
    internal static class CoreRealmProperties
    {
        internal static string GenerateCoreEnumClass(string targetEnumTypeName, ImmutableArray<ObjPropInfo> propsOfType)
        {
            try
            {
                (string AliasedPrimaryAttributeType, string CanonicalPrimaryAttributeType, PrimitiveType)? primaryAttrDataNullable = targetEnumTypeName switch
                {
                    "RealmPropertyString" => ("RealmPropertyStringAttribute", "RealmPropertyPrimaryAttribute", PrimitiveType.@string),
                    "RealmPropertyInt" => ("RealmPropertyIntAttribute", "RealmPropertyPrimaryMinMaxAttribute", PrimitiveType.@int),
                    "RealmPropertyInt64" => ("RealmPropertyInt64Attribute", "RealmPropertyPrimaryMinMaxAttribute", PrimitiveType.@long),
                    "RealmPropertyBool" => ("RealmPropertyBoolAttribute", "RealmPropertyPrimaryAttribute", PrimitiveType.@bool),
                    "RealmPropertyFloat" => ("RealmPropertyFloatAttribute", "RealmPropertyPrimaryMinMaxAttribute", PrimitiveType.@double),
                    // "RealmPropertyEnum" => ("RealmPropertyEnumAttribute", "RealmPropertyEnumAttribute", PrimitiveType.@enum),
                    _ => throw new Exception($"Unexpected propType '{targetEnumTypeName}'")
                };
                if (!primaryAttrDataNullable.HasValue)
                    throw new Exception($"Unexpected propType '{targetEnumTypeName}'");
                (string AliasedPrimaryAttributeType, string CanonicalPrimaryAttributeType, PrimitiveType ValuePrimitiveType) = primaryAttrDataNullable!.Value;

                string newline = $$"""



            """;

                string? canonicalPropDecls = string.Join(newline, propsOfType.Select(p =>
                {
                    if (p.NamespaceRaw == "__NONE__")
                        return "";
                    string? thisAliasedPrimaryAttributeType = p.Type switch
                    {
                        PropType.@enum => "RealmPropertyEnumAttribute",
                        _ => AliasedPrimaryAttributeType
                    };
                    string? thisCanonicalPrimaryAttributeType = p.Type switch
                    {
                        PropType.@enum => "RealmPropertyEnumAttribute",
                        _ => CanonicalPrimaryAttributeType
                    };
                    PrimitiveType thisValuePrimitiveType = p.Type switch
                    {
                        PropType.@enum => PrimitiveType.@enum,
                        _ => ValuePrimitiveType
                    };
                    return p.ToCoreEnumDeclaration(thisAliasedPrimaryAttributeType, thisCanonicalPrimaryAttributeType);
                }));
                //{{(targetEnumTypeName == "RealmPropertyInt" ? $"using RealmPropertyEnumAttribute = ACE.Entity.Enum.Properties.RealmPropertyEnumAttribute<{ValuePrimitiveType}>;" : "")}}}

                return $$""""
            global using {{targetEnumTypeName}} = ACRealms.RealmProps.Underlying.{{targetEnumTypeName}};
            using ACRealms.Rulesets.Enums;
            using ACRealms.RealmProps.Enums;
            using System;
            using System.ComponentModel;
            using {{AliasedPrimaryAttributeType}} = ACRealms.RealmProps.{{CanonicalPrimaryAttributeType}}<{{ValuePrimitiveType}}>;
            
            // THIS FILE IS AUTOMATICALLY GENERATED

            namespace ACRealms.RealmProps.Underlying;

            [RequiresPrimaryAttribute<{{CanonicalPrimaryAttributeType}}<{{ValuePrimitiveType}}>, {{ValuePrimitiveType}}>]
            public enum {{targetEnumTypeName}} : uint
            {
                Undef = 0,

            {{canonicalPropDecls}}
            }
            
            """";
            }
            catch (Exception ex)
            {
                Helpers.ReThrowWrappedException($"GenerateCoreEnumClass ({targetEnumTypeName})", ex);
                throw;
            }
        }
    }
}
