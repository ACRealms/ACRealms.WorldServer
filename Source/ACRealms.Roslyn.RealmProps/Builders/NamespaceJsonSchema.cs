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
            var realmPropertiesSchema = new Dictionary<string, object>()
            {
                {   "$schema", "http://json-schema.org/draft-07/schema" },
                {   "$id", id },
                {   "type", "object" },
                {   "minItems", 1 },
                {   "properties", properties },
                {   "additionalProperties", false }
            };

            return realmPropertiesSchema;
        }

        private static Dictionary<string, object> MakePropertySchema(NamespaceData data)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < data.NestedClassNames.Length; i++)
                sb.Append("../");
            var relativeDir = sb.ToString();
            var optionsBasePath = $"{relativeDir}/options-base.json";

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

            foreach (var propInt in data.ObjProps.Array.Where(p => p.Type == PropType.integer))
            {
                var propertySchema = new Dictionary<string, object>();
                var conv = (string s) => int.Parse(s);

                var directValueSchema = new Dictionary<string, object>()
                {
                    { "$ref", $"#/properties/{propInt.Key}/definitions/val" }
                };

                var defSchema = new Dictionary<string, object>()
                {
                    { "val", new Dictionary<string, object>()
                    {
                        { "type", "integer" },
                        { "minimum", conv(propInt.AttributeMinValue) },
                        { "maximum", conv(propInt.AttributeMaxValue) },
                        { "default", conv(propInt.AttributeDefault) }
                    } }
                };

                propertySchema.Add("description", propInt.Description);

                propertySchema.Add("definitions", defSchema);

                var typeOneSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "value", directValueSchema },
                        { "probability", probabilitySchema },
                        { "compose", composeSchema },
                        { "locked", lockedSchema } } },
                    { "required", new List<string>() { "value" } },
                    { "additionalProperties", false }
                };

                var typeTwoSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "low", directValueSchema },
                        { "high", directValueSchema},
                        { "probability", probabilitySchema },
                        { "reroll", rerollSchema },
                        { "compose", composeSchema },
                        { "locked", lockedSchema } } },
                    { "required", new List<string>() { "low", "high" } },
                    { "additionalProperties", false }
                };

                propertySchema.Add("oneOf", new List<object>()
                {
                    directValueSchema,
                    new Dictionary<string, object>()
                    {
                        { "oneOf", new List<object>()
                        {
                            typeOneSchema,
                            typeTwoSchema
                        } }
                    }
                });
                propertyJsonSchema.Add(propInt.Key, propertySchema);
            }

            /*
            foreach (var propLong in propertyDefinitionsInt64)
            {
                var att = propLong.Value.PrimaryAttribute;
                var propertySchema = new Dictionary<string, object>();

                var directValueSchema = new Dictionary<string, object>()
                {
                    { "$ref", $"#/properties/{propLong.Key}/definitions/val" }
                };

                var defSchema = new Dictionary<string, object>()
                {
                    { "val", new Dictionary<string, object>()
                    {
                        { "type", "integer" },
                        { "minimum", att.MinValue },
                        { "maximum", att.MaxValue },
                        { "default", att.DefaultValue }
                    } }
                };

                var descriptionAtt = propLong.Key.GetAttributeOfType<System.ComponentModel.DescriptionAttribute>();
                propertySchema.Add("description", descriptionAtt?.Description ?? "(no description)");

                propertySchema.Add("definitions", defSchema);

                var typeOneSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "value", directValueSchema },
                        { "probability", probabilitySchema },
                        { "compose", composeSchema },
                        { "locked", lockedSchema } } },
                    { "required", new List<string>() { "value" } },
                    { "additionalProperties", false }
                };

                var typeTwoSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "low", directValueSchema },
                        { "high", directValueSchema},
                        { "probability", probabilitySchema },
                        { "reroll", rerollSchema },
                        { "compose", composeSchema },
                        { "locked", lockedSchema } } },
                    { "required", new List<string>() { "low", "high" } },
                    { "additionalProperties", false }
                };

                propertySchema.Add("oneOf", new List<object>()
                {
                    directValueSchema,
                    new Dictionary<string, object>()
                    {
                        { "oneOf", new List<object>()
                        {
                            typeOneSchema,
                            typeTwoSchema
                        } }
                    }
                });

                propertyJsonSchema.Add(propLong.Key.ToString(), propertySchema);
            }

            foreach (var propFloat in propertyDefinitionsFloat)
            {
                var att = propFloat.Value.PrimaryAttribute;
                var propertySchema = new Dictionary<string, object>();

                var directValueSchema = new Dictionary<string, object>()
                {
                    { "$ref", $"#/properties/{propFloat.Key}/definitions/val" }
                };

                var defSchema = new Dictionary<string, object>()
                {
                    { "val", new Dictionary<string, object>()
                    {
                        { "type", "number" },
                        { "minimum", Math.Round(att.MinValue, 6) },
                        { "maximum", Math.Round(att.MaxValue, 6) },
                        { "default", att.DefaultValue }
                    } }
                };

                var descriptionAtt = propFloat.Key.GetAttributeOfType<System.ComponentModel.DescriptionAttribute>();
                propertySchema.Add("description", descriptionAtt?.Description ?? "(no description)");

                propertySchema.Add("definitions", defSchema);

                var typeOneSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "value", directValueSchema },
                        { "probability", probabilitySchema },
                        { "compose", composeSchema },
                        { "locked", lockedSchema } } },
                    { "required", new List<string>() { "value" } },
                    { "additionalProperties", false }
                };

                var typeTwoSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "low", directValueSchema },
                        { "high", directValueSchema},
                        { "probability", probabilitySchema },
                        { "reroll", rerollSchema },
                        { "compose", composeSchema },
                        { "locked", lockedSchema } } },
                    { "required", new List<string>() { "low", "high" } },
                    { "additionalProperties", false }
                };

                propertySchema.Add("oneOf", new List<object>()
                {
                    directValueSchema,
                    new Dictionary<string, object>()
                    {
                        { "oneOf", new List<object>()
                        {
                            typeOneSchema,
                            typeTwoSchema
                        } }
                    }
                });
                propertyJsonSchema.Add(propFloat.Key.ToString(), propertySchema);
            }

            foreach (var propString in propertyDefinitionsString)
            {
                var att = propString.Value.PrimaryAttribute;
                var propertySchema = new Dictionary<string, object>();

                var directValueSchema = new Dictionary<string, object>()
                {
                    { "$ref", $"#/properties/{propString.Key}/definitions/val" }
                };

                var defSchema = new Dictionary<string, object>()
                {
                    { "val", new Dictionary<string, object>()
                    {
                        { "type", "string" },
                        { "default", att.DefaultValue }
                    } }
                };

                var descriptionAtt = propString.Key.GetAttributeOfType<System.ComponentModel.DescriptionAttribute>();
                propertySchema.Add("description", descriptionAtt?.Description ?? "(no description)");

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

            foreach (var propBool in propertyDefinitionsBool)
            {
                var att = propBool.Value.PrimaryAttribute;
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
                        { "default", att.DefaultValue }
                    } }
                };

                var descriptionAtt = propBool.Key.GetAttributeOfType<System.ComponentModel.DescriptionAttribute>();
                propertySchema.Add("description", descriptionAtt?.Description ?? "(no description)");

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
            */
            return propertyJsonSchema;
        }
    }
}
