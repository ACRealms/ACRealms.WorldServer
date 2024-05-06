using System;
using System.Collections.Generic;
using System.Text;

namespace ACE.Server.Network.GameEvent.Events
{
    class GameEventAllegianceLoginNotification : GameEventMessage
    {
        public GameEventAllegianceLoginNotification(ISession session, uint playerGuid, bool isLoggedIn)
            : base (GameEventType.AllegianceLoginNotification, GameMessageGroup.UIQueue, session, 12)
        {
            Writer.Write(playerGuid);
            Writer.Write(Convert.ToUInt32(isLoggedIn));
        }
    }
}
