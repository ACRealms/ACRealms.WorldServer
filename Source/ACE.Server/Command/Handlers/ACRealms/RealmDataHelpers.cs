using ACE.Database;
using ACE.Database.Adapter;
using ACE.Database.SQLFormatters.World;
using ACE.Server.Command.Handlers.Processors;
using ACE.Server.Managers;
using ACE.Server.Network;
using log4net;
using System;
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

        public static void ImportJsonRealmsIndex(ISession session, string realmsIndexJsonFile, List<RealmToImport> realms)
        {
            Dictionary<string, RealmToImport> realmsDict = null;
            Dictionary<ushort, RealmToImport> realmsById = new Dictionary<ushort, RealmToImport>();
            try
            {
                realmsDict = realms.ToDictionary(x => x.Realm.Name);
            }
            catch
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Couldn't create realms dictionary. Is there a problem with a realm name?");
                return;
            }

            Dictionary<string, ushort> result = null;
            try
            {
                var fileText = File.ReadAllText(realmsIndexJsonFile);
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ushort>>(fileText);

                //Map ids
                foreach (var item in result)
                {
                    var importItem = realmsDict[item.Key];
                    importItem.Realm.SetId(item.Value);
                    realmsById.Add(item.Value, importItem);
                }

                //Map parents
                foreach (var item in result)
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
                foreach (var item in result)
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
                return;
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
                return;
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
                    return null;
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
