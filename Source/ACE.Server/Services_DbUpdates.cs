using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;

using ACE.Common;
using ACE.Database;
using ACE.Database.Models.World;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ACE.Server
{
    partial class Services
    {
        private static void CheckForWorldDatabaseUpdate(IServiceProvider services)
        {
            log.Info($"Automatic World Database Update started...");

            try
            {
                var ctxFactory = services.GetRequiredService<IDbContextFactory<WorldDbContext>>();

                var currentVersion = WorldDatabase.GetVersion(ctxFactory);

                log.Info($"Current World Database version: Base - {currentVersion?.BaseVersion ?? "No Version"} | Patch - {currentVersion?.PatchVersion ?? "No Version"}");

                var url = "https://api.github.com/repos/ACEmulator/ACE-World-16PY-Patches/releases/latest";

                using var client = new WebClient();
                var html = client.GetStringFromURL(url).Result;
                var json = JsonSerializer.Deserialize<JsonElement>(html);
                string tag = json.GetProperty("tag_name").GetString();
                string dbURL = json.GetProperty("assets")[0].GetProperty("browser_download_url").GetString();
                string dbFileName = json.GetProperty("assets")[0].GetProperty("name").GetString();

                if (currentVersion == null || currentVersion.PatchVersion != tag)
                {
                    var tagSplit = tag.Split(".");
                    int patchMajor, patchMinor, patchBuild;

                    if (currentVersion != null)
                    {
                        var patchVersionSplit = currentVersion.PatchVersion.Split(".");
                        int.TryParse(patchVersionSplit[0], out patchMajor);
                        int.TryParse(patchVersionSplit[1], out patchMinor);
                        int.TryParse(patchVersionSplit[2], out patchBuild);
                    }
                    else
                    {
                        patchMajor = 0; patchMinor = 0; patchBuild = 0;
                    }
                    int.TryParse(tagSplit[0], out var tagMajor);
                    int.TryParse(tagSplit[1], out var tagMinor);
                    int.TryParse(tagSplit[2], out var tagBuild);

                    if (tagMajor > patchMajor || tagMinor > patchMinor || (tagBuild > patchBuild && patchBuild != 0))
                    {
                        log.Info($"Latest patch version is {tag} -- Update Required!");
                        UpdateToLatestWorldDatabase(dbURL, dbFileName);

                        var newVersion = WorldDatabase.GetVersion(ctxFactory);

                        log.Info($"Updated World Database version: Base - {newVersion.BaseVersion} | Patch - {newVersion.PatchVersion}");
                    }
                    else
                    {
                        log.Info($"Latest patch version is {tag} -- No Update Required!");
                    }
                }
                else
                {
                    log.Info($"Latest patch version is {tag} -- No Update Required!");
                }
            }
            catch (Exception ex)
            {
                log.Info($"Unable to continue with Automatic World Database Update due to the following error: {ex}");
            }
            log.Info($"Automatic World Database Update complete.");
        }

        private static void UpdateToLatestWorldDatabase(string dbURL, string dbFileName)
        {
            // For security, validate the filename returned from the server, because we're deleting the previous file with that name.
            foreach (var badChar in Path.GetInvalidFileNameChars())
            {
                if (dbFileName.Contains(badChar))
                    throw new InvalidDataException($"The filename {dbFileName} contains illegal characters.");
            }

            Console.WriteLine();

            if (Program.IsRunningInContainer)
            {
                Console.WriteLine(" ");
                Console.WriteLine("This process will take a while, depending on many factors, and may look stuck while reading and importing the world database, please be patient! ");
                Console.WriteLine(" ");
            }

            Console.Write($"Downloading {dbFileName} .... ");
            using var client = new WebClient();
            try
            {
                var dlTask = client.DownloadFile(dbURL, dbFileName);
                dlTask.Wait();
            }
            catch
            {
                Console.Write($"Download for {dbFileName} failed!");
                return;
            }
            Console.WriteLine("download complete!");

            Console.Write($"Extracting {dbFileName} .... ");
            ZipFile.ExtractToDirectory(dbFileName, ".", true);
            Console.WriteLine("extraction complete!");
            Console.Write($"Deleting {dbFileName} .... ");
            File.Delete(dbFileName);
            Console.WriteLine("Deleted!");

            var sqlFile = dbFileName.Substring(0, dbFileName.Length - 4);
            Console.Write($"Importing {sqlFile} into SQL server at {ConfigManager.Config.MySql.World.Host}:{ConfigManager.Config.MySql.World.Port} (This will take a while, please be patient) .... ");
            using (var sr = File.OpenText(sqlFile))
            {
                var sqlConnect = new MySqlConnector.MySqlConnection($"server={ConfigManager.Config.MySql.World.Host};port={ConfigManager.Config.MySql.World.Port};user={ConfigManager.Config.MySql.World.Username};password={ConfigManager.Config.MySql.World.Password};{ConfigManager.Config.MySql.World.ConnectionOptions};database={ConfigManager.Config.MySql.World.Database}");

                StringBuilder completeSQLline = new StringBuilder();
                bool create_table_encountered = false;
                var line = string.Empty;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 0)
                        continue;
                    if (line.Contains("CREATE TABLE"))
                        create_table_encountered = true;

                    // AC Realms World DB Updates do not drop and re-create the world db, as there may be separate tables that we want to keep
                    if (!create_table_encountered)
                    {
                        if (line.Contains("DROP DATABASE"))
                            continue;
                        if (line.Contains("CREATE DATABASE"))
                            continue;
                        if (line.Contains("USE `ace_world`;"))
                            continue;
                    }

                    //do minimal amount of work here
                    if (line.EndsWith(";"))
                    {
                        string work;
                        if (completeSQLline.Length != 0)
                        {
                            completeSQLline.AppendLine(line);
                            work = completeSQLline.ToString();
                            completeSQLline.Clear();
                        }
                        else
                            work = line;

                        try
                        {
                            ExecuteScript(new MySqlConnector.MySqlCommand(work, sqlConnect));
                        }
                        catch (MySqlConnector.MySqlException) { }
                    }
                    else
                        completeSQLline.AppendLine(line);
                }
                CleanupConnection(sqlConnect);
            }
            Console.WriteLine(" complete!");

            Console.Write($"Deleting {sqlFile} .... ");
            File.Delete(sqlFile);
            Console.WriteLine("Deleted!");
        }

        private static string GetContentFolder()
        {
            var sqlConnect = new MySqlConnector.MySqlConnection($"server={ConfigManager.Config.MySql.Shard.Host};port={ConfigManager.Config.MySql.Shard.Port};user={ConfigManager.Config.MySql.Shard.Username};password={ConfigManager.Config.MySql.Shard.Password};database={ConfigManager.Config.MySql.Shard.Database};{ConfigManager.Config.MySql.Shard.ConnectionOptions}");
            var sqlQuery = "SELECT `value` FROM config_properties_string WHERE `key` = 'content_folder';";
            var sqlCommand = new MySqlConnector.MySqlCommand(sqlQuery, sqlConnect);

            sqlConnect.Open();
            var sqlReader = sqlCommand.ExecuteReader();

            var content_folder = "";

            if (sqlReader.HasRows)
            {
                while (sqlReader.Read())
                {
                    content_folder = sqlReader.GetString(0);
                    break;
                }
            }
            else
                content_folder = @".\Content";

            sqlReader.Close();
            sqlCommand.Connection.Close();

            // handle relative path
            if (content_folder.StartsWith("."))
            {
                var cwd = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
                content_folder = cwd + content_folder;
            }

            return content_folder;
        }

        private static void AutoApplyWorldCustomizations()
        {
            var content_folders_search_option = ConfigManager.Config.Offline.RecurseWorldCustomizationPaths ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var content_folders = new List<string> { GetContentFolder() };
            content_folders.AddRange(ConfigManager.Config.Offline.WorldCustomizationAddedPaths);
            content_folders.Sort();

            Console.WriteLine($"Searching for World Customization SQL scripts .... ");

            content_folders.ForEach(path =>
            {
                var contentDI = new DirectoryInfo(path);
                contentDI = new DirectoryInfo(Path.Combine(contentDI.FullName, "sql")); // Require explicit sql folder for this
                if (contentDI.Exists)
                {
                    Console.WriteLine($"Searching for SQL files within {path} .... ");
                    var sqlConnect = new MySqlConnector.MySqlConnection($"server={ConfigManager.Config.MySql.World.Host};port={ConfigManager.Config.MySql.World.Port};user={ConfigManager.Config.MySql.World.Username};password={ConfigManager.Config.MySql.World.Password};database={ConfigManager.Config.MySql.World.Database};{ConfigManager.Config.MySql.World.ConnectionOptions}");
                    foreach (var file in contentDI.GetFiles("*.sql", content_folders_search_option).OrderBy(f => f.FullName))
                    {
                        Console.Write($"Found {file.FullName} .... ");
                        var sqlDBFile = File.ReadAllText(file.FullName);
                        sqlDBFile = sqlDBFile.Replace("realms_world", ConfigManager.Config.MySql.World.Database);
                        sqlDBFile = sqlDBFile.Replace("ace_world", ConfigManager.Config.MySql.World.Database);
                        var script = new MySqlConnector.MySqlCommand(sqlDBFile, sqlConnect);

                        Console.Write($"Importing into World database on SQL server at {ConfigManager.Config.MySql.World.Host}:{ConfigManager.Config.MySql.World.Port} .... ");
                        try
                        {
                            ExecuteScript(script);
                            //Console.Write($" {count} database records affected ....");
                            Console.WriteLine(" complete!");
                        }
                        catch (MySqlConnector.MySqlException ex)
                        {
                            Console.WriteLine($" error!");
                            Console.WriteLine($" Unable to apply patch due to following exception: {ex}");
                        }
                    }
                    CleanupConnection(sqlConnect);
                }
                else
                {
                    log.Error($"Could not find content path {contentDI.FullName}");
                }
            });

            Console.WriteLine($"World Customization SQL scripts import complete!");
        }

        private static void AutoApplyDatabaseUpdates()
        {
            log.Info($"Automatic Database Patching not implemented for AC Realms.");
            return;

            //log.Info($"Automatic Database Patching started...");
            //Thread.Sleep(1000);

            //PatchDatabase("Authentication", ConfigManager.Config.MySql.Authentication.Host, ConfigManager.Config.MySql.Authentication.Port, ConfigManager.Config.MySql.Authentication.Username, ConfigManager.Config.MySql.Authentication.Password, ConfigManager.Config.MySql.Authentication.Database, ConfigManager.Config.MySql.Shard.Database, ConfigManager.Config.MySql.World.Database);
            //PatchDatabase("Shard", ConfigManager.Config.MySql.Shard.Host, ConfigManager.Config.MySql.Shard.Port, ConfigManager.Config.MySql.Shard.Username, ConfigManager.Config.MySql.Shard.Password, ConfigManager.Config.MySql.Authentication.Database, ConfigManager.Config.MySql.Shard.Database, ConfigManager.Config.MySql.World.Database);
            //PatchDatabase("World", ConfigManager.Config.MySql.World.Host, ConfigManager.Config.MySql.World.Port, ConfigManager.Config.MySql.World.Username, ConfigManager.Config.MySql.World.Password, ConfigManager.Config.MySql.Authentication.Database, ConfigManager.Config.MySql.Shard.Database, ConfigManager.Config.MySql.World.Database);

            //Thread.Sleep(1000);
            //log.Info($"Automatic Database Patching complete.");
        }

        private static void PatchDatabase(string dbType, string host, uint port, string username, string password, string authDB, string shardDB, string worldDB)
        {
            //var updatesPath = $"DatabaseSetupScripts{Path.DirectorySeparatorChar}Updates{Path.DirectorySeparatorChar}{dbType}";
            //var updatesFile = $"{updatesPath}{Path.DirectorySeparatorChar}applied_updates.txt";

            //if (!Directory.Exists(updatesPath))
            //{
            //    // File not found in Environment.CurrentDirectory
            //    // Lets try the ExecutingAssembly Location
            //    var executingAssemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;

            //    var directoryName = Path.GetFullPath(Path.GetDirectoryName(executingAssemblyLocation));

            //    updatesPath = Path.Combine(directoryName, $"DatabaseSetupScripts{Path.DirectorySeparatorChar}Updates{Path.DirectorySeparatorChar}{dbType}");

            //    if (!Directory.Exists(updatesPath))
            //    {
            //        Console.WriteLine($" error!");
            //        Console.WriteLine($" Unable to locate updates directory");
            //    }
            //    else
            //    {
            //        updatesFile = $"{updatesPath}{Path.DirectorySeparatorChar}applied_updates.txt";
            //    }

            //}

            //var appliedUpdates = Array.Empty<string>();

            //var containerUpdatesFile = $"/ace/Config/{dbType}_applied_updates.txt";
            //if (IsRunningInContainer && File.Exists(containerUpdatesFile))
            //    File.Copy(containerUpdatesFile, updatesFile, true);

            //if (File.Exists(updatesFile))
            //    appliedUpdates = File.ReadAllLines(updatesFile);

            //Console.WriteLine($"Searching for {dbType} update SQL scripts .... ");
            //foreach (var file in new DirectoryInfo(updatesPath).GetFiles("*.sql").OrderBy(f => f.Name))
            //{
            //    if (appliedUpdates.Contains(file.Name))
            //        continue;

            //    Console.Write($"Found {file.Name} .... ");
            //    var sqlDBFile = File.ReadAllText(file.FullName);
            //    var database = "";
            //    switch (dbType)
            //    {
            //        case "Authentication":
            //            database = authDB;
            //            break;
            //        case "Shard":
            //            database = shardDB;
            //            break;
            //        case "World":
            //            database = worldDB;
            //            break;
            //    }
            //    var sqlConnect = new MySqlConnector.MySqlConnection($"server={host};port={port};user={username};password={password};database={database};DefaultCommandTimeout=120;SslMode=None;AllowPublicKeyRetrieval=true");
            //    sqlDBFile = sqlDBFile.Replace("realms_auth", authDB);
            //    sqlDBFile = sqlDBFile.Replace("realms_shard", shardDB);
            //    sqlDBFile = sqlDBFile.Replace("realms_world", worldDB);
            //    var script = new MySqlConnector.MySqlCommand(sqlDBFile, sqlConnect);

            //    Console.Write($"Importing into {database} database on SQL server at {host}:{port} .... ");
            //    try
            //    {
            //        ExecuteScript(script);
            //        //Console.Write($" {count} database records affected ....");
            //        Console.WriteLine(" complete!");
            //    }
            //    catch (MySqlConnector.MySqlException ex)
            //    {
            //        Console.WriteLine($" error!");
            //        Console.WriteLine($" Unable to apply patch due to following exception: {ex}");
            //    }
            //    File.AppendAllText(updatesFile, file.Name + Environment.NewLine);
            //    CleanupConnection(sqlConnect);
            //}

            //if (IsRunningInContainer && File.Exists(updatesFile))
            //    File.Copy(updatesFile, containerUpdatesFile, true);

            //Console.WriteLine($"{dbType} update SQL scripts import complete!");
        }
    }
}
