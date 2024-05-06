using ACE.Entity.Enum;

namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventTradeFailure : GameEventMessage
    {
        public GameEventTradeFailure(ISession session, uint objectGuid, WeenieError reason)
            : base(GameEventType.TradeFailure, GameMessageGroup.UIQueue, session, 12)
        {
            Writer.Write(objectGuid);
            Writer.Write((uint)reason);
        }
    }
}
