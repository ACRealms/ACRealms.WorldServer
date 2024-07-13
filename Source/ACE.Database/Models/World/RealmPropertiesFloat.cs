using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World;

/// <summary>
/// Float Properties of Realms
/// </summary>
public partial class RealmPropertiesFloat
{
    /// <summary>
    /// Id of the object this property belongs to
    /// </summary>
    public ushort RealmId { get; set; }

    /// <summary>
    /// Type of Property the value applies to (RealmPropertyFloat.????)
    /// </summary>
    public ushort Type { get; set; }

    /// <summary>
    /// Value of this Property
    /// </summary>
    public double? Value { get; set; }

    /// <summary>
    /// If true, this property cannot be overriden by inherited realms or rulesets.
    /// </summary>
    public bool Locked { get; set; }

    public double? RandomLowRange { get; set; }

    public double? RandomHighRange { get; set; }

    public double? Probability { get; set; }

    public byte RandomType { get; set; }

    public byte CompositionType { get; set; }

    public virtual Realm Realm { get; set; }
}
