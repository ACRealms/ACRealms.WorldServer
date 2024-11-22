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
using System.Runtime.CompilerServices;
using System.Text.Json;
using Corvus.Json;
using Corvus.Json.Internal;

namespace ACRealms.RealmProps.IntermediateModels;
public readonly partial struct RealmPropertySchema
{
    /// <summary>
    /// Generated from JSON Schema.
    /// </summary>
    public readonly partial struct ObjPropListForGroupEntity
    {
        /// <summary>
        /// Matches the value against each of the any of values, and returns the result of calling the provided match function for the first match found.
        /// </summary>
        /// <param name = "context">The context to pass to the match function.</param>
        /// <param name = "match0">The function to call if the value matches the <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropListEntity"/> type.</param>
        /// <param name = "match1">The function to call if the value matches the <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.PropNameEntityArray"/> type.</param>
        /// <param name = "defaultMatch">The fallback match.</param>
        public TOut Match<TIn, TOut>(in TIn context, Matcher<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropListEntity, TIn, TOut> match0, Matcher<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.PropNameEntityArray, TIn, TOut> match1, Matcher<ObjPropListForGroupEntity, TIn, TOut> defaultMatch)
        {
            var oneOf0 = this.As<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropListEntity>();
            if (oneOf0.IsValid())
            {
                return match0(oneOf0, context);
            }

            var oneOf1 = this.As<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.PropNameEntityArray>();
            if (oneOf1.IsValid())
            {
                return match1(oneOf1, context);
            }

            return defaultMatch(this, context);
        }

        /// <summary>
        /// Matches the value against each of the any of values, and returns the result of calling the provided match function for the first match found.
        /// </summary>
        /// <param name = "match0">The function to call if the value matches the <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropListEntity"/> type.</param>
        /// <param name = "match1">The function to call if the value matches the <see cref = "ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.PropNameEntityArray"/> type.</param>
        /// <param name = "defaultMatch">The fallback match.</param>
        public TOut Match<TOut>(Matcher<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropListEntity, TOut> match0, Matcher<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.PropNameEntityArray, TOut> match1, Matcher<ObjPropListForGroupEntity, TOut> defaultMatch)
        {
            var oneOf0 = this.As<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropListEntity>();
            if (oneOf0.IsValid())
            {
                return match0(oneOf0);
            }

            var oneOf1 = this.As<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.PropNameEntityArray>();
            if (oneOf1.IsValid())
            {
                return match1(oneOf1);
            }

            return defaultMatch(this);
        }
    }
}