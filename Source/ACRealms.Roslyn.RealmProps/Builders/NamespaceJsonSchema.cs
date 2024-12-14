using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ACRealms.Roslyn.RealmProps.Builders
{
    internal static class NamespaceJsonSchema
    {
        internal static string GenerateSchemaSourceCode(NamespaceData data)
        {
            try
            {
                object schema = MakeNamespaceSchema(data);
                var schemaSerialized = JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });

                return $$"""
                    /*
                    {{schemaSerialized}}
                    */
                    """;
            }
            catch (Exception ex)
            {
                Helpers.ReThrowWrappedException($"NamespaceJsonSchema.GenerateSchemaSourceCode ({data.NamespaceFull})", ex);
                throw;
            }
        }

        private static object MakeNamespaceSchema(NamespaceData data)
        {
            var properties = MakePropertySchema(data);

            var idShort = data.NamespaceFull.Replace(".", "/");
            var id = $"https://realm.ac/schema/v1/generated/realm-properties/{idShort}.json";

            var sb = new StringBuilder();
            for (int i = 0; i < data.NestedClassNames.Length + 1; i++)
                sb.Append("../");
            var relativeDir = sb.ToString();
            var optionsBasePath = $"{relativeDir}options-base.json";

            var definitionsDict = new Dictionary<string, Dictionary<string, object>>()
            {
                { "commonOpts", new Dictionary<string, object> { { "$ref", $"{optionsBasePath}#/definitions/commonOpts" } } }
            };

            var realmPropertiesSchema = new Dictionary<string, object>()
            {
                {   "$schema", "http://json-schema.org/draft-07/schema" },
                {   "$id", id },
                {   "type", "object" },
                {   "minItems", 1 },
                {   "definitions", definitionsDict },
                {   "properties", properties },
                {   "additionalProperties", false }
            };

            return realmPropertiesSchema;
        }

        private static Dictionary<string, object> MakeScopeDef(ObjPropInfo objPropInfo)
        {
            var dict = new Dictionary<string, object>()
            {
                { "type", "object" },
                { "properties", new Dictionary<string, object>() {
                    { "compiled_from_meta", new Dictionary<string, object>() {
                        { "type", "string" } } } }
                }
            };

            return dict;
        }

        private static Dictionary<string, object> MakePropertySchema(NamespaceData data)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < data.NestedClassNames.Length + 1; i++)
                sb.Append("../");
            var relativeDir = sb.ToString();
            var optionsBasePath = $"{relativeDir}options-base.json";

            var probabilitySchema = new Dictionary<string, object>()
            {
                { "$ref", $"{optionsBasePath}#/definitions/probability" }
            };

            var composeSchema = new Dictionary<string, object>()
            {
                { "$ref", $"{optionsBasePath}#/definitions/compose" }
            };
            var lockedSchema = new Dictionary<string, object>()
            {
                { "$ref", $"{optionsBasePath}#/definitions/locked" }
            };
            var rerollSchema = new Dictionary<string, object>()
            {
                { "$ref", $"{optionsBasePath}#/definitions/reroll" }
            };

            var propertyJsonSchema = new Dictionary<string, object>();

            var propsOf = (PropType type) => data.ObjProps.Array.Where(p => p.Type == type);

            foreach (var propInt in propsOf(PropType.integer))
            {
                var defaultVal = int.Parse(propInt.AttributeDefault);
                var minVal = int.Parse(propInt.AttributeMinValue);
                var maxVal = int.Parse(propInt.AttributeMaxValue);
                propertyJsonSchema.Add(propInt.Key, MakeNumericPropSchema(rerollSchema, propInt, "integer", defaultVal, minVal, maxVal));
            }

            
            foreach (var propLong in propsOf(PropType.int64))
            {
                var defaultVal = long.Parse(propLong.AttributeDefault);
                var minVal = long.Parse(propLong.AttributeMinValue);
                var maxVal = long.Parse(propLong.AttributeMaxValue);
                propertyJsonSchema.Add(propLong.Key, MakeNumericPropSchema(rerollSchema, propLong, "integer", defaultVal, minVal, maxVal));
            }

            foreach (var propFloat in propsOf(PropType.@float))
            {
                var defaultVal = double.Parse(propFloat.AttributeDefault);
                var minVal = Math.Round(double.Parse(propFloat.AttributeMinValue), 6);
                var maxVal = Math.Round(double.Parse(propFloat.AttributeMaxValue), 6);
                propertyJsonSchema.Add(propFloat.Key, MakeNumericPropSchema(rerollSchema, propFloat, "number", defaultVal, minVal, maxVal));
            }

            foreach (var propString in propsOf(PropType.@string))
            {
                var propertySchema = new Dictionary<string, object>();

                var directValueSchema = new Dictionary<string, object>()
                {
                    { "$ref", $"#/properties/{propString.Key}/definitions/val" }
                };

                var defaultval = propString.AttributeDefault;

                var defSchema = new Dictionary<string, object>()
                {
                    { "val", new Dictionary<string, object>()
                    {
                        { "type", "string" },
                        { "default", defaultval.Substring(1, defaultval.Length - 2) }
                    } }
                };

                propertySchema.Add("description", propString.Description);

                propertySchema.Add("definitions", defSchema);

                var typeOneSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "value", directValueSchema },
                        { "probability", probabilitySchema },
                        { "locked", lockedSchema } } },
                    { "required", new List<string>() { "value" } },
                    { "additionalProperties", false }
                };


                propertySchema.Add("oneOf", new List<object>()
                {
                    directValueSchema,
                    typeOneSchema
                });
                propertyJsonSchema.Add(propString.Key.ToString(), propertySchema);
            }

            foreach (var propBool in propsOf(PropType.boolean))
            {
                var propertySchema = new Dictionary<string, object>();
                
                var directValueSchema = new Dictionary<string, object>()
                {
                    { "$ref", $"#/properties/{propBool.Key}/definitions/val" }
                };

                var defSchema = new Dictionary<string, object>()
                {
                    { "val", new Dictionary<string, object>()
                    {
                        { "type", "boolean" },
                        { "default", propBool.AttributeDefault == "true" }
                    } }
                };

                propertySchema.Add("description", propBool.Description);

                propertySchema.Add("definitions", defSchema);

                var typeOneSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "value", directValueSchema },
                        { "probability", probabilitySchema },
                        { "locked", lockedSchema } } },
                    { "required", new List<string>() { "value" } },
                    { "additionalProperties", false }
                };

                propertySchema.Add("oneOf", new List<object>()
                {
                    directValueSchema,
                    typeOneSchema
                });

                propertyJsonSchema.Add(propBool.Key.ToString(), propertySchema);
            }
            
            return propertyJsonSchema;
        }

        private static Dictionary<string, object> MakeNumericPropSchema(Dictionary<string, object> rerollSchema, ObjPropInfo propInfo, string valType, object defaultVal, object min, object max)
        {
            var conv = static (string s) => double.Parse(s);
            var scopeDefSchema = MakeScopeDef(propInfo);

            var directValueSchema = new Dictionary<string, object>()
                {
                    { "$ref", $"#/properties/{propInfo.Key}/definitions/val" }
                };

            var propDefSchema = new Dictionary<string, object>()
                {
                    { "allOf", new Dictionary<string, object>[] {
                        new Dictionary<string, object> {
                            { "oneOf", new Dictionary<string, object>[] {
                                new Dictionary<string, object> {
                                    { "type", "object" },
                                    { "properties", new Dictionary<string, object> { { "value", directValueSchema } } },
                                    { "required", new string[] { "value" } } },
                                new Dictionary<string, object> {
                                    { "type", "object" },
                                    { "properties", new Dictionary<string, object> {
                                        { "low", directValueSchema },
                                        { "high", directValueSchema },
                                        { "reroll", rerollSchema } } },
                                    { "required", new string[] { "low", "high" } } } } } },
                        new Dictionary<string, object>
                        {
                            { "$ref", "#/definitions/commonOpts" },
                            { "allOf", new Dictionary<string, object>[] { new Dictionary<string, object> {
                                { "type", "object" },
                                { "properties", new Dictionary<string, object> {
                                    { "scope", new Dictionary<string, object> { { "$ref", $"#/properties/{propInfo.Key}/definitions/scopeDef" } } } } } } } }
                        } } }
                };

            var defSchema = new Dictionary<string, object>()
                {
                    { "val", new Dictionary<string, object>() {
                        { "type", valType },
                        { "minimum", min },
                        { "maximum", max },
                        { "default", defaultVal } } },
                    { "scopeDef", scopeDefSchema },
                    { "propDef", propDefSchema }
                };

            var mainOneOfSchema = new Dictionary<string, object>[]
            {
                    directValueSchema,
                    new Dictionary<string, object> {
                        { "$ref", $"#/properties/{propInfo.Key}/definitions/propDef" },
                        { "unevaluatedProperties", false } },
                    new Dictionary<string, object> {
                        { "type", "array" },
                        { "items",  new Dictionary<string, object> {
                            { "allOf", new object[] { new Dictionary<string, object> { { "$ref", $"#/properties/{propInfo.Key}/definitions/propDef" } } } },
                            { "required", new string[] { "scope" } },
                            { "unevaluatedProperties", false } }
                        },
                        { "unevaluatedProperties", false }
                    }
            };

            var propertySchema = new Dictionary<string, object>()
                {
                    { "description", propInfo.Description },
                    { "definitions", defSchema },
                    { "oneOf", mainOneOfSchema }
                };

            return propertySchema;
        }
    }
}
