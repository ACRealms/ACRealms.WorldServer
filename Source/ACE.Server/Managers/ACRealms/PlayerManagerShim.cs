using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Server.Entity;
using ACE.Server.Network;
using ACE.Server.Network.GameMessages;
using ACE.Server.Realms;
using ACE.Server.WorldObjects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Managers.ACRealms
{
    public class PlayerManagerShim
    {
        const bool UseLegacyPlayerManager = true;
        static IPlayerManager Instance { get; }

        static PlayerManagerShim()
        {
            if (UseLegacyPlayerManager)
                Instance = new LegacyPlayerManager();
            else
                Instance = new AeternumService();
        }

        public static CanonicalCharacterNameStore CanonicalStore => Instance.CanonicalStore;
        public static void Initialize() => Instance.Initialize();

        public static void AddPlayerToLogoffQueue(Player player)
            => Instance.AddPlayerToLogoffQueue(player);

        public static void Tick() => Instance.Tick();

        /// <summary>
        /// This will save any player in the OfflinePlayers dictionary that has ChangesDetected. The biotas are saved in parallel.
        /// </summary>
        public static void SaveOfflinePlayersWithChanges() => Instance.SaveOfflinePlayersWithChanges();

        /// <summary>
        /// This would be used when a new player is created after the server has started.
        /// When a new Player is created, they're created in an offline state, and then set to online shortly after as the login sequence continues.
        /// </summary>
        public static void AddOfflinePlayer(Player player) => Instance.AddOfflinePlayer(player);

        /// <summary>
        /// This will return null if the player wasn't found.
        /// </summary>
        public static OfflinePlayer GetOfflinePlayer(ObjectGuid guid) => Instance.GetOfflinePlayer(guid);

        /// <summary>
        /// This will return null if the player wasn't found.
        /// </summary>
        public static OfflinePlayer GetOfflinePlayer(ulong guid) => Instance.GetOfflinePlayer(guid);

        public static bool DoesBasicNameExist(string name) => Instance.DoesBasicNameExist(name);
        public static IImmutableList<ulong> GetPlayerGuidsByBasicName(string name) => Instance.GetPlayerGuidsByBasicName(name);

        public static IPlayer GetPlayerGuidByCanonicalName(CanonicalCharacterName name) => Instance.GetPlayerGuidByCanonicalName(name);

        /// <summary>
        /// This will return null of the name was not found.
        /// </summary>
        public static OfflinePlayer GetOfflinePlayer(string name) => Instance.GetOfflinePlayer(name);

        public static List<OfflinePlayer> GetOfflinePlayersWithName(string name) => Instance.GetOfflinePlayersWithName(name);

        public static List<IPlayer> GetAllPlayers() => Instance.GetAllPlayers();

        public static Dictionary<ulong, IPlayer> GetAccountPlayers(uint accountId) => Instance.GetAccountPlayers(accountId);

        public static int GetOfflineCount() => Instance.GetOfflineCount();

        public static List<OfflinePlayer> GetAllOffline() => Instance.GetAllOffline();

        public static int GetOnlineCount() => Instance.GetOnlineCount();

        /// <summary>
        /// This will return null if the player wasn't found.
        /// </summary>
        public static Player GetOnlinePlayer(ObjectGuid guid) => Instance.GetOnlinePlayer(guid);

        /// <summary>
        /// This will return null if the player wasn't found.
        /// </summary>
        public static Player GetOnlinePlayer(ulong guid) => Instance.GetOnlinePlayer(guid);

        /// <summary>
        /// This will return null of the name was not found.
        /// </summary>
        public static Player GetOnlinePlayer(string name) => Instance.GetOnlinePlayer(name);

        public static List<Player> GetAllOnline() => Instance.GetAllOnline();


        /// <summary>
        /// This will return true if the player was successfully added.
        /// It will return false if the player was not found in the OfflinePlayers dictionary (which should never happen), or player already exists in the OnlinePlayers dictionary (which should never happen).
        /// This will always be preceded by a call to GetOfflinePlayer()
        /// </summary>
        public static bool SwitchPlayerFromOfflineToOnline(Player player) => Instance.SwitchPlayerFromOfflineToOnline(player);

        /// <summary>
        /// This will return true if the player was successfully added.
        /// It will return false if the player was not found in the OnlinePlayers dictionary (which should never happen), or player already exists in the OfflinePlayers dictionary (which should never happen).
        /// </summary>
        public static bool SwitchPlayerFromOnlineToOffline(Player player) => Instance.SwitchPlayerFromOnlineToOffline(player);

        public static async Task<bool> IsCharacterNameAvailableForCreation(string name) => await Instance.IsCharacterNameAvailableForCreation(name);

        public static async Task<bool> IsCharacterNameAvailable(string name, ushort? realmId, bool? overrideUniquePerHomeRealm = null)
            => await Instance.IsCharacterNameAvailable(name, realmId, overrideUniquePerHomeRealm);

        public static void IsCharacterNameAvailable(string name, ushort? realmId, Action<bool> callback, bool? overrideUniquePerHomeRealm = null)
            => Instance.IsCharacterNameAvailable(name, realmId, callback, overrideUniquePerHomeRealm);


        public static void HandlePlayerRename(ISession session, string oldName, string newName)
            => Instance.HandlePlayerRename(session, oldName, newName);

        /// <summary>
        /// Called when a character is initially deleted on the character select screen
        /// </summary>
        public static void HandlePlayerDelete(ulong characterGuid) => Instance.HandlePlayerDelete(characterGuid);

        /// <summary>
        /// This will return true if the player was successfully found and removed from the OfflinePlayers dictionary.
        /// It will return false if the player was not found in the OfflinePlayers dictionary (which should never happen).
        /// </summary>
        public static bool ProcessDeletedPlayer(ulong guid) => Instance.ProcessDeletedPlayer(guid);


        /// <summary>
        /// This will return null if the name was not found.
        /// </summary>
        public static IPlayer FindByName(string name) => Instance.FindByName(name);

        /// <summary>
        /// This will return null if the name was not found.
        /// </summary>
        public static IPlayer FindByName(string name, out bool isOnline) => Instance.FindByName(name, out isOnline);

        /// <summary>
        /// This will return null if the guid was not found.
        /// </summary>
        public static IPlayer FindByGuid(ObjectGuid guid) => Instance.FindByGuid(guid);

        /// <summary>
        /// This will return null if the guid was not found.
        /// </summary>
        public static IPlayer FindByGuid(ObjectGuid guid, out bool isOnline) => Instance.FindByGuid(guid, out isOnline);

        /// <summary>
        /// This will return null if the guid was not found.
        /// </summary>
        public static IPlayer FindByGuid(ulong guid) => Instance.FindByGuid(guid);

        /// <summary>
        /// This will return null if the guid was not found.
        /// </summary>
        public static IPlayer FindByGuid(ulong guid, out bool isOnline) => Instance.FindByGuid(guid, out isOnline);


        /// <summary>
        /// Returns a list of all players who are under a monarch
        /// </summary>
        /// <param name="monarch">The monarch of an allegiance</param>
        public static List<IPlayer> FindAllByMonarch(ObjectGuid monarch) => Instance.FindAllByMonarch(monarch);


        public static List<Player> GetOnlineInverseFriends(ObjectGuid guid) => Instance.GetOnlineInverseFriends(guid);


        /// <summary>
        /// Broadcasts GameMessage to all online sessions.
        /// </summary>
        public static void BroadcastToAll(GameMessage msg) => Instance.BroadcastToAll(msg);

        public static void BroadcastToAuditChannel(Player issuer, string message)
            => Instance.BroadcastToAuditChannel(issuer, message);

        public static void BroadcastToChannel(Channel channel, Player sender, string message, bool ignoreSquelch = false, bool ignoreActive = false)
            => Instance.BroadcastToChannel(channel, sender, message, ignoreSquelch, ignoreActive);

        public static void LogBroadcastChat(Channel channel, WorldObject sender, string message)
            => Instance.LogBroadcastChat(channel, sender, message);

        public static void BroadcastToChannelFromConsole(Channel channel, string message)
            => Instance.BroadcastToChannelFromConsole(channel, message);

        public static void BroadcastToChannelFromEmote(Channel channel, string message)
            => Instance.BroadcastToChannelFromEmote(channel, message);

        public static bool GagPlayer(Player issuer, string playerName)
            => Instance.GagPlayer(issuer, playerName);

        public static bool UnGagPlayer(Player issuer, string playerName)
            => Instance.UnGagPlayer(issuer, playerName);

        public static void BootAllPlayers() => Instance.BootAllPlayers();

        public static void UpdatePKStatusForAllPlayers(string worldType, bool enabled)
            => Instance.UpdatePKStatusForAllPlayers(worldType, enabled);

        public static bool IsAccountAtMaxCharacterSlots(string accountName)
            => Instance.IsAccountAtMaxCharacterSlots(accountName);
    }

}
