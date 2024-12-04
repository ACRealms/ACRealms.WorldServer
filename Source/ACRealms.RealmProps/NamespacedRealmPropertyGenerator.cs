using ACRealms.RealmProps.IntermediateModels;
using Corvus.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Frozen;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;
using System.Threading;
using System.Xml;
using JsonObject = Corvus.Json.JsonObject;
/*using Corvus.Json.Validator;
using JsonObject = Corvus.Json.JsonObject;
using JsonSchema = Corvus.Json.Validator.JsonSchema;
using ValidationContext = Corvus.Json.ValidationContext;*/

#nullable enable

namespace ACRealms.CodeGen
{
    // To debug: Test Explorer -> ACRealms.Tests.Tests.RealmPropGenerator -> Debug CanGenerateRealmProps test
    // Important: Significant changes to this generator's process or algorithm should be reflected in ACRealms.RoslynAnalyzer.ACR20XX_RealmProps where applicable
    //            This allows for more accurate errors to be displayed during compilation
    //
    // The C# model classes in ACRealms.RealmProps.RealmPropModels are generated through a manual script invocation as needed
//#pragma warning disable RS1038 // Compiler extensions should be implemented in assemblies with compiler-provided references
    [Generator(LanguageNames.CSharp)]
//#pragma warning restore RS1038 // Compiler extensions should be implemented in assemblies with compiler-provided references
    public class NamespacedRealmPropertyGenerator : IIncrementalGenerator
    {
        const bool USE_VERBOSE_STACKTRACE = true;


        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Regex for property names: https://regexr.com/8915n

            // TODO: DELETE FILES THAT AREN'T GENERATED
            IncrementalValuesProvider<AdditionalText> realmPropsDirContext = context.AdditionalTextsProvider
                .Where(static text => { var sep = Path.DirectorySeparatorChar; return text.Path.Contains($"{sep}RealmProps{sep}json"); });

            /*IncrementalValuesProvider<(string Path, string Contents)> jsonSchemaFiles =
                realmPropsDirContext.Where(static text => { var sep = Path.DirectorySeparatorChar; return text.Path.Contains($"{sep}RealmProps{sep}json-schema{sep}"); })
                .Select(static (text, cancellationToken) => (text.Path, text.GetText(cancellationToken)!.ToString()));*/

            IncrementalValuesProvider<(string Path, string Contents)> realmPropFiles = realmPropsDirContext
                .Where(static text => { var sep = Path.DirectorySeparatorChar; return text.Path.Contains($"{sep}RealmProps{sep}json{sep}"); })
                .Where(static text => Path.GetExtension(text.Path).Equals(".jsonc", StringComparison.Ordinal))
                .Select(static (text, cancellationToken) => {
                    try
                    {
                        return (text.Path, text.GetText(cancellationToken)!.ToString());
                    }
                    catch (Exception ex)
                    {
                        ReThrowWrappedException("2" + text.Path, ex);
                        throw;
                    }
                });

            // Expensive part here, should only run for the file contents that have changed
            IncrementalValuesProvider<NamespaceData> realmPropNamespaces = realmPropFiles
                .Select(static (file, cancellationToken) =>
                {
                    int step = 0;
                    try
                    {
                        var commentsRemoved = RemoveJsonComments(file.Contents, cancellationToken);
                        step++;

                        var jObject = JsonObject.Parse(commentsRemoved);
                        step++;

                        // RealmPropertySchema is a class, and not cache-safe so we shouldn't return it. Convert to NamespaceData record
                        var namespaceObj = RealmPropertySchema.FromJson(jObject.AsJsonElement);
                        step++;

                        string namespaceRaw = namespaceObj.Namespace.GetString()!;

                        var props = GetProps(namespaceRaw, namespaceObj);
                        step++;

                        var array = props.ToImmutableArray();
                        step++;

                        var propsWrapper = new ImmutableArrayWrapper<ObjPropInfo>(array);
                        step++;

                        return new NamespaceData(file.Path, namespaceObj.Namespace.GetString()!, propsWrapper);
                    }
                    catch (Exception ex)
                    {
                        ReThrowWrappedException($"at Step 1.{step}: " + file.Path, ex);
                        throw;
                    }
                });

