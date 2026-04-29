using ACE.Entity.Enum.Properties;
using System.Collections.Frozen;
using static ACRealms.Roslyn.RealmProps.Builders.SerializationHelpers;

namespace ACRealms.Roslyn.RealmProps.Builders.Phase2Src
{
    internal static class WeeniePropSchema
    {
        internal enum Primitive
        {
            @int,
            @long,
            @float,
            @string,
            @bool
        }

        internal static readonly FrozenDictionary<string, Primitive> PropTypeMap = new Dictionary<string, Primitive>()
        {
            { "WeeniePropertyInt", Primitive.@int},
            { "WeeniePropertyInt64", Primitive.@long },
            { "WeeniePropertyFloat", Primitive.@float },
            { "WeeniePropertyBool", Primitive.@bool },
            { "WeeniePropertyString", Primitive.@string },
        }.ToFrozenDictionary();

        internal static readonly FrozenDictionary<Primitive, Type> PrimitiveTypeMap = new Dictionary<Primitive, Type>()
        {
            { Primitive.@int, typeof(PropertyInt) },
            { Primitive.@long, typeof(PropertyInt64) },
            { Primitive.@float, typeof(PropertyFloat) },
            { Primitive.@bool, typeof(PropertyBool) },
            { Primitive.@string, typeof(PropertyString) },
        }.ToFrozenDictionary();

        internal static string GenerateEntitySchemaSourceCode(string entityType)
        {
            if (!PropTypeMap.ContainsKey(entityType))
                throw new ArgumentException($"EntityType {entityType} not among WeeniePropSchema possible entity types allowed (this is a bug)");

            Primitive prim = PropTypeMap[entityType];
            if (!PrimitiveTypeMap.ContainsKey(prim))
                throw new NotImplementedException($"EntityType {entityType} maps to prim {prim} but no corresponding enum type found. This is a bug.");

            var enumType = PrimitiveTypeMap[prim];
            string[] propsForPrimitiveType = Enum.GetNames(PrimitiveTypeMap[prim]);

            List<string> schemaArray = [];
            List<string> schema = [];

            foreach(var prop in propsForPrimitiveType)
            {
                List<string> singlePropSchema = [];
                AddStringProp(singlePropSchema, "const", prop);
                schemaArray.Add(
                $$"""
                { {{SerializePropsUnwrapped(singlePropSchema)}} }
                """);
            }
            
            var schemas = SerializeArrayUnwrapped(schemaArray);
            // We must wrap in a comment as the source generator treats the output as a C# file
            return
            $$"""
                /*
                {
                  "$schema": "http://json-schema.org/draft-07/schema",
                  "type": "string",
                  "anyOf": [
                  {{schemas}}
                  ]
                }
                */
                """;
        }
    }
}
