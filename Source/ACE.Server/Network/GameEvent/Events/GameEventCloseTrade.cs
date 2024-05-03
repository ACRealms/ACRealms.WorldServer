using ACE.Entity.Enum;

namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventCloseTrade : GameEventMessage
    {
        public GameEventCloseTrade(ISession session, EndTradeReason endTradeReason)
            : base(GameEventType.CloseTrade, GameMessageGroup.UIQueue, session)
        {
            Writer.Write((uint)endTradeReason);
        }
    }
}
