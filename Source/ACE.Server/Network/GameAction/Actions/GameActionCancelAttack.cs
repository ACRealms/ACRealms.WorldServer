using ACE.Entity;

namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionCancelAttack
    {
        [GameAction(GameActionType.CancelAttack)]
        public static void Handle(ClientMessage message, ISession session)
        {
            session.Player.HandleActionCancelAttack();
        }
    }
}
