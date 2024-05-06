using ACE.Entity;
using ACE.Entity.Enum;

namespace ACE.Server.Network.GameEvent.Events
{
    /// <summary>
    /// Chess piece move response
    /// </summary>
    public class GameEventMoveResponse : GameEventMessage
    {
        public GameEventMoveResponse(ISession session, ObjectGuid boardGuid, ChessMoveResult result)
            : base(GameEventType.MoveResponse, GameMessageGroup.UIQueue, session, 12)
        {
            Writer.Write(boardGuid.ClientGUID);
            Writer.Write((int)result);
        }
    }
}
