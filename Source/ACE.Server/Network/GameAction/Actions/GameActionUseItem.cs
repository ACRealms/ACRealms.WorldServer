using System;

namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionUseItem
    {
        [GameAction(GameActionType.Use)]
        public static void Handle(ClientMessage message, Session session)
        {
            var itemGuid = message.Payload.ReadGuid(session);

            //Console.WriteLine($"{session.Player.Name}.GameAction 0x36 - Use({itemGuid:X8})");

            session.Player.HandleActionUseItem(itemGuid);
        }
    }
}
