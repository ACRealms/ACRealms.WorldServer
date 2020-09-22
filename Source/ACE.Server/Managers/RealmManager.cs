using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ACE.Database;
using ACE.Entity.Models;
using ACE.Server.Realms;
using ACE.Database.Adapter;

namespace ACE.Server.Managers
{
    public static class RealmManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly ReaderWriterLockSlim realmsLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<ushort, WorldRealm> realms = new Dictionary<ushort, WorldRealm>();

        public static Realm DefaultRealm { get; private set; }

        public static void Initialize()
        {
           /* var results = DatabaseManager.World.GetAllRealms();

            foreach(var realm in results)
            {
                lock (realmsLock)
                {
                    realms[realm.Id] = realm;
                }
            }*/
        }

        public static WorldRealm GetRealm(ushort realmId)
        {
            lock (realmsLock)
            {
                if (realmId > 0x7FFF)
                    return null;
                if (realms.TryGetValue(realmId, out var realm))
                    return realm;
                var dbitem = DatabaseManager.World.GetRealm(realmId);
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
                return realm;
            }
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
                foreach (var realm in realms.Values)
                    realm.NeedsRefresh = true;

                DatabaseManager.World.ClearRealmCache();
                realms.Clear();
            }
        }

        internal static object GetRealm(object realmID)
        {
            throw new NotImplementedException();
        }
    }
}
