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
    public readonly partial struct ObjPropOrGroupEntity
    {
        /// <summary>
        /// Generated from JSON Schema.
        /// </summary>
        public readonly partial struct AllOf1Entity
        {
            /// <summary>
            /// Conversion to <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf1Entity.RequiredEnum"/>.
            /// </summary>
            /// <param name = "value">The value from which to convert.</param>
            public static explicit operator ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf1Entity.RequiredEnum(AllOf1Entity value)
            {
                if ((value.backing & Backing.JsonElement) != 0)
                {
                    return new(value.AsJsonElement);
                }

                if ((value.backing & Backing.Object) != 0)
                {
                    return new(value.objectBacking);
                }

                return ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf1Entity.RequiredEnum.Undefined;
            }

            /// <summary>
            /// Conversion from <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf1Entity.RequiredEnum"/>.
            /// </summary>
            /// <param name = "value">The value from which to convert.</param>
            public static explicit operator AllOf1Entity(ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf1Entity.RequiredEnum value)
            {
                if (value.HasJsonElementBacking)
                {
                    return new(value.AsJsonElement);
                }

                return value.ValueKind switch
                {
                    JsonValueKind.Object => new(value.AsPropertyBacking()),
                    _ => Undefined
                };
            }
        }
    }
}