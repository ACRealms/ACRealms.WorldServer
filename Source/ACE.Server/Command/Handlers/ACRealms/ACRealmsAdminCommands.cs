using ACE.Database;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Server.Entity;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace ACE.Server.Command.Handlers.ACRealms
{
    public static class ACRealmsAdminCommands
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

        [CommandHandler("evict-player-houses", AccessLevel.Admin, CommandHandlerFlag.None, 1, "Evicts a player from all of their housing",
            "Character name")]
        public static void HandleEvictHouse(ISession session, params string[] parameters)
        {
            var name = string.Join(" ", parameters);

            var characterName = name.TrimStart(' ').TrimEnd(' ');
            if (characterName.StartsWith("+"))
                characterName = characterName.Substring(1);

            var player = PlayerManager.FindByName(characterName);
            if (player == null)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Character not found: '{characterName}'", ChatMessageType.Broadcast);
                return;
            }

            var houses = HouseManager.GetCharacterHouses(player.Guid.Full);
            foreach (var house in houses)
            {
                try
                {
                    var actualHouse = HouseManager.GetHouseSynchronously(house.Guid);
                    HouseManager.HandleEviction(actualHouse, player.Guid.Full, false, true, movingHouse: false);
                    CommandHandlerHelper.WriteOutputInfo(session, $"Evicted '{player.Name}' from house {house.Guid}", ChatMessageType.Broadcast);
                }
                catch (Exception)
                {
                    CommandHandlerHelper.WriteOutputInfo(session, $"Failed to evict '{player.Name}' from house {house.Guid}", ChatMessageType.Broadcast);
                    throw;
                }
            }
        }


        [CommandHandler("move-player-house-instance", AccessLevel.Admin, CommandHandlerFlag.None, 3, "Moves houses owned by a player to a new realm",
            "Character name, Old Realm Name (optional), New Realm Name")]
        public static void HandleMovePlayerHouseInstance(ISession session, params string[] parameters)
        {
            if (!string.Join(" ", parameters).Contains(','))
            {
                CommandHandlerHelper.WriteOutputInfo(session, "You must include the character name followed by a comma and then the other parameters.\n Example: @move-player-house-instance Character Name, InstanceID", ChatMessageType.Broadcast);
                return;
            }

            var names = string.Join(" ", parameters).Split(",");

            var characterName = names[0].TrimStart(' ').TrimEnd(' ');
            if (characterName.StartsWith("+"))
                characterName = characterName.Substring(1);

            var player = PlayerManager.FindByName(characterName);
            if (player == null)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Character not found: '{characterName}'", ChatMessageType.Broadcast);
                return;
            }

            if (names.Length < 2)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Must specify either the old realm name, and new realm name, or just the new realm name to move houses from all other instances: '{characterName}'", ChatMessageType.Broadcast);
                return;
            }
            if (names.Length > 3)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Too many commas in command", ChatMessageType.Broadcast);
                return;
            }

            string? oldRealmName = null;
            string newRealmName;
            if (names.Length == 2)
                newRealmName = names[1].Trim();
            else
            {
                oldRealmName = names[0].Trim();
                newRealmName = names[2].Trim();
            }

            Func<string, WorldRealm?> getRealm = (realmName) =>
            {
                WorldRealm realm;
                realm = RealmManager.GetRealmByName(realmName, includeRulesets: false);

                if (realm == null)
                    CommandHandlerHelper.WriteOutputInfo(session, $"Realm '{realmName}' not found.", ChatMessageType.Broadcast);
                return realm;
            };

            WorldRealm? oldRealm = null;
            WorldRealm newRealm;
            if (oldRealmName != null)
            {
                var r = getRealm(oldRealmName);
                if (r == null)
                    return;
                else
                    oldRealm = r;
            }
            var nr = getRealm(newRealmName);
            if (nr == null)
                return;
            else
                newRealm = nr;

            var houses = HouseManager.GetCharacterHouses(player.Guid.Full);
            if (houses.Count == 0)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Character '{characterName}' has no houses.", ChatMessageType.Broadcast);
                return;
            }

            if (oldRealm != null)
            {
                houses = houses.Where(x => x.Guid.StaticRealmID == oldRealm.Realm.Id).ToList();
                if (houses.Count == 0)
                {
                    CommandHandlerHelper.WriteOutputInfo(session, $"Character '{characterName}' has no houses in realm '{oldRealm.Realm.Name}'.", ChatMessageType.Broadcast);
                    return;
                }
            }

            var housesWithTargets = houses.Select(h =>
                new {
                    House = h,
                    TargetInstanceId = newRealm.StandardRules.GetDefaultInstanceID(player, h.Location.AsLocalPosition())
                }
            ).Where(t => t.House.Guid != new ObjectGuid(t.House.Guid.ClientGUID, t.TargetInstanceId)).ToList();

            if (housesWithTargets.Count == 0)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"All houses owned by '{characterName}' are already located on the player's default instance ID for the realm '{newRealm.Realm.Name}'");
                return;
            }

            foreach(var jobInfo in housesWithTargets)
            {
                var srcHouse = HouseManager.GetHouseSynchronously(jobInfo.House.Guid, true, isRealmMigration: true);
                var oldRealmId = srcHouse.Guid.StaticRealmID;
                var srcRealmInfo = oldRealmId.HasValue ? RealmManager.GetRealm(oldRealmId.Value, false)?.Realm?.Name : $"[ instance {srcHouse.Guid.Instance ?? 0} ]";

                var targetHouseGuid = new ObjectGuid(srcHouse.Guid.ClientGUID, jobInfo.TargetInstanceId);
                var destHouse = HouseManager.GetHouseSynchronously(targetHouseGuid, true, isRealmMigration: true);

                HouseManager.ChangeOwnedHouse(player, srcHouse, destHouse);
            }
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
            if (characterName.StartsWith("+"))
                characterName = characterName.Substring(1);

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

        // GetAllPlayers is really not performant...
        [CommandHandler("allplayersinrealmid", AccessLevel.Admin, CommandHandlerFlag.None, 1,
            "Lists all players with a given home realm id")]
        public static void HandleAllPlayersInRealmId(ISession session, params string[] parameters)
        {
            if (parameters.Length != 1)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Must pass exactly one argument.", ChatMessageType.Broadcast);
                return;
            }

            if (!int.TryParse(parameters[0], out var rawRealmId))
            {
                // Didn't have the time to support realm name for this command yet, and I'd rather have some kind of generic parser improvements for these commands soon
                CommandHandlerHelper.WriteOutputInfo(session, $"Invalid parameter: must pass an integer (realm ID).", ChatMessageType.Broadcast);
                return;
            }
            var players = PlayerManager.GetAllPlayers().Select(x => new {
                x.Name,
                HomeRealm = x.GetProperty(PropertyInt.HomeRealm)
            });

            if (rawRealmId == 0)
                players = players.Where(x => x.HomeRealm == 0 || x.HomeRealm == null);
            else
                players = players.Where(x => x.HomeRealm == rawRealmId);
            players = players.OrderBy(x => x.Name).ToList();

            string realmLabel = rawRealmId switch
            {
                0 => ReservedRealm.NULL.ToString(),
                < 0 or > 0x7FFF => $"[ Invalid Realm {rawRealmId} ]",
                _ => RealmManager.GetRealm((ushort)rawRealmId, includeRulesets: false)?.Realm?.Name ?? $"[ Missing Realm {rawRealmId} ]"
            };

            StringBuilder sb = new();
            sb.AppendLine($"Characters assigned to realm {realmLabel}:");

            foreach (var p in players)
                sb.AppendLine(p.Name);
            CommandHandlerHelper.WriteOutputInfo(session, sb.ToString());
        }

        // GetAllPlayers is really not performant...
        [CommandHandler("showallplayerhomerealms", AccessLevel.Admin, CommandHandlerFlag.None, 0,
            "Shows total (including offline) population for each home realm")]
        public static void HandleShowAllPlayerHomeRealms(ISession session, params string[] parameters)
        {
            var counts = PlayerManager.GetAllPlayers().GroupBy(p => p.GetProperty(PropertyInt.HomeRealm)).ToDictionary(g => g.Key ?? -1, g => g.Count()).OrderBy(x => x.Key);
            StringBuilder sb = new();
            sb.AppendLine("Realm Total Character Counts");
            foreach (var group in counts)
            {
                string label = group.Key switch
                {
                    -1 => "Undefined (use realm ID 0 to migrate)",
                    0 => ReservedRealm.NULL.ToString(),
                    < 0 or > 0x7FFF => "Invalid (Out of range)",
                    _ => RealmManager.GetRealm((ushort)group.Key, false)?.Realm?.Name ?? "Invalid (Realm Missing)"
                };
                sb.AppendLine($"{(group.Key == -1 ? "null" : group.Key)} ({label}): {group.Value}");
            }
            CommandHandlerHelper.WriteOutputInfo(session, sb.ToString());
        }

        [CommandHandler("mass-transfer-home-realm", AccessLevel.Admin, CommandHandlerFlag.None, 2,
            "Transfers all characters from a home realm for a character. It is strongly recommended to run this without any players logged in!",
            "Previous Realm ID or Realm Name, New Realm ID or Realm Name")]
        public static void HandleMassTransferHomeRealm(ISession session, params string[] parameters)
        {
            if (WorldManager.WorldStatus == WorldManager.WorldStatusState.Open)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"World must be closed before running this command.", ChatMessageType.Broadcast);
                return;
            }

            if (!string.Join(" ", parameters).Contains(','))
            {
                CommandHandlerHelper.WriteOutputInfo(session, "You must include the realm name or ID followed by a comma and then the other realm name or ID.\n Example: @mass-transfer-home-realm Modern Realm, Classic", ChatMessageType.Broadcast);
                return;
            }

            var names = string.Join(" ", parameters).Split(",");

            if (names.Length != 2)
            {
                CommandHandlerHelper.WriteOutputInfo(session, "Failed to parse parameters. Requires exactly one comma", ChatMessageType.Broadcast);
                return;
            }

            var realmNameOrId = names[0].Trim();

            WorldRealm? oldRealm;
            WorldRealm newRealm;
            ushort oldRealmId;
            if (!ushort.TryParse(realmNameOrId, out oldRealmId))
            {
                oldRealm = RealmManager.GetRealmByName(realmNameOrId, includeRulesets: false);
                if (oldRealm == null)
                {
                    CommandHandlerHelper.WriteOutputInfo(session, $"Error, cannot migrate home realm. Old realm '{realmNameOrId}' not found. If passing a raw realm id for the old realm (but not the new realm), this check is not enforced.", ChatMessageType.Broadcast);
                    return;
                }
                oldRealmId = oldRealm.Realm.Id;
            }

            var newRealmNameOrId = names[1].Trim();
            if (ushort.TryParse(newRealmNameOrId, out var newRealmId))
                newRealm = RealmManager.GetRealm(newRealmId, includeRulesets: false);
            else
                newRealm = RealmManager.GetRealmByName(newRealmNameOrId, includeRulesets: false);

            if (newRealm == null)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Error, cannot migrate home realm. New realm '{realmNameOrId}' not found", ChatMessageType.Broadcast);
                return;
            }

            var players = PlayerManager.GetAllPlayers();
            foreach(var p in players)
            {
                var playerHomeRealmId = p.GetProperty(PropertyInt.HomeRealm) ?? 0;
                if (playerHomeRealmId != oldRealmId)
                    continue;
                RealmManager.SetHomeRealm(p, newRealm);
            }
            CommandHandlerHelper.WriteOutputInfo(session, $"Completed transfer of characters from realm {oldRealmId} to {newRealm.Realm.Name}.", ChatMessageType.Broadcast);
        }

        [CommandHandler("force-all-characters-to-new-home-realm", AccessLevel.Admin, CommandHandlerFlag.None, 1,
            "Transfers all characters to the specified realm, regardless of where they are now. It is strongly recommended to run this without any players logged in!",
            "New Realm ID or Realm Name")]
        public static void HandleForceAllCharactersToNewHomeRealm(ISession session, params string[] parameters)
        {
            if (WorldManager.WorldStatus == WorldManager.WorldStatusState.Open)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"World must be closed before running this command.", ChatMessageType.Broadcast);
                return;
            }

            var names = string.Join(" ", parameters).Split(",");

            if (names.Length != 1)
            {
                CommandHandlerHelper.WriteOutputInfo(session, "Failed to parse parameters. Requires exactly one name, no commas", ChatMessageType.Broadcast);
                return;
            }

            var newRealmNameOrId = names[0].Trim();
            WorldRealm newRealm;
            if (ushort.TryParse(newRealmNameOrId, out var newRealmId))
                newRealm = RealmManager.GetRealm(newRealmId, includeRulesets: false);
            else
                newRealm = RealmManager.GetRealmByName(newRealmNameOrId, includeRulesets: false);

            if (newRealm == null)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Error, cannot migrate home realm. New realm '{newRealmNameOrId}' not found", ChatMessageType.Broadcast);
                return;
            }

            var players = PlayerManager.GetAllPlayers();
            foreach (var p in players)
            {
                RealmManager.SetHomeRealm(p, newRealm);
            }
            CommandHandlerHelper.WriteOutputInfo(session, $"Completed transfer of characters to {newRealm.Realm.Name}.", ChatMessageType.Broadcast);
        }

#if METHODSTATISTICS
        [CommandHandler("dump-method-statistics", AccessLevel.Admin, CommandHandlerFlag.None, 0, "Dump method statistics")]
        public static void HandleDumpMethodStatistics(ISession session, params string[] parameters)
        {
            System.IO.File.WriteAllText("method-statistics.txt",PerfStats.MethodStatistics.Dump());
            CommandHandlerHelper.WriteOutputInfo(session, $"Dumped method statistics to method-statistics.txt", ChatMessageType.Broadcast);
        }
#endif

    }
}
