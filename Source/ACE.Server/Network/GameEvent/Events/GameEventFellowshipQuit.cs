namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventFellowshipQuit : GameEventMessage
    {
        public GameEventFellowshipQuit(ISession session, uint playerId)
            : base(GameEventType.FellowshipQuit, GameMessageGroup.UIQueue, session)
        {
            Writer.Write(playerId);
        }
    }
}
