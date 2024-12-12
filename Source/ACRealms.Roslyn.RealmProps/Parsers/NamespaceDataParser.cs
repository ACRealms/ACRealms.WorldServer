using ACRealms.Roslyn.RealmProps.CompilerDomainModels;
using ACRealms.Roslyn.RealmProps.IntermediateModels;
using Corvus.Json;

namespace ACRealms.Roslyn.RealmProps.Parsers
{
    internal static class NamespaceDataParser
    {
        internal static IEnumerable<ObjPropInfo> GetProps(string namespaceRaw, RealmPropertySchema schema)
            => GetProps(namespaceRaw, schema.Properties).Concat(GetProps(namespaceRaw, schema.Groups));

        private static IEnumerable<ObjPropInfo> GetProps(string namespaceRaw, RealmPropertySchema.GroupArray? groupsDefinition)
        {
            if (groupsDefinition is null)
                return [];
            if (groupsDefinition.Value.IsNullOrUndefined())
                return [];
            return groupsDefinition.Value.SelectMany(group =>
            {
                PropType gPropType = PropType.None;
                if (group.Type.IsNotNullOrUndefined())
                    Enum.TryParse(group.Type.AsString.GetString()!, false, out gPropType);

                var gDefaults = new GroupDefaults(group.DescriptionFormat.GetString())
                {
                    KeyPrefix = group.KeyPrefix.IsNullOrUndefined() ? string.Empty : group.KeyPrefix.AsString.GetString()!,
                    KeySuffix = group.KeySuffix.IsNullOrUndefined() ? string.Empty : group.KeySuffix.AsString.GetString()!,
                    MinValue = GetNumericValue(gPropType, group.MinValue),
                    MaxValue = GetNumericValue(gPropType, group.MaxValue),
                    Default = GetLiteralValue(gPropType, group.Default, group.Enum.AsString.GetString()),
                    Enum = group.Enum.GetString(),
                    PropType = gPropType,
                    RerollRestrictedTo = group.RerollRestrictedTo.IsNotNullOrUndefined() ? group.RerollRestrictedTo.AsString.GetString() : null
                };

                RealmPropertySchema.ObjPropListForGroup props = group.Properties;
                if (props.IsArrayShortPropListForGroup)
                {
                    return props.AsArrayShortPropListForGroup.Select(shortKeyNode =>
                    {
                        string shortKey = shortKeyNode.GetString()!;
                        string key = $"{gDefaults.KeyPrefix}{shortKey}{gDefaults.KeySuffix}";

                        return new ObjPropInfo(namespaceRaw, key)
                        {
                            Description = gDefaults.GetDescriptionFromFormat(shortKey, null),
                            MinValue = gDefaults.MinValue,
                            MaxValue = gDefaults.MaxValue,
                            Default = gDefaults.Default,
                            Enum = gDefaults.Enum,
                            Type = gDefaults.PropType,
                            ObsoleteReason = gDefaults.ObsoleteReason,
                            RerollRestrictedTo = gDefaults.RerollRestrictedTo
                        };
                    });
                }
                else if (props.IsObjPropList)
                {
                    return props.AsObjPropList.Select(kvp =>
                    {
                        string shortKey = kvp.Key.GetString();
                        string key = $"{gDefaults.KeyPrefix}{shortKey}{gDefaults.KeySuffix}";

                        return GetProp(namespaceRaw, shortKey, kvp.Value, gDefaults);
                    });
                }
                else
                    throw new ArgumentException("Unhandled group props type");
            });
        }

        private static IEnumerable<ObjPropInfo> GetProps(string namespaceRaw, RealmPropertySchema.ObjPropList? propertiesDefinition)
        {
            if (!propertiesDefinition.HasValue || propertiesDefinition.Value.IsNullOrUndefined())
                return [];
            return propertiesDefinition.Value.Select(kvp => GetProp(namespaceRaw, kvp.Key.GetString(), kvp.Value));
        }

