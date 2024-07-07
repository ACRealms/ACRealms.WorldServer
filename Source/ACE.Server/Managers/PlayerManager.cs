global using PlayerManager = ACE.Server.Managers.ACRealms.PlayerManagerShim;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using log4net;

using ACE.Common;
using ACE.Database;
using ACE.Database.Models.Shard;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Server.Entity;
using ACE.Server.Network.Enum;
using ACE.Server.Network.GameEvent.Events;
using ACE.Server.Network.GameMessages;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.WorldObjects;

using Biota = ACE.Entity.Models.Biota;
using ACE.Entity.ACRealms;
using System.Collections.Concurrent;
using ACE.Server.Network;
using ACE.Server.Command.Handlers;
using ACE.Server.Realms;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Collections.Frozen;
using ACE.Server.Entity.ACRealms;
using ACE.Server.Managers.ACRealms;
using ACRealms.DataStructures.Collections;

namespace ACE.Server.Managers
{

    public abstract class PlayerServiceBase
    {
        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ReaderWriterLockSlim playersLock = new ReaderWriterLockSlim();
        private readonly Dictionary<ulong, Player> onlinePlayers = new Dictionary<ulong, Player>();
        private readonly Dictionary<ulong, OfflinePlayer> offlinePlayers = new Dictionary<ulong, OfflinePlayer>();

        protected abstract IReadOnlyDictionary<ulong, IPlayer> PrimaryStore { get; init; }

        protected readonly ConcurrentDictionary<ulong, IPlayer> canonicalBackingStore;

        // A note on the value type (why there is no ConcurrentHashSet): https://github.com/dotnet/runtime/issues/39919
        // ImmutableArray is better than using ConcurrentDictionary<ulong, byte> as the value
        // - The value of this array generally has a small number of elements. Most of the time, just a single element
        // - The only time the array will need to be rebuilt is when a new character is created (where the name matches an already existing player name), or when a character is renamed
        // Using a Lazy<T> value here allows for atomic writes to the underlying store
        protected readonly ConcurrentLazyAssociationMap<string, ulong> basicPlayerNames = new ConcurrentLazyAssociationMap<string, ulong>(StringComparer.OrdinalIgnoreCase);

        // indexed by player name
        private readonly Dictionary<string, IPlayer> playerNames = new Dictionary<string, IPlayer>(StringComparer.OrdinalIgnoreCase);

        // indexed by account id
        //protected readonly Dictionary<uint, Dictionary<ulong, IPlayer>> playerAccounts = new Dictionary<uint, Dictionary<ulong, IPlayer>>();
        protected ConcurrentLazyAssociationMap<uint, ulong> playerAccounts { get; } = new ConcurrentLazyAssociationMap<uint, ulong>();

        public CanonicalCharacterNameStore CanonicalStore { get; init; }

        /// <summary>
        /// OfflinePlayers will be saved to the database every 1 hour
        /// </summary>
        protected readonly TimeSpan databaseSaveInterval = TimeSpan.FromHours(1);

        protected DateTime lastDatabaseSave = DateTime.MinValue;

        internal PlayerServiceBase()
        {
            canonicalBackingStore = new ConcurrentDictionary<ulong, IPlayer>();
            CanonicalStore = new CanonicalCharacterNameStore(canonicalBackingStore);
        }

        protected readonly LinkedList<Player> playersPendingLogoff = new LinkedList<Player>();

        public virtual void AddPlayerToLogoffQueue(Player player)
        {
            if (!playersPendingLogoff.Contains(player))
                playersPendingLogoff.AddLast(player);
        }

