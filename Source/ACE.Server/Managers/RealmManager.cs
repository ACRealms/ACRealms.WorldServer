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
        private static readonly Dictionary<ushort, WorldRealm> realms = new Dictionary<ushort, WorldRealm>();
        private static readonly Dictionary<string, WorldRealm> realmsByName = new Dictionary<string, WorldRealm>();

        public static Realm DefaultRealm { get; private set; }

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
                if (realms.TryGetValue(realmId, out var realm))
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
                    realms[realmId] = null;
                    return null;
                }
                var ruleset = BuildRuleset(erealm);
                realm = new WorldRealm(erealm, ruleset);
                realms[realmId] = realm;
                realmsByName[erealm.Name] = realm;
                return realm;
            }
        }

        public static WorldRealm GetRealm(string name)
        {
            if (!realmsByName.ContainsKey(name))
                return null;
            return GetRealm(realmsByName[name].Realm.Id);
        }

        private static bool ValidateCircularDependency(Realm realm)
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

        private static AppliedRuleset BuildRuleset(Realm realm)
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
                foreach (var realm in realms.Values.Where(x => x != null))
                {
                    realm.NeedsRefresh = true;
                }

                DatabaseManager.World.ClearRealmCache();
                realms.Clear();
                realmsByName.Clear();
            }
        }

        internal static WorldRealm GetBaseRealm(Player player)
        {
            if (!player.Location.IsEphemeralRealm)
                return realmsByName[player.RealmRuleset.Realm.Name];

            var realmId = player.GetPosition(PositionType.EphemeralRealmExitTo)?.RealmID ?? player.HomeRealm;
            return realms[realmId];
        }

        internal static Landblock GetNewEphemeralLandblock(uint landcell, Player owner, Realm realmTemplate)
        {
            EphemeralRealm ephemeralRealm = EphemeralRealm.Initialize(owner, realmTemplate);
            var iid = LandblockManager.GetFreeInstanceID(true, ephemeralRealm.Ruleset.Realm.Id, (ushort)(landcell >> 16));
            var longcell = ((ulong)iid << 32) | landcell;
            var landblock = LandblockManager.GetLandblock(longcell, false, false, ephemeralRealm);

            log.Info($"GetNewEphemeralLandblock created for player {owner.Name}, realm ruleset {ephemeralRealm.Ruleset.Realm.Id}, longcell {longcell}.");
            return landblock;
        }
    }
}
