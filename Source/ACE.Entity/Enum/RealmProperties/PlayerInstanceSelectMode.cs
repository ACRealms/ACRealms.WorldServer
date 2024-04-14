using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Entity.Enum.RealmProperties
{
    public enum PlayerInstanceSelectMode : uint
    {
        Undefined = 0,
        Same = 1,
        SameIfSameLandblock = 2,
        HomeRealm = 3,
        PersonalRealm = 4,
        RealmDefaultInstanceID = 5,
    }
}
