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
        public readonly partial struct AllOf3Entity
        {
            /// <summary>
            /// Generated from JSON Schema.
            /// </summary>
            public readonly partial struct ThenEntity
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
                    /// <summary>
                    /// JSON property name for <see cref = "MaxValue"/>.
                    /// </summary>
                    public static ReadOnlySpan<byte> MaxValueUtf8 => "max_value"u8;

                    /// <summary>
                    /// JSON property name for <see cref = "MaxValue"/>.
                    /// </summary>
                    public const string MaxValue = "max_value";
                    /// <summary>
                    /// JSON property name for <see cref = "MinValue"/>.
                    /// </summary>
                    public static ReadOnlySpan<byte> MinValueUtf8 => "min_value"u8;

                    /// <summary>
                    /// JSON property name for <see cref = "MinValue"/>.
                    /// </summary>
                    public const string MinValue = "min_value";
                }

                /// <summary>
                /// Gets the (optional) <c>default</c> property.
                /// </summary>
                public ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity Default
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
                                return new ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity(result);
                            }
                        }

                        if ((this.backing & Backing.Object) != 0)
                        {
                            if (this.objectBacking.TryGetValue(JsonPropertyNames.Default, out JsonAny result))
                            {
                                return result.As<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity>();
                            }
                        }

                        return default;
                    }
                }

                /// <summary>
                /// Gets the (optional) <c>max_value</c> property.
                /// </summary>
                public ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity MaxValue
                {
                    get
                    {
                        if ((this.backing & Backing.JsonElement) != 0)
                        {
                            if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
                            {
                                return default;
                            }

                            if (this.jsonElementBacking.TryGetProperty(JsonPropertyNames.MaxValueUtf8, out JsonElement result))
                            {
                                return new ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity(result);
                            }
                        }

                        if ((this.backing & Backing.Object) != 0)
                        {
                            if (this.objectBacking.TryGetValue(JsonPropertyNames.MaxValue, out JsonAny result))
                            {
                                return result.As<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity>();
                            }
                        }

                        return default;
                    }
                }

                /// <summary>
                /// Gets the (optional) <c>min_value</c> property.
                /// </summary>
                public ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity MinValue
                {
                    get
                    {
                        if ((this.backing & Backing.JsonElement) != 0)
                        {
                            if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
                            {
                                return default;
                            }

                            if (this.jsonElementBacking.TryGetProperty(JsonPropertyNames.MinValueUtf8, out JsonElement result))
                            {
                                return new ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity(result);
                            }
                        }

                        if ((this.backing & Backing.Object) != 0)
                        {
                            if (this.objectBacking.TryGetValue(JsonPropertyNames.MinValue, out JsonAny result))
                            {
                                return result.As<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity>();
                            }
                        }

                        return default;
                    }
                }

                /// <summary>
                /// Creates an instance of a <see cref = "ThenEntity"/>.
                /// </summary>
                public static ThenEntity Create(ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity? @default = null, ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity? maxValue = null, ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity? minValue = null)
                {
                    var builder = ImmutableList.CreateBuilder<JsonObjectProperty>();
                    if (@default is ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity @default__)
                    {
                        builder.Add(JsonPropertyNames.Default, @default__.AsAny);
                    }

                    if (maxValue is ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity maxValue__)
                    {
                        builder.Add(JsonPropertyNames.MaxValue, maxValue__.AsAny);
                    }

                    if (minValue is ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity minValue__)
                    {
                        builder.Add(JsonPropertyNames.MinValue, minValue__.AsAny);
                    }

                    return new(builder.ToImmutable());
                }

                /// <summary>
                /// Sets default.
                /// </summary>
                /// <param name = "value">The value to set.</param>
                /// <returns>The entity with the updated property.</returns>
                public ThenEntity WithDefault(in ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity value)
                {
                    return this.SetProperty(JsonPropertyNames.Default, value);
                }

                /// <summary>
                /// Sets max_value.
                /// </summary>
                /// <param name = "value">The value to set.</param>
                /// <returns>The entity with the updated property.</returns>
                public ThenEntity WithMaxValue(in ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity value)
                {
                    return this.SetProperty(JsonPropertyNames.MaxValue, value);
                }

                /// <summary>
                /// Sets min_value.
                /// </summary>
                /// <param name = "value">The value to set.</param>
                /// <returns>The entity with the updated property.</returns>
                public ThenEntity WithMinValue(in ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity value)
                {
                    return this.SetProperty(JsonPropertyNames.MinValue, value);
                }

                private static ValidationContext __CorvusValidateDefault(in JsonObjectProperty property, in ValidationContext validationContext, ValidationLevel level)
                {
                    return property.ValueAs<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity>().Validate(validationContext, level);
                }

                private static ValidationContext __CorvusValidateMinValue(in JsonObjectProperty property, in ValidationContext validationContext, ValidationLevel level)
                {
                    return property.ValueAs<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity>().Validate(validationContext, level);
                }

                private static ValidationContext __CorvusValidateMaxValue(in JsonObjectProperty property, in ValidationContext validationContext, ValidationLevel level)
                {
                    return property.ValueAs<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValLongEntity>().Validate(validationContext, level);
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
                        if (property.NameEquals(JsonPropertyNames.DefaultUtf8))
                        {
                            propertyValidator = __CorvusValidateDefault;
                            return true;
                        }
                        else if (property.NameEquals(JsonPropertyNames.MinValueUtf8))
                        {
                            propertyValidator = __CorvusValidateMinValue;
                            return true;
                        }
                        else if (property.NameEquals(JsonPropertyNames.MaxValueUtf8))
                        {
                            propertyValidator = __CorvusValidateMaxValue;
                            return true;
                        }
                    }
                    else
                    {
                        if (property.NameEquals(JsonPropertyNames.Default))
                        {
                            propertyValidator = __CorvusValidateDefault;
                            return true;
                        }
                        else if (property.NameEquals(JsonPropertyNames.MinValue))
                        {
                            propertyValidator = __CorvusValidateMinValue;
                            return true;
                        }
                        else if (property.NameEquals(JsonPropertyNames.MaxValue))
                        {
                            propertyValidator = __CorvusValidateMaxValue;
                            return true;
                        }
                    }

                    propertyValidator = null;
                    return false;
                }
            }
        }
    }
}