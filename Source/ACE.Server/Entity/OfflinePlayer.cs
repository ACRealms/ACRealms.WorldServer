using System;
using System.Threading;
using ACE.Common;
using ACE.Database;
using ACE.Database.Models.Auth;
using ACE.Entity;
using ACE.Entity.ACRealms;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.Entity.ACRealms;
using ACE.Server.Entity.Actions;
using ACE.Server.Managers;
using ACE.Server.Realms;
using ACE.Server.WorldObjects;
using log4net;
using Newtonsoft.Json.Linq;

namespace ACE.Server.Entity
{
    public sealed class OfflinePlayer : IPlayer
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Aeternum Aeternum { get; private set; }
        public bool IsOnline => false;

        /// <summary>
        /// This is object property overrides that should have come from the shard db (or init to defaults of object is new to this instance).
        /// You should not manipulate these values directly. To manipulate this use the exposed SetProperty and RemoveProperty functions instead.
        /// </summary>
        public Biota Biota { get; }

        /// <summary>
        /// This is just a wrapper around Biota.Id
        /// </summary>
        public ObjectGuid Guid { get; }

        public Account Account { get; }

        /// <summary>
        /// Restore a WorldObject from the database.
        /// Any properties tagged as Ephemeral will be removed from the biota.
        /// </summary>
        internal OfflinePlayer(Biota biota, Aeternum aeternum)
            : this(biota)
        {
            if (aeternum == null)
                throw new ArgumentNullException("aeternum");
            Aeternum = aeternum;
        }

        private OfflinePlayer(Biota biota)
        {
            Biota = biota;

            Guid = new ObjectGuid(Biota.Id);

            var character = DatabaseManager.Shard.BaseDatabase.GetCharacterStubByGuid(Guid.Full);

            if (character != null)
                Account = DatabaseManager.Authentication.GetAccountById(character.AccountId);

            CanonicalName = CanonicalCharacterName.FromPlayer(this);
        }

        internal static OfflinePlayer Genesis(Biota biota)
        {
            var player = new OfflinePlayer(biota);
            var aeternum = new Aeternum(player);
            player.Aeternum = aeternum;
            return player;
        }

        public bool IsDeleted => DatabaseManager.Shard.BaseDatabase.GetCharacterStubByGuid(Guid.Full).IsDeleted;
        public bool IsPendingDeletion => DatabaseManager.Shard.BaseDatabase.GetCharacterStubByGuid(Guid.Full).DeleteTime > 0 && !IsDeleted;

        public DateTime LastRequestedDatabaseSave { get; protected set; }

        public bool ChangesDetected { get; set; }

        public ReaderWriterLockSlim BiotaDatabaseLock { get; } = new ReaderWriterLockSlim();

        /// <summary>
        /// This will set the LastRequestedDatabaseSave to UtcNow and ChangesDetected to false.<para />
        /// If enqueueSave is set to true, DatabaseManager.Shard.SaveBiota() will be called for the biota.<para />
        /// Set enqueueSave to false if you want to perform all the normal routines for a save but not the actual save. This is useful if you're going to collect biotas in bulk for bulk saving.
        /// </summary>
        public void SaveBiotaToDatabase(bool enqueueSave = true)
        {
            LastRequestedDatabaseSave = DateTime.UtcNow;
            ChangesDetected = false;

            if (enqueueSave)
                DatabaseManager.Shard.SaveBiota(Biota, BiotaDatabaseLock, null);
        }


        #region GetProperty Functions
        public bool? GetProperty(PropertyBool property)
        {
            return Biota.GetProperty(property, BiotaDatabaseLock);
        }
        public uint? GetProperty(PropertyDataId property)
        {
            return Biota.GetProperty(property, BiotaDatabaseLock);
        }
        public double? GetProperty(PropertyFloat property)
        {
            return Biota.GetProperty(property, BiotaDatabaseLock);
        }
        public ulong? GetProperty(PropertyInstanceId property)
        {
            return Biota.GetProperty(property, BiotaDatabaseLock);
        }
        public int? GetProperty(PropertyInt property)
        {
            return Biota.GetProperty(property, BiotaDatabaseLock);
        }
        public long? GetProperty(PropertyInt64 property)
        {
            return Biota.GetProperty(property, BiotaDatabaseLock);
        }
        public string GetProperty(PropertyString property)
        {
            return Biota.GetProperty(property, BiotaDatabaseLock);
        }
        #endregion

