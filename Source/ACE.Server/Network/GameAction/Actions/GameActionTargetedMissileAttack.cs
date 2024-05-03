
namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionTargetedMissileAttack
    {
        [GameAction(GameActionType.TargetedMissileAttack)]
        public static void Handle(ClientMessage message, ISession session)
        {
            var targetGuid = message.Payload.ReadGuid(session);
            var attackHeight = message.Payload.ReadUInt32();
            var accuracyLevel = message.Payload.ReadSingle();

            session.Player.HandleActionTargetedMissileAttack(targetGuid, attackHeight, accuracyLevel);
        }
    }
}
