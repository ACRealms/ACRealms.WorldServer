using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Text;

namespace ACRealms.Roslyn.RealmProps
{
    // "propertytype": { "type": "string", "enum": [ "integer", "boolean", "float", "int64", "string", "enum" ] },

    enum PropType
    {
        None,
        @enum,
        integer,
        int64,
        @float,
        boolean,
        @string
    }

    enum PrimitiveType
    {
        @null,
        @bool,
        @int,
        @long,
        @double,
        @string,
        @enum
    }

    record ObjPropInfo(string NamespaceRaw, string Key)
    {
        public string CoreKey { get; } = $"{NamespaceRaw.Replace('.', '_')}_{Key}";
        public required string Description { get; init; }
        public required PropType Type { get; init; }
        public string? Enum { get; init; }
        public string? Default { get; init; }

        public string? MinValue { get; init; }
        public string? MaxValue { get; init; }
        public string? RerollRestrictedTo { get; init; }
        public string? ObsoleteReason { get; init; }
        public string? DefaultFromServerProp { get; init; }

        public static readonly FrozenDictionary<PropType, string> PropMap = new Dictionary<PropType, string>()
        {
            { PropType.@string, "RealmPropertyString" },
            { PropType.integer, "RealmPropertyInt" },
            { PropType.boolean, "RealmPropertyBool" },
            { PropType.int64, "RealmPropertyInt64" },
            { PropType.@float, "RealmPropertyFloat" },
            { PropType.@enum, "RealmPropertyInt" }
        }.ToFrozenDictionary();

        private string CoreEnumType => PropMap[Type];
        public string ToNamespacedAliasDeclaration(string spacer)
        {
            string? obs = ObsoleteReason != null ? $$"""

            {{spacer}}[System.Obsolete("{{ObsoleteReason}}")]
            """ : "";

            return
            $$"""
            {{spacer}}/// <summary>{{Description}}</summary>{{obs}}
            {{spacer}}public const {{CoreEnumType}} {{Key}} = {{CoreEnumType}}.{{CoreKey}};
            """;
        }

        private string DefaultLiteral(PrimitiveType valuePrimitiveType)
        {
            return valuePrimitiveType switch
            {
                PrimitiveType.@bool => "false",
                PrimitiveType.@double => "0.0",
                PrimitiveType.@int => "0",
                PrimitiveType.@long => "0",
                PrimitiveType.@string => "\"\"",
                PrimitiveType.@enum => $"({Enum})0",
                _ => "<!INVALID!>"
            };
        }

        private IEnumerable<string> CorePrimaryAttributeArgs(string canonicalPrimaryAttributeType, PrimitiveType valuePrimitiveType)
        {
            return canonicalPrimaryAttributeType switch
            {
                "RealmPropertyPrimaryAttribute" => [Default ?? DefaultLiteral(valuePrimitiveType)],
                "RealmPropertyEnumAttribute" => [Default ?? DefaultLiteral(valuePrimitiveType)],
                "RealmPropertyPrimaryMinMaxAttribute" => [
                    Default ?? DefaultLiteral(valuePrimitiveType),
                    MinValue ?? DefaultLiteral(valuePrimitiveType),
                    MaxValue ?? DefaultLiteral(valuePrimitiveType)
                ],
                _ => throw new NotImplementedException($"Missing handler for {canonicalPrimaryAttributeType}")
            };
        }

        private string CorePrimaryAttribute(string aliasedPrimaryAttributeType, string canonicalPrimaryAttributeType, PrimitiveType valuePrimitiveType)
        {
            string typeArg = valuePrimitiveType switch
            {
                PrimitiveType.@enum => $"<{Enum}>",
                _ => String.Empty
            };

            string? primaryArgs = string.Join(", ", CorePrimaryAttributeArgs(canonicalPrimaryAttributeType, valuePrimitiveType));
            return
            $$"""
            [{{aliasedPrimaryAttributeType}}{{typeArg}}({{primaryArgs}})]
            """;
        }

        public string ToCoreEnumDeclaration(
            string aliasedPrimaryAttributeType,
            string canonicalPrimaryAttributeType,
            PrimitiveType valuePrimitiveType)
        {
            string? obs = ObsoleteReason != null ? $$"""

                [Obsolete("{{ObsoleteReason}}")]
            """ : "";
            string? rerollRestriction = RerollRestrictedTo != null ? $$"""

                [RerollRestrictedTo(RealmPropertyRerollType.{{RerollRestrictedTo}})]
            """ : "";
            return
            $$"""
                [Description("{{Description}}")]{{obs}}{{rerollRestriction}}
                {{CorePrimaryAttribute(aliasedPrimaryAttributeType, canonicalPrimaryAttributeType, valuePrimitiveType)}}
                {{CoreKey}},
            """;
        }
    }
}
