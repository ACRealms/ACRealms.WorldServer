
namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionAddToTrade
    {
        [GameAction(GameActionType.AddToTrade)]
        public static void Handle(ClientMessage message, ISession session)
        {
            var itemGuid = message.Payload.ReadGuid(session);
            var tradeSlot = message.Payload.ReadUInt32();

            session.Player.HandleActionAddToTrade(itemGuid, tradeSlot);
        }
    }
}