            context.RegisterSourceOutput(realmPropNamespaces, (spc, data) =>
            {
                var fileName = string.Join("/", data.NestedClassNames);
                var sourceCode = GenerateSourceCode(data);
                spc.AddSource($"NamespacedProps/{fileName}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
            });



            // And for all the properties combined, regardless of namespace, we create the core enums
            // These will be equivalent to the RealmProperty enums which were hand-maintained before ACRealms v2.2
            IncrementalValueProvider<ImmutableArray<ObjPropInfo>> combinedProps =
                realmPropNamespaces.Collect().Select(static (x, cancellationToken) => {

                    // Here we add some undef props so that at least 1 prop of each type is made, and so that we have a value of 0 for the undefined prop
                    var undefProps = new List<PropType> { PropType.integer, PropType.@string, PropType.boolean, PropType.int64, PropType.@float }.Select(type =>
                    {
                        return new ObjPropInfo("__NONE__", "Undef")
                        {
                            Description = "",
                            Type = type
                        };
                    });
                    return x.SelectMany(x => x.ObjProps.Array).Concat(undefProps).ToImmutableArray();
                });
            var types = ObjPropInfo.PropMap.Select(static kvp => ((PropType propType, string enumType))(kvp.Key, kvp.Value)).ToImmutableArray();
            var typesProvider = combinedProps.SelectMany(static (_, cancellationToken) => ObjPropInfo.PropMap.Values.Distinct());

            IncrementalValuesProvider<(string TargetEnumTypeName, ImmutableArray<ObjPropInfo> AllProps)> corePropsSourceUnfiltered = typesProvider.Combine(combinedProps);
            var corePropsSource = corePropsSourceUnfiltered.Select((data, cancellationToken) =>
                ((string TargetEnumTypeName, ImmutableArray<ObjPropInfo> PropsOfType))(
                    data.TargetEnumTypeName,
                    data.AllProps.Where(p =>
                        ObjPropInfo.PropMap[p.Type] == data.TargetEnumTypeName
                    ).OrderBy(p => p.CoreKey).ToImmutableArray()
                ));

            context.RegisterSourceOutput(corePropsSource, (spc, data) =>
            {
                if (data.PropsOfType.IsEmpty)
                    return;

                var sourceCode = GenerateCoreEnumClass(data.TargetEnumTypeName, data.PropsOfType);
                spc.AddSource($"CoreProps/{data.TargetEnumTypeName}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
            });
        }

        public static string RemoveJsonComments(string jsonc, CancellationToken token)
        {
            var sb = new StringBuilder();
            using var reader = new StringReader(jsonc);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                token.ThrowIfCancellationRequested();
                // Remove single-line comments
                var index = line.IndexOf("//");
                if (index >= 0)
                    line = line[..index];

                // Remove block comments (simple implementation)
                // You can enhance this to handle multi-line block comments
                sb.AppendLine(line);
            }

            return sb.ToString();
        }


        // "propertytype": { "type": "string", "enum": [ "integer", "boolean", "float", "int64", "string", "enum" ] },

        enum PropType
        {
            None,
            @enum,
            integer,
            int64,
            @float,
            boolean,
            @string
        }

        enum PrimitiveType
        {
            @null,
            @bool,
            @int,
            @long,
            @double,
            @string,
            @enum
        }

        record ObjPropInfo(string NamespaceRaw, string Key)
        {
            public string CoreKey { get; } = $"{NamespaceRaw.Replace('.', '_')}_{Key}";
            public required string Description { get; init; }
            public required PropType Type { get; init; }
            public string? Enum { get; init; }
            public string? Default { get; init; }

            public string? MinValue { get; init; }
            public string? MaxValue { get; init; }
            public string? RerollRestrictedTo { get; init; }
            public string? ObsoleteReason { get; init; }
            public string? DefaultFromServerProp { get; init; }

