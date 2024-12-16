using static ACRealms.Roslyn.RealmProps.Builders.SerializationHelpers;

namespace ACRealms.Roslyn.RealmProps.Builders.Phase2Src
{
    internal static class NamespaceJsonSchema
    {
        internal static string GenerateSchemaSourceCode(NamespaceData data)
        {
            try
            {
                var relativeDir = string.Concat(Enumerable.Repeat("../", data.NestedClassNames.Length + 1));
                var optionsBasePath = $"{relativeDir}options-base.json";
                var properties = MakePropertySchema(data);

                // We must wrap in a comment as the source generator treats the output as a C# file
                return
                $$"""
                /*
                {
                  "$schema": "http://json-schema.org/draft-07/schema",
                  "type": "object",
                  "definitions": {
                    "opts": { "$anchor": "opts.common", "$ref": "{{optionsBasePath}}#/definitions/commonOpts" },
                    "reroll": { "$anchor": "opts.reroll", "$ref": "{{optionsBasePath}}#/definitions/reroll" }
                  },
                  "properties": {
                    {{properties}}
                  },
                  "additionalProperties": false
                }
                */
                """;
            }
            catch (Exception ex)
            {
                Helpers.ReThrowWrappedException($"NamespaceJsonSchema.GenerateSchemaSourceCode ({data.NamespaceFull})", ex);
                throw;
            }
        }

        private static string MakeScopeDef(ObjPropInfo objPropInfo)
        {
            return
            $$"""
            {
              "$anchor": "{{objPropInfo.Key}}.s",
              "type": "object",
              "properties": {
                "fill_me_in": {
                  "type": "string"
                }
              }
            }
            """;
        }

        private static string MakePropertySchema(NamespaceData data)
        {
            List<string> propertyJsonSchema = [];

            foreach (var prop in data.ObjProps.Array)
            {
                string propSchema;
                if (prop is { Type: PropType.integer })
                {
                    var defaultVal = int.Parse(prop.AttributeDefault);
                    var minVal = int.Parse(prop.AttributeMinValue);
                    var maxVal = int.Parse(prop.AttributeMaxValue);
                    propSchema = MakeNumericPropSchema(prop, "integer", defaultVal, minVal, maxVal);
                }
                else if (prop is { Type: PropType.int64 })
                {
                    var defaultVal = long.Parse(prop.AttributeDefault);
                    var minVal = long.Parse(prop.AttributeMinValue);
                    var maxVal = long.Parse(prop.AttributeMaxValue);
                    propSchema = MakeNumericPropSchema(prop, "integer", defaultVal, minVal, maxVal);
                }
                else if (prop is { Type: PropType.@float })
                {
                    var defaultVal = double.Parse(prop.AttributeDefault);
                    var minVal = Math.Round(double.Parse(prop.AttributeMinValue), 6);
                    var maxVal = Math.Round(double.Parse(prop.AttributeMaxValue), 6);
                    propSchema = MakeNumericPropSchema(prop, "number", defaultVal, minVal, maxVal);
                }
                else if (prop is { Type: PropType.@string })
                {
                    var defaultVal = prop.AttributeDefault;
                    propSchema = MakeBasicPropSchema(prop, "string", defaultVal);
                }
                else if (prop is { Type: PropType.boolean })
                {
                    var defaultVal = prop.AttributeDefault;
                    propSchema = MakeBasicPropSchema(prop, "boolean", defaultVal);
                }
                else
                    return "";

                AddProp(propertyJsonSchema, prop.Key, propSchema);
            }

            return SerializePropsUnwrapped(propertyJsonSchema);
        }

        private static string MakeBasicPropSchema(ObjPropInfo propInfo, string valType, string defaultValLiteral)
        {
            var scopeDefSchema = MakeScopeDef(propInfo);
            var sVal = $$"""
                        { "$ref": "#{{propInfo.Key}}.v" }
                        """;

            return
            $$"""
            {
              "description": "{{propInfo.Description}}",
              "definitions": {
                "v": { "$anchor": "{{propInfo.Key}}.v", "type": "{{valType}}", "default": {{defaultValLiteral}} },
                "s": {{scopeDefSchema}},
                "p": { "$anchor": "{{propInfo.Key}}.p",
                  "allOf": [
                    { "type": "object", "properties": { "value": {{sVal}} }, "required": ["value"] },
                    { {{RefSnippet("#opts.common")}},
                      "allOf": [ { "type": "object", "properties": { "scope": {{ RefLiteral($"#{propInfo.Key}.s") }} } } ] } ]
                }
              },
              "oneOf": [
                {{sVal}},
                { {{RefSnippet($"#{propInfo.Key}.p")}}, "unevaluatedProperties": false },
                { "type": "array", "items": { "allOf": [ {{RefLiteral($"#{propInfo.Key}.p")}} ], "required": ["scope"], "unevaluatedProperties": false } }
              ]
            }
            """;
        }

        private static string MakeNumericPropSchema(ObjPropInfo propInfo, string valType, object defaultVal, object min, object max)
        {
            var scopeDefSchema = MakeScopeDef(propInfo);
            var sVal = RefLiteral($"#{propInfo.Key}.v");
            
            return
            $$"""
            {
              "description": "{{propInfo.Description}}",
              "definitions": {
                "v": { "$anchor": "{{propInfo.Key}}.v", "type": "{{valType}}", "minimum": {{min}}, "maximum": {{max}}, "default": {{defaultVal}} },
                "s": {{scopeDefSchema}},
                "p": { "$anchor": "{{propInfo.Key}}.p",
                  "allOf": [
                    { "oneOf": [
                      { "type": "object", "properties": { "value": {{sVal}} }, "required": ["value"] },
                      { "type": "object", "properties": { "low": {{sVal}}, "high": {{sVal}}, "reroll": {{ RefLiteral("#opts.reroll") }} }, "required": ["low","high"] } ] },
                    { {{ RefSnippet("#opts.common") }},
                      "allOf": [ { "type": "object", "properties": { "scope": {{ RefLiteral($"#{propInfo.Key}.s") }} } } ] } ]
                }
              },
              "oneOf": [
                {{sVal}},
                { {{ RefSnippet($"#{propInfo.Key}.p") }}, "unevaluatedProperties": false },
                { "type": "array", "items": { "allOf": [ {{RefLiteral($"#{propInfo.Key}.p") }} ], "required": ["scope"], "unevaluatedProperties": false } }
              ]  
            }
            """;
        }
    }
}
