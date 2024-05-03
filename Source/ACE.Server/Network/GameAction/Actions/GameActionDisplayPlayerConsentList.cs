
namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionDisplayPlayerConsentList
    {
        [GameAction(GameActionType.DisplayPlayerConsentList)]
        public static void Handle(ClientMessage message, ISession session)
        {
            session.Player.HandleActionDisplayPlayerConsentList();
        }
    }
}
