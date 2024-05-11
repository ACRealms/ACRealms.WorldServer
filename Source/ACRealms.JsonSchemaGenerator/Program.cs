using ACE.Entity.Enum.Properties;
using System.CommandLine;
using Newtonsoft.Json;
using ACE.Entity.Enum;

namespace ACRealms.JsonSchemaGenerator
{
    public static class Paths
    {
        public static string SolutionPath { get; } = new Func<string>(() =>
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (!directory!.GetFiles("*.sln").Any())
                directory = directory.Parent;
            return directory.FullName;
        })();

        public static string DefaultContentPath { get; } = $"{SolutionPath}/../Content";
        public static string DefaultJsonPath { get; } = $"{DefaultContentPath}/json";
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var fileOption = new Option<DirectoryInfo?>(name: "--content", description: "The path to the content folder",
                getDefaultValue: () => new DirectoryInfo(Paths.DefaultContentPath));

            var rootCommand = new RootCommand { fileOption };
            rootCommand.SetHandler(Run!, fileOption);
            await rootCommand.InvokeAsync(args);
        }

        public static void Run(DirectoryInfo directory)
        {
            var realmDir = new DirectoryInfo($"{directory}/json/realms/realm");
            var realmFiles = realmDir.GetFiles() ?? throw new ArgumentException("Could not find files");
            var realmNames = realmFiles.Select(f =>
            {
                var dobj = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(f.FullName));
                return (name: (string)dobj!.name.Value, description: (string?)dobj!.properties?.Description?.Value ?? "This realm is missing a 'Description' property. You can add one!");
            }).ToList();

            var rulesetDir = new DirectoryInfo($"{directory}/json/realms/ruleset");
            var rulesetFiles = rulesetDir.GetFiles();
            var rulesetNames = rulesetFiles.Select(f =>
            {
                var dobj = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(f.FullName));
                return (name: (string)dobj!.name.Value, description: (string?)dobj!.properties?.Description?.Value ?? "This ruleset is missing a 'Description' property. You can add one!");
            }).ToList();

            var generatedPath = $"{directory}/json-schema/generated";
            if (Directory.Exists(generatedPath))
                Directory.CreateDirectory(generatedPath);

            var realmNamesSchemaConsts = new List<object>();
            foreach(var realmInfo in realmNames)
            {
                realmNamesSchemaConsts.Add(new Dictionary<string, object>()
                {
                    { "type", "string" },
                    { "description", realmInfo.description },
                    { "const", realmInfo.name }
                });
            }

            var realmNamesSchema = new Dictionary<string, object>()
            {
                { "$schema", "http://json-schema.org/draft-07/schema" },
                { "$id", "https://realm.ac/schema/v1/generated/realm-names.json" },
                { "oneOf", realmNamesSchemaConsts }
            };

            var rulesetNamesSchemaConsts = new List<object>();
            foreach (var rulesetInfo in rulesetNames)
            {
                rulesetNamesSchemaConsts.Add(new Dictionary<string, object>()
                {
                    { "type", "string" },
                    { "description", rulesetInfo.description },
                    { "const", rulesetInfo.name }
                });
            }

            var rulesetNamesSchema = new Dictionary<string, object>()
            {
                { "$schema", "http://json-schema.org/draft-07/schema" },
                { "$id", "https://realm.ac/schema/v1/generated/ruleset-names.json" },
                { "oneOf", rulesetNamesSchemaConsts }
            };

            var applyRulesetsRandomSchemaProperties = new Dictionary<string, object>();
            foreach(var rulesetInfo in rulesetNames)
            {
                applyRulesetsRandomSchemaProperties.Add(rulesetInfo.name, new Dictionary<string, object>()
                {
                    { "description", rulesetInfo.description },
                    { "$ref", "#/definitions/probabilityValue" }
                });
            }

            var applyRulesetsRandomSchema = new Dictionary<string, object>()
            {
                {   "$schema", "http://json-schema.org/draft-07/schema" },
                {   "$id", "https://realm.ac/schema/v1/generated/apply-rulesets-random.json" },
                {   "type", "object" },
                {   "definitions", new Dictionary<string, object>() {
                    {   "probabilityValue", new Dictionary<string, object>() {
                        {   "default", "auto" },
                        {   "oneOf", new List<object>() {
                                new Dictionary<string, object>() {
                                    { "type", "number" },
                                    { "minimum", 0 },
                                    { "maximum", 1 } },
                                new Dictionary<string, object>() {
                                    { "const", "auto" } } } } } } } },
                {   "properties", applyRulesetsRandomSchemaProperties }
            };


            File.WriteAllText($"{generatedPath}/realm-names.json", JsonConvert.SerializeObject(realmNamesSchema, Formatting.Indented));
            File.WriteAllText($"{generatedPath}/ruleset-names.json", JsonConvert.SerializeObject(rulesetNamesSchema, Formatting.Indented));
            File.WriteAllText($"{generatedPath}/apply-rulesets-random.json", JsonConvert.SerializeObject(applyRulesetsRandomSchema, Formatting.Indented));

            var properties = MakePropertySchema();
            var realmPropertiesSchema = new Dictionary<string, object>()
            {
                {   "$schema", "http://json-schema.org/draft-07/schema" },
                {   "$id", "https://realm.ac/schema/v1/generated/realm-properties.json" },
                {   "type", "object" },
                {   "minItems", 1 },
                {   "definitions", new Dictionary<string, object>() {
                    {   "reroll", new Dictionary<string, object>() {
                            { "description", "landblock: Reroll once during landblock load; always: Reroll each time the property is accessed by game logic; Never: use the default value" },
                            { "default", "landblock" },
                            { "enum", new List<string>() { "landblock", "always", "never" } } } },
                    {   "probability", new Dictionary<string, object>() {
                            { "type", "number" },
                            { "description", "The probability of this property taking effect or being composed" },
                            { "default", 1 },
                            { "minimum", 0 },
                            { "maximum", 1 } } },
                    {   "compose", new Dictionary<string, object>() {
                            { "description", "add, multiply, or replace the previously composed ruleset property" },
                            { "default", "replace" },
                            { "enum", new List<string>() { "add", "multiply", "replace" } } } },
                    {   "locked", new Dictionary<string, object>() {
                            { "type", "boolean" },
                            { "description", "If true, the value may not be further modified by other rulesets or sub-realms" },
                            { "default", false } } } } },
                {   "properties", properties },
                {   "additionalProperties", false }
            };
            File.WriteAllText($"{generatedPath}/realm-properties.json", JsonConvert.SerializeObject(realmPropertiesSchema, Formatting.Indented));
        }

        private static object MakePropertySchema()
        {
            var propertyDefinitionsBool = RealmPropertyHelper.MakePropDict<RealmPropertyBool, RealmPropertyBoolAttribute>().Where(x => x.Key != RealmPropertyBool.Undef);
            var propertyDefinitionsInt = RealmPropertyHelper.MakePropDict<RealmPropertyInt, RealmPropertyIntAttribute>().Where(x => x.Key != RealmPropertyInt.Undef);
            var propertyDefinitionsInt64 = RealmPropertyHelper.MakePropDict<RealmPropertyInt64, RealmPropertyInt64Attribute>().Where(x => x.Key != RealmPropertyInt64.Undef);
            var propertyDefinitionsString = RealmPropertyHelper.MakePropDict<RealmPropertyString, RealmPropertyStringAttribute>().Where(x => x.Key != RealmPropertyString.Undef);
            var propertyDefinitionsFloat = RealmPropertyHelper.MakePropDict<RealmPropertyFloat, RealmPropertyFloatAttribute>().Where(x => x.Key != RealmPropertyFloat.Undef);

            var probabilitySchema = new Dictionary<string, object>()
            {
                { "$ref", "#/definitions/probability" }
            };

            var composeSchema = new Dictionary<string, object>()
            {
                { "$ref", "#/definitions/compose" }
            };
            var lockedSchema = new Dictionary<string, object>()
            {
                { "$ref", "#/definitions/locked" }
            };
            var rerollSchema = new Dictionary<string, object>()
            {
                { "$ref", "#/definitions/reroll" }
            };

            var propertyJsonSchema = new Dictionary<string, object>();

            foreach (var propInt in propertyDefinitionsInt)
            {
                var att = propInt.Value;
                var propertySchema = new Dictionary<string, object>();

                var directValueSchema = new Dictionary<string, object>()
                {
                    { "$ref", $"#/properties/{propInt.Key}/definitions/val" }
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

                var descriptionAtt = propInt.Key.GetAttributeOfType<System.ComponentModel.DescriptionAttribute>();
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
                propertyJsonSchema.Add(propInt.Key.ToString(), propertySchema);
            }

            foreach (var propLong in propertyDefinitionsInt64)
            {
                var att = propLong.Value;
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
                var att = propFloat.Value;
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
                var att = propString.Value;
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
                var att = propBool.Value;
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

            return propertyJsonSchema;
        }
    }
}
