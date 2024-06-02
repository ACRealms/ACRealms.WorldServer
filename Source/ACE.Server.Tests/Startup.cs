using ACE.Common;
using ACE.Database;
using ACE.Database.Models.Auth;
using ACE.Database.Models.Shard;
using ACE.Database.Models.World;
using ACE.DatLoader;
using ACE.Server;
using ACE.Server.Managers;
using ACE.Server.Network.Managers;
using ACRealms.Tests.Fixtures;
using ACRealms.Tests.Fixtures.Database;
using ACRealms.Tests.Fixtures.Network;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.DependencyInjection;
using static ACE.Server.Services;
using Autofac.Extensions.DependencyInjection;
using Autofac.Core;
using ACRealms.Tests.Helpers;
using ACE.Common.ACRealms;

namespace ACRealms.Tests
{
    public class Startup
    {
        public void ConfigureHostApplicationBuilder(IHostApplicationBuilder hostApplicationBuilder)
        {
            ConfigManager.Initialize();
            ACRealmsConfigManager.Initialize();
            TestDatabaseService.Initialize(hostApplicationBuilder.Services);
        }

        public void Configure(IServiceProvider provider)
        {
            WorldFixture.Service = new ACRealmsTestService(provider);            
        }
    }

    [CollectionDefinition(nameof(WorldCollection))]
    public class WorldCollection : ICollectionFixture<WorldFixture> { }

    public class WorldFixture : IDisposable
    {
        public static ACRealmsTestService Service { get; set; }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            Service.Dispose();
            IsDisposed = true;
        }
    }

    public class ACRealmsTestService : IACRealmsService, IDisposable
    {
        public ACRealmsTestService(IServiceProvider provider)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            TestDatabaseService.BuildDBs(provider);
            DatabaseManager.Initialize(provider);
            DatabaseManager.Start();
            DatManager.Initialize(ConfigManager.Config.Server.DatFilesDirectory, true);
            ACE.Server.Physics.Common.LandDefs.LandHeightTable = DatManager.PortalDat.RegionDesc.LandDefs.LandHeightTable;
            
            InitializePropertyManager();
            GuidManager.Initialize();
            PlayerManager.Initialize();
            HouseManager.Initialize();
            RealmManager.Initialize(false);
            NetworkManager.Initialize(new FakeNetworkManager());
            WorldManager.Initialize();
            RealmFixtures.LoadRealmFixture(RealmFixtures.FixtureName.simple);
        }

        private void InitializePropertyManager()
        {
            PropertyManager.Initialize();
            PropertyManager.ModifyBool("acr_enable_ruleset_seeds", true);
        }

        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            PropertyManager.StopUpdating();

            WorldManager.StopWorld();
            while (WorldManager.WorldActive)
                Thread.Sleep(10);

            DatabaseManager.Stop();
            while (DatabaseManager.Shard.QueueCount > 0)
                Thread.Sleep(10);
            IsDisposed = true;
        }
    }
}
