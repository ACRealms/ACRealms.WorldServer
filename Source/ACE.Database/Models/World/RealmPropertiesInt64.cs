using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World;

/// <summary>
/// Int64 Properties of Realms
/// </summary>
public partial class RealmPropertiesInt64
{
    /// <summary>
    /// Id of the object this property belongs to
    /// </summary>
    public ushort RealmId { get; set; }

    /// <summary>
    /// Type of Property the value applies to (RealmPropertyInt64.????)
    /// </summary>
    public ushort Type { get; set; }

    /// <summary>
    /// Value of this Property
    /// </summary>
    public long? Value { get; set; }

    /// <summary>
    /// If true, this property cannot be overriden by inherited realms or rulesets.
    /// </summary>
    public bool Locked { get; set; }

    public double? Probability { get; set; }

    public long? RandomLowRange { get; set; }

    public long? RandomHighRange { get; set; }

    public byte RandomType { get; set; }

    public byte CompositionType { get; set; }

    public virtual Realm Realm { get; set; }
}
