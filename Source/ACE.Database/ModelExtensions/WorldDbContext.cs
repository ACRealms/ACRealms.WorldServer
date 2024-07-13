global using Version = ACE.Database.Models.World.Version;
using ACE.Database.Models.Shard;
using ACE.Database.Models.World;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Database.Models.World
{
    public class WorldDesignContextFactory : IDesignTimeDbContextFactory<WorldDbContext>
    {
        public WorldDbContext CreateDbContext(string[] args)
        {
            if (Common.ConfigManager.Config == null)
                Common.ConfigManager.Initialize("../ACE.Server/bin/x64/Debug/net8.0/Config.js");

            var config = Common.ConfigManager.Config.MySql.World;
            var connString = $"server={config.Host};port={config.Port};user={config.Username};password={config.Password};database={config.Database};{config.ConnectionOptions}";

            var optionsBuilder = new DbContextOptionsBuilder<WorldDbContext>();
            optionsBuilder.UseMySql(connString, ServerVersion.AutoDetect(connString), builder =>
            {
                builder
                    .EnableRetryOnFailure(10)
                    .DefaultDataTypeMappings(m => m.WithClrBoolean(MySqlBooleanType.Bit1));
            });

            return new WorldDbContext(optionsBuilder.Options);
        }
    }

    public partial class WorldDbContext
    {
        public WorldDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = Common.ConfigManager.Config.MySql.World;

                var connectionString = $"server={config.Host};port={config.Port};user={config.Username};password={config.Password};database={config.Database};{config.ConnectionOptions}";

                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), builder =>
                {
                    builder
                        .EnableRetryOnFailure(10)
                        .DefaultDataTypeMappings(m => m.WithClrBoolean(MySqlBooleanType.Bit1));
                })
                .UseModel(CompiledModels.World.WorldDbContextModel.Instance);
            }
        }
    }
}
