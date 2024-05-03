namespace ACE.Server.Network.GameAction.Actions
{
    /// <summary>
    /// Teleports a player killer to the PK arena
    /// </summary>
    public static class GameActionTeleToPkArena
    {
        [GameAction(GameActionType.TeleToPkArena)]
        public static void Handle(ClientMessage message, ISession session)
        {
            session.Player.HandleActionTeleToPkArena();
        }
    }
}
