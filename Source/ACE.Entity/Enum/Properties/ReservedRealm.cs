using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ACE.Entity.Enum.Properties
{
    public enum ReservedRealm : ushort
    {
        @default = 0,

        [Description("The realm where player hideouts are located at.")]
        hideout = 0x7FFF
    }
}
