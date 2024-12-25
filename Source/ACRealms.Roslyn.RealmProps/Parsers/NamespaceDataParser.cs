using ACRealms.Roslyn.RealmProps.CompilerDomainModels;
using ACRealms.Roslyn.RealmProps.IntermediateModels;
using Corvus.Json;

namespace ACRealms.Roslyn.RealmProps.Parsers
{
    internal static class NamespaceDataParser
    {
        internal static IEnumerable<ObjPropInfo> GetProps(string namespaceRaw, PropDefs schema)
            => GetProps(namespaceRaw, schema.Properties).Concat(GetProps(namespaceRaw, schema.Groups));

        private static IEnumerable<ObjPropInfo> GetProps(string namespaceRaw, PropDefs.GroupArray? groupsDefinition)
        {
            if (groupsDefinition is null)
                return [];
            if (groupsDefinition.Value.IsNullOrUndefined())
                return [];
            return groupsDefinition.Value.SelectMany(group =>
            {
                if (!group.Properties.HasValue)
                    return [];

                PropType gPropType = PropType.None;
                var groupExt = group.As<Group.Extended>();

                if (!Enum.TryParse(group.Type.AsString.GetString()!, false, out gPropType))
                    throw new Exception("Missing required type for group");

                var groupMinMax = groupExt.As<PropDefExtensionMinMax>();
                var groupEnum = groupExt.As<Group.Extended.GroupAttrs.Typed.Enum>();

                var gDefaults = new GroupDefaults(group.DescriptionFormat?.GetString())
                {
                    KeyPrefix = group.KeyPrefix?.GetString() ?? string.Empty,
                    KeySuffix = group.KeySuffix?.GetString() ?? string.Empty,
                    MinValue = GetNumericValue(gPropType, groupMinMax.MinValue),
                    MaxValue = GetNumericValue(gPropType, groupMinMax.MaxValue),
                    Default = GetLiteralValue(gPropType, groupMinMax.Default, groupEnum.EnumValue?.AsString.GetString()),
                    Enum = groupEnum.EnumValue?.AsString.GetString(),
                    PropType = gPropType,
                    RerollRestrictedTo = groupMinMax.RerollRestrictedTo?.GetString()
                };

                var props = group.Properties.Value;
                if (props.IsArrayShortPropList)
                {
                    return props.AsArrayShortPropList.Select(shortKeyNode =>
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
                else if (props.IsGroupPropsObj)
                {
                    return props.AsGroupPropsObj.Select(kvp =>
                    {
                        string shortKey = kvp.Key.GetString();
                        string key = $"{gDefaults.KeyPrefix}{shortKey}{gDefaults.KeySuffix}";
                        var groupProp = kvp.Value;
                        return GetProp(namespaceRaw, shortKey, groupProp, gDefaults);
                    });
                }
                else
                    throw new ArgumentException("Unhandled group props type");
            });
        }

        private static IEnumerable<ObjPropInfo> GetProps(string namespaceRaw, Props? propertiesDefinition)
        {
            if (!propertiesDefinition.HasValue || propertiesDefinition.Value.IsNullOrUndefined())
                return [];
            return propertiesDefinition.Value.Select(kvp => GetProp(namespaceRaw, kvp.Key.GetString(), kvp.Value));
        }

        private static string GetUnformattedDescription(DescriptionArray descriptionArray)
        {
            // This needs to eventually be handled with fixed line widths and proper wrapping
            string newline = $$"""

            """ + " ";
            return string.Join(newline, descriptionArray.Select(x => x.AsString.GetString()!));
        }

        private static string? GetUnformattedDescription(Description? descriptionDef)
        {
            if (!descriptionDef.HasValue || descriptionDef.Value.IsNullOrUndefined())
                return null;
            var desc = descriptionDef.Value;
            if (desc.IsStringForm)
            {
                var def = descriptionDef.Value.AsStringForm;
                return def.AsString.GetString();
                /*if (def.IsValStringSimple)
                    return def.AsValStringSimple.AsString.GetString();
                if (def.IsDescriptionPattern)
                    return def.AsDescriptionPattern.AsString.GetString();*/
            }
            if (desc.IsDescriptionArray)
                return GetUnformattedDescription(desc.AsDescriptionArray);
            throw new ArgumentException("Unhandled description type in schema");
        }

        private static string GetFullDescription(string shortKey, Description? descriptionDef, GroupDefaults? groupDefaults)
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

        private static string? GetNumericValue(PropType type, ValFloat? numericValueSrc)
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
                JsonValueKind.Undefined => null,
                JsonValueKind.True => "true",
                JsonValueKind.Null => "null",
                JsonValueKind.Number => valueSrc.Value.ToString(),
                JsonValueKind.False => "false",
                JsonValueKind.String => valueSrc.Value.ToString(),
                JsonValueKind.Array => type switch
                {
                    PropType.@string => string.Join("", valueSrc.Value.AsAny.AsArray.Select(s => s.AsString.GetString())),
                    _ => throw new ArgumentException("unexpected array")
                },
                _ => throw new ArgumentException($"Unexpected type {valueSrc.Value.ValueKind}")
            };
            return GetSanitizedLiteralValue(type, s!, enumType);
        }

