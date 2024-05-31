using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ACE.Entity.Enum.Properties
{
    public enum ReservedRealm : ushort
    {
        NULL = 0,

        Reserved1 = 0x7FE0,
        Reserved2 = 0x7FE1,
        Reserved3 = 0x7FE2,
        Reserved4 = 0x7FE3,
        Reserved5 = 0x7FE4,
        Reserved6 = 0x7FE5,
        Reserved7 = 0x7FE6,
        Reserved8 = 0x7FE7,
        Reserved9 = 0x7FE8,
        Reserved10 = 0x7FE9,
        Reserved11 = 0x7FEA,
        Reserved12 = 0x7FEB,
        Reserved13 = 0x7FEC,
        Reserved14 = 0x7FED,
        Reserved15 = 0x7FEE,
        Reserved16 = 0x7FEF,
        Reserved17 = 0x7FF0,
        Reserved18 = 0x7FF1,
        Reserved19 = 0x7FF2,
        Reserved20 = 0x7FF3,
        Reserved21 = 0x7FF4,
        Reserved22 = 0x7FF5,
        Reserved23 = 0x7FF6,
        Reserved24 = 0x7FF7,
        Reserved25 = 0x7FF8,
        Reserved26 = 0x7FF9,
        Reserved27 = 0x7FFA,
        Reserved28 = 0x7FFB,
        Reserved29 = 0x7FFC,

        RealmSelector = 0x7FFD,
        @default = 0x7FFE,

        [Description("The realm where player hideouts are located at.")]
        hideout = 0x7FFF
    }
}
