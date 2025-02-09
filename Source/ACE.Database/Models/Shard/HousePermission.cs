using System;
using System.Collections.Generic;

namespace ACE.Database.Models.Shard;

public partial class HousePermission
{
    /// <summary>
    /// GUID of House Biota Object
    /// </summary>
    public ulong HouseId { get; set; }

    /// <summary>
    /// GUID of Player Biota Object being granted permission to this house
    /// </summary>
    public ulong PlayerGuid { get; set; }

    /// <summary>
    /// Permission includes access to House Storage
    /// </summary>
    public bool Storage { get; set; }

    public virtual Biota House { get; set; }
}
