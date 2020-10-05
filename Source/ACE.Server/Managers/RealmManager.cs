using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ACE.Database;
using ACE.Entity.Models;
using ACE.Server.Realms;
using ACE.Database.Adapter;
using ACE.Entity.Enum.Properties;
using System.Linq;
using ACE.Server.WorldObjects;
using ACE.Server.Entity;

namespace ACE.Server.Managers
{
    public static class RealmManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly ReaderWriterLockSlim realmsLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<ushort, WorldRealm> Realms = new Dictionary<ushort, WorldRealm>();
        private static readonly Dictionary<string, WorldRealm> RealmsByName = new Dictionary<string, WorldRealm>();

        public static ACE.Entity.Models.Realm DefaultRealm { get; private set; }

        public static Dictionary<RealmPropertyBool, RealmPropertyBoolAttribute> PropertyDefinitionsBool;
        public static Dictionary<RealmPropertyInt, RealmPropertyIntAttribute> PropertyDefinitionsInt;
        public static Dictionary<RealmPropertyInt64, RealmPropertyInt64Attribute> PropertyDefinitionsInt64;
        public static Dictionary<RealmPropertyFloat, RealmPropertyFloatAttribute> PropertyDefinitionsFloat;
        public static Dictionary<RealmPropertyString, RealmPropertyStringAttribute> PropertyDefinitionsString;

        private static Dictionary<E, A> MakePropDict<E, A>()
        {
            return typeof(E).GetEnumNames().Select(n =>
            {
                var value = (E)Enum.Parse(typeof(E), n);
                var attributes = typeof(E).GetMember(n)
                    .FirstOrDefault(m => m.DeclaringType == typeof(E))
                    .GetCustomAttributes(typeof(A), false);

                if (attributes.Length == 0)
                    throw new Exception($"Enum {typeof(E).Name}.{n} is missing a {typeof(A)} attribute.");
                if (attributes.Length != 1)
                    throw new Exception($"Enum {typeof(E).Name}.{n} must have no more than 1 {typeof(A)} attributes.");

                var attribute = (A)attributes[0];
                return (value, attribute);
            }).ToDictionary((pair) => pair.value, (pair) => pair.attribute);
        }
        public static void Initialize()
        {
            PropertyDefinitionsBool = MakePropDict<RealmPropertyBool, RealmPropertyBoolAttribute>();
            PropertyDefinitionsInt = MakePropDict<RealmPropertyInt, RealmPropertyIntAttribute>();
            PropertyDefinitionsInt64 = MakePropDict<RealmPropertyInt64, RealmPropertyInt64Attribute>();
            PropertyDefinitionsString = MakePropDict<RealmPropertyString, RealmPropertyStringAttribute>();
            PropertyDefinitionsFloat = MakePropDict<RealmPropertyFloat, RealmPropertyFloatAttribute>();

            /* var results = DatabaseManager.World.GetAllRealms();

             foreach(var realm in results)
             {
                 lock (realmsLock)
                 {
                     realms[realm.Id] = realm;
                 }
             }*/
        }

        public static WorldRealm GetRealm(ushort? realm_id)
        {
            if (!realm_id.HasValue)
                return null;
            var realmId = realm_id.Value;

            lock (realmsLock)
            {
                if (realmId > 0x7FFF)
                    return null;
                if (Realms.TryGetValue(realmId, out var realm))
                    return realm;

                Database.Models.World.Realm dbitem;
                if (realmId == 0)
                {
                    dbitem = new Database.Models.World.Realm();
                    dbitem.Type = 1;
                    dbitem.Name = "Undefined Realm";
                }
                else
                    dbitem = DatabaseManager.World.GetRealm(realmId);

                if (dbitem == null)
                    return null;

                var erealm = RealmConverter.ConvertToEntityRealm(dbitem, true);
                if (!ValidateCircularDependency(erealm))
                {
                    log.Error("Circular inheritance chain detected for realm " + erealm.Id.ToString());
                    Realms[realmId] = null;
                    return null;
                }
                var ruleset = BuildRuleset(erealm);
                realm = new WorldRealm(erealm, ruleset);
                Realms[realmId] = realm;
                RealmsByName[erealm.Name] = realm;
                return realm;
            }
        }

        public static WorldRealm GetRealm(string name)
        {
            if (!RealmsByName.ContainsKey(name))
                return null;
            return GetRealm(RealmsByName[name].Realm.Id);
        }

