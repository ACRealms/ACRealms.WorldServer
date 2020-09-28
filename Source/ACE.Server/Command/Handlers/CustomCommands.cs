using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Server.Network;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.WorldObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACE.Server.Command.Handlers
{
    public static class CustomCommands
    {
        [CommandHandler("telerealm", AccessLevel.Developer, CommandHandlerFlag.RequiresWorld, 0, "Teleports the current player to another realm.")]
        public static void HandleMoveRealm(Session session, params string[] parameters)
        {
            if (parameters.Length < 1)
                return;
            if (!ushort.TryParse(parameters[0], out var realmid))
                return;

            var pos = session.Player.GetPosition(PositionType.Location);
            var newpos = new Position(pos);
            newpos.SetToDefaultRealmInstance(realmid);

            session.Player.Teleport(newpos);
            var positionMessage = new GameMessageSystemChat($"Teleporting to realm {realmid}.", ChatMessageType.Broadcast);
            session.Network.EnqueueSend(positionMessage);
        }
    }
}
