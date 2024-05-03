using ACE.Entity.Enum;

namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventConfirmationRequest : GameEventMessage
    {
        public GameEventConfirmationRequest(ISession session, ConfirmationType confirmationType, uint context, string text)
            : base(GameEventType.CharacterConfirmationRequest, GameMessageGroup.UIQueue, session)
        {
            Writer.Write((uint)confirmationType);
            Writer.Write(context);
            Writer.WriteString16L(text);
        }
    }
}
