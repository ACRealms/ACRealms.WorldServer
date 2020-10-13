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
using ACE.Server.Command.Handlers.Processors;

namespace ACE.Server.Managers
{
    public static class RealmManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string NULL_REALM_NAME = "null-realm";
        private static readonly ReaderWriterLockSlim realmsLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<ushort, WorldRealm> Realms = new Dictionary<ushort, WorldRealm>();
        private static readonly Dictionary<string, WorldRealm> RealmsByName = new Dictionary<string, WorldRealm>();
        private static readonly Dictionary<(WorldRealm, Realm), RulesetTemplate> EphemeralRealmCache = new Dictionary<(WorldRealm, Realm), RulesetTemplate>();
        private static bool ImportComplete;

        private static WorldRealm _defaultRealm;
        public static WorldRealm DefaultRealm
        {
            get { return _defaultRealm; }
            private set
            {
                _defaultRealm = value;
                DefaultRuleset = AppliedRuleset.MakeRerolledRuleset(value.RulesetTemplate);
            }
        }
        public static AppliedRuleset DefaultRuleset { get; private set; }


        public static void Initialize()
        {
            RealmConverter.Initialize();

            var erealm = RealmConverter.ConvertToEntityRealm(new Database.Models.World.Realm()
            {
                Id = 0,
                Name = NULL_REALM_NAME,
                Type = 1
            }, true);

            DefaultRealm = new WorldRealm(erealm, RulesetTemplate.MakeTopLevelRuleset(erealm));
            /* var results = DatabaseManager.World.GetAllRealms();

             foreach(var realm in results)
             {
                 lock (realmsLock)
                 {
                     realms[realm.Id] = realm;
                 }
             }*/

            //Import-realms
            DeveloperContentCommands.HandleImportRealms(null, null);
            if (!ImportComplete)
                throw new Exception("Import of realms.jsonc did not complete successfully.");
        }

        public static WorldRealm GetRealm(ushort? realm_id)
        {
            if (!realm_id.HasValue)
                return null;
            var realmId = realm_id.Value;
            if (realmId == 0)
                return DefaultRealm;
            if (realmId > 0x7FFF)
                return null;

            lock (realmsLock)
            {
                if (Realms.TryGetValue(realmId, out var realm))
                    return realm;

                var dbitem = DatabaseManager.World.GetRealm(realmId);
                if (dbitem == null)
                    return null;

                var erealm = RealmConverter.ConvertToEntityRealm(dbitem, true);
                var ruleset = BuildRuleset(erealm);
                realm = new WorldRealm(erealm, ruleset);
                Realms[realmId] = realm;
                RealmsByName[erealm.Name] = realm;
                return realm;
            }
        }