        private static string GetUnformattedDescription(RealmPropertySchema.DescriptionArray descriptionArray)
        {
            // This needs to eventually be handled with fixed line widths and proper wrapping
            string newline = $$"""

            """ + " ";
            return string.Join(newline, descriptionArray.Select(x => x.AsString.GetString()!));
        }

        private static string? GetUnformattedDescription(RealmPropertySchema.ValDescription? descriptionDef)
        {
            if (!descriptionDef.HasValue || descriptionDef.Value.IsNullOrUndefined())
                return null;
            if (descriptionDef.Value.AsString.GetString() is string d)
                return d;
            if (descriptionDef.Value.IsOneOf0Entity)
            {
                RealmPropertySchema.ValDescription.OneOf0Entity def = descriptionDef.Value.AsOneOf0Entity;
                if (def.IsValStringSimple)
                    return def.AsValStringSimple.AsString.GetString();
                if (def.IsDescriptionPattern)
                    return def.AsDescriptionPattern.AsString.GetString();
            }
            if (descriptionDef.Value.IsDescriptionArray)
                return GetUnformattedDescription(descriptionDef.Value.AsDescriptionArray);
            throw new ArgumentException("Unhandled description type in schema");
        }

        private static string GetFullDescription(string shortKey, RealmPropertySchema.ValDescription? descriptionDef, GroupDefaults? groupDefaults)
        {
            string? unformattedDescription = GetUnformattedDescription(descriptionDef);
            if (unformattedDescription == null)
            {
                if (groupDefaults == null || groupDefaults.DescriptionFormat == null)
                    return "No description (1)";
                unformattedDescription = groupDefaults.DescriptionFormat;
            }
            if (groupDefaults == null)
                return unformattedDescription;

            return groupDefaults!.GetDescriptionFromFormat(shortKey, unformattedDescription);
        }

        private static string? GetNumericValue(PropType type, RealmPropertySchema.ValFloat? numericValueSrc)
        {
            if (!numericValueSrc.HasValue || numericValueSrc.Value.IsNullOrUndefined())
                return null;

            string s = numericValueSrc.ToString();
            return GetSanitizedLiteralValue(type, s);
        }

        private static string GetSanitizedIntValue(string unsanitizedValue)
        {
            if (!int.TryParse(unsanitizedValue, out int intResult))
                throw new ArgumentException($"Invalid integer value ({unsanitizedValue})");
            return intResult.ToString();
        }

        private static string GetSanitizedLongValue(string unsanitizedValue)
        {
            if (!long.TryParse(unsanitizedValue, out long longResult))
                throw new ArgumentException($"Invalid int64 value ({unsanitizedValue})");
            return longResult.ToString();
        }

        private static string GetSanitizedDoubleValue(string unsanitizedValue)
        {
            if (!double.TryParse(unsanitizedValue, out double doubleResult))
                throw new ArgumentException($"Invalid numeric (double) value ({unsanitizedValue})");
            return doubleResult.ToString();
        }

        private static string GetSanitizedBoolValue(string unsanitizedValue)
        {
            return unsanitizedValue switch
            {
                "false" => "false",
                "true" => "true",
                _ => throw new ArgumentException($"Invalid boolean value {unsanitizedValue}")
            };
        }

