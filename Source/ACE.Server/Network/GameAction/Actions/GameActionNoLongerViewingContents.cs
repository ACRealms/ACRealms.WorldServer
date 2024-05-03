
namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionNoLongerViewingContents
    {
        [GameAction(GameActionType.NoLongerViewingContents)]
        public static void Handle(ClientMessage message, ISession session)
        {
            var objectGuid = message.Payload.ReadGuid(session);

            session.Player.HandleActionNoLongerViewingContents(objectGuid);
        }
    }
}
