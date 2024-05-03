namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventPopupString : GameEventMessage
    {
        public GameEventPopupString(ISession session, string message)
            : base(GameEventType.PopupString, GameMessageGroup.UIQueue, session)
        {
            Writer.WriteString16L(message);
        }
    }
}
