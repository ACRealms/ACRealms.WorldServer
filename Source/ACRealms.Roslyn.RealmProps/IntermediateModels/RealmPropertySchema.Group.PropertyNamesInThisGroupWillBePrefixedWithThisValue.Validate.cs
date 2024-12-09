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
    public readonly partial struct Group
    {
        /// <summary>
        /// Generated from JSON Schema.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Property names in this group will be prefixed with this value
        /// </para>
        /// </remarks>
        public readonly partial struct PropertyNamesInThisGroupWillBePrefixedWithThisValue
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

                    result = result.PushSchemaLocation("https://realm.ac/schema/v1/realm-property-schema.json#/definitions/group/properties/key_prefix");
                }

                JsonValueKind valueKind = this.ValueKind;

                result = CorvusValidation.StringValidationHandler(this, valueKind, result, level);

                if (level == ValidationLevel.Flag && !result.IsValid)
                {
                    return result;
                }

                result = CorvusValidation.CompositionAllOfValidationHandler(this, result, level);

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
                /// A constant for the <c>maxLength</c> keyword.
                /// </summary>
                public static readonly long MaxLength = 30;

                /// <summary>
                /// String validation.
                /// </summary>
                /// <param name="value">The value to validate.</param>
                /// <param name="valueKind">The <see cref="JsonValueKind" /> of the value to validate.</param>
                /// <param name="validationContext">The current validation context.</param>
                /// <param name="level">The current validation level.</param>
                /// <returns>The resulting validation context after validation.</returns>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                internal static ValidationContext StringValidationHandler(
                    in PropertyNamesInThisGroupWillBePrefixedWithThisValue value,
                    JsonValueKind valueKind,
                    in ValidationContext validationContext,
                    ValidationLevel level = ValidationLevel.Flag)
                {
                    if (valueKind != JsonValueKind.String)
                    {
                        if (level == ValidationLevel.Verbose)
                        {
                            ValidationContext ignoredResult = validationContext;
                            ignoredResult = ignoredResult.WithResult(isValid: true, "Validation maxLength - ignored because the value is not a string", "maxLength");

                            return ignoredResult;
                        }

                        return validationContext;
                    }

                    ValidationContext result = validationContext;
                    value.AsString.TryGetValue(StringValidator, new Corvus.Json.Validate.ValidationContextWrapper(result, level), out result);

                    return result;

                    static bool StringValidator(ReadOnlySpan<char> input, in Corvus.Json.Validate.ValidationContextWrapper context, out ValidationContext result)
                    {
                        int length = Corvus.Json.Validate.CountRunes(input);
                        result = context.Context;

                        if (length <= MaxLength)
                        {
                            if (context.Level == ValidationLevel.Verbose)
                            {
                                result = result.WithResult(isValid: true, validationLocationReducedPathModifier: new JsonReference("maxLength"), $"Validation maxLength - {input.ToString()} of {length} is less than or equal to {MaxLength}");
                            }
                        }
                        else
                        {
                            if (context.Level == ValidationLevel.Flag)
                            {
                                result = context.Context.WithResult(isValid: false);
                                return true;
                            }
                            else if (context.Level >= ValidationLevel.Detailed)
                            {
                                result = result.WithResult(isValid: false, validationLocationReducedPathModifier: new JsonReference("maxLength"), $"Validation maxLength - {input.ToString()} of {length} is greater than {MaxLength}");
                            }
                            else
                            {
                                result = result.WithResult(isValid: false, validationLocationReducedPathModifier: new JsonReference("maxLength"), "Validation maxLength - is greater than the required length.");
                            }
                        }

                        return true;
                    }
                }

                /// <summary>
                /// Composition validation (all-of).
                /// </summary>
                /// <param name="value">The value to validate.</param>
                /// <param name="validationContext">The current validation context.</param>
                /// <param name="level">The current validation level.</param>
                /// <returns>The resulting validation context after validation.</returns>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                internal static ValidationContext CompositionAllOfValidationHandler(
                    in PropertyNamesInThisGroupWillBePrefixedWithThisValue value,
                    in ValidationContext validationContext,
                    ValidationLevel level = ValidationLevel.Flag)
                {
                    ValidationContext result = validationContext;
                    ValidationContext childContextBase = result;

                    ValidationContext allOfResult0 = childContextBase.CreateChildContext();
                    if (level > ValidationLevel.Basic)
                    {
                        allOfResult0 = allOfResult0.PushValidationLocationReducedPathModifier(new("#/allOf/0/$ref"));
                    }

                    allOfResult0 = value.As<ACRealms.Roslyn.RealmProps.IntermediateModels.RealmPropertySchema.PropName>().Validate(allOfResult0, level);

                    if (!allOfResult0.IsValid)
                    {
                        if (level >= ValidationLevel.Basic)
                        {
                            result = result.MergeChildContext(allOfResult0, true).PushValidationLocationProperty("allOf").WithResult(isValid: false, "Validation - allOf failed to validate against the schema.").PopLocation();
                        }
                        else
                        {
                            result = result.MergeChildContext(allOfResult0, false).WithResult(isValid: false);
                            return result;
                        }
                    }
                    else
                    {
                        result = result.MergeChildContext(allOfResult0, level >= ValidationLevel.Detailed);
                    }

                    return result;
                }
            }
        }
    }
}
