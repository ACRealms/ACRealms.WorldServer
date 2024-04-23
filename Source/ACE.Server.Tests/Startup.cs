using ACE.Common;
using ACE.Database;
using ACE.Database.Models.Auth;
using ACRealms.Tests.Fixtures;
using ACRealms.Tests.Fixtures.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.DependencyInjection;

namespace ACRealms.Tests
{
    public class Startup
    {
        public IHostBuilder CreateHostBuilder() => new HostBuilder();

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigManager.Initialize();
            TestDatabaseService.Initialize(services);
        }

        public void Configure(IServiceProvider provider)
        {
            TestDatabaseService.BuildDBs(provider);
            DatabaseManager.Initialize(provider);
        }
    }
}
