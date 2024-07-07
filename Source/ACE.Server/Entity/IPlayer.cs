using System;
using System.Threading;
using ACE.Database.Models.Auth;
using ACE.Database.Models.Shard;
using ACE.Entity;
using ACE.Entity.ACRealms;
using ACE.Entity.Enum.Properties;
using ACE.Server.Entity.ACRealms;
using ACE.Server.Realms;
using ACE.Server.WorldObjects;

namespace ACE.Server.Entity
{
    /// <summary>
    /// This interface is used by Player and OfflinePlayer.
    /// It allows us to maintain two separate lists for online players (Player) and offline players (OfflinePlayer) in PlayerManager and return generic IPlayer results.
    /// </summary>
    public interface IPlayer
        : ICanonicallyResolvable<IPlayer, CanonicalCharacterName>
    {
        ObjectGuid Guid { get; }
        Aeternum Aeternum { get; }
        Account Account { get; }
        bool IsOnline { get; }
        bool? GetProperty(PropertyBool property);
        uint? GetProperty(PropertyDataId property);
        double? GetProperty(PropertyFloat property);
        ulong? GetProperty(PropertyInstanceId property);
        int? GetProperty(PropertyInt property);
        long? GetProperty(PropertyInt64 property);
        string GetProperty(PropertyString property);

        void SetProperty(PropertyBool property, bool value);
        void SetProperty(PropertyDataId property, uint value);
        void SetProperty(PropertyFloat property, double value);
        void SetProperty(PropertyInstanceId property, ulong value);
        void SetProperty(PropertyInt property, int value);
        void SetProperty(PropertyInt64 property, long value);
        void SetProperty(PropertyString property, string value);

        void RemoveProperty(PropertyBool property);
        void RemoveProperty(PropertyDataId property);
        void RemoveProperty(PropertyFloat property);
        void RemoveProperty(PropertyInstanceId property);
        void RemoveProperty(PropertyInt property);
        void RemoveProperty(PropertyInt64 property);
        void RemoveProperty(PropertyString property);


        string Name { get; }
        new CanonicalCharacterName CanonicalName { get; }

        int? Level { get; }

        int? Heritage { get; }

        int? Gender { get; }


        bool IsDeleted { get; }
        bool IsPendingDeletion { get; }


        ulong? MonarchId { get; set; }

        ulong? PatronId { get; set; }

        ulong AllegianceXPCached { get; set; }

        ulong AllegianceXPGenerated { get; set; }

        int? AllegianceRank { get; set; }

        int? AllegianceOfficerRank { get; set; }

        bool ExistedBeforeAllegianceXpChanges { get; set; }

        uint? HouseId { get; set; }

        ulong? HouseInstance { get; set; }

        int? HousePurchaseTimestamp { get; set; }

        int? HouseRentTimestamp { get; set; }


        uint GetCurrentLoyalty();

        uint GetCurrentLeadership();


        Allegiance Allegiance { get; set; }

        AllegianceNode AllegianceNode { get; set; }

        /// <summary>
        /// This method forces a player to be immediately saved to the database
        /// It should only be called in critical sections that must guarantee
        /// lock-step with other players
        /// </summary>
        void SaveBiotaToDatabase(bool enqueueSave = true);

        void UpdateProperty(PropertyInstanceId prop, ulong? value, bool broadcast = false);

        int? HomeRealmIDRaw { get; }
        ushort HomeRealm { get; }
        string DisplayedHomeRealmName { get; }
        bool ChangesDetected { get; }

        ACE.Entity.Models.Biota Biota { get; }

        ReaderWriterLockSlim BiotaDatabaseLock { get; }
    }
}
