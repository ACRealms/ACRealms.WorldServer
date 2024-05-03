namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionTeleToLifestone
    {
        [GameAction(GameActionType.TeleToLifestone)]
        public static void Handle(ClientMessage message, ISession session)
        {
            session.Player.HandleActionTeleToLifestone();
        }
    }
}
