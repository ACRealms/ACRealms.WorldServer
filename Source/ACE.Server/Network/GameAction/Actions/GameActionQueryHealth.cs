
namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionQueryHealth
    {
        [GameAction(GameActionType.QueryHealth)]
        public static void Handle(ClientMessage message, Session session)
        {
            var objectGuid = message.Payload.ReadGuid(session);

            session.Player.HandleActionQueryHealth(objectGuid);
        }
    }
}
