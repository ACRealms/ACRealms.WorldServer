using ACE.Common;
using ACE.Database;
using ACE.Database.Models.Shard;
using ACE.Entity;
using ACE.Entity.ACRealms;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.Command.Handlers;
using ACE.Server.Entity;
using ACE.Server.Entity.ACRealms;
using ACE.Server.Network;
using ACE.Server.Network.Enum;
using ACE.Server.Network.GameEvent.Events;
using ACE.Server.Network.GameMessages;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Realms;
using ACE.Server.WorldObjects;
using ACRealms.DataStructures.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#nullable enable

namespace ACE.Server.Managers.ACRealms
{
    public class AeternumService : PlayerServiceBase, IPlayerManager
    {
        readonly DefaultNullConcurrentDictionary<ulong, Aeternum> primaryStore;
        protected override IReadOnlyDictionary<ulong, IPlayer> PrimaryStore { get; init; }

        ImmutableHashSet<Aeternum> onlinePlayers;
        ImmutableHashSet<Aeternum> offlinePlayers;
        ImmutableHashSet<Aeternum> allPlayers;

        internal AeternumService()
        {
            primaryStore = new DefaultNullConcurrentDictionary<ulong, Aeternum>();
            PrimaryStore = new CovariantReadOnlyDictionary<ulong, Aeternum, IPlayer>(primaryStore);
            allPlayers = [];
            offlinePlayers = [];
            onlinePlayers = [];
        }

        public void Initialize()
        {
            var results = DatabaseManager.Shard.BaseDatabase.GetAllPlayerBiotasInParallel();

            Parallel.ForEach(results, ConfigManager.Config.Server.Threading.DatabaseParallelOptions, result =>
            {
                var offlinePlayer = new OfflinePlayer(result);
                var aeternum = new Aeternum(offlinePlayer);
                offlinePlayers = offlinePlayers.Add(aeternum);
                allPlayers = allPlayers.Add(aeternum);
                primaryStore.TryAdd(offlinePlayer.Guid.Full, aeternum);
                basicPlayerNames.Add(offlinePlayer.Name, offlinePlayer.Guid.Full);
                if (offlinePlayer.Account != null)
                    playerAccounts.Add(offlinePlayer.Account.AccountId, offlinePlayer.Guid.Full);
                else
                    log.Error($"AeternumService.Initialize: couldn't find account for player {offlinePlayer.Name} ({offlinePlayer.Guid})");
            });
        }

        public override void SaveOfflinePlayersWithChanges()
        {
            lastDatabaseSave = DateTime.UtcNow;

            var biotas = new Collection<(Biota biota, ReaderWriterLockSlim rwLock)>();

            foreach (var player in offlinePlayers.Where(p => p.ChangesDetected))
            {
                if (player.ChangesDetected)
                {
                    player.SaveBiotaToDatabase(false);
                    biotas.Add((player.Biota, player.BiotaDatabaseLock));
                }
            }

            DatabaseManager.Shard.SaveBiotasInParallel(biotas, null, true);
        }

        public override void AddOfflinePlayer(Player player)
        {
            var offlinePlayer = new OfflinePlayer(player.Biota);
            var aeternum = new Aeternum(offlinePlayer);
            primaryStore.TryAdd(offlinePlayer.Guid.Full, aeternum);
            allPlayers = allPlayers.Add(aeternum);
            offlinePlayers = offlinePlayers.Add(aeternum);
            basicPlayerNames.Add(offlinePlayer.Name, offlinePlayer.Guid.Full);
            playerAccounts.Add(offlinePlayer.Account.AccountId, offlinePlayer.Guid.Full);
        }

        public Aeternum? GetAeternum(ObjectGuid guid) => GetAeternum(guid.Full);
        public Aeternum? GetAeternum(ulong guid) => primaryStore[guid];

        public override OfflinePlayer? GetOfflinePlayer(ObjectGuid guid) => GetOfflinePlayer(guid.Full);

        public override OfflinePlayer? GetOfflinePlayer(ulong guid) => GetAeternum(guid);

        public override bool DoesBasicNameExist(string name)
            => basicPlayerNames.ContainsKey(name);

        public override IImmutableList<ulong> GetPlayerGuidsByBasicName(string name)
            => basicPlayerNames[name].Value;

