using ACE.Database;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
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
        [CommandHandler("get-home-realm", AccessLevel.Admin, CommandHandlerFlag.None, 1,
            "Get a home realm for a character.",
            "Character name")]
        public static void HandleGetHomeRealm(ISession session, params string[] parameters)
        {
            var name = string.Join(" ", parameters);

            var characterName = name.TrimStart(' ').TrimEnd(' ');
            if (characterName.StartsWith("+"))
                characterName = characterName.Substring(1);

            var onlinePlayer = PlayerManager.GetOnlinePlayer(characterName);
            var offlinePlayer = PlayerManager.GetOfflinePlayer(characterName);

            int? homeRealmRawId;
            if (onlinePlayer != null)
                homeRealmRawId = onlinePlayer.GetProperty(PropertyInt.HomeRealm);
            else if (offlinePlayer != null)
                homeRealmRawId = offlinePlayer.GetProperty(PropertyInt.HomeRealm);
            else
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Character not found: '{characterName}'", ChatMessageType.Broadcast);
                return;
            }

            var prefix = $"Home Realm for '{characterName}':";

            if (homeRealmRawId == null)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"{prefix} [not present]", ChatMessageType.Broadcast);
                return;
            }
            if (homeRealmRawId.Value < 0 || homeRealmRawId > 0x7FFF)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"{prefix} {homeRealmRawId} (invalid, out of range)", ChatMessageType.Broadcast);
                return;
            }
            var homeRealmId = (ushort)homeRealmRawId;

            if (homeRealmId == 0)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"{prefix} {homeRealmId} (invalid, NULL realm not allowed)", ChatMessageType.Broadcast);
                return;
            }

            var realm = RealmManager.GetRealm(homeRealmId, includeRulesets: false);
            if (realm == null)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"{prefix} {homeRealmId} (invalid, valid realm not found)", ChatMessageType.Broadcast);
                return;
            }

            CommandHandlerHelper.WriteOutputInfo(session, $"{prefix} {homeRealmId} ({realm.Realm.Name})", ChatMessageType.Broadcast);
            return;
        }


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
