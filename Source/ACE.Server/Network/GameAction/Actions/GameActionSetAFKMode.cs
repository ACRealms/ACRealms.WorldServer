using System;

namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionSetAFKMode
    {
        [GameAction(GameActionType.SetAfkMode)]
        public static void Handle(ClientMessage message, ISession session)
        {
            var afk = Convert.ToBoolean(message.Payload.ReadUInt32());

            session.Player.HandleActionSetAFKMode(afk);
        }
    }
}
