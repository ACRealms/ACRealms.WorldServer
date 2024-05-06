using ACE.Entity;

namespace ACE.Server.Network.GameMessages.Messages
{
    public class GameMessagePlayerCreate : GameMessage
    {
        public ObjectGuid Guid { get; init; }
        public GameMessagePlayerCreate(ObjectGuid guid) : base(GameMessageOpcode.PlayerCreate, GameMessageGroup.SmartboxQueue, 8)
        {
            if (TestMode)
                Guid = guid;

            Writer.WriteGuid(guid);
        }
    }
}
