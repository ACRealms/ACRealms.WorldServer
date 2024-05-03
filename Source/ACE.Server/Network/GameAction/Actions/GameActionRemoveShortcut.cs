
namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionRemoveShortcut
    {
        [GameAction(GameActionType.RemoveShortCut)]
        public static void Handle(ClientMessage message, ISession session)
        {
            var index = message.Payload.ReadUInt32();

            session.Player.HandleActionRemoveShortcut(index);
        }
    }
}
