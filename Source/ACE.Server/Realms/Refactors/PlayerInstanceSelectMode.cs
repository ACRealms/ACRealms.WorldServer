using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Realms
{
    public enum PlayerInstanceSelectMode : ushort
    {
        Undefined,
        Same,
        SameIfSameLandblock,
        HomeRealm,
        PersonalRealm,
        RealmDefaultInstanceID,
        PerRuleset
    }
}
