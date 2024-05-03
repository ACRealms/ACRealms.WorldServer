namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventUpdateHealth : GameEventMessage
    {
        public GameEventUpdateHealth(ISession session, uint objectid, float health)
            : base(GameEventType.UpdateHealth, GameMessageGroup.UIQueue, session)
        {
            Writer.Write(objectid);
            Writer.Write(health);
        }
    }
}
