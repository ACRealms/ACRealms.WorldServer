
namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionDropItem
    {
        [GameAction(GameActionType.DropItem)]

        public static void Handle(ClientMessage message, ISession session)
        {
            var itemGuid = message.Payload.ReadGuid(session);

            session.Player.HandleActionDropItem(itemGuid);
        }
    }
}
