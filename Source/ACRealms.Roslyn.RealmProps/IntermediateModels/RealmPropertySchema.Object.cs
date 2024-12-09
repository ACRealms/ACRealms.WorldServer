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
using System.Text.Json;
using Corvus.Json;
using Corvus.Json.Internal;

namespace ACRealms.Roslyn.RealmProps.IntermediateModels;
/// <summary>
/// Generated from JSON Schema.
/// </summary>
public readonly partial struct RealmPropertySchema
    : IJsonObject<ACRealms.Roslyn.RealmProps.IntermediateModels.RealmPropertySchema>
{
    /// <summary>
    /// Conversion from <see cref="ImmutableList{JsonObjectProperty}"/>.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static implicit operator RealmPropertySchema(ImmutableList<JsonObjectProperty> value)
    {
        return new(value);
    }

    /// <summary>
    /// Conversion to <see cref="ImmutableList{JsonObjectProperty}"/>.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static implicit operator ImmutableList<JsonObjectProperty>(RealmPropertySchema value)
    {
        return
            __CorvusObjectHelpers.GetPropertyBacking(value);
    }

    /// <summary>
    /// Conversion from JsonObject.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static implicit operator RealmPropertySchema(JsonObject value)
    {
        if (value.HasDotnetBacking && value.ValueKind == JsonValueKind.Object)
        {
            return new(
                __CorvusObjectHelpers.GetPropertyBacking(value));
        }

        return new(value.AsJsonElement);
    }

    /// <summary>
    /// Conversion to JsonObject.
    /// </summary>
    /// <param name="value">The value from which to convert.</param>
    public static implicit operator JsonObject(RealmPropertySchema value)
    {
        return
            value.AsObject;
    }

    /// <inheritdoc/>
    public Corvus.Json.JsonAny this[in JsonPropertyName name]
    {
        get
        {
            if (this.TryGetProperty(name, out Corvus.Json.JsonAny result))
            {
                return result;
            }

            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Gets the number of properties in the object.
    /// </summary>
    public int Count
    {
        get
        {
            if ((this.backing & Backing.JsonElement) != 0)
            {
                return this.jsonElementBacking.GetPropertyCount();
            }

            if ((this.backing & Backing.Object) != 0)
            {
                return this.objectBacking.Count;
            }

            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Gets the <c>$schema_version</c> property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the instance is valid, this property will not be <see cref="JsonValueKind.Undefined"/>.
    /// </para>
    /// </remarks>
    public ACRealms.Roslyn.RealmProps.IntermediateModels.RealmPropertySchema.SchemaVersionEntity SchemaVersion
    {
        get
        {
            if ((this.backing & Backing.JsonElement) != 0)
            {
                if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
                {
                    return default;
                }

                if (this.jsonElementBacking.TryGetProperty(JsonPropertyNames.SchemaVersionUtf8, out JsonElement result))
                {
                    return new(result);
                }
            }

            if ((this.backing & Backing.Object) != 0)
            {
                if (this.objectBacking.TryGetValue(JsonPropertyNames.SchemaVersion, out JsonAny result))
                {
                    return result.As<ACRealms.Roslyn.RealmProps.IntermediateModels.RealmPropertySchema.SchemaVersionEntity>();
                }
            }

            return default;
        }
    }

    /// <summary>
    /// Gets the (optional) <c>additionalProperties</c> property.
    /// </summary>
    public Corvus.Json.JsonNotAny AdditionalProperties
    {
        get
        {
            if ((this.backing & Backing.JsonElement) != 0)
            {
                if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
                {
                    return default;
                }

                if (this.jsonElementBacking.TryGetProperty(JsonPropertyNames.AdditionalPropertiesUtf8, out JsonElement result))
                {
                    return new(result);
                }
            }

            if ((this.backing & Backing.Object) != 0)
            {
                if (this.objectBacking.TryGetValue(JsonPropertyNames.AdditionalProperties, out JsonAny result))
                {
                    return result.As<Corvus.Json.JsonNotAny>();
                }
            }

            return default;
        }
    }

    /// <summary>
    /// Gets the (optional) <c>groups</c> property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A group of properties that can share the same set of defaults.
    /// </para>
    /// </remarks>
    public ACRealms.Roslyn.RealmProps.IntermediateModels.RealmPropertySchema.GroupArray Groups
    {
        get
        {
            if ((this.backing & Backing.JsonElement) != 0)
            {
                if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
                {
                    return default;
                }

                if (this.jsonElementBacking.TryGetProperty(JsonPropertyNames.GroupsUtf8, out JsonElement result))
                {
                    return new(result);
                }
            }

            if ((this.backing & Backing.Object) != 0)
            {
                if (this.objectBacking.TryGetValue(JsonPropertyNames.Groups, out JsonAny result))
                {
                    return result.As<ACRealms.Roslyn.RealmProps.IntermediateModels.RealmPropertySchema.GroupArray>();
                }
            }

            return default;
        }
    }

    /// <summary>
    /// Gets the <c>namespace</c> property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the instance is valid, this property will not be <see cref="JsonValueKind.Undefined"/>.
    /// </para>
    /// </remarks>
    public Corvus.Json.JsonString Namespace
    {
        get
        {
            if ((this.backing & Backing.JsonElement) != 0)
            {
                if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
                {
                    return default;
                }

                if (this.jsonElementBacking.TryGetProperty(JsonPropertyNames.NamespaceUtf8, out JsonElement result))
                {
                    return new(result);
                }
            }

            if ((this.backing & Backing.Object) != 0)
            {
                if (this.objectBacking.TryGetValue(JsonPropertyNames.Namespace, out JsonAny result))
                {
                    return result.As<Corvus.Json.JsonString>();
                }
            }

            return default;
        }
    }

    /// <summary>
    /// Gets the (optional) <c>properties</c> property.
    /// </summary>
    public ACRealms.Roslyn.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropList Properties
    {
        get
        {
            if ((this.backing & Backing.JsonElement) != 0)
            {
                if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
                {
                    return default;
                }

                if (this.jsonElementBacking.TryGetProperty(JsonPropertyNames.PropertiesUtf8, out JsonElement result))
                {
                    return new(result);
                }
            }

            if ((this.backing & Backing.Object) != 0)
            {
                if (this.objectBacking.TryGetValue(JsonPropertyNames.Properties, out JsonAny result))
                {
                    return result.As<ACRealms.Roslyn.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropList>();
                }
            }

            return default;
        }
    }

    /// <inheritdoc/>
    public static RealmPropertySchema FromProperties(IDictionary<JsonPropertyName, JsonAny> source)
    {
        return new(source.Select(kvp => new JsonObjectProperty(kvp.Key, kvp.Value)).ToImmutableList());
    }

    /// <inheritdoc/>
    public static RealmPropertySchema FromProperties(params (JsonPropertyName Name, JsonAny Value)[] source)
    {
        return new(source.Select(s => new JsonObjectProperty(s.Name, s.Value.AsAny)).ToImmutableList());
    }

    /// <summary>
    /// Creates an instance of the type from the given immutable list of properties.
    /// </summary>
    /// <param name="source">The list of properties.</param>
    /// <returns>An instance of the type initialized from the list of properties.</returns>
    public static RealmPropertySchema FromProperties(ImmutableList<JsonObjectProperty> source)
    {
        return new(source);
    }

    /// <summary>
    /// Creates an instance of a <see cref="RealmPropertySchema"/>.
    /// </summary>
    public static RealmPropertySchema Create(
        in Corvus.Json.JsonString namespaceValue,
        in Corvus.Json.JsonNotAny? additionalProperties = null,
        in ACRealms.Roslyn.RealmProps.IntermediateModels.RealmPropertySchema.GroupArray? groups = null,
        in ACRealms.Roslyn.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropList? properties = null)
    {
        var builder = ImmutableList.CreateBuilder<JsonObjectProperty>();
        builder.Add(JsonPropertyNames.SchemaVersion, ACRealms.Roslyn.RealmProps.IntermediateModels.RealmPropertySchema.SchemaVersionEntity.ConstInstance.AsAny);
        builder.Add(JsonPropertyNames.Namespace, namespaceValue.AsAny);

        if (additionalProperties is not null)
        {
            builder.Add(JsonPropertyNames.AdditionalProperties, additionalProperties.Value.AsAny);
        }

        if (groups is not null)
        {
            builder.Add(JsonPropertyNames.Groups, groups.Value.AsAny);
        }

        if (properties is not null)
        {
            builder.Add(JsonPropertyNames.Properties, properties.Value.AsAny);
        }

        return new(builder.ToImmutable());
    }

    /// <inheritdoc/>
    public ImmutableList<JsonObjectProperty> AsPropertyBacking()
    {
        return __CorvusObjectHelpers.GetPropertyBacking(this);
    }
    /// <inheritdoc/>
    public ImmutableList<JsonObjectProperty>.Builder AsPropertyBackingBuilder()
    {
        return __CorvusObjectHelpers.GetPropertyBacking(this).ToBuilder();
    }

    /// <inheritdoc/>
    public JsonObjectEnumerator EnumerateObject()
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

    /// <inheritdoc/>
    public bool HasProperties()
    {
        if ((this.backing & Backing.Object) != 0)
        {
            return this.objectBacking.Count > 0;
        }

        if ((this.backing & Backing.JsonElement) != 0)
        {
            using JsonElement.ObjectEnumerator enumerator = this.jsonElementBacking.EnumerateObject();
            return enumerator.MoveNext();
        }

        throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public bool HasProperty(in JsonPropertyName name)
    {
        if ((this.backing & Backing.Object) != 0)
        {
            return this.objectBacking.ContainsKey(name);
        }

        if ((this.backing & Backing.JsonElement) != 0)
        {
            return name.TryGetProperty(this.jsonElementBacking, out JsonElement _);
        }

        throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public bool HasProperty(string name)
    {
        if ((this.backing & Backing.Object) != 0)
        {
            return this.objectBacking.ContainsKey(name);
        }

        if ((this.backing & Backing.JsonElement) != 0)
        {
            return this.jsonElementBacking.TryGetProperty(name, out _);
        }

        throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public bool HasProperty(ReadOnlySpan<char> name)
    {
        if ((this.backing & Backing.Object) != 0)
        {
            return this.objectBacking.ContainsKey(name);
        }

        if ((this.backing & Backing.JsonElement) != 0)
        {
            return this.jsonElementBacking.TryGetProperty(name, out _);
        }

        throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public bool HasProperty(ReadOnlySpan<byte> name)
    {
        if ((this.backing & Backing.Object) != 0)
        {
            return this.objectBacking.ContainsKey(name);
        }

        if ((this.backing & Backing.JsonElement) != 0)
        {
            return this.jsonElementBacking.TryGetProperty(name, out _);
        }

        throw new InvalidOperationException();
    }

    /// <summary>
    /// Sets the <c>$schema_version</c> property.
    /// </summary>
    /// <param name="value">The new property value</param>
    /// <returns>The instance with the property set.</returns>
    public RealmPropertySchema WithSchemaVersion(in ACRealms.Roslyn.RealmProps.IntermediateModels.RealmPropertySchema.SchemaVersionEntity value)
    {
        return this.SetProperty(JsonPropertyNames.SchemaVersion, value);
    }

    /// <summary>
    /// Sets the (optional) <c>additionalProperties</c> property.
    /// </summary>
    /// <param name="value">The new property value</param>
    /// <returns>The instance with the property set.</returns>
    public RealmPropertySchema WithAdditionalProperties(in Corvus.Json.JsonNotAny value)
    {
        return this.SetProperty(JsonPropertyNames.AdditionalProperties, value);
    }

    /// <summary>
    /// Sets the (optional) <c>groups</c> property.
    /// </summary>
    /// <param name="value">The new property value</param>
    /// <returns>The instance with the property set.</returns>
    /// <remarks>
    /// <para>
    /// A group of properties that can share the same set of defaults.
    /// </para>
    /// </remarks>
    public RealmPropertySchema WithGroups(in ACRealms.Roslyn.RealmProps.IntermediateModels.RealmPropertySchema.GroupArray value)
    {
        return this.SetProperty(JsonPropertyNames.Groups, value);
    }

    /// <summary>
    /// Sets the <c>namespace</c> property.
    /// </summary>
    /// <param name="value">The new property value</param>
    /// <returns>The instance with the property set.</returns>
    public RealmPropertySchema WithNamespace(in Corvus.Json.JsonString value)
    {
        return this.SetProperty(JsonPropertyNames.Namespace, value);
    }

    /// <summary>
    /// Sets the (optional) <c>properties</c> property.
    /// </summary>
    /// <param name="value">The new property value</param>
    /// <returns>The instance with the property set.</returns>
    public RealmPropertySchema WithProperties(in ACRealms.Roslyn.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropList value)
    {
        return this.SetProperty(JsonPropertyNames.Properties, value);
    }

    /// <summary>
    /// Get a property.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns><c>True</c> if the property was present.</returns>
    /// <exception cref="InvalidOperationException">The value is not an object.</exception>
    public bool TryGetProperty(in JsonPropertyName name, out JsonAny value)
    {
        if ((this.backing & Backing.JsonElement) != 0)
        {
            if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
            {
                value = default;
                return false;
            }

            if (name.TryGetProperty(this.jsonElementBacking, out JsonElement element))
            {
                value = new(element);
                return true;
            }

            value = default;
            return false;
        }

        if ((this.backing & Backing.Object) != 0)
        {
            if (this.objectBacking.TryGetValue(name, out JsonAny result))
            {
                value = result;
                return true;
            }

            value = default;
            return false;
        }

        throw new InvalidOperationException();
    }

    /// <summary>
    /// Get a property.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns><c>True</c> if the property was present.</returns>
    /// <exception cref="InvalidOperationException">The value is not an object.</exception>
    public bool TryGetProperty(string name, out JsonAny value)
    {
        if ((this.backing & Backing.JsonElement) != 0)
        {
            if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
            {
                value = default;
                return false;
            }

            if (this.jsonElementBacking.TryGetProperty(name, out JsonElement element))
            {
                value = new(element);
                return true;
            }

            value = default;
            return false;
        }

        if ((this.backing & Backing.Object) != 0)
        {
            if (this.objectBacking.TryGetValue(name, out JsonAny result))
            {
                value = result;
                return true;
            }

            value = default;
            return false;
        }

        throw new InvalidOperationException();
    }

    /// <summary>
    /// Get a property.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns><c>True</c> if the property was present.</returns>
    /// <exception cref="InvalidOperationException">The value is not an object.</exception>
    public bool TryGetProperty(ReadOnlySpan<char> name, out JsonAny value)
    {
        if ((this.backing & Backing.JsonElement) != 0)
        {
            if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
            {
                value = default;
                return false;
            }

            if (this.jsonElementBacking.TryGetProperty(name, out JsonElement element))
            {
                value = new(element);
                return true;
            }

            value = default;
            return false;
        }

        if ((this.backing & Backing.Object) != 0)
        {
            if (this.objectBacking.TryGetValue(name, out JsonAny result))
            {
                value = result;
                return true;
            }

            value = default;
            return false;
        }

        throw new InvalidOperationException();
    }

    /// <summary>
    /// Get a property.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns><c>True</c> if the property was present.</returns>
    /// <exception cref="InvalidOperationException">The value is not an object.</exception>
    public bool TryGetProperty(ReadOnlySpan<byte> name, out JsonAny value)
    {
        if ((this.backing & Backing.JsonElement) != 0)
        {
            if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
            {
                value = default;
                return false;
            }

            if (this.jsonElementBacking.TryGetProperty(name, out JsonElement element))
            {
                value = new(element);
                return true;
            }

            value = default;
            return false;
        }

        if ((this.backing & Backing.Object) != 0)
        {
            if (this.objectBacking.TryGetValue(name, out JsonAny result))
            {
                value = result;
                return true;
            }

            value = default;
            return false;
        }

        throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public bool TryGetProperty<TValue>(in JsonPropertyName name, out TValue value)
        where TValue : struct, IJsonValue<TValue>
    {
        if ((this.backing & Backing.JsonElement) != 0)
        {
            if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
            {
                value = default;
                return false;
            }

            if (name.TryGetProperty(this.jsonElementBacking, out JsonElement element))
            {
#if NET8_0_OR_GREATER
                value = TValue.FromJson(element);
#else
                value = JsonValueNetStandard20Extensions.FromJsonElement<TValue>(element);
#endif

                return true;
            }

            value = default;
            return false;
        }

        if ((this.backing & Backing.Object) != 0)
        {
            if (this.objectBacking.TryGetValue(name, out JsonAny result))
            {
#if NET8_0_OR_GREATER
                value = TValue.FromAny(result);
#else
                value = result.As<TValue>();
#endif
                return true;
            }

            value = default;
            return false;
        }

        throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public bool TryGetProperty<TValue>(string name, out TValue value)
        where TValue : struct, IJsonValue<TValue>
    {
        if ((this.backing & Backing.JsonElement) != 0)
        {
            if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
            {
                value = default;
                return false;
            }

            if (this.jsonElementBacking.TryGetProperty(name, out JsonElement element))
            {
#if NET8_0_OR_GREATER
                value = TValue.FromJson(element);
#else
                value = JsonValueNetStandard20Extensions.FromJsonElement<TValue>(element);
#endif

                return true;
            }

            value = default;
            return false;
        }

        if ((this.backing & Backing.Object) != 0)
        {
            if (this.objectBacking.TryGetValue(name, out JsonAny result))
            {
#if NET8_0_OR_GREATER
                value = TValue.FromAny(result);
#else
                value = result.As<TValue>();
#endif
                return true;
            }

            value = default;
            return false;
        }

        throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public bool TryGetProperty<TValue>(ReadOnlySpan<char> name, out TValue value)
        where TValue : struct, IJsonValue<TValue>
    {
        if ((this.backing & Backing.JsonElement) != 0)
        {
            if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
            {
                value = default;
                return false;
            }

            if (this.jsonElementBacking.TryGetProperty(name, out JsonElement element))
            {
#if NET8_0_OR_GREATER
                value = TValue.FromJson(element);
#else
                value = JsonValueNetStandard20Extensions.FromJsonElement<TValue>(element);
#endif

                return true;
            }

            value = default;
            return false;
        }

        if ((this.backing & Backing.Object) != 0)
        {
            if (this.objectBacking.TryGetValue(name, out JsonAny result))
            {
#if NET8_0_OR_GREATER
                value = TValue.FromAny(result);
#else
                value = result.As<TValue>();
#endif
                return true;
            }

            value = default;
            return false;
        }

        throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public bool TryGetProperty<TValue>(ReadOnlySpan<byte> name, out TValue value)
        where TValue : struct, IJsonValue<TValue>
    {
        if ((this.backing & Backing.JsonElement) != 0)
        {
            if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
            {
                value = default;
                return false;
            }

            if (this.jsonElementBacking.TryGetProperty(name, out JsonElement element))
            {
#if NET8_0_OR_GREATER
                value = TValue.FromJson(element);
#else
                value = JsonValueNetStandard20Extensions.FromJsonElement<TValue>(element);
#endif

                return true;
            }

            value = default;
            return false;
        }

        if ((this.backing & Backing.Object) != 0)
        {
            if (this.objectBacking.TryGetValue(name, out JsonAny result))
            {
#if NET8_0_OR_GREATER
                value = TValue.FromAny(result);
#else
                value = result.As<TValue>();
#endif
                return true;
            }

            value = default;
            return false;
        }

        throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public RealmPropertySchema SetProperty<TValue>(in JsonPropertyName name, TValue value)
        where TValue : struct, IJsonValue
    {
        return new(__CorvusObjectHelpers.GetPropertyBackingWith(this, name, value.AsAny));
    }

    /// <inheritdoc />
    public RealmPropertySchema RemoveProperty(in JsonPropertyName name)
    {
        return new(__CorvusObjectHelpers.GetPropertyBackingWithout(this, name));
    }

    /// <inheritdoc />
    public RealmPropertySchema RemoveProperty(string name)
    {
        return new(__CorvusObjectHelpers.GetPropertyBackingWithout(this, name));
    }

    /// <inheritdoc />
    public RealmPropertySchema RemoveProperty(ReadOnlySpan<char> name)
    {
        return new(__CorvusObjectHelpers.GetPropertyBackingWithout(this, name));
    }

    /// <inheritdoc />
    public RealmPropertySchema RemoveProperty(ReadOnlySpan<byte> name)
    {
        return new(__CorvusObjectHelpers.GetPropertyBackingWithout(this, name));
    }

    /// <summary>
    /// Provides UTF8 and string versions of the JSON property names on the object.
    /// </summary>
    public static class JsonPropertyNames
    {
        /// <summary>
        /// Gets the JSON property name for <see cref="SchemaVersion"/>.
        /// </summary>
        public const string SchemaVersion = "$schema_version";

        /// <summary>
        /// Gets the JSON property name for <see cref="AdditionalProperties"/>.
        /// </summary>
        public const string AdditionalProperties = "additionalProperties";

        /// <summary>
        /// Gets the JSON property name for <see cref="Groups"/>.
        /// </summary>
        public const string Groups = "groups";

        /// <summary>
        /// Gets the JSON property name for <see cref="Namespace"/>.
        /// </summary>
        public const string Namespace = "namespace";

        /// <summary>
        /// Gets the JSON property name for <see cref="Properties"/>.
        /// </summary>
        public const string Properties = "properties";

        /// <summary>
        /// Gets the JSON property name for <see cref="SchemaVersion"/>.
        /// </summary>
        public static ReadOnlySpan<byte> SchemaVersionUtf8 => "$schema_version"u8;

        /// <summary>
        /// Gets the JSON property name for <see cref="AdditionalProperties"/>.
        /// </summary>
        public static ReadOnlySpan<byte> AdditionalPropertiesUtf8 => "additionalProperties"u8;

        /// <summary>
        /// Gets the JSON property name for <see cref="Groups"/>.
        /// </summary>
        public static ReadOnlySpan<byte> GroupsUtf8 => "groups"u8;

        /// <summary>
        /// Gets the JSON property name for <see cref="Namespace"/>.
        /// </summary>
        public static ReadOnlySpan<byte> NamespaceUtf8 => "namespace"u8;

        /// <summary>
        /// Gets the JSON property name for <see cref="Properties"/>.
        /// </summary>
        public static ReadOnlySpan<byte> PropertiesUtf8 => "properties"u8;
    }

    private static class __CorvusObjectHelpers
    {
        /// <summary>
        /// Builds an <see cref="ImmutableList{JsonObjectProperty}"/> from the object.
        /// </summary>
        /// <returns>An immutable list of <see cref="JsonAny"/> built from the object.</returns>
        /// <exception cref="InvalidOperationException">The value is not an object.</exception>
        public static ImmutableList<JsonObjectProperty> GetPropertyBacking(in RealmPropertySchema that)
        {
            if ((that.backing & Backing.Object) != 0)
            {
                return that.objectBacking;
            }

            if ((that.backing & Backing.JsonElement) != 0)
            {
                return PropertyBackingBuilders.GetPropertyBackingBuilder(that.jsonElementBacking).ToImmutable();
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Builds an <see cref="ImmutableList{JsonObjectProperty}"/> from the object, without a specific property.
        /// </summary>
        /// <returns>An immutable list of <see cref="JsonObjectProperty"/>, built from the existing object, without the given property.</returns>
        /// <exception cref="InvalidOperationException">The value is not an object.</exception>
        public static ImmutableList<JsonObjectProperty> GetPropertyBackingWithout(in RealmPropertySchema that, in JsonPropertyName name)
        {
            if ((that.backing & Backing.Object) != 0)
            {
                return that.objectBacking.Remove(name);
            }

            if ((that.backing & Backing.JsonElement) != 0)
            {
                return PropertyBackingBuilders.GetPropertyBackingBuilderWithout(that.jsonElementBacking, name).ToImmutable();
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Builds an <see cref="ImmutableList{JsonObjectProperty}"/> from the object, without a specific property.
        /// </summary>
        /// <returns>An immutable list of <see cref="JsonObjectProperty"/>, built from the existing object, without the given property.</returns>
        /// <exception cref="InvalidOperationException">The value is not an object.</exception>
        public static ImmutableList<JsonObjectProperty> GetPropertyBackingWithout(in RealmPropertySchema that, ReadOnlySpan<char> name)
        {
            if ((that.backing & Backing.Object) != 0)
            {
                return that.objectBacking.Remove(name);
            }

            if ((that.backing & Backing.JsonElement) != 0)
            {
                return PropertyBackingBuilders.GetPropertyBackingBuilderWithout(that.jsonElementBacking, name).ToImmutable();
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Builds an <see cref="ImmutableList{JsonObjectProperty}"/> from the object, without a specific property.
        /// </summary>
        /// <returns>An immutable list of <see cref="JsonObjectProperty"/>, built from the existing object, without the given property.</returns>
        /// <exception cref="InvalidOperationException">The value is not an object.</exception>
        public static ImmutableList<JsonObjectProperty> GetPropertyBackingWithout(in RealmPropertySchema that, ReadOnlySpan<byte> name)
        {
            if ((that.backing & Backing.Object) != 0)
            {
                return that.objectBacking.Remove(name);
            }

            if ((that.backing & Backing.JsonElement) != 0)
            {
                return PropertyBackingBuilders.GetPropertyBackingBuilderWithout(that.jsonElementBacking, name).ToImmutable();
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Builds an <see cref="ImmutableList{JsonObjectProperty}"/> from the object, without a specific property.
        /// </summary>
        /// <returns>An immutable list of <see cref="JsonObjectProperty"/>, built from the existing object, without the given property.</returns>
        /// <exception cref="InvalidOperationException">The value is not an object.</exception>
        public static ImmutableList<JsonObjectProperty> GetPropertyBackingWithout(in RealmPropertySchema that, string name)
        {
            if ((that.backing & Backing.Object) != 0)
            {
                return that.objectBacking.Remove(name);
            }

            if ((that.backing & Backing.JsonElement) != 0)
            {
                return PropertyBackingBuilders.GetPropertyBackingBuilderWithout(that.jsonElementBacking, name).ToImmutable();
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Builds an <see cref="ImmutableList{JsonObjectProperty}"/> from the object, without a specific property.
        /// </summary>
        /// <returns>An immutable list of <see cref="JsonObjectProperty"/>, built from the existing object, with the given property.</returns>
        /// <exception cref="InvalidOperationException">The value is not an object.</exception>
        public static ImmutableList<JsonObjectProperty> GetPropertyBackingWith(in RealmPropertySchema that, in JsonPropertyName name, in JsonAny value)
        {
            if ((that.backing & Backing.Object) != 0)
            {
                return that.objectBacking.SetItem(name, value);
            }

            if ((that.backing & Backing.JsonElement) != 0)
            {
                return PropertyBackingBuilders.GetPropertyBackingBuilderReplacing(that.jsonElementBacking, name, value).ToImmutable();
            }

            throw new InvalidOperationException();
        }
    }
}