        private static ObjPropInfo GetProp(string namespaceRaw, string key, UngroupedPropObj prop)
        {
            PropType type = PropType.None;

            if (!Enum.TryParse(prop.Type.GetString()!, false, out type))
                throw new Exception("Missing required type for property");
            var extended = prop.As<UngroupedPropObj.ObjPropSelection.Typed>();
            var propBase = prop.As<PropBase>();

            string description = GetUnformattedDescription(prop.Description) ?? "No description";

            var propAsEnum = extended.As<PropDefExtensionEnum>();
            string? enumType = type == PropType.@enum ? propAsEnum.Enum.AsString.GetString() : null;

            var minMax = extended.As<PropDefExtensionMinMax>();

            return new ObjPropInfo(namespaceRaw, key)
            {
                Description = description,
                MinValue = GetNumericValue(type, minMax.MinValue),
                MaxValue = GetNumericValue(type, minMax.MaxValue),
                Default = GetLiteralValue(type, propBase.Default, enumType),
                Enum = enumType,
                DefaultFromServerProp = propBase.DefaultFromServerProperty?.GetString(),
                ObsoleteReason = propBase.Obsolete?.GetString(),
                RerollRestrictedTo = minMax.RerollRestrictedTo?.GetString(),
                Type = type
            };
        }

        private static ObjPropInfo GetProp(string namespaceRaw, string shortKey, GroupProp propDef, GroupDefaults groupDefaults)
        {
            string key = $"{groupDefaults.KeyPrefix}{shortKey}{groupDefaults.KeySuffix}";

            if (propDef.IsDescription)
            {
                string? description = GetUnformattedDescription(propDef.AsDescription);
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
            else if (propDef.IsPropObj)
            {
                var prop = propDef.AsPropObj;

                PropType type = groupDefaults.PropType;

                string? unformattedDesc = GetUnformattedDescription(prop.AsExtendedUntypedAttrs.Description);
                string desc = groupDefaults?.GetDescriptionFromFormat(shortKey, unformattedDesc) ?? unformattedDesc;

                var propAsEnum = prop.As<PropDefExtensionEnum>();
                string? enumType = type == PropType.@enum ? propAsEnum.Enum.AsString.GetString() : null;

                var minMax = prop.As<PropDefExtensionMinMax>();

                return new ObjPropInfo(namespaceRaw, key)
                {
                    Description = desc!,
                    MinValue = GetNumericValue(type, minMax.MinValue) ?? groupDefaults.MinValue,
                    MaxValue = GetNumericValue(type, minMax.MaxValue) ?? groupDefaults.MaxValue,
                    Default = GetLiteralValue(type, prop.Default, enumType) ?? groupDefaults.Default,
                    Enum = enumType,
                    DefaultFromServerProp = prop.DefaultFromServerProperty?.GetString(),
                    ObsoleteReason = prop.Obsolete?.GetString(),
                    RerollRestrictedTo = minMax.RerollRestrictedTo?.GetString(),
                    Type = type
                };
            }
            else
                throw new ArgumentException("Unhandled obj prop type");
        }
    }
}