        #region SetProperty Functions
        public void SetProperty(PropertyBool property, bool value)
        {
            Biota.SetProperty(property, value, BiotaDatabaseLock, out var changed);
            if (changed)
                ChangesDetected = true;
        }
        public void SetProperty(PropertyDataId property, uint value)
        {
            Biota.SetProperty(property, value, BiotaDatabaseLock, out var changed);
            if (changed)
                ChangesDetected = true;
        }
        public void SetProperty(PropertyFloat property, double value)
        {
            Biota.SetProperty(property, value, BiotaDatabaseLock, out var changed);
            if (changed)
                ChangesDetected = true;
        }
        public void SetProperty(PropertyInstanceId property, ulong value)
        {
            Biota.SetProperty(property, value, BiotaDatabaseLock, out var changed);
            if (changed)
                ChangesDetected = true;

        }
        public void SetProperty(PropertyInt property, int value)
        {
            Biota.SetProperty(property, value, BiotaDatabaseLock, out var changed);
            if (changed)
                ChangesDetected = true;
        }
        public void SetProperty(PropertyInt64 property, long value)
        {
            Biota.SetProperty(property, value, BiotaDatabaseLock, out var changed);
            if (changed)
                ChangesDetected = true;
        }
        public void SetProperty(PropertyString property, string value)
        {
            Biota.SetProperty(property, value, BiotaDatabaseLock, out var changed);
            if (changed)
                ChangesDetected = true;
        }
        #endregion

        #region RemoveProperty Functions
        public void RemoveProperty(PropertyBool property)
        {
            if (Biota.TryRemoveProperty(property, BiotaDatabaseLock))
                ChangesDetected = true;
        }
        public void RemoveProperty(PropertyDataId property)
        {
            if (Biota.TryRemoveProperty(property, BiotaDatabaseLock))
                ChangesDetected = true;
        }
        public void RemoveProperty(PropertyFloat property)
        {
            if (Biota.TryRemoveProperty(property, BiotaDatabaseLock))
                ChangesDetected = true;
        }
        public void RemoveProperty(PropertyInstanceId property)
        {
            if (Biota.TryRemoveProperty(property, BiotaDatabaseLock))
                ChangesDetected = true;
        }
        public void RemoveProperty(PropertyInt property)
        {
            if (Biota.TryRemoveProperty(property, BiotaDatabaseLock))
                ChangesDetected = true;
        }
        public void RemoveProperty(PropertyInt64 property)
        {
            if (Biota.TryRemoveProperty(property, BiotaDatabaseLock))
                ChangesDetected = true;
        }
        public void RemoveProperty(PropertyString property)
        {
            if (Biota.TryRemoveProperty(property, BiotaDatabaseLock))
                ChangesDetected = true;
        }
        #endregion

        #region Position Functions

        public InstancedPosition GetPositionUnsafe(PositionType property)
        {
            var rawPos = Biota.GetPosition(property, BiotaDatabaseLock);
            if (rawPos == null)
                return null;
            return new InstancedPosition(rawPos, rawPos.Instance);
        }

        public InstancedPosition SetPositionUnsafe(PositionType property, InstancedPosition pos)
        {
            Biota.SetPosition(property, pos.GetPosition(), BiotaDatabaseLock);
            ChangesDetected = true;

            var rawPos = Biota.GetPosition(property, BiotaDatabaseLock);
            return new InstancedPosition(rawPos, rawPos.Instance);
        }

        public LocalPosition GetLocalPositionUnsafe(PositionType property)
        {
            var rawPos = Biota.GetPosition(property, BiotaDatabaseLock);
            if (rawPos == null)
                return null;
            return new LocalPosition(rawPos);
        }

        #endregion

        public string Name => GetProperty(PropertyString.Name);

        private CanonicalCharacterName _canonicalName;
        CanonicalName<IPlayer, CanonicalCharacterName> ICanonicallyResolvable<IPlayer, CanonicalCharacterName>.CanonicalName => _canonicalName;

        public CanonicalCharacterName CanonicalName
        {
            get { return _canonicalName; }
            private set { _canonicalName = value; }
        }

        public int? Level => GetProperty(PropertyInt.Level);

        public int? Heritage => GetProperty(PropertyInt.HeritageGroup);

        public int? Gender => GetProperty(PropertyInt.Gender);


        public ulong? MonarchId
        {
            get => GetProperty(PropertyInstanceId.Monarch);
            set { if (!value.HasValue) RemoveProperty(PropertyInstanceId.Monarch); else SetProperty(PropertyInstanceId.Monarch, value.Value); }
        }

        public ulong? PatronId
        {
            get => GetProperty(PropertyInstanceId.Patron);
            set { if (!value.HasValue) RemoveProperty(PropertyInstanceId.Patron); else SetProperty(PropertyInstanceId.Patron, value.Value); }
        }

