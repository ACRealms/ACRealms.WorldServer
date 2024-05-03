using ACE.Entity;

namespace ACE.Server.Network.GameEvent.Events
{
    /// <summary>
    /// End of chess game
    /// </summary>
    public class GameEventGameOver : GameEventMessage
    {
        public GameEventGameOver(ISession session, ObjectGuid boardGuid, int teamWinner)
            : base(GameEventType.GameOver, GameMessageGroup.UIQueue, session)
        {
            Writer.Write(boardGuid.ClientGUID);
            Writer.Write(teamWinner);
        }
    }
}
