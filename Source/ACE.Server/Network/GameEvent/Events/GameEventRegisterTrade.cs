using ACE.Entity;

namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventRegisterTrade : GameEventMessage
    {
        public GameEventRegisterTrade(ISession session, ObjectGuid initiator, ObjectGuid partner)
            : base(GameEventType.RegisterTrade, GameMessageGroup.UIQueue, session, 20)
        {
            Writer.WriteGuid(initiator);
            Writer.WriteGuid(partner);
            Writer.Write(0L);
        }
    }
}
