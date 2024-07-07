using ACE.Database.Models.Auth;
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
    public interface IPlayerManager
    {
        void Initialize();

        CanonicalCharacterNameStore CanonicalStore { get; }

        void AddPlayerToLogoffQueue(Player player);

        void Tick();

        /// <summary>
        /// This will save any player in the OfflinePlayers dictionary that has ChangesDetected. The biotas are saved in parallel.
        /// </summary>
        void SaveOfflinePlayersWithChanges();


        /// <summary>
        /// This would be used when a new player is created after the server has started.
        /// When a new Player is created, they're created in an offline state, and then set to online shortly after as the login sequence continues.
        /// </summary>
        void AddOfflinePlayer(Player player);

        /// <summary>
        /// This will return null if the player wasn't found.
        /// </summary>
        OfflinePlayer GetOfflinePlayer(ObjectGuid guid);


        /// <summary>
        /// This will return null if the player wasn't found.
        /// </summary>
        OfflinePlayer GetOfflinePlayer(ulong guid);

        bool DoesBasicNameExist(string name);
        IImmutableList<ulong> GetPlayerGuidsByBasicName(string name);

        IPlayer GetPlayerGuidByCanonicalName(CanonicalCharacterName name);

        /// <summary>
        /// This will return null of the name was not found.
        /// </summary>
        OfflinePlayer GetOfflinePlayer(string name);

        List<OfflinePlayer> GetOfflinePlayersWithName(string name);

        List<IPlayer> GetAllPlayers();

        Dictionary<ulong, IPlayer> GetAccountPlayers(uint accountId);

        int GetOfflineCount();

        List<OfflinePlayer> GetAllOffline();

        int GetOnlineCount();

        /// <summary>
        /// This will return null if the player wasn't found.
        /// </summary>
        Player GetOnlinePlayer(ObjectGuid guid);

        /// <summary>
        /// This will return null if the player wasn't found.
        /// </summary>
        Player GetOnlinePlayer(ulong guid);

        /// <summary>
        /// This will return null of the name was not found.
        /// </summary>
        Player GetOnlinePlayer(string name);

        List<Player> GetAllOnline();


        /// <summary>
        /// This will return true if the player was successfully added.
        /// It will return false if the player was not found in the OfflinePlayers dictionary (which should never happen), or player already exists in the OnlinePlayers dictionary (which should never happen).
        /// This will always be preceded by a call to GetOfflinePlayer()
        /// </summary>
        bool SwitchPlayerFromOfflineToOnline(Player player);

        /// <summary>
        /// This will return true if the player was successfully added.
        /// It will return false if the player was not found in the OnlinePlayers dictionary (which should never happen), or player already exists in the OfflinePlayers dictionary (which should never happen).
        /// </summary>
        bool SwitchPlayerFromOnlineToOffline(Player player);

        Task<bool> IsCharacterNameAvailableForCreation(string name);
        Task<bool> IsCharacterNameAvailable(string name, ushort? realmId, bool? overrideUniquePerHomeRealm = null);

        void IsCharacterNameAvailable(string name, ushort? realmId, Action<bool> callback, bool? overrideUniquePerHomeRealm = null);


        void HandlePlayerRename(ISession session, string oldName, string newName);

        /// <summary>
        /// Called when a character is initially deleted on the character select screen
        /// </summary>
        void HandlePlayerDelete(ulong characterGuid);

        /// <summary>
        /// This will return true if the player was successfully found and removed from the OfflinePlayers dictionary.
        /// It will return false if the player was not found in the OfflinePlayers dictionary (which should never happen).
        /// </summary>
        bool ProcessDeletedPlayer(ulong guid);


        /// <summary>
        /// This will return null if the name was not found.
        /// </summary>
        IPlayer FindByName(string name);

        /// <summary>
        /// This will return null if the name was not found.
        /// </summary>
        IPlayer FindByName(string name, out bool isOnline);

        /// <summary>
        /// This will return null if the guid was not found.
        /// </summary>
        IPlayer FindByGuid(ObjectGuid guid);

        /// <summary>
        /// This will return null if the guid was not found.
        /// </summary>
        IPlayer FindByGuid(ObjectGuid guid, out bool isOnline);

        /// <summary>
        /// This will return null if the guid was not found.
        /// </summary>
        IPlayer FindByGuid(ulong guid);

        /// <summary>
        /// This will return null if the guid was not found.
        /// </summary>
        IPlayer FindByGuid(ulong guid, out bool isOnline);

        /// <summary>
        /// Returns a list of all players who are under a monarch
        /// </summary>
        /// <param name="monarch">The monarch of an allegiance</param>
        List<IPlayer> FindAllByMonarch(ObjectGuid monarch);
        List<Player> GetOnlineInverseFriends(ObjectGuid guid);

        /// <summary>
        /// Broadcasts GameMessage to all online sessions.
        /// </summary>
        void BroadcastToAll(GameMessage msg);
        void BroadcastToAuditChannel(Player issuer, string message);
        void BroadcastToChannel(Channel channel, Player sender, string message, bool ignoreSquelch = false, bool ignoreActive = false);
        void LogBroadcastChat(Channel channel, WorldObject sender, string message);
        void BroadcastToChannelFromConsole(Channel channel, string message);
        void BroadcastToChannelFromEmote(Channel channel, string message);
        bool GagPlayer(Player issuer, string playerName);
        bool UnGagPlayer(Player issuer, string playerName);
        void BootAllPlayers();
        void UpdatePKStatusForAllPlayers(string worldType, bool enabled);

        bool IsAccountAtMaxCharacterSlots(uint accountId);

        [Obsolete("Use the accountId overload")]
        bool IsAccountAtMaxCharacterSlots(Account account);
    }

}