            public static readonly FrozenDictionary<PropType, string> PropMap = new Dictionary<PropType, string>()
            {
                { PropType.@string, "RealmPropertyString" },
                { PropType.integer, "RealmPropertyInt" },
                { PropType.boolean, "RealmPropertyBool" },
                { PropType.int64, "RealmPropertyInt64" },
                { PropType.@float, "RealmPropertyFloat" },
                { PropType.@enum, "RealmPropertyInt" }
            }.ToFrozenDictionary();

            private string CoreEnumType => PropMap[Type];
            public string ToNamespacedAliasDeclaration(string spacer)
            {
                var obs = ObsoleteReason != null ? $$"""

                {{spacer}}[System.Obsolete("{{ObsoleteReason}}")]
                """ : "";

                return
                $$"""
                {{spacer}}/// <summary>{{Description}}</summary>{{obs}}
                {{spacer}}public const {{CoreEnumType}}Staging {{Key}} = {{CoreEnumType}}Staging.{{CoreKey}};
                """;
            }

            private string DefaultLiteral(PrimitiveType valuePrimitiveType)
            {
                return valuePrimitiveType switch
                {
                    PrimitiveType.@bool => "false",
                    PrimitiveType.@double => "0.0",
                    PrimitiveType.@int => "0",
                    PrimitiveType.@long => "0",
                    PrimitiveType.@string => "\"\"",
                    PrimitiveType.@enum => $"({Enum})0",
                    _ => "<!INVALID!>"
                };
            }

            private IEnumerable<string> CorePrimaryAttributeArgs(string canonicalPrimaryAttributeType, PrimitiveType valuePrimitiveType)
            {
                return canonicalPrimaryAttributeType switch
                {
                    "RealmPropertyPrimaryAttribute" => [Default ?? DefaultLiteral(valuePrimitiveType)],
                    "RealmPropertyEnumAttribute" => [Default ?? DefaultLiteral(valuePrimitiveType)],
                    "RealmPropertyPrimaryMinMaxAttribute" => [
                        Default ?? DefaultLiteral(valuePrimitiveType),
                        MinValue ?? DefaultLiteral(valuePrimitiveType),
                        MaxValue ?? DefaultLiteral(valuePrimitiveType)
                    ],
                    _ => throw new NotImplementedException($"Missing handler for {canonicalPrimaryAttributeType}")
                };
            }

            private string CorePrimaryAttribute(string aliasedPrimaryAttributeType, string canonicalPrimaryAttributeType, PrimitiveType valuePrimitiveType)
            {
                string typeArg = valuePrimitiveType switch
                {
                    PrimitiveType.@enum => $"<{Enum}>",
                    _ => String.Empty
                };

                var primaryArgs = string.Join(", ", CorePrimaryAttributeArgs(canonicalPrimaryAttributeType, valuePrimitiveType));
                return
                $$"""
                [{{aliasedPrimaryAttributeType}}{{typeArg}}({{primaryArgs}})]
                """;
            }

            public string ToCoreEnumDeclaration(
                string aliasedPrimaryAttributeType,
                string canonicalPrimaryAttributeType,
                PrimitiveType valuePrimitiveType)
            {
                var obs = ObsoleteReason != null ? $$"""

                    [Obsolete("{{ObsoleteReason}}")]
                """ : "";
                var rerollRestriction = RerollRestrictedTo != null ? $$"""

                    [RerollRestrictedTo(RealmPropertyRerollType.{{RerollRestrictedTo}})]
                """ : "";
                return
                $$"""
                    [Description("{{Description}}")]{{obs}}{{rerollRestriction}}
                    {{CorePrimaryAttribute(aliasedPrimaryAttributeType, canonicalPrimaryAttributeType, valuePrimitiveType)}}
                    {{CoreKey}},
                """;
            }
        }

        private static string GetUnformattedDescription(RealmPropertySchema.DescriptionArrayEntity descriptionArray)
        {
            // This needs to eventually be handled with fixed line widths and proper wrapping
            string newline = $$"""

                """ + " ";
            return string.Join(newline, descriptionArray.Select(x => x.AsString.GetString()!));
        }

