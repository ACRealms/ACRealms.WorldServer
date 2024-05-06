using ACE.Entity;

namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventAcceptTrade : GameEventMessage
    {
        public GameEventAcceptTrade(ISession session, ObjectGuid whoAccepted)
            : base(GameEventType.AcceptTrade, GameMessageGroup.UIQueue, session, 8)
        {
            Writer.WriteGuid(whoAccepted);
        }
    }
}