        public override IPlayer GetPlayerGuidByCanonicalName(CanonicalCharacterName name)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Aeternum> PlayersWithName(string name) => basicPlayerNames[name].Value.Select(guid => primaryStore[guid]!);

        public override OfflinePlayer? GetOfflinePlayer(string name)
        {
            var players = GetOfflinePlayersWithName(name);
            if (players.Count == 0)
                return null;
            if (players.Count == 1)
                return players[0];

            var not_deleted = players.FirstOrDefault(x => !x.IsDeleted && !x.IsPendingDeletion);
            if (not_deleted != null)
                return not_deleted;

            return null;
        }

        public override List<OfflinePlayer> GetOfflinePlayersWithName(string name)
            => PlayersWithName(name).Select(x => (OfflinePlayer?)x).Where(x => x != null).Cast<OfflinePlayer>().ToList();

        public override List<Player> GetOnlinePlayersWithName(string name)
            => PlayersWithName(name).Select(x => (Player?)x).Where(x => x != null).Cast<Player>().ToList();

        public override List<IPlayer> GetAllPlayers() => allPlayers.Cast<IPlayer>().ToList();

        public override Dictionary<ulong, IPlayer> GetAccountPlayers(uint accountId)
            => playerAccounts[accountId].Value.ToDictionary(g => g, g => (IPlayer)primaryStore[g]!);

        public override int GetOfflineCount() => offlinePlayers.Count;

        public override List<OfflinePlayer> GetAllOffline() => offlinePlayers.Select(x => (OfflinePlayer)x).ToList();

        public override int GetOnlineCount() => onlinePlayers.Count;

        public override Player? GetOnlinePlayer(ObjectGuid guid) => GetOnlinePlayer(guid.Full);

        public override Player? GetOnlinePlayer(ulong guid)
        {
            var p = primaryStore[guid];
            if (p.IsOnline)
                return p;
            return null;
        }

        public override Player? GetOnlinePlayer(string name)
        {
            var players = GetOnlinePlayersWithName(name);
            if (players.Count == 0)
                return null;
            if (players.Count == 1)
                return players[0];

            var not_deleted = players.FirstOrDefault(x => !x.IsDeleted && !x.IsPendingDeletion);
            if (not_deleted != null)
                return not_deleted;

            return null;
        }

