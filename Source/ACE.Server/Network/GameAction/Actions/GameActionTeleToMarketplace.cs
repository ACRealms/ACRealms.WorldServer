namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionTeleToMarketPlace
    {
        [GameAction(GameActionType.TeleToMarketPlace)]
        public static void Handle(ClientMessage clientMessage, ISession session)
        {
            session.Player.HandleActionTeleToMarketPlace();
        }
    }
}
