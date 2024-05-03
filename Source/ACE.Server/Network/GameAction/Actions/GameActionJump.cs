using ACE.Server.Network.Structure;

namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionJump
    {
        [GameAction(GameActionType.Jump)]
        public static void Handle(ClientMessage message, ISession session)
        {
            var jumpPack = new JumpPack(message.Payload);

            var objectGuid = message.Payload.ReadGuid(session);
            var spellId = message.Payload.ReadUInt32();

            session.Player.HandleActionJump(jumpPack);

            if (session.Player.IsPlayerMovingTo)
                session.Player.StopExistingMoveToChains();

            if (session.Player.IsPlayerMovingTo2)
                session.Player.StopExistingMoveToChains2();
        }
    }
}