        private static RulesetTemplate BuildRuleset(ACE.Entity.Models.Realm realm)
        {
            if (realm.ParentRealmID == null)
                return RulesetTemplate.MakeTopLevelRuleset(realm);
            var parent = GetRealm(realm.ParentRealmID.Value);
            return RulesetTemplate.MakeRuleset(parent.RulesetTemplate, realm);
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
                EphemeralRealmCache.Clear();
            }
        }

        internal static WorldRealm GetBaseRealm(Player player)
        {
            if (!player.Location.IsEphemeralRealm)
                return GetRealm(player.RealmRuleset.Template.Realm.Id);

            var realmId = player.GetPosition(PositionType.EphemeralRealmExitTo)?.RealmID ?? player.HomeRealm;
            return GetRealm(realmId);
        }

        internal static Landblock GetNewEphemeralLandblock(uint landcell, Player owner, ACE.Entity.Models.Realm realmTemplate)
        {
            EphemeralRealm ephemeralRealm = EphemeralRealm.Initialize(owner, realmTemplate);
            var iid = LandblockManager.GetFreeInstanceID(true, ephemeralRealm.RulesetTemplate.Realm.Id, (ushort)(landcell >> 16));
            var longcell = ((ulong)iid << 32) | landcell;
            var landblock = LandblockManager.GetLandblock(longcell, false, false, ephemeralRealm);

            log.Info($"GetNewEphemeralLandblock created for player {owner.Name}, realm ruleset {ephemeralRealm.RulesetTemplate.Realm.Id}, longcell {longcell}.");
            return landblock;
        }

        internal static void FullUpdateRealmsRepository(Dictionary<string, RealmToImport> realmsDict,
            Dictionary<ushort, RealmToImport> realmsById)
        {
            lock (realmsLock)
            {
                if (!ValidateRealmUpdates(realmsDict, realmsById))
                    return;

                try
                {
                    DatabaseManager.World.ReplaceAllRealms(realmsById);
                    ClearCache();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                    throw;
                }
            }
            ImportComplete = true;
        }

        private static bool ValidateRealmUpdates(Dictionary<string, RealmToImport> newRealmsByName,
            Dictionary<ushort, RealmToImport> newRealmsById)
        {
            //Ensure realm 0 (null-realm) is not included in the new realms file
            if (newRealmsById.ContainsKey(0))
            {
                log.Error("realms.jsonc may not contain id 0, which is a reserved id.");
                return false;
            }
            if (newRealmsByName.ContainsKey(NULL_REALM_NAME))
            {
                log.Error($"May not import a realm named {NULL_REALM_NAME}, which is a reserved name.");
                return false;
            }

            //Check for renames
            foreach (var realm in RealmsByName.Values)
            {
                if (realm.Realm.Id == 0)
                    continue;

                if (newRealmsByName.TryGetValue(realm.Realm.Name, out var newRealmToImport) && newRealmToImport.Realm.Id != realm.Realm.Id)
                {
                    log.Error($"Realm {realm.Realm.Name} attempted to have its numeric ID changed to a different value during realm import, which is not supported.");
                    return false;
                }
                if (newRealmsById.TryGetValue(realm.Realm.Id, out var newRealmToImport2) && newRealmToImport2.Realm.Name != realm.Realm.Name)
                {
                    log.Error($"Realm {realm.Realm.Id} ({realm.Realm.Name}) attempted to have its unique name changed to a different value during realm import, which is not supported.");
                    return false;
                }
            }

            //Check for deletions
            foreach (var realmId in Realms.Keys)
            {
                if (realmId == 0)
                    continue;

                if (!newRealmsById.ContainsKey(realmId))
                {
                    log.Error($"Realm {realmId} is missing in realms.jsonc. Realms may not be removed once added. Unable to continue sync.");
                    return false;
                }
            }
            foreach (var realmName in RealmsByName.Keys)
            {
                if (realmName == NULL_REALM_NAME)
                    continue;

                if (!newRealmsByName.ContainsKey(realmName))
                {
                    log.Error($"Realm {realmName} is missing in realms.jsonc. Realms may not be removed once added. Unable to continue sync.");
                    return false;
                }
            }

            //Ensure realm in each link exists
            foreach (var realm in newRealmsByName.Values)
            {
                foreach (var link in realm.Links)
                {
                    if (!newRealmsByName.ContainsKey(link.Import_RulesetToApply))
                    {
                        log.Error($"New realm {realm.Realm.Name} has a linked realm {link.Import_RulesetToApply} which was not found in the import set. Unable to continue sync.");
                        return false;
                    }
                    link.RealmId = realm.Realm.Id;
                    link.LinkedRealmId = newRealmsByName[link.Import_RulesetToApply].Realm.Id;
                }
            }

            //Check for circular dependencies - from top to bottom of tree
            Queue<Database.Models.World.Realm> realmsToCheck = new Queue<Database.Models.World.Realm>();
            foreach (var importItem in newRealmsByName.Values)
            {
                if (importItem.Realm.ParentRealmId == null)
                {
                    realmsToCheck.Enqueue(importItem.Realm);
                }
            }

            HashSet<ushort> realmsChecked = new HashSet<ushort>();
            while (realmsToCheck.TryDequeue(out var realmToCheck))
            {
                if (realmsChecked.Contains(realmToCheck.Id))
                {
                    log.Error($"A circular dependency was detected when attempting to import realm {realmToCheck.Id}.");
                    return false;
                }

                realmsChecked.Add(realmToCheck.Id);
                foreach (var realm in realmToCheck.Descendents.Values)
                    realmsToCheck.Enqueue(realm);
            }

            if (realmsChecked.Count != newRealmsById.Count)
            {
                var badRealm = newRealmsById.First(x => !realmsChecked.Contains(x.Key)).Value;
                log.Error($"A circular dependency was detected when attempting to import realm {badRealm.Realm.Name}.");
                return false;
            }

            // Check for circular dependencies in realm links. Not sure of a good algorithm for this as it gets complex.
            // So for now, restrict realm links to only rulesets that do not have a parent.
            var itemsToCheck = newRealmsByName.Values.Select(i => new RealmToImportMarked() { ImportItem = i }).ToDictionary(x => x.ImportItem.Realm.Id);
            var unmarkedNodes = itemsToCheck.Values.ToHashSet();

            //Check for duplicate names (may be unnecessary due to the RecursiveCheckCircularDependency section below)
            foreach (var item in itemsToCheck.Values)
            {
                foreach (var link in item.ImportItem.Links)
                {
                    var thisRealm = newRealmsById[link.RealmId].Realm;
                    if (thisRealm.Name == link.Import_RulesetToApply || thisRealm.Id == link.LinkedRealmId || link.RealmId == link.LinkedRealmId)
                    {
                        log.Error($"Error importing realm {thisRealm.Name}: A realm cannot have a linked ruleset with the same name as that realm.");
                        return false;
                    }
                }
            }

            //DepthFirstSearch traversal to find circular references on entire graph
            while (unmarkedNodes.Count > 0)
            {
                var item = unmarkedNodes.First();
                try
                {
                    RecursiveCheckCircularDependency(unmarkedNodes.First(), itemsToCheck, unmarkedNodes);
                }
                catch (InvalidOperationException)
                {
                    log.Error($"Error importing realm {item.ImportItem.Realm.Name}: A circular dependency was detected.");
                    return false;
                }
            }

            return true;
        }

        private class RealmToImportMarked
        {
            public RealmToImport ImportItem { get; set; }
            public bool TemporaryMark { get; set; }
            public bool PermanentMark { get; set; }
        }

        //https://en.wikipedia.org/wiki/Topological_sorting#Depth-first_search
        private static void RecursiveCheckCircularDependency(RealmToImportMarked item, Dictionary<ushort, RealmToImportMarked> dict, HashSet<RealmToImportMarked> unmarkedNodes)
        {
            if (item.PermanentMark)
                return;
            if (item.TemporaryMark)
                throw new InvalidOperationException();
            item.TemporaryMark = true;
            if (item.ImportItem.Realm.ParentRealmId.HasValue)
                RecursiveCheckCircularDependency(dict[item.ImportItem.Realm.ParentRealmId.Value], dict, unmarkedNodes);
            foreach (var link in item.ImportItem.Links)
                RecursiveCheckCircularDependency(dict[link.LinkedRealmId], dict, unmarkedNodes);
            item.TemporaryMark = false;
            item.PermanentMark = true;
            unmarkedNodes.Remove(item);
        }

        internal static RulesetTemplate GetEphemeralRealmRulesetTemplate(WorldRealm baseRealm, Realm appliedRealm)
        {
            lock(realmsLock)
            {
                if (EphemeralRealmCache.TryGetValue((baseRealm, appliedRealm), out var storedruleset))
                    return storedruleset;
                return null;
            }
        }

        internal static RulesetTemplate SyncRulesetForEphemeralRealm(WorldRealm baseRealm, Realm appliedRealm, RulesetTemplate template)
        {
            var key = (baseRealm, appliedRealm);
            lock (realmsLock)
            {
                if (EphemeralRealmCache.TryGetValue(key, out var storedruleset))
                {
                    return storedruleset;
                }
                else
                { 
                    EphemeralRealmCache[key] = template;
                    return template;
                }
            }
        }
    }
}
