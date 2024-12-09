namespace ACRealms;

// https://github.com/corvus-dotnet/corvus.jsonschema?tab=readme-ov-file#getting-started
// For old version (.net 8) - dotnet tool install --global Corvus.Json.JsonSchema.TypeGeneratorTool --version 3.1.1
// generatejsonschematypes ACRealms.RealmProps\PropDefs\json-schema\realm-property-schema.json
/* 
 generatejsonschematypes ACRealms.RealmProps\PropDefs\json-schema\realm-property-schema.json --rootNamespace ACRealms.Roslyn.RealmProps.IntermediateModels --outputPath=ACRealms.Roslyn.RealmProps\IntermediateModels 
*/

/*
 * For new version:
dotnet tool install --global Corvus.Json.JsonSchema.TypeGeneratorTool

 generatejsonschematypes ACRealms.RealmProps\PropDefs\json-schema\realm-property-schema.json --rootNamespace ACRealms.Roslyn.RealmProps.IntermediateModels --outputPath=ACRealms.Roslyn.RealmProps\IntermediateModels

Also try

 generatejsonschematypes ACRealms.RealmProps\PropDefs\json-schema\realm-property-schema.json --rootNamespace ACRealms.Roslyn.RealmProps.IntermediateModels --outputPath=ACRealms.Roslyn.RealmProps\IntermediateModels --useSchema Draft201909

*/
