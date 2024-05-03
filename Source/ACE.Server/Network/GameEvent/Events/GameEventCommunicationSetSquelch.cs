using ACE.Server.Network.Structure;

namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventSetSquelchDB : GameEventMessage
    {
        public GameEventSetSquelchDB(ISession session, SquelchDB db)
            : base(GameEventType.SetSquelchDB, GameMessageGroup.UIQueue, session)
        {
            Writer.Write(db);
        }
    }
}