        private static string? GetUnformattedDescription(RealmPropertySchema.ValDescriptionEntity? descriptionDef)
        {
            if (!descriptionDef.HasValue || descriptionDef.Value.IsNullOrUndefined())
                return null;
            if (descriptionDef.Value.AsString.GetString() is string d)
                return d;
            if (descriptionDef.Value.IsOneOf0Entity)
            {
                var def = descriptionDef.Value.AsOneOf0Entity;
                if (def.IsValStringSimpleEntity)
                    return def.AsValStringSimpleEntity.AsString.GetString();
                if (def.IsDescriptionPatternEntity)
                    return def.AsDescriptionPatternEntity.AsString.GetString();
            }
            if (descriptionDef.Value.IsDescriptionArrayEntity)
                return GetUnformattedDescription(descriptionDef.Value.AsDescriptionArrayEntity);
            throw new ArgumentException("Unhandled description type in schema");
        }

        private static string GetFullDescription(string shortKey, RealmPropertySchema.ValDescriptionEntity? descriptionDef, GroupDefaults? groupDefaults)
        {
            var unformattedDescription = GetUnformattedDescription(descriptionDef);
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

        private static string? GetNumericValue(PropType type, RealmPropertySchema.ValFloatEntity? numericValueSrc)
        {
            if (!numericValueSrc.HasValue || numericValueSrc.Value.IsNullOrUndefined())
                return null;
            
            var s = numericValueSrc.ToString();
            return GetSanitizedLiteralValue(type, s);
        }

        private static string GetSanitizedIntValue(string unsanitizedValue)
        {
            if (!int.TryParse(unsanitizedValue, out var intResult))
                throw new ArgumentException($"Invalid integer value ({unsanitizedValue})");
            return intResult.ToString();
        }

        private static string GetSanitizedLongValue(string unsanitizedValue)
        {
            if (!long.TryParse(unsanitizedValue, out var longResult))
                throw new ArgumentException($"Invalid int64 value ({unsanitizedValue})");
            return longResult.ToString();
        }

        private static string GetSanitizedDoubleValue(string unsanitizedValue)
        {
            if (!double.TryParse(unsanitizedValue, out var doubleResult))
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
            var result = type switch
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

        private static string? GetLiteralValue(PropType type, RealmPropertySchema.ObjPropOrGroupEntity.DefaultEntity? valueSrc, string? enumType = null)
        {
            if (!valueSrc.HasValue || valueSrc.Value.IsNullOrUndefined())
                return null;
            var s = valueSrc.Value.ValueKind switch {
                System.Text.Json.JsonValueKind.Undefined => null,
                System.Text.Json.JsonValueKind.True => "true",
                System.Text.Json.JsonValueKind.Null => "null",
                System.Text.Json.JsonValueKind.Number => valueSrc.Value.ToString(),
                System.Text.Json.JsonValueKind.False => "false",
                System.Text.Json.JsonValueKind.String => valueSrc.Value.ToString(),
                System.Text.Json.JsonValueKind.Array => type switch {
                    PropType.@string => string.Join("", valueSrc.Value.Select(s => s.AsString.GetString())),
                    _ => throw new ArgumentException("unexpected array")
                },
                _ => throw new ArgumentException($"Unexpected type {valueSrc.Value.ValueKind}")
            };
            return GetSanitizedLiteralValue(type, s!, enumType);
        }

        private static ObjPropInfo GetProp(string namespaceRaw, string shortKey, RealmPropertySchema.PropDefEntity propDef, GroupDefaults? groupDefaults = null)
        {
            string key;
            if (groupDefaults == null)
                key = shortKey;
            else
                key = $"{groupDefaults.KeyPrefix}{shortKey}{groupDefaults.KeySuffix}";

            if (propDef.IsValDescriptionEntity)
            {
                if (groupDefaults == null)
                    throw new ArgumentException("Prop from string short description definition only allowed within a group!");

                var description = GetUnformattedDescription(propDef.AsValDescriptionEntity);
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
            else if (propDef.IsObjPropEntity)
            {
                var prop = propDef.AsObjPropEntity;
                Enum.TryParse(prop.Type.GetString()!, false, out PropType propType);

                var unformattedDesc = GetUnformattedDescription(prop.Description);

                var desc = groupDefaults?.GetDescriptionFromFormat(shortKey, unformattedDesc) ?? unformattedDesc;
                var type = propType == PropType.None ? groupDefaults!.PropType : propType;

                return new ObjPropInfo(namespaceRaw, key)
                {
                    Description = desc!,
                    MinValue = GetNumericValue(propType, prop.MinValue) ?? groupDefaults?.MinValue,
                    MaxValue = GetNumericValue(propType, prop.MaxValue) ?? groupDefaults?.MaxValue,
                    Default = GetLiteralValue(propType, prop.Default) ?? groupDefaults?.Default,
                    Enum = prop.Enum.GetString() ?? groupDefaults?.Enum,
                    DefaultFromServerProp = prop.DefaultFromServerProperty.GetString(),
                    ObsoleteReason = prop.Obsolete.GetString(),
                    RerollRestrictedTo = prop.RerollRestrictedTo.IsNotNullOrUndefined() ? prop.RerollRestrictedTo.AsString.GetString() : null,
                    Type = type
                };
            }
            else
                throw new ArgumentException("Unhandled obj prop type");
        }


        // Intermediate record for the fallback values to use for a group of property definitions within the schema compilation
        record GroupDefaults(string? DescriptionFormat = null)
        {
            public required string KeyPrefix { get; init; }
            public required string KeySuffix { get; init; }
            private string? Description { get; init; } = DescriptionFormat switch
            {
                null => null,
                string f when f.Contains('{') => null,
                _ => DescriptionFormat
            };

            public string? MinValue { get; init; }
            public string? MaxValue { get; init; }
            public string? Default { get; init; }
            public string? Enum { get; init; }
            public PropType PropType { get; init; }
            public string? ObsoleteReason { get; init; }
            public string? RerollRestrictedTo { get; init; }

            private string Format(string shortKey, string? unformattedDescription)
            {
                if (unformattedDescription == null)
                    return DescriptionFormat?.Replace("{short_key}", shortKey) ?? "No description (3)";

                return DescriptionFormat?.Replace("{short_key}", shortKey)?.Replace("{short_description}", unformattedDescription ?? "No description (5)")
                    ?? unformattedDescription?.Replace("{short_key}", shortKey)
                    ?? "No description (4)";
            }

            public string GetDescriptionFromFormat(string shortKey, string? unformattedDescription = null)
            {
                return Format(shortKey, unformattedDescription);
            }
        }

        private static IEnumerable<ObjPropInfo> GetProps(string namespaceRaw, RealmPropertySchema.GroupEntityArray? groupsDefinition)
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

                var props = group.Properties;
                if (props.IsPropNameEntityArray)
                {
                    return props.AsPropNameEntityArray.Select(shortKeyNode =>
                    {
                        string shortKey = shortKeyNode.GetString()!;
                        var key = $"{gDefaults.KeyPrefix}{shortKey}{gDefaults.KeySuffix}";

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
                else if (props.IsObjPropListEntity)
                {
                    return props.AsObjPropListEntity.Select(kvp =>
                    {
                        string shortKey = kvp.Key.GetString();
                        var key = $"{gDefaults.KeyPrefix}{shortKey}{gDefaults.KeySuffix}";

                        return GetProp(namespaceRaw, shortKey, kvp.Value, gDefaults);
                    });
                }
                else
                    throw new ArgumentException("Unhandled group props type");
            });
        }


        private static IEnumerable<ObjPropInfo> GetProps(string namespaceRaw, RealmPropertySchema.ObjPropListEntity? propertiesDefinition)
        {
            if (!propertiesDefinition.HasValue || propertiesDefinition.Value.IsNullOrUndefined())
                return [];
            return propertiesDefinition.Value.Select(kvp => GetProp(namespaceRaw, kvp.Key.GetString(), kvp.Value));
        }

        private static IEnumerable<ObjPropInfo> GetProps(string namespaceRaw, RealmPropertySchema schema)
            => GetProps(namespaceRaw, schema.Properties).Concat(GetProps(namespaceRaw, schema.Groups));

        public sealed class ImmutableArrayWrapper<T> : IEquatable<ImmutableArrayWrapper<T>>
        {
            public ImmutableArray<T> Array { get; }

            public ImmutableArrayWrapper(ImmutableArray<T> array)
            {
                Array = array;
            }

            public override bool Equals(object? obj)
            {
                return Equals(obj as ImmutableArrayWrapper<T>);
            }

            public bool Equals(ImmutableArrayWrapper<T>? other)
            {
                if (other is null)
                    return false;

                if (ReferenceEquals(this, other))
                    return true;

                if (Array.Length != other.Array.Length)
                    return false;

                for (int i = 0; i < Array.Length; i++)
                {
                    if (!EqualityComparer<T>.Default.Equals(Array[i], other.Array[i]))
                        return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                var hash = new HashCode();

                foreach (var item in Array)
                {
                    hash.Add(item);
                }

                return hash.ToHashCode();
            }
        }

        record NamespaceData
        {
            public string OriginalPath { get; private init; }
            public ImmutableArrayWrapper<ObjPropInfo> ObjProps { get; private init; }
            public string NamespaceFull { get; private init; }
            public ImmutableArray<string> NestedClassNames { get; private init; }
            public NamespaceData(string originalPath, string namespaceFull, ImmutableArrayWrapper<ObjPropInfo> objProps)
            {
                OriginalPath = originalPath;
                NamespaceFull = namespaceFull;
                ObjProps = objProps;

                var parts = namespaceFull.Split('.');
                
                NestedClassNames = parts.ToImmutableArray();
            }

            internal string ToCompilationSource()
            {
                string newline =
                $$"""

                
                """;
                var declSpacer = new string(' ', (NestedClassNames.Length + 1) * 4);
                var declarations = ObjProps.Array.Select(p => p.ToNamespacedAliasDeclaration(declSpacer));
                var declarationsText = string.Join(newline, declarations);

                var nestedClassDecl = GetNestedClassDecl(declarationsText, new Queue<string>(NestedClassNames), 1);

                return $$"""
                // THIS FILE IS AUTOMATICALLY GENERATED
                namespace ACRealms;

                public static partial class Props
                {
                {{nestedClassDecl}}
                }
                """;
            }

            private string GetNestedClassDecl(string declarationsText, Queue<string> classNameQueue, int depth)
            {
                var className = classNameQueue.Dequeue();
                bool isLast = classNameQueue.Count == 0;

                // Could optimize this to not allocate duplicate strings, but I don't think it adds up to much
                var spacer1 = new string(' ', 4);
                var spacer2 = new string(' ', 4 * depth);
                var spacer3 = new string(' ', 4 * (depth > 2 ? (depth - 1) : 1));
                var spacer4 = new string(' ', isLast ? 4 * depth : 4);
                var decl =
                $$""""
                {{spacer3}}public static{{(isLast ? " " : " partial ")}}class {{className}}
                {{spacer2}}{
                {{(isLast ? $"{declarationsText}" : $"{spacer1}{GetNestedClassDecl(declarationsText, classNameQueue, depth + 1)}")}}
                {{spacer2}}}
                """";
                return decl;
            }
        }

        private static string GenerateSourceCode(NamespaceData data)
        {
            try
            {
                return $$"""
                
                {{data.ToCompilationSource()}}
                """;
            }
            catch (Exception ex)
            {
                ReThrowWrappedException($"GenerateSourceCode ({data.NamespaceFull})", ex);
                throw;
            }
        }

        private string GenerateCoreEnumClass(string targetEnumTypeName, ImmutableArray<ObjPropInfo> propsOfType)
        {
            try
            {
                (string AliasedPrimaryAttributeType, string CanonicalPrimaryAttributeType, PrimitiveType)? primaryAttrDataNullable = targetEnumTypeName switch
                {
                    "RealmPropertyString" => ("RealmPropertyStringAttribute", "RealmPropertyPrimaryAttribute", PrimitiveType.@string),
                    "RealmPropertyInt" => ("RealmPropertyIntAttribute", "RealmPropertyPrimaryMinMaxAttribute", PrimitiveType.@int),
                    "RealmPropertyInt64" => ("RealmPropertyInt64Attribute", "RealmPropertyPrimaryMinMaxAttribute", PrimitiveType.@long),
                    "RealmPropertyBool" => ("RealmPropertyBoolAttribute", "RealmPropertyPrimaryAttribute", PrimitiveType.@bool),
                    "RealmPropertyFloat" => ("RealmPropertyFloatAttribute", "RealmPropertyPrimaryMinMaxAttribute", PrimitiveType.@double),
                   // "RealmPropertyEnum" => ("RealmPropertyEnumAttribute", "RealmPropertyEnumAttribute", PrimitiveType.@enum),
                    _ => throw new Exception($"Unexpected propType '{targetEnumTypeName}'")
                };
                if (!primaryAttrDataNullable.HasValue)
                    throw new Exception($"Unexpected propType '{targetEnumTypeName}'");
                var (AliasedPrimaryAttributeType, CanonicalPrimaryAttributeType, ValuePrimitiveType) = primaryAttrDataNullable!.Value;

                string newline = $$"""



                """;

                var canonicalPropDecls = string.Join(newline, propsOfType.Select(p =>
                {
                    if (p.NamespaceRaw == "__NONE__")
                        return "";
                    var thisAliasedPrimaryAttributeType = p.Type switch
                    {
                        PropType.@enum => "RealmPropertyEnumAttribute",
                        _ => AliasedPrimaryAttributeType
                    };
                    var thisCanonicalPrimaryAttributeType = p.Type switch
                    {
                        PropType.@enum => "RealmPropertyEnumAttribute",
                        _ => CanonicalPrimaryAttributeType
                    };
                    var thisValuePrimitiveType = p.Type switch
                    {
                        PropType.@enum => PrimitiveType.@enum,
                        _ => ValuePrimitiveType
                    };
                    return p.ToCoreEnumDeclaration(thisAliasedPrimaryAttributeType, thisCanonicalPrimaryAttributeType, thisValuePrimitiveType);
                }));
                //{{(targetEnumTypeName == "RealmPropertyInt" ? $"using RealmPropertyEnumAttribute = ACE.Entity.Enum.Properties.RealmPropertyEnumAttribute<{ValuePrimitiveType}>;" : "")}}}

                return $$""""
                using ACE.Entity.Enum.RealmProperties;
                using System;
                using System.ComponentModel;
                using {{AliasedPrimaryAttributeType}} = ACE.Entity.Enum.Properties.{{CanonicalPrimaryAttributeType}}<{{ValuePrimitiveType}}>;

                // THIS FILE IS AUTOMATICALLY GENERATED

                namespace ACE.Entity.Enum.Properties;

                [RequiresPrimaryAttribute<{{CanonicalPrimaryAttributeType}}<{{ValuePrimitiveType}}>, {{ValuePrimitiveType}}>]
                public enum {{targetEnumTypeName}}Staging : ushort
                {
                    Undef = 0,

                {{canonicalPropDecls}}
                }
                
                """";
            }
            catch (Exception ex)
            {
                ReThrowWrappedException($"GenerateCoreEnumClass ({targetEnumTypeName})", ex);
                throw;
            }
        }
        private static string FormatStackTrace(Exception ex)
        {
            if (ex.StackTrace == null || ex.StackTrace.Length == 0)
                return "None";
            IEnumerable<string> frames = ex.StackTrace.Split('\n');
            if (!USE_VERBOSE_STACKTRACE)
#pragma warning disable CS0162 // Unreachable code detected
                frames = frames.Where(x => x.Contains(nameof(NamespacedRealmPropertyGenerator)));
#pragma warning restore CS0162 // Unreachable code detected
            return string.Join("; ", frames).Replace("\r", "").Replace("\n", "");
        }

        [DoesNotReturn]
        private static void ReThrowWrappedException(string identifier, Exception originalException)
            => throw new Exception($"Exception compiling namespace '{identifier}': {originalException.Message}; StackTrace: {FormatStackTrace(originalException)}", originalException);
    }
}
