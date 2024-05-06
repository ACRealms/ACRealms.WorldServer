namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventPingResponse : GameEventMessage
    {
        public GameEventPingResponse(ISession session)
            : base(GameEventType.PingResponse, GameMessageGroup.UIQueue, session, 4) { }
    }
}
