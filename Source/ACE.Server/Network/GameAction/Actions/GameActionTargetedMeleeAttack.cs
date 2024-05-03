namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionTargetedMeleeAttack
    {
        [GameAction(GameActionType.TargetedMeleeAttack)]
        public static void Handle(ClientMessage message, ISession session)
        {
            var targetGuid = message.Payload.ReadGuid(session);
            var attackHeight = message.Payload.ReadUInt32();
            var powerLevel = message.Payload.ReadSingle();

            session.Player.HandleActionTargetedMeleeAttack(targetGuid, attackHeight, powerLevel);
        }
    }
}
