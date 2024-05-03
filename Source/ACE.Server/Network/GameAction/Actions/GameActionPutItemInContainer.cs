
namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionPutItemInContainer
    {
        [GameAction(GameActionType.PutItemInContainer)]
        public static void Handle(ClientMessage message, ISession session)
        {
            var itemGuid = message.Payload.ReadGuid(session);
            var containerGuid = message.Payload.ReadGuid(session);
            var placement = message.Payload.ReadInt32();

            session.Player.HandleActionPutItemInContainer(itemGuid, containerGuid, placement);
        }
    }
}
