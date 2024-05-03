
namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionRemoveAllFriends
    {
        [GameAction(GameActionType.RemoveAllFriends)]
        public static void Handle(ClientMessage message, ISession session)
        {
            session.Player.HandleActionRemoveAllFriends();
        }
    }
}
