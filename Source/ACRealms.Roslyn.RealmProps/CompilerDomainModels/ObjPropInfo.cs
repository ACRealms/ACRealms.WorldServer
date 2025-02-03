using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        public string TargetEnumTypeName => PropMap[Type];
        public ImmutableArray<PropContext> Contexts { get; init; } = [];

        public static readonly FrozenDictionary<PropType, string> PropMap = new Dictionary<PropType, string>()
        {
            { PropType.@string, "RealmPropertyString" },
            { PropType.integer, "RealmPropertyInt" },
            { PropType.boolean, "RealmPropertyBool" },
            { PropType.int64, "RealmPropertyInt64" },
            { PropType.@float, "RealmPropertyFloat" },
            { PropType.@enum, "RealmPropertyInt" }
        }.ToFrozenDictionary();

        public static readonly FrozenDictionary<PropType, PrimitiveType> PrimitiveMap = new Dictionary<PropType, PrimitiveType>()
        {
            { PropType.@string, PrimitiveType.@string },
            { PropType.integer, PrimitiveType.@int },
            { PropType.boolean, PrimitiveType.@bool },
            { PropType.int64, PrimitiveType.@long },
            { PropType.@float, PrimitiveType.@double },
            { PropType.@enum, PrimitiveType.@enum }
        }.ToFrozenDictionary();

        public string CoreEnumType => PropMap[Type];
        public PrimitiveType PrimitiveType => PrimitiveMap[Type];
        public string PropTypeDecl => PrimitiveType switch
        {
            PrimitiveType.@enum => Enum,
            _ => PrimitiveType.ToString()
        };

        public string ToNamespacedAliasDeclaration(string spacer)
        {
            string? obs = ObsoleteReason != null ? $$"""

            {{spacer}}[System.Obsolete("{{ObsoleteReason}}")]
            """ : "";

            var ctxs = Contexts
                .OrderByDescending(static x => x.Required)
                .Select<PropContext, (PropContext ctx, string ctxType)>(
                    ctx => (ctx, EntityToContextEntityMapping.GetScopedAttributeType(ctx.Entity)))
                .ToImmutableArray();
            IEnumerable<string> declArgsCollection = ["IAppliedRuleset ruleset", .. ctxs.Select(x => 
            {
                var decl = $"{x.ctxType} {x.ctx.Name}";
                if (!x.ctx.Required)
                    decl += " = null";
                return decl;
            })];
            var declArgs = string.Join(", ", declArgsCollection);

            IEnumerable<string> invokeArgsCollection = [$"{CoreEnumType}.{CoreKey}", .. ctxs.Select(x => $"(\"{x.ctx.Name}\", {x.ctx.Name})")];
            var invokeArgs = string.Join(", ", invokeArgsCollection);

            var returnCast = Type == PropType.@enum ? $"({PropTypeDecl})" : string.Empty;
            /*return
            $$"""
            {{spacer}}/// <summary>{{Description}}</summary>{{obs}}
            {{spacer}}public const {{CoreEnumType}} {{Key}} = {{CoreEnumType}}.{{CoreKey}};
            """;*/
            return
            $$"""
            {{spacer}}public static {{PropTypeDecl}} {{Key}}({{declArgs}})
            {{spacer}}  => {{returnCast}}ruleset.ValueOf({{invokeArgs}});

            """;
            /*
            public static int StrengthAdded2(IAppliedRuleset ruleset, ICanonicalContextEntity SpawnedCreature)
            {
                return ruleset.ValueOf(ACRealms.Props.Creature.Attributes.StrengthAdded, ("SpawnedCreature", SpawnedCreature));
            }
            */
            }

        private string GetDefaultLiteral()
        {
            return PrimitiveType switch
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

        public string AttributeDefault => Default ?? GetDefaultLiteral();
        public string AttributeMinValue => MinValue ?? GetDefaultLiteral();
        public string AttributeMaxValue => MaxValue ?? GetDefaultLiteral(); 
        private IEnumerable<string> CorePrimaryAttributeArgs(string canonicalPrimaryAttributeType)
        {
            string[] args = DefaultFromServerProp != null ? [$"\"{DefaultFromServerProp}\""] : [];
            string[] rest = canonicalPrimaryAttributeType switch
            {
                "RealmPropertyPrimaryAttribute" => [AttributeDefault],
                "RealmPropertyEnumAttribute" => [AttributeDefault],
                "RealmPropertyPrimaryMinMaxAttribute" => [
                    AttributeDefault,
                    AttributeMinValue,
                    AttributeMaxValue
                ],
                _ => throw new NotImplementedException($"Missing handler for {canonicalPrimaryAttributeType}")
            };
        
            return [.. args, .. rest];
        }

        private string CorePrimaryAttribute(string aliasedPrimaryAttributeType, string canonicalPrimaryAttributeType)
        {
            string typeArg = PrimitiveType switch
            {
                PrimitiveType.@enum => $"<{Enum}>",
                _ => string.Empty
            };

            string? primaryArgs = string.Join(", ", CorePrimaryAttributeArgs(canonicalPrimaryAttributeType));
            return
            $$"""
            [{{aliasedPrimaryAttributeType}}{{typeArg}}({{primaryArgs}})]
            """;
        }

        public string ToCoreEnumDeclaration(
            string aliasedPrimaryAttributeType,
            string canonicalPrimaryAttributeType)
        {
            string? obs = ObsoleteReason != null ? $$"""

                [Obsolete("{{ObsoleteReason}}")]
            """ : "";

            string? rerollRestriction = RerollRestrictedTo != null ? $$"""

                [RerollRestrictedTo(RealmPropertyRerollType.{{RerollRestrictedTo}})]
            """ : "";

            StringBuilder sbCtxs = new();
            int count = 0;
            string newline = """

            """;

            foreach (var ctx in Contexts)
            {
                count++;
                string scopedAttributeType = EntityToContextEntityMapping.GetScopedAttributeType(ctx.Entity);
                var decl =
                $$"""
                    [ScopedWith<{{scopedAttributeType}}>("{{ctx.Name}}", required: {{(ctx.Required ? "true" : "false")}}, entity: "{{ctx.Entity}}", "{{ctx.Description}}")]{{(count == Contexts.Length ? newline : "")}}
                """;
                sbCtxs.Append(decl);
            }
            string contextDecls = Contexts.IsEmpty ? "" : $$"""

            {{sbCtxs}}
            """;

            return
            $$"""
                [Description("{{Description}}")]{{obs}}{{rerollRestriction}}
                {{CorePrimaryAttribute(aliasedPrimaryAttributeType, canonicalPrimaryAttributeType)}}{{contextDecls}}
                {{CoreKey}},
            """;
        }
    }
}