        public ulong AllegianceXPCached
        {
            get => (ulong)(GetProperty(PropertyInt64.AllegianceXPCached) ?? 0);
            set { if (value == 0) RemoveProperty(PropertyInt64.AllegianceXPCached); else SetProperty(PropertyInt64.AllegianceXPCached, (long)value); }
        }

        public ulong AllegianceXPGenerated
        {
            get => (ulong)(GetProperty(PropertyInt64.AllegianceXPGenerated) ?? 0);
            set { if (value == 0) RemoveProperty(PropertyInt64.AllegianceXPGenerated); else SetProperty(PropertyInt64.AllegianceXPGenerated, (long)value); }
        }

        public int? AllegianceRank
        {
            get => GetProperty(PropertyInt.AllegianceRank);
            set { if (!value.HasValue) RemoveProperty(PropertyInt.AllegianceRank); else SetProperty(PropertyInt.AllegianceRank, value.Value); }
        }

        public int? AllegianceOfficerRank
        {
            get => GetProperty(PropertyInt.AllegianceOfficerRank);
            set { if (!value.HasValue) RemoveProperty(PropertyInt.AllegianceOfficerRank); else SetProperty(PropertyInt.AllegianceOfficerRank, value.Value); }
        }

        /// <summary>
        /// This flag indicates if a player can pass up allegiance XP
        /// </summary>
        public bool ExistedBeforeAllegianceXpChanges
        {
            get => GetProperty(PropertyBool.ExistedBeforeAllegianceXpChanges) ?? true;
            set { if (value) RemoveProperty(PropertyBool.ExistedBeforeAllegianceXpChanges); else SetProperty(PropertyBool.ExistedBeforeAllegianceXpChanges, value); }
        }

        /// <summary>
        /// Used for allegiance recall to monarch's mansion / villa
        /// </summary>
        public ulong? HouseInstance
        {
            get => GetProperty(PropertyInstanceId.House);
            set { if (!value.HasValue) RemoveProperty(PropertyInstanceId.House); else SetProperty(PropertyInstanceId.House, value.Value); }
        }

        public int? HousePurchaseTimestamp
        {
            get => GetProperty(PropertyInt.HousePurchaseTimestamp);
            set { if (!value.HasValue) RemoveProperty(PropertyInt.HousePurchaseTimestamp); else SetProperty(PropertyInt.HousePurchaseTimestamp, value.Value); }
        }

        public int? HouseRentTimestamp
        {
            get => GetProperty(PropertyInt.HouseRentTimestamp);
            set { if (!value.HasValue) RemoveProperty(PropertyInt.HouseRentTimestamp); else SetProperty(PropertyInt.HouseRentTimestamp, value.Value); }
        }

        public uint? HouseId
        {
            get => GetProperty(PropertyDataId.HouseId);
            set { if (!value.HasValue) RemoveProperty(PropertyDataId.HouseId); else SetProperty(PropertyDataId.HouseId, value.Value); }
        }


        public uint GetCurrentLoyalty()
        {
            return (uint?)GetProperty(PropertyInt.CurrentLoyaltyAtLastLogoff) ?? 0;
        }

        public uint GetCurrentLeadership()
        {
            return (uint?)GetProperty(PropertyInt.CurrentLeadershipAtLastLogoff) ?? 0;
        }


        public Allegiance Allegiance { get; set; }

        public AllegianceNode AllegianceNode { get; set; }

        public void UpdateProperty(PropertyInstanceId prop, ulong? value, bool broadcast = false)
        {
            if (value != null)
                SetProperty(prop, value.Value);
            else
                RemoveProperty(prop);
        }

        public int? HomeRealmIDRaw => GetProperty(PropertyInt.HomeRealm);
        public ushort HomeRealm
        {
            get
            {
                int intid = GetProperty(PropertyInt.HomeRealm) ?? 0;
                if ((intid < 0) || (uint)intid > 0x7FFF)
                {
                    log.Error("Player " + Name + " HomeRealm out of range.");
                    return 0;
                }
                return (ushort)intid;
            }
            internal set
            {
                if (value == 0)
                {
                    RemoveProperty(PropertyInt.HomeRealm);
                    return;
                }
                if (value > 0x7FFF)
                {
                    log.Error("Cannot set HomeRealm for Player " + Name + ". Must be between 0 and 32767");
                    return;
                }
                SetProperty(PropertyInt.HomeRealm, value);
            }
        }

        // This will always return a valid display name, but is not guaranteed to return a valid realm name
        public string DisplayedHomeRealmName => Managers.RealmManager.GetDisplayNameForAnyRawRealmId(HomeRealmIDRaw);
    }
}
