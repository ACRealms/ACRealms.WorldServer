namespace ACE.Server.Network.GameAction.Actions
{
    /// <summary>
    /// Skips a chess move
    /// </summary>
    public static class GameActionChessMovePass
    {
        [GameAction(GameActionType.ChessMovePass)]
        public static void Handle(ClientMessage message, ISession session)
        {
            session.Player.HandleActionChessMovePass();
        }
    }
}
