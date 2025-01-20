using ACRealms.Rulesets.Enums;

# nullable enable
namespace ACRealms.Rulesets
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Property names are directly tied to json document")]
    public class RealmPropertyJsonModel
    {
        public string? value { get; set; }
        public string? low { get; set; }
        public string? high { get; set; }
        public bool? locked { get; set; }
        public double? probability { get; set; }
        public RealmPropertyRerollType? reroll { get; set; }
        public RealmPropertyCompositionType compose { get; set; }
        public Dictionary<string, Dictionary<string, object>> scope { get; set; }
        public void ValidateAll()
        {
            if ((low == null && high != null) || (low != null && high == null))
                throw new Exception("Both low and high values must be present if one is present.");
            if (value == null && low == null)
                throw new Exception("Either value or low/high range must be provided.");
            if (value != null && low != null)
                throw new Exception("If providing a value, may not provide a low/high range.");
        }

        public void ValidateValuePresent()
        {
            if (value == null)
                throw new Exception("value must be present");
        }
    }
}
