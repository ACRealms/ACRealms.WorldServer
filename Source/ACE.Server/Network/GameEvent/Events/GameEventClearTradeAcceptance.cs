using ACE.Entity;
using ACE.Entity.Enum;

namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventClearTradeAcceptance : GameEventMessage
    {
        public GameEventClearTradeAcceptance(ISession session)
            : base(GameEventType.ClearTradeAcceptance, GameMessageGroup.UIQueue, session, 4)
        {
        }
    }
}
