
namespace ACE.Server.Network.GameAction.Actions
{
    /// <summary>
    /// This method handles the Game Action F7B1 - 0x0055 Stackable Split to Container.   This is sent to the server when the player
    /// splits a stack in a container.   There are different messages for split to 3D (world) and to split to wield.   We get from the client
    /// the guid of the item stack we are about to split, the container Id, the place 0 - MaxPackCapacity ie the slot in the container we are trying to split to
    /// and finally we get the amount we are trying to split out. Og II
    /// </summary>
    public static class GameActionStackableSplitToContainer
    {
        [GameAction(GameActionType.StackableSplitToContainer)]
        public static void Handle(ClientMessage message, ISession session)
        {
            // Read in the applicable data.
            var stackId = message.Payload.ReadGuid(session);
            var containerId = message.Payload.ReadGuid(session);
            int place = message.Payload.ReadInt32();
            int amount = message.Payload.ReadInt32();

            session.Player.HandleActionStackableSplitToContainer(stackId, containerId, place, amount);
        }
    }
}
