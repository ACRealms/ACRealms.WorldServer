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
    /// <summary>
    /// Generated from JSON Schema.
    /// </summary>
    public readonly partial struct PropDefEntity
    {
        /// <summary>
        /// Gets the value as a <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropEntity"/>.
        /// </summary>
        public ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropEntity AsObjPropEntity
        {
            get
            {
                return (ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropEntity)this;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is a valid <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropEntity"/>.
        /// </summary>
        public bool IsObjPropEntity
        {
            get
            {
                return ((ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropEntity)this).IsValid();
            }
        }

        /// <summary>
        /// Gets the value as a <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropEntity"/>.
        /// </summary>
        /// <param name = "result">The result of the conversion.</param>
        /// <returns><c>True</c> if the conversion was valid.</returns>
        public bool TryGetAsObjPropEntity(out ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropEntity result)
        {
            result = (ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropEntity)this;
            return result.IsValid();
        }

        /// <summary>
        /// Gets the value as a <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity"/>.
        /// </summary>
        public ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity AsValStringEntity
        {
            get
            {
                return (ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity)this;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is a valid <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity"/>.
        /// </summary>
        public bool IsValStringEntity
        {
            get
            {
                return ((ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity)this).IsValid();
            }
        }

        /// <summary>
        /// Gets the value as a <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity"/>.
        /// </summary>
        /// <param name = "result">The result of the conversion.</param>
        /// <returns><c>True</c> if the conversion was valid.</returns>
        public bool TryGetAsValStringEntity(out ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity result)
        {
            result = (ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity)this;
            return result.IsValid();
        }
    }
}