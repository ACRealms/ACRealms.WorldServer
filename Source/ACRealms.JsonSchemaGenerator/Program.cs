using System.CommandLine;
using Newtonsoft.Json;

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
            var realmFiles = realmDir.GetFiles()?.Where(f => f.Name.EndsWith(".jsonc"))?.ToList() ?? throw new ArgumentException("Could not find realm files");
            var realmNames = realmFiles.Select(f =>
            {
                try
                {
                    var dobj = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(f.FullName));
                    return (name: (string?)dobj!.name.Value, description: (string?)(dobj!.properties?.Description?.Value ?? "This realm is missing a 'Description' property. You can add one!"));
                }
                catch (Exception)
                {
                    return (name: null, description: null);
                }
            }).Where(data => data.name != null).Select(d => (name: d.name!, description: d.description!)).ToList();

            var rulesetDir = new DirectoryInfo($"{directory}/json/realms/ruleset");
            var rulesetFiles = rulesetDir.GetFiles().Where(f => f.Name.EndsWith(".jsonc"))?.ToList() ?? new List<FileInfo>();
            var rulesetNames = rulesetFiles.Select(f =>
            {
                try
                {
                    var dobj = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(f.FullName));
                    return (name: (string?)dobj!.name.Value, description: (string?)(dobj!.properties?.Description?.Value ?? "This ruleset is missing a 'Description' property. You can add one!"));
                }
                catch (Exception)
                {
                    return (name: null, description: null);
                }
            }).Where(data => data.name != null).Select(d => (name: d.name!, description: d.description!)).ToList();

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

            if (!Directory.Exists(generatedPath))
                Directory.CreateDirectory(generatedPath);

            File.WriteAllText($"{generatedPath}/realm-names.json", JsonConvert.SerializeObject(realmNamesSchema, Formatting.Indented));
            File.WriteAllText($"{generatedPath}/ruleset-names.json", JsonConvert.SerializeObject(rulesetNamesSchema, Formatting.Indented));
            File.WriteAllText($"{generatedPath}/apply-rulesets-random.json", JsonConvert.SerializeObject(applyRulesetsRandomSchema, Formatting.Indented));
        }
    }
}
