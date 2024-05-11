using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Entity.Enum.RealmProperties
{
    public enum WorldObjectInstanceSelectMode : uint
    {
        Undefined = 0,
        Same,
        RealmDefaultInstanceID,


        // Do not add entries below this item
        reserved
    }
}
