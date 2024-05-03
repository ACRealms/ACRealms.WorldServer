namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionClearAllegianceName
    {
        [GameAction(GameActionType.ClearAllegianceName)]
        public static void Handle(ClientMessage message, ISession session)
        {
            session.Player.HandleActionClearAllegianceName();
        }
    }
}
