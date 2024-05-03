using ACE.Entity.Enum;

namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventWeenieError : GameEventMessage
    {
        public GameEventWeenieError(ISession session, WeenieError errorType)
            : base(GameEventType.WeenieError, GameMessageGroup.UIQueue, session)
        {
            Writer.Write((uint)errorType);
        }
    }
}
