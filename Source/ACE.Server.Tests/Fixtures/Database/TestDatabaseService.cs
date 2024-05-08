using ACE.Common;
using ACE.Common.Cryptography;
using ACE.Database;
using ACE.Database.Models.Auth;
using ACE.Database.Models.Shard;
using ACE.Database.Models.World;
using ACE.Server;
using ACRealms.Tests.Fixtures;
using ACRealms.Tests.Fixtures.Database;
using ACRealms.Tests.Helpers;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace ACRealms.Tests.Fixtures.Database
{
    // http://xunitpatterns.com/Table%20Truncation%20Teardown.html
    // https://umplify.github.io/xunit-dependency-injection/
    // https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/
    public class TestDatabaseService : IDisposable
    {
        public static string TestUserName { get; } = "acrealms_test_db_user";
        private static string TestUserFull { get; } = $"'{TestUserName}'@'%'";
        public static string TestUserPassword { get; } = RandomPassword();

        // Can be temporarily false when trying to make specific tests pass (faster execution), but some tests need this to be true
        const bool RESET_DB_EACH_RUN = false;

        public static string AuthDbName { get; } = "acrealms_test_auth";
        public static string WorldDbName { get; } = "acrealms_test_world";
        public static string ShardDbName { get; } = "acrealms_test_shard";
        public static string DatabaseScriptRoot { get; } = $"{Paths.SolutionPath}/../Database";
        public static string AuthInitScript { get; } = $"{DatabaseScriptRoot}/Base/AuthenticationBase.sql";
        public static string WorldInitScript { get; } = $"{DatabaseScriptRoot}/Base/WorldBase.sql";
        public static string ShardInitScript { get; } = $"{DatabaseScriptRoot}/Base/ShardBase.sql";
        public static string WorldUpdatesDir { get; } = $"{DatabaseScriptRoot}/ACRealms/World";
        public static string ShardUpdatesDir { get; } = $"{DatabaseScriptRoot}/ACRealms/Shard";

        // World Db
        public static string WorldDbDownloadUrl { get; } = "https://github.com/ACEmulator/ACE-World-16PY-Patches/releases/download/v0.9.279/ACE-World-Database-v0.9.279.sql.zip";
        public static string WorldDbFolder { get; } = Path.Combine(Paths.LocalDataPath, "world_db");
        public static string WorldDbLocalScript { get; } = $"{WorldDbFolder}/ACE-World-Database-v0.9.279.sql";
        public const string WorldDbScriptMD5 = "bc912cf0ecaec2704347ddd338818bf1";
        public static string WorldDbLocalZip { get; } = $"{WorldDbLocalScript}.zip";
        
        public static void Initialize(IServiceCollection services)
        {
            CleanupDbState();
            using (var context = new RootDbContext())
            {
                var connection = context.Database.GetDbConnection();
                connection.Open();

                // Separate users with restricted privileges are created for tests, in case the repository or a script uses hardcoded database names somewhere
                var command = connection.CreateCommand();
                command.CommandText = @$"
                    CREATE USER {TestUserFull} IDENTIFIED BY '{TestUserPassword}';
                    CREATE DATABASE IF NOT EXISTS `{AuthDbName}`;
                    CREATE DATABASE IF NOT EXISTS `{WorldDbName}`;
                    CREATE DATABASE IF NOT EXISTS `{ShardDbName}`;
                    GRANT ALL PRIVILEGES ON {AuthDbName}.* TO {TestUserFull};
                    GRANT ALL PRIVILEGES ON {WorldDbName}.* TO {TestUserFull};
                    GRANT ALL PRIVILEGES ON {ShardDbName}.* TO {TestUserFull};";
                command.ExecuteNonQuery();
            }

            var config = ConfigManager.Config.MySql.World;
            var connectionStringBase = $"server={config.Host};port={config.Port};user={TestUserName};password={TestUserPassword};{config.ConnectionOptions}";

            services.AddDbContextFactory<AuthDbContext>(options =>
            {
                var connectionString = $"{connectionStringBase};database={AuthDbName}";
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }).AddDbContextFactory<WorldDbContext>(options =>
            {
                var connectionString = $"{connectionStringBase};database={WorldDbName}";
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }).AddDbContextFactory<ShardDbContext>(options =>
            {
                var connectionString = $"{connectionStringBase};database={ShardDbName}";
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
        }

        public static void BuildDBs(IServiceProvider provider)
        {
            bool validDb = false;
            try
            {
                using (var context = provider.GetRequiredService<IDbContextFactory<WorldDbContext>>().CreateDbContext())
                {
                    if (context.Weenie.First() != null)
                        validDb = true;
                }
            }
            catch (Exception) { }


            if (validDb && !RESET_DB_EACH_RUN)
            {
                using (var context = provider.GetRequiredService<IDbContextFactory<AuthDbContext>>().CreateDbContext())
                {
                    context.Account.RemoveRange(context.Account);
                    context.SaveChanges();
                }
                using (var context = provider.GetRequiredService<IDbContextFactory<ShardDbContext>>().CreateDbContext())
                {
                    context.Character.RemoveRange(context.Character);
                    context.SaveChanges();
                }

                return;
            }

            EnsureWorldDbDownloaded();

            using (var context = provider.GetRequiredService<IDbContextFactory<AuthDbContext>>().CreateDbContext())
            {
                var script = File.ReadAllText(AuthInitScript).Replace("realms_auth", AuthDbName);
                ExecuteScript(script, context);
            }
            
            using (var context = provider.GetRequiredService<IDbContextFactory<WorldDbContext>>().CreateDbContext())
            {
                var script = File.ReadAllText(WorldInitScript).Replace("realms_world", WorldDbName);
                ExecuteScript(script, context);

                using (var sr = File.OpenText(WorldDbLocalScript))
                {
                    var line = string.Empty;
                    var completeSQLline = string.Empty;
                    var connection = context.Database.GetDbConnection();
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Replace("ace_world", WorldDbName);
                        if (line.EndsWith(";"))
                        {
                            completeSQLline += line + Environment.NewLine;
                            var command = connection.CreateCommand();
                            command.CommandText = completeSQLline;
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (MySqlConnector.MySqlException)
                            {
                            }
                            completeSQLline = string.Empty;
                        }
                        else
                            completeSQLline += line + Environment.NewLine;
                    }
                    if (connection.State != System.Data.ConnectionState.Closed)
                    {
                        try
                        {
                            connection.Close();
                        }
                        catch
                        {
                        }
                    }
                }

                foreach(var worldUpdateScript in Directory.GetFiles(WorldUpdatesDir).OrderBy(_ => _))
                {
                    script = File.ReadAllText(worldUpdateScript).Replace("realms_world", WorldDbName);
                    ExecuteScript(script, context);
                }
            }
            using (var context = provider.GetRequiredService<IDbContextFactory<ShardDbContext>>().CreateDbContext())
            {
                var script = File.ReadAllText(ShardInitScript).Replace("realms_shard", ShardDbName);
                ExecuteScript(script, context);

                foreach (var shardUpdateScript in Directory.GetFiles(ShardUpdatesDir).OrderBy(_ => _))
                {
                    script = File.ReadAllText(shardUpdateScript).Replace("realms_shard", ShardDbName);
                    ExecuteScript(script, context);
                }
            }
        }

        static string FileMD5Sum(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = new BufferedStream(System.IO.File.OpenRead(filePath), 1200000))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        static bool IsFileValid(string filePath, string knownMd5, bool raiseOnMd5Mismatch = true)
        {
            // Return false and download will
            if (!System.IO.File.Exists(filePath))
                return false;

            // We want to make sure that the test data is deterministic. Ensure that the scripts aren't modified.
            if (raiseOnMd5Mismatch && FileMD5Sum(filePath) != knownMd5)
                throw new InvalidDataException($"The contents of {filePath} does not match the known MD5.");

            return true;
        }

        static void EnsureWorldDbDownloaded()
        {
            if (!IsFileValid(WorldDbLocalScript, WorldDbScriptMD5))
            {
                Console.Write($"Downloading world DB to {WorldDbLocalScript}.... ");
                using var client = new WebClient();
                try
                {
                    if (!Directory.Exists(WorldDbFolder))
                        Directory.CreateDirectory(WorldDbFolder);
                    if (File.Exists(WorldDbLocalZip))
                        File.Delete(WorldDbLocalZip);
                    var dlTask = client.DownloadFile(WorldDbDownloadUrl, WorldDbLocalZip);
                    dlTask.Wait();
                }
                catch
                {
                    Console.Write($"Download for {WorldDbDownloadUrl} failed!");
                    throw new Exception("Failed to fetch world DB");
                }
                Console.WriteLine("Download complete!");

                if (File.Exists(WorldDbLocalScript))
                    File.Delete(WorldDbLocalScript);
                System.IO.Compression.ZipFile.ExtractToDirectory(WorldDbLocalZip, WorldDbFolder, true);
                if (!IsFileValid(WorldDbLocalScript, WorldDbScriptMD5, false))
                {
                    if (File.Exists(WorldDbLocalScript))
                        throw new NotImplementedException("The downloaded World DB has changed from the known World DB. Please report this to the ACRealms devs.");
                    else
                        throw new FileNotFoundException("The downloaded World DB was unable to be located on disk.");
                }
            }
        }

        private static void ExecuteScript(string text, DbContext context)
        {
            var connection = context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = text;
            command.ExecuteNonQuery();
        }

        static void CleanupDbState()
        {
            using (var context = new RootDbContext())
            {
                var connection = context.Database.GetDbConnection();
                connection.Open();
                var command = connection.CreateCommand();
                if (RESET_DB_EACH_RUN) // Can be temporarily false when trying to make specific tests pass, but some tests need this to be true
                {
                    command.CommandText = @$"
                    DROP DATABASE IF EXISTS `{AuthDbName}`;
                    DROP DATABASE IF EXISTS `{WorldDbName}`;
                    DROP DATABASE IF EXISTS `{ShardDbName}`;
                    DROP USER IF EXISTS {TestUserFull};";
                }
                else
                {
#pragma warning disable CS0162 // Unreachable code detected
                    command.CommandText = $"DROP USER IF EXISTS {TestUserFull};";
#pragma warning restore CS0162 // Unreachable code detected
                }
                
                command.ExecuteNonQuery();
            }
        }
        public void Dispose()
        {
        }

        private static string RandomPassword()
        {
            string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            var len = 32;
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (len-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }
    }
}
