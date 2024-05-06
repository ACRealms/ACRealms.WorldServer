using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Network.GameEvent.Events
{
    class GameEventPortalStorm : GameEventMessage
    {
        public GameEventPortalStorm(ISession session)
            : base(GameEventType.MiscPortalStorm, GameMessageGroup.UIQueue, session, 4)
        {
        }
    }
}
