//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using Corvus.Json;

namespace ACRealms.RealmProps.IntermediateModels;
public readonly partial struct RealmPropertySchema
{
    public readonly partial struct ObjPropOrGroupEntity
    {
        /// <summary>
        /// Generated from JSON Schema.
        /// </summary>
        public readonly partial struct AllOf3Entity
        {
            /// <summary>
            /// Gets the value as a <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf3Entity.ThenEntity"/>.
            /// </summary>
            public ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf3Entity.ThenEntity AsThenEntity
            {
                get
                {
                    return (ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf3Entity.ThenEntity)this;
                }
            }

            /// <summary>
            /// Gets a value indicating whether this is a valid <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf3Entity.ThenEntity"/>.
            /// </summary>
            public bool IsThenEntity
            {
                get
                {
                    return ((ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf3Entity.ThenEntity)this).IsValid();
                }
            }

            /// <summary>
            /// Gets the value as a <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf3Entity.ThenEntity"/>.
            /// </summary>
            /// <param name = "result">The result of the conversion.</param>
            /// <returns><c>True</c> if the conversion was valid.</returns>
            public bool TryGetAsThenEntity(out ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf3Entity.ThenEntity result)
            {
                result = (ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf3Entity.ThenEntity)this;
                return result.IsValid();
            }
        }
    }
}