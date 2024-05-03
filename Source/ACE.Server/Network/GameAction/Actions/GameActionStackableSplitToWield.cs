
using ACE.Entity;
using ACE.Entity.Enum;

namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionStackableSplitToWield
    {
        [GameAction(GameActionType.StackableSplitToWield)]
        public static void Handle(ClientMessage message, Session session)
        {
            // Read in the applicable data.
            ObjectGuid stackId = message.Payload.ReadGuid(session);
            var location = (EquipMask)message.Payload.ReadInt32();
            int amount = message.Payload.ReadInt32();

            session.Player.HandleActionStackableSplitToWield(stackId, location, amount);
        }
    }
}
