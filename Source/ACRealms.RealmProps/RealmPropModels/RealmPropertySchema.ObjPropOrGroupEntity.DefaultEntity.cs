//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System.Buffers;
using System.ComponentModel;
using System.Collections.Immutable;
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
        /// <remarks>
        /// <para>
        /// The hard default value for the realm property (ultimate fallback)
        /// </para>
        /// </remarks>
        [System.Text.Json.Serialization.JsonConverter(typeof(Corvus.Json.Internal.JsonValueConverter<DefaultEntity>))]
        public readonly partial struct DefaultEntity
        {
            private readonly Backing backing;
            private readonly JsonElement jsonElementBacking;
            private readonly string stringBacking;
            private readonly bool boolBacking;
            private readonly BinaryJsonNumber numberBacking;
            private readonly ImmutableList<JsonAny> arrayBacking;
            private readonly ImmutableList<JsonObjectProperty> objectBacking;
            /// <summary>
            /// Initializes a new instance of the <see cref = "DefaultEntity"/> struct.
            /// </summary>
            public DefaultEntity()
            {
                this.jsonElementBacking = default;
                this.backing = Backing.JsonElement;
                this.stringBacking = string.Empty;
                this.boolBacking = default;
                this.numberBacking = default;
                this.arrayBacking = ImmutableList<JsonAny>.Empty;
                this.objectBacking = ImmutableList<JsonObjectProperty>.Empty;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref = "DefaultEntity"/> struct.
            /// </summary>
            /// <param name = "value">The value from which to construct the instance.</param>
            public DefaultEntity(in JsonElement value)
            {
                this.jsonElementBacking = value;
                this.backing = Backing.JsonElement;
                this.stringBacking = string.Empty;
                this.boolBacking = default;
                this.numberBacking = default;
                this.arrayBacking = ImmutableList<JsonAny>.Empty;
                this.objectBacking = ImmutableList<JsonObjectProperty>.Empty;
            }

            /// <summary>
            /// Gets the schema location from which this type was generated.
            /// </summary>
            public static string SchemaLocation { get; } = "https://realm.ac/schema/v1/realm-property-schema.json#/definitions/objPropOrGroup/properties/default";
            /// <summary>
            /// Gets a Null instance.
            /// </summary>
            public static DefaultEntity Null { get; } = new(JsonValueHelpers.NullElement);
            /// <summary>
            /// Gets an Undefined instance.
            /// </summary>
            public static DefaultEntity Undefined { get; }
            /// <summary>
            /// Gets the default instance of the type.
            /// </summary>
            public static DefaultEntity DefaultInstance { get; }

            /// <inheritdoc/>
            public JsonAny AsAny
            {
                get
                {
                    if ((this.backing & Backing.JsonElement) != 0)
                    {
                        return new(this.jsonElementBacking);
                    }

                    if ((this.backing & Backing.String) != 0)
                    {
                        return new(this.stringBacking);
                    }

                    if ((this.backing & Backing.Bool) != 0)
                    {
                        return new(this.boolBacking);
                    }

                    if ((this.backing & Backing.Number) != 0)
                    {
                        return new(this.numberBacking);
                    }

                    if ((this.backing & Backing.Array) != 0)
                    {
                        return new(this.arrayBacking);
                    }

                    if ((this.backing & Backing.Object) != 0)
                    {
                        return new(this.objectBacking);
                    }

                    if ((this.backing & Backing.Null) != 0)
                    {
                        return JsonAny.Null;
                    }

                    return JsonAny.Undefined;
                }
            }

            /// <inheritdoc/>
            public JsonElement AsJsonElement
            {
                get
                {
                    if ((this.backing & Backing.JsonElement) != 0)
                    {
                        return this.jsonElementBacking;
                    }

                    if ((this.backing & Backing.String) != 0)
                    {
                        return JsonValueHelpers.StringToJsonElement(this.stringBacking);
                    }

                    if ((this.backing & Backing.Bool) != 0)
                    {
                        return JsonValueHelpers.BoolToJsonElement(this.boolBacking);
                    }

                    if ((this.backing & Backing.Number) != 0)
                    {
                        return JsonValueHelpers.NumberToJsonElement(this.numberBacking);
                    }

                    if ((this.backing & Backing.Array) != 0)
                    {
                        return JsonValueHelpers.ArrayToJsonElement(this.arrayBacking);
                    }

                    if ((this.backing & Backing.Object) != 0)
                    {
                        return JsonValueHelpers.ObjectToJsonElement(this.objectBacking);
                    }

                    if ((this.backing & Backing.Null) != 0)
                    {
                        return JsonValueHelpers.NullElement;
                    }

                    return default;
                }
            }

            /// <inheritdoc/>
            JsonString IJsonValue.AsString
            {
                get
                {
                    if ((this.backing & Backing.JsonElement) != 0)
                    {
                        return new(this.jsonElementBacking);
                    }

                    if ((this.backing & Backing.String) != 0)
                    {
                        return new(this.stringBacking);
                    }

                    throw new InvalidOperationException();
                }
            }

            /// <inheritdoc/>
            JsonBoolean IJsonValue.AsBoolean
            {
                get
                {
                    if ((this.backing & Backing.JsonElement) != 0)
                    {
                        return new(this.jsonElementBacking);
                    }

                    if ((this.backing & Backing.Bool) != 0)
                    {
                        return new(this.boolBacking);
                    }

                    throw new InvalidOperationException();
                }
            }

            /// <inheritdoc/>
            JsonNumber IJsonValue.AsNumber
            {
                get
                {
                    if ((this.backing & Backing.JsonElement) != 0)
                    {
                        return new(this.jsonElementBacking);
                    }

                    if ((this.backing & Backing.Number) != 0)
                    {
                        return new(this.numberBacking);
                    }

                    throw new InvalidOperationException();
                }
            }

            /// <inheritdoc/>
            JsonObject IJsonValue.AsObject
            {
                get
                {
                    if ((this.backing & Backing.JsonElement) != 0)
                    {
                        return new(this.jsonElementBacking);
                    }

                    if ((this.backing & Backing.Object) != 0)
                    {
                        return new(this.objectBacking);
                    }

                    throw new InvalidOperationException();
                }
            }

            /// <inheritdoc/>
            JsonArray IJsonValue.AsArray
            {
                get
                {
                    if ((this.backing & Backing.JsonElement) != 0)
                    {
                        return new(this.jsonElementBacking);
                    }

                    if ((this.backing & Backing.Array) != 0)
                    {
                        return new(this.arrayBacking);
                    }

                    throw new InvalidOperationException();
                }
            }

            /// <inheritdoc/>
            public bool HasJsonElementBacking
            {
                get
                {
                    return (this.backing & Backing.JsonElement) != 0;
                }
            }

            /// <inheritdoc/>
            public bool HasDotnetBacking
            {
                get
                {
                    return (this.backing & Backing.Dotnet) != 0;
                }
            }

            /// <inheritdoc/>
            public JsonValueKind ValueKind
            {
                get
                {
                    if ((this.backing & Backing.JsonElement) != 0)
                    {
                        return this.jsonElementBacking.ValueKind;
                    }

                    if ((this.backing & Backing.String) != 0)
                    {
                        return JsonValueKind.String;
                    }

                    if ((this.backing & Backing.Bool) != 0)
                    {
                        return this.boolBacking ? JsonValueKind.True : JsonValueKind.False;
                    }

                    if ((this.backing & Backing.Number) != 0)
                    {
                        return JsonValueKind.Number;
                    }

                    if ((this.backing & Backing.Array) != 0)
                    {
                        return JsonValueKind.Array;
                    }

                    if ((this.backing & Backing.Object) != 0)
                    {
                        return JsonValueKind.Object;
                    }

                    if ((this.backing & Backing.Null) != 0)
                    {
                        return JsonValueKind.Null;
                    }

                    return JsonValueKind.Undefined;
                }
            }

            /// <summary>
            /// Conversion from JsonAny.
            /// </summary>
            /// <param name = "value">The value from which to convert.</param>
            /// <exception cref = "InvalidOperationException">The value was not compatible with this type.</exception>
            public static implicit operator DefaultEntity(in JsonAny value)
            {
                return value.As<DefaultEntity>();
            }

            /// <summary>
            /// Conversion to JsonAny.
            /// </summary>
            /// <param name = "value">The value from which to convert.</param>
            /// <exception cref = "InvalidOperationException">The value was not compatible with this type.</exception>
            public static implicit operator JsonAny(in DefaultEntity value)
            {
                return value.AsAny;
            }

            /// <summary>
            /// Equality operator.
            /// </summary>
            /// <param name = "left">The lhs.</param>
            /// <param name = "right">The rhs.</param>
            /// <returns><c>True</c> if the values are equal.</returns>
            public static bool operator ==(in DefaultEntity left, in DefaultEntity right)
            {
                return left.Equals(right);
            }

            /// <summary>
            /// Inequality operator.
            /// </summary>
            /// <param name = "left">The lhs.</param>
            /// <param name = "right">The rhs.</param>
            /// <returns><c>True</c> if the values are equal.</returns>
            public static bool operator !=(in DefaultEntity left, in DefaultEntity right)
            {
                return !left.Equals(right);
            }

            /// <summary>
            /// Gets an instance of the JSON value from a JsonAny value.
            /// </summary>
            /// <param name = "value">The <see cref = "JsonAny"/> value from which to instantiate the instance.</param>
            /// <returns>An instance of this type, initialized from the <see cref = "JsonAny"/>.</returns>
            /// <remarks>The returned value will have a <see cref = "IJsonValue.ValueKind"/> of <see cref = "JsonValueKind.Undefined"/> if the
            /// value cannot be constructed from the given instance (e.g. because they have an incompatible dotnet backing type.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static DefaultEntity FromAny(in JsonAny value)
            {
                if (value.HasJsonElementBacking)
                {
                    return new(value.AsJsonElement);
                }

                JsonValueKind valueKind = value.ValueKind;
                return valueKind switch
                {
                    JsonValueKind.String => new((string)value.AsString),
                    JsonValueKind.True => new(true),
                    JsonValueKind.False => new(false),
                    JsonValueKind.Number => new(value.AsNumber.AsBinaryJsonNumber),
                    JsonValueKind.Array => new(value.AsArray.AsImmutableList()),
                    JsonValueKind.Object => new(value.AsObject.AsPropertyBacking()),
                    JsonValueKind.Null => Null,
                    _ => Undefined,
                };
            }

            /// <summary>
            /// Gets an instance of the JSON value from a <see cref = "JsonElement"/> value.
            /// </summary>
            /// <param name = "value">The <see cref = "JsonElement"/> value from which to instantiate the instance.</param>
            /// <returns>An instance of this type, initialized from the <see cref = "JsonElement"/>.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static DefaultEntity FromJson(in JsonElement value)
            {
                return new(value);
            }

#if NET8_0_OR_GREATER
    /// <summary>
    /// Gets an instance of the JSON value from a boolean value.
    /// </summary>
    /// <typeparam name = "TValue">The type of the value.</typeparam>
    /// <param name="value">The value from which to instantiate the instance.</param>
    /// <returns>An instance of this type, initialized from the value.</returns>
    /// <remarks>This will be DefaultEntity.Undefined if the type is not compatible.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static DefaultEntity IJsonValue<DefaultEntity>.FromBoolean<TValue>(in TValue value)
    {
        if (value.HasJsonElementBacking)
        {
            return new(value.AsJsonElement);
        }

        if (value.ValueKind == JsonValueKind.True)
        {
            return new(true);
        }

        if (value.ValueKind == JsonValueKind.False)
        {
            return new(false);
        }

        return Undefined;
    }
#endif
#if NET8_0_OR_GREATER
    /// <summary>
    /// Gets an instance of the JSON value from a string value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value from which to instantiate the instance.</param>
    /// <returns>An instance of this type, initialized from the value.</returns>
    /// <remarks>This will be DefaultEntity.Undefined if the type is not compatible.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static DefaultEntity IJsonValue<DefaultEntity>.FromString<TValue>(in TValue value)
    {
        if (value.HasJsonElementBacking)
        {
            return new(value.AsJsonElement);
        }

        if (value.ValueKind == JsonValueKind.String)
        {
            return new(value.GetString()!);
        }

        return Undefined;
    }
#endif
#if NET8_0_OR_GREATER
    /// <summary>
    /// Gets an instance of the JSON value from a number value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value from which to instantiate the instance.</param>
    /// <returns>An instance of this type, initialized from the value.</returns>
    /// <remarks>This will be DefaultEntity.Undefined if the type is not compatible.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static DefaultEntity IJsonValue<DefaultEntity>.FromNumber<TValue>(in TValue value)
    {
        if (value.HasJsonElementBacking)
        {
            return new(value.AsJsonElement);
        }

        if (value.ValueKind == JsonValueKind.Number)
        {
            return new(value.AsBinaryJsonNumber);
        }

        return Undefined;
    }
#endif
#if NET8_0_OR_GREATER
    /// <summary>
    /// Gets an instance of the JSON value from an array value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value from which to instantiate the instance.</param>
    /// <returns>An instance of this type, initialized from the value.</returns>
    /// <remarks>This will be DefaultEntity.Undefined if the type is not compatible.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static DefaultEntity IJsonValue<DefaultEntity>.FromArray<TValue>(in TValue value)
    {
        if (value.HasJsonElementBacking)
        {
            return new(value.AsJsonElement);
        }

        if (value.ValueKind == JsonValueKind.Array)
        {
            return new(value.AsImmutableList());
        }

        return Undefined;
    }
#endif
#if NET8_0_OR_GREATER
    /// <summary>
    /// Gets an instance of the JSON value from an object value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value from which to instantiate the instance.</param>
    /// <returns>An instance of this type, initialized from the value.</returns>
    /// <remarks>This will be DefaultEntity.Undefined if the type is not compatible.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static DefaultEntity IJsonValue<DefaultEntity>.FromObject<TValue>(in TValue value)
    {
        if (value.HasJsonElementBacking)
        {
            return new(value.AsJsonElement);
        }

        if (value.ValueKind == JsonValueKind.Object)
        {
            return new(value.AsPropertyBacking());
        }

        return Undefined;
    }
#endif
            /// <summary>
            /// Parses a JSON string into a DefaultEntity.
            /// </summary>
            /// <param name = "json">The json string to parse.</param>
            /// <param name = "options">The (optional) JsonDocumentOptions.</param>
            /// <returns>A <see cref = "DefaultEntity"/> instance built from the JSON string.</returns>
            public static DefaultEntity Parse(string json, JsonDocumentOptions options = default)
            {
                using var jsonDocument = JsonDocument.Parse(json, options);
                return new DefaultEntity(jsonDocument.RootElement.Clone());
            }

            /// <summary>
            /// Parses a JSON string into a DefaultEntity.
            /// </summary>
            /// <param name = "utf8Json">The json string to parse.</param>
            /// <param name = "options">The (optional) JsonDocumentOptions.</param>
            /// <returns>A <see cref = "DefaultEntity"/> instance built from the JSON string.</returns>
            public static DefaultEntity Parse(Stream utf8Json, JsonDocumentOptions options = default)
            {
                using var jsonDocument = JsonDocument.Parse(utf8Json, options);
                return new DefaultEntity(jsonDocument.RootElement.Clone());
            }

            /// <summary>
            /// Parses a JSON string into a DefaultEntity.
            /// </summary>
            /// <param name = "utf8Json">The json string to parse.</param>
            /// <param name = "options">The (optional) JsonDocumentOptions.</param>
            /// <returns>A <see cref = "DefaultEntity"/> instance built from the JSON string.</returns>
            public static DefaultEntity Parse(ReadOnlyMemory<byte> utf8Json, JsonDocumentOptions options = default)
            {
                using var jsonDocument = JsonDocument.Parse(utf8Json, options);
                return new DefaultEntity(jsonDocument.RootElement.Clone());
            }

            /// <summary>
            /// Parses a JSON string into a DefaultEntity.
            /// </summary>
            /// <param name = "json">The json string to parse.</param>
            /// <param name = "options">The (optional) JsonDocumentOptions.</param>
            /// <returns>A <see cref = "DefaultEntity"/> instance built from the JSON string.</returns>
            public static DefaultEntity Parse(ReadOnlyMemory<char> json, JsonDocumentOptions options = default)
            {
                using var jsonDocument = JsonDocument.Parse(json, options);
                return new DefaultEntity(jsonDocument.RootElement.Clone());
            }

            /// <summary>
            /// Parses a JSON string into a DefaultEntity.
            /// </summary>
            /// <param name = "utf8Json">The json string to parse.</param>
            /// <param name = "options">The (optional) JsonDocumentOptions.</param>
            /// <returns>A <see cref = "DefaultEntity"/> instance built from the JSON string.</returns>
            public static DefaultEntity Parse(ReadOnlySequence<byte> utf8Json, JsonDocumentOptions options = default)
            {
                using var jsonDocument = JsonDocument.Parse(utf8Json, options);
                return new DefaultEntity(jsonDocument.RootElement.Clone());
            }

            /// <summary>
            /// Parses a JSON value from a buffer.
            /// </summary>
            /// <param name = "buffer">The buffer from which to parse the value.</param>
            /// <returns>The parsed value.</returns>
            static DefaultEntity ParseValue(ReadOnlySpan<char> buffer)
            {
#if NET8_0_OR_GREATER
        return IJsonValue<DefaultEntity>.ParseValue(buffer);
#else
                return JsonValueHelpers.ParseValue<DefaultEntity>(buffer);
#endif
            }

            /// <summary>
            /// Parses a JSON value from a buffer.
            /// </summary>
            /// <param name = "buffer">The buffer from which to parse the value.</param>
            /// <returns>The parsed value.</returns>
            static DefaultEntity ParseValue(ReadOnlySpan<byte> buffer)
            {
#if NET8_0_OR_GREATER
        return IJsonValue<DefaultEntity>.ParseValue(buffer);
#else
                return JsonValueHelpers.ParseValue<DefaultEntity>(buffer);
#endif
            }

            /// <summary>
            /// Parses a JSON value from a buffer.
            /// </summary>
            /// <param name = "reader">The reader from which to parse the value.</param>
            /// <returns>The parsed value.</returns>
            static DefaultEntity ParseValue(ref Utf8JsonReader reader)
            {
#if NET8_0_OR_GREATER
        return IJsonValue<DefaultEntity>.ParseValue(ref reader);
#else
                return JsonValueHelpers.ParseValue<DefaultEntity>(ref reader);
#endif
            }

            /// <summary>
            /// Gets the value as an instance of the target value.
            /// </summary>
            /// <typeparam name = "TTarget">The type of the target.</typeparam>
            /// <returns>An instance of the target type.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TTarget As<TTarget>()
                where TTarget : struct, IJsonValue<TTarget>
            {
#if NET8_0_OR_GREATER
        if ((this.backing & Backing.JsonElement) != 0)
        {
            return TTarget.FromJson(this.jsonElementBacking);
        }

        if ((this.backing & Backing.String) != 0)
        {
            return TTarget.FromString(this);
        }

        if ((this.backing & Backing.Bool) != 0)
        {
            return TTarget.FromBoolean(this);
        }

        if ((this.backing & Backing.Number) != 0)
        {
            return TTarget.FromNumber(this);
        }

        if ((this.backing & Backing.Array) != 0)
        {
            return TTarget.FromArray(this);
        }

        if ((this.backing & Backing.Object) != 0)
        {
            return TTarget.FromObject(this);
        }

        if ((this.backing & Backing.Null) != 0)
        {
            return TTarget.Null;
        }

        return TTarget.Undefined;
#else
                return this.As<DefaultEntity, TTarget>();
#endif
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj)
            {
                return (obj is IJsonValue jv && this.Equals(jv.AsAny)) || (obj is null && this.IsNull());
            }

            /// <inheritdoc/>
            public bool Equals<T>(in T other)
                where T : struct, IJsonValue<T>
            {
                return JsonValueHelpers.CompareValues(this, other);
            }

            /// <summary>
            /// Equality comparison.
            /// </summary>
            /// <param name = "other">The other item with which to compare.</param>
            /// <returns><see langword="true"/> if the values were equal.</returns>
            public bool Equals(in DefaultEntity other)
            {
                return JsonValueHelpers.CompareValues(this, other);
            }

            /// <inheritdoc/>
            public void WriteTo(Utf8JsonWriter writer)
            {
                if ((this.backing & Backing.JsonElement) != 0)
                {
                    if (this.jsonElementBacking.ValueKind != JsonValueKind.Undefined)
                    {
                        this.jsonElementBacking.WriteTo(writer);
                    }

                    return;
                }

                if ((this.backing & Backing.Array) != 0)
                {
                    JsonValueHelpers.WriteItems(this.arrayBacking, writer);
                    return;
                }

                if ((this.backing & Backing.Bool) != 0)
                {
                    writer.WriteBooleanValue(this.numberBacking.GetByteAsBool());
                    return;
                }

                if ((this.backing & Backing.Number) != 0)
                {
                    this.numberBacking.WriteTo(writer);
                    return;
                }

                if ((this.backing & Backing.Object) != 0)
                {
                    JsonValueHelpers.WriteProperties(this.objectBacking, writer);
                    return;
                }

                if ((this.backing & Backing.String) != 0)
                {
                    writer.WriteStringValue(this.stringBacking);
                    return;
                }

                if ((this.backing & Backing.Null) != 0)
                {
                    writer.WriteNullValue();
                    return;
                }
            }

            /// <inheritdoc/>
            public override int GetHashCode()
            {
                return JsonValueHelpers.GetHashCode(this);
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return this.Serialize();
            }
        }
    }
}