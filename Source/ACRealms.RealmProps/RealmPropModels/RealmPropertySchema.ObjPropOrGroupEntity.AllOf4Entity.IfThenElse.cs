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
    public readonly partial struct ObjPropOrGroupEntity
    {
        /// <summary>
        /// Generated from JSON Schema.
        /// </summary>
        public readonly partial struct AllOf4Entity
        {
            /// <summary>
            /// Gets a value indicating whether this matches an If/Then case, return the value as the type defined for the then case.
            /// </summary>
            /// <param name = "result">This value cast to the 'then' type, when the 'if' schema matches, otherwise an Undefined instance of the 'then' type.</param>
            /// <returns><c>True</c> if the 'if' case matched, and the 'else' instance was provided.</returns>
            public bool MatchIfWithTypeFloat(out ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf4Entity.ThenEntity result)
            {
                if (this.As<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf4Entity.WithTypeFloat>().IsValid())
                {
                    result = this.As<ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf4Entity.ThenEntity>();
                    return true;
                }

                result = ACRealms.RealmProps.IntermediateModels.RealmPropertySchema.ObjPropOrGroupEntity.AllOf4Entity.ThenEntity.Undefined;
                return false;
            }
        }
    }
}