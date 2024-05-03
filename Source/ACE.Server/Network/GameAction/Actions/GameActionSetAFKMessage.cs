using ACE.Common.Extensions;

namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionSetAFKMessage
    {
        [GameAction(GameActionType.SetAfkMessage)]
        public static void Handle(ClientMessage message, ISession session)
        {
            var msg = message.Payload.ReadString16L();

            session.Player.HandleActionSetAFKMessage(msg);
        }
    }
}
