using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World;

public partial class RealmRulesetLinks
{
    public ushort RealmId { get; set; }

    public ushort Order { get; set; }

    public ushort LinkType { get; set; }

    public ushort LinkedRealmId { get; set; }

    public byte? ProbabilityGroup { get; set; }

    /// <summary>
    /// A random number between 0 and 1 will be generated. The first link with a probability value greater than the number value in the probability_group will be applied, and the rest in the group ignored (per landblock).
    /// </summary>
    public double? Probability { get; set; }

    public virtual Realm LinkedRealm { get; set; }

    public virtual Realm Realm { get; set; }
}
