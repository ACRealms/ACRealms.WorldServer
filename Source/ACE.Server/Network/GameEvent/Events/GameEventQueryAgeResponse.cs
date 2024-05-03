namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventQueryAgeResponse : GameEventMessage
    {
        public GameEventQueryAgeResponse(ISession session, string targetName, string age)
            : base(GameEventType.QueryAgeResponse, GameMessageGroup.UIQueue, session)
        {
            Writer.WriteString16L(targetName);
            Writer.WriteString16L(age);
        }
    }
}
