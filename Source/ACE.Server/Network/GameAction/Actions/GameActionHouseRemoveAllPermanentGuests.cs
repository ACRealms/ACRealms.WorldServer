using System;

namespace ACE.Server.Network.GameAction.Actions
{
    /// <summary>
    /// Removes all guests, /house guest remove_all
    /// </summary>
    public static class GameActionHouseRemoveAllPermanentGuests
    {
        [GameAction(GameActionType.RemoveAllPermanentGuests)]
        public static void Handle(ClientMessage message, ISession session)
        {
            //Console.WriteLine("Received 0x25E - House - RemoveAllPermanentGuests");

            session.Player.HandleActionRemoveAllGuests();
        }
    }
}
