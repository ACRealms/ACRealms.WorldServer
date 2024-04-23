using ACE.Common;
using ACE.Database;
using ACE.Database.Models.Auth;
using ACE.Database.Models.Shard;
using ACE.Database.Models.World;
using ACE.Server;
using ACRealms.Tests.Fixtures;
using ACRealms.Tests.Fixtures.Database;
using ACRealms.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ACRealms.Tests.Fixtures.Database
{
    // http://xunitpatterns.com/Table%20Truncation%20Teardown.html
    // https://umplify.github.io/xunit-dependency-injection/
    // https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/
    public class TestDatabaseService : IDisposable
    {
        public static string TestUserName { get; } = "acrealms_test_db_user";
        private static string TestUserFull { get; } = $"'{TestUserName}'@'localhost'";
        public static string TestUserPassword { get; } = RandomPassword();
        public static string AuthDbName { get; } = "acrealms_test_auth";
        public static string WorldDbName { get; } = "acrealms_test_world";
        public static string ShardDbName { get; } = "acrealms_test_shard";
        public static string DatabaseScriptRoot { get; } = $"{Helpers.Paths.SolutionPath}/../Database";
        public static string AuthInitScript { get; } = $"{DatabaseScriptRoot}/Base/AuthenticationBase.sql";
        public static string WorldInitScript { get; } = $"{DatabaseScriptRoot}/World/WorldBase.sql";
        public static string ShardInitScript { get; } = $"{DatabaseScriptRoot}/Shard/ShardBase.sql";

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
                    CREATE DATABASE `{AuthDbName}`;
                    CREATE DATABASE `{WorldDbName}`;
                    CREATE DATABASE `{ShardDbName}`;
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
            });
            services.AddDbContextFactory<WorldDbContext>(options =>
            {
                var connectionString = $"{connectionStringBase};database={WorldDbName}";
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
            services.AddDbContextFactory<ShardDbContext>(options =>
            {
                var connectionString = $"{connectionStringBase};database={ShardDbName}";
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
        }

        public static void BuildDBs(IServiceProvider provider)
        {
            using (var context = provider.GetRequiredService<IDbContextFactory<AuthDbContext>>().CreateDbContext())
            {
                var script = File.ReadAllText(AuthInitScript).Replace("realms_auth", AuthDbName);
                ExecuteScript(script, context);
            }
            using (var context = provider.GetRequiredService<IDbContextFactory<AuthDbContext>>().CreateDbContext())
            {
                var script = File.ReadAllText(WorldInitScript).Replace("realms_world", WorldDbName);
                ExecuteScript(script, context);
            }
            using (var context = provider.GetRequiredService<IDbContextFactory<AuthDbContext>>().CreateDbContext())
            {
                var script = File.ReadAllText(ShardInitScript).Replace("realms_shard", ShardDbName);
                ExecuteScript(script, context);
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
                command.CommandText = @$"
                    DROP DATABASE IF EXISTS `{AuthDbName}`;
                    DROP DATABASE IF EXISTS `{WorldDbName}`;
                    DROP DATABASE IF EXISTS `{ShardDbName}`;
                    DROP USER IF EXISTS {TestUserFull};";
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
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
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

    //[CollectionDefinition("Database")]
    //public class DatabaseCollection : ICollectionFixture<TestDatabaseService>
    //{
    //}
}
