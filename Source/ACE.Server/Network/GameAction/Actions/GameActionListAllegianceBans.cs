namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionListAllegianceBans
    {
        [GameAction(GameActionType.ListAllegianceBans)]
        public static void Handle(ClientMessage message, ISession session)
        {
            session.Player.HandleActionListAllegianceBans();
        }
    }
}
