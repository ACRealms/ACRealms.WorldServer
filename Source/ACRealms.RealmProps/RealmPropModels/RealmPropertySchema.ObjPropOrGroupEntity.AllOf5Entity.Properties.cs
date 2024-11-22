//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Corvus.Json;
using Corvus.Json.Internal;

namespace ACRealms.RealmProps.IntermediateModels;
public readonly partial struct RealmPropertySchema
{
    public readonly partial struct ObjPropOrGroupEntity
    {
        /// <summary>
        /// Generated from JSON Schema.
        /// </summary>
        public readonly partial struct AllOf5Entity
        {
            /// <summary>
            /// The well-known property names in the JSON object.
            /// </summary>
            public static class JsonPropertyNames
            {
                /// <summary>
                /// JSON property name for <see cref = "Default"/>.
                /// </summary>
                public static ReadOnlySpan<byte> DefaultUtf8 => "default"u8;

                /// <summary>
                /// JSON property name for <see cref = "Default"/>.
                /// </summary>
                public const string Default = "default";
            }

            /// <summary>
            /// Gets the (optional) <c>default</c> property.
            /// </summary>
            public ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValBoolEntity Default
            {
                get
                {
                    if ((this.backing & Backing.JsonElement) != 0)
                    {
                        if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
                        {
                            return default;
                        }

                        if (this.jsonElementBacking.TryGetProperty(JsonPropertyNames.DefaultUtf8, out JsonElement result))
                        {
                            return new ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValBoolEntity(result);
                        }
                    }

                    if ((this.backing & Backing.Object) != 0)
                    {
                        if (this.objectBacking.TryGetValue(JsonPropertyNames.Default, out JsonAny result))
                        {
                            return result.As<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValBoolEntity>();
                        }
                    }

                    return default;
                }
            }

            /// <summary>
            /// Creates an instance of a <see cref = "AllOf5Entity"/>.
            /// </summary>
            public static AllOf5Entity Create(ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValBoolEntity? @default = null)
            {
                var builder = ImmutableList.CreateBuilder<JsonObjectProperty>();
                if (@default is ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValBoolEntity @default__)
                {
                    builder.Add(JsonPropertyNames.Default, @default__.AsAny);
                }

                return new(builder.ToImmutable());
            }

            /// <summary>
            /// Sets default.
            /// </summary>
            /// <param name = "value">The value to set.</param>
            /// <returns>The entity with the updated property.</returns>
            public AllOf5Entity WithDefault(in ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValBoolEntity value)
            {
                return this.SetProperty(JsonPropertyNames.Default, value);
            }

            /// <summary>
            /// Tries to get the validator for the given property.
            /// </summary>
            /// <param name = "property">The property for which to get the validator.</param>
            /// <param name = "hasJsonElementBacking"><c>True</c> if the object containing the property has a JsonElement backing.</param>
            /// <param name = "propertyValidator">The validator for the property, if provided by this schema.</param>
            /// <returns><c>True</c> if the validator was found.</returns>
            private bool __TryGetCorvusLocalPropertiesValidator(in JsonObjectProperty property, bool hasJsonElementBacking, [NotNullWhen(true)] out ObjectPropertyValidator? propertyValidator)
            {
                if (hasJsonElementBacking)
                {
                }
                else
                {
                }

                propertyValidator = null;
                return false;
            }
        }
    }
}