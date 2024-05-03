namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionListAllegianceOfficers
    {
        [GameAction(GameActionType.ListAllegianceOfficers)]
        public static void Handle(ClientMessage message, ISession session)
        {
            session.Player.HandleActionListAllegianceOfficers();
        }
    }
}
