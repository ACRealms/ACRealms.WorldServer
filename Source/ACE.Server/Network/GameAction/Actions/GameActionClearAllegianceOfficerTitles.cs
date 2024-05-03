namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionClearAllegianceOfficerTitles
    {
        [GameAction(GameActionType.ClearAllegianceOfficerTitles)]
        public static void Handle(ClientMessage message, ISession session)
        {
            session.Player.HandleActionClearAllegianceOfficerTitles();
        }
    }
}
