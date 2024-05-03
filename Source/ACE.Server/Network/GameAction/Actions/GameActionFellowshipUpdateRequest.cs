using System;

namespace ACE.Server.Network.GameAction.Actions
{
    public class GameActionFellowshipUpdateRequest
    {
        [GameAction(GameActionType.FellowshipUpdateRequest)]
        public static void Handle(ClientMessage message, ISession session)
        {
            // indicates if fellowship panel on client is visible
            var panelOpen = Convert.ToBoolean(message.Payload.ReadInt32());

            session.Player.HandleFellowshipUpdateRequest(panelOpen);
        }
    }
}
