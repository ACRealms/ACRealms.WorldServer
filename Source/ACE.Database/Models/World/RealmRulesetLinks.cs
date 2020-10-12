using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public partial class RealmRulesetLinks
    {
        public ushort RealmId { get; set; }
        public ushort Order { get; set; }
        public ushort LinkType { get; set; }
        public ushort LinkedRealmId { get; set; }
        public byte? ProbabilityGroup { get; set; }
        public double? Probability { get; set; }

        public virtual Realm LinkedRealm { get; set; }
        public virtual Realm Realm { get; set; }
    }
}