        public override List<Player> GetAllOnline() => onlinePlayers.Select(x => (Player)x).ToList();

        
        public override bool SwitchPlayerFromOfflineToOnline(Player player)
        {
            SwitchPlayerFromOfflineToOnline_Synchronized(player);
            AllegianceManager.LoadPlayer(player);

            player.SendFriendStatusUpdates();

            return true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void SwitchPlayerFromOfflineToOnline_Synchronized(Player player)
        {
            var offlinePlayer = (OfflinePlayer)player.Aeternum.Player;

            offlinePlayers = offlinePlayers.Remove(player.Aeternum);
            if (offlinePlayer.ChangesDetected)
                player.ChangesDetected = true;

            player.Allegiance = offlinePlayer.Allegiance;
            player.AllegianceNode = offlinePlayer.AllegianceNode;

            player.Aeternum.SetToOnline(player);
            onlinePlayers = onlinePlayers.Add(player.Aeternum);
        }

        /// <summary>
        /// This will return true if the player was successfully added.
        /// It will return false if the player was not found in the OnlinePlayers dictionary (which should never happen), or player already exists in the OfflinePlayers dictionary (which should never happen).
        /// </summary>
        public override bool SwitchPlayerFromOnlineToOffline(Player player)
        {
            SwitchPlayerFromOnlineToOffline_Synchronized(player);

            player.SendFriendStatusUpdates(false);
            player.HandleAllegianceOnLogout();

            return true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void SwitchPlayerFromOnlineToOffline_Synchronized(Player player)
        {
            var offlinePlayer = new OfflinePlayer(player.Biota, player.Aeternum);
            offlinePlayer.Allegiance = player.Allegiance;
            offlinePlayer.AllegianceNode = player.AllegianceNode;

            onlinePlayers = onlinePlayers.Remove(player.Aeternum);
            offlinePlayers = offlinePlayers.Add(offlinePlayer.Aeternum);
        }

        public override async Task<bool> IsCharacterNameAvailableForCreation(string name)
        {
            var opts = Common.ACRealms.ACRealmsConfigManager.Config.CharacterCreationOptions;

            ushort? realmId;

            if (opts.CharacterNamesUniquePerHomeRealm)
                realmId = null;
            else if (opts.UseRealmSelector)
                realmId = 0; // Effectively makes it so only admin character names can't be used
            else
                realmId = RealmManager.ServerDefaultRealm.Realm.Id;

            return await IsCharacterNameAvailable(name, realmId);
        }

        public override async Task<bool> IsCharacterNameAvailable(string name, ushort? realmId, bool? overrideUniquePerHomeRealm = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            DatabaseManager.Shard.IsCharacterNameAvailable(name, realmId, isAvailable =>
            {
                tcs.TrySetResult(isAvailable);
            }, overrideUniquePerHomeRealm);

            return await tcs.Task;
        }

        public override void IsCharacterNameAvailable(string name, ushort? realmId, Action<bool> callback, bool? overrideUniquePerHomeRealm = null)
            => DatabaseManager.Shard.IsCharacterNameAvailable(name, realmId, callback, overrideUniquePerHomeRealm);

        public override void HandlePlayerRename(ISession session, string oldName, string newName)
        {
            var onlinePlayer = PlayerManager.GetOnlinePlayer(oldName);
            var offlinePlayer = PlayerManager.GetOfflinePlayer(oldName);
            if (onlinePlayer != null)
            {
                IsCharacterNameAvailable(newName, onlinePlayer.HomeRealm, isAvailable =>
                {
                    if (!isAvailable)
                    {
                        CommandHandlerHelper.WriteOutputInfo(session, $"Error, a player named \"{newName}\" already exists.", ChatMessageType.Broadcast);
                        return;
                    }

                    onlinePlayer.Character.Name = newName;
                    onlinePlayer.CharacterChangesDetected = true;
                    onlinePlayer.Name = newName;
                    onlinePlayer.SavePlayerToDatabase();

                    CommandHandlerHelper.WriteOutputInfo(session, $"Player named \"{oldName}\" renamed to \"{newName}\" successfully!", ChatMessageType.Broadcast);

                    onlinePlayer.Session.LogOffPlayer();
                });
            }
            else if (offlinePlayer != null)
            {
                DatabaseManager.Shard.IsCharacterNameAvailable(newName, offlinePlayer.HomeRealm, isAvailable =>
                {
                    if (!isAvailable)
                    {
                        CommandHandlerHelper.WriteOutputInfo(session, $"Error, a player named \"{newName}\" already exists.", ChatMessageType.Broadcast);
                        return;
                    }

                    var character = DatabaseManager.Shard.BaseDatabase.GetCharacterStubByName(oldName);

                    DatabaseManager.Shard.GetCharacters(character.AccountId, false, result =>
                    {
                        var foundCharacterMatch = result.Where(c => c.Id == character.Id).FirstOrDefault();

                        if (foundCharacterMatch == null)
                        {
                            CommandHandlerHelper.WriteOutputInfo(session, $"Error, a player named \"{oldName}\" cannot be found.", ChatMessageType.Broadcast);
                        }

                        DatabaseManager.Shard.RenameCharacter(foundCharacterMatch, newName, new ReaderWriterLockSlim(), null);
                    });

                    offlinePlayer.SetProperty(PropertyString.Name, newName);
                    offlinePlayer.SaveBiotaToDatabase();

                    CommandHandlerHelper.WriteOutputInfo(session, $"Player named \"{oldName}\" renamed to \"{newName}\" successfully!", ChatMessageType.Broadcast);
                });
            }
            else
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Error, a player named \"{oldName}\" cannot be found.", ChatMessageType.Broadcast);
                return;
            }
        }

