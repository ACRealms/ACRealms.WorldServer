using System.Collections.Generic;

namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionCreateTinkeringTool
    {
        [GameAction(GameActionType.CreateTinkeringTool)]
        public static void Handle(ClientMessage message, ISession session)
        {
            var toolGuid = message.Payload.ReadGuid(session);
            uint itemcount = message.Payload.ReadUInt32();

            var items = new List<uint>();

            while (itemcount > 0)
            {
                itemcount--;
                items.Add(message.Payload.ReadUInt32());
            }

            session.Player.HandleSalvaging(toolGuid, items);
        }
    }
}
