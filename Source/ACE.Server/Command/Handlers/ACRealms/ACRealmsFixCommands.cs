using ACE.Entity.Enum;
using ACE.Server.Entity;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.Realms;
using ACE.Server.WorldObjects;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Command.Handlers.ACRealms
{
    public static class ACRealmsFixCommands
    {
        [CommandHandler("acr-fix-null-sanctuary", AccessLevel.Developer, CommandHandlerFlag.RequiresWorld, 0,
            "Finds all characters with a null sanctuary (lifestone), sets them to the current location.")]
        public static void HandleACRFixNullSanctuary(ISession session, params string[] parameters)
        {
            // This command is to fix an issue with character creation on previous versions of ACRealms where the sanctuary location would not be set unless using the realm selector.
            if (parameters.Length != 1 || parameters[0] != "CONFIRM")
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Warning: This command is not undoable and will set all player lifestones to your current location (your location, not the player location) if the player does not have a lifestone set. To confirm, use /acr-fix-null-sanctuary CONFIRM", ChatMessageType.Broadcast);
                return;
            }

            // the underlying type is LocalPosition but using SetPositionUnsafe can be done with an instanced position for an offline player
            // the instance id will just be ignored when deserializing
            var adminLoc = new InstancedPosition(session.Player.Location);
            var adminLocLocal = adminLoc.AsLocalPosition();

            int count = 0;
            foreach(var player in PlayerManager.GetAllPlayers())
            {
                if (player is OfflinePlayer offline)
                {
                    if (offline.GetLocalPositionUnsafe(ACE.Entity.Enum.Properties.PositionType.Sanctuary) != null)
                        continue;
                    offline.SetPositionUnsafe(ACE.Entity.Enum.Properties.PositionType.Sanctuary, adminLoc);
                }
                else if (player is Player online)
                {
                    if (online.Sanctuary != null)
                        continue;
                    online.Sanctuary = adminLocLocal;
                }
                player.SaveBiotaToDatabase();
                CommandHandlerHelper.WriteOutputInfo(session, $"Set sanctuary location for {player.Name}.");
                count++;
            }

            CommandHandlerHelper.WriteOutputInfo(session, $"Completed acr-fix-null-sanctuary. Fixed {count} characters");
        }
    }
}
