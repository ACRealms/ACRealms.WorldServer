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
    public readonly partial struct ValStringEntity
    {
        /// <summary>
        /// Gets the value as a <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.OneOf0Entity"/>.
        /// </summary>
        public ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.OneOf0Entity AsOneOf0Entity
        {
            get
            {
                return (ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.OneOf0Entity)this;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is a valid <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.OneOf0Entity"/>.
        /// </summary>
        public bool IsOneOf0Entity
        {
            get
            {
                return ((ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.OneOf0Entity)this).IsValid();
            }
        }

        /// <summary>
        /// Gets the value as a <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.OneOf0Entity"/>.
        /// </summary>
        /// <param name = "result">The result of the conversion.</param>
        /// <returns><c>True</c> if the conversion was valid.</returns>
        public bool TryGetAsOneOf0Entity(out ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.OneOf0Entity result)
        {
            result = (ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.OneOf0Entity)this;
            return result.IsValid();
        }

        /// <summary>
        /// Gets the value as a <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.Type1EntityArray"/>.
        /// </summary>
        public ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.Type1EntityArray AsType1EntityArray
        {
            get
            {
                return (ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.Type1EntityArray)this;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is a valid <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.Type1EntityArray"/>.
        /// </summary>
        public bool IsType1EntityArray
        {
            get
            {
                return ((ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.Type1EntityArray)this).IsValid();
            }
        }

        /// <summary>
        /// Gets the value as a <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.Type1EntityArray"/>.
        /// </summary>
        /// <param name = "result">The result of the conversion.</param>
        /// <returns><c>True</c> if the conversion was valid.</returns>
        public bool TryGetAsType1EntityArray(out ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.Type1EntityArray result)
        {
            result = (ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ValStringEntity.Type1EntityArray)this;
            return result.IsValid();
        }
    }
}