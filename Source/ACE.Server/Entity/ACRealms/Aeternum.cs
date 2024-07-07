using ACE.Database.Models.Auth;
using ACE.Database.Models.Shard;
using ACE.Entity;
using ACE.Entity.ACRealms;
using ACE.Entity.Enum.Properties;
using ACE.Server.Realms;
using ACE.Server.WorldObjects;
using System;
using System.Threading;

namespace ACE.Server.Entity.ACRealms
{
    public class Aeternum : IPlayer, IEquatable<Aeternum>
    {
        Aeternum IPlayer.Aeternum => this;
        private Player OnlinePlayer { get; set; }
        private OfflinePlayer OfflinePlayer { get; set; }

        public IPlayer Player => OnlinePlayer ?? (IPlayer)OfflinePlayer ?? OnlinePlayer;

        public ObjectGuid Guid { get; init; }
        public Account Account { get; init; }
        public bool IsOnline => OnlinePlayer != null;
        public string Name => Player.Name;

        private CanonicalCharacterName _canonicalName;

        CanonicalName<IPlayer, CanonicalCharacterName> ICanonicallyResolvable<IPlayer, CanonicalCharacterName>.CanonicalName => _canonicalName;
        public CanonicalCharacterName CanonicalName
        {
            get { return _canonicalName; }
            private set { _canonicalName = value; }
        }

        internal Aeternum(OfflinePlayer player)
        {
            Guid = player.Guid;
            Account = player.Account;
            OfflinePlayer = player;
        }

        public int? Level => Player.Level;
        public int? Heritage => Player.Heritage;
        public int? Gender => Player.Gender;
        public bool IsDeleted => Player.IsDeleted;
        public bool IsPendingDeletion => Player.IsPendingDeletion;
        public ulong? MonarchId { get => Player.MonarchId; set => Player.MonarchId = value; }
        public ulong? PatronId { get => Player.PatronId; set => Player.PatronId = value; }
        public ulong AllegianceXPCached { get => Player.AllegianceXPCached; set => Player.AllegianceXPCached = value; }
        public ulong AllegianceXPGenerated { get => Player.AllegianceXPGenerated; set => Player.AllegianceXPGenerated = value; }
        public int? AllegianceRank { get => Player.AllegianceRank; set => Player.AllegianceRank = value; }
        public int? AllegianceOfficerRank { get => Player.AllegianceOfficerRank; set => Player.AllegianceOfficerRank = value; }
        public bool ExistedBeforeAllegianceXpChanges { get => Player.ExistedBeforeAllegianceXpChanges; set => Player.ExistedBeforeAllegianceXpChanges = value; }
        public uint? HouseId { get => Player.HouseId; set => Player.HouseId = value; }
        public ulong? HouseInstance { get => Player.HouseInstance; set => Player.HouseInstance = value; }
        public int? HousePurchaseTimestamp { get => Player.HousePurchaseTimestamp; set => Player.HousePurchaseTimestamp = value; }
        public int? HouseRentTimestamp { get => Player.HouseRentTimestamp; set => Player.HouseRentTimestamp = value; }
        public Allegiance Allegiance { get => Player.Allegiance; set => Player.Allegiance = value; }
        public AllegianceNode AllegianceNode { get => Player.AllegianceNode; set => Player.AllegianceNode = value; }
        public int? HomeRealmIDRaw => Player.HomeRealmIDRaw;
        public ushort HomeRealm => Player.HomeRealm;
        public string DisplayedHomeRealmName => Player.DisplayedHomeRealmName;
        public uint GetCurrentLeadership() => Player.GetCurrentLeadership();
        public uint GetCurrentLoyalty() => Player.GetCurrentLoyalty();
        public bool? GetProperty(PropertyBool property) => Player.GetProperty(property);
        public uint? GetProperty(PropertyDataId property) => Player.GetProperty(property);
        public double? GetProperty(PropertyFloat property) => Player.GetProperty(property);
        public ulong? GetProperty(PropertyInstanceId property) => Player.GetProperty(property);
        public int? GetProperty(PropertyInt property) => Player.GetProperty(property);
        public long? GetProperty(PropertyInt64 property) => Player.GetProperty(property);
        public string GetProperty(PropertyString property) => Player.GetProperty(property);
        public void RemoveProperty(PropertyBool property) => Player.RemoveProperty(property);
        public void RemoveProperty(PropertyDataId property) => Player.RemoveProperty(property);
        public void RemoveProperty(PropertyFloat property) => Player.RemoveProperty(property);
        public void RemoveProperty(PropertyInstanceId property) => Player.RemoveProperty(property);
        public void RemoveProperty(PropertyInt property) => Player.RemoveProperty(property);
        public void RemoveProperty(PropertyInt64 property) => Player.RemoveProperty(property);
        public void RemoveProperty(PropertyString property) => Player.RemoveProperty(property);
        public void SaveBiotaToDatabase(bool enqueueSave = true) => Player.SaveBiotaToDatabase(enqueueSave);
        public void SetProperty(PropertyBool property, bool value) => Player.SetProperty(property, value);
        public void SetProperty(PropertyDataId property, uint value) => Player.SetProperty(property, value);
        public void SetProperty(PropertyFloat property, double value) => Player.SetProperty(property, value);
        public void SetProperty(PropertyInstanceId property, ulong value) => Player.SetProperty(property, value);
        public void SetProperty(PropertyInt property, int value) => Player.SetProperty(property, value);
        public void SetProperty(PropertyInt64 property, long value) => Player.SetProperty(property, value);
        public void SetProperty(PropertyString property, string value) => Player.SetProperty(property, value);
        public void UpdateProperty(PropertyInstanceId prop, ulong? value, bool broadcast = false) => Player.UpdateProperty(prop, value, broadcast);

        public bool Equals(Aeternum other) => Guid == other.Guid;

        public override int GetHashCode() => Guid.Full.GetHashCode();

        public bool ChangesDetected => Player.ChangesDetected;

        public ACE.Entity.Models.Biota Biota => Player.Biota;
        public ReaderWriterLockSlim BiotaDatabaseLock => Player.BiotaDatabaseLock;

        public static implicit operator Player(Aeternum aeternum)
        {
            var p = aeternum.Player;
            if (p.IsOnline)
                return (Player)p;
            return null;
        }

        //public static explicit operator Player(Aeternum aeternum)
        //{ 
        //    var p = aeternum.Player;
        //    if (p.IsOnline)
        //        return (Player)p;
        //    return null;
        //}

        public static implicit operator OfflinePlayer(Aeternum aeternum)
        {
            var p = aeternum.Player;
            if (!p.IsOnline)
                return (OfflinePlayer)p;
            return null;
        }

        internal void SetToOffline(OfflinePlayer player)
        {
            if (player.Guid != Guid)
                throw new ArgumentException("Guid mismatch");
            if (Player is OfflinePlayer)
                throw new InvalidOperationException("Player is already offline");
            OfflinePlayer = player;
            OnlinePlayer = null;
        }

        internal void SetToOnline(Player player)
        {
            if (player.Guid != Guid)
                throw new ArgumentException("Guid mismatch");
            if (Player is Player)
                throw new InvalidOperationException("Player is already online");
            OnlinePlayer = player;
            OfflinePlayer = null;
        }
    }
}
