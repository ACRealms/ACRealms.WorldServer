namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventEvasionDefenderNotification : GameEventMessage
    {
        public GameEventEvasionDefenderNotification(ISession session, string attackerName)
            : base(GameEventType.EvasionDefenderNotification, GameMessageGroup.UIQueue, session)
        {
            Writer.WriteString16L(attackerName);
        }
    }
}
