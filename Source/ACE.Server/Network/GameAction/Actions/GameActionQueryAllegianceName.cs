namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionQueryAllegianceName
    {
        [GameAction(GameActionType.QueryAllegianceName)]
        public static void Handle(ClientMessage message, ISession session)
        {
            session.Player.HandleActionQueryAllegianceName();
        }
    }
}
