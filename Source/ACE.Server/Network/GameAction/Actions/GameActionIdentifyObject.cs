
namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionIdentifyObject
    {
        [GameAction(GameActionType.IdentifyObject)]
        public static void Handle(ClientMessage message, Session session)
        {
            var objectGuid = message.Payload.ReadGuid(session);

            session.Player.HandleActionIdentifyObject(objectGuid);
        }
    }
}