        private static bool ValidateCircularDependency(ACE.Entity.Models.Realm realm)
        {
            var parentids = new HashSet<ushort>();
            parentids.Add(realm.Id);
            while(realm.ParentRealmID != null)
            {
                if (parentids.Contains(realm.ParentRealmID.Value))
                    return false;
                realm = GetRealm(realm.ParentRealmID.Value).Realm;
            }
            return true;
        }

        private static AppliedRuleset BuildRuleset(ACE.Entity.Models.Realm realm)
        {
            if (realm.ParentRealmID == null)
                return AppliedRuleset.MakeTopLevelRuleset(realm);
            var parent = GetRealm(realm.ParentRealmID.Value);
            return AppliedRuleset.ApplyRuleset(parent.Ruleset, realm);
        }

        internal static void ClearCache()
        {
            lock (realmsLock)
            {
                foreach (var realm in Realms.Values.Where(x => x != null))
                {
                    realm.NeedsRefresh = true;
                }

                DatabaseManager.World.ClearRealmCache();
                Realms.Clear();
                RealmsByName.Clear();
            }
        }

        internal static WorldRealm GetBaseRealm(Player player)
        {
            if (!player.Location.IsEphemeralRealm)
                return RealmsByName[player.RealmRuleset.Realm.Name];

            var realmId = player.GetPosition(PositionType.EphemeralRealmExitTo)?.RealmID ?? player.HomeRealm;
            return Realms[realmId];
        }

        internal static Landblock GetNewEphemeralLandblock(uint landcell, Player owner, ACE.Entity.Models.Realm realmTemplate)
        {
            EphemeralRealm ephemeralRealm = EphemeralRealm.Initialize(owner, realmTemplate);
            var iid = LandblockManager.GetFreeInstanceID(true, ephemeralRealm.Ruleset.Realm.Id, (ushort)(landcell >> 16));
            var longcell = ((ulong)iid << 32) | landcell;
            var landblock = LandblockManager.GetLandblock(longcell, false, false, ephemeralRealm);

            log.Info($"GetNewEphemeralLandblock created for player {owner.Name}, realm ruleset {ephemeralRealm.Ruleset.Realm.Id}, longcell {longcell}.");
            return landblock;
        }

        internal static void FullUpdateRealmsRepository(Dictionary<string, Database.Models.World.Realm> realmsDict,
            Dictionary<ushort, Database.Models.World.Realm> realmsById)
        {
            lock(realmsLock)
            {
                if (!ValidateRealmUpdates(realmsDict, realmsById))
                    return;

                try
                {
                    DatabaseManager.World.ReplaceAllRealms(realmsById);
                    ClearCache();
                }
                catch(Exception ex)
                {
                    log.Error(ex.Message, ex);
                    throw;
                }
            }
        }

        private static bool ValidateRealmUpdates(Dictionary<string, Database.Models.World.Realm> newRealmsByName,
            Dictionary<ushort, Database.Models.World.Realm> newRealmsById)
        {
            //Make sure that existing realms aren't being renamed
            foreach (var realm in RealmsByName.Values)
            {
                if (newRealmsByName.TryGetValue(realm.Realm.Name, out var newRealm) && newRealm.Id != realm.Realm.Id)
                {
                    log.Error($"Realm {realm.Realm.Name} attempted to have its numeric ID changed to a different value during realm import, which is not supported.");
                    return false;
                }
                if (newRealmsById.TryGetValue(realm.Realm.Id, out var newRealm2) && newRealm2.Name != realm.Realm.Name)
                {
                    log.Error($"Realm {realm.Realm.Id} ({realm.Realm.Name}) attempted to have its unique name changed to a different value during realm import, which is not supported.");
                    return false;
                }
            }

            foreach(var realmId in Realms.Keys)
            {
               if (!newRealmsById.ContainsKey(realmId))
                {
                    log.Error($"Realm {realmId} is missing in realms.json. Realms may not be removed once added. Unable to continue sync.");
                    return false;
                }
            }

            foreach (var realmName in RealmsByName.Keys)
            {
                if (!newRealmsByName.ContainsKey(realmName))
                {
                    log.Error($"Realm {realmName} is missing in realms.json. Realms may not be removed once added. Unable to continue sync.");
                    return false;
                }
            }
            return true;
        }
    }
}
