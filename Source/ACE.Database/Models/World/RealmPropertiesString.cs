using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World;

/// <summary>
/// String Properties of Realms
/// </summary>
public partial class RealmPropertiesString
{
    /// <summary>
    /// Id of the object this property belongs to
    /// </summary>
    public ushort RealmId { get; set; }

    /// <summary>
    /// Type of Property the value applies to (RealmPropertyString.????)
    /// </summary>
    public ushort Type { get; set; }

    /// <summary>
    /// Value of this Property
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// If true, this property cannot be overriden by inherited realms or rulesets.
    /// </summary>
    public bool Locked { get; set; }

    public double? Probability { get; set; }

    public virtual Realm Realm { get; set; }
}
