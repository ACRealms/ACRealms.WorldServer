using ACE.Entity.Enum.Properties;
using System.Collections.Frozen;
using static ACRealms.Roslyn.RealmProps.Builders.SerializationHelpers;

namespace ACRealms.Roslyn.RealmProps.Builders.Phase2Src
{
    internal static class EntitySchema
    {
        internal enum Primitive
        {
            @int,
            @long,
            @double,
            @string,
            @bool
        }

        internal static readonly FrozenDictionary<Primitive, Type> PrimitiveTypeMap = new Dictionary<Primitive, Type>()
        {
            { Primitive.@int, typeof(PropertyInt) },
        }.ToFrozenDictionary();

        internal static string GenerateEntitySchemaSourceCode(string entityType)
        {
            Dictionary<Primitive, string[]> propsForPrimitiveType = [];

            foreach(var prim in PrimitiveTypeMap.Keys)
                propsForPrimitiveType[prim] = Enum.GetNames(PrimitiveTypeMap[prim]);

            List<string> schemaArray = new List<string>();

            foreach(var prim in PrimitiveTypeMap.Keys)
            {
                List<string> schema = [];

                List<string> propertiesSchema = [];
                foreach(var prop in propsForPrimitiveType[prim])
                {
                    List<string> singlePropSchema = [];
                    AddStringProp(singlePropSchema, "$ref", $"#{prim}");
                    AddUnwrappedObjectProp(propertiesSchema, prop, SerializePropsUnwrapped(singlePropSchema));
                }
                AddUnwrappedObjectProp(schema, "properties", SerializePropsUnwrapped(propertiesSchema));

                schemaArray.Add(
                $$"""
                {
                {{SerializePropsUnwrapped(schema)}}
                }
                """);
            }
            var schemas = SerializeArrayUnwrapped(schemaArray);
            // We must wrap in a comment as the source generator treats the output as a C# file
            return
            $$"""
                /*
                {
                  "$schema": "http://json-schema.org/draft-07/schema",
                  "type": "object",
                  "definitions": {
                    "base": { "$ref": "../../scope-base.json#" },
                    "int": { "$anchor": "int", "$ref": "../../scope-base.json#/definitions/paramsInt" }
                  },
                  "anyOf": [
                  {{schemas}}
                  ],
                  "minItems": 1,
                  "unevaluatedProperties": false
                }
                */
                """;
        }
    }
}
