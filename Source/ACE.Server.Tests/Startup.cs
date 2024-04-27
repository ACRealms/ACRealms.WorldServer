using ACE.Common;
using ACE.Database;
using ACE.Database.Models.Auth;
using ACE.Database.Models.Shard;
using ACE.Database.Models.World;
using ACE.DatLoader;
using ACE.Server;
using ACE.Server.Managers;
using ACRealms.Tests.Fixtures;
using ACRealms.Tests.Fixtures.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.DependencyInjection;

namespace ACRealms.Tests
{
    public class Startup
    {
        public void ConfigureHostApplicationBuilder(IHostApplicationBuilder hostApplicationBuilder)
        {
            ConfigManager.Initialize();
            hostApplicationBuilder.Services.Configure<HostOptions>(options =>
            {
                options.ServicesStartConcurrently = false;
                options.ServicesStopConcurrently = false;
            });
            TestDatabaseService.Initialize(hostApplicationBuilder.Services);

            // If we don't use a scope, the DbContextFactory will be unusable as soon as tests finish,
            // immediately before ACRealmsTestService begins disposing, and will cause a crash
            hostApplicationBuilder.Services.AddKeyedScoped<Services.IACRealmsService, ACRealmsTestService>("ACRealmsFull");
        }

        public void Configure(IServiceProvider provider)
        {
            provider.GetRequiredKeyedService<Services.IACRealmsService>("ACRealmsFull");
        }
    }

    public class ACRealmsTestService : Services.IACRealmsService, IDisposable
    {
        public ACRealmsTestService(IServiceProvider provider)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            TestDatabaseService.BuildDBs(provider);
            DatabaseManager.Initialize(provider);
            DatabaseManager.Start();
            DatManager.Initialize(ConfigManager.Config.Server.DatFilesDirectory, true);
            ACE.Server.Physics.Common.LandDefs.LandHeightTable = DatManager.PortalDat.RegionDesc.LandDefs.LandHeightTable;
            PropertyManager.Initialize();
            GuidManager.Initialize();
            PlayerManager.Initialize();
            HouseManager.Initialize();
            RealmManager.Initialize(false);
            WorldManager.Initialize();
        }

        public void Dispose()
        {
            PropertyManager.StopUpdating();
            DatabaseManager.Stop();
            WorldManager.StopWorld();
            while (WorldManager.WorldActive)
            {
                Thread.Sleep(10);
            }
            while (DatabaseManager.Shard.QueueCount > 0)
            {
                Thread.Sleep(10);
            }
        }
    }
}
