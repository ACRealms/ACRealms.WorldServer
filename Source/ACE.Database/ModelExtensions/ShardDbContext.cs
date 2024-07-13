using ACE.Database.Models.Shard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Database.Models.Shard
{
    public class ShardDesignContextFactory : IDesignTimeDbContextFactory<ShardDbContext>
    {
        public ShardDbContext CreateDbContext(string[] args)
        {
            if (Common.ConfigManager.Config == null)
                Common.ConfigManager.Initialize("../ACE.Server/bin/x64/Debug/net8.0/Config.js");

            var config = Common.ConfigManager.Config.MySql.Shard;
            var connString = $"server={config.Host};port={config.Port};user={config.Username};password={config.Password};database={config.Database};{config.ConnectionOptions}";

            var optionsBuilder = new DbContextOptionsBuilder<ShardDbContext>();
            optionsBuilder.UseMySql(connString, ServerVersion.AutoDetect(connString), builder =>
            {
                builder
                    .EnableRetryOnFailure(10)
                    .DefaultDataTypeMappings(m => m.WithClrBoolean(MySqlBooleanType.Bit1));
            });

            return new ShardDbContext(optionsBuilder.Options);
        }
    }

    public partial class ShardDbContext
    {
        public ShardDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = Common.ConfigManager.Config.MySql.Shard;

                var connectionString = $"server={config.Host};port={config.Port};user={config.Username};password={config.Password};database={config.Database};{config.ConnectionOptions}";

                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), builder =>
                {
                    builder
                        .EnableRetryOnFailure(10)
                        .DefaultDataTypeMappings(m => m.WithClrBoolean(MySqlBooleanType.Bit1));
                })
                .UseModel(CompiledModels.Shard.ShardDbContextModel.Instance);
            }
        }
    }
}
