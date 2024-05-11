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
                return (string)dobj!.name.Value;
            }).ToList();

            var rulesetDir = new DirectoryInfo($"{directory}/json/realms/ruleset");
            var rulesetFiles = rulesetDir.GetFiles();
            var rulesetNames = rulesetFiles.Select(f =>
            {
                var dobj = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(f.FullName));
                return (string)dobj!.name.Value;
            }).ToList();

            var generatedPath = $"{directory}/json-schema/generated";
            if (Directory.Exists(generatedPath))
                Directory.CreateDirectory(generatedPath);

            var realmNamesSchema = new Dictionary<string, dynamic>()
            {
                { "enum", realmNames }
            };

            var rulesetNamesSchema = new Dictionary<string, dynamic>()
            {
                { "enum", rulesetNames }
            };

            File.WriteAllText($"{generatedPath}/realm-names.json", JsonConvert.SerializeObject(realmNamesSchema, Formatting.Indented));
            File.WriteAllText($"{generatedPath}/ruleset-names.json", JsonConvert.SerializeObject(rulesetNamesSchema, Formatting.Indented));

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
                            { "enum", new List<string>() { "landblock", "always", "never" } }
                        }
                    },
                    {   "compose", new Dictionary<string, object>() {
                            { "description", "add, multiply, or replace the previously composed ruleset property" },
                            { "enum", new List<string>() { "add", "multiply", "replace" } } }
                    },
                    {   "locked", new Dictionary<string, object>() {
                            { "type", "boolean" },
                            { "description", "If true, the value may not be further modified by other rulesets or sub-realms" } }
                    } }
                },
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
                var lowSchema = new Dictionary<string, object>()
                {
                    { "low", directValueSchema }
                };
                var highSchema = new Dictionary<string, object>()
                {
                    { "high", directValueSchema }
                };

                var defSchema = new Dictionary<string, object>()
                {
                    { "val", new Dictionary<string, object>()
                    {
                        { "type", "integer" },
                        { "minimum", att.MinValue },
                        { "maximum", att.MaxValue }
                    } }
                };

                var descriptionAtt = propInt.Key.GetAttributeOfType<System.ComponentModel.DescriptionAttribute>();
                if (descriptionAtt != null)
                    propertySchema.Add("description", descriptionAtt.Description);

                propertySchema.Add("definitions", defSchema);

                var typeOneSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "value", directValueSchema },
                        { "compose", composeSchema },
                        { "locked", lockedSchema } } },
                    { "required", new List<string>() { "value" } },
                    { "additionalProperties", false }
                };

                var typeTwoSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "low", lowSchema },
                        { "high", highSchema},
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
                var lowSchema = new Dictionary<string, object>()
                {
                    { "low", directValueSchema }
                };
                var highSchema = new Dictionary<string, object>()
                {
                    { "high", directValueSchema }
                };

                var defSchema = new Dictionary<string, object>()
                {
                    { "val", new Dictionary<string, object>()
                    {
                        { "type", "integer" },
                        { "minimum", att.MinValue },
                        { "maximum", att.MaxValue }
                    } }
                };

                var descriptionAtt = propLong.Key.GetAttributeOfType<System.ComponentModel.DescriptionAttribute>();
                if (descriptionAtt != null)
                    propertySchema.Add("description", descriptionAtt.Description);

                propertySchema.Add("definitions", defSchema);

                var typeOneSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "value", directValueSchema },
                        { "compose", composeSchema },
                        { "locked", lockedSchema } } },
                    { "required", new List<string>() { "value" } },
                    { "additionalProperties", false }
                };

                var typeTwoSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "low", lowSchema },
                        { "high", highSchema},
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
                var lowSchema = new Dictionary<string, object>()
                {
                    { "low", directValueSchema }
                };
                var highSchema = new Dictionary<string, object>()
                {
                    { "high", directValueSchema }
                };

                var defSchema = new Dictionary<string, object>()
                {
                    { "val", new Dictionary<string, object>()
                    {
                        { "type", "number" },
                        { "minimum", Math.Round(att.MinValue, 6) },
                        { "maximum", Math.Round(att.MaxValue, 6) }
                    } }
                };

                var descriptionAtt = propFloat.Key.GetAttributeOfType<System.ComponentModel.DescriptionAttribute>();
                if (descriptionAtt != null)
                    propertySchema.Add("description", descriptionAtt.Description);

                propertySchema.Add("definitions", defSchema);

                var typeOneSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "value", directValueSchema },
                        { "compose", composeSchema },
                        { "locked", lockedSchema } } },
                    { "required", new List<string>() { "value" } },
                    { "additionalProperties", false }
                };

                var typeTwoSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "low", lowSchema },
                        { "high", highSchema},
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
                //var att = propString.Value;
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
                    } }
                };

                var descriptionAtt = propString.Key.GetAttributeOfType<System.ComponentModel.DescriptionAttribute>();
                if (descriptionAtt != null)
                    propertySchema.Add("description", descriptionAtt.Description);

                propertySchema.Add("definitions", defSchema);

                var typeOneSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "value", directValueSchema },
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
                // var att = propString.Value;
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
                    } }
                };

                var descriptionAtt = propBool.Key.GetAttributeOfType<System.ComponentModel.DescriptionAttribute>();
                if (descriptionAtt != null)
                    propertySchema.Add("description", descriptionAtt.Description);

                propertySchema.Add("definitions", defSchema);

                var typeOneSchema = new Dictionary<string, object>()
                {
                    { "type", "object" },
                    { "properties", new Dictionary<string, object>() {
                        { "value", directValueSchema },
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
