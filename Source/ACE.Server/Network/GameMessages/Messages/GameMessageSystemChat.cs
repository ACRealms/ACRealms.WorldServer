using ACE.Entity;
using ACE.Entity.Enum;

namespace ACE.Server.Network.GameMessages.Messages
{
    public class GameMessageSystemChat : GameMessage
    {
        public string Message { get; init; }
        public ChatMessageType ChatMessageType { get; init; }

        public GameMessageSystemChat(string message, ChatMessageType chatMessageType)
            : base(GameMessageOpcode.ServerMessage, GameMessageGroup.UIQueue)
        {
            if (TestMode)
            {
                Message = message;
                ChatMessageType = chatMessageType;
            }

            Writer.WriteString16L(message);
            Writer.Write((int)chatMessageType);
        }
    }
}