        public virtual void Tick()
        {
            // Database Save
            if (lastDatabaseSave + databaseSaveInterval <= DateTime.UtcNow)
                SaveOfflinePlayersWithChanges();

            var currentUnixTime = Time.GetUnixTime();

            while (playersPendingLogoff.Count > 0)
            {
                var first = playersPendingLogoff.First.Value;

                if (first.LogoffTimestamp <= currentUnixTime)
                {
                    playersPendingLogoff.RemoveFirst();
                    first.LogOut_Inner();
                    first.Session.logOffRequestTime = DateTime.UtcNow;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// This will save any player in the OfflinePlayers dictionary that has ChangesDetected. The biotas are saved in parallel.
        /// </summary>
        public virtual void SaveOfflinePlayersWithChanges()
        {
            lastDatabaseSave = DateTime.UtcNow;

            var biotas = new Collection<(Biota biota, ReaderWriterLockSlim rwLock)>();

            playersLock.EnterReadLock();
            try
            {
                foreach (var player in offlinePlayers.Values)
                {
                    if (player.ChangesDetected)
                    {
                        player.SaveBiotaToDatabase(false);
                        biotas.Add((player.Biota, player.BiotaDatabaseLock));
                    }
                }
            }
            finally
            {
                playersLock.ExitReadLock();
            }

            DatabaseManager.Shard.SaveBiotasInParallel(biotas, null, true);
        }


        /// <summary>
        /// This would be used when a new player is created after the server has started.
        /// When a new Player is created, they're created in an offline state, and then set to online shortly after as the login sequence continues.
        /// </summary>
        public virtual void AddOfflinePlayer(Player player)
        {
            //playersLock.EnterWriteLock();
            //try
            //{
            //    var offlinePlayer = new OfflinePlayer(player.Biota);
            //    offlinePlayers[offlinePlayer.Guid.Full] = offlinePlayer;

            //    playerNames[offlinePlayer.Name] = offlinePlayer;

            //    if (!playerAccounts.TryGetValue(offlinePlayer.Account.AccountId, out var playerAccountsDict))
            //    {
            //        playerAccountsDict = new Dictionary<ulong, IPlayer>();
            //        playerAccounts[offlinePlayer.Account.AccountId] = playerAccountsDict;
            //    }
            //    playerAccountsDict[offlinePlayer.Guid.Full] = offlinePlayer;
            //}
            //finally
            //{
            //    playersLock.ExitWriteLock();
            //}
        }

        /// <summary>
        /// This will return null if the player wasn't found.
        /// </summary>
        public virtual OfflinePlayer GetOfflinePlayer(ObjectGuid guid)
        {
            return GetOfflinePlayer(guid.Full);
        }

        /// <summary>
        /// This will return null if the player wasn't found.
        /// </summary>
        public virtual OfflinePlayer GetOfflinePlayer(ulong guid)
        {
            playersLock.EnterReadLock();
            try
            {
                if (offlinePlayers.TryGetValue(guid, out var value))
                    return value;
            }
            finally
            {
                playersLock.ExitReadLock();
            }

            return null;
        }

        public virtual bool DoesBasicNameExist(string name)
            => basicPlayerNames.ContainsKey(name);

        public virtual IImmutableList<ulong> GetPlayerGuidsByBasicName(string name)
            => basicPlayerNames[name].Value;

        public virtual IPlayer GetPlayerGuidByCanonicalName(CanonicalCharacterName name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This will return null of the name was not found.
        /// </summary>
        public virtual OfflinePlayer GetOfflinePlayer(string name)
        {
            var admin = "+" + name;

            playersLock.EnterReadLock();
            try
            {
                var offlinePlayerList = offlinePlayers.Values.Where(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) || p.Name.Equals(admin, StringComparison.OrdinalIgnoreCase)).ToList();
                if (offlinePlayerList.Count == 0)
                    return null;

                if (offlinePlayerList.Count == 1)
                    return offlinePlayerList[0];

                var not_deleted = offlinePlayerList.FirstOrDefault(x => x.IsDeleted || x.IsPendingDeletion);
                if (not_deleted != null)
                    return not_deleted;

                return offlinePlayerList[0];
            }
            finally
            {
                playersLock.ExitReadLock();
            }
        }

        public virtual List<OfflinePlayer> GetOfflinePlayersWithName(string name)
        {
            var admin = "+" + name;

            playersLock.EnterReadLock();
            try
            {
                return offlinePlayers.Values.Where(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) || p.Name.Equals(admin, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            finally
            {
                playersLock.ExitReadLock();
            }
        }
        public abstract List<Player> GetOnlinePlayersWithName(string name);

        public virtual List<IPlayer> GetAllPlayers()
        {
            var offlinePlayers = GetAllOffline();
            var onlinePlayers = GetAllOnline();

            var allPlayers = new List<IPlayer>();

            allPlayers.AddRange(offlinePlayers);
            allPlayers.AddRange(onlinePlayers);

            return allPlayers;
        }

        public virtual Dictionary<ulong, IPlayer> GetAccountPlayers(uint accountId)
        {
            throw new NotImplementedException();
            //playersLock.EnterReadLock();
            //try
            //{
            //    playerAccounts.TryGetValue(accountId, out var accountPlayers);
            //    return accountPlayers;
            //}
            //finally
            //{
            //    playersLock.ExitReadLock();
            //}
        }

        public virtual int GetOfflineCount()
        {
            playersLock.EnterReadLock();
            try
            {
                return offlinePlayers.Count;
            }
            finally
            {
                playersLock.ExitReadLock();
            }
        }

        public virtual List<OfflinePlayer> GetAllOffline()
        {
            var results = new List<OfflinePlayer>();

            playersLock.EnterReadLock();
            try
            {
                foreach (var player in offlinePlayers.Values)
                    results.Add(player);
            }
            finally
            {
                playersLock.ExitReadLock();
            }

            return results;
        }

        public virtual int GetOnlineCount()
        {
            playersLock.EnterReadLock();
            try
            {
                return onlinePlayers.Count;
            }
            finally
            {
                playersLock.ExitReadLock();
            }
        }

        /// <summary>
        /// This will return null if the player wasn't found.
        /// </summary>
        public virtual Player GetOnlinePlayer(ObjectGuid guid)
        {
            return GetOnlinePlayer(guid.Full);
        }

        /// <summary>
        /// This will return null if the player wasn't found.
        /// </summary>
        public virtual Player GetOnlinePlayer(ulong guid)
        {
            playersLock.EnterReadLock();
            try
            {
                if (onlinePlayers.TryGetValue(guid, out var value))
                    return value;
            }
            finally
            {
                playersLock.ExitReadLock();
            }

            return null;
        }

        /// <summary>
        /// This will return null of the name was not found.
        /// </summary>
        public virtual Player GetOnlinePlayer(string name)
        {
            var admin = "+" + name;

            playersLock.EnterReadLock();
            try
            {
                var onlinePlayer = onlinePlayers.Values.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) || p.Name.Equals(admin, StringComparison.OrdinalIgnoreCase));

                if (onlinePlayer != null)
                    return onlinePlayer;
            }
            finally
            {
                playersLock.ExitReadLock();
            }

            return null;
        }

        public virtual List<Player> GetAllOnline()
        {
            var results = new List<Player>();

            playersLock.EnterReadLock();
            try
            {
                foreach (var player in onlinePlayers.Values)
                    results.Add(player);
            }
            finally
            {
                playersLock.ExitReadLock();
            }

            return results;
        }


        /// <summary>
        /// This will return true if the player was successfully added.
        /// It will return false if the player was not found in the OfflinePlayers dictionary (which should never happen), or player already exists in the OnlinePlayers dictionary (which should never happen).
        /// This will always be preceded by a call to GetOfflinePlayer()
        /// </summary>
        public virtual bool SwitchPlayerFromOfflineToOnline(Player player)
        {
            playersLock.EnterWriteLock();
            try
            {
                if (!offlinePlayers.Remove(player.Guid.Full, out var offlinePlayer))
                    return false; // This should never happen

                if (offlinePlayer.ChangesDetected)
                    player.ChangesDetected = true;

                player.Allegiance = offlinePlayer.Allegiance;
                player.AllegianceNode = offlinePlayer.AllegianceNode;

                if (!onlinePlayers.TryAdd(player.Guid.Full, player))
                    return false;

                playerNames[offlinePlayer.Name] = player;

                //playerAccounts[offlinePlayer.Account.AccountId][offlinePlayer.Guid.Full] = player;
            }
            finally
            {
                playersLock.ExitWriteLock();
            }

            AllegianceManager.LoadPlayer(player);

            player.SendFriendStatusUpdates();

            return true;
        }

        /// <summary>
        /// This will return true if the player was successfully added.
        /// It will return false if the player was not found in the OnlinePlayers dictionary (which should never happen), or player already exists in the OfflinePlayers dictionary (which should never happen).
        /// </summary>
        public virtual bool SwitchPlayerFromOnlineToOffline(Player player)
        {
            playersLock.EnterWriteLock();
            try
            {
                if (!onlinePlayers.Remove(player.Guid.Full, out _))
                    return false; // This should never happen

                var offlinePlayer = new OfflinePlayer(player.Biota);

                offlinePlayer.Allegiance = player.Allegiance;
                offlinePlayer.AllegianceNode = player.AllegianceNode;

                if (!offlinePlayers.TryAdd(offlinePlayer.Guid.Full, offlinePlayer))
                    return false;

                playerNames[offlinePlayer.Name] = offlinePlayer;

                //playerAccounts[offlinePlayer.Account.AccountId][offlinePlayer.Guid.Full] = offlinePlayer;
            }
            finally
            {
                playersLock.ExitWriteLock();
            }

            player.SendFriendStatusUpdates(false);
            player.HandleAllegianceOnLogout();

            return true;
        }

        public virtual async Task<bool> IsCharacterNameAvailableForCreation(string name)
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

        public virtual async Task<bool> IsCharacterNameAvailable(string name, ushort? realmId, bool? overrideUniquePerHomeRealm = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            DatabaseManager.Shard.IsCharacterNameAvailable(name, realmId, isAvailable =>
            {
                tcs.TrySetResult(isAvailable);
            }, overrideUniquePerHomeRealm);

            return await tcs.Task;
        }

        public virtual void IsCharacterNameAvailable(string name, ushort? realmId, Action<bool> callback, bool? overrideUniquePerHomeRealm = null)
            => DatabaseManager.Shard.IsCharacterNameAvailable(name, realmId, callback, overrideUniquePerHomeRealm);

        public virtual void HandlePlayerRename(ISession session, string oldName, string newName)
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

        /// <summary>
        /// Called when a character is initially deleted on the character select screen
        /// </summary>
        public virtual void HandlePlayerDelete(ulong characterGuid)
        {
            AllegianceManager.HandlePlayerDelete(characterGuid);

            HouseManager.HandlePlayerDelete(characterGuid);
        }

        /// <summary>
        /// This will return true if the player was successfully found and removed from the OfflinePlayers dictionary.
        /// It will return false if the player was not found in the OfflinePlayers dictionary (which should never happen).
        /// </summary>
        public virtual bool ProcessDeletedPlayer(ulong guid)
        {
            playersLock.EnterWriteLock();
            try
            {
                if (!offlinePlayers.Remove(guid, out var offlinePlayer))
                    return false; // This should never happen

                playerNames.Remove(offlinePlayer.Name);
                basicPlayerNames.Remove(offlinePlayer.Name, guid);
                playerAccounts.Remove(offlinePlayer.Account.AccountId, offlinePlayer.Guid.Full);
                //playerAccounts[offlinePlayer.Account.AccountId].Remove(offlinePlayer.Guid.Full);
            }
            finally
            {
                playersLock.ExitWriteLock();
            }

            return true;
        }


        /// <summary>
        /// This will return null if the name was not found.
        /// </summary>
        public virtual IPlayer FindByName(string name)
        {
            return FindByName(name, out _);
        }

        /// <summary>
        /// This will return null if the name was not found.
        /// </summary>
        public virtual IPlayer FindByName(string name, out bool isOnline)
        {
            playersLock.EnterReadLock();
            try
            {
                playerNames.TryGetValue(name.TrimStart('+'), out var player);

                isOnline = player != null && player is Player;

                return player;
            }
            finally
            {
                playersLock.ExitReadLock();
            }
        }

        /// <summary>
        /// This will return null if the guid was not found.
        /// </summary>
        public virtual IPlayer FindByGuid(ObjectGuid guid)
        {
            return FindByGuid(guid, out _);
        }

        /// <summary>
        /// This will return null if the guid was not found.
        /// </summary>
        public virtual IPlayer FindByGuid(ObjectGuid guid, out bool isOnline)
        {
            return FindByGuid(guid.Full, out isOnline);
        }

        /// <summary>
        /// This will return null if the guid was not found.
        /// </summary>
        public virtual IPlayer FindByGuid(ulong guid)
        {
            return FindByGuid(guid, out _);
        }

        /// <summary>
        /// This will return null if the guid was not found.
        /// </summary>
        public virtual IPlayer FindByGuid(ulong guid, out bool isOnline)
        {
            playersLock.EnterReadLock();
            try
            {
                if (onlinePlayers.TryGetValue(guid, out var onlinePlayer))
                {
                    isOnline = true;
                    return onlinePlayer;
                }

                isOnline = false;

                if (offlinePlayers.TryGetValue(guid, out var offlinePlayer))
                    return offlinePlayer;
            }
            finally
            {
                playersLock.ExitReadLock();
            }

            return null;
        }


        /// <summary>
        /// Returns a list of all players who are under a monarch
        /// </summary>
        /// <param name="monarch">The monarch of an allegiance</param>
        public virtual List<IPlayer> FindAllByMonarch(ObjectGuid monarch)
        {
            var results = new List<IPlayer>();

            playersLock.EnterReadLock();
            try
            {
                // this kind of sucks, possibly investigate?
                var onlinePlayersResult = onlinePlayers.Values.Where(p => p.MonarchId == monarch.Full);
                var offlinePlayersResult = offlinePlayers.Values.Where(p => p.MonarchId == monarch.Full);

                results.AddRange(onlinePlayersResult);
                results.AddRange(offlinePlayersResult);
            }
            finally
            {
                playersLock.ExitReadLock();
            }

            return results;
        }


        /// <summary>
        /// This will return a list of Players that have this guid as a friend.
        /// </summary>
        public virtual List<Player> GetOnlineInverseFriends(ObjectGuid guid)
        {
            var results = new List<Player>();

            playersLock.EnterReadLock();
            try
            {
                foreach (var player in onlinePlayers.Values)
                {
                    if (player.Character.HasAsFriend(guid.Full, player.CharacterDatabaseLock))
                        results.Add(player);
                }
            }
            finally
            {
                playersLock.ExitReadLock();
            }

            return results;
        }


        /// <summary>
        /// Broadcasts GameMessage to all online sessions.
        /// </summary>
        public virtual void BroadcastToAll(GameMessage msg)
        {
            foreach (var player in GetAllOnline())
                player.Session.Network.EnqueueSend(msg);
        }

        public virtual void BroadcastToAuditChannel(Player issuer, string message)
        {
            if (issuer != null)
                BroadcastToChannel(Channel.Audit, issuer, message, true, true);
            else
                BroadcastToChannelFromConsole(Channel.Audit, message);

            //if (PropertyManager.GetBool("log_audit", true).Item)
            //log.Info($"[AUDIT] {(issuer != null ? $"{issuer.Name} says on the Audit channel: " : "")}{message}");

            //LogBroadcastChat(Channel.Audit, issuer, message);
        }

        public virtual void BroadcastToChannel(Channel channel, Player sender, string message, bool ignoreSquelch = false, bool ignoreActive = false)
        {
            if ((sender.ChannelsActive.HasValue && sender.ChannelsActive.Value.HasFlag(channel)) || ignoreActive)
            {
                foreach (var player in GetAllOnline().Where(p => (p.ChannelsActive ?? 0).HasFlag(channel)))
                {
                    if (!player.SquelchManager.Squelches.Contains(sender) || ignoreSquelch)
                        player.Session.Network.EnqueueSend(new GameEventChannelBroadcast(player.Session, channel, sender.Guid == player.Guid ? "" : sender.Name, message));
                }

                LogBroadcastChat(channel, sender, message);
            }
        }

        public virtual void LogBroadcastChat(Channel channel, WorldObject sender, string message)
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

        public virtual void BroadcastToChannelFromConsole(Channel channel, string message)
        {
            foreach (var player in GetAllOnline().Where(p => (p.ChannelsActive ?? 0).HasFlag(channel)))
                player.Session.Network.EnqueueSend(new GameEventChannelBroadcast(player.Session, channel, "CONSOLE", message));

            LogBroadcastChat(channel, null, message);
        }

        public virtual void BroadcastToChannelFromEmote(Channel channel, string message)
        {
            foreach (var player in GetAllOnline().Where(p => (p.ChannelsActive ?? 0).HasFlag(channel)))
                player.Session.Network.EnqueueSend(new GameEventChannelBroadcast(player.Session, channel, "EMOTE", message));
        }

        public virtual bool GagPlayer(Player issuer, string playerName)
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

        public virtual bool UnGagPlayer(Player issuer, string playerName)
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

        public virtual void BootAllPlayers()
        {
            foreach (var player in GetAllOnline().Where(p => p.Session.AccessLevel < AccessLevel.Advocate))
                player.Session.Terminate(SessionTerminationReason.WorldClosed, new GameMessageBootAccount(" because the world is now closed"), null, "The world is now closed");
        }

        public virtual void UpdatePKStatusForAllPlayers(string worldType, bool enabled)
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

        public virtual bool IsAccountAtMaxCharacterSlots(string accountName)
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

  
    public class LegacyPlayerManager : PlayerServiceBase, IPlayerManager
    {

        protected override IReadOnlyDictionary<ulong, IPlayer> PrimaryStore { get; init; } = new ConcurrentDictionary<ulong, IPlayer>();

        /// <summary>
        /// This will load all the players from the database into the OfflinePlayers dictionary. It should be called before WorldManager is initialized.
        /// </summary>
        public void Initialize()
        {
            //var results = DatabaseManager.Shard.BaseDatabase.GetAllPlayerBiotasInParallel();

            //Parallel.ForEach(results, ConfigManager.Config.Server.Threading.DatabaseParallelOptions, result =>
            //{
            //    var offlinePlayer = new OfflinePlayer(result);

            //    lock (offlinePlayers)
            //        offlinePlayers[offlinePlayer.Guid.Full] = offlinePlayer;

            //    lock (playerNames)
            //        playerNames[offlinePlayer.Name] = offlinePlayer;

            //   // canonicalBackingStore.TryAdd(result.Id, new StaticPlayer(result));
            //   // PushGuidForBasicName(offlinePlayer.Name, offlinePlayer.Guid.Full);

            //    lock (playerAccounts)
            //    {
            //        if (offlinePlayer.Account != null)
            //        {
            //            if (!playerAccounts.TryGetValue(offlinePlayer.Account.AccountId, out var playerAccountsDict))
            //            {
            //                playerAccountsDict = new Dictionary<ulong, IPlayer>();
            //                playerAccounts[offlinePlayer.Account.AccountId] = playerAccountsDict;
            //            }
            //            playerAccountsDict[offlinePlayer.Guid.Full] = offlinePlayer;
            //        }
            //        else
            //            log.Error($"PlayerManager.Initialize: couldn't find account for player {offlinePlayer.Name} ({offlinePlayer.Guid})");
            //    }
            //});
        }



    }
}
