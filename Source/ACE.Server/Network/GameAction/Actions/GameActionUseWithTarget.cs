using System;

namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionUseWithTarget
    {
        [GameAction(GameActionType.UseWithTarget)]
        public static void Handle(ClientMessage message, Session session)
        {
            var sourceObjectGuid = message.Payload.ReadGuid(session);
            var targetObjectGuid = message.Payload.ReadGuid(session);

            //Console.WriteLine($"{session.Player.Name}.UseWithTarget({sourceObjectGuid:X8}, {targetObjectGuid:X8})");

            session.Player.HandleActionUseWithTarget(sourceObjectGuid, targetObjectGuid);
        }
    }
}
