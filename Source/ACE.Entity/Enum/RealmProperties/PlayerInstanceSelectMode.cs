using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Entity.Enum.RealmProperties
{
    public enum PlayerInstanceSelectMode : uint
    {
        Undefined,
        Same,
        SameIfSameLandblock,
        HomeRealm,
        PersonalRealm,
        RealmDefaultInstanceID
    }
}
