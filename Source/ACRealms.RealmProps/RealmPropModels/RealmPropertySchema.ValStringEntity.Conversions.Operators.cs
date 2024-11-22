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

namespace ACRealms.RealmProps.IntermediateModels;
public readonly partial struct RealmPropertySchema
{
    /// <summary>
    /// Generated from JSON Schema.
    /// </summary>
    public readonly partial struct ValStringEntity
    {
        /// <summary>
        /// Conversion to <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.OneOf0Entity"/>.
        /// </summary>
        /// <param name = "value">The value from which to convert.</param>
        public static explicit operator ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.OneOf0Entity(ValStringEntity value)
        {
            if ((value.backing & Backing.JsonElement) != 0)
            {
                return new(value.AsJsonElement);
            }

            if ((value.backing & Backing.String) != 0)
            {
                return new(value.stringBacking);
            }

            return ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.OneOf0Entity.Undefined;
        }

        /// <summary>
        /// Conversion from <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.OneOf0Entity"/>.
        /// </summary>
        /// <param name = "value">The value from which to convert.</param>
        public static implicit operator ValStringEntity(ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.OneOf0Entity value)
        {
            if (value.HasJsonElementBacking)
            {
                return new(value.AsJsonElement);
            }

            return value.ValueKind switch
            {
                JsonValueKind.String => new((string)value),
                _ => Undefined
            };
        }

        /// <summary>
        /// Conversion to <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.Type1EntityArray"/>.
        /// </summary>
        /// <param name = "value">The value from which to convert.</param>
        public static explicit operator ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.Type1EntityArray(ValStringEntity value)
        {
            if ((value.backing & Backing.JsonElement) != 0)
            {
                return new(value.AsJsonElement);
            }

            if ((value.backing & Backing.Array) != 0)
            {
                return new(value.arrayBacking);
            }

            return ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.Type1EntityArray.Undefined;
        }

        /// <summary>
        /// Conversion from <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.Type1EntityArray"/>.
        /// </summary>
        /// <param name = "value">The value from which to convert.</param>
        public static implicit operator ValStringEntity(ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.Type1EntityArray value)
        {
            if (value.HasJsonElementBacking)
            {
                return new(value.AsJsonElement);
            }

            return value.ValueKind switch
            {
                JsonValueKind.Array => new(value.AsImmutableList()),
                _ => Undefined
            };
        }
    }
}