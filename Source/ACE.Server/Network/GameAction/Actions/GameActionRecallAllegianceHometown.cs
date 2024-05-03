namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionRecallAllegianceHometown
    {
        [GameAction(GameActionType.RecallAllegianceHometown)]
        public static void Handle(ClientMessage clientMessage, ISession session)
        {
            session.Player.HandleActionRecallAllegianceHometown();
        }
    }
}
