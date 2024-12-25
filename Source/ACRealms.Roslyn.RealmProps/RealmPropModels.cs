namespace ACRealms;

// https://github.com/corvus-dotnet/corvus.jsonschema?tab=readme-ov-file#getting-started

/* If editing the schemas in PropDefs/json-schema (but not the prop defs themselves), it is necessary to regenerate the types manually after editing. 
 * Typically you won't need to touch these files unless contributing to the ACRealms compiler itself.
 
dotnet tool install --global Corvus.Json.JsonSchema.TypeGeneratorTool

cd .\Source\ACRealms.RealmProps\PropDefs\json-schema
generatejsonschematypes root.json --rootNamespace ACRealms.Roslyn.RealmProps.IntermediateModels --outputPath=..\..\..\ACRealms.Roslyn.RealmProps\IntermediateModels --disableNamingHeuristic DocumentationNameHeuristic --useImplicitOperatorString true
*/
