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
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Corvus.Json;
using Corvus.Json.Internal;

namespace ACRealms.Roslyn.RealmProps.IntermediateModels;
/// <summary>
/// Generated from JSON Schema.
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(Corvus.Json.Internal.JsonValueConverter<Description>))]
public readonly partial struct Description
    : IJsonValue<ACRealms.Roslyn.RealmProps.IntermediateModels.Description>
{
    private readonly Backing backing;
    private readonly JsonElement jsonElementBacking;
    private readonly string stringBacking;
    private readonly ImmutableList<JsonAny> arrayBacking;

    /// <summary>
    /// Initializes a new instance of the <see cref="Description"/> struct.
    /// </summary>
    public Description()
    {
        this.jsonElementBacking = default;
        this.backing = Backing.JsonElement;
        this.stringBacking = string.Empty;
        this.arrayBacking = ImmutableList<JsonAny>.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Description"/> struct.
    /// </summary>
    /// <param name="value">The value from which to construct the instance.</param>
    public Description(in JsonElement value)
    {
        this.jsonElementBacking = value;
        this.backing = Backing.JsonElement;
        this.stringBacking = string.Empty;
        this.arrayBacking = ImmutableList<JsonAny>.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Description"/> struct.
    /// </summary>
    /// <param name="value">The value from which to construct the instance.</param>
    public Description(ImmutableList<JsonAny> value)
    {
        this.backing = Backing.Array;
        this.jsonElementBacking = default;
        this.stringBacking = string.Empty;
        this.arrayBacking = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Description"/> struct.
    /// </summary>
    /// <param name="value">The value from which to construct the instance.</param>
    public Description(string value)
    {
        this.backing = Backing.String;
        this.jsonElementBacking = default;
        this.stringBacking = value;
        this.arrayBacking = ImmutableList<JsonAny>.Empty;
    }

    /// <summary>
    /// Gets the schema location from which this type was generated.
    /// </summary>
    public static string SchemaLocation { get; } = "components/description.json";

    /// <summary>
    /// Gets a Null instance.
    /// </summary>
    public static Description Null { get; } = new(JsonValueHelpers.NullElement);

    /// <summary>
    /// Gets an Undefined instance.
    /// </summary>
    public static Description Undefined { get; }

    /// <summary>
    /// Gets the default instance.
    /// </summary>
    public static Description DefaultInstance { get; }

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

            if ((this.backing & Backing.Array) != 0)
            {
                return new(this.arrayBacking);
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

            if ((this.backing & Backing.Array) != 0)
            {
                return JsonValueHelpers.ArrayToJsonElement(this.arrayBacking);
            }

            if ((this.backing & Backing.Null) != 0)
            {
                return JsonValueHelpers.NullElement;
            }

            return default;
        }
    }

    /// <inheritdoc/>
    public JsonString AsString
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

            throw new InvalidOperationException();
        }
    }

    /// <inheritdoc/>
    public JsonArray AsArray
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

    /// <summary>
    /// Gets the instance as a <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm" />.
    /// </summary>
    public ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm AsStringForm
    {
        get
        {
            return this.As<ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm>();
        }
    }

    /// <summary>
    /// Gets a value indicating whether the instance is a <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm" />.
    /// </summary>
    public bool IsStringForm
    {
        get
        {
            return this.As<ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm>().IsValid();
        }
    }

    /// <summary>
    /// Gets the instance as a <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray" />.
    /// </summary>
    public ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray AsDescriptionArray
    {
        get
        {
            return this.As<ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray>();
        }
    }

    /// <summary>
    /// Gets a value indicating whether the instance is a <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray" />.
    /// </summary>
    public bool IsDescriptionArray
    {
        get
        {
            return this.As<ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray>().IsValid();
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

            if ((this.backing & Backing.Array) != 0)
            {
                return JsonValueKind.Array;
            }

            return JsonValueKind.Undefined;
        }
    }

    /// <summary>
    /// Conversion from JsonAny.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static implicit operator Description(JsonAny value)
    {
        return value.As<Description>();
    }

    /// <summary>
    /// Conversion to JsonAny.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static implicit operator JsonAny(Description value)
    {
        return value.AsAny;
    }

    /// <summary>
    /// Conversion to <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm"/>.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static explicit operator ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm(Description value)
    {
        return value.As<ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm>();
    }

    /// <summary>
    /// Conversion from <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm"/>.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static implicit operator Description(ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm value)
    {
        return value.As<Description>();
    }

    /// <summary>
    /// Conversion to <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionPattern"/>.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static explicit operator ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionPattern(Description value)
    {
        return value.As<ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionPattern>();
    }

    /// <summary>
    /// Conversion from <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionPattern"/>.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static explicit operator Description(ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionPattern value)
    {
        return value.As<Description>();
    }

    /// <summary>
    /// Conversion to <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.ValStringSimple"/>.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static explicit operator ACRealms.Roslyn.RealmProps.IntermediateModels.ValStringSimple(Description value)
    {
        return value.As<ACRealms.Roslyn.RealmProps.IntermediateModels.ValStringSimple>();
    }

    /// <summary>
    /// Conversion from <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.ValStringSimple"/>.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static explicit operator Description(ACRealms.Roslyn.RealmProps.IntermediateModels.ValStringSimple value)
    {
        return value.As<Description>();
    }

    /// <summary>
    /// Conversion to <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray"/>.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static explicit operator ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray(Description value)
    {
        return value.As<ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray>();
    }

    /// <summary>
    /// Conversion from <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray"/>.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static implicit operator Description(ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray value)
    {
        return value.As<Description>();
    }

    /// <summary>
    /// Conversion to <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.StringMultilineFromArray"/>.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static explicit operator ACRealms.Roslyn.RealmProps.IntermediateModels.StringMultilineFromArray(Description value)
    {
        return value.As<ACRealms.Roslyn.RealmProps.IntermediateModels.StringMultilineFromArray>();
    }

    /// <summary>
    /// Conversion from <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.StringMultilineFromArray"/>.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static explicit operator Description(ACRealms.Roslyn.RealmProps.IntermediateModels.StringMultilineFromArray value)
    {
        return value.As<Description>();
    }

    /// <summary>
    /// Conversion to <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.StringSplitArray"/>.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static explicit operator ACRealms.Roslyn.RealmProps.IntermediateModels.StringSplitArray(Description value)
    {
        return value.As<ACRealms.Roslyn.RealmProps.IntermediateModels.StringSplitArray>();
    }

    /// <summary>
    /// Conversion from <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.StringSplitArray"/>.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static explicit operator Description(ACRealms.Roslyn.RealmProps.IntermediateModels.StringSplitArray value)
    {
        return value.As<Description>();
    }

    /// <summary>
    /// Operator ==.
    /// </summary>
    /// <param name="left">The lhs of the operator.</param>
    /// <param name="right">The rhs of the operator.</param>
    /// <returns>
    /// <c>True</c> if the values are equal.
    /// </returns>
    public static bool operator ==(in Description left, in Description right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Operator !=.
    /// </summary>
    /// <param name="left">The lhs of the operator.</param>
    /// <param name="right">The rhs of the operator.</param>
    /// <returns>
    /// <c>True</c> if the values are not equal.
    /// </returns>
    public static bool operator !=(in Description left, in Description right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Gets an instance of the JSON value from a <see cref="JsonElement"/> value.
    /// </summary>
    /// <param name="value">The <see cref="JsonElement"/> value from which to instantiate the instance.</param>
    /// <returns>An instance of this type, initialized from the <see cref="JsonElement"/>.</returns>
    /// <remarks>The returned value will have a <see cref = "IJsonValue.ValueKind"/> of <see cref = "JsonValueKind.Undefined"/> if the
    /// value cannot be constructed from the given instance (e.g. because they have an incompatible .NET backing type).
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Description FromJson(in JsonElement value)
    {
        return new(value);
    }

    /// <summary>
    /// Gets an instance of the JSON value from a <see cref="JsonAny"/> value.
    /// </summary>
    /// <param name="value">The <see cref="JsonAny"/> value from which to instantiate the instance.</param>
    /// <returns>An instance of this type, initialized from the <see cref="JsonAny"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Description FromAny(in JsonAny value)
    {
        if (value.HasJsonElementBacking)
        {
            return new(value.AsJsonElement);
        }

        return value.ValueKind switch
        {
            JsonValueKind.String => new(value.AsString.GetString()!),
            JsonValueKind.Array => new(value.AsArray.AsImmutableList()),
            JsonValueKind.Null => Null,
            _ => Undefined,
        };
    }

#if NET8_0_OR_GREATER
    /// <summary>
    /// Gets an instance of the JSON value from the provided value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value from which to instantiate the instance.</param>
    /// <returns>An instance of this type, initialized from the provided value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Description IJsonValue<Description>.FromBoolean<TValue>(in TValue value)
    {
        if (value.HasJsonElementBacking)
        {
            return new(value.AsJsonElement);
        }

        return Undefined;
    }
#endif

    /// <summary>
    /// Gets an instance of the JSON value from the provided value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value from which to instantiate the instance.</param>
    /// <returns>An instance of this type, initialized from the provided value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Description FromString<TValue>(in TValue value)
        where TValue : struct, IJsonString<TValue>
    {
        if (value.HasJsonElementBacking)
        {
            return new(value.AsJsonElement);
        }

        return value.ValueKind switch
        {
            JsonValueKind.String => new(value.GetString()!),
            JsonValueKind.Null => Null,
            _ => Undefined,
        };
    }

#if NET8_0_OR_GREATER
    /// <summary>
    /// Gets an instance of the JSON value from the provided value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value from which to instantiate the instance.</param>
    /// <returns>An instance of this type, initialized from the provided value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Description IJsonValue<Description>.FromNumber<TValue>(in TValue value)
    {
        if (value.HasJsonElementBacking)
        {
            return new(value.AsJsonElement);
        }

        return Undefined;
    }
#endif

#if NET8_0_OR_GREATER
    /// <summary>
    /// Gets an instance of the JSON value from the provided value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value from which to instantiate the instance.</param>
    /// <returns>An instance of this type, initialized from the provided value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Description IJsonValue<Description>.FromObject<TValue>(in TValue value)
    {
        if (value.HasJsonElementBacking)
        {
            return new(value.AsJsonElement);
        }

        return Undefined;
    }
#endif

    /// <summary>
    /// Gets an instance of the JSON value from the provided value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value from which to instantiate the instance.</param>
    /// <returns>An instance of this type, initialized from the provided value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Description FromArray<TValue>(in TValue value)
        where TValue : struct, IJsonArray<TValue>
    {
        if (value.HasJsonElementBacking)
        {
            return new(value.AsJsonElement);
        }

        return value.ValueKind switch
        {
            JsonValueKind.Array => new(value.AsImmutableList()),
            JsonValueKind.Null => Null,
            _ => Undefined,
        };
    }

    /// <summary>
    /// Parses the Description.
    /// </summary>
    /// <param name="source">The source of the JSON string to parse.</param>
    /// <param name="options">The (optional) JsonDocumentOptions.</param>
    public static Description Parse(string source, JsonDocumentOptions options = default)
    {
        using var jsonDocument = JsonDocument.Parse(source, options);
        return new(jsonDocument.RootElement.Clone());
    }

    /// <summary>
    /// Parses the Description.
    /// </summary>
    /// <param name="source">The source of the JSON string to parse.</param>
    /// <param name="options">The (optional) JsonDocumentOptions.</param>
    public static Description Parse(Stream source, JsonDocumentOptions options = default)
    {
        using var jsonDocument = JsonDocument.Parse(source, options);
        return new(jsonDocument.RootElement.Clone());
    }

    /// <summary>
    /// Parses the Description.
    /// </summary>
    /// <param name="source">The source of the JSON string to parse.</param>
    /// <param name="options">The (optional) JsonDocumentOptions.</param>
    public static Description Parse(ReadOnlyMemory<byte> source, JsonDocumentOptions options = default)
    {
        using var jsonDocument = JsonDocument.Parse(source, options);
        return new(jsonDocument.RootElement.Clone());
    }

    /// <summary>
    /// Parses the Description.
    /// </summary>
    /// <param name="source">The source of the JSON string to parse.</param>
    /// <param name="options">The (optional) JsonDocumentOptions.</param>
    public static Description Parse(ReadOnlyMemory<char> source, JsonDocumentOptions options = default)
    {
        using var jsonDocument = JsonDocument.Parse(source, options);
        return new(jsonDocument.RootElement.Clone());
    }

    /// <summary>
    /// Parses the Description.
    /// </summary>
    /// <param name="source">The source of the JSON string to parse.</param>
    /// <param name="options">The (optional) JsonDocumentOptions.</param>
    public static Description Parse(ReadOnlySequence<byte> source, JsonDocumentOptions options = default)
    {
        using var jsonDocument = JsonDocument.Parse(source, options);
        return new(jsonDocument.RootElement.Clone());
    }

    /// <summary>
    /// Parses the Description.
    /// </summary>
    /// <param name="source">The source of the JSON string to parse.</param>
    public static Description ParseValue(string source)
    {
#if NET8_0_OR_GREATER
        return IJsonValue<Description>.ParseValue(source);
#else
        return JsonValueHelpers.ParseValue<Description>(source.AsSpan());
#endif
    }

    /// <summary>
    /// Parses the Description.
    /// </summary>
    /// <param name="source">The source of the JSON string to parse.</param>
    public static Description ParseValue(ReadOnlySpan<char> source)
    {
#if NET8_0_OR_GREATER
        return IJsonValue<Description>.ParseValue(source);
#else
        return JsonValueHelpers.ParseValue<Description>(source);
#endif
    }

    /// <summary>
    /// Parses the Description.
    /// </summary>
    /// <param name="source">The source of the JSON string to parse.</param>
    public static Description ParseValue(ReadOnlySpan<byte> source)
    {
#if NET8_0_OR_GREATER
        return IJsonValue<Description>.ParseValue(source);
#else
        return JsonValueHelpers.ParseValue<Description>(source);
#endif
    }

    /// <summary>
    /// Parses the Description.
    /// </summary>
    /// <param name="source">The source of the JSON string to parse.</param>
    public static Description ParseValue(ref Utf8JsonReader source)
    {
#if NET8_0_OR_GREATER
        return IJsonValue<Description>.ParseValue(ref source);
#else
        return JsonValueHelpers.ParseValue<Description>(ref source);
#endif
    }

    /// <summary>
    /// Gets the value as an instance of the target value.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
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
            return TTarget.FromString(this.AsString);
        }

        if ((this.backing & Backing.Array) != 0)
        {
            return TTarget.FromArray(this.AsArray);
        }

        if ((this.backing & Backing.Null) != 0)
        {
            return TTarget.Null;
        }

        return TTarget.Undefined;
#else
        return this.As<Description, TTarget>();
#endif
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return
            (obj is IJsonValue jv && this.Equals(jv.As<Description>())) ||
            (obj is null && this.IsNull());
    }

    /// <inheritdoc/>
    public bool Equals<T>(in T other)
        where T : struct, IJsonValue<T>
    {
        return this.Equals(other.As<Description>());
    }

    /// <summary>
    /// Equality comparison.
    /// </summary>
    /// <param name="other">The other item with which to compare.</param>
    /// <returns><see langword="true"/> if the values were equal.</returns>
    public bool Equals(in Description other)
    {
        JsonValueKind thisKind = this.ValueKind;
        JsonValueKind otherKind = other.ValueKind;
        if (thisKind != otherKind)
        {
            return false;
        }

        if (thisKind == JsonValueKind.Null || thisKind == JsonValueKind.Undefined)
        {
            return true;
        }

        if (thisKind == JsonValueKind.Array)
        {
            JsonArray thisArray = this.AsArray;
            JsonArray otherArray = other.AsArray;
            JsonArrayEnumerator lhs = thisArray.EnumerateArray();
            JsonArrayEnumerator rhs = otherArray.EnumerateArray();
            while (lhs.MoveNext())
            {
                if (!rhs.MoveNext())
                {
                    return false;
                }

                if (!lhs.Current.Equals(rhs.Current))
                {
                    return false;
                }
            }

            return !rhs.MoveNext();
        }

        if (thisKind == JsonValueKind.String)
        {
            if (this.backing == Backing.JsonElement)
            {
                if (other.backing == Backing.String)
                {
                    return this.jsonElementBacking.ValueEquals(other.stringBacking);
                }
                else
                {
                    other.jsonElementBacking.TryGetValue(CompareValues, this.jsonElementBacking, out bool areEqual);
                    return areEqual;
                }

            }

            if (other.backing == Backing.JsonElement)
            {
                return other.jsonElementBacking.ValueEquals(this.stringBacking);
            }

            return this.stringBacking.Equals(other.stringBacking);

            static bool CompareValues(ReadOnlySpan<byte> span, in JsonElement firstItem, out bool value)
            {
                value = firstItem.ValueEquals(span);
                return true;
            }
        }

        return false;
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
        return this.ValueKind switch
        {
            JsonValueKind.Array => JsonValueHelpers.GetArrayHashCode(this),
            JsonValueKind.Object => JsonValueHelpers.GetObjectHashCode(((IJsonValue)this).AsObject),
            JsonValueKind.Number => JsonValueHelpers.GetHashCodeForNumber(((IJsonValue)this).AsNumber),
            JsonValueKind.String => JsonValueHelpers.GetHashCodeForString(this),
            JsonValueKind.True => true.GetHashCode(),
            JsonValueKind.False => false.GetHashCode(),
            JsonValueKind.Null => JsonValueHelpers.NullHashCode,
            _ => JsonValueHelpers.UndefinedHashCode,
        };
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.Serialize();
    }

    /// <summary>
    /// Matches the value against the composed values, and returns the result of calling the provided match function for the first match found.
    /// </summary>
    /// <typeparam name="TIn">The immutable context to pass in to the match function.</typeparam>
    /// <typeparam name="TOut">The result of calling the match function.</typeparam>
    /// <param name="context">The context to pass to the match function.</param>
    /// <param name="matchStringForm">Match a <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm"/>.</param>
    /// <param name="matchDescriptionArray">Match a <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray"/>.</param>
    /// <param name="defaultMatch">Match any other value.</param>
    /// <returns>An instance of the value returned by the match function.</returns>
    public TOut Match<TIn, TOut>(
        in TIn context,
        Matcher<ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm, TIn, TOut> matchStringForm,
        Matcher<ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray, TIn, TOut> matchDescriptionArray,
        Matcher<ACRealms.Roslyn.RealmProps.IntermediateModels.Description, TIn, TOut> defaultMatch)
    {
        ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm matchStringFormValue = this.As<ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm>();
        if (matchStringFormValue.IsValid())
        {
            return matchStringForm(matchStringFormValue, context);
        }

        ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray matchDescriptionArrayValue = this.As<ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray>();
        if (matchDescriptionArrayValue.IsValid())
        {
            return matchDescriptionArray(matchDescriptionArrayValue, context);
        }

        return defaultMatch(this, context);
    }

    /// <summary>
    /// Matches the value against the composed values, and returns the result of calling the provided match function for the first match found.
    /// </summary>
    /// <typeparam name="TOut">The result of calling the match function.</typeparam>
    /// <param name="matchStringForm">Match a <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm"/>.</param>
    /// <param name="matchDescriptionArray">Match a <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray"/>.</param>
    /// <param name="defaultMatch">Match any other value.</param>
    /// <returns>An instance of the value returned by the match function.</returns>
    public TOut Match<TOut>(
        Matcher<ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm, TOut> matchStringForm,
        Matcher<ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray, TOut> matchDescriptionArray,
        Matcher<ACRealms.Roslyn.RealmProps.IntermediateModels.Description, TOut> defaultMatch)
    {
        ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm matchStringFormValue = this.As<ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm>();
        if (matchStringFormValue.IsValid())
        {
            return matchStringForm(matchStringFormValue);
        }

        ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray matchDescriptionArrayValue = this.As<ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray>();
        if (matchDescriptionArrayValue.IsValid())
        {
            return matchDescriptionArray(matchDescriptionArrayValue);
        }

        return defaultMatch(this);
    }

    /// <summary>
    /// Gets the value as a <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm" />.
    /// </summary>
    /// <param name="result">The result of the conversions.</param>
    /// <returns><see langword="true" /> if the conversion was valid.</returns>
    public bool TryGetAsStringForm(out ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm result)
    {
        result = this.As<ACRealms.Roslyn.RealmProps.IntermediateModels.Description.StringForm>();
        return result.IsValid();
    }

    /// <summary>
    /// Gets the value as a <see cref="ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray" />.
    /// </summary>
    /// <param name="result">The result of the conversions.</param>
    /// <returns><see langword="true" /> if the conversion was valid.</returns>
    public bool TryGetAsDescriptionArray(out ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray result)
    {
        result = this.As<ACRealms.Roslyn.RealmProps.IntermediateModels.DescriptionArray>();
        return result.IsValid();
    }
}
