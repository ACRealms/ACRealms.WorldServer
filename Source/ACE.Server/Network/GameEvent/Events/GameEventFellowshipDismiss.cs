using ACE.Server.WorldObjects;

namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventFellowshipDismiss : GameEventMessage
    {
        public GameEventFellowshipDismiss(ISession session, Player dismissedPlayer)
            : base(GameEventType.FellowshipDismiss, GameMessageGroup.UIQueue, session, 8)
        {
            // can be both S2C and C2S?
            Writer.Write(dismissedPlayer.Guid.ClientGUID);
        }
    }
}
