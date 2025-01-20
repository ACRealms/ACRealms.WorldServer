using ACRealms.Roslyn.RealmProps.IntermediateModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACRealms.Roslyn.RealmProps.CompilerDomainModels
{
    // Intermediate record for the fallback values to use for a group of property definitions within the schema compilation
    internal record GroupDefaults(string? DescriptionFormat = null)
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
        public required PropType PropType { get; init; }
        public string? ObsoleteReason { get; init; }
        public string? RerollRestrictedTo { get; init; }
        public required ImmutableArray<PropContext> Contexts { get; init; } = [];

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
}
