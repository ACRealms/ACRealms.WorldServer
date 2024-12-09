//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable enable

using System.Runtime.CompilerServices;
using System.Text.Json;
using Corvus.Json;

namespace ACRealms.Roslyn.RealmProps.IntermediateModels;

/// <summary>
/// Generated from JSON Schema.
/// </summary>
public readonly partial struct RealmPropertySchema
{
    /// <summary>
    /// Generated from JSON Schema.
    /// </summary>
    /// <remarks>
    /// <para>
    /// list of string fragments to be concatenated together to allow json string to be broken up into multiple lines in the editor
    /// </para>
    /// </remarks>
    public readonly partial struct ValStringArray
    {
        /// <inheritdoc/>
        public ValidationContext Validate(in ValidationContext validationContext, ValidationLevel level = ValidationLevel.Flag)
        {
            ValidationContext result = validationContext;
            if (level > ValidationLevel.Flag && !result.IsUsingResults)
            {
                result = result.UsingResults();
            }

            if (level > ValidationLevel.Basic)
            {
                if (!result.IsUsingStack)
                {
                    result = result.UsingStack();
                }

                result = result.PushSchemaLocation("https://realm.ac/schema/v1/realm-property-schema.json#/definitions/valStringArray");
            }

            JsonValueKind valueKind = this.ValueKind;

            result = CorvusValidation.TypeValidationHandler(valueKind, result, level);

            if (level == ValidationLevel.Flag && !result.IsValid)
            {
                return result;
            }

            result = CorvusValidation.ArrayValidationHandler(this, valueKind, result, level);

            if (level == ValidationLevel.Flag && !result.IsValid)
            {
                return result;
            }

            if (level > ValidationLevel.Basic)
            {
                result = result.PopLocation();
            }

            return result;
        }

        /// <summary>
        /// Validation constants for the type.
        /// </summary>
        public static partial class CorvusValidation
        {
            /// <summary>
            /// A constant for the <c>maxItems</c> keyword.
            /// </summary>
            public static readonly long MaxItems = 320;

            /// <summary>
            /// Core type validation.
            /// </summary>
            /// <param name="valueKind">The <see cref="JsonValueKind" /> of the value to validate.</param>
            /// <param name="validationContext">The current validation context.</param>
            /// <param name="level">The current validation level.</param>
            /// <returns>The resulting validation context after validation.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static ValidationContext TypeValidationHandler(
                JsonValueKind valueKind,
                in ValidationContext validationContext,
                ValidationLevel level = ValidationLevel.Flag)
            {
                return Corvus.Json.ValidateWithoutCoreType.TypeArray(valueKind, validationContext, level, "type");
            }

            /// <summary>
            /// Array validation.
            /// </summary>
            /// <param name="value">The value to validate.</param>
            /// <param name="valueKind">The <see cref="JsonValueKind" /> of the value to validate.</param>
            /// <param name="validationContext">The current validation context.</param>
            /// <param name="level">The current validation level.</param>
            /// <returns>The resulting validation context after validation.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static ValidationContext ArrayValidationHandler(
                in ValStringArray value,
                JsonValueKind valueKind,
                in ValidationContext validationContext,
                ValidationLevel level)
            {
                ValidationContext result = validationContext;
                if (valueKind != JsonValueKind.Array)
                {
                    if (level == ValidationLevel.Verbose)
                    {
                        ValidationContext ignoredResult = validationContext;
                        ignoredResult = ignoredResult.WithResult(isValid: true, "Validation items - ignored because the value is not an array", "items");
                        ignoredResult = ignoredResult.WithResult(isValid: true, "Validation maxItems - ignored because the value is not an array", "maxItems");

                        return ignoredResult;
                    }

                    return validationContext;
                }

                int length = 0;
                using JsonArrayEnumerator<ACRealms.Roslyn.RealmProps.IntermediateModels.RealmPropertySchema.ValStringArray.ValStringArrayEntity> arrayEnumerator = value.EnumerateArray();
                while (arrayEnumerator.MoveNext())
                {
                    if (level > ValidationLevel.Basic)
                    {
                        result = result.PushDocumentArrayIndex(length);
                    }
                    if (level > ValidationLevel.Basic)
                    {
                        result = result.PushValidationLocationReducedPathModifier(new("#/items"));
                    }

                    var nonTupleItemsResult = arrayEnumerator.Current.Validate(result.CreateChildContext(), level);
                    if (level == ValidationLevel.Flag && !nonTupleItemsResult.IsValid)
                    {
                        return nonTupleItemsResult;
                    }

                    result = result.MergeResults(nonTupleItemsResult.IsValid, level, nonTupleItemsResult);
                    if (level > ValidationLevel.Basic)
                    {
                        result = result.PopLocation();
                    }

                    result = result.WithLocalItemIndex(length);

                    if (level > ValidationLevel.Basic)
                    {
                        result = result.PopLocation();
                    }

                    length++;
                }

                if (length <= MaxItems)
                {
                    if (level == ValidationLevel.Verbose)
                    {
                        result = result.WithResult(isValid: true, $"Validation maxItems - array of length {length} is less than or equal to {MaxItems}", "maxItems");
                    }
                }
                else
                {
                    if (level == ValidationLevel.Flag)
                    {
                        return ValidationContext.InvalidContext;
                    }
                    else if (level >= ValidationLevel.Detailed)
                    {
                        result = result.WithResult(isValid: false, $"Validation maxItems - array of length {length} is greater than {MaxItems}", "maxItems");
                    }
                    else
                    {
                        result = result.WithResult(isValid: false, "Validation maxItems - is greater than the required length.", "maxItems");
                    }
                }

                return result;
            }
        }
    }
}
