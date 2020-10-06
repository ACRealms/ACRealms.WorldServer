using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public partial class RealmRulesetLinks
    {
        public short RealmId { get; set; }
        public string LinkedRealmId { get; set; }
        public int LinkType { get; set; }
        public int Order { get; set; }
        public double? Probability { get; set; }
        public int? ProbabilityGroup { get; set; }
    }
}