        private static string GetSanitizedLiteralValue(PropType type, string unsanitizedValue, string? enumType = null)
        {
            string? result = type switch
            {
                PropType.integer => GetSanitizedIntValue(unsanitizedValue),
                PropType.int64 => GetSanitizedLongValue(unsanitizedValue),
                PropType.@float => GetSanitizedDoubleValue(unsanitizedValue),
                PropType.boolean => GetSanitizedBoolValue(unsanitizedValue),
                PropType.@string => unsanitizedValue,
                PropType.@enum => $"{enumType}.{unsanitizedValue.Replace("\"", "")}",
                _ => throw new ArgumentException($"Invalid PropType ({type})")
            };
            return result;
        }

        private static string? GetLiteralValue(PropType type, JsonAny? valueSrc, string? enumType = null)
        {
            if (!valueSrc.HasValue || valueSrc.Value.IsNullOrUndefined())
                return null;
            string? s = valueSrc.Value.ValueKind switch
            {
                System.Text.Json.JsonValueKind.Undefined => null,
                System.Text.Json.JsonValueKind.True => "true",
                System.Text.Json.JsonValueKind.Null => "null",
                System.Text.Json.JsonValueKind.Number => valueSrc.Value.ToString(),
                System.Text.Json.JsonValueKind.False => "false",
                System.Text.Json.JsonValueKind.String => valueSrc.Value.ToString(),
                System.Text.Json.JsonValueKind.Array => type switch
                {
                    PropType.@string => string.Join("", valueSrc.Value.AsAny.AsArray.Select(s => s.AsString.GetString())),
                    _ => throw new ArgumentException("unexpected array")
                },
                _ => throw new ArgumentException($"Unexpected type {valueSrc.Value.ValueKind}")
            };
            return GetSanitizedLiteralValue(type, s!, enumType);
        }

        private static ObjPropInfo GetProp(string namespaceRaw, string shortKey, RealmPropertySchema.PropDef propDef, GroupDefaults? groupDefaults = null)
        {
            string key;
            if (groupDefaults == null)
                key = shortKey;
            else
                key = $"{groupDefaults.KeyPrefix}{shortKey}{groupDefaults.KeySuffix}";

            if (propDef.IsValDescription)
            {
                if (groupDefaults == null)
                    throw new ArgumentException("Prop from string short description definition only allowed within a group!");

                string? description = GetUnformattedDescription(propDef.AsValDescription);
                description = groupDefaults.GetDescriptionFromFormat(shortKey, description);

                return new ObjPropInfo(namespaceRaw, key)
                {
                    Type = groupDefaults.PropType!,
                    Description = description,
                    Default = groupDefaults.Default,
                    Enum = groupDefaults.Enum,
                    MinValue = groupDefaults.MinValue,
                    MaxValue = groupDefaults.MaxValue,
                    ObsoleteReason = groupDefaults.ObsoleteReason,
                    RerollRestrictedTo = groupDefaults.RerollRestrictedTo,
                };
            }
            else if (propDef.IsObjProp)
            {
                RealmPropertySchema.ObjProp prop = propDef.AsObjProp;
                Enum.TryParse(prop.Type.GetString()!, false, out PropType propType);

                string? unformattedDesc = GetUnformattedDescription(prop.Description);

                string desc = groupDefaults?.GetDescriptionFromFormat(shortKey, unformattedDesc) ?? unformattedDesc;
                PropType type = propType == PropType.None ? groupDefaults!.PropType : propType;
                string? enumType = prop.Enum.GetString() ?? groupDefaults?.Enum;

                return new ObjPropInfo(namespaceRaw, key)
                {
                    Description = desc!,
                    MinValue = GetNumericValue(propType, prop.MinValue) ?? groupDefaults?.MinValue,
                    MaxValue = GetNumericValue(propType, prop.MaxValue) ?? groupDefaults?.MaxValue,
                    Default = GetLiteralValue(propType, prop.Default, enumType) ?? groupDefaults?.Default,
                    Enum = enumType,
                    DefaultFromServerProp = prop.DefaultFromServerProperty.GetString(),
                    ObsoleteReason = prop.Obsolete.GetString(),
                    RerollRestrictedTo = prop.RerollRestrictedTo.IsNotNullOrUndefined() ? prop.RerollRestrictedTo.AsString.GetString() : null,
                    Type = type
                };
            }
            else
                throw new ArgumentException("Unhandled obj prop type");
        }
    }
}
