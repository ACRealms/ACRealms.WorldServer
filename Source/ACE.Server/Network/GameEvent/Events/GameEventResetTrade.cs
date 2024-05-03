using ACE.Entity;

namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventResetTrade : GameEventMessage
    {
        public GameEventResetTrade(ISession session, ObjectGuid whoReset)
            : base(GameEventType.ResetTrade, GameMessageGroup.UIQueue, session)
        {
            Writer.WriteGuid(whoReset);
        }
    }
}
