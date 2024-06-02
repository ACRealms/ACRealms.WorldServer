using ACE.Database;
using ACE.Entity.Enum;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Command.Handlers.ACRealms
{
    public class ACRealmsAdminCommands
    {
        [CommandHandler("set-home-realm", AccessLevel.Admin, CommandHandlerFlag.None, 2,
            "Set a home realm for a character.",
            "Character name, Realm ID or Realm Name")]
        public static void HandleSetHomeRealm(ISession session, params string[] parameters)
        {
            if (!string.Join(" ", parameters).Contains(','))
            {
                CommandHandlerHelper.WriteOutputInfo(session, "Error, cannot set home realm. You must include the character name followed by a comma and then the realm name or ID.\n Example: @set-home-realm Character Name, Realm Name", ChatMessageType.Broadcast);
                return;
            }

            var names = string.Join(" ", parameters).Split(",");

            var characterName = names[0].TrimStart(' ').TrimEnd(' ');
            var realmNameOrId = names[1].TrimStart(' ').TrimEnd(' ');

            WorldRealm realm;
            if (ushort.TryParse(realmNameOrId, out var realmId))
                realm = RealmManager.GetRealm(realmId, includeRulesets: false);
            else
                realm = RealmManager.GetRealmByName(realmNameOrId, includeRulesets: false);

            if (realm == null)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Error, cannot set home realm. Realm '{realmNameOrId}' not found.", ChatMessageType.Broadcast);
                return;
            }

            if (characterName.StartsWith("+"))
                characterName = characterName.Substring(1);

            var onlinePlayer = PlayerManager.GetOnlinePlayer(characterName);
            var offlinePlayer = PlayerManager.GetOfflinePlayer(characterName);

            if (onlinePlayer != null)
            {
                ChatPacket.SendServerMessage(onlinePlayer.Session, $"An admin has changed your home realm to '{realm.Realm.Name}'", ChatMessageType.Broadcast);
                RealmManager.SetHomeRealm(onlinePlayer, realm.Realm.Id, false);
            }
            else if (offlinePlayer != null)
            {
                RealmManager.SetHomeRealm(offlinePlayer, realm);
            }
            else
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Error, a player named \"{characterName}\" cannot be found.", ChatMessageType.Broadcast);
                return;
            }

            CommandHandlerHelper.WriteOutputInfo(session, $"Character \"{characterName}\" assigned to home realm \"{realmNameOrId}\" successfully!", ChatMessageType.Broadcast);
        }
    }
}
