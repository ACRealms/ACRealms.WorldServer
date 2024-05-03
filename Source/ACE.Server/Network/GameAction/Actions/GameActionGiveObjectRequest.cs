
namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionGiveObjectRequest
    {
        [GameAction(GameActionType.GiveObjectRequest)]
        public static void Handle(ClientMessage message, ISession session)
        {
            var targetGuid = message.Payload.ReadGuid(session);
            var objectGuid = message.Payload.ReadGuid(session);
            int amount = message.Payload.ReadInt32();

            session.Player.HandleActionGiveObjectRequest(targetGuid, objectGuid, amount);
        }
    }
}