        public override void HandlePlayerDelete(ulong characterGuid)
        {
            AllegianceManager.HandlePlayerDelete(characterGuid);
            HouseManager.HandlePlayerDelete(characterGuid);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool ProcessDeletedPlayer(ulong guid)
        {
            var aeternum = primaryStore[guid]!;
            offlinePlayers = offlinePlayers.Remove(aeternum);
            allPlayers = allPlayers.Remove(aeternum);
            basicPlayerNames.Remove(aeternum.CanonicalName.Name, guid);
            playerAccounts.Remove(aeternum.Account.AccountId, guid);
            return true;
        }

        public override IPlayer? FindByName(string name) => FindByName(name, out _);

        public override IPlayer? FindByName(string name, out bool isOnline)
        {
            var player = basicPlayerNames[name].Value.Select(guid => primaryStore[guid]).FirstOrDefault();
            isOnline = player?.IsOnline ?? false;
            return player;
        }

        public override IPlayer? FindByGuid(ObjectGuid guid) => FindByGuid(guid, out _);
        public override IPlayer? FindByGuid(ObjectGuid guid, out bool isOnline) => FindByGuid(guid.Full, out isOnline);
        public override IPlayer? FindByGuid(ulong guid) => FindByGuid(guid, out _);
        public override IPlayer? FindByGuid(ulong guid, out bool isOnline)
        {
            var player = primaryStore[guid];
            isOnline = player?.IsOnline ?? false;
            return player;
        }

        public override List<IPlayer> FindAllByMonarch(ObjectGuid monarch) => allPlayers.Where(p => p.MonarchId == monarch.Full).ToList<IPlayer>();

        public override List<Player> GetOnlineInverseFriends(ObjectGuid guid) => onlinePlayers.Where(pl => {
            var p = (Player)pl;
            if (p == null)
                return false;
            return p.Character.HasAsFriend(guid.Full, p.CharacterDatabaseLock);
        }).Select(p => (Player)p).Where(p => p != null).ToList();

        public override void BroadcastToAll(GameMessage msg)
        {
            foreach(var player in onlinePlayers)
            {
                var p = (Player)player;
                if (p == null) continue;

                p.Session.Network.EnqueueSend(msg);
            }
        }

        public override void BroadcastToAuditChannel(Player issuer, string message)
        {
            if (issuer != null)
                BroadcastToChannel(Channel.Audit, issuer, message, true, true);
            else
                BroadcastToChannelFromConsole(Channel.Audit, message);
        }

        public override void BroadcastToChannel(Channel channel, Player sender, string message, bool ignoreSquelch = false, bool ignoreActive = false)
        {
            if ((sender.ChannelsActive.HasValue && sender.ChannelsActive.Value.HasFlag(channel)) || ignoreActive)
            {
                foreach (var player in onlinePlayers.Where(p => p.Player is Player pl && (pl.ChannelsActive ?? 0).HasFlag(channel)))
                {
                    var p = (Player)player;
                    if (p == null)
                        continue;

                    if (!p.SquelchManager.Squelches.Contains(sender) || ignoreSquelch)
                        p.Session.Network.EnqueueSend(new GameEventChannelBroadcast(p.Session, channel, sender.Guid == p.Guid ? "" : sender.Name, message));
                }

                LogBroadcastChat(channel, sender, message);
            }
        }

        public override void LogBroadcastChat(Channel channel, WorldObject sender, string message)
        {
            switch (channel)
            {
                case Channel.Abuse:
                    if (!PropertyManager.GetBool("chat_log_abuse").Item)
                        return;
                    break;
                case Channel.Admin:
                    if (!PropertyManager.GetBool("chat_log_admin").Item)
                        return;
                    break;
                case Channel.AllBroadcast: // using this to sub in for a WorldBroadcast channel which isn't technically a channel
                    if (!PropertyManager.GetBool("chat_log_global").Item)
                        return;
                    break;
                case Channel.Audit:
                    if (!PropertyManager.GetBool("chat_log_audit").Item)
                        return;
                    break;
                case Channel.Advocate1:
                case Channel.Advocate2:
                case Channel.Advocate3:
                    if (!PropertyManager.GetBool("chat_log_advocate").Item)
                        return;
                    break;
                case Channel.Debug:
                    if (!PropertyManager.GetBool("chat_log_debug").Item)
                        return;
                    break;
                case Channel.Fellow:
                case Channel.FellowBroadcast:
                    if (!PropertyManager.GetBool("chat_log_fellow").Item)
                        return;
                    break;
                case Channel.Help:
                    if (!PropertyManager.GetBool("chat_log_help").Item)
                        return;
                    break;
                case Channel.Olthoi:
                    if (!PropertyManager.GetBool("chat_log_olthoi").Item)
                        return;
                    break;
                case Channel.QA1:
                case Channel.QA2:
                    if (!PropertyManager.GetBool("chat_log_qa").Item)
                        return;
                    break;
                case Channel.Sentinel:
                    if (!PropertyManager.GetBool("chat_log_sentinel").Item)
                        return;
                    break;

                case Channel.SocietyCelHanBroadcast:
                case Channel.SocietyEldWebBroadcast:
                case Channel.SocietyRadBloBroadcast:
                    if (!PropertyManager.GetBool("chat_log_society").Item)
                        return;
                    break;

                case Channel.AllegianceBroadcast:
                case Channel.CoVassals:
                case Channel.Monarch:
                case Channel.Patron:
                case Channel.Vassals:
                    if (!PropertyManager.GetBool("chat_log_allegiance").Item)
                        return;
                    break;

                case Channel.AlArqas:
                case Channel.Holtburg:
                case Channel.Lytelthorpe:
                case Channel.Nanto:
                case Channel.Rithwic:
                case Channel.Samsur:
                case Channel.Shoushi:
                case Channel.Yanshi:
                case Channel.Yaraq:
                    if (!PropertyManager.GetBool("chat_log_townchans").Item)
                        return;
                    break;

                default:
                    return;
            }

            if (channel != Channel.AllBroadcast)
                log.Info($"[CHAT][{channel.ToString().ToUpper()}] {(sender != null ? sender.Name : "[SYSTEM]")} says on the {channel} channel, \"{message}\"");
            else
                log.Info($"[CHAT][GLOBAL] {(sender != null ? sender.Name : "[SYSTEM]")} issued a world broadcast, \"{message}\"");
        }

        public override void BroadcastToChannelFromConsole(Channel channel, string message)
        {
            foreach (var player in onlinePlayers.Where(p => p.Player is Player pl && (pl.ChannelsActive ?? 0).HasFlag(channel)))
            {
                var p = (Player)player;
                if (p == null)
                    continue;
                p.Session.Network.EnqueueSend(new GameEventChannelBroadcast(p.Session, channel, "CONSOLE", message));
            }
            
            LogBroadcastChat(channel, null, message);
        }

        public override void BroadcastToChannelFromEmote(Channel channel, string message)
        {
            foreach (var player in onlinePlayers.Where(p => p.Player is Player pl && (pl.ChannelsActive ?? 0).HasFlag(channel)))
            {
                var p = (Player)player;
                if (p == null)
                    continue;
                p.Session.Network.EnqueueSend(new GameEventChannelBroadcast(p.Session, channel, "EMOTE", message));
            }
        }

        public override bool GagPlayer(Player issuer, string playerName)
        {
            var player = FindByName(playerName);

            if (player == null)
                return false;

            player.SetProperty(ACE.Entity.Enum.Properties.PropertyBool.IsGagged, true);
            player.SetProperty(ACE.Entity.Enum.Properties.PropertyFloat.GagTimestamp, Common.Time.GetUnixTime());
            player.SetProperty(ACE.Entity.Enum.Properties.PropertyFloat.GagDuration, 300);

            player.SaveBiotaToDatabase();

            BroadcastToAuditChannel(issuer, $"{issuer.Name} has gagged {player.Name} for five minutes.");

            return true;
        }

        public override bool UnGagPlayer(Player issuer, string playerName)
        {
            var player = FindByName(playerName);

            if (player == null)
                return false;

            player.RemoveProperty(ACE.Entity.Enum.Properties.PropertyBool.IsGagged);
            player.RemoveProperty(ACE.Entity.Enum.Properties.PropertyFloat.GagTimestamp);
            player.RemoveProperty(ACE.Entity.Enum.Properties.PropertyFloat.GagDuration);

            player.SaveBiotaToDatabase();

            BroadcastToAuditChannel(issuer, $"{issuer.Name} has ungagged {player.Name}.");

            return true;
        }

        public override void BootAllPlayers()
        {
            foreach (var player in GetAllOnline().Where(p => p.Session.AccessLevel < AccessLevel.Advocate))
                player.Session.Terminate(SessionTerminationReason.WorldClosed, new GameMessageBootAccount(" because the world is now closed"), null, "The world is now closed");
        }

        public override void UpdatePKStatusForAllPlayers(string worldType, bool enabled)
        {
            switch (worldType)
            {
                case "pk_server":
                    if (enabled)
                    {
                        foreach (var player in GetAllOnline())
                            player.SetPlayerKillerStatus(PlayerKillerStatus.PK, true);

                        foreach (var player in GetAllOffline())
                        {
                            player.SetProperty(PropertyInt.PlayerKillerStatus, (int)PlayerKillerStatus.NPK);
                            player.SetProperty(PropertyFloat.MinimumTimeSincePk, 0);
                        }

                        var msg = $"This world has been changed to a Player Killer world. All players will become Player Killers in {PropertyManager.GetDouble("pk_respite_timer").Item} seconds.";
                        BroadcastToAll(new GameMessageSystemChat(msg, ChatMessageType.WorldBroadcast));
                        LogBroadcastChat(Channel.AllBroadcast, null, msg);
                    }
                    else
                    {
                        foreach (var player in GetAllOnline())
                            player.SetPlayerKillerStatus(PlayerKillerStatus.NPK, true);

                        foreach (var player in GetAllOffline())
                        {
                            player.SetProperty(PropertyInt.PlayerKillerStatus, (int)PlayerKillerStatus.NPK);
                            player.SetProperty(PropertyFloat.MinimumTimeSincePk, 0);
                        }

                        var msg = "This world has been changed to a Non Player Killer world. All players are now Non-Player Killers.";
                        BroadcastToAll(new GameMessageSystemChat(msg, ChatMessageType.WorldBroadcast));
                        LogBroadcastChat(Channel.AllBroadcast, null, msg);
                    }
                    break;
                case "pkl_server":
                    if (PropertyManager.GetBool("pk_server").Item)
                        return;
                    if (enabled)
                    {
                        foreach (var player in GetAllOnline())
                            player.SetPlayerKillerStatus(PlayerKillerStatus.PKLite, true);

                        foreach (var player in GetAllOffline())
                        {
                            player.SetProperty(PropertyInt.PlayerKillerStatus, (int)PlayerKillerStatus.NPK);
                            player.SetProperty(PropertyFloat.MinimumTimeSincePk, 0);
                        }

                        var msg = $"This world has been changed to a Player Killer Lite world. All players will become Player Killer Lites in {PropertyManager.GetDouble("pk_respite_timer").Item} seconds.";
                        BroadcastToAll(new GameMessageSystemChat(msg, ChatMessageType.WorldBroadcast));
                        LogBroadcastChat(Channel.AllBroadcast, null, msg);
                    }
                    else
                    {
                        foreach (var player in GetAllOnline())
                            player.SetPlayerKillerStatus(PlayerKillerStatus.NPK, true);

                        foreach (var player in GetAllOffline())
                        {
                            player.SetProperty(PropertyInt.PlayerKillerStatus, (int)PlayerKillerStatus.NPK);
                            player.SetProperty(PropertyFloat.MinimumTimeSincePk, 0);
                        }

                        var msg = "This world has been changed to a Non Player Killer world. All players are now Non-Player Killers.";
                        BroadcastToAll(new GameMessageSystemChat(msg, ChatMessageType.WorldBroadcast));
                        LogBroadcastChat(Channel.AllBroadcast, null, msg);
                    }
                    break;
            }
        }

        public override bool IsAccountAtMaxCharacterSlots(string accountName)
        {
            var slotsAvailable = (int)PropertyManager.GetLong("max_chars_per_account").Item;
            var onlinePlayersTotal = 0;
            var offlinePlayersTotal = 0;

            playersLock.EnterReadLock();
            try
            {
                onlinePlayersTotal = onlinePlayers.Count(a => a.Value.Account.AccountName.Equals(accountName, StringComparison.OrdinalIgnoreCase));
                offlinePlayersTotal = offlinePlayers.Count(a => a.Value.Account.AccountName.Equals(accountName, StringComparison.OrdinalIgnoreCase));
            }
            finally
            {
                playersLock.ExitReadLock();
            }

            return (onlinePlayersTotal + offlinePlayersTotal) >= slotsAvailable;
        }
    }
}
