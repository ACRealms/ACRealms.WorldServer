using ACE.Database;
using ACE.Database.Adapter;
using ACE.Database.SQLFormatters.World;
using ACE.Entity.Enum.Properties;
using ACE.Server.Command.Handlers.Processors;
using ACE.Server.Managers;
using ACE.Server.Network;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Command.Handlers
{
    public static class RealmDataHelpers
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<RealmToImport> ImportJsonRealmsFolder(ISession session, string json_folder)
        {
            var sep = Path.DirectorySeparatorChar;
            var json_folder_realm = $"{json_folder}{sep}realm{sep}";
            var json_folder_ruleset = $"{json_folder}{sep}ruleset{sep}";

            var list = ImportJsonRealmsFromSubFolder(session, json_folder_realm).ToList();
            if (list == null)
                return null;

            var list2 = ImportJsonRealmsFromSubFolder(session, json_folder_ruleset);
            if (list2 != null)
                list.AddRange(list2);
            return list;
        }

        public static bool VerifyAndMergeRealmIndex(ISession session, Dictionary<string, ushort> priorLockedIndex, List<RealmToImport> realmsToImport, out Dictionary<string, ushort> newLockedIndex)
        {
            // Todo: Refactor to allow for rulesets but not realms to be removed from db
            var dbRealmsFull = DatabaseManager.World.GetAllRealms(cacheUsage: false);


            var dbWorldRealmIds = dbRealmsFull.Where(r => r.Type == (ushort)ACE.Entity.Enum.RealmType.Realm).Select(x => x.Id).ToHashSet();
            var dbWorldRulesetIds = dbRealmsFull.Where(r => r.Type == (ushort)ACE.Entity.Enum.RealmType.Ruleset).Select(x => x.Id).ToHashSet();

            var dbRealms = dbRealmsFull.ToDictionary(x => x.Name, x => x.Id);
            var priorIndexById = priorLockedIndex.ToDictionary(x => x.Value, x => x.Key);
            var dbRealmsById = dbRealms.ToDictionary(x => x.Value, x => x.Key);
            newLockedIndex = new Dictionary<string, ushort>(StringComparer.InvariantCultureIgnoreCase);

            bool changesDetected = false;

            
            foreach (var dbRealmName in dbRealms.Keys)
            {
                var dbRealmId = dbRealms[dbRealmName];

                if (!priorLockedIndex.ContainsKey(dbRealmName))
                {
                    if (Enum.TryParse<ReservedRealm>(dbRealmName, true, out var reservedRealm))
                        continue;

                    if (priorIndexById.ContainsKey(dbRealmId))
                    {
                        var takenRealmName = priorIndexById[dbRealmId];
                        throw new InvalidDataException($"Index file defines realm {takenRealmName} with id {dbRealmId} but but the a realm exists in the database with the same ID under the name {dbRealmName}");
                    }

                    CommandHandlerHelper.WriteOutputInfo(session, $"Syncing realm {dbRealmName} to index file.");
                    changesDetected = true;
                    newLockedIndex.Add(dbRealmName, dbRealmId);
                    continue;
                }
                else if (Enum.TryParse<ReservedRealm>(dbRealmName, true, out var reservedRealm))
                    throw new InvalidDataException($"A reserved realm exists with the name {reservedRealm} and may not be used as a realm name.");

                var priorRealmId = priorLockedIndex[dbRealmName];

                if (priorRealmId != dbRealmId)
                    throw new InvalidDataException($"Realm or Ruleset {dbRealmName} exists in DB with ID {dbRealmId} but defined in index file with id {priorRealmId}");
                else
                    newLockedIndex.Add(dbRealmName, priorLockedIndex[dbRealmName]);
            }

            foreach (var priorRealmName in priorLockedIndex.Keys)
            {
                if (newLockedIndex.ContainsKey(priorRealmName))
                    continue;

                var priorRealmId = priorLockedIndex[priorRealmName];

                if (dbRealmsById.ContainsKey(priorRealmId))
                    throw new InvalidDataException($"Realm or Ruleset {priorRealmName} exists in DB with ID {dbRealms[priorRealmName]} but defined in index file with id {priorRealmId}");
                else if (dbRealms.ContainsKey(priorRealmName))
                    throw new InvalidDataException($"Realm or Ruleset {priorRealmName} defined in index file with ID {priorRealmId} but defined in DB with id {dbRealms[priorRealmName]}");

                CommandHandlerHelper.WriteOutputInfo(session, $"Found new realm {priorRealmName} in index file with self-assigned ID of {priorRealmId}.");
                newLockedIndex.Add(priorRealmName, priorRealmId);
            }

            var reservedIds = newLockedIndex.Select(x => x.Value).ToHashSet();

            var reservedRealmIdsByEnum = Enum.GetValues<ReservedRealm>();
            var reservedRealmIdsRaw = reservedRealmIdsByEnum.Select(x => (ushort)x).ToHashSet();
            foreach(var enumVal in reservedRealmIdsByEnum)
            {
                if (reservedIds.Contains((ushort)enumVal) && !dbRealmsById.ContainsKey((ushort)enumVal))
                    throw new InvalidDataException($"The ID {(ushort)enumVal} is not permitted to be manually added to the index file because there is a system-defined realm {enumVal} with that ID.");
                reservedIds.Add((ushort)enumVal);
            }

            ushort nextId = 0;
            var findNextId = () => {
                while (reservedIds.Contains(++nextId)) { }
                
                return nextId;
            };
            

            foreach(var realm in realmsToImport)
            {
                var name = realm.Realm.Name;
                if (newLockedIndex.ContainsKey(name))
                    continue;
                changesDetected = true;
                newLockedIndex.Add(name, findNextId());
                reservedIds.Add(newLockedIndex[name]);
            }

            // Check for missing realms that exist in db
            var realmNamesDefinedInConfig = realmsToImport.Select(x => x.Realm.Name).ToHashSet(StringComparer.Ordinal);
            foreach (var dbRealmName in dbRealms.Keys)
            {
                if (reservedRealmIdsRaw.Contains(dbRealms[dbRealmName]))
                    continue;
                if (!realmNamesDefinedInConfig.Contains(dbRealmName))
                {
                    throw new InvalidDataException($"Expected realm file defining realm with name {dbRealmName} - Deletion of rulesets not implemented yet");
                }
            }


            // Be absolutely sure
            if (!changesDetected)
            {
                if (!priorLockedIndex.OrderBy(kvp => kvp.Key).SequenceEqual(newLockedIndex.OrderBy(kvp => kvp.Key)))
                    throw new InvalidOperationException("Internal error when attempting to sync the index file.");
            }
            else
            {
                if (priorLockedIndex.OrderBy(kvp => kvp.Key).SequenceEqual(newLockedIndex.OrderBy(kvp => kvp.Key)))
                    throw new InvalidOperationException("Internal error when attempting to sync the index file.");
            }

            return changesDetected;
        }

        public static void ImportJsonRealmsIndex(ISession session, string realmsIndexJsonFile, List<RealmToImport> realms)
        {
            Dictionary<string, RealmToImport> realmsDict = null;
            try
            {
                realmsDict = realms.ToDictionary(x => x.Realm.Name);
            }
            catch(Exception ex)
            {
                log.Error(ex);
                CommandHandlerHelper.WriteOutputError(session, $"Couldn't create realms dictionary. Is there a problem with a realm name?");
                throw;
            }

            Dictionary<string, ushort> realmsIndex = null;
            if (!File.Exists(realmsIndexJsonFile))
            {
                realmsIndex = new Dictionary<string, ushort>();
            }
            else
            {
                try
                {
                    var fileText = File.ReadAllText(realmsIndexJsonFile);
                    realmsIndex = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ushort>>(fileText);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    CommandHandlerHelper.WriteOutputError(session, $"Failed to interpret contents of {realmsIndexJsonFile}.");
                    throw;
                }
            }

            if (VerifyAndMergeRealmIndex(session, realmsIndex, realms, out var newIndex))
            {
                var newIndexJson = JsonConvert.SerializeObject(newIndex, Formatting.Indented) + Environment.NewLine;
                File.WriteAllText(realmsIndexJsonFile, newIndexJson);
                CommandHandlerHelper.WriteOutputInfo(session, $"Changes were made to index lockfile {realmsIndexJsonFile}, please commit these changes to your git repository.");
            }
            realmsIndex = newIndex;

            Dictionary<ushort, RealmToImport> realmsById = new Dictionary<ushort, RealmToImport>();
            try
            {
                //Map ids
                foreach (var item in realmsIndex)
                {
                    var importItem = realmsDict[item.Key];
                    importItem.Realm.SetId(item.Value);
                    realmsById.Add(item.Value, importItem);
                }

                //Map parents
                foreach (var item in realmsIndex)
                {
                    var importItem = realmsDict[item.Key];
                    if (importItem.Realm.ParentRealmName != null)
                    {
                        if (!realmsDict.TryGetValue(importItem.Realm.ParentRealmName, out var parentImportItem))
                            throw new Exception($"Couldn't find parent realm with name {importItem.Realm.ParentRealmName}");
                        importItem.Realm.ParentRealmId = parentImportItem.Realm.Id;
                    }
                }

                //Map descendents
                foreach (var item in realmsIndex)
                {
                    var importItem = realmsDict[item.Key];
                    if (importItem.Realm.ParentRealmId == null)
                        continue;

                    var parentImportItem = realmsById[importItem.Realm.ParentRealmId.Value];
                    parentImportItem.Realm.Descendents.Add(importItem.Realm.Id, importItem.Realm);
                }
            }
            catch (Exception ex)
            {
                log.Error($"Couldn't import {realmsIndexJsonFile}", ex);
                CommandHandlerHelper.WriteOutputInfo(session, $"Couldn't import {realmsIndexJsonFile}");
                throw;
            }

            try
            {
                RealmManager.FullUpdateRealmsRepository(realmsDict, realmsById);
                Console.WriteLine($"Imported {realmsById.Count} realms.");
            }
            catch (Exception ex)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Failed to update realms repository.");
                log.Error(ex.ToString());
                log.Error(ex.StackTrace);
                log.Error($"Failed to update realms repository." );
                throw;
            }
        }

        public static List<RealmToImport> ImportJsonRealmsFromSubFolder(ISession session, string json_folder)
        {
            var di = new DirectoryInfo(json_folder);

            var files = di.Exists ? di.GetFiles($"*.jsonc") : null;

            if (files == null)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Couldn't find {json_folder}*.jsonc");
                return null;
            }

            List<RealmToImport> list = new List<RealmToImport>();
            foreach (var file in files)
            {
                var jsondata = File.ReadAllText(file.FullName);
                var realmToImport = RealmManager.DeserializeRealmJson(session, file.FullName, jsondata);
                if (realmToImport == null)
                    throw new InvalidDataException($"Unable to deserialize realmfile {file.FullName}");
                list.Add(realmToImport);
            }
            return list;
        }

        public static void ExportSQLRealm(ISession session, string param)
        {
            DirectoryInfo di = DeveloperContentCommands.VerifyContentFolder(session, false);

            var sep = Path.DirectorySeparatorChar;

            if (param == "all")
            {
                var realms = DatabaseManager.World.GetAllRealms(true);
                foreach (var id in realms.Select(x => x.Id))
                    ExportSQLRealm(session, id.ToString());
                return;
            }
            if (!ushort.TryParse(param, out var realmId))
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"{param} not a valid realm id");
                return;
            }

            var realm = DatabaseManager.World.GetRealm(realmId);
            if (realm == null)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Couldn't find realm id {realmId}");
                return;
            }

            var sql_folder = $"{di.FullName}{sep}sql{sep}realms{sep}";

            di = new DirectoryInfo(sql_folder);

            if (!di.Exists)
                di.Create();

            var sqlWriter = new RealmSQLWriter();

            var sql_filename = sqlWriter.GetDefaultFileName(realm);

            try
            {
                var sqlFile = new StreamWriter(sql_folder + sql_filename);

                sqlFile.WriteLine("-- This export is intended for reference and troubleshooting purposes only.");
                sqlFile.WriteLine("-- It is not recommended to run these statements on a live server.");
                sqlWriter.CreateSQLDELETEStatement(realm, sqlFile);
                sqlFile.WriteLine();

                sqlWriter.CreateSQLINSERTStatement(realm, sqlFile);
                sqlFile.WriteLine();

                sqlFile.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CommandHandlerHelper.WriteOutputInfo(session, $"Failed to export {sql_folder}{sql_filename}");
                return;
            }

            CommandHandlerHelper.WriteOutputInfo(session, $"Exported {sql_folder}{sql_filename}");
        }
    }
}
