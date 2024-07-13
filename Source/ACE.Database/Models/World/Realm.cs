using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World;

/// <summary>
/// Dynamic Realm of a Shard/World
/// </summary>
public partial class Realm
{
    /// <summary>
    /// Unique Realm Id within the Shard
    /// </summary>
    public ushort Id { get; set; }

    public ushort Type { get; set; }

    /// <summary>
    /// Name of this realm
    /// </summary>
    public string Name { get; set; }

    public ushort? ParentRealmId { get; set; }

    /// <summary>
    /// Maximum number of properties that will be picked from the ruleset at random.
    /// </summary>
    public ushort? PropertyCountRandomized { get; set; }

    public virtual ICollection<RealmPropertiesBool> RealmPropertiesBool { get; set; } = new List<RealmPropertiesBool>();

    public virtual ICollection<RealmPropertiesFloat> RealmPropertiesFloat { get; set; } = new List<RealmPropertiesFloat>();

    public virtual ICollection<RealmPropertiesInt> RealmPropertiesInt { get; set; } = new List<RealmPropertiesInt>();

    public virtual ICollection<RealmPropertiesInt64> RealmPropertiesInt64 { get; set; } = new List<RealmPropertiesInt64>();

    public virtual ICollection<RealmPropertiesString> RealmPropertiesString { get; set; } = new List<RealmPropertiesString>();

    public virtual ICollection<RealmRulesetLinks> RealmRulesetLinksLinkedRealm { get; set; } = new List<RealmRulesetLinks>();

    public virtual ICollection<RealmRulesetLinks> RealmRulesetLinksRealm { get; set; } = new List<RealmRulesetLinks>();
}
