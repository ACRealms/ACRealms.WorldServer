namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventEvasionAttackerNotification : GameEventMessage
    {
        public GameEventEvasionAttackerNotification(ISession session, string defenderName)
            : base(GameEventType.EvasionAttackerNotification, GameMessageGroup.UIQueue, session)
        {
            Writer.WriteString16L(defenderName);
        }
    }
}
